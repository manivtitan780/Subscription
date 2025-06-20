#region Header
// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.Model
// File Name:           RatioCount.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu, Brijesh Dubey
// Created On:          06-19-2025 19:06
// Last Updated On:     06-19-2025 19:16
// *****************************************/
#endregion

namespace Subscription.Model;

public record struct RatioCount(string User, string Period, decimal HireCount, decimal OfferCount,  decimal Ratio);
