namespace PM_QLTBHTD.Application.DTOs
{
    /// <summary>
    /// Dữ liệu nhập cho 1 chỉ tiêu trong 1 lần gọi POST /api/chi-tiet-kiem-tra/phieu/{id}.
    /// </summary>
    public class NhapChiTietKiemTraDto
    {
        public int ID_ChiTieu { get; set; }

        /// <summary>Giá trị đơn — dùng khi chỉ tiêu không có CBM_ChiTieu_Input (0 dòng input).</summary>
        public decimal? GiaTriNhap_So { get; set; }

        public string? GiaTriNhap_Chu { get; set; }

        /// <summary>
        /// Các biến đầu vào khi chỉ tiêu có nhiều CBM_ChiTieu_Input.
        /// Key = MaInput (phải khớp MaInput trong CBM_ChiTieu_Input).
        /// </summary>
        public Dictionary<string, decimal>? DanhSachInput { get; set; }

        /// <summary>Danh sách giá trị đo theo tháng — dùng khi chỉ tiêu có LoaiTinhDiem='LF' (Load Factor).</summary>
        public List<ThangDoDto>? DanhSachThang { get; set; }

        public string? GhiChu { get; set; }
    }

    /// <summary>
    /// Request body cho POST /api/chi-tiet-kiem-tra/phieu/{id}.
    /// </summary>
    public class NhapPhieuKiemTraRequest
    {
        public List<NhapChiTietKiemTraDto> DanhSachChiTieu { get; set; } = [];

        /// <summary>Nếu true, tự động gọi tính điểm nhóm gốc sau khi lưu Si.</summary>
        public bool TuDongTinhDiem { get; set; } = false;

        /// <summary>ID NhomChiTieu cần tính điểm (nếu TuDongTinhDiem = true).</summary>
        public int? ID_NhomChiTieuTinhDiem { get; set; }
    }

    public class NhapPhieuKiemTraResponse
    {
        public List<ChiTietKiemTraDto> KetQuaNhap { get; set; } = [];
        public KetQuaNhomDto? KetQuaTinhDiem { get; set; }
    }
}
