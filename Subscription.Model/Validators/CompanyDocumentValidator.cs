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

// Added using statement for validation constants to eliminate magic strings
using Subscription.Model.Constants;

namespace Subscription.Model.Validators;

public class CompanyDocumentValidator : AbstractValidator<CompanyDocuments>
{
    public CompanyDocumentValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        // Using ValidationMessages constants to eliminate magic strings and improve maintainability
        RuleFor(x => x.DocumentName).NotEmpty().WithMessage(ValidationMessages.DocumentNameRequired)
                                    .Length(BusinessConstants.ValidationRanges.DocumentNameMinLength, BusinessConstants.FieldLengths.DocumentName).WithMessage(ValidationMessages.DocumentNameLength);

        // Using ValidationMessages constants to eliminate magic strings and improve maintainability
        RuleFor(x => x.Notes).NotEmpty().WithMessage(ValidationMessages.DocumentNotesRequired)
                             .Length(BusinessConstants.ValidationRanges.DocumentNotesMinLength, BusinessConstants.FieldLengths.Notes).WithMessage(ValidationMessages.DocumentNotesLength);

        // Using ValidationMessages constants to eliminate magic strings and improve maintainability
        RuleFor(x => x.Files).NotEmpty().WithMessage(ValidationMessages.FileSelectionRequired);
    }
}