using Assignment_Server.Models;
using Microsoft.EntityFrameworkCore;

namespace Assignment_Server.Data
{
    public class FoodDbContext : DbContext
    {
        public FoodDbContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<FoodImage> FoodImages { get; set; }
        public DbSet<Food> Foods { get; set; }
        public DbSet<Category> Categories { get; set; }
    }
}
