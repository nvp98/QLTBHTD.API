using System.ComponentModel.DataAnnotations;

namespace PM_QLTBHTD.Domain.Entities
{
    /// <summary>Ngăn lộ trong trạm — nhóm thiết bị (1 MBA, 1 MC, 3 DCL, 3 TU, 3 TI...) cùng bị
    /// ngắt điện chung khi kiểm tra offline/chuyên sâu.</summary>
    public class CBM_NganLo
    {
        [Key]
        public int ID_NganLo { get; set; }
        public int ID_Tram { get; set; }

        public string TenNganLo { get; set; } = string.Empty;
        public string? MaNganLo { get; set; }
        public int TrangThai { get; set; }
    }
}
