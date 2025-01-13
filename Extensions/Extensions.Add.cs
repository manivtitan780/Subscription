#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Extensions
// File Name:           Extensions.Add.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu
// Created On:          02-07-2024 15:02
// Last Updated On:     10-29-2024 15:10
// *****************************************/

#endregion

#region Using

using System.Data;

using Microsoft.Data.SqlClient;

#endregion

namespace Extensions;

/// <summary>
/// </summary>
public static partial class Extensions
{
    /// <summary>
    ///     Appends an '@' character at the start of the given string if it does not already start with '@'.
    /// </summary>
    /// <param name="s">The string to which '@' should be appended.</param>
    /// <returns>The modified string starting with '@'.</returns>
    private static string AppendAtRateChar(this string s) => s.StartsWith('@') ? s : "@" + s;

    /// <summary>
    ///     Adds a SqlParameter of SqlDbType.Binary to the SqlCommand's Parameters collection.
    /// </summary>
    /// <param name="t">The SqlCommand to which the SqlParameter is added.</param>
    /// <param name="name">The name of the SqlParameter.</param>
    /// <param name="size">The size of the SqlParameter.</param>
    /// <param name="value">The value of the SqlParameter.</param>
    /// <param name="output">
    ///     Optional parameter that indicates whether the SqlParameter is an output parameter. Default is
    ///     false.
    /// </param>
    public static void Binary(this SqlCommand t, string name, int size, object value, bool output = false) => t.Parameters.Add(new(name.AppendAtRateChar(), SqlDbType.Binary, size)
                                                                                                                               {
                                                                                                                                   Value = value,
                                                                                                                                   Direction =
                                                                                                                                       output
                                                                                                                                           ? ParameterDirection
                                                                                                                                              .InputOutput
                                                                                                                                           : ParameterDirection
                                                                                                                                              .Input
                                                                                                                               });

    /// <summary>
    ///     Adds a SqlParameter of SqlDbType.Bit to the SqlCommand's Parameters collection.
    /// </summary>
    /// <param name="t">The SqlCommand to which the SqlParameter is added.</param>
    /// <param name="name">The name of the SqlParameter.</param>
    /// <param name="value">The value of the SqlParameter.</param>
    /// <param name="output">
    ///     Optional parameter that indicates whether the SqlParameter is an output parameter. Default is
    ///     false.
    /// </param>
    public static void Bit(this SqlCommand t, string name, object value, bool output = false) => t.Parameters.Add(new(name.AppendAtRateChar(), SqlDbType.Bit)
                                                                                                                  {
                                                                                                                      Value = value,
                                                                                                                      Direction =
                                                                                                                          output ? ParameterDirection
                                                                                                                             .InputOutput : ParameterDirection
                                                                                                                             .Input
                                                                                                                  });

    /// <summary>
    ///     Adds a SqlParameter of SqlDbType.Char or SqlDbType.NChar to the SqlCommand's Parameters collection.
    /// </summary>
    /// <param name="t">The SqlCommand to which the SqlParameter is added.</param>
    /// <param name="name">The name of the SqlParameter.</param>
    /// <param name="size">The size of the SqlParameter.</param>
    /// <param name="value">The value of the SqlParameter.</param>
    /// <param name="isNType">Optional parameter that indicates whether the SqlParameter is of type NChar. Default is true.</param>
    /// <param name="output">
    ///     Optional parameter that indicates whether the SqlParameter is an output parameter. Default is
    ///     false.
    /// </param>
    public static void Char(this SqlCommand t, string name, int size, object? value, bool isNType = true,
                            bool output = false) => t.Parameters.Add(new(name.AppendAtRateChar(), isNType ? SqlDbType.NChar : SqlDbType.Char, size)
                                                                     {
                                                                         Value = value,
                                                                         Direction = output ? ParameterDirection.InputOutput : ParameterDirection.Input
                                                                     });

    /// <summary>
    ///     Adds a SqlParameter of SqlDbType.Date to the SqlCommand's Parameters collection.
    /// </summary>
    /// <param name="t">The SqlCommand to which the SqlParameter is added.</param>
    /// <param name="name">The name of the SqlParameter.</param>
    /// <param name="value">The value of the SqlParameter.</param>
    /// <param name="output">
    ///     Optional parameter that indicates whether the SqlParameter is an output parameter. Default is
    ///     false.
    /// </param>
    public static void Date(this SqlCommand t, string name, object value, bool output = false) => t.Parameters.Add(new(name.AppendAtRateChar(), SqlDbType.Date)
                                                                                                                   {
                                                                                                                       Value = value,
                                                                                                                       Direction =
                                                                                                                           output ? ParameterDirection
                                                                                                                              .InputOutput : ParameterDirection
                                                                                                                              .Input
                                                                                                                   });

