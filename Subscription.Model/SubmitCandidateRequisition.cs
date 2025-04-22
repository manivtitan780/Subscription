#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            ProfSvc_AppTrack
// Project:             ProfSvc_Classes
// File Name:           SubmitCandidateRequisition.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja
// Created On:          12-09-2022 15:57
// Last Updated On:     10-26-2023 21:20
// *****************************************/

#endregion

namespace Subscription.Model;

/// <summary>
///     Represents a class for submitting candidate requisitions in the Professional Services application.
/// </summary>
/// <remarks>
///     The SubmitCandidateRequisition class includes methods and properties for handling candidate requisition
///     submissions.
///     It includes properties such as Text, and methods like Clear and Copy.
/// </remarks>
public class SubmitCandidateRequisition
{
	/// <summary>
	///     Initializes a new instance of the <see cref="SubmitCandidateRequisition" /> class.
	/// </summary>
	/// <remarks>
	///     This constructor is called when creating a new instance of the <see cref="SubmitCandidateRequisition" /> class.
	///     It calls the Clear method to reset the Text property to its default state (an empty string).
	/// </remarks>
	public SubmitCandidateRequisition()
	{
		Clear();
	}

	/// <summary>
	///     Initializes a new instance of the SubmitCandidateRequisition class with the specified text.
	/// </summary>
	/// <param name="text">The text to initialize the SubmitCandidateRequisition instance with.</param>
	/// <remarks>
	///     This constructor is used when you want to create a new SubmitCandidateRequisition instance and immediately set its
	///     Text property.
	/// </remarks>
	public SubmitCandidateRequisition(string text) => Text = text;

	/// <summary>
	///     Gets or sets the text for the SubmitCandidateRequisition instance.
	/// </summary>
	/// <value>
	///     A string representing the text. This value should be between 5 and 1000 characters long.
	/// </value>
	/// <remarks>
	///     This property is used to hold the text information for a SubmitCandidateRequisition instance.
	///     It is validated to ensure that the text is between 5 and 1000 characters long.
	/// </remarks>
	[StringLength(1000, MinimumLength = 5, ErrorMessage = "Notes should be between 5 and 1000 characters long.")]
	public string Text
	{
		get;
		set;
	}

	/// <summary>
	///     Clears the Text property of the SubmitCandidateRequisition instance.
	/// </summary>
	/// <remarks>
	///     This method is used to reset the Text property to its default state (an empty string).
	///     It is typically called when a new submission process is initiated.
	/// </remarks>
	public void Clear()
	{
		Text = "";
	}

	/// <summary>
	///     Creates a copy of the current instance of the SubmitCandidateRequisition class.
	/// </summary>
	/// <returns>
	///     A new instance of the SubmitCandidateRequisition class with the same values as the current instance.
	/// </returns>
	public SubmitCandidateRequisition Copy() => MemberwiseClone() as SubmitCandidateRequisition;
}