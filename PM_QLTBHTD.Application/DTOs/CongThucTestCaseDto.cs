namespace PM_QLTBHTD.Application.DTOs
{
    public class CongThucTestCaseDto
    {
        public int ID_TestCase { get; set; }
        public int ID_CongThuc { get; set; }
        public string TenTestCase { get; set; } = string.Empty;
        public string InputJson { get; set; } = "{}";
        public decimal KetQuaMongDoi { get; set; }
        public decimal? KetQuaThucTeLanCuoi { get; set; }
        public bool? DatLanCuoi { get; set; }
        public DateTime? ThoiGianChayCuoi { get; set; }
        public string? LoiLanCuoi { get; set; }
        public string? MoTa { get; set; }
    }

    public class CreateCongThucTestCaseDto
    {
        public int ID_CongThuc { get; set; }
        public string TenTestCase { get; set; } = string.Empty;
        public string InputJson { get; set; } = "{}";
        public decimal KetQuaMongDoi { get; set; }
        public string? MoTa { get; set; }
    }

    public class UpdateCongThucTestCaseDto
    {
        public string TenTestCase { get; set; } = string.Empty;
        public string InputJson { get; set; } = "{}";
        public decimal KetQuaMongDoi { get; set; }
        public string? MoTa { get; set; }
    }
}
