using System.ComponentModel.DataAnnotations;

namespace PM_QLTBHTD.Domain.Entities
{
    public class CBM_KhuVuc
    {
        [Key]
        public int ID_KhuVuc { get; set; }
        public string TenKhuVuc { get; set; } = string.Empty;
        public int TrangThai { get; set; }
    }
}
