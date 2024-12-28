#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            ProfSvc_AppTrack
// Project:             ProfSvc_Classes
// File Name:           StatusCode.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja
// Created On:          12-06-2022 18:52
// Last Updated On:     10-26-2023 21:20
// *****************************************/

#endregion

namespace Subscription.Model;

/// <summary>
///     Represents a status code in the Professional Services application.
/// </summary>
/// <remarks>
///     A status code is a code that represents a specific status in the application.
///     It is used to track the status of various entities in the system, such as users, processes, etc.
///     Each status code has a unique identifier, a code, a status, a description, an icon, and other properties.
/// </remarks>
public class StatusCode
{
	/// <summary>
	///     Initializes a new instance of the <see cref="StatusCode" /> class and resets its properties to their default
	///     values.
	/// </summary>
	public StatusCode()
	{
		Clear();
	}

	/// <summary>
	///     Initializes a new instance of the <see cref="StatusCode" /> class.
	/// </summary>
	/// <param name="id">The unique identifier for the status code.</param>
	/// <param name="code">The code for the status.</param>
	/// <param name="status">The status of the StatusCode.</param>
	/// <param name="description">The description of the status code.</param>
	/// <param name="appliesToCode">The code that this status code applies to.</param>
	/// <param name="appliesTo">The entity to which this status code applies.</param>
	/// <param name="icon">The icon for the status code.</param>
	/// <param name="submitCandidate">Indicates whether the status code can be used to submit a candidate.</param>
	/// <param name="showCommission">Indicates whether to show the commission for the status code.</param>
	/// <param name="color">The color associated with the StatusCode.</param>
	/// <param name="createdDate">The creation date of the StatusCode.</param>
	/// <param name="updatedDate">The updated date of the StatusCode.</param>
	public StatusCode(int id, string code, string status, string description, string appliesToCode, string appliesTo, string icon, bool submitCandidate,
					  bool showCommission, string color, string createdDate, string updatedDate)
	{
		ID = id;
		Code = code;
		Description = description;
		Status = status;
		Icon = icon;
		AppliesTo = appliesTo;
		AppliesToCode = appliesToCode;
		SubmitCandidate = submitCandidate;
		ShowCommission = showCommission;
		Color = color;
		CreatedDate = createdDate;
		UpdatedDate = updatedDate;
	}

	/// <summary>
	///     Initializes a new instance of the <see cref="StatusCode" /> class.
	/// </summary>
	/// <param name="id">The unique identifier for the status code.</param>
	/// <param name="code">The code for the status.</param>
	/// <param name="status">The status of the StatusCode.</param>
	/// <param name="icon">The icon for the status code.</param>
	/// <param name="appliesToCode">The code that this status code applies to.</param>
	/// <param name="submitCandidate">A value indicating whether the status code can be used to submit a candidate.</param>
	/// <param name="showCommission">A value indicating whether to show the commission for the status code.</param>
	/// <remarks>
	///     This constructor is used to create a new instance of the <see cref="StatusCode" /> class with the specified
	///     parameters.
	/// </remarks>
	public StatusCode(int id, string code, string status, string icon, string appliesToCode, bool submitCandidate, bool showCommission)
	{
		ID = id;
		Code = code;
		Description = "";
		Status = status;
		Icon = icon;
		AppliesTo = "";
		AppliesToCode = appliesToCode;
		SubmitCandidate = submitCandidate;
		ShowCommission = showCommission;
		Color = "";
		CreatedDate = "";
		UpdatedDate = "";
	}

