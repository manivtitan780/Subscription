#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            ProfSvc_AppTrack
// Project:             ProfSvc_Classes
// File Name:           CandidateActivity.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja
// Created On:          09-17-2022 20:01
// Last Updated On:     10-26-2023 21:06
// *****************************************/

#endregion

#region Using

#endregion

namespace Subscription.Model;

/// <summary>
///     Represents a candidate's activity in the application.
/// </summary>
/// <remarks>
///     This class is used to track and manage a candidate's activity such as interviews, status updates, and other related
///     actions.
/// </remarks>
public class CandidateActivity
{
	/// <summary>
	///     Initializes a new instance of the <see cref="CandidateActivity" /> class.
	/// </summary>
	/// <remarks>
	///     This constructor initializes the CandidateActivity object and clears its data.
	/// </remarks>
	public CandidateActivity()
	{
		Clear();
	}

	/// <summary>
	///     Initializes a new instance of the <see cref="CandidateActivity" /> class.
	/// </summary>
	/// <param name="requisition">The requisition associated with the candidate activity.</param>
	/// <param name="updatedDate">The date when the candidate activity was last updated.</param>
	/// <param name="updatedBy">The user who last updated the candidate activity.</param>
	/// <param name="positions">The number of positions associated with the candidate activity.</param>
	/// <param name="positionFilled">The number of positions filled in the candidate activity.</param>
	/// <param name="status">The status of the candidate activity.</param>
	/// <param name="notes">Any notes associated with the candidate activity.</param>
	/// <param name="id">The unique identifier for the candidate activity.</param>
	/// <param name="schedule">A value indicating whether the candidate activity is scheduled.</param>
	/// <param name="appliesTo">The entity to which the candidate activity applies.</param>
	/// <param name="color">The color associated with the candidate activity.</param>
	/// <param name="icon">The icon associated with the candidate activity.</param>
	/// <param name="doRoleHaveRight">A value indicating whether the role has rights to the candidate activity.</param>
	/// <param name="lastActionBy">The user who last performed an action on the candidate activity.</param>
	/// <param name="requisitionID">The unique identifier for the requisition associated with the candidate activity.</param>
	/// <param name="candidateUpdatedBy">The user who last updated the candidate.</param>
	/// <param name="countSubmitted">The number of times the candidate activity has been submitted.</param>
	/// <param name="statusCode">The status code of the candidate activity.</param>
	/// <param name="showCalendar">A value indicating whether to show the calendar for the candidate activity.</param>
	/// <param name="dateTimeInterview">The date and time of the interview in the candidate activity.</param>
	/// <param name="typeOfInterview">The type of interview in the candidate activity.</param>
	/// <param name="phoneNumber">The phone number associated with the candidate activity.</param>
	/// <param name="interviewDetails">The details of the interview in the candidate activity.</param>
	/// <param name="undone">A value indicating whether the candidate activity is undone.</param>
	public CandidateActivity(string requisition, DateTime updatedDate, string updatedBy, int positions, int positionFilled, string status, string notes, int id,
							 bool schedule, string appliesTo, string color, string icon, bool doRoleHaveRight, string lastActionBy, int requisitionID, string candidateUpdatedBy,
							 int countSubmitted, string statusCode, bool showCalendar, DateTime dateTimeInterview, string typeOfInterview, string phoneNumber, string interviewDetails, bool undone)
	{
		Requisition = requisition;
		UpdatedDate = updatedDate;
		UpdatedBy = updatedBy;
		Positions = positions;
		PositionFilled = positionFilled;
		Status = status;
		Notes = notes;
		ID = id;
		Schedule = schedule;
		AppliesTo = appliesTo;
		Color = color;
		Icon = icon;
		DoRoleHaveRight = doRoleHaveRight;
		LastActionBy = lastActionBy;
		RequisitionID = requisitionID;
		CandidateUpdatedBy = candidateUpdatedBy;
		CountSubmitted = countSubmitted;
		StatusCode = statusCode;
		ShowCalendar = showCalendar;
		DateTimeInterview = dateTimeInterview;
		TypeOfInterview = typeOfInterview;
		PhoneNumber = phoneNumber;
		InterviewDetails = interviewDetails;
		Undone = undone;
	}

