using Microsoft.EntityFrameworkCore;
using PM_QLTBHTD.Domain.Entities;
using PM_QLTBHTD.Domain.IRepository;
using PM_QLTBHTD.Infrastructure.Persistence;

namespace PM_QLTBHTD.Infrastructure.Repository
{
    public class LoaiThietBiRepository : GenericRepository<CBM_LoaiThietBi>, ILoaiThietBiRepository
    {
        public LoaiThietBiRepository(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<CBM_LoaiThietBi>> GetAllActiveAsync()
            => await _dbSet.Where(x => x.TrangThai == 1).ToListAsync();
    }
}
