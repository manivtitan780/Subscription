#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            ProfSvc_AppTrack
// Project:             ProfSvc_Classes
// File Name:           EmailTemplates.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja
// Created On:          04-15-2023 15:49
// Last Updated On:     10-26-2023 21:17
// *****************************************/

#endregion

namespace Subscription.Model;

/// <summary>
///     Represents an email template with CC, Subject, and Template properties.
/// </summary>
/// <remarks>
///     This class can be used to create and manage email templates. It includes methods for clearing the template and
///     creating a copy of the template.
/// </remarks>
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
public class EmailTemplates
{
	/// <summary>
	///     Initializes a new instance of the <see cref="EmailTemplates" /> class and resets its properties to their default
	///     values.
	/// </summary>
	public EmailTemplates()
	{
		Clear();
	}

	/// <summary>
	///     Initializes a new instance of the <see cref="EmailTemplates" /> class.
	/// </summary>
	/// <param name="cc">
	///     The CC field for the email template. This field contains the email addresses that should receive a
	///     copy of the email. Multiple email addresses should be separated by a comma.
	/// </param>
	/// <param name="subject">
	///     The subject of the email template. This subject can contain placeholders which will be replaced
	///     with actual values when the email is being sent.
	/// </param>
	/// <param name="template">
	///     The email template. This is a string that contains placeholders which are replaced with actual
	///     values when sending an email.
	/// </param>
	public EmailTemplates(string? cc, string? subject, string? template)
	{
		CC = cc;
		Subject = subject;
		Template = template;
	}

	/// <summary>
	///     Gets or sets the CC field for the email template. This field contains the email addresses that should receive a
	///     copy of the email.
	///     Multiple email addresses should be separated by a comma.
	/// </summary>
	public string? CC
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets the subject of the email template. This subject can contain placeholders which will be replaced with
	///     actual values when the email is being sent.
	/// </summary>
	public string? Subject
	{
		get;
		set;
	}

	/// <summary>
	///     Gets or sets the email template. This is a string that contains placeholders which are replaced with actual values
	///     when sending an email.
	/// </summary>
	/// <value>
	///     The email template.
	/// </value>
	/// <example>
	///     An example of a template could be: "Hello $FIRST_NAME$, your application has been received."
	/// </example>
	public string? Template
	{
		get;
		set;
	}

	/// <summary>
	///     Clears the email template by resetting the CC, Subject, and Template properties to an empty string.
	/// </summary>
	public void Clear()
	{
		CC = "";
		Subject = "";
		Template = "";
	}

	/// <summary>
	///     Creates a shallow copy of the current EmailTemplates instance.
	/// </summary>
	/// <returns>
	///     A new EmailTemplates object that is a copy of the current instance.
	/// </returns>
	public EmailTemplates? Copy() => MemberwiseClone() as EmailTemplates;
}