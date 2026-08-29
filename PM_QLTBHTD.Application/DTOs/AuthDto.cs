namespace PM_QLTBHTD.Application.DTOs
{
    public class LoginRequestDto
    {
        public string TenDangNhap { get; set; } = string.Empty;
        public string MatKhau { get; set; } = string.Empty;
    }

    public class LoginResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public NguoiDungDto NguoiDung { get; set; } = null!;
    }
}
