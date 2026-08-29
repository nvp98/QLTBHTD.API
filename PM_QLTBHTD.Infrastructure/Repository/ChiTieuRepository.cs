using Microsoft.EntityFrameworkCore;
using PM_QLTBHTD.Domain.Entities;
using PM_QLTBHTD.Domain.IRepository;
using PM_QLTBHTD.Infrastructure.Persistence;

namespace PM_QLTBHTD.Infrastructure.Repository
{
    public class ChiTieuRepository : GenericRepository<CBM_ChiTieu>, IChiTieuRepository
    {
        public ChiTieuRepository(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<CBM_ChiTieu>> GetByNhomChiTieuAsync(int idNhomChiTieu)
            => await _dbSet.Where(x => x.ID_NhomChiTieu == idNhomChiTieu).ToListAsync();

        public async Task<IEnumerable<CBM_ChiTieu>> GetAllActiveAsync()
            => await _dbSet.Where(x => x.TrangThai == 1).ToListAsync();
    }
}
