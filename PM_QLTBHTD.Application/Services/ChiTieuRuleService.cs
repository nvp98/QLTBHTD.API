using Microsoft.EntityFrameworkCore;
using PM_QLTBHTD.Application.DTOs;
using PM_QLTBHTD.Application.Exceptions;
using PM_QLTBHTD.Application.Helpers;
using PM_QLTBHTD.Application.Interfaces;
using PM_QLTBHTD.Domain.Entities;
using PM_QLTBHTD.Domain.IRepository;

namespace PM_QLTBHTD.Application.Services
{
    public class ChiTieuRuleService : IChiTieuRuleService
    {
        private readonly IChiTieuRuleRepository _repository;
        private readonly IAppDbContext _db;

        public ChiTieuRuleService(IChiTieuRuleRepository repository, IAppDbContext db)
        {
            _repository = repository;
            _db = db;
        }

        /// <summary>Config Validator — chặn SỚM việc xoá/di chuyển Rule cuối cùng của 1 chỉ tiêu
        /// đang có ≥2 Formula active (xem FormulaRuleValidator), vì lúc đó chỉ tiêu sẽ rơi lại đúng
        /// trạng thái ném LoiFormulaException khi chạy phiếu thật.</summary>
        private async Task KiemTraThieuRuleGopTruocKhiXoaAsync(int idChiTieu, int idRuleDangXoa)
        {
            var soRuleKhac = await _db.ChiTieuRules
                .Where(r => r.ID_ChiTieu == idChiTieu && r.ID_Rule != idRuleDangXoa)
                .CountAsync();
            if (soRuleKhac > 0) return; // vẫn còn Rule khác cho chỉ tiêu này — an toàn

            var soFormula = await _db.ChiTieuFormulas
                .Where(f => f.ID_ChiTieu == idChiTieu && f.TrangThai == 1)
                .CountAsync();

            if (FormulaRuleValidator.SeThieuRuleGopFormula(soFormula, 0))
                throw new ThieuRuleGopFormulaException(idChiTieu, soFormula);
        }

        private static ChiTieuRuleDto ToDto(CBM_ChiTieu_Rule e) => new()
        {
            ID_Rule    = e.ID_Rule,
            ID_ChiTieu = e.ID_ChiTieu,
            TenMuc     = e.TenMuc,
            Diem_Si    = e.Diem_Si,
            BieuThuc   = e.BieuThuc,
            LoaiRule   = e.LoaiRule,
            HanhDongKhuyenCao = e.HanhDongKhuyenCao,
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
                LoaiRule   = dto.LoaiRule,
                HanhDongKhuyenCao = dto.HanhDongKhuyenCao,
            };
            await _repository.AddAsync(entity);
            await _repository.SaveChangesAsync();
            return ToDto(entity);
        }

        public async Task<ChiTieuRuleDto?> UpdateAsync(int id, UpdateChiTieuRuleDto dto)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return null;

            if (dto.ID_ChiTieu != entity.ID_ChiTieu)
                await KiemTraThieuRuleGopTruocKhiXoaAsync(entity.ID_ChiTieu, entity.ID_Rule);

            entity.ID_ChiTieu = dto.ID_ChiTieu;
            entity.TenMuc     = dto.TenMuc;
            entity.Diem_Si    = dto.Diem_Si;
            entity.BieuThuc   = dto.BieuThuc;
            entity.LoaiRule   = dto.LoaiRule;
            entity.HanhDongKhuyenCao = dto.HanhDongKhuyenCao;

            _repository.Update(entity);
            await _repository.SaveChangesAsync();
            return ToDto(entity);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return false;

            await KiemTraThieuRuleGopTruocKhiXoaAsync(entity.ID_ChiTieu, entity.ID_Rule);

            _repository.Delete(entity);
            await _repository.SaveChangesAsync();
            return true;
        }
    }
}
