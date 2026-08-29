using PM_QLTBHTD.Domain.Entities;

namespace PM_QLTBHTD.Domain.IRepository
{
    public interface ITramDienRepository : IGenericRepository<CBM_TramDien>
    {
        Task<IEnumerable<CBM_TramDien>> GetByKhuVucAsync(int idKhuVuc);
        Task<IEnumerable<CBM_TramDien>> GetAllActiveAsync();
    }
}
