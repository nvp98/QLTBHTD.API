using PM_QLTBHTD.Application.Services.IService;

namespace PM_QLTBHTD.Application.Services
{
    /// <summary>
    /// Chưa có hàm nghiệp vụ nào đăng ký — hiện tại toàn bộ Formula đã cấu hình (DT1, DT2...)
    /// dùng LoaiFormula='NCALC' là đủ. Đăng ký hàm mới khi có nhu cầu thật (vd tam giác Duval)
    /// bằng cách thêm 1 dòng vào _functions, không cần sửa FormulaEngine.
    /// </summary>
    public class FormulaFunctionRegistry : IFormulaFunctionRegistry
    {
        private static readonly Dictionary<string, Func<Dictionary<string, decimal>, decimal>> _functions = new();

        public decimal Invoke(string tenFunction, Dictionary<string, decimal> thamSo)
        {
            if (!_functions.TryGetValue(tenFunction, out var fn))
                throw new InvalidOperationException(
                    $"Function '{tenFunction}' chưa được đăng ký trong FormulaFunctionRegistry.");
            return fn(thamSo);
        }
    }
}
