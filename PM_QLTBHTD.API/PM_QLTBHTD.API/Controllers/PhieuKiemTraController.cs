using Microsoft.AspNetCore.Mvc;
using PM_QLTBHTD.Application.DTOs;
using PM_QLTBHTD.Application.Services;

namespace PM_QLTBHTD.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PhieuKiemTraController : ControllerBase
    {
        private readonly IPhieuKiemTraService _service;

        public PhieuKiemTraController(IPhieuKiemTraService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
            => Ok(await _service.GetAllAsync());

        [HttpGet("by-thietbi/{idThietBi}")]
        public async Task<IActionResult> GetByThietBi(int idThietBi)
            => Ok(await _service.GetByThietBiAsync(idThietBi));

        [HttpGet("by-ngay")]
        public async Task<IActionResult> GetByNgay([FromQuery] DateTime tuNgay, [FromQuery] DateTime denNgay)
            => Ok(await _service.GetByNgayAsync(tuNgay, denNgay));

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var item = await _service.GetByIdAsync(id);
            return item == null ? NotFound() : Ok(item);
        }

        [HttpGet("{id}/detail")]
        public async Task<IActionResult> GetDetail(int id)
        {
            var item = await _service.GetDetailAsync(id);
            return item == null ? NotFound() : Ok(item);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreatePhieuKiemTraDto dto)
        {
            var created = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.ID_Phieu }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdatePhieuKiemTraDto dto)
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
    }
}
