#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.Model
// File Name:           UserList.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu
// Created On:          4-17-2024 19:15
// Last Updated On:     4-17-2024 19:16
// *****************************************/

#endregion

namespace Subscription.Model;

public class UserList
{
	public int ID
	{
		get;
		set;
	}

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