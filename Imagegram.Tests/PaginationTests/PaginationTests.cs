using Imagegram.Database;
using Imagegram.Features;
using Imagegram.Features.Posts.GetPaginated.Pagination;
using Imagegram.Features.Posts.GetPaginated.PaginationStrategies;
using Imagegram.Tests.PaginationTests.Helpers;
using Microsoft.EntityFrameworkCore;

namespace Imagegram.Tests.PaginationTests;

public class PaginationTests:IDisposable
{
    private PageSize _pageSize;
    
    public PaginationTests()
    {
        using var dbContext = TestEnvironment.CreateDbContext();
        dbContext.Database.EnsureCreated();
        
        var databaseSeeder = new DatabaseSeeder();
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
        var firstPage = await new FirstPagePaginationStrategy(TestEnvironment.CreateDbContext())
            .PaginateAsync(pageSize: _pageSize, cursor: null);

        AssertFirstPageIsValid(firstPage, and => and.CanNavigateToTheNextPage());
        
        // Go to 2nd page
        var secondPage = await new NextPagePaginationStrategy(TestEnvironment.CreateDbContext())
            .PaginateAsync(pageSize: _pageSize, firstPage.EndCursor);
        
        AssertSecondPageIsValid(secondPage);
        
        // Go to 3d page
        var thirdPage = await new NextPagePaginationStrategy(TestEnvironment.CreateDbContext())
            .PaginateAsync(_pageSize, secondPage.EndCursor);
        
        AssertThirdPageIsValid(thirdPage);
        
        // Go to 4th page
        var forthPage = await new NextPagePaginationStrategy(TestEnvironment.CreateDbContext())
            .PaginateAsync(_pageSize, thirdPage.EndCursor);
        
        AssertForthPageIsValid(forthPage);
        
        // Go back to 3th page
        thirdPage = await new PreviousPagePaginationStrategy(TestEnvironment.CreateDbContext())
            .PaginateAsync(_pageSize, forthPage.StartCursor);
        
        AssertThirdPageIsValid(thirdPage);
        
        // Go back to 2nd page
        secondPage = await new PreviousPagePaginationStrategy(TestEnvironment.CreateDbContext())
            .PaginateAsync(_pageSize, thirdPage.StartCursor);
        
        AssertSecondPageIsValid(secondPage);
        
        // Go back to 1st page
        firstPage = await new PreviousPagePaginationStrategy(TestEnvironment.CreateDbContext())
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

    public void Dispose()
    {
        using var dbContext = TestEnvironment.CreateDbContext();
        dbContext.Database.EnsureDeleted();
    }
}