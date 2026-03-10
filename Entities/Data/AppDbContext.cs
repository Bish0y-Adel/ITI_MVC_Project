using Entities.Models;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Entities.Data
{
    public class AppDbContext : IdentityDbContext<User>
    {
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // Configure the connection string for the database
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.LogTo(Console.WriteLine, LogLevel.Information);
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            ConfigureIndexes(modelBuilder);
            ConfigureRelationships(modelBuilder);
            ConfigureDecimalPrecision(modelBuilder);
            ConfigureGlobalQueryFilters(modelBuilder);
            SeedRoles(modelBuilder);
        }

        private static void SeedRoles(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<IdentityRole>().HasData(
                new IdentityRole
                {
                    Id = "dc41a3d9-6f2f-4e1a-9a58-583e7ebb5850",
                    Name = "Admin",
                    NormalizedName = "ADMIN",
                    ConcurrencyStamp = "00a64c96-bb71-4e85-af4f-e9719be9e0a9"
                },
                new IdentityRole
                {
                    Id = "093f9758-a398-4181-a6b0-fb1c64d2be7d",
                    Name = "Customer",
                    NormalizedName = "CUSTOMER",
                    ConcurrencyStamp = "48c8a45b-1862-4d9c-905e-e0836ca030ae"
                },
                new IdentityRole
                {
                    Id = "3834c685-d308-4758-86a4-72d0f3a07b38",
                    Name = "SubAdmin",
                    NormalizedName = "SUBADMIN",
                    ConcurrencyStamp = "5a760f2d-24f6-4121-9dd4-a6969c34361c"
                }
            );
        }

        #region Configuration Methods

        private static void ConfigureIndexes(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>()
                .HasIndex(c => c.Name)
                .IsUnique();

            modelBuilder.Entity<Product>()
                .HasIndex(p => p.SKU)
                .IsUnique();

            modelBuilder.Entity<Order>()
                .HasIndex(o => o.OrderNumber)
                .IsUnique();

            // One user can have only one cart
            modelBuilder.Entity<Cart>()
                .HasIndex(c => c.UserId)
                .IsUnique();

            // Prevent duplicate product inside same cart
            modelBuilder.Entity<CartItem>()
                .HasIndex(ci => new { ci.CartId, ci.ProductId })
                .IsUnique()
                .HasFilter("IsDeleted = 0");
        }

        private static void ConfigureRelationships(ModelBuilder modelBuilder)
        {
            // Category: self-relationship FK
            modelBuilder.Entity<Category>()
                .HasOne(c => c.ParentCategory)
                .WithMany(c => c.SubCategories)
                .HasForeignKey(c => c.ParentCategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // Address → User
            modelBuilder.Entity<Address>()
                .HasOne(a => a.User)
                .WithMany(u => u.Addresses)
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Product → Category
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // Order → User
            modelBuilder.Entity<Order>()
                .HasOne(o => o.User)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Order → ShippingAddress
            modelBuilder.Entity<Order>()
                .HasOne(o => o.ShippingAddress)
                .WithMany(a => a.Orders)
                .HasForeignKey(o => o.ShippingAddressId)
                .OnDelete(DeleteBehavior.Restrict);

            // OrderItem → Order
            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // OrderItem → Product
            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Product)
                .WithMany(p => p.OrderItems)
                .HasForeignKey(oi => oi.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            // Cart → User (One-to-One)
            modelBuilder.Entity<Cart>()
                .HasOne(c => c.User)
                .WithOne(u => u.Cart)
                .HasForeignKey<Cart>(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);


            // CartItem → Cart
            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.Cart)
                .WithMany(c => c.CartItems)
                .HasForeignKey(ci => ci.CartId)
                .OnDelete(DeleteBehavior.Cascade);

            // CartItem → Product
            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.Product)
                .WithMany(p => p.CartItems)
                .HasForeignKey(ci => ci.ProductId)
                .OnDelete(DeleteBehavior.Restrict);


        }

        private static void ConfigureDecimalPrecision(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>()
                .Property(p => p.Price)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Order>()
                .Property(o => o.TotalAmount)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<OrderItem>()
                .Property(oi => oi.UnitPrice)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<OrderItem>()
                .Property(oi => oi.LineTotal)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<CartItem>()
                .Property(ci => ci.UnitPrice)
                .HasColumnType("decimal(18,2)");
        }

        private static void ConfigureGlobalQueryFilters(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Address>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<Category>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<Product>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<Order>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<OrderItem>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<Cart>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<CartItem>().HasQueryFilter(e => !e.IsDeleted);
        }

        #endregion

        #region SaveChanges Overrides (Soft Delete + Audit)

        public override int SaveChanges()
        {
            ApplyAuditAndSoftDelete();
            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(
            CancellationToken cancellationToken = default)
        {
            ApplyAuditAndSoftDelete();
            return await base.SaveChangesAsync(cancellationToken);
        }

        private void ApplyAuditAndSoftDelete()
        {
            foreach (var entry in ChangeTracker.Entries<BaseEntity>())
            {
                // Cart data is transient — use hard delete, not soft delete
                bool isCartEntity = entry.Entity is Cart or CartItem;

                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedAt = DateTime.UtcNow;
                        break;

                    case EntityState.Modified:
                        entry.Entity.UpdatedAt = DateTime.UtcNow;
                        break;

                    case EntityState.Deleted:
                        if (isCartEntity)
                            break; // let EF perform a real DELETE

                        entry.State = EntityState.Modified;
                        entry.Entity.IsDeleted = true;
                        entry.Entity.UpdatedAt = DateTime.UtcNow;
                        break;
                }
            }
        }

        #endregion
    }
}