using System;
using PropertyBase.Contracts;
using PropertyBase.Entities;

namespace PropertyBase.Data.Repositories
{
    public class AgencyRepository : BaseRepository<Agency>, IAgencyRepository
    {
        public AgencyRepository(PropertyBaseDbContext dbContext): base(dbContext)
        {
        }
    }
}

