#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             ExtendedComponents
// File Name:           NumericTextBox.razor.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu
// Created On:          12-13-2024 21:12
// Last Updated On:     01-13-2025 16:01
// *****************************************/

#endregion

namespace ExtendedComponents;

public partial class NumericTextBox<TValue> : ComponentBase
{
    private TValue? _value;

    [Parameter]
    public string Currency
    {
        get;
        set;
    } = "USD";

    [Parameter]
    public int? Decimals
    {
        get;
        set;
    } = 2;

    [Parameter]
    public string Format
    {
        get;
        set;
    } = "c2";

    [Parameter]
    public string? ID
    {
        get;
        set;
    }

    [Parameter]
    public TValue? Max
    {
        get;
        set;
    }

    [Parameter]
    public TValue? Min
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
    public bool ShowSpinButton
    {
        get;
        set;
    }

    /// <summary>
    ///     Gets or sets the selected value in the NumericTextBox.
    /// </summary>
    /// <value>
    ///     The selected value in the NumericTextBox.
    /// </value>
    /// <remarks>
    ///     This property is used to bind a value to the NumericTextBox.
    ///     The value corresponds to the selected item in the NumericTextBox.
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
    ///     Gets or sets the event callback that is invoked when the NumericTextBox's selected value changes.
    /// </summary>
    /// <value>
    ///     The event callback for the NumericTextBox value change event.
    /// </value>
    /// <remarks>
    ///     This event callback is invoked when the user selects a different item in the NumericTextBox, causing the selected
    ///     value
    ///     to change.
    ///     The callback receives the new selected value as an argument.
    /// </remarks>
    [Parameter]
    public EventCallback<TValue?> ValueChanged
    {
        get;
        set;
    }

    /// <summary>
    ///     Gets or sets the expression that identifies the value to be bound to the NumericTextBox.
    /// </summary>
    /// <value>
    ///     An expression that identifies the value to be bound to the NumericTextBox.
    /// </value>
    /// <remarks>
    ///     This property is used to bind a value to the NumericTextBox. The expression is typically a lambda expression
    ///     that identifies the property of a model object that provides the value. The NumericTextBox's selected item
    ///     will be used to update this value.
    /// </remarks>
    [Parameter]
    public Expression<Func<TValue>>? ValueExpression
    {
        get;
        set;
    }

    protected override Task OnInitializedAsync()
    {
        _value = default;
        return base.OnInitializedAsync();
    }
}