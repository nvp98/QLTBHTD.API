using PM_QLTBHTD.Application.DTOs;

namespace PM_QLTBHTD.Application.Services.IService
{
    public interface IVaiTroService
    {
        Task<IEnumerable<VaiTroDto>> GetAllAsync();
    }
}
