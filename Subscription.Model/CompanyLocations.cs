#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.Model
// File Name:           CompanyLocations.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu
// Created On:          02-11-2024 20:02
// Last Updated On:     10-29-2024 15:10
// *****************************************/

#endregion

namespace Subscription.Model;

public class CompanyLocations
{
    public string City
    {
        set;
        get;
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
        set;
    }

    public int ID
    {
        get;
        set;
    }

    public string Notes
    {
        get;
        set;
    }

    public string Phone
    {
        get;
        set;
    }

    public bool PrimaryLocation
    {
        get;
        set;
    }

    public string State
    {
        get;
        set;
    }

    public int StateID
    {
        get;
        set;
    }

    public string StreetName
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

    public string ZipCode
    {
        get;
        set;
    }

    public void Clear()
    {
        City = "";
        CompanyID = 0;
        CompanyName = "";
        CreatedBy = "ADMIN";
        CreatedDate = DateTime.Today;
        EmailAddress = "";
        Extension = "";
        Fax = "";
        ID = 0;
        Notes = "";
        Phone = "";
        PrimaryLocation = false;
        State = "";
        StateID = 1;
        StreetName = "";
        UpdatedBy = "ADMIN";
        UpdatedDate = DateTime.Today;
        ZipCode = "";
    }

    public CompanyLocations Copy() => MemberwiseClone() as CompanyLocations;
}