#region Header
// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.API
// File Name:           RequisitionController.Gets.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu, Brijesh Dubey
// Created On:          07-19-2025 20:07
// Last Updated On:     07-19-2025 20:20
// *****************************************/
#endregion

namespace Subscription.API.Controllers;

public partial class RequisitionController
{
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

        try
        {
            string _requisitions = "[]", _statusCount = "[]";
            await _connection.OpenAsync();
            await using SqlDataReader _reader = await _command.ExecuteReaderAsync();

            await _reader.ReadAsync();
            int _page = 0;
            if (requisitionID > 0 && !thenProceed)
            {
                _page = _reader.GetInt32(0);

                return new ReturnGridRequisition {Page = _page};
            }

            int _count = _reader.NInt32(0);

            await _reader.NextResultAsync();

            if (await _reader.ReadAsync())
            {
                _requisitions = _reader.NString(0);
            }

            await _reader.NextResultAsync();
            if (getCompanyInformation)
            {
                if (await _reader.ReadAsync())
                {
                    _statusCount = _reader.NString(0);
                }
            }

            await _reader.NextResultAsync();
            _page = reqSearch.Page;
            
            if (await _reader.ReadAsync())
            {
                _page = _reader.GetInt32(0);
            }

            return Ok(new
                      {
                          Count = _count,
                          Requisitions = _requisitions,
                          Companies = "[]",
                          CompanyContacts = "[]",
                          Status = _statusCount,
                          Page = _page
                      });
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error in GetGridRequisitions {ExceptionMessage}", ex.Message);
            return StatusCode(500, ex.Message);
        }
    }


    [HttpGet]
    public async Task<ActionResult<ReturnRequisitionDetails>> GetRequisitionDetails(int requisitionID, string roleID = "RC")
    {
        if (requisitionID == 0)
        {
            return StatusCode(500, "Requisition ID is not provided.");
        }

        return await ExecuteReaderAsync("GetRequisitionDetails", command =>
                                                                 {
                                                                     command.Int("RequisitionID", requisitionID);
                                                                     command.Varchar("RoleID", 2, roleID);
                                                                 }, async reader =>
                                                                    {
                                                                        // Result Set 1: Requisition details
                                                                        string requisitionDetail = "{}";
                                                                        if (await reader.ReadAsync())
                                                                        {
                                                                            requisitionDetail = reader.NString(0, "{}");
                                                                        }

                                                                        // Result Set 2: Activity
                                                                        await reader.NextResultAsync();
                                                                        string activity = "[]";
                                                                        if (await reader.ReadAsync())
                                                                        {
                                                                            activity = reader.NString(0, "[]");
                                                                        }

                                                                        // Result Set 3: Documents
                                                                        await reader.NextResultAsync();
                                                                        string documents = "[]";
                                                                        if (await reader.ReadAsync())
                                                                        {
                                                                            documents = reader.NString(0, "[]");
                                                                        }

                                                                        // Result Set 4: Notes
                                                                        await reader.NextResultAsync();
                                                                        string notes = "[]";
                                                                        if (await reader.ReadAsync())
                                                                        {
                                                                            notes = reader.NString(0, "[]");
                                                                        }

                                                                        return new ReturnRequisitionDetails
                                                                               {
                                                                                   Activity = activity,
                                                                                   Documents = documents,
                                                                                   Requisition = requisitionDetail,
                                                                                   Notes = notes
                                                                               };
                                                                    }, "GetRequisitionDetails", "Error fetching requisition details.");
    }

    [HttpGet]
    public async Task<ActionResult<string>> SearchRequisitions(string filter)
    {
        return await ExecuteQueryAsync("SearchRequisitions", command => { command.Varchar("Requisition", 30, filter); }, "SearchRequisitions", "Error searching requisitions.");
    }

}