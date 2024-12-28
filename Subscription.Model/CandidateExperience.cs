#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.Model
// File Name:           CandidateExperience.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu
// Created On:          05-10-2024 21:05
// Last Updated On:     10-29-2024 15:10
// *****************************************/

#endregion

namespace Subscription.Model;

/// <summary>
///     Represents a candidate's work experience in the Professional Services application.
/// </summary>
/// <remarks>
///     The CandidateExperience class includes properties such as ID, Employer, Start, End, Location, Description,
///     UpdatedBy, and Title.
///     It also includes methods for creating a new instance, copying an instance, and resetting the instance properties.
/// </remarks>
public class CandidateExperience
{
	/// <summary>
	///     Initializes a new instance of the <see cref="CandidateExperience" /> class and resets its properties to their
	///     default values.
	/// </summary>
	public CandidateExperience()
    {
        Clear();
    }

	/// <param name="id">The unique identifier for the candidate's experience.</param>
	/// <param name="employer">The name of the employer for the candidate's experience.</param>
	/// <param name="start">The start date of the candidate's experience.</param>
	/// <param name="end">The end date of the candidate's experience.</param>
	/// <param name="location">The job location of the candidate's experience.</param>
	/// <param name="description">The description of the candidate's experience.</param>
	/// <param name="updatedBy">The identifier of the user who last updated the candidate's experience.</param>
	/// <param name="title">The job title of the candidate's experience.</param>
	public CandidateExperience(int id, string employer, string start, string end, string location, string description, string updatedBy, string title)
    {
        ID = id;
        Employer = employer;
        Start = start;
        End = end;
        Location = location;
        Description = description;
        UpdatedBy = updatedBy;
        Title = title;
    }

	/// <summary>
	///     Gets or sets the description of the candidate's experience.
	/// </summary>
	/// <value>
	///     The description of the candidate's experience.
	/// </value>
	/// <remarks>
	///     This property provides detailed information about the candidate's experience. It can include roles,
	///     responsibilities, achievements, etc.
	/// </remarks>
	public string Description
    {
        get;
        set;
    }

	/// <summary>
	///     Gets or sets the name of the employer for the candidate's experience.
	///     This property is required.
	/// </summary>
	public string Employer
    {
        get;
        set;
    }

	/// <summary>
	///     Gets or sets the end date of the candidate's experience.
	/// </summary>
	/// <value>
	///     The end date of the candidate's experience.
	/// </value>
	/// <remarks>
	///     If the candidate is currently employed in the role, this property may be empty.
	/// </remarks>
	public string End
    {
        get;
        set;
    }

	/// <summary>
	///     Gets or sets the unique identifier for the candidate's experience.
	/// </summary>
	/// <value>
	///     The unique identifier for the candidate's experience.
	/// </value>
	/// <remarks>
	///     This property is automatically assigned when a new candidate experience is created.
	/// </remarks>
	public int ID
    {
        get;
        set;
    }

	/// <summary>
	///     Gets or sets the job location of the candidate's experience.
	/// </summary>
	/// <value>
	///     The job location of the candidate's experience.
	/// </value>
	/// <remarks>
	///     This property is required and must be provided when creating or updating a candidate's experience.
	/// </remarks>
	public string Location
    {
        get;
        set;
    }

	/// <summary>
	///     Gets or sets the start date of the candidate's experience.
	/// </summary>
	/// <value>
	///     The start date of the candidate's experience.
	/// </value>
	public string Start
    {
        get;
        set;
    }

	/// <summary>
	///     Gets or sets the job title of the candidate's experience.
	///     This property is required.
	/// </summary>
	public string Title
    {
        get;
        set;
    }

    public string UpdatedBy
    {
        get;
        set;
    }

	/// <summary>
	///     Resets all properties of the CandidateExperience object to their default values.
	/// </summary>
	/// <remarks>
	///     This method is used to clear the current state of the CandidateExperience object,
	///     setting all properties back to their default values. This includes setting all string properties to an empty string
	///     and the ID to 0.
	/// </remarks>
	public void Clear()
    {
        ID = 0;
        Employer = "";
        Start = "";
        End = "";
        Location = "";
        Description = "";
        UpdatedBy = "";
        Title = "";
    }

	/// <summary>
	///     Creates a deep copy of the current CandidateExperience object.
	/// </summary>
	/// <returns>
	///     A new CandidateExperience object that is a deep copy of this instance.
	/// </returns>
	public CandidateExperience Copy() => MemberwiseClone() as CandidateExperience;
}