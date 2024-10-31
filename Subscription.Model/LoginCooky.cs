#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.Model
// File Name:           LoginCooky.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu
// Created On:          04-20-2024 20:04
// Last Updated On:     10-29-2024 15:10
// *****************************************/

#endregion

namespace Subscription.Model;

/// <summary>
///     Represents a user's login session in the application.
/// </summary>
/// <remarks>
///     This class is used to store and manage information about a user's login session,
///     including their ID, first name, last name, email address, role, role ID, last login date, and login IP.
/// </remarks>
[Serializable]
public class LoginCooky
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="LoginCooky" /> class.
    /// </summary>
    /// <remarks>
    ///     This constructor initializes the properties of the <see cref="LoginCooky" /> class with default values.
    /// </remarks>
    public LoginCooky()
    {
        UserID = string.Empty;
        FirstName = string.Empty;
        LastName = string.Empty;
        Email = string.Empty;
        Role = string.Empty;
        RoleID = string.Empty;
        Login = default;
        LoginIP = "0.0.0.0";
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="LoginCooky" /> class with the specified values.
    /// </summary>
    /// <param name="userID">The ID of the user.</param>
    /// <param name="firstName">The first name of the user.</param>
    /// <param name="lastName">The last name of the user.</param>
    /// <param name="emailAddress">The email address of the user.</param>
    /// <param name="role">The role of the user.</param>
    /// <param name="roleID">The ID of the user's role.</param>
    /// <param name="lastLoginDate">The last login date of the user.</param>
    /// <param name="loginIP">The IP address from which the user last logged in.</param>
    /// <remarks>
    ///     This constructor initializes the properties of the <see cref="LoginCooky" /> class with the provided values.
    /// </remarks>
    public LoginCooky(string userID = "", string firstName = "", string lastName = "", string emailAddress = "", string role = "", string roleID = "", DateTime lastLoginDate = default,
                      string loginIP = "0.0.0.0")
    {
        UserID = userID;
        FirstName = firstName;
        LastName = lastName;
        Email = emailAddress;
        Role = role;
        RoleID = roleID;
        Login = lastLoginDate;
        LoginIP = loginIP;
    }

    /// <summary>
    ///     Gets or sets the email address associated with the user's login session.
    /// </summary>
    /// <value>
    ///     The email address of the user.
    /// </value>
    /// <remarks>
    ///     This property is used when sending notifications or for user identification.
    /// </remarks>
    public string Email
    {
        get;
        set;
    }

    /// <summary>
    ///     Gets or sets the first name associated with the user's login session.
    /// </summary>
    /// <value>
    ///     The first name of the user.
    /// </value>
    /// <remarks>
    ///     This property is used for personalizing user experience and communications.
    /// </remarks>
    public string FirstName
    {
        get;
        set;
    }

    /// <summary>
    ///     Gets or sets the last name associated with the user's login session.
    /// </summary>
    /// <value>
    ///     The last name of the user.
    /// </value>
    /// <remarks>
    ///     This property is used for personalizing user experience and communications.
    /// </remarks>
    public string LastName
    {
        get;
        set;
    }

    /// <summary>
    ///     Gets or sets the date and time of the user's last login session.
    /// </summary>
    /// <value>
    ///     The date and time when the user last logged in.
    /// </value>
    /// <remarks>
    ///     This property is used for tracking user activity and for security audits.
    /// </remarks>
    public DateTime Login
    {
        get;
        set;
    }

    /// <summary>
    ///     Gets or sets the IP address associated with the user's login session.
    /// </summary>
    /// <value>
    ///     The IP address from which the user last logged in.
    /// </value>
    /// <remarks>
    ///     This property is used for tracking user activity and for security audits.
    /// </remarks>
    public string LoginIP
    {
        get;
        set;
    }

    /// <summary>
    ///     Gets or sets the role associated with the user's login session.
    /// </summary>
    /// <value>
    ///     The role of the user.
    /// </value>
    /// <remarks>
    ///     This property is used for determining the user's permissions and access levels within the application.
    /// </remarks>
    public string Role
    {
        get;
        set;
    }

    /// <summary>
    ///     Gets or sets the role identifier for the user's login session.
    /// </summary>
    /// <value>
    ///     The role identifier is a string that represents the role assigned to the user during the login session.
    /// </value>
    public string RoleID
    {
        get;
        set;
    }

    /// <summary>
    ///     Gets or sets the unique identifier for the user's login session.
    /// </summary>
    /// <value>
    ///     The unique identifier for the user's login session.
    /// </value>
    /// <remarks>
    ///     This property is used to uniquely identify a user's login session in the application.
    /// </remarks>
    public string UserID
    {
        get;
        set;
    }
}