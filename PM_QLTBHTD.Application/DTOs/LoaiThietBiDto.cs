namespace PM_QLTBHTD.Application.DTOs
{
    public class LoaiThietBiDto
    {
        public int ID_LoaiThietBi { get; set; }
        public string TenLoaiTB { get; set; } = string.Empty;
        public string KyHieu { get; set; } = string.Empty;
        public int TrangThai { get; set; }
    }

    public class CreateLoaiThietBiDto
    {
        public string TenLoaiTB { get; set; } = string.Empty;
        public string KyHieu { get; set; } = string.Empty;
        public int TrangThai { get; set; } = 1;
    }

    public class UpdateLoaiThietBiDto
    {
        public string TenLoaiTB { get; set; } = string.Empty;
        public string KyHieu { get; set; } = string.Empty;
        public int TrangThai { get; set; }
    }
}
