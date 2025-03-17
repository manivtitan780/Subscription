#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            ProfSvc_AppTrack
// Project:             ProfSvc_Classes
// File Name:           JobOption.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja
// Created On:          09-17-2022 20:01
// Last Updated On:     10-26-2023 21:17
// *****************************************/

#endregion

namespace Subscription.Model;

/// <summary>
///     Represents a job option in the professional services' domain.
/// </summary>
/// <remarks>
///     A job option is a configurable aspect of a job, such as its benefits, duration, expenses, and more.
///     This class provides a way to encapsulate these aspects in a single object.
/// </remarks>
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
public class JobOptions
{
	/// <summary>
	///     Initializes a new instance of the <see cref="JobOptions" /> class.
	/// </summary>
	public JobOptions()
	{
		Clear();
	}

	/// <summary>
	///     Initializes a new instance of the <see cref="JobOptions" /> class.
	/// </summary>
	/// <param name="code">The code for the job option.</param>
	/// <param name="option">The option for the job.</param>
	/// <param name="description">The description of the job option. Default is an empty string.</param>
	/// <param name="updatedDate">The date when the job option was last updated. Default is an empty string.</param>
	/// <param name="duration">A value indicating whether the job option has a duration. Default is false.</param>
	/// <param name="rate">A value indicating whether the rate is included in the job option. Default is false.</param>
	/// <param name="sal">A value indicating whether the salary is included in the job option. Default is false.</param>
	/// <param name="tax">The tax for the job option. Default is an empty string.</param>
	/// <param name="exp">A value indicating whether the job option has an expense. Default is false.</param>
	/// <param name="placeFee">A value indicating whether the job option has a placement fee. Default is false.</param>
	/// <param name="benefits">A value indicating whether the job option includes benefits. Default is false.</param>
	/// <param name="showHours">A value indicating whether to show hours for the job option. Default is false.</param>
	/// <param name="rateText">The text for the rate of the job option. Default is "Rate".</param>
	/// <param name="percentText">The text for the percent of the job option. Default is "Percent".</param>
	/// <param name="costPercent">The cost percent for the job option. Default is 0.</param>
	/// <param name="showPercent">A value indicating whether to show percent for the job option. Default is false.</param>
	public JobOptions(string code, string option, string description = "", string updatedDate = "", bool duration = false, bool rate = false, bool sal = false, string tax = "",
					 bool exp = false, bool placeFee = false, bool benefits = false, bool showHours = false, string rateText = "Rate", string percentText = "Percent", decimal costPercent = 0,
					 bool showPercent = false)
	{
		Code = code;
		Option = option;
		Description = description;
		UpdatedDate = updatedDate;
		Duration = duration;
		Rate = rate;
		Sal = sal;
		Tax = tax;
		Exp = exp;
		PlaceFee = placeFee;
		Benefits = benefits;
		ShowHours = showHours;
		RateText = rateText;
		PercentText = percentText;
		CostPercent = costPercent;
		ShowPercent = showPercent;
	}

