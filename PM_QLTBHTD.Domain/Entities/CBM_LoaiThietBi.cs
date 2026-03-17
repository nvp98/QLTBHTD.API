using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM_QLTBHTD.Domain.Entities
{
    public class CBM_LoaiThietBi
    {
        public int ID_LoaiThietBi { get; set; }
        public string TenLoaiTB { get; set; } = string.Empty;
        public string KyHieu { get; set; } = string.Empty;
        public int TrangThai { get; set; }
    }
}
