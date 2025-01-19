#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            ProfSvc_AppTrack
// Project:             ProfSvc_Classes
// File Name:           CandidateEducation.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja
// Created On:          09-17-2022 20:01
// Last Updated On:     10-26-2023 21:07
// *****************************************/

#endregion

namespace Subscription.Model;

/// <summary>
///     Represents a candidate's education information.
/// </summary>
/// <remarks>
///     This class is used to store and manage the education details of a candidate.
///     It includes properties such as the name of the college or institution, the degree or course name,
///     the country and state of the institution, the year of graduation, and the user who last updated the record.
/// </remarks>
public class CandidateEducation
{
	/// <summary>
	///     Initializes a new instance of the <see cref="CandidateEducation" /> class and resets its properties to their
	///     default values.
	/// </summary>
	public CandidateEducation()
	{
		Clear();
	}

	/// <summary>
	///     Initializes a new insta///
	///     <summary>
	///         Initializes a new instance of the <see cref="CandidateEducation" /> class.
	///     </summary>
	///     <param name="id">The unique identifier for the CandidateEducation instance.</param>
	///     <param name="degree">The degree, diploma, or course name of the candidate's education.</param>
	///     <param name="college">The name of the college or institution where the candidate received their education.</param>
	///     <param name="state">The state of the educational institution where the candidate received their education.</param>
	///     <param name="country">The country of the candidate's education institution.</param>
	///     <param name="year">The year of graduation.</param>
	///     <param name="updatedBy">The username of the person who last updated the education record.</param>
	/// </summary>
	public CandidateEducation(int id, string degree, string college, string state, string country, string year, string updatedBy)
	{
		ID = id;
		Degree = degree;
		College = college;
		State = state;
		Country = country;
		Year = year;
		UpdatedBy = updatedBy;
	}

	/// <summary>
	///     Gets or sets the name of the college or institution where the candidate received their education.
	/// </summary>
	/// <value>
	///     The name of the college or institution.
	/// </value>
	/// <remarks>
	///     This property is required. If not provided, an error message will be returned.
	/// </remarks>
	public string College
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets the country of the candidate's education institution.
	/// </summary>
	public string Country
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets the degree, diploma, or course name of the candidate's education.
	///     This property is required.
	/// </summary>
	public string Degree
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets the unique identifier for the CandidateEducation instance.
	/// </summary>
	/// <value>
	///     The unique identifier of the CandidateEducation instance.
	/// </value>
	/// <remarks>
	///     This property is used as the primary key in the database and is hidden from the user interface.
	/// </remarks>
	public int ID
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets the state of the educational institution where the candidate received their education.
	/// </summary>
	/// <value>
	///     The state of the educational institution.
	/// </value>
	/// <remarks>
	///     This property is used in the 'Edit Education' dialog for candidates.
	/// </remarks>
	public string State
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets the username of the person who last updated the education record.
	/// </summary>
	/// <value>
	///     The username of the person who last updated the education record.
	/// </value>
	/// <remarks>
	///     This property is used to track who made the last update to the education record of a candidate.
	///     It is typically the username of the logged-in user who made the change.
	/// </remarks>
	public string UpdatedBy
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets the year of graduation.
	/// </summary>
	/// <value>
	///     The year of graduation as a string.
	/// </value>
	/// <remarks>
	///     This property is used to store the year in which the candidate completed their education.
	///     It is represented as a string and not a DateTime object because it only contains the year part, not the full date.
	/// </remarks>
	public string Year
	{
		get;
		set;
	}

	/// <summary>
	///     Resets all properties of the CandidateEducation instance to their default values.
	/// </summary>
	/// <remarks>
	///     This method is typically used to prepare the CandidateEducation instance for reuse or to ensure that all properties
	///     are in a known state.
	/// </remarks>
	public void Clear()
	{
		ID = 0;
		Degree = "";
		College = "";
		State = "";
		Country = "";
		Year = "";
		UpdatedBy = "";
	}

	/// <summary>
	///     Creates a deep copy of the current instance of the CandidateEducation class.
	/// </summary>
	/// <returns>
	///     A new dep copy of the CandidateEducation class with the same values as the current instance.
	/// </returns>
	public CandidateEducation Copy() => MemberwiseClone() as CandidateEducation;
}