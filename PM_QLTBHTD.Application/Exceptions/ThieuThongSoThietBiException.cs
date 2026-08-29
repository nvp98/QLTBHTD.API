namespace PM_QLTBHTD.Application.Exceptions
{
    public class ThieuThongSoThietBiException : Exception
    {
        public int IdThietBi { get; }
        public string MaThongSo { get; }

        public ThieuThongSoThietBiException(int idThietBi, string maThongSo)
            : base($"Thiết bị ID={idThietBi} chưa cấu hình thông số kỹ thuật '{maThongSo}'. " +
                   "Vui lòng nhập tại trang Quản lý thiết bị trước khi lập phiếu.")
        {
            IdThietBi = idThietBi;
            MaThongSo = maThongSo;
        }
    }
}
