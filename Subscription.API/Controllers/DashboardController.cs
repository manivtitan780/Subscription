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
        /*Stopwatch _stopwatch = Stopwatch.StartNew();
        _stopwatch.Reset();
        _stopwatch.Start();*/
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

            string _users = "[]", _consolidatedMetrics = "[]", _recentActivity = "[]", _placements = "[]", _requisitionTimingAnalytics = "[]", _companyTimingAnalytics = "[]";
            // Optimized: Using if instead of while since exactly 1 row is returned
            if (await _reader.ReadAsync())
            {
                _users = _reader.NString(0);
            }

            await _reader.NextResultAsync();

            // Optimized: Using if instead of while since exactly 1 row is returned
            if (await _reader.ReadAsync())
            {
                _consolidatedMetrics = _reader.NString(0);
            }

            await _reader.NextResultAsync();
            // Optimized: Using if instead of while since exactly 1 row is returned
            if (await _reader.ReadAsync())
            {
                _recentActivity = _reader.NString(0);
            }

            await _reader.NextResultAsync();
            // Optimized: Using if instead of while since exactly 1 row is returned
            if (await _reader.ReadAsync())
            {
                _placements = _reader.NString(0);
            }

            await _reader.NextResultAsync();
            // Optimized: Using if instead of while since exactly 1 row is returned
            if (await _reader.ReadAsync())
            {
                _requisitionTimingAnalytics = _reader.NString(0);
            }

            await _reader.NextResultAsync();
            // Optimized: Using if instead of while since exactly 1 row is returned
            if (await _reader.ReadAsync())
            {
                _companyTimingAnalytics = _reader.NString(0);
            }

            // Removed: _reader.CloseAsync() is unnecessary with await using pattern

            /*_stopwatch.Stop();
            await System.IO.File.AppendAllTextAsync(@"C:\Logs\ZipLog.txt", $"Elapsed time: {_stopwatch.ElapsedMilliseconds} ms\n");
            //await File.WriteAllTextAsync(@"C:\Logs\ZipLog.txt", $"Elapsed time: {_stopwatch.ElapsedMilliseconds} ms\n");*/

            return Ok(new ReturnDashboard
                      {
                          Users = _users,
                          ConsolidatedMetrics = _consolidatedMetrics,
                          RecentActivity = _recentActivity,
                          Placements = _placements,
                          RequisitionTimingAnalytics = _requisitionTimingAnalytics,
                          CompanyTimingAnalytics = _companyTimingAnalytics
                      });
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error fetching dashboard data. {ExceptionMessage}", ex.Message);
            return StatusCode(500, ex.Message);
        }
        finally
        {
            await _connection.CloseAsync();
        }
    }
}