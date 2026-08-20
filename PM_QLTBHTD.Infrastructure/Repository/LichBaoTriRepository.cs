using PM_QLTBHTD.Domain.Entities;
using PM_QLTBHTD.Domain.IRepository;
using PM_QLTBHTD.Infrastructure.Persistence;

namespace PM_QLTBHTD.Infrastructure.Repository
{
    public class LichBaoTriRepository : GenericRepository<CBM_LichBaoTri>, ILichBaoTriRepository
    {
        public LichBaoTriRepository(AppDbContext context) : base(context)
        {
        }
    }
}
