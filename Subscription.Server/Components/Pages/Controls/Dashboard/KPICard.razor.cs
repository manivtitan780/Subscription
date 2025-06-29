#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.Server
// File Name:           KPICard.razor.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu, Brijesh Dubey
// Created On:          06-28-2025 15:06
// Last Updated On:     06-28-2025 15:35
// *****************************************/

#endregion

namespace Subscription.Server.Components.Pages.Controls.Dashboard;

public partial class KPICard : ComponentBase
{
    [Parameter]
    public string CssClass { get; set; } = "";

    [Parameter]
    public string IconClass { get; set; } = "e-icons e-chart";

    [Parameter]
    public string Label { get; set; } = "";

    [Parameter]
    public string Value { get; set; } = "0";
}