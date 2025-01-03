#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.Server
// File Name:           EditContact.razor.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu
// Created On:          04-22-2024 15:04
// Last Updated On:     10-29-2024 15:10
// *****************************************/

#endregion

#region Using

#endregion

namespace Subscription.Server.Components.Pages.Controls.Companies;

public partial class EditContact
{
    private readonly CompanyContactsValidator _companyContactValidator = new();

    [Parameter]
    public EventCallback<MouseEventArgs> Cancel
    {
        get;
        set;
    }

    private SfDataForm CompanyContactEditForm
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

    private List<LocationDrop> Location
    {
        get;
        set;
    } = [];

    [Parameter]
    public CompanyContacts Model
    {
        get;
        set;
    } = new();

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

    private async Task DialogOpen(BeforeOpenEventArgs args)
    {
        Dictionary<string, string> _parameters = new()
                                                 {
                                                     {"companyID", Model.CompanyID.ToString()}
                                                 };
        Location = await General.GetRest<List<LocationDrop>>("Company/GetLocationList", _parameters);
        CompanyContactEditForm.EditContext?.Validate();
    }

    private void LocationChanged(ChangeEventArgs<int, LocationDrop> location)
    {
        Model.StreetName = location.ItemData.StreetAddress;
        Model.City = location.ItemData.City;
        Model.State = location.ItemData.State;
        Model.ZipCode = location.ItemData.Zip;
        Model.StateID = location.ItemData.StateID;
        StateHasChanged();
    }

    protected override void OnParametersSet()
    {
        Context = new(Model);
        Context.OnFieldChanged += Context_OnFieldChanged;
        base.OnParametersSet();
    }

    private async Task SaveCompanyContact(EditContext args)
    {
        await General.DisplaySpinner(Spinner);
        await Save.InvokeAsync(args);
        await Dialog.HideAsync();
        await General.DisplaySpinner(Spinner, false);
    }

    internal async Task ShowDialog() => await Dialog.ShowAsync();
}