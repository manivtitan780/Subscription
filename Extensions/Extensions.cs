#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Extensions
// File Name:           Extensions.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu, Brijesh Dubey
// Created On:          02-07-2024 15:02
// Last Updated On:     01-28-2025 19:01
// *****************************************/

#endregion

using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO.Compression;
using System.Net.Mail;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace Extensions;

[SuppressMessage("ReSharper", "UnusedMember.Global"), SuppressMessage("ReSharper", "UnusedParameter.Global")]
public static partial class Extensions
{
    /// <summary>
    ///     Compresses the input string using GZip compression.
    /// </summary>
    /// <param name="s">
    ///     The string to be compressed. If the string is null or empty, a zero-length byte array is returned.
    /// </param>
    /// <returns>
    ///     A byte array containing the compressed data, or a zero-length byte array if the input string is null or empty.
    /// </returns>
    /// <example>
    ///     string originalString = "Hello, world!";
    ///     byte[] compressedData = originalString.CompressGZip();
    /// </example>
    public static byte[] CompressGZip(this string s)
    {
        if (s.NullOrWhiteSpace())
        {
            // Return a zero-length byte array instead of throwing an exception
            return [];
        }

        byte[] _byteArray = Encoding.UTF8.GetBytes(s);

        using MemoryStream _memStream = new();
        using GZipStream _gZipStream = new(_memStream, CompressionMode.Compress);

        _gZipStream.Write(_byteArray, 0, _byteArray.Length);
        _gZipStream.Close();

        return _memStream.ToArray();
    }

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
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string CultureCurrency(this decimal d, string format = "c2", CultureInfo? c = null)
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
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string CultureDate(this DateTime d, string format = "d", CultureInfo? c = null)
    {
        c ??= new("en-us");

        return d.ToString(format, c);
    }

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
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string CulturePercentage(this decimal d, string format = "p2", CultureInfo? c = null)
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
    public static object DBNull(this double? i) => i is null or 0 ? System.DBNull.Value : i;

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
    ///     Decompresses a GZip compressed byte array to a string.
    /// </summary>
    /// <param name="byteString">
    ///     The byte array to be decompressed.
    /// </param>
    /// <returns>
    ///     A string representation of the decompressed byte array.
    /// </returns>
    public static string DecompressGZip(this byte[] byteString)
    {
        using MemoryStream _memStreamReader = new(byteString);
        using GZipStream _gZipStream = new(_memStreamReader, CompressionMode.Decompress);
        using MemoryStream _memStream = new();

        // Copy the decompressed data to the MemoryStream
        try
        {
            _gZipStream.CopyTo(_memStream);

            // Convert the decompressed byte array to a string and return it
            return Encoding.UTF8.GetString(_memStream.ToArray());
        }
        catch
        {
            return string.Empty;
        }
    }

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
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string StripAndFormatPhoneNumber(this string s) => s.StripPhoneNumber().FormatPhoneNumber();

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

    /// <summary>
    ///     Determines if the provided string is a valid email address.
    /// </summary>
    /// <param name="email">The string to be validated as an email address.</param>
    /// <returns>
    ///     True if the string is a valid email address; otherwise, false.
    /// </returns>
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

    /// <summary>
    ///     Determines if a given string is a valid URL.
    /// </summary>
    /// <param name="url">The string to be checked.</param>
    /// <returns>
    ///     True if the string is a valid URL; otherwise, false.
    /// </returns>
    public static bool IsValidUrl(this string url)
    {
        string _url = url.ToLowerInvariant();

        // Check if the string is null, empty, or consists only of white-space characters
        if (_url.NullOrWhiteSpace())
        {
            return false;
        }

        // Check if the string does not start with the HTTP/HTTPS protocols
        if (!_url.StartsWith("http://") || !_url.StartsWith("https://"))
        {
            // Add the HTTPS protocol to the string
            _url = $"https://{_url}";
        }

        // Attempt to create a Uri object from the string
        // Check if the Uri object's Scheme property is HTTP or HTTPS
        return Uri.TryCreate(_url, UriKind.Absolute, out Uri? _result) && (_result.Scheme == Uri.UriSchemeHttp || _result.Scheme == Uri.UriSchemeHttps);
    }

    [GeneratedRegex("[^0-9]")]
    private static partial Regex MyRegex();

    /// <summary>
    ///     Checks if the given string is NOT null, empty, or consists only of white-space characters.
    /// </summary>
    /// <param name="s">The string to be checked.</param>
    /// <returns>
    ///     True if the string contains characters other than null, empty, or consists only of white-space characters;
    ///     otherwise, false.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool NotNullOrWhiteSpace(this string? s) => !string.IsNullOrWhiteSpace(s);

    /// <summary>
    ///     Checks if the given string is null, empty, or consists only of white-space characters.
    /// </summary>
    /// <param name="s">The string to be checked.</param>
    /// <returns>
    ///     True if the string is null, empty, or consists only of white-space characters; otherwise, false.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool NullOrWhiteSpace(this string? s) => string.IsNullOrWhiteSpace(s);

    /// <summary>
    ///     Checks if the given integer is zero.
    /// </summary>
    /// <param name="s">The integer to be checked.</param>
    /// <returns>
    ///     True if the integer is zero; otherwise, false.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool NullOrWhiteSpace(this object? o) => o == null || o.ToString().NullOrWhiteSpace();

    /// <summary>
    ///     Generates a random integer number within a specified range.
    /// </summary>
    /// <param name="p">
    ///     The object this extension method applies to.
    /// </param>
    /// <param name="negative">
    ///     A boolean value indicating whether the generated number can be negative.
    ///     If <c>true</c>, the method generates a number between -5000 and 5000.
    ///     If <c>false</c>, the method generates a number between 0 and 5000.
    /// </param>
    /// <param name="lowerBound">
    ///     The lower bound of the range from which to generate the number. Defaults to 0.
    /// </param>
    /// <param name="negativeLowerBound">
    ///     The lower bound of the range from which to generate the number if <paramref name="negative" />
    ///     is <c>true</c>. Defaults to -5000.
    /// </param>
    /// <param name="upperBound">
    ///     The upper bound of the range from which to generate the number. Defaults to 5000.
    /// </param>
    /// <returns>
    ///     A random integer number within the specified range.
    /// </returns>
    public static int RandomNumber(this object p, bool negative = true, int lowerBound = 0, int negativeLowerBound = -5000,
                                   int upperBound = 5000) => !negative ? RandomNumberGenerator.GetInt32(lowerBound, upperBound) : RandomNumberGenerator.GetInt32(negativeLowerBound, upperBound);

    /// <summary>
    ///     Removes the leading comma from the given string.
    /// </summary>
    /// <param name="s">The string from which the leading comma should be removed.</param>
    /// <returns>
    ///     A string without the leading comma. If the string does not start with a comma, the original string is returned.
    /// </returns>
    public static string RemoveLeadingComma(this string s) => s.StartsWith(',') ? s[1..].Trim() : s;

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
    public static string StripPhoneNumber(this string s) => MyRegex().Replace(s, string.Empty);

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