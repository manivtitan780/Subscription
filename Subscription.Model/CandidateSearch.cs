#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            ProfSvc_AppTrack
// Project:             ProfSvc_Classes
// File Name:           CandidateSearch.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja
// Created On:          12-09-2022 15:57
// Last Updated On:     10-26-2023 15:15
// *****************************************/

#endregion

namespace Subscription.Model;

/// <summary>
///     Represents a search for candidates in the professional services domain.
/// </summary>
/// <remarks>
///     This class is used to encapsulate all the parameters needed for a candidate search. It includes parameters for
///     filtering by various attributes such as name, skills, location, and more.
/// </remarks>
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
public class CandidateSearch
{
	/// <summary>
	///     Initializes a new instance of the <see cref="CandidateSearch" /> class.
	/// </summary>
	public CandidateSearch()
	{
		Clear();
	}

	/// <summary>
	///     Initializes a new instance of the <see cref="CandidateSearch" /> class.
	/// </summary>
	/// <param name="name">The name used in the candidate search.</param>
	/// <param name="allCandidates">If set to <c>true</c>, all candidates should be included in the search.</param>
	/// <param name="myCandidates">If set to <c>true</c>, the search is limited to the user's candidates.</param>
	/// <param name="includeAdmin">If set to <c>true</c>, admin should be included in the search.</param>
	/// <param name="keywords">The keywords used to filter candidates in the search.</param>
	/// <param name="skills">The skills used to filter the candidate search.</param>
	/// <param name="state">If set to <c>true</c>, candidates from all states should be included.</param>
	/// <param name="cityZip">If set to <c>true</c>, the city or zip code is included in the search.</param>
	/// <param name="stateID">The identifier for the state.</param>
	/// <param name="cityName">The name of the city where the candidate search is being conducted.</param>
	/// <param name="proximity">The proximity for the candidate search.</param>
	/// <param name="proximityUnit">The proximity unit for the candidate search.</param>
	/// <param name="eligibility">The eligibility for the candidate search.</param>
	/// <param name="relocate">The relocate preference for the candidate search.</param>
	/// <param name="jobOptions">The job options for the candidate search.</param>
	/// <param name="securityClearance">The security clearance level for the candidate search.</param>
	/// <param name="page">The page number for the candidate search results.</param>
	/// <param name="itemCount">The number of items per page in the candidate search results.</param>
	/// <param name="sortField">The field to sort the candidate search results.</param>
	/// <param name="sortDirection">The direction to sort the candidate search results.</param>
	/// <param name="user">The user performing the candidate search.</param>
	/// <param name="activeRequisitionsOnly">If set to <c>true</c>, only active requisitions should be included in the search.</param>
	public CandidateSearch(string name, bool allCandidates, bool myCandidates, bool includeAdmin, string keywords, string skills, bool state, bool cityZip,
						   string stateID, string cityName, int proximity, byte proximityUnit, int eligibility, string relocate, string jobOptions, string securityClearance,
						   int page, int itemCount, byte sortField, byte sortDirection, string user, bool activeRequisitionsOnly)
	{
		Name = name;
		AllCandidates = allCandidates;
		// MyCandidates = myCandidates;
		IncludeAdmin = includeAdmin;
		Keywords = keywords;
		Skills = skills;
		State = state;
		CityZip = cityZip;
		StateID = stateID;
		CityName = cityName;
		Proximity = proximity;
		ProximityUnit = proximityUnit;
		Eligibility = eligibility;
		Relocate = relocate;
		JobOptions = jobOptions;
		SecurityClearance = securityClearance;
		Page = page;
		ItemCount = itemCount;
		SortField = sortField;
		SortDirection = sortDirection;
		User = user;
		ActiveRequisitionsOnly = activeRequisitionsOnly;
	}

