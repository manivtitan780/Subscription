#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.Server
// File Name:           ViewWordDocument.razor.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu
// Created On:          12-06-2024 15:12
// Last Updated On:     12-06-2024 19:12
// *****************************************/

#endregion

#region Using

using BeforeOpenEventArgs = Syncfusion.Blazor.Popups.BeforeOpenEventArgs;
using ClickEventArgs = Syncfusion.Blazor.DocumentEditor.ClickEventArgs;

#endregion

namespace Subscription.Server.Components.Pages.Controls.Common;

/// <summary>
///     A Razor component that provides functionality to view Word documents.
/// </summary>
/// <remarks>
///     This component is used in various parts of the application where viewing of Word documents is required.
///     It takes several parameters including the location and name of the document, the ID and type of the entity
///     associated with the document, and the internal file name.
///     The entity type is represented by the `EntityType` enumeration, which includes Candidate, Requisition, Companies,
///     Leads, and Benefits.
/// </remarks>
public partial class ViewWordDocument
{
    /// <summary>
    ///     Represents a list of custom toolbar items for the document editor.
    /// </summary>
    /// <remarks>
    ///     This list includes toolbar items for saving, printing, finding text, and closing the document editor.
    ///     Each item is represented by a `CustomToolbarItemModel` object with properties for the item's ID, text, and prefix
    ///     icon.
    /// </remarks>
    private readonly List<object> _items =
    [
        /*new CustomToolbarItemModel {Id = "save", Text = "Save", PrefixIcon = "savePrefix"},*/
        new CustomToolbarItemModel {Id = "print", Text = "Print", PrefixIcon = "printPrefix"},
        "Find", new CustomToolbarItemModel {Id = "close", Text = "Close", PrefixIcon = "closePrefix"}
    ];

    /// <summary>
    ///     Gets or sets the dialog component of the ViewWordDocument Razor component.
    /// </summary>
    /// <value>
    ///     The dialog component used to display the Word document in a modal dialog.
    /// </value>
    /// <remarks>
    ///     This dialog is shown or hidden using the ShowDialog and OverlayClick methods respectively.
    /// </remarks>
    private SfDialog Dialog
    {
        get;
        set;
    }

    /// <summary>
    ///     Gets or sets the location of the document to be viewed.
    /// </summary>
    /// <value>
    ///     The location of the document.
    /// </value>
    /// <remarks>
    ///     This parameter is used to specify the location of the Word document that needs to be viewed.
    ///     It is used in the `ToolbarClicked` method to save the document and is also displayed in the header of the dialog
    ///     component.
    /// </remarks>
    [Parameter]
    public string DocumentLocation
    {
        get;
        set;
    }

    /// <summary>
    ///     Gets or sets the name of the document to be viewed.
    /// </summary>
    /// <value>
    ///     The name of the document.
    /// </value>
    /// <remarks>
    ///     This parameter is used to specify the name of the Word document that needs to be viewed.
    ///     It is displayed in the header of the dialog component along with the document location.
    /// </remarks>
    [Parameter]
    public string DocumentName
    {
        get;
        set;
    }

    /// <summary>
    ///     Gets or sets the Syncfusion Document Editor Container used for viewing the Word document.
    /// </summary>
    /// <value>
    ///     The Syncfusion Document Editor Container.
    /// </value>
    /// <remarks>
    ///     This property is used to interact with the Syncfusion Document Editor Container, which provides the functionality
    ///     to view and manipulate Word documents.
    ///     It is used in various methods such as `OpenDialog`, `ShowDialog`, and `ToolbarClicked` to load the document, resize
    ///     the editor, save changes, print the document, and close the dialog.
    /// </remarks>
    private SfDocumentEditorContainer EditorDocument
    {
        get;
        set;
    }

