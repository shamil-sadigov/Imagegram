using Imagegram.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace Imagegram.Database;

#pragma warning disable CS8618


public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> ops) : base(ops)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Post> Posts { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}