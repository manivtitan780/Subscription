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
    public List<string> DateRange { get; set; } = [];

    public List<int> TotalRequisitions { get; set; } = [];

    public List<int> ActiveRequisitions { get; set; } = [];

    public List<int> CandidatesInInterview { get; set; } = [];

    public List<int> OffersExtended { get; set; } = [];

    public List<int> CandidatesHired { get; set; } = [];

    public List<decimal> HireToOfferRatio { get; set; } = [];
    
    public List<RecentActivity> RecentActivities { get; set; } = [];
    
    public List<HiredPlacement> Placements { get; set; } = [];

    public List<string> User { get; set; } = [];
}