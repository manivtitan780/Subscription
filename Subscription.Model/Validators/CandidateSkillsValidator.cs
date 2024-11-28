#region Header

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

        RuleFor(x => x.Skill).NotEmpty().WithMessage("Skill Name cannot be empty")
                             .Length(2, 100).WithMessage("Skill Name should be between {MinLength} and {MaxLength} characters.");

        When(x => x.ExpMonth > 0, () =>
                                  {
                                      RuleFor(x => x.LastUsed).Must(CheckYear)
                                                              .WithMessage("Last Used should not be beyond current year. If currently being used or unknown enter Zero.");
                                  });

        RuleFor(x => x.ExpMonth).Must(CheckMonths).WithMessage("Experience in months should not be more than 1000.");
    }

    /// <summary>
    ///     Checks if the candidate's experience in months with the skill is not more than 1000.
    /// </summary>
    /// <param name="expMonths">
    ///     The number of months of experience the candidate has with the skill.
    /// </param>
    /// <returns>
    ///     True if the candidate's experience in months with the skill is less than or equal to 1000, otherwise false.
    /// </returns>
    private static bool CheckMonths(short expMonths)
	{
		return expMonths <= 1000;
	}

	/// <summary>
    ///     Checks if the year the skill was last used is not beyond the current year.
    /// </summary>
    /// <param name="lastUsed">
    ///     The year the skill was last used. If the skill is currently being used or the last used year is unknown, this value
    ///     should be zero.
    /// </param>
    /// <returns>
    ///     True if the year the skill was last used is less than or equal to the current year, otherwise false.
    /// </returns>
    private static bool CheckYear(short lastUsed)
	{
		return lastUsed <= DateTime.Today.Year;
	}
}