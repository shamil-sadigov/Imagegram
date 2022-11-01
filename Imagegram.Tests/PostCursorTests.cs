using FluentAssertions;
using Imagegram.Features;
using Imagegram.Features.Posts.GetPaginated;
using Imagegram.Features.Posts.GetPaginated.Pagination;

namespace Imagegram.Tests;

public class PostCursorTests
{
    [Theory]
    [InlineData(1, 10, 999, "1:10:999")]
    [InlineData(2, 20, 87, "2:20:87")]
    [InlineData(3, 6, 12, "3:6:12")]
    public void PostCursor_should_be_converted_to_correct_base64_representation(
        int postId,
        int commentCount, 
        long timestamp, 
        string expectedValue)
    {
        var cursor = new PostCursor(postId, commentCount, timestamp);

        var base64EncodedCursor = cursor.ToBase64();

        base64EncodedCursor.Should().Be(expectedValue.ConvertToBase64());
    }
    
    [Theory]
    [InlineData(1, 10, 999, "1:10:999")]
    [InlineData(2, 20, 87, "2:20:87")]
    [InlineData(3, 6, 12, "3:6:12")]
    public void PostCursor_can_be_created_from_correctly_encoded_base64_value(
        int postId,
        int expectedCommentCount, 
        long expectedTimestampt, 
        string value)
    {
        // Arrange
        var base64EncodedValue = value.ConvertToBase64();

        // Act
        var succeeded = PostCursor.TryCreateFromBase64(base64EncodedValue, out PostCursor? createdCursor);
        
        // Assert
        succeeded.Should().BeTrue();
        createdCursor.Should().NotBeNull();
        createdCursor!.PostId.Should().Be(postId);
        createdCursor.Timestamp.Should().Be(expectedTimestampt);
        createdCursor.CommentCount.Should().Be(expectedCommentCount);
    }
    
    [Theory]
    [InlineData("2:999#")]
    [InlineData("2:incorrect:value")]
    [InlineData("the wall")]
    public void PostCursor_cannot_be_created_from_invalid_base64_value(string value)
    {
        // Act
        var convertToBase64 = value.ConvertToBase64();
        
        var succeeded = PostCursor.TryCreateFromBase64(convertToBase64, out PostCursor? createdCursor);
        
        // Assert
        succeeded.Should().BeFalse();
        createdCursor.Should().BeNull();
    }
}