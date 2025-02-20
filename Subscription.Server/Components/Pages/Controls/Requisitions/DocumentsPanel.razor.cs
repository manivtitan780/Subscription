#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            ProfSvc_AppTrack
// Project:             ProfSvc_AppTrack
// File Name:           DocumentsPanel.razor.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja
// Created On:          12-09-2022 15:57
// Last Updated On:     10-02-2023 19:24
// *****************************************/

#endregion

namespace Subscription.Server.Components.Pages.Controls.Requisitions;

/// <summary>
///     Represents a panel for managing requisition documents.
/// </summary>
/// <remarks>
///     This class provides functionalities for deleting, downloading, and editing requisition documents.
///     It also provides functionalities for controlling the display of the documents panel, such as the height of the
///     panel and the height of each row in the documents grid.
/// </remarks>
public partial class DocumentsPanel
{
    private string _internalFileName = "", _documentName = "", _documentLocation = "";
    private int _requisitionID;

    private int _selectedID;

    /// <summary>
    ///     Gets or sets the event callback for deleting a document.
    /// </summary>
    /// <value>
    ///     The event callback that takes an integer as parameter representing the ID of the document to be deleted.
    /// </value>
    /// <remarks>
    ///     This property is used to initiate the deletion process for a document in the DocumentsPanel.
    ///     The integer parameter represents the ID of the document to be deleted.
    /// </remarks>
    [Parameter]
    public EventCallback<int> DeleteDocument
    {
        get;
        set;
    }

    /// <summary>
    ///     Gets or sets the ConfirmDialog component used for user action confirmation.
    /// </summary>
    /// <value>
    ///     The ConfirmDialog component.
    /// </value>
    /// <remarks>
    ///     This property is used to display a dialog for confirming various user actions such as deletion of documents.
    /// </remarks>
    private ConfirmDialog DialogConfirm
    {
        get;
        set;
    }

    /// <summary>
    ///     Gets or sets the ViewPDFDocument component used for displaying PDF documents.
    /// </summary>
    /// <value>
    ///     The ViewPDFDocument component.
    /// </value>
    /// <remarks>
    ///     This property is used to manage the display of PDF documents in the DocumentsPanel.
    ///     It is used in the ViewDocumentDialog method to show the dialog with the PDF document.
    /// </remarks>
    private ViewPDFDocument DocumentViewPDF
    {
        get;
        set;
    }

    /// <summary>
    ///     Gets or sets the event callback for downloading a document.
    /// </summary>
    /// <value>
    ///     The event callback that takes an integer as parameter representing the ID of the document to be downloaded.
    /// </value>
    /// <remarks>
    ///     This property is used to initiate the download process for a document in the DocumentsPanel.
    ///     The integer parameter represents the ID of the document to be downloaded.
    /// </remarks>
    [Parameter]
    public EventCallback<int> DownloadDocument
    {
        get;
        set;
    }

    /// <summary>
    ///     Gets or sets a value indicating whether the user has the rights to edit the documents.
    /// </summary>
    /// <value>
    ///     <c>true</c> if the user has edit rights; otherwise, <c>false</c>. The default value is <c>true</c>.
    /// </value>
    /// <remarks>
    ///     This property is used to control the visibility of the edit and delete buttons in the DocumentsPanel.
    ///     If the user has edit rights and is the owner of the requisition, the edit and delete buttons are displayed.
    /// </remarks>
    [Parameter]
    public bool EditRights
    {
        get;
        set;
    } = true;

    /// <summary>
    ///     Gets or sets the type of the entity associated with the DocumentsPanel.
    /// </summary>
    /// <value>
    ///     The type of the entity.
    /// </value>
    /// <remarks>
    ///     This property is used to determine the controller for the REST request in the ViewDocumentDialog method.
    ///     The type of the entity can be one of the values from the EntityType enumeration.
    /// </remarks>
    [Parameter]
    public EntityType EntityTypeName
    {
        get;
        set;
    } = EntityType.Requisition;

    /// <summary>
    ///     Gets or sets the grid for downloading requisition documents.
    /// </summary>
    /// <value>
    ///     The grid of type <see cref="SfGrid{RequisitionDocuments}" />.
    /// </value>
    /// <remarks>
    ///     This property is used to manage the grid display of requisition documents for download.
    ///     It is used in various places throughout the application, such as in the DeleteDocumentMethod and
    ///     DownloadDocumentDialog methods,
    ///     to initiate the deletion or download of a selected requisition document.
    /// </remarks>
    public SfGrid<RequisitionDocuments> GridDownload
    {
        get;
        set;
    }

    /// <summary>
    ///     Gets or sets the height of the DocumentsPanel.
    /// </summary>
    /// <value>
    ///     The height of the DocumentsPanel in pixels or percentage. The default value is "450px".
    /// </value>
    /// <remarks>
    ///     This property is used to control the display of the DocumentsPanel. It determines the vertical space the
    ///     DocumentsPanel occupies.
    /// </remarks>
    [Parameter]
    public string Height
    {
        get;
        set;
    } = "450px";

    /// <summary>
    ///     Gets or sets the JavaScript runtime for the DocumentsPanel.
    /// </summary>
    /// <value>
    ///     The JavaScript runtime used for interop calls between .NET and JavaScript in the DocumentsPanel.
    /// </value>
    /// <remarks>
    ///     This property is injected by the framework and provides the ability to invoke JavaScript functions from .NET code.
    /// </remarks>
    [Inject]
    private IJSRuntime JsRuntime
    {
        get;
        set;
    }

