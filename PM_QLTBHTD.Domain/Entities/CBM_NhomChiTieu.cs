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

        /// <summary>
        /// Trọng số CANONICAL của nhóm khi được tham chiếu làm biến NHOM_CON trong công thức
        /// tổng hợp của nhóm cha (vd "Chất lượng dầu" Wi=6 khi tham gia TS1). Dùng làm fallback
        /// khi CBM_CongThuc_Bien.TrongSo (override riêng cho 1 công thức) để trống.
        /// </summary>
        public decimal? TrongSo_Wi { get; set; }
    }
}
