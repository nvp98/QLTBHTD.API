using System.ComponentModel.DataAnnotations;

namespace PM_QLTBHTD.Domain.Entities
{
    /// <summary>
    /// Audit trail cho các giá trị TRUNG GIAN (không phải Sᵢ cuối/CSSK tổng) phát sinh trong lúc
    /// tính điểm — vd LF (tỉ lệ mang tải), % tốc độ sinh khí, TDCG (tổng 6 khí), hoặc từng biến
    /// NCalc (Sc/St/Sr...) đã bind khi evaluate 1 công thức tổng hợp COMPOSITE. Ghi 1 lần mỗi khi
    /// tính lại (không upsert) để giữ lịch sử — phục vụ truy vết "vì sao kết quả kỳ này khác kỳ
    /// trước" mà không cần chạy lại toàn bộ pipeline.
    /// </summary>
    public class CBM_KetQuaTrungGian
    {
        [Key]
        public long ID_KetQua { get; set; }

        public int IDPhieu { get; set; }

        /// <summary>'CHITIEU' | 'NHOM'.</summary>
        public string LoaiPham { get; set; } = string.Empty;

        /// <summary>ID_ChiTieu hoặc ID_NhomChiTieu tuỳ LoaiPham.</summary>
        public int ID_Pham { get; set; }

        /// <summary>Tên định danh giá trị — vd 'LF', 'TOC_DO_SINH_KHI', 'TDCG', hoặc đúng tên biến
        /// NCalc trong công thức tổng hợp (vd 'Sc', 'St', 'Sr').</summary>
        public string MaKetQua { get; set; } = string.Empty;

        public decimal? GiaTri { get; set; }

        public string? Nhan { get; set; }

        public DateTime ThoiGianTinh { get; set; } = DateTime.UtcNow;
    }
}
