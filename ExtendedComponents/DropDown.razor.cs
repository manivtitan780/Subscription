#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             ExtendedComponents
// File Name:           DropDown.razor.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu
// Created On:          12-13-2024 19:12
// Last Updated On:     12-13-2024 21:12
// *****************************************/

#endregion

using Syncfusion.Blazor.Data;

namespace ExtendedComponents;

public partial class DropDown<TValue, TItem> : ComponentBase
{
    private SfDropDownList<TValue, TItem>? _drop;
    private TValue? _value;

    [Parameter]
    public bool AllowFilter
    {
        get;
        set;
    }

    /// <summary>
    ///     Gets or sets the event callback that is invoked when the value of the DropDown changes.
    /// </summary>
    /// <value>
    ///     The event callback for the DropDown value change event.
    /// </value>
    /// <remarks>
    ///     This event callback is invoked when the user selects a different item in the DropDown.
    ///     The callback receives a ChangeEventArgs object that contains the old and new values.
    /// </remarks>
    [Parameter]
    public EventCallback<ChangeEventArgs<TValue, TItem>> DropDownValueChange
    {
        get;
        set;
    }

    [Parameter]
    public string? ID
    {
        get;
        set;
    }

    /// <summary>
    ///     Gets or sets the field in the data source that provides the values for the DropDown.
    /// </summary>
    /// <value>
    ///     The name of the field that provides the values for the DropDown.
    /// </value>
    /// <remarks>
    ///     This property is used to bind a field in the data source to the value of the DropDown items.
    ///     The value of this field is used as the value for each item in the DropDown.
    ///     When an item is selected in the DropDown, the value of this field is used as the selected value.
    /// </remarks>
    [Parameter]
    public string Key
    {
        get;
        set;
    } = "KeyValue";

    [Parameter]
    public IEnumerable<TItem>? Model
    {
        get;
        set;
    }

    [Parameter]
    public string? Placeholder
    {
        get;
        set;
    }

    [Parameter]
    public bool ShowClearButton
    {
        get;
        set;
    }

    /// <summary>
    ///     Gets or sets the name of the property to be used as the display text in the DropDown.
    /// </summary>
    /// <value>
    ///     The name of the property to be used as the display text.
    /// </value>
    /// <remarks>
    ///     This property is used to specify which property of the items in the DropDown's data source should be used as the
    ///     display text.
    ///     The display text is shown in the DropDown's input field and in the dropdown list.
    /// </remarks>
    [Parameter]
    public string Text
    {
        get;
        set;
    } = "Text";

    /// <summary>
    ///     Gets or sets the selected value in the DropDown.
    /// </summary>
    /// <value>
    ///     The selected value in the DropDown.
    /// </value>
    /// <remarks>
    ///     This property is used to bind a value to the DropDown.
    ///     The value corresponds to the selected item in the DropDown.
    ///     If the selected value changes, the ValueChanged event is invoked.
    /// </remarks>
    [Parameter]
    public TValue? Value
    {
        get => _value;
        set
        {
            if (EqualityComparer<TValue>.Default.Equals(value, _value))
            {
                return;
            }

            _value = value;
            ValueChanged.InvokeAsync(value);
        }
    }

    /// <summary>
    ///     Gets or sets the event callback that is invoked when the DropDown's selected value changes.
    /// </summary>
    /// <value>
    ///     The event callback for the DropDown value change event.
    /// </value>
    /// <remarks>
    ///     This event callback is invoked when the user selects a different item in the DropDown, causing the selected value
    ///     to change.
    ///     The callback receives the new selected value as an argument.
    /// </remarks>
    [Parameter]
    public EventCallback<TValue> ValueChanged
    {
        get;
        set;
    }

    /// <summary>
    ///     Gets or sets the expression that identifies the value to be bound to the DropDown.
    /// </summary>
    /// <value>
    ///     An expression that identifies the value to be bound to the DropDown.
    /// </value>
    /// <remarks>
    ///     This property is used to bind a value to the DropDown. The expression is typically a lambda expression
    ///     that identifies the property of a model object that provides the value. The DropDown's selected item
    ///     will be used to update this value.
    /// </remarks>
    [Parameter]
    public Expression<Func<TValue>>? ValueExpression
    {
        get;
        set;
    }

    /// <summary>
    ///     Gets or sets the width of the DropDown control.
    /// </summary>
    /// <value>
    ///     A string that represents the width of the DropDown control. The default value is "98%".
    /// </value>
    /// <remarks>
    ///     This property is used to set the width of the DropDown control. The width can be set in percentages or pixels.
    ///     For example, "50%" will make the DropDown control take up half of the available width, while "200px" will make the
    ///     DropDown control 200 pixels wide.
    ///     If this property is not set, the DropDown control will take up 98% of the available width by default.
    /// </remarks>
    [Parameter]
    public string Width
    {
        get;
        set;
    } = "98%";

    [Parameter]
    public Query Query
    {
        get;
        set;
    } = new();

    /// <summary>
    ///     Asynchronously refreshes the data in the DropDown control.
    /// </summary>
    /// <remarks>
    ///     This method is used to refresh the data in the DropDown control by re-fetching the data from the data source.
    ///     It is useful when the data source has been updated and the changes need to be reflected in the DropDown.
    /// </remarks>
    // ReSharper disable once UnusedMember.Global
    public Task? Refresh() => _drop?.RefreshDataAsync();

    protected override Task OnInitializedAsync()
    {
        return base.OnInitializedAsync();
    }
}