#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.Server
// File Name:           EditCompany.razor.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu
// Created On:          04-22-2024 15:04
// Last Updated On:     10-29-2024 15:10
// *****************************************/

#endregion

#region Using

#endregion

namespace Subscription.Server.Components.Pages.Controls.Companies;

public partial class EditCompany
{
    private readonly CompanyDetailsValidator _companyValidator = new();

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

    private EditContext Context
    {
        get;
        set;
    }

    private SfDialog Dialog
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
        await General.DisplaySpinner(Spinner);
        await Cancel.InvokeAsync(args);
        await Dialog.HideAsync();
        await General.DisplaySpinner(Spinner, false);
    }

    private void Context_OnFieldChanged(object sender, FieldChangedEventArgs e)
    {
        Context.Validate();
    }

    private void DialogOpen(BeforeOpenEventArgs args)
    {
        CompanyEditForm.EditContext?.Validate();
    }

    protected override void OnParametersSet()
    {
        Context = new(Model);
        Context.OnFieldChanged += Context_OnFieldChanged;
        base.OnParametersSet();
    }

    private async Task SaveCompany(EditContext args)
    {
        await General.DisplaySpinner(Spinner);
        await Save.InvokeAsync(args);
        await Dialog.HideAsync();
        await General.DisplaySpinner(Spinner, false);
    }

    internal async Task ShowDialog() => await Dialog.ShowAsync();
}