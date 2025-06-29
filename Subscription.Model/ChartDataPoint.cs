#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.Model
// File Name:           ChartDataPoint.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu, Brijesh Dubey
// Created On:          06-28-2025 15:06
// Last Updated On:     06-28-2025 15:58
// *****************************************/

#endregion

namespace Subscription.Model;

public class ChartDataPoint
{
    public int ActiveRequisitions { get; set; }

    public int CandidatesHired { get; set; }

    public int CandidatesInInterview { get; set; }

    public int OffersExtended { get; set; }

    public int TotalRequisitions { get; set; }

    public string User { get; set; }
}