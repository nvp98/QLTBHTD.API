namespace PM_QLTBHTD.Application.Exceptions
{
    public class LoaiThietBiDangSuDungException : Exception
    {
        public int IdLoaiThietBi { get; }
        public int SoThietBiDangDung { get; }
        public int SoNhomChiTieuDangDung { get; }

        public LoaiThietBiDangSuDungException(int idLoaiThietBi, int soThietBiDangDung, int soNhomChiTieuDangDung)
            : base($"Không thể xóa — loại thiết bị này đang có {soThietBiDangDung} thiết bị và {soNhomChiTieuDangDung} " +
                   "nhóm chỉ tiêu đã cấu hình. Xóa/chuyển các thiết bị và nhóm chỉ tiêu đó trước khi xóa khỏi danh mục.")
        {
            IdLoaiThietBi = idLoaiThietBi;
            SoThietBiDangDung = soThietBiDangDung;
            SoNhomChiTieuDangDung = soNhomChiTieuDangDung;
        }
    }
}
