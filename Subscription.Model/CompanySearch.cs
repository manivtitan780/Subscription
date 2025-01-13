#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.Model
// File Name:           CompanySearch.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu
// Created On:          2-8-2024 15:59
// Last Updated On:     2-8-2024 15:59
// *****************************************/

#endregion


namespace Subscription.Model;

[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
public class CompanySearch
{
    public string? CompanyName
    {
        get;
        set;
    }

    public int ItemCount
    {
        get;
        set;
    }

    public int Page
    {
        get;
        set;
    }

    public byte SortField
    {
        get;
        set;
    }

    public byte SortDirection
    {
        get;
        set;
    }

    public string? User
    {
        get;
        set;
    }

    public void Clear()
    {
        CompanyName = "";
        ItemCount = 25;
        Page = 1;
        SortField = 1;
        SortDirection = 1;
        User = "ADMIN";
    }
}