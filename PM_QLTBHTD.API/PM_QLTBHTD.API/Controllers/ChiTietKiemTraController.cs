using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using PM_QLTBHTD.Application.DTOs;
using PM_QLTBHTD.Application.Exceptions;
using PM_QLTBHTD.Application.Services.IService;

namespace PM_QLTBHTD.API.Controllers
{
    [ApiController]
    [Route("api/chi-tiet-kiem-tra")]
    public class ChiTietKiemTraController : ControllerBase
    {
        private readonly IChiTietKiemTraService _service;
        private readonly IChiTieuScoringService _scoringService;
        private readonly IScoringEngine _engine;

        public ChiTietKiemTraController(
            IChiTietKiemTraService service,
            IChiTieuScoringService scoringService,
            IScoringEngine engine)
        {
            _service = service;
            _scoringService = scoringService;
            _engine = engine;
        }

        [HttpGet("by-phieu/{idPhieu}")]
        public async Task<IActionResult> GetByPhieu(int idPhieu)
            => Ok(await _service.GetByPhieuAsync(idPhieu));

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var item = await _service.GetByIdAsync(id);
            return item == null ? NotFound() : Ok(item);
        }

        /// <summary>
        /// Nhận batch nhập liệu, tính Si cho từng chỉ tiêu, và tuỳ chọn tính điểm nhóm ngay.
        /// Body: NhapPhieuKiemTraRequest (danh sách chỉ tiêu + tuỳ chọn tự động tính điểm).
        /// </summary>
        [HttpPost("phieu/{idPhieu}")]
        [Authorize(Roles = "Admin,KyThuatVien")]
        public async Task<IActionResult> NhapLieu(
            int idPhieu,
            [FromBody] NhapPhieuKiemTraRequest request,
            CancellationToken ct)
        {
            try
            {
                await _scoringService.TinhVaLuuDiemSiAsync(idPhieu, request.DanhSachChiTieu, ct);

                var ketQuaNhap = await _service.GetByPhieuAsync(idPhieu);

                KetQuaNhomDto? ketQuaTinhDiem = null;
                if (request.TuDongTinhDiem && request.ID_NhomChiTieuTinhDiem.HasValue)
                {
                    var diem = await _engine.TinhDiemNhomAsync(
                        request.ID_NhomChiTieuTinhDiem.Value, idPhieu, ct);
                    ketQuaTinhDiem = new KetQuaNhomDto
                    {
                        ID_NhomChiTieu = request.ID_NhomChiTieuTinhDiem.Value,
                        Diem = diem
                    };
                }

                decimal? tongDiem = null;
                string? canhBaoTongDiem = null;
                try
                {
                    tongDiem = await _engine.TinhVaLuuTongDiemAsync(idPhieu, ct);
                }
                catch (NhieuNhomGocException ex)
                {
                    canhBaoTongDiem = ex.Message;
                }

                return Ok(new NhapPhieuKiemTraResponse
                {
                    KetQuaNhap = ketQuaNhap.ToList(),
                    KetQuaTinhDiem = ketQuaTinhDiem,
                    TongDiem_Soqt = tongDiem,
                    CanhBaoTongDiem = canhBaoTongDiem
                });
            }
            catch (ThieuBieuThucNguongException ex)
            {
                return UnprocessableEntity(new { Error = ex.Message, ex.IdChiTieu });
            }
            catch (ThieuInputChiTieuException ex)
            {
                return UnprocessableEntity(new { Error = ex.Message, ex.IdChiTieu, ex.MaInput });
            }
            catch (ThieuDuLieuChiTietKiemTraException ex)
            {
                return UnprocessableEntity(new { Error = ex.Message, ex.IdChiTieu, ex.IdPhieu });
            }
            catch (ThieuPhanLoaiNguongException ex)
            {
                return UnprocessableEntity(new { Error = ex.Message, ex.IdChiTieu });
            }
            catch (ThieuTaiDinhMucException ex)
            {
                return UnprocessableEntity(new { Error = ex.Message, ex.IdThietBi });
            }
            catch (LoiFormulaException ex)
            {
                return UnprocessableEntity(new { Error = ex.Message, ex.IdChiTieu, ex.MaKetQua });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,KyThuatVien,TruongTram")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateChiTietKiemTraDto dto)
        {
            var updated = await _service.UpdateAsync(id, dto);
            return updated == null ? NotFound() : Ok(updated);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,KyThuatVien")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _service.DeleteAsync(id);
            return result ? NoContent() : NotFound();
        }
    }
}
