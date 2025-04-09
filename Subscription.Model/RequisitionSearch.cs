#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            ProfSvc_AppTrack
// Project:             ProfSvc_Classes
// File Name:           RequisitionSearch.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja
// Created On:          12-09-2022 15:57
// Last Updated On:     10-26-2023 21:19
// *****************************************/

#endregion

namespace Subscription.Model;

/// <summary>
///     Represents a search operation for requisitions.
/// </summary>
/// <remarks>
///     This class provides properties to specify various search criteria for requisitions,
///     such as the code, company, creation date, due date, and more. It also includes properties
///     to manage the pagination of the search results.
/// </remarks>
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
public class RequisitionSearch
{
	/// <summary>
	///     Initializes a new instance of the <see cref="RequisitionSearch" /> class and resets its properties to their default
	///     values.
	/// </summary>
	public RequisitionSearch()
	{
		Clear();
	}

	/// <summary>
	///     Initializes a new instance of the <see cref="RequisitionSearch" /> class.
	/// </summary>
	/// <param name="itemCount">The total number of items that match the requisition search criteria.</param>
	/// <param name="page">The page number for the requisition search results.</param>
	/// <param name="sortField">The field by which the requisition search results are sorted.</param>
	/// <param name="sortDirection">The sort direction for the requisition search.</param>
	/// <param name="code">The code for the RequisitionSearch.</param>
	/// <param name="title">The title of the requisition.</param>
	/// <param name="company">The company associated with the requisition.</param>
	/// <param name="option">The option for the RequisitionSearch.</param>
	/// <param name="status">The status of the requisition search.</param>
	/// <param name="createdBy">The user who created the requisition.</param>
	/// <param name="createdOn">The date and time when the requisition was created.</param>
	/// <param name="createdOnEnd">The end date for the creation time range of the requisition search.</param>
	/// <param name="due">The due date for the requisition search.</param>
	/// <param name="dueEnd">The end date for the due time range of the requisition search.</param>
	/// <param name="recruiter">Indicates whether the requisition is for a recruiter.</param>
	/// <param name="user">The user who is performing the requisition search.</param>
	/// <param name="optRequisitionID">The optional requisition ID for the RequisitionSearch.</param>
	public RequisitionSearch(int itemCount, int page, byte sortField, byte sortDirection, string code, string title, string company, string option,
							 string status, string createdBy, DateTime createdOn, DateTime createdOnEnd, DateTime due, DateTime dueEnd, bool recruiter, string user, int optRequisitionID = 0)
	{
		ItemCount = itemCount;
		Page = page;
		SortField = sortField;
		SortDirection = sortDirection;
		Code = code;
		Title = title;
		Company = company;
		Option = option;
		Status = status;
		CreatedBy = createdBy;
		CreatedOn = createdOn;
		CreatedOnEnd = createdOnEnd;
		Due = due;
		DueEnd = dueEnd;
		Recruiter = recruiter;
		User = user;
		OptRequisitionID = optRequisitionID;
	}

