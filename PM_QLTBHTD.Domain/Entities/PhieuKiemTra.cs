using System.ComponentModel.DataAnnotations;

namespace PM_QLTBHTD.Domain.Entities
{
    public class PhieuKiemTra
    {
        [Key]
        public int ID_Phieu { get; set; }
        public string? SoPhieu { get; set; }
        public int ID_ThietBi { get; set; }

        /// <summary>Nhóm chỉ tiêu được đo trong phiếu này. NULL = kiểm tra toàn diện tất cả nhóm.</summary>
        public int? ID_NhomChiTieu { get; set; }

        public DateTime NgayKiemTra { get; set; }
        public string? NguoiKiemTra { get; set; }

        public decimal? TongDiem_Soqt { get; set; }
        public string? CapDoCanhBao { get; set; }

        public string? GhiChuChung { get; set; }
    }
}
