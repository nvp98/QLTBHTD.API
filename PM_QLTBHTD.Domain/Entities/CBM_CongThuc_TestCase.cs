using System.ComponentModel.DataAnnotations;

namespace PM_QLTBHTD.Domain.Entities
{
    /// <summary>
    /// Bộ test case mẫu (input giả lập + kết quả mong đợi) lưu lại cho 1 công thức tổng hợp —
    /// theo đề xuất "xac-nhan-chuan-hoa-kien-truc-cuoi.md" mục 2 (Formula Test + regression).
    /// Khi công thức bị sửa lại (đổi hệ số/trọng số), chạy lại toàn bộ test case đã lưu để phát
    /// hiện ngay nếu kết quả lệch so với trước, thay vì chỉ phát hiện khi có phiếu thật chạy vào.
    /// </summary>
    public class CBM_CongThuc_TestCase
    {
        [Key]
        public int ID_TestCase { get; set; }

        public int ID_CongThuc { get; set; }

        public string TenTestCase { get; set; } = string.Empty;

        /// <summary>JSON {"Sc":2.1,"St":3,"Sr":1} — giá trị giả lập cho từng biến khai trong công thức.</summary>
        public string InputJson { get; set; } = "{}";

        public decimal KetQuaMongDoi { get; set; }

        /// <summary>Cache lần chạy gần nhất — cập nhật mỗi khi "Chạy test".</summary>
        public decimal? KetQuaThucTeLanCuoi { get; set; }
        public bool? DatLanCuoi { get; set; }
        public DateTime? ThoiGianChayCuoi { get; set; }
        public string? LoiLanCuoi { get; set; }

        public string? MoTa { get; set; }
    }
}
