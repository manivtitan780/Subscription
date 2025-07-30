#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.API
// File Name:           DashboardController.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu, Brijesh Dubey
// Created On:          06-18-2025 20:06
// Last Updated On:     07-04-2025 16:21
// *****************************************/

#endregion

namespace Subscription.API.Controllers;

[ApiController, Route("api/[controller]/[action]")]
public class DashboardController : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ReturnDashboard>> GetDashboard(string roleName, string user)
    {
        await using SqlConnection _connection = new(Start.ConnectionString);
        string _procedureName = roleName switch
                                {
                                    "FD" or "RS" => "DashboardAccountsManager_Refactor",
                                    "RC" => "DashboardRecruiters_Refactor",
                                    "AD" => "DashboardAdmin_Refactor",
                                    _ => "DashboardAccountsManager_Refactor"
                                };
        await using SqlCommand _command = new(_procedureName, _connection);
        _command.CommandType = CommandType.StoredProcedure;
        if (roleName is not "AD")
        {
            _command.Varchar("User", 10, user);
        }

        try
        {
            await _connection.OpenAsync();
            await using SqlDataReader _reader = await _command.ExecuteReaderAsync();

            string _users = await ReadNextResultAsync(_reader);
            await _reader.NextResultAsync();
            string _consolidatedMetrics = await ReadNextResultAsync(_reader);
            await _reader.NextResultAsync();
            string _recentActivity = await ReadNextResultAsync(_reader);
            await _reader.NextResultAsync();
            string _placements = await ReadNextResultAsync(_reader);
            await _reader.NextResultAsync();
            string _requisitionTimingAnalytics = await ReadNextResultAsync(_reader);
            await _reader.NextResultAsync();
            string _companyTimingAnalytics = await ReadNextResultAsync(_reader);

            // Removed: _reader.CloseAsync() is unnecessary with await using pattern

            return Ok(new ReturnDashboard
                      {
                          Users = _users,
                          ConsolidatedMetrics = _consolidatedMetrics,
                          RecentActivity = _recentActivity,
                          Placements = _placements,
                          RequisitionTimingAnalytics = _requisitionTimingAnalytics,
                          CompanyTimingAnalytics = _companyTimingAnalytics
                      });

            async Task<string> ReadNextResultAsync(SqlDataReader reader)
            {
                return await reader.ReadAsync() ? reader.NString(0) : "[]";
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error fetching dashboard data. {ExceptionMessage}", ex.Message);
            return StatusCode(500, "An error occurred while fetching dashboard data.");
        }
    }
}