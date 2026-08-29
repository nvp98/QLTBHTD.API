using Microsoft.EntityFrameworkCore;
using PM_QLTBHTD.Domain.Entities;
using PM_QLTBHTD.Domain.IRepository;
using PM_QLTBHTD.Infrastructure.Persistence;

namespace PM_QLTBHTD.Infrastructure.Repository
{
    public class ChiTieuFormulaThamSoRepository : GenericRepository<CBM_ChiTieu_Formula_ThamSo>, IChiTieuFormulaThamSoRepository
    {
        public ChiTieuFormulaThamSoRepository(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<CBM_ChiTieu_Formula_ThamSo>> GetByFormulaAsync(int idFormula)
            => await _dbSet.Where(x => x.ID_Formula == idFormula).ToListAsync();
    }
}
