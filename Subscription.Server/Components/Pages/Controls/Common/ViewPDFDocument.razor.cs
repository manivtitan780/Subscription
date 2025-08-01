﻿#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.Server
// File Name:           ViewPDFDocument.razor.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu, Brijesh Dubey
// Created On:          02-05-2025 20:02
// Last Updated On:     05-09-2025 20:03
// *****************************************/

#endregion

#region Using

using Extensions.Memory;
using ToolbarItem = Syncfusion.Blazor.SfPdfViewer.ToolbarItem;
using WFormatType = Syncfusion.DocIO.FormatType;
using WordDocument = Syncfusion.DocIO.DLS.WordDocument;

#endregion

namespace Subscription.Server.Components.Pages.Controls.Common;

/// <summary>
///     The ViewPDFDocument class is a component that provides functionality to view a PDF document.
///     It includes parameters for the document location, document name, entity ID, entity type, and internal file name.
///     The ShowDialog method can be used to display the PDF document in a dialog.
/// </summary>
public partial class ViewPDFDocument
{
    /// <summary>
    ///     Defines the set of toolbar items available in the PDF viewer. The toolbar includes page navigation,
    ///     magnification, selection, pan, print, and download options.
    /// </summary>
    private readonly List<ToolbarItem> _toolbarItems =
    [
        ToolbarItem.PageNavigationTool,
        ToolbarItem.MagnificationTool,
        ToolbarItem.SelectionTool,
        ToolbarItem.PanTool,
        ToolbarItem.SearchOption,
        ToolbarItem.PrintOption /*,
        ToolbarItem.DownloadOption*/
    ];

    /// <summary>
    ///     Gets or sets the SfDialog instance used to display the PDF document.
    ///     This dialog is configured with various settings such as modal display, minimum height, and custom templates for the
    ///     header and content.
    ///     The dialog's visibility is controlled programmatically using the ShowAsync and HideAsync methods.
    /// </summary>
    private SfDialog Dialog { get; set; }

    /// <summary>
    ///     Gets or sets the location of the document. This property is used to determine the type of the document
    ///     and the way it should be processed. The value of this property should be a string representing the path
    ///     or URL of the document.
    /// </summary>
    [Parameter]
    public string DocumentLocation { get; set; }

    /// <summary>
    ///     Gets or sets the name of the document. This property is used to display the document's name
    ///     in the dialog header when the PDF document is viewed.
    /// </summary>
    [Parameter]
    public string DocumentName { get; set; }

    private string DownloadFileName { get; set; }

    /// <summary>
    ///     Gets or sets the ID of the entity. This property is used to identify the specific entity
    ///     associated with the document. The value of this property should be an integer representing
    ///     the unique identifier of the entity.
    /// </summary>
    [Parameter]
    public int EntityID { get; set; }

    /// <summary>
    ///     Gets or sets the type of the entity. This property is used to determine the specific type of the entity
    ///     associated with the document. The value of this property should be one of the values from the EntityType
    ///     enumeration,
    ///     which includes Candidate, Requisition, Companies, Leads, and Benefits.
    /// </summary>
    [Parameter]
    public EntityType EntityType { get; set; }

    /// <summary>
    ///     Gets or sets the internal file name of the document. This property is used to construct the file path
    ///     for the document based on the entity type and entity ID. The value of this property should be a string
    ///     representing the internal name of the file.
    /// </summary>
    [Parameter]
    public string InternalFileName { get; set; }

    /// <summary>
    ///     Gets or sets the PdfViewer component. This component is used to display the PDF document in the dialog.
    ///     It is a private member of the ViewPDFDocument class and is used internally in the OpenDialog method to load and
    ///     display the PDF document.
    /// </summary>
    private SfPdfViewer2 PdfViewer { get; set; }

    /// <summary>
    ///     Gets or sets the SfSpinner component. This component is used to display a loading spinner
    ///     while the PDF document is being loaded and processed.
    /// </summary>
    private SfSpinner Spinner { get; set; }

    /// <summary>
    ///     Closes the dialog containing the PDF viewer.
    ///     This method is called when the dialog is closed.
    ///     It unloads the PDF viewer and hides the loading spinner.
    /// </summary>
    /// <param name="arg">
    ///     The argument passed to the method when the dialog is closed.
    /// </param>
    /// <returns>
    ///     A Task representing the asynchronous operation.
    /// </returns>
    private async Task CloseDialog(CloseEventArgs arg)
    {
        await PdfViewer.UnloadAsync();
    }

