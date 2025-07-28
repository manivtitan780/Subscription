#region Header
// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.API
// File Name:           RequisitionController.Deletes.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu, Brijesh Dubey
// Created On:          07-19-2025 20:07
// Last Updated On:     07-19-2025 20:19
// *****************************************/
#endregion

using System.Text;

namespace Subscription.API.Controllers;

public partial class RequisitionController
{
    [HttpPost]
    public async Task<ActionResult<string>> DeleteRequisitionDocument(int documentID, string user)
    {
        return await ExecuteReaderAsync("DeleteRequisitionDocuments", 
            command =>
            {
                command.Int("RequisitionDocId", documentID);
                command.Varchar("User", 10, user);
            },
            async reader =>
            {
                await reader.NextResultAsync(); // Skip the first result set
                if (await reader.ReadAsync())
                {
                    return reader.NString(0);
                }
                return "[]";
            },
            "DeleteRequisitionDocument", 
            "An unexpected error occurred while deleting the requisition document.");
    }

}