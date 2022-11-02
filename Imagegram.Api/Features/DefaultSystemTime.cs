namespace Imagegram.Api.Features;

public class DefaultSystemTime:ISystemTime
{
    public DateTimeOffset CurrentUtc => DateTimeOffset.UtcNow;
}