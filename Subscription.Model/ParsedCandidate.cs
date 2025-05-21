#region Header
// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.Model
// File Name:           ParsedCandidate.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu, Brijesh Dubey
// Created On:          05-07-2025 18:05
// Last Updated On:     05-07-2025 18:56
// *****************************************/
#endregion

namespace Subscription.Model;

public class ParsedCandidate
{
    public List<string> Files { get; set; }

    public string FileName { get; set; }
    
    public string Mime { get; set; }
    
    public byte[] DocumentBytes { get; set; } = [];

    public string SubmissionNotes { get; set; } = "";
}