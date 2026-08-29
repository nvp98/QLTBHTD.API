using PM_QLTBHTD.Application.DTOs;

namespace PM_QLTBHTD.Application.Services.IService
{
    public interface IThietBiThongSoService
    {
        Task<List<ThietBiThongSoDto>> GetByThietBiAsync(int idThietBi);
        Task<List<ThietBiThongSoUsageDto>> GetByThongSoAsync(int idThongSo);
        Task<ThietBiThongSoDto> CreateAsync(CreateThietBiThongSoDto dto);
        Task<ThietBiThongSoDto?> UpdateAsync(int id, UpdateThietBiThongSoDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
