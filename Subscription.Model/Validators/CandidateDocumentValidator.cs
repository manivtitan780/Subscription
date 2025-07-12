#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            ProfSvc_AppTrack
// Project:             ProfSvc_Classes
// File Name:           CandidateDocumentValidator.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja
// Created On:          07-11-2023 21:25
// Last Updated On:     10-14-2023 20:19
// *****************************************/

#endregion

// Added using statement for validation constants to eliminate magic strings
using Subscription.Model.Constants;

namespace Subscription.Model.Validators;

/// <summary>
///     Represents a validator for the <see cref="CandidateDocument" /> class.
/// </summary>
/// <remarks>
///     This class is responsible for validating the properties of a <see cref="CandidateDocument" /> instance.
///     It validates the Name, Notes, DocumentTypeID, and Files properties of a <see cref="CandidateDocument" />.
/// </remarks>
public class CandidateDocumentValidator : AbstractValidator<CandidateDocument>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="CandidateDocumentValidator" /> class.
    /// </summary>
    /// <remarks>
    ///     This constructor sets up the validation rules for a <see cref="CandidateDocument" /> instance.
    ///     It includes rules for the Name, Notes, DocumentTypeID, and Files properties.
    /// </remarks>
    public CandidateDocumentValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;
        
        // Using ValidationMessages constants to eliminate magic strings and improve maintainability
        RuleFor(x => x.Name).NotEmpty().WithMessage(ValidationMessages.DocumentNameRequired)
                            .Length(BusinessConstants.ValidationRanges.DocumentNameMinLength, BusinessConstants.FieldLengths.DocumentName).WithMessage(ValidationMessages.DocumentNameLength);

        // Using ValidationMessages constants to eliminate magic strings and improve maintainability
        RuleFor(x => x.Notes).NotEmpty().WithMessage(ValidationMessages.DocumentNotesRequired)
                             .Length(BusinessConstants.ValidationRanges.DocumentNotesMinLength, BusinessConstants.FieldLengths.Notes).WithMessage(ValidationMessages.DocumentNotesLength);

        // Using ValidationMessages constants to eliminate magic strings and improve maintainability
        RuleFor(x => x.DocumentTypeID).NotEmpty().WithMessage(ValidationMessages.FieldShouldNotBeEmpty("Document Type"));

        // Using ValidationMessages constants to eliminate magic strings and improve maintainability
        RuleFor(x => x.Files).NotEmpty().WithMessage(ValidationMessages.FileSelectionRequired);
    }
}