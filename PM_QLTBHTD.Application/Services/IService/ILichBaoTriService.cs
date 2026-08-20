using PM_QLTBHTD.Application.DTOs;

namespace PM_QLTBHTD.Application.Services.IService
{
    public interface ILichBaoTriService
    {
        Task<PagedResult<LichBaoTriDto>> GetPagedAsync(
            string? search, string? trangThai, int? idTram,
            DateTime? tuNgay, DateTime? denNgay, int page, int? pageSize);

        Task<LichBaoTriDto?> GetByIdAsync(int id);
        Task<IEnumerable<LichBaoTriDto>> GetByThietBiAsync(int idThietBi);
        Task<LichBaoTriDto> CreateAsync(CreateLichBaoTriDto dto);
        Task<LichBaoTriDto?> UpdateAsync(int id, UpdateLichBaoTriDto dto);
        Task<bool> DeleteAsync(int id);
        Task<LichBaoTriDto?> HoanThanhAsync(int id, HoanThanhLichBaoTriDto dto);
        Task<LichBaoTriDto?> HuyAsync(int id);
        Task<ThongKeLichBaoTriDto> GetThongKeAsync();
    }
}
