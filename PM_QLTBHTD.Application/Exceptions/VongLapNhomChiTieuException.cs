namespace PM_QLTBHTD.Application.Exceptions
{
    public class VongLapNhomChiTieuException : Exception
    {
        public int IdNhomBiLap { get; }
        public IReadOnlyList<int> DuongDi { get; }

        public VongLapNhomChiTieuException(int idNhomBiLap, IEnumerable<int> duongDi)
            : base($"Phát hiện vòng lặp trong cây NhomChiTieu: nhóm ID={idNhomBiLap} đã xuất hiện trong đường đi [{string.Join("→", duongDi)}]. " +
                   "Kiểm tra cấu hình ID_NhomCha và CBM_CongThuc_Bien (NHOM_CON).")
        {
            IdNhomBiLap = idNhomBiLap;
            DuongDi = duongDi.ToList().AsReadOnly();
        }
    }
}
