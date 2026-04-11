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
                   join nhom in _db.NhomChiTieus on p.ID_NhomChiTieu equals nhom.ID_NhomChiTieu into nhomJoin
                   from nhom in nhomJoin.DefaultIfEmpty()
                   select new PhieuKiemTraDto
                   {
                       ID_Phieu       = p.ID_Phieu,
                       ID_ThietBi     = p.ID_ThietBi,
                       TenThietBi     = tb.TenThietBi,
                       ID_NhomChiTieu = p.ID_NhomChiTieu,
                       TenNhom        = nhom != null ? nhom.TenNhom : null,
                       NgayKiemTra    = p.NgayKiemTra,
                       NguoiKiemTra   = p.NguoiKiemTra,
                       TongDiem_Soqt  = p.TongDiem_Soqt,
                       CapDoCanhBao   = p.CapDoCanhBao,
                       GhiChuChung    = p.GhiChuChung
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
                ID_ThietBi     = dto.ID_ThietBi,
                ID_NhomChiTieu = dto.ID_NhomChiTieu,
                NgayKiemTra    = DateTime.Now,
                NguoiKiemTra   = dto.NguoiKiemTra,
                GhiChuChung    = dto.GhiChuChung,
                SoPhieu        = await GenerateSoPhieuAsync(dto.ID_ThietBi, DateTime.Now)
            };
            await _phieuRepo.AddAsync(phieu);
            await _phieuRepo.SaveChangesAsync();

            // Lưu chi tiết và thu thập (idChiTieu, diemDat) để tính CSSK
            var ketQuaChiTiets = new List<(int IdChiTieu, decimal? DiemDat)>();
            foreach (var ct in dto.ChiTiets)
            {
                var diemDat = await TinhDiemAsync(ct.ID_ChiTieu, ct.GiaTriNhap_So);
                var chiTiet = new ChiTietKiemTra
                {
                    IDPhieu        = phieu.ID_Phieu,
                    ID_ChiTieu     = ct.ID_ChiTieu,
                    GiaTriNhap_So  = ct.GiaTriNhap_So,
                    GiaTriNhap_Chu = ct.GiaTriNhap_Chu,
                    Diem_Si_DatDuoc = diemDat,
                    GhiChu         = ct.GhiChu
                };
                await _chiTietRepo.AddAsync(chiTiet);
                ketQuaChiTiets.Add((ct.ID_ChiTieu, diemDat));
            }

            // Tính CSSK theo công thức có trọng số (kết quả 0-100)
            //phieu.TongDiem_Soqt = await TinhCSSKAsync(ketQuaChiTiets);
            //phieu.CapDoCanhBao  = XepLoaiCapDo(phieu.TongDiem_Soqt ?? 0);
            _phieuRepo.Update(phieu);
            await _phieuRepo.SaveChangesAsync();

            return (await GetByIdAsync(phieu.ID_Phieu))!;
        }

        public async Task<PhieuKiemTraDto?> UpdateAsync(int id, UpdatePhieuKiemTraDto dto)
        {
            var entity = await _phieuRepo.GetByIdAsync(id);
            if (entity == null) return null;

            entity.ID_ThietBi     = dto.ID_ThietBi;
            entity.ID_NhomChiTieu = dto.ID_NhomChiTieu;
            entity.NgayKiemTra    = dto.NgayKiemTra;
            entity.NguoiKiemTra   = dto.NguoiKiemTra;
            entity.TongDiem_Soqt  = dto.TongDiem_Soqt;
            entity.CapDoCanhBao   = dto.CapDoCanhBao;
            entity.GhiChuChung    = dto.GhiChuChung;
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

        /// <summary>
        /// Sinh mã phiếu theo format: PKT-{KyHieu}-{yyyyMMdd}-{STT:D3}
        /// Ví dụ: PKT-MBA01-20260410-003
        /// </summary>
        private async Task<string> GenerateSoPhieuAsync(int idThietBi, DateTime ngay)
        {
            // Lấy ký hiệu thiết bị (KyHieu của LoaiThietBi hoặc SoHieu của ThietBi)
            var tb = await _db.ThietBis
                .Where(t => t.ID_ThietBi == idThietBi)
                .Select(t => new { t.TenThietBi, t.ID_LoaiTB })
                .FirstOrDefaultAsync();

            string kyHieu = "TB";
            if (tb != null)
            {
                if (!string.IsNullOrWhiteSpace(tb.TenThietBi))
                {
                    kyHieu = tb.TenThietBi.Trim();
                }
                else
                {
                    var loai = await _db.LoaiThietBis
                        .Where(l => l.ID_LoaiThietBi == tb.ID_LoaiTB)
                        .Select(l => l.KyHieu)
                        .FirstOrDefaultAsync();
                    if (!string.IsNullOrWhiteSpace(loai))
                        kyHieu = loai!.Trim();
                }
            }

            // Chuẩn hoá: bỏ khoảng trắng, ký tự đặc biệt
            kyHieu = System.Text.RegularExpressions.Regex.Replace(kyHieu, @"[^\w]", "").ToUpper();

            string ngayStr = ngay.ToString("yyyyMMdd");

            // STT = số phiếu của thiết bị này trong ngày hôm nay + 1
            var ngayBatDau = ngay.Date;
            var ngayKetThuc = ngayBatDau.AddDays(1);
            var soPhieuHomNay = await _db.PhieuKiemTras
                .CountAsync(p => p.ID_ThietBi == idThietBi
                              && p.NgayKiemTra >= ngayBatDau
                              && p.NgayKiemTra < ngayKetThuc);

            int stt = soPhieuHomNay + 1;

            return $"PKT-{kyHieu}-{ngayStr}-{stt:D3}";
        }

        private async Task<decimal?> TinhDiemAsync(int idChiTieu, decimal? giaTriSo)
        {
            if (giaTriSo == null) return null;
            var nguongs = await _nguongRepo.GetByChiTieuAsync(idChiTieu);
            return nguongs.FirstOrDefault(ng => KiemTraNguong(giaTriSo.Value, ng))?.Diem_Si;
        }

        /// <summary>
        /// Kiểm tra giá trị có nằm trong ngưỡng không.
        /// CanDuoi_BaoGom: true = ≥  (giaTri >= CanDuoi), false = >  (giaTri > CanDuoi)
        /// CanTren_BaoGom: true = ≤  (giaTri <= CanTren), false = &lt;  (giaTri &lt; CanTren)
        /// NULL ở một đầu = không giới hạn phía đó.
        /// </summary>
        private static bool KiemTraNguong(decimal giaTri, CBM_Nguong ng)
        {
            bool thoaCanDuoi = ng.CanDuoi == null
                || (ng.CanDuoi_BaoGom ? giaTri >= ng.CanDuoi.Value : giaTri > ng.CanDuoi.Value);

            bool thoaCanTren = ng.CanTren == null
                || (ng.CanTren_BaoGom ? giaTri <= ng.CanTren.Value : giaTri < ng.CanTren.Value);

            return thoaCanDuoi && thoaCanTren;
        }

        // /// <summary>
        // /// Tính CSSK (Chỉ số Sức khỏe) theo công thức có trọng số, kết quả 0–100.
        // /// Bước 1: Với mỗi NhomChiTieu → điểm nhóm = Σ(Diem_Si × W_i) / Σ(W_i)  [thang 0-10]
        // /// Bước 2: CSSK = (Σ điểm nhóm / số nhóm) × 10                             [thang 0-100]
        // /// </summary>
        // private async Task<decimal> TinhCSSKAsync(List<(int IdChiTieu, decimal? DiemDat)> ketQuas)
        // {
        //     var coGiaTri = ketQuas.Where(x => x.DiemDat.HasValue).ToList();
        //     if (coGiaTri.Count == 0) return 0;

        //     var ids = coGiaTri.Select(x => x.IdChiTieu).ToList();

        //     // Lấy TrongSo_Wi và ID_NhomChiTieu cho từng chỉ tiêu
        //     var chiTieuMeta = await _db.ChiTieus
        //         .Where(c => ids.Contains(c.ID_ChiTieu))
        //         .Select(c => new { c.ID_ChiTieu, c.ID_NhomChiTieu, c.TrongSo_Wi })
        //         .ToListAsync();

        //     // Nhóm theo NhomChiTieu, tính điểm trung bình có trọng số từng nhóm
        //     var nhomGroups = chiTieuMeta.GroupBy(c => c.ID_NhomChiTieu);
        //     decimal tongDiemNhom = 0;
        //     int soNhom = 0;

        //     foreach (var nhom in nhomGroups)
        //     {
        //         decimal weightedSum = 0, weightSum = 0;
        //         foreach (var ct in nhom)
        //         {
        //             var item = coGiaTri.FirstOrDefault(x => x.IdChiTieu == ct.ID_ChiTieu);
        //             if (item.DiemDat.HasValue && ct.TrongSo_Wi.HasValue && ct.TrongSo_Wi.Value > 0)
        //             {
        //                 weightedSum += item.DiemDat.Value * ct.TrongSo_Wi.Value;
        //                 weightSum   += ct.TrongSo_Wi.Value;
        //             }
        //         }
        //         if (weightSum > 0)
        //         {
        //             tongDiemNhom += weightedSum / weightSum; // điểm nhóm: 0-10
        //             soNhom++;
        //         }
        //     }

        //     if (soNhom == 0) return 0;
        //     // Chuyển sang thang 0-100, làm tròn 1 chữ số thập phân
        //     return Math.Round(tongDiemNhom / soNhom * 10, 1);
        // }

        // private static string XepLoaiCapDo(decimal tongDiem) => tongDiem switch
        // {
        //     >= 85 => "A - Tốt",
        //     >= 70 => "B - Khá",
        //     >= 50 => "C - Trung bình",
        //     >= 30 => "D - Kém",
        //     _ => "E - Nguy hiểm"
        // };
    }
}
