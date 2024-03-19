#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.Client
// File Name:           EditCompany.razor.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu
// Created On:          3-7-2024 19:59
// Last Updated On:     3-18-2024 20:37
// *****************************************/

#endregion

#region Using

using Microsoft.AspNetCore.Components.Forms;

using Syncfusion.Blazor.DataForm;

#endregion

namespace Subscription.Client.Pages.Controls.Companies;

public partial class EditCompany
{
    [Parameter]
    public EventCallback<MouseEventArgs> Cancel
    {
        get;
        set;
    }

    private SfDataForm CompanyEditForm
    {
        get;
        set;
    }

    private SfDialog Dialog
    {
        get;
        set;
    }

    private EditContext EditContext
    {
        get;
        set;
    }

    [Parameter]
    public CompanyDetails Model
    {
        get;
        set;
    } = new();

    [Parameter]
    public List<IntValues> NAICS
    {
        get;
        set;
    } = [];

    [Parameter]
    public EventCallback<EditContext> Save
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
    public List<IntValues> State
    {
        get;
        set;
    } = [];

    private async Task CancelForm(MouseEventArgs args)
    {
        await Cancel.InvokeAsync(args);
        await Dialog.HideAsync();
    }

    private void DialogOpen(BeforeOpenEventArgs args) => CompanyEditForm.EditContext?.Validate();

    protected override void OnParametersSet()
    {
        EditContext = new(Model);
        base.OnParametersSet();
    }

    private async Task SaveCompany(EditContext args)
    {
        await Save.InvokeAsync(args);
        await Dialog.HideAsync();
    }

    internal async Task ShowDialog() => await Dialog.ShowAsync();
}