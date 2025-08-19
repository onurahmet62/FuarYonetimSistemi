using FuarYonetimSistemi.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuarYonetimSistemi.Application.Interfaces
{
    public interface IWordService
    {
        /// <summary>
        /// Katılımcı bilgilerini Word belgesi olarak oluştur
        /// </summary>
        Task<byte[]> GenerateParticipantWordAsync(ParticipantDto participant, string fairName);

        /// <summary>
        /// Tüm katılımcıları içeren Word belgesi oluştur
        /// </summary>
        Task<byte[]> GenerateAllParticipantsWordAsync(IEnumerable<ParticipantDto> participants, string fairName);
    }
}
