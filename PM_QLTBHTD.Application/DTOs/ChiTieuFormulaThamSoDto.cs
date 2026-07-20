namespace PM_QLTBHTD.Application.DTOs
{
    public class ChiTieuFormulaThamSoDto
    {
        public int ID_ThamSo { get; set; }
        public int ID_Formula { get; set; }
        public string MaThamSo { get; set; } = string.Empty;
        public string NguonGiaTri { get; set; } = "HANGSO";
        public string? MaInput { get; set; }
        public int? ID_FormulaNguon { get; set; }
        public int? ID_ChiTieuNguon { get; set; }
        public string? TenThuocTinhTB { get; set; }
        public decimal? GiaTriHangSo { get; set; }
    }

    public class CreateChiTieuFormulaThamSoDto
    {
        public int ID_Formula { get; set; }
        public string MaThamSo { get; set; } = string.Empty;
        public string NguonGiaTri { get; set; } = "HANGSO";
        public string? MaInput { get; set; }
        public int? ID_FormulaNguon { get; set; }
        public int? ID_ChiTieuNguon { get; set; }
        public string? TenThuocTinhTB { get; set; }
        public decimal? GiaTriHangSo { get; set; }
    }

    public class UpdateChiTieuFormulaThamSoDto : CreateChiTieuFormulaThamSoDto { }
}
