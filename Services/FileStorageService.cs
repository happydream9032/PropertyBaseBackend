using System;
using Imagekit;
using Imagekit.Models;
using Imagekit.Sdk;
using PropertyBase.Contracts;
using PropertyBase.DTOs;

namespace PropertyBase.Services
{
    public class FileStorageService : IFileStorageService
    {

        private readonly ImagekitClient _imagekitClient;
        private readonly string _folderNaame;

        public FileStorageService()
        {
            _imagekitClient = new ImagekitClient(DotNetEnv.Env.GetString("IMAGEKIT_PUBLIC_KEY"),
                DotNetEnv.Env.GetString("IMAGEKIT_PRIVATE_KEY"), DotNetEnv.Env.GetString("IMAGEKIT_URL_ENDPOINT"));
            _folderNaame = "Profile";
            
        }

        public async Task<ResultDelete> DeleteFile(string fileId)
        {
            return await _imagekitClient.DeleteFileAsync(fileId);
        }

        public async Task<Result> Upload(IFormFile file)
        {
            string base64String = "";
          using(var ms = new MemoryStream())
            {
                await file.CopyToAsync(ms);
                var fileByteArray = ms.ToArray();

                base64String = Convert.ToBase64String(fileByteArray);
            }

            FileCreateRequest obj = new FileCreateRequest
            {
                file = base64String,
                fileName = Guid.NewGuid().ToString(),
                folder = _folderNaame,
            };

            var uploadedFile = await _imagekitClient.UploadAsync(obj);
            return uploadedFile;
        }

       
    }
}

