#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Extensions
// File Name:           Extensions.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu
// Created On:          02-07-2024 15:02
// Last Updated On:     12-30-2024 20:12
// *****************************************/

#endregion

#region Using

using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

//using ProfSvc_AppTrack.Code;

#endregion

namespace Extensions;

[SuppressMessage("ReSharper", "UnusedMember.Global"), SuppressMessage("ReSharper", "UnusedParameter.Global")]
public static partial class Extensions
{
    /// <summary>
    ///     Formats the given decimal value to a currency string representation using the specified format and culture.
    ///     If no culture is provided, it defaults to US culture.
    /// </summary>
    /// <param name="d">The decimal value to be formatted.</param>
    /// <param name="format">
    ///     The format string to use for formatting the currency. Defaults to "c2" for currency format with
    ///     two decimal places.
    /// </param>
    /// <param name="c">
    ///     The CultureInfo object representing the culture to use for formatting. Defaults to null, in which case
    ///     US culture is used.
    /// </param>
    /// <returns>
    ///     A string representation of the decimal value formatted as currency according to the specified format and
    ///     culture.
    /// </returns>
    public static string CultureCurrency(this decimal d, string format = "c2", CultureInfo c = null)
    {
        c ??= new("en-us");

        return d.ToString(format, c);
    }

    /// <summary>
    ///     Formats the given DateTime object to a string representation using the specified format and culture.
    ///     If no culture is provided, it defaults to US culture.
    /// </summary>
    /// <param name="d">The DateTime object to be formatted.</param>
    /// <param name="format">The format string to use for formatting the date. Defaults to "d" for short date pattern.</param>
    /// <param name="c">
    ///     The CultureInfo object representing the culture to use for formatting. Defaults to null, in which case
    ///     US culture is used.
    /// </param>
    /// <returns>A string representation of the DateTime object formatted according to the specified format and culture.</returns>
    public static string CultureDate(this DateTime d, string format = "d", CultureInfo c = null)
    {
        c ??= new("en-us");

        return d.ToString(format, c);
    }

    /// <summary>
    ///     Converts a nullable double value to a DBNull value if it is null or zero.
    /// </summary>
    /// <param name="i">The nullable double value to be checked.</param>
    /// <returns>
    ///     A DBNull value if the input is null or zero; otherwise, the input value.
    /// </returns>
    public static object DBNull(this double? i) => i == null || i == 0 ? System.DBNull.Value : i;

    /// <summary>
    ///     Converts an integer value to a DBNull value if it is zero.
    /// </summary>
    /// <param name="i">The integer value to be checked.</param>
    /// <returns>A DBNull value if the input is zero; otherwise, the input value.</returns>
    public static object DBNull(this int i) => i == 0 ? System.DBNull.Value : i;

    /// <summary>
    ///     Converts a string value to a DBNull value if it is null, whitespace, or optionally, if it is "0".
    /// </summary>
    /// <param name="s">The string value to be checked.</param>
    /// <param name="isZero">
    ///     A boolean value indicating whether to treat the string "0" as a DBNull. Defaults to false, in which case
    ///     "0" is not treated as a DBNull.
    /// </param>
    /// <returns>
    ///     A DBNull value if the input is null, whitespace, or "0" (if isZero is true); otherwise, the trimmed input value.
    /// </returns>
    public static object DBNull(this string s, bool isZero = false) => isZero ? string.IsNullOrWhiteSpace(s) || s.Trim() == "0" ? System.DBNull.Value : s.Trim() :
                                                                       string.IsNullOrWhiteSpace(s) ? System.DBNull.Value : s.Trim();

    /// <summary>
    ///     Formats the given string to a phone number format.
    /// </summary>
    /// <param name="s">The string to be formatted.</param>
    /// <returns>
    ///     A string representation of the phone number in the format (###) ###-####.
    ///     If the input string cannot be converted to a long or is less than or equal to zero, an empty string is returned.
    /// </returns>
    /// <remarks>
    ///     The method uses the ToInt64 extension method to convert the input string to a long.
    ///     The resulting long is then formatted to a phone number format.
    /// </remarks>
    public static string FormatPhoneNumber(this string s) => s.ToInt64() > 0 ? $"{s.ToInt64():(###) ###-####}" : "";

    /// <summary>
    ///     Converts a Base64 encoded string to a regular string.
    /// </summary>
    /// <param name="s">The Base64 encoded string to be converted.</param>
    /// <returns>
    ///     A string that represents the original text before the Base64 encoding.
    /// </returns>
    public static string FromBase64String(this string s)
    {
        byte[] _bytes = Convert.FromBase64String(s);
        return Encoding.UTF8.GetString(_bytes);
    }

    /// <summary>
    ///     Decodes a string that has been HTML-encoded for HTTP transmission.
    /// </summary>
    /// <param name="s">The HTML-encoded string to decode.</param>
    /// <returns>
    ///     A decoded string.
    /// </returns>
    public static string HtmlDecode(this string s) => HttpUtility.HtmlDecode(s);

    /// <summary>
    ///     Encodes a string to be displayed in a browser without being interpreted as HTML.
    /// </summary>
    /// <param name="s">The string to encode.</param>
    /// <returns>
    ///     An HTML-encoded string.
    /// </returns>
    public static string HtmlEncode(this string s) => HttpUtility.HtmlEncode(s);

    public static bool IsValidEmail(this string email)
    {
        try
        {
            MailAddress _address = new(email);
            return _address.Address == email;
        }
        catch
        {
            return false;
        }
    }

