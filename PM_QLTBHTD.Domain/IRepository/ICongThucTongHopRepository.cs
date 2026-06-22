using PM_QLTBHTD.Domain.Entities;

namespace PM_QLTBHTD.Domain.IRepository
{
    public interface ICongThucTongHopRepository : IGenericRepository<CBM_CongThucTongHop>
    {
        Task<CBM_CongThucTongHop?> GetActiveByNhomAsync(int idNhomChiTieu);
        Task<IEnumerable<CBM_CongThucTongHop>> GetByNhomAsync(int idNhomChiTieu);
    }
}
