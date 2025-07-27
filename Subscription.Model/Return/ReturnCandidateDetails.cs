#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.Model
// File Name:           ReturnCandidateDetails.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu, Brijesh Dubey
// Created On:          07-11-2025 19:07
// Last Updated On:     07-27-2025 18:40
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
    string Documents,
    string TimeLine);