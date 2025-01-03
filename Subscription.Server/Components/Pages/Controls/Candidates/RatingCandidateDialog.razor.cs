#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.Server
// File Name:           MPCCandidateDialog.razor.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu
// Created On:          01-01-2025 19:01
// Last Updated On:     01-01-2025 21:01
// *****************************************/

#endregion

namespace Subscription.Server.Components.Pages.Controls.Candidates;

/// <summary>
///     Represents a dialog for managing Most Placeable Candidate (MPC) data.
/// </summary>
/// <remarks>
///     This dialog provides a user interface for viewing and editing MPC data. It includes parameters for handling events
///     such as saving changes and cancelling edits, as well as properties for controlling the appearance of the dialog,
///     such as the visibility of a spinner and the height of rows in the MPC grid. The dialog can be shown by calling the
///     `ShowDialog` method.
/// </remarks>
public partial class RatingCandidateDialog
{
    private readonly CandidateRatingValidator _candidateRatingValidator = new();

	/// <summary>
	///     Gets or sets the event callback that is invoked when the cancel action is triggered in the dialog.
	/// </summary>
	/// <remarks>
	///     This event callback is used to handle the cancellation of the editing operation in the dialog.
	///     It is invoked when the user clicks on the cancel button in the dialog.
	/// </remarks>
	[Parameter]
    public EventCallback<MouseEventArgs> Cancel
    {
        get;
        set;
    }

    private EditContext Context
    {
        get;
        set;
    }

	/// <summary>
	///     Gets or sets the instance of the Syncfusion Blazor Dialog component used in the MPCCandidateDialog.
	/// </summary>
	/// <remarks>
	///     This dialog is used to display and edit the experience details of a candidate.
	///     It is shown or hidden using the ShowDialog and CallCancelMethod methods respectively.
	/// </remarks>
	private SfDialog Dialog
    {
        get;
        set;
    }

	/// <summary>
	///     Gets or sets the form used for editing a candidate's MPC record..
	/// </summary>
	/// <remarks>
	///     This form is used within the MPCCandidateDialog to capture the details of a candidate's MPC record..
	///     It includes fields for the employer, description, location, title, and start and end dates of the experience.
	/// </remarks>
	private SfDataForm EditRatingForm
    {
        get;
        set;
    }

	/// <summary>
	///     Gets or sets the MPC that is being edited in the dialog.
	/// </summary>
	/// <value>
	///     The MPC.
	/// </value>
	/// <remarks>
	///     This property is bound to the form fields in the dialog, and changes to this property
	///     are reflected in the form and vice versa.
	/// </remarks>
	[Parameter]
    public CandidateRatingMPC Model
    {
        get;
        set;
    }

	/// <summary>
	///     Gets or sets the list of Most Placeable Candidate (MPC) records displayed in the grid.
	/// </summary>
	/// <value>
	///     The list of MPC records.
	/// </value>
	/// <remarks>
	///     This property is bound to the data source of the grid in the dialog. Changes to this property
	///     are reflected in the grid and vice versa. Each record in the list represents a row in the grid.
	/// </remarks>
	[Parameter]
    public List<CandidateRating> RatingGrid
    {
        get;
        set;
    }

	/// <summary>
	///     Gets or sets the height of the rows in the Most Placeable Candidate (MPC) grid.
	/// </summary>
	/// <value>
	///     The height of the rows in the MPC grid.
	/// </value>
	/// <remarks>
	///     This property is used to control the height of the rows in the MPC grid displayed in the dialog.
	///     The default value is 38.
	/// </remarks>
	[Parameter]
    public int RowHeight
    {
        get;
        set;
    } = 38;

	/// <summary>
	///     Gets or sets the event callback that is invoked when the save action is triggered in the dialog.
	/// </summary>
	/// <remarks>
	///     This event callback is used to handle the saving of the editing operation in the dialog.
	///     It is invoked when the user clicks on the save button in the dialog.
	/// </remarks>
	[Parameter]
    public EventCallback<EditContext> Save
    {
        get;
        set;
    }

	/// <summary>
	///     Gets or sets the instance of the Syncfusion spinner control used in the dialog.
	/// </summary>
	/// <remarks>
	///     This spinner control is displayed when the dialog is performing an operation such as saving or canceling.
	///     The visibility of the spinner is controlled programmatically based on the state of the operation.
	/// </remarks>
	private SfSpinner Spinner
    {
        get;
        set;
    }

	/// <summary>
	///     Asynchronously cancels the operation of editing a candidate's MPC record..
	/// </summary>
	/// <param name="args">The mouse event arguments associated with the cancel action.</param>
	/// <remarks>
	///     This method is invoked when the user decides to cancel the operation of editing a candidate's MPC record..
	///     It calls the 'CallCancelMethod' of the 'General' class, passing the necessary parameters to hide the dialog
	///     and enable the dialog buttons.
	/// </remarks>
	/// <returns>A task that represents the asynchronous operation.</returns>
	private async Task CancelRatingDialog(MouseEventArgs args)
    {
        await General.DisplaySpinner(Spinner);
        await Cancel.InvokeAsync(args);
        await Dialog.HideAsync();
        await General.DisplaySpinner(Spinner, false);
    }

    private void Context_OnFieldChanged(object sender, FieldChangedEventArgs e) => Context.Validate();

    protected override void OnParametersSet()
    {
        Context = new(Model);
        Context.OnFieldChanged += Context_OnFieldChanged;
        base.OnParametersSet();
    }

	/// <summary>
	///     Validates the form context when the dialog is opened.
	/// </summary>
	/// <remarks>
	///     This method is invoked when the dialog is opened. It validates the form context
	///     associated with the editing of a candidate's MPC record. If the form context is not valid,
	///     the form will not be submitted.
	/// </remarks>
	private void OpenDialog(BeforeOpenEventArgs arg) => Context.Validate();

	/// <summary>
	///     Asynchronously saves the changes made in the MPCCandidateDialog.
	/// </summary>
	/// <param name="editContext">The edit context associated with the save action.</param>
	/// <returns>A task that represents the asynchronous operation.</returns>
	/// <remarks>
	///     This method invokes the CallSaveMethod from the General class, passing in the edit context, spinner, footer dialog,
	///     dialog, and save event callback.
	///     It is responsible for executing the save operation when the user confirms the changes in the MPCCandidateDialog.
	/// </remarks>
	private async Task SaveRatingDialog(EditContext editContext)
    {
        await General.DisplaySpinner(Spinner);
        await Save.InvokeAsync(editContext);
        await Dialog.HideAsync();
        await General.DisplaySpinner(Spinner, false);
    }

	/// <summary>
	///     Asynchronously shows the dialog for editing a candidate's MPC record..
	/// </summary>
	/// <returns>
	///     A <see cref="Task" /> that represents the asynchronous operation.
	/// </returns>
	/// <remarks>
	///     This method is used to display the dialog for editing the experience details of a candidate.
	///     It is invoked when the system is prepared for the editing operation.
	/// </remarks>
	public async Task ShowDialog() => await Dialog.ShowAsync();
}