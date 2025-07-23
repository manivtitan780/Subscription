﻿#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.Server
// File Name:           ContactPanel.razor.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu
// Created On:          04-22-2024 15:04
// Last Updated On:     10-29-2024 15:10
// *****************************************/

#endregion

namespace Subscription.Server.Components.Pages.Controls.Companies;

public partial class ContactPanel
{
    private int _selectedID;

    /// <summary>
    ///     Gets or sets the event callback that triggers when an education detail is to be deleted.
    /// </summary>
    /// <value>
    ///     The event callback for deleting an education detail.
    /// </value>
    [Parameter]
    public EventCallback<int> DeleteCompanyContact
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
    ///     Gets or sets the event callback that triggers when a contact is to be edited.
    /// </summary>
    /// <value>
    ///     The event callback for editing a contact.
    /// </value>
    [Parameter]
    public EventCallback<int> EditCompanyContact
    {
        get;
        set;
    }

    private SfGrid<CompanyContacts> Grid
    {
        get;
        set;
    }

    [Parameter]
    public List<CompanyContacts> Model
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
    ///     Gets the selected row in the grid. This property is of type <see cref="CompanyContacts" />.
    /// </summary>
    /// <value>
    ///     The selected row in the grid, represented as a CandidateEducation object.
    /// </value>
    /// <remarks>
    ///     This property is set internally when a row is selected in the grid. It is used to hold the education details of the
    ///     selected candidate.
    /// </remarks>
    internal CompanyContacts SelectedRow
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
    private async Task DeleteCompanyContactMethod(int id)
    {
        _selectedID = id;
        int _index = await Grid.GetRowIndexByPrimaryKeyAsync(id);
        await Grid.SelectRowAsync(_index);
        if (await DialogService.ConfirmAsync(null, "Delete Company Contact", General.DialogOptions("Are you sure you want to <strong>disable</strong> this <i>Company Contact</i>?")))
        {
            await DeleteCompanyContact.InvokeAsync(_selectedID);
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
    private async Task EditCompanyContactDialog(int id)
    {
        _selectedID = id;
        int _index = await Grid.GetRowIndexByPrimaryKeyAsync(id);
        await Grid.SelectRowAsync(_index);
        await EditCompanyContact.InvokeAsync(id);
    }

    /// <summary>
    ///     Handles the row selection event in the contacts details grid.
    /// </summary>
    /// <param name="contact">The event arguments containing the selected row data of type <see cref="CompanyContacts" />.</param>
    /// <remarks>
    ///     This method is triggered when a row is selected in the contacts grid.
    ///     It sets the SelectedRow property to the data of the selected row.
    /// </remarks>
    private void RowSelected(RowSelectEventArgs<CompanyContacts> contact)
    {
        if (contact != null)
        {
            SelectedRow = contact.Data;
        }
    }

    // Memory optimization: Use same pattern as LocationPanel with pre-allocated string array
    private static MarkupString SetupAddress(CompanyContacts contact)
    {
        // Memory optimization: Use pre-allocated string array instead of string concatenation
        string[] addressParts = new string[4]; // Street, City, State, Zip
        int partsCount = 0;

        if (contact.StreetName.NotNullOrWhiteSpace())
        {
            addressParts[partsCount++] = contact.StreetName;
        }

        if (contact.City.NotNullOrWhiteSpace())
        {
            addressParts[partsCount++] = contact.City;
        }

        if (contact.StateID > 0)
        {
            try
            {
                string stateName = contact.State?.Trim();
                if (stateName.NotNullOrWhiteSpace())
                {
                    addressParts[partsCount++] = $"<strong>{stateName}</strong>";
                }
            }
            catch (Exception ex)
            {
                // Log error for random State access failures - maintaining original behavior
                Log.Error(ex, "Error accessing State property for contact {ContactId}", contact.ID);
            }
        }

        if (contact.ZipCode.NotNullOrWhiteSpace())
        {
            addressParts[partsCount++] = contact.ZipCode;
        }

        return string.Join("<br/>", addressParts.AsSpan(0, partsCount)).ToMarkupString();
    }

    // Template string optimization: Extract formatted name helper
    private static string GetFormattedName(CompanyContacts contact) =>
        $"{contact.FirstName} {contact.LastName}";

    // Template string optimization: Extract formatted update info helper
    private static string GetFormattedUpdateInfo(CompanyContacts contact) =>
        $"{contact.UpdatedDate.CultureDate()} [{contact.UpdatedBy}]";
}