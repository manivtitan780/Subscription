#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.Model
// File Name:           ValidationMessages.cs
// Created By:          Claude Code (Anthropic)
// Created On:          07-12-2025
// Last Updated On:     07-12-2025
// *****************************************/

#endregion

namespace Subscription.Model.Constants;

/// <summary>
///     Provides standardized validation error messages used across FluentValidation validators.
///     Centralizes magic strings to improve maintainability and ensure consistency.
/// </summary>
/// <remarks>
///     This class eliminates magic string duplication found across 25+ validator files.
///     All constants are compile-time inlined with zero runtime performance impact.
/// </remarks>
public static class ValidationMessages
{
    #region Common Validation Messages

    /// <summary>
    ///     Standard "is required" message for mandatory fields.
    ///     Usage: Field validation where empty values are not allowed.
    /// </summary>
    public const string Required = "is required.";

    /// <summary>
    ///     Standard "should not be empty" message for non-null but empty fields.
    ///     Usage: String fields that cannot be empty or whitespace.
    /// </summary>
    public const string ShouldNotBeEmpty = "should not be empty.";

    /// <summary>
    ///     Standard email validation message.
    ///     Usage: Email format validation across all entities.
    /// </summary>
    public const string ValidEmailRequired = "Please enter a valid e-mail address.";

    /// <summary>
    ///     Standard file selection message for upload controls.
    ///     Usage: Document upload validators.
    /// </summary>
    public const string FileSelectionRequired = "Select a file to upload.";

    #endregion

    #region Length Validation Messages

    /// <summary>
    ///     Template for between-length validation messages.
    ///     Usage: Use with .WithMessage(ValidationMessages.FieldBetweenLength("Field Name"))
    /// </summary>
    public const string BetweenLength = "must be between {MinLength} and {MaxLength} characters.";

    /// <summary>
    ///     Template for maximum length validation messages.
    ///     Usage: Use with .WithMessage(ValidationMessages.FieldMaxLength("Field Name"))
    /// </summary>
    public const string MaxLength = "must be less than or equal to {MaxLength} characters.";

    /// <summary>
    ///     Template for minimum length validation messages.
    ///     Usage: Use with .WithMessage(ValidationMessages.FieldMinLength("Field Name"))
    /// </summary>
    public const string MinLength = "must be at least {MinLength} characters.";

    #endregion

    #region Document Validation Messages

    /// <summary>
    ///     Document name required validation message.
    ///     Usage: CandidateDocumentValidator, CompanyDocumentValidator, RequisitionDocumentValidator.
    /// </summary>
    public const string DocumentNameRequired = "Document Name should not be empty.";

    /// <summary>
    ///     Document name length validation message.
    ///     Usage: Document name fields with 2-255 character range.
    /// </summary>
    public const string DocumentNameLength = "Document should be between 2 and 255 characters.";

    /// <summary>
    ///     Document notes required validation message.
    ///     Usage: Document validators requiring notes.
    /// </summary>
    public const string DocumentNotesRequired = "Document Notes should not be empty.";

    /// <summary>
    ///     Document notes length validation message.
    ///     Usage: Document notes fields with 10-2000 character range.
    /// </summary>
    public const string DocumentNotesLength = "Document Notes should be between 10 and 2000 characters.";

    #endregion

    #region URL and Website Validation Messages

    /// <summary>
    ///     Website URL protocol validation message.
    ///     Usage: CompanyDetailsValidator for website URL format.
    /// </summary>
    public const string UrlProtocolRequired = "Website Url should start with either http:// or https://";

    /// <summary>
    ///     General URL format validation message.
    ///     Usage: URL format validation across entities.
    /// </summary>
    public const string ValidUrlRequired = "Please enter a valid Website Url.";

    #endregion

    #region Password Validation Messages

    /// <summary>
    ///     Password complexity requirements message.
    ///     Usage: UserValidator password validation.
    /// </summary>
    public const string PasswordComplexity = "Password should be between 6 and 16 characters and contain at least 1 uppercase, lowercase character and 1 of either a numeric or special character.";

    #endregion

    #region Helper Methods

    /// <summary>
    ///     Creates a standardized "field is required" message.
    /// </summary>
    /// <param name="fieldName">The name of the field requiring validation.</param>
    /// <returns>Formatted required field validation message.</returns>
    /// <example>
    ///     ValidationMessages.FieldRequired("First Name") returns "First Name is required."
    /// </example>
    public static string FieldRequired(string fieldName) => $"{fieldName} {Required}";

    /// <summary>
    ///     Creates a standardized "field should not be empty" message.
    /// </summary>
    /// <param name="fieldName">The name of the field requiring validation.</param>
    /// <returns>Formatted empty field validation message.</returns>
    /// <example>
    ///     ValidationMessages.FieldShouldNotBeEmpty("Last Name") returns "Last Name should not be empty."
    /// </example>
    public static string FieldShouldNotBeEmpty(string fieldName) => $"{fieldName} {ShouldNotBeEmpty}";

    /// <summary>
    ///     Creates a standardized between-length validation message.
    /// </summary>
    /// <param name="fieldName">The name of the field requiring validation.</param>
    /// <returns>Formatted length validation message with placeholders.</returns>
    /// <example>
    ///     ValidationMessages.FieldBetweenLength("Title") returns "Title must be between {MinLength} and {MaxLength} characters."
    /// </example>
    public static string FieldBetweenLength(string fieldName) => $"{fieldName} {BetweenLength}";

    /// <summary>
    ///     Creates a standardized maximum length validation message.
    /// </summary>
    /// <param name="fieldName">The name of the field requiring validation.</param>
    /// <returns>Formatted max length validation message with placeholder.</returns>
    /// <example>
    ///     ValidationMessages.FieldMaxLength("Summary") returns "Summary must be less than or equal to {MaxLength} characters."
    /// </example>
    public static string FieldMaxLength(string fieldName) => $"{fieldName} {MaxLength}";

    /// <summary>
    ///     Creates a standardized minimum length validation message.
    /// </summary>
    /// <param name="fieldName">The name of the field requiring validation.</param>
    /// <returns>Formatted min length validation message with placeholder.</returns>
    /// <example>
    ///     ValidationMessages.FieldMinLength("Password") returns "Password must be at least {MinLength} characters."
    /// </example>
    public static string FieldMinLength(string fieldName) => $"{fieldName} {MinLength}";

    #endregion
}