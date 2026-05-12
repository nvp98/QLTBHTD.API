namespace PM_QLTBHTD.Application.DTOs
{
    public class PhieuKiemTraDto
    {
        public int ID_Phieu { get; set; }
        public int ID_ThietBi { get; set; }
        public string TenThietBi { get; set; } = string.Empty;
        public int? ID_NhomChiTieu { get; set; }
        public string? TenNhom { get; set; }
        public DateTime NgayKiemTra { get; set; }
        public string? NguoiKiemTra { get; set; }
        public decimal? TongDiem_Soqt { get; set; }
        public string? CapDoCanhBao { get; set; }
        public string? GhiChuChung { get; set; }
    }

    public class CreateChiTietInputValueDto
    {
        public int ID_Input { get; set; }
        public decimal GiaTriSo { get; set; }
    }

    /// <summary>
    /// Giá trị biến đầu vào theo tên — dùng cho chỉ tiêu Nguong có BieuThuc_Logic
    /// mà chưa khai báo CBM_ChiTieu_Input trong cơ sở dữ liệu.
    /// Backend dùng MaInput để xây dict biến và đánh giá BieuThuc_Logic bằng NCalc.
    /// </summary>
    public class CreateChiTietInputNamedDto
    {
        public int ID_ChiTieu { get; set; }
        public string MaInput { get; set; } = string.Empty;
        public decimal GiaTriSo { get; set; }
    }

    public class CreatePhieuKiemTraDto
    {
        public int ID_ThietBi { get; set; }
        /// <summary>Nhóm chỉ tiêu cần đo. NULL = kiểm tra toàn diện.</summary>
        public int? ID_NhomChiTieu { get; set; }
        public DateTime NgayKiemTra { get; set; }
        public string? NguoiKiemTra { get; set; }
        public string? GhiChuChung { get; set; }
        public List<CreateChiTietKiemTraDto> ChiTiets { get; set; } = new();
        /// <summary>Giá trị đầu vào cho chỉ tiêu kiểu Rule (nhiều biến, có ID_Input từ DB).</summary>
        public List<CreateChiTietInputValueDto> ChiTietInputs { get; set; } = new();
        /// <summary>Giá trị biến theo tên cho chỉ tiêu Nguong có BieuThuc_Logic phức tạp.</summary>
        public List<CreateChiTietInputNamedDto> ChiTietInputsNamed { get; set; } = new();
    }

    public class UpdatePhieuKiemTraDto
    {
        public int ID_ThietBi { get; set; }
        public int? ID_NhomChiTieu { get; set; }
        public DateTime NgayKiemTra { get; set; }
        public string? NguoiKiemTra { get; set; }
        public decimal? TongDiem_Soqt { get; set; }
        public string? CapDoCanhBao { get; set; }
        public string? GhiChuChung { get; set; }
    }

    public class PhieuKiemTraDetailDto : PhieuKiemTraDto
    {
        public List<ChiTietKiemTraDto> ChiTiets { get; set; } = new();
    }
}
