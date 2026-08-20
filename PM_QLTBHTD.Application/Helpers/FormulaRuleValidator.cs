namespace PM_QLTBHTD.Application.Helpers
{
    /// <summary>
    /// Config Validator — ngang hàng NguongValidator/CongThucValidator: chặn SỚM, ngay lúc lưu
    /// cấu hình, trạng thái "≥2 CBM_ChiTieu_Formula đang hoạt động nhưng 0 CBM_ChiTieu_Rule" cho
    /// 1 chỉ tiêu. ChiTieuScoringService.TinhDiemTuFormulaAsync ném LoiFormulaException đúng lúc
    /// này (không Rule CONG_THUC, không Rule BANG_MUC, và có &gt;1 kết quả Formula cần gộp) — lỗi
    /// đó bị catch riêng ở tầng chỉ tiêu trong TinhVaLuuDiemSiAsync nên chỉ in ra Console.Error
    /// (server log), Sᵢ lặng lẽ thành null, người tạo phiếu không thấy gì. Validator này giúp lộ
    /// lỗi cấu hình ra NGAY lúc soạn Formula/Rule, thay vì chỉ phát hiện khi có phiếu thật chạy vào.
    /// </summary>
    public static class FormulaRuleValidator
    {
        public static bool SeThieuRuleGopFormula(int soFormulaActive, int soRule)
            => soFormulaActive >= 2 && soRule == 0;
    }
}
