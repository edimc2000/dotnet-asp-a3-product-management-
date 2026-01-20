using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ProductManagement;

public partial class ProductManagementDb : DbContext
{
    public ProductManagementDb()
    {
    }

    public ProductManagementDb(DbContextOptions<ProductManagementDb> options)
        : base(options)
    {
    }

    public virtual DbSet<Product> Products { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)

        => optionsBuilder.UseSqlite("Data Source=./Database/product_management.db");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
