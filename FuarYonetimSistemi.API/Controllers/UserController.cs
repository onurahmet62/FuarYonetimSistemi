using FuarYonetimSistemi.Application.DTOs;
using FuarYonetimSistemi.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace FuarYonetimSistemi.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        public async Task<IActionResult> AddUser(UserDto dto)
        {
            var user = await _userService.AddUserAsync(dto);
            return Ok(user);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(Guid id, UserDto dto)
        {
            var user = await _userService.UpdateUserAsync(id, dto);
            return Ok(user);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> SoftDeleteUser(Guid id)
        {
            await _userService.SoftDeleteUserAsync(id);
            return NoContent();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(Guid id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
                return NotFound();

            return Ok(user);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        [HttpPost("{id}/reset-password")]
        public async Task<IActionResult> ResetPassword(Guid id)
        {
            var newPassword = await _userService.ResetPasswordAsync(id);
            return Ok(new { NewPassword = newPassword });
        }
    }
}
