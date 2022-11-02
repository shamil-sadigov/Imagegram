namespace Imagegram.Database.Entities;

#pragma warning disable CS8618
public class BaseEntity
{
    public int Id { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
}