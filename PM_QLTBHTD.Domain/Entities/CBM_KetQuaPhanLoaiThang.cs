using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PM_QLTBHTD.Domain.Entities
{
    /// <summary>
    /// Kết quả phân loại từng tháng cho 1 chỉ tiêu kiểu LF trong 1 phiếu kiểm tra —
    /// lưu lại mức (N0..N4) và trọng số đã khớp, dùng làm bằng chứng/audit cho LF tính được.
    /// </summary>
    public class CBM_KetQuaPhanLoaiThang
    {
        [Key]
        [Column("ID")]
        public int ID_KetQua { get; set; }

        public int IDPhieu { get; set; }
        public int ID_ChiTieu { get; set; }

        public int Nam { get; set; }
        public int Thang { get; set; }
        public decimal GiaTriDo { get; set; }

        public string MaMuc { get; set; } = string.Empty;
        public decimal TrongSo { get; set; }
    }
}