    /// <summary>
    ///     Adds a DateTime parameter to the SqlCommand.
    /// </summary>
    /// <param name="t">The SqlCommand to which the parameter will be added.</param>
    /// <param name="name">The name of the parameter.</param>
    /// <param name="value">The value of the parameter.</param>
    /// <param name="output">
    ///     Optional. If set to true, the parameter is set as InputOutput. Default is false, setting the
    ///     parameter as Input.
    /// </param>
    public static void DateTime(this SqlCommand t, string name, object value, bool output = false) => t.Parameters.Add(new(name.AppendAtRateChar(), SqlDbType.DateTime)
                                                                                                                       {
                                                                                                                           Value = value,
                                                                                                                           Direction =
                                                                                                                               output ? ParameterDirection
                                                                                                                                      .InputOutput
                                                                                                                                   : ParameterDirection.Input
                                                                                                                       });

    /// <summary>
    ///     Adds a new parameter to the SqlCommand of type Decimal.
    /// </summary>
    /// <param name="t">The SqlCommand to which the parameter will be added.</param>
    /// <param name="name">The name of the parameter.</param>
    /// <param name="precision">The maximum number of digits used to represent the decimal number.</param>
    /// <param name="scale">The number of digits to the right of the decimal point.</param>
    /// <param name="value">The value of the parameter.</param>
    /// <param name="output">Optional. Indicates whether the parameter is an output parameter. Default is false.</param>
    public static void Decimal(this SqlCommand t, string name, byte precision, byte scale, object value, bool output = false) => t.Parameters.Add(new(name.AppendAtRateChar(), SqlDbType.Decimal)
                                                                                                                                                  {
                                                                                                                                                      Value = value,
                                                                                                                                                      Direction =
                                                                                                                                                          output ? ParameterDirection.InputOutput
                                                                                                                                                              : ParameterDirection.Input,
                                                                                                                                                      Precision = precision,
                                                                                                                                                      Scale = scale
                                                                                                                                                  });

    /// <summary>
    ///     Adds an integer parameter to the specified SqlCommand.
    /// </summary>
    /// <param name="t">The SqlCommand to which the parameter will be added.</param>
    /// <param name="name">The name of the parameter.</param>
    /// <param name="value">The value of the parameter.</param>
    /// <param name="output">
    ///     Optional. If set to true, the parameter is set as InputOutput. Default is false, setting the
    ///     parameter as Input.
    /// </param>
    /// <returns>The SqlParameter that was added to the SqlCommand.</returns>
    public static void Int(this SqlCommand t, string name, object value, bool output = false) => t.Parameters.Add(new(name.AppendAtRateChar(), SqlDbType.Int)
                                                                                                                  {
                                                                                                                      Value = value,
                                                                                                                      Direction =
                                                                                                                          output ? ParameterDirection
                                                                                                                                 .InputOutput
                                                                                                                              : ParameterDirection.Input
                                                                                                                  });

    /// <summary>
    ///     Adds a parameter to the SqlCommand of type SmallDateTime.
    /// </summary>
    /// <param name="t">The SqlCommand to which the parameter will be added.</param>
    /// <param name="name">The name of the parameter.</param>
    /// <param name="value">The value of the parameter.</param>
    /// <param name="output">Optional parameter indicating whether the parameter is an output parameter. Default is false.</param>
    public static void SmallDateTime(this SqlCommand t, string name, object value, bool output = false) => t.Parameters.Add(new(name.AppendAtRateChar(), SqlDbType.SmallDateTime)
                                                                                                                            {
                                                                                                                                Value = value,
                                                                                                                                Direction =
                                                                                                                                    output ? ParameterDirection
                                                                                                                                           .InputOutput
                                                                                                                                        : ParameterDirection
                                                                                                                                           .Input
                                                                                                                            });

    /// <summary>
    ///     Adds a new parameter to the SqlCommand with SqlDbType of SmallInt.
    /// </summary>
    /// <param name="t">The SqlCommand to which the parameter is added.</param>
    /// <param name="name">The name of the parameter.</param>
    /// <param name="value">The value of the parameter.</param>
    /// <param name="output">Optional parameter that indicates whether the parameter is an output parameter. Default is false.</param>
    public static void SmallInt(this SqlCommand t, string name, object value, bool output = false) => t.Parameters.Add(new(name.AppendAtRateChar(), SqlDbType.SmallInt)
                                                                                                                       {
                                                                                                                           Value = value,
                                                                                                                           Direction =
                                                                                                                               output ? ParameterDirection
                                                                                                                                      .InputOutput
                                                                                                                                   : ParameterDirection.Input
                                                                                                                       });