	/// <summary>
	///     Gets or sets the entity to which the candidate activity applies.
	/// </summary>
	/// <value>
	///     The entity to which the candidate activity applies.
	/// </value>
	/// <remarks>
	///     This property is used to specify the entity (such as a job position, department, or project)
	///     that the candidate activity is related to or impacts.
	/// </remarks>
	public string AppliesTo
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets the username of the user who last updated the candidate.
	/// </summary>
	/// <value>
	///     The username of the user who last updated the candidate.
	/// </value>
	/// <remarks>
	///     This property is used to track who made the most recent update to the candidate's information.
	/// </remarks>
	public string CandidateUpdatedBy
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets the color associated with the candidate activity.
	/// </summary>
	/// <value>
	///     The color associated with the candidate activity.
	/// </value>
	/// <remarks>
	///     This property is used to visually distinguish candidate activities.
	///     The color is represented as a string in hexadecimal color code format (e.g., "#FFFFFF" for white).
	/// </remarks>
	public string Color
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets the number of times the candidate activity has been submitted.
	/// </summary>
	/// <value>
	///     The number of times the candidate activity has been submitted.
	/// </value>
	/// <remarks>
	///     This property is used to track the number of submissions for a particular candidate activity.
	///     Each submission could represent a different stage or update in the candidate's activity.
	/// </remarks>
	public int CountSubmitted
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets the date and time of the interview for the candidate.
	/// </summary>
	/// <value>
	///     The date and time of the interview.
	/// </value>
	/// <remarks>
	///     This property is used in the `EditActivityDialog` component for data binding,
	///     and in the `SaveCandidateActivity` method in `CandidatesController` for saving the interview date and time to the
	///     database.
	///     If the value is `DateTime.MinValue`, `DBNull.Value` will be used in the database.
	/// </remarks>
	public DateTime DateTimeInterview
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets a value indicating whether the role has rights to the candidate activity.
	/// </summary>
	/// <value>
	///     <c>true</c> if the role has rights to the candidate activity; otherwise, <c>false</c>.
	/// </value>
	/// <remarks>
	///     This property is used to determine if the current role has the necessary permissions to perform actions on the
	///     candidate activity.
	/// </remarks>
	public bool DoRoleHaveRight
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets the icon associated with the candidate activity.
	/// </summary>
	/// <value>
	///     The icon associated with the candidate activity.
	/// </value>
	/// <remarks>
	///     This property is used to specify a visual representation or symbol for the candidate activity.
	/// </remarks>
	public string Icon
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets the unique identifier for the candidate activity.
	/// </summary>
	/// <value>
	///     The unique identifier for the candidate activity.
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
	///     Gets or sets the details of the interview for the candidate activity.
	/// </summary>
	/// <value>
	///     A string containing the detailed information about the interview.
	/// </value>
	/// <remarks>
	///     This property is used in the user interface to provide a detailed description of the interview process. It can
	///     include information such as interview questions, interviewer's feedback, and other relevant details.
	/// </remarks>
	public string InterviewDetails
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets the user who last performed an action on the candidate activity.
	/// </summary>
	/// <value>
	///     The user who last performed an action on the candidate activity.
	/// </value>
	/// <remarks>
	///     This property is used to track the user who last interacted with the candidate activity.
	///     It could be useful for auditing purposes or to provide context for subsequent actions.
	/// </remarks>
	public string LastActionBy
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets the new status code for the candidate's activity.
	/// </summary>
	/// <value>
	///     The new status code as a string.
	/// </value>
	/// <remarks>
	///     This property is used to update the status of a candidate's activity. It is bound to a dropdown control in the UI
	///     for selection of new status.
	/// </remarks>
	public string NewStatusCode
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets the notes for the candidate's activity.
	/// </summary>
	/// <value>
	///     The notes for the candidate's activity. This is a required field and should be between 2 to 1000 characters.
	/// </value>
	/// <remarks>
	///     This property is used in various parts of the application such as the ActivityPanel, ActivityPanelRequisition, and
	///     EditActivityDialog.
	///     It is displayed in the UI and can be edited by the user.
	/// </remarks>
	public string Notes
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets the phone number associated with the candidate activity.
	/// </summary>
	/// <value>
	///     The phone number associated with the candidate activity.
	/// </value>
	/// <remarks>
	///     This property is used in the `EditActivityDialog` component for data binding.
	/// </remarks>
	public string PhoneNumber
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets the number of positions filled in the candidate activity.
	/// </summary>
	/// <value>
	///     The number of positions filled in the candidate activity.
	/// </value>
	/// <remarks>
	///     This property is used to track the number of positions that have been successfully filled
	///     in the context of the candidate activity. It provides a measure of the progress made in
	///     filling the positions associated with the candidate activity.
	/// </remarks>
	public int PositionFilled
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets the number of positions associated with the candidate activity.
	/// </summary>
	/// <value>
	///     The number of positions associated with the candidate activity.
	/// </value>
	/// <remarks>
	///     This property is used to specify the total number of positions that are related to or impacted by the candidate
	///     activity.
	/// </remarks>
	public int Positions
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets the requisition associated with the candidate's activity.
	/// </summary>
	/// <value>
	///     The requisition is a string value that represents a specific job or position the candidate has applied for or is
	///     being considered for.
	/// </value>
	/// <remarks>
	///     This property is used in the `ActivityPanel` and `ActivityPanelRequisition` classes to display the requisition in
	///     the user interface.
	/// </remarks>
	public string Requisition
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets the ID of the requisition associated with the candidate's activity.
	/// </summary>
	/// <value>
	///     The ID of the requisition.
	/// </value>
	/// <remarks>
	///     This property is used when saving candidate activity data to the database.
	/// </remarks>
	public int RequisitionID
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets a value indicating whether the candidate activity is scheduled.
	/// </summary>
	/// <value>
	///     <c>true</c> if the candidate activity is scheduled; otherwise, <c>false</c>.
	/// </value>
	/// <remarks>
	///     This property is used to determine if a candidate activity has been scheduled. If it is set to <c>true</c>,
	///     it means the activity is scheduled. If it is set to <c>false</c>, it means the activity is not yet scheduled.
	/// </remarks>
	public bool Schedule
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets a value indicating whether to show the calendar.
	/// </summary>
	/// <value>
	///     <c>true</c> if the calendar should be shown; otherwise, <c>false</c>.
	/// </value>
	/// <remarks>
	///     This property is used to control the visibility of the calendar in the user interface.
	///     When set to <c>true</c>, the calendar is displayed. When set to <c>false</c>, the calendar is hidden.
	/// </remarks>
	public bool ShowCalendar
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets the status of the candidate activity.
	/// </summary>
	/// <value>
	///     The status of the candidate activity.
	/// </value>
	/// <remarks>
	///     This property is used to represent the current state of the candidate's activity in the application.
	///     It is displayed in various parts of the application such as the ActivityPanel and EditActivityDialog.
	/// </remarks>
	public string Status
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets the status code of the candidate's activity.
	/// </summary>
	/// <value>
	///     A string representing the status code of the activity. This code is used to track the current state of the
	///     activity.
	/// </value>
	/// <remarks>
	///     The status code is used in various parts of the application to determine the next steps in the workflow,
	///     and to control access to certain functionalities based on the current status of the activity.
	/// </remarks>
	public string StatusCode
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets the type of interview for the candidate's activity.
	/// </summary>
	/// <value>
	///     The type of interview as a string.
	/// </value>
	/// <remarks>
	///     This property is used in the `EditActivityDialog` component for data binding and in the `CandidatesController` for
	///     saving candidate activity.
	/// </remarks>
	public string TypeOfInterview
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets a value indicating whether the candidate activity is undone.
	/// </summary>
	/// <value>
	///     <c>true</c> if the candidate activity is undone; otherwise, <c>false</c>.
	/// </value>
	/// <remarks>
	///     This property is used to track if a candidate's activity is undone or not.
	///     An undone activity is one that was previously completed but has been reverted for some reason.
	/// </remarks>
	public bool Undone
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets the username of the user who last updated the candidate activity.
	/// </summary>
	/// <value>
	///     The username of the user who last updated the candidate activity.
	/// </value>
	/// <remarks>
	///     This property is used to track who made the most recent changes to the candidate activity. It is displayed in the
	///     activity panel and is also used for permission checks.
	/// </remarks>
	public string UpdatedBy
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets the date when the candidate activity was last updated.
	/// </summary>
	/// <value>
	///     The date when the candidate activity was last updated.
	/// </value>
	public DateTime UpdatedDate
	{
		get;
		set;
	}

