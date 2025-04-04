#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.Server
// File Name:           GridHeader.razor.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu
// Created On:          04-22-2024 15:04
// Last Updated On:     10-29-2024 15:10
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

    private SfAutoComplete<string, KeyValues> Acb
    {
        get;
        set;
    }

    /// <summary>
    ///     Gets or sets the event callback that is triggered when the "All" button in the grid footer is clicked.
    ///     This event callback is of type <see cref="EventCallback{MouseEventArgs}" />, which means it will provide event data
    ///     about the mouse event.
    /// </summary>
    [Parameter]
    public EventCallback<MouseEventArgs> AllAlphabet
    {
        get;
        set;
    }

    /// <summary>
    ///     Gets or sets the event callback that is triggered when an alphabet button in the grid footer is clicked.
    ///     This event callback is of type <see cref="EventCallback{Char}" />, which means it will provide the clicked alphabet
    ///     character as event data.
    /// </summary>
    [Parameter]
    public EventCallback<char> AlphabetMethod
    {
        get;
        set;
    }

    private IEnumerable<string> Alphabets
    {
        get;
        set;
    } = Enumerable.Range('A', 26).Select(x => ((char)x).ToString());

    /// <summary>
    ///     Gets or sets the event callback that is triggered when the "Clear" button in the grid footer is clicked.
    ///     This event callback is of type <see cref="EventCallback{MouseEventArgs}" />, which means it will provide event data
    ///     about the mouse event.
    /// </summary>
    [Parameter]
    public EventCallback<MouseEventArgs> ClearFilter
    {
        get;
        set;
    }

    /// <summary>
    ///     Gets or sets the event callback that is triggered when the AutoComplete component is created.
    /// </summary>
    /// <remarks>
    ///     The Created property is an event callback that gets triggered when the AutoComplete component is created. The
    ///     Created event callback can be used to perform any custom actions or initializations when the AutoComplete component
    ///     is created.
    /// </remarks>
    [Parameter]
    public EventCallback<object> Created
    {
        get;
        set;
    }

    [Parameter]
    public int CurrentPage
    {
        get;
        set;
    } = 1;

    [Parameter]
    public string Endpoint
    {
        get;
        set;
    } = "";

    [Parameter]
    public RenderFragment GridContent
    {
        get;
        set;
    }

    [Parameter]
    public EventCallback<PagerItemClickEventArgs> ItemClick
    {
        get;
        set;
    }

    [Parameter]
    public string Name
    {
        get;
        set;
    } = "";

    [Parameter]
    public int NumericCount
    {
        get;
        set;
    } = 7;

    private SfPager Pager
    {
        get;
        set;
    }

    [Parameter]
    public int PageSize
    {
        get;
        set;
    } = 25;

    [Parameter]
    public EventCallback<PageSizeChangedArgs> PageSizeChanged
    {
        get;
        set;
    }

    [Parameter]
    public List<int> PageSizes
    {
        get;
        set;
    } = [5, 10, 15, 20, 25, 50, 75, 100];

    [Parameter]
    public CompanySearch SearchModel
    {
        get;
        set;
    } = new();

    [Parameter]
    public bool ShowAlphabet
    {
        get;
        set;
    } = true;

    [Parameter]
    public int TotalCount
    {
        get;
        set;
    } = 100;

    /// <summary>
    ///     Gets or sets the event callback that is triggered when the value of the AutoCompleteButton changes.
    /// </summary>
    /// <value>
    ///     An event callback that includes the ChangeEventArgs with the new value and the associated KeyValues.
    /// </value>
    /// <remarks>
    ///     This property is used to handle the change event of the AutoCompleteButton.
    ///     It provides the new value and the associated KeyValues to the event handler.
    /// </remarks>
    [Parameter]
    public EventCallback<ChangeEventArgs<string, KeyValues>> ValueChange
    {
        get;
        set;
    }

    [Parameter]
    public EventCallback<MouseEventArgs> AddMethod
    {
        get;
        set;
    }

    [Parameter]
    public EventCallback<MouseEventArgs> RefreshGrid
    {
        get;
        set;
    }

    protected override void OnParametersSet()
    {
        _endpoint = Endpoint;
        base.OnParametersSet();
    }

    /// <summary>
    ///     The DropDownAdaptor class is a specialized DataAdaptor used for handling data operations for a dropdown control.
    ///     It overrides the ReadAsync method to provide custom data retrieval logic, specifically for autocomplete
    ///     functionality.
    /// </summary>
    public class DropDownAdaptor : DataAdaptor
    {
        /// <summary>
        ///     Asynchronously retrieves data for a dropdown control using autocomplete functionality.
        /// </summary>
        /// <param name="dm">The DataManagerRequest object containing additional request parameters.</param>
        /// <param name="key">An optional key to further specify the data retrieval.</param>
        /// <returns>
        ///     A task that represents the asynchronous operation. The task result contains the data retrieved for the dropdown
        ///     control.
        /// </returns>
        /// <remarks>
        ///     This method uses the 'General.GetAutocompleteAsync' method to retrieve the data, passing in the method name and
        ///     parameter name stored in the '_method' and '_parameterName' fields respectively, along with the DataManagerRequest
        ///     object.
        /// </remarks>
        public override Task<object> ReadAsync(DataManagerRequest dm, string key = null) => General.GetAutocompleteAsync(_endpoint, dm);
    }
}