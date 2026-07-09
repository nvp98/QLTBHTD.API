namespace PM_QLTBHTD.Application.Exceptions
{
    public class ThieuPhanLoaiNguongException : Exception
    {
        public int IdChiTieu { get; }

        public ThieuPhanLoaiNguongException(int idChiTieu)
            : base($"Chỉ tiêu ID={idChiTieu} có LoaiTinhDiem='LF' nhưng chưa cấu hình mức phân loại " +
                   "(CBM_ChiTieu_PhanLoaiNguong). Vui lòng khai báo các mức N0..N4 trước khi nhập số liệu.")
        {
            IdChiTieu = idChiTieu;
        }
    }
}
