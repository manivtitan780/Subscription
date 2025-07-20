#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.API
// File Name:           RequisitionController.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu, Brijesh Dubey
// Created On:          07-16-2025 16:07
// Last Updated On:     07-19-2025 20:28
// *****************************************/

#endregion

namespace Subscription.API.Controllers;

/// <summary>
///     Main RequisitionController class - now split into partial classes for better organization.<br/>
///     This file contains the constructor and shared configuration.<br/>
///     Partial class structure:<br/>
///     - RequisitionController.cs (main constructor and setup)<br/>
///     - RequisitionController.Gets.cs (GET operations)<br/>
///     - RequisitionController.Saves.cs (POST/Save operations)<br/>
///     - RequisitionController.Deletes.cs (DELETE operations)<br/>
///     - RequisitionController.Helpers.cs (helper methods and utilities)
/// </summary>
[ApiController, Route("api/[controller]/[action]"), SuppressMessage("ReSharper", "UnusedMember.Local")]
public partial class RequisitionController(SmtpClient smtpClient) : ControllerBase
{
    // Constructor parameter (SmtpClient smtpClient) is automatically available to all partial class files
    // No additional initialization needed - all methods are implemented in their respective partial files
}