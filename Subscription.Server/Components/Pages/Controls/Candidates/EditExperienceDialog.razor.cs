#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.Server
// File Name:           EditExperienceDialog.razor.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu, Brijesh Dubey
// Created On:          12-03-2024 15:12
// Last Updated On:     07-26-2025 15:29
// *****************************************/

#endregion

namespace Subscription.Server.Components.Pages.Controls.Candidates;

/// <summary>
///     Represents a dialog for editing a candidate's experience in the application.
/// </summary>
/// <remarks>
///     This dialog is used within the Candidate page to allow users to modify the experience details of a candidate.
///     It provides functionalities to show the dialog, save the changes, and cancel the operation.
/// </remarks>
public partial class EditExperienceDialog
{
    private readonly CandidateExperienceValidator _candidateExperienceValidator = new();

    /// <summary>
	///     Gets or sets the event callback that is invoked when the cancel action is triggered in the dialog.
	/// </summary>
	/// <remarks>
	///     This event callback is used to handle the cancellation of the editing operation in the dialog.
	///     It is invoked when the user clicks on the cancel button in the dialog.
	/// </remarks>
	[Parameter]
    public EventCallback<MouseEventArgs> Cancel { get; set; }

	/// <summary>
	///     Gets or sets the edit context used for validating the form in the EditExperienceDialog.
	/// </summary>
	/// <remarks>
	///     This context is used to manage the state of the form and perform validation when fields are changed.
	///     It is initialized with the candidate's experience model and is validated when the dialog is opened.
	/// </remarks>
	private EditContext Context { get; set; }

	/// <summary>
	///     Gets or sets the instance of the Syncfusion Blazor Dialog component used in the EditExperienceDialog.
	/// </summary>
	/// <remarks>
	///     This dialog is used to display and edit the experience details of a candidate.
	///     It is shown or hidden using the ShowDialog and CallCancelMethod methods respectively.
	/// </remarks>
	private SfDialog Dialog { get; set; }

	/// <summary>
	///     Gets or sets the form used for editing a candidate's experience.
	/// </summary>
	/// <remarks>
	///     This form is used within the EditExperienceDialog to capture the details of a candidate's experience.
	///     It includes fields for the employer, description, location, title, and start and end dates of the experience.
	/// </remarks>
	private SfDataForm EditExperienceForm { get; set; }

	/// <summary>
	///     Gets or sets the candidate's experience that is being edited in the dialog.
	/// </summary>
	/// <value>
	///     The candidate's experience.
	/// </value>
	/// <remarks>
	///     This property is bound to the form fields in the dialog, and changes to this property
	///     are reflected in the form and vice versa.
	/// </remarks>
	[Parameter]
    public CandidateExperience Model { get; set; }

	/// <summary>
	///     Gets or sets the event callback that is invoked when the save action is triggered in the dialog.
	/// </summary>
	/// <remarks>
	///     This event callback is used to handle the saving of the editing operation in the dialog.
	///     It is invoked when the user clicks on the save button in the dialog.
	/// </remarks>
	[Parameter]
    public EventCallback<EditContext> Save { get; set; }

    private bool VisibleSpinner { get; set; }

	/// <summary>
	///     Asynchronously cancels the operation of editing a candidate's experience.
	/// </summary>
	/// <param name="args">The mouse event arguments associated with the cancel action.</param>
	/// <remarks>
	///     This method is invoked when the user decides to cancel the operation of editing a candidate's experience.
	///     It calls the 'CallCancelMethod' of the 'General' class, passing the necessary parameters to hide the dialog
	///     and enable the dialog buttons.
	/// </remarks>
	/// <returns>A task that represents the asynchronous operation.</returns>
	private async Task CancelExperienceDialog(MouseEventArgs args)
    {
        VisibleSpinner = true;
        await Cancel.InvokeAsync(args);
        await Dialog.HideAsync();
        VisibleSpinner = false;
    }

    protected override void OnParametersSet()
    {
		if (Context?.Model != Model)
		{
			Context = null; // Immediate reference cleanup for GC
			Context = new(Model);
		}
        base.OnParametersSet();
    }

	/// <summary>
	///     Validates the form context when the dialog is opened.
	/// </summary>
	/// <remarks>
	///     This method is invoked when the dialog is opened. It validates the form context
	///     associated with the editing of a candidate's experience. If the form context is not valid,
	///     the form will not be submitted.
	/// </remarks>
	private void OpenDialog() => Context.Validate();

	/// <summary>
	///     Asynchronously saves the changes made in the EditExperienceDialog.
	/// </summary>
	/// <param name="editContext">The edit context associated with the save action.</param>
	/// <returns>A task that represents the asynchronous operation.</returns>
	/// <remarks>
	///     This method invokes the CallSaveMethod from the General class, passing in the edit context, spinner, footer dialog,
	///     dialog, and save event callback.
	///     It is responsible for executing the save operation when the user confirms the changes in the EditExperienceDialog.
	/// </remarks>
	private async Task SaveExperienceDialog(EditContext editContext)
    {
        VisibleSpinner = true;
        await Save.InvokeAsync(editContext);
        await Dialog.HideAsync();
        VisibleSpinner = false;
    }

	/// <summary>
	///     Asynchronously shows the dialog for editing a candidate's experience.
	/// </summary>
	/// <returns>
	///     A <see cref="Task" /> that represents the asynchronous operation.
	/// </returns>
	/// <remarks>
	///     This method is used to display the dialog for editing the experience details of a candidate.
	///     It is invoked in the `Candidate.EditExperience()` method when the system is prepared for the editing operation.
	/// </remarks>
	public async Task ShowDialog() => await Dialog.ShowAsync();
}