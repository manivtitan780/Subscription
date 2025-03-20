#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.Model
// File Name:           RoleValidator.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu, Brijesh Dubey
// Created On:          03-20-2025 20:03
// Last Updated On:     03-20-2025 20:03
// *****************************************/

#endregion

namespace Subscription.Model.Validators;

/// <summary>
///     Provides validation for the <see cref="Role" /> class.
/// </summary>
/// <remarks>
///     This class extends the <see cref="AbstractValidator{T}" /> class, where T is <see cref="Role" />.
///     It defines the validation rules for the properties of the <see cref="Role" /> class.
/// </remarks>
public class RoleValidator : AbstractValidator<Role>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="RoleValidator" /> class.
    /// </summary>
    /// <remarks>
    ///     This constructor sets up the validation rules for the Role object. It includes rules for the Description, ID, and
    ///     RoleName properties.
    ///     The Description must not be empty and must be less than 200 characters.
    ///     The ID must not be empty, must be exactly 2 characters, and must not already exist in the system when adding a new
    ///     role.
    ///     The RoleName must not be empty, must be between 2 and 50 characters, and must not already exist in the system for
    ///     the given ID.
    /// </remarks>
    public RoleValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.Description).NotEmpty().WithMessage("Role Description cannot be empty.")
                                   .MaximumLength(255).WithMessage("Role Description should be less than {MaxLength} characters.");

        /*When(x => x.IsAdd, () =>
                           {
                               RuleFor(x => x.ID).NotEmpty().WithMessage("Role ID cannot be empty.")
                                                 .Length(2).WithMessage("Role ID should be exactly {MaxLength} characters.")
                                                 .Must(CheckRoleIDExists).WithMessage("Role ID already exists. Enter another Role ID.");
                           });*/

        RuleFor(x => x.RoleName).NotEmpty().WithMessage("Role Name cannot be empty.")
                                .Length(2, 10).WithMessage("Role Name should be between {MinLength} and {MaxLength} characters.");
        //.Must((obj, role) => CheckRoleExists(obj.ID, role)).WithMessage("Role Name already exists. Enter another Role Name.");
    }

    /*/// <summary>
    ///     Checks whether a role with the given ID and name already exists in the system.
    /// </summary>
    /// <param name="roleID">The ID of the role to check.</param>
    /// <param name="role">The name of the role to check.</param>
    /// <returns>Returns true if the role does not exist, false otherwise.</returns>
    private static bool CheckRoleExists(string roleID, string role)
    {
        RestClient _restClient = new(GeneralClass.ApiHost ?? string.Empty);
        RestRequest _request = new("Admin/CheckRole")
                               {
                                   RequestFormat = DataFormat.Json
                               };
        _request.AddQueryParameter("id", roleID);
        _request.AddQueryParameter("text", role);
        bool _response = _restClient.GetAsync<bool>(_request).Result;

        return !_response;
    }

    /// <summary>
    ///     Checks whether a Role ID already exists in the system.
    /// </summary>
    /// <param name="roleID">The ID of the Role to check.</param>
    /// <returns>Returns true if the Role ID does not exist, false otherwise.</returns>
    private static bool CheckRoleIDExists(string roleID)
    {
        RestClient _restClient = new(GeneralClass.ApiHost ?? string.Empty);
        RestRequest _request = new("Admin/CheckRoleID")
                               {
                                   RequestFormat = DataFormat.Json
                               };
        _request.AddQueryParameter("id", roleID);
        bool _response = _restClient.GetAsync<bool>(_request).Result;

        return !_response;
    }*/
}