using Microsoft.EntityFrameworkCore;
using Test.Domain.Entities;

namespace Test.Desktop.DbContexts;

internal class AppDbContext : DbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseNpgsql("Host = localhost; Port=5432; Database = TestDb; UserId=postgres; Password=jasurbek20040414;");


    public DbSet<Product> Products { get; set; }
}
