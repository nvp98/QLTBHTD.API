using Microsoft.EntityFrameworkCore;
using NCalc;
using PM_QLTBHTD.Application.DTOs;
using PM_QLTBHTD.Application.Exceptions;
using PM_QLTBHTD.Application.Interfaces;
using PM_QLTBHTD.Application.Services;
using PM_QLTBHTD.Domain.Entities;
using PM_QLTBHTD.Domain.IRepository;
using System.Text.Json;

namespace PM_QLTBHTD.Infrastructure.Services
{
    public class ScoringEngine : IScoringEngine
    {
        private const int MaxDepth = 20;

        private readonly IAppDbContext _db;
        private readonly IKetQuaNhomRepository _ketQuaRepo;

        public ScoringEngine(IAppDbContext db, IKetQuaNhomRepository ketQuaRepo)
        {
            _db = db;
            _ketQuaRepo = ketQuaRepo;
        }

        public async Task<decimal> TinhDiemNhomAsync(int idNhomChiTieu, int idPhieu, CancellationToken ct = default)
        {
            var duongDi = new HashSet<int>();
            return await TinhDiemDeQuyAsync(idNhomChiTieu, idPhieu, duongDi, 0, ct);
        }

        public async Task<IReadOnlyList<KetQuaNhomDto>> TinhDiemCayAsync(int idLoaiThietBi, int idPhieu, CancellationToken ct = default)
        {
            var tatCaNhom = await _db.NhomChiTieus
                .Where(x => x.ID_LoaiThietBi == idLoaiThietBi && x.TrangThai == 1)
                .ToListAsync(ct);

            var ket = new List<KetQuaNhomDto>();
            foreach (var nhom in tatCaNhom)
            {
                try
                {
                    var diem = await TinhDiemNhomAsync(nhom.ID_NhomChiTieu, idPhieu, ct);
                    var cache = await _ketQuaRepo.GetByPhieuNhomAsync(idPhieu, nhom.ID_NhomChiTieu);
                    ket.Add(new KetQuaNhomDto
                    {
                        ID_NhomChiTieu = nhom.ID_NhomChiTieu,
                        TenNhom = nhom.TenNhom,
                        LoaiNhom = nhom.LoaiNhom,
                        CapDo = nhom.CapDo,
                        Diem = diem,
                        BienDaBind = cache?.BienDaBind,
                        ThoiGianTinh = cache?.ThoiGianTinh ?? DateTime.UtcNow
                    });
                }
                catch (CongThucKhongTonTaiException)
                {
                    // Nhóm COMPOSITE chưa có công thức — bỏ qua, không lỗi toàn bộ cây
                }
            }

            return BuildTree(ket, tatCaNhom);
        }

