#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.Server
// File Name:           ConfirmDialog.razor.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu
// Created On:          11-19-2024 20:11
// Last Updated On:     11-19-2024 20:11
// *****************************************/

#endregion

namespace Subscription.Server.Components.Pages.Controls.Common;

/// <summary>
///     Represents a dialog component for confirming user actions.
/// </summary>
/// <remarks>
///     This class provides a dialog interface for confirming various user actions. It includes parameters for customizing
///     the dialog's appearance and behavior.
///     The dialog can be used to confirm actions such as deletion, cancellation, and activity toggling.
/// </remarks>
public partial class ConfirmDialog
{
	/// <summary>
	///     Gets or sets the event callback that is invoked when the cancel action is performed.
	/// </summary>
	/// <value>
	///     The event callback for the cancel action.
	/// </value>
	/// <remarks>
	///     This event callback is used to handle the user's cancel action in the confirmation dialog.
	///     It is invoked in the CancelEntity method after ensuring that the DeleteButton is not disabled.
	/// </remarks>
	[Parameter]
    public EventCallback<MouseEventArgs> Cancel
    {
        get;
        set;
    }

    private bool CancelDisabled
    {
        get;
        set;
    }

	/// <summary>
	///     Gets or sets the event callback that is invoked when the delete action is performed.
	/// </summary>
	/// <value>
	///     The event callback for the delete action.
	/// </value>
	/// <remarks>
	///     This event callback is used to handle the user's delete action in the confirmation dialog.
	///     It is invoked in the DeleteEntity method after ensuring that the DeleteButton is not disabled.
	/// </remarks>
	[Parameter]
    public EventCallback<MouseEventArgs> Delete
    {
        get;
        set;
    }

    private bool DeleteDisabled
    {
        get;
        set;
    }

	/// <summary>
	///     Gets or sets the SfDialog component of the confirmation dialog.
	/// </summary>
	/// <value>
	///     The SfDialog component of the confirmation dialog.
	/// </value>
	/// <remarks>
	///     This property is used to control the visibility and behavior of the confirmation dialog.
	///     It is used in the CancelEntity, DeleteEntity, ModalClick, and ShowDialog methods to show or hide the dialog.
	/// </remarks>
	private SfDialog Dialog
    {
        get;
        set;
    }

	/// <summary>
	///     Gets or sets the entity that the confirmation dialog is acting upon.
	/// </summary>
	/// <value>
	///     The entity that the confirmation dialog is acting upon.
	/// </value>
	/// <remarks>
	///     This property is used to specify the entity that the confirmation dialog is acting upon.
	///     It is used in the dialog's header and content to provide context to the user about the action they are confirming.
	///     For example, if the entity is "User", the dialog might ask "Are you sure you want to delete this User?".
	/// </remarks>
	[Parameter]
    public string Entity
    {
        get;
        set;
    }

	/// <summary>
	///     Gets or sets the height of the confirmation dialog.
	/// </summary>
	/// <value>
	///     The height of the confirmation dialog.
	/// </value>
	/// <remarks>
	///     This property is used to specify the height of the confirmation dialog.
	///     The height is specified as a string and can be any valid CSS height value.
	///     The default height is "160px".
	/// </remarks>
	[Parameter]
    public string Height
    {
        get;
        set;
    } = "160px";

	/// <summary>
	///     Gets or sets a value indicating whether the confirmation dialog is for undoing an activity.
	/// </summary>
	/// <value>
	///     <c>true</c> if the confirmation dialog is for undoing an activity; otherwise, <c>false</c>.
	/// </value>
	/// <remarks>
	///     This property is used to determine the type of confirmation dialog to display to the user.
	///     If this property is set to <c>true</c>, the dialog will ask the user to confirm undoing the previous Submission
	///     Status.
	///     Note that this operation cannot be reversed and the Status has to be submitted again.
	/// </remarks>
	[Parameter]
    public bool IsUndoActivity
    {
        get;
        set;
    }

	/// <summary>
	///     Gets or sets the spinner component of the confirmation dialog.
	/// </summary>
	/// <value>
	///     The spinner component of the confirmation dialog.
	/// </value>
	/// <remarks>
	///     This property is used to manage the spinner component displayed in the confirmation dialog.
	///     The spinner is shown while the dialog is processing an action, providing a visual cue to the user that processing
	///     is underway.
	/// </remarks>
	private SfSpinner Spinner
    {
        get;
        set;
    }

