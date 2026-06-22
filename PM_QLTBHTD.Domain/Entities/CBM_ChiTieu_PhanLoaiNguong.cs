using System.ComponentModel.DataAnnotations;

namespace PM_QLTBHTD.Domain.Entities
{
    /// <summary>
    /// Định nghĩa các mức phân loại cho chỉ tiêu kiểu đếm-theo-tháng (ví dụ LF — Load Factor).
    /// Mỗi mức có khoảng giá trị và trọng số/điểm riêng.
    /// </summary>
    public class CBM_ChiTieu_PhanLoaiNguong
    {
        [Key]
        public int ID_PhanLoai { get; set; }

        public int ID_ChiTieu { get; set; }

        /// <summary>Mã mức, ví dụ: N0, N1, N2, N3, N4.</summary>
        public string MaMuc { get; set; } = string.Empty;

        public decimal? GiaTriTu { get; set; }
        public decimal? GiaTriDen { get; set; }
        public bool GiaTriTu_BaoGom { get; set; } = true;
        public bool GiaTriDen_BaoGom { get; set; } = false;

        /// <summary>Điểm/trọng số áp dụng cho tháng nào rơi vào mức này.</summary>
        public decimal TrongSo { get; set; }

        public int ThuTu { get; set; } = 0;
    }
}
