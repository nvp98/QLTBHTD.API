using Microsoft.EntityFrameworkCore;
using NCalc;
using PM_QLTBHTD.Application.DTOs;
using PM_QLTBHTD.Application.Exceptions;
using PM_QLTBHTD.Application.Interfaces;
using PM_QLTBHTD.Domain.Entities;
using PM_QLTBHTD.Domain.IRepository;
using System.Text.Json;

namespace PM_QLTBHTD.Application.Services
{
    public class ScoringEngine : IScoringEngine
    {
        private const int MaxDepth = 20;

        private readonly IAppDbContext _db;
        private readonly IKetQuaNhomRepository _ketQuaRepo;
        private readonly IPhieuKiemTraRepository _phieuRepo;

        public ScoringEngine(IAppDbContext db, IKetQuaNhomRepository ketQuaRepo, IPhieuKiemTraRepository phieuRepo)
        {
            _db = db;
            _ketQuaRepo = ketQuaRepo;
            _phieuRepo = phieuRepo;
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

        public async Task<decimal?> TinhVaLuuTongDiemAsync(int idPhieu, CancellationToken ct = default)
        {
            var phieu = await _phieuRepo.GetByIdAsync(idPhieu);
            if (phieu == null) return null;

            int? idNhomGoc = phieu.ID_NhomChiTieu;

            if (idNhomGoc == null)
            {
                var thietBi = await _db.ThietBis.FirstOrDefaultAsync(t => t.ID_ThietBi == phieu.ID_ThietBi, ct);
                if (thietBi == null) return null;

                var goc = await _db.NhomChiTieus
                    .Where(n => n.ID_LoaiThietBi == thietBi.ID_LoaiTB && n.ID_NhomCha == null && n.TrangThai == 1)
                    .Select(n => n.ID_NhomChiTieu)
                    .ToListAsync(ct);

                if (goc.Count == 0) return null; // cây chưa cấu hình xong — chưa tính được, không phải lỗi
                if (goc.Count > 1) throw new NhieuNhomGocException(thietBi.ID_LoaiTB, goc.Count);

                idNhomGoc = goc[0];
            }

            decimal diem;
            try
            {
                diem = await TinhDiemNhomAsync(idNhomGoc.Value, idPhieu, ct);
            }
            catch (Exception)
            {
                // Thiếu công thức/dữ liệu ở đâu đó trong cây (cây đang xây dở) — coi là "chưa tính được".
                return null;
            }

            phieu.TongDiem_Soqt = diem;
            phieu.CapDoCanhBao = XepHangCapDo(diem);
            _phieuRepo.Update(phieu);
            await _phieuRepo.SaveChangesAsync();

            return diem;
        }

        private static string XepHangCapDo(decimal diem)
        {
            if (diem >= 8) return "Tốt";
            if (diem >= 6) return "Bình thường";
            if (diem >= 4) return "Chú ý";
            if (diem >= 2) return "Cảnh báo";
            return "Nguy hiểm";
        }

        private async Task<decimal> TinhDiemDeQuyAsync(
            int idNhomChiTieu, int idPhieu,
            HashSet<int> duongDi, int depth, CancellationToken ct)
        {
            if (depth > MaxDepth)
                throw new VongLapNhomChiTieuException(idNhomChiTieu, duongDi);

            if (duongDi.Contains(idNhomChiTieu))
                throw new VongLapNhomChiTieuException(idNhomChiTieu, duongDi);

            // KHÔNG đọc cache trước — luôn tính lại từ dữ liệu mới nhất (kể cả chỉ tiêu
            // lấy fallback từ phiếu trước) rồi ghi đè cache; cache chỉ dùng để lưu snapshot
            // audit, không dùng để rút ngắn tính toán giữa các lần gọi khác nhau.
            var nhom = await _db.NhomChiTieus.FirstOrDefaultAsync(x => x.ID_NhomChiTieu == idNhomChiTieu, ct)
                ?? throw new InvalidOperationException($"Không tìm thấy NhomChiTieu ID={idNhomChiTieu}.");

            decimal diem;
            string bienDaBind;

            if (nhom.LoaiNhom == "LEAF")
            {
                // Điểm LEAF = trung bình CÓ TRỌNG SỐ (CBM_ChiTieu.TrongSo_Wi) các Si của chỉ tiêu
                // ĐANG HOẠT ĐỘNG trong nhóm. Mỗi chỉ tiêu: ưu tiên giá trị đo trong chính phiếu này;
                // nếu phiếu này không đo chỉ tiêu đó (do tần suất đo khác nhau giữa các chỉ tiêu) thì
                // lấy kết quả GẦN NHẤT trước đó của cùng thiết bị.
                // TrongSo_Wi để trống/0 ở TẤT CẢ chỉ tiêu trong nhóm → coi như trọng số bằng nhau,
                // tương đương trung bình cộng thường (an toàn ngược với cấu hình cũ chưa từng đặt Wi).
                var chiTieuList = await _db.ChiTieus
                    .Where(c => c.ID_NhomChiTieu == idNhomChiTieu && c.TrangThai == 1)
                    .Select(c => new { c.ID_ChiTieu, c.TrongSo_Wi })
                    .ToListAsync(ct);

                var weightedSi = new List<(decimal Si, decimal TrongSo)>();
                foreach (var c in chiTieuList)
                {
                    var si = await LayDiemChiTieuGanNhatAsync(c.ID_ChiTieu, idPhieu, ct);
                    if (si.HasValue) weightedSi.Add((si.Value, c.TrongSo_Wi ?? 1m));
                }

                if (weightedSi.Count == 0)
                {
                    diem = 0m;
                }
                else
                {
                    var tongWi = weightedSi.Sum(x => x.TrongSo);
                    diem = tongWi > 0
                        ? weightedSi.Sum(x => x.Si * x.TrongSo) / tongWi
                        : weightedSi.Average(x => x.Si);
                }
                bienDaBind = JsonSerializer.Serialize(new
                {
                    LeafSiValues = weightedSi.Select(x => new { x.Si, x.TrongSo })
                });
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
                var weightedValues = new List<(double Value, decimal TrongSo)>();

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
                    weightedValues.Add((val, bien.TrongSo ?? 1m));
                }

                duongDi.Remove(idNhomChiTieu);

                if (congThuc.LoaiCongThuc is "WEIGHTED_AVG" or "WEIGHTED_AVG_SCALED")
                {
                    // Tự tính ΣSiWi/ΣWi (WEIGHTED_AVG, thang 0-3 — vd Soqt/Soqtc) hoặc
                    // ΣSiWi/(3·ΣWi)×10 (WEIGHTED_AVG_SCALED, thang 0-10 — vd TS1/TS2) trực tiếp
                    // từ cột CBM_CongThuc_Bien.TrongSo — KHÔNG evaluate BieuThuc, để tránh rủi ro
                    // người dùng gõ tay hệ số vào cả tử và mẫu của biểu thức rồi quên đồng bộ 2 bên.
                    diem = TinhTrungBinhCoTrongSo(weightedValues, scale10: congThuc.LoaiCongThuc == "WEIGHTED_AVG_SCALED");
                }
                else
                {
                    var expr = new Expression(congThuc.BieuThuc);
                    foreach (var kv in bindValues)
                        expr.Parameters[kv.Key] = kv.Value;

                    var rawResult = expr.Evaluate();
                    diem = Convert.ToDecimal(rawResult);
                }

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
            var si = await LayDiemChiTieuGanNhatAsync(idChiTieu, idPhieu, ct);

            if (si == null)
                throw new ThieuDuLieuChiTietKiemTraException(idChiTieu, idPhieu);

            return (double)si.Value;
        }

        /// <summary>
        /// Điểm Sᵢ của 1 chỉ tiêu cho 1 phiếu: ưu tiên giá trị đo TRONG CHÍNH phiếu này;
        /// nếu không có (chỉ tiêu chưa tới kỳ đo lại trong phiếu này), lấy kết quả GẦN NHẤT
        /// đã đo trước đó của CÙNG thiết bị (theo NgayKiemTra, không lấy dữ liệu tương lai
        /// so với ngày của phiếu đang tính — tránh sai lệch khi tính lại phiếu cũ sau khi
        /// đã có phiếu mới hơn). Trả null nếu chỉ tiêu này chưa từng được đo lần nào.
        /// </summary>
        private async Task<decimal?> LayDiemChiTieuGanNhatAsync(int idChiTieu, int idPhieu, CancellationToken ct)
        {
            var siHienTai = await _db.ChiTietKiemTras
                .Where(x => x.IDPhieu == idPhieu && x.ID_ChiTieu == idChiTieu && x.Diem_Si_DatDuoc != null)
                .Select(x => (decimal?)x.Diem_Si_DatDuoc)
                .FirstOrDefaultAsync(ct);
            if (siHienTai != null) return siHienTai;

            var phieuHienTai = await _db.PhieuKiemTras.FirstOrDefaultAsync(p => p.ID_Phieu == idPhieu, ct);
            if (phieuHienTai == null) return null;

            var siGanNhat = await (
                from ct2 in _db.ChiTietKiemTras
                join p2 in _db.PhieuKiemTras on ct2.IDPhieu equals p2.ID_Phieu
                where p2.ID_ThietBi == phieuHienTai.ID_ThietBi
                      && ct2.ID_ChiTieu == idChiTieu
                      && ct2.Diem_Si_DatDuoc != null
                      && p2.NgayKiemTra <= phieuHienTai.NgayKiemTra
                      && p2.ID_Phieu != idPhieu
                orderby p2.NgayKiemTra descending, p2.ID_Phieu descending
                select ct2.Diem_Si_DatDuoc
            ).FirstOrDefaultAsync(ct);

            return siGanNhat;
        }

        /// <summary>
        /// ΣSiWi/ΣWi (scale10=false, thang 0-3 — vd Soqt/Soqtc) hoặc ΣSiWi/(3·ΣWi)×10
        /// (scale10=true, thang 0-10 — vd TS1/TS2). TrongSo=0/thiếu ở TẤT CẢ biến → coi
        /// như trọng số bằng nhau (an toàn ngược, tương đương trung bình cộng thường).
        /// </summary>
        private static decimal TinhTrungBinhCoTrongSo(List<(double Value, decimal TrongSo)> items, bool scale10)
        {
            if (items.Count == 0) return 0m;

            var tongTrongSo = items.Sum(x => x.TrongSo);
            var tuSo = items.Sum(x => (decimal)x.Value * x.TrongSo);

            if (tongTrongSo == 0)
            {
                tongTrongSo = items.Count;
                tuSo = items.Sum(x => (decimal)x.Value);
            }

            var trungBinh = tuSo / tongTrongSo;
            return scale10 ? trungBinh / 3m * 10m : trungBinh;
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
