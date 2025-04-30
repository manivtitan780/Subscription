#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.Model
// File Name:           CandidateDocument.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu
// Created On:          05-10-2024 21:05
// Last Updated On:     12-18-2024 21:12
// *****************************************/

#endregion

namespace Subscription.Model;

/// <summary>
///     Represents a candidate document with various properties such as name, location, notes, and document type.
/// </summary>
public class CandidateDocument
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="CandidateDocument" /> class.
    /// </summary>
    /// <remarks>
    ///     This constructor calls the Clear method to reset the properties of the instance to their default values.
    /// </remarks>
    public CandidateDocument()
    {
        Clear();
    }

    /*/// <summary>
    ///     Initializes a new instance of the <see cref="CandidateDocument" /> class with specified parameters.
    /// </summary>
    /// <param name="id">The unique identifier for the candidate document.</param>
    /// <param name="name">The name of the candidate document.</param>
    /// <param name="location">The location of the candidate document.</param>
    /// <param name="notes">Any notes associated with the candidate document.</param>
    /// <param name="updatedBy">The identifier of the user who last updated the document.</param>
    /// <param name="documentType">The type of the candidate document.</param>
    /// <param name="internalFileName">The internal file name of the candidate document.</param>
    /// <param name="documentTypeID">The identifier of the document type.</param>
    /// <remarks>
    ///     This constructor initializes the properties of the instance with the provided parameters. If the Files property is
    ///     not null, it is cleared; otherwise, a new list is instantiated.
    /// </remarks>
    public CandidateDocument(int id, string name, string location, string notes, string updatedBy, string documentType, string internalFileName, int documentTypeID)
    {
        ID = id;
        Name = name;
        Location = location;
        Notes = notes;
        UpdatedBy = updatedBy;
        DocumentType = documentType;
        InternalFileName = internalFileName;
        DocumentTypeID = documentTypeID;
        if (Files != null)
        {
            Files.Clear();
        }
        else
        {
            Files = new();
        }
    }*/

    /// <summary>
    ///     Gets or sets the type of the candidate document.
    /// </summary>
    /// <value>
    ///     The type of the candidate document.
    /// </value>
    /// <remarks>
    ///     This property is used to specify the type of the document. The actual meaning of specific types is determined by
    ///     the document type definitions in the system.
    /// </remarks>
    public string DocumentType
    {
        get;
        set;
    }

    /// <summary>
    ///     Gets or sets the identifier for the type of the candidate document.
    /// </summary>
    /// <value>
    ///     The identifier for the document type.
    /// </value>
    /// <remarks>
    ///     This property is used to categorize the document. The actual meaning of specific identifiers is determined by the
    ///     document type definitions in the system.
    /// </remarks>
    public int DocumentTypeID
    {
        get;
        set;
    }

    /// <summary>
    ///     Gets or sets the list of file names associated with the candidate document.
    /// </summary>
    /// <value>
    ///     The list of file names associated with the candidate document.
    /// </value>
    /// <remarks>
    ///     This property is used to manage the file names of the documents associated with a candidate.
    ///     It is used in the document upload process where a file name is added to the list when a file is selected,
    ///     and the list is cleared when a file's removed from the upload queue.
    /// </remarks>
    public List<string> Files
    {
        get;
        set;
    }

    /// <summary>
    ///     Gets or sets the unique identifier for the candidate document.
    /// </summary>
    /// <value>
    ///     The unique identifier for the candidate document.
    /// </value>
    public int ID
    {
        get;
        set;
    }

    /// <summary>
    ///     Gets or sets the internal file name of the candidate document.
    /// </summary>
    /// <value>
    ///     The internal file name of the candidate document.
    /// </value>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public string InternalFileName
    {
        get;
        set;
    }

    /// <summary>
    ///     Gets or sets the location of the candidate document.
    /// </summary>
    /// <value>
    ///     The location of the candidate document.
    /// </value>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public string Location
    {
        get;
        set;
    }

    /// <summary>
    ///     Gets or sets the name of the candidate document.
    /// </summary>
    /// <value>
    ///     The name of the candidate document.
    /// </value>
    public string Name
    {
        get;
        set;
    }

    /// <summary>
    ///     Gets or sets the notes for the candidate document.
    /// </summary>
    /// <value>
    ///     The notes for the candidate document.
    /// </value>
    public string Notes
    {
        get;
        set;
    }

    /// <summary>
    ///     Gets or sets the identifier of the user who last updated the document.
    /// </summary>
    /// <value>
    ///     The identifier of the user who last updated the document.
    /// </value>
    public string UpdatedBy
    {
        get;
        set;
    }
    
    /// <summary>
    ///     Resets the properties of the CandidateDocument instance to their default values.
    /// </summary>
    /// <remarks>
    ///     This method is used to clear the data of the current CandidateDocument instance. It sets the ID to 0, Name,
    ///     Location, Notes, UpdatedBy, and DocumentType to empty strings. The DocumentTypeID is set to 5, and the
    ///     InternalFileName is set to a new GUID. If the Files list is not null, it is cleared; otherwise, a new list is
    ///     instantiated.
    /// </remarks>
    public void Clear()
    {
        ID = 0;
        Name = "";
        Location = "";
        Notes = "";
        UpdatedBy = "";
        DocumentType = "Others";
        InternalFileName = Guid.Empty.ToString("N");
        DocumentTypeID = 5;
        Files ??= [];
        Files.Clear();
    }

    /// <summary>
    ///     Creates a deep copy of the current CandidateDocument object.
    /// </summary>
    /// <returns>
    ///     A new CandidateDocument object that is a deep copy of this instance.
    /// </returns>
    public CandidateDocument Copy() => MemberwiseClone() as CandidateDocument;
}