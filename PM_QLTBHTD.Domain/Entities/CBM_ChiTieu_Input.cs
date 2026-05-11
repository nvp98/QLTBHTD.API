using System.ComponentModel.DataAnnotations;

namespace PM_QLTBHTD.Domain.Entities
{
    public class CBM_ChiTieu_Input
    {
        [Key]
        public int ID_Input { get; set; }
        public int ID_ChiTieu { get; set; }
        public string MaInput { get; set; }
        public string TenInput { get; set; }
    }
}
