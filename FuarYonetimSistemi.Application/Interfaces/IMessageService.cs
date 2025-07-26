using FuarYonetimSistemi.Application.DTOs;
using FuarYonetimSistemi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuarYonetimSistemi.Application.Interfaces
{
    public interface IMessageService
    {
        Task<Message> CreateMessageAsync(CreateMessageDto dto);
        Task<IEnumerable<Message>> GetMessagesBetweenUsersAsync(Guid user1Id, Guid user2Id);
        Task<IEnumerable<Message>> GetMessagesForUserAsync(Guid userId);
        Task<bool> DeleteMessageAsync(Guid messageId);
    }


}
