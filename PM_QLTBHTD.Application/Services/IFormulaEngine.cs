namespace PM_QLTBHTD.Application.Services
{
    public interface IFormulaEngine
    {
        /// <summary>
        /// Evaluate toàn bộ CBM_ChiTieu_Formula của 1 Chỉ tiêu, theo thứ tự ThuTu tăng dần
        /// (formula sau được phép dùng kết quả formula trước — chaining). Formula thuần tính
        /// toán, KHÔNG chấm điểm. Trả về Dictionary rỗng nếu Chỉ tiêu không có Formula nào
        /// (chỉ tiêu "cổ điển" — Threshold/Rule áp trực tiếp lên GiaTriNhap_So như trước giờ).
        /// </summary>
        Task<Dictionary<string, decimal>> EvaluateAllAsync(
            int idChiTieu,
            int idPhieu,
            int idThietBi,
            Dictionary<string, decimal> inputValues,
            CancellationToken ct = default);
    }
}
