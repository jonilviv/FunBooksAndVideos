using FunBooksAndVideos.Entities;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

/*
dotnet ef migrations add InitialCreate

dotnet ef database update

 */

namespace FunBooksAndVideos.Data
{
    [ExcludeFromCodeCoverage]
    public sealed class FunDbContext : DbContext, IFunDbContext
    {
        public FunDbContext(DbContextOptions<FunDbContext> options) : base(options) { }

        public DbSet<Customer> Customers { get; set; }

        public DbSet<Product> Products { get; set; }

        public DbSet<PurchaseOrder> PurchaseOrders { get; set; }

        public DbSet<ShippingSlip> ShippingSlips { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PurchaseOrder>()
                .HasOne(p => p.Customer)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PurchaseOrder>()
                .HasOne(p => p.ShippingSlip)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PurchaseOrder>()
                .HasMany(p => p.Products)
                .WithMany()
                .UsingEntity(j => j.ToTable("PurchaseOrderProduct"));
        }
    }
}