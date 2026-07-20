using PM_QLTBHTD.Application.DTOs;
using PM_QLTBHTD.Domain.Entities;
using PM_QLTBHTD.Domain.IRepository;

namespace PM_QLTBHTD.Application.Services
{
    public class ChiTieuFormulaService : IChiTieuFormulaService
    {
        private readonly IChiTieuFormulaRepository _repository;

        public ChiTieuFormulaService(IChiTieuFormulaRepository repository)
            => _repository = repository;

        private static ChiTieuFormulaDto ToDto(CBM_ChiTieu_Formula e) => new()
        {
            ID_Formula  = e.ID_Formula,
            ID_ChiTieu  = e.ID_ChiTieu,
            MaKetQua    = e.MaKetQua,
            ThuTu       = e.ThuTu,
            LoaiFormula = e.LoaiFormula,
            BieuThuc    = e.BieuThuc,
            TenFunction = e.TenFunction,
            TrangThai   = e.TrangThai,
            MoTa        = e.MoTa,
        };

        public async Task<IEnumerable<ChiTieuFormulaDto>> GetByChiTieuAsync(int idChiTieu)
            => (await _repository.GetByChiTieuAsync(idChiTieu)).Select(ToDto);

        public async Task<ChiTieuFormulaDto?> GetByIdAsync(int id)
        {
            var e = await _repository.GetByIdAsync(id);
            return e == null ? null : ToDto(e);
        }

        public async Task<ChiTieuFormulaDto> CreateAsync(CreateChiTieuFormulaDto dto)
        {
            var entity = new CBM_ChiTieu_Formula
            {
                ID_ChiTieu  = dto.ID_ChiTieu,
                MaKetQua    = dto.MaKetQua,
                ThuTu       = dto.ThuTu,
                LoaiFormula = dto.LoaiFormula,
                BieuThuc    = dto.BieuThuc,
                TenFunction = dto.TenFunction,
                TrangThai   = dto.TrangThai,
                MoTa        = dto.MoTa,
            };
            await _repository.AddAsync(entity);
            await _repository.SaveChangesAsync();
            return ToDto(entity);
        }

        public async Task<ChiTieuFormulaDto?> UpdateAsync(int id, UpdateChiTieuFormulaDto dto)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return null;

            entity.ID_ChiTieu  = dto.ID_ChiTieu;
            entity.MaKetQua    = dto.MaKetQua;
            entity.ThuTu       = dto.ThuTu;
            entity.LoaiFormula = dto.LoaiFormula;
            entity.BieuThuc    = dto.BieuThuc;
            entity.TenFunction = dto.TenFunction;
            entity.TrangThai   = dto.TrangThai;
            entity.MoTa        = dto.MoTa;

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
