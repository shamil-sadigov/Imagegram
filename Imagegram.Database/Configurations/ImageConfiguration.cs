using Imagegram.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Imagegram.Database.Configurations;

#pragma warning disable CS8618
public class ImageConfiguration:IEntityTypeConfiguration<PostImage>
{
    public void Configure(EntityTypeBuilder<PostImage> entity)
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
    }
}