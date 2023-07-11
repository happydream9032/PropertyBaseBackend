using System;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PropertyBase.Contracts;
using PropertyBase.Exceptions;

namespace PropertyBase.Features.Properties.DeleteProperty
{
    public class DeletePropertyHandler : IRequestHandler<DeletePropertyRequest,DeletePropertyResponse>
    {
        private readonly IPropertyRepository _propertyRepository;
        private readonly IFileStorageService _fileStorageService;
        private readonly ILoggedInUserService _loggedInUserService;

        public DeletePropertyHandler(IPropertyRepository propertyRepository,
            IFileStorageService fileStorageService,
            ILoggedInUserService loggedInUserService
            )
        {
            _propertyRepository = propertyRepository;
            _fileStorageService = fileStorageService;
            _loggedInUserService = loggedInUserService;
        }

        public async Task<DeletePropertyResponse> Handle(DeletePropertyRequest request, CancellationToken cancellationToken)
        {
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
                throw new RequestException(StatusCodes.Status401Unauthorized, "You are not authorized to delete this property. Kindly contact the owner.");
            }

            foreach (var image in property.Images)
            {
                await _fileStorageService.DeleteFile(image.FileId);
            }

            await _propertyRepository.DeleteAsync(property);

            return new DeletePropertyResponse
            {
                Message = "Property successfully deleted",
                Success = true
            };
        }
    }
}

