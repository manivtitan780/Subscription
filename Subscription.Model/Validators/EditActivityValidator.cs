#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            ProfSvc_AppTrack
// Project:             ProfSvc_Classes
// File Name:           EditActivityValidator.cs
// Created By:          Gowtham Selvaraj
// Created On:          02-01-2024 18:00
// Last Updated On:     02-01-2024 18:00
// *****************************************/

#endregion
namespace Subscription.Model.Validators;

/// <summary>
///     Provides validation for the <see cref="CandidateActivity" /> class.
/// </summary>
/// <remarks>
///     This class extends the <see cref="AbstractValidator{T}" /> class, where T is <see cref="CandidateActivity" />.
///     It defines the validation rules for the properties of the <see cref="CandidateActivity" /> class.
/// </remarks>
public class EditActivityValidator : AbstractValidator<CandidateActivity>
{
	/// <summary>
	///     Initializes a new instance of the <see cref="EditActivityValidator" /> class.
	/// </summary>
	/// <remarks>
	///     This constructor sets up the validation rules for the CandidateActivityRequisition object. It includes rules for Activity Notes properties.
	/// </remarks>
	public EditActivityValidator()
	{
		RuleLevelCascadeMode = CascadeMode.Stop;

		RuleFor(x => x.Status).NotEmpty().WithMessage("Activity Status is required.");
		
		RuleFor(x => x.Notes).NotEmpty().WithMessage("Activity Notes is required.")
			.Length(2, 2000).WithMessage("Notes should be between {MinLength} to {MaxLength} characters."); ;
	}
}

