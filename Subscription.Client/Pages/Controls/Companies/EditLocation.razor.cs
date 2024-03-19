#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.Client
// File Name:           EditLocation.razor.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu
// Created On:          3-19-2024 16:15
// Last Updated On:     3-19-2024 16:15
// *****************************************/

#endregion

using Microsoft.AspNetCore.Components.Forms;

using Syncfusion.Blazor.DataForm;

namespace Subscription.Client.Pages.Controls.Companies;

public partial class EditLocation
{
    private SfDialog Dialog
    {
        get;
        set;
    }

    private SfSpinner Spinner
    {
        get;
        set;
    }

    [Parameter]
    public CompanyLocations Model
    {
        get;
        set;
    }

    private SfDataForm CompanyEditForm
    {
        get;
        set;
    }

    private Task DialogOpen(BeforeOpenEventArgs arg)
    {
        return null;
    }

    private Task SaveCompanyLocation(EditContext arg)
    {
        return null;
    }

    private Task CancelForm(MouseEventArgs arg)
    {
        return null;
    }
}