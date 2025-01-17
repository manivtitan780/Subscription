#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.API
// File Name:           RequisitionController.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu
// Created On:          12-21-2024 19:12
// Last Updated On:     01-11-2025 20:01
// *****************************************/

#endregion

using System.Diagnostics.CodeAnalysis;

namespace Subscription.API.Controllers;

[ApiController, Route("api/[controller]/[action]"), SuppressMessage("ReSharper", "UnusedMember.Local")]
public class RequisitionController : ControllerBase
{
    /// <summary>
    ///     Generates a location string based on the provided requisition details and state name.
    /// </summary>
    /// <param name="requisition">An instance of the RequisitionDetails class containing the city and zip code.</param>
    /// <param name="stateName">The name of the state.</param>
    /// <returns>
    ///     A string representing the location, in the format of "City, State, ZipCode". If any part is not available, it
    ///     will be omitted from the string.
    /// </returns>
    private static string? GenerateLocation(RequisitionDetails requisition, string? stateName)
    {
        string? _location = "";
        if (!requisition.City.NullOrWhiteSpace())
        {
            _location = requisition.City;
        }

        if (!stateName.NullOrWhiteSpace())
        {
            _location += ", " + stateName;
        }
        else
        {
            _location = stateName;
        }

        if (!requisition.ZipCode.NullOrWhiteSpace())
        {
            _location += ", " + requisition.ZipCode;
        }
        else
        {
            _location = requisition.ZipCode;
        }

        return _location;
    }

    /// <summary>
    ///     Asynchronously retrieves a dictionary of requisition data based on the provided search parameters.
    /// </summary>
    /// <param name="reqSearch">An instance of the RequisitionSearch class containing the search parameters.</param>
    /// <param name="getCompanyInformation">
    ///     A boolean value indicating whether to retrieve company information. Default value
    ///     is false.
    /// </param>
    /// <param name="requisitionID">An optional integer representing a specific requisition ID. Default value is 0.</param>
    /// <param name="thenProceed">
    ///     A boolean value indicating whether to proceed if the requisition ID is greater than 0.
    ///     Default value is false.
    /// </param>
    /// <param name="user">
    ///     A string value containing the logged-in user whose role should be a recruiter to fetch additional
    ///     information from the companies list.
    /// </param>
    /// <returns>
    ///     A dictionary containing requisition data, including requisitions, companies, contacts, skills, status count,
    ///     count, and page.
    /// </returns>
    [HttpGet]
    public async Task<ActionResult<ReturnGridRequisition>> GetGridRequisitions([FromBody] RequisitionSearch reqSearch, bool getCompanyInformation = false, int requisitionID = 0,
                                                                               bool thenProceed = false, string user = "")
    {
        await using SqlConnection _connection = new(Start.ConnectionString);
        await using SqlCommand _command = new("GetRequisitions", _connection);
        _command.CommandType = CommandType.StoredProcedure;
        _command.Int("Count", reqSearch.ItemCount);
        _command.Int("Page", reqSearch.Page);
        _command.Int("SortRow", reqSearch.SortField);
        _command.TinyInt("SortOrder", reqSearch.SortDirection);
        _command.Varchar("Code", 15, reqSearch.Code);
        _command.Varchar("Title", 2000, reqSearch.Title);
        _command.Varchar("Company", 2000, reqSearch.Company);
        _command.Varchar("Option", 30, reqSearch.Option);
        _command.Varchar("Status", 1000, reqSearch.Status);
        _command.Varchar("CreatedBy", 10, reqSearch.CreatedBy);
        _command.DateTime("CreatedOn", reqSearch.CreatedOn);
        _command.DateTime("CreatedOnEnd", reqSearch.CreatedOnEnd);
        _command.DateTime("Due", reqSearch.Due);
        _command.DateTime("DueEnd", reqSearch.DueEnd);
        _command.Bit("Recruiter", reqSearch.Recruiter);
        _command.Bit("GetCompanyInformation", getCompanyInformation);
        _command.Varchar("User", 10, reqSearch.User);
        _command.Int("OptionalRequisitionID", requisitionID);
        _command.Bit("ThenProceed", thenProceed);
        _command.Varchar("LoggedUser", 10, user);

        string? _requisitions = "[]";
        int _count = 0, _page = 0;
        string? _companies = "[]";
        string? _companyContacts = "[]";
        string? _statusCount = "[]";
        try
        {
            await _connection.OpenAsync();
            await using SqlDataReader _reader = await _command.ExecuteReaderAsync();

            await _reader.ReadAsync();
            if (requisitionID > 0 && !thenProceed)
            {
                _page = _reader.GetInt32(0);
                await _reader.CloseAsync();

                await _connection.CloseAsync();

                return new ReturnGridRequisition {Page = _page};
            }

            _count = _reader.GetInt32(0);

            await _reader.NextResultAsync();

            while (await _reader.ReadAsync())
            {
                _requisitions = _reader.NString(0);
            }

            await _reader.NextResultAsync();
            if (getCompanyInformation)
            {
                while (await _reader.ReadAsync())
                {
                    _companies = _reader.NString(0);
                }

                await _reader.NextResultAsync();
                while (await _reader.ReadAsync())
                {
                    _companyContacts = _reader.NString(0);
                }

                await _reader.NextResultAsync();
                while (await _reader.ReadAsync())
                {
                    _statusCount = _reader.NString(0);
                }
            }
            else
            {
                await _reader.NextResultAsync();
                await _reader.NextResultAsync();
            }

            await _reader.NextResultAsync();
            _page = reqSearch.Page;
            while (await _reader.ReadAsync())
            {
                _page = _reader.GetInt32(0);
            }

            await _reader.CloseAsync();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error in GetGridRequisitions {ExceptionMessage}", ex.Message);
            return StatusCode(500, ex.Message);
        }
        finally
        {
            await _connection.CloseAsync();
        }

        return Ok(new
                  {
                      Count = _count,
                      Requisitions = _requisitions,
                      Companies = _companies,
                      CompanyContacts = _companyContacts,
                      Status = _statusCount,
                      Page = _page
                  });
    }

