#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Profsvc_AppTrack
// Project:             Profsvc_AppTrack.Client
// File Name:           AdminGridFooter.razor.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu
// Created On:          1-5-2024 16:13
// Last Updated On:     1-27-2024 16:11
// *****************************************/

#endregion

namespace Subscription.Server.Components.Pages.Controls.Admin;

/// <summary>
///     Represents the footer section of an administrative grid in the application.
///     This component displays the total count of items in the grid.
/// </summary>
public partial class AdminGridFooter
{
    /// <summary>
    ///     Gets or sets the total count of rows in the administrative grid.
    ///     This value is used to display the total number of items in the grid footer.
    /// </summary>
    [Parameter]
    public int Count
    {
        get;
        set;
    }
}