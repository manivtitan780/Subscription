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

// Added using statement for validation constants to eliminate magic strings
using Subscription.Model.Constants;

namespace Subscription.Model.Validators;

public class CompanyContactsValidator : AbstractValidator<CompanyContacts>
{
    public CompanyContactsValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        // Using ValidationMessages constants to eliminate magic strings and improve maintainability
        RuleFor(x => x.CompanyID).NotEmpty().WithMessage(ValidationMessages.FieldShouldNotBeEmpty("Company ID"));

        RuleFor(x => x.Prefix).MaximumLength(10).WithMessage("Prefix should not be more than 10 characters.");

        // Using ValidationMessages constants to eliminate magic strings and improve maintainability
        RuleFor(x => x.FirstName).NotEmpty().WithMessage(ValidationMessages.FieldShouldNotBeEmpty("First Name"))
                                 .MaximumLength(BusinessConstants.FieldLengths.Name).WithMessage(ValidationMessages.FieldMaxLength("First Name"));

        RuleFor(x => x.MiddleInitial).MaximumLength(10).WithMessage("Initial should not be more than 10 characters.");

        // Using ValidationMessages constants to eliminate magic strings and improve maintainability
        RuleFor(x => x.LastName).NotEmpty().WithMessage(ValidationMessages.FieldShouldNotBeEmpty("Last Name"))
                                .MaximumLength(BusinessConstants.FieldLengths.Name).WithMessage(ValidationMessages.FieldMaxLength("Last Name"));

        RuleFor(x => x.Suffix).MaximumLength(10).WithMessage("Suffix should not be more than 10 characters.");

        // Using ValidationMessages constants to eliminate magic strings and improve maintainability
        RuleFor(x => x.LocationID).NotEmpty().WithMessage(ValidationMessages.FieldRequired("Location") + " Select a location from the list.");

        // Using ValidationMessages constants to eliminate magic strings and improve maintainability
        RuleFor(x => x.EmailAddress).NotEmpty().WithMessage(ValidationMessages.FieldShouldNotBeEmpty("Email Address"))
                                    .Length(1, BusinessConstants.FieldLengths.Email).WithMessage(ValidationMessages.FieldMaxLength("Email Address"))
                                    .Must(s => s.IsValidEmail()).WithMessage(ValidationMessages.ValidEmailRequired);

        // Using ValidationPatterns constants to eliminate magic strings and improve maintainability
        RuleFor(x => x.Phone).NotEmpty().WithMessage(ValidationMessages.FieldShouldNotBeEmpty("Company Phone Number"))
                             .Length(BusinessConstants.FieldLengths.PhoneNumber).WithMessage($"Phone Number {ValidationPatterns.PhoneNumberMessage}");

        // Using ValidationMessages constants to eliminate magic strings and improve maintainability
        RuleFor(x => x.Role).NotEmpty().WithMessage(ValidationMessages.FieldRequired("Role") + " Select a role from the list.");
    }
}