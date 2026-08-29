using System.ComponentModel.DataAnnotations;

namespace PM_QLTBHTD.Domain.Entities
{
    public class CBM_TramDien
    {
        [Key]
        public int IDTram { get; set; }
        public int IDKhuVuc { get; set; }

        public string TenTram { get; set; } = string.Empty;
        public string? DiaDiem { get; set; }
        public int TrangThai { get; set; }
    }
}
