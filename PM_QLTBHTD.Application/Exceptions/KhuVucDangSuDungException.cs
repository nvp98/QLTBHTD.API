namespace PM_QLTBHTD.Application.Exceptions
{
    public class KhuVucDangSuDungException : Exception
    {
        public int IdKhuVuc { get; }
        public int SoTramDangDung { get; }

        public KhuVucDangSuDungException(int idKhuVuc, int soTramDangDung)
            : base($"Không thể xóa — khu vực này đang có {soTramDangDung} trạm điện. " +
                   "Xóa hoặc chuyển các trạm đó sang khu vực khác trước khi xóa.")
        {
            IdKhuVuc = idKhuVuc;
            SoTramDangDung = soTramDangDung;
        }
    }
}
