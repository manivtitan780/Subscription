#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             ExtendedComponents
// File Name:           TextBox.razor.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu, Brijesh Dubey
// Created On:          02-06-2025 16:02
// Last Updated On:     05-13-2025 20:49
// *****************************************/

#endregion

namespace ExtendedComponents;

public partial class TextBox : ComponentBase
{
    // Memory optimization: Cache EqualityComparer to avoid repeated instantiation
    private static readonly EqualityComparer<string> StringComparer = EqualityComparer<string>.Default;
    
    private string _value = "";

    private SfTextBox? Box { get; set; }

    /// <summary>
    ///     Gets or sets the EditContext for the TextBoxControl component.
    ///     EditContext provides a context for tracking the validation state of a form and its inputs.
    ///     This context is used to interact with the form and its fields programmatically.
    /// </summary>
    [CascadingParameter, SuppressMessage("ReSharper", "PreferConcreteValueOverDefault")]
    public EditContext EditContext { get; set; } = default!;

    [Parameter]
    public bool Enabled { get; set; } = true;

    [Parameter]
    public string? ID { get; set; }

    [Parameter]
    public int MaxLength { get; set; } = 50;

    [Parameter]
    public string? Placeholder { get; set; }

    [Parameter]
    public bool Readonly { get; set; }

    [Parameter]
    public InputType TextBoxType { get; set; } = InputType.Text;

    [Parameter]
    public bool ValidateOnInput { get; set; }

    /// <summary>
    ///     Gets or sets the selected date in the TextBoxControl component.
    ///     This property is bound to the Value property of the SfTextBox component in the Razor view.
    ///     When the value is set, it checks for equality with the current value. If the new value is different, it updates the
    ///     current value and invokes the ValueChanged event.
    /// </summary>
    [Parameter]
    public string Value
    {
        get => _value;
        set
        {
            // Memory optimization: Use cached comparer instead of creating new instance
            if (StringComparer.Equals(value, _value))
            {
                return;
            }

            _value = value;
            ValueChanged.InvokeAsync(value);
        }
    }

    /// <summary>
    ///     Gets or sets the ValueChanged event. This event is triggered when the TextBox value in the control is changed by
    ///     the
    ///     user.
    ///     The event provides the new TextBox value selected by the user.
    /// </summary>
    [Parameter]
    public EventCallback<string> ValueChanged { get; set; }

    /// <summary>
    ///     Gets or sets the expression that identifies the value to be used by the TextBox control.
    ///     This expression is used to bind the date control's value to a specified property.
    /// </summary>
    [Parameter]
    public Expression<Func<string>>? ValueExpression { get; set; }

    [Parameter]
    public string Width { get; set; } = "100%";
}