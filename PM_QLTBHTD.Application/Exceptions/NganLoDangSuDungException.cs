namespace PM_QLTBHTD.Application.Exceptions
{
    public class NganLoDangSuDungException : Exception
    {
        public int IdNganLo { get; }
        public int SoThietBiDangDung { get; }

        public NganLoDangSuDungException(int idNganLo, int soThietBiDangDung)
            : base($"Không thể xóa — ngăn lộ này đang có {soThietBiDangDung} thiết bị. " +
                   "Xóa hoặc chuyển các thiết bị đó sang ngăn lộ khác trước khi xóa.")
        {
            IdNganLo = idNganLo;
            SoThietBiDangDung = soThietBiDangDung;
        }
    }
}
