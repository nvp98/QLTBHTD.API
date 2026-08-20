using Microsoft.AspNetCore.Mvc;
using PM_QLTBHTD.Application.DTOs;
using PM_QLTBHTD.Application.Exceptions;
using PM_QLTBHTD.Application.Services.IService;

namespace PM_QLTBHTD.API.Controllers
{
    [ApiController]
    [Route("api/thongso")]
    public class ThongSoController : ControllerBase
    {
        private readonly IThongSoService _service;

        public ThongSoController(IThongSoService service) => _service = service;

        [HttpGet]
        public async Task<IActionResult> GetAllThongSo()
            => Ok(await _service.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetThongSoById(int id)
        {
            var item = await _service.GetByIdAsync(id);
            return item == null ? NotFound() : Ok(item);
        }

        [HttpPost]
        public async Task<IActionResult> CreateThongSo([FromBody] CreateThongSoDto dto)
        {
            var created = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetThongSoById), new { id = created.ID_ThongSo }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateThongSo(int id, [FromBody] UpdateThongSoDto dto)
        {
            var updated = await _service.UpdateAsync(id, dto);
            return updated == null ? NotFound() : Ok(updated);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteThongSo(int id)
        {
            try
            {
                var result = await _service.DeleteAsync(id);
                return result ? NoContent() : NotFound();
            }
            catch (ThongSoDangSuDungException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
