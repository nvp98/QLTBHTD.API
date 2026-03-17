using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM_QLTBHTD.Domain.Entities
{
    public class CBM_KhuVuc
    {
        public int ID_KhuVuc { get; set; }
        public string TenKhuVuc { get; set; } = string.Empty;
        public int TrangThai { get; set; }
    }
}
