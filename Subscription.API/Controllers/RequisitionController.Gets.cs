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
        if (reqSearch == null)
        {
            return BadRequest("Search criteria cannot be null.");
        }
        return await ExecuteReaderAsync("GetRequisitions", command =>
        {
            command.Int("Count", reqSearch.ItemCount);
            command.Int("Page", reqSearch.Page);
            command.Int("SortRow", reqSearch.SortField);
            command.TinyInt("SortOrder", reqSearch.SortDirection);
            command.Varchar("Code", 15, reqSearch.Code);
            command.Varchar("Title", 2000, reqSearch.Title);
            command.Varchar("Company", 2000, reqSearch.Company);
            command.Varchar("Option", 30, reqSearch.Option);
            command.Varchar("Status", 1000, reqSearch.Status);
            command.Varchar("CreatedBy", 10, reqSearch.CreatedBy);
            command.DateTime("CreatedOn", reqSearch.CreatedOn);
            command.DateTime("CreatedOnEnd", reqSearch.CreatedOnEnd);
            command.DateTime("Due", reqSearch.Due);
            command.DateTime("DueEnd", reqSearch.DueEnd);
            command.Bit("Recruiter", reqSearch.Recruiter);
            command.Bit("GetCompanyInformation", getCompanyInformation);
            command.Varchar("User", 10, reqSearch.User);
            command.Int("OptionalRequisitionID", requisitionID);
            command.Bit("ThenProceed", thenProceed);
            command.Varchar("LoggedUser", 10, user);
        }, async reader =>
        {
            if (requisitionID > 0 && !thenProceed)
            {
                await reader.ReadAsync();
                return new ReturnGridRequisition { Page = reader.GetInt32(0) };
            }

            await reader.ReadAsync();
            int _count = reader.NInt32(0);

            await reader.NextResultAsync();
            string _requisitions = await reader.ReadAsync() ? reader.NString(0) : "[]";

            await reader.NextResultAsync();
            string _statusCount = "[]";
            if (getCompanyInformation)
            {
                _statusCount = await reader.ReadAsync() ? reader.NString(0) : "[]";
            }

            await reader.NextResultAsync();
            int _page = await reader.ReadAsync() ? reader.GetInt32(0) : reqSearch.Page;

            return new ReturnGridRequisition
            {
                Count = _count,
                Requisitions = _requisitions,
                Status = _statusCount,
                Page = _page
            };
        }, "GetGridRequisitions", "An error occurred while fetching requisitions.");
    }


    [HttpGet]
    public async Task<ActionResult<ReturnRequisitionDetails>> GetRequisitionDetails(int requisitionID, string roleID = "RC")
    {
        if (requisitionID == 0)
        {
            return BadRequest("Requisition ID is not provided.");
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
                                                                        
                                                                        // Result Set 3: Timeline
                                                                        await reader.NextResultAsync();
                                                                        string _timeline = "[]";
                                                                        if (await reader.ReadAsync())
                                                                        {
                                                                            _timeline = reader.NString(0, "[]");
                                                                        }

                                                                        // Result Set 4: Documents
                                                                        await reader.NextResultAsync();
                                                                        string documents = "[]";
                                                                        if (await reader.ReadAsync())
                                                                        {
                                                                            documents = reader.NString(0, "[]");
                                                                        }

                                                                        // Result Set 5: Notes
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
                                                                                   Notes = notes,
                                                                                   Timeline = _timeline
                                                                               };
                                                                    }, "GetRequisitionDetails", "Error fetching requisition details.");
    }

    [HttpGet]
    public async Task<ActionResult<string>> SearchRequisitions(string filter)
    {
        return await ExecuteQueryAsync("SearchRequisitions", command => { command.Varchar("Requisition", 30, filter); }, "SearchRequisitions", "Error searching requisitions.");
    }

}