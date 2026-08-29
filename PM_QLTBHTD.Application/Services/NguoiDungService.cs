using Microsoft.EntityFrameworkCore;
using PM_QLTBHTD.Application.DTOs;
using PM_QLTBHTD.Application.Exceptions;
using PM_QLTBHTD.Application.Interfaces;
using PM_QLTBHTD.Application.Services.IService;
using PM_QLTBHTD.Domain.Entities;
using PM_QLTBHTD.Domain.IRepository;

namespace PM_QLTBHTD.Application.Services
{
    public class NguoiDungService : INguoiDungService
    {
        private readonly INguoiDungRepository _repository;
        private readonly IAppDbContext _db;

        public NguoiDungService(INguoiDungRepository repository, IAppDbContext db)
        {
            _repository = repository;
            _db = db;
        }

        public async Task<IEnumerable<NguoiDungDto>> GetAllAsync()
        {
            var query =
                from nd in _db.NguoiDungs
                join vt in _db.VaiTros on nd.ID_VaiTro equals vt.ID_VaiTro
                join tr in _db.TramDiens on nd.ID_Tram equals tr.IDTram into trGroup
                from tr in trGroup.DefaultIfEmpty()
                orderby nd.HoTen
                select new NguoiDungDto
                {
                    ID_NguoiDung = nd.ID_NguoiDung,
                    TenDangNhap = nd.TenDangNhap,
                    HoTen = nd.HoTen,
                    Email = nd.Email,
                    ID_VaiTro = nd.ID_VaiTro,
                    MaVaiTro = vt.MaVaiTro,
                    TenVaiTro = vt.TenVaiTro,
                    ID_Tram = nd.ID_Tram,
                    TenTram = tr != null ? tr.TenTram : null,
                    TrangThai = nd.TrangThai,
                    NgayTao = nd.NgayTao,
                };
            return await query.ToListAsync();
        }

        public async Task<NguoiDungDto?> GetByIdAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            return entity == null ? null : await MapToDtoAsync(entity);
        }

        public async Task<NguoiDungDto> CreateAsync(CreateNguoiDungDto dto)
        {
            var existing = await _repository.GetByTenDangNhapAsync(dto.TenDangNhap);
            if (existing != null)
                throw new TenDangNhapDaTonTaiException(dto.TenDangNhap);

            var entity = new CBM_NguoiDung
            {
                TenDangNhap = dto.TenDangNhap,
                MatKhau_Hash = BCrypt.Net.BCrypt.HashPassword(dto.MatKhau),
                HoTen = dto.HoTen,
                Email = dto.Email,
                ID_VaiTro = dto.ID_VaiTro,
                ID_Tram = dto.ID_Tram,
                TrangThai = 1,
                NgayTao = DateTime.Now,
            };
            await _repository.AddAsync(entity);
            await _repository.SaveChangesAsync();
            return await MapToDtoAsync(entity);
        }

        public async Task<NguoiDungDto?> UpdateAsync(int id, UpdateNguoiDungDto dto)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return null;

            entity.HoTen = dto.HoTen;
            entity.Email = dto.Email;
            entity.ID_VaiTro = dto.ID_VaiTro;
            entity.ID_Tram = dto.ID_Tram;
            entity.TrangThai = dto.TrangThai;
            _repository.Update(entity);
            await _repository.SaveChangesAsync();
            return await MapToDtoAsync(entity);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            // Không xóa cứng tài khoản — chỉ khóa, để giữ nguyên lịch sử NguoiKiemTra/NguoiPhuTrach
            // đã tham chiếu tên người dùng này ở các phiếu/lịch bảo trì trước đó.
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return false;

            entity.TrangThai = 0;
            _repository.Update(entity);
            await _repository.SaveChangesAsync();
            return true;
        }

        public async Task DoiMatKhauAsync(int idNguoiDung, DoiMatKhauDto dto)
        {
            var entity = await _repository.GetByIdAsync(idNguoiDung)
                ?? throw new SaiTaiKhoanMatKhauException();

            if (!BCrypt.Net.BCrypt.Verify(dto.MatKhauCu, entity.MatKhau_Hash))
                throw new MatKhauCuKhongDungException();

            entity.MatKhau_Hash = BCrypt.Net.BCrypt.HashPassword(dto.MatKhauMoi);
            _repository.Update(entity);
            await _repository.SaveChangesAsync();
        }

        public async Task DatLaiMatKhauAsync(int idNguoiDung, DatLaiMatKhauDto dto)
        {
            var entity = await _repository.GetByIdAsync(idNguoiDung);
            if (entity == null) return;

            entity.MatKhau_Hash = BCrypt.Net.BCrypt.HashPassword(dto.MatKhauMoi);
            _repository.Update(entity);
            await _repository.SaveChangesAsync();
        }

        private async Task<NguoiDungDto> MapToDtoAsync(CBM_NguoiDung x)
        {
            var vaiTro = await _db.VaiTros.FirstAsync(v => v.ID_VaiTro == x.ID_VaiTro);
            var tram = x.ID_Tram.HasValue
                ? await _db.TramDiens.FirstOrDefaultAsync(t => t.IDTram == x.ID_Tram)
                : null;

            return new NguoiDungDto
            {
                ID_NguoiDung = x.ID_NguoiDung,
                TenDangNhap = x.TenDangNhap,
                HoTen = x.HoTen,
                Email = x.Email,
                ID_VaiTro = x.ID_VaiTro,
                MaVaiTro = vaiTro.MaVaiTro,
                TenVaiTro = vaiTro.TenVaiTro,
                ID_Tram = x.ID_Tram,
                TenTram = tram?.TenTram,
                TrangThai = x.TrangThai,
                NgayTao = x.NgayTao,
            };
        }
    }
}
