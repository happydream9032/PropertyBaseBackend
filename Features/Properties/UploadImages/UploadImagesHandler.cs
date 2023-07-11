using System;
using MediatR;
using PropertyBase.Contracts;
using PropertyBase.Data.Repositories;
using PropertyBase.Entities;
using PropertyBase.Exceptions;
using PropertyBase.Services;

namespace PropertyBase.Features.Properties.UploadImages
{
    public class UploadImagesHandler : IRequestHandler<UploadImagesRequest,UploadImagesResponse>
    {
        private readonly IPropertyRepository _propertyRepository;
        private readonly IFileStorageService _fileStorageService;

        public UploadImagesHandler(
            IPropertyRepository propertyRepository,
            IFileStorageService fileStorageService)
        {
            _propertyRepository = propertyRepository;
            _fileStorageService = fileStorageService;
        }

        public async Task<UploadImagesResponse> Handle(UploadImagesRequest request, CancellationToken cancellationToken)
        {
            var validator = new UploadImagesValidator();
            var validationResult = await validator.ValidateAsync(request);

            if (validationResult.Errors.Count > 0)
            {
                var validationErrors = new List<string>();

                foreach (var error in validationResult.Errors)
                {
                    validationErrors.Add(error.ErrorMessage);
                }

                throw new RequestException(StatusCodes.Status400BadRequest, validationErrors.FirstOrDefault());
            }

            var property = await _propertyRepository.GetByIdAsync(request.PropertyId);

            if (property == null)
            {
                throw new RequestException(StatusCodes.Status400BadRequest, "Property does not exist.");
            }

            List<PropertyImage> propertyImages = new();

            foreach (var file in request.Files)
            {
                var uploadedFile = await _fileStorageService.Upload(file, ImageStorageFolder.Property);
                
                propertyImages.Add(new PropertyImage
                {
                    ImageURL = uploadedFile.url,
                    FileId = uploadedFile.fileId,
                    Verified = true
                });
            }

            propertyImages.AddRange(property.Images);
            property.Images = propertyImages;

            property.Status = PropertyStatus.Published;
            property.PublishedDate = DateTime.UtcNow;

            await _propertyRepository.SaveChangesAsync();

            return new UploadImagesResponse
            {
                Message = "Images uploaded successfully",
                Success = true
            };
        }
    }
}

