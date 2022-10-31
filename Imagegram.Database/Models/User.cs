namespace Imagegram.Database.Models;

#pragma warning disable CS8618


// No need for salt. it's just app
public sealed class User:BaseEntity
{
    public string Email { get; set; }
    public string Password { get; set; }
}