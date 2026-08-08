using System.ComponentModel.DataAnnotations;

namespace PM_QLTBHTD.Domain.Entities
{
    public class CBM_ChiTieu_Rule
    {
        [Key]
        public int ID_Rule { get; set; }
        public int ID_ChiTieu { get; set; }
        public string TenMuc { get; set; }
        public decimal Diem_Si { get; set; }
        public string BieuThuc { get; set; }

        /// <summary>
        /// 'BANG_MUC' (mặc định) = BieuThuc là điều kiện boolean cho 1 mức điểm cố định (Diem_Si),
        /// nhiều dòng cùng ID_ChiTieu, khớp đầu tiên theo Diem_Si giảm dần thắng — hành vi gốc.
        /// 'CONG_THUC' = 1 dòng duy nhất, BieuThuc là biểu thức NCalc SỐ evaluate trực tiếp ra Si
        /// (vd 'Min(Si_DT1, Si_DT2)'), dùng để gộp Si của nhiều Formula/Threshold trong cùng Chỉ tiêu.
        /// </summary>
        public string LoaiRule { get; set; } = "BANG_MUC";

        /// <summary>Hành động khuyến cáo khi chỉ tiêu rơi vào mức này (VD "Tách MBA khỏi vận hành").</summary>
        public string? HanhDongKhuyenCao { get; set; }
    }

}
