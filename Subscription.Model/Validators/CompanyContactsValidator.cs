#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.Model
// File Name:           CompanyContactsValidator.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu
// Created On:          04-25-2024 15:04
// Last Updated On:     10-29-2024 15:10
// *****************************************/

#endregion

namespace Subscription.Model.Validators;

public class CompanyContactsValidator : AbstractValidator<CompanyContacts>
{
    public CompanyContactsValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.CompanyID).NotEmpty().WithMessage("Company ID should not be empty.");

        RuleFor(x => x.Prefix).MaximumLength(10).WithMessage("Prefix should not be more than 10 characters.");

        RuleFor(x => x.FirstName).NotEmpty().WithMessage("First Name should not be empty.")
                                 .MaximumLength(50).WithMessage("First Name should not be more than 50 characters.");

        RuleFor(x => x.MiddleInitial).MaximumLength(10).WithMessage("Initial should not be more than 10 characters.");

        RuleFor(x => x.LastName).NotEmpty().WithMessage("Last Name should not be empty.")
                                .MaximumLength(50).WithMessage("Last Name should not be more than 50 characters.");

        RuleFor(x => x.Suffix).MaximumLength(10).WithMessage("Suffix should not be more than 10 characters.");

        RuleFor(x => x.LocationID).NotEmpty().WithMessage("Location is required. Select a location from the list.");

        RuleFor(x => x.EmailAddress).NotEmpty().WithMessage("Email Address should not be empty.")
                                    .Length(1, 255).WithMessage("Email Address should be less than 255 characters.")
                                    .Must(s => s.IsValidEmail()).WithMessage("Please enter a valid e-mail address.");

        RuleFor(x => x.Phone).NotEmpty().WithMessage("Company Phone Number should not be empty.")
                             .Length(10).WithMessage("Phone Number should be exactly 10 digits and in the format (000) 000-0000.");

        RuleFor(x => x.Role).NotEmpty().WithMessage("Role is required. Select a role from the list.");
    }
}