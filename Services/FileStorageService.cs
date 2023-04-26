using System;
using Imagekit;
using Imagekit.Models;
using Imagekit.Sdk;
using PropertyBase.Contracts;
using PropertyBase.DTOs;
using PropertyBase.Entities;
using PropertyBase.Exceptions;

namespace PropertyBase.Services
{
    public class FileStorageService : IFileStorageService
    {

        private readonly ImagekitClient _imagekitClient;

        public FileStorageService()
        {
            _imagekitClient = new ImagekitClient(DotNetEnv.Env.GetString("IMAGEKIT_PUBLIC_KEY"),
                DotNetEnv.Env.GetString("IMAGEKIT_PRIVATE_KEY"), DotNetEnv.Env.GetString("IMAGEKIT_URL_ENDPOINT"));
            
        }

        public async Task<ResultDelete> DeleteFile(string fileId)
        {
            return await _imagekitClient.DeleteFileAsync(fileId);
        }

        public async Task<Result> Upload(IFormFile file, ImageStorageFolder folderName)
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
                folder = folderName == ImageStorageFolder.Profile? "Profile":"Property",
            };
            
            var uploadedFile = await _imagekitClient.UploadAsync(obj);
            return uploadedFile;
        }

        public bool ValidateFileSize(IFormFile file)
        {
            if(file.Length > 10485760)
            {
                return false;
            }

            return true;
        }

       
    }
}

