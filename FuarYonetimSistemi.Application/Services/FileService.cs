using FuarYonetimSistemi.Application.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FuarYonetimSistemi.Infrastructure.Services
{
    public class FileService : IFileService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly string[] _allowedImageTypes = { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp" };
        private readonly string[] _allowedMimeTypes = { "image/jpeg", "image/png", "image/gif", "image/bmp", "image/webp" };

        public FileService(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<(string fileName, string filePath, long fileSize)> UploadFileAsync(IFormFile file, string folder)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("Dosya boş olamaz.");

            // Güvenlik kontrolü
            if (!IsValidImageFile(file))
                throw new ArgumentException("Geçersiz dosya türü. Sadece resim dosyaları kabul edilir.");

            if (!IsValidFileSize(file))
                throw new ArgumentException("Dosya boyutu çok büyük. Maksimum 5MB olabilir.");

            // Klasör oluştur
            var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", folder);
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            // Benzersiz dosya adı oluştur
            var fileExtension = Path.GetExtension(file.FileName).ToLower();
            var fileName = $"{Guid.NewGuid()}{fileExtension}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            // Dosyayı kaydet
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            // Relative path döndür (veritabanında saklamak için)
            var relativePath = Path.Combine("uploads", folder, fileName).Replace("\\", "/");

            return (fileName, relativePath, file.Length);
        }

        public async Task<bool> DeleteFileAsync(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return false;

            try
            {
                var fullPath = Path.Combine(_webHostEnvironment.WebRootPath, filePath.Replace("/", "\\"));

                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                    return true;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        public bool FileExists(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return false;

            var fullPath = Path.Combine(_webHostEnvironment.WebRootPath, filePath.Replace("/", "\\"));
            return File.Exists(fullPath);
        }

        public async Task<byte[]> GetFileContentAsync(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentException("Dosya yolu boş olamaz.");

            var fullPath = Path.Combine(_webHostEnvironment.WebRootPath, filePath.Replace("/", "\\"));

            if (!File.Exists(fullPath))
                throw new FileNotFoundException("Dosya bulunamadı.");

            return await File.ReadAllBytesAsync(fullPath);
        }

        public bool IsValidImageFile(IFormFile file)
        {
            if (file == null)
                return false;

            // Dosya uzantısı kontrolü
            var extension = Path.GetExtension(file.FileName).ToLower();
            if (!_allowedImageTypes.Contains(extension))
                return false;

            // MIME type kontrolü
            if (!_allowedMimeTypes.Contains(file.ContentType.ToLower()))
                return false;

            return true;
        }

        public bool IsValidFileSize(IFormFile file, long maxSizeInBytes = 5 * 1024 * 1024)
        {
            return file != null && file.Length > 0 && file.Length <= maxSizeInBytes;
        }
    }
}