#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.Model
// File Name:           ValidationPatterns.cs
// Created By:          Claude Code (Anthropic)
// Created On:          07-12-2025
// Last Updated On:     07-12-2025
// *****************************************/

#endregion

#region Using

using System.Text.RegularExpressions;

#endregion

namespace Subscription.Model.Constants;

/// <summary>
///     Provides standardized regular expression patterns and associated error messages
///     used across FluentValidation validators for consistent data format validation.
/// </summary>
/// <remarks>
///     This class centralizes regex patterns found across multiple validators to ensure
///     consistency and eliminate magic string duplication. All patterns are compile-time
///     constants with zero runtime performance impact.
/// </remarks>
public static class ValidationPatterns
{
    #region Phone Number Validation

    /// <summary>
    ///     Regular expression pattern for US phone number validation (10 digits only).
    ///     Matches exactly 10 consecutive digits with no formatting.
    /// </summary>
    /// <example>
    ///     Valid: "1234567890"
    ///     Invalid: "(123) 456-7890", "123-456-7890", "123.456.7890"
    /// </example>
    public const string PhoneNumber = @"^\d{10}$";

    /// <summary>
    ///     Error message for phone number format validation.
    ///     Usage: CandidateDetailsValidator, CompanyContactsValidator, CompanyDetailsValidator, CompanyLocationsValidator.
    /// </summary>
    public const string PhoneNumberMessage = "must be in the format (000) 000-0000.";

    #endregion

    #region ZIP Code Validation

    /// <summary>
    ///     Regular expression pattern for US ZIP code validation including ZIP+4 format.
    ///     Matches 5 digits optionally followed by 4 additional digits.
    /// </summary>
    /// <example>
    ///     Valid: "12345", "123456789"
    ///     Invalid: "1234", "12345-6789" (with hyphen)
    /// </example>
    public const string ZipCodeExtended = @"^\d{5}(\d{4})?$";

    /// <summary>
    ///     Regular expression pattern for basic US ZIP code validation (5 digits only).
    ///     Matches exactly 5 consecutive digits.
    /// </summary>
    /// <example>
    ///     Valid: "12345"
    ///     Invalid: "1234", "123456", "12345-6789"
    /// </example>
    public const string ZipCodeBasic = @"^\d{5}$";

    /// <summary>
    ///     Error message for extended ZIP code format validation.
    ///     Usage: CandidateDetailsValidator for ZIP code with optional ZIP+4.
    /// </summary>
    public const string ZipCodeExtendedMessage = "must be in the format 00000 or 00000-0000.";

    /// <summary>
    ///     Error message for basic ZIP code format validation.
    ///     Usage: CompanyDetailsValidator, CompanyLocationsValidator for 5-digit ZIP codes.
    /// </summary>
    public const string ZipCodeBasicMessage = "should be exactly 5 digits.";

    #endregion

    #region Password Validation Patterns

    /// <summary>
    ///     Regular expression pattern to check if password contains at least one numeric character.
    ///     Usage: UserValidator password complexity validation.
    /// </summary>
    public const string HasNumber = @"[0-9]+";

    /// <summary>
    ///     Regular expression pattern to check if password contains at least one lowercase character.
    ///     Usage: UserValidator password complexity validation.
    /// </summary>
    public const string HasLowerCase = @"[a-z]+";

    /// <summary>
    ///     Regular expression pattern to check if password contains at least one uppercase character.
    ///     Usage: UserValidator password complexity validation.
    /// </summary>
    public const string HasUpperCase = @"[A-Z]+";

    /// <summary>
    ///     Regular expression pattern to check if password length is between 6 and 16 characters.
    ///     Usage: UserValidator password length validation.
    /// </summary>
    public const string HasMin6Max16 = @".{6,16}";

    /// <summary>
    ///     Regular expression pattern to check if password contains at least one special character.
    ///     Includes common special characters: !@#$%^&*(),.?":{}|<>
    ///     Usage: UserValidator password complexity validation.
    /// </summary>
    public const string HasSpecialChar = @"[!@#$%^&*(),.?"":{ }|<>]+";

    #endregion

    #region URL Validation Patterns

    /// <summary>
    ///     Regular expression pattern to validate HTTP/HTTPS URL protocols.
    ///     Ensures URLs start with either http:// or https://
    ///     Usage: CompanyDetailsValidator for website URL validation.
    /// </summary>
    public const string HttpProtocol = @"^https?://";

    #endregion

    #region Helper Methods

    /// <summary>
    ///     Creates a compiled Regex instance for phone number validation.
    ///     Use this method when you need a Regex object rather than the pattern string.
    /// </summary>
    /// <returns>Compiled Regex for phone number validation.</returns>
    public static Regex GetPhoneNumberRegex() => new(PhoneNumber, RegexOptions.Compiled);

    /// <summary>
    ///     Creates a compiled Regex instance for extended ZIP code validation.
    ///     Use this method when you need a Regex object rather than the pattern string.
    /// </summary>
    /// <returns>Compiled Regex for extended ZIP code validation.</returns>
    public static Regex GetZipCodeExtendedRegex() => new(ZipCodeExtended, RegexOptions.Compiled);

    /// <summary>
    ///     Creates a compiled Regex instance for basic ZIP code validation.
    ///     Use this method when you need a Regex object rather than the pattern string.
    /// </summary>
    /// <returns>Compiled Regex for basic ZIP code validation.</returns>
    public static Regex GetZipCodeBasicRegex() => new(ZipCodeBasic, RegexOptions.Compiled);

    /// <summary>
    ///     Creates a compiled Regex instance for HTTP protocol validation.
    ///     Use this method when you need a Regex object rather than the pattern string.
    /// </summary>
    /// <returns>Compiled Regex for HTTP/HTTPS protocol validation.</returns>
    public static Regex GetHttpProtocolRegex() => new(HttpProtocol, RegexOptions.Compiled | RegexOptions.IgnoreCase);

    #endregion
}