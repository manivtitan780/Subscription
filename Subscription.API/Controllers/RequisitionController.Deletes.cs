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

namespace Subscription.API.Controllers;

public partial class RequisitionController
{
    [HttpPost]
    public async Task<ActionResult<string>> DeleteRequisitionDocument(int documentID, string user)
    {
        await using SqlConnection _connection = new(Start.ConnectionString);
        await _connection.OpenAsync();
        try
        {
            string _documents = "[]";
            await using SqlCommand _command = new("DeleteRequisitionDocuments", _connection);
            _command.CommandType = CommandType.StoredProcedure;
            _command.Int("RequisitionDocId", documentID);
            _command.Varchar("User", 10, user);
            await using SqlDataReader _reader = await _command.ExecuteReaderAsync();

            _reader.NextResult();
            if (_reader.Read())
            {
                _documents = _reader.NString(0);
            }

            return Ok(_documents);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error in DeleteRequisitionDocument {ExceptionMessage}", ex.Message);
            return StatusCode(500, ex.Message);
        }
    }

}