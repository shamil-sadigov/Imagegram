using System.Web;

namespace Imagegram.Features;

public static class StringExtensions
{
    public static string UrlEncoded(this string value)
    {
       return HttpUtility.UrlEncode(value);
    }
    
    public static string UrlDecoded(this string value)
    {
        return HttpUtility.UrlDecode(value);
    }

    
}