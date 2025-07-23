#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.Server
// File Name:           RequisitionNotesPanel.razor.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu, Brijesh Dubey
// Created On:          07-23-2025 21:07
// Last Updated On:     07-23-2025 21:11
// *****************************************/

#endregion

namespace Subscription.Server.Components.Pages.Controls.Requisitions;

public partial class RequisitionNotesPanel : ComponentBase
{
    private int _selectedID;

	/// <summary>
	///     Gets or sets the event callback that is triggered when a note entry is to be deleted.
	/// </summary>
	/// <value>
	///     The event callback that takes an integer parameter representing the ID of the note entry to be deleted.
	/// </value>
	[Parameter]
    public EventCallback<int> DeleteNotes { get; set; }

	/// <summary>
	///     Gets or sets the dialog service used for displaying confirmation dialogs.
	/// </summary>
	/// <value>
	///     An instance of <see cref="SfDialogService" /> that provides methods for showing dialogs and handling user
	///     interactions
	///     with those dialogs.
	/// </value>
	/// <remarks>
	///     The <see cref="SfDialogService" /> is used to display confirmation dialogs to the user. It provides methods such as
	///     <see cref="SfDialogService.ConfirmAsync" /> to show a confirmation dialog and await the user's response.
	///     This service is injected into the component and used in methods to confirm actions before proceeding.
	/// </remarks>
	[Inject]
    private SfDialogService DialogService { get; set; }

	/// <summary>
	///     Gets or sets the event callback that is triggered when a note entry is to be edited.
	/// </summary>
	/// <value>
	///     The event callback that takes an integer parameter representing the ID of the note entry to be edited.
	/// </value>
	[Parameter]
    public EventCallback<int> EditNotes { get; set; }

	/// <summary>
	///     Gets or sets a value indicating whether the current user has rights to edit the note entries.
	/// </summary>
	/// <value>
	///     true if the current user has edit rights; otherwise, false. The default is true.
	/// </value>
	/// <remarks>
	///     This property is a boolean flag that determines if the current user has the rights to edit the note entries.
	///     If EditRights is true, the user can edit the entries; if EditRights is false, the user cannot. The default value is
	///     true, meaning that, by default, users have the rights to edit the entries.
	/// </remarks>
	[Parameter]
    public bool EditRights { get; set; } = true;

	/// <summary>
	///     Gets or sets the Syncfusion Blazor Grid component that displays the list of a candidate's notes.
	/// </summary>
	/// <value>
	///     The Syncfusion Blazor Grid component that displays the candidate's notes.
	/// </value>
	/// <remarks>
	///     The GridExperience property is a Syncfusion Blazor Grid component that displays a list of a candidate's
	///     notes.
	///     Each row in the grid represents a note entry of the candidate.
	///     The grid provides functionalities such as sorting, filtering, and paging.
	///     The grid is bound to the Model property, which provides the data source for the grid.
	/// </remarks>
	private SfGrid<CandidateNotes> GridNotes { get; set; }

	/// <summary>
	///     Gets or sets the model for the note entries.
	/// </summary>
	/// <value>
	///     A list of CandidateNotes objects representing the note entries of a candidate.
	/// </value>
	/// <remarks>
	///     The Model property is a list of CandidateNotes objects that represent the note entries of a candidate.
	///     These entries are displayed in a grid in the NotesPanel.
	///     Each entry includes details about a particular note of the candidate.
	/// </remarks>
	[Parameter]
    public List<CandidateNotes> Model { get; set; }

	/// <summary>
	///     Gets or sets the height of each row in the grid displaying the note entries.
	/// </summary>
	/// <value>
	///     An integer representing the height of each row in pixels. The default is 38.
	/// </value>
	/// <remarks>
	///     The RowHeight property determines the height of each row in the grid that displays the note entries of a
	///     candidate.
	///     This property allows for customization of the grid's appearance and can be adjusted to accommodate the amount of
	///     information in each row.
	/// </remarks>
	[Parameter]
    public int RowHeight { get; set; } = 42;

