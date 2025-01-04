#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            ProfSvc_AppTrack
// Project:             ProfSvc_Classes
// File Name:           CandidateRating.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja
// Created On:          12-09-2022 15:57
// Last Updated On:     10-26-2023 21:09
// *****************************************/

#endregion

namespace Subscription.Model;

/// <summary>
///     Represents a rating given to a candidate in the Professional Services application.
/// </summary>
/// <remarks>
///     This class includes properties such as Date, User, Rating, and Comments.
///     It also includes methods for resetting the properties to their default values and creating a shallow copy of the
///     CandidateRating object.
/// </remarks>
public readonly record struct CandidateRating(DateTime DateTime, string Name, byte Rating, string Comment);
