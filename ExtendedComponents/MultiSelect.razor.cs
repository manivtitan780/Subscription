#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             ExtendedComponents
// File Name:           MultiSelect.razor.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu, Brijesh Dubey
// Created On:          07-11-2025 19:07
// Last Updated On:     07-11-2025 21:11
// *****************************************/

#endregion

namespace ExtendedComponents;

public partial class MultiSelect<TItem, TValue> : ComponentBase
{
    // Memory optimization: Cache EqualityComparer to avoid repeated instantiation
    private static readonly EqualityComparer<TValue> ValueComparer = EqualityComparer<TValue>.Default;
    
    private TValue? _value;

    [Parameter]
    public bool AllowFilter { get; set; } = true;

    [Parameter]
    public bool Enabled { get; set; } = true;

    [Parameter]
    public string? ID { get; set; }

    [Parameter]
    public string? Key { get; set; }

    [Parameter]
    public IEnumerable<TItem>? Model { get; set; }

    [Parameter]
    public string? Placeholder { get; set; }

    [Parameter]
    public string? Text { get; set; }

    /// <summary>
    ///     Gets or sets the selected value in the MultiSelect.
    /// </summary>
    /// <value>
    ///     The selected value in the MultiSelect.
    /// </value>
    /// <remarks>
    ///     This property is used to bind a value to the MultiSelect.
    ///     The value corresponds to the selected item in the MultiSelect.
    ///     If the selected value changes, the ValueChanged event is invoked.
    /// </remarks>
    [Parameter]
    public TValue? Value
    {
        get => _value;
        set
        {
            // Memory optimization: Use cached comparer instead of creating new instance
            if (ValueComparer.Equals(value, _value))
            {
                return;
            }

            _value = value;
            ValueChanged.InvokeAsync(value);
        }
    }

    /// <summary>
    ///     Gets or sets the event callback that is invoked when the MultiSelect's selected value changes.
    /// </summary>
    /// <value>
    ///     The event callback for the MultiSelect value change event.
    /// </value>
    /// <remarks>
    ///     This event callback is invoked when the user selects a different item in the MultiSelect, causing the selected
    ///     value
    ///     to change.
    ///     The callback receives the new selected value as an argument.
    /// </remarks>
    [Parameter]
    public EventCallback<TValue> ValueChanged { get; set; }

    /// <summary>
    ///     Gets or sets the expression that identifies the value to be bound to the MultiSelect.
    /// </summary>
    /// <value>
    ///     An expression that identifies the value to be bound to the MultiSelect.
    /// </value>
    /// <remarks>
    ///     This property is used to bind a value to the MultiSelect. The expression is typically a lambda expression
    ///     that identifies the property of a model object that provides the value. The MultiSelect's selected item
    ///     will be used to update this value.
    /// </remarks>
    [Parameter]
    public Expression<Func<TValue>>? ValueExpression { get; set; }

    [Parameter]
    public string Width { get; set; } = "100%";

    // Memory optimization: Removed unnecessary OnInitialized override  
    // _value is already initialized to default(TValue?) by the field declaration
}