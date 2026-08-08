namespace PM_QLTBHTD.Application.Exceptions
{
    public class ThongSoDangSuDungException : Exception
    {
        public int IdThongSo { get; }
        public int SoThietBiDangDung { get; }

        public ThongSoDangSuDungException(int idThongSo, int soThietBiDangDung)
            : base($"Không thể xóa — thông số này đang được {soThietBiDangDung} thiết bị sử dụng. " +
                   "Xóa giá trị đã khai ở từng thiết bị trước khi xóa khỏi danh mục.")
        {
            IdThongSo = idThongSo;
            SoThietBiDangDung = soThietBiDangDung;
        }
    }
}
