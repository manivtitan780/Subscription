#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.API
// File Name:           Start.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu, Brijesh Dubey
// Created On:          02-06-2025 16:02
// Last Updated On:     03-28-2025 16:03
// *****************************************/

#endregion

#region Using

using System.Diagnostics.CodeAnalysis;

#endregion

namespace Subscription.API;

[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
public static class Start
{
    public static string Access
    {
        get;
        set;
    } = "";

    public static string AccountName
    {
        get;
        set;
    }= "";

    public static string APIHost
    {
        get;
        set;
    } = "";

    public static string AzureBlob
    {
        get;
        set;
    } = "";

    public static string AzureBlobContainer
    {
        get;
        set;
    } = "sub";

    public static string AzureKey
    {
        get;
        set;
    } = "";

    public static string AzureOpenAIEndpoint
    {
        get;
        set;
    } = "";

    public static string AzureOpenAIKey
    {
        get;
        set;
    } = "";

    public static string CachePort
    {
        get;
        set;
    }="";

    public static string CacheServer
    {
        get;
        set;
    } = "";

    public static string ConnectionString
    {
        get;
        set;
    } = "";

    public static string EmailClientID
    {
        get;
        set;
    } = "";

    public static string EmailHost
    {
        get;
        set;
    } = "";

    public static string EmailPassword
    {
        get;
        set;
    } = "";

    public static string EmailSecret
    {
        get;
        set;
    } = "";

    public static string EmailUsername
    {
        get;
        set;
    } = "";

    public static int Port
    {
        get;
        set;
    } = 587;

    public static string TenantID
    {
        get;
        set;
    } = "";

    public static string SystemChatMessage
    {
        get;
        set;
    } = "You are an assistant that extracts structured data from resumes in JSON format.";

    public static string DeploymentName
    {
        get;
        set;
    } = "gpt-4o-prof";

    public static string Prompt
    {
        get;
        set;
    }
}