	/// <summary>
	///     Gets or sets a value indicating whether the confirmation dialog is for toggling the status of an entity.
	/// </summary>
	/// <value>
	///     <c>true</c> if the confirmation dialog is for toggling the status of an entity; otherwise, <c>false</c>.
	/// </value>
	/// <remarks>
	///     This property is used to determine the type of confirmation dialog to display to the user.
	///     If this property is set to <c>true</c>, the dialog will ask the user to confirm toggling the status of the entity.
	///     The specific toggle action (enable or disable) is determined by the <see cref="ToggleValue" /> property.
	/// </remarks>
	[Parameter]
    public bool ToggleStatus
    {
        get;
        set;
    }

	/// <summary>
	///     Gets or sets the toggle value for the confirmation dialog.
	/// </summary>
	/// <value>
	///     The toggle value for the confirmation dialog.
	/// </value>
	/// <remarks>
	///     This property is used to control the state of the confirmation dialog.
	///     It can take three values: 1 for Activate, 2 for Deactivate, and 0 for Not toggle.
	///     This value is used in the dialog's header and content to provide context to the user about the action they are
	///     confirming.
	///     For example, if the ToggleValue is 1, the dialog might ask "Are you sure you want to enable this User?".
	/// </remarks>
	[Parameter]
    public byte ToggleValue
    {
        get;
        set;
    }

	/// <summary>
	///     Gets or sets the width of the confirmation dialog.
	/// </summary>
	/// <value>
	///     The width of the confirmation dialog.
	/// </value>
	/// <remarks>
	///     This property is used to specify the width of the confirmation dialog.
	///     The width is specified as a string and can be any valid CSS width value.
	///     The default width is "560px".
	/// </remarks>
	[Parameter]
    public string Width
    {
        get;
        set;
    } = "560px";

	/// <summary>
	///     Handles the cancel action in the confirmation dialog.
	/// </summary>
	/// <param name="args">The <see cref="MouseEventArgs" /> instance containing the event data.</param>
	/// <remarks>
	///     This method is invoked when the user clicks the cancel button in the confirmation dialog.
	///     It ensures that the DeleteButton and CancelButton are disabled during the execution of the cancel action to prevent
	///     multiple triggers.
	///     After the cancel action is performed, it re-enables the buttons and hides the dialog.
	/// </remarks>
	private async Task CancelEntity(MouseEventArgs args)
    {
        if (!DeleteDisabled)
        {
            DeleteDisabled = true;
            CancelDisabled = true;
            await Cancel.InvokeAsync(args);
            DeleteDisabled = false;
            CancelDisabled = false;
            await Dialog.HideAsync();
        }
    }

	/// <summary>
	///     Executes the delete action in the confirmation dialog.
	/// </summary>
	/// <param name="args">The mouse event arguments associated with the delete action.</param>
	/// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
	/// <remarks>
	///     This method is invoked when the user performs a delete action in the confirmation dialog.
	///     It first checks if the DeleteButton is not disabled. If it's not, it disables the DeleteButton and the
	///     CancelButton,
	///     invokes the Delete event callback, and then re-enables the DeleteButton and the CancelButton.
	///     Finally, it hides the confirmation dialog.
	/// </remarks>
	private async Task DeleteEntity(MouseEventArgs args)
    {
        if (!DeleteDisabled)
        {
            DeleteDisabled = true;
            CancelDisabled = true;
            await Delete.InvokeAsync(args);
            DeleteDisabled = false;
            CancelDisabled = false;
            await Dialog.HideAsync();
        }
    }

	/// <summary>
	///     Handles the click event of the modal overlay.
	/// </summary>
	/// <remarks>
	///     This method is invoked when the user clicks on the modal overlay of the confirmation dialog.
	///     It hides the dialog, effectively cancelling the user's action.
	/// </remarks>
	private Task ModalClick() => Dialog.HideAsync();

	/// <summary>
	///     Displays the confirmation dialog asynchronously.
	/// </summary>
	/// <returns>
	///     A Task that represents the asynchronous operation.
	/// </returns>
	/// <remarks>
	///     This method is used to display the confirmation dialog. It is typically invoked when a user action requires
	///     confirmation,
	///     such as deleting a record or toggling a status. The dialog is displayed asynchronously, allowing the user interface
	///     to remain
	///     responsive while the dialog is being prepared and shown.
	/// </remarks>
	internal Task ShowDialog() => Dialog.ShowAsync();
}