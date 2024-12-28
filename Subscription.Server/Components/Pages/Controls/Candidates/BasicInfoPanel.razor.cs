#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.Server
// File Name:           BasicInfoPanel.razor.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu
// Created On:          05-11-2024 20:05
// Last Updated On:     10-29-2024 15:10
// *****************************************/

#endregion

namespace Subscription.Server.Components.Pages.Controls.Candidates;

/// <summary>
///     Represents a panel in the user interface for displaying and managing basic information about a candidate.
///     This includes parameters for various candidate details such as requisition status, address setup, eligibility,
///     experience, tax term, job option, communication, rating date, rating note, MPC note, and MPC date.
/// </summary>
public partial class BasicInfoPanel
{
	/// <summary>
	///     Gets or sets the MPC (Most Placeable Candidate) date as a MarkupString.
	///     This date represents when the candidate was marked as the most placeable.
	/// </summary>
	[Parameter]
    public MarkupString GetMPCDate
    {
        get;
        set;
    }

	/// <summary>
	///     Gets or sets the MPC note as a markup string. This note is associated with the candidate's basic information.
	/// </summary>
	[Parameter]
    public MarkupString GetMPCNote
    {
        get;
        set;
    }

	/// <summary>
	///     Gets or sets the rating date for the candidate as a MarkupString. This date represents when the candidate's rating
	///     was last updated.
	/// </summary>
	[Parameter]
    public MarkupString GetRatingDate
    {
        get;
        set;
    }

	/// <summary>
	///     Gets or sets the rating note for the candidate as a MarkupString. This note is displayed in the BasicInfoPanel.
	/// </summary>
	[Parameter]
    public MarkupString GetRatingNote
    {
        get;
        set;
    }

	/// <summary>
	///     Gets or sets a value indicating whether the candidate is associated with a requisition.
	/// </summary>
	[Parameter]
    public bool IsRequisition
    {
        get;
        set;
    }

	/// <summary>
	///     Gets or sets the model representing the details of a candidate. This model is used to bind data to the
	///     BasicInfoPanel.
	/// </summary>
	[Parameter]
    public CandidateDetails Model
    {
        get;
        set;
    }

	/// <summary>
	///     Gets or sets the communication details for the candidate as a MarkupString. This property is a Parameter, meaning
	///     it can be used to pass data from a parent component.
	/// </summary>
	[Parameter]
    public MarkupString SetCommunication
    {
        get;
        set;
    }

	/// <summary>
	///     Gets or sets the eligibility status of the candidate as a MarkupString. This property is used to display the
	///     eligibility status in the user interface.
	/// </summary>
	[Parameter]
    public MarkupString SetEligibility
    {
        get;
        set;
    }

	/// <summary>
	///     Gets or sets the experience of the candidate as a MarkupString. This property is a Parameter, meaning it can be
	///     supplied a value when this component is included in the markup of another component.
	/// </summary>
	[Parameter]
    public MarkupString SetExperience
    {
        get;
        set;
    }

	/// <summary>
	///     Gets or sets the job option for the candidate. This is a markup string that represents the job option in the user
	///     interface.
	/// </summary>
	[Parameter]
    public MarkupString SetJobOption
    {
        get;
        set;
    }

	/// <summary>
	///     Gets or sets the tax term for the candidate. This is a markup string that can include HTML content.
	/// </summary>
	[Parameter]
    public MarkupString SetTaxTerm
    {
        get;
        set;
    }

	/// <summary>
	///     Gets or sets the address setup for the candidate as a markup string. This property is used to display and manage
	///     the candidate's address information in the user interface.
	/// </summary>
	[Parameter]
    public MarkupString SetupAddress
    {
        get;
        set;
    }
}