    /// <summary>
    ///     Gets or sets the ID of the entity associated with the document.
    /// </summary>
    /// <value>
    ///     The ID of the entity.
    /// </value>
    /// <remarks>
    ///     This parameter is used to specify the ID of the entity (such as Candidate, Requisition, Companies, Leads, or
    ///     Benefits) associated with the Word document that needs to be viewed.
    ///     It is used in the `OpenDialog` method to determine the entity type and load the appropriate document from the
    ///     specified location.
    /// </remarks>
    [Parameter]
    public int EntityID
    {
        get;
        set;
    }

    /// <summary>
    ///     Gets or sets the type of the entity associated with the document.
    /// </summary>
    /// <value>
    ///     The type of the entity.
    /// </value>
    /// <remarks>
    ///     This parameter is used to specify the type of the entity (such as Candidate=1, Requisition=2, Companies=3, Leads=4,
    ///     or Benefits=5) associated with the Word document that needs to be viewed.
    ///     It is used in the `OpenDialog` method to determine the entity type and load the appropriate document from the
    ///     specified location.
    /// </remarks>
    [Parameter]
    public EntityType EntityType
    {
        get;
        set;
    }

    /// <summary>
    ///     Gets or sets the internal file name of the document to be viewed.
    /// </summary>
    /// <value>
    ///     The internal file name of the document.
    /// </value>
    /// <remarks>
    ///     This parameter is used to specify the internal file name of the Word document that needs to be viewed.
    ///     It is used in the `OpenDialog` method to load the appropriate document from the specified location.
    /// </remarks>
    [Parameter]
    public string InternalFileName
    {
        get;
        set;
    }

    /// <summary>
    ///     Gets or sets a value indicating whether the document is currently being opened.
    /// </summary>
    /// <value>
    ///     <c>true</c> if the document is currently being opened; otherwise, <c>false</c>.
    /// </value>
    /// <remarks>
    ///     This property is used to manage the state of the document opening process.
    ///     It is set to <c>true</c> when the `OpenDialog` method is called and set to <c>false</c> when the document has been
    ///     loaded into the `SfDocumentEditor`.
    /// </remarks>
    private bool IsOpening
    {
        get;
        set;
    } = true;

    /*/// <summary>
    ///     Gets or sets the serialized string representation of the Word document.
    /// </summary>
    /// <value>
    ///     The serialized string representation of the Word document.
    /// </value>
    /// <remarks>
    ///     This property is used to store the serialized string representation of the Word document that needs to be viewed.
    ///     It is used in the `OpenDialog` method to open the document in the `SfDocumentEditor`.
    /// </remarks>
    private string SerializedString
    {
        get;
        set;
    }*/

    /// <summary>
    ///     Gets or sets the spinner control used in the ViewWordDocument component.
    /// </summary>
    /// <value>
    ///     The spinner control.
    /// </value>
    /// <remarks>
    ///     This control is used to display a loading spinner while the Word document is being loaded into the
    ///     DocumentEditorContainer.
    ///     It is referenced in the Razor markup of the ViewWordDocument component and is controlled in the `OpenDialog`
    ///     method.
    /// </remarks>
    private SfSpinner Spinner
    {
        get;
        set;
    }

    /// <summary>
    ///     Asynchronously opens the dialog to view the Word document associated with the specified entity.
    /// </summary>
    /// <remarks>
    ///     This method is responsible for loading the Word document associated with the entity specified by the `EntityID` and
    ///     `EntityType` parameters.
    ///     It first shows a loading spinner, then determines the entity type and loads the appropriate document from the
    ///     specified location.
    ///     The document is loaded into a `SfDocumentEditorContainer` for viewing.
    ///     After the document is loaded, the spinner is hidden and the state of the component is updated.
    /// </remarks>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    private async Task OpenDialog()
    {
        await Spinner.ShowAsync();
        string _entity = EntityType switch
                         {
                             EntityType.Candidate => "Candidate",
                             EntityType.Requisition => "Requisition",
                             EntityType.Companies => "Company",
                             EntityType.Leads => "Lead",
                             EntityType.Benefits => "Benefits",
                             _ => "Candidate"
                         };

        MemoryStream _stream = new(await File.ReadAllBytesAsync(Path.Combine(Start.UploadPath, "Uploads", _entity, EntityID.ToString(), InternalFileName))); //TODO: Include the correct path
        //PdfViewer.LoadAsync("data:application/pdf;base64," + _base64String);
        WordDocument _document = WordDocument.Load(_stream, ImportFormatType.Docx);
        await _stream.DisposeAsync();
        string _serializedString = System.Text.Json.JsonSerializer.Serialize(_document);
        using SfDocumentEditor _editor = EditorDocument.DocumentEditor;
        await _editor.OpenAsync(_serializedString);
        await Spinner.HideAsync();
        IsOpening = false;
        await EditorDocument.ResizeAsync();
        StateHasChanged();
    }

