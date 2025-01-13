#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            ProfSvc_AppTrack
// Project:             ProfSvc_Classes
// File Name:           RequisitionDetails.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja
// Created On:          12-09-2022 15:57
// Last Updated On:     10-26-2023 21:19
// *****************************************/

#endregion

namespace Subscription.Model;

/// <summary>
///     Represents the details of a requisition in the professional services' domain.
/// </summary>
/// <remarks>
///     This class includes information such as the requisition ID, company name, contact name, position title,
///     description, number of positions, duration, location, experience required, expected rate, placement fee, type of
///     placement, job options, reporting details, salary range, expense details, expected start date, status, priority,
///     creation and update details, required skills, required education, eligibility, security clearance requirement,
///     benefits, OFCCP compliance, due date, submission details, location details, mandatory and optional requirements,
///     assigned recruiter, and various IDs related to the requisition.
/// </remarks>
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
public class RequisitionDetails
{
	/// <summary>
	///     Initializes a new instance of the <see cref="RequisitionDetails" /> class and resets its properties to their
	///     default values.
	/// </summary>
	public RequisitionDetails()
	{
		Clear();
	}

	/// <summary>
	///     Initializes a new instance of the <see cref="RequisitionDetails" /> class.
	/// </summary>
	/// <param name="requisitionID">The unique identifier for the requisition.</param>
	/// <param name="companyName">The name of the company associated with the requisition.</param>
	/// <param name="contactName">The contact name associated with the requisition.</param>
	/// <param name="positionTitle">The title of the position for the requisition.</param>
	/// <param name="description">The description of the requisition.</param>
	/// <param name="positions">The number of positions for the requisition.</param>
	/// <param name="duration">The duration of the requisition.</param>
	/// <param name="durationCode">The duration code of the requisition.</param>
	/// <param name="location">The location of the requisition.</param>
	/// <param name="experience">The experience required for the requisition.</param>
	/// <param name="expRateLow">The lower limit of the expected rate for the requisition.</param>
	/// <param name="expRateHigh">The upper limit of the expected rate for the requisition.</param>
	/// <param name="expLoadLow">The lower limit of the expected load for the requisition.</param>
	/// <param name="expLoadHigh">The upper limit of the expected load for the requisition.</param>
	/// <param name="placementFee">The placement fee for the requisition.</param>
	/// <param name="placementType">The type of placement for the requisition.</param>
	/// <param name="jobOptions">The job options for the requisition.</param>
	/// <param name="reportTo">The person to report to for the requisition.</param>
	/// <param name="salaryLow">The lower limit of the salary for the requisition.</param>
	/// <param name="salaryHigh">The upper limit of the salary for the requisition.</param>
	/// <param name="expensesPaid">Indicates whether expenses are paid for the requisition.</param>
	/// <param name="expectedStart">The expected start date for the requisition.</param>
	/// <param name="status">The status of the requisition.</param>
	/// <param name="priority">The priority of the requisition.</param>
	/// <param name="createdBy">The person who created the requisition.</param>
	/// <param name="createdDate">The date the requisition was created.</param>
	/// <param name="updatedBy">The person who last updated the requisition.</param>
	/// <param name="updatedDate">The date the requisition was last updated.</param>
	/// <param name="skillsRequired">The skills required for the requisition.</param>
	/// <param name="education">The education required for the requisition.</param>
	/// <param name="eligibility">The eligibility for the requisition.</param>
	/// <param name="securityClearance">Indicates whether security clearance is required for the requisition.</param>
	/// <param name="benefits">The benefits for the requisition.</param>
	/// <param name="benefitNotes">The notes for the benefits of the requisition.</param>
	/// <param name="ofccp">Indicates whether the requisition is OFCCP compliant.</param>
	/// <param name="dueDate">The due date for the requisition.</param>
	/// <param name="submitCandidate">Indicates whether to submit a candidate for the requisition.</param>
	/// <param name="city">The city of the requisition.</param>
	/// <param name="stateID">The state ID of the requisition.</param>
	/// <param name="zipCode">The zip code of the requisition.</param>
	/// <param name="mandatory">The mandatory requirements for the requisition.</param>
	/// <param name="optional">The optional requirements for the requisition.</param>
	/// <param name="assignedRecruiter">The recruiter assigned to the requisition.</param>
	/// <param name="priorityID">The priority ID of the requisition.</param>
	/// <param name="eligibilityID">The eligibility ID of the requisition.</param>
	/// <param name="experienceID">The experience ID of the requisition.</param>
	/// <param name="educationID">The education ID of the requisition.</param>
	/// <param name="jobOption">The job option of the requisition.</param>
	/// <param name="companyID">The company ID of the requisition.</param>
	/// <param name="contactID">The contact ID of the requisition.</param>
	/// <param name="statusCode">The status code of the requisition.</param>
	/// <param name="companyCity">The city of the company for the requisition.</param>
	/// <param name="companyState">The state of the company for the requisition.</param>
	/// <param name="companyZip">The zip code of the company for the requisition.</param>
	public RequisitionDetails(int requisitionID, string? companyName, string? contactName, string? positionTitle, string? description, int positions, string? duration, string? durationCode,
							  string? location, string? experience, decimal expRateLow, decimal expRateHigh, decimal expLoadLow, decimal expLoadHigh, decimal placementFee, bool placementType,
							  string? jobOptions, string? reportTo, decimal salaryLow, decimal salaryHigh, bool expensesPaid, DateTime expectedStart, string? status, string? priority,
							  string? createdBy, DateTime createdDate, string? updatedBy, DateTime updatedDate, string? skillsRequired, string? education, string? eligibility, bool securityClearance,
							  bool benefits, string? benefitNotes, bool ofccp, DateTime dueDate, bool submitCandidate, string? city, int stateID, string? zipCode,
							  string? mandatory, string? optional, string? assignedRecruiter, short priorityID, int eligibilityID, int experienceID, int educationID, string? jobOption,
							  int companyID, int contactID, string? statusCode, string? companyCity, string? companyState, string? companyZip)
	{
		RequisitionID = requisitionID;
		CompanyName = companyName;
		ContactName = contactName;
		PositionTitle = positionTitle;
		Description = description;
		Positions = positions;
		Duration = duration;
		DurationCode = durationCode;
		Location = location;
		Experience = experience;
		ExpRateLow = expRateLow;
		ExpRateHigh = expRateHigh;
		ExpLoadLow = expLoadLow;
		ExpLoadHigh = expLoadHigh;
		PlacementFee = placementFee;
		PlacementType = placementType;
		JobOptions = jobOptions;
		ReportTo = reportTo;
		SalaryLow = salaryLow;
		SalaryHigh = salaryHigh;
		ExpensesPaid = expensesPaid;
		ExpectedStart = expectedStart;
		Status = status;
		Priority = priority;
		CreatedBy = createdBy;
		CreatedDate = createdDate;
		UpdatedBy = updatedBy;
		UpdatedDate = updatedDate;
		SkillsRequired = skillsRequired;
		Education = education;
		Eligibility = eligibility;
		SecurityClearance = securityClearance;
		Benefits = benefits;
		BenefitNotes = benefitNotes;
		OFCCP = ofccp;
		DueDate = dueDate;
		SubmitCandidate = submitCandidate;
		City = city;
		StateID = stateID;
		ZipCode = zipCode;
		Mandatory = mandatory;
		Optional = optional;
		AssignedTo = assignedRecruiter;
		PriorityID = priorityID;
		EligibilityID = eligibilityID;
		ExperienceID = experienceID;
		EducationID = educationID;
		JobOptionID = jobOption;
		CompanyID = companyID;
		ContactID = contactID;
		StatusCode = statusCode;
		CompanyCity = companyCity;
		CompanyState = companyState;
		CompanyZip = companyZip;
	}

