namespace PM_QLTBHTD.Application.DTOs
{
    public class ThietBiDto
    {
        public int ID_ThietBi { get; set; }
        public int ID_Tram { get; set; }
        public string TenTram { get; set; } = string.Empty;
        public int ID_LoaiTB { get; set; }
        public string TenLoaiTB { get; set; } = string.Empty;
        public string TenThietBi { get; set; } = string.Empty;
        public string? SoHieu { get; set; }
        public string? NhanHieu { get; set; }
        public int? NamSanXuat { get; set; }
        public int TrangThai { get; set; }
        public string? GhiChu { get; set; }
    }

    public class CreateThietBiDto
    {
        public int ID_Tram { get; set; }
        public int ID_LoaiTB { get; set; }
        public string TenThietBi { get; set; } = string.Empty;
        public string? SoHieu { get; set; }
        public string? NhanHieu { get; set; }
        public int? NamSanXuat { get; set; }
        public int TrangThai { get; set; } = 1;
        public string? GhiChu { get; set; }
    }

    public class UpdateThietBiDto
    {
        public int ID_Tram { get; set; }
        public int ID_LoaiTB { get; set; }
        public string TenThietBi { get; set; } = string.Empty;
        public string? SoHieu { get; set; }
        public string? NhanHieu { get; set; }
        public int? NamSanXuat { get; set; }
        public int TrangThai { get; set; }
        public string? GhiChu { get; set; }
    }
}
