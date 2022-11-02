using FluentAssertions;
using Imagegram.Features;
using Imagegram.Features.Posts.GetPaginated.Pagination;

namespace Imagegram.Tests;

public class PostCursorTests
{
    [Theory]
    [InlineData(1, 10, 999, "1:10:999")]
    [InlineData(2, 20, 87, "2:20:87")]
    [InlineData(3, 6, 12, "3:6:12")]
    public void PostCursor_should_be_converted_to_correct_urlEncoded_format(
        int postId,
        int commentCount, 
        long timestamp, 
        string expectedValue)
    {
        var cursor = new PostCursor(postId, commentCount, timestamp);

        var urlEncodedValue = cursor.UrlEncoded();

        urlEncodedValue.Should().Be(expectedValue.UrlEncoded());
    }
    
    [Theory]
    [InlineData(1, 10, 999, "1:10:999")]
    [InlineData(2, 20, 87, "2:20:87")]
    [InlineData(3, 6, 12, "3:6:12")]
    public void PostCursor_can_be_created_from_correctly_urlEncoded_value(
        int postId,
        int expectedCommentCount, 
        long expectedTimestamp, 
        string value)
    {
        // Arrange
        var urlEncodedValue = value.UrlEncoded();

        // Act
        var succeeded = PostCursor.TryCreateFromUrlEncoded(urlEncodedValue, out PostCursor? createdCursor);
        
        // Assert
        succeeded.Should().BeTrue();
        createdCursor.Should().NotBeNull();
        createdCursor!.PostId.Should().Be(postId);
        createdCursor.Timestamp.Should().Be(expectedTimestamp);
        createdCursor.CommentCount.Should().Be(expectedCommentCount);
    }
    
    [Theory]
    [InlineData("2:999#")]
    [InlineData("2:incorrect:value")]
    [InlineData("the wall")]
    public void PostCursor_cannot_be_created_from_invalid_urlEncoded_value(string value)
    {
        // Act
        var convertToBase64 = value.UrlEncoded();
        
        var succeeded = PostCursor.TryCreateFromUrlEncoded(convertToBase64, out PostCursor? createdCursor);
        
        // Assert
        succeeded.Should().BeFalse();
        createdCursor.Should().BeNull();
    }
}