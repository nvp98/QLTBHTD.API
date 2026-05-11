namespace PM_QLTBHTD.Application.DTOs
{
    public class ChiTieuInputDto
    {
        public int     ID_Input   { get; set; }
        public int     ID_ChiTieu { get; set; }
        public string  MaInput    { get; set; } = string.Empty;
        public string  TenInput   { get; set; } = string.Empty;
    }

    public class CreateChiTieuInputDto
    {
        public int     ID_ChiTieu { get; set; }
        public string  MaInput    { get; set; } = string.Empty;
        public string  TenInput   { get; set; } = string.Empty;
    }

    public class UpdateChiTieuInputDto
    {
        public int     ID_ChiTieu { get; set; }
        public string  MaInput    { get; set; } = string.Empty;
        public string  TenInput   { get; set; } = string.Empty;
    }
}
