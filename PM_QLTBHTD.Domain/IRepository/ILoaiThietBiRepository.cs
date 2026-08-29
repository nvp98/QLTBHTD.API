using PM_QLTBHTD.Domain.Entities;

namespace PM_QLTBHTD.Domain.IRepository
{
    public interface ILoaiThietBiRepository : IGenericRepository<CBM_LoaiThietBi>
    {
        Task<IEnumerable<CBM_LoaiThietBi>> GetAllActiveAsync();
    }
}
