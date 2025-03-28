#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            ProfSvc_AppTrack
// Project:             ProfSvc_AppTrack
// File Name:           ActivityPanelRequisition.razor.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja
// Created On:          12-09-2022 15:57
// Last Updated On:     09-30-2023 19:13
// *****************************************/

#endregion

namespace Subscription.Server.Components.Pages.Controls.Requisitions;

/// <summary>
///     Represents a panel for managing candidate activities in a requisition.
/// </summary>
/// <remarks>
///     This class is used in the context of requisitions, where activities related to candidates are managed.
///     It provides functionalities for editing and undoing candidate activities, and for distinguishing between
///     requisitions for individuals and companies.
/// </remarks>
public partial class ActivityPanelRequisition
{
    /// <summary>
    ///     Gets or sets the EventCallback for editing a candidate activity.
    /// </summary>
    /// <value>
    ///     The EventCallback that is invoked when a candidate activity is to be edited.
    /// </value>
    /// <remarks>
    ///     This EventCallback is invoked when a user initiates the edit action for a specific activity in the activity grid.
    ///     The integer parameter represents the ID of the activity to be edited.
    /// </remarks>
    [Parameter]
    public EventCallback<int> EditActivity
    {
        get;
        set;
    }

    /// <summary>
    ///     Gets or sets the SfGrid control for CandidateActivity in the ActivityPanelRequisition.
    /// </summary>
    /// <value>
    ///     The SfGrid control for CandidateActivity.
    /// </value>
    /// <remarks>
    ///     This property is used to manage the grid display of candidate activities in the activity panel.
    ///     It provides functionalities such as getting the row index by primary key and selecting a row asynchronously.
    /// </remarks>
    private SfGrid<CandidateActivity> GridActivity
    {
        get;
        set;
    }

    /// <summary>
    ///     Gets or sets a value indicating whether the requisition is for a company.
    /// </summary>
    /// <value>
    ///     <c>true</c> if this requisition is for a company; otherwise, <c>false</c>.
    /// </value>
    /// <remarks>
    ///     This property is used to distinguish between requisitions for individuals and companies.
    /// </remarks>
    [Parameter]
    public bool IsCompany
    {
        get;
        set;
    }

    /// <summary>
    ///     Gets or sets the IJSRuntime instance for the ActivityPanelRequisition.
    /// </summary>
    /// <value>
    ///     The IJSRuntime instance used for invoking JavaScript functions from C#.
    /// </value>
    /// <remarks>
    ///     This property is used to interact with the JavaScript runtime in the context of a Blazor component.
    ///     It allows the component to call JavaScript functions and use JavaScript libraries.
    /// </remarks>
    [Inject]
    private IJSRuntime JsRuntime
    {
        get;
        set;
    }

    /// <summary>
    ///     Gets or sets the list of CandidateActivity objects for the ActivityPanelRequisition.
    /// </summary>
    /// <value>
    ///     The list of CandidateActivity objects. Each object represents a candidate's activity.
    /// </value>
    /// <remarks>
    ///     This property is used to populate the activity panel with the activities of the candidates.
    ///     Each CandidateActivity object in the list corresponds to a row in the activity panel.
    /// </remarks>
    [Parameter]
    public List<CandidateActivity> Model
    {
        get;
        set;
    }

    /// <summary>
    ///     Gets or sets the Role ID for the current user in the ActivityPanelRequisition.
    /// </summary>
    /// <value>
    ///     The Role ID of the current user. The default value is "RS".
    /// </value>
    /// <remarks>
    ///     This property is used to determine the user's role for permission checks.
    ///     For example, if the Role ID is "AD", the user has the ability to undo activities regardless of who updated them.
    /// </remarks>
    [Parameter]
    public int RoleID
    {
        get;
        set;
    } = 5;

