using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Product_API.Models;

namespace Product_API.Data
{
  public class AppDbContext : DbContext
  {
    protected readonly IConfiguration Configuration;

    public AppDbContext(IConfiguration configuration)
    {
      // This solve problem about UTC DateTime
      AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
      Configuration = configuration;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
      // connect to postgres with connection string from app settings
      options.UseNpgsql(Configuration.GetConnectionString("WebApiDatabase"));
    }

    public DbSet<Product> Products { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      modelBuilder.Entity<Product>().HasData(
        new Product()
        {
          Id = 1,
          Name = "Lenovo Legion",
          Detail = "Laptop Lenovo",
          Price = 3000,
          ImageUrl = "",
          Create = DateTime.Now,
          Update = DateTime.Now,
        }
      );
    }
  }
}