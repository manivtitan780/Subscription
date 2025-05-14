#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.Server
// File Name:           AdvancedRequisitionSearch.razor.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu, Brijesh Dubey
// Created On:          04-12-2025 20:04
// Last Updated On:     04-14-2025 19:04
// *****************************************/

#endregion

#region Using

using Syncfusion.Blazor.Calendars;

#endregion

namespace Subscription.Server.Components.Pages.Controls.Requisitions;

public partial class AdvancedRequisitionSearch : ComponentBase
{
    [Parameter]
    public EventCallback<MouseEventArgs> Cancel { get; set; }

    [Parameter]
    public List<Company> Companies { get; set; }

    private EditContext Context { get; set; }

    private DateTime CreatedEndMax { get; set; }

    private DateTime CreatedEndMin { get; set; }

    private SfDatePicker<DateTime> CreatedMax { get; set; }

    private SfDialog Dialog { get; set; }

    private DateTime DueEndMax { get; set; }

    private DateTime DueEndMin { get; set; }

    private SfDatePicker<DateTime> DueMax { get; set; }

    [Parameter]
    public List<JobOptions> JobOption { get; set; }

    [Parameter]
    public RequisitionSearch Model { get; set; } = new();

    [Parameter]
    public EventCallback<EditContext> Search { get; set; }

    private SfDataForm SearchForm { get; set; }

    private List<KeyValues> ShowRequisitions { get; } = [];

    [Parameter]
    //public List<KeyValues> StatusDropDown { get; set; }
    public List<string> StatusDropDown { get; set; }

    private bool VisibleSpinner { get; set; }

    private async Task CancelSearchDialog(MouseEventArgs args)
    {
        VisibleSpinner = true;
        await Cancel.InvokeAsync(args);
        await Dialog.HideAsync();
        VisibleSpinner = false;
    }

    private void Context_OnFieldChanged(object sender, FieldChangedEventArgs e) => Context.Validate();

    private void CreatedOnSelect(ChangedEventArgs<DateTime> date)
    {
        DateTime _date = date.Value;
        CreatedEndMin = _date;
        CreatedEndMax = _date.AddMonths(36);
        Model.CreatedOnEnd = _date.AddMonths(36);
    }

    private void DueOnSelect(ChangedEventArgs<DateTime> date)
    {
        DateTime _date = date.Value;
        DueEndMin = _date;
        DueEndMax = _date.AddMonths(36);
        Model.DueEnd = _date.AddMonths(36);
    }

    protected override void OnParametersSet()
    {
        Context = new(Model);
        Context.OnFieldChanged += Context_OnFieldChanged;
        base.OnParametersSet();
    }

    private void OpenDialog() => Context.Validate();
    
    private async Task SearchCandidateDialog(EditContext context)
    {
        VisibleSpinner = true;
        await Search.InvokeAsync(Context);
        await Dialog.HideAsync();
        VisibleSpinner = false;
    }

    public async Task ShowDialog() => await Dialog.ShowAsync();
}