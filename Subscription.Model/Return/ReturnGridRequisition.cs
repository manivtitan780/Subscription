#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.Model
// File Name:           ReturnGridRequisition.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu
// Created On:          01-10-2025 19:01
// Last Updated On:     01-10-2025 19:01
// *****************************************/

#endregion

namespace Subscription.Model.Return;

public record struct ReturnGridRequisition(int Count, string Requisitions, string Companies, string CompanyContacts, string Status, int Page);