	/// <summary>
	///     Gets or sets a value indicating whether only active requisitions should be included in the search.
	/// </summary>
	/// <value>
	///     <c>true</c> if only active requisitions should be included; otherwise, <c>false</c>.
	/// </value>
	/// <remarks>
	///     This property is used in the CandidateSearch class to filter the search results based on the active status of the
	///     requisitions.
	///     When this property is set to true, the search will only return candidates that are associated with active
	///     requisitions.
	/// </remarks>
	public bool ActiveRequisitionsOnly
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets a value indicating whether all candidates should be included in the search.
	/// </summary>
	/// <value>
	///     <c>true</c> if all candidates should be included; otherwise, <c>false</c>.
	/// </value>
	/// <remarks>
	///     This property is used in the CandidateSearch class to filter the search results.
	///     When this property is set to true, the search will return all candidates.
	///     When set to false, the search will return a subset of candidates based on other search parameters.
	/// </remarks>
	public bool AllCandidates
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets the city name for the candidate search.
	/// </summary>
	/// <value>
	///     The name of the city where the candidate search is being conducted.
	/// </value>
	/// <remarks>
	///     This property is used to filter the candidates based on their location. If the city name is provided, the search
	///     will return only those candidates who are located in the specified city.
	/// </remarks>
	public string CityName
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets a value indicating whether the city or zip code is included in the candidate search.
	/// </summary>
	/// <value>
	///     <c>true</c> if the city or zip code is included in the search; otherwise, <c>false</c>.
	/// </value>
	public bool CityZip
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets the eligibility of the candidate.
	/// </summary>
	/// <value>
	///     The eligibility is represented as an integer.
	/// </value>
	/// <remarks>
	///     This property is used in the search criteria to filter candidates based on their eligibility status.
	/// </remarks>
	public int Eligibility
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets a value indicating whether to include admin in the candidate search.
	/// </summary>
	/// <value>
	///     <c>true</c> if admin should be included in the search; otherwise, <c>false</c>.
	/// </value>
	/// <remarks>
	///     This property is used in the CandidateSearch class to filter the search results based on the admin status of the
	///     candidates.
	///     When this property is set to true, the search will include candidates who are admins.
	/// </remarks>
	public bool IncludeAdmin
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets the count of items in the CandidateSearch.
	/// </summary>
	/// <value>
	///     The count of items.
	/// </value>
	public int ItemCount
	{
		get;
		set;
	} = 25;

	/// <summary>
	///     Gets or sets the job options for the candidate search.
	/// </summary>
	/// <value>
	///     The job options used in the candidate search. This can be used to filter candidates based on specific job options.
	/// </value>
	/// <remarks>
	///     This property is bound to the 'JobOptions' field in the 'AdvancedCandidateSearch' component. Any changes to this
	///     property will reflect in the UI.
	/// </remarks>
	public string JobOptions
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets the keywords used in the candidate search.
	/// </summary>
	/// <value>
	///     The keywords used to filter candidates in the search.
	/// </value>
	/// <remarks>
	///     This property is used to specify the keywords that will be used to filter the candidates in the search.
	///     The search will return candidates that have these keywords in their profiles.
	/// </remarks>
	public string Keywords
	{
		get;
		set;
	}

	/*
	/// <summary>
	///     Gets or sets a value indicating whether the search is limited to the user's candidates.
	/// </summary>
	/// <value>
	///     <c>true</c> if the search is limited to the user's candidates; otherwise, <c>false</c>.
	/// </value>
	/// <remarks>
	///     This property is used to filter the search results. If set to true, the search will only return candidates
	///     associated with the current user.
	/// </remarks>
	public bool MyCandidates
	{
		get;
		set;
	}
	*/

