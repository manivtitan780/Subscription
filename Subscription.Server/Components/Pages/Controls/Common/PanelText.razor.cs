#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Profsvc_AppTrack
// Project:             Profsvc_AppTrack
// File Name:           PanelText.razor.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja
// Created On:          11-23-2023 19:53
// Last Updated On:     12-29-2023 15:32
// *****************************************/

#endregion

namespace Subscription.Server.Components.Pages.Controls.Common;

/// <summary>
///     Represents a panel that displays text with various formatting options. The text can be formatted as an email,
///     telephone number, integer, decimal, or plain text.
/// </summary>
/// <remarks>
///     This class provides several properties that control the formatting and display of the text.
///     The 'IsEmail' and 'IsTel' properties determine if the text should be formatted as an email or telephone number,
///     respectively.
///     The 'Label' property specifies a label for the text.
///     The 'Text' property holds the actual text to be displayed.
///     The 'MarkupText' property allows for text with HTML markup.
///     The 'IntValue' and 'DecimalValue' properties allow for displaying integer and decimal values, respectively.
///     The 'FormatCurrency' property determines if the decimal value should be formatted as currency.
///     The 'ID' property allows for specifying an HTML ID for the text div.
/// </remarks>
public partial class PanelText
{
	/// <summary>
	///     Gets or sets the decimal value to be displayed in the panel.
	/// </summary>
	/// <value>
	///     The decimal value to be displayed. The default value is 0.
	/// </value>
	/// <remarks>
	///     This property holds the decimal value to be displayed in the panel.
	///     If the 'FormatCurrency' property is set to true, the decimal value is displayed as a currency value.
	///     If the 'ID' property is set, the decimal value is wrapped in a div with the specified ID.
	/// </remarks>
	[Parameter]
	public decimal DecimalValue
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets a value indicating whether the decimal value should be formatted as currency.
	/// </summary>
	/// <value>
	///     True if the decimal value should be formatted as currency; otherwise, false. The default value is false.
	/// </value>
	/// <remarks>
	///     When this property is set to true, the 'DecimalValue' property is displayed as a currency value.
	///     The currency format is determined by the current culture of the application.
	/// </remarks>
	[Parameter]
	public bool FormatCurrency
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets the HTML ID for the text div.
	/// </summary>
	/// <value>
	///     The HTML ID for the text div. The default value is null.
	/// </value>
	/// <remarks>
	///     This property allows for specifying an HTML ID for the text div.
	///     If this property is set, the text or the hyperlink (in case of email or telephone number) is wrapped in a div with
	///     the specified ID.
	/// </remarks>
	[Parameter]
	public string ID
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets the integer value to be displayed in the panel.
	/// </summary>
	/// <value>
	///     The integer value to be displayed. The default value is 0.
	/// </value>
	/// <remarks>
	///     This property holds the integer value to be displayed in the panel.
	///     If the 'ID' property is set, the integer value is wrapped in a div with the specified ID.
	/// </remarks>
	[Parameter]
	public int IntValue
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets a value indicating whether the text should be formatted as an email.
	/// </summary>
	/// <value>
	///     True if the text should be formatted as an email; otherwise, false. The default value is false.
	/// </value>
	/// <remarks>
	///     When this property is set to true, the text is displayed as a hyperlink with a "mailto:" URL.
	///     If the 'ID' property is set, the hyperlink is wrapped in a div with the specified ID.
	/// </remarks>
	[Parameter]
	public bool IsEmail
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets a value indicating whether the text should be formatted as a telephone number.
	/// </summary>
	/// <value>
	///     True if the text should be formatted as a telephone number; otherwise, false. The default value is false.
	/// </value>
	/// <remarks>
	///     When this property is set to true, the text is displayed as a hyperlink with a "tel:" URL.
	///     If the 'ID' property is set, the hyperlink is wrapped in a div with the specified ID.
	/// </remarks>
	[Parameter]
	public bool IsTel
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets the label for the text.
	/// </summary>
	/// <value>
	///     The label for the text. The default value is null.
	/// </value>
	/// <remarks>
	///     This property specifies a label that can be used to provide additional information or context for the text.
	///     The label is not displayed as part of the text itself, but can be used in conjunction with other properties
	///     to control the formatting and display of the text.
	/// </remarks>
	[Parameter]
	public string Label
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets the text with HTML markup to be displayed in the panel.
	/// </summary>
	/// <value>
	///     The text with HTML markup to be displayed. The default value is an empty MarkupString.
	/// </value>
	/// <remarks>
	///     This property holds the text with HTML markup to be displayed in the panel.
	///     The text is rendered as HTML, allowing for more complex formatting than the 'Text' property.
	///     If 'ID' property is set, the text is wrapped in a div with the specified ID.
	/// </remarks>
	[Parameter]
	public MarkupString MarkupText
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets the text to be displayed in the panel.
	/// </summary>
	/// <value>
	///     The text to be displayed. The default value is null.
	/// </value>
	/// <remarks>
	///     This property holds the actual text to be displayed in the panel.
	///     The text can be formatted based on the other properties of the class.
	///     For example, if 'IsEmail' or 'IsTel' is set to true, the text will be displayed as a hyperlink.
	///     If 'IntValue' or 'DecimalValue' is set, the text will be displayed as an integer or decimal respectively.
	///     If 'FormatCurrency' is set to true, the decimal value will be formatted as currency.
	/// </remarks>
	[Parameter]
	public string Text
	{
		get;
		set;
	}
}