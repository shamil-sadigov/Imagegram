using System.Text;

namespace Imagegram.Features.Users.GetUserAccessToken;

// TODO: Configure in options
public class AccessTokenOptions
{
    /// <summary>
    /// Private key on the basis of which jwt will be generated
    /// </summary>
    public string SecretKey { get; set; }
    
    public string AppName { get; set; }
    
    public TimeSpan TokenLifetime { get; set; }
    
    public byte[] GetSecretBytes()
        => Encoding.UTF8.GetBytes(SecretKey);
}