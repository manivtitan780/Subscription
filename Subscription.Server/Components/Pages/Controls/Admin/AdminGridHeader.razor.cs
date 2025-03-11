#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Profsvc_AppTrack
// Project:             Profsvc_AppTrack.Client
// File Name:           AdminGridHeader.razor.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu
// Created On:          1-5-2024 16:13
// Last Updated On:     1-27-2024 16:11
// *****************************************/

#endregion

#region Using

//using Profsvc_AppTrack.Client.Code;

#endregion

namespace Subscription.Server.Components.Pages.Controls.Admin;

/// <summary>
///     Represents a reusable grid header component for administrative pages.
///     This component provides various parameters for customization such as `Add`, `AutocompleteMethod`,
///     `AutocompleteParameterName`, `Entity`, `HeaderContentPlural`, `HeaderContentSingular`, `RefreshGrid`,
///     `ShowNewButton`, and `ValueChange`.
/// </summary>
public partial class AdminGridHeader
{
    private static string _method, _parameterName;

    /// <summary>
    ///     Gets or sets the event callback that is triggered when the 'Add' button is clicked.
    ///     This event callback is of type <see cref="EventCallback{MouseEventArgs}" />, which means it will provide
    ///     information about the mouse event that triggered the callback.
    /// </summary>
    [Parameter]
    public EventCallback<MouseEventArgs> Add
    {
        get;
        set;
    }

    /// <summary>
    ///     Gets or sets the AutoCompleteButton control used in the grid header.
    ///     This control provides an interface for input with autocomplete suggestions, enhancing the user experience when
    ///     interacting with the grid.
    /// </summary>
    /*private AutoCompleteButton AutoCompleteControl
    {
        get;
        set;
    }*/

    [Parameter]
    public string AutocompleteID
    {
        get;
        set;
    }

    /// <summary>
    ///     Gets or sets the method name to be used for autocomplete functionality in the grid header.
    ///     This method should exist in the parent component or page and be responsible for providing autocomplete suggestions.
    /// </summary>
    [Parameter]
    public string AutocompleteMethod
    {
        get;
        set;
    }

    /// <summary>
    ///     Gets or sets the parameter name to be used in conjunction with the autocomplete method in the grid header.
    ///     This parameter should be recognized by the autocomplete method in the parent component or page to provide relevant
    ///     autocomplete suggestions.
    /// </summary>
    [Parameter]
    public string AutocompleteParameterName
    {
        get;
        set;
    }

    /// <summary>
    ///     Gets or sets the entity name to be used for the autocomplete functionality in the grid header.
    ///     This entity name should correspond to the data entity that the grid is displaying or manipulating.
    /// </summary>
    [Parameter]
    public string Entity
    {
        get;
        set;
    }

    /// <summary>
    ///     Gets or sets the plural form of the content header. This is displayed as the main header on the administrative
    ///     grid.
    /// </summary>
    [Parameter]
    public string HeaderContentPlural
    {
        get;
        set;
    }

    /// <summary>
    ///     Gets or sets the singular form of the content header. This is used in the 'Add' button label and the placeholder
    ///     text for the autocomplete functionality in the administrative grid.
    /// </summary>
    [Parameter]
    public string HeaderContentSingular
    {
        get;
        set;
    }

    /// <summary>
    ///     Gets or sets the event callback that is triggered when the 'Refresh' button is clicked.
    ///     This event callback is of type <see cref="EventCallback{MouseEventArgs}" />, which means it will provide
    ///     information about the mouse event that triggered the callback.
    /// </summary>
    [Parameter]
    public EventCallback<MouseEventArgs> RefreshGrid
    {
        get;
        set;
    }

    /// <summary>
    ///     Gets or sets the display status of the 'New' button in the grid header.
    ///     The value of this property should be a valid CSS display value. For example, "block" to show the button, "none" to
    ///     hide it.
    ///     The default value is "unset".
    /// </summary>
    [Parameter]
    public string ShowNewButton
    {
        get;
        set;
    } = "unset";

    /// <summary>
    ///     Gets or sets the event callback that is triggered when the value of the autocomplete field changes.
    ///     This event callback is of type <see cref="EventCallback{ChangeEventArgs}" />, which means it will provide
    ///     information about the change event, including the new value and the associated key-value pairs.
    /// </summary>
    [Parameter]
    public EventCallback<ChangeEventArgs<string, KeyValues>> ValueChange
    {
        get;
        set;
    }

    /// <summary>
    ///     Asynchronously handles the post-rendering logic of the component.
    /// </summary>
    /// <param name="firstRender">
    ///     A boolean value that indicates whether this is the first time the component is being rendered.
    ///     If true, this is the first rendering of the component; if false, the component has been rendered before.
    /// </param>
    /// <returns>
    ///     A <see cref="Task" /> that represents the asynchronous operation.
    /// </returns>
    /// <remarks>
    ///     This method is invoked after the component has been rendered. It sets the `_method` and `_parameterName` fields
    ///     with the values of `AutocompleteMethod` and `AutocompleteParameterName` properties respectively.
    /// </remarks>
    protected override Task OnAfterRenderAsync(bool firstRender)
    {
        _method = AutocompleteMethod;
        _parameterName = AutocompleteParameterName;
        return base.OnAfterRenderAsync(firstRender);
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
        public override Task<object> ReadAsync(DataManagerRequest dm, string key = null) => General.GetAutocompleteAsync(_method, dm);
    }
}