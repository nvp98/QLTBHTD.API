namespace PM_QLTBHTD.Application.DTOs
{
    public class NhomChiTieuDto
    {
        public int ID_NhomChiTieu { get; set; }
        public string TenNhom { get; set; } = string.Empty;
        public int ID_LoaiThietBi { get; set; }
        public int PhienBan { get; set; }
        public int TrangThai { get; set; }
    }

    public class CreateNhomChiTieuDto
    {
        public string TenNhom { get; set; } = string.Empty;
        public int ID_LoaiThietBi { get; set; }
        public int PhienBan { get; set; } = 1;
        public int TrangThai { get; set; } = 1;
    }

    public class UpdateNhomChiTieuDto
    {
        public string TenNhom { get; set; } = string.Empty;
        public int ID_LoaiThietBi { get; set; }
        public int PhienBan { get; set; }
        public int TrangThai { get; set; }
    }
}
