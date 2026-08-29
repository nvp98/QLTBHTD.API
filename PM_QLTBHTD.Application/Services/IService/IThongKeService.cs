using PM_QLTBHTD.Application.DTOs;

namespace PM_QLTBHTD.Application.Services.IService
{
    public interface IThongKeService
    {
        Task<ThongKeTongHopDto> GetTongHopAsync();
        Task<IEnumerable<LichSuCSSKDto>> GetLichSuThietBiAsync(int idThietBi);
        Task<IEnumerable<BaoCaoTramItemDto>> GetBaoCaoTramAsync();
        Task<IEnumerable<TongHopTheoLoaiDto>> GetTongHopTheoLoaiAsync();
        Task<IEnumerable<CanhBaoThietBiDto>> GetCanhBaoAsync();
        Task<IEnumerable<XuHuongThangDto>> GetXuHuongThangAsync(int soThang, int? idTram, int? idLoaiTB, int? idThietBi);
    }
}
