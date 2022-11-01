namespace Imagegram.Features.Posts.GetPaginated;

public struct PageSize
{
    // TODO: Validate in API that value is Less than Min and more than Max
    public const int MaxAllowedPageSize = 50;
    public const int MinAllowedPageSize = 1;
    
    public int Value { get; }

    public PageSize(int value)
    {
        if (value < MinAllowedPageSize)
            throw new ArgumentOutOfRangeException(nameof(value));
        
        Value = Math.Min(value, MaxAllowedPageSize);
    }
    
    public static implicit operator int (PageSize pageSize)
    {
        return pageSize.Value;
    }
}