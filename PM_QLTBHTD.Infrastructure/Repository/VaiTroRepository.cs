using PM_QLTBHTD.Domain.Entities;
using PM_QLTBHTD.Domain.IRepository;
using PM_QLTBHTD.Infrastructure.Persistence;

namespace PM_QLTBHTD.Infrastructure.Repository
{
    public class VaiTroRepository : GenericRepository<CBM_VaiTro>, IVaiTroRepository
    {
        public VaiTroRepository(AppDbContext context) : base(context) { }
    }
}
