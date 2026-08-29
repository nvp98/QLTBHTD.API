namespace PM_QLTBHTD.Application.DTOs
{
    public class ChiTieuFormulaDto
    {
        public int ID_Formula { get; set; }
        public int ID_ChiTieu { get; set; }
        public string MaKetQua { get; set; } = string.Empty;
        public int ThuTu { get; set; }
        public string LoaiFormula { get; set; } = "NCALC";
        public string? BieuThuc { get; set; }
        public string? TenFunction { get; set; }
        public int TrangThai { get; set; } = 1;
        public string? MoTa { get; set; }
    }

    public class CreateChiTieuFormulaDto
    {
        public int ID_ChiTieu { get; set; }
        public string MaKetQua { get; set; } = string.Empty;
        public int ThuTu { get; set; }
        public string LoaiFormula { get; set; } = "NCALC";
        public string? BieuThuc { get; set; }
        public string? TenFunction { get; set; }
        public int TrangThai { get; set; } = 1;
        public string? MoTa { get; set; }
    }

    public class UpdateChiTieuFormulaDto : CreateChiTieuFormulaDto { }
}
