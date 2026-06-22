using System.ComponentModel.DataAnnotations;

namespace PM_QLTBHTD.Domain.Entities
{
    public class CBM_NhomChiTieu
    {
        [Key]
        public int ID_NhomChiTieu { get; set; }
        public string TenNhom { get; set; } = string.Empty;

        public int ID_LoaiThietBi { get; set; }
        public int PhienBan { get; set; }
        public int TrangThai { get; set; }

        /// <summary>ID nhóm cha trong cây phân cấp (null = gốc). Không dùng FK — logic layer kiểm tra.</summary>
        public int? ID_NhomCha { get; set; }

        /// <summary>Tầng trong cây: 1 = lá, số lớn hơn = tầng tổng hợp cao hơn.</summary>
        public int CapDo { get; set; } = 1;

        /// <summary>'LEAF' = có ChiTieu con trực tiếp; 'COMPOSITE' = gộp nhiều NhomChiTieu con.</summary>
        public string LoaiNhom { get; set; } = "LEAF";
    }
}