	/// <summary>
	///     Gets or sets the name used for candidate search.
	/// </summary>
	/// <value>
	///     The name used in the candidate search.
	/// </value>
	public string Name
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets the current page number in the candidate search.
	/// </summary>
	/// <value>
	///     The current page number.
	/// </value>
	/// <remarks>
	///     This property is used to navigate through the paginated candidate search results.
	///     It is used in conjunction with the ItemCount property to determine the range of candidates to display.
	/// </remarks>
	public int Page
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets the proximity for the candidate search.
	/// </summary>
	/// <value>
	///     The proximity is an integer that represents the distance within which the search for candidates is performed.
	/// </value>
	/// <remarks>
	///     The proximity is used in conjunction with the CityName and StateID properties to define the geographical area for
	///     the search.
	/// </remarks>
	public int Proximity
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets the unit of proximity for the candidate search.
	/// </summary>
	/// <value>
	///     The unit of proximity as a byte.
	/// </value>
	/// <remarks>
	///     This property is used in conjunction with the Proximity property to define the search radius.
	///     The value of this property determines the unit of measurement for the Proximity property.
	///     For example, if ProximityUnit is 1, the Proximity is measured in miles; if ProximityUnit is 2, the Proximity is
	///     measured in kilometers.
	/// </remarks>
	public byte ProximityUnit
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets a value indicating the candidate's willingness to relocate.
	/// </summary>
	/// <value>
	///     A string that represents the candidate's relocation preference.
	///     The value can be "%" to indicate any preference, or specific values can be used to indicate specific preferences.
	/// </value>
	/// <remarks>
	///     This property is used in the candidate search process to filter candidates based on their relocation preferences.
	/// </remarks>
	public string Relocate
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets the security clearance level for the candidate search.
	/// </summary>
	/// <value>
	///     The security clearance level as a string.
	/// </value>
	/// <remarks>
	///     This property is used to filter candidates based on their security clearance level.
	///     The value can be any string representing a security clearance level.
	///     Use "%" for no filtering or to include all security clearance levels.
	/// </remarks>
	public string SecurityClearance
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets the skills for the candidate search.
	/// </summary>
	/// <value>
	///     The skills used to filter the candidate search. This is a string that can contain multiple skills separated by
	///     commas.
	/// </value>
	/// <remarks>
	///     This property is used in the advanced candidate search to filter candidates based on their skills.
	///     The skills are specified as a string, where each skill is separated by a comma.
	/// </remarks>
	public string Skills
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets the direction of sorting for the candidate search.
	/// </summary>
	/// <value>
	///     The sort direction as a byte. A value of 0 represents descending order, and a value of 1 represents ascending
	///     order.
	/// </value>
	public byte SortDirection
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets the field by which the candidate search results should be sorted.
	/// </summary>
	/// <value>
	///     The byte value representing the sort field. Each byte value corresponds to a different field:
	///     1 - Default, 2 - Name, 3 - Phone, 4 - Email, 5 - Location, 8 - Status.
	/// </value>
	/// <remarks>
	///     This property is used in conjunction with the SortDirection property to determine the order of the search results.
	/// </remarks>
	public byte SortField
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets a value indicating whether to include candidates from all states in the search.
	/// </summary>
	/// <value>
	///     <c>true</c> if candidates from all states should be included; otherwise, <c>false</c>.
	/// </value>
	/// <remarks>
	///     This property is used in the CandidateSearch class to filter the search results based on the candidate's state.
	///     When this property is set to true, the search will return candidates from all states.
	/// </remarks>
	public bool State
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets the identifier for the state in the candidate search.
	/// </summary>
	/// <value>
	///     The identifier for the state.
	/// </value>
	/// <remarks>
	///     This property is used to filter candidates based on their location.
	///     The value corresponds to a specific state.
	/// </remarks>
	public string StateID
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets the user associated with the CandidateSearch instance.
	/// </summary>
	/// <value>
	///     The user is represented as a string. This property is used to identify the user performing the candidate search.
	/// </value>
	public string User
	{
		get;
		set;
	}

	/// <summary>
	///     Resets all properties of the CandidateSearch instance to their default values.
	/// </summary>
	/// <remarks>
	///     This method is used to clear or reset the search parameters in the CandidateSearch instance.
	///     It sets all the properties to their default values, preparing the instance for a new search.
	/// </remarks>
	public void Clear()
	{
		Name = "";
		AllCandidates = true;
		// MyCandidates = true;
		IncludeAdmin = true;
		Keywords = "";
		Skills = "";
		State = true;
		CityZip = false;
		StateID = "";
		CityName = "";
		Proximity = 25;
		ProximityUnit = 1;
		Eligibility = 0;
		Relocate = "";
		JobOptions = "";
		SecurityClearance = "";
		Page = 1;
		ItemCount = 25;
		SortField = 1;
		SortDirection = 0;
		User = "ADMIN";
		ActiveRequisitionsOnly = true;
	}

	/// <summary>
	///     Creates a deep copy of the current CandidateSearch object.
	/// </summary>
	/// <returns>
	///     A new CandidateSearch object that is a deep copy of this instance.
	/// </returns>
	public CandidateSearch Copy() => MemberwiseClone() as CandidateSearch;
}