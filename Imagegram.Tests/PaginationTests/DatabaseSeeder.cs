using System.Text.Json;
using Imagegram.Database;
using Imagegram.Database.Entities;
using Imagegram.Tests.PaginationTests.Dtos;
using Microsoft.EntityFrameworkCore;

namespace Imagegram.Tests.PaginationTests;

public class DatabaseSeeder
{
    private readonly DbContextOptions<ApplicationDbContext> _options;

    public DatabaseSeeder(DbContextOptions<ApplicationDbContext> options)
    {
        _options = options;
    }
    
    public async Task<List<Post>> SeedDataAsync()
    {
        var postsFile = Path.Combine(Directory.GetCurrentDirectory(), "PaginationTests\\Posts.json");
        var usersFile = Path.Combine(Directory.GetCurrentDirectory(), "PaginationTests\\Users.json");

        PostJsonDto[]? postDtos = JsonSerializer.Deserialize<PostJsonDto[]>(File.OpenRead(postsFile));
        UserJsonDto[]? userDtos = JsonSerializer.Deserialize<UserJsonDto[]>(File.OpenRead(usersFile));
        
        var db = new ApplicationDbContext(_options);

        await db.Database.EnsureDeletedAsync();
        await db.Database.EnsureCreatedAsync();

        var newUsers = userDtos!.Select(x => new User()
        {
            Email = x.Email,
            Password = x.Password
        }).ToList();

        db.Users.AddRange(newUsers);

        await db.SaveChangesAsync();


        // TODO Continue from here

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


    ImageInfo CreateSomeImageInfo() => new($"some-name-{Guid.NewGuid()}", $"some-uri-{Guid.NewGuid()}");

}