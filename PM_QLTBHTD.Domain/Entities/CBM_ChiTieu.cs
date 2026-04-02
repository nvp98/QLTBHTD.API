using System.ComponentModel.DataAnnotations;

namespace PM_QLTBHTD.Domain.Entities
{
    public class CBM_ChiTieu
    {
        [Key]
        public int ID_ChiTieu { get; set; }
        public int ID_NhomChiTieu { get; set; }
        public string? TenChiTieu { get; set; } = string.Empty;
        public decimal? TrongSo_Wi { get; set; }
        public int TrangThai { get; set; }

    }
}
