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
        private readonly IKetQuaTrungGianRepository _trungGianRepo;
        private readonly IFormulaEngine _formulaEngine;

        public ChiTieuScoringService(
            IAppDbContext db,
            IChiTietKiemTraRepository chiTietRepo,
            IChiTietKiemTraInputRepository inputRepo,
            IKetQuaPhanLoaiThangRepository phanLoaiThangRepo,
            IKetQuaTrungGianRepository trungGianRepo,
            IFormulaEngine formulaEngine)
        {
            _db = db;
            _chiTietRepo = chiTietRepo;
            _inputRepo = inputRepo;
            _phanLoaiThangRepo = phanLoaiThangRepo;
            _trungGianRepo = trungGianRepo;
            _formulaEngine = formulaEngine;
        }

        /// <summary>Gộp 1 dòng vào batch ghi CBM_KetQuaTrungGian (audit trail) — commit chung 1 lần
        /// SaveChangesAsync cuối TinhVaLuuDiemSiAsync (cùng DbContext scoped với các repo khác).</summary>
        private static void GhiTrungGian(
            List<CBM_KetQuaTrungGian> sink, int idPhieu, int idChiTieu, string maKetQua, decimal? giaTri, string? nhan = null)
        {
            if (giaTri is null) return; // không ghi "chưa tính được" — tránh rác audit vô nghĩa
            sink.Add(new CBM_KetQuaTrungGian
            {
                IDPhieu = idPhieu,
                LoaiPham = "CHITIEU",
                ID_Pham = idChiTieu,
                MaKetQua = maKetQua,
                GiaTri = giaTri,
                Nhan = nhan,
            });
        }

        public async Task TinhVaLuuDiemSiAsync(
            int idPhieu,
            IEnumerable<NhapChiTietKiemTraDto> danhSachNhap,
            CancellationToken ct = default)
        {
            var danhSachNhapList = danhSachNhap as IList<NhapChiTietKiemTraDto> ?? danhSachNhap.ToList();
            // Dùng cho LoaiTinhDiem='TDCG' — cho phép chỉ tiêu thành phần trong CÙNG batch đang nhập
            // (chưa kịp lưu DB) vẫn đọc được, thay vì phải nhập/lưu riêng từng chỉ tiêu rồi mới tính TDCG.
            var giaTriTrongBatch = danhSachNhapList
                .GroupBy(x => x.ID_ChiTieu)
                .ToDictionary(g => g.Key, g => g.Last().GiaTriNhap_So);

            var trungGianCanGhi = new List<CBM_KetQuaTrungGian>();

            foreach (var nhap in danhSachNhapList)
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
                string? hanhDongKhuyenCao;

                var chiTieuInputs = await _db.ChiTieuInputs
                    .Where(x => x.ID_ChiTieu == idChiTieu)
                    .ToListAsync(ct);

                try
                {
                    if (loaiTinhDiem == "Rule" && chiTieuInputs.Count > 0)
                    {
                        (diemSi, hanhDongKhuyenCao) = await TinhDiemRuleAsync(
                            idPhieu, idChiTieu, chiTieuInputs, nhap.DanhSachInput, giaTriTrongBatch, ct);
                    }
                    else if (loaiTinhDiem == "LF")
                    {
                        var phieuInfo = await _db.PhieuKiemTras
                            .Where(p => p.ID_Phieu == idPhieu)
                            .Select(p => new { p.ID_ThietBi, p.NgayKiemTra })
                            .FirstAsync(ct);

                        var (lf, si, hanhDong) = await TinhDiemLFAsync(
                            idPhieu, phieuInfo.ID_ThietBi, phieuInfo.NgayKiemTra, idChiTieu, nhap.GiaTriNhap_So, ct);
                        diemSi = si;
                        hanhDongKhuyenCao = hanhDong;
                        GhiTrungGian(trungGianCanGhi, idPhieu, idChiTieu, "LF", lf, "Tỉ lệ mang tải bình quân trượt 12 tháng");
                    }
                    else if (loaiTinhDiem == "TOC_DO_SINH_KHI")
                    {
                        var phieuInfo = await _db.PhieuKiemTras
                            .Where(p => p.ID_Phieu == idPhieu)
                            .Select(p => new { p.ID_ThietBi, p.NgayKiemTra })
                            .FirstAsync(ct);

                        var (phanTramNam, si, hanhDong) = await TinhDiemTocDoSinhKhiAsync(
                            idPhieu, phieuInfo.ID_ThietBi, phieuInfo.NgayKiemTra, idChiTieu, nhap.GiaTriNhap_So, ct);
                        diemSi = si;
                        hanhDongKhuyenCao = hanhDong;
                        GhiTrungGian(trungGianCanGhi, idPhieu, idChiTieu, "TOC_DO_SINH_KHI", phanTramNam, "% tốc độ sinh khí/năm, đã quy đổi theo số ngày thực tế");
                    }
                    else
                    {
                        if (chiTieuInputs.Count == 0)
                        {
                            (diemSi, hanhDongKhuyenCao) = await TinhDiemDonAsync(idChiTieu, nhap.GiaTriNhap_So, ct);
                        }
                        else
                        {
                            // Mỗi biến Input tự chọn nguồn (NguonGiaTri): 'MANUAL' — người dùng nhập tay
                            // (hành vi cũ), hoặc 'CHITIEU_CUNG_PHIEU' — tự lấy GiaTriNhap_So của 1 chỉ
                            // tiêu khác trong CÙNG phiếu (vd TDCG lấy từ 6 chỉ tiêu khí thành phần).
                            // Nhờ vậy 1 chỉ tiêu "tự tính từ chỉ tiêu khác" cấu hình HOÀN TOÀN qua UI
                            // (Input nguồn + Formula NCalc + Ngưỡng theo MaKetQua), không cần thêm
                            // LoaiTinhDiem/code C# riêng mỗi khi phát sinh nhu cầu tương tự.
                            var vars = await XayDungVarsAsync(idPhieu, idChiTieu, chiTieuInputs, nhap.DanhSachInput, giaTriTrongBatch, ct);
                            if (vars is null)
                            {
                                // Thiếu giá trị của 1 biến nguồn 'CHITIEU_CUNG_PHIEU' (chỉ tiêu nguồn
                                // chưa đo trong phiếu này) — chưa đủ dữ liệu, không phải lỗi cấu hình.
                                diemSi = null;
                                hanhDongKhuyenCao = null;
                            }
                            else
                            {
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

                                // Ghi lại từng kết quả Formula trung gian (vd DT1/DT2/TDCG) — đây chính
                                // là giá trị "không phải Sᵢ nhưng vẫn cần truy vết" theo yêu cầu audit.
                                foreach (var (maKetQua, giaTri) in ketQuaFormula)
                                    GhiTrungGian(trungGianCanGhi, idPhieu, idChiTieu, maKetQua, giaTri);

                                (diemSi, hanhDongKhuyenCao) = ketQuaFormula.Count > 0
                                    ? await TinhDiemTuFormulaAsync(idChiTieu, ketQuaFormula, ct)
                                    : await TinhDiemNhieuBienAsync(idChiTieu, vars, ct);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Không để 1 chỉ tiêu lỗi cấu hình (thiếu Input, thiếu BieuThuc_Logic, lỗi cú pháp
                    // Formula/Rule...) làm mất hết chi tiết của CẢ phiếu — SaveChangesAsync chỉ gọi 1 lần
                    // sau vòng lặp nên nếu không bắt ở đây, phiếu vẫn được tạo (đã lưu trước đó ở
                    // PhieuKiemTraService) nhưng KHÔNG có chi tiết nào được lưu. Ghi Si=null cho riêng
                    // chỉ tiêu này, giữ nguyên giá trị nhập thô, và tiếp tục các chỉ tiêu còn lại.
                    Console.Error.WriteLine(
                        $"[ChiTieuScoringService] Lỗi tính Si cho chỉ tiêu ID={idChiTieu} (phiếu ID={idPhieu}): {ex.GetType().Name} - {ex.Message}");
                    diemSi = null;
                    hanhDongKhuyenCao = null;
                }

                chiTiet.GiaTriNhap_So = giaTriHienThi;
                chiTiet.GiaTriNhap_Chu = nhap.GiaTriNhap_Chu;
                chiTiet.GhiChu = nhap.GhiChu;
                chiTiet.Diem_Si_DatDuoc = diemSi;
                chiTiet.HanhDongKhuyenCao = hanhDongKhuyenCao;

                if (isNew)
                    await _chiTietRepo.AddAsync(chiTiet);
                else
                    _chiTietRepo.Update(chiTiet);
            }

            if (trungGianCanGhi.Count > 0)
                await _trungGianRepo.AddRangeAsync(trungGianCanGhi);

            await _chiTietRepo.SaveChangesAsync();
        }

        public async Task TinhLaiToanBoSiAsync(int idPhieu, CancellationToken ct = default)
        {
            var chiTiets = await _db.ChiTietKiemTras
                .Where(x => x.IDPhieu == idPhieu)
                .Select(x => new { x.ID_ChiTieu, x.GiaTriNhap_So, x.GiaTriNhap_Chu, x.GhiChu })
                .ToListAsync(ct);

            if (chiTiets.Count == 0) return;

            var inputRows = await _inputRepo.FindAsync(x => x.IDPhieu == idPhieu);
            var inputsByChiTieu = inputRows
                .Where(x => x.MaInput is not null)
                .GroupBy(x => x.ID_ChiTieu)
                .ToDictionary(g => g.Key, g => g.ToDictionary(x => x.MaInput!, x => x.GiaTriSo));

            var danhSachNhap = chiTiets.Select(c => new NhapChiTietKiemTraDto
            {
                ID_ChiTieu = c.ID_ChiTieu,
                GiaTriNhap_So = c.GiaTriNhap_So,
                GiaTriNhap_Chu = c.GiaTriNhap_Chu,
                GhiChu = c.GhiChu,
                DanhSachInput = inputsByChiTieu.TryGetValue(c.ID_ChiTieu, out var vars) && vars.Count > 0 ? vars : null,
            }).ToList();

            await TinhVaLuuDiemSiAsync(idPhieu, danhSachNhap, ct);
        }

        /// <summary>
        /// LoaiTinhDiem='Rule': CBM_ChiTieu_Rule.LoaiRule='BANG_MUC' — nhiều dòng điều kiện boolean,
        /// khớp đầu tiên theo Diem_Si giảm dần thắng. Trước đây chỉ có ở PhieuKiemTraService.CreateAsync;
        /// chuyển vào đây để CreateAsync có thể delegate toàn bộ việc tính Si qua service này.
        /// </summary>
        private async Task<(decimal? Si, string? HanhDong)> TinhDiemRuleAsync(
            int idPhieu, int idChiTieu, List<CBM_ChiTieu_Input> chiTieuInputs,
            Dictionary<string, decimal>? danhSachInput,
            IReadOnlyDictionary<int, decimal?> giaTriTrongBatch, CancellationToken ct)
        {
            // Dùng chung XayDungVarsAsync với nhánh Formula phía dưới — trước đây hàm này tự đọc
            // thẳng danhSachInput (chỉ hỗ trợ nhập tay), bỏ qua hoàn toàn NguonGiaTri='CHITIEU_CUNG_PHIEU'
            // /'THIETBI_THONGSO' của từng biến, khiến các biến đó luôn bị NCalc coi là 0 một cách âm
            // thầm (không lỗi, sai kết quả) — bug thật phát hiện qua phiếu thật (chỉ tiêu Dòng động cơ
            // OLTC, biến Ir nguồn THIETBI_THONGSO luôn ra Sᵢ=0 dù giá trị đo thực tế tốt).
            var vars = await XayDungVarsAsync(idPhieu, idChiTieu, chiTieuInputs, danhSachInput, giaTriTrongBatch, ct);
            if (vars is null || vars.Count == 0) return (null, null);
            await LuuInputsAsync(idPhieu, idChiTieu, vars);

            var rules = await _db.ChiTieuRules
                .Where(r => r.ID_ChiTieu == idChiTieu && r.LoaiRule == "BANG_MUC")
                .OrderByDescending(r => r.Diem_Si)
                .ToListAsync(ct);

            foreach (var rule in rules)
            {
                if (NguongEvaluator.EvalNCalc(rule.BieuThuc, vars))
                    return (rule.Diem_Si, rule.HanhDongKhuyenCao);
            }

            return (null, null);
        }

        private async Task<(decimal? Si, string? HanhDong)> TinhDiemDonAsync(int idChiTieu, decimal? giaTri, CancellationToken ct)
        {
            var nguongs = await _db.Nguongs
                .Where(x => x.ID_ChiTieu == idChiTieu)
                .OrderBy(x => x.ThuTu)
                .ToListAsync(ct);

            foreach (var ng in nguongs)
            {
                if (NguongEvaluator.KiemTraNguongVoiGiaTri(giaTri, ng))
                    return (ng.Diem_Si, ng.HanhDongKhuyenCao);
            }

            return (null, null);
        }

        private async Task<(decimal? Si, string? HanhDong)> TinhDiemNhieuBienAsync(
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
                    return (ng.Diem_Si, ng.HanhDongKhuyenCao);
            }

            return (null, null);
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
        private async Task<(decimal? Si, string? HanhDong)> TinhDiemTuFormulaAsync(
            int idChiTieu, Dictionary<string, decimal> ketQuaFormula, CancellationToken ct)
        {
            var congThucRule = await _db.ChiTieuRules
                .Where(r => r.ID_ChiTieu == idChiTieu && r.LoaiRule == "CONG_THUC")
                .FirstOrDefaultAsync(ct);
            if (congThucRule != null)
                return (NguongEvaluator.EvalNCalcNumeric(congThucRule.BieuThuc, ketQuaFormula), congThucRule.HanhDongKhuyenCao);

            var bangMucRules = await _db.ChiTieuRules
                .Where(r => r.ID_ChiTieu == idChiTieu && r.LoaiRule == "BANG_MUC")
                .OrderByDescending(r => r.Diem_Si)
                .ToListAsync(ct);
            if (bangMucRules.Count > 0)
            {
                foreach (var rule in bangMucRules)
                    if (NguongEvaluator.EvalNCalc(rule.BieuThuc, ketQuaFormula))
                        return (rule.Diem_Si, rule.HanhDongKhuyenCao);
                return (null, null); // có Rule nhưng không dòng nào khớp — coi như chưa xác định được Si
            }

            // Không có Rule -> tra Ngưỡng theo MaKetQua như cũ.
            var siTheoKetQua = new Dictionary<string, decimal>();
            string? hanhDongDuyNhat = null;
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
                        hanhDongDuyNhat = ng.HanhDongKhuyenCao;
                        break;
                    }
                }

                if (si is null)
                    throw new LoiFormulaException(idChiTieu, maKetQua,
                        $"Giá trị {giaTri} không khớp ngưỡng nào (CBM_Nguong.MaKetQua='{maKetQua}').");

                siTheoKetQua[$"Si_{maKetQua}"] = si.Value;
            }

            if (siTheoKetQua.Count == 1)
                return (siTheoKetQua.Values.First(), hanhDongDuyNhat);

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
        public async Task<(decimal? Lf, decimal? DiemSi, string? HanhDong)> TinhDiemLFAsync(
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

            if (soThangHopLe == 0) return (null, null, null);

            var lf = tongTrongSo / soThangHopLe;

            var nguongs = await _db.Nguongs
                .Where(x => x.ID_ChiTieu == idChiTieu)
                .OrderBy(x => x.ThuTu)
                .ToListAsync(ct);

            decimal? diemSi = null;
            string? hanhDong = null;
            foreach (var ng in nguongs)
            {
                if (NguongEvaluator.KiemTraNguongVoiGiaTri(lf, ng))
                {
                    diemSi = ng.Diem_Si;
                    hanhDong = ng.HanhDongKhuyenCao;
                    break;
                }
            }

            return (lf, diemSi, hanhDong);
        }

        /// <summary>
        /// Tính điểm cho chỉ tiêu kiểu LoaiTinhDiem='TOC_DO_SINH_KHI' (ví dụ tốc độ sinh khí DGA %/năm,
        /// QT.40 mục 17.3.7). Người dùng chỉ nhập giá trị đo hiện tại (ppm) — hệ thống tự tìm giá trị đo
        /// GẦN NHẤT trước đó của cùng chỉ tiêu + cùng thiết bị, tính % theo công thức IEEE C57.104:
        /// % = (giá trị hiện tại - giá trị trước) / GiaTri_L1 × 100, rồi QUY ĐỔI theo đúng số ngày thực
        /// tế giữa 2 lần đo (annualize ×365/số ngày) vì lịch đo thực tế hiếm khi cách nhau đúng 365 ngày.
        /// Không tìm được giá trị đo trước đó nào (thiết bị mới/lần đo đầu tiên) → trả về Si=null
        /// ("chưa tính được"), không mặc định điểm nào để tránh che giấu tình trạng thật.
        /// </summary>
        private async Task<(decimal? PhanTramNam, decimal? DiemSi, string? HanhDong)> TinhDiemTocDoSinhKhiAsync(
            int idPhieu, int idThietBi, DateTime ngayKiemTra, int idChiTieu, decimal? giaTriHienTai,
            CancellationToken ct = default)
        {
            if (giaTriHienTai is null) return (null, null, null);

            var giaTriL1 = await _db.ChiTieus
                .Where(c => c.ID_ChiTieu == idChiTieu)
                .Select(c => c.GiaTri_L1)
                .FirstOrDefaultAsync(ct);

            if (giaTriL1 is null or 0) throw new ThieuGiaTriL1Exception(idChiTieu);

            var lanDoTruoc = await (
                from ctCu in _db.ChiTietKiemTras
                join p in _db.PhieuKiemTras on ctCu.IDPhieu equals p.ID_Phieu
                where p.ID_ThietBi == idThietBi
                      && ctCu.ID_ChiTieu == idChiTieu
                      && p.ID_Phieu != idPhieu
                      && p.NgayKiemTra < ngayKiemTra
                      && ctCu.GiaTriNhap_So != null
                orderby p.NgayKiemTra descending, p.ID_Phieu descending
                select new { p.NgayKiemTra, ctCu.GiaTriNhap_So }
            ).FirstOrDefaultAsync(ct);

            if (lanDoTruoc is null) return (null, null, null); // chưa có mốc so sánh — chưa tính được

            var soNgay = (ngayKiemTra.Date - lanDoTruoc.NgayKiemTra.Date).Days;
            if (soNgay <= 0) return (null, null, null); // dữ liệu lịch sử bất thường (cùng ngày/tương lai) — bỏ qua

            var phanTramNam = (giaTriHienTai.Value - lanDoTruoc.GiaTriNhap_So!.Value) / giaTriL1.Value
                               * 100m * (365m / soNgay);

            var nguongs = await _db.Nguongs
                .Where(x => x.ID_ChiTieu == idChiTieu)
                .OrderBy(x => x.ThuTu)
                .ToListAsync(ct);

            decimal? diemSi = null;
            string? hanhDong = null;
            foreach (var ng in nguongs)
            {
                if (NguongEvaluator.KiemTraNguongVoiGiaTri(phanTramNam, ng))
                {
                    diemSi = ng.Diem_Si;
                    hanhDong = ng.HanhDongKhuyenCao;
                    break;
                }
            }

            return (phanTramNam, diemSi, hanhDong);
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

        /// <summary>
        /// Dựng bộ biến (vars) cho Input của 1 chỉ tiêu — mỗi biến tự chọn nguồn qua
        /// CBM_ChiTieu_Input.NguonGiaTri:
        ///   'MANUAL' (mặc định) — bắt buộc có trong <paramref name="danhSachInput"/>, thiếu thì ném
        ///     ThieuInputChiTieuException (lỗi cấu hình/nhập liệu thật — người dùng bỏ sót ô bắt buộc).
        ///   'CHITIEU_CUNG_PHIEU' — tự lấy GiaTriNhap_So của CBM_ChiTieu_Input.ID_ChiTieuNguon trong
        ///     CÙNG phiếu (ưu tiên đọc <paramref name="giaTriTrongBatch"/> — cùng batch đang nhập,
        ///     chưa kịp lưu DB; fallback đọc DB nếu chỉ tiêu nguồn đã lưu từ lần nhập trước). Thiếu
        ///     giá trị (chỉ tiêu nguồn chưa đo) → trả về null cho CẢ hàm (không phải lỗi cấu hình,
        ///     chỉ là "chưa đủ dữ liệu" — caller tự hiểu và gán Sᵢ=null, không ném exception ồn ào).
        /// Đây là cơ chế DUY NHẤT cần thiết để cấu hình 1 chỉ tiêu "tự tính từ chỉ tiêu khác" (vd
        /// TDCG = tổng 6 khí) hoàn toàn qua UI (Input nguồn + Formula + Ngưỡng), không cần thêm
        /// LoaiTinhDiem/code C# riêng mỗi khi phát sinh nhu cầu tương tự.
        /// </summary>
        private async Task<Dictionary<string, decimal>?> XayDungVarsAsync(
            int idPhieu, int idChiTieu,
            List<CBM_ChiTieu_Input> chiTieuInputs,
            Dictionary<string, decimal>? danhSachInput,
            IReadOnlyDictionary<int, decimal?> giaTriTrongBatch,
            CancellationToken ct)
        {
            var vars = new Dictionary<string, decimal>();
            int? idThietBi = null; // lazy — chỉ query khi thật sự cần (nguồn THIETBI_THONGSO)

            foreach (var inp in chiTieuInputs)
            {
                var nguon = inp.NguonGiaTri ?? "MANUAL";

                if (nguon == "MANUAL")
                {
                    if (danhSachInput == null || !danhSachInput.TryGetValue(inp.MaInput, out var val))
                        throw new ThieuInputChiTieuException(idChiTieu, inp.MaInput);
                    vars[inp.MaInput] = val;
                }
                else if (nguon == "CHITIEU_CUNG_PHIEU")
                {
                    if (inp.ID_ChiTieuNguon is null)
                        throw new ThieuCauHinhInputException(idChiTieu, inp.MaInput);

                    decimal? giaTri = giaTriTrongBatch.TryGetValue(inp.ID_ChiTieuNguon.Value, out var vBatch) && vBatch is not null
                        ? vBatch
                        : await _db.ChiTietKiemTras
                            .Where(x => x.IDPhieu == idPhieu && x.ID_ChiTieu == inp.ID_ChiTieuNguon.Value)
                            .Select(x => x.GiaTriNhap_So)
                            .FirstOrDefaultAsync(ct);

                    if (giaTri is null) return null; // chỉ tiêu nguồn chưa đo — chưa đủ dữ liệu
                    vars[inp.MaInput] = giaTri.Value;
                }
                else if (nguon == "THIETBI_THONGSO")
                {
                    if (string.IsNullOrWhiteSpace(inp.MaThongSoThietBi))
                        throw new ThieuCauHinhInputException(idChiTieu, inp.MaInput);

                    idThietBi ??= await _db.PhieuKiemTras
                        .Where(p => p.ID_Phieu == idPhieu)
                        .Select(p => p.ID_ThietBi)
                        .FirstAsync(ct);

                    var giaTri = await (
                        from t in _db.ThietBiThongSos
                        join ts in _db.ThongSos on t.ID_ThongSo equals ts.ID_ThongSo
                        where t.ID_ThietBi == idThietBi.Value && ts.MaThongSo == inp.MaThongSoThietBi
                        select (decimal?)t.GiaTri
                    ).FirstOrDefaultAsync(ct);

                    if (giaTri is null)
                        throw new ThieuThongSoThietBiException(idThietBi.Value, inp.MaThongSoThietBi);
                    vars[inp.MaInput] = giaTri.Value;
                }
                else
                {
                    throw new ThieuCauHinhInputException(idChiTieu, inp.MaInput);
                }
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
