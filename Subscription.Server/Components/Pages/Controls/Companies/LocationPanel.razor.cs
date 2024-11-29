#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.Client
// File Name:           LocationPanel.razor.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu
// Created On:          2-15-2024 15:26
// Last Updated On:     2-17-2024 19:8
// *****************************************/

#endregion

namespace Subscription.Server.Components.Pages.Controls.Companies;

public partial class LocationPanel
{
    private int _selectedID;

    /// <summary>
    ///     Gets or sets the event callback that triggers when an education detail is to be deleted.
    /// </summary>
    /// <value>
    ///     The event callback for deleting an education detail.
    /// </value>
    [Parameter]
    public EventCallback<int> DeleteCompanyLocation
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
    public EventCallback<int> EditCompanyLocation
    {
        get;
        set;
    }

    private SfGrid<CompanyLocations> Grid
    {
        get;
        set;
    }

    [Parameter]
    public List<CompanyLocations> Model
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
    ///     Gets the selected row in the grid. This property is of type <see cref="CompanyLocations" />.
    /// </summary>
    /// <value>
    ///     The selected row in the grid, represented as a CandidateEducation object.
    /// </value>
    /// <remarks>
    ///     This property is set internally when a row is selected in the grid. It is used to hold the education details of the
    ///     selected candidate.
    /// </remarks>
    public CompanyLocations SelectedRow
    {
        get;
        private set;
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
    private async Task DeleteCompanyLocationMethod(int id)
    {
        _selectedID = id;
        int _index = await Grid.GetRowIndexByPrimaryKeyAsync(id);
        await Grid.SelectRowAsync(_index);
        //await DialogConfirm.ShowDialog();
        if (await DialogService.ConfirmAsync(null, "Delete Company Location", General.DialogOptions("Are you sure you want to <strong>disable</strong> this <i>Company Location</i>?")))
        {
            await DeleteCompanyLocation.InvokeAsync(_selectedID);
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
    private async Task EditCompanyLocationDialog(int id)
    {
        _selectedID = id;
        int _index = await Grid.GetRowIndexByPrimaryKeyAsync(id);
        await Grid.SelectRowAsync(_index);
        await EditCompanyLocation.InvokeAsync(id);
    }

    /// <summary>
    ///     Handles the row selection event in the locations details grid.
    /// </summary>
    /// <param name="location">The event arguments containing the selected row data of type <see cref="CompanyLocations" />.</param>
    /// <remarks>
    ///     This method is triggered when a row is selected in the education details grid.
    ///     It sets the SelectedRow property to the data of the selected row.
    /// </remarks>
    private void RowSelected(RowSelectEventArgs<CompanyLocations> location)
    {
        if (location != null)
        {
            SelectedRow = location.Data;
        }
    }

    private MarkupString SetupAddress(CompanyLocations location)
    {
        string _generateAddress = location.StreetName;

        if (_generateAddress == "")
        {
            _generateAddress = location.City;
        }
        else
        {
            _generateAddress += location.City == "" ? "" : $"<br/>{location.City}";
        }

        if (location.StateID > 0)
        {
            if (_generateAddress == "")
            {
                _generateAddress = $"<strong>{location.State.Trim()}</strong>";
            }
            else
            {
                try //Because sometimes the default values are not getting set. It's so random that it can't be debugged. And it never fails during debugging session.
                {
                    _generateAddress += $", <strong>{location.State.Trim()}</strong>";
                }
                catch
                {
                    //
                }
            }
        }

        if (location.ZipCode != "")
        {
            if (_generateAddress == "")
            {
                _generateAddress = location.ZipCode;
            }
            else
            {
                _generateAddress += ", " + location.ZipCode;
            }
        }

        if (_generateAddress != null && _generateAddress.StartsWith(","))
        {
            _generateAddress = _generateAddress[1..].Trim();
        }

        return _generateAddress.ToMarkupString();
    }
}