using System;
using PropertyBase.DTOs;
using PropertyBase.Entities;

namespace PropertyBase.Contracts
{
    public interface IFileStorageService
    {
        Task<Result> Upload(IFormFile file, ImageStorageFolder folderName);
        Task<ResultDelete> DeleteFile(string fileId);
    }
}

