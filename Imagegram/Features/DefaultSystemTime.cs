namespace Imagegram.Features;

public class DefaultSystemTime:ISystemTime
{
    public DateTimeOffset CurrentUtc => DateTimeOffset.UtcNow;
}