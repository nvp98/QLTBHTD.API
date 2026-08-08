using PM_QLTBHTD.Domain.Entities;
using PM_QLTBHTD.Domain.IRepository;
using PM_QLTBHTD.Infrastructure.Persistence;

namespace PM_QLTBHTD.Infrastructure.Repository
{
    public class ThongSoRepository : GenericRepository<CBM_ThongSo>, IThongSoRepository
    {
        public ThongSoRepository(AppDbContext context) : base(context) { }
    }
}
