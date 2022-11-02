using Imagegram.Database;
using Imagegram.Database.Entities;

namespace Imagegram.Tests.ApplicationDbContextTests;

public static class ApplicationDbContextExtensions
{
    public static async Task SaveEntitiesAsync(
        this ApplicationDbContext dbContext, 
        Post post, 
        params User[]? users)
    {
        await dbContext.Posts.AddAsync(post);
            
        if(users is not null)
            await dbContext.Users.AddRangeAsync(users);
        
        await dbContext.SaveChangesAsync();
    }

}