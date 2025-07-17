#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.API
// File Name:           RequisitionController.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu, Brijesh Dubey
// Created On:          07-16-2025 16:07
// Last Updated On:     07-16-2025 19:55
// *****************************************/

#endregion

namespace Subscription.API.Controllers;

[ApiController, Route("api/[controller]/[action]"), SuppressMessage("ReSharper", "UnusedMember.Local")]
public class RequisitionController(SmtpClient smtpClient) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<string>> ChangeRequisitionStatus(int requisitionID, string statusCode, string user)
    {
        return await ExecuteQueryAsync("ChangeRequisitionStatus", command =>
                                                                  {
                                                                      command.Int("RequisitionID", requisitionID);
                                                                      command.Char("Status", 3, statusCode);
                                                                      command.Varchar("User", 10, user);
                                                                  }, "ChangeRequisitionStatus", "Error changing requisition status.");
    }

    [HttpPost]
    public async Task<ActionResult<string>> DeleteRequisitionDocument([FromQuery] int documentID, [FromQuery] string user)
    {
        await using SqlConnection _connection = new(Start.ConnectionString);
        await _connection.OpenAsync();
        //List<RequisitionDocuments> _documents = [];
        try
        {
            string _documents = "[]";
            await using SqlCommand _command = new("DeleteRequisitionDocuments", _connection);
            _command.CommandType = CommandType.StoredProcedure;
            _command.Int("RequisitionDocId", documentID);
            _command.Varchar("User", 10, user);
            await using SqlDataReader _reader = await _command.ExecuteReaderAsync();
            _reader.NextResult();
            // Memory optimization: Replace while loop with if statement for single record
            if (_reader.Read())
            {
                _documents = _reader.NString(0);
                /*_documents.Add(new(_reader.GetInt32(0), _reader.GetInt32(1), _reader.NString(2), _reader.NString(3),
                                   _reader.NString(6), $"{_reader.NDateTime(5)} [{_reader.NString(4)}]", _reader.NString(7),
                                   _reader.GetString(8)));*/
            }

            // Memory optimization: Removed manual CloseAsync() - await using handles disposal automatically

            return Ok(_documents);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error in DeleteRequisitionDocument {ExceptionMessage}", ex.Message);
            return StatusCode(500, ex.Message);
        }
    }

    // Memory optimization: Centralized query execution helpers to eliminate connection leaks and code duplication
    private async Task<ActionResult<string>> ExecuteQueryAsync(string procedureName, Action<SqlCommand> parameterBinder, string logContext, string errorMessage)
    {
        await using SqlConnection _connection = new(Start.ConnectionString);
        await using SqlCommand _command = new(procedureName, _connection);
        _command.CommandType = CommandType.StoredProcedure;

        parameterBinder(_command);

        string _result = "[]";
        try
        {
            await _connection.OpenAsync();
            _result = (await _command.ExecuteScalarAsync())?.ToString() ?? "[]";
        }
        catch (SqlException ex)
        {
            Log.Error(ex, "Error executing {logContext} query. {ExceptionMessage}", logContext, ex.Message);
            return StatusCode(500, errorMessage);
        }
        // Memory optimization: Removed manual CloseAsync() - await using handles disposal automatically

        return Ok(_result);
    }

    // Memory optimization: ExecuteReaderAsync helper for complex data retrieval operations
    private async Task<ActionResult<T>> ExecuteReaderAsync<T>(string procedureName, Action<SqlCommand> parameterBinder, Func<SqlDataReader, Task<T>> readerProcessor, string logContext,
                                                              string errorMessage)
    {
        await using SqlConnection _connection = new(Start.ConnectionString);
        await using SqlCommand _command = new(procedureName, _connection);
        _command.CommandType = CommandType.StoredProcedure;

        parameterBinder(_command);

        try
        {
            await _connection.OpenAsync();
            await using SqlDataReader _reader = await _command.ExecuteReaderAsync();
            T _result = await readerProcessor(_reader);
            return Ok(_result);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error executing {logContext} query. {ExceptionMessage}", logContext, ex.Message);
            return StatusCode(500, errorMessage);
        }
        // Memory optimization: Removed manual CloseAsync() - await using handles disposal automatically
    }

    // Memory optimization: ExecuteScalarAsync helper for single value operations
    private async Task<ActionResult<T>> ExecuteScalarAsync<T>(string procedureName, Action<SqlCommand> parameterBinder, string logContext, string errorMessage)
    {
        await using SqlConnection _connection = new(Start.ConnectionString);
        await using SqlCommand _command = new(procedureName, _connection);
        _command.CommandType = CommandType.StoredProcedure;

        parameterBinder(_command);

        try
        {
            await _connection.OpenAsync();
            object _result = await _command.ExecuteScalarAsync();
            return Ok((T)Convert.ChangeType(_result ?? default(T), typeof(T)));
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error executing {logContext} query. {ExceptionMessage}", logContext, ex.Message);
            return StatusCode(500, errorMessage);
        }
        // Memory optimization: Removed manual CloseAsync() - await using handles disposal automatically
    }

    // Memory optimization: Efficient string building for location generation
    private static string GenerateLocation(RequisitionDetails requisition, string stateName)
    {
        List<string> _parts = [];

        if (requisition.City.NotNullOrWhiteSpace())
        {
            _parts.Add(requisition.City);
        }

        if (stateName.NotNullOrWhiteSpace())
        {
            _parts.Add(stateName);
        }

        if (requisition.ZipCode.NotNullOrWhiteSpace())
        {
            _parts.Add(requisition.ZipCode);
        }

        return string.Join(", ", _parts);
    }

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

        // string _companies = "[]";
        // string _companyContacts = "[]";
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
                // Memory optimization: Removed manual CloseAsync() - await using handles disposal automatically

                // Memory optimization: Removed manual CloseAsync() - await using handles disposal automatically

                return new ReturnGridRequisition {Page = _page};
            }

            int _count = _reader.NInt32(0);

            await _reader.NextResultAsync();

            // Memory optimization: Replace while loop with if statement for single record
            if (await _reader.ReadAsync())
            {
                _requisitions = _reader.NString(0);
            }

            await _reader.NextResultAsync();
            if (getCompanyInformation)
            {
                // Memory optimization: Replace while loop with if statement for single record
                if (await _reader.ReadAsync())
                {
                    _statusCount = _reader.NString(0);
                }
            }

            await _reader.NextResultAsync();
            _page = reqSearch.Page;
            // Memory optimization: Replace while loop with if statement for single record
            if (await _reader.ReadAsync())
            {
                _page = _reader.GetInt32(0);
            }

            // Memory optimization: Removed manual CloseAsync() - await using handles disposal automatically

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
    public async Task<ActionResult<ReturnRequisitionDetails>> GetRequisitionDetails([FromQuery] int requisitionID, [FromQuery] string roleID = "RC")
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

    [HttpPost]
    public async Task<ActionResult<string>> SaveNotes(CandidateNotes candidateNote, int requisitionID, string user)
    {
        if (candidateNote == null)
        {
            return Ok("[]");
        }

        return await ExecuteQueryAsync("SaveNote", command =>
                                                   {
                                                       command.Int("Id", candidateNote.ID);
                                                       command.Int("CandidateID", requisitionID);
                                                       command.Varchar("Note", -1, candidateNote.Notes);
                                                       command.Varchar("EntityType", 5, "REQ");
                                                       command.Varchar("User", 10, user);
                                                   }, "SaveNotes", "Error saving notes.");
    }

    [HttpPost]
    public async Task<ActionResult<int>> SaveRequisition(RequisitionDetails requisition, [FromQuery] string user, [FromQuery] string jsonPath = "",
                                                         [FromQuery]
                                                         string emailAddress = "maniv@titan-techs.com")
    {
        await using SqlConnection _connection = new(Start.ConnectionString);
        await using SqlCommand _command = new("SaveRequisition", _connection);
        _command.CommandType = CommandType.StoredProcedure;
        _command.Int("RequisitionId", requisition.RequisitionID, true);
        _command.Int("Company", requisition.CompanyID);
        _command.Int("HiringMgr", requisition.ContactID);
        _command.Varchar("City", 50, requisition.City);
        _command.Int("StateId", requisition.StateID);
        _command.Varchar("Zip", 10, requisition.ZipCode);
        _command.TinyInt("IsHot", requisition.PriorityID);
        _command.Varchar("Title", 200, requisition.PositionTitle);
        _command.Varchar("Description", -1, requisition.Description);
        _command.Int("Positions", requisition.Positions);
        _command.DateTime("ExpStart", requisition.ExpectedStart);
        _command.DateTime("Due", requisition.DueDate);
        _command.Int("Education", requisition.EducationID);
        _command.Varchar("Skills", 2000, requisition.SkillsRequired);
        _command.Varchar("OptionalRequirement", 8000, requisition.Optional);
        _command.Char("JobOption", 1, requisition.JobOptionID);
        _command.Int("ExperienceID", requisition.ExperienceID);
        _command.Int("Eligibility", requisition.EligibilityID);
        _command.Varchar("Duration", 50, requisition.Duration);
        _command.Char("DurationCode", 1, requisition.DurationCode);
        _command.Decimal("ExpRateLow", 9, 2, requisition.ExpRateLow);
        _command.Decimal("ExpRateHigh", 9, 2, requisition.ExpRateHigh);
        _command.Decimal("ExpLoadLow", 9, 2, requisition.ExpLoadLow);
        _command.Decimal("ExpLoadHigh", 9, 2, requisition.ExpLoadHigh);
        _command.Decimal("SalLow", 9, 2, requisition.SalaryLow);
        _command.Decimal("SalHigh", 9, 2, requisition.SalaryHigh);
        _command.Bit("ExpPaid", requisition.ExpensesPaid);
        _command.Char("Status", 3, requisition.StatusCode);
        _command.Bit("Security", requisition.SecurityClearance);
        _command.Decimal("PlacementFee", 8, 2, requisition.PlacementFee);
        _command.Varchar("BenefitsNotes", -1, requisition.BenefitNotes);
        _command.Bit("OFCCP", requisition.OFCCP);
        _command.Varchar("User", 10, user);
        _command.Varchar("Assign", 550, requisition.AssignedTo);
        _command.Varchar("MandatoryRequirement", 8000, requisition.Mandatory);
        try
        {
            string _reqCode = "";

            await _connection.OpenAsync();
            await using SqlDataReader _reader = await _command.ExecuteReaderAsync();

            // Memory optimization: Replace while loop with if statement for single record
            if (await _reader.ReadAsync())
            {
                _reqCode = _reader.NString(0);
            }

            await _reader.NextResultAsync();
            List<EmailTemplates> _templates = [];
            Dictionary<string, string> _emailAddresses = new(), _emailCC = new();

            while (await _reader.ReadAsync())
            {
                _templates.Add(new(_reader.NString(0), _reader.NString(1), _reader.NString(2)));
            }

            await _reader.NextResultAsync();
            while (await _reader.ReadAsync())
            {
                _emailAddresses.Add(_reader.NString(0), _reader.NString(1));
            }

            await _reader.NextResultAsync();
            string _stateName = "";
            // Memory optimization: Replace while loop with if statement for single record
            if (await _reader.ReadAsync())
            {
                _stateName = _reader.GetString(0);
            }

            // Memory optimization: Removed manual CloseAsync() - await using handles disposal automatically

            if (_templates.Count <= 0)
            {
                return Ok(0);
            }

            EmailTemplates _templateSingle = _templates[0];
            if (!_templateSingle.CC.NullOrWhiteSpace())
            {
                string[] _ccArray = _templateSingle.CC.Split(",");
                foreach (string _cc in _ccArray)
                {
                    _emailCC.Add(_cc, _cc);
                }
            }

            _templateSingle.Subject = _templateSingle.Subject.Replace("$TODAY$", DateTime.Today.CultureDate())
                                                     .Replace("$REQ_ID$", _reqCode)
                                                     .Replace("$REQ_TITLE$", requisition.PositionTitle)
                                                     .Replace("$COMPANY$", requisition.CompanyName)
                                                     .Replace("$DESCRIPTION$", requisition.Description)
                                                     .Replace("$LOCATION$", GenerateLocation(requisition, _stateName))
                                                     .Replace("$LOGGED_USER$", user);
            _templateSingle.Template = _templateSingle.Template.Replace("$TODAY$", DateTime.Today.CultureDate())
                                                      .Replace("$REQ_ID$", _reqCode)
                                                      .Replace("$REQ_TITLE$", requisition.PositionTitle)
                                                      .Replace("$COMPANY$", requisition.CompanyName)
                                                      .Replace("$DESCRIPTION$", requisition.Description)
                                                      .Replace("$LOCATION$", GenerateLocation(requisition, _stateName))
                                                      .Replace("$LOGGED_USER$", user);

            /*GMailSend _send = new();
                GMailSend.SendEmail(jsonPath, emailAddress, _emailCC, _emailAddresses, _templateSingle.Subject, _templateSingle.Template, null);*/
            // Memory optimization: Use injected SmtpClient instead of creating new instances

            MailMessage _mailMessage = new()
                                       {
                                           From = new("maniv@hire-titan.com", "Mani-Meow"),
                                           Subject = _templateSingle.Subject,
                                           Body = _templateSingle.Template,
                                           IsBodyHtml = true
                                       };
            _mailMessage.To.Add("manivenkit@gmail.com"); //TODO: After testing remove this and enable the below code
            // _mailMessage.To.Add("jolly@hire-titan.com");
            /*foreach (KeyValues _emailAddress in _emailAddresses)
            {
                _mailMessage.To.Add(new MailAddress(_emailAddress.KeyValue, _emailAddress.Text));
            }
            foreach (KeyValues _cc in _emailCC)
            {
                _mailMessage.CC.Add(new MailAddress(_cc.KeyValue, _cc.Text));
            }*/

            await smtpClient.SendMailAsync(_mailMessage);
            return Ok(0);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error saving requisition. {ExceptionMessage}", ex.Message);
            return StatusCode(500, ex.Message);
        }
    }

    [HttpGet]
    public async Task<ActionResult<string>> SearchRequisitions(string filter)
    {
        return await ExecuteQueryAsync("SearchRequisitions", command => { command.Varchar("Requisition", 30, filter); }, "SearchRequisitions", "Error searching requisitions.");
    }

    [HttpPost, RequestSizeLimit(62914560)]
    public async Task<ActionResult<string>> UploadDocument(IFormFile file)
    {
        string _fileName = file.FileName; //Request.Form["filename"];
        string _requisitionID = Request.Form["requisitionID"].ToString();
        string _internalFileName = Guid.NewGuid().ToString("N");

        // Create a BlobStorage instance
        IAzureBlobStorage _storage = StorageFactory.Blobs.AzureBlobStorageWithSharedKey(Start.AccountName, Start.AzureKey);

        // Create the folder path
        string _blobPath = $"{Start.AzureBlobContainer}/Requisition/{_requisitionID}/{_internalFileName}";

        await using (Stream stream = file.OpenReadStream())
        {
            await _storage.WriteAsync(_blobPath, stream);
        }

        await using SqlConnection _connection = new(Start.ConnectionString);
        List<RequisitionDocuments> _documents = new();
        try
        {
            string _returnVal = "[]";

            await using SqlCommand _command = new("SaveRequisitionDocuments", _connection);
            _command.CommandType = CommandType.StoredProcedure;
            _command.Int("RequisitionId", _requisitionID);
            _command.Varchar("DocumentName", 255, Request.Form["name"].ToString());
            _command.Varchar("DocumentLocation", 255, _fileName);
            _command.Varchar("DocumentNotes", 2000, Request.Form["notes"].ToString());
            _command.Varchar("InternalFileName", 50, _internalFileName);
            _command.Varchar("DocsUser", 10, Request.Form["user"].ToString());
            await _connection.OpenAsync();
            await using SqlDataReader _reader = await _command.ExecuteReaderAsync();
            while (_reader.Read())
            {
                _returnVal = _reader.NString(0);
            }

            // Memory optimization: Removed manual CloseAsync() - await using handles disposal automatically

            return Ok(_returnVal);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error saving requisition document. {ExceptionMessage}", ex.Message);
            return StatusCode(500, ex.Message);
        }
    }
}