        private async Task<decimal> TinhDiemDeQuyAsync(
            int idNhomChiTieu, int idPhieu,
            HashSet<int> duongDi, int depth, CancellationToken ct)
        {
            if (depth > MaxDepth)
                throw new VongLapNhomChiTieuException(idNhomChiTieu, duongDi);

            if (duongDi.Contains(idNhomChiTieu))
                throw new VongLapNhomChiTieuException(idNhomChiTieu, duongDi);

            // Đọc cache trước
            var cache = await _ketQuaRepo.GetByPhieuNhomAsync(idPhieu, idNhomChiTieu);
            if (cache != null)
                return cache.Diem;

            var nhom = await _db.NhomChiTieus.FirstOrDefaultAsync(x => x.ID_NhomChiTieu == idNhomChiTieu, ct)
                ?? throw new InvalidOperationException($"Không tìm thấy NhomChiTieu ID={idNhomChiTieu}.");

            decimal diem;
            string bienDaBind;

            if (nhom.LoaiNhom == "LEAF")
            {
                // Điểm LEAF = trung bình các Si của các chỉ tiêu trong nhóm đã nhập
                var siList = await (from ct2 in _db.ChiTietKiemTras
                                    join c in _db.ChiTieus on ct2.ID_ChiTieu equals c.ID_ChiTieu
                                    where ct2.IDPhieu == idPhieu && c.ID_NhomChiTieu == idNhomChiTieu
                                          && ct2.Diem_Si_DatDuoc != null
                                    select ct2.Diem_Si_DatDuoc!.Value).ToListAsync(ct);

                diem = siList.Count > 0 ? siList.Average() : 0m;
                bienDaBind = JsonSerializer.Serialize(new { LeafSiValues = siList });
            }
            else // COMPOSITE
            {
                var congThuc = await _db.CongThucTongHops
                    .FirstOrDefaultAsync(x => x.ID_NhomChiTieu == idNhomChiTieu && x.TrangThai == 1, ct)
                    ?? throw new CongThucKhongTonTaiException(idNhomChiTieu);

                var biens = await _db.CongThucBiens
                    .Where(x => x.ID_CongThuc == congThuc.ID_CongThuc)
                    .ToListAsync(ct);

                duongDi.Add(idNhomChiTieu);
                var bindValues = new Dictionary<string, double>();

                foreach (var bien in biens)
                {
                    double val = bien.NguonBien switch
                    {
                        "HANGSO" => (double)(bien.GiaTriHangSo ?? 0m),
                        "CHITIEU" => await LayDiemChiTieuAsync(bien.ID_ChiTieuNguon!.Value, idPhieu, ct),
                        "NHOM_CON" => (double)await TinhDiemDeQuyAsync(
                                            bien.ID_NhomCon!.Value, idPhieu, duongDi, depth + 1, ct),
                        _ => throw new InvalidOperationException($"NguonBien không hợp lệ: '{bien.NguonBien}'")
                    };
                    bindValues[bien.MaBien] = val;
                }

                duongDi.Remove(idNhomChiTieu);

                var expr = new Expression(congThuc.BieuThuc);
                foreach (var kv in bindValues)
                    expr.Parameters[kv.Key] = kv.Value;

                var rawResult = expr.Evaluate();
                diem = Convert.ToDecimal(rawResult);

                if (congThuc.ThangDiem_Min.HasValue && diem < congThuc.ThangDiem_Min.Value)
                    diem = congThuc.ThangDiem_Min.Value;
                if (congThuc.ThangDiem_Max.HasValue && diem > congThuc.ThangDiem_Max.Value)
                    diem = congThuc.ThangDiem_Max.Value;

                bienDaBind = JsonSerializer.Serialize(bindValues);
            }

            // Ghi cache
            await _ketQuaRepo.UpsertAsync(new CBM_KetQuaNhom
            {
                IDPhieu = idPhieu,
                ID_NhomChiTieu = idNhomChiTieu,
                Diem = diem,
                BienDaBind = bienDaBind,
                ThoiGianTinh = DateTime.UtcNow
            });
            await _ketQuaRepo.SaveChangesAsync();

            return diem;
        }

        private async Task<double> LayDiemChiTieuAsync(int idChiTieu, int idPhieu, CancellationToken ct)
        {
            var si = await _db.ChiTietKiemTras
                .Where(x => x.IDPhieu == idPhieu && x.ID_ChiTieu == idChiTieu)
                .Select(x => (decimal?)x.Diem_Si_DatDuoc)
                .FirstOrDefaultAsync(ct);

            if (si == null)
                throw new ThieuDuLieuChiTietKiemTraException(idChiTieu, idPhieu);

            return (double)si.Value;
        }

        private static IReadOnlyList<KetQuaNhomDto> BuildTree(
            List<KetQuaNhomDto> flat, List<CBM_NhomChiTieu> nhoms)
        {
            var lookup = flat.ToDictionary(x => x.ID_NhomChiTieu);
            var nhomLookup = nhoms.ToDictionary(x => x.ID_NhomChiTieu);
            var roots = new List<KetQuaNhomDto>();

            foreach (var node in flat)
            {
                if (nhomLookup.TryGetValue(node.ID_NhomChiTieu, out var nhom)
                    && nhom.ID_NhomCha.HasValue
                    && lookup.TryGetValue(nhom.ID_NhomCha.Value, out var parent))
                {
                    parent.NhomCon.Add(node);
                }
                else
                {
                    roots.Add(node);
                }
            }

            return roots.AsReadOnly();
        }
    }
}
