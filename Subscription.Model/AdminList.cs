#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            ProfSvc_AppTrack
// Project:             ProfSvc_Classes
// File Name:           AdminList.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja
// Created On:          09-17-2022 20:01
// Last Updated On:     10-26-2023 15:15
// *****************************************/

#endregion

namespace Subscription.Model;

/// <summary>
///     Represents a list of administrators in the Professional Services application.
/// </summary>
/// <remarks>
///     This class is used to manage a list of administrators in the Professional Services application.
///     It includes properties for the administrator's ID, code, text, creation date, last updated date, enabled status,
///     and entity type.
/// </remarks>
public class AdminList
{
	/// <summary>
	///     Initializes a new instance of the <see cref="AdminList" /> class.
	/// </summary>
	/// <remarks>
	///     This constructor is responsible for setting up the initial state of an AdminList object.
	///     It is typically used when creating a new AdminList object in the application.
	/// </remarks>
	public AdminList()
	{
		Clear();
	}

	/// <summary>
	///     Initializes a new instance of the <see cref="AdminList" /> class with specified parameters.
	/// </summary>
	/// <param name="id">The ID of the entity.</param>
	/// <param name="text">The text of the entity.</param>
	/// <param name="created">The creation date of the entity.</param>
	/// <param name="updated">The last updated date of the entity.</param>
	/// <param name="enabled">The text status of the entity. Default is "Active".</param>
	/// <param name="isEnabled">The status of the entity. Default is true.</param>
	/// <remarks>
	///     This constructor is used to create a new AdminList object with specific properties.
	/// </remarks>
	public AdminList(int id, string text, string created, string updated, string enabled = "Active", bool isEnabled = true)
	{
		ID = id;
		Code = "";
		Text = text;
		CreatedDate = created;
		UpdatedDate = updated;
		Enabled = enabled;
		IsEnabled = isEnabled;
		Entity = "";
	}

	/// <summary>
	///     Initializes a new instance of the <see cref="AdminList" /> class with specified parameters.
	/// </summary>
	/// <param name="code">The code of the entity.</param>
	/// <param name="text">The text of the entity.</param>
	/// <param name="created">The creation date of the entity.</param>
	/// <param name="updated">The last updated date of the entity.</param>
	/// <param name="enabled">The text status of the entity. Default is "Active".</param>
	/// <param name="isEnabled">The status of the entity. Default is true.</param>
	/// <param name="entity">The entity to which this class belongs to. Default is null.</param>
	/// <remarks>
	///     This constructor is used to create a new AdminList object with specific properties.
	/// </remarks>
	public AdminList(string code, string text, string created, string updated, string enabled = "Active", bool isEnabled = true, string entity = null)
	{
		Code = code;
		Text = text;
		CreatedDate = created;
		UpdatedDate = updated;
		Enabled = enabled;
		IsEnabled = isEnabled;
		Entity = entity;
	}

	/// <summary>
	///     Gets or sets the code for the AdminList instance.
	/// </summary>
	/// <value>
	///     The code is a unique identifier used to reference a specific AdminList instance. It is used in various operations
	///     such as editing a tax term in the Admin section of the ProfSvc_AppTrack application.
	/// </value>
	public string Code
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets the creation date of the AdminList.
	/// </summary>
	/// <value>
	///     The date when the AdminList was created.
	/// </value>
	public string CreatedDate
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets a value indicating whether the AdminList is enabled.
	/// </summary>
	/// <value>
	///     A string that represents the enabled status of the AdminList.
	///     This property gets or sets the value that indicates whether the AdminList is enabled.
	/// </value>
	public string Enabled
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets the Entity property of the AdminList class.
	/// </summary>
	/// <value>
	///     A string representing the entity type of the administrative list item. This property is used to distinguish between
	///     different types of administrative list items, such as "Title" in the context of designations, or "Education" in the
	///     context of education records.
	/// </value>
	public string Entity
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets the unique identifier for an instance of the AdminList class.
	/// </summary>
	public int ID
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets a value indicating whether the AdminList instance is being added.
	/// </summary>
	/// <value>
	///     True if the AdminList instance is being added; otherwise, false. The default is false.
	/// </value>
	/// <remarks>
	///     This property is used to determine the mode of operation in various parts of the application.
	///     For example, in the TaxTerm page of the ProfSvc_AppTrack application, it is used to distinguish between adding a
	///     new tax term and editing an existing one.
	///     In the AdminListValidator, it is used to apply different validation rules based on whether a new entity is being
	///     added or an existing one is being updated.
	/// </remarks>
	public bool IsAdd
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets a value indicating whether the AdminList is enabled.
	/// </summary>
	/// <value>
	///     A boolean that represents the enabled status of the AdminList.
	///     This property gets or sets the value that indicates whether the AdminList is enabled.
	///     If true, the AdminList is considered active and can be used in the application.
	///     If false, the AdminList is considered inactive and will not be used in the application.
	/// </value>
	public bool IsEnabled
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets the text of the AdminList entity.
	/// </summary>
	/// <value>
	///     The text of the AdminList entity.
	/// </value>
	/// <remarks>
	///     This property is used to represent the textual content of the AdminList entity. It is used in various parts of the
	///     application, including in the AdminListDialog and Designation components.
	/// </remarks>
	public string Text
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets the date when the admin list was last updated.
	///     This property is displayed in the "Last Updated" column in the admin interface.
	/// </summary>
	public string UpdatedDate
	{
		get;
		set;
	}

	/// <summary>
	///     Clears the data of the AdminList instance.
	/// </summary>
	/// <remarks>
	///     This method resets all properties of the AdminList instance to their default values. It is typically used when
	///     preparing an AdminList instance for reuse, such as when closing a dialog or canceling an edit operation in the
	///     admin interface of the application.
	/// </remarks>
	public void Clear()
	{
		ID = 0;
		Code = "";
		Text = "";
		CreatedDate = "";
		UpdatedDate = DateTime.Today.ToShortDateString();
		Enabled = "Active";
		IsEnabled = true;
		Entity = "";
	}

	/// <summary>
	///     Creates a deep copy of the current AdminList object.
	/// </summary>
	/// <returns>
	///     A new AdminList object that is a deep copy of this instance.
	/// </returns>
	/// <remarks>
	///     This method is used when you need to make a copy of the current object without affecting the original one.
	///     It is particularly useful when you want to make changes to a copy of the object, while keeping the original object
	///     unchanged.
	/// </remarks>
	public AdminList Copy() => MemberwiseClone() as AdminList;
}