namespace PM_QLTBHTD.Application.DTOs
{
    public class ChiTieuInputDto
    {
        public int     ID_Input   { get; set; }
        public int     ID_ChiTieu { get; set; }
        public string  MaInput    { get; set; } = string.Empty;
        public string  TenInput   { get; set; } = string.Empty;
        public string  NguonGiaTri     { get; set; } = "MANUAL";
        public int?    ID_ChiTieuNguon { get; set; }
        public string? TenChiTieuNguon { get; set; }
        public string? MaThongSoThietBi { get; set; }
    }

    public class CreateChiTieuInputDto
    {
        public int     ID_ChiTieu { get; set; }
        public string  MaInput    { get; set; } = string.Empty;
        public string  TenInput   { get; set; } = string.Empty;
        public string  NguonGiaTri     { get; set; } = "MANUAL";
        public int?    ID_ChiTieuNguon { get; set; }
        public string? MaThongSoThietBi { get; set; }
    }

    public class UpdateChiTieuInputDto
    {
        public int     ID_ChiTieu { get; set; }
        public string  MaInput    { get; set; } = string.Empty;
        public string  TenInput   { get; set; } = string.Empty;
        public string  NguonGiaTri     { get; set; } = "MANUAL";
        public int?    ID_ChiTieuNguon { get; set; }
        public string? MaThongSoThietBi { get; set; }
    }
}
