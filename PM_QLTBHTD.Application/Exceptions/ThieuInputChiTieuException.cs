namespace PM_QLTBHTD.Application.Exceptions
{
    public class ThieuInputChiTieuException : Exception
    {
        public int IdChiTieu { get; }
        public string MaInput { get; }

        public ThieuInputChiTieuException(int idChiTieu, string maInput)
            : base($"Chỉ tiêu ID={idChiTieu} yêu cầu biến '{maInput}' nhưng không tìm thấy giá trị tương ứng trong ChiTietKiemTra_Input. " +
                   "Nhập liệu không đủ số biến yêu cầu.")
        {
            IdChiTieu = idChiTieu;
            MaInput = maInput;
        }
    }
}
