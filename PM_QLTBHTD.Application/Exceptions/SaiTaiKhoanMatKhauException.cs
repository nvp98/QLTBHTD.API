namespace PM_QLTBHTD.Application.Exceptions
{
    public class SaiTaiKhoanMatKhauException : Exception
    {
        public SaiTaiKhoanMatKhauException()
            : base("Tên đăng nhập hoặc mật khẩu không đúng.")
        {
        }
    }

    public class TaiKhoanBiKhoaException : Exception
    {
        public TaiKhoanBiKhoaException()
            : base("Tài khoản đã bị khóa. Liên hệ quản trị viên để được hỗ trợ.")
        {
        }
    }

    public class TenDangNhapDaTonTaiException : Exception
    {
        public TenDangNhapDaTonTaiException(string tenDangNhap)
            : base($"Tên đăng nhập '{tenDangNhap}' đã tồn tại.")
        {
        }
    }

    public class MatKhauCuKhongDungException : Exception
    {
        public MatKhauCuKhongDungException()
            : base("Mật khẩu cũ không đúng.")
        {
        }
    }
}
