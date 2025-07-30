#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.API
// File Name:           CandidateController.Gets.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu, Brijesh Dubey
// Created On:          07-29-2025 21:07
// Last Updated On:     07-29-2025 21:20
// *****************************************/

#endregion

namespace Subscription.API.Controllers;

/// <summary>
///     Partial class containing GET operations for CandidateController.
///     Handles data retrieval methods for candidates, documents, and file operations.
/// </summary>
public partial class CandidateController
{
    [HttpGet]
    public async Task<ActionResult<string>> DownloadFile(int documentID)
    {
        return await ExecuteQueryAsync("DownloadCandidateDocument", command => { command.Int("DocumentID", documentID); }, "DownloadFile", "Error downloading candidate document.");
    }

    [HttpGet]
    public async Task<ActionResult<string>> DownloadResume(int candidateID, string resumeType)
    {
        return await ExecuteQueryAsync("DownloadCandidateResume", command =>
                                                                  {
                                                                      command.Int("CandidateID", candidateID);
                                                                      command.Varchar("ResumeType", 20, resumeType);
                                                                  }, "DownloadResume", "Error downloading candidate resume.");
    }

    [HttpGet]
    public async Task<ActionResult<string>> GenerateSummary(int candidateID, int requisitionID)
    {
        return await ExecuteQueryAsync("GenerateCandidateSummary", command =>
                                                                   {
                                                                       command.Int("CandidateID", candidateID);
                                                                       command.Int("RequisitionID", requisitionID);
                                                                   }, "GenerateSummary", "Error generating candidate summary.");
    }

    [HttpGet]
    public async Task<ActionResult<ReturnCandidateDetails>> GetCandidateDetails(int candidateID, string roleID)
    {
        return await ExecuteReaderAsync("GetDetailCandidate", command =>
                                                              {
                                                                  command.Int("@CandidateID", candidateID);
                                                                  command.Char("@RoleID", 2, roleID);
                                                              }, async reader =>
                                                                 {
                                                                     // Memory optimization: Interned string constants for optimal performance
                                                                     const string emptyArray = "[]";
                                                                     const string emptyObject = "{}";

                                                                     // Result Set 1: Candidate Details (special handling for JSON extraction)
                                                                     string _candRating = "", _candMPC = "";
                                                                     string candidateDetails = await ReadNextResultAsync();

                                                                     // Special case: Use empty object for candidate details if no data
                                                                     if (candidateDetails == emptyArray)
                                                                     {
                                                                         candidateDetails = emptyObject;
                                                                     }

                                                                     if (candidateDetails != emptyObject)
                                                                     {
                                                                         // Memory optimization: Use System.Text.Json instead of Newtonsoft.Json
                                                                         using JsonDocument _candidateDoc = JsonDocument.Parse(candidateDetails);
                                                                         JsonElement root = _candidateDoc.RootElement;

                                                                         _candRating = root.TryGetProperty("RateNotes", out JsonElement ratingProp) ? ratingProp.GetString() ?? "" : "";
                                                                         _candMPC = root.TryGetProperty("MPCNotes", out JsonElement mpcProp) ? mpcProp.GetString() ?? "" : "";
                                                                     }

                                                                     // Result Set 2: Notes
                                                                     await reader.NextResultAsync();
                                                                     string notes = await ReadNextResultAsync();

                                                                     // Result Set 3: Skills
                                                                     await reader.NextResultAsync();
                                                                     string skills = await ReadNextResultAsync();

                                                                     // Result Set 4: Education
                                                                     await reader.NextResultAsync();
                                                                     string education = await ReadNextResultAsync();

                                                                     // Result Set 5: Experience
                                                                     await reader.NextResultAsync();
                                                                     string experience = await ReadNextResultAsync();

                                                                     // Result Set 6: Activity
                                                                     await reader.NextResultAsync();
                                                                     string activity = await ReadNextResultAsync();

                                                                     // Result Set 7: Managers (skip - no data reading)
                                                                     await reader.NextResultAsync();

                                                                     //Result Set 8: Submissions Timelines
                                                                     await reader.NextResultAsync();
                                                                     string _timeline = await ReadNextResultAsync();

                                                                     //Result Set 9: Documents
                                                                     await reader.NextResultAsync();
                                                                     string _documents = await ReadNextResultAsync();

                                                                     // Process Rating and MPC data using System.Text.Json consistently
                                                                     List<CandidateRating> _ratingList = [];
                                                                     List<CandidateMPC> _mpcList = [];
                                                                     CandidateRatingMPC _ratingMPC = new();

                                                                     if (!_candRating.NotNullOrWhiteSpace() && !_candMPC.NotNullOrWhiteSpace())
                                                                     {
                                                                         return new(candidateDetails, notes, skills, education, experience, activity, _ratingList, _mpcList, _ratingMPC, _documents, _timeline);
                                                                     }

                                                                     try
                                                                     {
                                                                         // Memory optimization: Deserialize MPC data using System.Text.Json if available
                                                                         if (_candMPC.NotNullOrWhiteSpace())
                                                                         {
                                                                             _mpcList = (JsonSerializer.Deserialize(_candMPC, JsonContext.CaseInsensitive.ListCandidateMPC) ?? []).OrderByDescending(x => x.DateTime).ToList();
                                                                         }

                                                                         // Memory optimization: Deserialize Rating data using System.Text.Json if available
                                                                         if (_candRating.NotNullOrWhiteSpace())
                                                                         {
                                                                             _ratingList = (JsonSerializer.Deserialize(_candRating, JsonContext.CaseInsensitive.ListCandidateRating) ?? []).OrderByDescending(x => x.DateTime).ToList();
                                                                         }

                                                                         // Set the latest Rating/MPC for the combined object with null safety
                                                                         CandidateMPC _latestMPC = _mpcList.FirstOrDefault();
                                                                         CandidateRating _latestRating = _ratingList.FirstOrDefault();

                                                                         _ratingMPC = new()
                                                                                      {
                                                                                          ID = candidateID,
                                                                                          Rating = _latestRating.Rating,
                                                                                          RatingComments = _latestRating.Comment ?? "",
                                                                                          MPC = _latestMPC.MPC,
                                                                                          MPCComments = _latestMPC.Comment ?? ""
                                                                                      };
                                                                     }
                                                                     catch (Exception ex)
                                                                     {
                                                                         Log.Error(ex, "Error processing rating/MPC data for candidate {CandidateID}", candidateID);
                                                                     }

                                                                     return new ReturnCandidateDetails(candidateDetails, notes, skills, education, experience, activity, _ratingList, _mpcList, _ratingMPC, _documents, _timeline);

                                                                     // Memory optimization: Local function eliminates code duplication for result reading
                                                                     async Task<string> ReadNextResultAsync() => await reader.ReadAsync() ? reader.NString(0, emptyArray) : emptyArray;
                                                                 }, "GetCandidateDetails", "Error getting candidate details.");
    }

