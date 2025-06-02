using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace FuarYonetimSistemi.Application.Interfaces
{
    public interface IFileService
    {
        /// <summary>
        /// Dosya yükleme
        /// </summary>
        Task<(string fileName, string filePath, long fileSize)> UploadFileAsync(IFormFile file, string folder);

        /// <summary>
        /// Dosya silme
        /// </summary>
        Task<bool> DeleteFileAsync(string filePath);

        /// <summary>
        /// Dosya var mı kontrol
        /// </summary>
        bool FileExists(string filePath);

        /// <summary>
        /// Dosya byte array olarak getir
        /// </summary>
        Task<byte[]> GetFileContentAsync(string filePath);

        /// <summary>
        /// Logo için izin verilen dosya türlerini kontrol et
        /// </summary>
        bool IsValidImageFile(IFormFile file);

        /// <summary>
        /// Dosya boyutu kontrolü
        /// </summary>
        bool IsValidFileSize(IFormFile file, long maxSizeInBytes = 5 * 1024 * 1024); // Default 5MB
    }
}