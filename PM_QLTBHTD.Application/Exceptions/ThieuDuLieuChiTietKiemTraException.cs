namespace PM_QLTBHTD.Application.Exceptions
{
    public class ThieuDuLieuChiTietKiemTraException : Exception
    {
        public int IdChiTieu { get; }
        public int IdPhieu { get; }

        public ThieuDuLieuChiTietKiemTraException(int idChiTieu, int idPhieu)
            : base($"Chỉ tiêu ID={idChiTieu} chưa có Diem_Si_DatDuoc trong phiếu ID={idPhieu}. " +
                   "Hãy nhập số liệu và lưu phiếu trước khi tính điểm nhóm.")
        {
            IdChiTieu = idChiTieu;
            IdPhieu = idPhieu;
        }
    }
}
