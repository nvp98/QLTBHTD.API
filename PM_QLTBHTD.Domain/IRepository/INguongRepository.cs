using PM_QLTBHTD.Domain.Entities;

namespace PM_QLTBHTD.Domain.IRepository
{
    public interface INguongRepository : IGenericRepository<CBM_Nguong>
    {
        Task<IEnumerable<CBM_Nguong>> GetByChiTieuAsync(int idChiTieu);
    }
}
