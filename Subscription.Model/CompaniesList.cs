#region Header
// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.Model
// File Name:           CompaniesList.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu, Brijesh Dubey
// Created On:          02-08-2025 19:02
// Last Updated On:     02-08-2025 19:02
// *****************************************/
#endregion

namespace Subscription.Model;

public class CompaniesList
{
    public int ID
    {
        get;
        set;
    }

    public string CompanyName
    {
        get;
        set;
    } = "";

    public string UpdatedBy
    {
        get;
        set;
    }

    public string CreatedBy
    {
        get;
        set;
    }
}