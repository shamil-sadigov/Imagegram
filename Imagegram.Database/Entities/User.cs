namespace Imagegram.Database.Entities;

#pragma warning disable CS8618

public sealed class User:BaseEntity
{
    public string Email { get; set; }
    
    /// <summary>
    /// Password in protected format. Not in raw
    /// </summary>
    public string Password { get; set; }
}