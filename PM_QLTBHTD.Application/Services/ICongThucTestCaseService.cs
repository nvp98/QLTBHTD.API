using PM_QLTBHTD.Application.DTOs;

namespace PM_QLTBHTD.Application.Services
{
    public interface ICongThucTestCaseService
    {
        Task<List<CongThucTestCaseDto>> GetByCongThucAsync(int idCongThuc);
        Task<CongThucTestCaseDto> CreateAsync(CreateCongThucTestCaseDto dto);
        Task<CongThucTestCaseDto?> UpdateAsync(int id, UpdateCongThucTestCaseDto dto);
        Task<bool> DeleteAsync(int id);
        /// <summary>Chạy lại TOÀN BỘ test case đã lưu của 1 công thức — evaluate BieuThuc hiện tại
        /// (không phải lúc lưu test case) với InputJson của từng case, so với KetQuaMongDoi.
        /// Dùng làm cơ chế regression khi công thức bị sửa lại.</summary>
        Task<List<CongThucTestCaseDto>> ChayTatCaAsync(int idCongThuc);
    }
}
