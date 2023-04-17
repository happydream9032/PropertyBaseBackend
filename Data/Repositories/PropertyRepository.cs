using System;
using PropertyBase.Contracts;
using PropertyBase.Entities;

namespace PropertyBase.Data.Repositories
{
    public class PropertyRepository : BaseRepository<Property>, IPropertyRepository
    {
        public PropertyRepository(PropertyBaseDbContext dbContext):base(dbContext)
        {
        }
    }
}

