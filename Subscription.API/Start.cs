#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.API
// File Name:           Start.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu
// Created On:          04-16-2024 20:04
// Last Updated On:     10-29-2024 15:10
// *****************************************/

#endregion

namespace Subscription.API;

public static class Start
{
    public static string Access
    {
        get;
        set;
    }

    public static string APIHost
    {
        get;
        set;
    }

    public static string CachePort
    {
        get;
        set;
    }

    public static string CacheServer
    {
        get;
        set;
    }

    public static string AzureKey
    {
        get;
        set;
    }

    public static string AccountName
    {
        get;
        set;
    }
    
    public static string ConnectionString
    {
        get;
        set;
    }

    public static string AzureBlob
    {
        get;
        set;
    }

    public static string AzureBlobContainer
    {
        get;
        set;
    } = "sub";
}