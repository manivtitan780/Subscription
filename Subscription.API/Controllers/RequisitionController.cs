#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.API
// File Name:           RequisitionController.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu, Brijesh Dubey
// Created On:          02-06-2025 19:02
// Last Updated On:     05-14-2025 19:51
// *****************************************/

#endregion

namespace Subscription.API.Controllers;

[ApiController, Route("api/[controller]/[action]"), SuppressMessage("ReSharper", "UnusedMember.Local")]
public class RequisitionController : ControllerBase
{
    /// <summary>
    ///     Deletes a requisition document from the database.
    /// </summary>
    /// <param name="documentID">
    ///     The ID of the document to be deleted.
    /// </param>
    /// <param name="user">
    ///     The user who is performing the delete operation.
    /// </param>
    /// <returns>
    ///     A dictionary containing the details of the deleted document.
    /// </returns>
    /// <description>
    ///     The code first creates a SqlCommand object named _command with the name of the stored procedure to be executed, which is "DeleteRequisitionDocuments", and
    ///     the SQL connection object _connection. The CommandType property of _command is set to CommandType.StoredProcedure, indicating that the command text is the
    ///     name of a stored procedure.
    ///     Next, two parameters are added to _command using extension methods Int and Varchar defined in the Extensions class. The Int method adds an integer
    ///     parameter named "RequisitionDocId" with the value of documentID. The Varchar method adds a string parameter named "User" with a maximum size of 10
    ///     characters and the value of user.
    ///     Then, the code executes the command with _command.ExecuteReaderAsync(), which sends the SqlCommand to the SqlConnection and builds a SqlDataReader. The
    ///     SqlDataReader provides a way of reading a forward-only stream of rows from a SQL Server database. The NextResult method is called to advance the data
    ///     reader to the next result, when multiple result sets are returned.
    ///     The code then checks if the data reader has any rows using the HasRows property. If it does, it enters a while loop that continues as long as there are
    ///     more rows to read with the Read method.
    ///     Inside the loop, a new RequisitionDocuments object is created with data from the current row and added to the _documents list.
    ///     After all rows have been read, the data reader is closed with CloseAsync.
    ///     If any exceptions occur during the execution of the try block, the catch block is executed. In this case, the catch block is empty, so no action is taken
    ///     when an exception occurs.
    ///     Finally, the method returns a new dictionary containing a single key-value pair. The key is "Document" and the value is the _documents list.
    /// </description>
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
            while (_reader.Read())
            {
                _documents = _reader.NString(0);
                /*_documents.Add(new(_reader.GetInt32(0), _reader.GetInt32(1), _reader.NString(2), _reader.NString(3),
                                   _reader.NString(6), $"{_reader.NDateTime(5)} [{_reader.NString(4)}]", _reader.NString(7),
                                   _reader.GetString(8)));*/
            }

            await _reader.CloseAsync();

