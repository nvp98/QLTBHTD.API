using PM_QLTBHTD.Application.DTOs;

namespace PM_QLTBHTD.Application.Services
{
    public interface IThongKeService
    {
        Task<ThongKeTongHopDto> GetTongHopAsync();
        Task<IEnumerable<LichSuCSSKDto>> GetLichSuThietBiAsync(int idThietBi);
        Task<IEnumerable<BaoCaoTramItemDto>> GetBaoCaoTramAsync();
        Task<IEnumerable<CanhBaoThietBiDto>> GetCanhBaoAsync();
    }
}
