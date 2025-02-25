#region Header
// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.Model
// File Name:           RequisitionDocumentValidator.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu, Brijesh Dubey
// Created On:          02-24-2025 19:02
// Last Updated On:     02-24-2025 19:02
// *****************************************/
#endregion

namespace Subscription.Model.Validators;

public class RequisitionDocumentValidator : AbstractValidator<RequisitionDocuments>
{
    public RequisitionDocumentValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.Name).NotEmpty().WithMessage("Document Name should not be empty.")
                            .Length(2, 255).WithMessage("Document should be between 2 and 255 characters.");

        RuleFor(x => x.Notes).NotEmpty().WithMessage("Document Notes should not be empty.")
                             .Length(10, 2000).WithMessage("Document Notes should be between 10 and 2000 characters.");

        RuleFor(x => x.Files).NotEmpty().WithMessage("Select a file to upload.");
    }

}