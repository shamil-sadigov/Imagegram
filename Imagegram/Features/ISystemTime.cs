namespace Imagegram.Features;

public interface ISystemTime
{
    DateTimeOffset CurrentUtc { get; }
}