using System.ComponentModel.DataAnnotations;

namespace PM_QLTBHTD.Domain.Entities
{
    /// <summary>
    /// Công thức tổng hợp cho nhóm LoaiNhom='COMPOSITE'.
    /// Mỗi nhóm COMPOSITE phải có đúng 1 dòng TrangThai=1 (ACTIVE) tại một thời điểm.
    /// Sửa công thức = tạo dòng mới (TrangThai=1) + cập nhật dòng cũ (TrangThai=0).
    /// </summary>
    public class CBM_CongThucTongHop
    {
        [Key]
        public int ID_CongThuc { get; set; }

        /// <summary>FK logic tới CBM_NhomChiTieu (LoaiNhom='COMPOSITE'). Không dùng DB FK.</summary>
        public int ID_NhomChiTieu { get; set; }

        /// <summary>Biểu thức NCalc tự do. Các biến tham chiếu khai báo trong CBM_CongThuc_Bien.</summary>
        public string BieuThuc { get; set; } = string.Empty;

        /// <summary>Mô tả loại công thức (gợi ý UI): WEIGHTED_AVG / WEIGHTED_AVG_SCALED / LINEAR_COMBINE / PRODUCT / CUSTOM_NCALC.</summary>
        public string LoaiCongThuc { get; set; } = "CUSTOM_NCALC";

        public int PhienBan { get; set; } = 1;

        /// <summary>1 = đang dùng (ACTIVE); 0 = đã thay thế.</summary>
        public int TrangThai { get; set; } = 1;

        public decimal? ThangDiem_Min { get; set; }
        public decimal? ThangDiem_Max { get; set; }

        public string? MoTa { get; set; }
    }
}
