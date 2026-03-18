namespace PM_QLTBHTD.Application.DTOs
{
    public class ChiTieuDto
    {
        public int ID_ChiTieu { get; set; }
        public int ID_NhomChiTieu { get; set; }
        public string TenChiTieu { get; set; } = string.Empty;
        public decimal TrongSo_Wi { get; set; }
        public int TrangThai { get; set; }
    }

    public class CreateChiTieuDto
    {
        public int ID_NhomChiTieu { get; set; }
        public string TenChiTieu { get; set; } = string.Empty;
        public decimal TrongSo_Wi { get; set; }
        public int TrangThai { get; set; } = 1;
    }

    public class UpdateChiTieuDto
    {
        public int ID_NhomChiTieu { get; set; }
        public string TenChiTieu { get; set; } = string.Empty;
        public decimal TrongSo_Wi { get; set; }
        public int TrangThai { get; set; }
    }
}
