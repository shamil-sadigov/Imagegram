using FluentAssertions;
using Imagegram.Features.Posts.GetPaginated;

namespace Imagegram.Tests;

public class PostCursorTests
{
    [Theory]
    [InlineData(10, 999, "10:999")]
    [InlineData(20, 87, "20:87")]
    [InlineData(3, 12, "3:12")]
    public void PostCursor_should_be_converted_to_correct_base64_representation(
        int numberOfComments, 
        long rowVersion, 
        string expectedValue)
    {
        var cursor = new PostCursor(numberOfComments, rowVersion);

        var base64EncodedCursor = cursor.ToBase64();

        base64EncodedCursor.Should().Be(expectedValue.ConvertToBase64());
    }
    
    [Theory]
    [InlineData(10, 999, "10:999")]
    [InlineData(20, 87, "20:87")]
    [InlineData(3, 12, "3:12")]
    public void PostCursor_can_be_created_from_correctly_encoded_base64_value(
        int expectedNumberOfComments, 
        long expectedRowVersion, 
        string value)
    {
        // Arrange
        var base64EncodedValue = value.ConvertToBase64();

        // Act
        var succeeded = PostCursor.TryCreateFromBase64(base64EncodedValue, out PostCursor? createdCursor);
        
        // Assert
        succeeded.Should().BeTrue();
        createdCursor.Should().NotBeNull();
        createdCursor!.RowVersion.Should().Be(expectedRowVersion);
        createdCursor!.NumberOfComments.Should().Be(expectedNumberOfComments);
    }
    
    [Theory]
    [InlineData("2:999#")]
    [InlineData("incorrect:value")]
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