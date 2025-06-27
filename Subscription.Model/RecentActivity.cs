#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.Model
// File Name:           RecentActivity.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu, Brijesh Dubey
// Created On:          06-18-2025 20:06
// Last Updated On:     06-25-2025 19:59
// *****************************************/

#endregion

using JetBrains.Annotations;

namespace Subscription.Model;

public class RecentActivity
{
    public string Company { get; set; }

    public string Requisition { get; set; }

    public int NumPosition { get; set; }

    public string Title { get; set; }

    public string CandidateName { get; set; }

    public string CurrentStatus { get; set; }

    public DateTime DateFirstSubmitted { get; set; }

    public DateTime LastActivityDate { get; set; }

    public string User { get; set; }
}