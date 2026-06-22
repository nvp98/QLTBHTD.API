using System.ComponentModel.DataAnnotations;

namespace PM_QLTBHTD.Domain.Entities
{
    /// <summary>
    /// Kết quả phân loại từng tháng đo cho chỉ tiêu kiểu LF.
    /// Tự sinh khi nhập số liệu. Dùng để COUNT theo mức khi tổng hợp điểm.
    /// </summary>
    public class CBM_KetQuaPhanLoaiThang
    {
        [Key]
        public int ID { get; set; }

        public int IDPhieu { get; set; }
        public int ID_ChiTieu { get; set; }

        public int Nam { get; set; }
        public int Thang { get; set; }

        public decimal GiaTriDo { get; set; }

        /// <summary>Mã mức đã phân loại, ví dụ: N0, N1, N2.</summary>
        public string MaMuc { get; set; } = string.Empty;

        public decimal TrongSo { get; set; }
    }
}
