#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            ProfSvc_AppTrack
// Project:             ProfSvc_Classes
// File Name:           CandidateMPC.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja
// Created On:          12-09-2022 15:57
// Last Updated On:     10-26-2023 21:08
// *****************************************/

#endregion

namespace Subscription.Model;

/// <summary>
///     Represents a CandidateMPC class in the ProfSvc_Classes namespace.
///     This class is used to manage and process data related to a candidate's MPC status.
/// </summary>
/// <remarks>
///     The CandidateMPC class includes properties such as Date, User, MPC, and Comments.
///     It also includes methods for creating a new instance of the class, resetting properties to their default values,
///     and creating a copy of the current object.
/// </remarks>
public class CandidateMPC
{
	/// <summary>
	///     Initializes a new instance of the <see cref="CandidateMPC" /> class and resets its properties to their default
	///     values.
	/// </summary>
	public CandidateMPC()
	{
		Clear();
	}

	/// <summary>
	///     Initializes a new instance of the <see cref="CandidateMPC" /> class.
	/// </summary>
	/// <param name="date">The date when the MPC was created or updated.</param>
	/// <param name="name">The user who created or updated the MPC.</param>
	/// <param name="mpc">A boolean value indicating whether the candidate is an MPC.</param>
	/// <param name="comments">Any comments related to the candidate's MPC status.</param>
	public CandidateMPC(DateTime date, string name, bool mpc, string comments)
	{
		Date = date;
		Name = name;
		MPC = mpc;
		Comments = comments;
	}

	/// <summary>
	///     Gets or sets the comments associated with the CandidateMPC object.
	/// </summary>
	/// <value>
	///     The comments as a string.
	/// </value>
	public string Comments
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets the date associated with the CandidateMPC object.
	/// </summary>
	/// <value>
	///     The date as a DateTime.
	/// </value>
	public DateTime Date
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets a value indicating whether the CandidateMPC object is marked as MPC.
	/// </summary>
	/// <value>
	///     true if the CandidateMPC is marked as MPC; otherwise, false.
	/// </value>
	public bool MPC
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets the user associated with the CandidateMPC object.
	/// </summary>
	/// <value>
	///     The user as a string.
	/// </value>
	public string Name
	{
		get;
		set;
	}

	/// <summary>
	///     Resets the properties of the CandidateMPC object to their default values.
	/// </summary>
	public void Clear()
	{
		Date = DateTime.Today;
		Name = "";
		MPC = false;
		Comments = "";
	}

	/// <summary>
	///     Creates a deep copy of the current CandidateMPC object.
	/// </summary>
	/// <returns>
	///     A new CandidateMPC object with the same values as the current object.
	/// </returns>
	public CandidateMPC Copy() => MemberwiseClone() as CandidateMPC;
}