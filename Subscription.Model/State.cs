#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.Model
// File Name:           State.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu, Brijesh Dubey
// Created On:          03-21-2025 14:03
// Last Updated On:     03-21-2025 14:03
// *****************************************/

#endregion

namespace Subscription.Model;

/// <summary>
///     Represents a state with properties such as ID, state name, and code.
/// </summary>
/// <remarks>
///     The State class is used to manage and manipulate state data. It includes properties for the state's ID, name, and
///     code,
///     as well as methods for resetting the state's properties and creating a copy of the state.
/// </remarks>
public class State
{
	/// <summary>
	///     Initializes a new instance of the <see cref="State" /> class with the specified ID, state name, and code and resets
	///     its properties to their default values.
	/// </summary>
	/// <remarks>
	///     This constructor is used to create a new state with their default values.
	/// </remarks>
	public State()
    {
        Clear();
    }

	/// <summary>
	///     Initializes a new instance of the <see cref="State" /> class with the specified ID, state name, and code.
	/// </summary>
	/// <param name="id">The unique identifier for the state.</param>
	/// <param name="state">The name of the state.</param>
	/// <param name="code">The code of the state.</param>
	/// <remarks>
	///     This constructor is used to create a new state with the provided ID, state name, and code.
	/// </remarks>
	public State(int id, string state, string code)
    {
        ID = id;
        StateName = state;
        Code = code;
    }

	/// <summary>
	///     Gets or sets the code of the state.
	/// </summary>
	/// <value>
	///     The code of the state.
	/// </value>
	/// <remarks>
	///     This property is used to represent the code of the state. It is typically a short, unique identifier for each
	///     state.
	/// </remarks>
	public string Code
    {
        get;
        set;
    }

    public string CreatedDate
    {
        get;
        set;
    }

	/// <summary>
	///     Gets or sets the unique identifier for the state.
	/// </summary>
	/// <value>
	///     The unique identifier for the state.
	/// </value>
	/// <remarks>
	///     This property is used to uniquely identify a state. It is used as the primary key in the database and is also used
	///     in the UI for state selection and management.
	/// </remarks>
	public int ID
    {
        get;
        set;
    }

	/// <summary>
	///     Gets or sets a value indicating whether the state is being added.
	/// </summary>
	/// <value>
	///     true if the state is being added; otherwise, false.
	/// </value>
	/// <remarks>
	///     This property is used to determine the operation being performed on the state.
	///     When a new state is being added, this property is set to true.
	///     When an existing state is being edited, this property is set to false.
	/// </remarks>
	public bool IsAdd
    {
        get;
        set;
    }

	/// <summary>
	///     Gets or sets the name of the state.
	/// </summary>
	/// <value>
	///     The name of the state.
	/// </value>
	/// <remarks>
	///     This property is used to represent the name of the state in a human-readable format.
	/// </remarks>
	public string StateName
    {
        get;
        set;
    }

    public string UpdatedDate
    {
        get;
        set;
    }

	/// <summary>
	///     Resets the properties of the State object to their default values.
	/// </summary>
	/// <remarks>
	///     The ID property is set to 0, and the StateName and Code properties are set to empty strings.
	///     This method is typically used to prepare a State object for reuse or to clear its current state.
	/// </remarks>
	public void Clear()
    {
        ID = 0;
        StateName = "";
        Code = "";
        CreatedDate = DateTime.Today.CultureDate();
        UpdatedDate = DateTime.Today.CultureDate();
    }

	/// <summary>
	///     Creates a shallow copy of the current State object.
	/// </summary>
	/// <returns>
	///     A shallow copy of the current State object.
	/// </returns>
	public State Copy() => MemberwiseClone() as State;
}