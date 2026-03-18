using PM_QLTBHTD.Domain.Entities;

namespace PM_QLTBHTD.Domain.IRepository
{
    public interface IThietBiRepository : IGenericRepository<CBM_ThietBi>
    {
        Task<IEnumerable<CBM_ThietBi>> GetByTramAsync(int idTram);
        Task<IEnumerable<CBM_ThietBi>> GetByLoaiThietBiAsync(int idLoaiTB);
        Task<IEnumerable<CBM_ThietBi>> GetAllActiveAsync();
    }
}
