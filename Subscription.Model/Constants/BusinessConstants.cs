#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.Model
// File Name:           BusinessConstants.cs
// Created By:          Claude Code (Anthropic)
// Created On:          07-12-2025
// Last Updated On:     07-12-2025
// *****************************************/

#endregion

namespace Subscription.Model.Constants;

/// <summary>
///     Provides standardized business rule constants and field length constraints
///     used across FluentValidation validators for consistent business logic validation.
/// </summary>
/// <remarks>
///     This class centralizes business rules and field constraints found across validators
///     to ensure consistency and eliminate magic numbers. All constants are compile-time
///     values with zero runtime performance impact.
/// </remarks>
public static class BusinessConstants
{
    #region Business Rule Limits

    /// <summary>
    ///     Maximum hourly rate allowed for candidates.
    ///     Usage: CandidateDetailsValidator for hourly rate validation.
    /// </summary>
    public const decimal MaxHourlyRate = 2_000;

    /// <summary>
    ///     Maximum salary allowed for candidates.
    ///     Usage: CandidateDetailsValidator for salary validation.
    /// </summary>
    public const decimal MaxSalary = 10_000_000;

    /// <summary>
    ///     Maximum experience in months allowed for candidate skills.
    ///     Usage: CandidateSkillsValidator for experience validation.
    /// </summary>
    public const int MaxExperienceMonths = 1_000;

    #endregion

    #region Field Length Constraints

    /// <summary>
    ///     Provides standardized field length constraints used across multiple entities.
    /// </summary>
    public static class FieldLengths
    {
        #region Common Field Lengths

        /// <summary>
        ///     Standard length for name fields (First Name, Last Name, etc.).
        ///     Usage: CandidateDetailsValidator, CompanyContactsValidator, UserValidator.
        /// </summary>
        public const int Name = 50;

        /// <summary>
        ///     Standard length for title/position fields.
        ///     Usage: CandidateDetailsValidator, CompanyContactsValidator for job titles.
        /// </summary>
        public const int Title = 200;

        /// <summary>
        ///     Standard length for email address fields.
        ///     Usage: All validators with email validation.
        /// </summary>
        public const int Email = 255;

        /// <summary>
        ///     Standard length for address fields.
        ///     Usage: CandidateDetailsValidator, CompanyDetailsValidator, CompanyLocationsValidator.
        /// </summary>
        public const int Address = 255;

        /// <summary>
        ///     Standard length for city fields.
        ///     Usage: CandidateDetailsValidator, CompanyDetailsValidator, CompanyLocationsValidator.
        /// </summary>
        public const int City = 50;

        /// <summary>
        ///     Standard length for state fields.
        ///     Usage: Multiple validators for state/province validation.
        /// </summary>
        public const int State = 5;

        /// <summary>
        ///     Standard length for ZIP/postal code fields.
        ///     Usage: Multiple validators for postal code validation.
        /// </summary>
        public const int ZipCode = 10;

        #endregion

        #region Document and Content Fields

        /// <summary>
        ///     Standard length for document name fields.
        ///     Usage: CandidateDocumentValidator, CompanyDocumentValidator, RequisitionDocumentValidator.
        /// </summary>
        public const int DocumentName = 255;

        /// <summary>
        ///     Standard length for general notes and comments fields.
        ///     Usage: Multiple validators for notes validation.
        /// </summary>
        public const int Notes = 2_000;

        /// <summary>
        ///     Standard length for summary fields.
        ///     Usage: CandidateDetailsValidator for candidate summary.
        /// </summary>
        public const int Summary = 8_000;

        /// <summary>
        ///     Maximum length for extracted text resume content.
        ///     Usage: CandidateDetailsValidator for text resume validation.
        /// </summary>
        public const int TextResume = 250_000;

        /// <summary>
        ///     Standard length for keywords fields.
        ///     Usage: CandidateDetailsValidator, RequisitionDetailsValidator for keyword validation.
        /// </summary>
        public const int Keywords = 500;

        #endregion

        #region Specialized Fields

        /// <summary>
        ///     Standard length for company/organization name fields.
        ///     Usage: CompanyDetailsValidator, RequisitionDetailsValidator.
        /// </summary>
        public const int CompanyName = 100;

        /// <summary>
        ///     Standard length for phone number fields.
        ///     Usage: Multiple validators for phone validation.
        /// </summary>
        public const int PhoneNumber = 10;

        /// <summary>
        ///     Standard length for short description fields.
        ///     Usage: AdminListValidator, DocumentTypesValidator for description validation.
        /// </summary>
        public const int ShortDescription = 100;

        /// <summary>
        ///     Standard length for URL/website fields.
        ///     Usage: CompanyDetailsValidator for website URL validation.
        /// </summary>
        public const int Url = 500;

        /// <summary>
        ///     Standard length for relocation and security notes.
        ///     Usage: CandidateDetailsValidator for specialized notes.
        /// </summary>
        public const int SpecializedNotes = 2_000;

        /// <summary>
        ///     Standard length for LinkedIn profile URLs.
        ///     Usage: CandidateDetailsValidator for LinkedIn validation.
        /// </summary>
        public const int LinkedIn = 255;

        #endregion
    }

    #endregion

    #region Validation Ranges

    /// <summary>
    ///     Provides standardized validation ranges for numeric fields.
    /// </summary>
    public static class ValidationRanges
    {
        #region Document Field Ranges

        /// <summary>
        ///     Minimum length for document names.
        ///     Usage: Document validators requiring minimum name length.
        /// </summary>
        public const int DocumentNameMinLength = 2;

        /// <summary>
        ///     Minimum length for document notes.
        ///     Usage: Document validators requiring substantial notes.
        /// </summary>
        public const int DocumentNotesMinLength = 10;

        #endregion

        #region Password Requirements

        /// <summary>
        ///     Minimum password length.
        ///     Usage: UserValidator for password validation.
        /// </summary>
        public const int PasswordMinLength = 6;

        /// <summary>
        ///     Maximum password length.
        ///     Usage: UserValidator for password validation.
        /// </summary>
        public const int PasswordMaxLength = 16;

        #endregion

        #region Business Logic Ranges

        /// <summary>
        ///     Minimum value for numeric ratings or scores.
        ///     Usage: CandidateRatingValidator, various rating validators.
        /// </summary>
        public const int MinRating = 1;

        /// <summary>
        ///     Maximum value for numeric ratings or scores.
        ///     Usage: CandidateRatingValidator, various rating validators.
        /// </summary>
        public const int MaxRating = 10;

        #endregion
    }

    #endregion
}