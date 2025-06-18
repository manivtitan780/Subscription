#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.Model
// File Name:           TimeMetric.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu, Brijesh Dubey
// Created On:          06-18-2025 20:06
// Last Updated On:     06-18-2025 20:40
// *****************************************/

#endregion

namespace Subscription.Model;

public class TimeMetric
{
    public string DateRange { get; set; } = "";

    public int TotalRequisitions { get; set; }

    public int ActiveRequisitions { get; set; }

    public int CandidatesInInterview { get; set; }

    public int OffersExtended { get; set; }

    public int CandidatesHired { get; set; }

    public decimal HireToOfferRatio { get; set; }
}