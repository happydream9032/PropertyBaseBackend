using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PropertyBase.Entities;

namespace PropertyBase.Data
{
    public class PropertyBaseDbContext : IdentityDbContext<User>
    {
        private readonly IHttpContextAccessor _contextAccessor;

        public PropertyBaseDbContext(DbContextOptions<PropertyBaseDbContext> options):base(options)
        {
        }

        public PropertyBaseDbContext(DbContextOptions<PropertyBaseDbContext> options,
            IHttpContextAccessor contextAccessor): base(options)
        {
            _contextAccessor = contextAccessor;
        }

        public DbSet<Property> Properties { get; set; }
        public DbSet<PropertyImage> PropertyImages { get; set; }
        public DbSet<Agency> Agencies { get; set; }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var loggedInUserId = _contextAccessor.HttpContext?.User?.FindFirst("uid")?.Value;

            foreach (var entry in ChangeTracker.Entries<BaseEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedAt = DateTime.UtcNow;
                        entry.Entity.CreatedByUserId = loggedInUserId!;
                        entry.Entity.UpdatedAt = DateTime.UtcNow;
                        break;
                    case EntityState.Modified:
                        entry.Entity.UpdatedAt = DateTime.UtcNow;
                        break;
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            
            builder.Entity<PropertyImage>()
                .HasOne(c => c.Property)
                .WithMany(c => c.Images)
                .HasForeignKey(c => c.PropertyId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Property>()
                .HasOne(c => c.Agency)
                .WithMany(c => c.Properties)
                .HasForeignKey(c => c.AgencyId);
        }
    }
}

