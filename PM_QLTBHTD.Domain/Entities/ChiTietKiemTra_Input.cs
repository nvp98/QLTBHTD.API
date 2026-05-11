using System.ComponentModel.DataAnnotations;

namespace PM_QLTBHTD.Domain.Entities
{
    public class ChiTietKiemTra_Input
    {
        [Key]
        public int ID { get; set; }

        public int IDPhieu { get; set; }
        public int ID_ChiTieu { get; set; }
        public decimal GiaTriSo { get; set; }
    }
}
