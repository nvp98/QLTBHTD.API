using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM_QLTBHTD.Domain.Entities
{
    public class CBM_TramDien
    {
        public int IDTram { get; set; }
        public int IDKhuVuc { get; set; }

        public string TenTram { get; set; } = string.Empty;
        public string? DiaDiem { get; set; }
        public int TrangThai { get; set; }
    }
}
