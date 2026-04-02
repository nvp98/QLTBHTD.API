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

    }
}
