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

#endregion

namespace Subscription.Model.Validators;

public class CompanyDetailsValidator : AbstractValidator<CompanyDetails>
{
    public CompanyDetailsValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.Name).NotEmpty().WithMessage("Company Name should not be empty.")
                            .Length(1, 100).WithMessage("Company Name should be less than 100 characters.");

        RuleFor(x => x.EIN).NotEmpty().WithMessage("Company EIN# should not be empty.")
                           .Length(9).WithMessage("Company EIN# should be exactly 9 digits and in the format 00-0000000")
                           .Must((companyDetails, text) => CheckEINExists(text, companyDetails.ID))
                           .WithMessage("Company EIN# is already associated with another company. Please enter the correct EIN#.");

        RuleFor(x => x.EmailAddress).NotEmpty().WithMessage("Email Address should not be empty.")
                                    .Length(1, 255).WithMessage("Email Address should be less than 255 characters.")
                                    .Must(s => s.IsValidEmail()).WithMessage("Please enter a valid e-mail address.");

        RuleFor(x => x.Website).MaximumLength(255).WithMessage("Website Address should be less than 255 characters.")
                               .Must(s => !s.NullOrWhiteSpace() && (s.StartsWith("http://") || s.StartsWith("https://"))).WithMessage("Website Url should start with either http:// or https://")
                               .Must(s => !s.NullOrWhiteSpace() && s.IsValidUrl()).WithMessage("Please enter a valid Website Url.");

        When(x => !x.DUNS.NullOrWhiteSpace(), () =>
                                              {
                                                  RuleFor(x => x.DUNS).Length(9)
                                                                      .WithMessage("Company DUNS# should be exactly 9 digits and in the format 00-000-0000");
                                              });

        RuleFor(x => x.Notes).Length(0, 2000).WithMessage("Company Notes should be less than 2000 characters.");

        RuleFor(x => x.StreetName).NotEmpty().WithMessage("Address should not be empty.")
                                  .Length(5, 500).WithMessage("Address should be between 5 and 500 characters.");

        RuleFor(x => x.City).NotEmpty().WithMessage("City Name should not be empty.")
                            .Length(2, 100).WithMessage("City Name should be between 2 and 100 characters.");

        RuleFor(x => x.StateID).NotEmpty().WithMessage("Select a State");

        RuleFor(x => x.ZipCode).NotEmpty().WithMessage("Zip Code should not be empty.")
                               .Length(5).WithMessage("Zip Code should be exactly 5 digits.");

        RuleFor(x => x.Phone).NotEmpty().WithMessage("Company Phone Number should not be empty.")
                             .Length(10).WithMessage("Phone Number should be exactly 10 digits and in the format (000) 000-0000.");

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