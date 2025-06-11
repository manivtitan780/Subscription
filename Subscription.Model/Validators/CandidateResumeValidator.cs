#region Header
// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.Model
// File Name:           CandidateResumeValidator.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu, Brijesh Dubey
// Created On:          06-11-2025 19:06
// Last Updated On:     06-11-2025 19:27
// *****************************************/
#endregion

namespace Subscription.Model.Validators;

public class CandidateResumeValidator: AbstractValidator<CandidateResume>
{
    public CandidateResumeValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.Files).NotEmpty().WithMessage("Select a file to upload.");
    }
}