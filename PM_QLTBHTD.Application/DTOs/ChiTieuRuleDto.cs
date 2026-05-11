namespace PM_QLTBHTD.Application.DTOs
{
    public class ChiTieuRuleDto
    {
        public int     ID_Rule    { get; set; }
        public int     ID_ChiTieu { get; set; }
        public string  TenMuc     { get; set; } = string.Empty;
        public decimal Diem_Si    { get; set; }
        public string  BieuThuc   { get; set; } = string.Empty;
    }

    public class CreateChiTieuRuleDto
    {
        public int     ID_ChiTieu { get; set; }
        public string  TenMuc     { get; set; } = string.Empty;
        public decimal Diem_Si    { get; set; }
        public string  BieuThuc   { get; set; } = string.Empty;
    }

    public class UpdateChiTieuRuleDto
    {
        public int     ID_ChiTieu { get; set; }
        public string  TenMuc     { get; set; } = string.Empty;
        public decimal Diem_Si    { get; set; }
        public string  BieuThuc   { get; set; } = string.Empty;
    }
}
