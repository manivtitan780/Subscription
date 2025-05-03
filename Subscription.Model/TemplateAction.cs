#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            ProfSvc_AppTrack
// Project:             ProfSvc_Classes
// File Name:           TemplateAction.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja
// Created On:          04-09-2023 19:56
// Last Updated On:     10-14-2023 16:01
// *****************************************/

#endregion

namespace Subscription.Model;

/// <summary>
///     Represents the actions that can be performed on a template in the Professional Services classes.
/// </summary>
/// <remarks>
///     This enumeration is used to specify the type of action performed on a template.
/// </remarks>
/// <value>
///     The TemplateAction enumeration has the following values:
///     CandidateCreated, CandidateUpdated, CandidateSubmitted, CandidateDeleted,
///     CandidateStatusChanged, RequisitionCreated, RequisitionUpdated,
///     RequisitionStatusChanged, CandidateSubmissionUpdated, NoAction.
/// </value>
public enum TemplateAction
{
    CandidateCreated = 1, CandidateUpdated = 2, CandidateSubmitted = 3, CandidateDeleted = 4, CandidateStatusChanged = 5, RequisitionCreated = 6, RequisitionUpdated = 7, RequisitionStatusChanged = 8,
    CandidateSubmissionUpdated = 9, NoAction = 10
}