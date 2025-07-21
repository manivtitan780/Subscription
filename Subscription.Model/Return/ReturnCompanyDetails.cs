#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.Model
// File Name:           ReturnCompanyDetails.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu
// Created On:          01-07-2025 15:01
// Last Updated On:     01-08-2025 15:01
// *****************************************/

#endregion

namespace Subscription.Model.Return;

public record struct ReturnCompanyDetails(string Company, string Contacts, string Locations, string Documents, string Requisitions, string Notes);