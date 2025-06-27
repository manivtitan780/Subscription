#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.Model
// File Name:           HiredPlacement.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu, Brijesh Dubey
// Created On:          06-18-2025 20:06
// Last Updated On:     06-23-2025 15:19
// *****************************************/

#endregion

using JetBrains.Annotations;

namespace Subscription.Model;

[UsedImplicitly] 
public record struct HiredPlacement(
    string Company,
    string RequisitionNumber,
    int NumPosition,
    string Title,
    string CandidateName,
    DateTime DateHired,
    DateTime StartDate,
    DateTime DateInvoiced,
    DateTime DatePaid,
    decimal SalaryOffered,
    decimal PlacementFee,
    decimal CommissionPercent,
    decimal CommissionEarned,
    string User);
/*{
    public string Company { get; set; }

    public string RequisitionNumber { get; set; }

    public int NumPosition { get; set; }

    public string Title { get; set; }

    public string CandidateName { get; set; }

    public DateTime DateHired { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime DateInvoiced { get; set; }

    public DateTime DatePaid { get; set; }

    public decimal SalaryOffered { get; set; }

    public decimal PlacementFee { get; set; }

    public decimal CommissionPercent { get; set; }

    public decimal CommissionEarned { get; set; }
}*/