using System;
using Microsoft.EntityFrameworkCore;
using MediatR;
using PropertyBase.Contracts;
using PropertyBase.Exceptions;

namespace PropertyBase.Features.Properties.DeleteImage
{
    public class DeleteImageHandler : IRequestHandler<DeleteImageRequest,DeleteImageResponse>
    {
        private readonly IPropertyRepository _propertyRepository;
        private readonly IFileStorageService _fileStorageService;
        private readonly ILoggedInUserService _loggedInUserService;

        public DeleteImageHandler(IPropertyRepository propertyRepository,
            IFileStorageService fileStorageService,
            ILoggedInUserService loggedInUserService
            )
        {
            _propertyRepository = propertyRepository;
            _fileStorageService = fileStorageService;
            _loggedInUserService = loggedInUserService;
        }

        public async Task<DeleteImageResponse> Handle(DeleteImageRequest request, CancellationToken cancellationToken)
        {
            var validator = new DeleteImageValidator();
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

            var property = await _propertyRepository.GetQueryable()
                                   .Where(c => c.Id == request.PropertyId)
                                   .Include(c => c.Images)
                                   .FirstOrDefaultAsync();

            if (property == null)
            {
                throw new RequestException(StatusCodes.Status400BadRequest, "Property not found.");
            }

            if (property.CreatedByUserId != _loggedInUserService.UserId)
            {
                throw new RequestException(StatusCodes.Status401Unauthorized, "You are not authorized to update this property. Kindly contact the owner.");
            }

            // delete file from cloud storage
            await _fileStorageService.DeleteFile(request.FileId);
            // remove file reference from database
            property.Images = property.Images.FindAll(c => c.FileId != request.FileId);
            await _propertyRepository.SaveChangesAsync();

            return new DeleteImageResponse
            {
                Message = "Image deleted successfully",
                Success = true
            };
        }
    }
}

