using System.Web;

namespace Imagegram.Api.Extensions;

public static class StringExtensions
{
    public static string UrlEncoded(this string value) => HttpUtility.UrlEncode(value);

    public static string UrlDecoded(this string value) => HttpUtility.UrlDecode(value);
}