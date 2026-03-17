using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM_QLTBHTD.Domain.Entities
{
    public class CBM_ChiTieu
    {
        public int ID_ChiTieu { get; set; }
        public int ID_NhomChiTieu { get; set; }
        public string TenChiTieu { get; set; } = string.Empty;
        public decimal TrongSo_Wi { get; set; }
        public int TrangThai { get; set; }

    }
}
