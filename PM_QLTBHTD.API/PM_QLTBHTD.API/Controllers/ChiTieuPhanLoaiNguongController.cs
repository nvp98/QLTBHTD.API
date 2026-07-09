using Microsoft.AspNetCore.Mvc;
using PM_QLTBHTD.Application.DTOs;
using PM_QLTBHTD.Application.Services;

namespace PM_QLTBHTD.API.Controllers
{
    /// <summary>Cấu hình mức phân loại (N0..N4) cho chỉ tiêu kiểu LoaiTinhDiem='LF' (Load Factor).</summary>
    [ApiController]
    [Route("api/chitieu-phanloai")]
    public class ChiTieuPhanLoaiNguongController : ControllerBase
    {
        private readonly IChiTieuPhanLoaiNguongService _service;

        public ChiTieuPhanLoaiNguongController(IChiTieuPhanLoaiNguongService service)
            => _service = service;

        [HttpGet("by-chitieu/{idChiTieu}")]
        public async Task<IActionResult> GetByChiTieu(int idChiTieu)
            => Ok(await _service.GetByChiTieuAsync(idChiTieu));

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var item = await _service.GetByIdAsync(id);
            return item == null ? NotFound() : Ok(item);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateChiTieuPhanLoaiNguongDto dto)
        {
            var created = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.ID_PhanLoai }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateChiTieuPhanLoaiNguongDto dto)
        {
            var updated = await _service.UpdateAsync(id, dto);
            return updated == null ? NotFound() : Ok(updated);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _service.DeleteAsync(id);
            return result ? NoContent() : NotFound();
        }

        /// <summary>Kết quả phân loại từng tháng đã tính cho 1 phiếu — dùng để hiển thị lại sau khi nhập.</summary>
        [HttpGet("ket-qua-thang/{idPhieu}/{idChiTieu}")]
        public async Task<IActionResult> GetKetQuaThang(int idPhieu, int idChiTieu)
            => Ok(await _service.GetKetQuaThangAsync(idPhieu, idChiTieu));
    }
}
