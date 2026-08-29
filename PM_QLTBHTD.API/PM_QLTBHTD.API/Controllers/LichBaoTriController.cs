using Microsoft.AspNetCore.Mvc;
using PM_QLTBHTD.Application.DTOs;
using PM_QLTBHTD.Application.Services.IService;

namespace PM_QLTBHTD.API.Controllers
{
    [ApiController]
    [Route("api/lich-bao-tri")]
    public class LichBaoTriController : ControllerBase
    {
        private readonly ILichBaoTriService _service;

        public LichBaoTriController(ILichBaoTriService service) => _service = service;

        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] string? search, [FromQuery] string? trangThai, [FromQuery] int? idTram,
            [FromQuery] DateTime? tuNgay, [FromQuery] DateTime? denNgay,
            [FromQuery] int page = 1, [FromQuery] int? pageSize = null)
            => Ok(await _service.GetPagedAsync(search, trangThai, idTram, tuNgay, denNgay, page, pageSize));

        [HttpGet("thong-ke")]
        public async Task<IActionResult> GetThongKe()
            => Ok(await _service.GetThongKeAsync());

        [HttpGet("thiet-bi/{idThietBi}")]
        public async Task<IActionResult> GetByThietBi(int idThietBi)
            => Ok(await _service.GetByThietBiAsync(idThietBi));

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var item = await _service.GetByIdAsync(id);
            return item == null ? NotFound() : Ok(item);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateLichBaoTriDto dto)
        {
            var created = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.ID_LichBaoTri }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateLichBaoTriDto dto)
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

        [HttpPost("{id}/hoan-thanh")]
        public async Task<IActionResult> HoanThanh(int id, [FromBody] HoanThanhLichBaoTriDto dto)
        {
            var result = await _service.HoanThanhAsync(id, dto);
            return result == null ? NotFound() : Ok(result);
        }

        [HttpPost("{id}/huy")]
        public async Task<IActionResult> Huy(int id)
        {
            var result = await _service.HuyAsync(id);
            return result == null ? NotFound() : Ok(result);
        }
    }
}
