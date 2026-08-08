using PM_QLTBHTD.Domain.Entities;
using PM_QLTBHTD.Domain.IRepository;
using PM_QLTBHTD.Infrastructure.Persistence;

namespace PM_QLTBHTD.Infrastructure.Repository
{
    public class KetQuaTrungGianRepository : IKetQuaTrungGianRepository
    {
        private readonly AppDbContext _context;

        public KetQuaTrungGianRepository(AppDbContext context) => _context = context;

        public async Task AddRangeAsync(IEnumerable<CBM_KetQuaTrungGian> items)
            => await _context.CBM_KetQuaTrungGian.AddRangeAsync(items);

        public async Task<int> SaveChangesAsync()
            => await _context.SaveChangesAsync();
    }
}
