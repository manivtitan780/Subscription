#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.Model
// File Name:           CompanyContacts.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu
// Created On:          2-11-2024 20:37
// Last Updated On:     2-11-2024 21:6
// *****************************************/

#endregion

namespace Subscription.Model;

public class CompanyContacts
{
    public string City
    {
        get;
        init;
    }

    public int CompanyID
    {
        get;
        set;
    }

    public string CompanyName
    {
        get;
        set;
    }

    public string CreatedBy
    {
        get;
        set;
    }

    public DateTime CreatedDate
    {
        get;
        set;
    }

    public string Department
    {
        get;
        set;
    }

    public string Title
    {
        get;
        set;
    }

    public string EmailAddress
    {
        get;
        set;
    }

    public string Extension
    {
        get;
        set;
    }

    public string Fax
    {
        get;
        init;
    }

    public string FirstName
    {
        get;
        set;
    }

    public int ID
    {
        get;
        init;
    }

    public string LastName
    {
        get;
        set;
    }

    public string MiddleInitial
    {
        get;
        set;
    }

    public string Phone
    {
        get;
        init;
    }

    public string Prefix
    {
        get;
        set;
    }

    public string State
    {
        get;
        init;
    }

    public int StateID
    {
        get;
        init;
    }

    public string StreetName
    {
        get;
        init;
    }

    public string Suffix
    {
        get;
        set;
    }

    public string UpdatedBy
    {
        get;
        init;
    }

    public DateTime UpdatedDate
    {
        get;
        init;
    }

    public string ZipCode
    {
        get;
        init;
    }

    public string Notes
    {
        get;
        set;
    }
}