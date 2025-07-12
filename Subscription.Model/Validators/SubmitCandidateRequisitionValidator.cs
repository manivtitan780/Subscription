#region Header
// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.Model
// File Name:           SubmitCandidateRequisitionValidator.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu, Brijesh Dubey
// Created On:          04-05-2025 16:04
// Last Updated On:     04-05-2025 16:04
// *****************************************/
#endregion

using Subscription.Model.Constants;

namespace Subscription.Model.Validators;

public class SubmitCandidateRequisitionValidator:AbstractValidator<SubmitCandidateRequisition>
{
    public SubmitCandidateRequisitionValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        // Using ValidationMessages constants to eliminate magic strings and improve maintainability
        RuleFor(x => x.Text).NotEmpty().WithMessage(ValidationMessages.FieldShouldNotBeEmpty("Notes"))
                            .Length(5, 1000).WithMessage(ValidationMessages.FieldBetweenLength("Notes"));
    }
}