    public static bool IsValidUrl(this string url)
    {
        string _url = url.ToLowerInvariant();
        if (_url.NullOrWhiteSpace())
        {
            return false;
        }

        if (!_url.StartsWith("http://") || !_url.StartsWith("https://"))
        {
            _url = $"https://{_url}";
        }

        return Uri.TryCreate(_url, UriKind.Absolute, out Uri _result) && (_result.Scheme == Uri.UriSchemeHttp || _result.Scheme == Uri.UriSchemeHttps);
    }

    /// <summary>
    ///     Checks if the given string is NOT null, empty, or consists only of white-space characters.
    /// </summary>
    /// <param name="s">The string to be checked.</param>
    /// <returns>
    ///     True if the string contains characters other than null, empty, or consists only of white-space characters;
    ///     otherwise, false.
    /// </returns>
    public static bool NotNullOrWhiteSpace(this string s) => !string.IsNullOrWhiteSpace(s);

    /// <summary>
    ///     Checks if the given string is null, empty, or consists only of white-space characters.
    /// </summary>
    /// <param name="s">The string to be checked.</param>
    /// <returns>
    ///     True if the string is null, empty, or consists only of white-space characters; otherwise, false.
    /// </returns>
    public static bool NullOrWhiteSpace(this string s) => string.IsNullOrWhiteSpace(s);

    /// <summary>
    ///     Checks if the given integer is zero.
    /// </summary>
    /// <param name="s">The integer to be checked.</param>
    /// <returns>
    ///     True if the integer is zero; otherwise, false.
    /// </returns>
    public static bool NullOrWhiteSpace(this int s) => s.Equals(0);

    /// <summary>
    ///     Checks if the given object is null or its string representation is null, empty, or consists only of white-space
    ///     characters.
    /// </summary>
    /// <param name="o">The object to be checked.</param>
    /// <returns>
    ///     True if the object is null or its string representation is null, empty, or consists only of white-space characters;
    ///     otherwise, false.
    /// </returns>
    public static bool NullOrWhiteSpace(this object o) => o == null || o.ToString().NullOrWhiteSpace();

    /// <summary>
    ///     Generates a random integer number. The range of the generated number can be controlled by the 'negative' parameter.
    /// </summary>
    /// <param name="p">The object this extension method applies to.</param>
    /// <param name="negative">
    ///     A boolean value indicating whether the generated number can be negative.
    ///     If set to true, the method generates a number between -5000 and 5000.
    ///     If set to false, the method generates a number between 0 and 5000.
    /// </param>
    /// <returns>
    ///     A random integer number. The range of the number depends on the 'negative' parameter.
    /// </returns>
    public static int RandomNumber(this object p, bool negative = true) => !negative ? RandomNumberGenerator.GetInt32(0, 5000) : RandomNumberGenerator.GetInt32(-5000, 5000);

    /// <summary>
    ///     Removes the leading comma from the given string.
    /// </summary>
    /// <param name="s">The string from which the leading comma should be removed.</param>
    /// <returns>
    ///     A string without the leading comma. If the string does not start with a comma, the original string is returned.
    /// </returns>
    public static string RemoveLeadingComma(this string s) => s.StartsWith(",") ? s[1..].Trim() : s;

    /// <summary>
    ///     Converts a string to a boolean value.
    /// </summary>
    /// <param name="s">The string to be converted.</param>
    /// <returns>
    ///     True if the string is "1"; otherwise, false.
    /// </returns>
    public static bool StringToBoolean(this string s) => s == "1";

    /// <summary>
    ///     Strips non-numeric characters from a phone number.
    /// </summary>
    /// <param name="s">The phone number string to be processed.</param>
    /// <returns>
    ///     A string containing only the numeric characters from the input string.
    /// </returns>
    public static string StripPhoneNumber(this string s) => Regex.Replace(s, "[^0-9]", string.Empty);

    /// <summary>
    ///     Converts a regular string to a Base64 encoded string.
    /// </summary>
    /// <param name="s">The string to be converted.</param>
    /// <returns>
    ///     A Base64 encoded string that represents the original text.
    /// </returns>
    public static string ToBase64String(this string s)
    {
        byte[] _bytes = Encoding.UTF8.GetBytes(s);
        return Convert.ToBase64String(_bytes);
    }

    /// <summary>
    ///     Converts the given Stream object to a byte array.
    /// </summary>
    /// <param name="s">The Stream object to be converted.</param>
    /// <returns>
    ///     A byte array that represents the content of the Stream object.
    /// </returns>
    /// <remarks>
    ///     The method creates a new MemoryStream, copies the content of the input Stream into it,
    ///     and then converts the MemoryStream to a byte array.
    /// </remarks>
    public static byte[] ToStreamByteArray(this Stream s)
    {
        using MemoryStream _memoryStream = new();
        s.CopyTo(_memoryStream);
        return _memoryStream.ToArray();
    }

    /// <summary>
    ///     Decodes a URL-encoded string.
    /// </summary>
    /// <param name="s">The URL-encoded string to decode.</param>
    /// <returns>
    ///     A decoded string.
    /// </returns>
    public static string UrlDecode(this string s) => HttpUtility.UrlDecode(s);

    /// <summary>
    ///     Encodes a URL string.
    /// </summary>
    /// <param name="s">The string to be encoded.</param>
    /// <returns>
    ///     A URL-encoded string.
    /// </returns>
    public static string UrlEncode(this string s) => HttpUtility.UrlEncode(s);
}