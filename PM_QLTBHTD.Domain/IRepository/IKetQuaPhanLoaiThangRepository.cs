using PM_QLTBHTD.Domain.Entities;

namespace PM_QLTBHTD.Domain.IRepository
{
    public interface IKetQuaPhanLoaiThangRepository : IGenericRepository<CBM_KetQuaPhanLoaiThang>
    {
        Task<IEnumerable<CBM_KetQuaPhanLoaiThang>> GetByPhieuChiTieuAsync(int idPhieu, int idChiTieu);
    }
}
