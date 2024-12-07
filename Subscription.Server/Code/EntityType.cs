#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.Server
// File Name:           EntityType.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu
// Created On:          12-07-2024 16:12
// Last Updated On:     12-07-2024 16:12
// *****************************************/

#endregion

namespace Subscription.Server.Code;

/// <summary>
///     Represents the type of entity in the application.
/// </summary>
/// <remarks>
///     This enumeration is used in various parts of the application to distinguish between different types of entities
///     such as Candidates, Requisitions, Companies, Leads, and Benefits.
/// </remarks>
public enum EntityType
{
    Candidate = 1, Requisition = 2, Companies = 3, Leads = 4, Benefits = 5
}