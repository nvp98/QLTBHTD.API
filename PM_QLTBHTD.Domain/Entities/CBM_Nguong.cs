using System.ComponentModel.DataAnnotations;

namespace PM_QLTBHTD.Domain.Entities
{
    public class CBM_Nguong
    {
        [Key]
        public int ID_Nguong { get; set; }
        public int ID_ChiTieu { get; set; }

        public decimal? CanTren { get; set; }
        public decimal? CanDuoi { get; set; }
        public decimal? Diem_Si { get; set; }

        /// <summary>true = bao gồm đầu mút dưới (≥), false = không bao gồm (>). Default: true</summary>
        public bool CanDuoi_BaoGom { get; set; } = true;
        /// <summary>true = bao gồm đầu mút trên (≤), false = không bao gồm (&lt;). Default: false</summary>
        public bool CanTren_BaoGom { get; set; } = false;

        /// <summary>Biểu thức logic NCalc (tham chiếu biến bằng tên key). Ưu tiên hơn kiểm tra range khi khác null/rỗng.</summary>
        public string? BieuThuc_Logic { get; set; }

        /// <summary>Thứ tự ưu tiên khi nhiều dòng Nguong cùng một ChiTieu (số nhỏ = kiểm tra trước).</summary>
        public int ThuTu { get; set; } = 0;

        /// <summary>
        /// Khi Chỉ tiêu có CBM_ChiTieu_Formula: tên MaKetQua của Formula mà ngưỡng này áp dụng
        /// (1 Chỉ tiêu có thể có nhiều Formula, mỗi Formula 1 bảng ngưỡng riêng).
        /// NULL = áp trực tiếp lên GiaTriNhap_So (Chỉ tiêu không có Formula) — tương thích ngược.
        /// </summary>
        public string? MaKetQua { get; set; }

        /// <summary>Hành động khuyến cáo khi chỉ tiêu rơi vào mức này (VD "Tăng tần suất kiểm tra 6 tháng/lần").</summary>
        public string? HanhDongKhuyenCao { get; set; }
    }
}
