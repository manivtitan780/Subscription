#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.Server
// File Name:           TimelineDialog.razor.cs
// Created By:          Claude Code
// Created On:          07-26-2025 19:45
// Last Updated On:     07-26-2025 19:45
// *****************************************/

#endregion

namespace Subscription.Server.Components.Pages.Controls.Candidates;

/// <summary>
///     The TimelineDialog class represents a dialog component for displaying submission timeline data.
/// </summary>
/// <remarks>
///     This component displays a chronological timeline of candidate/requisition submission activities
///     using Syncfusion Timeline component. It shows status progression, notes, and interview details.
/// </remarks>
public partial class TimelineDialog : ComponentBase, IDisposable
{
    /// <summary>
    ///     Gets or sets the dialog reference for the timeline popup.
    /// </summary>
    private SfDialog Dialog { get; set; }

    /// <summary>
    ///     Gets or sets the submission timeline data to display.
    /// </summary>
    /// <value>
    ///     An array of SubmissionTimeline objects representing the timeline entries.
    /// </value>
    /// <remarks>
    ///     The timeline data is displayed in descending order by CreatedDate (newest first).
    ///     Each entry contains submission status, notes, creator information, and interview details if applicable.
    /// </remarks>
    [Parameter]
    public SubmissionTimeline[] Model { get; set; } = [];

    /// <summary>
    ///     Gets or sets the event callback triggered when the dialog is cancelled or closed.
    /// </summary>
    [Parameter]
    public EventCallback<MouseEventArgs> Cancel { get; set; }

    /// <summary>
    ///     Gets or sets the visibility state of the loading spinner.
    /// </summary>
    private bool VisibleSpinner { get; set; }

    /// <summary>
    ///     Clean disposal pattern for the timeline dialog component.
    /// </summary>
    public void Dispose()
    {
        // No event handlers or resources to dispose
        GC.SuppressFinalize(this);
    }

    /// <summary>
    ///     Handles the dialog close event.
    /// </summary>
    /// <param name="args">The mouse event arguments.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    private async Task CloseDialog(MouseEventArgs args)
    {
        VisibleSpinner = true;
        await Cancel.InvokeAsync(args);
        await Dialog.HideAsync();
        VisibleSpinner = false;
    }

    /// <summary>
    ///     Handles the dialog open event and performs any initialization.
    /// </summary>
    /// <remarks>
    ///     This method is called when the dialog is opened. Currently no specific initialization is required.
    /// </remarks>
    private void OpenDialog()
    {
        // Timeline data is already bound via Model parameter
        // No additional initialization required
    }

    /// <summary>
    ///     Shows the timeline dialog asynchronously.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task ShowDialog() => await Dialog.ShowAsync();
}