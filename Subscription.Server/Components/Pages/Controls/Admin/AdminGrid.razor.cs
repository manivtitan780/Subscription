#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.Server
// File Name:           AdminGrid.razor.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu, Brijesh Dubey
// Created On:          03-10-2025 14:03
// Last Updated On:     03-11-2025 20:03
// *****************************************/

#endregion

// ReSharper disable MemberCanBePrivate.Global

namespace Subscription.Server.Components.Pages.Controls.Admin;

public partial class AdminGrid
{
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
    ///     Gets or sets the event callback for the add method.
    /// </summary>
    /// <value>
    ///     The event callback for the add method.
    /// </value>
    /// <remarks>
    ///     This property is used to specify the event that is triggered when the add method is invoked.
    ///     The event handler is responsible for handling the logic associated with the addition of new data to the grid.
    /// </remarks>
    [Parameter]
    public EventCallback<MouseEventArgs> AddMethod
    {
        get;
        set;
    }

    /// <summary>
    ///     Gets or sets the method name for the autocomplete functionality.
    /// </summary>
    /// <value>
    ///     The name of the method to be used for autocomplete.
    /// </value>
    /// <remarks>
    ///     This property is used to specify the method that provides autocomplete suggestions in the admin grid.
    ///     The method should return a list of suggestions based on the input.
    /// </remarks>
    [Parameter]
    public string AutocompleteMethod
    {
        get;
        set;
    }

    /// <summary>
    ///     Gets or sets the parameter for the autocomplete method.
    /// </summary>
    /// <value>
    ///     The parameter for the autocomplete method.
    /// </value>
    /// <remarks>
    ///     This property is used to specify the parameter that the autocomplete method requires.
    ///     The parameter is used by the autocomplete method to filter and return the appropriate suggestions.
    /// </remarks>
    [Parameter]
    public string AutocompleteParameter
    {
        get;
        set;
    }

    /// <summary>
    ///     Gets or sets the child content of the AdminGrid component.
    /// </summary>
    /// <value>
    ///     The child content of the AdminGrid component.
    /// </value>
    /// <remarks>
    ///     This property is used to specify the content that should be rendered inside the grid columns of the AdminGrid
    ///     component.
    ///     The content is specified as a RenderFragment, which represents a segment of UI content.
    /// </remarks>
    [Parameter]
    public RenderFragment ChildContent
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

    /// <summary>
    ///     Gets or sets a value indicating whether the grid's virtualization feature is enabled.
    /// </summary>
    /// <value>
    ///     true if virtualization is enabled; otherwise, false. The default is false.
    /// </value>
    /// <remarks>
    ///     When virtualization is enabled, the grid optimizes its rendering performance by only creating
    ///     and rendering the visible rows in the viewport, which is particularly useful for large data sets.
    /// </remarks>
    [Parameter]
    public bool EnableVirtualization
    {
        get;
        set;
    }

    /// <summary>
    ///     Gets or sets the entity name for the grid.
    /// </summary>
    /// <value>
    ///     The name of the entity.
    /// </value>
    /// <remarks>
    ///     This property is used to specify the entity that the grid is managing.
    ///     It is used in various operations and displays within the grid component.
    /// </remarks>
    [Parameter]
    public string Entity
    {
        get;
        set;
    }

    [Parameter]
    public RenderFragment GridContent
    {
        get;
        set;
    }

    /// <summary>
    ///     Gets or sets the plural form of the header content for the admin grid.
    /// </summary>
    /// <value>
    ///     The plural form of the header content.
    /// </value>
    /// <remarks>
    ///     This property is used to specify the plural form of the header content displayed in the admin grid.
    ///     It is typically used when there is more than one item of the same type in the grid.
    /// </remarks>
    [Parameter]
    public string HeaderContentPlural
    {
        get;
        set;
    }

    /// <summary>
    ///     Gets or sets the singular form of the header content for the admin grid.
    /// </summary>
    /// <value>
    ///     The singular form of the header content.
    /// </value>
    /// <remarks>
    ///     This property is used to specify the singular form of the header content that is displayed in the admin grid.
    ///     For example, if the grid is displaying a list of users, the singular form could be "User".
    /// </remarks>
    [Parameter]
    public string HeaderContentSingular
    {
        get;
        set;
    }

    /// <summary>
    ///     Gets or sets the height of the AdminGrid component.
    /// </summary>
    /// <value>
    ///     The height of the AdminGrid component.
    /// </value>
    /// <remarks>
    ///     This property is used to specify the height of the AdminGrid component.
    ///     The height is specified as a string and can be set to any valid CSS height value, default is 140px.
    /// </remarks>
    [Parameter]
    public string Height
    {
        get;
        set;
    } = "140px";

    [Parameter]
    public string Name
    {
        get;
        set;
    }

    /// <summary>
    ///     Gets or sets the page identifier for the admin grid.
    /// </summary>
    /// <value>
    ///     The page identifier.
    /// </value>
    /// <remarks>
    ///     This property is used to specify the identifier of the page that the admin grid is displayed on.
    ///     It is used in the header of the admin grid for navigation purposes.
    /// </remarks>
    [Parameter]
    public string Page
    {
        get;
        set;
    }

    [Parameter]
    public Query Query
    {
        get;
        set;
    } = new();

    /// <summary>
    ///     Gets or sets the event callback for the refresh grid action.
    /// </summary>
    /// <value>
    ///     The event callback for the refresh grid action.
    /// </value>
    /// <remarks>
    ///     This property is used to specify the event that is triggered when the refresh grid action is invoked.
    ///     The event handler is responsible for handling the logic associated with refreshing the data in the grid.
    /// </remarks>
    [Parameter]
    public EventCallback<MouseEventArgs> RefreshGrid
    {
        get;
        set;
    }

    /// <summary>
    ///     Gets or sets the Role ID associated with the admin grid.
    /// </summary>
    /// <value>
    ///     The Role ID as a string.
    /// </value>
    /// <remarks>
    ///     This property is used to specify the Role ID for the admin grid. It is used in the Header component and can be used
    ///     to control access or functionality based on the role of the user.
    /// </remarks>
    [Parameter]
    public string RoleID
    {
        get;
        set;
    }

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

    /// <summary>
    ///     Gets or sets the width of the AdminGrid component.
    /// </summary>
    /// <value>
    ///     The width of the AdminGrid component.
    /// </value>
    /// <remarks>
    ///     This property is used to specify the width of the AdminGrid component.
    ///     The width is specified as a string and can be expressed in any valid CSS unit, default value is 346px.
    /// </remarks>
    [Parameter]
    public string Width
    {
        get;
        set;
    } = "346px";

    protected override void OnAfterRender(bool firstRender)
    {
        if (firstRender)
        {
            Query ??= new();
            Query.AddParams("MethodName", AutocompleteMethod);
            Query.AddParams("ParameterName", AutocompleteParameter);
        }
        base.OnAfterRender(firstRender);
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
        public override async Task<object> ReadAsync(DataManagerRequest dm, string key = null)
        {
            object _dataSource = await General.GetAutocompleteAsync("Admin/GetSearch", dm, dm.Params["MethodName"].ToString(), dm.Params["ParameterName"].ToString());
            return _dataSource;
        }
    }
}