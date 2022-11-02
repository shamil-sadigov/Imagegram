namespace Imagegram.Api.Features;

public interface ISystemTime
{
    DateTimeOffset CurrentUtc { get; }
}