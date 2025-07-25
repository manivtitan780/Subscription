#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.Server
// File Name:           EducationPanel.razor.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu, Brijesh Dubey
// Created On:          02-05-2025 20:02
// Last Updated On:     07-25-2025 15:43
// *****************************************/

#endregion

namespace Subscription.Server.Components.Pages.Controls.Candidates;

/// <summary>
///     The EducationPanel class is a component that represents the education details of a candidate.
///     It provides functionalities to view, edit, and delete the education details of a candidate.
/// </summary>
/// <remarks>
///     The EducationPanel class includes several parameters:
///     - DeleteEducation: An event callback that triggers when an education detail is to be deleted.
///     - EditEducation: An event callback that triggers when an education detail is to be edited.
///     - EditRights: A boolean value indicating whether the user has the rights to edit the education details.
///     - GridEducation: A grid that displays the education details of a candidate.
///     - IsRequisition: A boolean value indicating whether the current operation is a requisition.
///     - Model: A list of CandidateEducation objects representing the education details of a candidate.
///     - RowHeight: The height of a row in the grid.
///     - SelectedRow: The selected row in the grid.
/// </remarks>
public partial class EducationPanel
{
    private int _selectedID;

	/// <summary>
	///     Gets or sets the event callback that triggers when an education detail is to be deleted.
	/// </summary>
	/// <value>
	///     The event callback for deleting an education detail.
	/// </value>
	[Parameter]
    public EventCallback<int> DeleteEducation { get; set; }

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
	///     This service is injected into the component and used in methods like <see cref="DeleteEducationMethod" /> to
	///     confirm actions before proceeding.
	/// </remarks>
	[Inject]
    private SfDialogService DialogService { get; set; }

	/// <summary>
	///     Gets or sets the event callback that triggers when an education detail is to be edited.
	/// </summary>
	/// <value>
	///     The event callback for editing an education detail.
	/// </value>
	[Parameter]
    public EventCallback<int> EditEducation { get; set; }

	/// <summary>
	///     Gets or sets a value indicating whether the user has the rights to edit the education details.
	/// </summary>
	/// <value>
	///     true if the user has the rights to edit; otherwise, false. The default value is true.
	/// </value>
	[Parameter]
    public bool EditRights { get; set; } = true;

	/// <summary>
	///     Gets or sets the grid that displays the education details of a candidate.
	/// </summary>
	/// <value>
	///     The grid displaying the education details of a candidate.
	/// </value>
	private SfGrid<CandidateEducation> GridEducation { get; set; }

	/// <summary>
	///     Gets or sets a value indicating whether the current operation is a requisition.
	/// </summary>
	/// <value>
	///     true if the current operation is a requisition; otherwise, false. The default value is false.
	/// </value>
	[Parameter]
    public bool IsRequisition { get; set; }

	/// <summary>
	///     Gets or sets a list of CandidateEducation objects representing the education details of a candidate.
	/// </summary>
	/// <value>
	///     A list of CandidateEducation objects.
	/// </value>
	[Parameter]
    public CandidateEducation[] Model //List<CandidateEducation> 
    {
        get;
        set;
    }

	/// <summary>
	///     Gets or sets the height of a row in the grid.
	/// </summary>
	/// <value>
	///     The height of a row in the grid. The default value is 38.
	/// </value>
	[Parameter]
    public int RowHeight { get; set; } = 42;

	/// <summary>
	///     Gets the selected row in the grid. This property is of type <see cref="Subscription.Model.CandidateEducation" />.
	/// </summary>
	/// <value>
	///     The selected row in the grid, represented as a CandidateEducation object.
	/// </value>
	/// <remarks>
	///     This property is set internally when a row is selected in the grid. It is used to hold the education details of the
	///     selected candidate.
	/// </remarks>
	internal CandidateEducation SelectedRow { get; private set; }

	/// <summary>
	///     Gets or sets the logged-in Username.
	/// </summary>
	/// <value>
	///     The currently logged-in Username.
	/// </value>
	[Parameter]
    public string UserName { get; set; }

	/// <summary>
	///     Asynchronously deletes the education detail of a candidate.
	/// </summary>
	/// <param name="id">The ID of the education detail to be deleted.</param>
	/// <returns>A Task representing the asynchronous operation.</returns>
	/// <remarks>
	///     This method sets the selected ID to the provided ID, gets the index of the row in the grid corresponding to the ID,
	///     selects the row in the grid, and shows a confirmation dialog.
	/// </remarks>
	private async Task DeleteEducationMethod(int id)
    {
        _selectedID = id;
        int _index = await GridEducation.GetRowIndexByPrimaryKeyAsync(id);
        await GridEducation.SelectRowAsync(_index);
        if (await DialogService.ConfirmAsync(null, "Delete Education", General.DialogOptions("Are you sure you want to <strong>delete</strong> this <i>Candidate Education</i>?")))
        {
            await DeleteEducation.InvokeAsync(_selectedID);
        }
    }

	/// <summary>
	///     Asynchronously opens the dialog for editing the education details of a candidate.
	/// </summary>
	/// <param name="id">The ID of the education detail to be edited.</param>
	/// <returns>A Task representing the asynchronous operation.</returns>
	/// <remarks>
	///     This method sets the selected ID to the provided ID, gets the index of the row in the grid corresponding to the
	///     provided ID, selects the row in the grid, and invokes the EditEducation event callback.
	/// </remarks>
	private async Task EditEducationDialog(int id)
    {
        _selectedID = id;
        int _index = await GridEducation.GetRowIndexByPrimaryKeyAsync(id);
        await GridEducation.SelectRowAsync(_index);
        await EditEducation.InvokeAsync(id);
    }

	/// <summary>
	///     Handles the row selection event in the education details grid.
	/// </summary>
	/// <param name="education">The event arguments containing the selected row data of type <see cref="CandidateEducation" />.</param>
	/// <remarks>
	///     This method is triggered when a row is selected in the education details grid.
	///     It sets the SelectedRow property to the data of the selected row.
	/// </remarks>
	private void RowSelected(RowSelectEventArgs<CandidateEducation> education)
    {
        if (education != null)
        {
            SelectedRow = education.Data;
        }
    }
}