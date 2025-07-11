#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Extensions
// File Name:           Extensions.To.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu
// Created On:          02-07-2024 15:02
// Last Updated On:     01-15-2025 15:01
// *****************************************/

#endregion

#region Using

using System.Diagnostics.CodeAnalysis;
using System.Globalization;

using Microsoft.AspNetCore.Components;
using Microsoft.Data.SqlClient;

#endregion

namespace Extensions;

[SuppressMessage("ReSharper", "MemberCanBePrivate.Global"), SuppressMessage("ReSharper", "UnusedMember.Global")]
public static partial class Extensions
{
    public static async IAsyncEnumerable<T> FillList<T>(this SqlDataReader reader, Func<SqlDataReader, T> projection)
    {
        //List<T> _results = [];
        while (await reader.ReadAsync())
        {
            yield return projection(reader);
        }

        //return _results;
    }

	/// <summary>
	///     Returns a boolean value from the SqlDataReader based on the provided index.
	///     If the value is DBNull, the method will return the provided nullReplaceValue.
	/// </summary>
	/// <param name="read">The SqlDataReader instance.</param>
	/// <param name="index">The zero-based column ordinal.</param>
	/// <param name="nullReplaceValue">The value to return if the column value is DBNull. Default is false.</param>
	/// <returns>The boolean value of the specified column or nullReplaceValue if the column value is DBNull.</returns>
	public static bool NBoolean(this SqlDataReader read, int index, bool nullReplaceValue = false) => read.IsDBNull(index) ? nullReplaceValue : read.GetBoolean(index);

	/// <summary>
	///     Returns a DateTime value from the SqlDataReader based on the provided index.
	///     If the value is DBNull, the method will return the provided nullReplaceValue.
	/// </summary>
	/// <param name="read">The SqlDataReader instance.</param>
	/// <param name="index">The zero-based column ordinal.</param>
	/// <param name="nullReplaceValue">The value to return if the column value is DBNull. Default is DateTime default value.</param>
	/// <returns>The DateTime value of the specified column or nullReplaceValue if the column value is DBNull.</returns>
	public static DateTime NDateTime(this SqlDataReader read, int index, DateTime nullReplaceValue = default) => read.IsDBNull(index) ? nullReplaceValue : read.GetDateTime(index);

	/// <summary>
	///     Returns a decimal value from the SqlDataReader based on the provided index.
	///     If the value is DBNull, the method will return the provided nullReplaceValue.
	/// </summary>
	/// <param name="read">The SqlDataReader instance.</param>
	/// <param name="index">The zero-based column ordinal.</param>
	/// <param name="nullReplaceValue">The value to return if the column value is DBNull. Default is 0.</param>
	/// <returns>The decimal value of the specified column or nullReplaceValue if the column value is DBNull.</returns>
	public static decimal NDecimal(this SqlDataReader read, int index, decimal nullReplaceValue = 0) => read.IsDBNull(index) ? nullReplaceValue : read.GetDecimal(index);

	/// <summary>
	///     Returns a short integer value from the SqlDataReader based on the provided index.
	///     If the value is DBNull, the method will return the provided nullReplaceValue.
	/// </summary>
	/// <param name="read">The SqlDataReader instance.</param>
	/// <param name="index">The zero-based column ordinal.</param>
	/// <param name="nullReplaceValue">The value to return if the column value is DBNull. Default is 0.</param>
	/// <returns>The short integer value of the specified column or nullReplaceValue if the column value is DBNull.</returns>
	public static short NInt16(this SqlDataReader read, int index, short nullReplaceValue = 0) => read.IsDBNull(index) ? nullReplaceValue : read.GetInt16(index);

	/// <summary>
	///     Returns an integer value from the SqlDataReader based on the provided index.
	///     If the value is DBNull, the method will return the provided nullReplaceValue.
	/// </summary>
	/// <param name="read">The SqlDataReader instance.</param>
	/// <param name="index">The zero-based column ordinal.</param>
	/// <param name="nullReplaceValue">The value to return if the column value is DBNull. Default is 0.</param>
	/// <returns>The integer value of the specified column or nullReplaceValue if the column value is DBNull.</returns>
	public static int NInt32(this SqlDataReader read, int index, int nullReplaceValue = 0) => read.IsDBNull(index) ? nullReplaceValue : read.GetInt32(index);

