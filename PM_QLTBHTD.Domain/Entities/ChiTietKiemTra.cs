using System.ComponentModel.DataAnnotations;

namespace PM_QLTBHTD.Domain.Entities
{
    public class ChiTietKiemTra
    {
        [Key]
        public int ID_ChiTiet { get; set; }
        public int IDPhieu { get; set; }
        public int ID_ChiTieu { get; set; }

        public decimal? GiaTriNhap_So { get; set; }
        public string? GiaTriNhap_Chu { get; set; }

        public decimal? Diem_Si_DatDuoc { get; set; }

        public string? GhiChu { get; set; }

    }
}
