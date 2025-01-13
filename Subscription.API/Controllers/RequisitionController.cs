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

[ApiController, Route("api/[controller]/[action]")]
[SuppressMessage("ReSharper", "UnusedMember.Local")]
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
        await using SqlCommand _command = new("GetGridRequisitions", _connection);
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