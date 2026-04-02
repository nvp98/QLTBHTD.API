using Microsoft.AspNetCore.Mvc;
using PM_QLTBHTD.Application.DTOs;
using PM_QLTBHTD.Application.Services;

namespace PM_QLTBHTD.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TramDienController : ControllerBase
    {
        private readonly ITramDienService _service;

        public TramDienController(ITramDienService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? search, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
            => Ok(await _service.GetPagedAsync(search, page, pageSize));

        [HttpGet("active")]
        public async Task<IActionResult> GetAllActive()
            => Ok(await _service.GetAllActiveAsync());

        [HttpGet("by-khuvuc/{idKhuVuc}")]
        public async Task<IActionResult> GetByKhuVuc(int idKhuVuc)
            => Ok(await _service.GetByKhuVucAsync(idKhuVuc));

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var item = await _service.GetByIdAsync(id);
            return item == null ? NotFound() : Ok(item);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateTramDienDto dto)
        {
            var created = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.IDTram }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateTramDienDto dto)
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
