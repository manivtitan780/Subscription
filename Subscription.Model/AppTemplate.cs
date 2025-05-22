#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.Model
// File Name:           AppTemplate.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu, Brijesh Dubey
// Created On:          05-03-2025 19:05
// Last Updated On:     05-03-2025 19:41
// *****************************************/

#endregion

namespace Subscription.Model;

/// <summary>
///     Represents a template in the Professional Services application.
/// </summary>
/// <remarks>
///     A template is a predefined set of data that can be used as a starting point for creating new items.
///     This class provides properties for storing template data and methods for manipulating it.
/// </remarks>
public class AppTemplate
{
	/// <summary>
	///     Initializes a new instance of the <see cref="AppTemplate" /> class and resets its properties to their default values.
	/// </summary>
	public AppTemplate()
    {
        Clear();
    }

	/// <summary>
	///     Initializes a new instance of the <see cref="AppTemplate" /> class.
	/// </summary>
	/// <param name="id">The unique identifier for the template.</param>
	/// <param name="templateName">The name of the template.</param>
	/// <param name="subject">The subject of the template.</param>
	/// <param name="cc">The CC property of the Template class.</param>
	/// <param name="templateContent">The content of the template.</param>
	/// <remarks>
	///     This constructor is used to create a new template with the specified values.
	/// </remarks>
	public AppTemplate(int id, string templateName, string subject, string cc, string templateContent)
    {
        ID = id;
        TemplateName = templateName;
        Subject = subject;
        CC = cc;
        TemplateContent = templateContent;
        Notes = "";
        CreatedDate = DateTime.Now;
        CreatedBy = "";
        UpdatedDate = DateTime.Now;
        UpdatedBy = "";
        Status = "Active";
        IsEnabled = true;
		SendTo = "";
        Action = 0;
    }

	/// <summary>
	///     Initializes a new instance of the <see cref="AppTemplate" /> class.
	/// </summary>
	/// <param name="id">The unique identifier for the template.</param>
	/// <param name="templateName">The name of the template.</param>
	/// <param name="subject">The subject of the template.</param>
	/// <param name="cc">The CC property of the Template class.</param>
	/// <param name="templateContent">The content of the template.</param>
	/// <param name="notes">The notes for the template.</param>
	/// <param name="createdDate">The creation date of the template.</param>
	/// <param name="createdBy">The creator of the template.</param>
	/// <param name="updatedDate">The date and time when the template was last updated.</param>
	/// <param name="updatedBy">The name of the user who last updated the template.</param>
	/// <param name="status">The status of the template.</param>
	/// <param name="enabled">A boolean value indicating whether the template is enabled.</param>
	/// <param name="sendTo">The recipient of the template.</param>
	/// <param name="action">The action to be performed with the template.</param>
	/// <remarks>
	///     This constructor is used to create a new template with the specified values.
	/// </remarks>
	public AppTemplate(int id, string templateName, string subject, string cc, string templateContent, string notes, DateTime createdDate, string createdBy,
					   DateTime updatedDate, string updatedBy, string status, bool enabled, string sendTo, byte action)
    {
        ID = id;
        TemplateName = templateName;
        Subject = subject;
        CC = cc;
        TemplateContent = templateContent;
        Notes = notes;
        CreatedDate = createdDate;
        CreatedBy = createdBy;
        UpdatedDate = updatedDate;
        UpdatedBy = updatedBy;
        Status = status;
        IsEnabled = enabled;
        SendTo = sendTo;
        Action = action;
    }

	/// <summary>
	///     Gets or sets the Action property of the Template class.
	/// </summary>
	/// <value>
	///     The Action property represents a byte value used to define the action associated with the template.
	/// </value>
	/// <remarks>
	///     This property is used in the TemplateDialog component for data binding and in the AdminController for saving the
	///     template.
	/// </remarks>
	public byte Action { get; set; }

	public string ActionText { get; set; }

	/// <summary>
	///     Gets or sets the CC property of the Template class.
	/// </summary>
	/// <value>
	///     The CC property represents a string value used in the TemplateDialog component for data binding.
	/// </value>
	public string CC { get; set; }

	/// <summary>
	///     Gets or sets the creator of the template.
	/// </summary>
	/// <value>
	///     The creator of the template.
	/// </value>
	/// <remarks>
	///     This property is used to track who created the template. It is automatically set when a new template is
	///     instantiated.
	/// </remarks>
	public string CreatedBy { get; set; }

