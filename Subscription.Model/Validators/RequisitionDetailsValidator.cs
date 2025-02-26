#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            ProfSvc_AppTrack
// Project:             ProfSvc_Classes
// File Name:           RequisitionDetailsValidator.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja
// Created On:          07-14-2023 19:54
// Last Updated On:     10-15-2023 19:22
// *****************************************/

#endregion

namespace Subscription.Model.Validators;

/// <summary>
///     Represents a validator for the <see cref="RequisitionDetails" /> class.
/// </summary>
/// <remarks>
///     This class provides validation rules for the properties of the <see cref="ProfSvc_Classes.RequisitionDetails" />
///     class.
///     It includes rules for validating the CompanyID, ZipCode, City, PositionTitle, Description, Mandatory, DueDate,
///     ExpRateHigh, SalaryHigh, and ExpLoadHigh properties.
/// </remarks>
public class RequisitionDetailsValidator : AbstractValidator<RequisitionDetails>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="RequisitionDetailsValidator" /> class.
    /// </summary>
    /// <remarks>
    ///     This constructor sets up the validation rules for the <see cref="RequisitionDetails" /> class.
    ///     It includes rules for validating the CompanyID, ZipCode, City, PositionTitle, Description, Mandatory, DueDate,
    ///     ExpRateHigh, SalaryHigh, and ExpLoadHigh properties.
    /// </remarks>
    public RequisitionDetailsValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.CompanyID).NotNull().WithMessage("Company is required")
                                 .GreaterThan(0).WithMessage("Company is required.");

        RuleFor(x => x.ZipCode).NotEmpty().WithMessage("Zip Code is required.")
                               .Length(5).WithMessage("Zip Code should be exactly {MaxLength} characters long.");

        RuleFor(x => x.City).NotEmpty().WithMessage("City is required.")
                            .Length(2, 50).WithMessage("City should be between {MinLength} and {MaxLength} characters long.");

        RuleFor(x => x.PositionTitle).NotEmpty().WithMessage("Position Title/Role is required.")
                                     .Length(3, 200).WithMessage("Position Title/Role should not be between {MinLength} and {MaxLength} characters long.");

        RuleFor(x => x.Description).NotEmpty().WithMessage("Position Description is required.");

        RuleFor(x => x.Mandatory).NotEmpty().WithMessage("Keywords is required.")
                                 .MinimumLength(5).WithMessage("Keywords should have more than {MinLength} characters.");

        RuleFor(x => x.DueDate).GreaterThan(x => x.ExpectedStart).WithMessage("Due Date should be later than Expected Start Date.");

        RuleFor(x => x.ExpRateHigh).GreaterThanOrEqualTo(x => x.ExpRateLow).WithMessage("Maximum Pay Rate should be higher than or equal to Minimum Pay Rate");

        RuleFor(x => x.SalaryHigh).GreaterThanOrEqualTo(x => x.SalaryLow).WithMessage("Maximum Salary should be higher than or equal to Minimum Salary");

        RuleFor(x => x.ExpLoadHigh).GreaterThanOrEqualTo(x => x.ExpLoadLow).WithMessage("Maximum Load Rate should be higher than or equal to Minimum Load Rate");
    }
}