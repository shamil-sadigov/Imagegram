using Imagegram.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Imagegram.Database.Configurations;

#pragma warning disable CS8618
public class PostConfiguration:IEntityTypeConfiguration<Post>
{
    public void Configure(EntityTypeBuilder<Post> entity)
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
    }
}