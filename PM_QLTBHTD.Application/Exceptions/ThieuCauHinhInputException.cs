namespace PM_QLTBHTD.Application.Exceptions
{
    public class ThieuCauHinhInputException : Exception
    {
        public int IdChiTieu { get; }
        public string MaInput { get; }

        public ThieuCauHinhInputException(int idChiTieu, string maInput)
            : base($"Biến Input '{maInput}' của chỉ tiêu ID={idChiTieu} có NguonGiaTri không hợp lệ " +
                   "hoặc 'CHITIEU_CUNG_PHIEU' nhưng chưa chọn Chỉ tiêu nguồn. Vui lòng cấu hình lại.")
        {
            IdChiTieu = idChiTieu;
            MaInput = maInput;
        }
    }
}
