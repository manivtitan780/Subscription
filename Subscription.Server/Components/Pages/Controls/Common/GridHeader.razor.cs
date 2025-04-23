#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.Server
// File Name:           GridHeader.razor.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu, Brijesh Dubey
// Created On:          02-06-2025 19:02
// Last Updated On:     04-12-2025 16:04
// *****************************************/

#endregion

namespace Subscription.Server.Components.Pages.Controls.Common;

public partial class GridHeader
{
    private static string _endpoint = "";

    private Dictionary<string, object> _htmlAttribute = new()
                                                        {
                                                            {"maxlength", 30},
                                                            {"autocomplete", "off"}
                                                            //{"autocomplete", $"hoohoo_{new string(Enumerable.Range(0, 3).Select(_ => "abcdefghijklmnopqrstuvwxyz"[_random.Next(26)]).ToArray())}"}
                                                        };

    private SfAutoComplete<string, KeyValues> Acb { get; set; }

    [Parameter]
    public EventCallback<MouseEventArgs> AddMethod { get; set; }

    [Parameter]
    public EventCallback<MouseEventArgs> AdvancedSearch { get; set; }

    [Parameter]
    public EventCallback<MouseEventArgs> AllAlphabet { get; set; }

    [Parameter]
    public EventCallback<char> AlphabetMethod { get; set; }

    private IEnumerable<string> Alphabets { get; set; } = Enumerable.Range('A', 26).Select(x => ((char)x).ToString());

    [Parameter]
    public EventCallback<MouseEventArgs> ClearFilter { get; set; }

    [Parameter]
    public EventCallback<object> Created { get; set; }

    [Parameter]
    public int CurrentPage { get; set; } = 1;

    [Parameter]
    public string Endpoint { get; set; } = "";

    [Parameter]
    public RenderFragment GridContent { get; set; }

    [Parameter]
    public EventCallback<PagerItemClickEventArgs> ItemClick { get; set; }

    [Parameter]
    public string Name { get; set; } = "";

    [Parameter]
    public int NumericCount { get; set; } = 7;

    [Parameter]
    public EventCallback<PageChangedEventArgs> PageChanged { get; set; }

    private SfPager Pager { get; set; }

    [Parameter]
    public int PageSize { get; set; } = 25;

    [Parameter]
    public EventCallback<PageSizeChangedArgs> PageSizeChanged { get; set; }

    [Parameter]
    public List<int> PageSizes { get; set; } = [5, 10, 15, 20, 25, 50, 75, 100];

    [Parameter]
    public EventCallback<MouseEventArgs> RefreshGrid { get; set; }

    //[Parameter]
    //public CompanySearch SearchModel { get; set; } = new();

    [Parameter]
    public bool ShowAdd { get; set; } = true;

    [Parameter]
    public bool ShowAlphabet { get; set; } = true;

    [Parameter]
    public bool ShowSearch { get; set; } = true;

    [Parameter]
    public bool ShowSubmit { get; set; } = false;

    [Parameter]
    public EventCallback<MouseEventArgs> Submit { get; set; }

    [Parameter]
    public int TotalCount { get; set; } = 100;

    [Parameter]
    public EventCallback<ChangeEventArgs<string, KeyValues>> ValueChange { get; set; }

    protected override void OnParametersSet()
    {
        _endpoint = Endpoint;
        base.OnParametersSet();
    }

    public class DropDownAdaptor : DataAdaptor
    {
        public override Task<object> ReadAsync(DataManagerRequest dm, string key = null) => General.GetAutocompleteAsync(_endpoint, dm);
    }
}