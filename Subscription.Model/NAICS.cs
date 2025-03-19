#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.Model
// File Name:           NAICS.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu
// Created On:          03-08-2024 20:03
// Last Updated On:     10-29-2024 15:10
// *****************************************/

#endregion

namespace Subscription.Model;

public class NAICS
{
    public int ID
    {
        get;
        set;
    }

    public string Title
    {
        get;
        set;
    }

    public string CreatedDate
    {
        get;
        set;
    }

    public string UpdatedDate
    {
        get;
        set;
    }
    
    public void Clear()
    {
        ID = 0;
        Title = string.Empty;
        CreatedDate = DateTime.Today.CultureDate();
        UpdatedDate = DateTime.Today.CultureDate();
    }
    
    public NAICS Copy() => MemberwiseClone() as NAICS;
}