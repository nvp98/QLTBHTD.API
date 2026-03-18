using PM_QLTBHTD.Application.DTOs;
using PM_QLTBHTD.Domain.Entities;
using PM_QLTBHTD.Domain.IRepository;

namespace PM_QLTBHTD.Application.Services
{
    public class ThietBiService : IThietBiService
    {
        private readonly IThietBiRepository _repository;

        public ThietBiService(IThietBiRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<ThietBiDto>> GetAllAsync()
        {
            var items = await _repository.GetAllAsync();
            return items.Select(x => MapToDto(x));
        }

        public async Task<IEnumerable<ThietBiDto>> GetAllActiveAsync()
        {
            var items = await _repository.GetAllActiveAsync();
            return items.Select(x => MapToDto(x));
        }

        public async Task<IEnumerable<ThietBiDto>> GetByTramAsync(int idTram)
        {
            var items = await _repository.GetByTramAsync(idTram);
            return items.Select(x => MapToDto(x));
        }

        public async Task<IEnumerable<ThietBiDto>> GetByLoaiThietBiAsync(int idLoaiTB)
        {
            var items = await _repository.GetByLoaiThietBiAsync(idLoaiTB);
            return items.Select(x => MapToDto(x));
        }

        public async Task<ThietBiDto?> GetByIdAsync(int id)
        {
            var item = await _repository.GetByIdAsync(id);
            return item == null ? null : MapToDto(item);
        }

        public async Task<ThietBiDto> CreateAsync(CreateThietBiDto dto)
        {
            var entity = new CBM_ThietBi
            {
                ID_Tram = dto.ID_Tram,
                ID_LoaiTB = dto.ID_LoaiTB,
                TenThietBi = dto.TenThietBi,
                SoHieu = dto.SoHieu,
                NhanHieu = dto.NhanHieu,
                NamSanXuat = dto.NamSanXuat,
                TrangThai = dto.TrangThai,
                GhiChu = dto.GhiChu
            };
            await _repository.AddAsync(entity);
            await _repository.SaveChangesAsync();
            return MapToDto(entity);
        }

        public async Task<ThietBiDto?> UpdateAsync(int id, UpdateThietBiDto dto)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return null;

            entity.ID_Tram = dto.ID_Tram;
            entity.ID_LoaiTB = dto.ID_LoaiTB;
            entity.TenThietBi = dto.TenThietBi;
            entity.SoHieu = dto.SoHieu;
            entity.NhanHieu = dto.NhanHieu;
            entity.NamSanXuat = dto.NamSanXuat;
            entity.TrangThai = dto.TrangThai;
            entity.GhiChu = dto.GhiChu;
            _repository.Update(entity);
            await _repository.SaveChangesAsync();
            return MapToDto(entity);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return false;

            _repository.Delete(entity);
            await _repository.SaveChangesAsync();
            return true;
        }

        private static ThietBiDto MapToDto(CBM_ThietBi x) => new()
        {
            ID_ThietBi = x.ID_ThietBi,
            ID_Tram = x.ID_Tram,
            ID_LoaiTB = x.ID_LoaiTB,
            TenThietBi = x.TenThietBi,
            SoHieu = x.SoHieu,
            NhanHieu = x.NhanHieu,
            NamSanXuat = x.NamSanXuat,
            TrangThai = x.TrangThai,
            GhiChu = x.GhiChu
        };
    }
}
