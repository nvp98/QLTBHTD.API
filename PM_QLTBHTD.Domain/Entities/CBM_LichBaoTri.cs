using System.ComponentModel.DataAnnotations;

namespace PM_QLTBHTD.Domain.Entities
{
    public class CBM_LichBaoTri
    {
        [Key]
        public int ID_LichBaoTri { get; set; }
        public int ID_ThietBi { get; set; }

        /// <summary>Trỏ tới bản ghi gốc của chuỗi định kỳ (NULL = lịch gốc hoặc lịch đột xuất).</summary>
        public int? ID_LichBaoTriGoc { get; set; }

        /// <summary>"DinhKy" | "DotXuat".</summary>
        public string LoaiBaoTri { get; set; } = string.Empty;

        /// <summary>Chu kỳ lặp lại (số tháng). NULL = lịch đột xuất, không tự sinh lịch kế tiếp.</summary>
        public int? ChuKyThang { get; set; }

        public DateTime NgayKeHoach { get; set; }
        public DateTime? NgayThucHien { get; set; }

        /// <summary>"ChoThucHien" | "HoanThanh" | "DaHuy". Quá hạn/Sắp đến hạn được suy ra ở tầng DTO.</summary>
        public string TrangThai { get; set; } = "ChoThucHien";

        public string? NguoiPhuTrach { get; set; }
        public string? NoiDungCongViec { get; set; }
        public string? GhiChu { get; set; }

        /// <summary>Phiếu kiểm tra tạo ra sau khi hoàn thành bảo trì (tuỳ chọn, không bắt buộc).</summary>
        public int? ID_PhieuKetQua { get; set; }

        public DateTime NgayTao { get; set; } = DateTime.Now;
    }
}
