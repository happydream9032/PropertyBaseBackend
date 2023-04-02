using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PropertyBase.Entities;

namespace PropertyBase.Data
{
    public class PropertyBaseDbContext : IdentityDbContext<User>
    {
        public PropertyBaseDbContext(DbContextOptions<PropertyBaseDbContext> options):base(options)
        {
        }

        public DbSet<Property> Properties { get; set; }
        public DbSet<PropertyImage> PropertyImages { get; set; }
        public DbSet<Agency> Agencies { get; set; }

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

