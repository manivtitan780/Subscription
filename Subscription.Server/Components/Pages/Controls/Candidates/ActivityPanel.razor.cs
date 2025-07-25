#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.Server
// File Name:           ActivityPanel.razor.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu, Brijesh Dubey
// Created On:          03-03-2025 20:03
// Last Updated On:     05-19-2025 15:50
// *****************************************/

#endregion

namespace Subscription.Server.Components.Pages.Controls.Candidates;

/// <summary>
///     Represents a panel for managing candidate activities.
/// </summary>
/// <remarks>
///     This class provides functionality for editing, undoing, and managing candidate activities.
///     It also allows for the customization of the panel's display, such as setting the row height.
/// </remarks>
public partial class ActivityPanel
{
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
    /// </remarks>
    [Inject]
    private SfDialogService DialogService { get; set; }

    /// <summary>
    ///     Gets or sets the event callback that is invoked when an activity is edited.
    /// </summary>
    /// <value>
    ///     The event callback for editing an activity.
    /// </value>
    /// <remarks>
    ///     This callback is invoked with the ID of the activity to be edited.
    /// </remarks>
    [Parameter]
    public EventCallback<int> EditActivity { get; set; }

    /// <summary>
    ///     Gets or sets the Syncfusion Blazor Grid for displaying candidate activities.
    /// </summary>
    /// <value>
    ///     The Syncfusion Blazor Grid for candidate activities.
    /// </value>
    /// <remarks>
    ///     This property is used to manage the display of candidate activities in a grid format.
    ///     It provides functionalities such as getting the row index by primary key and selecting a row asynchronously.
    /// </remarks>
    private SfGrid<CandidateActivity> GridActivity { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether the current activity is a requisition.
    /// </summary>
    /// <value>
    ///     <c>true</c> if this activity is a requisition; otherwise, <c>false</c>.
    /// </value>
    /// <remarks>
    ///     This property is used to determine the type of the activity. If it's a requisition, certain features or behaviors
    ///     of the panel might change.
    /// </remarks>
    [Parameter]
    public bool IsRequisition { get; set; }

    /// <summary>
    ///     Gets or sets the list of candidate activities.
    /// </summary>
    /// <value>
    ///     The list of candidate activities.
    /// </value>
    /// <remarks>
    ///     This property is used to store and manage the activities of candidates. Each activity is represented by an instance
    ///     of the <see cref="CandidateActivity" /> class.
    /// </remarks>
    [Parameter]
    // Memory optimization: Changed from List<CandidateActivity> to CandidateActivity[] for better cache locality
    public CandidateActivity[] Model { get; set; }

    /// <summary>
    ///     Gets or sets the RoleID associated with the logged-in user.
    /// </summary>
    /// <value>
    ///     String value containing the	RoleID associated with the logged-in user.
    /// </value>
    /// <remarks>
    ///     This property is used to check the role of the user and to determine the rights for the activity.
    /// </remarks>
    [Parameter]
    public int RoleID { get; set; } = 5;

    /// <summary>
    ///     Gets or sets the height of the rows in the activity panel.
    /// </summary>
    /// <value>
    ///     The height of the rows in pixels.
    /// </value>
    /// <remarks>
    ///     This property is used to control the display of the activity panel. It determines the height of each row in the
    ///     grid. The default value is 38 pixels.
    /// </remarks>
    [Parameter]
    public int RowHeight { get; set; } = 38;

    /// <summary>
    ///     Gets or sets the selected row in the activity panel.
    /// </summary>
    /// <value>
    ///     The selected row, represented by an instance of the <see cref="CandidateActivity" /> class.
    /// </value>
    /// <remarks>
    ///     This property is used to keep track of the user's current selection in the activity panel.
    ///     It is updated whenever a row is selected, and is used in various operations such as editing or undoing an activity.
    /// </remarks>
    internal CandidateActivity SelectedRow { get; private set; }

    /// <summary>
    ///     Gets or sets the event callback that is invoked when an activity is undone.
    /// </summary>
    /// <value>
    ///     The event callback for undoing an activity.
    /// </value>
    /// <remarks>
    ///     This callback is invoked with the ID of the activity to be undone.
    /// </remarks>
    [Parameter]
    public EventCallback<int> UndoCandidateActivity { get; set; }

    [Parameter]
    public string User { get; set; }

    /// <summary>
    ///     Asynchronously opens the dialog for editing a candidate activity.
    /// </summary>
    /// <param name="id">
    ///     The ID of the candidate activity to be edited.
    /// </param>
    /// <remarks>
    ///     This method first finds the row index of the activity with the given ID in the grid,
    ///     then selects that row, and finally invokes the EditActivity event callback with the given ID.
    /// </remarks>
    /// <returns>
    ///     A <see cref="Task" /> representing the asynchronous operation.
    /// </returns>
    private async Task EditActivityDialog(int id)
    {
        int _index = await GridActivity.GetRowIndexByPrimaryKeyAsync(id);
        await GridActivity.SelectRowAsync(_index);
        await EditActivity.InvokeAsync(id);
    }

    /// <summary>
    ///     Asynchronously handles the data bound event of the grid.
    /// </summary>
    /// <param name="arg">The event data.</param>
    /// <remarks>
    ///     This method is invoked when the grid's data is bound. If there are any rows in the grid,
    ///     it selects the first row asynchronously.
    /// </remarks>
    private async Task GridBound(object arg)
    {
        if (GridActivity.CurrentViewData.Any())
        {
            await GridActivity.SelectRowAsync(0);
        }
    }

    /// <summary>
    ///     Handles the event when a row is selected in the activity panel.
    /// </summary>
    /// <param name="activity">
    ///     The event arguments containing the data of the selected row, represented by an instance of the
    ///     <see cref="CandidateActivity" /> class.
    /// </param>
    /// <remarks>
    ///     This method updates the <see cref="SelectedRow" /> property with the data of the selected row.
    ///     It is invoked when a row in the activity panel is selected by the user.
    /// </remarks>
    private void RowSelected(RowSelectEventArgs<CandidateActivity> activity)
    {
        if (activity != null)
        {
            SelectedRow = activity.Data;
        }
    }

    /// <summary>
    ///     Gets or sets the event callback that is invoked when an activity is undone.
    /// </summary>
    /// <value>
    ///     The event callback for undoing an activity.
    /// </value>
    /// <remarks>
    ///     This callback is invoked with the ID of the activity to be undone.
    ///     It is used when the user wants to revert the changes made to a candidate's activity.
    /// </remarks>
    private async Task UndoActivity(int activityID)
    {
        int _index = await GridActivity.GetRowIndexByPrimaryKeyAsync(activityID);
        await GridActivity.SelectRowAsync(_index);
        if (await DialogService.ConfirmAsync(null, "Undo Candidate Activity?",
                                             General.DialogOptions("Are you sure you want to <strong>undo</strong> this Candidate Activity?<br/><br/>Note: This action is irreversible.")))
        {
            await UndoCandidateActivity.InvokeAsync(activityID);
        }
    }

    /*
    private async Task DetailExpanded(DetailsExpandedEventArgs<CandidateActivity> activity)
    {
        /*if (activity != null)
        {
            await GridActivity.SelectRowAsync(activity.RowIndex - 1);
            SelectedRow = activity.Data;
        }#1#
    }
*/
}