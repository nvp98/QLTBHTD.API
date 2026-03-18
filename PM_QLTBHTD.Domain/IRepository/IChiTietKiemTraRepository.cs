using PM_QLTBHTD.Domain.Entities;

namespace PM_QLTBHTD.Domain.IRepository
{
    public interface IChiTietKiemTraRepository : IGenericRepository<ChiTietKiemTra>
    {
        Task<IEnumerable<ChiTietKiemTra>> GetByPhieuAsync(int idPhieu);
        Task DeleteByPhieuAsync(int idPhieu);
    }
}
