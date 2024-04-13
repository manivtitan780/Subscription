#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.Model
// File Name:           CompanyLocationsValidator.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu
// Created On:          3-19-2024 16:27
// Last Updated On:     3-19-2024 18:52
// *****************************************/

#endregion

namespace Subscription.Model.Validators;

public class CompanyLocationsValidator : AbstractValidator<CompanyLocations>
{
    public CompanyLocationsValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

		//When(x => x.CompanyID != 0, () =>
		//							{
		//								RuleFor(x => x.CompanyID).NotEmpty().WithMessage("Select a Company");
		//							});

        RuleFor(x => x.EmailAddress).NotEmpty().WithMessage("Email Address should not be empty.")
                                    .Length(1, 255).WithMessage("Email Address should be less than {MaxLength} characters.")
                                    .Must(s => s.IsValidEmail()).WithMessage("Please enter a valid e-mail address.");

        RuleFor(x => x.StreetName).NotEmpty().WithMessage("Address should not be empty.")
                                  .Length(5, 500).WithMessage("Address should be between {MinLength} and {MaxLength} characters.");

        RuleFor(x => x.City).NotEmpty().WithMessage("City Name should not be empty.")
                            .Length(2, 100).WithMessage("City Name should be between {MinLength} and {MaxLength} characters.");

        RuleFor(x => x.StateID).NotEmpty().WithMessage("Select a State");

        RuleFor(x => x.ZipCode).NotEmpty().WithMessage("Zip Code should not be empty.")
                               .Length(5).WithMessage("Zip Code should be exactly {MaxLength} digits.");

        RuleFor(x => x.Phone).NotEmpty().WithMessage("Company Phone Number should not be empty.")
                             .Length(10).WithMessage("Phone Number should be exactly {MaxLength} digits and in the format (000) 000-0000.");

        RuleFor(x => x.Notes).Length(0, 2000).WithMessage("Company Location Notes should be less than 2000 characters.");
    }
}