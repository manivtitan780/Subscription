#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.Server
// File Name:           DocumentPanel.razor.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu
// Created On:          04-22-2024 15:04
// Last Updated On:     10-29-2024 15:10
// *****************************************/

#endregion

namespace Subscription.Server.Components.Pages.Controls.Companies;

public partial class DocumentPanel
{
    private int _selectedID;

    /// <summary>
    ///     Gets or sets the event callback that triggers when an education detail is to be deleted.
    /// </summary>
    /// <value>
    ///     The event callback for deleting an education detail.
    /// </value>
    [Parameter]
    public EventCallback<int> DeleteCompanyDocument
    {
        get;
        set;
    }

    [Inject]
    private SfDialogService DialogService
    {
        get;
        set;
    }

    /// <summary>
    ///     Gets or sets the event callback that triggers when an education detail is to be edited.
    /// </summary>
    /// <value>
    ///     The event callback for editing an education detail.
    /// </value>
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
    } = 40;

    /// <summary>
    ///     Gets the selected row in the grid. This property is of type <see cref="CompanyDocuments" />.
    /// </summary>
    /// <value>
    ///     The selected row in the grid, represented as a CandidateEducation object.
    /// </value>
    /// <remarks>
    ///     This property is set internally when a row is selected in the grid. It is used to hold the education details of the
    ///     selected candidate.
    /// </remarks>
    private CompanyDocuments SelectedRow
    {
        get;
        set;
    }

    /// <summary>
    ///     Asynchronously deletes the education detail of a candidate.
    /// </summary>
    /// <param name="id">The ID of the education detail to be deleted.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    /// <remarks>
    ///     This method sets the selected ID to the provided ID, gets the index of the row in the grid corresponding to the ID,
    ///     selects the row in the grid, and shows a confirmation dialog.
    /// </remarks>
    private async Task DeleteCompanyDocumentMethod(int id)
    {
        _selectedID = id;
        int _index = await Grid.GetRowIndexByPrimaryKeyAsync(id);
        await Grid.SelectRowAsync(_index);
        //await DialogConfirm.ShowDialog();
        if (await DialogService.ConfirmAsync(null, "Delete Company Document", General.DialogOptions("Are you sure you want to <strong>delete</strong> this <i>Company Document</i>?")))
        {
            await DeleteCompanyDocument.InvokeAsync(_selectedID);
        }
    }

    /// <summary>
    ///     Asynchronously opens the dialog for editing the education details of a candidate.
    /// </summary>
    /// <param name="id">The ID of the education detail to be edited.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    /// <remarks>
    ///     This method sets the selected ID to the provided ID, gets the index of the row in the grid corresponding to the
    ///     provided ID, selects the row in the grid, and invokes the EditEducation event callback.
    /// </remarks>
    private async Task EditCompanyDocumentDialog(int id)
    {
        _selectedID = id;
        int _index = await Grid.GetRowIndexByPrimaryKeyAsync(id);
        await Grid.SelectRowAsync(_index);
        await EditCompanyDocument.InvokeAsync(id);
    }

    /// <summary>
    ///     Handles the row selection event in the Documents details grid.
    /// </summary>
    /// <param name="document">The event arguments containing the selected row data of type <see cref="CompanyDocuments" />.</param>
    /// <remarks>
    ///     This method is triggered when a row is selected in the education details grid.
    ///     It sets the SelectedRow property to the data of the selected row.
    /// </remarks>
    private void RowSelected(RowSelectEventArgs<CompanyDocuments> document)
    {
        if (document != null)
        {
            SelectedRow = document.Data;
        }
    }
}