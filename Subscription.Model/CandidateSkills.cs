#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            ProfSvc_AppTrack
// Project:             ProfSvc_Classes
// File Name:           CandidateSkills.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja
// Created On:          12-09-2022 15:57
// Last Updated On:     10-26-2023 21:09
// *****************************************/

#endregion

namespace Subscription.Model;

/// <summary>
///     Represents a candidate's set of skills in the Professional Services application.
/// </summary>
/// <remarks>
///     This class includes properties such as ID, Skill, LastUsed, ExpMonth, and UpdatedBy.
///     It also includes methods for creating and copying instances of the CandidateSkills class.
/// </remarks>
public class CandidateSkills
{
	/// <summary>
	///     Initializes a new instance of the <see cref="CandidateSkills" /> class.
	/// </summary>
	/// <remarks>
	///     This constructor is used to create a new CandidateSkills object with default values.
	/// </remarks>
	public CandidateSkills()
	{
		Clear();
	}

	/// <summary>
	///     Initializes a new instance of the <see cref="CandidateSkills" /> class.
	/// </summary>
	/// <param name="id">The ID of the candidate's skill.</param>
	/// <param name="skill">The skill of the candidate.</param>
	/// <param name="lastUsed">The year when the skill was last used by the candidate.</param>
	/// <param name="expMonth">The number of months of experience the candidate has with the skill.</param>
	/// <param name="updatedBy">The identifier of the user who last updated the candidate's skills.</param>
	public CandidateSkills(int id, string? skill, short lastUsed, short expMonth, string? updatedBy)
	{
		ID = id;
		Skill = skill;
		LastUsed = lastUsed;
		ExpMonth = expMonth;
		UpdatedBy = updatedBy;
	}

	/// <summary>
	///     Gets or sets the number of months of experience the candidate has with the skill.
	/// </summary>
	/// <value>
	///     The number of months of experience the candidate has with the skill. If the candidate is currently using the skill
	///     or the experience duration is unknown, this value should be zero.
	/// </value>
	public int ExpMonth
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets the ID of the candidate's skill.
	/// </summary>
	/// <value>
	///     The ID of the candidate's skill.
	/// </value>
	public int ID
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets the year when the skill was last used by the candidate.
	/// </summary>
	/// <value>
	///     The year when the skill was last used by the candidate. If the skill is currently being used or the last used year
	///     is unknown, this value should be zero.
	/// </value>
	public int LastUsed
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets the skill of the candidate.
	/// </summary>
	/// <value>
	///     The skill of the candidate.
	/// </value>
	public string? Skill
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets the identifier of the user who last updated the candidate's skills.
	/// </summary>
	/// <value>
	///     The identifier of the user who last updated the candidate's skills.
	/// </value>
	public string? UpdatedBy
	{
		get;
		set;
	}

	/// <summary>
	///     Resets all properties of the CandidateSkills object to their default values.
	/// </summary>
	/// <remarks>
	///     This method is typically used to prepare the CandidateSkills object for reuse or to ensure that it doesn't contain
	///     any leftover data.
	/// </remarks>
	public void Clear()
	{
		ID = 0;
		Skill = "";
		LastUsed = 0;
		ExpMonth = 0;
		UpdatedBy = "";
	}

	/// <summary>
	///     Creates a shallow copy of the current CandidateSkills object.
	/// </summary>
	/// <returns>
	///     A new CandidateSkills object that is a shallow copy of this instance.
	/// </returns>
	public CandidateSkills? Copy() => MemberwiseClone() as CandidateSkills;
}