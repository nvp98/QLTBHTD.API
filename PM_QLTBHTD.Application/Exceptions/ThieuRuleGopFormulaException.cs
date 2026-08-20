namespace PM_QLTBHTD.Application.Exceptions
{
    public class ThieuRuleGopFormulaException : Exception
    {
        public int IdChiTieu { get; }
        public int SoFormula { get; }

        public ThieuRuleGopFormulaException(int idChiTieu, int soFormula)
            : base($"Không thể lưu: Chỉ tiêu ID={idChiTieu} sẽ có {soFormula} Formula đang hoạt động " +
                   "nhưng chưa có Rule (CONG_THUC hoặc BANG_MUC) nào để gộp kết quả thành 1 Sᵢ — " +
                   "tính điểm sẽ LỖI khi chạy phiếu kiểm tra thật. Hãy thêm Rule trước, hoặc giữ tối đa 1 Formula.")
        {
            IdChiTieu = idChiTieu;
            SoFormula = soFormula;
        }
    }
}
