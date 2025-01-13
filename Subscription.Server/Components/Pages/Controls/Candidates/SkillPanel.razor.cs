#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.Server
// File Name:           SkillPanel.razor.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu
// Created On:          11-19-2024 20:11
// Last Updated On:     11-30-2024 20:11
// *****************************************/

#endregion

namespace Subscription.Server.Components.Pages.Controls.Candidates;

/// <summary>
///     The SkillPanel class is a Blazor component that represents a panel for managing candidate skills in the
///     application.
///     It provides functionalities for deleting and editing skills, and it displays the skills in a grid.
/// </summary>
/// <remarks>
///     The SkillPanel class includes properties for managing the skills of a candidate, such as DeleteSkill, EditRights,
///     EditSkill, GridSkill, IsRequisition, Model, ModelSkill, RowHeight, and SelectedRow.
///     These properties represent various aspects of the skill management process, including the event callbacks for
///     deleting and editing skills, the grid that displays the skills, the current user's rights to edit the skills, the
///     current context (whether it is a requisition or not), the model for the skills, the height of each row in the grid,
///     and the currently selected row in the grid.
///     The class also includes methods for deleting and editing skills, as well as handling the row selection event in the
///     grid.
/// </remarks>
public partial class SkillPanel
{
    private int _selectedID;

	/// <summary>
	///     Gets or sets the event callback that is triggered when a skill entry is to be deleted.
	/// </summary>
	/// <value>
	///     The event callback that takes an integer parameter representing the ID of the skill entry to be deleted.
	/// </value>
	[Parameter]
    public EventCallback<int> DeleteSkill
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
	///     This service is injected into the component and used in methods like <see cref="DeleteSkillMethod" />
	///     to confirm actions before proceeding.
	/// </remarks>
	[Inject]
    private SfDialogService DialogService
    {
        get;
        set;
    }

	/// <summary>
	///     Gets or sets a value indicating whether the current user has rights to edit the skill entries.
	/// </summary>
	/// <value>
	///     true if the current user has edit rights; otherwise, false. The default is true.
	/// </value>
	/// <remarks>
	///     This property is a boolean flag that determines if the current user has the rights to edit the skill entries.
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
	///     Gets or sets the event callback that is triggered when a skill entry is to be edited.
	/// </summary>
	/// <value>
	///     The event callback that takes an integer parameter representing the ID of the skill entry to be edited.
	/// </value>
	[Parameter]
    public EventCallback<int> EditSkill
    {
        get;
        set;
    }

	/// <summary>
	///     Gets or sets the event callback that is triggered when a skill entry is to be edited.
	/// </summary>
	/// <value>
	///     The event callback that takes an integer parameter representing the ID of the skill entry to be edited.
	/// </value>
	private SfGrid<CandidateSkills> GridSkill
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
	///     Gets or sets the model representing the details of a candidate.
	/// </summary>
	/// <value>
	///     The model of type CandidateDetails containing the details of a candidate.
	/// </value>
	/// <remarks>
	///     This property is used to bind the data of a candidate to the SkillPanel component.
	///     It is used in the component's markup to display and manage the candidate's skills.
	/// </remarks>
	[Parameter]
    public CandidateDetails Model
    {
        get;
        set;
    }

	/// <summary>
	///     Gets or sets the list of candidate skills.
	/// </summary>
	/// <value>
	///     The list of skills associated with a candidate.
	/// </value>
	/// <remarks>
	///     This property represents the skills of a candidate. Each skill is represented by an instance of the CandidateSkills
	///     class.
	/// </remarks>
	[Parameter]
    public List<CandidateSkills> ModelSkill
    {
        get;
        set;
    }

	/// <summary>
	///     Gets or sets the height of each row in the grid displaying the skill entries.
	/// </summary>
	/// <value>
	///     An integer representing the height of each row in pixels. The default is 38.
	/// </value>
	/// <remarks>
	///     The RowHeight property determines the height of each row in the grid that displays the skill entries of a
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
	///     A CandidateSkills object representing the currently selected skill entry in the grid.
	/// </value>
	/// <remarks>
	///     The SelectedRow property is used to keep track of the currently selected row in the grid that displays the
	///     skill entries of a candidate.
	///     This property is updated whenever a row is selected in the grid, and it is used to perform operations on the
	///     selected skill entry, such as editing or deleting the entry.
	/// </remarks>
	internal CandidateSkills SelectedRow
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
	///     Asynchronously deletes the skill detail of a candidate.
	/// </summary>
	/// <param name="id">The ID of the skill detail to be deleted.</param>
	/// <returns>A Task representing the asynchronous operation.</returns>
	/// <remarks>
	///     This method sets the selected ID to the provided ID, gets the index of the row in the grid corresponding to the ID,
	///     selects the row in the grid, and shows a confirmation dialog.
	/// </remarks>
	private async Task DeleteSkillMethod(int id)
    {
        _selectedID = id;
        int _index = await GridSkill.GetRowIndexByPrimaryKeyAsync(id);
        await GridSkill.SelectRowAsync(_index);
        if (await DialogService.ConfirmAsync(null, "Delete Skill", General.DialogOptions("Are you sure you want to <strong>delete</strong> this <i>Candidate Skill</i>?")))
        {
            await DeleteSkill.InvokeAsync(_selectedID);
        }
    }

	/// <summary>
	///     Asynchronously opens the dialog for editing the skill details of a candidate.
	/// </summary>
	/// <param name="id">The ID of the skill detail to be edited.</param>
	/// <returns>A Task representing the asynchronous operation.</returns>
	/// <remarks>
	///     This method sets the selected ID to the provided ID, gets the index of the row in the grid corresponding to the
	///     provided ID, selects the row in the grid, and invokes the EditExperience event callback.
	/// </remarks>
	private async Task EditSkillDialog(int id)
    {
        _selectedID = id;
        int _index = await GridSkill.GetRowIndexByPrimaryKeyAsync(id);
        await GridSkill.SelectRowAsync(_index);
        await EditSkill.InvokeAsync(id);
    }

	/// <summary>
	///     Handles the row selection event in the skill details grid.
	/// </summary>
	/// <param name="skill">
	///     The event arguments containing the selected row data of type
	///     <see cref="CandidateSkills" />.
	/// </param>
	/// <remarks>
	///     This method is triggered when a row is selected in the skill details grid.
	///     It sets the SelectedRow property to the data of the selected row.
	/// </remarks>
	private void RowSelected(RowSelectEventArgs<CandidateSkills> skill)
    {
        if (skill != null)
        {
            SelectedRow = skill.Data;
        }
    }
}