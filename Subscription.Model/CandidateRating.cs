#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            ProfSvc_AppTrack
// Project:             ProfSvc_Classes
// File Name:           CandidateRating.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja
// Created On:          12-09-2022 15:57
// Last Updated On:     10-26-2023 21:09
// *****************************************/

#endregion

namespace Subscription.Model;

/// <summary>
///     Represents a rating given to a candidate in the Professional Services application.
/// </summary>
/// <remarks>
///     This class includes properties such as Date, User, Rating, and Comments.
///     It also includes methods for resetting the properties to their default values and creating a shallow copy of the
///     CandidateRating object.
/// </remarks>
public class CandidateRating
{
	/// <summary>
	///     Initializes a new instance of the <see cref="CandidateRating" /> class and sets its properties to their default
	///     values.
	/// </summary>
	public CandidateRating()
	{
		Clear();
	}

	/// <summary>
	///     Initializes a new instance of the <see cref="CandidateRating" /> class.
	/// </summary>
	/// <param name="date">The date when the rating was given.</param>
	/// <param name="name">The user who provided the rating.</param>
	/// <param name="rating">The rating of the candidate.</param>
	/// <param name="comments">The comments for the candidate's rating.</param>
	public CandidateRating(DateTime date, string name, byte rating, string comments)
	{
		Date = date;
		Name = name;
		Rating = rating;
		Comments = comments;
	}

	/// <summary>
	///     Gets or sets the comments for the candidate's rating.
	/// </summary>
	/// <value>
	///     The comments for the candidate's rating.
	/// </value>
	public string Comments
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets the date when the rating was given to the candidate.
	/// </summary>
	/// <value>
	///     The date when the rating was given.
	/// </value>
	public DateTime Date
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets the rating of the candidate.
	/// </summary>
	/// <value>
	///     The rating of the candidate.
	/// </value>
	public int Rating
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets the user who provided the rating for the candidate.
	/// </summary>
	/// <value>
	///     The user who provided the rating.
	/// </value>
	public string Name
	{
		get;
		set;
	}

	/// <summary>
	///     Resets the properties of the CandidateRating object to their default values.
	/// </summary>
	public void Clear()
	{
		Date = DateTime.Today;
		Name = "";
		Rating = 0;
		Comments = "";
	}

	/// <summary>
	///     Creates a deep copy of the current CandidateRating object.
	/// </summary>
	/// <returns>
	///     A new CandidateRating object with the same values as the current object.
	/// </returns>
	public CandidateRating Copy() => MemberwiseClone() as CandidateRating;
}