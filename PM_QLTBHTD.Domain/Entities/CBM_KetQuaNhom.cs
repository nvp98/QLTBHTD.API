using System.ComponentModel.DataAnnotations;

namespace PM_QLTBHTD.Domain.Entities
{
    /// <summary>
    /// Cache điểm đã tính theo từng (IDPhieu, ID_NhomChiTieu).
    /// Upsert mỗi khi ScoringEngine tính xong — đọc cache trước khi tính lại.
    /// </summary>
    public class CBM_KetQuaNhom
    {
        [Key]
        public int ID_KetQua { get; set; }

        public int IDPhieu { get; set; }
        public int ID_NhomChiTieu { get; set; }

        public decimal Diem { get; set; }

        /// <summary>JSON snapshot các biến đã bind lúc tính (dùng để audit/debug).</summary>
        public string? BienDaBind { get; set; }

        public DateTime ThoiGianTinh { get; set; } = DateTime.UtcNow;
    }
}
