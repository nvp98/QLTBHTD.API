using PM_QLTBHTD.Domain.Entities;

namespace PM_QLTBHTD.Application.Interfaces
{
    /// <summary>
    /// Interface read-only để services dùng LINQ join query trực tiếp trên các DbSet
    /// </summary>
    public interface IAppDbContext
    {
        IQueryable<CBM_KhuVuc> KhuVucs { get; }
        IQueryable<CBM_LoaiThietBi> LoaiThietBis { get; }
        IQueryable<CBM_TramDien> TramDiens { get; }
        IQueryable<CBM_ThietBi> ThietBis { get; }
        IQueryable<CBM_NhomChiTieu> NhomChiTieus { get; }
        IQueryable<CBM_ChiTieu> ChiTieus { get; }
        IQueryable<CBM_Nguong> Nguongs { get; }
        IQueryable<PhieuKiemTra> PhieuKiemTras { get; }
        IQueryable<ChiTietKiemTra> ChiTietKiemTras { get; }
        IQueryable<ChiTietKiemTra_Input> ChiTietKiemTra_Inputs { get; }
        IQueryable<CBM_ChiTieu_Input> ChiTieuInputs { get; }
        IQueryable<CBM_ChiTieu_Rule> ChiTieuRules { get; }
    }
}
