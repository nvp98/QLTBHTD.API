namespace PM_QLTBHTD.Application.DTOs
{
    public class KhuVucDto
    {
        public int ID_KhuVuc { get; set; }
        public string TenKhuVuc { get; set; } = string.Empty;
        public int TrangThai { get; set; }
    }

    public class CreateKhuVucDto
    {
        public string TenKhuVuc { get; set; } = string.Empty;
        public int TrangThai { get; set; } = 1;
    }

    public class UpdateKhuVucDto
    {
        public string TenKhuVuc { get; set; } = string.Empty;
        public int TrangThai { get; set; }
    }
}
