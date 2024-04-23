using Microsoft.EntityFrameworkCore;
using Test.Domain.Entities;

namespace Test.Data.DbContexts;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Product> Products { get; set; }
}