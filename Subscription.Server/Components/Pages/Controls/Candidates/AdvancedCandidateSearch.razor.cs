#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.Server
// File Name:           AdvancedCandidateSearch.razor.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu, Brijesh Dubey
// Created On:          04-14-2025 20:04
// Last Updated On:     04-17-2025 19:57
// *****************************************/

#endregion

#region Using

using FilteringEventArgs = Syncfusion.Blazor.DropDowns.FilteringEventArgs;

#endregion

namespace Subscription.Server.Components.Pages.Controls.Candidates;

public partial class AdvancedCandidateSearch : ComponentBase
{
    // Memory optimization: Static readonly arrays to prevent repeated allocations
    private static readonly IntValues[] ProximityValues = 
    [
        new() {Text = "1", KeyValue = 1}, new() {Text = "5", KeyValue = 5}, new() {Text = "10", KeyValue = 10}, new() {Text = "20", KeyValue = 20}, new() {Text = "25", KeyValue = 25},
        new() {Text = "30", KeyValue = 30}, new() {Text = "40", KeyValue = 40}, new() {Text = "50", KeyValue = 50}, new() {Text = "60", KeyValue = 60}, new() {Text = "70", KeyValue = 70},
        new() {Text = "80", KeyValue = 80}, new() {Text = "90", KeyValue = 90}, new() {Text = "100", KeyValue = 100}, new() {Text = "125", KeyValue = 125}, new() {Text = "150", KeyValue = 150},
        new() {Text = "175", KeyValue = 175}, new() {Text = "200", KeyValue = 200}, new() {Text = "250", KeyValue = 250}, new() {Text = "300", KeyValue = 300}, new() {Text = "400", KeyValue = 400},
        new() {Text = "500", KeyValue = 500}, new() {Text = "600", KeyValue = 600}, new() {Text = "700", KeyValue = 700}, new() {Text = "800", KeyValue = 800}, new() {Text = "900", KeyValue = 900},
        new() {Text = "1000", KeyValue = 1000}
    ];

    private static readonly IntValues[] ProximityUnits = 
    [
        new() {Text = "miles", KeyValue = 1}, 
        new() {Text = "kilometers", KeyValue = 2}
    ];

    private static readonly KeyValues[] SecurityClearanceOptions = 
    [
        new() {KeyValue = "", Text = "All"}, 
        new() {KeyValue = "0", Text = "No"}, 
        new() {KeyValue = "1", Text = "Yes"}
    ];

    private static readonly KeyValues[] RelocateOptions = 
    [
        new() {KeyValue = "", Text = "All"}, 
        new() {KeyValue = "0", Text = "No"}, 
        new() {KeyValue = "1", Text = "Yes"}
    ];

    [Parameter]
    public EventCallback<MouseEventArgs> Cancel { get; set; }

    private EditContext Context { get; set; }

    private SfDialog Dialog { get; set; }

    [Parameter]
    public IntValues[] Eligibility { get; set; } = [];

    [Parameter]
    public KeyValues[] JobOptions { get; set; } = [];

    [Parameter]
    public CandidateSearch Model { get; set; } = new();

    private List<IntValues> ProximityUnit { get; set; } = [];

    private List<IntValues> ProximityValue { get; set; } = [];

    private List<KeyValues> Relocate { get; set; } = [];

    [Parameter]
    public EventCallback<EditContext> Search { get; set; }

    private SfDataForm SearchForm { get; set; }

    private List<KeyValues> SecurityClearance { get; set; } = [];

    [Parameter]
    public StateCache[] StateDropDown { get; set; } = [];

    public SfSwitch<bool> SwitchIncludeAdmin { get; set; }

    private bool SwitchIncludeAdminDisabled { get; set; }

    private bool VisibleSpinner { get; set; }

    [Inject]
    private ZipCodeService ZipCodeService { get; set; }

    private async Task CancelSearchDialog(MouseEventArgs args)
    {
        VisibleSpinner = true;
        await Cancel.InvokeAsync(args);
        await Dialog.HideAsync();
        VisibleSpinner = false;
    }

    // Removed: Redundant Context_OnFieldChanged event handler - prevents memory leak risk

    private IEnumerable<KeyValues> DataSource { get; set; } = [];

    // Memory optimization: Cache all zip codes once instead of fetching on every keystroke
    private KeyValues[] _allZipCodes = [];

    private void Filtering(FilteringEventArgs value)
    {
        if (value != null && value.Text.Length > 1)
        {
            // Performance optimization: Filter locally from cached zip codes instead of Redis query on every keystroke
            if (_allZipCodes.Length == 0)
            {
                DataSource = [];
                return;
            }

            IEnumerable<KeyValues> _zipCodes = _allZipCodes.Where(zip => zip.KeyValue.StartsWith(value.Text, StringComparison.OrdinalIgnoreCase));
            DataSource = _zipCodes;
        }
    }

    protected override async Task OnInitializedAsync()
    {
        await Task.Yield();
        
        // Memory optimization: Use static readonly arrays instead of creating new objects
        ProximityValue = ProximityValues.ToList();
        ProximityUnit = ProximityUnits.ToList();
        SecurityClearance = SecurityClearanceOptions.ToList();
        Relocate = RelocateOptions.ToList();

        // Performance optimization: Load all zip codes once from Redis instead of on every keystroke
        _allZipCodes = await ZipCodeService.GetZipCodes() ?? [];

        SwitchIncludeAdminDisabled = Model.AllCandidates;
        Model.IncludeAdmin = true;
    }

    protected override void OnParametersSet()
    {
        // Memory optimization: Only create new EditContext if Model has changed
        if (Context?.Model != Model)
        {
            Context = null;  // Immediate reference cleanup for GC
            Context = new(Model);
        }
        base.OnParametersSet();
    }

    private void OpenDialog() => Context.Validate();

    private async Task SearchCandidateDialog(EditContext editContext)
    {
        VisibleSpinner = true;
        await Search.InvokeAsync(Context);
        await Dialog.HideAsync();
        VisibleSpinner = false;
    }

    internal async Task ShowDialog() => await Dialog.ShowAsync();

    private void ValueChangedShowCandidates(ChangeArgs<bool> candidate)
    {
        SwitchIncludeAdminDisabled = candidate.Value;
        Model.IncludeAdmin = true;
    }
}