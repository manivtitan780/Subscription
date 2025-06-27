#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.Model
// File Name:           ReturnDashboard.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu, Brijesh Dubey
// Created On:          06-18-2025 21:06
// Last Updated On:     06-18-2025 21:07
// *****************************************/

#endregion

namespace Subscription.Model.Return;

public record struct ReturnDashboard(
    string Users,
    string TotalRequisitions,
    string ActiveRequisitions,
    string CandidatesInInterview,
    string OffersExtended,
    string CandidatesHired,
    string HireToOfferRatio,
    string RecentActivity,
    string Placements);