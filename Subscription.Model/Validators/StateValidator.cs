#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            ProfSvc_AppTrack
// Project:             ProfSvc_Classes
// File Name:           StateValidator.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja
// Created On:          07-07-2023 20:10
// Last Updated On:     10-15-2023 20:03
// *****************************************/

#endregion

namespace Subscription.Model.Validators;

/// <summary>
///     Provides validation for the State class.
/// </summary>
/// <remarks>
///     The StateValidator class extends the AbstractValidator class and provides custom validation rules for the State
///     class.
///     It includes rules for validating the state code and state name, and also checks if a state with the same code or
///     name already exists.
///     The validation rules are defined in the constructor of the StateValidator class.
/// </remarks>
public class StateValidator : AbstractValidator<State>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="StateValidator" /> class.
    /// </summary>
    /// <remarks>
    ///     This constructor sets up the validation rules for the State class. It includes rules for validating the state code
    ///     and state name,
    ///     and also checks if a state with the same code or name already exists. The validation rules are defined in the
    ///     constructor of the StateValidator class.
    /// </remarks>
    public StateValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        When(x => x.ID == 0, () =>
                             {
                                 RuleFor(x => x.Code).NotEmpty().WithMessage("State Code should not be empty.")
                                                     .Length(2).WithMessage("State Code should be exactly {MaxLength} characters.");
                                 //.Must(CheckStateCodeExists).WithMessage("State Code already exists. Enter another State Code.");
                             });

        RuleFor(x => x.StateName).NotEmpty().WithMessage("State Name should not be empty.")
                                 .Length(2, 50).WithMessage("State Code should be between {MinLength} and {MaxLength} characters.");
                                 //.Must((obj, state) => CheckStateExists(obj.Code, state));
    }

    /*/// <summary>
    ///     Checks if a state code already exists in the system.
    /// </summary>
    /// <param name="stateCode">The state code to check.</param>
    /// <returns>Returns false if the state code already exists, otherwise returns true.</returns>
    /// <remarks>
    ///     This method uses a REST client to asynchronously send a GET request to the "Admin/CheckStateCode" endpoint of the
    ///     API.
    ///     The state code is passed as a query parameter in the request. The API is expected to return a boolean value
    ///     indicating
    ///     whether the state code exists or not. The method then negates this value before returning it, so that it returns
    ///     true
    ///     if the state code does not exist and false if it does.
    /// </remarks>
    private static bool CheckStateCodeExists(string stateCode)
    {
        RestClient _restClient = new(GeneralClass.ApiHost ?? "");
        RestRequest _request = new("Admin/CheckStateCode")
                               {
                                   RequestFormat = DataFormat.Json
                               };
        _request.AddQueryParameter("code", stateCode);
        bool _response = _restClient.GetAsync<bool>(_request).Result;

        return !_response;
    }

    /// <summary>
    ///     Checks if a state with the given code and name already exists.
    /// </summary>
    /// <param name="stateCode">The code of the state to check.</param>
    /// <param name="state">The name of the state to check.</param>
    /// <returns>
    ///     Returns false if a state with the given code and name already exists, otherwise true.
    /// </returns>
    /// <remarks>
    ///     This method makes an asynchronous GET request to the "Admin/CheckState" endpoint of the API,
    ///     passing the state code and name as query parameters. The API is expected to return a boolean
    ///     value indicating whether a state with the given code and name exists.
    /// </remarks>
    private static bool CheckStateExists(string stateCode, string state)
    {
        RestClient _restClient = new(GeneralClass.ApiHost ?? "");
        RestRequest _request = new("Admin/CheckState")
                               {
                                   RequestFormat = DataFormat.Json
                               };
        _request.AddQueryParameter("code", stateCode);
        _request.AddQueryParameter("text", state);
        bool _response = _restClient.GetAsync<bool>(_request).Result;

        return !_response;
    }*/
}