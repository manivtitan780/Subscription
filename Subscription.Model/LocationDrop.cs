#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.Model
// File Name:           LocationDrop.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu
// Created On:          04-25-2024 19:04
// Last Updated On:     10-29-2024 15:10
// *****************************************/

#endregion

namespace Subscription.Model;

[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
public class LocationDrop
{
    public string City
    {
        get;
        set;
    }

    public int ID
    {
        get;
        set;
    }

    public string Location
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

    public string StreetAddress
    {
        get;
        set;
    }

    public string Zip
    {
        get;
        set;
    }

    public void Clear()
    {
        ID = 0;
        Location = "";
        StreetAddress = "";
        City = "";
        State = "";
        Zip = "";
        StateID = 0;
    }

    public LocationDrop Copy() => MemberwiseClone() as LocationDrop;
}