    [HttpGet]
    public async Task<ActionResult<ReturnGrid>> GetGridCandidates([FromBody] CandidateSearch searchModel)
    {
        if (searchModel == null)
        {
            return BadRequest("Search model cannot be null.");
        }

        return await ExecuteReaderAsync("GetCandidates", command =>
                                                         {
                                                             command.Int("RecordsPerPage", searchModel!.ItemCount);
                                                             command.Int("PageNumber", searchModel.Page);
                                                             command.Int("SortColumn", searchModel.SortField);
                                                             command.TinyInt("SortDirection", searchModel.SortDirection);
                                                             command.Varchar("Name", 30, searchModel.Name ?? "");
                                                             command.Bit("MyCandidates", !searchModel.AllCandidates);
                                                             command.Bit("IncludeAdmin", searchModel.IncludeAdmin);
                                                             command.Varchar("Keywords", 2000, searchModel.Keywords ?? "");
                                                             command.Varchar("Skill", 2000, searchModel.Skills ?? "");
                                                             command.Bit("SearchState", !searchModel.CityZip);
                                                             command.Varchar("City", 30, searchModel.CityName ?? "");
                                                             command.Varchar("State", 1000, searchModel.StateID ?? "");
                                                             command.Int("Proximity", searchModel.Proximity);
                                                             command.TinyInt("ProximityUnit", searchModel.ProximityUnit);
                                                             command.Varchar("Eligibility", 10, searchModel.Eligibility);
                                                             command.Varchar("Reloc", 10, searchModel.Relocate ?? "");
                                                             command.Varchar("JobOptions", 10, searchModel.JobOptions ?? "");
                                                             command.Varchar("Security", 10, searchModel.SecurityClearance ?? "");
                                                             command.Varchar("User", 10, searchModel.User ?? "");
                                                             command.Bit("ActiveRequisitionsOnly", searchModel.ActiveRequisitionsOnly);
                                                             command.Bit("ShowArchive", searchModel.ShowArchive);
                                                         }, async reader =>
                                                            {
                                                                // Result Set 1: Count
                                                                await reader.ReadAsync();
                                                                int count = reader.NInt32(0);

                                                                // Result Set 2: Candidates data
                                                                await reader.NextResultAsync();
                                                                string candidates = "[]";
                                                                if (await reader.ReadAsync())
                                                                {
                                                                    candidates = reader.NString(0, "[]");
                                                                }

                                                                return new ReturnGrid
                                                                       {
                                                                           Count = count,
                                                                           Data = candidates
                                                                       };
                                                            }, "GetGridCandidates", "Error getting grid candidates.");
    }

    [HttpGet]
    public async Task<ActionResult<string>> SearchCandidates(string filter)
    {
        return await ExecuteQueryAsync("SearchCandidates", command => { command.Varchar("Name", 30, filter); }, "SearchCandidates", "Error searching candidates.");
    }
}