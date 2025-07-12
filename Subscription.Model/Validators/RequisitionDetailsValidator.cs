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

using Subscription.Model.Constants;

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

        // Using ValidationMessages constants to eliminate magic strings and improve maintainability
        RuleFor(x => x.CompanyID).NotNull().WithMessage(ValidationMessages.FieldRequired("Company"))
                                 .GreaterThan(0).WithMessage(ValidationMessages.FieldRequired("Company"));

        // Using ValidationMessages constants to eliminate magic strings and improve maintainability
        RuleFor(x => x.ZipCode).NotEmpty().WithMessage(ValidationMessages.FieldRequired("Zip Code"))
                               .Length(BusinessConstants.FieldLengths.State).WithMessage(ValidationPatterns.ZipCodeBasicMessage);

        // Using ValidationMessages constants to eliminate magic strings and improve maintainability
        RuleFor(x => x.City).NotEmpty().WithMessage(ValidationMessages.FieldRequired("City"))
                            .Length(2, BusinessConstants.FieldLengths.City).WithMessage(ValidationMessages.FieldBetweenLength("City"));

        // Using ValidationMessages constants to eliminate magic strings and improve maintainability
        RuleFor(x => x.PositionTitle).NotEmpty().WithMessage(ValidationMessages.FieldRequired("Position Title/Role"))
                                     .Length(3, BusinessConstants.FieldLengths.Title).WithMessage(ValidationMessages.FieldBetweenLength("Position Title/Role"));

        // Using ValidationMessages constants to eliminate magic strings and improve maintainability
        RuleFor(x => x.Description).NotEmpty().WithMessage(ValidationMessages.FieldRequired("Position Description"));

        // Using ValidationMessages constants to eliminate magic strings and improve maintainability
        RuleFor(x => x.Mandatory).NotEmpty().WithMessage(ValidationMessages.FieldRequired("Keywords"))
                                 .MinimumLength(5).WithMessage(ValidationMessages.FieldMinLength("Keywords"));

        RuleFor(x => x.DueDate).GreaterThan(x => x.ExpectedStart).WithMessage("Due Date should be later than Expected Start Date.");

        RuleFor(x => x.ExpRateHigh).GreaterThanOrEqualTo(x => x.ExpRateLow).WithMessage("Maximum Pay Rate should be higher than or equal to Minimum Pay Rate");

        RuleFor(x => x.SalaryHigh).GreaterThanOrEqualTo(x => x.SalaryLow).WithMessage("Maximum Salary should be higher than or equal to Minimum Salary");

        RuleFor(x => x.ExpLoadHigh).GreaterThanOrEqualTo(x => x.ExpLoadLow).WithMessage("Maximum Load Rate should be higher than or equal to Minimum Load Rate");
    }
}