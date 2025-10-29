using BusinessCard.Models;
using Microsoft.EntityFrameworkCore;

namespace BusinessCard.Data
{
    public class BusinessCardsDbContext : DbContext
    {
        public BusinessCardsDbContext(DbContextOptions<BusinessCardsDbContext> options) : base(options)
        {
        }

        public DbSet<Card> Cards { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Card entity
            modelBuilder.Entity<Card>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Gender)
                    .IsRequired()
                    .HasMaxLength(10);

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Phone)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.Address)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.IsDeleted)
                    .IsRequired()
                    .HasDefaultValue(false);

                entity.Property(e => e.CreatedAt)
                    .IsRequired();

                // Add indexes for better performance
                entity.HasIndex(e => e.Email).IsUnique();
                entity.HasIndex(e => e.Phone).IsUnique();

                entity.Property(e => e.Image)
                    .HasMaxLength(1048576); // Max 1MB base64 string

                // Soft delete filter - exclude deleted cards by default
                entity.HasQueryFilter(e => !e.IsDeleted);
            });
        }
    }
}