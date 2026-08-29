using System.ComponentModel.DataAnnotations;

namespace PM_QLTBHTD.Domain.Entities
{
    public class CBM_LoaiThietBi
    {
        [Key]
        public int ID_LoaiThietBi { get; set; }
        public string TenLoaiTB { get; set; } = string.Empty;
        public string KyHieu { get; set; } = string.Empty;
        public int TrangThai { get; set; }
    }
}
