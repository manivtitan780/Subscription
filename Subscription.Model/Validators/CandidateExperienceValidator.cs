#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.Model
// File Name:           CandidateExperienceValidator.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu
// Created On:          12-02-2024 20:12
// Last Updated On:     12-04-2024 20:12
// *****************************************/

#endregion

using Subscription.Model.Constants;

namespace Subscription.Model.Validators;

/// <summary>
///     Provides validation rules for the <see cref="CandidateExperience" /> model.
/// </summary>
/// <remarks>
///     This validator ensures that the required fields for a candidate's experience details are provided.
///     It checks that the Employer, Location, and Title fields are not empty and provides appropriate error messages if
///     they are.
/// </remarks>
public class CandidateExperienceValidator : AbstractValidator<CandidateExperience>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="CandidateExperienceValidator" /> class.
    /// </summary>
    /// <remarks>
    ///     This constructor sets up the validation rules for the <see cref="CandidateExperience" /> model, ensuring that
    ///     the Employer, Location, and Title fields are not empty and providing appropriate error messages if they are.
    /// </remarks>
    public CandidateExperienceValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        // Using ValidationMessages constants to eliminate magic strings and improve maintainability
        RuleFor(x => x.Employer).NotEmpty().WithMessage(ValidationMessages.FieldRequired("Employer"));

        // Using ValidationMessages constants to eliminate magic strings and improve maintainability
        RuleFor(x => x.Location).NotEmpty().WithMessage(ValidationMessages.FieldRequired("Location"));

        // Using ValidationMessages constants to eliminate magic strings and improve maintainability
        RuleFor(x => x.Title).NotEmpty().WithMessage(ValidationMessages.FieldRequired("Job Title"));
    }
}