	/// <summary>
	///     Gets or sets the name of the person or entity to whom the requisition is assigned.
	/// </summary>
	public string? AssignedTo
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets the identifier of the entity to which the requisition is assigned.
	/// </summary>
	/// <value>
	///     The identifier of the assigned entity.
	/// </value>
	public int AssignedToID
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets the benefit notes for the requisition.
	/// </summary>
	/// <value>
	///     The benefit notes.
	/// </value>
	public string? BenefitNotes
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets a value indicating whether benefits are included in the requisition details.
	/// </summary>
	/// <value>
	///     <c>true</c> if benefits are included; otherwise, <c>false</c>.
	/// </value>
	public bool Benefits
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets the city for the requisition details.
	/// </summary>
	/// <value>
	///     The city name.
	/// </value>
	public string? City
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets the city of the company associated with the requisition.
	/// </summary>
	/// <value>
	///     The city of the company.
	/// </value>
	public string? CompanyCity
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets the identifier for the company associated with the requisition.
	/// </summary>
	/// <value>
	///     The identifier for the company.
	/// </value>
	public int CompanyID
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets the name of the company associated with the requisition.
	/// </summary>
	public string? CompanyName
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets the state of the company associated with the requisition.
	/// </summary>
	/// <value>
	///     The state of the company.
	/// </value>
	/// <remarks>
	///     This property is used to store and retrieve the state where the company associated with the requisition is located.
	///     It is used in the `RequisitionDetailsPanel` class to display and manage the company's state in the application's
	///     user interface.
	/// </remarks>
	public string? CompanyState
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets the zip code of the company associated with the requisition.
	/// </summary>
	/// <value>
	///     The zip code of the company.
	/// </value>
	public string? CompanyZip
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets the Contact ID associated with the requisition.
	/// </summary>
	public int ContactID
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets the contact name associated with the requisition.
	/// </summary>
	public string? ContactName
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets the username of the user who created the requisition.
	/// </summary>
	public string? CreatedBy
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets the date when the requisition was created.
	/// </summary>
	/// <value>
	///     The date of creation.
	/// </value>
	public DateTime CreatedDate
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets the description of the requisition.
	/// </summary>
	/// <value>
	///     The description of the requisition.
	/// </value>
	public string? Description
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets the due date for the requisition.
	/// </summary>
	/// <value>
	///     The due date for the requisition.
	/// </value>
	public DateTime DueDate
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets the duration of the requisition.
	/// </summary>
	/// <value>
	///     The duration of the requisition.
	/// </value>
	public string? Duration
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets the duration code for the requisition.
	/// </summary>
	/// <value>
	///     The duration code.
	/// </value>
	public string? DurationCode
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets the education requirements for the requisition.
	/// </summary>
	/// <value>
	///     A string representing the education requirements.
	/// </value>
	public string? Education
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets the Education ID associated with the Requisition Details.
	/// </summary>
	/// <value>
	///     The identifier for the Education level required for the Requisition.
	/// </value>
	public int EducationID
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets the eligibility requirements for the requisition.
	/// </summary>
	/// <value>
	///     The eligibility requirements for the requisition.
	/// </value>
	public string? Eligibility
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets the eligibility ID associated with the requisition.
	/// </summary>
	/// <value>
	///     The eligibility ID.
	/// </value>
	public int EligibilityID
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets the expected start date for the requisition.
	/// </summary>
	/// <value>
	///     The expected start date for the requisition.
	/// </value>
	public DateTime ExpectedStart
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets a value indicating whether the expenses are paid.
	/// </summary>
	/// <value>
	///     <c>true</c> if expenses are paid; otherwise, <c>false</c>.
	/// </value>
	public bool ExpensesPaid
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets the experience requirements for the requisition.
	/// </summary>
	/// <value>
	///     A string representing the experience requirements.
	/// </value>
	public string? Experience
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets the experience identifier.
	/// </summary>
	/// <value>
	///     The identifier of the experience.
	/// </value>
	public int ExperienceID
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets the expected high load for the requisition.
	/// </summary>
	/// <value>
	///     The expected high load for the requisition.
	/// </value>
	public decimal ExpLoadHigh
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets the expected lower limit of the load for the requisition.
	/// </summary>
	/// <value>
	///     A decimal representing the expected lower limit of the load.
	/// </value>
	public decimal ExpLoadLow
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets the maximum expected rate for the requisition.
	/// </summary>
	/// <value>
	///     The maximum expected rate.
	/// </value>
	public decimal ExpRateHigh
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets the lower boundary of the expected rate for the requisition.
	/// </summary>
	/// <value>
	///     The lower boundary of the expected rate.
	/// </value>
	public decimal ExpRateLow
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets the Job Option ID associated with the Requisition Details.
	/// </summary>
	/// <value>
	///     The Job Option ID is a string value representing the unique identifier for a job option in the requisition process.
	/// </value>
	public string? JobOptionID
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets the job options for the requisition.
	/// </summary>
	/// <value>
	///     A string representing the job options.
	/// </value>
	public string? JobOptions
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets the JobOptionsID associated with the RequisitionDetails.
	/// </summary>
	/// <value>The JobOptionsID string.</value>
	public string? JobOptionsID
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets the location associated with the requisition details.
	/// </summary>
	/// <value>The location string.</value>
	public string? Location
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets the mandatory requirements for the requisition.
	/// </summary>
	/// <value>
	///     The mandatory requirements for the requisition.
	/// </value>
	public string? Mandatory
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets a value indicating whether the OFCCP compliance is required.
	/// </summary>
	/// <value>
	///     <c>true</c> if OFCCP compliance is required; otherwise, <c>false</c>.
	/// </value>
	public bool OFCCP
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets the optional skills for the requisition.
	/// </summary>
	/// <value>
	///     A string that represents the optional skills for the requisition. The skills are separated by commas.
	/// </value>
	/// <remarks>
	///     This property is used in the `SetSkills` method of the `CompanyRequisitions` class to split the optional skills and
	///     match them with the `Skills` list.
	/// </remarks>
	public string? Optional
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets the placement fee for the requisition.
	/// </summary>
	/// <value>
	///     The placement fee represented as a decimal.
	/// </value>
	public decimal PlacementFee
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets a value indicating whether the placement type is valid or not.
	/// </summary>
	/// <value>
	///     <c>true</c> if the placement type is valid; otherwise, <c>false</c>.
	/// </value>
	public bool PlacementType
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets the number of positions for the requisition.
	/// </summary>
	public int Positions
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets the title of the position for the requisition.
	/// </summary>
	public string? PositionTitle
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
	public string? Priority
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets the priority ID of the requisition.
	/// </summary>
	/// <value>
	///     The priority ID is used to determine the urgency or importance of the requisition.
	///     It is represented as an integer value.
	/// </value>
	public int PriorityID
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets the report to property in the requisition details.
	/// </summary>
	/// <value>
	///     The name of the person or department to whom the requisition should be reported.
	/// </value>
	public string? ReportTo
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
	public int RequisitionID
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets the maximum salary for the requisition.
	/// </summary>
	/// <value>
	///     The highest possible salary for the position.
	/// </value>
	public decimal SalaryHigh
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets the lower bound of the salary range for the requisition.
	/// </summary>
	/// <value>
	///     The lower bound of the salary range.
	/// </value>
	public decimal SalaryLow
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets a value indicating whether the requisition requires a security clearance.
	/// </summary>
	/// <value>
	///     <c>true</c> if a security clearance is required; otherwise, <c>false</c>.
	/// </value>
	public bool SecurityClearance
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets the skills required for the requisition.
	/// </summary>
	/// <value>
	///     A string representing the skills required for the requisition. Skills are separated by commas.
	/// </value>
	/// <remarks>
	///     This property is used to specify the skills that are necessary for the requisition.
	///     The skills are represented as a string where each skill is separated by a comma.
	///     This property is used in the `SetSkills` method of the `CompanyRequisitions` class to set the skills required for
	///     company requisitions.
	/// </remarks>
	public string? SkillsRequired
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets the identifier for the state associated with the requisition.
	/// </summary>
	/// <value>
	///     The identifier for the state.
	/// </value>
	/// <remarks>
	///     This property is used to link the requisition to a specific state. It is used in the UI to populate and manage the
	///     state selection in the requisition details panel.
	/// </remarks>
	public int StateID
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
	public string? Status
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets the status code for the requisition details.
	/// </summary>
	/// <value>
	///     The status code is a string that represents the current status of the requisition.
	/// </value>
	public string? StatusCode
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
	public string? Title
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets the username of the user who last updated the requisition details.
	/// </summary>
	public string? UpdatedBy
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets the date and time when the requisition details were last updated.
	/// </summary>
	public DateTime UpdatedDate
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets the ZipCode for the RequisitionDetails.
	/// </summary>
	/// <value>
	///     The ZipCode is a string representing the postal code for the requisition.
	/// </value>
	public string? ZipCode
	{
		get;
		set;
	}

