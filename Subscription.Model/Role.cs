#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            ProfSvc_AppTrack
// Project:             ProfSvc_Classes
// File Name:           Role.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja
// Created On:          09-17-2022 20:01
// Last Updated On:     10-26-2023 21:19
// *****************************************/

#endregion

namespace ProfSvc_Classes;

/// <summary>
///     Represents a role in the Professional Services application.
/// </summary>
/// <remarks>
///     A role defines a set of permissions that determine what actions a user can perform.
///     Each user is assigned one or more roles, and the user's permissions are the union of the permissions of all their
///     roles.
/// </remarks>
public class Role
{
	/// <summary>
	///     Initializes a new instance of the <see cref="Role" /> class and resets its properties to their default values.
	/// </summary>
	public Role()
	{
		Clear();
	}

	/// <summary>
	///     Initializes a new instance of the <see cref="Role" /> class.
	/// </summary>
	/// <param name="id">The unique identifier for the role.</param>
	/// <param name="roleName">The name of the role.</param>
	/// <param name="viewCandidate">Indicates whether the role has permission to view candidates.</param>
	/// <param name="viewRequisition">Indicates whether the role has permission to view requisitions.</param>
	/// <param name="editCandidate">Indicates whether the role has permission to edit candidate information.</param>
	/// <param name="editRequisition">Indicates whether the role has permission to edit requisitions.</param>
	/// <param name="changeCandStatus">Indicates whether the role has permission to change candidate status.</param>
	/// <param name="changeReqStatus">Indicates whether the role has permission to change requisition status.</param>
	/// <param name="sendEmail">Indicates whether the role has permission to send emails to candidates.</param>
	/// <param name="forwardResume">Indicates whether the role has permission to forward resumes.</param>
	/// <param name="downloadResume">Indicates whether the role has permission to download resumes.</param>
	/// <param name="submitCandidate">Indicates whether the role has permission to submit candidates.</param>
	/// <param name="viewClients">Indicates whether the role has permission to view clients.</param>
	/// <param name="editClients">Indicates whether the role has permission to edit clients.</param>
	/// <param name="description">The description of the role. This is optional.</param>
	public Role(string id, string roleName, bool viewCandidate, bool viewRequisition, bool editCandidate, bool editRequisition, bool changeCandStatus, bool changeReqStatus,
				bool sendEmail, bool forwardResume, bool downloadResume, bool submitCandidate, bool viewClients, bool editClients, string description = "")
	{
		ID = id;
		RoleName = roleName;
		ViewCandidate = viewCandidate;
		ViewRequisition = viewRequisition;
		EditCandidate = editCandidate;
		EditRequisition = editRequisition;
		ChangeCandidateStatus = changeCandStatus;
		ChangeRequisitionStatus = changeReqStatus;
		SendEmailCandidate = sendEmail;
		ForwardResume = forwardResume;
		DownloadResume = downloadResume;
		SubmitCandidate = submitCandidate;
		ViewClients = viewClients;
		EditClients = editClients;
		Description = description;
	}

	/// <summary>
	///     Gets or sets a value indicating whether the role has permission to change candidate status.
	/// </summary>
	/// <value>
	///     true if the role can change candidate status; otherwise, false.
	/// </value>
	public bool ChangeCandidateStatus
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets a value indicating whether the role has permission to change requisition status.
	/// </summary>
	/// <value>
	///     true if the role can change requisition status; otherwise, false.
	/// </value>
	public bool ChangeRequisitionStatus
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets the description of the role.
	/// </summary>
	/// <value>
	///     The description of the role.
	/// </value>
	public string Description
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets a value indicating whether the role has permission to download resumes.
	/// </summary>
	/// <value>
	///     true if the role can download resumes; otherwise, false.
	/// </value>
	public bool DownloadResume
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets a value indicating whether the role has permission to edit candidate information.
	/// </summary>
	/// <value>
	///     true if the role can edit candidate information; otherwise, false.
	/// </value>
	public bool EditCandidate
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets a value indicating whether the role has permission to edit clients.
	/// </summary>
	/// <value>
	///     true if the role can edit clients; otherwise, false.
	/// </value>
	public bool EditClients
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets a value indicating whether the role has permission to edit requisitions.
	/// </summary>
	/// <value>
	///     true if the role can edit requisitions; otherwise, false.
	/// </value>
	public bool EditRequisition
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets a value indicating whether the role has permission to forward resumes.
	/// </summary>
	/// <value>
	///     true if the role can forward resumes; otherwise, false.
	/// </value>
	public bool ForwardResume
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets the identifier for the Role.
	/// </summary>
	/// <value>
	///     The string value representing the unique identifier of the Role.
	/// </value>
	/// <remarks>
	///     This property is used to uniquely identify a Role. It is used in various operations such as fetching user rights
	///     based on roles.
	/// </remarks>
	public string ID
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets a value indicating whether the role is being added.
	/// </summary>
	/// <value>
	///     true if the role is being added; otherwise, false.
	/// </value>
	public bool IsAdd
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets the name of the role.
	/// </summary>
	/// <value>
	///     The name of the role.
	/// </value>
	public string RoleName
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets a value indicating whether the role has permission to send emails to candidates.
	/// </summary>
	/// <value>
	///     true if the role can send emails to candidates; otherwise, false.
	/// </value>
	public bool SendEmailCandidate
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets a value indicating whether the role has permission to submit candidates.
	/// </summary>
	/// <value>
	///     true if the role can submit candidates; otherwise, false.
	/// </value>
	public bool SubmitCandidate
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets a value indicating whether the role has permission to view candidates.
	/// </summary>
	/// <value>
	///     true if the role can view candidates; otherwise, false.
	/// </value>
	public bool ViewCandidate
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets a value indicating whether the role has permission to view clients.
	/// </summary>
	/// <value>
	///     true if the role can view clients; otherwise, false.
	/// </value>
	public bool ViewClients
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets a value indicating whether the role has permission to view requisitions.
	/// </summary>
	/// <value>
	///     true if the role can view requisitions; otherwise, false.
	/// </value>
	public bool ViewRequisition
	{
		get;
		set;
	}

	/// <summary>
	///     Resets all properties of the Role instance to their default values.
	/// </summary>
	/// <remarks>
	///     This method is used to clear all the properties of the Role instance. It sets all string properties to an empty
	///     string and all boolean properties to false.
	///     It is typically used when a new Role needs to be created or an existing Role needs to be reset.
	/// </remarks>
	public void Clear()
	{
		ID = "";
		RoleName = "";
		ViewCandidate = false;
		ViewRequisition = false;
		EditCandidate = false;
		EditRequisition = false;
		ChangeCandidateStatus = false;
		ChangeRequisitionStatus = false;
		SendEmailCandidate = false;
		ForwardResume = false;
		DownloadResume = false;
		SubmitCandidate = false;
		ViewClients = false;
		EditClients = false;
		Description = "";
	}

	/// <summary>
	///     Creates a copy of the current Role instance.
	/// </summary>
	/// <returns>
	///     A new Role object that is a copy of the current instance.
	/// </returns>
	/// <remarks>
	///     This method uses the MemberwiseClone method to create a shallow copy of the current object.
	///     Shallow copy means the method creates a new object and then copies the nonstatic fields of the current object to
	///     the new object.
	///     If a field is a value type, a bit-by-bit copy of the field is performed.
	///     If a field is a reference type, the reference is copied but the referred object is not; therefore, the original
	///     object and its clone refer to the same object.
	/// </remarks>
	public Role Copy() => MemberwiseClone() as Role;
}