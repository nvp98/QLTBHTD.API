using PM_QLTBHTD.Domain.Entities;

namespace PM_QLTBHTD.Domain.IRepository
{
    public interface IChiTieuRepository : IGenericRepository<CBM_ChiTieu>
    {
        Task<IEnumerable<CBM_ChiTieu>> GetByNhomChiTieuAsync(int idNhomChiTieu);
        Task<IEnumerable<CBM_ChiTieu>> GetAllActiveAsync();
    }
}
