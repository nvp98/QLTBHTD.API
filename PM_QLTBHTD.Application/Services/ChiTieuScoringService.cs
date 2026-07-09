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
        private readonly IKetQuaPhanLoaiThangRepository _phanLoaiThangRepo;

        public ChiTieuScoringService(
            IAppDbContext db,
            IChiTietKiemTraRepository chiTietRepo,
            IChiTietKiemTraInputRepository inputRepo,
            IKetQuaPhanLoaiThangRepository phanLoaiThangRepo)
        {
            _db = db;
            _chiTietRepo = chiTietRepo;
            _inputRepo = inputRepo;
            _phanLoaiThangRepo = phanLoaiThangRepo;
        }

        public async Task TinhVaLuuDiemSiAsync(
            int idPhieu,
            IEnumerable<NhapChiTietKiemTraDto> danhSachNhap,
            CancellationToken ct = default)
        {
            foreach (var nhap in danhSachNhap)
            {
                var idChiTieu = nhap.ID_ChiTieu;

                var loaiTinhDiem = await _db.ChiTieus
                    .Where(c => c.ID_ChiTieu == idChiTieu)
                    .Select(c => c.LoaiTinhDiem)
                    .FirstOrDefaultAsync(ct);

                var chiTiet = await _db.ChiTietKiemTras
                    .FirstOrDefaultAsync(x => x.IDPhieu == idPhieu && x.ID_ChiTieu == idChiTieu, ct);

                bool isNew = chiTiet == null;
                chiTiet ??= new ChiTietKiemTra
                {
                    IDPhieu = idPhieu,
                    ID_ChiTieu = idChiTieu
                };

                decimal? giaTriHienThi = nhap.GiaTriNhap_So;
                decimal? diemSi;

                if (loaiTinhDiem == "LF")
                {
                    var (lf, si) = await TinhDiemLFAsync(idPhieu, idChiTieu, nhap.DanhSachThang, ct);
                    giaTriHienThi = lf;
                    diemSi = si;
                }
                else
                {
                    var chiTieuInputs = await _db.ChiTieuInputs
                        .Where(x => x.ID_ChiTieu == idChiTieu)
                        .ToListAsync(ct);

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
                }

                chiTiet.GiaTriNhap_So = giaTriHienThi;
                chiTiet.GiaTriNhap_Chu = nhap.GiaTriNhap_Chu;
                chiTiet.GhiChu = nhap.GhiChu;
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

        /// <summary>
        /// Tính điểm cho chỉ tiêu kiểu LoaiTinhDiem='LF' (ví dụ Mang tải MBA):
        /// mỗi tháng đo được phân vào 1 mức N0..N4 (CBM_ChiTieu_PhanLoaiNguong),
        /// LF = Σ TrongSo các tháng / số tháng hợp lệ, sau đó tra CBM_Nguong theo LF để ra Diem_Si cuối.
        /// </summary>
        private async Task<(decimal? Lf, decimal? DiemSi)> TinhDiemLFAsync(
            int idPhieu, int idChiTieu, List<ThangDoDto>? danhSachThang, CancellationToken ct)
        {
            if (danhSachThang == null || danhSachThang.Count == 0)
                throw new ThieuDuLieuThangDoException(idChiTieu, idPhieu);

            var mucPhanLoai = await _db.PhanLoaiNguongs
                .Where(x => x.ID_ChiTieu == idChiTieu)
                .OrderBy(x => x.ThuTu)
                .ToListAsync(ct);

            if (mucPhanLoai.Count == 0)
                throw new ThieuPhanLoaiNguongException(idChiTieu);

            var cuData = await _phanLoaiThangRepo.FindAsync(x => x.IDPhieu == idPhieu && x.ID_ChiTieu == idChiTieu);
            foreach (var row in cuData)
                _phanLoaiThangRepo.Delete(row);

            decimal tongTrongSo = 0;
            int soThangHopLe = 0;

            foreach (var thang in danhSachThang)
            {
                var muc = mucPhanLoai.FirstOrDefault(m => KhopKhoangPhanLoai(thang.GiaTriDo, m));
                if (muc == null) continue; // giá trị ngoài mọi khoảng đã định nghĩa — bỏ qua tháng này

                await _phanLoaiThangRepo.AddAsync(new CBM_KetQuaPhanLoaiThang
                {
                    IDPhieu = idPhieu,
                    ID_ChiTieu = idChiTieu,
                    Nam = thang.Nam,
                    Thang = thang.Thang,
                    GiaTriDo = thang.GiaTriDo,
                    MaMuc = muc.MaMuc,
                    TrongSo = muc.TrongSo
                });

                tongTrongSo += muc.TrongSo;
                soThangHopLe++;
            }

            await _phanLoaiThangRepo.SaveChangesAsync();

            if (soThangHopLe == 0) return (null, null);

            var lf = tongTrongSo / soThangHopLe;

            var nguongs = await _db.Nguongs
                .Where(x => x.ID_ChiTieu == idChiTieu)
                .OrderBy(x => x.ThuTu)
                .ToListAsync(ct);

            decimal? diemSi = null;
            foreach (var ng in nguongs)
            {
                if (NguongEvaluator.KiemTraNguongVoiGiaTri(lf, ng))
                {
                    diemSi = ng.Diem_Si;
                    break;
                }
            }

            return (lf, diemSi);
        }

        private static bool KhopKhoangPhanLoai(decimal giaTri, CBM_ChiTieu_PhanLoaiNguong m)
        {
            if (m.GiaTriTu is not null)
            {
                bool duoi = m.GiaTriTu_BaoGom ? giaTri >= m.GiaTriTu.Value : giaTri > m.GiaTriTu.Value;
                if (!duoi) return false;
            }
            if (m.GiaTriDen is not null)
            {
                bool tren = m.GiaTriDen_BaoGom ? giaTri <= m.GiaTriDen.Value : giaTri < m.GiaTriDen.Value;
                if (!tren) return false;
            }
            return true;
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
