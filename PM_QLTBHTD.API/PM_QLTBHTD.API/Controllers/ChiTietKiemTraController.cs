using Microsoft.AspNetCore.Mvc;
using PM_QLTBHTD.Application.DTOs;
using PM_QLTBHTD.Application.Services;

namespace PM_QLTBHTD.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChiTietKiemTraController : ControllerBase
    {
        private readonly IChiTietKiemTraService _service;

        public ChiTietKiemTraController(IChiTietKiemTraService service)
        {
            _service = service;
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

        [HttpPost("phieu/{idPhieu}")]
        public async Task<IActionResult> Create(int idPhieu, [FromBody] CreateChiTietKiemTraDto dto)
        {
            var created = await _service.CreateAsync(idPhieu, dto);
            return CreatedAtAction(nameof(GetById), new { id = created.ID_ChiTiet }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateChiTietKiemTraDto dto)
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
