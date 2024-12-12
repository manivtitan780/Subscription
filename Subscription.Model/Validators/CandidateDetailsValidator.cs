#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.Model
// File Name:           CandidateDetailsValidator.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu
// Created On:          12-10-2024 20:12
// Last Updated On:     12-11-2024 20:12
// *****************************************/

#endregion

namespace Subscription.Model.Validators;

public class CandidateDetailsValidator : AbstractValidator<CandidateDetails>
{
    public CandidateDetailsValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(candidate => candidate.FirstName).NotEmpty().WithMessage("First Name is required.")
                                                 .Length(1, 50).WithMessage("First Name must be between 1 and 50 characters.");

        RuleFor(candidate => candidate.MiddleName).MaximumLength(50).WithMessage("Middle Name must be less than or equal to 50 characters.");

        RuleFor(candidate => candidate.LastName).NotEmpty().WithMessage("Last Name is required.")
                                                .Length(1, 50).WithMessage("Last Name must be between 1 and 50 characters.");

        RuleFor(candidate => candidate.Title).NotEmpty().WithMessage("Title is required.")
                                             .Length(1, 200).WithMessage("Title must be between 1 and 200 characters.");

        RuleFor(candidate => candidate.Address1).NotEmpty().WithMessage("Address is required.")
                                                .Length(1, 255).WithMessage("Address must be between 1 and 255 characters.");

        RuleFor(candidate => candidate.Address2).MaximumLength(255).WithMessage("Address 2 must be less or equal to 255 characters.");

        RuleFor(candidate => candidate.City).NotEmpty().WithMessage("City is required.")
                                            .Length(1, 50).WithMessage("City must be between 1 and 50 characters.");

        RuleFor(candidate => candidate.Email).NotEmpty().WithMessage("Email is required.")
                                             .Length(5, 255).WithMessage("Email must be between 5 and 255 characters.")
                                             .Must(s => s.IsValidEmail()).WithMessage("Please enter a valid e-mail address.");

        RuleFor(candidate => candidate.Phone1).NotEmpty().WithMessage("Primary Phone is required.")
                                              .Matches(@"^\d{10}$").WithMessage("Primary Phone must be in the format (000) 000-0000.");

        RuleFor(candidate => candidate.Phone2).Matches(@"^\d{10}$").When(candidate => !candidate.Phone2.NullOrWhiteSpace())
                                              .WithMessage("Secondary Phone must be in the format (000) 000-0000.");


        RuleFor(candidate => candidate.ZipCode).NotEmpty().WithMessage("Zip Code is required.")
                                               .Matches(@"^\d{5}(\d{4})?$")
                                               .WithMessage("Zip Code must be in the format 00000 or 00000-0000.");

        RuleFor(candidate => candidate.HourlyRate).InclusiveBetween(0, 2_000).WithMessage("Minimum Hourly Rate must be less than or equal to $2,000.");

        RuleFor(candidate => candidate.HourlyRateHigh).InclusiveBetween(0, 2_000).WithMessage("Maximum Hourly Rate High must be less than or equal to $2,000.");

        RuleFor(candidate => candidate.SalaryLow).InclusiveBetween(0, 10_000_000).WithMessage("Minimum Salary must be less than or equal to $10,000,000.");

        RuleFor(candidate => candidate.SalaryHigh).InclusiveBetween(0, 10_000_000).WithMessage("Maximum Salary must be less than or equal to $10,000,000.");

        RuleFor(candidate => candidate.Keywords).NotEmpty().WithMessage("Keywords are required.")
                                                .Length(3, 500).WithMessage("Keywords must be between 3 and 500 characters.");

        RuleFor(candidate => candidate.Summary).MaximumLength(8000)
                                               .WithMessage("Summary must be less than or equal to 8000 characters.");

        RuleFor(candidate => candidate.TextResume).MaximumLength(250_000)
                                                  .WithMessage("Text Resume must be less than or equal to 250,000 characters.");

        RuleFor(candidate => candidate.RelocationNotes).MaximumLength(2_000).WithMessage("Relocation Notes must be less than or equal to 2,000 characters.");

        RuleFor(candidate => candidate.SecurityNotes).MaximumLength(2_000).WithMessage("Security Notes must be less than or equal to 2,000 characters.");

        RuleFor(candidate => candidate.LinkedIn).MaximumLength(255).WithMessage("LinkedIn must be less than or equal to 255 characters.");

        RuleFor(candidate => candidate.Facebook).MaximumLength(255).WithMessage("Facebook must be less than or equal to 255 characters.");

        RuleFor(candidate => candidate.Twitter).MaximumLength(255).WithMessage("X Profile Name must be less than or equal to 255 characters.");

        RuleFor(candidate => candidate.GooglePlus).MaximumLength(255).WithMessage("Bluesky Profile Name must be less than or equal to 255 characters.");

        RuleFor(candidate => candidate.MPCNotes).MaximumLength(2_000).WithMessage("MPC Notes must be less than or equal to 2,000 characters.");

        RuleFor(candidate => candidate.RateNotes).MaximumLength(2_000).WithMessage("Rating Notes must be less than or equal to 2,000 characters.");
    }
}