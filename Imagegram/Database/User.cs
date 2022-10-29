namespace Imagegram.Database;

#pragma warning disable CS8618


// No need for salt. it's just app
public class User
{
    public int Id { get; set; }
    
    public string Email { get; set; }
    public string Password { get; set; }
}