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
    public DbSet<Comment> Comments { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Password).HasMaxLength(256);
            entity.Property(x => x.Email).HasMaxLength(256);
        });

        modelBuilder.Entity<Post>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.HasOne<User>()
                .WithOne()
                .HasForeignKey<Post>(x => x.OwnerId);

            entity.HasMany<Comment>()
                .WithOne()
                .HasForeignKey(x => x.PostId);

            entity.Property(x => x.Description)
                .HasMaxLength(2048);

            entity.Property(x => x.ImageUrl)
                .HasMaxLength(2048);
        });

        modelBuilder.Entity<Comment>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Text)
                .HasMaxLength(2048);

            entity.HasOne<User>()
                .WithOne()
                .HasForeignKey<Comment>(x => x.UserId);
        });
    }
}