    /// <summary>
    ///     Determines the format type of the document based on its file extension.
    /// </summary>
    /// <returns>
    ///     Returns a value of the WFormatType (Word Format Type) enumeration which represents the format of the document.
    ///     The possible return values are Docx, Doc, Rtf, or Automatic.
    /// </returns>
    private WFormatType GetWFormatType()
    {
        if (DocumentLocation.EndsWith(".docx"))
        {
            return WFormatType.Docx;
        }

        if (DocumentLocation.EndsWith(".doc"))
        {
            return WFormatType.Doc;
        }

        return DocumentLocation.EndsWith(".rtf") ? WFormatType.Rtf : WFormatType.Automatic;
    }

    /// <summary>
    ///     Asynchronously opens the dialog to display the PDF document. This method first shows a loading spinner,
    ///     then reads the document file based on the entity type and entity ID, and converts it to a base64 string.
    ///     If the document is not a PDF, it is converted to a PDF. The base64 string is then loaded into the PDF viewer.
    ///     Finally, the loading spinner is hidden, and the component state is updated.
    ///     
    ///     Memory optimizations: Uses ReusableMemoryStream and ArrayPool-based Base64 conversion to avoid LOH allocations.
    /// </summary>
    /// <returns>A Task representing the asynchronous operation.</returns>
    private async Task OpenDialog()
    {
        await Spinner.ShowAsync();
        await Task.Yield();
        
        string _entity = EntityType switch
                         {
                             EntityType.Candidate => "Candidate",
                             EntityType.Requisition => "Requisition",
                             EntityType.Companies => "Company",
                             EntityType.Leads => "Leads",
                             EntityType.Benefits => "Benefits",
                             _ => "Candidate"
                         };

        //Connect to the Azure Blob Storage
        using IAzureBlobStorage _storage = StorageFactory.Blobs.AzureBlobStorageWithSharedKey(Start.AccountName, Start.AzureKey);

        //Create the Path to the File in Azure Blob Storage
        string _blobPath = $"{Start.AzureBlobContainer}/{_entity}/{EntityID}/{InternalFileName}";

        //Read the file into a Bytes Array
        byte[] _memBytes = await _storage.ReadBytesAsync(_blobPath);
        
        string _dataUri;
        if (DocumentLocation.EndsWith(".pdf"))
        {
            // Memory optimization: Use ArrayPool-based Base64 conversion to avoid LOH allocation
            _dataUri = Base64Helper.CreatePdfDataUri(_memBytes);
        }
        else
        {
            // Memory optimization: Use RecyclableMemoryStream with tagging for monitoring
            using var _inputStream = ReusableMemoryStream.Get("pdf-input", _memBytes);
            using var _outputStream = ReusableMemoryStream.Get("pdf-output");
            
            using (var _doc = new WordDocument(_inputStream, GetWFormatType()))
            using (var _pdfDocument = new DocIORenderer().ConvertToPDF(_doc))
            {
                _pdfDocument.Save(_outputStream);
                _outputStream.Position = 0;
                
                // Memory optimization: Use ArrayPool-based Base64 conversion to avoid LOH allocation
                _dataUri = Base64Helper.CreatePdfDataUri(_outputStream);
            }
        }

        // Memory optimization: Remove manual nulling - let GC handle disposal
        DownloadFileName = DocumentLocation.EndsWith(".pdf") ? DocumentLocation : $"{DocumentLocation}.pdf";
        await PdfViewer.LoadAsync(_dataUri);
        await Spinner.HideAsync();
        StateHasChanged();
    }

    /// <summary>
    ///     Handles the OverlayClick event of the SfDialog.
    ///     This method is invoked when the modal overlay of the dialog is clicked.
    ///     It hides the dialog asynchronously.
    /// </summary>
    /// <param name="args">The arguments of the OverlayModalClick event.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    private async Task OverlayClick(OverlayModalClickEventArgs args) => await Dialog.HideAsync();

    /// <summary>
    ///     Asynchronously displays the PDF document in a dialog.
    /// </summary>
    /// <remarks>
    ///     This method uses the `ShowAsync` method of the `SfDialog` instance to display the PDF document.
    ///     The document's location, name, associated entity ID and type, and internal file name are all taken into account
    ///     when displaying the document.
    ///     This method is typically called when the document location ends with ".pdf", indicating that the document is a PDF
    ///     file.
    /// </remarks>
    /// <returns>
    ///     A `Task` representing the asynchronous operation.
    /// </returns>
    public async Task ShowDialog() => await Dialog.ShowAsync();
}