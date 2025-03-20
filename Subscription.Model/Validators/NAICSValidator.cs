#region Header
// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.Model
// File Name:           NAICSValidartor.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu, Brijesh Dubey
// Created On:          03-20-2025 15:03
// Last Updated On:     03-20-2025 15:03
// *****************************************/
#endregion

namespace Subscription.Model.Validators;

public class NAICSValidator:AbstractValidator<NAICS>
{
    public NAICSValidator()
    {
        RuleFor(x => x.Title).NotEmpty().WithMessage("NAICS Title is required")
                             .MaximumLength(255).WithMessage("NAICS Title must be less than 255 characters");
    }
}