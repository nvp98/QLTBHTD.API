using Microsoft.EntityFrameworkCore;
using PM_QLTBHTD.Application.DTOs;
using PM_QLTBHTD.Application.Interfaces;
using PM_QLTBHTD.Application.Services.IService;
using PM_QLTBHTD.Domain.Entities;
using PM_QLTBHTD.Domain.IRepository;

namespace PM_QLTBHTD.Application.Services
{
    public class LichBaoTriService : ILichBaoTriService
    {
        private readonly ILichBaoTriRepository _repository;
        private readonly IAppDbContext _db;

        public LichBaoTriService(ILichBaoTriRepository repository, IAppDbContext db)
        {
            _repository = repository;
            _db = db;
        }

        public async Task<PagedResult<LichBaoTriDto>> GetPagedAsync(
            string? search, string? trangThai, int? idTram,
            DateTime? tuNgay, DateTime? denNgay, int page, int? pageSize)
        {
            var query =
                from lbt in _db.LichBaoTris
                join tb in _db.ThietBis on lbt.ID_ThietBi equals tb.ID_ThietBi
                join tr in _db.TramDiens on tb.ID_Tram equals tr.IDTram
                select new { lbt, tb, tr };

            if (!string.IsNullOrEmpty(search))
                query = query.Where(x => x.tb.TenThietBi.Contains(search));
            if (!string.IsNullOrEmpty(trangThai))
                query = query.Where(x => x.lbt.TrangThai == trangThai);
            if (idTram.HasValue)
                query = query.Where(x => x.tr.IDTram == idTram.Value);
            if (tuNgay.HasValue)
                query = query.Where(x => x.lbt.NgayKeHoach >= tuNgay.Value);
            if (denNgay.HasValue)
                query = query.Where(x => x.lbt.NgayKeHoach <= denNgay.Value);

            var total = await query.CountAsync();

            query = query.OrderBy(x => x.lbt.NgayKeHoach);
            if (pageSize.HasValue)
                query = query.Skip((page - 1) * pageSize.Value).Take(pageSize.Value);

            var items = await query.ToListAsync();

            return new PagedResult<LichBaoTriDto>
            {
                Items = items.Select(x => MapToDto(x.lbt, x.tb.TenThietBi, x.tb.ID_Tram, x.tr.TenTram)),
                Total = total,
                Page = page,
                PageSize = pageSize ?? total,
            };
        }

        public async Task<LichBaoTriDto?> GetByIdAsync(int id)
        {
            var row = await (
                from lbt in _db.LichBaoTris
                join tb in _db.ThietBis on lbt.ID_ThietBi equals tb.ID_ThietBi
                join tr in _db.TramDiens on tb.ID_Tram equals tr.IDTram
                where lbt.ID_LichBaoTri == id
                select new { lbt, tb, tr }
            ).FirstOrDefaultAsync();

            return row == null ? null : MapToDto(row.lbt, row.tb.TenThietBi, row.tb.ID_Tram, row.tr.TenTram);
        }

        public async Task<IEnumerable<LichBaoTriDto>> GetByThietBiAsync(int idThietBi)
        {
            var rows = await (
                from lbt in _db.LichBaoTris
                join tb in _db.ThietBis on lbt.ID_ThietBi equals tb.ID_ThietBi
                join tr in _db.TramDiens on tb.ID_Tram equals tr.IDTram
                where lbt.ID_ThietBi == idThietBi
                orderby lbt.NgayKeHoach descending
                select new { lbt, tb, tr }
            ).ToListAsync();

            return rows.Select(x => MapToDto(x.lbt, x.tb.TenThietBi, x.tb.ID_Tram, x.tr.TenTram));
        }

        public async Task<LichBaoTriDto> CreateAsync(CreateLichBaoTriDto dto)
        {
            var entity = new CBM_LichBaoTri
            {
                ID_ThietBi = dto.ID_ThietBi,
                LoaiBaoTri = dto.LoaiBaoTri,
                ChuKyThang = dto.LoaiBaoTri == "DinhKy" ? dto.ChuKyThang : null,
                NgayKeHoach = dto.NgayKeHoach,
                TrangThai = "ChoThucHien",
                NguoiPhuTrach = dto.NguoiPhuTrach,
                NoiDungCongViec = dto.NoiDungCongViec,
                GhiChu = dto.GhiChu,
            };
            await _repository.AddAsync(entity);
            await _repository.SaveChangesAsync();

            return (await GetByIdAsync(entity.ID_LichBaoTri))!;
        }

        public async Task<LichBaoTriDto?> UpdateAsync(int id, UpdateLichBaoTriDto dto)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return null;

            entity.LoaiBaoTri = dto.LoaiBaoTri;
            entity.ChuKyThang = dto.LoaiBaoTri == "DinhKy" ? dto.ChuKyThang : null;
            entity.NgayKeHoach = dto.NgayKeHoach;
            entity.NguoiPhuTrach = dto.NguoiPhuTrach;
            entity.NoiDungCongViec = dto.NoiDungCongViec;
            entity.GhiChu = dto.GhiChu;
            _repository.Update(entity);
            await _repository.SaveChangesAsync();

