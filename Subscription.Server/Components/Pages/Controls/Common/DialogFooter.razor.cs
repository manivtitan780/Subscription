#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Profsvc_AppTrack
// Project:             Profsvc_AppTrack.Client
// File Name:           DialogFooter.razor.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu
// Created On:          1-5-2024 16:13
// Last Updated On:     1-27-2024 16:15
// *****************************************/

#endregion

namespace Subscription.Server.Components.Pages.Controls.Common;

/// <summary>
///     Represents a dialog footer in the admin controls of the application.
///     This class contains properties and methods for managing the Cancel and Save buttons in the dialog.
/// </summary>
public partial class DialogFooter
{
    /// <summary>
    ///     Gets or sets the text displayed on the Cancel button in the dialog footer.
    /// </summary>
    [Parameter]
    public string Cancel
    {
        get;
        set;
    } = "Cancel";

    /// <summary>
    ///     Gets or sets the Cancel button in the dialog footer.
    ///     This button is used to cancel the current operation and close the dialog.
    /// </summary>
    private SfButton CancelButton
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
    ///     Gets or sets the EventCallback for the Cancel button in the dialog footer.
    ///     This callback is invoked when the Cancel button is clicked, triggering the associated event handler.
    /// </summary>
    [Parameter]
    public EventCallback<MouseEventArgs> CancelMethod
    {
        get;
        set;
    }

    /// <summary>
    ///     Gets or sets the text displayed on the Save button in the dialog footer.
    /// </summary>
    [Parameter]
    public string Save
    {
        get;
        set;
    } = "Save";

    /// <summary>
    ///     Gets or sets the Save button in the dialog footer.
    ///     This button is used to save the current operation and close the dialog.
    /// </summary>
    private SfButton SaveButton
    {
        get;
        set;
    }

    private bool SaveDisabled
    {
        get;
        set;
    }

    /// <summary>
    ///     Checks if either the Cancel or Save button in the dialog footer is disabled.
    /// </summary>
    /// <returns>
    ///     Returns true if either the Cancel or Save button is disabled, otherwise false.
    /// </returns>
    internal bool ButtonsDisabled() => CancelDisabled || SaveDisabled;

    /// <summary>
    ///     Disables both the Cancel and Save buttons in the dialog footer.
    /// </summary>
    /// <remarks>
    ///     This method is used to disable both the Cancel and Save buttons in the dialog footer, preventing any further user
    ///     interaction with these buttons until they are re-enabled.
    /// </remarks>
    internal void DisableButtons()
    {
        CancelDisabled = true;
        SaveDisabled = true;
    }

    /// <summary>
    ///     Enables both the Cancel and Save buttons in the dialog footer.
    /// </summary>
    /// <remarks>
    ///     This method is used to enable both the Cancel and Save buttons in the dialog footer, allowing further user
    ///     interaction with these buttons.
    /// </remarks>
    internal void EnableButtons()
    {
        CancelDisabled = false;
        SaveDisabled = false;
    }
}