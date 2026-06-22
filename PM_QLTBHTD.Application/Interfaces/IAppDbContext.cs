using PM_QLTBHTD.Domain.Entities;

namespace PM_QLTBHTD.Application.Interfaces
{
    /// <summary>
    /// Interface read-only để services dùng LINQ join query trực tiếp trên các DbSet.
    /// </summary>
    public interface IAppDbContext
    {
        // Danh mục
        IQueryable<CBM_KhuVuc> KhuVucs { get; }
        IQueryable<CBM_LoaiThietBi> LoaiThietBis { get; }
        IQueryable<CBM_TramDien> TramDiens { get; }
        IQueryable<CBM_ThietBi> ThietBis { get; }

        // Chỉ tiêu đánh giá
        IQueryable<CBM_NhomChiTieu> NhomChiTieus { get; }
        IQueryable<CBM_ChiTieu> ChiTieus { get; }
        IQueryable<CBM_Nguong> Nguongs { get; }
        IQueryable<CBM_ChiTieu_Input> ChiTieuInputs { get; }
        IQueryable<CBM_ChiTieu_Rule> ChiTieuRules { get; }

        // Công thức tổng hợp
        IQueryable<CBM_CongThucTongHop> CongThucTongHops { get; }
        IQueryable<CBM_CongThuc_Bien> CongThucBiens { get; }
        IQueryable<CBM_KetQuaNhom> KetQuaNhoms { get; }

        // Phiếu kiểm tra
        IQueryable<PhieuKiemTra> PhieuKiemTras { get; }
        IQueryable<ChiTietKiemTra> ChiTietKiemTras { get; }
        IQueryable<ChiTietKiemTra_Input> ChiTietKiemTra_Inputs { get; }

        // Phân loại theo thời gian
        IQueryable<CBM_ChiTieu_PhanLoaiNguong> PhanLoaiNguongs { get; }
        IQueryable<CBM_KetQuaPhanLoaiThang> KetQuaPhanLoaiThangs { get; }
    }
}
