#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.Server
// File Name:           AddRequisitionDocument.razor.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu, Brijesh Dubey
// Created On:          07-24-2025 19:07
// Last Updated On:     07-24-2025 19:41
// *****************************************/

#endregion

namespace Subscription.Server.Components.Pages.Controls.Requisitions;

/// <summary>
///     Represents a component for adding a new requisition document in the application.
/// </summary>
/// <remarks>
///     This component provides a dialog for uploading a new document to a requisition.
///     It includes event callbacks for handling the upload process, such as before and after upload events,
///     and a cancel event. It also provides a model for the requisition documents and methods for enabling
///     and disabling dialog buttons. Additionally, it provides a method for showing the dialog.
/// </remarks>
public partial class AddRequisitionDocument : IDisposable
{
    private readonly RequisitionDocumentValidator _candidateDocumentValidator = new();

    /// <summary>
    ///     Gets or sets the EditForm instance for the AddRequisitionDocument component.
    /// </summary>
    /// <value>
    ///     The EditForm instance.
    /// </value>
    /// <remarks>
    ///     This property is used to manage the form state and handle form submissions in the AddRequisitionDocument component.
    /// </remarks>
    private SfDataForm AddDocumentForm { get; set; }

    // Memory optimization: Use RecyclableMemoryStream to prevent LOH allocations for large files
    internal RecyclableMemoryStream AddedDocument { get; set; } // = RecyclableMemoryStreamManager.Shared.GetStream();

    /// <summary>
    ///     Gets or sets the EventCallback triggered when the cancel event occurs in the AddRequisitionDocument component.
    /// </summary>
    /// <value>
    ///     The EventCallback for the cancel event.
    /// </value>
    /// <remarks>
    ///     This EventCallback is invoked when the cancel action is triggered in the AddRequisitionDocument component.
    ///     It is typically used to handle the cancellation of the operation, such as stopping the upload process or closing
    ///     the dialog.
    /// </remarks>
    [Parameter]
    public EventCallback<MouseEventArgs> Cancel { get; set; }

    private EditContext Context { get; set; }

    /// <summary>
    ///     Gets or sets the dialog component of the AddRequisitionDocument page.
    /// </summary>
    /// <value>
    ///     The dialog component of the AddRequisitionDocument page.
    /// </value>
    /// <remarks>
    ///     This property represents the dialog component that is used to add a new requisition document.
    ///     It provides the functionality to show, hide, and interact with the dialog.
    /// </remarks>
    private SfDialog Dialog { get; set; }

    internal string FileName { get; set; }

    internal string Mime { get; set; }

    /// <summary>
    ///     Gets or sets the model for the RequisitionDocuments.
    /// </summary>
    /// <value>
    ///     The model for the RequisitionDocuments.
    /// </value>
    [Parameter]
    public RequisitionDocuments Model { get; set; }

    /// <summary>
    ///     Gets or sets the EventCallback for saving the EditContext.
    /// </summary>
    /// <remarks>
    ///     This EventCallback is invoked when the save operation is triggered in the AddRequisitionDocument component.
    ///     It is typically used to perform the actual save operation, such as persisting the changes to a database or a file.
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

    /// <summary>
    ///     Gets or sets the spinner control used in the AddRequisitionDocument component.
    /// </summary>
    /// <remarks>
    ///     This spinner is displayed during asynchronous operations such as document upload or save to indicate progress.
    /// </remarks>
    private SfSpinner Spinner { get; set; }

    private bool VisibleSpinner { get; set; }

    /// <summary>
    ///     Memory optimization: Properly dispose RecyclableMemoryStream
    /// </summary>
    public void Dispose()
    {
        AddedDocument?.Dispose();
        GC.SuppressFinalize(this);
    }

    /// <summary>
    ///     Asynchronously cancels the document dialog operation.
    /// </summary>
    /// <param name="args">The mouse event arguments associated with the cancel action.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <remarks>
    ///     This method calls the general cancel method, which hides the spinner and dialog, and enables the dialog buttons.
    /// </remarks>
    private async Task CancelDocumentDialog(MouseEventArgs args)
    {
        VisibleSpinner = true;
        await Cancel.InvokeAsync(args);
        await Dialog.HideAsync();
        VisibleSpinner = false;
    }

    // Removed: Unnecessary Context_OnFieldChanged event handler - validation handled by form validation

    /// <summary>
    ///     Asynchronously handles the event when a file is removed from the upload control.
    /// </summary>
    /// <param name="arg">The arguments associated with the file removing event.</param>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    /// <remarks>
    ///     This method is triggered when a file is removed from the upload control in the AddRequisitionDocument dialog.
    ///     It sets the Files property of the Model to null and notifies the EditContext about the change.
    /// </remarks>
    private async Task OnFileRemoved(RemovingEventArgs arg)
    {
        await Task.Yield();
        Model.Files = null;
        Context?.NotifyFieldChanged(Context.Field(nameof(Model.Files)));
    }

    /// <summary>
    ///     Asynchronously handles the file selection event.
    /// </summary>
    /// <param name="file">The selected file event arguments.</param>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    /// <remarks>
    ///     This method is triggered when a file is selected in the file uploader control.
    ///     It updates the `Files` property of the `Model` with the name of the selected file and notifies the `EditContext` of
    ///     the change.
    /// </remarks>
    private async Task OnFileSelected(SelectedEventArgs file)
    {
        await Task.Yield();
        if (Model.Files is null)
        {
            Model.Files = [file.FilesData[0].Name];
        }
        else
        {
            Model.Files.Clear();
            Model.Files.Add(file.FilesData[0].Name);
        }

        Context?.NotifyFieldChanged(Context.Field(nameof(Model.Files)));
    }

    protected override void OnParametersSet()
    {
        // Memory optimization: Only create new EditContext if Model reference changed
        if (Context?.Model != Model)
        {
            Context = new(Model);
        }

        base.OnParametersSet();
    }

    /// <summary>
    ///     Asynchronously prepares the dialog for adding a requisition document before it is opened.
    /// </summary>
    /// <param name="arg">The arguments associated with the dialog opening event.</param>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    /// <remarks>
    ///     This method is invoked before the dialog for adding a requisition document is opened.
    ///     It prepares the dialog by setting the edit context and validating it.
    /// </remarks>
    private void OpenDialog(BeforeOpenEventArgs arg) => Context?.Validate();

    /// <summary>
    ///     Asynchronously saves the document in the dialog.
    /// </summary>
    /// <param name="editContext">The edit context associated with the save action.</param>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    /// <remarks>
    ///     This method calls the general save method, which executes the provided save method, shows the spinner, disables the
    ///     dialog buttons, and then hides the spinner and dialog, and enables the dialog buttons.
    /// </remarks>
    private async Task SaveDocumentDialog(EditContext editContext)
    {
        VisibleSpinner = true;
        await Save.InvokeAsync(editContext);
        await Dialog.HideAsync();
        VisibleSpinner = false;
    }

    /// <summary>
    ///     Asynchronously shows the dialog for adding a requisition document.
    /// </summary>
    /// <remarks>
    ///     This method is used to display the dialog that allows users to add a new document to a requisition.
    ///     It is called in various parts of the application where a new document needs to be added to a requisition.
    /// </remarks>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    public async Task ShowDialog() => await Dialog.ShowAsync();

    private async Task UploadDocument(UploadChangeEventArgs file)
    {
        foreach (UploadFiles _file in file.Files)
        {
            // Memory optimization: Reuse existing ReusableMemoryStream instead of creating new instances
            if (AddedDocument == null)
            {
                AddedDocument = ReusableMemoryStream.Get("requisition-document-upload");
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