	/// <summary>
	///     Returns a string value from the SqlDataReader based on the provided index.
	///     If the value is DBNull or an empty string (if checkEmptyString is true), the method will return the provided
	///     nullReplaceValue.
	/// </summary>
	/// <param name="read">The SqlDataReader instance.</param>
	/// <param name="index">The zero-based column ordinal.</param>
	/// <param name="nullReplaceValue">
	///     The value to return if the column value is DBNull or an empty string. Default is an
	///     empty string.
	/// </param>
	/// <param name="checkEmptyString">A boolean value indicating whether to check for empty strings. Default is false.</param>
	/// <returns>
	///     The string value of the specified column or nullReplaceValue if the column value is DBNull or an empty string
	///     (if checkEmptyString is true).
	/// </returns>
	public static string NString(this SqlDataReader read, int index, string nullReplaceValue = "", bool checkEmptyString = false) =>
        checkEmptyString ? read.IsDBNull(index) || read.GetString(index) == "" ? nullReplaceValue : read.GetString(index).Trim() :
        read.IsDBNull(index) ? nullReplaceValue : read.GetString(index).Trim();

	/// <summary>
	///     Converts the object representation of a boolean value to its boolean equivalent.
	/// </summary>
	/// <param name="o">An object containing the value to convert.</param>
	/// <returns>
	///     A boolean value that is equivalent to the boolean value contained in the object. If the object is null, or
	///     does not represent a valid boolean value, the method returns false.
	/// </returns>
	public static bool ToBoolean(this object? o)
	{
		return o switch
			   {
				   null => false,
				   bool _b => _b,
				   _ => bool.TryParse(o.ToString(), out bool _outDate) && _outDate
			   };
	}

	public static string ToBooleanString(this bool b, string trueString = "true", string falseString = "false") => b ? trueString : falseString;

	/// <summary>
	///     Converts the object representation of a byte value to its byte equivalent.
	///     If the object is null or cannot be converted, the method will return the provided nullValue.
	/// </summary>
	/// <param name="o">The object to convert.</param>
	/// <param name="nullValue">The value to return if the object is null or cannot be converted. Default is 0.</param>
	/// <returns>
	///     The byte representation of the object or nullValue if the object is null or cannot be converted.
	/// </returns>
	public static byte ToByte(this object? o, byte nullValue = 0) => o != null && byte.TryParse(o.ToString(), NumberStyles.Number, CultureInfo.CurrentCulture.NumberFormat,
																								out byte _out) ? _out : nullValue;

	/// <summary>
	///     Converts the object representation of a DateTime value to its DateTime equivalent.
	/// </summary>
	/// <param name="o">An object containing the value to convert.</param>
	/// <returns>
	///     A DateTime value that is equivalent to the DateTime value contained in the object. If the object is null, or
	///     does not represent a valid DateTime value, the method returns DateTime.MinValue.
	/// </returns>
	public static DateTime ToDateTime(this object? o)
    {
        return o switch
               {
                   DateTime _time => _time,
                   null => System.DateTime.MinValue,
                   _ => System.DateTime.TryParse(o.ToString(), out DateTime _outDate) ? _outDate : System.DateTime.MinValue
               };
    }

	/// <summary>
	///     Converts the string representation of a date and time to its DateTime equivalent.
	///     If the string is null or empty, or if the conversion fails, it returns DateTime.MinValue.
	/// </summary>
	/// <param name="s">A string containing a date and time to convert.</param>
	/// <param name="format">
	///     A string that defines the format of the date and time in the input string. The default format is
	///     "mm/dd/yyyy".
	/// </param>
	/// <returns>
	///     The DateTime equivalent of the date and time contained in s, if the conversion succeeded, or DateTime.MinValue
	///     if the string is null or empty, or the conversion failed.
	/// </returns>
	public static DateTime ToDateTime(this string s, string format = "MM/dd/yyyy")
    {
        if (s.NullOrWhiteSpace())
        {
            return System.DateTime.MinValue;
        }

        return System.DateTime.TryParseExact(s, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime _outDate) ? _outDate : System.DateTime.MinValue;
    }

