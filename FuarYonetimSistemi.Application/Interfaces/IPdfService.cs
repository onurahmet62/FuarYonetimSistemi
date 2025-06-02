using FuarYonetimSistemi.Application.DTOs;
using System;
using System.Threading.Tasks;

namespace FuarYonetimSistemi.Application.Interfaces
{
    public interface IPdfService
    {
        /// <summary>
        /// Katılımcı bilgilerini PDF olarak oluştur
        /// </summary>
        Task<byte[]> GenerateParticipantPdfAsync(ParticipantDto participant);

        /// <summary>
        /// Tüm katılımcıları listeleyen PDF oluştur
        /// </summary>
        Task<byte[]> GenerateParticipantListPdfAsync(System.Collections.Generic.IEnumerable<ParticipantDto> participants);
    }
}