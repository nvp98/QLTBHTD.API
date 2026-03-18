using PM_QLTBHTD.Domain.Entities;

namespace PM_QLTBHTD.Domain.IRepository
{
    public interface IKhuVucRepository : IGenericRepository<CBM_KhuVuc>
    {
        Task<IEnumerable<CBM_KhuVuc>> GetAllActiveAsync();
    }
}
