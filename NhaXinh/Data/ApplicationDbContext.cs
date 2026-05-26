using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NhaXinh.Models;

namespace NhaXinh.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Banner> Banners { get; set; }
        public DbSet<News> News { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Category>(entity =>
            {
                entity.HasIndex(c => c.Slug).IsUnique();

                entity.HasOne(c => c.ParentCategory)
                      .WithMany(c => c.SubCategories)
                      .HasForeignKey(c => c.ParentId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<Product>(entity =>
            {
                entity.HasIndex(p => p.Slug).IsUnique();

                entity.Property(p => p.Price).HasColumnType("decimal(18,2)");
                entity.Property(p => p.DiscountPrice).HasColumnType("decimal(18,2)");

                entity.HasOne(p => p.Category)
                      .WithMany(c => c.Products)
                      .HasForeignKey(p => p.CategoryId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<Order>(entity =>
            {
                entity.HasIndex(o => o.OrderCode).IsUnique();

                entity.Property(o => o.TotalAmount).HasColumnType("decimal(18,2)");

                entity.HasOne(o => o.User)
                      .WithMany(u => u.Orders)
                      .HasForeignKey(o => o.UserId)
                      .OnDelete(DeleteBehavior.Restrict);
            });
            builder.Entity<OrderDetail>(entity =>
            {
                entity.Property(od => od.UnitPrice).HasColumnType("decimal(18,2)");
                entity.Property(od => od.SubTotal).HasColumnType("decimal(18,2)");

                entity.HasOne(od => od.Order)
                      .WithMany(o => o.OrderDetails)
                      .HasForeignKey(od => od.OrderId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(od => od.Product)
                      .WithMany(p => p.OrderDetails)
                      .HasForeignKey(od => od.ProductId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<News>(entity =>
            {
                entity.HasIndex(n => n.Slug).IsUnique();
            });
        }
    }
}
