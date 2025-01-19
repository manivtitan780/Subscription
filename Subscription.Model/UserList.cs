#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.Model
// File Name:           UserList.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu
// Created On:          04-17-2024 19:04
// Last Updated On:     10-29-2024 15:10
// *****************************************/

#endregion

namespace Subscription.Model;

[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
public class UserList
{
    public byte Role
    {
        get;
        set;
    }

    public string UserName
    {
        get;
        set;
    }
}