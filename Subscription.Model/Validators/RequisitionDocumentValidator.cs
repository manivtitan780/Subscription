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

using Subscription.Model.Constants;

namespace Subscription.Model.Validators;

public class RequisitionDocumentValidator : AbstractValidator<RequisitionDocuments>
{
    public RequisitionDocumentValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        // Using ValidationMessages constants to eliminate magic strings and improve maintainability
        RuleFor(x => x.Name).NotEmpty().WithMessage(ValidationMessages.DocumentNameRequired)
                            .Length(BusinessConstants.ValidationRanges.DocumentNameMinLength, BusinessConstants.FieldLengths.DocumentName).WithMessage(ValidationMessages.DocumentNameLength);

        // Using ValidationMessages constants to eliminate magic strings and improve maintainability
        RuleFor(x => x.Notes).NotEmpty().WithMessage(ValidationMessages.DocumentNotesRequired)
                             .Length(BusinessConstants.ValidationRanges.DocumentNotesMinLength, BusinessConstants.FieldLengths.Notes).WithMessage(ValidationMessages.DocumentNotesLength);

        // Using ValidationMessages constants to eliminate magic strings and improve maintainability
        RuleFor(x => x.Files).NotEmpty().WithMessage(ValidationMessages.FileSelectionRequired);
    }

}