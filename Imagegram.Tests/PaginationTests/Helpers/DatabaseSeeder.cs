using System.Text.Json;
using Imagegram.Database.Entities;
using Imagegram.Tests.PaginationTests.Dtos;

namespace Imagegram.Tests.PaginationTests.Helpers;

public class DatabaseSeeder
{
    public async Task<List<Post>> SeedDataAsync()
    {
        var postsFile = Path.Combine(Directory.GetCurrentDirectory(), "PaginationTests\\MockData\\Posts.json");
        var usersFile = Path.Combine(Directory.GetCurrentDirectory(), "PaginationTests\\MockData\\Users.json");

        EnsureFilesExist(postsFile, usersFile);

        PostJsonDto[]? postDtos = JsonSerializer.Deserialize<PostJsonDto[]>(File.OpenRead(postsFile));
        UserJsonDto[]? userDtos = JsonSerializer.Deserialize<UserJsonDto[]>(File.OpenRead(usersFile));
        
        var db = TestEnvironment.CreateDbContext();
        
        var newUsers = userDtos!.Select(x => new User()
        {
            Email = x.Email,
            ProtectedPassword = x.Password
        }).ToList();

        db.Users.AddRange(newUsers);

        await db.SaveChangesAsync();
        
        var posts = postDtos!.Select(dto =>
        {
            var post = new Post
            (
                newUsers.Random().Id,
                dto.Description,
                CreateSomeImageInfo(),
                CreateSomeImageInfo(),
                DateTimeOffset.UtcNow 
            );

            foreach (var commentDto in dto.Comments.OrderBy(x => x.CreatedAt))
                post.AddComment(commentDto.Text, newUsers.Random().Id, commentDto.CreatedAt);

            return post;
        }).ToList();

        db.Posts.AddRange(posts);
        await db.SaveChangesAsync();
        
        return posts;
    }

    private static void EnsureFilesExist(string postsFile, string usersFile)
    {
        if (!File.Exists(postsFile))
        {
            throw new FileNotFoundException($"{postsFile} is not found for seeding!");
        }

        if (!File.Exists(usersFile))
        {
            throw new FileNotFoundException($"{usersFile} is not found for seeding!");
        }
    }
    
    ImageInfo CreateSomeImageInfo() => new($"some-name-{Guid.NewGuid()}", $"some-uri-{Guid.NewGuid()}");
}