#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.API
// File Name:           CandidateController.Gets.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu, Brijesh Dubey
// Created On:          07-16-2025 19:07
// Last Updated On:     07-17-2025 16:23
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
                                                                     // Result Set 1: Candidate Details
                                                                     string _candRating = "", _candMPC = "";
                                                                     string candidateDetails = "{}";
                                                                     if (await reader.ReadAsync())
                                                                     {
                                                                         candidateDetails = reader.NString(0, "{}");

                                                                         // Memory optimization: Use System.Text.Json instead of Newtonsoft.Json
                                                                         using JsonDocument _candidateDoc = JsonDocument.Parse(candidateDetails);
                                                                         JsonElement root = _candidateDoc.RootElement;

                                                                         _candRating = root.TryGetProperty("RateNotes", out JsonElement ratingProp) ? ratingProp.GetString() ?? "" : "";
                                                                         _candMPC = root.TryGetProperty("MPCNotes", out JsonElement mpcProp) ? mpcProp.GetString() ?? "" : "";
                                                                     }

                                                                     // Result Set 2: Notes
                                                                     await reader.NextResultAsync();
                                                                     string notes = "[]";
                                                                     if (await reader.ReadAsync())
                                                                     {
                                                                         notes = reader.NString(0, "[]");
                                                                     }

                                                                     // Result Set 3: Skills
                                                                     await reader.NextResultAsync();
                                                                     string skills = "[]";
                                                                     if (await reader.ReadAsync())
                                                                     {
                                                                         skills = reader.NString(0, "[]");
                                                                     }

                                                                     // Result Set 4: Education
                                                                     await reader.NextResultAsync();
                                                                     string education = "[]";
                                                                     if (await reader.ReadAsync())
                                                                     {
                                                                         education = reader.NString(0, "[]");
                                                                     }

                                                                     // Result Set 5: Experience
                                                                     await reader.NextResultAsync();
                                                                     string experience = "[]";
                                                                     if (await reader.ReadAsync())
                                                                     {
                                                                         experience = reader.NString(0, "[]");
                                                                     }

                                                                     // Result Set 6: Activity
                                                                     await reader.NextResultAsync();
                                                                     string activity = "[]";
                                                                     if (await reader.ReadAsync())
                                                                     {
                                                                         activity = reader.NString(0, "[]");
                                                                     }

                                                                     // Result Set 7: Managers
                                                                     await reader.NextResultAsync();

                                                                     //Result Set 8: Documents
                                                                     await reader.NextResultAsync();
                                                                     string documents = "[]";
                                                                     if (await reader.ReadAsync())
                                                                     {
                                                                         documents = reader.NString(0, "[]");
                                                                     }

                                                                      // Process Rating and MPC data using System.Text.Json consistently
                                                                     List<CandidateRating> ratingList = [];
                                                                     List<CandidateMPC> mpcList = [];
                                                                     CandidateRatingMPC ratingMPC = new();

                                                                     if (_candRating.NotNullOrWhiteSpace() || _candMPC.NotNullOrWhiteSpace())
                                                                     {
                                                                         try
                                                                         {
                                                                             // Memory optimization: Deserialize MPC data using System.Text.Json if available
                                                                             if (_candMPC.NotNullOrWhiteSpace())
                                                                             {
                                                                                 List<CandidateMPC> allMPC = JsonSerializer.Deserialize<List<CandidateMPC>>(_candMPC) ?? [];
                                                                                 mpcList = allMPC.OrderByDescending(x => x.DateTime).ToList();
                                                                             }

                                                                             // Memory optimization: Deserialize Rating data using System.Text.Json if available
                                                                             if (_candRating.NotNullOrWhiteSpace())
                                                                             {
                                                                                 List<CandidateRating> allRating = JsonSerializer.Deserialize<List<CandidateRating>>(_candRating) ?? [];
                                                                                 ratingList = allRating.OrderByDescending(x => x.DateTime).ToList();
                                                                             }

                                                                             // Set the latest Rating/MPC for the combined object with null safety
                                                                             CandidateMPC latestMPC = mpcList.FirstOrDefault();
                                                                             CandidateRating latestRating = ratingList.FirstOrDefault();

                                                                             ratingMPC = new()
                                                                                         {
                                                                                             ID = candidateID,
                                                                                             Rating = latestRating.Rating,
                                                                                             RatingComments = latestRating.Comment ?? "",
                                                                                             MPC = latestMPC.MPC,
                                                                                             MPCComments = latestMPC.Comment ?? ""
                                                                                         };
                                                                         }
                                                                         catch (Exception ex)
                                                                         {
                                                                             Log.Error(ex, "Error processing rating/MPC data for candidate {CandidateID}", candidateID);
                                                                         }
                                                                     }

                                                                     return new ReturnCandidateDetails(candidateDetails,
                                                                                                       notes,
                                                                                                       skills,
                                                                                                       education,
                                                                                                       experience,
                                                                                                       activity,
                                                                                                       ratingList,
                                                                                                       mpcList,
                                                                                                       ratingMPC,
                                                                                                       documents);
                                                                 }, "GetCandidateDetails", "Error getting candidate details.");
    }

    [HttpGet]
    public async Task<ActionResult<ReturnGrid>> GetGridCandidates([FromBody] CandidateSearch searchModel = null)
    {
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