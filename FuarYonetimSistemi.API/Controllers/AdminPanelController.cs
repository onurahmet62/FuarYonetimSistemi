using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FuarYonetimSistemi.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminPanelController : ControllerBase
    {
        [HttpGet("secret")]
        [Authorize(Roles = "Admin")]
        public IActionResult GetAdminSecret()
        {
            return Ok("Bu bilgi sadece Admin rolüne sahip kullanıcılar içindir.");
        }

        [HttpGet("common")]
        [Authorize]
        public IActionResult GetCommonData()
        {
            return Ok("Giriş yapmış tüm kullanıcılar bu veriye erişebilir.");
        }
    }
}