	/// <summary>
	///     Resets all properties of the CandidateActivity instance to their default values.
	/// </summary>
	/// <remarks>
	///     This method is typically used when initializing a new instance of the CandidateActivity class or when needing to
	///     reset the state of an existing instance.
	/// </remarks>
	public void Clear()
	{
		Requisition = "";
		UpdatedDate = DateTime.MinValue;
		UpdatedBy = "";
		Positions = 0;
		PositionFilled = 0;
		Status = "";
		Notes = "";
		ID = 0;
		Schedule = false;
		AppliesTo = "";
		Color = "#000000";
		Icon = "";
		DoRoleHaveRight = false;
		LastActionBy = "";
		RequisitionID = 0;
		CandidateUpdatedBy = "";
		CountSubmitted = 0;
		StatusCode = "";
		ShowCalendar = false;
		DateTimeInterview = DateTime.MinValue;
		TypeOfInterview = "P";
		PhoneNumber = "";
		InterviewDetails = "";
		Undone = false;
	}

	/// <summary>
	///     Creates a shallow copy of the current <see cref="CandidateActivity" /> object.
	/// </summary>
	/// <returns>
	///     A new <see cref="CandidateActivity" /> object that is a shallow copy of this instance.
	/// </returns>
	/// <remarks>
	///     This method is used when you want to create a duplicate of the current object with the same values.
	///     Note that this is a shallow copy, so any reference types within the object will still point to the same instance.
	/// </remarks>
	public CandidateActivity Copy() => MemberwiseClone() as CandidateActivity;
}