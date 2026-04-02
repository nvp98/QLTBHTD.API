using Microsoft.EntityFrameworkCore;
using PM_QLTBHTD.Application.DTOs;
using PM_QLTBHTD.Application.Interfaces;
using PM_QLTBHTD.Domain.Entities;
using PM_QLTBHTD.Domain.IRepository;

namespace PM_QLTBHTD.Application.Services
{
    public class PhieuKiemTraService : IPhieuKiemTraService
    {
        private readonly IPhieuKiemTraRepository _phieuRepo;
        private readonly IChiTietKiemTraRepository _chiTietRepo;
        private readonly INguongRepository _nguongRepo;
        private readonly IAppDbContext _db;

        public PhieuKiemTraService(
            IPhieuKiemTraRepository phieuRepo,
            IChiTietKiemTraRepository chiTietRepo,
            INguongRepository nguongRepo,
            IAppDbContext db)
        {
            _phieuRepo = phieuRepo;
            _chiTietRepo = chiTietRepo;
            _nguongRepo = nguongRepo;
            _db = db;
        }

        private IQueryable<PhieuKiemTraDto> JoinQuery()
        {
            return from p in _db.PhieuKiemTras
                   join tb in _db.ThietBis on p.ID_ThietBi equals tb.ID_ThietBi
                   select new PhieuKiemTraDto
                   {
                       ID_Phieu = p.ID_Phieu,
                       ID_ThietBi = p.ID_ThietBi,
                       TenThietBi = tb.TenThietBi,
                       NgayKiemTra = p.NgayKiemTra,
                       NguoiKiemTra = p.NguoiKiemTra,
                       TongDiem_Soqt = p.TongDiem_Soqt,
                       CapDoCanhBao = p.CapDoCanhBao,
                       GhiChuChung = p.GhiChuChung
                   };
        }

        public async Task<PagedResult<PhieuKiemTraDto>> GetPagedAsync(string? search, int page, int pageSize)
        {
            var query = JoinQuery().Where(x =>
                string.IsNullOrEmpty(search)
                || x.TenThietBi.Contains(search)
                || (x.NguoiKiemTra != null && x.NguoiKiemTra.Contains(search)));

            var total = await query.CountAsync();
            var items = await query.OrderByDescending(x => x.NgayKiemTra)
                                   .Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PagedResult<PhieuKiemTraDto> { Items = items, Total = total, Page = page, PageSize = pageSize };
        }

        public async Task<IEnumerable<PhieuKiemTraDto>> GetByThietBiAsync(int idThietBi)
            => await JoinQuery().Where(x => x.ID_ThietBi == idThietBi)
                                .OrderByDescending(x => x.NgayKiemTra).ToListAsync();

        public async Task<IEnumerable<PhieuKiemTraDto>> GetByNgayAsync(DateTime tuNgay, DateTime denNgay)
            => await JoinQuery().Where(x => x.NgayKiemTra >= tuNgay && x.NgayKiemTra <= denNgay)
                                .OrderByDescending(x => x.NgayKiemTra).ToListAsync();

        public async Task<PhieuKiemTraDto?> GetByIdAsync(int id)
            => await JoinQuery().FirstOrDefaultAsync(x => x.ID_Phieu == id);

        public async Task<PhieuKiemTraDetailDto?> GetDetailAsync(int idPhieu)
        {
            var phieu = await JoinQuery().FirstOrDefaultAsync(x => x.ID_Phieu == idPhieu);
            if (phieu == null) return null;

            var chiTiets = await (from ct in _db.ChiTietKiemTras
                                  join c in _db.ChiTieus on ct.ID_ChiTieu equals c.ID_ChiTieu
                                  where ct.IDPhieu == idPhieu
                                  select new ChiTietKiemTraDto
                                  {
                                      ID_ChiTiet = ct.ID_ChiTiet,
                                      IDPhieu = ct.IDPhieu,
                                      ID_ChiTieu = ct.ID_ChiTieu,
                                      TenChiTieu = c.TenChiTieu,
                                      GiaTriNhap_So = ct.GiaTriNhap_So,
                                      GiaTriNhap_Chu = ct.GiaTriNhap_Chu,
                                      Diem_Si_DatDuoc = ct.Diem_Si_DatDuoc,
                                      GhiChu = ct.GhiChu
                                  }).ToListAsync();

            return new PhieuKiemTraDetailDto
            {
                ID_Phieu = phieu.ID_Phieu,
                ID_ThietBi = phieu.ID_ThietBi,
                TenThietBi = phieu.TenThietBi,
                NgayKiemTra = phieu.NgayKiemTra,
                NguoiKiemTra = phieu.NguoiKiemTra,
                TongDiem_Soqt = phieu.TongDiem_Soqt,
                CapDoCanhBao = phieu.CapDoCanhBao,
                GhiChuChung = phieu.GhiChuChung,
                ChiTiets = chiTiets
            };
        }

