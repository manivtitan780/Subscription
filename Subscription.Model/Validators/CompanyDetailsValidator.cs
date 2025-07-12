#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.Model
// File Name:           CompanyDetailsValidator.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu
// Created On:          3-12-2024 20:30
// Last Updated On:     3-20-2024 19:28
// *****************************************/

#endregion

#region Using

using RestSharp;
// Added using statement for validation constants to eliminate magic strings
using Subscription.Model.Constants;

#endregion

namespace Subscription.Model.Validators;

public class CompanyDetailsValidator : AbstractValidator<CompanyDetails>
{
    public CompanyDetailsValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        // Using ValidationMessages constants to eliminate magic strings and improve maintainability
        RuleFor(x => x.Name).NotEmpty().WithMessage(ValidationMessages.FieldShouldNotBeEmpty("Company Name"))
                            .Length(1, BusinessConstants.FieldLengths.CompanyName).WithMessage(ValidationMessages.FieldMaxLength("Company Name"));

        // Using ValidationMessages constants to eliminate magic strings and improve maintainability
        RuleFor(x => x.EIN).NotEmpty().WithMessage(ValidationMessages.FieldShouldNotBeEmpty("Company EIN#"))
                           .Length(9).WithMessage("Company EIN# should be exactly 9 digits and in the format 00-0000000")
                           .Must((companyDetails, text) => CheckEINExists(text, companyDetails.ID))
                           .WithMessage("Company EIN# is already associated with another company. Please enter the correct EIN#.");

        // Using ValidationMessages constants to eliminate magic strings and improve maintainability
        RuleFor(x => x.EmailAddress).NotEmpty().WithMessage(ValidationMessages.FieldShouldNotBeEmpty("Email Address"))
                                    .Length(1, BusinessConstants.FieldLengths.Email).WithMessage(ValidationMessages.FieldMaxLength("Email Address"))
                                    .Must(s => s.IsValidEmail()).WithMessage(ValidationMessages.ValidEmailRequired);

        RuleFor(x => x.Website).MaximumLength(255).WithMessage("Website Address should be less than 255 characters.")
                               .Must(s => !s.NullOrWhiteSpace() && (s.StartsWith("http://") || s.StartsWith("https://"))).WithMessage(ValidationMessages.UrlProtocolRequired)
                               .Must(s => !s.NullOrWhiteSpace() && s.IsValidUrl()).WithMessage(ValidationMessages.ValidUrlRequired);

        When(x => !x.DUNS.NullOrWhiteSpace(), () =>
                                              {
                                                  RuleFor(x => x.DUNS).Length(9)
                                                                      .WithMessage("Company DUNS# should be exactly 9 digits and in the format 00-000-0000");
                                              });

        RuleFor(x => x.Notes).Length(0, 2000).WithMessage("Company Notes should be less than 2000 characters.");

        // Using ValidationMessages constants to eliminate magic strings and improve maintainability
        RuleFor(x => x.StreetName).NotEmpty().WithMessage(ValidationMessages.FieldShouldNotBeEmpty("Address"))
                                  .Length(5, 500).WithMessage("Address should be between 5 and 500 characters.");

        // Using ValidationMessages constants to eliminate magic strings and improve maintainability
        RuleFor(x => x.City).NotEmpty().WithMessage(ValidationMessages.FieldShouldNotBeEmpty("City Name"))
                            .Length(2, 100).WithMessage("City Name should be between 2 and 100 characters.");

        // Using ValidationMessages constants to eliminate magic strings and improve maintainability
        RuleFor(x => x.StateID).NotEmpty().WithMessage("Select a State");

        // Using ValidationPatterns constants to eliminate magic strings and improve maintainability
        RuleFor(x => x.ZipCode).NotEmpty().WithMessage(ValidationMessages.FieldShouldNotBeEmpty("Zip Code"))
                               .Length(5).WithMessage(ValidationPatterns.ZipCodeBasicMessage);

        // Using ValidationPatterns constants to eliminate magic strings and improve maintainability
        RuleFor(x => x.Phone).NotEmpty().WithMessage(ValidationMessages.FieldShouldNotBeEmpty("Company Phone Number"))
                             .Length(BusinessConstants.FieldLengths.PhoneNumber).WithMessage($"Phone Number {ValidationPatterns.PhoneNumberMessage}");

        RuleFor(x => x.LocationNotes).Length(0, 2000).WithMessage("Location Notes should be less than 2000 characters.");
    }

    private static bool CheckEINExists(string ein, int companyID)
    {
        using RestClient _client = new(GeneralModel.APIHost ?? "");
        RestRequest _request = new("Company/CheckEIN")
                               {
                                   RequestFormat = DataFormat.Json
                               };

        _request.AddQueryParameter("ein", ein);
        _request.AddQueryParameter("companyID", companyID);
        bool _response = _client.GetAsync<bool>(_request).Result;
        return !_response;
    }
}