            return Ok(_documents);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error in DeleteRequisitionDocument {ExceptionMessage}", ex.Message);
            return StatusCode(500, ex.Message);
        }
        finally
        {
            await _connection.CloseAsync();
        }
    }

    /// <summary>
    ///     Generates a location string based on the provided requisition details and state name.
    /// </summary>
    /// <param name="requisition">An instance of the RequisitionDetails class containing the city and zip code.</param>
    /// <param name="stateName">The name of the state.</param>
    /// <returns>
    ///     A string representing the location, in the format of "City, State, ZipCode". If any part is not available, it
    ///     will be omitted from the string.
    /// </returns>
    private static string GenerateLocation(RequisitionDetails requisition, string stateName)
    {
        string _location = "";
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
                await _reader.CloseAsync();

                await _connection.CloseAsync();

                return new ReturnGridRequisition {Page = _page};
            }

            int _count = _reader.NInt32(0);

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
                    _statusCount = _reader.NString(0);
                }
            }

            await _reader.NextResultAsync();
            _page = reqSearch.Page;
            while (await _reader.ReadAsync())
            {
                _page = _reader.GetInt32(0);
            }

            await _reader.CloseAsync();

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
        finally
        {
            await _connection.CloseAsync();
        }
    }

    [HttpPost]
    public async Task<ActionResult<string>> ChangeRequisitionStatus(int requisitionID, string statusCode, string user)
    {
        await using SqlConnection _connection = new(Start.ConnectionString);
        await using SqlCommand _command = new("ChangeRequisitionStatus", _connection);
        _command.CommandType = CommandType.StoredProcedure;
        _command.Int("RequisitionID", requisitionID);
        _command.Char("Status", 3, statusCode);
        _command.Varchar("User", 10, user);
        try
        {
            await _connection.OpenAsync();
            string _status = (await _command.ExecuteScalarAsync())?.ToString();
            await _connection.CloseAsync();

            return Ok(_status);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error changing requisition status. {ExceptionMessage}", ex.Message);
            return StatusCode(500, ex.Message);
        }
        finally
        {
            await _connection.CloseAsync();
        }
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

    /// <summary>
    ///     Asynchronously retrieves the details of a specific requisition.
    /// </summary>
    /// <param name="requisitionID">The ID of the requisition to retrieve details for.</param>
    /// <param name="roleID">The ID of the role. Default value is "RC".</param>
    /// <returns>A dictionary containing the requisition details, activity, and documents related to the specified requisition.</returns>
    [HttpGet]
    public async Task<ActionResult<ReturnRequisitionDetails>> GetRequisitionDetails([FromQuery] int requisitionID, [FromQuery] string roleID = "RC")
    {
        await using SqlConnection _connection = new(Start.ConnectionString);
        if (requisitionID == 0)
        {
            return StatusCode(500, "Requisition ID is not provided.");
        }

        await using SqlCommand _command = new("GetRequisitionDetails", _connection);
        _command.CommandType = CommandType.StoredProcedure;
        _command.Int("RequisitionID", requisitionID);
        _command.Varchar("RoleID", 2, roleID);
        try
        {
            string _requisitionDetail = "{}", _activity = "[]", _documents = "[]", _notes = "[]";
            await _connection.OpenAsync();
            await using SqlDataReader _reader = await _command.ExecuteReaderAsync();
            while (await _reader.ReadAsync())
            {
                _requisitionDetail = _reader.NString(0, "{}");
            }

            await _reader.NextResultAsync(); //Activity
            while (await _reader.ReadAsync())
            {
                _activity = _reader.NString(0, "[]");
            }

            await _reader.NextResultAsync();
            while (await _reader.ReadAsync())
            {
                _documents = _reader.NString(0, "[]");
            }

            await _reader.NextResultAsync();
            while (await _reader.ReadAsync())
            {
                _notes = _reader.NString(0, "[]");
            }

            await _reader.CloseAsync();

            return Ok(new ReturnRequisitionDetails
                      {
                          Activity = _activity,
                          Documents = _documents,
                          Requisition = _requisitionDetail,
                          Notes = _notes
                      });
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error fetching requisition details. {ExceptionMessage}", ex.Message);
            return StatusCode(500, ex.Message);
        }
        finally
        {
            await _connection.CloseAsync();
        }
    }

    [HttpPost]
    public async Task<ActionResult<string>> SaveNotes(CandidateNotes candidateNote, int requisitionID, string user)
    {
        if (candidateNote == null)
        {
            return Ok("[]");
        }

        await using SqlConnection _connection = new(Start.ConnectionString);
        await using SqlCommand _command = new("SaveNote", _connection);
        _command.CommandType = CommandType.StoredProcedure;
        _command.Int("Id", candidateNote.ID);
        _command.Int("CandidateID", requisitionID);
        _command.Varchar("Note", -1, candidateNote.Notes);
        _command.Varchar("EntityType", 5, "REQ");
        _command.Varchar("User", 10, user);
        try
        {
            string _returnVal = "[]";
            await _connection.OpenAsync();
            _returnVal = (await _command.ExecuteScalarAsync())?.ToString();
            return Ok(_returnVal);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error saving notes. {ExceptionMessage}", ex.Message);
            return StatusCode(500, ex.Message);
        }
        finally
        {
            await _connection.CloseAsync();
        }
    }

    /// <summary>
    ///     Saves a requisition to the database.
    /// </summary>
    /// <param name="requisition">The details of the requisition to be saved.</param>
    /// <param name="user">The user who is performing the save operation.</param>
    /// <param name="jsonPath">The path to the JSON file containing the requisition details.</param>
    /// <param name="emailAddress">The email address to which notifications will be sent. Defaults to "maniv@titan-techs.com".</param>
    /// <returns>
    ///     An integer indicating the status of the save operation. Returns -1 if the requisition is null, and 0
    ///     otherwise.
    /// </returns>
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

            while (await _reader.ReadAsync())
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
            while (await _reader.ReadAsync())
            {
                _stateName = _reader.GetString(0);
            }

            await _reader.CloseAsync();

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
            using SmtpClient _smtpClient = new(Start.EmailHost, Start.Port);
            _smtpClient.Credentials = new NetworkCredential(Start.EmailUsername, Start.EmailPassword);
            _smtpClient.EnableSsl = true;

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

            await _smtpClient.SendMailAsync(_mailMessage);
            return Ok(0);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error saving requisition. {ExceptionMessage}", ex.Message);
            return StatusCode(500, ex.Message);
        }
        finally
        {
            await _connection.CloseAsync();
        }
    }

    [HttpGet]
    public async Task<ActionResult<string>> SearchRequisitions(string filter)
    {
        await using SqlConnection _connection = new(Start.ConnectionString);
        await using SqlCommand _command = new("SearchRequisitions", _connection);
        _command.CommandType = CommandType.StoredProcedure;
        _command.Varchar("Requisition", 30, filter);

        try
        {
            await _connection.OpenAsync();

            string _requisitions = (await _command.ExecuteScalarAsync())?.ToString();

            return Ok(_requisitions);
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
    }

    /// <summary>
    ///     Asynchronously uploads a document to a specific requisition.
    /// </summary>
    /// <param name="file">The file to be uploaded as an IFormFile.</param>
    /// <returns>A dictionary containing the details of the uploaded document.</returns>
    /// <remarks>
    ///     This method receives the file to be uploaded as an IFormFile. It also retrieves additional information such as the
    ///     requisition ID, the filename, and other details from the form data of the request. The file is saved in a specific
    ///     directory structure and its details are stored in the database using a stored procedure.
    /// </remarks>
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

            await _reader.CloseAsync();

            return Ok(_returnVal);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error saving requisition document. {ExceptionMessage}", ex.Message);
            return StatusCode(500, ex.Message);
        }
    }
}