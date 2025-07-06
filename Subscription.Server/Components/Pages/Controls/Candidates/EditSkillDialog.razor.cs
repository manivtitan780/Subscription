#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.Server
// File Name:           EditSkillDialog.razor.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu
// Created On:          11-27-2024 15:11
// Last Updated On:     11-28-2024 15:11
// *****************************************/

#endregion

namespace Subscription.Server.Components.Pages.Controls.Candidates;

/// <summary>
///     Represents a dialog for editing the skills of a candidate in the application.
///     This dialog is used in the context of the Candidate page.
/// </summary>
/// <remarks>
///     The dialog provides parameters for handling the Cancel and Save events,
///     as well as for setting and getting the Model of type CandidateSkills.
///     The ShowDialog method can be used to display the dialog.
/// </remarks>
public partial class EditSkillDialog
{
    private readonly CandidateSkillsValidator _candidateSkillsValidator = new();

    /// <summary>
    ///     Gets or sets the event callback that is invoked when the cancel action is triggered in the dialog.
    /// </summary>
    /// <remarks>
    ///     This event callback is used to handle the cancellation of the editing operation in the dialog.
    ///     It is invoked when the user clicks on the cancel button in the dialog.
    /// </remarks>
    [Parameter]
    public EventCallback<MouseEventArgs> Cancel { get; set; }

    private EditContext Context { get; set; }

    /// <summary>
    ///     Gets or sets the instance of the Syncfusion Blazor Dialog component used in the EditSkillDialog.
    /// </summary>
    /// <remarks>
    ///     This dialog is used to display and edit the experience details of a candidate.
    ///     It is shown or hidden using the ShowDialog and CallCancelMethod methods respectively.
    /// </remarks>
    private SfDialog Dialog { get; set; }

    /// <summary>
    ///     Gets or sets the notes that is being edited in the dialog.
    /// </summary>
    /// <value>
    ///     The notes.
    /// </value>
    /// <remarks>
    ///     This property is bound to the form fields in the dialog, and changes to this property
    ///     are reflected in the form and vice versa.
    /// </remarks>
    [Parameter]
    public CandidateSkills Model { get; set; } = new();

    /// <summary>
    ///     Gets or sets the event callback that is invoked when the save action is triggered in the dialog.
    /// </summary>
    /// <remarks>
    ///     This event callback is used to handle the saving of the editing operation in the dialog.
    ///     It is invoked when the user clicks on the save button in the dialog.
    /// </remarks>
    [Parameter]
    public EventCallback<EditContext> Save { get; set; }

    /// <summary>
    ///     Gets or sets the instance of the Syncfusion spinner control used in the dialog.
    /// </summary>
    /// <remarks>
    ///     This spinner control is displayed when the dialog is performing an operation such as saving or canceling.
    ///     The visibility of the spinner is controlled programmatically based on the state of the operation.
    /// </remarks>
    private SfSpinner Spinner { get; set; }

    /// <summary>
    ///     Asynchronously cancels the operation of editing a candidate's skill.
    /// </summary>
    /// <param name="args">The mouse event arguments associated with the cancel action.</param>
    /// <remarks>
    ///     This method is invoked when the user decides to cancel the operation of editing a candidate's skill.
    ///     It calls the 'CallCancelMethod' of the 'General' class, passing the necessary parameters to hide the dialog
    ///     and enable the dialog buttons.
    /// </remarks>
    /// <returns>A task that represents the asynchronous operation.</returns>
    private async Task CancelSkillDialog(MouseEventArgs args)
    {
        await General.DisplaySpinner(Spinner);
        await Cancel.InvokeAsync(args);
        await Dialog.HideAsync();
        await General.DisplaySpinner(Spinner, false);
    }

     /// <summary>
    ///     Validates the form context when the dialog is opened.
    /// </summary>
    /// <remarks>
    ///     This method is invoked when the dialog is opened. It validates the form context
    ///     associated with the editing of a candidate's skill. If the form context is not valid,
    ///     the form will not be submitted.
    /// </remarks>
    private void OpenDialog() => Context.Validate();

    /// <summary>
    ///     Asynchronously saves the changes made in the EditSkillDialog.
    /// </summary>
    /// <param name="editContext">The edit context associated with the save action.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <remarks>
    ///     This method invokes the CallSaveMethod from the General class, passing in the edit context, spinner, footer dialog,
    ///     dialog, and save event callback.
    ///     It is responsible for executing the save operation when the user confirms the changes in the EditSkillDialog.
    /// </remarks>
    private async Task SaveSkillDialog(EditContext editContext)
    {
        await General.DisplaySpinner(Spinner);
        await Save.InvokeAsync(editContext);
        await Dialog.HideAsync();
        await General.DisplaySpinner(Spinner, false);
    }
    private CandidateSkills _candSkills = new();

    protected override Task OnParametersSetAsync()
    {
        // ReSharper disable once InvertIf
        if (_candSkills != Model)
        {
            _candSkills = Model;
            Context = new(Model);
        }
 
        //Context.OnFieldChanged += Context_OnFieldChanged;
        return base.OnParametersSetAsync();
    }

    /// <summary>
    ///     Asynchronously shows the dialog for editing a candidate's skill.
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