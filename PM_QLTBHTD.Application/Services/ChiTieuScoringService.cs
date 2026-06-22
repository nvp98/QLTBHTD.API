using Microsoft.EntityFrameworkCore;
using PM_QLTBHTD.Application.DTOs;
using PM_QLTBHTD.Application.Exceptions;
using PM_QLTBHTD.Application.Helpers;
using PM_QLTBHTD.Application.Interfaces;
using PM_QLTBHTD.Domain.Entities;
using PM_QLTBHTD.Domain.IRepository;

namespace PM_QLTBHTD.Application.Services
{
    /// <summary>
    /// Tính Diem_Si_DatDuoc cho từng chỉ tiêu lá và ghi vào ChiTietKiemTra.
    /// Phải chạy TRƯỚC khi ScoringEngine.TinhDiemNhomAsync được gọi.
    /// </summary>
    public class ChiTieuScoringService : IChiTieuScoringService
    {
        private readonly IAppDbContext _db;
        private readonly IChiTietKiemTraRepository _chiTietRepo;
        private readonly IChiTietKiemTraInputRepository _inputRepo;

        public ChiTieuScoringService(
            IAppDbContext db,
            IChiTietKiemTraRepository chiTietRepo,
            IChiTietKiemTraInputRepository inputRepo)
        {
            _db = db;
            _chiTietRepo = chiTietRepo;
            _inputRepo = inputRepo;
        }

        public async Task TinhVaLuuDiemSiAsync(
            int idPhieu,
            IEnumerable<NhapChiTietKiemTraDto> danhSachNhap,
            CancellationToken ct = default)
        {
            foreach (var nhap in danhSachNhap)
            {
                var idChiTieu = nhap.ID_ChiTieu;

                var chiTieuInputs = await _db.ChiTieuInputs
                    .Where(x => x.ID_ChiTieu == idChiTieu)
                    .ToListAsync(ct);

                var chiTiet = await _db.ChiTietKiemTras
                    .FirstOrDefaultAsync(x => x.IDPhieu == idPhieu && x.ID_ChiTieu == idChiTieu, ct);

                bool isNew = chiTiet == null;
                chiTiet ??= new ChiTietKiemTra
                {
                    IDPhieu = idPhieu,
                    ID_ChiTieu = idChiTieu
                };

                chiTiet.GiaTriNhap_So = nhap.GiaTriNhap_So;
                chiTiet.GiaTriNhap_Chu = nhap.GiaTriNhap_Chu;
                chiTiet.GhiChu = nhap.GhiChu;

                decimal? diemSi;

                if (chiTieuInputs.Count == 0)
                {
                    diemSi = await TinhDiemDonAsync(idChiTieu, nhap.GiaTriNhap_So, ct);
                }
                else
                {
                    var vars = XayDungVars(idChiTieu, chiTieuInputs, nhap.DanhSachInput);
                    await LuuInputsAsync(idPhieu, idChiTieu, vars);
                    diemSi = await TinhDiemNhieuBienAsync(idChiTieu, vars, ct);
                }

                chiTiet.Diem_Si_DatDuoc = diemSi;

                if (isNew)
                    await _chiTietRepo.AddAsync(chiTiet);
                else
                    _chiTietRepo.Update(chiTiet);
            }

            await _chiTietRepo.SaveChangesAsync();
        }

        private async Task<decimal?> TinhDiemDonAsync(int idChiTieu, decimal? giaTri, CancellationToken ct)
        {
            var nguongs = await _db.Nguongs
                .Where(x => x.ID_ChiTieu == idChiTieu)
                .OrderBy(x => x.ThuTu)
                .ToListAsync(ct);

            foreach (var ng in nguongs)
            {
                if (NguongEvaluator.KiemTraNguongVoiGiaTri(giaTri, ng))
                    return ng.Diem_Si;
            }

            return null;
        }

        private async Task<decimal?> TinhDiemNhieuBienAsync(
            int idChiTieu,
            Dictionary<string, decimal> vars,
            CancellationToken ct)
        {
            var nguongs = await _db.Nguongs
                .Where(x => x.ID_ChiTieu == idChiTieu)
                .OrderBy(x => x.ThuTu)
                .ToListAsync(ct);

            if (!nguongs.Any(x => !string.IsNullOrWhiteSpace(x.BieuThuc_Logic)))
                throw new ThieuBieuThucNguongException(idChiTieu);

            foreach (var ng in nguongs)
            {
                if (NguongEvaluator.KiemTraNguongVoiGiaTri(vars, ng))
                    return ng.Diem_Si;
            }

            return null;
        }

        private static Dictionary<string, decimal> XayDungVars(
            int idChiTieu,
            List<CBM_ChiTieu_Input> chiTieuInputs,
            Dictionary<string, decimal>? danhSachInput)
        {
            var vars = new Dictionary<string, decimal>();

            foreach (var inp in chiTieuInputs)
            {
                if (danhSachInput == null || !danhSachInput.TryGetValue(inp.MaInput, out var val))
                    throw new ThieuInputChiTieuException(idChiTieu, inp.MaInput);

                vars[inp.MaInput] = val;
            }

            return vars;
        }

        private async Task LuuInputsAsync(int idPhieu, int idChiTieu, Dictionary<string, decimal> vars)
        {
            // Xóa cũ, thêm mới
            var existing = await _inputRepo.FindAsync(x => x.IDPhieu == idPhieu && x.ID_ChiTieu == idChiTieu);
            foreach (var e in existing)
                _inputRepo.Delete(e);

            foreach (var kv in vars)
            {
                await _inputRepo.AddAsync(new ChiTietKiemTra_Input
                {
                    IDPhieu = idPhieu,
                    ID_ChiTieu = idChiTieu,
                    MaInput = kv.Key,
                    GiaTriSo = kv.Value
                });
            }

            await _inputRepo.SaveChangesAsync();
        }
    }
}
