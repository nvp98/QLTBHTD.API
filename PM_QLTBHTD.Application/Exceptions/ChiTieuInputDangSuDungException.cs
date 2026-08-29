namespace PM_QLTBHTD.Application.Exceptions
{
    public class ChiTieuInputDangSuDungException : Exception
    {
        public int IdInput { get; }
        public int SoDuLieuDaNhap { get; }

        public ChiTieuInputDangSuDungException(int idInput, int soDuLieuDaNhap)
            : base($"Không thể xóa — biến input này đã có {soDuLieuDaNhap} giá trị được nhập trong các phiếu " +
                   "kiểm tra. Xóa dữ liệu đó trước khi xóa khỏi danh mục.")
        {
            IdInput = idInput;
            SoDuLieuDaNhap = soDuLieuDaNhap;
        }
    }
}
