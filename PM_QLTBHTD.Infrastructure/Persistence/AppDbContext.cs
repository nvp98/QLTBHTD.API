using Microsoft.EntityFrameworkCore;
using PM_QLTBHTD.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace PM_QLTBHTD.Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<CBM_ChiTieu> CBM_ChiTieus { get; set; }
        public DbSet<CBM_Nguong> CBM_Nguongs { get; set; }
        public DbSet<CBM_NhomChiTieu> CBM_NhomChiTieus { get; set; }
        public DbSet<CBM_LoaiThietBi> CBM_LoaiThietBis { get; set; }
        public DbSet<CBM_ThietBi> CBM_ThietBis { get; set; }
        public DbSet<CBM_TramDien> CBM_TramDiens { get; set; }
        public DbSet<CBM_KhuVuc> CBM_KhuVucs { get; set; }
        public DbSet<PhieuKiemTra> PhieuKiemTras { get; set; }
        public DbSet<ChiTietKiemTra> ChiTietKiemTras { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ===== APPLY CONFIG =====
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }
    }
}
