using Imagegram.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace Imagegram.Database;

#pragma warning disable CS8618

// TODO: Migrate
// TODo: Register
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
        
        modelBuilder.Entity<PostImage>(entity =>
        {
            entity.HasKey(x => x.Id);
            
            entity.OwnsOne(x => x.ProcessedImage, w =>
            {
                w.Property(x => x.Name)
                    .HasMaxLength(1024)
                    .HasColumnName("ProcessedImageName");
                
                w.Property(x => x.Uri)
                    .HasMaxLength(2048)
                    .HasColumnName("ProcessedImageUri");
            });
            
            entity.OwnsOne(x => x.OriginalImage, w =>
            {
                w.Property(x => x.Name)
                    .HasMaxLength(1024)
                    .HasColumnName("OriginalImageName");
                
                w.Property(x => x.Uri)
                    .HasMaxLength(2048)
                    .HasColumnName("OriginalImageUri");
            });
            
            entity.HasOne<Post>()
                .WithOne()
                .HasForeignKey<PostImage>(x => x.PostId);
        });
    }
}