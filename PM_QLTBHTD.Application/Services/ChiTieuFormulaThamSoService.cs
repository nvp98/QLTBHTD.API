using PM_QLTBHTD.Application.DTOs;
using PM_QLTBHTD.Application.Services.IService;
using PM_QLTBHTD.Domain.Entities;
using PM_QLTBHTD.Domain.IRepository;

namespace PM_QLTBHTD.Application.Services
{
    public class ChiTieuFormulaThamSoService : IChiTieuFormulaThamSoService
    {
        private readonly IChiTieuFormulaThamSoRepository _repository;

        public ChiTieuFormulaThamSoService(IChiTieuFormulaThamSoRepository repository)
            => _repository = repository;

        private static ChiTieuFormulaThamSoDto ToDto(CBM_ChiTieu_Formula_ThamSo e) => new()
        {
            ID_ThamSo       = e.ID_ThamSo,
            ID_Formula      = e.ID_Formula,
            MaThamSo        = e.MaThamSo,
            NguonGiaTri     = e.NguonGiaTri,
            MaInput         = e.MaInput,
            ID_FormulaNguon = e.ID_FormulaNguon,
            ID_ChiTieuNguon = e.ID_ChiTieuNguon,
            TenThuocTinhTB  = e.TenThuocTinhTB,
            GiaTriHangSo    = e.GiaTriHangSo,
        };

        public async Task<IEnumerable<ChiTieuFormulaThamSoDto>> GetByFormulaAsync(int idFormula)
            => (await _repository.GetByFormulaAsync(idFormula)).Select(ToDto);

        public async Task<ChiTieuFormulaThamSoDto?> GetByIdAsync(int id)
        {
            var e = await _repository.GetByIdAsync(id);
            return e == null ? null : ToDto(e);
        }

        public async Task<ChiTieuFormulaThamSoDto> CreateAsync(CreateChiTieuFormulaThamSoDto dto)
        {
            var entity = new CBM_ChiTieu_Formula_ThamSo
            {
                ID_Formula      = dto.ID_Formula,
                MaThamSo        = dto.MaThamSo,
                NguonGiaTri     = dto.NguonGiaTri,
                MaInput         = dto.MaInput,
                ID_FormulaNguon = dto.ID_FormulaNguon,
                ID_ChiTieuNguon = dto.ID_ChiTieuNguon,
                TenThuocTinhTB  = dto.TenThuocTinhTB,
                GiaTriHangSo    = dto.GiaTriHangSo,
            };
            await _repository.AddAsync(entity);
            await _repository.SaveChangesAsync();
            return ToDto(entity);
        }

        public async Task<ChiTieuFormulaThamSoDto?> UpdateAsync(int id, UpdateChiTieuFormulaThamSoDto dto)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return null;

            entity.ID_Formula      = dto.ID_Formula;
            entity.MaThamSo        = dto.MaThamSo;
            entity.NguonGiaTri     = dto.NguonGiaTri;
            entity.MaInput         = dto.MaInput;
            entity.ID_FormulaNguon = dto.ID_FormulaNguon;
            entity.ID_ChiTieuNguon = dto.ID_ChiTieuNguon;
            entity.TenThuocTinhTB  = dto.TenThuocTinhTB;
            entity.GiaTriHangSo    = dto.GiaTriHangSo;

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
