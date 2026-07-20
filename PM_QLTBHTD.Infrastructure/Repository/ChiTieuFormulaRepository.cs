using Microsoft.EntityFrameworkCore;
using PM_QLTBHTD.Domain.Entities;
using PM_QLTBHTD.Domain.IRepository;
using PM_QLTBHTD.Infrastructure.Persistence;

namespace PM_QLTBHTD.Infrastructure.Repository
{
    public class ChiTieuFormulaRepository : GenericRepository<CBM_ChiTieu_Formula>, IChiTieuFormulaRepository
    {
        public ChiTieuFormulaRepository(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<CBM_ChiTieu_Formula>> GetByChiTieuAsync(int idChiTieu)
            => await _dbSet.Where(x => x.ID_ChiTieu == idChiTieu && x.TrangThai == 1)
                            .OrderBy(x => x.ThuTu)
                            .ToListAsync();
    }
}
