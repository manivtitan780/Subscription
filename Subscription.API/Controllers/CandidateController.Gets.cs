#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.API
// File Name:           CandidateController.Gets.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu, Brijesh Dubey
// Created On:          07-16-2025 16:00
// Last Updated On:     07-16-2025 16:00
// *****************************************/

#endregion

using Microsoft.AspNetCore.Http.HttpResults;

namespace Subscription.API.Controllers;

/// <summary>
/// Partial class containing GET operations for CandidateController.
/// Handles data retrieval methods for candidates, documents, and file operations.
/// </summary>
public partial class CandidateController
{
    [HttpGet]
    public async Task<ActionResult<string>> DownloadFile(int documentID)
    {
        return await ExecuteQueryAsync("DownloadCandidateDocument", command =>
                                                                    {
                                                                        command.Int("DocumentID", documentID);
                                                                    }, "DownloadFile", "Error downloading candidate document.");
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
            string candidateDetails = "{}";
            if (await reader.ReadAsync())
            {
                candidateDetails = reader.NString(0, "{}");
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

            // Result Set 7: Documents
            await reader.NextResultAsync();
            string documents = "[]";
            if (await reader.ReadAsync())
            {
                documents = reader.NString(0, "[]");
            }

            // Result Set 8: Rating/MPC data
            await reader.NextResultAsync();
            string ratingData = "[]";
            if (await reader.ReadAsync())
            {
                ratingData = reader.NString(0, "[]");
            }

            // Process Rating and MPC data
            List<CandidateRating> ratingList = [];
            List<CandidateMPC> mpcList = [];
            CandidateRatingMPC ratingMPC = new();

            if (!ratingData.NullOrWhiteSpace() && ratingData != "[]")
            {
                try
                {
                    JArray jsonArray = JArray.Parse(ratingData);
                    if (jsonArray.Any())
                    {
                        // Sort by DateTime descending to get the latest first
                        JArray sortedArray = new(jsonArray.OrderByDescending(obj => DateTime.Parse(obj["DateTime"]!.ToString())));
                        
                        foreach (JToken item in sortedArray)
                        {
                            // Process Rating entries
                            if (item["Rating"] != null)
                            {
                                ratingList.Add(new CandidateRating
                                {
                                    Rating = item["Rating"].ToByte(),
                                    Comment = item["Comment"]?.ToString() ?? "",
                                    DateTime = DateTime.Parse(item["DateTime"]!.ToString()),
                                    Name = item["Name"]?.ToString() ?? "",
                                    //User = item["User"]?.ToString() ?? ""
                                });
                            }
                            
                            // Process MPC entries
                            if (item["MPC"] != null)
                            {
                                mpcList.Add(new CandidateMPC
                                {
                                    MPC = item["MPC"].ToBoolean(),
                                    Comment = item["Comment"]?.ToString() ?? "",
                                    DateTime = DateTime.Parse(item["DateTime"]!.ToString()),
                                    Name = item["Name"]?.ToString() ?? ""
                                });
                            }
                        }

                        // Set the latest Rating/MPC for the combined object
                        JToken latestItem = sortedArray.FirstOrDefault();
                        if (latestItem != null)
                        {
                            ratingMPC = new CandidateRatingMPC
                            {
                                ID = candidateID,
                                Rating = latestItem["Rating"]?.ToByte() ?? 0,
                                RatingComments = latestItem["Comment"]?.ToString() ?? "",
                                MPC = latestItem["MPC"]?.ToBoolean() ?? false,
                                MPCComments = latestItem["Comment"]?.ToString() ?? ""
                            };
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Error processing rating/MPC data for candidate {CandidateID}", candidateID);
                }
            }

            return new ReturnCandidateDetails(
                candidateDetails,
                notes,
                skills,
                education,
                experience,
                activity,
                ratingList,
                mpcList,
                ratingMPC,
                documents
            );
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
        return await ExecuteQueryAsync("SearchCandidates", command =>
                                                           {
                                                               command.Varchar("Filter", 50, filter);
                                                           }, "SearchCandidates", "Error searching candidates.");
    }
}