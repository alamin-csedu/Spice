using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Spice.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Spice.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Category> CategoryTb { get; set; }
        public DbSet<SubCategory> SubCategoryTb { get; set; }
        public DbSet<MenuItem> MenuItemTb { get; set; }
        public DbSet<Coupon> CouponTb { get; set; }
        public DbSet<ApplicationUser> ApplicationUserTb { get; set; }

        public DbSet<ShoppingCart> ShoppingCartTb { get; set; }
        public DbSet<OrderHeader> OrderHeaderTb { get; set; }
        public DbSet<OrderDetails> OrderDetailsTb { get; set; }

        internal Task FindAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
