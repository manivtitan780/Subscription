#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.Model
// File Name:           RecruiterConsolidatedMetrics.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu, Brijesh Dubey
// Created On:          07-03-2025 20:07
// Last Updated On:     07-03-2025 20:25
// *****************************************/

#endregion

namespace Subscription.Model;

/// <summary>
///     Model for recruiter-specific consolidated metrics.
///     Inherits from ConsolidatedMetrics but uses ActiveRequisitionsUpdated instead of ActiveRequisitions.
/// </summary>
public class RecruiterConsolidatedMetrics
{
    /// <summary>
    ///     Active requisitions updated count for the period (Recruiter-specific field)
    /// </summary>
    public int ActiveRequisitionsUpdated { get; set; }

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
    ///     Total submissions count for the period
    /// </summary>
    public int TotalSubmissions { get; set; }

    /// <summary>
    ///     User identifier (e.g., "KEVIN")
    /// </summary>
    public string User { get; set; }
}