using System;
using System.Collections.Generic;
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
                expr.EvaluateParameter += (string name, ParameterArgs args) =>
                {
                    args.Result = vars.TryGetValue(name, out var val) ? (object)(double)val : (object)0d;
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
        /// Ví dụ: "val >= 0.9 * 220 && val <= 1.1 * 220"
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
    }
}
