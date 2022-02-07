﻿using Domain;
using Domain.Product;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure
{
    public class Context : DbContext
    {
        public Context(DbContextOptions dbContextOptions) : base(dbContextOptions)
        {
        }

        public DbSet<User> Users { get; set; }

        public DbSet<Product> Products { get; set; }
        public DbSet<About> About { get; set; }
        public DbSet<Specifications> Specifications { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Format> Formats { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<ProductCategories> ProductCategories { get; set; }
        public DbSet<ProductComments> ProductComments { get; set; }
        public DbSet<ProductFormats> ProductFormats { get; set; }
        public DbSet<ProductImages> ProductImages { get; set; }
    }
}