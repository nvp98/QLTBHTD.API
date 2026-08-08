namespace PM_QLTBHTD.Application.DTOs
{
    public class ThongSoDto
    {
        public int     ID_ThongSo  { get; set; }
        public string  MaThongSo   { get; set; } = string.Empty;
        public string  TenThongSo  { get; set; } = string.Empty;
        public string? DonVi       { get; set; }
        public string  LoaiDuLieu  { get; set; } = "DECIMAL";
        public int     TrangThai   { get; set; }
        public string? GhiChu      { get; set; }
    }

    public class CreateThongSoDto
    {
        public string  MaThongSo   { get; set; } = string.Empty;
        public string  TenThongSo  { get; set; } = string.Empty;
        public string? DonVi       { get; set; }
        public string  LoaiDuLieu  { get; set; } = "DECIMAL";
        public int     TrangThai   { get; set; } = 1;
        public string? GhiChu      { get; set; }
    }

    public class UpdateThongSoDto
    {
        public string  MaThongSo   { get; set; } = string.Empty;
        public string  TenThongSo  { get; set; } = string.Empty;
        public string? DonVi       { get; set; }
        public string  LoaiDuLieu  { get; set; } = "DECIMAL";
        public int     TrangThai   { get; set; }
        public string? GhiChu      { get; set; }
    }
}