    /// <summary>
    ///     Handles the opening event of the dialog.
    /// </summary>
    /// <param name="obj">The event arguments for the dialog opening event.</param>
    /// <remarks>
    ///     This method is triggered when the dialog is about to open. It sets the `IsOpening` property to <c>true</c> to
    ///     indicate that the document is currently being opened.
    /// </remarks>
    private void Opening(BeforeOpenEventArgs obj) => IsOpening = true;

    /// <summary>
    ///     Handles the OverlayModalClick event of the dialog component.
    /// </summary>
    /// <param name="args">The arguments associated with the OverlayModalClick event.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <remarks>
    ///     This method is triggered when the modal overlay of the dialog component is clicked.
    ///     It hides the dialog component asynchronously.
    /// </remarks>
    private async Task OverlayClick(OverlayModalClickEventArgs args) => await Dialog.HideAsync();

    /// <summary>
    ///     Displays the Word document in a modal dialog.
    /// </summary>
    /// <remarks>
    ///     This method is used to show the dialog component of the ViewWordDocument Razor component.
    ///     It is called in various parts of the application where viewing of Word documents is required,
    ///     such as in the DocumentsCompanyPanel, DocumentsPanel, and DownloadsPanel classes.
    /// </remarks>
    public async Task ShowDialog() => await Dialog.ShowAsync();

    /// <summary>
    ///     Handles the toolbar click events in the document viewer.
    /// </summary>
    /// <param name="args">The arguments associated with the toolbar click event.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <remarks>
    ///     This method is triggered when a toolbar button is clicked in the document viewer.
    ///     It handles the 'save', 'print', and 'close' actions.
    ///     The 'save' action saves the current state of the document to the specified document location in the Docx format.
    ///     The 'print' action sends the document to the printer.
    ///     The 'close' action hides the document viewer dialog.
    /// </remarks>
    /// <explain>
    ///     This method is part of the ViewWordDocument class, which is a Razor component for viewing Word documents. The
    ///     ToolbarClicked method is responsible for handling the toolbar click events in the document viewer. It takes a
    ///     ClickEventArgs parameter which contains the details of the toolbar click event.<br />
    ///     When a toolbar button is clicked, this method is triggered and performs the corresponding action. If the 'save'
    ///     button is clicked, the current state of the document is saved to the specified document location in the Docx
    ///     format. On clicking the 'print' button is, the document will be sent to the printer. On Clicking the 'Close' button,
    ///     the document viewer dialog is hidden.
    /// </explain>
    private async Task ToolbarClicked(ClickEventArgs args)
    {
        switch (args.Item.Id)
        {
            case "save":
                await EditorDocument.DocumentEditor.SaveAsync(DocumentLocation, FormatType.Docx);
                break;
            case "print":
                await EditorDocument.DocumentEditor.PrintAsync();
                break;
            case "close":
                await Dialog.HideAsync();
                break;
        }
    }
}

/// <summary>
///     Represents the type of entity in the application.
/// </summary>
/// <remarks>
///     This enumeration is used in various parts of the application to distinguish between different types of entities
///     such as Candidates, Requisitions, Companies, Leads, and Benefits.
/// </remarks>
public enum EntityType
{
    Candidate = 1, Requisition = 2, Companies = 3, Leads = 4, Benefits = 5
}