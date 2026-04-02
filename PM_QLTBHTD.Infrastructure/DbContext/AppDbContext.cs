using Microsoft.EntityFrameworkCore;
using PM_QLTBHTD.Application.Interfaces;
using PM_QLTBHTD.Domain.Entities;

namespace PM_QLTBHTD.Infrastructure.Persistence
{
    public class AppDbContext : DbContext, IAppDbContext
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

        // IAppDbContext — expose IQueryable cho LINQ join trong services
        IQueryable<CBM_KhuVuc> IAppDbContext.KhuVucs => CBM_KhuVuc.AsQueryable();
        IQueryable<CBM_LoaiThietBi> IAppDbContext.LoaiThietBis => CBM_LoaiThietBi.AsQueryable();
        IQueryable<CBM_TramDien> IAppDbContext.TramDiens => CBM_TramDien.AsQueryable();
        IQueryable<CBM_ThietBi> IAppDbContext.ThietBis => CBM_ThietBi.AsQueryable();
        IQueryable<CBM_NhomChiTieu> IAppDbContext.NhomChiTieus => CBM_NhomChiTieu.AsQueryable();
        IQueryable<CBM_ChiTieu> IAppDbContext.ChiTieus => CBM_ChiTieu.AsQueryable();
        IQueryable<CBM_Nguong> IAppDbContext.Nguongs => CBM_Nguong.AsQueryable();
        IQueryable<PhieuKiemTra> IAppDbContext.PhieuKiemTras => PhieuKiemTra.AsQueryable();
        IQueryable<ChiTietKiemTra> IAppDbContext.ChiTietKiemTras => ChiTietKiemTra.AsQueryable();
    }
}