	/// <summary>
	///     Gets or sets the entity to which this status code applies.
	/// </summary>
	/// <value>
	///     The entity to which this status code applies.
	/// </value>
	/// <remarks>
	///     This property is used to specify the entity (like a user, a process, etc.) to which the status code is applicable.
	///     It helps in filtering and identifying the status codes based on the entity they are associated with.
	/// </remarks>
	public string AppliesTo
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets the code that this status code applies to.
	/// </summary>
	/// <value>
	///     The code that this status code applies to.
	/// </value>
	/// <remarks>
	///     This property is used to filter status codes based on their applicability. For example, a status code with an
	///     `AppliesToCode` of "SCN" is applicable to scenarios where the `Code` property of the `StatusCode` object matches
	///     the `AppliesToCode`.
	/// </remarks>
	public string AppliesToCode
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets the code for the status.
	/// </summary>
	/// <value>
	///     The code for the status.
	/// </value>
	/// <remarks>
	///     This property is used in various parts of the application to filter and identify status codes.
	/// </remarks>
	public string Code
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets the color associated with the StatusCode.
	/// </summary>
	/// <value>
	///     The color represented as a string.
	/// </value>
	/// <remarks>
	///     This property is used in the StatusCodeDialog component for binding the color value to the SfColorPicker control in
	///     the UI.
	/// </remarks>
	public string Color
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets the creation date of the StatusCode.
	/// </summary>
	/// <value>
	///     The creation date represented as a string.
	/// </value>
	/// <remarks>
	///     This property is used to track when the StatusCode was created.
	///     The date is stored as a string in the format "MM/dd/yyyy".
	/// </remarks>
	public string CreatedDate
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets the description of the status code.
	/// </summary>
	/// <value>
	///     The description represented as a string.
	/// </value>
	/// <remarks>
	///     This property is used in the StatusCodeDialog component for binding the description value to the TextBoxControl in
	///     the UI,
	///     and in the StatusCodes component for displaying the description in the grid.
	/// </remarks>
	public string Description
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets the icon for the status code.
	/// </summary>
	/// <value>
	///     The icon represented as a string.
	/// </value>
	/// <remarks>
	///     This property is used in the StatusCodeDialog component for binding the icon value to the TextBoxControl in the UI.
	/// </remarks>
	public string Icon
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets the unique identifier for the StatusCode.
	/// </summary>
	/// <value>
	///     The unique identifier for the StatusCode.
	/// </value>
	public int ID
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets a value indicating whether the status code is being added or edited.
	/// </summary>
	/// <value>
	///     <c>true</c> if the status code is being added; otherwise, <c>false</c> if the status code is being edited.
	/// </value>
	public bool IsAdd
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets a value indicating whether to show the commission for the status code.
	/// </summary>
	/// <value>
	///     <c>true</c> if the commission should be shown; otherwise, <c>false</c>.
	/// </value>
	public bool ShowCommission
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets the status of the StatusCode.
	/// </summary>
	/// <value>
	///     The status represented as a string.
	/// </value>
	public string Status
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets a value indicating whether the status code can be used to submit a candidate.
	/// </summary>
	/// <value>
	///     <c>true</c> if this status code can be used to submit a candidate; otherwise, <c>false</c>.
	/// </value>
	public bool SubmitCandidate
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets the date when the StatusCode was last updated.
	/// </summary>
	/// <value>
	///     A string representing the last updated date in the format "MM/dd/yyyy".
	/// </value>
	public string UpdatedDate
	{
		get;
		set;
	}

	/// <summary>
	///     Resets all properties of the StatusCode instance to their default values.
	/// </summary>
	/// <remarks>
	///     This method is used to clear the current state of the StatusCode instance. It sets all string properties to an
	///     empty string, boolean properties to false, and the Color property to "black".
	/// </remarks>
	public void Clear()
	{
		Code = "";
		Status = "";
		Icon = "";
		AppliesTo = "";
		AppliesToCode = "";
		SubmitCandidate = false;
		ShowCommission = false;
		Color = "black";
		CreatedDate = "";
		UpdatedDate = "";
	}

	/// <summary>
	///     Creates a shallow copy of the current StatusCode object.
	/// </summary>
	/// <returns>
	///     A new StatusCode object with the same values as the current StatusCode object.
	/// </returns>
	public StatusCode Copy() => MemberwiseClone() as StatusCode;
}