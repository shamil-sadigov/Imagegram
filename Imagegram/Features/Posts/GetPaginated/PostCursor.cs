namespace Imagegram.Features.Posts.GetPaginated;

public sealed class PostCursor
{
    public int NumberOfComments { get; }
    public long Timestamp { get; }
    public int PostId { get; }

    public PostCursor(int postId, int numberOfComments, long timestamp)
    {
        if (numberOfComments < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(numberOfComments), "Should be greater than 0");
        }
        
        if (timestamp < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(timestamp), "Should be greater than 0");
        }
        
        if (postId < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(timestamp), "Should be greater than 0");
        }

        NumberOfComments = numberOfComments;
        Timestamp = timestamp;
        PostId = postId;
    }
    
    // TODO: This should be used on controller side
    public static bool TryCreateFromBase64(string base64EncodedValue, out PostCursor? cursor)
    {
        string[] values = base64EncodedValue.ConvertFromBase64().Split(":");

        cursor = default;
        
        if (values.Length != 3)
            return false;

        if (!int.TryParse(values[0], out var postId))
            return false;
        
        if (!int.TryParse(values[1], out var numberOfComments))
            return false;

        if (!long.TryParse(values[2], out var timestamp))
            return false;
        
        cursor =  new PostCursor(postId, numberOfComments, timestamp);
        return true;
    }
    
    public string ToBase64()
    {
        return $"{PostId}:{NumberOfComments}:{Timestamp}".ConvertToBase64();
    }
    
}