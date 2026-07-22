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
        private readonly IFormulaEngine _formulaEngine;

        public ChiTieuScoringService(
            IAppDbContext db,
            IChiTietKiemTraRepository chiTietRepo,
            IChiTietKiemTraInputRepository inputRepo,
            IKetQuaPhanLoaiThangRepository phanLoaiThangRepo,
            IFormulaEngine formulaEngine)
        {
            _db = db;
            _chiTietRepo = chiTietRepo;
            _inputRepo = inputRepo;
            _phanLoaiThangRepo = phanLoaiThangRepo;
            _formulaEngine = formulaEngine;
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

                var chiTieuInputs = await _db.ChiTieuInputs
                    .Where(x => x.ID_ChiTieu == idChiTieu)
                    .ToListAsync(ct);

                if (loaiTinhDiem == "Rule" && chiTieuInputs.Count > 0)
                {
                    diemSi = await TinhDiemRuleAsync(idPhieu, idChiTieu, chiTieuInputs, nhap.DanhSachInput, ct);
                }
                else if (loaiTinhDiem == "LF")
                {
                    var phieuInfo = await _db.PhieuKiemTras
                        .Where(p => p.ID_Phieu == idPhieu)
                        .Select(p => new { p.ID_ThietBi, p.NgayKiemTra })
                        .FirstAsync(ct);

                    var (_, si) = await TinhDiemLFAsync(
                        idPhieu, phieuInfo.ID_ThietBi, phieuInfo.NgayKiemTra, idChiTieu, nhap.GiaTriNhap_So, ct);
                    diemSi = si;
                }
                else
                {
                    if (chiTieuInputs.Count == 0)
                    {
                        diemSi = await TinhDiemDonAsync(idChiTieu, nhap.GiaTriNhap_So, ct);
                    }
                    else
                    {
                        var vars = XayDungVars(idChiTieu, chiTieuInputs, nhap.DanhSachInput);
                        await LuuInputsAsync(idPhieu, idChiTieu, vars);

                        // Chỉ tiêu có cấu hình Formula (CBM_ChiTieu_Formula): Input → Formula (giá trị
                        // trung gian) → Threshold theo MaKetQua → Rule (gộp nhiều Si nếu có nhiều Formula).
                        // Không có Formula nào → giữ nguyên hành vi cũ (Threshold đa biến trực tiếp trên Input).
                        var idThietBiCuaPhieu = await _db.PhieuKiemTras
                            .Where(p => p.ID_Phieu == idPhieu)
                            .Select(p => p.ID_ThietBi)
                            .FirstAsync(ct);

                        var ketQuaFormula = await _formulaEngine.EvaluateAllAsync(
                            idChiTieu, idPhieu, idThietBiCuaPhieu, vars, ct);

                        diemSi = ketQuaFormula.Count > 0
                            ? await TinhDiemTuFormulaAsync(idChiTieu, ketQuaFormula, ct)
                            : await TinhDiemNhieuBienAsync(idChiTieu, vars, ct);
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

        /// <summary>
        /// LoaiTinhDiem='Rule': CBM_ChiTieu_Rule.LoaiRule='BANG_MUC' — nhiều dòng điều kiện boolean,
        /// khớp đầu tiên theo Diem_Si giảm dần thắng. Trước đây chỉ có ở PhieuKiemTraService.CreateAsync;
        /// chuyển vào đây để CreateAsync có thể delegate toàn bộ việc tính Si qua service này.
        /// </summary>
        private async Task<decimal?> TinhDiemRuleAsync(
            int idPhieu, int idChiTieu, List<CBM_ChiTieu_Input> chiTieuInputs,
            Dictionary<string, decimal>? danhSachInput, CancellationToken ct)
        {
            var vars = new Dictionary<string, decimal>();
            foreach (var def in chiTieuInputs)
            {
                if (danhSachInput != null && danhSachInput.TryGetValue(def.MaInput, out var val))
                    vars[def.MaInput] = val;
            }

            if (vars.Count == 0) return null;
            await LuuInputsAsync(idPhieu, idChiTieu, vars);

            var rules = await _db.ChiTieuRules
                .Where(r => r.ID_ChiTieu == idChiTieu && r.LoaiRule == "BANG_MUC")
                .OrderByDescending(r => r.Diem_Si)
                .ToListAsync(ct);

            foreach (var rule in rules)
            {
                if (NguongEvaluator.EvalNCalc(rule.BieuThuc, vars))
                    return rule.Diem_Si;
            }

            return null;
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
        /// Kết luận Si từ các giá trị Formula đã tính (vd DT1, DT2), theo đúng 1 trong 2 nhánh —
        /// KHÔNG trộn cả hai để tránh nhập nhằng biến nào ứng với nguồn nào:
        ///   Có CBM_ChiTieu_Rule (BANG_MUC hoặc CONG_THUC) cho chỉ tiêu này
        ///     → dùng Rule, biến truyền vào Rule là giá trị Formula THÔ, đặt tên đúng MaKetQua
        ///       (vd "DT1", "DT2") — cho phép Rule kết luận Si trực tiếp từ nhiều kết quả Formula
        ///       cùng lúc (vd BANG_MUC "DT1&lt;=3 &amp;&amp; DT2&lt;=10" dựng bằng ExprBuilder AND/OR).
        ///   Không có Rule nào
        ///     → tra Threshold (CBM_Nguong.MaKetQua) cho từng kết quả Formula như trước (chỉ áp dụng
        ///       khi có đúng 1 Formula — nhiều Formula mà không có Rule để gộp là lỗi cấu hình).
        /// </summary>
        private async Task<decimal?> TinhDiemTuFormulaAsync(
            int idChiTieu, Dictionary<string, decimal> ketQuaFormula, CancellationToken ct)
        {
            var congThucRule = await _db.ChiTieuRules
                .Where(r => r.ID_ChiTieu == idChiTieu && r.LoaiRule == "CONG_THUC")
                .FirstOrDefaultAsync(ct);
            if (congThucRule != null)
                return NguongEvaluator.EvalNCalcNumeric(congThucRule.BieuThuc, ketQuaFormula);

            var bangMucRules = await _db.ChiTieuRules
                .Where(r => r.ID_ChiTieu == idChiTieu && r.LoaiRule == "BANG_MUC")
                .OrderByDescending(r => r.Diem_Si)
                .ToListAsync(ct);
            if (bangMucRules.Count > 0)
            {
                foreach (var rule in bangMucRules)
                    if (NguongEvaluator.EvalNCalc(rule.BieuThuc, ketQuaFormula))
                        return rule.Diem_Si;
                return null; // có Rule nhưng không dòng nào khớp — coi như chưa xác định được Si
            }

            // Không có Rule -> tra Ngưỡng theo MaKetQua như cũ.
            var siTheoKetQua = new Dictionary<string, decimal>();
            foreach (var (maKetQua, giaTri) in ketQuaFormula)
            {
                var nguongs = await _db.Nguongs
                    .Where(x => x.ID_ChiTieu == idChiTieu && x.MaKetQua == maKetQua)
                    .OrderBy(x => x.ThuTu)
                    .ToListAsync(ct);

                decimal? si = null;
                foreach (var ng in nguongs)
                {
                    if (NguongEvaluator.KiemTraNguongVoiGiaTri(giaTri, ng))
                    {
                        si = ng.Diem_Si;
                        break;
                    }
                }

                if (si is null)
                    throw new LoiFormulaException(idChiTieu, maKetQua,
                        $"Giá trị {giaTri} không khớp ngưỡng nào (CBM_Nguong.MaKetQua='{maKetQua}').");

                siTheoKetQua[$"Si_{maKetQua}"] = si.Value;
            }

            if (siTheoKetQua.Count == 1)
                return siTheoKetQua.Values.First();

            throw new LoiFormulaException(idChiTieu, string.Join(",", siTheoKetQua.Keys),
                "Chỉ tiêu có nhiều Formula nhưng chưa cấu hình CBM_ChiTieu_Rule (BANG_MUC hoặc " +
                "CONG_THUC) để gộp Si.");
        }

        /// <summary>
        /// Tính điểm cho chỉ tiêu kiểu LoaiTinhDiem='LF' (ví dụ Mang tải MBA).
        /// Mỗi phiếu kiểm tra chỉ đại diện 1 tháng — chỉ tiêu này chỉ nhận 1 giá trị đo (giaTriThangNay),
        /// là TẢI ĐỈNH TUYỆT ĐỐI (VD MVA), không phải tỉ số. Tự chia cho CBM_ThietBi.TaiDinhMuc (SB) để ra Si/SB.
        /// Tự gom tối đa 12 tháng: giá trị tháng này + tối đa 11 phiếu gần nhất trước đó của CÙNG thiết bị
        /// (dựa vào ChiTietKiemTra.GiaTriNhap_So đã lưu ở các phiếu trước), phân mỗi tháng vào 1 mức N0..N4
        /// (CBM_ChiTieu_PhanLoaiNguong), LF = Σ TrongSo các tháng / số tháng hợp lệ, rồi tra CBM_Nguong theo LF
        /// để ra Diem_Si cuối. Không ghi đè GiaTriNhap_So — giá trị hiển thị của phiếu vẫn là số đo tháng này.
        /// </summary>
        public async Task<(decimal? Lf, decimal? DiemSi)> TinhDiemLFAsync(
            int idPhieu, int idThietBi, DateTime ngayKiemTra, int idChiTieu, decimal? giaTriThangNay,
            CancellationToken ct = default)
        {
            var taiDinhMuc = await _db.ThietBis
                .Where(t => t.ID_ThietBi == idThietBi)
                .Select(t => t.TaiDinhMuc)
                .FirstOrDefaultAsync(ct);

            if (taiDinhMuc is null or 0)
                throw new ThieuTaiDinhMucException(idThietBi);

            var mucPhanLoai = await _db.PhanLoaiNguongs
                .Where(x => x.ID_ChiTieu == idChiTieu)
                .OrderBy(x => x.ThuTu)
                .ToListAsync(ct);

            if (mucPhanLoai.Count == 0)
                throw new ThieuPhanLoaiNguongException(idChiTieu);

            // Gom điểm của 12 tháng LIÊN TỤC LIỀN KỀ với thời điểm tính toán (tháng này + 11 tháng
            // liền trước theo lịch — KHÔNG lùi xa hơn cửa sổ 12 tháng để "gom cho đủ" khi có tháng
            // bị thiếu đo; tháng thiếu đo đơn giản là không có điểm, không lấy dữ liệu tháng xa hơn thay thế).
            // Mỗi tháng chỉ lấy 1 giá trị: tháng này ưu tiên lấy từ chính phiếu đang tính; nếu 1 tháng
            // có nhiều phiếu thì lấy phiếu mới nhất trong tháng đó (đã orderby desc).
            // GiaTriNhap_So lưu ở ChiTietKiemTra luôn là tải đỉnh tuyệt đối, chia cho SB tại đây.
            var thangBatDau = new DateTime(ngayKiemTra.Year, ngayKiemTra.Month, 1).AddMonths(-11);

            var diemThang = new List<(int Nam, int Thang, decimal GiaTriDo)>();
            if (giaTriThangNay is not null)
                diemThang.Add((ngayKiemTra.Year, ngayKiemTra.Month, giaTriThangNay.Value / taiDinhMuc.Value));

            var lichSu = await (
                from ctCu in _db.ChiTietKiemTras
                join p in _db.PhieuKiemTras on ctCu.IDPhieu equals p.ID_Phieu
                where p.ID_ThietBi == idThietBi
                      && ctCu.ID_ChiTieu == idChiTieu
                      && p.ID_Phieu != idPhieu
                      && p.NgayKiemTra >= thangBatDau
                      && p.NgayKiemTra <= ngayKiemTra
                      && ctCu.GiaTriNhap_So != null
                orderby p.NgayKiemTra descending, p.ID_Phieu descending
                select new { p.NgayKiemTra, ctCu.GiaTriNhap_So }
            ).ToListAsync(ct);

            var thangDaCo = new HashSet<(int Nam, int Thang)>(diemThang.Select(x => (x.Nam, x.Thang)));
            foreach (var row in lichSu)
            {
                var key = (Nam: row.NgayKiemTra.Year, Thang: row.NgayKiemTra.Month);
                if (!thangDaCo.Add(key)) continue; // tháng này đã có giá trị rồi (giữ bản ghi mới nhất)
                diemThang.Add((key.Nam, key.Thang, row.GiaTriNhap_So!.Value / taiDinhMuc.Value));
            }

            // Xóa snapshot phân loại cũ của phiếu này, ghi lại theo đúng cửa sổ 12 tháng vừa gom
            var cuData = await _phanLoaiThangRepo.FindAsync(x => x.IDPhieu == idPhieu && x.ID_ChiTieu == idChiTieu);
            foreach (var row in cuData)
                _phanLoaiThangRepo.Delete(row);

            decimal tongTrongSo = 0;
            int soThangHopLe = 0;

            foreach (var (nam, thang, giaTri) in diemThang)
            {
                var muc = mucPhanLoai.FirstOrDefault(m => KhopKhoangPhanLoai(giaTri, m));
                if (muc == null) continue; // giá trị ngoài mọi khoảng đã định nghĩa — bỏ qua tháng này

                await _phanLoaiThangRepo.AddAsync(new CBM_KetQuaPhanLoaiThang
                {
                    IDPhieu = idPhieu,
                    ID_ChiTieu = idChiTieu,
                    Nam = nam,
                    Thang = thang,
                    GiaTriDo = giaTri,
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