    /// <summary>
    ///     Gets or sets the height of a row in the ActivityPanelRequisition.
    /// </summary>
    /// <value>
    ///     The height of a row, in pixels. The default value is 38.
    /// </value>
    /// <remarks>
    ///     This property is used to set the height of each row in the activity panel.
    ///     Changing this value affects the display of the activity panel.
    /// </remarks>
    [Parameter]
    public int RowHeight
    {
        get;
        set;
    } = 38;

    /// <summary>
    ///     Gets or sets the selected CandidateActivity in the ActivityPanelRequisition.
    /// </summary>
    /// <remarks>
    ///     This property represents the currently selected row in the activity panel.
    ///     It is used to hold the data of the selected activity for further operations such as editing or undoing an activity.
    /// </remarks>
    public CandidateActivity SelectedRow
    {
        get;
        set;
    }

    /// <summary>
    ///     Gets or sets the EventCallback for undoing a candidate activity.
    /// </summary>
    /// <remarks>
    ///     This EventCallback is invoked when a user confirms the undo action for a specific activity in the activity grid.
    ///     The integer parameter represents the ID of the activity to be undone.
    /// </remarks>
    [Parameter]
    public EventCallback<int> UndoCandidateActivity
    {
        get;
        set;
    }

    /// <summary>
    ///     Gets or sets the user identifier. This identifier is used to check the permissions for editing and undoing
    ///     activities.
    /// </summary>
    [Parameter]
    public string User
    {
        get;
        set;
    }

    /// <summary>
    ///     Asynchronously triggers the editing process for a specific activity in the activity grid.
    /// </summary>
    /// <param name="id">The unique identifier of the activity to be edited.</param>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    private async Task EditActivityDialog(int id)
    {
        int _index = await GridActivity.GetRowIndexByPrimaryKeyAsync(id);
        await GridActivity.SelectRowAsync(_index);
        await EditActivity.InvokeAsync(id);
    }

    /// <summary>
    ///     Handles the event when a row is selected in the grid.
    /// </summary>
    /// <param name="activity">The event arguments containing the selected row data.</param>
    /// <remarks>
    ///     When a row is selected in the grid, this method sets the SelectedRow property to the data of the selected row.
    ///     If the selected row is null, no action is taken.
    /// </remarks>
    private void RowSelected(RowSelectEventArgs<CandidateActivity> activity)
    {
        if (activity != null)
        {
            SelectedRow = activity.Data;
        }
    }

    private async Task GridBound(object arg)
    {
        if (GridActivity.CurrentViewData.Any())
        {
            await GridActivity.SelectRowAsync(0);
        }
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
    /// </remarks>
    [Inject]
    private SfDialogService DialogService
    {
        get;
        set;
    }

    /// <summary>
    ///     Asynchronously undoes a specified activity.
    /// </summary>
    /// <param name="activityID">The ID of the activity to undo.</param>
    /// <remarks>
    ///     This method first selects the row in the grid that corresponds to the given activity ID.
    ///     Then, it displays a confirmation dialog to the user. If the user confirms,
    ///     the method invokes the UndoCandidateActivity event with the activity ID.
    /// </remarks>
    /// <returns>A task that represents the asynchronous operation.</returns>
    private async Task UndoActivity(int activityID)
    {
        /*await Task.Yield();
        int _index = await GridActivity.GetRowIndexByPrimaryKeyAsync(activityID);
        await GridActivity.SelectRowAsync(_index);*/
        /*if (await JsRuntime.Confirm("Are you sure you want to undo the previous Submission Status?\n Note: This operation cannot be reversed and the Status has to be submitted again."))
        {
            await UndoCandidateActivity.InvokeAsync(activityID);
        }*/



        //_selectedID = activityID;
        int _index = await GridActivity.GetRowIndexByPrimaryKeyAsync(activityID);
        await GridActivity.SelectRowAsync(_index);
        if (await DialogService.ConfirmAsync(null, "Undo Candidate Activity?",
                                             General.DialogOptions("Are you sure you want to <strong>undo</strong> this Candidate Activity?<br/><br/>Note: This action is irreversible.")))
        {
            await UndoCandidateActivity.InvokeAsync(activityID);
        }

    }
}