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

using Subscription.Model.Constants;

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

        // Using ValidationMessages constants to eliminate magic strings and improve maintainability
        RuleFor(x => x.EmailAddress).NotEmpty().WithMessage(ValidationMessages.FieldShouldNotBeEmpty("Email Address"))
                                    .Length(1, BusinessConstants.FieldLengths.Email).WithMessage(ValidationMessages.FieldMaxLength("Email Address"))
                                    .Must(s => s.IsValidEmail()).WithMessage(ValidationMessages.ValidEmailRequired);

        // Using ValidationMessages constants to eliminate magic strings and improve maintainability
        RuleFor(x => x.StreetName).NotEmpty().WithMessage(ValidationMessages.FieldShouldNotBeEmpty("Address"))
                                  .Length(5, 500).WithMessage(ValidationMessages.FieldBetweenLength("Address"));

        // Using ValidationMessages constants to eliminate magic strings and improve maintainability
        RuleFor(x => x.City).NotEmpty().WithMessage(ValidationMessages.FieldShouldNotBeEmpty("City Name"))
                            .Length(2, 100).WithMessage(ValidationMessages.FieldBetweenLength("City Name"));

        // Using ValidationMessages constants to eliminate magic strings and improve maintainability
        RuleFor(x => x.StateID).NotEmpty().WithMessage(ValidationMessages.FieldRequired("State"));

        // Using ValidationMessages constants and ValidationPatterns to eliminate magic strings and improve maintainability
        RuleFor(x => x.ZipCode).NotEmpty().WithMessage(ValidationMessages.FieldShouldNotBeEmpty("Zip Code"))
                               .Length(BusinessConstants.FieldLengths.State).WithMessage(ValidationPatterns.ZipCodeBasicMessage);

        // Using ValidationMessages constants and ValidationPatterns to eliminate magic strings and improve maintainability
        RuleFor(x => x.Phone).NotEmpty().WithMessage(ValidationMessages.FieldShouldNotBeEmpty("Company Phone Number"))
                             .Length(BusinessConstants.FieldLengths.PhoneNumber).WithMessage($"Phone Number {ValidationPatterns.PhoneNumberMessage}");

        // Using BusinessConstants for field length validation
        RuleFor(x => x.Notes).Length(0, BusinessConstants.FieldLengths.Notes).WithMessage(ValidationMessages.FieldMaxLength("Company Location Notes"));
    }
}