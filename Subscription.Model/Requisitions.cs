#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            ProfSvc_AppTrack
// Project:             ProfSvc_Classes
// File Name:           Requisitions.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja
// Created On:          12-09-2022 15:57
// Last Updated On:     10-26-2023 21:19
// *****************************************/

#endregion

namespace Subscription.Model;

/// <summary>
///     Represents a requisition in the professional services' domain.
/// </summary>
/// <remarks>
///     A requisition is a formal request or instruction from a business to obtain goods or services,
///     often for jobs or positions that need to be filled. This class provides properties for managing
///     the details of a requisition, such as its ID, code, title, associated company, job options,
///     status, and other related information.
/// </remarks>
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
public class Requisition
{
	/// <summary>
	///     Initializes a new instance of the <see cref="Requisition" /> class and resets its properties to their default
	///     values.
	/// </summary>
	public Requisition()
	{
		Clear();
	}

	/// <summary>
	///     Initializes a new instance of the <see cref="Requisition" /> class.
	/// </summary>
	/// <param name="id">The unique identifier for the requisition.</param>
	/// <param name="code">The code of the requisition.</param>
	/// <param name="title">The title of the requisition.</param>
	/// <param name="company">The name of the company associated with the requisition.</param>
	/// <param name="jobOptions">The job options for the requisition.</param>
	/// <param name="status">The status of the requisition.</param>
	/// <param name="updated">The updated timestamp for the requisition.</param>
	/// <param name="due">The due date of the requisition.</param>
	/// <param name="icon">The icon associated with the requisition.</param>
	/// <param name="priority">The priority of the requisition.</param>
	/// <param name="submitCandidate">Indicates whether a candidate can be submitted for the requisition.</param>
	/// <param name="canUpdate">Indicates whether the requisition can be updated.</param>
	/// <param name="changeStatus">Indicates whether the status of the requisition can be changed.</param>
	/// <param name="priorityColor">The color associated with the priority of the requisition.</param>
	/// <param name="assignedRecruiter">The recruiter assigned to the requisition.</param>
	/// <param name="updatedBy">The person who last updated the requisition.</param>
	public Requisition(int id, string code, string title, string company, string jobOptions, string status, string updated, string due,
						string icon, string priority, bool submitCandidate, bool canUpdate, bool changeStatus, string priorityColor, string assignedRecruiter, string updatedBy)
	{
		ID = id;
		Code = code;
		Title = title;
		Company = company;
		JobOptions = jobOptions;
		Status = status;
		Updated = updated;
		Due = due;
		Icon = icon;
		Priority = priority;
		SubmitCandidate = submitCandidate;
		CanUpdate = canUpdate;
		ChangeStatus = changeStatus;
		PriorityColor = priorityColor;
		AssignedRecruiter = assignedRecruiter;
		UpdatedBy = updatedBy;
	}

	/// <summary>
	///     Gets or sets the recruiter assigned to the requisition.
	/// </summary>
	/// <value>
	///     The name of the assigned recruiter.
	/// </value>
	public string AssignedRecruiter
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets a value indicating whether the requisition can be updated.
	/// </summary>
	/// <value>
	///     <c>true</c> if the requisition can be updated; otherwise, <c>false</c>.
	/// </value>
	public bool CanUpdate
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets a value indicating whether the status of the requisition can be changed.
	/// </summary>
	/// <value>
	///     <c>true</c> if the status of the requisition can be changed; otherwise, <c>false</c>.
	/// </value>
	public bool ChangeStatus
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets the code of the requisition.
	/// </summary>
	/// <value>
	///     The code of the requisition.
	/// </value>
	/// <remarks>
	///     The code is used as a unique identifier for the requisition and is displayed in various parts of the application.
	/// </remarks>
	public string Code
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets the company associated with the requisition.
	/// </summary>
	/// <value>
	///     The name of the company.
	/// </value>
	public string Company
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets the due date for the requisition.
	/// </summary>
	/// <value>
	///     The due date of the requisition.
	/// </value>
	public string Due
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets the icon for the requisition.
	/// </summary>
	/// <value>
	///     The icon associated with the requisition.
	/// </value>
	public string Icon
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets the unique identifier for the requisition.
	/// </summary>
	/// <value>
	///     The unique identifier for the requisition.
	/// </value>
	public int ID
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets the job options for the Requisition.
	/// </summary>
	/// <value>
	///     The job options are a string representation of the various options available for a job in the requisition.
	///     This can include details such as job type, job location, job duration, etc.
	/// </value>
	public string JobOptions
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets the priority of the requisition.
	/// </summary>
	/// <value>
	///     The priority of the requisition.
	/// </value>
	public string Priority
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets the color associated with the priority of the requisition.
	/// </summary>
	/// <value>
	///     The color associated with the priority of the requisition.
	/// </value>
	public string PriorityColor
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets the status of the requisition.
	/// </summary>
	/// <value>
	///     The status of the requisition.
	/// </value>
	public string Status
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets a value indicating whether a candidate can be submitted for the requisition.
	/// </summary>
	/// <value>
	///     <c>true</c> if a candidate can be submitted; otherwise, <c>false</c>.
	/// </value>
	public bool SubmitCandidate
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets the title of the requisition.
	/// </summary>
	/// <value>
	///     The title of the requisition.
	/// </value>
	public string Title
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets the updated timestamp for the requisition.
	/// </summary>
	/// <value>
	///     The updated timestamp.
	/// </value>
	public string Updated
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets the name of the user who last updated the requisition.
	/// </summary>
	/// <value>
	///     The name of the user who last updated the requisition.
	/// </value>
	public string UpdatedBy
	{
		get;
		set;
	}

	/// <summary>
	///     Resets all properties of the Requisitions object to their default values.
	/// </summary>
	public void Clear()
	{
		ID = 0;
		Code = "";
		Title = "";
		Company = "";
		JobOptions = "";
		Status = "";
		Updated = "";
		Due = "";
		Icon = "";
		Priority = "Medium";
		SubmitCandidate = false;
		CanUpdate = false;
		ChangeStatus = false;
		PriorityColor = "black";
		AssignedRecruiter = "";
		UpdatedBy = "";
	}

	/// <summary>
	///     Creates a shallow copy of the current Requisitions object.
	/// </summary>
	/// <returns>
	///     A new Requisitions object that is a copy of the current instance.
	/// </returns>
	public Requisition Copy() => MemberwiseClone() as Requisition;
}