            return await GetByIdAsync(id);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return false;

            _repository.Delete(entity);
            await _repository.SaveChangesAsync();
            return true;
        }

        public async Task<LichBaoTriDto?> HoanThanhAsync(int id, HoanThanhLichBaoTriDto dto)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return null;

            entity.NgayThucHien = dto.NgayThucHien;
            entity.TrangThai = "HoanThanh";
            if (!string.IsNullOrEmpty(dto.GhiChu)) entity.GhiChu = dto.GhiChu;
            _repository.Update(entity);

            // Định kỳ: tự sinh lịch kế tiếp sau khi hoàn thành
            if (entity.ChuKyThang.HasValue)
            {
                var ke = new CBM_LichBaoTri
                {
                    ID_ThietBi = entity.ID_ThietBi,
                    ID_LichBaoTriGoc = entity.ID_LichBaoTriGoc ?? entity.ID_LichBaoTri,
                    LoaiBaoTri = entity.LoaiBaoTri,
                    ChuKyThang = entity.ChuKyThang,
                    NgayKeHoach = dto.NgayThucHien.AddMonths(entity.ChuKyThang.Value),
                    TrangThai = "ChoThucHien",
                    NguoiPhuTrach = entity.NguoiPhuTrach,
                    NoiDungCongViec = entity.NoiDungCongViec,
                };
                await _repository.AddAsync(ke);
            }

            await _repository.SaveChangesAsync();
            return await GetByIdAsync(id);
        }

        public async Task<LichBaoTriDto?> HuyAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return null;

            entity.TrangThai = "DaHuy";
            _repository.Update(entity);
            await _repository.SaveChangesAsync();

            return await GetByIdAsync(id);
        }

        public async Task<ThongKeLichBaoTriDto> GetThongKeAsync()
        {
            var today = DateTime.Today;
            var sapToiHanNgay = today.AddDays(7);
            var dauThang = new DateTime(today.Year, today.Month, 1);

            var rows = await (
                from lbt in _db.LichBaoTris
                join tb in _db.ThietBis on lbt.ID_ThietBi equals tb.ID_ThietBi
                join tr in _db.TramDiens on tb.ID_Tram equals tr.IDTram
                where lbt.TrangThai == "ChoThucHien"
                select new { lbt, tb, tr }
            ).ToListAsync();

            var quaHan = rows.Where(x => x.lbt.NgayKeHoach.Date < today).ToList();
            var sapToiHan = rows.Where(x => x.lbt.NgayKeHoach.Date >= today && x.lbt.NgayKeHoach.Date <= sapToiHanNgay).ToList();

            var soHoanThanhThangNay = await _db.LichBaoTris.CountAsync(x =>
                x.TrangThai == "HoanThanh" && x.NgayThucHien != null && x.NgayThucHien.Value >= dauThang);

            var canChuY = quaHan.OrderBy(x => x.lbt.NgayKeHoach)
                .Concat(sapToiHan.OrderBy(x => x.lbt.NgayKeHoach))
                .Take(8)
                .Select(x => MapToDto(x.lbt, x.tb.TenThietBi, x.tb.ID_Tram, x.tr.TenTram))
                .ToList();

            return new ThongKeLichBaoTriDto
            {
                TongDangCho = rows.Count,
                SoQuaHan = quaHan.Count,
                SoSapToiHan7Ngay = sapToiHan.Count,
                SoHoanThanhThangNay = soHoanThanhThangNay,
                DanhSachCanChuY = canChuY,
            };
        }

        private static LichBaoTriDto MapToDto(CBM_LichBaoTri x, string tenThietBi, int idTram, string tenTram) => new()
        {
            ID_LichBaoTri = x.ID_LichBaoTri,
            ID_ThietBi = x.ID_ThietBi,
            TenThietBi = tenThietBi,
            ID_Tram = idTram,
            TenTram = tenTram,
            ID_LichBaoTriGoc = x.ID_LichBaoTriGoc,
            LoaiBaoTri = x.LoaiBaoTri,
            ChuKyThang = x.ChuKyThang,
            NgayKeHoach = x.NgayKeHoach,
            NgayThucHien = x.NgayThucHien,
            TrangThai = x.TrangThai,
            TrangThaiHienThi = TinhTrangThaiHienThi(x),
            NguoiPhuTrach = x.NguoiPhuTrach,
            NoiDungCongViec = x.NoiDungCongViec,
            GhiChu = x.GhiChu,
            ID_PhieuKetQua = x.ID_PhieuKetQua,
            NgayTao = x.NgayTao,
        };

        private static string TinhTrangThaiHienThi(CBM_LichBaoTri x)
        {
            if (x.TrangThai != "ChoThucHien") return x.TrangThai;

            var soNgayConLai = (x.NgayKeHoach.Date - DateTime.Today).TotalDays;
            if (soNgayConLai < 0) return "QuaHan";
            if (soNgayConLai <= 7) return "SapToiHan";
            return "ChoThucHien";
        }
    }
}
