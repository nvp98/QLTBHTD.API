namespace PM_QLTBHTD.Application.DTOs
{
    public class CongThucTongHopDto
    {
        public int ID_CongThuc { get; set; }
        public int ID_NhomChiTieu { get; set; }
        public string TenNhom { get; set; } = string.Empty;
        public string BieuThuc { get; set; } = string.Empty;
        public string LoaiCongThuc { get; set; } = string.Empty;
        public int PhienBan { get; set; }
        public int TrangThai { get; set; }
        public decimal? ThangDiem_Min { get; set; }
        public decimal? ThangDiem_Max { get; set; }
        public string? MoTa { get; set; }
        public List<CongThucBienDto> DanhSachBien { get; set; } = [];
    }

    public class CreateCongThucTongHopDto
    {
        public int ID_NhomChiTieu { get; set; }
        public string BieuThuc { get; set; } = string.Empty;
        public string LoaiCongThuc { get; set; } = "CUSTOM_NCALC";
        public int PhienBan { get; set; } = 1;
        public decimal? ThangDiem_Min { get; set; }
        public decimal? ThangDiem_Max { get; set; }
        public string? MoTa { get; set; }
    }

    public class UpdateCongThucTongHopDto
    {
        public string BieuThuc { get; set; } = string.Empty;
        public string LoaiCongThuc { get; set; } = "CUSTOM_NCALC";
        public int TrangThai { get; set; }
        public decimal? ThangDiem_Min { get; set; }
        public decimal? ThangDiem_Max { get; set; }
        public string? MoTa { get; set; }
    }
}
