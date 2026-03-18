using PM_QLTBHTD.Domain.Entities;

namespace PM_QLTBHTD.Domain.IRepository
{
    public interface IPhieuKiemTraRepository : IGenericRepository<PhieuKiemTra>
    {
        Task<IEnumerable<PhieuKiemTra>> GetByThietBiAsync(int idThietBi);
        Task<IEnumerable<PhieuKiemTra>> GetByNgayKiemTraAsync(DateTime tuNgay, DateTime denNgay);
        Task<PhieuKiemTra?> GetWithChiTietAsync(int idPhieu);
    }
}
