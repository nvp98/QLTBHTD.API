using Microsoft.EntityFrameworkCore;
using PM_QLTBHTD.Domain.Entities;
using PM_QLTBHTD.Domain.IRepository;
using PM_QLTBHTD.Infrastructure.Persistence;

namespace PM_QLTBHTD.Infrastructure.Repository
{
    public class NguoiDungRepository : GenericRepository<CBM_NguoiDung>, INguoiDungRepository
    {
        public NguoiDungRepository(AppDbContext context) : base(context) { }

        public async Task<CBM_NguoiDung?> GetByTenDangNhapAsync(string tenDangNhap)
            => await _dbSet.FirstOrDefaultAsync(x => x.TenDangNhap == tenDangNhap);
    }
}
