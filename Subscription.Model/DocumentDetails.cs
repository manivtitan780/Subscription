#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            ProfSvc_AppTrack
// Project:             ProfSvc_Classes
// File Name:           DocumentDetails.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja
// Created On:          02-23-2023 20:29
// Last Updated On:     10-26-2023 21:17
// *****************************************/

#endregion

namespace Subscription.Model;

/// <summary>
///     Represents a document in the Professional Services application.
/// </summary>
/// <remarks>
///     This class is used to store and manage details about a document, such as its name, location, associated entity, and
///     internal file name.
///     It is used in various parts of the application, including the CandidatesController, CompanyController,
///     LeadController, RequisitionController, Candidate, DocumentsCompanyPanel, DocumentsPanel, and DownloadsPanel
///     classes.
/// </remarks>
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
public class DocumentDetails
{
	/// <summary>
	///     Initializes a new instance of the <see cref="DocumentDetails" /> class and resets its properties to their default
	///     values.
	/// </summary>
	public DocumentDetails()
	{
		Clear();
	}

	/// <summary>
	///     Initializes a new instance of the <see cref="DocumentDetails" /> class.
	/// </summary>
	/// <param name="entityID">The identifier of the entity associated with the document.</param>
	/// <param name="documentName">The name of the document.</param>
	/// <param name="documentLocation">The location of the document.</param>
	/// <param name="internalFileName">The internal file name of the document.</param>
	/// <remarks>
	///     This constructor is used to create a new instance of the DocumentDetails class with the provided parameters.
	///     It is used in various parts of the application, including the CandidatesController, CompanyController,
	///     LeadController, and RequisitionController classes.
	/// </remarks>
	public DocumentDetails(int entityID, string? documentName, string? documentLocation, string? internalFileName)
	{
		EntityID = entityID;
		DocumentName = documentName;
		DocumentLocation = documentLocation;
		InternalFileName = internalFileName;
	}

	/// <summary>
	///     Gets or sets the location of the document.
	/// </summary>
	/// <value>
	///     The location of the document.
	/// </value>
	/// <remarks>
	///     This property is used to store the location of the document. It is used in various parts of the application,
	///     including the Candidate, DocumentsCompanyPanel, and DocumentsPanel classes.
	/// </remarks>
	public string? DocumentLocation
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets the name of the document.
	/// </summary>
	/// <value>
	///     The name of the document.
	/// </value>
	/// <remarks>
	///     This property is used to store the name of the document. It is used in various parts of the application, including
	///     the DocumentsCompanyPanel, DocumentsPanel, and DownloadsPanel classes.
	/// </remarks>
	public string? DocumentName
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets the identifier of the entity associated with the document.
	/// </summary>
	/// <value>
	///     The identifier of the entity.
	/// </value>
	/// <remarks>
	///     This property is used to store the identifier of the entity (such as a company, requisition, or candidate) that the
	///     document is associated with. It is used in various parts of the application, including the DocumentsCompanyPanel,
	///     DocumentsPanel, and DownloadsPanel classes.
	/// </remarks>
	public int EntityID
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets the internal file name of the document.
	/// </summary>
	/// <value>
	///     The internal file name of the document.
	/// </value>
	/// <remarks>
	///     This property is used to store the internal file name of the document, which may differ from the original file
	///     name. It is used in various parts of the application, including the Candidate, DocumentsCompanyPanel,
	///     DocumentsPanel, and DownloadsPanel classes.
	/// </remarks>
	public string? InternalFileName
	{
		get;
		set;
	}

	/// <summary>
	///     Resets the properties of the DocumentDetails instance to their default values.
	/// </summary>
	public void Clear()
	{
		EntityID = 0;
		DocumentName = string.Empty;
		DocumentLocation = string.Empty;
		InternalFileName = string.Empty;
	}

	/// <summary>
	///     Creates a shallow copy of the current DocumentDetails object.
	/// </summary>
	/// <returns>
	///     A new DocumentDetails object copied from the current instance.
	/// </returns>
	public DocumentDetails? Copy() => MemberwiseClone() as DocumentDetails;
}