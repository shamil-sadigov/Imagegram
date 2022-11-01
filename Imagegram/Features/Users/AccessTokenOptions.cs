using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Imagegram.Features.Users;

public class AccessTokenOptions
{
    /// <summary>
    /// Private key on the basis of which jwt will be generated
    /// </summary>
    public string SecretKey { get; set; }
    
    public string AppName { get; set; }
    
    public byte[] GetSecretBytes()
        => Encoding.UTF8.GetBytes(SecretKey);

    public void ThrowIfNotValid()
    {
        if (string.IsNullOrWhiteSpace(SecretKey))
            throw new ValidationException($"{nameof(SecretKey)} is not provided for {nameof(AccessTokenOptions)}");
        
        if (string.IsNullOrWhiteSpace(SecretKey))
            throw new ValidationException($"{nameof(AppName)} is not provided for {nameof(AccessTokenOptions)}");
    }
}
