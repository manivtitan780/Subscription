#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.Server
// File Name:           AdvancedCandidateSearch.razor.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu, Brijesh Dubey
// Created On:          04-14-2025 20:04
// Last Updated On:     04-14-2025 20:04
// *****************************************/

#endregion

namespace Subscription.Server.Components.Pages.Controls.Candidates;

public partial class AdvancedCandidateSearch : ComponentBase
{
    [Parameter]
    public object AutoCompleteCityZip { get; set; }

    /*public AutoCompleteButton AutoCompleteControl { get; set; }*/

    [Parameter]
    public EventCallback<MouseEventArgs> Cancel { get; set; }

    private SfDialog Dialog { get; set; }

    private DialogFooter DialogFooter { get; set; }

    private EditForm EditSearchForm { get; set; }

    [Parameter]
    public List<IntValues> EligibilityDropDown { get; set; } = [];

    [Parameter]
    public List<KeyValues> JobOptionsDropDown { get; set; } = [];

    [Parameter]
    public CandidateSearch Model { get; set; } = new();

    private List<IntValues> ProximityUnit
    {
        get; /*set;*/
        set;
    } = [];

    private List<IntValues> ProximityValue
    {
        get; /*set;*/
        set;
    } = [];

    private List<KeyValues> RelocateDropDown
    {
        get;
        set;
    } = [];

    [Parameter]
    public EventCallback<EditContext> Save { get; set; }

    private List<KeyValues> SecurityClearanceDropDown
    {
        get;
        set;
    } = [];

    [Parameter]
    public List<IntValues> StateDropDown { get; set; } = [];

    private bool VisibleSpinner { get; set; }

    private EditContext Context { get; set; }

    private SfDataForm SearchForm { get; set; }

    private async Task CancelSearchDialog(MouseEventArgs args)
    {
        await Task.CompletedTask;
        //await General.CallCancelMethod(args, Spinner, DialogFooter, Dialog, Cancel);
    }

    protected override async Task OnInitializedAsync()
    {
        await Task.Yield();
        ProximityValue.Clear();
        ProximityValue =
        [
            new() {Text = "1", KeyValue = 1}, new() {Text = "5", KeyValue = 5}, new() {Text = "10", KeyValue = 10}, new() {Text = "20", KeyValue = 20}, new() {Text = "25", KeyValue = 25},
            new() {Text = "30", KeyValue = 30}, new() {Text = "40", KeyValue = 40}, new() {Text = "50", KeyValue = 50}, new() {Text = "60", KeyValue = 60}, new() {Text = "70", KeyValue = 70},
            new() {Text = "80", KeyValue = 80}, new() {Text = "90", KeyValue = 90}, new() {Text = "100", KeyValue = 100}, new() {Text = "125", KeyValue = 125}, new() {Text = "150", KeyValue = 150},
            new() {Text = "175", KeyValue = 175}, new() {Text = "200", KeyValue = 200}, new() {Text = "250", KeyValue = 250}, new() {Text = "300", KeyValue = 300}, new() {Text = "400", KeyValue = 400}, 
            new() {Text = "500", KeyValue = 500}, new() {Text = "600", KeyValue = 600}, new() {Text = "700", KeyValue = 700}, new() {Text = "800", KeyValue = 800}, new() {Text = "900", KeyValue = 900}, 
            new() {Text = "1000", KeyValue = 1000}
        ];

        ProximityUnit.Clear();
        ProximityUnit = [new() {Text = "miles", KeyValue = 1}, new() {Text = "kilometers", KeyValue = 2}];

        SecurityClearanceDropDown.Clear();
        SecurityClearanceDropDown = [new() {KeyValue = "%", Text = "All"}, new() {KeyValue = "0", Text = "No"}, new() {KeyValue = "1", Text = "Yes"}];

        RelocateDropDown.Clear();
        RelocateDropDown = [new() {KeyValue = "%", Text = "All"}, new() {KeyValue = "0", Text = "No"}, new() {KeyValue = "1", Text = "Yes"}];
    }

    private void OpenDialog() => EditSearchForm.EditContext?.Validate();

    private async Task SearchCandidateDialog(EditContext editContext)
    {
        await Task.CompletedTask;
        //await General.CallSaveMethod(editContext, Spinner, DialogFooter, Dialog, Save);
    }

    internal async Task ShowDialog() => await Dialog.ShowAsync();

    public class CandidateCityZipAdaptor : DataAdaptor
    {
        public override Task<object> ReadAsync(DataManagerRequest dm, string key = null) => General.GetAutocompleteAsync("GetCityZipList", dm, "@CityZip", "cityZip");
    }
}