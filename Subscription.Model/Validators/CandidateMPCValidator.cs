#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.Model
// File Name:           CandidateMPCValidator.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu
// Created On:          01-01-2025 20:01
// Last Updated On:     01-01-2025 20:01
// *****************************************/

#endregion

namespace Subscription.Model.Validators;

public class CandidateMPCValidator : AbstractValidator<CandidateRatingMPC>
{
    public CandidateMPCValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.MPCComments).NotEmpty().WithMessage("Comments is required.")
                                .MaximumLength(255).WithMessage("Comments should not be more than 255 characters.");
    }
}