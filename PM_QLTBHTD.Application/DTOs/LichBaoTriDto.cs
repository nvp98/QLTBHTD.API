namespace PM_QLTBHTD.Application.DTOs
{
    public class LichBaoTriDto
    {
        public int ID_LichBaoTri { get; set; }
        public int ID_ThietBi { get; set; }
        public string TenThietBi { get; set; } = string.Empty;
        public int ID_Tram { get; set; }
        public string TenTram { get; set; } = string.Empty;

        public int? ID_LichBaoTriGoc { get; set; }
        public string LoaiBaoTri { get; set; } = string.Empty;
        public int? ChuKyThang { get; set; }

        public DateTime NgayKeHoach { get; set; }
        public DateTime? NgayThucHien { get; set; }

        public string TrangThai { get; set; } = string.Empty;

        /// <summary>Trạng thái hiển thị suy ra: ChoThucHien | QuaHan | SapToiHan | HoanThanh | DaHuy.</summary>
        public string TrangThaiHienThi { get; set; } = string.Empty;

        public string? NguoiPhuTrach { get; set; }
        public string? NoiDungCongViec { get; set; }
        public string? GhiChu { get; set; }
        public int? ID_PhieuKetQua { get; set; }
        public DateTime NgayTao { get; set; }
    }

    public class CreateLichBaoTriDto
    {
        public int ID_ThietBi { get; set; }
        public string LoaiBaoTri { get; set; } = string.Empty;
        public int? ChuKyThang { get; set; }
        public DateTime NgayKeHoach { get; set; }
        public string? NguoiPhuTrach { get; set; }
        public string? NoiDungCongViec { get; set; }
        public string? GhiChu { get; set; }
    }

    public class UpdateLichBaoTriDto
    {
        public string LoaiBaoTri { get; set; } = string.Empty;
        public int? ChuKyThang { get; set; }
        public DateTime NgayKeHoach { get; set; }
        public string? NguoiPhuTrach { get; set; }
        public string? NoiDungCongViec { get; set; }
        public string? GhiChu { get; set; }
    }

    public class HoanThanhLichBaoTriDto
    {
        public DateTime NgayThucHien { get; set; }
        public string? GhiChu { get; set; }
    }

    public class ThongKeLichBaoTriDto
    {
        public int TongDangCho { get; set; }
        public int SoQuaHan { get; set; }
        public int SoSapToiHan7Ngay { get; set; }
        public int SoHoanThanhThangNay { get; set; }
        public List<LichBaoTriDto> DanhSachCanChuY { get; set; } = new();
    }
}
