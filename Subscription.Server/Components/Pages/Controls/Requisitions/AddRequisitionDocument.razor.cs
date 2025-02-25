#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            ProfSvc_AppTrack
// Project:             ProfSvc_AppTrack
// File Name:           AddRequisitionDocument.razor.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja
// Created On:          12-09-2022 15:57
// Last Updated On:     09-30-2023 20:53
// *****************************************/

#endregion

using FluentValidation;

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
public partial class AddRequisitionDocument
{
    private EditContext _editContext;
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
    private SfDataForm AddDocumentForm
    {
        get;
        set;
    }

    /*/// <summary>
    ///     Gets or sets the EventCallback triggered after the upload event occurs in the AddRequisitionDocument component.
    /// </summary>
    /// <value>
    ///     The EventCallback for the after upload event.
    /// </value>
    /// <remarks>
    ///     This EventCallback is invoked after a file is uploaded in the AddRequisitionDocument component.
    ///     It is typically used to handle post-upload tasks, such as updating the UI or notifying the user about the upload
    ///     status.
    /// </remarks>
    [Parameter]
    public EventCallback<ActionCompleteEventArgs> AfterUpload
    {
        get;
        set;
    }

    /// <summary>
    ///     Gets or sets the EventCallback triggered before the upload event occurs in the AddRequisitionDocument component.
    /// </summary>
    /// <value>
    ///     The EventCallback for the before upload event.
    /// </value>
    /// <remarks>
    ///     This EventCallback is invoked before a file is uploaded in the AddRequisitionDocument component.
    ///     It is typically used to handle pre-upload tasks, such as validating the file or preparing the upload process.
    /// </remarks>
    [Parameter]
    public EventCallback<BeforeUploadEventArgs> BeforeUpload
    {
        get;
        set;
    }*/

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
    public EventCallback<MouseEventArgs> Cancel
    {
        get;
        set;
    }

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
    private SfDialog Dialog
    {
        get;
        set;
    }

    /// <summary>
    ///     Gets or sets the DialogFooter instance used in the AddRequisitionDocument component.
    /// </summary>
    /// <value>
    ///     The DialogFooter instance.
    /// </value>
    /// <remarks>
    ///     This property is used to manage the Cancel and Save buttons in the dialog of the AddRequisitionDocument component.
    /// </remarks>
    private DialogFooter FooterDialog
    {
        get;
        set;
    }

    /// <summary>
    ///     Gets or sets the model for the RequisitionDocuments.
    /// </summary>
    /// <value>
    ///     The model for the RequisitionDocuments.
    /// </value>
    [Parameter]
    public RequisitionDocuments Model
    {
        get;
        set;
    }

    /// <summary>
    ///     Gets or sets the EventCallback triggered when a file upload event occurs in the AddRequisitionDocument component.
    /// </summary>
    /// <remarks>
    ///     This EventCallback is invoked when a file is selected for upload in the AddRequisitionDocument component.
    ///     It is typically used to handle the file upload process, such as starting the upload, tracking the upload progress,
    ///     or handling upload errors.
    /// </remarks>
    [Parameter]
    public EventCallback<UploadChangeEventArgs> OnFileUpload
    {
        get;
        set;
    }

    /// <summary>
    ///     Gets or sets the EventCallback for saving the EditContext.
    /// </summary>
    /// <remarks>
    ///     This EventCallback is invoked when the save operation is triggered in the AddRequisitionDocument component.
    ///     It is typically used to perform the actual save operation, such as persisting the changes to a database or a file.
    /// </remarks>
    [Parameter]
    public EventCallback<EditContext> Save
    {
        get;
        set;
    }

    /// <summary>
    ///     Gets or sets the spinner control used in the AddRequisitionDocument component.
    /// </summary>
    /// <remarks>
    ///     This spinner is displayed during asynchronous operations such as document upload or save to indicate progress.
    /// </remarks>
    private SfSpinner Spinner
    {
        get;
        set;
    }

    private bool VisibleSpinner
    {
        get;
        set;
    }

    private EditContext Context
    {
        get;
        set;
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

    /*/// <summary>
    ///     Disables the buttons in the AddRequisitionDocument dialog.
    /// </summary>
    /// <remarks>
    ///     This method is used to disable the buttons in the AddRequisitionDocument dialog,
    ///     preventing any further user interaction with these buttons until they are re-enabled.
    ///     It is typically called before a document upload process to prevent any other actions during the upload.
    /// </remarks>
    internal void DisableButtons() => FooterDialog.DisableButtons();

    /// <summary>
    ///     Enables the buttons in the dialog footer of the AddRequisitionDocument component.
    /// </summary>
    /// <remarks>
    ///     This method calls the EnableButtons method of the DialogFooter component,
    ///     which enables both the Cancel and Save buttons in the dialog footer,
    ///     allowing further user interaction with these buttons.
    /// </remarks>
    internal void EnableButtons() => FooterDialog.EnableButtons();*/

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
        _editContext?.NotifyFieldChanged(_editContext.Field(nameof(Model.Files)));
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
        if (Model.Files == null)
        {
            Model.Files = new()
                          {
                              file.FilesData[0].Name
                          };
        }
        else
        {
            Model.Files.Clear();
            Model.Files.Add(file.FilesData[0].Name);
        }

        _editContext?.NotifyFieldChanged(_editContext.Field(nameof(Model.Files)));
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
    private async Task OpenDialog(BeforeOpenEventArgs arg)
    {
        await Task.Yield();
        _editContext = AddDocumentForm.EditContext;
        _editContext?.Validate();
    }

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
        // await General.CallSaveMethod(editContext, Spinner, FooterDialog, Dialog, Save);
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

    internal MemoryStream AddedDocument
    {
        get;
        set;
    } = new();

    internal string FileName
    {
        get;
        set;
    }

    internal string Mime
    {
        get;
        set;
    }

    private async Task UploadDocument(UploadChangeEventArgs file)
    {
        foreach (UploadFiles _file in file.Files)
        {
            Stream _str = _file.File.OpenReadStream(60 * 1024 * 1024);
            await _str.CopyToAsync(AddedDocument);
            FileName = _file.FileInfo.Name;
            Mime = _file.FileInfo.MimeContentType;
            AddedDocument.Position = 0;
            _str.Close();
        }
    }
}