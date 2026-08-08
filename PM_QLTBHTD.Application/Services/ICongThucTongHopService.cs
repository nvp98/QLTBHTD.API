using PM_QLTBHTD.Application.DTOs;
using PM_QLTBHTD.Application.Helpers;

namespace PM_QLTBHTD.Application.Services
{
    public interface ICongThucTongHopService
    {
        Task<IEnumerable<CongThucTongHopDto>> GetByNhomAsync(int idNhomChiTieu);
        Task<CongThucTongHopDto?> GetActiveByNhomAsync(int idNhomChiTieu);
        Task<CongThucTongHopDto?> GetByIdAsync(int id);
        Task<CongThucTongHopDto> CreateAsync(CreateCongThucTongHopDto dto);
        Task<CongThucTongHopDto?> UpdateAsync(int id, UpdateCongThucTongHopDto dto);
        Task<bool> DeleteAsync(int id);
        /// <summary>Config Validator — quét vòng lặp tham chiếu trong cây công thức của 1 loại thiết bị.</summary>
        Task<VongLapKetQua> ValidateVongLapAsync(int idLoaiThietBi);
    }
}
