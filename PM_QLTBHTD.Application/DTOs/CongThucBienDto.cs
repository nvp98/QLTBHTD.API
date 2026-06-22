namespace PM_QLTBHTD.Application.DTOs
{
    public class CongThucBienDto
    {
        public int ID_Bien { get; set; }
        public int ID_CongThuc { get; set; }
        public string MaBien { get; set; } = string.Empty;
        public string NguonBien { get; set; } = string.Empty;
        public int? ID_ChiTieuNguon { get; set; }
        public string? TenChiTieu { get; set; }
        public int? ID_NhomCon { get; set; }
        public string? TenNhomCon { get; set; }
        public decimal? GiaTriHangSo { get; set; }
        public string? MoTa { get; set; }
    }

    public class CreateCongThucBienDto
    {
        public int ID_CongThuc { get; set; }
        public string MaBien { get; set; } = string.Empty;
        public string NguonBien { get; set; } = "HANGSO";
        public int? ID_ChiTieuNguon { get; set; }
        public int? ID_NhomCon { get; set; }
        public decimal? GiaTriHangSo { get; set; }
        public string? MoTa { get; set; }
    }

    public class UpdateCongThucBienDto
    {
        public string MaBien { get; set; } = string.Empty;
        public string NguonBien { get; set; } = "HANGSO";
        public int? ID_ChiTieuNguon { get; set; }
        public int? ID_NhomCon { get; set; }
        public decimal? GiaTriHangSo { get; set; }
        public string? MoTa { get; set; }
    }
}
