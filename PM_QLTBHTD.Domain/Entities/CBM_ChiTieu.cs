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
        public string? LoaiTinhDiem { get; set; }
        /// <summary>Ngưỡng L1 (ppm) — chỉ dùng khi LoaiTinhDiem='TOC_DO_SINH_KHI', làm mẫu số quy đổi
        /// % tốc độ sinh khí/năm theo IEEE C57.104 (Bảng 8, QT.40 mục 17.3.7).</summary>
        public decimal? GiaTri_L1 { get; set; }

    }
}
