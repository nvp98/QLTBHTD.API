using PM_QLTBHTD.Domain.Entities;

namespace PM_QLTBHTD.Domain.IRepository
{
    public interface INganLoRepository : IGenericRepository<CBM_NganLo>
    {
        Task<IEnumerable<CBM_NganLo>> GetByTramAsync(int idTram);
        Task<IEnumerable<CBM_NganLo>> GetAllActiveAsync();
    }
}
