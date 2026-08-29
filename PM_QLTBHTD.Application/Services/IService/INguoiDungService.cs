using PM_QLTBHTD.Application.DTOs;

namespace PM_QLTBHTD.Application.Services.IService
{
    public interface INguoiDungService
    {
        Task<IEnumerable<NguoiDungDto>> GetAllAsync();
        Task<NguoiDungDto?> GetByIdAsync(int id);
        Task<NguoiDungDto> CreateAsync(CreateNguoiDungDto dto);
        Task<NguoiDungDto?> UpdateAsync(int id, UpdateNguoiDungDto dto);
        Task<bool> DeleteAsync(int id);
        Task DoiMatKhauAsync(int idNguoiDung, DoiMatKhauDto dto);
        Task DatLaiMatKhauAsync(int idNguoiDung, DatLaiMatKhauDto dto);
    }
}