	/// <summary>
	///     Gets or sets the currently selected row in the grid.
	/// </summary>
	/// <value>
	///     A CandidateNotes object representing the currently selected note entry in the grid.
	/// </value>
	/// <remarks>
	///     The SelectedRow property is used to keep track of the currently selected row in the grid that displays the
	///     note entries of a candidate.
	///     This property is updated whenever a row is selected in the grid, and it is used to perform operations on the
	///     selected note entry, such as editing or deleting the entry.
	/// </remarks>
	internal CandidateNotes SelectedRow { get; private set; }

	/// <summary>
	///     Gets or sets the logged-in Username.
	/// </summary>
	/// <value>
	///     The currently logged-in Username.
	/// </value>
	[Parameter]
    public string UserName { get; set; }

	/// <summary>
	///     Asynchronously deletes the note detail of a candidate.
	/// </summary>
	/// <param name="id">The ID of the note detail to be deleted.</param>
	/// <returns>A Task representing the asynchronous operation.</returns>
	/// <remarks>
	///     This method sets the selected ID to the provided ID, gets the index of the row in the grid corresponding to the ID,
	///     selects the row in the grid, and shows a confirmation dialog.
	/// </remarks>
	private async Task DeleteNotesMethod(int id)
    {
        _selectedID = id;
        int _index = await GridNotes.GetRowIndexByPrimaryKeyAsync(id);
        await GridNotes.SelectRowAsync(_index);
        if (await DialogService.ConfirmAsync(null, "Delete Notes", General.DialogOptions("Are you sure you want to <strong>delete</strong> this <i>Candidate Notes</i>?")))
        {
            await DeleteNotes.InvokeAsync(_selectedID);
        }
    }

	/// <summary>
	///     Asynchronously opens the dialog for editing the note details of a candidate.
	/// </summary>
	/// <param name="id">The ID of the note detail to be edited.</param>
	/// <returns>A Task representing the asynchronous operation.</returns>
	/// <remarks>
	///     This method sets the selected ID to the provided ID, gets the index of the row in the grid corresponding to the
	///     provided ID, selects the row in the grid, and invokes the EditExperience event callback.
	/// </remarks>
	private async Task EditNotesDialog(int id)
    {
        _selectedID = id;
        int _index = await GridNotes.GetRowIndexByPrimaryKeyAsync(id);
        await GridNotes.SelectRowAsync(_index);
        await EditNotes.InvokeAsync(id);
    }

	/// <summary>
	///     Memory optimization: Efficiently processes notes text for tooltip display.
	///     Replaces HTML break tags with newlines for clean text display.
	/// </summary>
	/// <param name="notes">The notes text containing HTML break tags</param>
	/// <returns>Clean text with newlines instead of HTML breaks</returns>
	private static string GetCleanNotesText(string notes) => string.IsNullOrEmpty(notes) ? "" : notes.Replace("<br>", Environment.NewLine).Replace("<br/>", Environment.NewLine);

	/// <summary>
	///     Memory optimization: Formats the update information using existing CultureDate extension method.
	///     Leverages the application's consistent date formatting pattern.
	/// </summary>
	/// <param name="note">The candidate note containing update information</param>
	/// <returns>Formatted string with date and user information</returns>
	private static string GetFormattedUpdateInfo(CandidateNotes note) => $"{note.UpdatedDate.CultureDate()} [{note.UpdatedBy}]";

	/// <summary>
	///     Handles the row selection event in the note details grid.
	/// </summary>
	/// <param name="note">
	///     The event arguments containing the selected row data of type
	///     <see cref="CandidateNotes" />.
	/// </param>
	/// <remarks>
	///     This method is triggered when a row is selected in the note details grid.
	///     It sets the SelectedRow property to the data of the selected row.
	/// </remarks>
	private void RowSelected(RowSelectEventArgs<CandidateNotes> note)
    {
        if (note != null)
        {
            SelectedRow = note.Data;
        }
    }
}