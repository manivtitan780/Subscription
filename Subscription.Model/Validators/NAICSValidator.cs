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

using Subscription.Model.Constants;

namespace Subscription.Model.Validators;

public class NAICSValidator:AbstractValidator<NAICS>
{
    public NAICSValidator()
    {
        // Using ValidationMessages constants to eliminate magic strings and improve maintainability
        RuleFor(x => x.Title).NotEmpty().WithMessage(ValidationMessages.FieldRequired("NAICS Title"))
                             .MaximumLength(BusinessConstants.FieldLengths.Email).WithMessage(ValidationMessages.FieldMaxLength("NAICS Title"));
    }
}