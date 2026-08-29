using System.ComponentModel.DataAnnotations;

namespace PM_QLTBHTD.Domain.Entities
{
    public class CBM_VaiTro
    {
        [Key]
        public int ID_VaiTro { get; set; }
        public string MaVaiTro { get; set; } = string.Empty;
        public string TenVaiTro { get; set; } = string.Empty;
        public string? MoTa { get; set; }
    }
}
