#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.Model
// File Name:           Company.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu
// Created On:          02-07-2024 19:02
// Last Updated On:     10-29-2024 15:10
// *****************************************/

#endregion

namespace Subscription.Model;

public class Company
{
    public string Address
    {
        get;
        set;
    }

    public string CompanyName
    {
        get;
        set;
    }

    public int ContactsCount
    {
        get;
        set;
    }

    public string EINNumber
    {
        get;
        set;
    }

    public string Email
    {
        get;
        set;
    }

    public int ID
    {
        get;
        init;
    }

    public int LocationsCount
    {
        get;
        set;
    }

    public string Phone
    {
        get;
        set;
    }

    public bool Status
    {
        get;
        set;
    }

    public string UpdatedBy
    {
        get;
        set;
    }

    public DateTime UpdatedDate
    {
        get;
        set;
    }

    public string Website
    {
        get;
        set;
    }

    public Company Copy() => MemberwiseClone() as Company;
}