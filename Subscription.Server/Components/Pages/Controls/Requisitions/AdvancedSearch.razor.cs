#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.Server
// File Name:           AdvancedSearch.razor.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu, Brijesh Dubey
// Created On:          04-12-2025 20:04
// Last Updated On:     04-12-2025 20:04
// *****************************************/

#endregion

#region Using

using Syncfusion.Blazor.Calendars;

#endregion

namespace Subscription.Server.Components.Pages.Controls.Requisitions;

public partial class AdvancedSearch : ComponentBase
{
    [Parameter]
    public object AutoCompleteCityZip { get; set; }

    [Parameter]
    public EventCallback<MouseEventArgs> Cancel { get; set; }

    [Parameter]
    public List<KeyValues> Companies { get; set; }

    private EditContext Context { get; set; }

    private DateTime CreatedEndMax { get; set; }

    private DateTime CreatedEndMin { get; set; }

    private SfDatePicker<DateTime> CreatedMax { get; set; }

    private SfDialog Dialog { get; set; }

    private DateTime DueEndMax { get; set; }

    private DateTime DueEndMin { get; set; }

    private SfDatePicker<DateTime> DueMax { get; set; }

    [Parameter]
    public List<IntValues> EligibilityDropDown { get; set; } = [];

    internal DialogFooter FooterDialog { get; set; }

    [Parameter]
    public List<KeyValues> JobOption { get; set; }

    [Parameter]
    public List<KeyValues> JobOptionsDropDown { get; set; } = [];

    [Parameter]
    public RequisitionSearch Model { get; set; } = new();

    [Parameter]
    public EventCallback<EditContext> Search { get; set; }

    private SfDataForm SearchForm { get; set; }

    private List<KeyValues> ShowRequisitions { get; } = [];

    private SfSpinner Spinner { get; set; }

    [Parameter]
    public List<IntValues> StateDropDown { get; set; } = [];

    [Parameter]
    public List<KeyValues> StatusDropDown { get; set; }

    private bool VisibleSpinner { get; set; }

    private async Task CancelSearchDialog(MouseEventArgs args)
    {
        await General.CallCancelMethod(args, Spinner, FooterDialog, Dialog, Cancel);
    }

    private void CreatedOnSelect(DateTime date)
    {
        CreatedEndMin = date;
        CreatedEndMax = date.AddMonths(6);
        Model.CreatedOnEnd = date.AddMonths(6);
    }

    private void DueOnSelect(DateTime date)
    {
        DueEndMin = date;
        DueEndMax = date.AddMonths(6);
        Model.DueEnd = date.AddMonths(6);
    }

    private async Task OpenDialog()
    {
        await Task.Yield();
        Model.Clear();
        Model.Status = "New,Open,Partially Filled";
        ShowRequisitions.Clear();
        ShowRequisitions.Add(new() {Key = "%", Value = "All Requisitions"});
        ShowRequisitions.Add(new() {Key = "ADMIN", Value = "My Requisitions"});
    }

    private async Task SearchCandidateDialog(EditContext context)
    {
        await General.CallSaveMethod(context, Spinner, FooterDialog, Dialog, Search);
    }

    public async Task ShowDialog()
    {
        await Dialog.ShowAsync();
    }
}