	/// <summary>
	///     Gets or sets the creation date of the template.
	/// </summary>
	/// <value>
	///     The creation date of the template.
	/// </value>
	/// <remarks>
	///     This property is used to track when the template was created. It is automatically set to the current date and time
	///     when a new template is instantiated.
	/// </remarks>
	public DateTime CreatedDate { get; set; }

	/// <summary>
	///     Gets or sets the unique identifier for the template.
	/// </summary>
	/// <value>
	///     The unique identifier for the template.
	/// </value>
	/// <remarks>
	///     This property is used to uniquely identify each template. It is automatically generated upon creation of a new
	///     template and cannot be changed.
	/// </remarks>
	public int ID { get; set; }

	/// <summary>
	///     Gets or sets a value indicating whether the template is enabled.
	/// </summary>
	/// <value>
	///     <c>true</c> if the template is enabled; otherwise, <c>false</c>.
	/// </value>
	/// <remarks>
	///     This property is used to control the active status of the template. If the template is enabled, it can be used in
	///     the application; otherwise, it is considered inactive.
	/// </remarks>
	public bool IsEnabled { get; set; }

	/// <summary>
	///     Gets or sets the notes for the template.
	/// </summary>
	/// <value>
	///     The notes for the template.
	/// </value>
	public string Notes { get; set; }

	/// <summary>
	///     Gets or sets the recipient(s) of the template.
	/// </summary>
	/// <value>
	///     A string representing the recipient(s) of the template.
	/// </value>
	/// <remarks>
	///     This property is used to specify the recipient(s) of the template. It can be any string value, typically email
	///     addresses separated by commas.
	/// </remarks>
	public string SendTo { get; set; }

	public List<string> SendToList { get => SendTo.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToList(); set => SendTo = string.Join(",", value); }

	/// <summary>
	///     Gets or sets the status of the template.
	/// </summary>
	/// <value>
	///     A string representing the status of the template.
	/// </value>
	/// <remarks>
	///     This property is used to track the current status of the template. It can be any string value, typically "Active"
	///     or "Inactive".
	/// </remarks>
	public string Status { get; set; }

	/// <summary>
	///     Gets or sets the subject of the template.
	/// </summary>
	/// <value>
	///     The subject of the template.
	/// </value>
	public string Subject { get; set; }

	/// <summary>
	///     Gets or sets the content of the template.
	/// </summary>
	/// <value>
	///     A string representing the content of the template.
	/// </value>
	/// <remarks>
	///     This property is used to store the actual content of the template. It can be any string value, including HTML or
	///     other markup.
	/// </remarks>
	public string TemplateContent { get; set; }

	/// <summary>
	///     Gets or sets the name of the template.
	/// </summary>
	/// <value>
	///     The name of the template.
	/// </value>
	public string TemplateName { get; set; }

	/// <summary>
	///     Gets or sets the name of the user who last updated the template.
	/// </summary>
	/// <value>
	///     A string representing the name of the user who last updated the template.
	/// </value>
	/// <remarks>
	///     This property is used to track the user who made the last modification to the template. It is automatically set in
	///     the constructors and the ClearData method.
	/// </remarks>
	public string UpdatedBy { get; set; }

	/// <summary>
	///     Gets or sets the date and time when the template was last updated.
	/// </summary>
	/// <value>
	///     A <see cref="System.DateTime" /> representing the date and time of the last update.
	/// </value>
	/// <remarks>
	///     This property is used to track the last modification of the template. It is automatically set in the constructors
	///     and the ClearData method.
	/// </remarks>
	public DateTime UpdatedDate { get; set; }

	/// <summary>
	///     Resets the properties of the Template instance to their default values.
	/// </summary>
	/// <remarks>
	///     This method is used to clear the data of the current Template instance. It sets the properties of the Template
	///     instance to their default values.
	///     For example, it sets string properties to an empty string, integer properties to 0, DateTime properties to the
	///     current date and time, and boolean properties to true.
	/// </remarks>
	public void Clear()
    {
        ID = 0;
        TemplateName = "";
        Subject = "";
        CC = "";
        TemplateContent = "";
        Notes = "";
        CreatedDate = DateTime.Now;
        CreatedBy = "";
        UpdatedDate = DateTime.Now;
        UpdatedBy = "";
        Status = "Active";
        IsEnabled = true;
		SendTo = "";
        Action = 0;
    }

	/// <summary>
	///     Creates a copy of the current Template instance.
	/// </summary>
	/// <returns>
	///     A new Template object that is a copy of this instance.
	/// </returns>
	public AppTemplate Copy() => MemberwiseClone() as AppTemplate;
}