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
                               RuleFor(x => x.KeyValue).NotEmpty().WithMessage("Job Option Code is required")
                                                   .Length(1).WithMessage("Job Option Code should be exactly {MaxLength} character.");
                               //.Must(CheckJobCodeExists).WithMessage("Job Option Code already exists. Enter another Job Option Code.");
                           });

        RuleFor(x => x.Description).MaximumLength(500).WithMessage("Job Option Description should be less than {MaxLength} characters.");

        RuleFor(x => x.Text).NotEmpty().WithMessage("Job Option should not be empty.")
                              .Length(2, 50).WithMessage("Job Option should be between {MinLength} and {MaxLength} characters.");
                              //.Must((obj, option) => CheckJobOptionExists(obj.Code, option)).WithMessage("Job Option already exists. Enter another Job Option.");

        RuleFor(x => x.PercentText).NotEmpty().WithMessage("Percent Text should not be empty.")
                                   .Length(2, 255).WithMessage("Percent Text should be between {MinLength} and {MaxLength} characters.");

        RuleFor(x => x.RateText).NotEmpty().WithMessage("Rate Text should not be empty.")
                                .Length(2, 255).WithMessage("Rate Text should be between {MinLength} and {MaxLength} characters.");

        RuleFor(x => x.Tax).MaximumLength(20).WithMessage("Job Option Description should be less than {MaxLength} characters.");
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
        RestClient _restClient = new(GeneralClass.ApiHost ?? string.Empty);
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
        RestClient _restClient = new(GeneralClass.ApiHost ?? string.Empty);
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