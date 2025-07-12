#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.Model
// File Name:           LoginModelValidator.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu
// Created On:          04-21-2024 21:04
// Last Updated On:     10-29-2024 15:10
// *****************************************/

#endregion

using Subscription.Model.Constants;

namespace Subscription.Model.Validators;

public class LoginModelValidator : AbstractValidator<LoginModel>
{
    public LoginModelValidator()
    {
        // Using ValidationMessages constants to eliminate magic strings and improve maintainability
        RuleFor(x => x.UserName).NotEmpty().WithMessage(ValidationMessages.FieldShouldNotBeEmpty("Username"))
                                .MaximumLength(10).WithMessage(ValidationMessages.FieldMaxLength("Username"));
        // Using ValidationMessages constants to eliminate magic strings and improve maintainability
        RuleFor(x => x.Password).NotEmpty().WithMessage(ValidationMessages.FieldShouldNotBeEmpty("Password"))
                                .MaximumLength(BusinessConstants.ValidationRanges.PasswordMaxLength).WithMessage(ValidationMessages.FieldMaxLength("Password"));
    }
}