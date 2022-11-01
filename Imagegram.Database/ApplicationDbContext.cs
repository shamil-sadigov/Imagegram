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

    // TODO: Extract to configuration types
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(x => x.Id);
            
            entity.Property(x => x.Password)
                .HasMaxLength(256);
            
            entity.Property(x => x.Email)
                .HasMaxLength(256);
        });

        // TODO: Add indexes
        modelBuilder.Entity<Post>(entity =>
        {
            entity.HasKey(x => x.Id);
            
            entity.HasOne(x=>x.Image)
                .WithOne()
                .HasForeignKey<PostImage>(x => x.PostId)
                .OnDelete(DeleteBehavior.Cascade);
            
            entity.Navigation(x => x.Image)
                .AutoInclude();

            entity.HasOne<User>()
                .WithMany()
                .HasForeignKey(x => x.CreatedBy);
                // .OnDelete(DeleteBehavior.Restrict);
            
            entity.HasMany(x=> x.Comments)
                .WithOne()
                .HasForeignKey(x => x.PostId)
                .OnDelete(DeleteBehavior.Cascade);
            
            entity.Metadata
                .FindNavigation(nameof(Post.Comments))
                .SetPropertyAccessMode(PropertyAccessMode.Field);
            
            entity.Property(x => x.Description)
                .HasMaxLength(3072);

            entity.Property(x => x.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();
        });


        modelBuilder.Entity<Comment>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Text)
                .HasMaxLength(3072);

            entity.HasOne<User>()
                .WithMany()
                .HasForeignKey(x => x.CommentedBy)
                .OnDelete(DeleteBehavior.NoAction);
        });
        
        modelBuilder.Entity<PostImage>(entity =>
        {
            entity.HasKey(x => x.PostId);
            
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

    
        });
    }
}