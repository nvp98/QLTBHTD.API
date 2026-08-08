using Microsoft.AspNetCore.Mvc;
using PM_QLTBHTD.Application.DTOs;
using PM_QLTBHTD.Application.Services;

namespace PM_QLTBHTD.API.Controllers
{
    [ApiController]
    [Route("api/cong-thuc-tong-hop")]
    public class CongThucTongHopController : ControllerBase
    {
        private readonly ICongThucTongHopService _service;

        public CongThucTongHopController(ICongThucTongHopService service)
        {
            _service = service;
        }

        [HttpGet("by-nhom/{idNhomChiTieu}")]
        public async Task<IActionResult> GetByNhom(int idNhomChiTieu)
            => Ok(await _service.GetByNhomAsync(idNhomChiTieu));

        [HttpGet("by-nhom/{idNhomChiTieu}/active")]
        public async Task<IActionResult> GetActive(int idNhomChiTieu)
        {
            var item = await _service.GetActiveByNhomAsync(idNhomChiTieu);
            return item == null ? NotFound() : Ok(item);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var item = await _service.GetByIdAsync(id);
            return item == null ? NotFound() : Ok(item);
        }

        [HttpGet("validate-vong-lap/{idLoaiThietBi}")]
        public async Task<IActionResult> ValidateVongLap(int idLoaiThietBi)
            => Ok(await _service.ValidateVongLapAsync(idLoaiThietBi));

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCongThucTongHopDto dto)
        {
            var created = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.ID_CongThuc }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateCongThucTongHopDto dto)
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
