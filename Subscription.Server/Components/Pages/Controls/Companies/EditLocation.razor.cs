#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.Server
// File Name:           EditLocation.razor.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu
// Created On:          04-22-2024 15:04
// Last Updated On:     04-26-2024 14:04
// *****************************************/

#endregion

namespace Subscription.Server.Components.Pages.Controls.Companies;

public partial class EditLocation
{
    [Parameter]
    public EventCallback<MouseEventArgs> Cancel { get; set; }

    private SfDataForm CompanyLocationEditForm { get; set; }

    private EditContext Context { get; set; }

    private SfDialog Dialog { get; set; }

    [Parameter]
    public CompanyLocations Model { get; set; }

    [Parameter]
    public EventCallback<EditContext> Save { get; set; }

    [Parameter]
    public List<IntValues> State { get; set; } = [];

    private bool VisibleSpinner { get; set; }

    private async Task CancelForm(MouseEventArgs args)
    {
        VisibleSpinner = true;
        await Cancel.InvokeAsync(args);
        await Dialog.HideAsync();
        VisibleSpinner = false;
    }

    private void DialogOpen(BeforeOpenEventArgs args)
    {
        CompanyLocationEditForm.EditContext.Validate();
        StateHasChanged();
    }

    protected override void OnParametersSet()
    {
        Context = new(Model);
        base.OnParametersSet();
    }

    private async Task SaveCompanyLocation(EditContext args)
    {
        VisibleSpinner = true;
        await Save.InvokeAsync(args);
        await Dialog.HideAsync();
        VisibleSpinner = false;
    }

    internal async Task ShowDialog() => await Dialog.ShowAsync();
}