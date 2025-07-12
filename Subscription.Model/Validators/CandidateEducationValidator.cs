#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.Model
// File Name:           CandidateEducationValidator.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu
// Created On:          12-02-2024 19:12
// Last Updated On:     12-04-2024 20:12
// *****************************************/

#endregion

using Subscription.Model.Constants;

namespace Subscription.Model.Validators;

/// <summary>
///     Provides validation rules for the <see cref="CandidateEducation" /> model.
/// </summary>
/// <remarks>
///     This validator ensures that the required fields for a candidate's education details are provided.
///     It checks that the College and Degree fields are not empty and provides appropriate error messages if they are.
/// </remarks>
public class CandidateEducationValidator : AbstractValidator<CandidateEducation>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="CandidateEducationValidator" /> class.
    /// </summary>
    /// <remarks>
    ///     This constructor sets up the validation rules for the <see cref="CandidateEducation" /> model, ensuring that
    ///     the College and Degree fields are not empty and providing appropriate error messages if they are.
    /// </remarks>
    public CandidateEducationValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        // Using ValidationMessages constants to eliminate magic strings and improve maintainability
        RuleFor(x => x.College).NotEmpty().WithMessage(ValidationMessages.FieldRequired("School/College/Institution"));

        // Using ValidationMessages constants to eliminate magic strings and improve maintainability
        RuleFor(x => x.Degree).NotEmpty().WithMessage(ValidationMessages.FieldRequired("Degree/Diploma/Course Name"));
    }
}