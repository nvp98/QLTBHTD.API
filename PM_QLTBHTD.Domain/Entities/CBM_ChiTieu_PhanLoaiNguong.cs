using System.ComponentModel.DataAnnotations;

namespace PM_QLTBHTD.Domain.Entities
{
    /// <summary>
    /// Mức phân loại theo tháng (N0..N4) cho chỉ tiêu kiểu LF (Load Factor, VD: mang tải MBA).
    /// Mỗi tháng đo được khớp vào 1 dòng theo khoảng [GiaTriTu, GiaTriDen] để lấy TrongSo.
    /// </summary>
    public class CBM_ChiTieu_PhanLoaiNguong
    {
        [Key]
        public int ID_PhanLoai { get; set; }

        public int ID_ChiTieu { get; set; }

        public string MaMuc { get; set; } = string.Empty;

        public decimal? GiaTriTu { get; set; }
        public decimal? GiaTriDen { get; set; }
        public bool GiaTriTu_BaoGom { get; set; }
        public bool GiaTriDen_BaoGom { get; set; }

        public decimal TrongSo { get; set; }
        public int ThuTu { get; set; }
    }
}
