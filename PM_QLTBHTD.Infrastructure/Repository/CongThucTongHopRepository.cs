using Microsoft.EntityFrameworkCore;
using PM_QLTBHTD.Domain.Entities;
using PM_QLTBHTD.Domain.IRepository;
using PM_QLTBHTD.Infrastructure.Persistence;

namespace PM_QLTBHTD.Infrastructure.Repository
{
    public class CongThucTongHopRepository : GenericRepository<CBM_CongThucTongHop>, ICongThucTongHopRepository
    {
        public CongThucTongHopRepository(AppDbContext context) : base(context) { }

        public async Task<CBM_CongThucTongHop?> GetActiveByNhomAsync(int idNhomChiTieu)
            => await _dbSet.FirstOrDefaultAsync(x => x.ID_NhomChiTieu == idNhomChiTieu && x.TrangThai == 1);

        public async Task<IEnumerable<CBM_CongThucTongHop>> GetByNhomAsync(int idNhomChiTieu)
            => await _dbSet.Where(x => x.ID_NhomChiTieu == idNhomChiTieu).ToListAsync();
    }
}
