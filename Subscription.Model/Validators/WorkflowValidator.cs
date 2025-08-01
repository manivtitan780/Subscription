﻿#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            ProfSvc_AppTrack
// Project:             ProfSvc_Classes
// File Name:           AppWorkflowValidator.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja
// Created On:          07-06-2023 15:43
// Last Updated On:     10-14-2023 20:18
// *****************************************/

#endregion

using Subscription.Model.Constants;

namespace Subscription.Model.Validators;

/// <summary>
///     Represents a validator for the <see cref="Workflow" /> class.
/// </summary>
/// <remarks>
///     This class extends the <see cref="AbstractValidator{T}" /> class, where T is <see cref="Workflow" />.
///     It defines the validation rules for the properties of an <see cref="Workflow" /> instance.
/// </remarks>
public class WorkflowValidator : AbstractValidator<Workflow>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="WorkflowValidator" /> class.
    /// </summary>
    /// <remarks>
    ///     This constructor sets up the validation rules for the properties of an <see cref="Workflow" /> instance.
    ///     The rules include:
    ///     - The 'Next' property should have a length between 0 and 100 characters.
    ///     - The 'RoleIDs' property should not be empty and should have a length between 1 and 50 characters.
    ///     - The 'Step' property should have a length between 0 and 3 characters.
    /// </remarks>
    public WorkflowValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.Next).Length(0, 100).WithMessage("Next Step should be less than {MaxLength} characters.");

        // Using ValidationMessages constants to eliminate magic strings and improve maintainability
        RuleFor(x => x.RoleIDs).NotEmpty().WithMessage(ValidationMessages.FieldRequired("At least 1 Role ID"))
                               .Length(1, BusinessConstants.FieldLengths.Name).WithMessage(ValidationMessages.FieldMaxLength("Role IDs"));

        // RuleFor(x => x.Step).Length(0, 3).WithMessage("Step should be less than {MaxLength} characters.");
    }
}