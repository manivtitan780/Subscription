#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.Model
// File Name:           StateCache.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu, Brijesh Dubey
// Created On:          05-06-2025 15:05
// Last Updated On:     05-06-2025 15:32
// *****************************************/

#endregion

namespace Subscription.Model;

public record struct StateCache(int KeyValue, string Text, string Code);