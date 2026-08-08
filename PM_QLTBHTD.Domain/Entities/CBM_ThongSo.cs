using System.ComponentModel.DataAnnotations;

namespace PM_QLTBHTD.Domain.Entities
{
    /// <summary>
    /// Danh mục thông số dùng chung cho nhiều thiết bị.
    /// </summary>
    public class CBM_ThongSo
    {
        [Key]
        public int ID_ThongSo { get; set; }

        public string MaThongSo { get; set; } = string.Empty;

        public string TenThongSo { get; set; } = string.Empty;

        public string? DonVi { get; set; }

        public string LoaiDuLieu { get; set; } = "DECIMAL";

        public int TrangThai { get; set; } = 1;

        public string? GhiChu { get; set; }
    }
}