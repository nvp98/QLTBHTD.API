using PM_QLTBHTD.Domain.Entities;

namespace PM_QLTBHTD.Domain.IRepository
{
    public interface IChiTietKiemTraInputRepository : IGenericRepository<ChiTietKiemTra_Input>
    {
        Task DeleteByPhieuAsync(int idPhieu);
    }
}
