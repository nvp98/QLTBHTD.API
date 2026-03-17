using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM_QLTBHTD.Domain.Entities
{
    public class PhieuKiemTra
    {
        public int ID_Phieu { get; set; }
        public int ID_ThietBi { get; set; }

        public DateTime NgayKiemTra { get; set; }
        public string? NguoiKiemTra { get; set; }

        public decimal? TongDiem_Soqt { get; set; }
        public string? CapDoCanhBao { get; set; }

        public string? GhiChuChung { get; set; }
    }
}
