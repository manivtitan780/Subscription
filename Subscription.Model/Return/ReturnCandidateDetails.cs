#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.Model
// File Name:           ReturnCandidateDetails.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu
// Created On:          01-08-2025 15:01
// Last Updated On:     01-08-2025 15:01
// *****************************************/

#endregion

namespace Subscription.Model.Return;

public record struct ReturnCandidateDetails(
    string Candidate,
    string Notes,
    string Skills,
    string Education,
    string Experience,
    string Activity,
    List<CandidateRating> Rating,
    List<CandidateMPC> MPC,
    CandidateRatingMPC RatingMPC,
    string Documents);