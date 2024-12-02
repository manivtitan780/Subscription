#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.Model
// File Name:           EditEducationDialogValidator.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu
// Created On:          12-02-2024 19:12
// Last Updated On:     12-02-2024 19:12
// *****************************************/

#endregion

namespace Subscription.Model.Validators;

public class EditEducationDialogValidator : AbstractValidator<CandidateEducation>
{
    public EditEducationDialogValidator()
    {
        RuleFor(x => x.College).NotEmpty().WithMessage("School/College/Institution is required.");
        RuleFor(x => x.Degree).NotEmpty().WithMessage("Degree/Diploma/Course Name is required.");
    }
}