	/// <summary>
	///     Clears all the properties of the RequisitionDetails object by setting them to their default values.
	/// </summary>
	public void Clear()
	{
		CompanyName = "";
		ContactName = "";
		PositionTitle = "";
		Description = "";
		Positions = 0;
		Duration = "";
		DurationCode = "M";
		Location = "";
		Experience = "";
		ExpRateLow = 0;
		ExpRateHigh = 0;
		ExpLoadLow = 0;
		ExpLoadHigh = 0;
		PlacementFee = 0;
		PlacementType = false;
		JobOptions = "";
		ReportTo = "";
		SalaryLow = 0;
		SalaryHigh = 0;
		ExpensesPaid = false;
		ExpectedStart = DateTime.Today.AddYears(-10);
		Status = "Open";
		Priority = "Medium";
		CreatedBy = "ADMIN";
		CreatedDate = DateTime.Today;
		UpdatedBy = "ADMIN";
		UpdatedDate = DateTime.Today;
		SkillsRequired = "";
		Education = "";
		Eligibility = "";
		SecurityClearance = false;
		Benefits = false;
		BenefitNotes = "";
		OFCCP = false;
		DueDate = DateTime.Today.AddDays(7);
		SubmitCandidate = false;
		City = "";
		StateID = 1;
		ZipCode = "";
		Mandatory = "";
		Optional = "";
		AssignedTo = "";
		PriorityID = 1;
		EligibilityID = 0;
		ExperienceID = 0;
		EducationID = 0;
		JobOptionID = "1";
		CompanyID = 0;
		ContactID = 0;
		StatusCode = "NEW";
		CompanyCity = "";
		CompanyState = "";
		CompanyZip = "";
	}

	/// <summary>
	///     Creates a shallow copy of the current RequisitionDetails object.
	/// </summary>
	/// <returns>
	///     A new RequisitionDetails object that is a shallow copy of this instance.
	/// </returns>
	public RequisitionDetails? Copy() => MemberwiseClone() as RequisitionDetails;
}