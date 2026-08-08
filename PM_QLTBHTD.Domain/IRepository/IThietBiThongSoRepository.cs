using PM_QLTBHTD.Domain.Entities;

namespace PM_QLTBHTD.Domain.IRepository
{
    public interface IThietBiThongSoRepository : IGenericRepository<CBM_ThietBi_ThongSo>
    {
        Task<List<CBM_ThietBi_ThongSo>> GetByThietBiAsync(int idThietBi);
        Task<List<CBM_ThietBi_ThongSo>> GetByThongSoAsync(int idThongSo);
    }
}
