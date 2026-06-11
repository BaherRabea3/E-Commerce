using Microsoft.AspNetCore.Http;

namespace Application.Common.Interfaces
{
    public interface IFileService
    {
        Task<string> UploadFileAsync(IFormFile file , string folderName);
        void DeleteFile(string? relativePath);
    }
}
