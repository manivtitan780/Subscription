#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.Server
// File Name:           AdvancedRequisitionSearch.razor.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu, Brijesh Dubey
// Created On:          07-24-2025 19:07
// Last Updated On:     07-24-2025 19:59
// *****************************************/

#endregion

#region Using

using Syncfusion.Blazor.Calendars;

#endregion

namespace Subscription.Server.Components.Pages.Controls.Requisitions;

public partial class AdvancedRequisitionSearch : ComponentBase, IDisposable
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

    [Parameter]
    public List<string> StatusDropDown { get; set; }

    private bool VisibleSpinner { get; set; }

    /// <summary>
    ///     Memory optimization: Clean disposal pattern
    /// </summary>
    public void Dispose()
    {
        // No event handlers to dispose after optimization
        GC.SuppressFinalize(this);
    }

    private async Task CancelSearchDialog(MouseEventArgs args)
    {
        VisibleSpinner = true;
        await Cancel.InvokeAsync(args);
        await Dialog.HideAsync();
        VisibleSpinner = false;
    }

    // Removed: Unnecessary Context_OnFieldChanged event handler - validation handled by form validation

    private void CreatedOnSelect(ChangedEventArgs<DateTime> date)
    {
        DateTime _date = date.Value;
        // Performance optimization: Calculate AddMonths(36) only once
        DateTime _maxDate = _date.AddMonths(36);

        CreatedEndMin = _date;
        CreatedEndMax = _maxDate;
        Model.CreatedOnEnd = _maxDate;
    }

    private void DueOnSelect(ChangedEventArgs<DateTime> date)
    {
        DateTime _date = date.Value;
        // Performance optimization: Calculate AddMonths(36) only once
        DateTime _maxDate = _date.AddMonths(36);

        DueEndMin = _date;
        DueEndMax = _maxDate;
        Model.DueEnd = _maxDate;
    }

    protected override void OnParametersSet()
    {
        // Memory optimization: Only create new EditContext if Model reference changed
        if (Context?.Model != Model)
        {
            Context = new(Model);
        }

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