	/// <summary>
	///     Converts the object representation of a value to its decimal equivalent.
	/// </summary>
	/// <param name="o">An object containing the value to convert.</param>
	/// <param name="nullValue">The value to return if the object is null. Default is 0.</param>
	/// <returns>
	///     A decimal value that is equivalent to the value contained in the object. If the object is null, or does not
	///     represent a valid decimal value, the method returns the nullValue.
	/// </returns>
	public static decimal ToDecimal(this object? o, decimal nullValue = 0) => o != null && decimal.TryParse(o.ToString(), NumberStyles.Number, CultureInfo.CurrentCulture.NumberFormat,
																							  out decimal _out) ? _out : nullValue;

	/// <summary>
	///     Converts the object representation of a value to its double equivalent.
	/// </summary>
	/// <param name="o">An object containing the value to convert.</param>
	/// <param name="nullValue">The value to return if the object is null. Default is 0.</param>
	/// <returns>
	///     A double value that is equivalent to the value contained in the object. If the object is null, or does not
	///     represent a valid double value, the method returns the nullValue.
	/// </returns>
	public static double ToDouble(this object? o, double nullValue = 0) => o != null && double.TryParse(o.ToString(), NumberStyles.Number, CultureInfo.CurrentCulture.NumberFormat,
																						  out double _out) ? _out : nullValue;

	/// <summary>
	///     Converts the string representation of a short integer value to its short integer equivalent.
	/// </summary>
	/// <param name="o">A string containing the value to convert.</param>
	/// <param name="nullValue">
	///     The value to return if the string is null or empty, or does not represent a valid short integer
	///     value. Default is 0.
	/// </param>
	/// <returns>
	///     A short integer value that is equivalent to the short integer value contained in the string. If the string is
	///     null or empty, or does not represent a valid short integer value, the method returns the nullValue.
	/// </returns>
	public static short ToInt16(this object? o, short nullValue = 0) => o != null && short.TryParse(o.ToString(), NumberStyles.Number, CultureInfo.CurrentCulture.NumberFormat,
																					  out short _out) ? _out : nullValue;

	/// <summary>
	///     Converts the object representation of a number to its 32-bit signed integer equivalent.
	/// </summary>
	/// <param name="o">An object containing the number to convert.</param>
	/// <param name="nullValue">The value to return if the object is null or does not represent a valid number. Default is 0.</param>
	/// <returns>
	///     A 32-bit signed integer that is equivalent to the number contained in the object. If the object is null or
	///     does not represent a valid number, the method returns nullValue.
	/// </returns>
	public static int ToInt32(this object? o, int nullValue = 0) => o != null && int.TryParse(o.ToString(), NumberStyles.Number, CultureInfo.CurrentCulture.NumberFormat,
																				out int _out) ? _out : nullValue;

	/// <summary>
	///     Converts the given object to a long integer (Int64).
	///     If the object is null or cannot be converted, the method will return the provided nullValue.
	/// </summary>
	/// <param name="o">The object to convert.</param>
	/// <param name="nullValue">The value to return if the object is null or cannot be converted. Default is 0.</param>
	/// <returns>
	///     The long integer (Int64) representation of the object or nullValue if the object is null or cannot be
	///     converted.
	/// </returns>
	public static long ToInt64(this object? o, int nullValue = 0)
	{
		if (o is null)
		{
			return nullValue;
		}
		return long.TryParse(o.ToString(), NumberStyles.Number, CultureInfo.CurrentCulture.NumberFormat,
							 out long _out) ? _out : nullValue;
	}

	/// <summary>
	///     Converts the given string to a MarkupString.
	/// </summary>
	/// <param name="s">The string to be converted.</param>
	/// <returns>
	///     A MarkupString where new line characters are replaced with HTML line break tags. If the input string is null,
	///     empty, or consists only of white-space characters, an empty MarkupString is returned.
	/// </returns>
	public static MarkupString ToMarkupString(this string? s)
    {
        if (s.NullOrWhiteSpace())
        {
            return (MarkupString)"";
        }

        s = s?.Replace(Environment.NewLine, "<br/>");

        return (MarkupString)s!;
    }
}