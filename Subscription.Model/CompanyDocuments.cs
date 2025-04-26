#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.Model
// File Name:           CompanyDocuments.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu
// Created On:          02-11-2024 20:02
// Last Updated On:     10-29-2024 15:10
// *****************************************/

#endregion

namespace Subscription.Model;

[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
public class CompanyDocuments
{
    public int CompanyID { get; set; }

    public string CompanyName { get; set; }

    public string DocumentName { get; set; }

    public string InternalFileName { get; set; }

    public string FileName { get; set; }

    public List<string> Files { get; set; }

    public int ID { get; set; }

    public string Notes { get; set; }

    public string UpdatedBy { get; set; }

    public DateTime UpdatedDate { get; set; }
}