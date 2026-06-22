namespace PM_QLTBHTD.Application.Exceptions
{
    public class ThieuBieuThucNguongException : Exception
    {
        public int IdChiTieu { get; }

        public ThieuBieuThucNguongException(int idChiTieu)
            : base($"Chỉ tiêu ID={idChiTieu} có khai báo Input nhiều biến nhưng không có Nguong nào có BieuThuc_Logic. " +
                   "Vui lòng cấu hình BieuThuc_Logic trong CBM_Nguong.")
        {
            IdChiTieu = idChiTieu;
        }
    }
}
