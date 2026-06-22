namespace PM_QLTBHTD.Application.DTOs
{
    public class NhomChiTieuDto
    {
        public int ID_NhomChiTieu { get; set; }
        public string TenNhom { get; set; } = string.Empty;
        public int ID_LoaiThietBi { get; set; }
        public string TenLoaiThietBi { get; set; } = string.Empty;
        public int? ID_NhomCha { get; set; }
        public int CapDo { get; set; }
        public string LoaiNhom { get; set; } = "LEAF";
        public int PhienBan { get; set; }
        public int TrangThai { get; set; }
        public bool CoCongThuc { get; set; }
    }

    /// <summary>Node trong cây NhomChiTieu — bao gồm các node con đệ quy.</summary>
    public class NhomChiTieuCayDto : NhomChiTieuDto
    {
        public List<NhomChiTieuCayDto> NhomCon { get; set; } = [];
    }

    public class CreateNhomChiTieuDto
    {
        public string TenNhom { get; set; } = string.Empty;
        public int ID_LoaiThietBi { get; set; }
        public int? ID_NhomCha { get; set; }
        public int CapDo { get; set; } = 1;
        public string LoaiNhom { get; set; } = "LEAF";
        public int PhienBan { get; set; } = 1;
        public int TrangThai { get; set; } = 1;
    }

    public class UpdateNhomChiTieuDto
    {
        public string TenNhom { get; set; } = string.Empty;
        public int ID_LoaiThietBi { get; set; }
        public int? ID_NhomCha { get; set; }
        public int CapDo { get; set; }
        public string LoaiNhom { get; set; } = "LEAF";
        public int PhienBan { get; set; }
        public int TrangThai { get; set; }
    }
}
