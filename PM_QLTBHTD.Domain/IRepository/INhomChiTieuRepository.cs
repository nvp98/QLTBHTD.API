using PM_QLTBHTD.Domain.Entities;

namespace PM_QLTBHTD.Domain.IRepository
{
    public interface INhomChiTieuRepository : IGenericRepository<CBM_NhomChiTieu>
    {
        Task<IEnumerable<CBM_NhomChiTieu>> GetByLoaiThietBiAsync(int idLoaiThietBi);
        Task<IEnumerable<CBM_NhomChiTieu>> GetAllActiveAsync();
    }
}
