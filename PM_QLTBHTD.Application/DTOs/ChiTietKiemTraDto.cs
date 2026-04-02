namespace PM_QLTBHTD.Application.DTOs
{
    public class ChiTietKiemTraDto
    {
        public int ID_ChiTiet { get; set; }
        public int IDPhieu { get; set; }
        public int ID_ChiTieu { get; set; }
        public string TenChiTieu { get; set; } = string.Empty;
        public decimal? GiaTriNhap_So { get; set; }
        public string? GiaTriNhap_Chu { get; set; }
        public decimal? Diem_Si_DatDuoc { get; set; }
        public string? GhiChu { get; set; }
    }

    public class CreateChiTietKiemTraDto
    {
        public int ID_ChiTieu { get; set; }
        public decimal? GiaTriNhap_So { get; set; }
        public string? GiaTriNhap_Chu { get; set; }
        public string? GhiChu { get; set; }
    }

    public class UpdateChiTietKiemTraDto
    {
        public int IDPhieu { get; set; }
        public int ID_ChiTieu { get; set; }
        public decimal? GiaTriNhap_So { get; set; }
        public string? GiaTriNhap_Chu { get; set; }
        public decimal? Diem_Si_DatDuoc { get; set; }
        public string? GhiChu { get; set; }
    }
}
