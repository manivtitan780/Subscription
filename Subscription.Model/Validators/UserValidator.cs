#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            ProfSvc_AppTrack
// Project:             ProfSvc_Classes
// File Name:           UserValidator.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja
// Created On:          07-05-2023 21:13
// Last Updated On:     10-15-2023 19:06
// *****************************************/

#endregion

#region Using

using System.Text.RegularExpressions;
// Added using statement for validation constants to eliminate magic strings
using Subscription.Model.Constants;

#endregion

namespace Subscription.Model.Validators;

/// <summary>
///     Represents a validator for the <see cref="User" /> class.
/// </summary>
/// <remarks>
///     This class extends the <see cref="AbstractValidator{T}" /> class, where T is a <see cref="User" />. It defines the
///     validation rules for the properties of a <see cref="User" /> object.
/// </remarks>
public partial class UserValidator : AbstractValidator<User>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="UserValidator" /> class.
    /// </summary>
    /// <remarks>
    ///     This constructor sets up validation rules for a <see cref="User" /> object.
    ///     It validates the following properties of the <see cref="User" /> object:
    ///     - FirstName: Should not be empty and should be less than 50 characters.
    ///     - LastName: Should not be empty and should be less than 50 characters.
    ///     - UserName: Should not be empty, should be between 3 and 50 characters, and should not already exist in the system
    ///     (only when adding a new user).
    ///     - Password: Should not be empty and should meet certain complexity requirements (only when adding a new user).
    ///     - EmailAddress: Should not be empty, should be less than 200 characters, and should be a valid email address.
    /// </remarks>
    public UserValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.FirstName).NotEmpty().WithMessage("First Name should not be empty.")
                                 .Length(1, 50).WithMessage("First Name should be less than {MaxLength} characters.");

        RuleFor(x => x.LastName).NotEmpty().WithMessage("Last Name should not be empty.")
                                .Length(1, 50).WithMessage("Last Name should be less than {MaxLength} characters.");

        When(x => x.IsAdd, () =>
                           {
                               RuleFor(x => x.UserName).NotEmpty().WithMessage("User Name should not be empty.")
                                                       .Length(3, 50).WithMessage("User Name should be between {MinLength} and {MaxLength} characters.");
                               //.Must(CheckUserNameExists).WithMessage("User Name already exists. Enter another User.");

                               RuleFor(x => x.Password).NotEmpty().WithMessage("Password should not be empty.")
                                                       .Must(CheckPassword)
                                                       .WithMessage(ValidationMessages.PasswordComplexity);
                           });

        // Using ValidationMessages constants to eliminate magic strings and improve maintainability
        RuleFor(x => x.EmailAddress).NotEmpty().WithMessage(ValidationMessages.FieldShouldNotBeEmpty("Email Address"))
                                    .Length(1, 200).WithMessage(ValidationMessages.FieldMaxLength("Email Address"))
                                    .Must(s => s.IsValidEmail()).WithMessage(ValidationMessages.ValidEmailRequired);
    }

    /// <summary>
    ///     Checks the validity of the provided password.
    /// </summary>
    /// <param name="password">The password to check.</param>
    /// <returns>Returns true if the password is valid, false otherwise.</returns>
    /// <remarks>
    ///     This method checks if the password meets the following criteria:
    ///     - Contains at least one lowercase letter.
    ///     - Contains at least one uppercase letter.
    ///     - Has a length of between 6 and 16 characters.
    ///     - Contains at least one numeric or special character.
    /// </remarks>
    private static bool CheckPassword(string password)
    {
        Regex _hasNumber = HasNumber();
        Regex _hasLowerCaseLetter = HasLowerCase();
        Regex _hasUpperCaseLetter = HasUpperCase();
        Regex _hasMinimum6Maximum16Chars = HasMin6Max16();
        Regex _hasSpecialChar = HasSpecialChar();
        return _hasLowerCaseLetter.IsMatch(password) && _hasUpperCaseLetter.IsMatch(password) && _hasMinimum6Maximum16Chars.IsMatch(password) &&
               (_hasNumber.IsMatch(password) || _hasSpecialChar.IsMatch(password));
    }

    [GeneratedRegex(@"[0-9]+")]
    private static partial Regex HasNumber();
    
    [GeneratedRegex(@"[a-z]+")]
    private static partial Regex HasLowerCase();
    
    [GeneratedRegex(@"[A-Z]+")]
    private static partial Regex HasUpperCase();
    
    [GeneratedRegex(@".{6,16}")]
    private static partial Regex HasMin6Max16();
    
    [GeneratedRegex(@"[!@#$%^&*(),.?"":{ }|<>]+")]
    private static partial Regex HasSpecialChar();

    /*/// <summary>
    ///     Checks if the provided username already exists in the system.
    /// </summary>
    /// <param name="userName">The username to check.</param>
    /// <returns>Returns true if the username does not exist, false otherwise.</returns>
    /// <remarks>
    ///     This method sends a GET request to the "Admin/CheckText" endpoint of the API,
    ///     passing the username as a parameter. The API is expected to return a boolean
    ///     indicating whether the username exists. This method inverts the response
    ///     (i.e., returns true if the API returns false and vice versa) because it is
    ///     used in a validation rule that requires a true result for validation to pass.
    /// </remarks>
    private static bool CheckUserNameExists(string userName)
    {
        RestClient _restClient = new(GeneralClass.ApiHost ?? "");
        RestRequest _request = new("Admin/CheckText")
                               {
                                   RequestFormat = DataFormat.Json
                               };
        _request.AddQueryParameter("id", 0);
        _request.AddQueryParameter("text", userName);
        _request.AddQueryParameter("entity", "User Name");
        bool _response = _restClient.GetAsync<bool>(_request).Result;

        return !_response;
    }*/
}