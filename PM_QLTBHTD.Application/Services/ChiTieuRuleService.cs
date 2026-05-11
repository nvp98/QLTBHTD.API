using PM_QLTBHTD.Application.DTOs;
using PM_QLTBHTD.Domain.Entities;
using PM_QLTBHTD.Domain.IRepository;

namespace PM_QLTBHTD.Application.Services
{
    public class ChiTieuRuleService : IChiTieuRuleService
    {
        private readonly IChiTieuRuleRepository _repository;

        public ChiTieuRuleService(IChiTieuRuleRepository repository)
            => _repository = repository;

        private static ChiTieuRuleDto ToDto(CBM_ChiTieu_Rule e) => new()
        {
            ID_Rule    = e.ID_Rule,
            ID_ChiTieu = e.ID_ChiTieu,
            TenMuc     = e.TenMuc,
            Diem_Si    = e.Diem_Si,
            BieuThuc   = e.BieuThuc,
        };

        public async Task<IEnumerable<ChiTieuRuleDto>> GetByChiTieuAsync(int idChiTieu)
            => (await _repository.GetByChiTieuAsync(idChiTieu)).Select(ToDto);

        public async Task<ChiTieuRuleDto?> GetByIdAsync(int id)
        {
            var e = await _repository.GetByIdAsync(id);
            return e == null ? null : ToDto(e);
        }

        public async Task<ChiTieuRuleDto> CreateAsync(CreateChiTieuRuleDto dto)
        {
            var entity = new CBM_ChiTieu_Rule
            {
                ID_ChiTieu = dto.ID_ChiTieu,
                TenMuc     = dto.TenMuc,
                Diem_Si    = dto.Diem_Si,
                BieuThuc   = dto.BieuThuc,
            };
            await _repository.AddAsync(entity);
            await _repository.SaveChangesAsync();
            return ToDto(entity);
        }

        public async Task<ChiTieuRuleDto?> UpdateAsync(int id, UpdateChiTieuRuleDto dto)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return null;

            entity.ID_ChiTieu = dto.ID_ChiTieu;
            entity.TenMuc     = dto.TenMuc;
            entity.Diem_Si    = dto.Diem_Si;
            entity.BieuThuc   = dto.BieuThuc;

            _repository.Update(entity);
            await _repository.SaveChangesAsync();
            return ToDto(entity);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return false;

            _repository.Delete(entity);
            await _repository.SaveChangesAsync();
            return true;
        }
    }
}
