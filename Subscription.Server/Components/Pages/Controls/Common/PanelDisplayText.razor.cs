#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.Server
// File Name:           PanelDisplayText.razor.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu
// Created On:          04-22-2024 15:04
// Last Updated On:     10-29-2024 15:10
// *****************************************/

#endregion

#region Using

using System.Diagnostics.CodeAnalysis;

#endregion

namespace Subscription.Server.Components.Pages.Controls.Common;

public partial class PanelDisplayText
{
    private string _label;

    [Parameter]
    public PanelDisplay Display
    {
        get;
        set;
    } = PanelDisplay.Horizontal;

    [Parameter, SuppressMessage("Microsoft.Performance", "BL0007", Justification = "This is a computed property.")]
    public string Label
    {
        get => _label;
        set => _label = !value.EndsWith(":") ? $"{value}:" : value;
    }

    [Parameter]
    public MarkupString MarkupText
    {
        get;
        set;
    }

    [Parameter]
    public string Text
    {
        get;
        set;
    }
}

public enum PanelDisplay
{
    Horizontal = 1, Vertical = 2
}