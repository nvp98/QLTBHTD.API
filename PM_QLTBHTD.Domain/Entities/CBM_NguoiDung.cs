using System.ComponentModel.DataAnnotations;

namespace PM_QLTBHTD.Domain.Entities
{
    public class CBM_NguoiDung
    {
        [Key]
        public int ID_NguoiDung { get; set; }
        public string TenDangNhap { get; set; } = string.Empty;
        public string MatKhau_Hash { get; set; } = string.Empty;
        public string HoTen { get; set; } = string.Empty;
        public string? Email { get; set; }
        public int ID_VaiTro { get; set; }

        /// <summary>Phạm vi trạm phụ trách — NULL = không giới hạn (Admin/GiamDoc/KySuCauHinh).
        /// Chưa dùng để lọc dữ liệu ở bản đầu, để dành cho phase sau (station-scoping).</summary>
        public int? ID_Tram { get; set; }

        /// <summary>1 = hoạt động, 0 = đã khóa.</summary>
        public int TrangThai { get; set; } = 1;
        public DateTime NgayTao { get; set; } = DateTime.Now;
    }
}
