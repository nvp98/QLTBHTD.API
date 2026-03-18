using Microsoft.EntityFrameworkCore;
using PM_QLTBHTD.Domain.Entities;

namespace PM_QLTBHTD.Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<CBM_ChiTieu> CBM_ChiTieu { get; set; }
        public DbSet<CBM_Nguong> CBM_Nguong { get; set; }
        public DbSet<CBM_NhomChiTieu> CBM_NhomChiTieu { get; set; }
        public DbSet<CBM_LoaiThietBi> CBM_LoaiThietBi { get; set; }
        public DbSet<CBM_ThietBi> CBM_ThietBi { get; set; }
        public DbSet<CBM_TramDien> CBM_TramDien { get; set; }
        public DbSet<CBM_KhuVuc> CBM_KhuVuc { get; set; }
        public DbSet<PhieuKiemTra> PhieuKiemTra { get; set; }
        public DbSet<ChiTietKiemTra> ChiTietKiemTra { get; set; }

    }
}
