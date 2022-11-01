namespace Imagegram.Database.Models;

#pragma warning disable CS8618


// No need for salt. it's just app
public sealed class User:BaseEntity
{
    // TODO: Add index
    public string Email { get; set; }
    
    /// <summary>
    /// Password in protected format. Not in raw
    /// </summary>
    public string Password { get; set; }
}