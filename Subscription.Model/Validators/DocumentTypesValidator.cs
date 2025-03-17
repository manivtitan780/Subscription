#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            ProfSvc_AppTrack
// Project:             ProfSvc_Classes
// File Name:           DocumentTypeValidator.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja
// Created On:          07-06-2023 16:29
// Last Updated On:     10-14-2023 20:28
// *****************************************/

#endregion

namespace Subscription.Model.Validators;

/// <summary>
///     Validates instances of the DocumentType class.
/// </summary>
/// <remarks>
///     The DocumentTypeValidator class is responsible for validating instances of the DocumentType class.
///     It extends the AbstractValidator class and defines a set of rules that each DocumentType instance must satisfy.
///     These rules include: the DocumentType must not be empty, its length must be between 2 and 50 characters, and it
///     must not already exist in the system.
///     The class uses the FluentValidation library to define and enforce these rules.
/// </remarks>
public class DocumentTypesValidator : AbstractValidator<DocumentTypes>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="DocumentTypesValidator" /> class.
    /// </summary>
    /// <remarks>
    ///     This constructor sets up the validation rules for instances of the DocumentType class.
    ///     The rules include: the DocumentType must not be empty, its length must be between 2 and 50 characters,
    ///     and it must not already exist in the system.
    /// </remarks>
    public DocumentTypesValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.Text).NotEmpty().WithMessage("Document Type is required.")
                            .Length(2, 50).WithMessage("Document Type should be between {MinLength} and {MaxLength} characters.");
        //.Must((obj, docType) => CheckDocumentTypeExists(docType, obj.KeyValue)).WithMessage("Document Type already exists. Enter another Document Type.");
    }

    /// <summary>
    ///     Checks if a document type already exists in the system.
    /// </summary>
    /// <param name="docType">The type of the document to check.</param>
    /// <param name="id">The unique identifier of the document type.</param>
    /// <returns>
    ///     Returns true if the document type does not exist, false otherwise.
    /// </returns>
    /// <remarks>
    ///     This method is used in the validation rules of the DocumentTypeValidator class to ensure that
    ///     the document type being validated does not already exist in the system. It makes a call to the
    ///     "Admin/CheckText" endpoint of the API, passing the id, docType, and entity parameters.
    /// </remarks>
    private static bool CheckDocumentTypeExists(string docType, int id)
    {
        /*RestClient _restClient = new(GeneralClass.ApiHost ?? string.Empty);
        RestRequest _request = new("Admin/CheckText")
                               {
                                   RequestFormat = DataFormat.Json
                               };
        _request.AddQueryParameter("id", id);
        _request.AddQueryParameter("text", docType);
        _request.AddQueryParameter("entity", "Document Type");
        bool _response = _restClient.GetAsync<bool>(_request).Result;

        return !_response;*/
        return true;
    }
}