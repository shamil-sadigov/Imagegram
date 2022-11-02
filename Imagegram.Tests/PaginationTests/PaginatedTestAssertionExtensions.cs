using FluentAssertions;
using Imagegram.Features;
using Imagegram.Features.Posts.GetPaginated.Pagination;

namespace Imagegram.Tests.PaginationTests;

public static class PaginatedTestAssertionExtensions
{
    public static PaginatedResult<PostDto, PostCursor> StartCursorShouldBe(
        this PaginatedResult<PostDto, PostCursor> page,
        int commentCount,
        int postId)
    {
        page.StartCursor!.PostId.Should().Be(postId);
        page.StartCursor!.CommentCount.Should().Be(commentCount);

        return page;
    }
    
    public static PaginatedResult<PostDto, PostCursor> EndCursorShouldBe(this PaginatedResult<PostDto, PostCursor> page,
        int commentCount,
        int postId)
    {
        page.EndCursor!.PostId.Should().Be(postId);
        page.EndCursor!.CommentCount.Should().Be(commentCount);
        
        return page;
    }
    
    public static PaginatedResult<PostDto, PostCursor> ShouldContainInOrder(
        this PaginatedResult<PostDto, PostCursor> page,
        params (int commentCount, int postId)[] tuple)
    {
        page.Items
            .Select(x=> (x.CommentCount, x.PostId))
            .Should()
            .ContainInOrder(tuple);

        return page;
    }
    
    public static PaginatedResult<PostDto, PostCursor> ShouldContainItemsCount(
        this PaginatedResult<PostDto, PostCursor> page,
        int itemsCount)
    {
        page.Items.Should().HaveCount(itemsCount);

        return page;
    }
    
    public static PaginatedResult<PostDto, PostCursor> ShouldHavePageSize(
        this PaginatedResult<PostDto, PostCursor> page,
        int requestedPageSize,
        int actualPageSize)
    {
        page.RequestedPageSize.Should().Be(requestedPageSize);
        page.ActualPageSize.Should().Be(actualPageSize);
        return page;
    }
    
    
    public static PaginatedResult<PostDto, PostCursor> CannotNavigateToTheNextPage(
        this PaginatedResult<PostDto, PostCursor> page)
    {
        page.HasMoreItems.Should().BeFalse();
        return page;
    }
    
    public static PaginatedResult<PostDto, PostCursor> CannotNavigateToThePreviousPage(
        this PaginatedResult<PostDto, PostCursor> page)
    {
        page.HasMoreItems.Should().BeFalse();
        return page;
    }
    
    public static PaginatedResult<PostDto, PostCursor> CanNavigateToTheNextPage(
        this PaginatedResult<PostDto, PostCursor> page)
    {
        page.HasMoreItems.Should().BeTrue();
        return page;
    }
    
    public static PaginatedResult<PostDto, PostCursor> CanNavigateToThePreviousPage(
        this PaginatedResult<PostDto, PostCursor> page)
    {
        page.HasMoreItems.Should().BeTrue();
        return page;
    }
}