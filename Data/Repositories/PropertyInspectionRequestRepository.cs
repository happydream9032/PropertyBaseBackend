using System;
using PropertyBase.Contracts;
using PropertyBase.Entities;

namespace PropertyBase.Data.Repositories
{
    public class PropertyInspectionRequestRepository : BaseRepository<PropertyInspectionRequest>,IPropertyInspectionRequestRepository
    {
        public PropertyInspectionRequestRepository(PropertyBaseDbContext dbContext):base(dbContext)
        {
        }
    }
}

