#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.API
// File Name:           DashboardController.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu, Brijesh Dubey
// Created On:          06-18-2025 20:06
// Last Updated On:     06-24-2025 20:30
// *****************************************/

#endregion

namespace Subscription.API.Controllers;

[ApiController, Route("api/[controller]/[action]")]
public class DashboardController : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ReturnDashboard>> GetAccountsManagerDashboard(string roleName, string user)
    {
        await using SqlConnection _connection = new(Start.ConnectionString);
        string _procedureName = roleName switch
                                {
                                    "FD" or "RS" => "DashboardAccountsManager",
                                    "RC" => "DashboardRecruiters",
                                    "AD" => "DashboardAdmin",
                                    _ => "DashboardAccountsManager"
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

            string _users = "[]", _totalRequisitions = "[]", _activeRequisitions = "[]", _candidatesInInterview = "[]", _offersExtended = "[]", _candidatesHired = "[]", _hireToOfferRatio = "[]",
                   _recentActivity = "[]", _placements = "[]";
            while (await _reader.ReadAsync())
            {
                _users = _reader.NString(0);
            }

            await _reader.NextResultAsync();

            while (await _reader.ReadAsync())
            {
                _totalRequisitions = _reader.NString(0);
            }

            await _reader.NextResultAsync();
            while (await _reader.ReadAsync())
            {
                _activeRequisitions = _reader.NString(0);
            }

            await _reader.NextResultAsync();
            while (await _reader.ReadAsync())
            {
                _candidatesInInterview = _reader.NString(0);
            }

            await _reader.NextResultAsync();
            while (await _reader.ReadAsync())
            {
                _offersExtended = _reader.NString(0);
            }

            await _reader.NextResultAsync();
            while (await _reader.ReadAsync())
            {
                _candidatesHired = _reader.NString(0);
            }

            await _reader.NextResultAsync();
            while (await _reader.ReadAsync())
            {
                _hireToOfferRatio = _reader.NString(0);
            }

            await _reader.NextResultAsync();
            while (await _reader.ReadAsync())
            {
                _recentActivity = _reader.NString(0);
            }

            await _reader.NextResultAsync();
            while (await _reader.ReadAsync())
            {
                _placements = _reader.NString(0);
            }

            await _reader.CloseAsync();

            return Ok(new ReturnDashboard
                      {
                          Users = _users, TotalRequisitions = _totalRequisitions, ActiveRequisitions = _activeRequisitions, CandidatesInInterview = _candidatesInInterview,
                          OffersExtended = _offersExtended, CandidatesHired = _candidatesHired, HireToOfferRatio = _hireToOfferRatio, RecentActivity = _recentActivity, Placements = _placements
                      });
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error changing candidate status. {ExceptionMessage}", ex.Message);
            return StatusCode(500, ex.Message);
        }
        finally
        {
            await _connection.CloseAsync();
        }
    }
}