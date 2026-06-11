using Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Services
{
    public class FileService : IFileService
    {
        private readonly string _basePath;
        public FileService(IConfiguration configuration)
        {
            _basePath = configuration["FileStorage:BasePath"]!;
        }

        public void DeleteFile(string? relativePath)
        {
            ArgumentException.ThrowIfNullOrEmpty(relativePath);

           var fullPath = Path.Combine(_basePath, relativePath!);

            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }
        }

        public async Task<string> UploadFileAsync(IFormFile file, string folderName)
        {

           var ext = Path.GetExtension(file.FileName).ToLower();

            var folder = Path.Combine(_basePath, "Uploads" , folderName);

            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            var fileName = $"{Guid.NewGuid()}{ext}";

            var fullPath = Path.Combine(folder, fileName);

            using (FileStream stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return Path.Combine(folderName, fileName);
           
        }
    }
}
