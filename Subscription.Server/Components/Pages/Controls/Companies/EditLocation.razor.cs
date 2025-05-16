#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.Server
// File Name:           EditLocation.razor.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu, Brijesh Dubey
// Created On:          02-05-2025 20:02
// Last Updated On:     04-25-2025 19:26
// *****************************************/

#endregion

namespace Subscription.Server.Components.Pages.Controls.Companies;

public partial class EditLocation : IDisposable
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
    public List<StateCache> State { get; set; } = [];

    private bool VisibleSpinner { get; set; }

    private async Task CancelForm(MouseEventArgs args)
    {
        VisibleSpinner = true;
        await Cancel.InvokeAsync(args);
        await Dialog.HideAsync();
        VisibleSpinner = false;
    }

    private void Context_OnFieldChanged(object sender, FieldChangedEventArgs e)
    {
        Context.Validate();
    }

    private void DialogOpen(BeforeOpenEventArgs args)
    {
        CompanyLocationEditForm.EditContext.Validate();
        StateHasChanged();
    }

    public void Dispose()
    {
        if (Context is not null)
        {
            Context.OnFieldChanged -= Context_OnFieldChanged;
        }

        GC.SuppressFinalize(this);
    }

    protected override void OnParametersSet()
    {
        Context = new(Model);
        Context.OnFieldChanged += Context_OnFieldChanged;
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