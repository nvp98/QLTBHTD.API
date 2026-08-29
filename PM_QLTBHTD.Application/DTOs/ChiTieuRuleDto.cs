namespace PM_QLTBHTD.Application.DTOs
{
    public class ChiTieuRuleDto
    {
        public int     ID_Rule    { get; set; }
        public int     ID_ChiTieu { get; set; }
        public string  TenMuc     { get; set; } = string.Empty;
        public decimal Diem_Si    { get; set; }
        public string  BieuThuc   { get; set; } = string.Empty;
        /// <summary>'BANG_MUC' (mặc định, điều kiện boolean từng mức) hoặc 'CONG_THUC' (biểu thức NCalc số gộp nhiều Si, vd Min(Si_DT1,Si_DT2)).</summary>
        public string  LoaiRule   { get; set; } = "BANG_MUC";
        public string? HanhDongKhuyenCao { get; set; }
    }

    public class CreateChiTieuRuleDto
    {
        public int     ID_ChiTieu { get; set; }
        public string  TenMuc     { get; set; } = string.Empty;
        public decimal Diem_Si    { get; set; }
        public string  BieuThuc   { get; set; } = string.Empty;
        public string  LoaiRule   { get; set; } = "BANG_MUC";
        public string? HanhDongKhuyenCao { get; set; }
    }

    public class UpdateChiTieuRuleDto
    {
        public int     ID_ChiTieu { get; set; }
        public string  TenMuc     { get; set; } = string.Empty;
        public decimal Diem_Si    { get; set; }
        public string  BieuThuc   { get; set; } = string.Empty;
        public string  LoaiRule   { get; set; } = "BANG_MUC";
        public string? HanhDongKhuyenCao { get; set; }
    }
}
