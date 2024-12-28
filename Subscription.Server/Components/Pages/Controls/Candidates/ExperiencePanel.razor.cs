#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.Server
// File Name:           ExperiencePanel.razor.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu
// Created On:          12-02-2024 20:12
// Last Updated On:     12-02-2024 21:12
// *****************************************/

#endregion

namespace Subscription.Server.Components.Pages.Controls.Candidates;

/// <summary>
///     Represents a user interface component that provides functionality for managing a candidate's experiences.
/// </summary>
/// <remarks>
///     The ExperiencePanel class is a Blazor component that provides the following functionalities:
///     - Deleting an experience entry (via the DeleteExperience event).
///     - Editing an experience entry (via the EditExperience event).
///     - Displaying a list of experience entries in a grid (via the GridExperience property).
///     - Determining if the current user has rights to edit the experience entries (via the EditRights property).
///     - Determining if the current context is a requisition (via the IsRequisition property).
///     - Providing a model for the experience entries (via the Model property).
///     - Customizing the row height in the grid (via the RowHeight property).
///     - Keeping track of the currently selected row in the grid (via the SelectedRow property).
/// </remarks>
public partial class ExperiencePanel
{
    private int _selectedID;

	/// <summary>
	///     Gets or sets the event callback that is triggered when an experience entry is to be deleted.
	/// </summary>
	/// <value>
	///     The event callback that takes an integer parameter representing the ID of the experience entry to be deleted.
	/// </value>
	[Parameter]
    public EventCallback<int> DeleteExperience
    {
        get;
        set;
    }

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
	///     This service is injected into the component and used in methods like <see cref="DeleteExperienceMethod" /> to
	///     confirm actions before proceeding.
	/// </remarks>
	[Inject]
    private SfDialogService DialogService
    {
        get;
        set;
    }

	/// <summary>
	///     Gets or sets the event callback that is triggered when an experience entry is to be edited.
	/// </summary>
	/// <value>
	///     The event callback that takes an integer parameter representing the ID of the experience entry to be edited.
	/// </value>
	[Parameter]
    public EventCallback<int> EditExperience
    {
        get;
        set;
    }

	/// <summary>
	///     Gets or sets a value indicating whether the current user has rights to edit the experience entries.
	/// </summary>
	/// <value>
	///     true if the current user has edit rights; otherwise, false. The default is true.
	/// </value>
	/// <remarks>
	///     This property is a boolean flag that determines if the current user has the rights to edit the experience entries.
	///     If EditRights is true, the user can edit the entries; if EditRights is false, the user cannot. The default value is
	///     true, meaning that, by default, users have the rights to edit the entries.
	/// </remarks>
	[Parameter]
    public bool EditRights
    {
        get;
        set;
    } = true;

	/// <summary>
	///     Gets or sets the Syncfusion Blazor Grid component that displays the list of a candidate's experiences.
	/// </summary>
	/// <value>
	///     The Syncfusion Blazor Grid component that displays the candidate's experiences.
	/// </value>
	/// <remarks>
	///     The GridExperience property is a Syncfusion Blazor Grid component that displays a list of a candidate's
	///     experiences.
	///     Each row in the grid represents an experience entry of the candidate.
	///     The grid provides functionalities such as sorting, filtering, and paging.
	///     The grid is bound to the Model property, which provides the data source for the grid.
	/// </remarks>
	private SfGrid<CandidateExperience> GridExperience
    {
        get;
        set;
    }

	/// <summary>
	///     Gets or sets a value indicating whether the current context is a requisition.
	/// </summary>
	/// <value>
	///     true if the current context is a requisition; otherwise, false. The default is false.
	/// </value>
	/// <remarks>
	///     This property is a boolean flag that determines if the current context is a requisition. If IsRequisition is true,
	///     the context is a requisition; if IsRequisition is false, the context is not a requisition. The default value is
	///     false, meaning that, by default, the context is not a requisition.
	/// </remarks>
	[Parameter]
    public bool IsRequisition
    {
        get;
        set;
    }

