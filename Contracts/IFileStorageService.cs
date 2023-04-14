using System;
using PropertyBase.DTOs;

namespace PropertyBase.Contracts
{
    public interface IFileStorageService
    {
        Task<Result> Upload(IFormFile file);
        Task<ResultDelete> DeleteFile(string fileId);
    }
}

