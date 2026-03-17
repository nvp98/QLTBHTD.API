using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM_QLTBHTD.Domain.Entities
{
    public class CBM_NhomChiTieu
    {
        public int ID_NhomChiTieu { get; set; }
        public string TenNhom { get; set; } = string.Empty;

        public int ID_LoaiThietBi { get; set; }
        public int PhienBan { get; set; }

        public int TrangThai { get; set; }
    }
}
