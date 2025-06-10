#region Header
// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.Model
// File Name:           CandidateResume.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu, Brijesh Dubey
// Created On:          06-10-2025 19:06
// Last Updated On:     06-10-2025 19:36
// *****************************************/
#endregion

namespace Subscription.Model;

public class CandidateResume
{
    public CandidateResume()
    {
        Clear();
    }

    public List<string> Files { get; set; }

    public bool UpdateTextResume { get; set; }

    public void Clear()
    {
        Files = [];
        UpdateTextResume = false;
    }
}