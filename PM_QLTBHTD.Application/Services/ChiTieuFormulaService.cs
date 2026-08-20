using Microsoft.EntityFrameworkCore;
using PM_QLTBHTD.Application.DTOs;
using PM_QLTBHTD.Application.Exceptions;
using PM_QLTBHTD.Application.Helpers;
using PM_QLTBHTD.Application.Interfaces;
using PM_QLTBHTD.Application.Services.IService;
using PM_QLTBHTD.Domain.Entities;
using PM_QLTBHTD.Domain.IRepository;

namespace PM_QLTBHTD.Application.Services
{
    public class ChiTieuFormulaService : IChiTieuFormulaService
    {
        private readonly IChiTieuFormulaRepository _repository;
        private readonly IChiTieuFormulaThamSoRepository _thamSoRepository;
        private readonly IAppDbContext _db;

        public ChiTieuFormulaService(
            IChiTieuFormulaRepository repository, IChiTieuFormulaThamSoRepository thamSoRepository, IAppDbContext db)
        {
            _repository = repository;
            _thamSoRepository = thamSoRepository;
            _db = db;
        }

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

        /// <summary>Config Validator — chặn SỚM trạng thái sẽ ném LoiFormulaException lúc chạy
        /// phiếu thật (xem FormulaRuleValidator). Tính số Formula active SAU KHI áp dụng thay đổi
        /// giả định (idFormulaDangSua = null khi Create, hoặc ID_Formula đang Update để loại trừ
        /// chính nó khỏi số đếm cũ trước khi cộng lại theo trạng thái mới).</summary>
        private async Task KiemTraThieuRuleGopTruocKhiLuuAsync(int idChiTieu, int? idFormulaDangSua, int trangThaiMoi)
        {
            var soFormulaKhac = await _db.ChiTieuFormulas
                .Where(f => f.ID_ChiTieu == idChiTieu && f.TrangThai == 1 && f.ID_Formula != (idFormulaDangSua ?? -1))
                .CountAsync();
            var soFormulaSauKhiLuu = soFormulaKhac + (trangThaiMoi == 1 ? 1 : 0);

            var soRule = await _db.ChiTieuRules.Where(r => r.ID_ChiTieu == idChiTieu).CountAsync();

            if (FormulaRuleValidator.SeThieuRuleGopFormula(soFormulaSauKhiLuu, soRule))
                throw new ThieuRuleGopFormulaException(idChiTieu, soFormulaSauKhiLuu);
        }

        public async Task<ChiTieuFormulaDto> CreateAsync(CreateChiTieuFormulaDto dto)
        {
            await KiemTraThieuRuleGopTruocKhiLuuAsync(dto.ID_ChiTieu, null, dto.TrangThai);

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

            await KiemTraThieuRuleGopTruocKhiLuuAsync(dto.ID_ChiTieu, id, dto.TrangThai);

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

            // Chặn nếu 1 tham số của Formula KHÁC đang lấy kết quả Formula này làm nguồn
            // (NguonGiaTri='FORMULA_KETQUA', ID_FormulaNguon=id) — xóa sẽ làm tham số đó dangling.
            var soThamSoThamChieu = await _db.ChiTieuFormulaThamSos.CountAsync(x => x.ID_FormulaNguon == id);
            if (soThamSoThamChieu > 0)
                throw new ChiTieuFormulaDangSuDungException(id, soThamSoThamChieu);

            // Tham số CỦA CHÍNH Formula này là sub-config thuần, không có ý nghĩa độc lập — cascade.
            var thamSoCuaFormula = await _thamSoRepository.FindAsync(x => x.ID_Formula == id);
            foreach (var ts in thamSoCuaFormula)
                _thamSoRepository.Delete(ts);
            await _thamSoRepository.SaveChangesAsync();

            _repository.Delete(entity);
            await _repository.SaveChangesAsync();
            return true;
        }
    }
}
