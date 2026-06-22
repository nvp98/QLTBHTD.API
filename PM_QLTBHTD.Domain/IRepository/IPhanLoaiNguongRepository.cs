using PM_QLTBHTD.Domain.Entities;

namespace PM_QLTBHTD.Domain.IRepository
{
    public interface IPhanLoaiNguongRepository : IGenericRepository<CBM_ChiTieu_PhanLoaiNguong>
    {
        Task<IEnumerable<CBM_ChiTieu_PhanLoaiNguong>> GetByChiTieuAsync(int idChiTieu);
    }
}
