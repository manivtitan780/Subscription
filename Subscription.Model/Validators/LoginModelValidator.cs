#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.Model
// File Name:           LoginModelValidator.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu
// Created On:          4-21-2024 21:4
// Last Updated On:     4-21-2024 21:6
// *****************************************/

#endregion

namespace Subscription.Model.Validators;

public class LoginModelValidator : AbstractValidator<LoginModel>
{
    public LoginModelValidator()
    {
        RuleFor(x => x.UserName).NotEmpty().WithMessage("Username should not be empty.")
                                .MaximumLength(10).WithMessage("Username should be maximum 10 characters.");
        RuleFor(x => x.Password).NotEmpty().WithMessage("Password should not be empty.")
                                .MaximumLength(16).WithMessage("Password should be maximum 16 characters.");
    }
}