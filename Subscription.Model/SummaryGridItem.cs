#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.Model
// File Name:           SummaryGridItem.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu, Brijesh Dubey
// Created On:          06-28-2025 15:06
// Last Updated On:     06-28-2025 15:59
// *****************************************/

#endregion

namespace Subscription.Model;

public class SummaryGridItem
{
    public int Active { get; set; }

    public int Hired { get; set; }

    public int Interviews { get; set; }

    public int Offers { get; set; }

    public int Requisitions { get; set; }

    public string SuccessRate { get; set; }

    public string TimePeriod { get; set; }
}