	public int OptRequisitionID
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets the code for the RequisitionSearch.
	/// </summary>
	/// <value>
	///     The code value.
	/// </value>
	/// <remarks>
	///     This property is used to filter the requisitions based on their code in the search operation.
	/// </remarks>
	public string Code
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets the company associated with the requisition.
	/// </summary>
	/// <value>
	///     The company associated with the requisition.
	/// </value>
	/// <remarks>
	///     This property is used to filter requisitions based on the associated company.
	/// </remarks>
	public string Company
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets the user who created the requisition.
	/// </summary>
	/// <value>
	///     The user who created the requisition.
	/// </value>
	/// <remarks>
	///     This property is used to filter requisitions based on the user who created them.
	/// </remarks>
	public string CreatedBy
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets the date and time when the requisition was created.
	/// </summary>
	/// <value>
	///     The date and time when the requisition was created.
	/// </value>
	public DateTime CreatedOn
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets the end date for the creation time range of the requisition search.
	/// </summary>
	/// <value>
	///     The end date for the creation time range.
	/// </value>
	/// <remarks>
	///     This property is used in conjunction with the CreatedOn property to define a date range for the requisition search.
	///     Requisitions created on or before this date will be included in the search results.
	/// </remarks>
	public DateTime CreatedOnEnd
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets the due date for the requisition search.
	/// </summary>
	/// <value>
	///     The due date for the requisition.
	/// </value>
	/// <remarks>
	///     This property is used to filter requisitions that are due by a certain date.
	/// </remarks>
	public DateTime Due
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets the end date for the due period in the requisition search.
	/// </summary>
	/// <value>
	///     The end date for the due period.
	/// </value>
	/// <remarks>
	///     This property is used to filter requisitions that are due by a certain end date.
	/// </remarks>
	public DateTime DueEnd
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets the total number of items that match the requisition search criteria.
	/// </summary>
	/// <value>
	///     The total number of items that match the requisition search criteria.
	/// </value>
	public int ItemCount
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets the option for the RequisitionSearch.
	/// </summary>
	/// <value>
	///     The option value used in the search.
	/// </value>
	/// <remarks>
	///     This property is used to specify an additional search criterion for the RequisitionSearch. It is bound to a
	///     dropdown control in the AdvancedRequisitionSearch component.
	/// </remarks>
	public string Option
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets the page number for the requisition search results.
	///     This property is used in pagination of the search results.
	/// </summary>
	public int Page
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets a value indicating whether the recruiter is involved in the requisition search.
	/// </summary>
	/// <value>
	///     <c>true</c> if the recruiter is involved; otherwise, <c>false</c>.
	/// </value>
	public bool Recruiter
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets the sort direction for the requisition search.
	/// </summary>
	/// <value>
	///     A byte that represents the sort direction. A value of 0 represents descending order, and a value of 1 represents
	///     ascending order.
	/// </value>
	/// <remarks>
	///     This property is used in conjunction with the SortField property to determine the ordering of the search results.
	/// </remarks>
	public byte SortDirection
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets the field by which the requisition search results are sorted.
	/// </summary>
	/// <remarks>
	///     The value of this property is a byte that corresponds to a specific field in the requisition data.
	///     For example, a value of 1 might correspond to sorting by requisition ID, a value of 2 might correspond to sorting
	///     by code, etc.
	///     The exact mapping of byte values to sort fields is implemented in the `Requisition.OnActionBegin()` method.
	/// </remarks>
	public byte SortField
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets the status of the requisition search.
	/// </summary>
	/// <value>
	///     The status of the requisition search as a string.
	/// </value>
	/// <remarks>
	///     This property is used to filter requisitions based on their status.
	///     The status value is case-insensitive, and it is converted to upper case in the application.
	/// </remarks>
	public string Status
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets the title of the requisition.
	/// </summary>
	/// <value>
	///     The title of the requisition.
	/// </value>
	public string Title
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets the user associated with the requisition search.
	/// </summary>
	/// <value>
	///     The username as a string.
	/// </value>
	/// <remarks>
	///     This property is used to filter requisitions based on the user.
	///     The username is case-insensitive, and it is converted to upper case in the application.
	/// </remarks>
	public string User
	{
		get;
		set;
	}

	/// <summary>
	///     Resets all properties of the RequisitionSearch object to their default values.
	/// </summary>
	/// <remarks>
	///     This method is typically used to clear the search parameters in preparation for a new search.
	/// </remarks>
	public void Clear()
	{
		ItemCount = 25;
		Page = 1;
		SortField = 1;
		SortDirection = 0;
		Code = "";
		Title = "";
		Company = "";
		Option = "";
		Status = "";
		CreatedBy = "";
		CreatedOn = new(1900, 1, 1);
		CreatedOnEnd = new(2099, 12, 31);
		Due = new(1900, 1, 1);
		DueEnd = new(2099, 12, 31);
		Recruiter = true;
		User = "%";
		OptRequisitionID = 0;
	}

	/// <summary>
	///     Creates a shallow copy of the current RequisitionSearch object.
	/// </summary>
	/// <returns>
	///     A shallow copy of the current RequisitionSearch object.
	/// </returns>
	public RequisitionSearch Copy() => MemberwiseClone() as RequisitionSearch;
}