	/// <summary>
	///     Gets or sets a value indicating whether the job option includes benefits.
	/// </summary>
	/// <value>
	///     <c>true</c> if the job option includes benefits; otherwise, <c>false</c>.
	/// </value>
	/// <remarks>
	///     This property is used to determine if the job option includes benefits.
	///     When benefits are associated with the job option, this property is set to <c>true</c>.
	///     When there are no benefits associated with the job option, this property is set to <c>false</c>.
	/// </remarks>
	public bool Benefits
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets the code for the job option.
	/// </summary>
	/// <value>
	///     The code for the job option.
	/// </value>
	public string Code
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets the cost percentage for the job option.
	/// </summary>
	/// <value>
	///     A decimal representing the cost percentage.
	/// </value>
	/// <remarks>
	///     This property is used in calculations related to job options. It can be set to any decimal value.
	/// </remarks>
	public decimal CostPercent
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets the description of the job option.
	/// </summary>
	/// <value>
	///     The description of the job option.
	/// </value>
	public string Description
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets a value indicating whether the job option has a duration.
	/// </summary>
	/// <value>
	///     <c>true</c> if the job option has a duration; otherwise, <c>false</c>.
	/// </value>
	/// <remarks>
	///     This property is used to determine if the job option has a duration.
	///     When a duration is associated with the job option, this property is set to <c>true</c>.
	///     When there is no duration associated with the job option, this property is set to <c>false</c>.
	/// </remarks>
	public bool Duration
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets a value indicating whether the job option includes expenses.
	/// </summary>
	/// <value>
	///     <c>true</c> if the job option includes expenses; otherwise, <c>false</c>.
	/// </value>
	/// <remarks>
	///     This property is used to determine if the job option includes expenses.
	///     When expenses are included in the job option, this property will be set to <c>true</c>.
	///     When expenses are not included in the job option, this property will be set to <c>false</c>.
	/// </remarks>
	public bool Exp
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets a value indicating whether the current JobOption instance is being added.
	/// </summary>
	/// <value>
	///     <c>true</c> if the current JobOption instance is being added; otherwise, <c>false</c>.
	/// </value>
	/// <remarks>
	///     This property is used to distinguish between adding a new JobOption and editing an existing one.
	///     When a new JobOption is being added, this property is set to <c>true</c>.
	///     When an existing JobOption is being edited, this property is set to <c>false</c>.
	/// </remarks>
	public bool IsAdd
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets the option for the job.
	/// </summary>
	/// <value>
	///     The option for the job.
	/// </value>
	public string Option
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets the percentage text for the job option.
	/// </summary>
	/// <value>
	///     The percentage text represented as a string.
	/// </value>
	/// <remarks>
	///     This property is used in the JobOptionDialog to bind the value of the TextBoxControl.
	/// </remarks>
	public string PercentText
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets a value indicating whether the placement fee is included in the job option.
	/// </summary>
	/// <value>
	///     A boolean value. If true, the placement fee will be included; otherwise, it will not.
	/// </value>
	/// <remarks>
	///     This property is used to control the inclusion of the placement fee in the job option. It is used in the
	///     calculation of the total cost of the job option.
	/// </remarks>
	public bool PlaceFee
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets a value indicating whether the rate is included in the job option.
	/// </summary>
	/// <value>
	///     A boolean value. If true, the rate will be included; otherwise, it will not.
	/// </value>
	/// <remarks>
	///     This property is used to control the inclusion of the rate in the job option. It is used in the calculation of the
	///     total cost of the job option.
	/// </remarks>
	public bool Rate
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets the rate text for the job option.
	/// </summary>
	/// <value>
	///     The rate text.
	/// </value>
	public string RateText
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets a value indicating whether the salary is included in the job option.
	/// </summary>
	/// <value>
	///     A boolean value. If true, the salary will be included; otherwise, it will not.
	/// </value>
	/// <remarks>
	///     This property is used to control the inclusion of the salary in the job option. It is used in the calculation of
	///     the total cost of the job option.
	/// </remarks>
	public bool Sal
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets a value indicating whether to display the hours in the job option.
	/// </summary>
	/// <value>
	///     A boolean value. If true, the hours will be displayed; otherwise, they will not.
	/// </value>
	/// <remarks>
	///     This property is used to control the visibility of the hours related to the job option in the user interface.
	/// </remarks>
	public bool ShowHours
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets a value indicating whether to display the percentage in the job option.
	/// </summary>
	/// <value>
	///     A boolean value. If true, the percentage will be displayed; otherwise, it will not.
	/// </value>
	/// <remarks>
	///     This property is used to control the visibility of the percentage related to the job option in the user interface.
	/// </remarks>
	public bool ShowPercent
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets the tax information for the job option.
	/// </summary>
	/// <value>
	///     A string representing the tax information.
	/// </value>
	/// <remarks>
	///     This property is used to store the tax information related to the job option. It is used in the calculation of the
	///     total cost of the job option.
	/// </remarks>
	public string Tax
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets the date when the JobOption was last updated.
	/// </summary>
	/// <value>
	///     A string representing the date in a format of "MM/dd/yyyy".
	/// </value>
	/// <remarks>
	///     This property is used to track the last modification of the JobOption instance. It is displayed in the "Last
	///     Updated" column of the Job Options grid in the Admin interface.
	/// </remarks>
	public string UpdatedDate
	{
		get;
		set;
	}

	/// <summary>
	///     Resets all properties of the JobOption instance to their default values.
	/// </summary>
	/// <remarks>
	///     This method is used to clear the data of the current JobOption instance. It sets all string properties to an empty
	///     string, all boolean properties to false, and the CostPercent property to 0. This method is typically used when
	///     reusing a JobOption instance for a new operation.
	/// </remarks>
	public void Clear()
	{
		Code = "";
		Option = "";
		Description = "";
		UpdatedDate = "";
		Duration = false;
		Rate = false;
		Sal = false;
		Tax = "";
		Exp = false;
		PlaceFee = false;
		Benefits = false;
		ShowHours = false;
		RateText = "Rate";
		PercentText = "Percent";
		CostPercent = 0M;
		ShowPercent = false;
		IsAdd = false;
	}

	/// <summary>
	///     Creates a copy of the current JobOption instance.
	/// </summary>
	/// <returns>
	///     A new JobOption object that is a copy of the current instance.
	/// </returns>
	/// <remarks>
	///     This method uses the MemberwiseClone method to create a shallow copy of the current object.
	/// </remarks>
	public JobOptions Copy() => MemberwiseClone() as JobOptions;
}