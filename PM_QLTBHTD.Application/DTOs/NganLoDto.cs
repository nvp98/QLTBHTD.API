namespace PM_QLTBHTD.Application.DTOs
{
    public class NganLoDto
    {
        public int ID_NganLo { get; set; }
        public int ID_Tram { get; set; }
        public string TenTram { get; set; } = string.Empty;
        public string TenNganLo { get; set; } = string.Empty;
        public string? MaNganLo { get; set; }
        public int TrangThai { get; set; }
        public int SoThietBi { get; set; }
    }

    public class CreateNganLoDto
    {
        public int ID_Tram { get; set; }
        public string TenNganLo { get; set; } = string.Empty;
        public string? MaNganLo { get; set; }
        public int TrangThai { get; set; } = 1;
    }

    public class UpdateNganLoDto
    {
        public int ID_Tram { get; set; }
        public string TenNganLo { get; set; } = string.Empty;
        public string? MaNganLo { get; set; }
        public int TrangThai { get; set; }
    }
}
