#region Header
// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.Model
// File Name:           CandidateDetailsResume.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu, Brijesh Dubey
// Created On:          05-07-2025 20:05
// Last Updated On:     05-07-2025 20:15
// *****************************************/
#endregion

namespace Subscription.Model;

public class CandidateDetailsResume
{
    public CandidateDetails CandidateDetails { get; set; } = new();
    public ParsedCandidate ParsedCandidate { get; set; } = new();
}