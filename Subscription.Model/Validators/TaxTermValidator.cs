#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.Model
// File Name:           TaxTermValidator.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu, Brijesh Dubey
// Created On:          03-21-2025 20:03
// Last Updated On:     03-21-2025 20:03
// *****************************************/

#endregion

namespace Subscription.Model.Validators;

public class TaxTermValidator : AbstractValidator<AdminList>
{
    public TaxTermValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        When(x => x is {IsAdd: true}, () =>
                                                          {
                                                              RuleFor(x => x.Code).Length(1).WithMessage(x => $"{x.Entity} Code should be 1 character in length.");
                                                              //.Must(CheckCodeExists).WithMessage(x => $"{x.Entity} Code already exists. Enter another code.");
                                                          });

        RuleFor(x => x.Text).NotEmpty().WithMessage(x => $"{x.Entity} should not be empty.")
                            .Length(2, 100).WithMessage(x => $"{x.Entity} should be between {{MinLength}} and {{MaxLength}} characters.");
        //.Must((obj, text) => CheckTextExists(text, obj.ID, obj.Entity, obj.Code)).WithMessage(x => $"{x.Entity} already exists. Enter another {x.Entity}");
    }

    /*/// <summary>
    ///     Checks if the provided code already exists in the system.
    /// </summary>
    /// <param name="code">
    ///     The code to check for existence.
    /// </param>
    /// <returns>
    ///     Returns true if the code does not exist, false otherwise.
    /// </returns>
    /// <remarks>
    ///     This method sends an asynchronous GET request to the "Admin/CheckTaxTermCode" endpoint of the API,
    ///     passing the provided code as a query parameter. The API is expected to return a boolean value indicating
    ///     whether the code exists. This method inverts the response from the API, so it returns true if the code
    ///     does not exist (i.e., the API returned false), and false if the code does exist (i.e., the API returned true).
    /// </remarks>
    private static bool CheckCodeExists(string code)
    {
        RestClient _restClient = new(GeneralClass.ApiHost ?? "");
        RestRequest _request = new("Admin/CheckTaxTermCode")
                               {
                                   RequestFormat = DataFormat.Json
                               };
        _request.AddQueryParameter("code", code);
        bool _response = _restClient.GetAsync<bool>(_request).Result;

        return !_response;
    }

    /// <summary>
    ///     Checks if the provided text already exists for the specified entity in the system.
    /// </summary>
    /// <param name="text">The text to check for existence.</param>
    /// <param name="id">The identifier of the entity.</param>
    /// <param name="entity">The type of the entity.</param>
    /// <param name="code">The code of the entity when the primary key is a string.</param>
    /// <returns>
    ///     Returns true if the text does not exist, false otherwise.
    /// </returns>
    /// <remarks>
    ///     This method makes a REST API call to the "Admin/CheckText" endpoint, passing the id, text, and entity as query
    ///     parameters.
    ///     The API is expected to return a boolean value indicating whether the text already exists for the specified entity.
    /// </remarks>
    private static bool CheckTextExists(string text, int id, string entity, string code)
    {
        RestClient _restClient = new(GeneralClass.ApiHost ?? "");
        RestRequest _request = new("Admin/CheckText")
                               {
                                   RequestFormat = DataFormat.Json
                               };
        _request.AddQueryParameter("id", id);
        _request.AddQueryParameter("text", text);
        _request.AddQueryParameter("entity", entity);
        _request.AddQueryParameter("code", code);
        bool _response = _restClient.GetAsync<bool>(_request).Result;

        return !_response;
    }*/
}