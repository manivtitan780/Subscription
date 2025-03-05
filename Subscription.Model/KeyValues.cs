#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.Model
// File Name:           KeyValues.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu
// Created On:          4-17-2024 19:32
// Last Updated On:     4-17-2024 19:33
// *****************************************/

#endregion

// ReSharper disable PropertyCanBeMadeInitOnly.Global
namespace Subscription.Model;

public class KeyValues
{
	public KeyValues()
	{
	}

	public KeyValues(string text, string keyValue)
	{
		Text = text;
		KeyValue = keyValue;
	}

	public string KeyValue
	{
		get;
		set;
	}

	public string Text
	{
		get;
		set;
	}
}