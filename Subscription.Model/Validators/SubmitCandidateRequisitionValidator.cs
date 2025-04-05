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

namespace Subscription.Model.Validators;

public class SubmitCandidateRequisitionValidator:AbstractValidator<SubmitCandidateRequisition>
{
    public SubmitCandidateRequisitionValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.Text).NotEmpty().WithMessage("Notes should not be empty")
                            .Length(5, 1000).WithMessage("Notes should be between 5 and 1000 characters.");
    }
}