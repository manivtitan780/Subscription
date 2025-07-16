#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.API
// File Name:           CandidateController.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu, Brijesh Dubey
// Created On:          02-06-2025 16:02
// Last Updated On:     07-16-2025 16:00
// *****************************************/

#endregion

#region Using

using System.Text.Json;

using RestSharp;

#endregion

namespace Subscription.API.Controllers;

/// <summary>
/// Main CandidateController class - now split into partial classes for better organization.
/// This file contains the constructor and shared configuration.
/// 
/// Partial class structure:
/// - CandidateController.cs (main constructor and setup)
/// - CandidateController.Gets.cs (GET operations)
/// - CandidateController.Saves.cs (POST/Save operations)
/// - CandidateController.Deletes.cs (DELETE operations)
/// - CandidateController.Helpers.cs (helper methods and utilities)
/// </summary>
[ApiController, Route("api/[controller]/[action]")]
public partial class CandidateController(SmtpClient smtpClient) : ControllerBase
{
    // Constructor parameter (SmtpClient smtpClient) is automatically available to all partial class files
    // No additional initialization needed - all methods are implemented in their respective partial files
}