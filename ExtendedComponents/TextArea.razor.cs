#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             ExtendedComponents
// File Name:           TextArea.razor.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu
// Created On:          12-13-2024 20:12
// Last Updated On:     12-13-2024 20:12
// *****************************************/

#endregion

namespace ExtendedComponents;

public partial class TextArea : ComponentBase
{
    private string _value;

    [Parameter]
    public string ID
    {
        get;
        set;
    }

    [Parameter]
    public int MaxLength
    {
        get;
        set;
    } = 1000;

    [Parameter]
    public string Placeholder
    {
        get;
        set;
    }

    [Parameter]
    public bool Readonly
    {
        get;
        set;
    } = false;

    [Parameter]
    public Resize ResizeMode
    {
        get;
        set;
    } = Resize.None;

    [Parameter]
    public int Rows
    {
        get;
        set;
    } = 5;

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
            if (EqualityComparer<string>.Default.Equals(value, _value))
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
    public EventCallback<string> ValueChanged
    {
        get;
        set;
    }

    /// <summary>
    ///     Gets or sets the expression that identifies the value to be used by the TextBox control.
    ///     This expression is used to bind the date control's value to a specified property.
    /// </summary>
    [Parameter]
    public Expression<Func<string>> ValueExpression
    {
        get;
        set;
    }

    [Parameter]
    public string Width
    {
        get;
        set;
    } = "100%";
}