    /// <summary>
    ///     Adds a new SqlParameter of SqlDbType.TinyInt to the SqlCommand's Parameters collection.
    /// </summary>
    /// <param name="t">The SqlCommand to which the SqlParameter is added.</param>
    /// <param name="name">The name of the SqlParameter.</param>
    /// <param name="value">The value of the SqlParameter.</param>
    /// <param name="output">
    ///     Optional. If set to true, the SqlParameter is input/output. Otherwise, it is input only. Default
    ///     value is false.
    /// </param>
    public static void TinyInt(this SqlCommand t, string name, object value, bool output = false) => t.Parameters.Add(new(name.AppendAtRateChar(), SqlDbType.TinyInt)
                                                                                                                      {
                                                                                                                          Value = value,
                                                                                                                          Direction =
                                                                                                                              output ? ParameterDirection
                                                                                                                                     .InputOutput
                                                                                                                                  : ParameterDirection.Input
                                                                                                                      });

    /// <summary>
    ///     Adds a parameter to the SqlCommand object with SqlDbType of UniqueIdentifier.
    /// </summary>
    /// <param name="t">The SqlCommand object to which the parameter is added.</param>
    /// <param name="name">The name of the parameter.</param>
    /// <param name="value">The value of the parameter.</param>
    /// <param name="output">Optional parameter that indicates whether the parameter is an output parameter. Default is false.</param>
    public static void UniqueIdentifier(this SqlCommand t, string name, object value, bool output = false) => t.Parameters.Add(new(name.AppendAtRateChar(), SqlDbType.UniqueIdentifier)
                                                                                                                               {
                                                                                                                                   Value = value,
                                                                                                                                   Direction = output ? ParameterDirection.InputOutput
                                                                                                                                                   : ParameterDirection.Input
                                                                                                                               });

    /// <summary>
    ///     Adds a new parameter to the SqlCommand with SqlDbType of VarChar or NVarChar, depending on the isNType flag.
    /// </summary>
    /// <param name="t">The SqlCommand to which the parameter is added.</param>
    /// <param name="name">The name of the parameter.</param>
    /// <param name="size">The maximum size, in characters, of the data within the column.</param>
    /// <param name="value">The value of the parameter.</param>
    /// <param name="isNType">A flag indicating whether the SqlDbType should be NVarChar. If false, SqlDbType will be VarChar.</param>
    /// <param name="output">A flag indicating whether the parameter is an output parameter.</param>
    public static void Varchar(this SqlCommand t, string name, int size, object? value, bool isNType = true, bool output = false) =>
        t.Parameters.Add(new(name.AppendAtRateChar(), isNType ? SqlDbType.NVarChar : SqlDbType.VarChar, size)
                         {
                             Value = value.NullOrWhiteSpace() ? "" : value, Direction = output ? ParameterDirection.InputOutput : ParameterDirection.Input
                         });

    /// <summary>
    ///     Adds a new parameter to the SqlCommand with a SqlDbType of VarChar. The value of the parameter is set to DBNull.
    /// </summary>
    /// <param name="t">The SqlCommand to which the parameter is added.</param>
    /// <param name="name">The name of the parameter.</param>
    /// <param name="size">
    ///     The maximum size, in characters, of the data within the column. The default value is inferred from
    ///     the parameter value.
    /// </param>
    /// <param name="isNType">
    ///     A boolean value that is true if the SQL Server data type is of NVarChar type; otherwise, false.
    ///     The default is true.
    /// </param>
    /// <param name="output">
    ///     A boolean value that is true if the parameter is an output parameter; otherwise, false. The default
    ///     is false.
    /// </param>
    public static void VarcharD(this SqlCommand t, string name, int size, bool isNType = true, bool output = false) =>
        t.Parameters.Add(new(name.AppendAtRateChar(), isNType ? SqlDbType.NVarChar : SqlDbType.VarChar, size)
                         {
                             Value = System.DBNull.Value, Direction = output ? ParameterDirection.InputOutput : ParameterDirection.Input
                         });

    /// <summary>
    ///     Adds a new SqlParameter of SqlDbType.Xml to the SqlCommand's Parameters collection.
    /// </summary>
    /// <param name="t">The SqlCommand to which the SqlParameter is added.</param>
    /// <param name="name">The name of the SqlParameter.</param>
    /// <param name="value">The value of the SqlParameter.</param>
    /// <returns>The SqlParameter that was added to the SqlCommand's Parameters collection.</returns>
    public static void Xml(this SqlCommand t, string name, object value) => t.Parameters.Add(new(name.AppendAtRateChar(), SqlDbType.Xml)
                                                                                             {
                                                                                                 Value = value
                                                                                             });
}