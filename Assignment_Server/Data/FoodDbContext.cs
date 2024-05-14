using Assignment_Server.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Assignment_Server.Data
{
    public class FoodDbContext : IdentityDbContext<User>
    {
        public FoodDbContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<FoodImage> FoodImages { get; set; }
        public DbSet<Food> Foods { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartDetail> CartDetails { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            List<IdentityRole> roles = new List<IdentityRole>()
            {
                new IdentityRole()
                {
                    Name = "admin",
                    NormalizedName = "ADMIN"
                },
                new IdentityRole()
                {
                    Name = "customer",
                    NormalizedName = "CUSTOMER"
                }
            };
            builder.Entity<IdentityRole>().HasData(roles);
        }
    }
}
