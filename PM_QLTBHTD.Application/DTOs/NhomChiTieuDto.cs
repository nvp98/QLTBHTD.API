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
        /// <summary>1=Online, 2=Offline, 3=Chuyên sâu (CBM EVNCPC-KT/QT.40). NULL = nhóm tổng hợp thuần.</summary>
        public int? Tier { get; set; }
        public bool CoCongThuc { get; set; }
        /// <summary>Trọng số canonical của nhóm khi tham gia công thức nhóm cha (NHOM_CON).</summary>
        public decimal? TrongSo_Wi { get; set; }
        /// <summary>Số chỉ tiêu đang hoạt động thuộc trực tiếp nhóm này — nhóm tổng hợp thuần (vd
        /// CHI1/TS1) luôn = 0 vì điểm chỉ gộp từ nhóm con, không có chỗ để nhập liệu trực tiếp.</summary>
        public int SoChiTieu { get; set; }
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
        public int? Tier { get; set; }
        public decimal? TrongSo_Wi { get; set; }
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
        public int? Tier { get; set; }
        public decimal? TrongSo_Wi { get; set; }
    }
}
