using FluentAssertions;
using FluentAssertions.Execution;
using FluentAssertions.Extensions;
using Imagegram.Database;
using Imagegram.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace Imagegram.Tests.ApplicationDbContextTests;

// TODO: Test that post is saved correctly

// [CollectionDefinition("Non-parallel-tests", DisableParallelization = true)]
public class ApplicationDbContextTests : IDisposable
{
    // TODO: Extract
    private const string ConnectionString = "Data Source=.;Initial Catalog=ImagegramTests;Integrated Security=True;";

    private readonly DbContextOptions<ApplicationDbContext> _dbOptions
        = new DbContextOptionsBuilder<ApplicationDbContext>().UseSqlServer(ConnectionString).Options;

    public void Dispose()
    {
        using var dbContext = new ApplicationDbContext(_dbOptions);
        dbContext.Database.EnsureDeleted();
    }
    
    [Fact]
    public async Task New_Post_is_correctly_saved_in_database()
    {
        // Arrange
        var (dbContext, user) = RecreateDatabaseWithSomeUser();

        var currentDateTime = DateTimeOffset.UtcNow;

        var newPost = new Post(
            user.Id,
            "some-description",
            new ImageInfo("original-image-name", "original-image-uri"),
            new ImageInfo("processed-image-name", "processed-image-uri"),
            currentDateTime);

        // Act

        await dbContext.SaveEntitiesAsync(newPost);

        // Assert
        using (var newDbContext = new ApplicationDbContext(_dbOptions))
        using (new AssertionScope())
        {
            var foundPost = newDbContext.Posts.First(x => x.Id == newPost.Id);

            foundPost.Description.Should().Be("some-description");
            foundPost.Image.OriginalImage.Name.Should().Be("original-image-name");
            foundPost.Image.OriginalImage.Uri.Should().Be("original-image-uri");

            foundPost.Image.ProcessedImage.Name.Should().Be("processed-image-name");
            foundPost.Image.ProcessedImage.Uri.Should().Be("processed-image-uri");
            foundPost.CreatedAt.Should().Be(currentDateTime);
            foundPost.CreatedBy.Should().Be(user.Id);
        }
    }

    [Fact]
    public async Task Post_with_new_comments_saved_correctly_in_database()
    {
        // Arrange
        (ApplicationDbContext dbContext, var postOwner) = RecreateDatabaseWithSomeUser();

        var newPost = CreateSomePost(postOwner);
        
        var someUser = CreateSomeUser();
        var someUser2 = CreateSomeUser();

        // Act

        await dbContext.SaveEntitiesAsync(newPost, someUser, someUser2);

        var currentDateTime = DateTimeOffset.UtcNow;

        using (var newDbContext = new ApplicationDbContext(_dbOptions))
        {
            var post = newDbContext.Posts.First(x => x.Id == newPost.Id);
            post.AddComment("user comment", someUser.Id, currentDateTime - 10.Seconds());
            post.AddComment("user2 comment", someUser2.Id, currentDateTime);
            await newDbContext.SaveChangesAsync();
        }

        // Assert
        using (var newDbContext = new ApplicationDbContext(_dbOptions))
        using (new AssertionScope())
        {
            var foundPost = newDbContext.Posts
                .Include(x => x.Comments)
                .First(x => x.Id == newPost.Id);

            foundPost.CommentCount.Should().Be(2);
            foundPost.LastTimeUpdatedAt.Should().Be(currentDateTime);

            foundPost.Comments.Should().HaveCount(2);

            foundPost.Comments.Should().Contain(
                comment => comment.CommentedBy == someUser.Id && comment.Text == "user comment");

            foundPost.Comments.Should().Contain(
                comment => comment.CommentedBy == someUser2.Id && comment.Text == "user2 comment");
        }
    }

    [Fact]
    public async Task Post_with_deleted_comments_saved_correctly_in_database()
    {
        // Arrange
        (ApplicationDbContext dbContext, User postOwner) = RecreateDatabaseWithSomeUser();

        var newPost = CreateSomePost(postOwner);
        newPost.AddComment("some-comment", postOwner.Id, DateTimeOffset.UtcNow);

        await dbContext.SaveEntitiesAsync(newPost);

        // Act
        var currentDateTime = DateTimeOffset.UtcNow;

        using (var newDbContext = new ApplicationDbContext(_dbOptions))
        {
            var post = newDbContext
                .Posts
                .Include(x => x.Comments)
                .First(x => x.Id == newPost.Id);

            var postOwnerComment = post.Comments.Single(x => x.CommentedBy == postOwner.Id);

            post.RemoveComment(postOwnerComment.Id, currentDateTime);

            await newDbContext.SaveChangesAsync();
        }

        // Assert
        using (var newDbContext = new ApplicationDbContext(_dbOptions))
        using (new AssertionScope())
        {
            var foundPost = newDbContext.Posts
                .Include(x => x.Comments)
                .First(x => x.Id == newPost.Id);

            foundPost.Comments.Should().HaveCount(0);
            foundPost.CommentCount.Should().Be(0);

            foundPost.LastTimeUpdatedAt.Should().Be(currentDateTime);
        }
    }

    private static User CreateSomeUser()
    {
        var user = new User
        {
            Email = $"{Guid.NewGuid()}@gmail.com",
            Password = "pass@#",
            CreatedAt = DateTimeOffset.Now
        };

        return user;
    }

    private static Post CreateSomePost(User user)
    {
        return new Post(
            user.Id,
            "some-description",
            new ImageInfo("original-image-name", "original-image-uri"),
            new ImageInfo("processed-image-name", "processed-image-uri"),
            DateTimeOffset.UtcNow);
    }

    private (ApplicationDbContext context, User user) RecreateDatabaseWithSomeUser()
    {
        var context = new ApplicationDbContext(_dbOptions);
        
        context.Database.EnsureCreated();

        if (!context.Database.CanConnect()) 
            throw new InvalidOperationException("Database is not accessible!");

        var user = new User
        {
            Email = "some-user@gmail.com",
            Password = "pass@#",
            CreatedAt = DateTimeOffset.Now
        };

        context.Users.Add(user);

        context.SaveChanges();
        return (context, user);
    }
}