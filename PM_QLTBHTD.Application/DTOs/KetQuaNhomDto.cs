namespace PM_QLTBHTD.Application.DTOs
{
    public class KetQuaNhomDto
    {
        public int ID_NhomChiTieu { get; set; }
        public string TenNhom { get; set; } = string.Empty;
        public string LoaiNhom { get; set; } = string.Empty;
        public int CapDo { get; set; }
        public decimal Diem { get; set; }
        public string? BienDaBind { get; set; }
        public DateTime ThoiGianTinh { get; set; }
        public List<KetQuaNhomDto> NhomCon { get; set; } = [];
    }

    public class TinhDiemCayResultDto
    {
        public int IDPhieu { get; set; }
        public List<KetQuaNhomDto> KetQuaCay { get; set; } = [];
    }
}
