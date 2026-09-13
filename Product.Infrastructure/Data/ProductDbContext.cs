using Microsoft.EntityFrameworkCore;
using Product.Domain.Entities;

namespace Product.Infrastructure.Data
{
    public class ProductDbContext : DbContext
    {
        public ProductDbContext(DbContextOptions<ProductDbContext> options)
            : base(options) { }

        public DbSet<ProductClass> Products { get; set; }
        public DbSet<MessageClass> Messages { get; set; }
        public DbSet<FoodClass> Foods { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProductClass>(entity =>
            {
                entity.ToTable("Products");
                entity.HasKey(p => p.Id);
                entity.Property(p => p.Id).ValueGeneratedOnAdd();
                entity.Property(p => p.Name).IsRequired();
            });

            modelBuilder.Entity<MessageClass>(entity =>
            {
                entity.ToTable("Messages");
                entity.HasKey(m => m.Id);
                entity.Property(m => m.Id).ValueGeneratedOnAdd();
                entity.Property(m => m.Content).IsRequired().HasMaxLength(500);
            });

            modelBuilder.Entity<FoodClass>(entity =>
            {
                entity.ToTable("Foods");
                entity.HasKey(f => f.Id);
                entity.Property(f => f.Id).ValueGeneratedOnAdd();
                entity.Property(f => f.Name).IsRequired().HasMaxLength(200);
            });
        }
    }
}
