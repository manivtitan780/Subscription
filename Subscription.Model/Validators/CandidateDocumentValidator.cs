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
        
        RuleFor(x => x.Name).NotEmpty().WithMessage("Document Name should not be empty.")
                            .Length(2, 255).WithMessage("Document should be between 2 and 255 characters.");

        RuleFor(x => x.Notes).NotEmpty().WithMessage("Document Notes should not be empty.")
                             .Length(10, 2000).WithMessage("Document Notes should be between 10 and 2000 characters.");

        RuleFor(x => x.DocumentTypeID).NotEmpty().WithMessage("Document Type should not be empty.");

        RuleFor(x => x.Files).NotEmpty().WithMessage("Select a file to upload.");
    }
}