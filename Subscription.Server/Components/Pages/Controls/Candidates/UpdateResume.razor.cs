#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.Server
// File Name:           UpdateResume.razor.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu, Brijesh Dubey
// Created On:          06-10-2025 19:06
// Last Updated On:     06-10-2025 19:31
// *****************************************/

#endregion

namespace Subscription.Server.Components.Pages.Controls.Candidates;

public partial class UpdateResume : ComponentBase, IDisposable
{
    private readonly CandidateResumeValidator _candidateResumeValidator = new();

    // Memory optimization: Use RecyclableMemoryStream to prevent LOH allocations for large files
    internal RecyclableMemoryStream AddedDocument { get; set; }

    /// <summary>
    ///     Gets or sets the event callback that is invoked when the document upload is cancelled.
    /// </summary>
    /// <value>
    ///     The event callback for the cancel event.
    /// </value>
    /// <remarks>
    ///     This event callback is triggered when the user cancels the upload of a document to the candidate.
    ///     It uses the MouseEventArgs to provide details about the mouse event that triggered the cancellation.
    /// </remarks>
    [Parameter]
    public EventCallback<MouseEventArgs> Cancel { get; set; }

    private EditContext Context { get; set; }

    /// <summary>
    ///     Gets or sets the Syncfusion Blazor Dialog component used in the AddDocumentDialog.
    /// </summary>
    /// <value>
    ///     The Syncfusion Blazor Dialog component.
    /// </value>
    /// <remarks>
    ///     This property is used to control the visibility and behavior of the dialog.
    ///     It provides methods to show or hide the dialog programmatically.
    /// </remarks>
    private SfDialog Dialog { get; set; }

    /// <summary>
    ///     Gets or sets the EditForm instance for the document upload dialog.
    /// </summary>
    /// <value>
    ///     The EditForm instance.
    /// </value>
    /// <remarks>
    ///     This EditForm instance is used to manage the form state and validation for the document upload dialog.
    ///     It is referenced in the OpenDialog method to validate the form before the dialog is opened.
    /// </remarks>
    private SfDataForm EditDocumentForm { get; set; }

    internal string FileName { get; set; }

    internal string Mime { get; set; }

    /// <summary>
    ///     Gets or sets the model representing a candidate document for the dialog.
    /// </summary>
    /// <value>
    ///     The model is of type <see cref="CandidateDocument" /> which represents the document details of a candidate.
    /// </value>
    /// <remarks>
    ///     This model is used to bind the form fields in the dialog, enabling the user to input or edit the document details
    ///     of a candidate.
    /// </remarks>
    [Parameter]
    public CandidateResume Model { get; set; }

