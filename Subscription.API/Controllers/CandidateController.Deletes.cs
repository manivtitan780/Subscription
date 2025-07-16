#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.API
// File Name:           CandidateController.Deletes.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu, Brijesh Dubey
// Created On:          07-16-2025 16:00
// Last Updated On:     07-16-2025 16:00
// *****************************************/

#endregion

namespace Subscription.API.Controllers;

/// <summary>
/// Partial class containing DELETE operations for CandidateController.
/// Handles all delete operations for candidates and related entities.
/// </summary>
public partial class CandidateController
{
    [HttpPost]
    public async Task<ActionResult<string>> DeleteCandidateDocument(int documentID, string user)
    {
        if (documentID == 0)
        {
            return Ok("[]");
        }

        return await ExecuteQueryAsync("DeleteCandidateDocument", command =>
                                                                  {
                                                                      command.Int("CandidateDocumentId", documentID);
                                                                      command.Varchar("User", 10, user); //TODO: make sure you delete the associated document from Azure filesystem too.
                                                                  }, "DeleteCandidateDocument", "Error deleting candidate document.");
    }

    [HttpPost]
    public async Task<ActionResult<string>> DeleteEducation(int id, int candidateID, string user)
    {
        if (id == 0)
        {
            return Ok("[]");
        }

        return await ExecuteQueryAsync("DeleteCandidateEducation", command =>
                                                                   {
                                                                       command.Int("Id", id);
                                                                       command.Int("candidateId", candidateID);
                                                                       command.Varchar("User", 10, user);
                                                                   }, "DeleteEducation", "Error deleting candidate education.");
    }

    [HttpPost]
    public async Task<ActionResult<string>> DeleteExperience(int id, int candidateID, string user)
    {
        if (id == 0)
        {
            return Ok("[]");
        }

        return await ExecuteQueryAsync("DeleteCandidateExperience", command =>
                                                                    {
                                                                        command.Int("Id", id);
                                                                        command.Int("candidateId", candidateID);
                                                                        command.Varchar("User", 10, user);
                                                                    }, "DeleteExperience", "Error deleting candidate experience.");
    }

    [HttpPost]
    public async Task<ActionResult<string>> DeleteNotes(int id, int candidateID, string user)
    {
        if (id == 0)
        {
            return Ok("[]");
        }

        return await ExecuteQueryAsync("DeleteCandidateNotes", command =>
                                                               {
                                                                   command.Int("Id", id);
                                                                   command.Int("candidateId", candidateID);
                                                                   command.Varchar("User", 10, user);
                                                               }, "DeleteNotes", "Error deleting candidate notes.");
    }

    [HttpPost]
    public async Task<ActionResult<string>> DeleteSkill(int id, int candidateID, string user)
    {
        if (id == 0)
        {
            return Ok("[]");
        }

        return await ExecuteQueryAsync("DeleteCandidateSkill", command =>
                                                               {
                                                                   command.Int("Id", id);
                                                                   command.Int("candidateId", candidateID);
                                                                   command.Varchar("User", 10, user);
                                                               }, "DeleteSkill", "Error deleting candidate skill.");
    }
}