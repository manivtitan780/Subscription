#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.Server
// File Name:           DocumentPanel.razor.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu, Brijesh Dubey
// Created On:          04-22-2024 15:04
// Last Updated On:     04-28-2025 20:30
// *****************************************/

#endregion

namespace Subscription.Server.Components.Pages.Controls.Companies;

public partial class DocumentPanel
{
    private int _selectedID;

    [Parameter]
    public EventCallback<int> DeleteCompanyDocument
    {
        get;
        set;
    }

    [Parameter]
    public bool CanAdministerDocuments
    {
        get;
        set;
    } = true;

    [Inject]
    private SfDialogService DialogService
    {
        get;
        set;
    }

    [Parameter]
    public EventCallback<int> DownloadDocument
    {
        get;
        set;
    }

    [Parameter]
    public EventCallback<int> EditCompanyDocument
    {
        get;
        set;
    }

    private SfGrid<CompanyDocuments> Grid
    {
        get;
        set;
    }

    [Parameter]
    public List<CompanyDocuments> Model
    {
        get;
        set;
    }

    [Parameter]
    public double RowHeight
    {
        get;
        set;
    } = 45;

    internal CompanyDocuments SelectedRow
    {
        get;
        set;
    } = new();

    private bool VisibleSpinner
    {
        get;
        set;
    }

    private ViewPDFDocument DocumentViewPDF
    {
        get;
        set;
    }

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

    private void RowSelected(RowSelectEventArgs<CompanyDocuments> document)
    {
        if (document != null)
        {
            SelectedRow = document.Data;
        }
    }

    private Task ViewDocumentDialog(CompanyDocuments con)
    {
        return null;
    }
}