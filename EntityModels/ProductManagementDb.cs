using static ProductManagement.Helper.Helper;
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
    {
        optionsBuilder.UseSqlite(DevConfiguration["Database:Details:ConnectionString"]);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}