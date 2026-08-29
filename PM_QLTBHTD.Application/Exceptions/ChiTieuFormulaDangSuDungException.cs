namespace PM_QLTBHTD.Application.Exceptions
{
    public class ChiTieuFormulaDangSuDungException : Exception
    {
        public int IdFormula { get; }
        public int SoThamSoThamChieu { get; }

        public ChiTieuFormulaDangSuDungException(int idFormula, int soThamSoThamChieu)
            : base($"Không thể xóa — công thức này đang được {soThamSoThamChieu} tham số của công thức khác " +
                   "lấy làm nguồn giá trị (FORMULA_KETQUA). Xóa các tham chiếu đó trước.")
        {
            IdFormula = idFormula;
            SoThamSoThamChieu = soThamSoThamChieu;
        }
    }
}
