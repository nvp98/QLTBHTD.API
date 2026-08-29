namespace PM_QLTBHTD.Application.DTOs
{
    public class NguoiDungDto
    {
        public int ID_NguoiDung { get; set; }
        public string TenDangNhap { get; set; } = string.Empty;
        public string HoTen { get; set; } = string.Empty;
        public string? Email { get; set; }
        public int ID_VaiTro { get; set; }
        public string MaVaiTro { get; set; } = string.Empty;
        public string TenVaiTro { get; set; } = string.Empty;
        public int? ID_Tram { get; set; }
        public string? TenTram { get; set; }
        public int TrangThai { get; set; }
        public DateTime NgayTao { get; set; }
    }

    public class CreateNguoiDungDto
    {
        public string TenDangNhap { get; set; } = string.Empty;
        public string MatKhau { get; set; } = string.Empty;
        public string HoTen { get; set; } = string.Empty;
        public string? Email { get; set; }
        public int ID_VaiTro { get; set; }
        public int? ID_Tram { get; set; }
    }

    public class UpdateNguoiDungDto
    {
        public string HoTen { get; set; } = string.Empty;
        public string? Email { get; set; }
        public int ID_VaiTro { get; set; }
        public int? ID_Tram { get; set; }
        public int TrangThai { get; set; }
    }

    public class DoiMatKhauDto
    {
        public string MatKhauCu { get; set; } = string.Empty;
        public string MatKhauMoi { get; set; } = string.Empty;
    }

    /// <summary>Admin đặt lại mật khẩu cho user khác (không cần biết mật khẩu cũ).</summary>
    public class DatLaiMatKhauDto
    {
        public string MatKhauMoi { get; set; } = string.Empty;
    }
}
