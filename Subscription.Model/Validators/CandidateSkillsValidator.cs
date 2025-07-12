﻿#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            ProfSvc_AppTrack
// Project:             ProfSvc_Classes
// File Name:           CandidateSkillsValidator.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja
// Created On:          08-01-2023 21:00
// Last Updated On:     10-14-2023 20:22
// *****************************************/

#endregion

using Subscription.Model.Constants;

namespace Subscription.Model.Validators;

/// <summary>
///     Represents a validator for the <see cref="CandidateSkills" /> class.
/// </summary>
/// <remarks>
///     This class provides validation rules for the properties of a <see cref="CandidateSkills" /> object.
///     It ensures that the Skill property is not empty and is between 2 and 100 characters.
///     It also validates that the ExpMonth property is not more than 1000 and the LastUsed property is not beyond the
///     current year.
/// </remarks>
public class CandidateSkillsValidator : AbstractValidator<CandidateSkills>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="CandidateSkillsValidator" /> class.
    /// </summary>
    /// <remarks>
    ///     This constructor sets up the rules for validating a <see cref="CandidateSkills" /> object.
    ///     It ensures that the Skill property is not empty and is between 2 and 100 characters.
    ///     It also validates that the ExpMonth property is not more than 1000 and the LastUsed property is not beyond the
    ///     current year.
    /// </remarks>
    public CandidateSkillsValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        // Using ValidationMessages constants to eliminate magic strings and improve maintainability
        RuleFor(x => x.Skill).NotEmpty().WithMessage(ValidationMessages.FieldShouldNotBeEmpty("Skill Name"))
                             .Length(2, 100).WithMessage(ValidationMessages.FieldBetweenLength("Skill Name"));

        When(x => x.ExpMonth > 0, () =>
                                  {
                                      RuleFor(x => x.LastUsed).Must(lastUsed => lastUsed <= DateTime.Today.Year)
                                                              .WithMessage("Last Used should not be beyond current year. If currently being used or unknown enter Zero.");
                                  });

        // Using BusinessConstants for maximum experience validation
        RuleFor(x => x.ExpMonth).Must(month => month <= BusinessConstants.MaxExperienceMonths).WithMessage($"Experience in months should not be more than {BusinessConstants.MaxExperienceMonths}.");
    }

}