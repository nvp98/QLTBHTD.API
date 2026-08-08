using Microsoft.EntityFrameworkCore;
using NCalc;
using PM_QLTBHTD.Application.DTOs;
using PM_QLTBHTD.Application.Interfaces;
using PM_QLTBHTD.Domain.Entities;
using PM_QLTBHTD.Domain.IRepository;
using System.Text.Json;

namespace PM_QLTBHTD.Application.Services
{
    /// <summary>
    /// Formula Test — "xac-nhan-chuan-hoa-kien-truc-cuoi.md" mục 2. Evaluate trực tiếp
    /// CBM_CongThucTongHop.BieuThuc bằng NCalc với input giả lập (KHÔNG tra DB thật/ChiTietKiemTra
    /// như ScoringEngine) — test thuần công thức, tách biệt khỏi dữ liệu phiếu kiểm tra thật nên
    /// an toàn chạy bất kỳ lúc nào. Chỉ áp dụng cho công thức evaluate BieuThuc trực tiếp
    /// (CUSTOM_NCALC/LINEAR_COMBINE/PRODUCT/CUSTOM_MONTHLY_CLASSIFY) — WEIGHTED_AVG/WEIGHTED_AVG_SCALED
    /// không dùng BieuThuc nên không test được qua đây (xem ScoringEngine.cs).
    /// </summary>
    public class CongThucTestCaseService : ICongThucTestCaseService
    {
        private const decimal SaiSoChoPhep = 0.01m;

        private readonly ICongThucTestCaseRepository _repo;
        private readonly IAppDbContext _db;

        public CongThucTestCaseService(ICongThucTestCaseRepository repo, IAppDbContext db)
        {
            _repo = repo;
            _db = db;
        }

        private static CongThucTestCaseDto ToDto(CBM_CongThuc_TestCase e) => new()
        {
            ID_TestCase = e.ID_TestCase,
            ID_CongThuc = e.ID_CongThuc,
            TenTestCase = e.TenTestCase,
            InputJson = e.InputJson,
            KetQuaMongDoi = e.KetQuaMongDoi,
            KetQuaThucTeLanCuoi = e.KetQuaThucTeLanCuoi,
            DatLanCuoi = e.DatLanCuoi,
            ThoiGianChayCuoi = e.ThoiGianChayCuoi,
            LoiLanCuoi = e.LoiLanCuoi,
            MoTa = e.MoTa,
        };

        public async Task<List<CongThucTestCaseDto>> GetByCongThucAsync(int idCongThuc)
            => (await _repo.GetByCongThucAsync(idCongThuc)).Select(ToDto).ToList();

        public async Task<CongThucTestCaseDto> CreateAsync(CreateCongThucTestCaseDto dto)
        {
            var entity = new CBM_CongThuc_TestCase
            {
                ID_CongThuc = dto.ID_CongThuc,
                TenTestCase = dto.TenTestCase,
                InputJson = dto.InputJson,
                KetQuaMongDoi = dto.KetQuaMongDoi,
                MoTa = dto.MoTa,
            };
            await _repo.AddAsync(entity);
            await _repo.SaveChangesAsync();
            return ToDto(entity);
        }

        public async Task<CongThucTestCaseDto?> UpdateAsync(int id, UpdateCongThucTestCaseDto dto)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return null;

            entity.TenTestCase = dto.TenTestCase;
            entity.InputJson = dto.InputJson;
            entity.KetQuaMongDoi = dto.KetQuaMongDoi;
            entity.MoTa = dto.MoTa;
            // Kết quả lần chạy trước không còn phản ánh đúng test case vừa sửa — xoá cache cũ.
            entity.KetQuaThucTeLanCuoi = null;
            entity.DatLanCuoi = null;
            entity.ThoiGianChayCuoi = null;
            entity.LoiLanCuoi = null;

            _repo.Update(entity);
            await _repo.SaveChangesAsync();
            return ToDto(entity);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return false;
            _repo.Delete(entity);
            await _repo.SaveChangesAsync();
            return true;
        }

        public async Task<List<CongThucTestCaseDto>> ChayTatCaAsync(int idCongThuc)
        {
            var congThuc = await _db.CongThucTongHops.FirstOrDefaultAsync(c => c.ID_CongThuc == idCongThuc)
                ?? throw new InvalidOperationException($"Không tìm thấy công thức ID={idCongThuc}.");

            var cases = await _repo.GetByCongThucAsync(idCongThuc);

            foreach (var tc in cases)
            {
                tc.ThoiGianChayCuoi = DateTime.UtcNow;
                try
                {
                    var vars = JsonSerializer.Deserialize<Dictionary<string, double>>(tc.InputJson)
                        ?? new Dictionary<string, double>();

                    var expr = new Expression(congThuc.BieuThuc);
                    foreach (var kv in vars)
                        expr.Parameters[kv.Key] = kv.Value;

                    var ketQua = Convert.ToDecimal(expr.Evaluate());
                    tc.KetQuaThucTeLanCuoi = ketQua;
                    tc.DatLanCuoi = Math.Abs(ketQua - tc.KetQuaMongDoi) <= SaiSoChoPhep;
                    tc.LoiLanCuoi = null;
                }
                catch (Exception ex)
                {
                    tc.KetQuaThucTeLanCuoi = null;
                    tc.DatLanCuoi = false;
                    tc.LoiLanCuoi = ex.Message;
                }

                _repo.Update(tc);
            }

            await _repo.SaveChangesAsync();
            return cases.Select(ToDto).ToList();
        }
    }
}
