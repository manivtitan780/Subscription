#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.Model
// File Name:           ConsolidatedMetrics.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu, Brijesh Dubey
// Created On:          07-03-2025 20:07
// Last Updated On:     07-03-2025 20:21
// *****************************************/

#endregion

namespace Subscription.Model;

/// <summary>
///     Consolidated metrics model for the refactored dashboard data structure.
///     Replaces the previous separate DateCounts and RatioCounts models by pivoting on Period.
/// </summary>
public class ConsolidatedMetrics
{
    /// <summary>
    ///     Active requisitions count for the period.
    ///     Note: For Recruiters, this field is named "ActiveRequisitionsUpdated"
    /// </summary>
    public int ActiveRequisitions { get; set; }

    /// <summary>
    ///     Hired submissions count for the period
    /// </summary>
    public int HIRSubmissions { get; set; }

    /// <summary>
    ///     Interview submissions count for the period
    /// </summary>
    public int INTSubmissions { get; set; }

    /// <summary>
    ///     Offer to hire ratio as a percentage (e.g., 15.50 for 15.5%)
    /// </summary>
    public decimal OEXHIRRatio { get; set; }

    /// <summary>
    ///     Offer extended submissions count for the period
    /// </summary>
    public int OEXSubmissions { get; set; }

    /// <summary>
    ///     Time period identifier ("7D", "MTD", "QTD", "HYTD", "YTD")
    /// </summary>
    public string Period { get; set; }

    /// <summary>
    ///     Number of requisitions created in the period.
    ///     Only present for Account Managers, not for Recruiters.
    /// </summary>
    public int RequisitionsCreated { get; set; }

    /// <summary>
    ///     Total submissions count for the period
    /// </summary>
    public int TotalSubmissions { get; set; }

    /// <summary>
    ///     User identifier (e.g., "DAVE", "KEVIN", etc.)
    /// </summary>
    public string User { get; set; }
}