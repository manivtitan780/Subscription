#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.Client
// File Name:           Login.razor.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu
// Created On:          4-19-2024 14:59
// Last Updated On:     4-19-2024 14:59
// *****************************************/

#endregion

namespace Subscription.Client.Pages;

public partial class Login
{
    private Task LoginToApplication(EditContext arg)
    {
        return null;
    }

    private LoginModel LoginModel
    {
        get;
        set;
    } = new();

    private EditForm LoginForm
    {
        get;
        set;
    }
}