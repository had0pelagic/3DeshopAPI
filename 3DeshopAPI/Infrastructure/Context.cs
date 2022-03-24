using Domain;
using Domain.Order;
using Domain.Payment;
using Domain.Product;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure
{
    public class Context : DbContext
    {
        public Context(DbContextOptions dbContextOptions) : base(dbContextOptions)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        }

        public DbSet<User> Users { get; set; }

        public DbSet<Product> Products { get; set; }
        public DbSet<ProductCategories> ProductCategories { get; set; }
        public DbSet<ProductFiles> ProductFiles { get; set; }
        public DbSet<ProductFormats> ProductFormats { get; set; }
        public DbSet<ProductImages> ProductImages { get; set; }
        public DbSet<About> About { get; set; }
        public DbSet<Specifications> Specifications { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Format> Formats { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<Domain.Product.File> Files { get; set; }

        public DbSet<Offer> Offers { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderImages> OrderImages { get; set; }
        public DbSet<OrderOffers> OrderOffers { get; set; }
        public DbSet<Job> Jobs { get; set; }

        public DbSet<Payment> Payments { get; set; }
    }
}