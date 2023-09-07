using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PropertyBase.Contracts;
using PropertyBase.DTOs.Property;
using PropertyBase.Entities;

namespace PropertyBase.Data
{
    public class PropertyBaseDbContext : IdentityDbContext<User>
    {
        private readonly ILoggedInUserService _loggedInUserService;

        public PropertyBaseDbContext(DbContextOptions<PropertyBaseDbContext> options) : base(options)
        {
        }

        public PropertyBaseDbContext(DbContextOptions<PropertyBaseDbContext> options,
            ILoggedInUserService loggedInUserService
            ) : base(options)
        {
            _loggedInUserService = loggedInUserService;
        }

        public DbSet<Property> Properties { get; set; }
        public DbSet<PropertyImage> PropertyImages { get; set; }
        public DbSet<Agency> Agencies { get; set; }
        public DbSet<PropertyInspectionRequest> PropertyInspectionRequests {get; set;}

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var loggedInUserId = _loggedInUserService.UserId;
            
            foreach (var entry in ChangeTracker.Entries<BaseEntity>())
            {
                
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedAt = DateTime.UtcNow;
                        entry.Entity.CreatedByUserId = loggedInUserId?? entry.Entity.CreatedByUserId;
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
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            builder.Entity<Property>()
                .HasOne(c => c.Agency)
                .WithMany(c => c.Properties)
                .HasForeignKey(c => c.AgencyId);
        }


    }
}

