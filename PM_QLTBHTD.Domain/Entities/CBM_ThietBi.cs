using System.ComponentModel.DataAnnotations;

namespace PM_QLTBHTD.Domain.Entities
{
    public class CBM_ThietBi
    {
        [Key]
        public int ID_ThietBi { get; set; }
        public int ID_Tram { get; set; }
        public int ID_LoaiTB { get; set; }

        public string TenThietBi { get; set; } = string.Empty;
        public string? SoHieu { get; set; }
        public string? NhanHieu { get; set; }

        public int? NamSanXuat { get; set; }
        public int TrangThai { get; set; }
        public string? GhiChu { get; set; }

    }
}
