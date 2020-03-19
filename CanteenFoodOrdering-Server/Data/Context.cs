using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using CanteenFoodOrdering_Server.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace CanteenFoodOrdering_Server.Data
{
    public class Context : IdentityDbContext<IdentityUser>
    {
        public Context(DbContextOptions<Context> options) : base(options)
        {
        }

        public DbSet<Order> Orders { get; set; }
        public DbSet<Dish> Dishes { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<OrderedDish> OrderedDishes { get; set; }
        public DbSet<OrderHistory> OrderHistories { get; set; }
        public DbSet<DishHistory> DishHistories { get; set; }
        public DbSet<OrderedDishHistory> OrderedDishHistories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
