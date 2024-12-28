#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.Model
// File Name:           Zip.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu
// Created On:          04-17-2024 20:04
// Last Updated On:     10-29-2024 15:10
// *****************************************/

#endregion

namespace Subscription.Model;

/// <summary>
///     Represents a Zip code and associated information such as city, state, and state ID.
/// </summary>
/// <remarks>
///     This class is used to encapsulate the details of a Zip code, including the associated city, state, and state ID.
///     It provides constructors for creating instances with different levels of detail, properties for accessing and
///     modifying the details,
///     and methods for operations such as clearing the details and creating a copy of the instance.
/// </remarks>
public class Zip
{
	/// <summary>
	///     Initializes a new instance of the <see cref="Zip" /> class and resets its properties to their default values.
	/// </summary>
	public Zip()
    {
        Clear();
    }

	/// <summary>
	///     Initializes a new instance of the <see cref="Zip" /> class with the specified zip code, city, and default state,
	///     and state ID.
	/// </summary>
	/// <param name="zip">The zip code.</param>
	/// <param name="city">The city associated with the zip code.</param>
	/// <remarks>
	///     This constructor is used to create a new instance of the <see cref="Zip" /> class with the specified zip code,
	///     city. State is set to empty and State ID is set to 0.
	///     It is used in various operations such as creating a new Zip instance in the 'GetCache' method of the
	///     'AdminController' class.
	/// </remarks>
	public Zip(string zip, string city)
    {
        ZipCode = zip;
        City = city;
        State = "";
        StateID = 0;
    }

	/// <summary>
	///     Initializes a new instance of the <see cref="Zip" /> class with the specified zip code, city, state, and state ID.
	/// </summary>
	/// <param name="zip">The zip code.</param>
	/// <param name="city">The city associated with the zip code.</param>
	/// <param name="state">The state associated with the zip code.</param>
	/// <param name="stateID">The ID of the state associated with the zip code.</param>
	/// <remarks>
	///     This constructor is used to create a new instance of the <see cref="Zip" /> class with the specified zip code,
	///     city, state, and state ID.
	///     It is used in various operations such as creating a new Zip instance in the 'GetCache' method of the
	///     'AdminController' class.
	/// </remarks>
	public Zip(string zip, string city, string state, int stateID)
    {
        ZipCode = zip;
        City = city;
        State = state;
        StateID = stateID;
        Latitude = 0F;
        Longitude = 0F;
    }

	/// <summary>
	///     Gets or sets the City associated with the Zip code.
	/// </summary>
	/// <value>
	///     The City.
	/// </value>
	/// <remarks>
	///     This property is used to represent the City associated with the instance of the Zip class.
	///     It is used in various operations such as setting the City in the 'ZipChange' methods of the 'EditCompanyDialog',
	///     'EditContactDialog', and 'RequisitionDetailsPanel' classes.
	/// </remarks>
	public string City
    {
        get;
        set;
    }

    public float Latitude
    {
        get;
        set;
    }

    public float Longitude
    {
        get;
        set;
    }

	/// <summary>
	///     Gets or sets the State associated with the Zip code.
	/// </summary>
	/// <value>
	///     The State.
	/// </value>
	/// <remarks>
	///     This property is used to represent the State associated with the instance of the Zip class.
	///     It is used in various operations such as setting the State in the 'ZipChange' methods of the 'EditCompanyDialog',
	///     'EditContactDialog', and 'RequisitionDetailsPanel' classes.
	/// </remarks>
	public string State
    {
        get;
        set;
    }

	/// <summary>
	///     Gets or sets the State ID associated with the Zip code.
	/// </summary>
	/// <value>
	///     The State ID.
	/// </value>
	/// <remarks>
	///     This property is used to represent the State ID associated with the instance of the Zip class.
	///     It is used in various operations such as setting the State ID in the 'ZipChange' methods of the
	///     'EditCompanyDialog', 'EditContactDialog', and 'RequisitionDetailsPanel' classes.
	/// </remarks>
	public int StateID
    {
        get;
        set;
    }

	/// <summary>
	///     Gets or sets the Zip code.
	/// </summary>
	/// <value>
	///     The Zip code.
	/// </value>
	/// <remarks>
	///     This property is used to represent the Zip code associated with the instance of the Zip class.
	///     It is used in various operations such as creating a copy of the Zip instance, resetting the ZipCode and City
	///     properties to their default values, and handling the change of the Zip code in the EditCompanyDialog and
	///     EditContactDialog.
	/// </remarks>
	public string ZipCode
    {
        get;
        set;
    }

	/// <summary>
	///     Resets the properties to their default values.
	/// </summary>
	public void Clear()
    {
        ZipCode = "";
        City = "";
        State = "";
        StateID = 0;
        Latitude = 0F;
        Longitude = 0F;
    }

	/// <summary>
	///     Creates a copy of the current Zip instance.
	/// </summary>
	/// <returns>
	///     A new Zip object that is a copy of this instance.
	/// </returns>
	public Zip Copy() => MemberwiseClone() as Zip;
}