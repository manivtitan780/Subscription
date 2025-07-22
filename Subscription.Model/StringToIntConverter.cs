#region Header
// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.Model
// File Name:           JsonConver.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu, Brijesh Dubey
// Created On:          07-22-2025 21:07
// Last Updated On:     07-22-2025 21:02
// *****************************************/
#endregion

using System.Text.Json;

namespace Subscription.Model;

/// <summary>
/// Custom JsonConverter to handle string-to-integer conversion for System.Text.Json compatibility
/// Resolves migration issues where APIs return integers as strings (e.g., "513210" -> 513210)
/// </summary>
public class StringToIntConverter : System.Text.Json.Serialization.JsonConverter<int>
{
    public override int Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            // Handle string-to-int conversion (e.g., "513210" -> 513210)
            string stringValue = reader.GetString();
            if (int.TryParse(stringValue, out int result))
            {
                return result;
            }

            // Return 0 for invalid strings to maintain backward compatibility
            return 0;
        }

        if (reader.TokenType == JsonTokenType.Number)
        {
            // Handle normal integer values
            return reader.GetInt32();
        }

        // Handle null or other token types
        return 0;
    }

    public override void Write(Utf8JsonWriter writer, int value, JsonSerializerOptions options)
    {
        // Always write integers as numbers (not strings)
        writer.WriteNumberValue(value);
    }
}