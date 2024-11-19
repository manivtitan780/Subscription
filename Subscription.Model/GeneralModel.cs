#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.Model
// File Name:           GeneralModel.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu
// Created On:          03-25-2024 19:03
// Last Updated On:     11-19-2024 20:11
// *****************************************/

#endregion

#region Using

using Microsoft.Extensions.Configuration;

#endregion

namespace Subscription.Model;

internal static class GeneralModel
{
    static GeneralModel()
    {
        using Lock.Scope _scope = Lock.EnterScope();
        IConfigurationBuilder _builder = new ConfigurationBuilder().AddJsonFile("appsettings.json", true, true);

        IConfigurationRoot _configuration = _builder.Build();

        APIHost = _configuration.GetSection("APIHost").Value;
    }

    /// <summary>
    ///     The object used for locking to ensure thread safety when initializing the General class.
    /// </summary>
    private static readonly Lock Lock = new();

    /// <summary>
    ///     Gets the API host value from the configuration.
    /// </summary>
    /// <value>
    ///     The API host as a string.
    /// </value>
    /// <remarks>
    ///     This property is used to configure the base address for REST client instances in various parts of the application.
    /// </remarks>
    public static string APIHost
    {
        get;
    }
}