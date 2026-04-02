namespace PM_QLTBHTD.Application.DTOs
{
    public class NguongDto
    {
        public int ID_Nguong { get; set; }
        public int ID_ChiTieu { get; set; }
        public string? TenChiTieu { get; set; } = string.Empty;
        public decimal? CanTren { get; set; }
        public decimal? CanDuoi { get; set; }
        public decimal? Diem_Si { get; set; }
    }

    public class CreateNguongDto
    {
        public int ID_ChiTieu { get; set; }
        public decimal? CanTren { get; set; }
        public decimal? CanDuoi { get; set; }
        public decimal? Diem_Si { get; set; }
    }

    public class UpdateNguongDto
    {
        public int ID_ChiTieu { get; set; }
        public decimal? CanTren { get; set; }
        public decimal? CanDuoi { get; set; }
        public decimal? Diem_Si { get; set; }
    }
}
