using System.ComponentModel.DataAnnotations;

namespace PM_QLTBHTD.Domain.Entities
{
    public class CBM_ChiTieu_Rule
    {
        [Key]
        public int ID_Rule { get; set; }
        public int ID_ChiTieu { get; set; }
        public string TenMuc { get; set; }
        public decimal Diem_Si { get; set; }
        public string BieuThuc { get; set; }
    }

}
