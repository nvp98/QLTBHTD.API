using PM_QLTBHTD.Domain.Entities;

namespace PM_QLTBHTD.Domain.IRepository
{
    public interface INguoiDungRepository : IGenericRepository<CBM_NguoiDung>
    {
        Task<CBM_NguoiDung?> GetByTenDangNhapAsync(string tenDangNhap);
    }
}