        public async Task<PhieuKiemTraDto> CreateAsync(CreatePhieuKiemTraDto dto)
        {
            var phieu = new PhieuKiemTra
            {
                ID_ThietBi = dto.ID_ThietBi,
                NgayKiemTra = dto.NgayKiemTra,
                NguoiKiemTra = dto.NguoiKiemTra,
                GhiChuChung = dto.GhiChuChung
            };
            await _phieuRepo.AddAsync(phieu);
            await _phieuRepo.SaveChangesAsync();

            decimal tongDiem = 0;
            foreach (var ct in dto.ChiTiets)
            {
                var diemDat = await TinhDiemAsync(ct.ID_ChiTieu, ct.GiaTriNhap_So);
                var chiTiet = new ChiTietKiemTra
                {
                    IDPhieu = phieu.ID_Phieu,
                    ID_ChiTieu = ct.ID_ChiTieu,
                    GiaTriNhap_So = ct.GiaTriNhap_So,
                    GiaTriNhap_Chu = ct.GiaTriNhap_Chu,
                    Diem_Si_DatDuoc = diemDat,
                    GhiChu = ct.GhiChu
                };
                await _chiTietRepo.AddAsync(chiTiet);
                tongDiem += diemDat ?? 0;
            }

            phieu.TongDiem_Soqt = tongDiem;
            phieu.CapDoCanhBao = XepLoaiCapDo(tongDiem);
            _phieuRepo.Update(phieu);
            await _phieuRepo.SaveChangesAsync();

            return (await GetByIdAsync(phieu.ID_Phieu))!;
        }

        public async Task<PhieuKiemTraDto?> UpdateAsync(int id, UpdatePhieuKiemTraDto dto)
        {
            var entity = await _phieuRepo.GetByIdAsync(id);
            if (entity == null) return null;

            entity.ID_ThietBi = dto.ID_ThietBi;
            entity.NgayKiemTra = dto.NgayKiemTra;
            entity.NguoiKiemTra = dto.NguoiKiemTra;
            entity.TongDiem_Soqt = dto.TongDiem_Soqt;
            entity.CapDoCanhBao = dto.CapDoCanhBao;
            entity.GhiChuChung = dto.GhiChuChung;
            _phieuRepo.Update(entity);
            await _phieuRepo.SaveChangesAsync();
            return await GetByIdAsync(id);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _phieuRepo.GetByIdAsync(id);
            if (entity == null) return false;

            await _chiTietRepo.DeleteByPhieuAsync(id);
            _phieuRepo.Delete(entity);
            await _phieuRepo.SaveChangesAsync();
            return true;
        }

        private async Task<decimal?> TinhDiemAsync(int idChiTieu, decimal? giaTriSo)
        {
            if (giaTriSo == null) return null;
            var nguongs = await _nguongRepo.GetByChiTieuAsync(idChiTieu);
            foreach (var ng in nguongs)
            {
                if (giaTriSo >= ng.CanDuoi && giaTriSo <= ng.CanTren)
                    return ng.Diem_Si;
            }
            return null;
        }

        private static string XepLoaiCapDo(decimal tongDiem) => tongDiem switch
        {
            >= 85 => "A - Tốt",
            >= 70 => "B - Khá",
            >= 50 => "C - Trung bình",
            >= 30 => "D - Kém",
            _ => "E - Nguy hiểm"
        };
    }
}
