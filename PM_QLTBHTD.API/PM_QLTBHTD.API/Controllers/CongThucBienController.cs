using Microsoft.AspNetCore.Mvc;
using PM_QLTBHTD.Application.DTOs;
using PM_QLTBHTD.Application.Services;

namespace PM_QLTBHTD.API.Controllers
{
    [ApiController]
    [Route("api/cong-thuc-bien")]
    public class CongThucBienController : ControllerBase
    {
        private readonly ICongThucBienService _service;

        public CongThucBienController(ICongThucBienService service)
        {
            _service = service;
        }

        [HttpGet("by-congthuc/{idCongThuc}")]
        public async Task<IActionResult> GetByCongThuc(int idCongThuc)
            => Ok(await _service.GetByCongThucAsync(idCongThuc));

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var item = await _service.GetByIdAsync(id);
            return item == null ? NotFound() : Ok(item);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCongThucBienDto dto)
        {
            var created = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.ID_Bien }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateCongThucBienDto dto)
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
