#region Header
// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.Model
// File Name:           RequisitionStatus.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu, Brijesh Dubey
// Created On:          06-02-2025 19:06
// Last Updated On:     06-02-2025 19:40
// *****************************************/
#endregion

namespace Subscription.Model;

public class RequisitionStatus
{
    public string Status { get; set; } = "NEW";

    public void Clear()
    {
        Status = "NEW";
    }
}