#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            ProfSvc_AppTrack
// Project:             ProfSvc_Classes
// File Name:           JobOptionValidator.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja
// Created On:          07-06-2023 20:45
// Last Updated On:     10-15-2023 19:04
// *****************************************/

#endregion

using Subscription.Model.Constants;

namespace Subscription.Model.Validators;

/// <summary>
///     Represents a validator for the <see cref="JobOptions" /> class.
/// </summary>
/// <remarks>
///     This class is responsible for validating the properties of a <see cref="JobOptions" /> instance.
///     It extends the <see cref="AbstractValidator{T}" /> class, where T is a <see cref="JobOptions" />.
///     The validation rules are defined in the constructor of this class.
/// </remarks>
public class JobOptionsValidator : AbstractValidator<JobOptions>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="JobOptionsValidator" /> class.
    /// </summary>
    /// <remarks>
    ///     This constructor sets up the validation rules for the <see cref="JobOptions" /> class.
    ///     The rules are defined for each property of the <see cref="JobOptions" /> class.
    ///     If a rule is violated, a corresponding message is returned.
    /// </remarks>
    public JobOptionsValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        When(x => x.IsAdd, () =>
                           {
                               // Using ValidationMessages constants to eliminate magic strings and improve maintainability
                               RuleFor(x => x.KeyValue).NotEmpty().WithMessage(ValidationMessages.FieldRequired("Job Option Code"))
                                                   .Length(1).WithMessage("Job Option Code should be exactly {MaxLength} character.");
                               //.Must(CheckJobCodeExists).WithMessage("Job Option Code already exists. Enter another Job Option Code.");
                           });

        RuleFor(x => x.Description).MaximumLength(500).WithMessage("Job Option Description should be less than {MaxLength} characters.");

        // Using ValidationMessages constants to eliminate magic strings and improve maintainability
        RuleFor(x => x.Text).NotEmpty().WithMessage(ValidationMessages.FieldShouldNotBeEmpty("Job Option"))
                              .Length(2, BusinessConstants.FieldLengths.Name).WithMessage(ValidationMessages.FieldBetweenLength("Job Option"));
                              //.Must((obj, option) => CheckJobOptionExists(obj.Code, option)).WithMessage("Job Option already exists. Enter another Job Option.");

        // Using ValidationMessages constants to eliminate magic strings and improve maintainability
        RuleFor(x => x.PercentText).NotEmpty().WithMessage(ValidationMessages.FieldShouldNotBeEmpty("Percent Text"))
                                   .Length(2, BusinessConstants.FieldLengths.Email).WithMessage(ValidationMessages.FieldBetweenLength("Percent Text"));

        // Using ValidationMessages constants to eliminate magic strings and improve maintainability
        RuleFor(x => x.RateText).NotEmpty().WithMessage(ValidationMessages.FieldShouldNotBeEmpty("Rate Text"))
                                .Length(2, BusinessConstants.FieldLengths.Email).WithMessage(ValidationMessages.FieldBetweenLength("Rate Text"));

        RuleFor(x => x.Tax).MaximumLength(20).WithMessage("Tax Terms should be less than {MaxLength} characters.");
    }

    /*/// <summary>
    ///     Checks if the provided job code already exists.
    /// </summary>
    /// <param name="jobCode">The job code to check.</param>
    /// <returns>Returns true if the job code does not exist; otherwise, false.</returns>
    /// <remarks>
    ///     This method makes a call to the "Admin/CheckJobCode" endpoint of the API, passing the job code as a query
    ///     parameter.
    ///     The API is expected to return a boolean value indicating whether the job code exists.
    /// </remarks>
    private static bool CheckJobCodeExists(string jobCode)
    {
        RestClient _restClient = new(GeneralClass.ApiHost ?? "");
        RestRequest _request = new("Admin/CheckJobCode")
                               {
                                   RequestFormat = DataFormat.Json
                               };
        _request.AddQueryParameter("id", jobCode);
        bool _response = _restClient.GetAsync<bool>(_request).Result;

        return !_response;
    }

    /// <summary>
    ///     Checks if a job option already exists.
    /// </summary>
    /// <param name="code">The code of the job option.</param>
    /// <param name="jobOption">The job option to check.</param>
    /// <returns>Returns false if the job option already exists, otherwise true.</returns>
    private static bool CheckJobOptionExists(string code, string jobOption)
    {
        RestClient _restClient = new(GeneralClass.ApiHost ?? "");
        RestRequest _request = new("Admin/CheckJobOption")
                               {
                                   RequestFormat = DataFormat.Json
                               };
        _request.AddQueryParameter("code", code);
        _request.AddQueryParameter("text", jobOption);
        bool _response = _restClient.GetAsync<bool>(_request).Result;

        return !_response;
    }*/
}