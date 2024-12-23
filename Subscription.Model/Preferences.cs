#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            ProfSvc_AppTrack
// Project:             ProfSvc_Classes
// File Name:           Preferences.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja
// Created On:          03-18-2023 19:49
// Last Updated On:     10-26-2023 21:19
// *****************************************/

#endregion

namespace Subscription.Model;

/// <summary>
///     Represents user preferences in the application.
/// </summary>
/// <remarks>
///     This class provides properties to manage user preferences such as color settings for priority levels,
///     permissions for recruiters and admins, and display settings.
/// </remarks>
public class Preferences
{
	/// <summary>
	///     Initializes a new instance of the <see cref="Preferences" /> class and resets its properties to their default
	///     values.
	/// </summary>
	public Preferences()
	{
		Clear();
	}

	/// <summary>
	///     Initializes a new instance of the <see cref="Preferences" /> class.
	/// </summary>
	/// <param name="id">The unique identifier for the preferences.</param>
	/// <param name="highPriorityColor">The color to represent high priority.</param>
	/// <param name="normalPriorityColor">The color to represent normal priority.</param>
	/// <param name="lowPriorityColor">The color to represent low priority.</param>
	/// <param name="recruitersSubmitCandidate">A value indicating whether recruiters can submit candidates.</param>
	/// <param name="adminCandidates">A value indicating whether admin has access to candidates.</param>
	/// <param name="adminRequisitions">A value indicating whether admin has access to requisitions.</param>
	/// <param name="changeRequisitionStatus">The permission level required to change requisition status.</param>
	/// <param name="changeCandidateStatus">The permission level required to change candidate status.</param>
	/// <param name="changeCandidateSubmissionStatus">The permission level required to change candidate submission status.</param>
	/// <param name="pageSize">The number of items to display per page.</param>
	/// <param name="sortOnPriority">A value indicating whether to sort items based on their priority.</param>
	public Preferences(int id, string highPriorityColor, string normalPriorityColor, string lowPriorityColor, bool recruitersSubmitCandidate, bool adminCandidates, bool adminRequisitions,
					   byte changeRequisitionStatus, byte changeCandidateStatus, byte changeCandidateSubmissionStatus, byte pageSize, bool sortOnPriority)
	{
		ID = id;
		HighPriorityColor = highPriorityColor;
		NormalPriorityColor = normalPriorityColor;
		LowPriorityColor = lowPriorityColor;
		RecruitersSubmitCandidate = recruitersSubmitCandidate;
		AdminCandidates = adminCandidates;
		AdminRequisitions = adminRequisitions;
		ChangeRequisitionStatus = changeRequisitionStatus;
		ChangeCandidateStatus = changeCandidateStatus;
		ChangeCandidateSubmissionStatus = changeCandidateSubmissionStatus;
		PageSize = pageSize;
		SortOnPriority = sortOnPriority;
	}

	/// <summary>
	///     Gets or sets a value indicating whether the admin has candidate management rights.
	/// </summary>
	/// <value>
	///     <c>true</c> if the admin has candidate management rights; otherwise, <c>false</c>.
	/// </value>
	public bool AdminCandidates
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets a value indicating whether admin has requisition rights.
	/// </summary>
	/// <value>
	///     <c>true</c> if admin has requisition rights; otherwise, <c>false</c>.
	/// </value>
	public bool AdminRequisitions
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets the status for changing candidate status.
	/// </summary>
	/// <value>
	///     A byte value representing the status for changing candidate status.
	/// </value>
	public byte ChangeCandidateStatus
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets the status for changing candidate submission.
	/// </summary>
	/// <value>
	///     A byte value representing the status for changing candidate submission.
	/// </value>
	public byte ChangeCandidateSubmissionStatus
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets the status for changing requisitions.
	/// </summary>
	/// <value>
	///     A byte value representing the status for changing requisitions.
	/// </value>
	public byte ChangeRequisitionStatus
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets the color for high priority items.
	/// </summary>
	/// <value>
	///     A string representing the color in hexadecimal format.
	/// </value>
	public string HighPriorityColor
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets the ID of the Preferences instance.
	/// </summary>
	/// <value>
	///     An integer representing the ID.
	/// </value>
	public int ID
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets the color for low priority items.
	/// </summary>
	/// <value>
	///     A string representing the color in hexadecimal format.
	/// </value>
	public string LowPriorityColor
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets the color for normal priority items.
	/// </summary>
	/// <value>
	///     A string representing the color in hexadecimal format.
	/// </value>
	public string NormalPriorityColor
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets the page size for the display of data.
	/// </summary>
	/// <value>
	///     The size of the page.
	/// </value>
	public byte PageSize
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets a value indicating whether recruiters can submit candidates.
	/// </summary>
	/// <value>
	///     <c>true</c> if recruiters are allowed to submit candidates; otherwise, <c>false</c>.
	/// </value>
	public bool RecruitersSubmitCandidate
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets a value indicating whether sorting should be based on priority.
	/// </summary>
	/// <value>
	///     <c>true</c> if sorting should be based on priority; otherwise, <c>false</c>.
	/// </value>
	public bool SortOnPriority
	{
		get;
		set;
	}

	/// <summary>
	///     Resets all properties of the Preferences instance to their default values.
	/// </summary>
	public void Clear()
	{
		ID = 0;
		HighPriorityColor = "#ff0000";
		NormalPriorityColor = "#000000";
		LowPriorityColor = "#008000";
		RecruitersSubmitCandidate = false;
		AdminCandidates = true;
		AdminRequisitions = true;
		ChangeRequisitionStatus = 0;
		ChangeCandidateStatus = 1;
		ChangeCandidateSubmissionStatus = 1;
		PageSize = 50;
		SortOnPriority = true;
	}

	/// <summary>
	///     Creates a shallow copy of the current Preferences instance.
	/// </summary>
	/// <returns>
	///     A new Preferences object that is a copy of the current instance.
	/// </returns>
	public Preferences Copy() => MemberwiseClone() as Preferences;
}