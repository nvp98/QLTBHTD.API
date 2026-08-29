using Microsoft.EntityFrameworkCore;
using PM_QLTBHTD.Application.DTOs;
using PM_QLTBHTD.Application.Exceptions;
using PM_QLTBHTD.Application.Interfaces;
using PM_QLTBHTD.Application.Services.IService;
using PM_QLTBHTD.Domain.Entities;
using PM_QLTBHTD.Domain.IRepository;

namespace PM_QLTBHTD.Application.Services
{
    public class NhomChiTieuService : INhomChiTieuService
    {
        private readonly INhomChiTieuRepository _repository;
        private readonly IAppDbContext _db;

        public NhomChiTieuService(INhomChiTieuRepository repository, IAppDbContext db)
        {
            _repository = repository;
            _db = db;
        }

        private IQueryable<NhomChiTieuDto> JoinQuery()
        {
            return from n in _db.NhomChiTieus
                   join l in _db.LoaiThietBis on n.ID_LoaiThietBi equals l.ID_LoaiThietBi
                   select new NhomChiTieuDto
                   {
                       ID_NhomChiTieu = n.ID_NhomChiTieu,
                       TenNhom = n.TenNhom,
                       ID_LoaiThietBi = n.ID_LoaiThietBi,
                       TenLoaiThietBi = l.TenLoaiTB,
                       ID_NhomCha = n.ID_NhomCha,
                       CapDo = n.CapDo,
                       LoaiNhom = n.LoaiNhom,
                       PhienBan = n.PhienBan,
                       TrangThai = n.TrangThai,
                       Tier = n.Tier,
                       TrongSo_Wi = n.TrongSo_Wi,
                       CoCongThuc = _db.CongThucTongHops.Any(c => c.ID_NhomChiTieu == n.ID_NhomChiTieu && c.TrangThai == 1),
                       SoChiTieu = _db.ChiTieus.Count(c => c.ID_NhomChiTieu == n.ID_NhomChiTieu && c.TrangThai == 1)
                   };
        }

        public async Task<PagedResult<NhomChiTieuDto>> GetPagedAsync(string? search, int page, int? pageSize)
        {
            var query = JoinQuery().Where(x =>
                string.IsNullOrEmpty(search)
                || x.TenNhom.Contains(search)
                || x.TenLoaiThietBi.Contains(search));

            var total = await query.CountAsync();
            if (pageSize.HasValue)
                query = query.Skip((page - 1) * pageSize.Value).Take(pageSize.Value);
            var items = await query.ToListAsync();
            return new PagedResult<NhomChiTieuDto> { Items = items, Total = total, Page = page, PageSize = pageSize ?? total };
        }

        public async Task<IEnumerable<NhomChiTieuDto>> GetAllActiveAsync()
            => await JoinQuery().Where(x => x.TrangThai == 1).ToListAsync();

        public async Task<IEnumerable<NhomChiTieuDto>> GetByLoaiThietBiAsync(int idLoaiThietBi)
            => await JoinQuery().Where(x => x.ID_LoaiThietBi == idLoaiThietBi).ToListAsync();

        /// <summary>Chỉ trả về nhóm CÓ chỉ tiêu trực tiếp để nhập — loại bỏ nhóm tổng hợp thuần
        /// (vd CHI1/CHI2/CHI3/TS1/TS2, SoChiTieu=0) vì chọn nhóm đó ở màn Tạo phiếu sẽ ra form trống,
        /// điểm của chúng chỉ tự gộp lên từ nhóm con chứ không có chỗ nhập liệu trực tiếp.</summary>
        public async Task<IEnumerable<NhomChiTieuDto>> GetKhaDungNhapLieuAsync(int idLoaiThietBi)
            => await JoinQuery()
                .Where(x => x.ID_LoaiThietBi == idLoaiThietBi && x.TrangThai == 1 && x.SoChiTieu > 0)
                .ToListAsync();

