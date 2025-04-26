#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.Server
// File Name:           AddCompanyDocument.razor.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu, Brijesh Dubey
// Created On:          04-26-2025 18:04
// Last Updated On:     04-26-2025 19:33
// *****************************************/

#endregion

namespace Subscription.Server.Components.Pages.Controls.Companies;

public partial class AddCompanyDocument : ComponentBase, IDisposable
{
    private readonly CompanyDocumentValidator _companyDocumentValidator = new();

    private MemoryStream AddedDocument { get; } = new();

    [Parameter]
    public EventCallback<MouseEventArgs> Cancel { get; set; }

    private EditContext Context { get; set; }

    private SfDialog Dialog { get; set; }

    private SfDataForm EditDocumentForm { get; set; }

    internal string FileName { get; set; }

    internal string Mime { get; set; }

    [Parameter]
    public CompanyDocuments Model { get; set; }

    [Parameter]
    public EventCallback<EditContext> Save { get; set; }

    [Parameter]
    public bool SequentialUpload { get; set; } = true;

    /// <summary>
    ///     Gets or sets a value indicating whether the spinner is visible.
    /// </summary>
    /// <value>
    ///     A boolean value indicating whether the spinner is visible.
    /// </value>
    /// <remarks>
    ///     This property is used to control the visibility of the spinner in the dialog. It is set to true when an
    ///     asynchronous operation is in progress, such as document upload or save, and set to false when the operation is
    ///     completed.
    /// </remarks>
    private bool VisibleSpinner { get; set; }

    /// <summary>
    ///     Disposes the resources used by the AddCompanyDocument component.
    /// </summary>
    /// <remarks>
    ///     This method is called when the component is being disposed. It unsubscribes from the OnFieldChanged event of the
    ///     edit context.
    /// </remarks>
    public void Dispose()
    {
        if (Context is not null)
        {
            Context.OnFieldChanged -= Context_OnFieldChanged;
        }

        GC.SuppressFinalize(this);
    }

    private async Task CancelDocumentDialog(MouseEventArgs args)
    {
        VisibleSpinner = true;
        await Cancel.InvokeAsync(args);
        await Dialog.HideAsync();
        VisibleSpinner = false;
    }

    /// <summary>
    ///     Handles the event when a field in the edit context changes.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event arguments.</param>
    /// <remarks>
    ///     This method is invoked when a field in the edit context changes. It validates the changed field.
    /// </remarks>
    private void Context_OnFieldChanged(object sender, FieldChangedEventArgs e) => Context.Validate();

    /// <summary>
    ///     Handles the event when a file is removed from the upload queue.
    /// </summary>
    /// <param name="arg">
    ///     The arguments associated with the file removal event.
    /// </param>
    /// <remarks>
    ///     This method is invoked when a file is removed from the upload queue in the document upload dialog.
    /// </remarks>
    private async Task OnFileRemoved(RemovingEventArgs arg)
    {
        await Task.Yield();
        Model.Files = null;
        Context.NotifyFieldChanged(Context.Field(nameof(Model.Files)));
    }

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

        Context.NotifyFieldChanged(Context.Field(nameof(Model.Files)));
    }

    /// <summary>
    ///     Initializes the edit context and sets up event handlers for field changes.
    /// </summary>
    /// <remarks>
    ///     This method is called when the component's parameters are set. It initializes the edit context for the model
    ///     and sets up event handlers for field changes.
    /// </remarks>
    protected override void OnParametersSet()
    {
        Context = new(Model);
        Context.OnFieldChanged += Context_OnFieldChanged;
        base.OnParametersSet();
    }

    private void OpenDialog() => Context.Validate();

    private async Task SaveDocumentDialog(EditContext editContext)
    {
        VisibleSpinner = true;
        await Save.InvokeAsync(editContext);
        await Dialog.HideAsync();
        VisibleSpinner = false;
    }

    /// <summary>
    ///     Asynchronously shows the dialog for adding a company document.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    /// <remarks>
    ///     This method is used to display the dialog that allows users to add a new document to a company.
    ///     It is called in various parts of the application where a new document needs to be added to a company.
    /// </remarks>
    public async Task ShowDialog() => await Dialog.ShowAsync();

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