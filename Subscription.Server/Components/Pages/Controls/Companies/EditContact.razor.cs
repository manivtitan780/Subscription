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

namespace Subscription.Server.Components.Pages.Controls.Companies;

public partial class EditContact : IDisposable
{
    private readonly CompanyContactsValidator _companyContactValidator = new();

    // Memory optimization: Track model changes to avoid unnecessary Context recreation
    private CompanyContacts _currentModel;

    [Parameter]
    public EventCallback<MouseEventArgs> Cancel { get; set; }

    private SfDataForm CompanyContactEditForm { get; set; }

    private EditContext Context { get; set; }

    private SfDialog Dialog { get; set; }

    private List<LocationDrop> Location { get; set; } = [];

    [Parameter]
    public CompanyContacts Model { get; set; } = new();

    [Parameter]
    public EventCallback<EditContext> Save { get; set; }

    private bool VisibleSpinner { get; set; }

    public void Dispose()
    {
        // Memory optimization: No event handlers to unsubscribe from
        // EditContext handles validation automatically
        GC.SuppressFinalize(this);
    }

    private async Task CancelForm(MouseEventArgs args)
    {
        VisibleSpinner = true;
        await Cancel.InvokeAsync(args);
        await Dialog.HideAsync();
        VisibleSpinner = false;
    }

    // Memory optimization: Removed redundant Context_OnFieldChanged event handler
    // EditContext already handles field validation automatically

    private async Task DialogOpen(BeforeOpenEventArgs args)
    {
        Dictionary<string, string> _parameters = new()
                                                 {
                                                     {"companyID", Model.CompanyID.ToString()}
                                                 };
        string _returnValue = await General.ExecuteRest<string>("Company/GetLocationList", _parameters,null, false);
        Location = General.DeserializeObject<List<LocationDrop>>(_returnValue);
        CompanyContactEditForm.EditContext?.Validate();
    }

    private void LocationChanged(ChangeEventArgs<int, LocationDrop> location)
    {
        LocationDrop _item = location.ItemData;
        Model.StreetName = _item.StreetAddress;
        Model.City = _item.City;
        Model.State = _item.State;
        Model.ZipCode = _item.Zip;
        Model.StateID = _item.StateID;
        StateHasChanged();
    }

    protected override void OnParametersSet()
    {
        // Memory optimization: Only create new Context if Model reference has changed
        // ReSharper disable once InvertIf
        if (_currentModel != Model)
        {
            _currentModel = Model;
            Context = new(Model);
            // Memory optimization: No event handler needed - EditContext handles validation automatically
        }
        
        base.OnParametersSet();
    }

    private async Task SaveCompanyContact(EditContext args)
    {
        VisibleSpinner = true;
        await Save.InvokeAsync(args);
        await Dialog.HideAsync();
        VisibleSpinner = false;
    }

    internal async Task ShowDialog() => await Dialog.ShowAsync();
}