#region Header
// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.Model
// File Name:           CompanyDocumentValidator.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu, Brijesh Dubey
// Created On:          04-26-2025 20:04
// Last Updated On:     04-26-2025 20:37
// *****************************************/
#endregion

namespace Subscription.Model.Validators;

public class CompanyDocumentValidator : AbstractValidator<CompanyDocuments>
{
    public CompanyDocumentValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.DocumentName).NotEmpty().WithMessage("Document Name should not be empty.")
                                    .Length(2, 255).WithMessage("Document should be between 2 and 255 characters.");

        RuleFor(x => x.Notes).NotEmpty().WithMessage("Document Notes should not be empty.")
                             .Length(10, 2000).WithMessage("Document Notes should be between 10 and 2000 characters.");

        RuleFor(x => x.Files).NotEmpty().WithMessage("Select a file to upload.");
    }
}