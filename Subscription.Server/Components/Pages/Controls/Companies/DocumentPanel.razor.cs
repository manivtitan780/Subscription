#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.Server
// File Name:           DocumentPanel.razor.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu, Brijesh Dubey
// Created On:          07-23-2025 15:07
// Last Updated On:     07-23-2025 15:59
// *****************************************/

#endregion

namespace Subscription.Server.Components.Pages.Controls.Companies;

public partial class DocumentPanel
{
    private int _selectedID;

    [Parameter]
    public bool CanAdministerDocuments { get; set; } = true;

    [Parameter]
    public EventCallback<int> DeleteCompanyDocument { get; set; }

    [Inject]
    private SfDialogService DialogService { get; set; }

    private ViewPDFDocument DocumentViewPDF { get; set; }

    [Parameter]
    public EventCallback<int> DownloadDocument { get; set; }

    [Parameter]
    public EventCallback<int> EditCompanyDocument { get; set; }

    private SfGrid<CompanyDocuments> Grid { get; set; }

    [Parameter]
    public List<CompanyDocuments> Model { get; set; }

    [Parameter]
    public double RowHeight { get; set; } = 45;

    internal CompanyDocuments SelectedRow { get; set; } = new();

    private bool VisibleSpinner { get; set; }

    private async Task DeleteCompanyDocumentMethod(int id)
    {
        _selectedID = id;
        int _index = await Grid.GetRowIndexByPrimaryKeyAsync(id);
        await Grid.SelectRowAsync(_index);
        if (await DialogService.ConfirmAsync(null, "Delete Company Document", General.DialogOptions("Are you sure you want to <strong>delete</strong> this <i>Company Document</i>?")))
        {
            await DeleteCompanyDocument.InvokeAsync(_selectedID);
        }
    }

    private async Task DownloadDocumentDialog(int id)
    {
        _selectedID = id;
        int _index = await Grid.GetRowIndexByPrimaryKeyAsync(id);
        await Grid.SelectRowAsync(_index);
        await DownloadDocument.InvokeAsync(id);
    }

    private async Task EditCompanyDocumentDialog(int id)
    {
        _selectedID = id;
        int _index = await Grid.GetRowIndexByPrimaryKeyAsync(id);
        await Grid.SelectRowAsync(_index);
        await EditCompanyDocument.InvokeAsync(id);
    }

    // Template optimization: Extract formatted update info helper
    private static string GetFormattedUpdateInfo(CompanyDocuments doc) => $"{doc.UpdatedDate.CultureDate()} [{doc.UpdatedBy}]";

    private void RowSelected(RowSelectEventArgs<CompanyDocuments> document)
    {
        if (document != null)
        {
            SelectedRow = document.Data;
        }
    }

    private async Task ViewDocumentDialog(CompanyDocuments con)
    {
        VisibleSpinner = true;
        string _fileExtension = Path.GetExtension(con.FileName);
        string[] _allowedExtensions = [".pdf", ".doc", ".docx", ".rtf"];
        if (_allowedExtensions.Contains(_fileExtension, StringComparer.OrdinalIgnoreCase))
        {
            await DocumentViewPDF.ShowDialog();
        }

        VisibleSpinner = false;
    }
}