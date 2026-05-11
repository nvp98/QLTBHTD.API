namespace PM_QLTBHTD.Application.Services
{
    public interface INguongScoringService
    {
        /// <summary>
        /// Tìm điểm số phù hợp nhất cho một chỉ tiêu dựa trên danh sách ngưỡng.
        /// Ngưỡng có BieuThuc_Logic sẽ được đánh giá bằng NCalc với toàn bộ tập giá trị;
        /// ngưỡng không có biểu thức sẽ kiểm tra giaTriDon theo khoảng [CanDuoi, CanTren].
        /// </summary>
        /// <param name="idChiTieu">ID chỉ tiêu cần tra cứu ngưỡng.</param>
        /// <param name="giaTriDon">Giá trị đo lường đơn của chỉ tiêu (dùng khi không có biểu thức).</param>
        /// <param name="tatCaGiaTri">Toàn bộ giá trị các chỉ tiêu liên quan, key = tên biến NCalc.</param>
        /// <returns>Diem_Si của ngưỡng thỏa mãn đầu tiên (ưu tiên điểm cao nhất), hoặc null nếu không khớp.</returns>
        Task<decimal?> TinhDiemAsync(
            int idChiTieu,
            decimal? giaTriDon,
            Dictionary<string, decimal> tatCaGiaTri);
    }
}
