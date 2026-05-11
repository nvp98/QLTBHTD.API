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
                    args.Result = vars.TryGetValue(name, out var val) ? (object)val : (object)0m;
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
    }
}
