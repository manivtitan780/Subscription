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

using Subscription.Model.Constants;

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

		// Using ValidationMessages constants to eliminate magic strings and improve maintainability
		RuleFor(x => x.Status).NotEmpty().WithMessage(ValidationMessages.FieldRequired("Activity Status"));
		
		// Using ValidationMessages constants and BusinessConstants to eliminate magic strings and improve maintainability
		RuleFor(x => x.Notes).NotEmpty().WithMessage(ValidationMessages.FieldRequired("Activity Notes"))
			.Length(2, BusinessConstants.FieldLengths.Notes).WithMessage(ValidationMessages.FieldBetweenLength("Notes"));
	}
}

