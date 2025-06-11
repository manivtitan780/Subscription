#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.Server
// File Name:           Start.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu, Brijesh Dubey
// Created On:          02-05-2025 20:02
// Last Updated On:     06-11-2025 19:41
// *****************************************/

#endregion

namespace Subscription.Server.Code;

public class Start
{
    public static string Access { get; set; }

    public static string AccountName { get; set; }

    public static string AllowedExtensions { get; set; }

    public static string APIHost { get; set; }

    public static string AzureBlob { get; set; }

    public static string AzureBlobContainer { get; set; } = "sub";

    public static string AzureKey { get; set; }

    public static string AzureOpenAIEndpoint { get; set; }

    public static string AzureOpenAIKey { get; set; }

    public static string CachePort { get; set; }

    public static string CacheServer { get; set; }

    public static string DeploymentName { get; set; }

    public static string Model { get; set; }

    public static string Prompt { get; set; }

    public static string UploadPath { get; set; }

    public static string UploadsPath { get; set; }
}