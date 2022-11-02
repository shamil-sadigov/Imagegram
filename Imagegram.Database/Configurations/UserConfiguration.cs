using Imagegram.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Imagegram.Database.Configurations;

#pragma warning disable CS8618
public class UserConfiguration:IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> entity)
    {
        entity.HasKey(x => x.Id);
            
        entity.Property(x => x.Password)
            .HasMaxLength(256);
            
        entity.Property(x => x.Email)
            .HasMaxLength(256);
    }
}