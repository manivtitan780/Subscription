#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.Server
// File Name:           RequisitionInfoPanel.razor.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu, Brijesh Dubey
// Created On:          07-23-2025 20:07
// Last Updated On:     07-23-2025 20:07
// *****************************************/

#endregion

namespace Subscription.Server.Components.Pages.Controls.Requisitions;

/// <summary>
///     The RequisitionInfoPanel component displays detailed information for a requisition,
///     including company details, job requirements, rates, and skills.
///     This component is optimized for memory efficiency and performance.
/// </summary>
public partial class RequisitionInfoPanel
{
    /// <summary>
    ///     Gets or sets the requisition details model containing all the information to display.
    /// </summary>
    [Parameter]
    public RequisitionDetails Model { get; set; } = new();

    /// <summary>
    ///     Gets or sets the skills markup to display in the skills section.
    ///     This is formatted HTML content showing required and optional skills.
    /// </summary>
    [Parameter]
    public MarkupString Skills { get; set; } = "".ToMarkupString();

    /// <summary>
    ///     Gets or sets the list of states for location formatting.
    ///     Used to convert StateID to state code in the location display.
    /// </summary>
    [Parameter]
    public List<StateCache> States { get; set; } = [];

    /// <summary>
    ///     Converts duration code abbreviation to full text for display.
    /// </summary>
    /// <param name="durationCode">The duration code (m, w, y, etc.)</param>
    /// <returns>Full duration text (months, weeks, years, etc.)</returns>
    /// <summary>
    /// Micro-optimization: Static dictionary for O(1) lookup, eliminates ToLower() allocation
    /// </summary>
    private static readonly Dictionary<string, string> DurationMappings = new(4, StringComparer.OrdinalIgnoreCase)
    {
        ["m"] = "months",
        ["w"] = "weeks",
        ["y"] = "years",
        ["d"] = "days"
    };

    private static string GetDurationCode(string durationCode) => 
        DurationMappings.TryGetValue(durationCode, out string mapped) ? mapped : durationCode;

    /// <summary>
    ///     Formats the project location from city, state, and zip code.
    ///     Memory optimization: Uses pre-allocated List to avoid multiple string concatenations.
    /// </summary>
    /// <returns>Formatted location string (City, State, ZipCode)</returns>
    private string GetLocation()
    {
        if (Model == null)
        {
            return "";
        }

        // Memory optimization: Use pre-allocated string array instead of List<string> to reduce overhead
        // Maximum possible parts: City, State, Zip (3 elements)
        string[] parts = new string[3];
        int partCount = 0;
        
        if (Model.City.NotNullOrWhiteSpace())
        {
            parts[partCount++] = Model.City;
        }

        if (Model.StateID.ToInt32() != 0)
        {
            StateCache state = States.FirstOrDefault(s => s.KeyValue == Model.StateID.ToInt32());
            if (state.Code.NotNullOrWhiteSpace())
            {
                parts[partCount++] = state.Code;
            }
        }

        if (Model.ZipCode.NotNullOrWhiteSpace())
        {
            parts[partCount++] = Model.ZipCode;
        }

        return string.Join(", ", parts.AsSpan(0, partCount));
    }
}