    /// <summary>
    ///     Gets or sets the event callback that is invoked when the document details are saved.
    /// </summary>
    /// <value>
    ///     The event callback for the save event.
    /// </value>
    /// <remarks>
    ///     This event callback is triggered when the user saves the document details in the dialog.
    ///     It uses the EditContext to provide the context of the form that is being edited.
    /// </remarks>
    [Parameter]
    public EventCallback<EditContext> Save { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether the document uploads should be sequential.
    /// </summary>
    /// <value>
    ///     <c>true</c> if the document uploads should be sequential; otherwise, <c>false</c>.
    /// </value>
    /// <remarks>
    ///     When this property is set to <c>true</c>, the documents will be uploaded one after the other in the order they were
    ///     added.
    ///     When this property is set to <c>false</c>, the documents will be uploaded concurrently.
    /// </remarks>
    [Parameter]
    public bool SequentialUpload { get; set; } = true;

    private bool VisibleSpinner { get; set; }

    [Parameter]
    public string ResumeType { get; set; }

    /// <summary>
    ///     Memory optimization: Properly dispose RecyclableMemoryStream and cleanup resources
    /// </summary>
    public void Dispose()
    {
        AddedDocument?.Dispose();
        GC.SuppressFinalize(this);
    }

    /// <summary>
    ///     Asynchronously executes the cancellation process for the document dialog.
    /// </summary>
    /// <param name="args">The mouse event arguments associated with the cancel action.</param>
    /// <remarks>
    ///     This method calls the general cancellation method, which hides the spinner and dialog, and enables the dialog
    ///     buttons.
    ///     It is typically invoked when the user triggers the cancel action in the workflow dialog.
    /// </remarks>
    /// <returns>A task that represents the asynchronous operation.</returns>
    private async Task CancelResumeDialog(MouseEventArgs args)
    {
        VisibleSpinner = true;
        await Cancel.InvokeAsync(args);
        await Dialog.HideAsync();
        VisibleSpinner = false;
    }

    /// <summary>
    ///     Handles the event when a file is removed from the upload queue.
    /// </summary>
    /// <param name="arg">
    ///     The arguments associated with the file removal event.
    /// </param>
    /// <remarks>
    ///     This method is invoked when a file is removed from the upload queue in the document upload dialog.
    ///     It clears the list of files in the model and notifies the edit context about the change.
    /// </remarks>
    private void OnFileRemoved(RemovingEventArgs arg)
    {
        Model.Files = null;
        Context.NotifyFieldChanged(Context.Field(nameof(Model.Files)));
    }

    /// <summary>
    ///     Handles the file selection event in the document upload process.
    /// </summary>
    /// <param name="file">
    ///     The selected file event arguments containing the details of the selected file.
    /// </param>
    /// <returns>
    ///     A <see cref="Task" /> representing the asynchronous operation.
    /// </returns>
    /// <remarks>
    ///     This method is invoked when a file is selected in the document upload dialog.
    ///     It updates the model's file list with the name of the selected file and notifies the edit context of the change.
    /// </remarks>
    private void OnFileSelected(SelectedEventArgs file)
    {
        if (Model.Files is null)
        {
            Model.Files = [file.FilesData[0].Name];
        }
        else
        {
            Model.Files.Clear();
            Model.Files.Add(file.FilesData[0].Name);
        }

        Context.NotifyFieldChanged(Context.Field(nameof(Model.Files)));
    }

    protected override void OnParametersSet()
    {
        // Memory optimization: Explicit cleanup before creating new EditContext
        if (Context?.Model != Model)
        {
            Context = null;  // Immediate reference cleanup for GC
            Context = new(Model);
        }
        base.OnParametersSet();
    }

    /// <summary>
    ///     Asynchronously prepares the document dialog for opening.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <remarks>
    ///     This method is invoked before the dialog is opened. It initializes the edit context for the workflow form and
    ///     validates it.
    /// </remarks>
    private void OpenDialog() => Context.Validate();

    /// <summary>
    ///     Asynchronously saves the document changes made in the dialog.
    /// </summary>
    /// <param name="editContext">The edit context associated with the save action.</param>
    /// <remarks>
    ///     This method calls the general save method, passing in the edit context, spinner, dialog footer, dialog, and save
    ///     event callback.
    ///     It is typically triggered when the user confirms the save operation in the dialog.
    /// </remarks>
    private async Task SaveResumeDialog(EditContext editContext)
    {
        VisibleSpinner = true;
        await Save.InvokeAsync(editContext);
        await Dialog.HideAsync();
        VisibleSpinner = false;
    }

    /// <summary>
    ///     Asynchronously displays the AddDocumentDialog.
    /// </summary>
    /// <returns>
    ///     A Task that represents the asynchronous operation.
    /// </returns>
    /// <remarks>
    ///     This method is used to programmatically display the dialog for adding a document to a candidate.
    ///     It uses the ShowAsync method of the Syncfusion Blazor Dialog component.
    /// </remarks>
    public async Task ShowDialog() => await Dialog.ShowAsync();

    private async Task UploadDocument(UploadChangeEventArgs file)
    {
        foreach (UploadFiles _file in file.Files)
        {
            // Memory optimization: Reuse existing RecyclableMemoryStream instead of creating new instances
            if (AddedDocument == null)
            {
                AddedDocument = ReusableMemoryStream.Get("resume-upload");
            }
            else
            {
                // Reset stream for reuse - more efficient than disposing and recreating
                AddedDocument.Position = 0;
                AddedDocument.SetLength(0);
            }

            // Performance optimization: Use proper using statement for stream disposal
            await using (Stream _str = _file.File.OpenReadStream(60 * 1024 * 1024)) //60MB maximum
            {
                await _str.CopyToAsync(AddedDocument);
            }

            FileName = _file.FileInfo.Name;
            Mime = _file.FileInfo.MimeContentType;
            AddedDocument.Position = 0;
        }
    }
}