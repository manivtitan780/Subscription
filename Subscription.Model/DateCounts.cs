#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.Model
// File Name:           DateCounts.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu, Brijesh Dubey
// Created On:          06-28-2025 15:06
// Last Updated On:     06-28-2025 15:54
// *****************************************/

#endregion

// ReSharper disable InconsistentNaming
namespace Subscription.Model;

public record struct DateCounts(string User, int LAST7D_COUNT, int MTD_COUNT, int QTD_COUNT, int HYTD_COUNT, int YTD_COUNT);