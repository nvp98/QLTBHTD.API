using NCalc;
using PM_QLTBHTD.Domain.Entities;

namespace PM_QLTBHTD.Application.Helpers
{
    internal static class NguongEvaluator
    {
        /// <summary>
        /// Đánh giá biểu thức logic NCalc với tập biến được truyền vào.
        /// Biến không có trong dict mặc định = 0. Trả về false khi có exception.
        /// </summary>
        internal static bool EvalNCalc(string bieuThuc, Dictionary<string, decimal> vars)
        {
            try
            {
                var expr = new Expression(bieuThuc);
                expr.EvaluateParameter += (name, args) =>
                {
                    args.Result = vars.TryGetValue(name, out var val) ? (double)val : 0d;
                };
                return (bool)expr.Evaluate();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"[NguongEvaluator] EvalNCalc lỗi biểu thức '{bieuThuc}': {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Kiểm tra giá trị đơn có nằm trong khoảng [CanDuoi, CanTren] của ngưỡng không.
        /// Trả về false nếu giaTri là null.
        /// </summary>
        internal static bool KiemTraRange(decimal? giaTri, CBM_Nguong ng)
        {
            if (giaTri is null)
                return false;

            var v = giaTri.Value;

            if (ng.CanDuoi is not null)
            {
                bool duoi = ng.CanDuoi_BaoGom ? v >= ng.CanDuoi.Value : v > ng.CanDuoi.Value;
                if (!duoi) return false;
            }

            if (ng.CanTren is not null)
            {
                bool tren = ng.CanTren_BaoGom ? v <= ng.CanTren.Value : v < ng.CanTren.Value;
                if (!tren) return false;
            }

            return true;
        }

        /// <summary>
        /// Kiểm tra giá trị đo đơn có khớp ngưỡng không.
        /// Nếu ngưỡng có BieuThuc_Logic, đánh giá NCalc với biến "val" = giá trị đo.
        /// Nếu không, dùng so sánh range CanDuoi/CanTren thông thường.
        /// Quy ước: biểu thức phải dùng tên biến "val" để tham chiếu giá trị đo.
        /// </summary>
        internal static bool KiemTraNguongVoiGiaTri(decimal? giaTri, CBM_Nguong ng)
        {
            if (giaTri is null)
                return false;

            if (!string.IsNullOrWhiteSpace(ng.BieuThuc_Logic))
            {
                var vars = new Dictionary<string, decimal> { ["val"] = giaTri.Value };
                return EvalNCalc(ng.BieuThuc_Logic, vars);
            }

            return KiemTraRange(giaTri, ng);
        }

        /// <summary>
        /// Kiểm tra ngưỡng khi chỉ tiêu có nhiều biến đầu vào.
        /// BẮTBUỘC phải có BieuThuc_Logic trong ngưỡng — ném InvalidOperationException nếu rỗng.
        /// Key trong vars phải khớp chính xác tên biến trong BieuThuc_Logic.
        /// KHÔNG fallback sang KiemTraRange để lỗi cấu hình lộ ra ngay.
        /// </summary>
        internal static bool KiemTraNguongVoiGiaTri(Dictionary<string, decimal> vars, CBM_Nguong ng)
        {
            if (string.IsNullOrWhiteSpace(ng.BieuThuc_Logic))
                throw new InvalidOperationException(
                    $"Ngưỡng ID={ng.ID_Nguong} (ChiTieu={ng.ID_ChiTieu}) không có BieuThuc_Logic " +
                    "nhưng chỉ tiêu này yêu cầu nhiều biến đầu vào. Vui lòng cấu hình BieuThuc_Logic.");

            return EvalNCalc(ng.BieuThuc_Logic, vars);
        }
    }
}
