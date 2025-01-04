#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            ProfSvc_AppTrack
// Project:             ProfSvc_Classes
// File Name:           CandidateMPC.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja
// Created On:          12-09-2022 15:57
// Last Updated On:     10-26-2023 21:08
// *****************************************/

#endregion

namespace Subscription.Model;

/// <summary>
///     Represents a CandidateMPC class in the ProfSvc_Classes namespace.
///     This class is used to manage and process data related to a candidate's MPC status.
/// </summary>
/// <remarks>
///     The CandidateMPC class includes properties such as Date, User, MPC, and Comments.
///     It also includes methods for creating a new instance of the class, resetting properties to their default values,
///     and creating a copy of the current object.
/// </remarks>
public readonly record struct CandidateMPC(DateTime DateTime, string Name, bool MPC, string Comment);