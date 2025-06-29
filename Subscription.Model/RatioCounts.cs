#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.Model
// File Name:           RatioCounts.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu, Brijesh Dubey
// Created On:          06-28-2025 15:06
// Last Updated On:     06-28-2025 15:55
// *****************************************/

#endregion

// ReSharper disable InconsistentNaming
namespace Subscription.Model;

public record struct RatioCounts(string User, float LAST7D_RATIO, float MTD_RATIO, float QTD_RATIO, float HYTD_RATIO, float YTD_RATIO);