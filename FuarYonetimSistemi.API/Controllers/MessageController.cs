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
    [Authorize]
    public class MessagesController : ControllerBase
    {
        private readonly IMessageService _messageService;

        public MessagesController(IMessageService messageService)
        {
            _messageService = messageService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateMessage([FromBody] CreateMessageDto dto)
        {
            var message = await _messageService.CreateMessageAsync(dto);
            return Ok(message);
        }

        [HttpGet("between/{user1Id}/{user2Id}")]
        public async Task<IActionResult> GetMessagesBetweenUsers(Guid user1Id, Guid user2Id)
        {
            var messages = await _messageService.GetMessagesBetweenUsersAsync(user1Id, user2Id);
            return Ok(messages);
        }

        [HttpGet("for-user/{userId}")]
        public async Task<IActionResult> GetMessagesForUser(Guid userId)
        {
            var messages = await _messageService.GetMessagesForUserAsync(userId);
            return Ok(messages);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMessage(Guid id)
        {
            var success = await _messageService.DeleteMessageAsync(id);
            if (!success) return NotFound();
            return NoContent();
        }
    }
}
