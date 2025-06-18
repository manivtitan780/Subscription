#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.Model
// File Name:           HiredPlacement.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu, Brijesh Dubey
// Created On:          06-18-2025 20:06
// Last Updated On:     06-18-2025 20:41
// *****************************************/

#endregion

namespace Subscription.Model;

public class HiredPlacement
{
    public string Company { get; set; } = "";

    public string RequisitionNumber { get; set; } = "";

    public int NumPosition { get; set; }

    public string Title { get; set; } = "";

    public string CandidateName { get; set; } = "";

    public DateTime DateHired { get; set; }

    public decimal SalaryOffered { get; set; }

    public decimal PlacementFee { get; set; }

    public decimal CommissionPercent { get; set; }

    public decimal CommissionEarned { get; set; }
}