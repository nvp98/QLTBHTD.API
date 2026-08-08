namespace PM_QLTBHTD.Application.DTOs
{
    public class ThietBiThongSoDto
    {
        public int     ID_ThietBi_ThongSo { get; set; }
        public int     ID_ThietBi { get; set; }
        public int     ID_ThongSo { get; set; }
        public string  MaThongSo  { get; set; } = string.Empty;
        public string  TenThongSo { get; set; } = string.Empty;
        public string? DonVi      { get; set; }
        public decimal GiaTri     { get; set; }
        public string? GhiChu     { get; set; }
    }

    public class CreateThietBiThongSoDto
    {
        public int     ID_ThietBi { get; set; }
        public int     ID_ThongSo { get; set; }
        public decimal GiaTri     { get; set; }
        public string? GhiChu     { get; set; }
    }

    public class UpdateThietBiThongSoDto
    {
        public int     ID_ThongSo { get; set; }
        public decimal GiaTri     { get; set; }
        public string? GhiChu     { get; set; }
    }

    public class ThietBiThongSoUsageDto
    {
        public int     ID_ThietBi_ThongSo { get; set; }
        public int     ID_ThietBi         { get; set; }
        public string  TenThietBi         { get; set; } = string.Empty;
        public decimal GiaTri             { get; set; }
        public string? GhiChu             { get; set; }
    }
}
