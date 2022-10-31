namespace Imagegram.Features.Posts.GetPaginated;

public sealed class PostCursor
{
    public int NumberOfComments { get; }
    public long RowVersion { get; }

    public PostCursor(int numberOfComments, long rowVersion)
    {
        if (numberOfComments < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(numberOfComments), "Should be greater than 0");
        }
        
        if (rowVersion < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(rowVersion), "Should be greater than 0");
        }

        NumberOfComments = numberOfComments;
        RowVersion = rowVersion;
    }
    
    // TODO: This should be used on controller side
    public static bool TryCreateFromBase64(string base64EncodedValue, out PostCursor? cursor)
    {
        string[] values = base64EncodedValue.ConvertFromBase64().Split(":");

        cursor = default;
        
        if (values.Length is > 2 or < 1)
            return false;

        if (!int.TryParse(values[0], out var numberOfComments))
            return false;

        if (!long.TryParse(values[1], out var rowVersion))
            return false;
        
        cursor =  new PostCursor(numberOfComments, rowVersion);
        return true;
    }
    
    public string ToBase64()
    {
        return $"{NumberOfComments}:{RowVersion}".ConvertToBase64();
    }
    
}