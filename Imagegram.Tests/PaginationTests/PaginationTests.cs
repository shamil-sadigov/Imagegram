using Imagegram.Database;
using Imagegram.Features;
using Imagegram.Features.Posts.GetPaginated.Pagination;
using Imagegram.Features.Posts.GetPaginated.PaginationStrategies;
using Microsoft.EntityFrameworkCore;

namespace Imagegram.Tests.PaginationTests;

// TODO: Inject connection string from configuration

public class PaginationTests
{
    private const string ConnectionString = "Data Source=.;Initial Catalog=ImagegramTests;Integrated Security=True;";

    private PageSize _pageSize;
    
    private readonly DbContextOptions<ApplicationDbContext> _dbOptions
        = new DbContextOptionsBuilder<ApplicationDbContext>().UseSqlServer(ConnectionString).Options;

    public PaginationTests()
    {
        var databaseSeeder = new DatabaseSeeder(_dbOptions);
        databaseSeeder.SeedDataAsync().Wait();
    }
    
    /*
        Pages of Posts sorted by CommentCount, LastTimeUpdatedAt, ID that are available in DB.
        Each Page contains 3 elements

        CommentCount    LastTimeUpdatedAt           ID
        --------------------------------------------------
        9               30.10.2022 18:23:51         6         <== Start cursor
        8               01.11.2022 4:23:51          7         
        8               30.10.2022 18:23:51         3         <== End cursor
        --------------------------------------------------  
        5               31.10.2022 18:23:51         10        
        4               31.10.2022 18:23:51         4         
        4               31.10.2022 18:23:51         2         
        --------------------------------------------------    
        4               30.10.2022 12:23:51         5         
        3               12.10.2022 12:12:51         11        
        1               21.10.2022 18:23:51         8     
        -------------------------------------------------    
        0               01.11.2022 5:15:52          9         
        0               01.11.2022 5:15:52          1 
            
    */
    
    [Fact]
    public async Task Navigating_over_pages_should_be_as_expected()
    {
        _pageSize = new PageSize(3);
        
        // 1st page
        var firstPage = await new FirstPagePaginationStrategy(new ApplicationDbContext(_dbOptions))
            .PaginateAsync(pageSize: _pageSize, cursor: null);

        AssertFirstPageIsValid(firstPage, and => and.CanNavigateToTheNextPage());
        
        // Go to 2nd page
        var secondPage = await new NextPagePaginationStrategy(new ApplicationDbContext(_dbOptions))
            .PaginateAsync(pageSize: _pageSize, firstPage.EndCursor);
        
        AssertSecondPageIsValid(secondPage);
        
        // Go to 3d page
        var thirdPage = await new NextPagePaginationStrategy(new ApplicationDbContext(_dbOptions))
            .PaginateAsync(_pageSize, secondPage.EndCursor);
        
        AssertThirdPageIsValid(thirdPage);
        
        // Go to 4th page
        var forthPage = await new NextPagePaginationStrategy(new ApplicationDbContext(_dbOptions))
            .PaginateAsync(_pageSize, thirdPage.EndCursor);
        
        AssertForthPageIsValid(forthPage);
        
        // Go back to 3th page
        thirdPage = await new PreviousPagePaginationStrategy(new ApplicationDbContext(_dbOptions))
            .PaginateAsync(_pageSize, forthPage.StartCursor);
        
        AssertThirdPageIsValid(thirdPage);
        
        // Go back to 2nd page
        secondPage = await new PreviousPagePaginationStrategy(new ApplicationDbContext(_dbOptions))
            .PaginateAsync(_pageSize, thirdPage.StartCursor);
        
        AssertSecondPageIsValid(secondPage);
        
        // Go back to 1st page
        firstPage = await new PreviousPagePaginationStrategy(new ApplicationDbContext(_dbOptions))
            .PaginateAsync(_pageSize, secondPage.StartCursor);
        
        AssertFirstPageIsValid(firstPage, and => and.CannotNavigateToThePreviousPage());
    }

    private void AssertForthPageIsValid(PaginatedResult<PostDto, PostCursor> forthPage)
    {
        forthPage
            .StartCursorShouldBe(commentCount: 0, postId: 9)
            .EndCursorShouldBe(commentCount: 0, postId: 1)
            .CannotNavigateToTheNextPage()
            .ShouldHavePageSize(requestedPageSize: _pageSize, actualPageSize: 2)
            .ShouldContainItemsCount(2)
            .ShouldContainInOrder
            (
                (commentCount: 0, postId: 9),
                (commentCount: 0, postId: 1)
            );
    }

    private void AssertFirstPageIsValid(
        PaginatedResult<PostDto, PostCursor> firstPage,
        Action<PaginatedResult<PostDto, PostCursor>> additionalAssertion)
    {
        firstPage
            .StartCursorShouldBe(commentCount: 9, postId: 6)
            .EndCursorShouldBe(commentCount: 8, postId: 3)
            .ShouldHavePageSize(requestedPageSize: _pageSize, actualPageSize: _pageSize)
            .ShouldContainItemsCount(3)
            .ShouldContainInOrder
            (
                (commentCount: 9, postId: 6),
                (commentCount: 8, postId: 7),
                (commentCount: 8, postId: 3)
            );

        additionalAssertion(firstPage);
    }

    private void AssertThirdPageIsValid(PaginatedResult<PostDto, PostCursor> thirdPage)
    {
        thirdPage
            .StartCursorShouldBe(commentCount: 4, postId: 5)
            .EndCursorShouldBe(commentCount: 1, postId: 8)
            .CanNavigateToTheNextPage()
            .ShouldHavePageSize(requestedPageSize: _pageSize, actualPageSize: _pageSize)
            .ShouldContainItemsCount(3)
            .ShouldContainInOrder
            (
                (commentCount: 4, postId: 5),
                (commentCount: 3, postId: 11),
                (commentCount: 1, postId: 8)
            );
    }

    private void AssertSecondPageIsValid(PaginatedResult<PostDto, PostCursor> secondPage)
    {
        secondPage
            .StartCursorShouldBe(commentCount: 5, postId: 10)
            .EndCursorShouldBe(commentCount: 4, postId: 2)
            .CanNavigateToTheNextPage()
            .ShouldHavePageSize(requestedPageSize: _pageSize, actualPageSize: _pageSize)
            .ShouldContainItemsCount(3)
            .ShouldContainInOrder
            (
                (commentCount: 5, postId: 10),
                (commentCount: 4, postId: 4),
                (commentCount: 4, postId: 2)
            );
    }
    
}