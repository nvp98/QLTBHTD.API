using PM_QLTBHTD.Application.DTOs;
using PM_QLTBHTD.Domain.Entities;
using PM_QLTBHTD.Domain.IRepository;

namespace PM_QLTBHTD.Application.Services
{
    public class PhieuKiemTraService : IPhieuKiemTraService
    {
        private readonly IPhieuKiemTraRepository _phieuRepo;
        private readonly IChiTietKiemTraRepository _chiTietRepo;
        private readonly INguongRepository _nguongRepo;

        public PhieuKiemTraService(
            IPhieuKiemTraRepository phieuRepo,
            IChiTietKiemTraRepository chiTietRepo,
            INguongRepository nguongRepo)
        {
            _phieuRepo = phieuRepo;
            _chiTietRepo = chiTietRepo;
            _nguongRepo = nguongRepo;
        }

        public async Task<IEnumerable<PhieuKiemTraDto>> GetAllAsync()
        {
            var items = await _phieuRepo.GetAllAsync();
            return items.Select(x => MapToDto(x));
        }

        public async Task<IEnumerable<PhieuKiemTraDto>> GetByThietBiAsync(int idThietBi)
        {
            var items = await _phieuRepo.GetByThietBiAsync(idThietBi);
            return items.Select(x => MapToDto(x));
        }

        public async Task<IEnumerable<PhieuKiemTraDto>> GetByNgayAsync(DateTime tuNgay, DateTime denNgay)
        {
            var items = await _phieuRepo.GetByNgayKiemTraAsync(tuNgay, denNgay);
            return items.Select(x => MapToDto(x));
        }

        public async Task<PhieuKiemTraDetailDto?> GetDetailAsync(int idPhieu)
        {
            var phieu = await _phieuRepo.GetWithChiTietAsync(idPhieu);
            if (phieu == null) return null;

            var chiTiets = await _chiTietRepo.GetByPhieuAsync(idPhieu);
            return new PhieuKiemTraDetailDto
            {
                ID_Phieu = phieu.ID_Phieu,
                ID_ThietBi = phieu.ID_ThietBi,
                NgayKiemTra = phieu.NgayKiemTra,
                NguoiKiemTra = phieu.NguoiKiemTra,
                TongDiem_Soqt = phieu.TongDiem_Soqt,
                CapDoCanhBao = phieu.CapDoCanhBao,
                GhiChuChung = phieu.GhiChuChung,
                ChiTiets = chiTiets.Select(ct => new ChiTietKiemTraDto
                {
                    ID_ChiTiet = ct.ID_ChiTiet,
                    IDPhieu = ct.IDPhieu,
                    ID_ChiTieu = ct.ID_ChiTieu,
                    GiaTriNhap_So = ct.GiaTriNhap_So,
                    GiaTriNhap_Chu = ct.GiaTriNhap_Chu,
                    Diem_Si_DatDuoc = ct.Diem_Si_DatDuoc,
                    GhiChu = ct.GhiChu
                }).ToList()
            };
        }

        public async Task<PhieuKiemTraDto?> GetByIdAsync(int id)
        {
            var item = await _phieuRepo.GetByIdAsync(id);
            return item == null ? null : MapToDto(item);
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

            return MapToDto(phieu);
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
            return MapToDto(entity);
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

        private static PhieuKiemTraDto MapToDto(PhieuKiemTra x) => new()
        {
            ID_Phieu = x.ID_Phieu,
            ID_ThietBi = x.ID_ThietBi,
            NgayKiemTra = x.NgayKiemTra,
            NguoiKiemTra = x.NguoiKiemTra,
            TongDiem_Soqt = x.TongDiem_Soqt,
            CapDoCanhBao = x.CapDoCanhBao,
            GhiChuChung = x.GhiChuChung
        };
    }
}
