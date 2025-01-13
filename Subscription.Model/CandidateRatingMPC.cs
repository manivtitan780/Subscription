#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.Model
// File Name:           CandidateRatingMPC.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu
// Created On:          05-10-2024 21:05
// Last Updated On:     10-29-2024 15:10
// *****************************************/

#endregion

namespace Subscription.Model;

/// <summary>
///     Represents a candidate's rating and Most Placeable Candidate (MPC) status.
/// </summary>
/// <remarks>
///     This class is used to manage and store the rating and MPC status of a candidate.
///     It includes properties for the candidate's unique identifier, rating, rating comments,
///     MPC status, and MPC comments. It also includes methods for creating a new instance of the class
///     and for creating a shallow copy of an existing instance.
/// </remarks>
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
public class CandidateRatingMPC
{
	/// <summary>
	///     Initializes a new instance of the <see cref="CandidateRatingMPC" /> class.
	/// </summary>
	/// <remarks>
	///     This constructor is used to reset the properties of the CandidateRatingMPC object to their default values.
	/// </remarks>
	public CandidateRatingMPC()
    {
        Clear();
    }

	/// <summary>
	///     Initializes a new instance of the <see cref="CandidateRatingMPC" /> class.
	/// </summary>
	/// <param name="id">The unique identifier of the candidate.</param>
	/// <param name="rating">The rating of the candidate.</param>
	/// <param name="ratingComments">The comments related to the candidate's rating.</param>
	/// <param name="mpc">A boolean value indicating whether the candidate is marked as MPC (Most Placeable Candidate).</param>
	/// <param name="mpcComments">The comments related to the candidate's MPC status.</param>
	public CandidateRatingMPC(int id, int rating, string? ratingComments, bool mpc, string? mpcComments)
    {
        ID = id;
        Rating = rating;
        RatingComments = ratingComments;
        MPC = mpc;
        MPCComments = mpcComments;
    }

	/// <summary>
	///     Gets or sets the identifier for the CandidateRatingMPC.
	/// </summary>
	/// <value>
	///     An integer value that represents the unique identifier of a CandidateRatingMPC instance.
	/// </value>
	/// <remarks>
	///     This property is used as a unique identifier for a CandidateRatingMPC instance. It is used in various parts of the
	///     application,
	///     including the CandidatesController for saving the MPC status and rating of a candidate.
	/// </remarks>
	public int ID
    {
        get;
        set;
    }

	/// <summary>
	///     Gets or sets a value indicating whether the candidate is a Most Placeable Candidate (MPC).
	/// </summary>
	/// <value>
	///     A boolean value that is true if the candidate is an MPC; otherwise, false.
	/// </value>
	/// <remarks>
	///     This property is used to store the MPC status of a candidate. It is used in various parts of the application,
	///     including the Candidate page, MPCCandidateDialog component, and in the CandidatesController for saving the MPC
	///     status.
	/// </remarks>
	public bool MPC
    {
        get;
        set;
    }

	/// <summary>
	///     Gets or sets the comments for the Candidate's MPC (Most Placeable Candidate) status.
	///     This property is used to store additional information or notes about the candidate's MPC status.
	/// </summary>
	public string? MPCComments
    {
        get;
        set;
    }

	/// <summary>
	///     Gets or sets the rating of a candidate.
	/// </summary>
	/// <value>
	///     A double precision floating point number representing the candidate's rating.
	/// </value>
	/// <remarks>
	///     This property is used to store the rating of a candidate. It is used in various parts of the application,
	///     including the Candidate page, RatingCandidateDialog component, and in the CandidatesController for saving the
	///     rating.
	/// </remarks>
	public double Rating
    {
        get;
        set;
    }

	/// <summary>
	///     Gets or sets the comments for the rating of a candidate.
	/// </summary>
	/// <value>
	///     The comments for the rating of a candidate.
	/// </value>
	/// <remarks>
	///     This property is used in the rating dialog of a candidate and is stored in the database via the
	///     `CandidatesController.SaveRating()` method.
	/// </remarks>
	public string? RatingComments
    {
        get;
        set;
    }

	/// <summary>
	///     Creates a deep copy of the current CandidateRatingMPC object.
	/// </summary>
	/// <returns>A new CandidateRatingMPC object that is a deep copy of this instance.</returns>
	public void Clear()
    {
        Rating = 0;
        RatingComments = "";
        MPC = false;
        MPCComments = "";
    }

	/// <summary>
	///     Creates a deep copy of the current object.
	/// </summary>
	/// <returns>
	///     A new CandidateRatingMPC object that is a deep copy of this instance.
	/// </returns>
	public CandidateRatingMPC? Copy() => MemberwiseClone() as CandidateRatingMPC;
}