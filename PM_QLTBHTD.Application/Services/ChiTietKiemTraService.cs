using Microsoft.EntityFrameworkCore;
using PM_QLTBHTD.Application.DTOs;
using PM_QLTBHTD.Application.Interfaces;
using PM_QLTBHTD.Application.Services.IService;
using PM_QLTBHTD.Domain.Entities;
using PM_QLTBHTD.Domain.IRepository;

namespace PM_QLTBHTD.Application.Services
{
    public class ChiTietKiemTraService : IChiTietKiemTraService
    {
        private readonly IChiTietKiemTraRepository _repository;
        private readonly IChiTietKiemTraInputRepository _inputRepository;
        private readonly IAppDbContext _db;

        public ChiTietKiemTraService(
            IChiTietKiemTraRepository repository, IChiTietKiemTraInputRepository inputRepository, IAppDbContext db)
        {
            _repository = repository;
            _inputRepository = inputRepository;
            _db = db;
        }

        private IQueryable<ChiTietKiemTraDto> JoinQuery()
        {
            return from ct in _db.ChiTietKiemTras
                   join c in _db.ChiTieus on ct.ID_ChiTieu equals c.ID_ChiTieu
                   join n in _db.NhomChiTieus on c.ID_NhomChiTieu equals n.ID_NhomChiTieu
                   select new ChiTietKiemTraDto
                   {
                       ID_ChiTiet = ct.ID_ChiTiet,
                       IDPhieu = ct.IDPhieu,
                       ID_ChiTieu = ct.ID_ChiTieu,
                       TenChiTieu = c.TenChiTieu,
                       ID_NhomChiTieu = n.ID_NhomChiTieu,
                       TenNhom = n.TenNhom,
                       GiaTriNhap_So = ct.GiaTriNhap_So,
                       GiaTriNhap_Chu = ct.GiaTriNhap_Chu,
                       Diem_Si_DatDuoc = ct.Diem_Si_DatDuoc,
                       HanhDongKhuyenCao = ct.HanhDongKhuyenCao,
                       GhiChu = ct.GhiChu
                   };
        }

        public async Task<IEnumerable<ChiTietKiemTraDto>> GetByPhieuAsync(int idPhieu)
            => await JoinQuery().Where(x => x.IDPhieu == idPhieu).ToListAsync();

        public async Task<ChiTietKiemTraDto?> GetByIdAsync(int id)
            => await JoinQuery().FirstOrDefaultAsync(x => x.ID_ChiTiet == id);

        public async Task<ChiTietKiemTraDto> CreateAsync(int idPhieu, CreateChiTietKiemTraDto dto)
        {
            var entity = new ChiTietKiemTra
            {
                IDPhieu = idPhieu,
                ID_ChiTieu = dto.ID_ChiTieu,
                GiaTriNhap_So = dto.GiaTriNhap_So,
                GiaTriNhap_Chu = dto.GiaTriNhap_Chu,
                GhiChu = dto.GhiChu
            };
            await _repository.AddAsync(entity);
            await _repository.SaveChangesAsync();
            return (await GetByIdAsync(entity.ID_ChiTiet))!;
        }

        public async Task<ChiTietKiemTraDto?> UpdateAsync(int id, UpdateChiTietKiemTraDto dto)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return null;

            entity.IDPhieu = dto.IDPhieu;
            entity.ID_ChiTieu = dto.ID_ChiTieu;
            entity.GiaTriNhap_So = dto.GiaTriNhap_So;
            entity.GiaTriNhap_Chu = dto.GiaTriNhap_Chu;
            entity.Diem_Si_DatDuoc = dto.Diem_Si_DatDuoc;
            entity.GhiChu = dto.GhiChu;
            _repository.Update(entity);
            await _repository.SaveChangesAsync();
            return await GetByIdAsync(id);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return false;

            // ChiTietKiemTra_Input khớp theo (IDPhieu, ID_ChiTieu) — không theo ID_ChiTiet — nên phải
            // dọn thủ công ở đây, khác với cascade qua IDPhieu khi xóa cả PhieuKiemTra.
            var inputs = await _inputRepository.FindAsync(
                x => x.IDPhieu == entity.IDPhieu && x.ID_ChiTieu == entity.ID_ChiTieu);
            foreach (var inp in inputs)
                _inputRepository.Delete(inp);
            await _inputRepository.SaveChangesAsync();

            _repository.Delete(entity);
            await _repository.SaveChangesAsync();
            return true;
        }
    }
}
