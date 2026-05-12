using System.ComponentModel.DataAnnotations;

namespace PM_QLTBHTD.Domain.Entities
{
    public class ChiTietKiemTra_Input
    {
        [Key]
        public int ID { get; set; }

        public int IDPhieu { get; set; }

        public int ID_ChiTieu { get; set; }

        /// <summary>FK tới CBM_ChiTieu_Input — dùng cho kiểu Rule có khai báo biến trong DB.</summary>
        public int? ID_Input { get; set; }

        /// <summary>Tên biến — dùng cho kiểu Nguong BieuThuc_Logic nhiều ẩn số (synthetic).</summary>
        public string? MaInput { get; set; }

        public decimal GiaTriSo { get; set; }
    }
}
