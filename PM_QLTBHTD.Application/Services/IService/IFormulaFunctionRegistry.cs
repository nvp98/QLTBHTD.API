namespace PM_QLTBHTD.Application.Services.IService
{
    /// <summary>
    /// Đăng ký các hàm C# đặt tên dùng cho CBM_ChiTieu_Formula.LoaiFormula='FUNCTION' —
    /// dùng khi logic quá phức tạp để viết thành 1 biểu thức NCalc (tra bảng nhiều chiều,
    /// vòng lặp...). Thêm hàm mới = đăng ký thêm 1 dòng trong FormulaFunctionRegistry, không
    /// đụng tới FormulaEngine.
    /// </summary>
    public interface IFormulaFunctionRegistry
    {
        decimal Invoke(string tenFunction, Dictionary<string, decimal> thamSo);
    }
}
