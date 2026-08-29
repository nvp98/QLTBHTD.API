namespace PM_QLTBHTD.Application.Exceptions
{
    public class LoiFormulaException : Exception
    {
        public int IdChiTieu { get; }
        public string MaKetQua { get; }

        public LoiFormulaException(int idChiTieu, string maKetQua, string lyDo)
            : base($"Chỉ tiêu ID={idChiTieu}, Formula '{maKetQua}' tính lỗi: {lyDo}")
        {
            IdChiTieu = idChiTieu;
            MaKetQua = maKetQua;
        }
    }
}
