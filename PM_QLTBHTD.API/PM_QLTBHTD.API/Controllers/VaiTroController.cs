using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PM_QLTBHTD.Application.Services.IService;

namespace PM_QLTBHTD.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class VaiTroController : ControllerBase
    {
        private readonly IVaiTroService _service;

        public VaiTroController(IVaiTroService service)
        {
            _service = service;
        }

        [HttpGet("get-all-vaitro")]
        public async Task<IActionResult> GetAll()
            => Ok(await _service.GetAllAsync());
    }
}
