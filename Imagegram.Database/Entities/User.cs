namespace Imagegram.Database.Entities;

#pragma warning disable CS8618

public sealed class User:BaseEntity
{
    public string Email { get; set; }
    
    public string ProtectedPassword { get; set; }
}