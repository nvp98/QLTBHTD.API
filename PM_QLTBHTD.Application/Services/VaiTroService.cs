using PM_QLTBHTD.Application.DTOs;
using PM_QLTBHTD.Application.Services.IService;
using PM_QLTBHTD.Domain.Entities;
using PM_QLTBHTD.Domain.IRepository;

namespace PM_QLTBHTD.Application.Services
{
    public class VaiTroService : IVaiTroService
    {
        private readonly IVaiTroRepository _repository;

        public VaiTroService(IVaiTroRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<VaiTroDto>> GetAllAsync()
        {
            var items = await _repository.GetAllAsync();
            return items.Select(MapToDto);
        }

        private static VaiTroDto MapToDto(CBM_VaiTro x) => new()
        {
            ID_VaiTro = x.ID_VaiTro,
            MaVaiTro = x.MaVaiTro,
            TenVaiTro = x.TenVaiTro,
            MoTa = x.MoTa,
        };
    }
}