        public async Task<NhomChiTieuDto?> GetByIdAsync(int id)
            => await JoinQuery().FirstOrDefaultAsync(x => x.ID_NhomChiTieu == id);

        public async Task<NhomChiTieuDto> CreateAsync(CreateNhomChiTieuDto dto)
        {
            var entity = new CBM_NhomChiTieu
            {
                TenNhom = dto.TenNhom,
                ID_LoaiThietBi = dto.ID_LoaiThietBi,
                ID_NhomCha = dto.ID_NhomCha,
                CapDo = dto.CapDo,
                LoaiNhom = dto.LoaiNhom,
                PhienBan = dto.PhienBan,
                TrangThai = dto.TrangThai,
                Tier = dto.Tier,
                TrongSo_Wi = dto.TrongSo_Wi
            };
            await _repository.AddAsync(entity);
            await _repository.SaveChangesAsync();
            return (await GetByIdAsync(entity.ID_NhomChiTieu))!;
        }

        public async Task<NhomChiTieuDto?> UpdateAsync(int id, UpdateNhomChiTieuDto dto)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return null;

            entity.TenNhom = dto.TenNhom;
            entity.ID_LoaiThietBi = dto.ID_LoaiThietBi;
            entity.ID_NhomCha = dto.ID_NhomCha;
            entity.CapDo = dto.CapDo;
            entity.LoaiNhom = dto.LoaiNhom;
            entity.PhienBan = dto.PhienBan;
            entity.TrangThai = dto.TrangThai;
            entity.Tier = dto.Tier;
            entity.TrongSo_Wi = dto.TrongSo_Wi;
            _repository.Update(entity);
            await _repository.SaveChangesAsync();
            return await GetByIdAsync(id);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return false;

            var soChiTieu = await _db.ChiTieus.CountAsync(x => x.ID_NhomChiTieu == id);
            var soNhomCon = await _db.NhomChiTieus.CountAsync(x => x.ID_NhomCha == id);
            var soPhieu = await _db.PhieuKiemTras.CountAsync(x => x.ID_NhomChiTieu == id);
            var soCongThucThamChieu = await _db.CongThucBiens.CountAsync(x => x.ID_NhomCon == id);
            if (soChiTieu > 0 || soNhomCon > 0 || soPhieu > 0 || soCongThucThamChieu > 0)
                throw new NhomChiTieuDangSuDungException(id, soChiTieu, soNhomCon, soPhieu, soCongThucThamChieu);

            _repository.Delete(entity);
            await _repository.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<NhomChiTieuCayDto>> GetCayAsync(int idLoaiThietBi)
        {
            var tatCa = await JoinQuery()
                .Where(x => x.ID_LoaiThietBi == idLoaiThietBi && x.TrangThai == 1)
                .ToListAsync();

            var cayDtos = tatCa.Select(x => new NhomChiTieuCayDto
            {
                ID_NhomChiTieu = x.ID_NhomChiTieu,
                TenNhom = x.TenNhom,
                ID_LoaiThietBi = x.ID_LoaiThietBi,
                TenLoaiThietBi = x.TenLoaiThietBi,
                ID_NhomCha = x.ID_NhomCha,
                CapDo = x.CapDo,
                LoaiNhom = x.LoaiNhom,
                PhienBan = x.PhienBan,
                TrangThai = x.TrangThai,
                Tier = x.Tier,
                TrongSo_Wi = x.TrongSo_Wi,
                CoCongThuc = x.CoCongThuc
            }).ToList();

            var lookup = cayDtos.ToDictionary(x => x.ID_NhomChiTieu);
            var roots = new List<NhomChiTieuCayDto>();

            foreach (var node in cayDtos)
            {
                if (node.ID_NhomCha.HasValue && lookup.TryGetValue(node.ID_NhomCha.Value, out var parent))
                    parent.NhomCon.Add(node);
                else
                    roots.Add(node);
            }

            return roots;
        }
    }
}
