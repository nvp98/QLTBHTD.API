using PM_QLTBHTD.Application.DTOs;

namespace PM_QLTBHTD.Application.Services
{
    public interface IChiTieuPhanLoaiNguongService
    {
        Task<IEnumerable<ChiTieuPhanLoaiNguongDto>> GetByChiTieuAsync(int idChiTieu);
        Task<ChiTieuPhanLoaiNguongDto?>              GetByIdAsync(int id);
        Task<ChiTieuPhanLoaiNguongDto>               CreateAsync(CreateChiTieuPhanLoaiNguongDto dto);
        Task<ChiTieuPhanLoaiNguongDto?>               UpdateAsync(int id, UpdateChiTieuPhanLoaiNguongDto dto);
        Task<bool>                                    DeleteAsync(int id);

        Task<IEnumerable<KetQuaPhanLoaiThangDto>> GetKetQuaThangAsync(int idPhieu, int idChiTieu);
    }
}