    /// <summary>
    ///     Gets or sets the list of RequisitionDocuments for the DocumentsPanel.
    /// </summary>
    /// <value>
    ///     The list of RequisitionDocuments. This list is used as the data source for the grid in the DocumentsPanel.
    /// </value>
    /// <remarks>
    ///     This property is used to populate the grid in the DocumentsPanel with the relevant requisition documents. It is set
    ///     by the parent component and updated whenever the list of requisition documents changes.
    /// </remarks>
    [Parameter]
    public List<RequisitionDocuments> Model
    {
        get;
        set;
    }

    /// <summary>
    ///     Gets or sets the height of each row in the DocumentsPanel grid.
    /// </summary>
    /// <value>
    ///     The height of a row in pixels. The default value is 38.
    /// </value>
    /// <remarks>
    ///     This property is used to control the display of the grid in the DocumentsPanel. It determines the vertical space
    ///     each row in the grid occupies.
    /// </remarks>
    [Parameter]
    public double RowHeight
    {
        get;
        set;
    } = 38;

    /// <summary>
    ///     Gets or sets the selected row in the DocumentsPanel.
    /// </summary>
    /// <value>
    ///     The selected row of type <see cref="RequisitionDocuments" />.
    /// </value>
    /// <remarks>
    ///     This property is used to hold the currently selected row in the DocumentsPanel. It is used in various places
    ///     throughout the application, such as in the DownloadDocument methods in the Leads, Requisition, and
    ///     CompanyRequisitions classes, to initiate the download of a selected requisition document.
    /// </remarks>
    public RequisitionDocuments SelectedRow
    {
        get;
        set;
    }

    /// <summary>
    ///     Gets or sets the SfSpinner control used in the DocumentsPanel.
    /// </summary>
    /// <value>
    ///     The SfSpinner is a Syncfusion control that provides a visual indication when an operation is being performed.
    ///     It is used in this context to indicate that a document is being loaded or an operation is being processed.
    /// </value>
    private SfSpinner Spinner
    {
        get;
        set;
    }

    /// <summary>
    ///     Gets or sets the User associated with the RequisitionDocuments in the DocumentsPanel.
    /// </summary>
    /// <value>
    ///     The User is a string representing the user who owns the RequisitionDocuments.
    ///     This property is used to determine if the current user has the rights to edit the RequisitionDocuments.
    /// </value>
    [Parameter]
    public string User
    {
        get;
        set;
    } = "";

    private bool VisibleSpinner
    {
        get;
        set;
    }

    /// <summary>
    ///     Asynchronously deletes a document with the specified ID.
    /// </summary>
    /// <param name="id">The ID of the document to delete.</param>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    /// <remarks>
    ///     This method first sets the selected ID to the provided ID, then finds the index of the document in the grid using
    ///     the provided ID.
    ///     It then selects the row in the grid corresponding to the found index and shows a confirmation dialog to the user.
    /// </remarks>
    private async Task DeleteDocumentMethod(int id)
    {
        _selectedID = id;
        int _index = await GridDownload.GetRowIndexByPrimaryKeyAsync(id);
        await GridDownload.SelectRowAsync(_index);
        await DialogConfirm.ShowDialog();
    }

    /// <summary>
    ///     Initiates the download process for a document.
    /// </summary>
    /// <param name="id">The ID of the document to be downloaded.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    private async Task DownloadDocumentDialog(int id)
    {
        int _index = await GridDownload.GetRowIndexByPrimaryKeyAsync(id);
        await GridDownload.SelectRowAsync(_index);
        await DownloadDocument.InvokeAsync(id);
    }

    /// <summary>
    ///     Handles the event when a row is selected in the grid.
    /// </summary>
    /// <param name="row">The selected row data of type RequisitionDocuments."/></param>
    private void RowSelected(RowSelectEventArgs<RequisitionDocuments> row)
    {
        if (row != null)
        {
            SelectedRow = row.Data;
        }
    }

    /// <summary>
    ///     Asynchronously displays a dialog to view a document.
    /// </summary>
    /// <param name="documentID">The ID of the document to be viewed.</param>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    /// <remarks>
    ///     This method fetches the document details from the server using the provided document ID.
    ///     It then determines the type of the document (PDF or Word) and displays the appropriate dialog for viewing the
    ///     document.
    /// </remarks>
    private async Task ViewDocumentDialog(int documentID)
    {
        VisibleSpinner = true;

        Dictionary<string, string> _parameters = new()
                                                 {
                                                     {"documentID", documentID.ToString()}
                                                 };

        string _controller = EntityTypeName == EntityType.Requisition ? "Requisition" : "Lead";
        string _response = await General.ExecuteRest<string>($"{_controller}/DownloadFile", _parameters, null, false);
        DocumentDetails _restResponse = JsonConvert.DeserializeObject<DocumentDetails>(_response);

        if (_restResponse != null)
        {
            string _location = _restResponse.DocumentLocation;
            if (_location.EndsWith(".pdf") || _location.EndsWith(".doc") || _location.EndsWith(".docx") || _location.EndsWith(".rtf"))
            {
                _requisitionID = _restResponse.EntityID;
                _documentName = _restResponse.DocumentName;
                _documentLocation = _location;
                _internalFileName = _restResponse.InternalFileName;
                await DocumentViewPDF.ShowDialog();
            }
        }

        VisibleSpinner = false;
    }
}