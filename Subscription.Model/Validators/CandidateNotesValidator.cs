#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.Model
// File Name:           CandidateNotesValidator.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu
// Created On:          12-04-2024 20:12
// Last Updated On:     12-04-2024 20:12
// *****************************************/

#endregion

using Subscription.Model.Constants;

namespace Subscription.Model.Validators;

/// <summary>
///     Represents a validator for the CandidateNotes class.
/// </summary>
/// <remarks>
///     The CandidateNotesValidator class extends the AbstractValidator class and defines rules for validating instances of
///     the CandidateNotes class.
///     The validation rules ensure that the Notes property of the CandidateNotes class is not empty and its length is
///     between 5 and 1000 characters.
/// </remarks>
public class CandidateNotesValidator : AbstractValidator<CandidateNotes>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="CandidateNotesValidator" /> class.
    /// </summary>
    /// <remarks>
    ///     This constructor sets the RuleLevelCascadeMode to Stop and defines validation rules for the Notes property of the
    ///     CandidateNotes class.
    ///     The Notes property must not be empty and its length should be between 5 and 1000 characters.
    /// </remarks>
    public CandidateNotesValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        // Using ValidationMessages constants to eliminate magic strings and improve maintainability
        RuleFor(x => x.Notes).NotEmpty().WithMessage(ValidationMessages.FieldShouldNotBeEmpty("Notes"))
                             .Length(5, 1000).WithMessage(ValidationMessages.FieldBetweenLength("Notes"));
    }
}