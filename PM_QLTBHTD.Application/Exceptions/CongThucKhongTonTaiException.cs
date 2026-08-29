namespace PM_QLTBHTD.Application.Exceptions
{
    public class CongThucKhongTonTaiException : Exception
    {
        public int IdNhomChiTieu { get; }

        public CongThucKhongTonTaiException(int idNhomChiTieu)
            : base($"Nhóm chỉ tiêu ID={idNhomChiTieu} chưa có công thức tổng hợp ACTIVE. Vui lòng cấu hình trước khi tính điểm.")
        {
            IdNhomChiTieu = idNhomChiTieu;
        }
    }
}
