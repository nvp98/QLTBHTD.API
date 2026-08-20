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

        // Danh mục
        public DbSet<CBM_KhuVuc> CBM_KhuVuc { get; set; }
        public DbSet<CBM_LoaiThietBi> CBM_LoaiThietBi { get; set; }
        public DbSet<CBM_TramDien> CBM_TramDien { get; set; }
        public DbSet<CBM_ThietBi> CBM_ThietBi { get; set; }
        public DbSet<CBM_ThongSo> CBM_ThongSo { get; set; }
        public DbSet<CBM_ThietBi_ThongSo> CBM_ThietBi_ThongSo { get; set; }

        // Chỉ tiêu đánh giá
        public DbSet<CBM_NhomChiTieu> CBM_NhomChiTieu { get; set; }
        public DbSet<CBM_ChiTieu> CBM_ChiTieu { get; set; }
        public DbSet<CBM_Nguong> CBM_Nguong { get; set; }
        public DbSet<CBM_ChiTieu_Input> CBM_ChiTieu_Input { get; set; }
        public DbSet<CBM_ChiTieu_Rule> CBM_ChiTieu_Rule { get; set; }
        public DbSet<CBM_ChiTieu_Formula> CBM_ChiTieu_Formula { get; set; }
        public DbSet<CBM_ChiTieu_Formula_ThamSo> CBM_ChiTieu_Formula_ThamSo { get; set; }

        // Công thức tổng hợp
        public DbSet<CBM_CongThucTongHop> CBM_CongThucTongHop { get; set; }
        public DbSet<CBM_CongThuc_Bien> CBM_CongThuc_Bien { get; set; }
        public DbSet<CBM_KetQuaNhom> CBM_KetQuaNhom { get; set; }
        public DbSet<CBM_KetQuaTrungGian> CBM_KetQuaTrungGian { get; set; }
        public DbSet<CBM_CongThuc_TestCase> CBM_CongThuc_TestCase { get; set; }

        // Phiếu kiểm tra
        public DbSet<PhieuKiemTra> PhieuKiemTra { get; set; }
        public DbSet<ChiTietKiemTra> ChiTietKiemTra { get; set; }
        public DbSet<ChiTietKiemTra_Input> ChiTietKiemTra_Input { get; set; }

        // Phân loại theo thời gian
        public DbSet<CBM_ChiTieu_PhanLoaiNguong> CBM_ChiTieu_PhanLoaiNguong { get; set; }
        public DbSet<CBM_KetQuaPhanLoaiThang> CBM_KetQuaPhanLoaiThang { get; set; }

        // Bảo trì
        public DbSet<CBM_LichBaoTri> CBM_LichBaoTri { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Bảng có trigger DB (TR_ChiTietKiemTra_Input_FillThietBi, tự điền ID_ThietBi sau INSERT)
            // — phải khai báo cho EF Core biết, nếu không nó dùng OUTPUT INSERTED.* để lấy ID vừa
            // sinh, mà SQL Server cấm OUTPUT (không INTO) trên bảng có trigger → DbUpdateException.
            modelBuilder.Entity<ChiTietKiemTra_Input>()
                .ToTable(tb => tb.HasTrigger("TR_ChiTietKiemTra_Input_FillThietBi"));

            // Unique: mỗi nhóm COMPOSITE chỉ có 1 công thức ACTIVE — ép ở app layer,
            // nhưng thêm filtered unique index để DB cũng chặn được duplicates.
            modelBuilder.Entity<CBM_CongThucTongHop>()
                .HasIndex(x => new { x.ID_NhomChiTieu, x.TrangThai })
                .HasFilter("[TrangThai] = 1")
                .IsUnique()
                .HasDatabaseName("UX_CongThucTongHop_NhomActive");

            // Unique: mỗi (IDPhieu, ID_NhomChiTieu) chỉ có 1 dòng cache
            modelBuilder.Entity<CBM_KetQuaNhom>()
                .HasIndex(x => new { x.IDPhieu, x.ID_NhomChiTieu })
                .IsUnique()
                .HasDatabaseName("UX_KetQuaNhom_PhieuNhom");
        }

        // IAppDbContext — expose IQueryable cho LINQ join trong services
        IQueryable<CBM_KhuVuc> IAppDbContext.KhuVucs => CBM_KhuVuc.AsQueryable();
        IQueryable<CBM_LoaiThietBi> IAppDbContext.LoaiThietBis => CBM_LoaiThietBi.AsQueryable();
        IQueryable<CBM_TramDien> IAppDbContext.TramDiens => CBM_TramDien.AsQueryable();
        IQueryable<CBM_ThietBi> IAppDbContext.ThietBis => CBM_ThietBi.AsQueryable();
        IQueryable<CBM_ThongSo> IAppDbContext.ThongSos => CBM_ThongSo.AsQueryable();
        IQueryable<CBM_ThietBi_ThongSo> IAppDbContext.ThietBiThongSos => CBM_ThietBi_ThongSo.AsQueryable();
        IQueryable<CBM_NhomChiTieu> IAppDbContext.NhomChiTieus => CBM_NhomChiTieu.AsQueryable();
        IQueryable<CBM_ChiTieu> IAppDbContext.ChiTieus => CBM_ChiTieu.AsQueryable();
        IQueryable<CBM_Nguong> IAppDbContext.Nguongs => CBM_Nguong.AsQueryable();
        IQueryable<CBM_ChiTieu_Input> IAppDbContext.ChiTieuInputs => CBM_ChiTieu_Input.AsQueryable();
        IQueryable<CBM_ChiTieu_Rule> IAppDbContext.ChiTieuRules => CBM_ChiTieu_Rule.AsQueryable();
        IQueryable<CBM_ChiTieu_Formula> IAppDbContext.ChiTieuFormulas => CBM_ChiTieu_Formula.AsQueryable();
        IQueryable<CBM_ChiTieu_Formula_ThamSo> IAppDbContext.ChiTieuFormulaThamSos => CBM_ChiTieu_Formula_ThamSo.AsQueryable();
        IQueryable<CBM_CongThucTongHop> IAppDbContext.CongThucTongHops => CBM_CongThucTongHop.AsQueryable();
        IQueryable<CBM_CongThuc_Bien> IAppDbContext.CongThucBiens => CBM_CongThuc_Bien.AsQueryable();
        IQueryable<CBM_KetQuaNhom> IAppDbContext.KetQuaNhoms => CBM_KetQuaNhom.AsQueryable();
        IQueryable<CBM_KetQuaTrungGian> IAppDbContext.KetQuaTrungGians => CBM_KetQuaTrungGian.AsQueryable();
        IQueryable<PhieuKiemTra> IAppDbContext.PhieuKiemTras => PhieuKiemTra.AsQueryable();
        IQueryable<ChiTietKiemTra> IAppDbContext.ChiTietKiemTras => ChiTietKiemTra.AsQueryable();
        IQueryable<ChiTietKiemTra_Input> IAppDbContext.ChiTietKiemTra_Inputs => ChiTietKiemTra_Input.AsQueryable();
        IQueryable<CBM_ChiTieu_PhanLoaiNguong> IAppDbContext.PhanLoaiNguongs => CBM_ChiTieu_PhanLoaiNguong.AsQueryable();
        IQueryable<CBM_KetQuaPhanLoaiThang> IAppDbContext.KetQuaPhanLoaiThangs => CBM_KetQuaPhanLoaiThang.AsQueryable();
        IQueryable<CBM_LichBaoTri> IAppDbContext.LichBaoTris => CBM_LichBaoTri.AsQueryable();
    }
}
