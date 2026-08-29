namespace PM_QLTBHTD.Application.DTOs
{
    public class TramDienDto
    {
        public int IDTram { get; set; }
        public int IDKhuVuc { get; set; }
        public string TenKhuVuc { get; set; } = string.Empty;
        public string TenTram { get; set; } = string.Empty;
        public string? DiaDiem { get; set; }
        public int TrangThai { get; set; }
    }

    public class CreateTramDienDto
    {
        public int IDKhuVuc { get; set; }
        public string TenTram { get; set; } = string.Empty;
        public string? DiaDiem { get; set; }
        public int TrangThai { get; set; } = 1;
    }

    public class UpdateTramDienDto
    {
        public int IDKhuVuc { get; set; }
        public string TenTram { get; set; } = string.Empty;
        public string? DiaDiem { get; set; }
        public int TrangThai { get; set; }
    }
}
