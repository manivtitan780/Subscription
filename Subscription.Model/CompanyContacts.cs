#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.Model
// File Name:           CompanyContacts.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu
// Created On:          02-11-2024 20:02
// Last Updated On:     04-25-2024 19:04
// *****************************************/

#endregion

namespace Subscription.Model;

[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
public class CompanyContacts
{
    public string? City
    {
        get;
        set;
    }

    public int CompanyID
    {
        get;
        set;
    }

    public string? CompanyName
    {
        get;
        set;
    }

    public string? CreatedBy
    {
        get;
        set;
    }

    public DateTime CreatedDate
    {
        get;
        set;
    }

    public string? Department
    {
        get;
        set;
    }

    public string? EmailAddress
    {
        get;
        set;
    }

    public string? Extension
    {
        get;
        set;
    }

    public string? Fax
    {
        get;
        set;
    }

    public string? FirstName
    {
        get;
        set;
    }

    public int ID
    {
        get;
        set;
    }

    public string? LastName
    {
        get;
        set;
    }

    public int LocationID
    {
        get;
        set;
    }

    public string? MiddleInitial
    {
        get;
        set;
    }

    public string? Notes
    {
        get;
        set;
    }

    public string? Phone
    {
        get;
        set;
    }

    public string? Prefix
    {
        get;
        set;
    }

    public bool PrimaryContact
    {
        get;
        set;
    }

    public string? Role
    {
        get;
        set;
    }

    public int? RoleID
    {
        get;
        set;
    }

    public string? RoleName
    {
        get;
        set;
    }

    public string? State
    {
        get;
        set;
    }

    public int StateID
    {
        get;
        set;
    }

    public string? StreetName
    {
        get;
        set;
    }

    public string? Suffix
    {
        get;
        set;
    }

    public string? Title
    {
        get;
        set;
    }

    public string? UpdatedBy
    {
        get;
        set;
    }

    public DateTime UpdatedDate
    {
        get;
        set;
    }

    public string? ZipCode
    {
        get;
        set;
    }

    public void Clear()
    {
        ID = 0;
        CompanyID = 0;
        LocationID = 0;
        FirstName = "";
        MiddleInitial = "";
        LastName = "";
        Prefix = "";
        Suffix = "";
        Title = "";
        RoleID = 0;
        RoleName = "";
        Role = "";
        Department = "";
        EmailAddress = "";
        Phone = "";
        Extension = "";
        Fax = "";
        Notes = "";
        PrimaryContact = false;
        CreatedBy = "";
        CreatedDate = DateTime.MinValue;
        UpdatedBy = "";
        UpdatedDate = DateTime.MinValue;
        StreetName = "";
        City = "";
        StateID = 0;
        State = "";
        ZipCode = "";
    }

    public CompanyContacts? Copy() => MemberwiseClone() as CompanyContacts;
}