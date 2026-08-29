using System.ComponentModel.DataAnnotations;

namespace PM_QLTBHTD.Domain.Entities
{
    /// <summary>
    /// Tầng tính toán trung gian giữa Input và Threshold/Rule — thuần tính toán, KHÔNG chấm điểm.
    /// Ví dụ: DT1 = T_tren - T_duoi.
    /// </summary>
    public class CBM_ChiTieu_Formula
    {
        [Key]
        public int ID_Formula { get; set; }
        public int ID_ChiTieu { get; set; }

        /// <summary>Tên biến kết quả, dùng làm MaKetQua khi tra CBM_Nguong. Vd 'DT1', 'DT2', 'Soqt'.</summary>
        public string MaKetQua { get; set; } = string.Empty;

        /// <summary>Thứ tự evaluate — formula ThuTu lớn hơn được phép tham chiếu kết quả formula ThuTu nhỏ hơn (chaining).</summary>
        public int ThuTu { get; set; } = 0;

        /// <summary>'NCALC' (biểu thức số học NCalc) hoặc 'FUNCTION' (hàm C# đã đăng ký theo tên).</summary>
        public string LoaiFormula { get; set; } = "NCALC";

        /// <summary>Dùng khi LoaiFormula='NCALC'. Vd 'T_tren - T_duoi'.</summary>
        public string? BieuThuc { get; set; }

        /// <summary>Dùng khi LoaiFormula='FUNCTION'. Tên hàm trong IFormulaFunctionRegistry.</summary>
        public string? TenFunction { get; set; }

        public int TrangThai { get; set; } = 1;
        public string? MoTa { get; set; }
    }
}