	/// <summary>
	///     Gets or sets the model for the experience entries.
	/// </summary>
	/// <value>
	///     A list of CandidateExperience objects representing the experience entries of a candidate.
	/// </value>
	/// <remarks>
	///     The Model property is a list of CandidateExperience objects that represent the experience entries of a candidate.
	///     These entries are displayed in a grid in the ExperiencePanel.
	///     Each entry includes details about a particular experience of the candidate.
	/// </remarks>
	[Parameter]
    public List<CandidateExperience> Model
    {
        get;
        set;
    }

	/// <summary>
	///     Gets or sets the height of each row in the grid displaying the experience entries.
	/// </summary>
	/// <value>
	///     An integer representing the height of each row in pixels. The default is 38.
	/// </value>
	/// <remarks>
	///     The RowHeight property determines the height of each row in the grid that displays the experience entries of a
	///     candidate.
	///     This property allows for customization of the grid's appearance and can be adjusted to accommodate the amount of
	///     information in each row.
	/// </remarks>
	[Parameter]
    public int RowHeight
    {
        get;
        set;
    } = 42;

	/// <summary>
	///     Gets or sets the currently selected row in the grid.
	/// </summary>
	/// <value>
	///     A CandidateExperience object representing the currently selected experience entry in the grid.
	/// </value>
	/// <remarks>
	///     The SelectedRow property is used to keep track of the currently selected row in the grid that displays the
	///     experience entries of a candidate.
	///     This property is updated whenever a row is selected in the grid, and it is used to perform operations on the
	///     selected experience entry, such as editing or deleting the entry.
	/// </remarks>
	internal CandidateExperience SelectedRow
    {
        get;
        private set;
    }

	/// <summary>
	///     Gets or sets the logged-in Username.
	/// </summary>
	/// <value>
	///     The currently logged-in Username.
	/// </value>
	[Parameter]
    public string UserName
    {
        get;
        set;
    }

	/// <summary>
	///     Asynchronously deletes the experience detail of a candidate.
	/// </summary>
	/// <param name="id">The ID of the experience detail to be deleted.</param>
	/// <returns>A Task representing the asynchronous operation.</returns>
	/// <remarks>
	///     This method sets the selected ID to the provided ID, gets the index of the row in the grid corresponding to the ID,
	///     selects the row in the grid, and shows a confirmation dialog.
	/// </remarks>
	private async Task DeleteExperienceMethod(int id)
    {
        _selectedID = id;
        int _index = await GridExperience.GetRowIndexByPrimaryKeyAsync(id);
        await GridExperience.SelectRowAsync(_index);
		if (await DialogService.ConfirmAsync(null, "Delete Experience", General.DialogOptions("Are you sure you want to <strong>disable</strong> this <i>Candidate Experience</i>?")))
		{
			await DeleteExperience.InvokeAsync(_selectedID);
		}
    }

	/// <summary>
	///     Asynchronously opens the dialog for editing the experience details of a candidate.
	/// </summary>
	/// <param name="id">The ID of the experience detail to be edited.</param>
	/// <returns>A Task representing the asynchronous operation.</returns>
	/// <remarks>
	///     This method sets the selected ID to the provided ID, gets the index of the row in the grid corresponding to the
	///     provided ID, selects the row in the grid, and invokes the EditExperience event callback.
	/// </remarks>
	private async Task EditExperienceDialog(int id)
    {
        _selectedID = id;
        int _index = await GridExperience.GetRowIndexByPrimaryKeyAsync(id);
        await GridExperience.SelectRowAsync(_index);
        await EditExperience.InvokeAsync(id);
    }

	/// <summary>
	///     Handles the row selection event in the experience details grid.
	/// </summary>
	/// <param name="experience">
	///     The event arguments containing the selected row data of type
	///     <see cref="CandidateExperience" />.
	/// </param>
	/// <remarks>
	///     This method is triggered when a row is selected in the experience details grid.
	///     It sets the SelectedRow property to the data of the selected row.
	/// </remarks>
	private void RowSelected(RowSelectEventArgs<CandidateExperience> experience)
    {
        if (experience != null)
        {
            SelectedRow = experience.Data;
        }
    }
}