    /// <summary>
    ///     Converts a numerical priority value to a string representation.
    /// </summary>
    /// <param name="priority">The numerical priority value. Expected values are 0, 2, or any other number.</param>
    /// <returns>A string representing the priority. "Low" for 0, "High" for 2, and "Medium" for any other number.</returns>
    private static string GetPriority(byte priority)
    {
        return priority switch
               {
                   0 => "Low",
                   2 => "High",
                   _ => "Medium"
               };
    }

    /// <summary>
    ///     Asynchronously retrieves the details of a specific requisition.
    /// </summary>
    /// <param name="requisitionID">The ID of the requisition to retrieve details for.</param>
    /// <param name="roleID">The ID of the role. Default value is "RC".</param>
    /// <returns>A dictionary containing the requisition details, activity, and documents related to the specified requisition.</returns>
    [HttpGet]
    public async Task<ActionResult<ReturnRequisitionDetails>> GetRequisitionDetails([FromQuery] int requisitionID, [FromQuery] string roleID = "RC")
    {
        await using SqlConnection _connection = new(Start.ConnectionString);
        if (requisitionID == 0)
        {
            return StatusCode(500, "Requisition ID is not provided.");
        }
        string _requisitionDetail = "{}";

        await using SqlCommand _command = new("GetGridRequisitionDetailsView", _connection);
        _command.CommandType = CommandType.StoredProcedure;
        _command.Int("RequisitionID", requisitionID);
        _command.Varchar("RoleID", 2, roleID);
        await _connection.OpenAsync();
        await using SqlDataReader _reader = await _command.ExecuteReaderAsync();
        if (_reader.HasRows) //Candidate Details
        {
            await _reader.ReadAsync();
            try
            {
                /*_requisitionDetail = new(requisitionID, _reader.GetString(0), _reader.NString(1), _reader.GetString(2), _reader.GetString(3),
                                         _reader.GetInt32(4), _reader.GetString(5), _reader.GetString(6), _reader.GetString(38), _reader.GetString(8),
                                         _reader.GetDecimal(9), _reader.GetDecimal(10), _reader.GetDecimal(11), _reader.GetDecimal(12), _reader.GetDecimal(13),
                                         _reader.GetBoolean(14), _reader.GetString(15), _reader.NString(16), _reader.GetDecimal(17), _reader.GetDecimal(18),
                                         _reader.GetBoolean(19), _reader.GetDateTime(20), _reader.GetString(21), _reader.GetString(22), _reader.GetString(23),
                                         _reader.GetDateTime(24), _reader.GetString(25), _reader.GetDateTime(26), _reader.NString(27), _reader.NString(28),
                                         _reader.NString(29), _reader.GetBoolean(30), _reader.GetBoolean(31), _reader.NString(32), _reader.GetBoolean(33),
                                         _reader.GetDateTime(34), _reader.GetBoolean(35), _reader.GetString(39), _reader.GetInt32(7), _reader.GetString(40),
                                         _reader.NString(41), _reader.NString(42), _reader.NString(43), _reader.GetByte(44), _reader.NInt32(45),
                                         _reader.NInt32(46), _reader.NInt32(47), _reader.NString(48), _reader.GetInt32(36), _reader.GetInt32(37),
                                         _reader.NString(49), _reader.NString(50), _reader.NString(51), _reader.NString(52));*/
            }
            catch (Exception)
            {
                //
            }
        }

        await _reader.NextResultAsync(); //Activity
        List<CandidateActivity> _activity = new();
        while (await _reader.ReadAsync())
        {
            _activity.Add(new(_reader.GetString(0), _reader.GetDateTime(1), _reader.GetString(2), _reader.GetInt32(3), _reader.GetInt32(4),
                              _reader.GetString(5), _reader.GetString(6), _reader.GetInt32(7), _reader.GetBoolean(8), _reader.GetString(9),
                              _reader.GetString(10), _reader.GetString(11), _reader.GetBoolean(12), _reader.GetString(13), _reader.GetInt32(14),
                              _reader.GetString(15), _reader.GetInt32(16), _reader.GetString(17), _reader.GetBoolean(18),
                              _reader.NDateTime(19), _reader.GetString(20), _reader.NString(21), _reader.NString(22),
                              _reader.GetBoolean(23)));
        }

        await _reader.NextResultAsync();
        List<RequisitionDocuments> _documents = [];
        if (_reader.HasRows)
        {
            while (await _reader.ReadAsync())
            {
                try
                {
                    _documents.Add(new(_reader.GetInt32(0), _reader.GetInt32(1), _reader.NString(2), _reader.NString(3), _reader.NString(6),
                                       $"{_reader.NDateTime(5)} [{_reader.NString(4)}]", _reader.NString(7), _reader.GetString(8)));
                }
                catch (Exception)
                {
                    //
                }
            }
        }

        await _reader.CloseAsync();

        await _connection.CloseAsync();

        return new Dictionary<string, object>
               {
                   {
                       "Requisition", _requisitionDetail
                   },
                   {
                       "Activity", _activity
                   },
                   {
                       "Documents", _documents
                   }
               };
    }

    [HttpGet]
    public async Task<ActionResult<string>> SearchRequisitions(string filter)
    {
        await using SqlConnection _connection = new(Start.ConnectionString);
        await using SqlCommand _command = new("SearchRequisitions", _connection);
        _command.CommandType = CommandType.StoredProcedure;
        _command.Varchar("Requisition", 30, filter);

        string? _requisitions = "[]";
        try
        {
            await _connection.OpenAsync();

            _requisitions = (await _command.ExecuteScalarAsync())?.ToString();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error searching requisitions. {ExceptionMessage}", ex.Message);
            return StatusCode(500, ex.Message);
        }
        finally
        {
            await _connection.CloseAsync();
        }

        return Ok(_requisitions);
    }
}