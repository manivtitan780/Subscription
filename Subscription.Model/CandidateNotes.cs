#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            ProfSvc_AppTrack
// Project:             ProfSvc_Classes
// File Name:           CandidateNotes.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja
// Created On:          12-09-2022 15:57
// Last Updated On:     10-26-2023 21:08
// *****************************************/

#endregion

namespace Subscription.Model;

/// <summary>
///     Represents a CandidateNotes class in the ProfSvc_Classes namespace.
/// </summary>
/// <remarks>
///     The CandidateNotes class contains properties and methods for managing notes related to a candidate.
///     This includes properties for the ID, notes, the user who last updated the notes, and the date and time of the last
///     update.
///     It also includes methods for initializing a new instance of the class, resetting the properties to their default
///     values, and creating a shallow copy of the current object.
/// </remarks>
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
public class CandidateNotes
{
	/// <summary>
	///     Initializes a new instance of the <see cref="CandidateNotes" /> class.
	/// </summary>
	/// <remarks>
	///     This constructor calls the Clear method to reset the properties of the CandidateNotes object to their default
	///     values.
	/// </remarks>
	public CandidateNotes()
	{
		Clear();
	}

	/// <summary>
	///     Initializes a new instance of the <see cref="CandidateNotes" /> class.
	/// </summary>
	/// <param name="id">The unique identifier of the candidate note.</param>
	/// <param name="updatedDate">The date and time when the candidate's notes were last updated.</param>
	/// <param name="updatedBy">The name of the user who last updated the candidate's notes.</param>
	/// <param name="notes">The notes for the candidate.</param>
	public CandidateNotes(int id, DateTime updatedDate, string? updatedBy, string? notes)
	{
		ID = id;
		UpdatedDate = updatedDate;
		UpdatedBy = updatedBy;
		Notes = notes;
	}

	/// <summary>
	///     Gets or sets the ID of the candidate note.
	/// </summary>
	/// <value>
	///     An integer representing the unique identifier of the candidate note.
	/// </value>
	/// <remarks>
	///     This property is used as the primary key for the CandidateNotes object.
	/// </remarks>
	public int ID
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets the notes for the candidate.
	/// </summary>
	/// <value>
	///     A string representing the notes for the candidate.
	/// </value>
	/// <remarks>
	///     This property is used to store any notes or comments about the candidate.
	/// </remarks>
	public string? Notes
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets the name of the user who last updated the candidate's notes.
	/// </summary>
	/// <value>
	///     A string representing the name of the user.
	/// </value>
	/// <remarks>
	///     This property is used to track the user who made the most recent modifications to the candidate's notes.
	/// </remarks>
	public string? UpdatedBy
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets the date and time when the candidate's notes were last updated.
	/// </summary>
	/// <value>
	///     A <see cref="DateTime" /> representing the date and time of the last update.
	/// </value>
	/// <remarks>
	///     This property is used to track the most recent modifications to the candidate's notes.
	/// </remarks>
	public DateTime UpdatedDate
	{
		get;
		set;
	}

	/// <summary>
	///     Resets the properties of the CandidateNotes object to their default values.
	/// </summary>
	/// <remarks>
	///     This method is used to clear the current state of the CandidateNotes object. It sets the ID to 0,
	///     the UpdatedDate to DateTime.MinValue, and the UpdatedBy and Notes to an empty string.
	/// </remarks>
	public void Clear()
	{
		ID = 0;
		UpdatedDate = DateTime.MinValue;
		UpdatedBy = "";
		Notes = "";
	}

	/// <summary>
	///     Creates a deep copy of the current CandidateNotes object.
	/// </summary>
	/// <returns>
	///     A new CandidateNotes object that is a deep copy of this instance.
	/// </returns>
	public CandidateNotes? Copy() => MemberwiseClone() as CandidateNotes;
}