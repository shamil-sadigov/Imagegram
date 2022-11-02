using Imagegram.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Imagegram.Database.Configurations;

#pragma warning disable CS8618
public class CommentConfiguration:IEntityTypeConfiguration<Comment>
{
    public void Configure(EntityTypeBuilder<Comment> entity)
    { 
        entity.HasKey(x => x.Id);

        entity.Property(x => x.Text)
            .HasMaxLength(3072);

        entity.HasOne<User>()
            .WithMany()
            .HasForeignKey(x => x.CommentedBy)
            .OnDelete(DeleteBehavior.NoAction);
    }
}