using System.Text;

namespace Imagegram.Features;

public static class StringExtensions
{
    public static string ConvertToBase64(this string value)
    {
        var bytes = Encoding.Default.GetBytes(value);
        return Convert.ToBase64String(bytes);
    }
    
    public static string ConvertFromBase64(this string value)
    {
        byte[] fromBase64String = Convert.FromBase64String(value);
        return Encoding.Default.GetString(fromBase64String);
    }

    
}