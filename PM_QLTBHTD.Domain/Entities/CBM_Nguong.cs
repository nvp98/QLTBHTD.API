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
    }
}
