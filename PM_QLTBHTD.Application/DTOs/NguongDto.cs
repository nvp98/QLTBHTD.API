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
        public bool CanDuoi_BaoGom { get; set; }
        public bool CanTren_BaoGom { get; set; }
    }

    public class CreateNguongDto
    {
        public int ID_ChiTieu { get; set; }
        public decimal? CanTren { get; set; }
        public decimal? CanDuoi { get; set; }
        public decimal? Diem_Si { get; set; }
        /// <summary>true = ≥ (bao gồm đầu mút dưới), false = > . Default: true</summary>
        public bool CanDuoi_BaoGom { get; set; } = true;
        /// <summary>true = ≤ (bao gồm đầu mút trên), false = &lt; . Default: false</summary>
        public bool CanTren_BaoGom { get; set; } = false;
    }

    public class UpdateNguongDto
    {
        public int ID_ChiTieu { get; set; }
        public decimal? CanTren { get; set; }
        public decimal? CanDuoi { get; set; }
        public decimal? Diem_Si { get; set; }
        /// <summary>true = ≥ (bao gồm đầu mút dưới), false = > . Default: true</summary>
        public bool CanDuoi_BaoGom { get; set; } = true;
        /// <summary>true = ≤ (bao gồm đầu mút trên), false = &lt; . Default: false</summary>
        public bool CanTren_BaoGom { get; set; } = false;
    }
}