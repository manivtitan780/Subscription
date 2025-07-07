#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.API
// File Name:           CandidateController.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu, Brijesh Dubey
// Created On:          02-06-2025 16:02
// Last Updated On:     07-07-2025 16:07
// *****************************************/

#endregion

#region Using

using System.Text.Json;

using RestSharp;

#endregion

namespace Subscription.API.Controllers;

[ApiController, Route("api/[controller]/[action]")]
public class CandidateController(SmtpClient smtpClient) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<string>> ChangeStatus(int candidateID, string user)
    {
        if (candidateID == 0)
        {
            return Ok("[]");
        }

        return await ExecuteQueryAsync("ChangeCandidateStatus", command =>
                                                                {
                                                                    command.Int("CandidateID", candidateID);
                                                                    command.Varchar("User", 10, user);
                                                                }, "ChangeCandidateStatus", "Error changing candidate status.");
    }

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

        /*await using SqlConnection _connection = new(Start.ConnectionString);
        await using SqlCommand _command = new("DeleteCandidateDocument", _connection);
        _command.CommandType = CommandType.StoredProcedure;
        _command.Int("CandidateDocumentId", documentID);
        _command.Varchar("User", 10, user);
        try
        {
            await _connection.OpenAsync();
            _documents = (await _command.ExecuteScalarAsync())?.ToString();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error deleting candidate document. {ExceptionMessage}", ex.Message);
            return StatusCode(500, ex.Message);
        }
        finally
        {
            await _connection.CloseAsync();
        }

        return Ok(_documents);*/
    }

    [HttpPost]
    public async Task<ActionResult<string>> DeleteEducation(int id, int candidateID, string user)
    {
        // await Task.Delay(1);
        //string _education = "[]";
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

        /*await using SqlConnection _connection = new(Start.ConnectionString);
        await using SqlCommand _command = new("DeleteCandidateEducation", _connection);
        _command.CommandType = CommandType.StoredProcedure;
        _command.Int("Id", id);
        _command.Int("candidateId", candidateID);
        _command.Varchar("User", 10, user);
        try
        {
            await _connection.OpenAsync();
            _education = (await _command.ExecuteScalarAsync())?.ToString();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error deleting education. {ExceptionMessage}", ex.Message);
            return StatusCode(500, ex.Message);
        }
        finally
        {
            await _connection.CloseAsync();
        }

        return Ok(_education);*/
    }

    [HttpPost]
    public async Task<ActionResult<string>> DeleteExperience(int id, int candidateID, string user)
    {
        // await Task.Delay(1);
        // string _experiences = "[]";
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

        /*await using SqlConnection _connection = new(Start.ConnectionString);
        await using SqlCommand _command = new("DeleteCandidateExperience", _connection);
        _command.CommandType = CommandType.StoredProcedure;
        _command.Int("Id", id);
        _command.Int("candidateId", candidateID);
        _command.Varchar("User", 10, user);
        try
        {
            await _connection.OpenAsync();
            _experiences = (await _command.ExecuteScalarAsync())?.ToString();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error deleting experience. {ExceptionMessage}", ex.Message);
            return StatusCode(500, ex.Message);
        }
        finally
        {
            await _connection.CloseAsync();
        }

        return Ok(_experiences);*/
    }

    [HttpPost]
    public async Task<ActionResult<string>> DeleteNotes(int id, int candidateID, string user)
    {
        //string _notes = "[]";
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

        /*await using SqlConnection _connection = new(Start.ConnectionString);
        await using SqlCommand _command = new("DeleteCandidateNotes", _connection);
        _command.CommandType = CommandType.StoredProcedure;
        _command.Int("Id", id);
        _command.Int("candidateId", candidateID);
        _command.Varchar("User", 10, user);
        try
        {
            await _connection.OpenAsync();
            _notes = (await _command.ExecuteScalarAsync())?.ToString();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error deleting notes. {ExceptionMessage}", ex.Message);
            return StatusCode(500, ex.Message);
        }
        finally
        {
            await _connection.CloseAsync();
        }

        return Ok(_notes);*/
    }

    [HttpPost]
    public async Task<ActionResult<string>> DeleteSkill(int id, int candidateID, string user)
    {
        //string _skills = "[]";
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

        /*await using SqlConnection _connection = new(Start.ConnectionString);
        await using SqlCommand _command = new("DeleteCandidateSkill", _connection);
        _command.CommandType = CommandType.StoredProcedure;
        _command.Int("Id", id);
        _command.Int("candidateId", candidateID);
        _command.Varchar("User", 10, user);
        try
        {
            await _connection.OpenAsync();
            _skills = (await _command.ExecuteScalarAsync())?.ToString();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error deleting skill. {ExceptionMessage}", ex.Message);
            return StatusCode(500, ex.Message);
        }
        finally
        {
            await _connection.CloseAsync();
        }

        return Ok(_skills);*/
    }

    [HttpGet]
    public async Task<ActionResult<string>> DownloadFile(int documentID)
    {
        if (documentID == 0)
        {
            return Ok("[]");
        }

        return await ExecuteQueryAsync("GetCandidateDocumentDetails", command => { command.Int("DocumentID", documentID); }, "DownloadFile", "Error fetching candidate document details.");

        /*await using SqlConnection _connection = new(Start.ConnectionString);
        await using SqlCommand _command = new("GetCandidateDocumentDetails", _connection);
        _command.CommandType = CommandType.StoredProcedure;
        _command.Int("DocumentID", documentID);

        string _documentDetails = "[]";
        try
        {
            await _connection.OpenAsync();
            _documentDetails = (await _command.ExecuteScalarAsync())?.ToString();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error downloading file. {ExceptionMessage}", ex.Message);
            return StatusCode(500, ex.Message);
        }
        finally
        {
            await _connection.CloseAsync();
        }

        return Ok(_documentDetails);*/
    }

    [HttpGet]
    public async Task<ActionResult<string>> DownloadResume(int candidateID, string resumeType)
    {
        if (candidateID == 0)
        {
            return Ok("[]");
        }

        return await ExecuteQueryAsync("DownloadCandidateResume", command =>
                                                                  {
                                                                      command.Int("CandidateID", candidateID);
                                                                      command.Varchar("ResumeType", 20, resumeType);
                                                                  }, "DeleteResume", "Error fetching candidate document details.");

        /*await using SqlConnection _connection = new(Start.ConnectionString);
        await using SqlCommand _command = new("DownloadCandidateResume", _connection);
        _command.CommandType = CommandType.StoredProcedure;
        _command.Int("CandidateID", candidateID);
        _command.Varchar("ResumeType", 20, resumeType);

        string _documentDetails = "[]";
        try
        {
            await _connection.OpenAsync();
            _documentDetails = (await _command.ExecuteScalarAsync())?.ToString();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error downloading resume. {ExceptionMessage}", ex.Message);
            return StatusCode(500, ex.Message);
        }
        finally
        {
            await _connection.CloseAsync();
        }

        return Ok(_documentDetails);*/
    }

    [HttpPost]
    public async Task<ActionResult<string>> DuplicateCandidate(int candidateID, string user)
    {
        if (candidateID == 0)
        {
            return Ok("[]");
        }

        ActionResult<string> _returnValue = await ExecuteQueryAsync("DuplicateCandidate", command =>
                                                                                          {
                                                                                              command.Int("CandidateID", candidateID);
                                                                                              command.Varchar("User", 10, user);
                                                                                          }, "DuplicateCandidate", "Error duplicating candidate.");
        int _duplicateCandidateID = _returnValue.Value.ToInt32();

        await General.CopyBlobs(candidateID.ToString(), _duplicateCandidateID.ToString()).ConfigureAwait(false); //TODO: Test this out thoroughly.
        return Ok(_duplicateCandidateID);

        /*await using SqlConnection _connection = new(Start.ConnectionString);
        await using SqlCommand _command = new("DuplicateCandidate", _connection);
        _command.CommandType = CommandType.StoredProcedure;
        _command.Int("CandidateID", candidateID);
        _command.Varchar("User", 10, user);

        try
        {
            await _connection.OpenAsync().ConfigureAwait(false);
            object _duplicateCandidate = await _command.ExecuteScalarAsync().ConfigureAwait(false);
            if (_duplicateCandidate != null)
            {
                _duplicateCandidateID = Convert.ToInt32(_duplicateCandidate);
            }

            await General.CopyBlobs(candidateID.ToString(), _duplicateCandidateID.ToString()).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error duplicating candidate. {ExceptionMessage}", ex.Message);
            return StatusCode(500, ex.Message);
        }
        finally
        {
            await _connection.CloseAsync().ConfigureAwait(false);
        }

        return Ok(_duplicateCandidateID);*/
    }

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
        finally
        {
            await _connection.CloseAsync();
        }

        return Ok(_result);
    }

    [HttpGet]
    public async Task<ActionResult<string>> GenerateSummary(int candidateID, int requisitionID)
    {
        await using SqlConnection _connection = new(Start.ConnectionString);
        await using SqlCommand _command = new("GetCandidateRequisitionDescription", _connection);
        _command.CommandType = CommandType.StoredProcedure;
        _command.Int("CandidateID", candidateID);
        _command.Int("RequisitionID", requisitionID);

        string _textResume = "", _jobDescription = "";
        try
        {
            await _connection.OpenAsync();
            await using SqlDataReader _reader = await _command.ExecuteReaderAsync();

            while (await _reader.ReadAsync())
            {
                _textResume = _reader.NString(0);
            }

            await _reader.NextResultAsync();
            while (await _reader.ReadAsync())
            {
                _jobDescription = _reader.NString(0);
            }

            await _reader.CloseAsync();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error getting candidate summary. {ExceptionMessage}", ex.Message);
            return StatusCode(500, ex.Message);
        }
        finally
        {
            await _connection.CloseAsync();
        }

        RestClient client = new(Start.AzureOpenAIEndpoint);
        RestRequest request = new("", Method.Post);
        request.AddHeader("Content-Type", "application/json");
        request.AddHeader("api-key", Start.AzureOpenAIKey);
        var requestBody = new
                          {
                              messages = new[]
                                         {
                                             new {role = "system", content = "You are a concise summary generator."},
                                             new {role = "user", content = string.Format(Start.Prompt, _textResume, _jobDescription)}
                                         },
                              temperature = 0.3,
                              max_tokens = 1000
                          };

        request.AddJsonBody(requestBody);

        RestResponse response = await client.ExecuteAsync(request);

        if (!response.IsSuccessful)
        {
            ApplicationException _ex = new($"Error from Azure OpenAI: {response.StatusCode} - {response.Content}");
            Log.Error(_ex, "Error getting candidate summary. {ExceptionMessage}", _ex.Message);
            return StatusCode(500, _ex.Message);
        }

        using JsonDocument _doc = JsonDocument.Parse(response.Content ?? string.Empty);
        string _content = _doc.RootElement.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString();

        return Ok(_content.NullOrWhiteSpace() ? "" : _content);
    }

    [HttpGet]
    public async Task<ActionResult<ReturnCandidateDetails>> GetCandidateDetails(int candidateID, string roleID)
    {
        await using SqlConnection _connection = new(Start.ConnectionString);
        string _candidate = "";
        string _candRating = "";
        string _candMPC = "";

        await using SqlCommand _command = new("GetDetailCandidate", _connection);
        _command.CommandType = CommandType.StoredProcedure;
        _command.Int("@CandidateID", candidateID);
        _command.Char("@RoleID", 2, roleID);

        try
        {
            await _connection.OpenAsync();
            await using SqlDataReader _reader = await _command.ExecuteReaderAsync();

            while (await _reader.ReadAsync()) //Candidate Details
            {
                _candidate = _reader.NString(0);
                JObject _candidateJson = JObject.Parse(_candidate);

                _candRating = _candidateJson["RateNotes"]?.ToString();
                _candMPC = _candidateJson["MPCNotes"]?.ToString();
            }

            await _reader.NextResultAsync(); //Notes
            string _notes = "[]";
            while (await _reader.ReadAsync())
            {
                _notes = _reader.NString(0);
            }

            await _reader.NextResultAsync(); //Skills
            string _skills = "[]";
            while (await _reader.ReadAsync())
            {
                _skills = _reader.NString(0);
            }

            await _reader.NextResultAsync(); //Education
            string _education = "[]";
            while (await _reader.ReadAsync())
            {
                _education = _reader.NString(0);
            }

            await _reader.NextResultAsync(); //Experience
            string _experience = "[]";
            while (await _reader.ReadAsync())
            {
                _experience = _reader.NString(0);
            }

            await _reader.NextResultAsync(); //Activity
            string _activity = "[]";
            while (await _reader.ReadAsync())
            {
                _activity = _reader.NString(0);
            }

            await _reader.NextResultAsync(); //Managers

            await _reader.NextResultAsync(); //Documents
            string _documents = "[]";
            while (await _reader.ReadAsync())
            {
                _documents = _reader.NString(0);
            }

            await _reader.CloseAsync();

            await _connection.CloseAsync();

            //Candidate Rating
            List<CandidateRating> _rating = [];
            if (_candRating.NotNullOrWhiteSpace())
            {
                _rating = General.DeserializeObject<List<CandidateRating>>(_candRating)!.OrderByDescending(x => x.DateTime).ToList();
            }

            //Candidate MPC
            List<CandidateMPC> _mpc = [];
            if (_candMPC.NotNullOrWhiteSpace())
            {
                _mpc = General.DeserializeObject<List<CandidateMPC>>(_candMPC)!.OrderByDescending(x => x.DateTime).ToList();
            }

            int _ratingFirst = 0;
            bool _mpcFirst = false;
            string _ratingComments = "", _mpcComments = "";
            if (_candRating.NotNullOrWhiteSpace())
            {
                CandidateRating _ratingFirstCandidate = _rating.FirstOrDefault();
                _ratingFirst = _ratingFirstCandidate.Rating;
                _ratingComments = _ratingFirstCandidate.Comment;
            }

            if (_candMPC.NotNullOrWhiteSpace())
            {
                CandidateMPC _mpcFirstCandidate = _mpc.FirstOrDefault();
                _mpcFirst = _mpcFirstCandidate.MPC;
                _mpcComments = _mpcFirstCandidate.Comment;
            }

            CandidateRatingMPC _ratingMPC = new(candidateID, _ratingFirst, _ratingComments, _mpcFirst, _mpcComments);

            return Ok(new ReturnCandidateDetails
                      {
                          Candidate = _candidate,
                          Notes = _notes,
                          Skills = _skills,
                          Education = _education,
                          Experience = _experience,
                          Activity = _activity,
                          Rating = _rating,
                          MPC = _mpc,
                          RatingMPC = _ratingMPC,
                          Documents = _documents
                      });
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error getting candidate details. {ExceptionMessage}", ex.Message);
            return StatusCode(500, ex.Message);
        }
    }

    private static string GetCandidateLocation(CandidateDetails candidateDetails, string stateName)
    {
        //string _location = "";
        List<string> _parts = [];

        if (candidateDetails!.City.NotNullOrWhiteSpace())
        {
            _parts.Add(candidateDetails.City);
            //_location = candidateDetails.City;
        }

        if (stateName.NotNullOrWhiteSpace())
        {
            _parts.Add(stateName);
            //_location += ", " + stateName;
        }
        /*else
        {
            _location = stateName;
        }*/

        if (candidateDetails.ZipCode.NotNullOrWhiteSpace())
        {
            _parts.Add(candidateDetails.ZipCode);
            //_location += ", " + candidateDetails.ZipCode;
        }
        /*else
        {
            _location = candidateDetails.ZipCode;
        }*/

        return string.Join(", ", _parts); //_location;
    }

    [HttpGet]
    public async Task<ActionResult<ReturnGrid>> GetGridCandidates([FromBody] CandidateSearch searchModel = null)
    {
        await using SqlConnection _connection = new(Start.ConnectionString);
        string _candidates = "[]";

        await using SqlCommand _command = new("GetCandidates", _connection);
        _command.CommandType = CommandType.StoredProcedure;
        _command.Int("RecordsPerPage", searchModel!.ItemCount);
        _command.Int("PageNumber", searchModel.Page);
        _command.Int("SortColumn", searchModel.SortField);
        _command.TinyInt("SortDirection", searchModel.SortDirection);
        _command.Varchar("Name", 30, searchModel.Name ?? "");
        //_command.Varchar("Phone", 20, searchModel.Phone);
        //_command.Varchar("Email", 255, searchModel.EmailAddress);
        _command.Bit("MyCandidates", !searchModel.AllCandidates);
        _command.Bit("IncludeAdmin", searchModel.IncludeAdmin);
        _command.Varchar("Keywords", 2000, searchModel.Keywords ?? "");
        _command.Varchar("Skill", 2000, searchModel.Skills ?? "");
        _command.Bit("SearchState", !searchModel.CityZip);
        _command.Varchar("City", 30, searchModel.CityName ?? "");
        _command.Varchar("State", 1000, searchModel.StateID ?? "");
        _command.Int("Proximity", searchModel.Proximity);
        _command.TinyInt("ProximityUnit", searchModel.ProximityUnit);
        _command.Varchar("Eligibility", 10, searchModel.Eligibility);
        _command.Varchar("Reloc", 10, searchModel.Relocate ?? "");
        _command.Varchar("JobOptions", 10, searchModel.JobOptions ?? "");
        //_command.Varchar("Communications",10, searchModel.Communication);
        _command.Varchar("Security", 10, searchModel.SecurityClearance ?? "");
        _command.Varchar("User", 10, searchModel.User ?? "");
        _command.Bit("ActiveRequisitionsOnly", searchModel.ActiveRequisitionsOnly);
        _command.Bit("ShowArchive", searchModel.ShowArchive);
        //_command.Int("OptionalCandidateID", candidateID);
        //_command.Bit("ThenProceed", thenProceed);

        int _count = -1;
        try
        {
            await _connection.OpenAsync();
            await using SqlDataReader _reader = await _command.ExecuteReaderAsync();

            await _reader.ReadAsync();
            _count = _reader.NInt32(0);

            await _reader.NextResultAsync();
            while (await _reader.ReadAsync())
            {
                _candidates = _reader.NString(0, "[]");
            }

            await _reader.CloseAsync();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error getting grid candidates. {ExceptionMessage}", ex.Message);
            return StatusCode(500, ex.Message);
        }
        finally
        {
            await _connection.CloseAsync();
        }

        return Ok(new
                  {
                      Count = _count,
                      Data = _candidates
                  });
    }

    private string GetMimeType(string fileName)
    {
        string extension = Path.GetExtension(fileName).ToLowerInvariant();
        return extension switch
               {
                   ".pdf" => "application/pdf",
                   ".doc" => "application/msword",
                   ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                   ".xls" => "application/vnd.ms-excel",
                   ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                   ".txt" => "text/plain",
                   ".jpg" or ".jpeg" => "image/jpeg",
                   ".png" => "image/png",
                   _ => "application/octet-stream"
               };
    }

    /*public async Task<ActionResult<string>> ParseCandidate(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("No file has been uploaded.");
        }

        string _fileContent = "";
        string _prompt = Start.Prompt;
        using (MemoryStream _stream = new())
        {
            await file.CopyToAsync(_stream);
            _stream.Position = 0;
            using (WordDocument _document = new(_stream, FormatType.Docx))
            {
                // Save the document as a string
                _fileContent = _document.GetText();
            }
        }

        string _detailedPrompt = string.Format(_prompt, _fileContent);

        Uri _endpoint = new(Start.AzureOpenAIEndpoint);
        AzureKeyCredential _credential = new(Start.AzureOpenAIKey);
        AzureOpenAIClient _client = new(_endpoint, _credential);

        ChatClient _chatClient = _client.GetChatClient(Start.DeploymentName); // This points to o3-mini

        ChatCompletionOptions _chatOptions = new()
                                             {
                                                 Temperature = 0.2f,
                                                 MaxOutputTokenCount = 10000,
                                                 TopP = 0.3f,
                                                 FrequencyPenalty = 0f,
                                                 PresencePenalty = 0f
                                             };
        //_chatOptions.Tools.Add(ChatTool.CreateFunctionTool();
        CreateToolFunction(_chatOptions);

        List<ChatMessage> _messages =
        [
            new SystemChatMessage(Start.SystemChatMessage),
            new UserChatMessage(_detailedPrompt)
        ];

        string _parsedJSON = "";
        string _tempJSONFileName = Path.Combine($"{Guid.NewGuid():N}.json");
        try
        {
            ChatCompletion _completeChatAsync = await _chatClient.CompleteChatAsync(_messages, _chatOptions);
            _parsedJSON = _completeChatAsync.Content[0].Text;
            //JsonSerializer.Serialize(_completeChatAsync);

            /* Parse JSON to Objects #1#
            JsonNode _rootNode = JsonNode.Parse(_parsedJSON)!;
            if (_rootNode != null)
            {
                string _firstName = _rootNode["FirstName"]?.ToString() ?? "";
                string _lastName = _rootNode["LastName"]?.ToString() ?? "";
                string _phone = _rootNode["PhoneNumbers"]?[0]?.ToString() ?? "";
                string _email = _rootNode["EmailAddresses"]?[0]?.ToString() ?? "";
                string _street = _rootNode["PostalAddress"]?["Street"]?.ToString() ?? "";
                string _city = _rootNode["PostalAddress"]?["City"]?.ToString() ?? "";
                string _stateName = _rootNode["PostalAddress"]?["State"]?.ToString() ?? "";
                int _stateID = 0;
                if (_stateName.NotNullOrWhiteSpace())
                {
                    RedisService _service = new(Start.CacheServer, Start.CachePort.ToInt32(), Start.Access, false);

                    RedisValue _value = await _service.GetAsync(CacheObjects.States.ToString());
                    List<State> _states = General.DeserializeObject<List<State>>(_value.ToString());
                    foreach (State _state in _states.Where(state => _stateName.Equals(state.Code.Trim(), StringComparison.OrdinalIgnoreCase) ||
                                                                    _stateName.Equals(state.StateName.Trim(), StringComparison.OrdinalIgnoreCase)))
                    {
                        _stateID = _state.ID;
                        break;
                    }
                }

                string _zip = _rootNode["PostalAddress"]?["Zip"]?.ToString() ?? "";
                string _summary = _rootNode["CandidateSummary"]?.ToString() ?? "";
                string _keywords = _rootNode["CandidateKeywords"]?.ToString() ?? "";

                /*Education#1#
                DataTable _tableEducation = Education(_rootNode["EducationInfo"] as JsonArray);

                /*Experience#1#
                DataTable _tableExperience = Experience(_rootNode["EmploymentInfo"] as JsonArray);

                /* Skills #1#
                DataTable _tableSkills = Skills(_rootNode["Skills"] as JsonArray);
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error parsing candidate. {ExceptionMessage}", ex.Message);
            return StatusCode(500, ex.Message);
        }

        return Ok(_parsedJSON);
    }*/

    [HttpPost, SuppressMessage("ReSharper", "CollectionNeverQueried.Local")]
    public async Task<ActionResult<int>> SaveCandidate(CandidateDetails candidateDetails, string userName = "")
    {
        if (candidateDetails == null)
        {
            return NotFound(-1);
        }

        await using SqlConnection _connection = new(Start.ConnectionString);
        await using SqlCommand _command = new("SaveCandidate", _connection);
        _command.CommandType = CommandType.StoredProcedure;
        _command.Int("@ID", candidateDetails.CandidateID, true);
        _command.Varchar("@FirstName", 50, candidateDetails.FirstName);
        _command.Varchar("@MiddleName", 50, candidateDetails.MiddleName);
        _command.Varchar("@LastName", 50, candidateDetails.LastName);
        _command.Varchar("@Title", 50, candidateDetails.Title);
        _command.Int("@Eligibility", candidateDetails.EligibilityID);
        _command.Decimal("@HourlyRate", 6, 2, candidateDetails.HourlyRate);
        _command.Decimal("@HourlyRateHigh", 6, 2, candidateDetails.HourlyRateHigh);
        _command.Decimal("@SalaryLow", 9, 2, candidateDetails.SalaryLow);
        _command.Decimal("@SalaryHigh", 9, 2, candidateDetails.SalaryHigh);
        _command.Int("@Experience", candidateDetails.ExperienceID);
        _command.Bit("@Relocate", candidateDetails.Relocate);
        _command.Varchar("@JobOptions", 50, candidateDetails.JobOptions);
        _command.Char("@Communication", 1, candidateDetails.Communication);
        _command.Varchar("@Keywords", 500, candidateDetails.Keywords);
        _command.Varchar("@Status", 3, "AVL");
        _command.Varchar("@TextResume", -1, candidateDetails.TextResume);
        _command.Varchar("@OriginalResume", 255, candidateDetails.OriginalResume);
        _command.Varchar("@FormattedResume", 255, candidateDetails.FormattedResume);
        _command.UniqueIdentifier("@OriginalFileID", DBNull.Value);
        _command.UniqueIdentifier("@FormattedFileID", DBNull.Value);
        _command.Varchar("@Address1", 255, candidateDetails.Address1);
        _command.Varchar("@Address2", 255, candidateDetails.Address2);
        _command.Varchar("@City", 50, candidateDetails.City);
        _command.Int("@StateID", candidateDetails.StateID);
        _command.Varchar("@ZipCode", 20, candidateDetails.ZipCode);
        _command.Varchar("@Email", 255, candidateDetails.Email);
        _command.Varchar("@Phone1", 15, candidateDetails.Phone1);
        _command.Varchar("@Phone2", 15, candidateDetails.Phone2);
        _command.Varchar("@Phone3", 15, candidateDetails.Phone3);
        _command.SmallInt("@Phone3Ext", candidateDetails.PhoneExt.ToInt16());
        _command.Varchar("@LinkedIn", 255, candidateDetails.LinkedIn);
        _command.Varchar("@Facebook", 255, candidateDetails.Facebook);
        _command.Varchar("@Twitter", 255, candidateDetails.Twitter);
        _command.Varchar("@Google", 255, candidateDetails.GooglePlus);
        _command.Bit("@Refer", candidateDetails.Refer);
        _command.Varchar("@ReferAccountMgr", 10, candidateDetails.ReferAccountManager);
        _command.Varchar("@TaxTerm", 10, candidateDetails.TaxTerm);
        _command.Bit("@Background", candidateDetails.Background);
        _command.Varchar("@Summary", -1, candidateDetails.Summary);
        _command.Varchar("@Objective", -1, "");
        _command.Bit("@EEO", candidateDetails.EEO);
        _command.Varchar("@RelocNotes", 200, candidateDetails.RelocationNotes);
        _command.Varchar("@SecurityClearanceNotes", 200, candidateDetails.SecurityNotes);
        _command.Varchar("@User", 10, userName);

        try
        {
            await _connection.OpenAsync();
            await using SqlDataReader _reader = await _command.ExecuteReaderAsync();

            List<EmailTemplates> _templates = [];
            Dictionary<string, string> _emailAddresses = new();
            Dictionary<string, string> _emailCC = new();

            while (await _reader.ReadAsync())
            {
                _templates.Add(new(_reader.NString(0), _reader.NString(1), _reader.NString(2)));
            }

            await _reader.NextResultAsync();
            while (await _reader.ReadAsync())
            {
                _emailAddresses.Add(_reader.NString(0), _reader.NString(1));
            }

            string _stateName = "";
            await _reader.NextResultAsync();
            while (await _reader.ReadAsync())
            {
                _stateName = _reader.GetString(0);
            }

            await _reader.CloseAsync();

            if (_templates.Count > 0)
            {
                EmailTemplates _templateSingle = _templates[0];
                if (_templateSingle.CC.NotNullOrWhiteSpace())
                {
                    string[] _ccArray = _templateSingle.CC!.Split(",");
                    foreach (string _cc in _ccArray)
                    {
                        _emailCC.Add(_cc, _cc);
                    }
                }

                _templateSingle.Subject = _templateSingle.Subject!.Replace("{TODAY}", DateTime.Today.CultureDate())
                                                         .Replace("{FullName}", $"{candidateDetails.FirstName} {candidateDetails.LastName}")
                                                         .Replace("{FirstName}", candidateDetails.FirstName)
                                                         .Replace("{LastName}", candidateDetails.LastName)
                                                         .Replace("{CandidateLocation}", GetCandidateLocation(candidateDetails, _stateName))
                                                         .Replace("{CandidatePhone}", candidateDetails.Phone1.StripAndFormatPhoneNumber())
                                                         .Replace("{CandidateSummary}", candidateDetails.Summary)
                                                         .Replace("{LoggedInUser}", userName);
                _templateSingle.Template = _templateSingle.Template!.Replace("{TODAY}", DateTime.Today.CultureDate())
                                                          .Replace("{FullName}", $"{candidateDetails.FirstName} {candidateDetails.LastName}")
                                                          .Replace("{FirstName}", candidateDetails.FirstName)
                                                          .Replace("{LastName}", candidateDetails.LastName)
                                                          .Replace("{CandidateLocation}", GetCandidateLocation(candidateDetails, _stateName))
                                                          .Replace("{CandidatePhone}", candidateDetails.Phone1.StripAndFormatPhoneNumber())
                                                          .Replace("{CandidateSummary}", candidateDetails.Summary)
                                                          .Replace("{LoggedInUser}", userName);

                /*SendResponse? _email = await Email.From("maniv@hire-titan.com")
                                                  .To("manivenkit@gmail.com", "Mani Bhai")
                                                  .Subject("Chup chaap accept kar")
                                                  .Body("Bhai ka message aayela hain. Accept karne ka, samjha kya?")
                                                  .SendAsync();*/
                //using SmtpClient _smtpClient = new(Start.EmailHost, Start.Port);
                //_smtpClient.Credentials = new NetworkCredential(Start.EmailUsername, Start.EmailPassword);
                //_smtpClient.EnableSsl = true;

                using MailMessage _mailMessage = new();
                _mailMessage.From = new("jolly@hire-titan.com", "Mani Bhai");
                _mailMessage.Subject = _templateSingle.Subject;
                _mailMessage.Body = _templateSingle.Template;
                _mailMessage.IsBodyHtml = true;
                _mailMessage.To.Add("manivenkit@gmail.com"); //TODO: Use Multiple
                smtpClient.Send(_mailMessage);
                /*GMailSend.SendEmail(jsonPath, emailAddress, _emailCC, _emailAddresses, _templateSingle.Subject, _templateSingle.Template, null);*/
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error saving candidate. {ExceptionMessage}", ex.Message);
            return StatusCode(500, ex.Message);
        }
        finally
        {
            await _connection.CloseAsync();
        }

        return Ok(0);
    }

    [HttpPost]
    public async Task<ActionResult<string>> SaveCandidateActivity(CandidateActivity activity, int candidateID, string user, string roleID = "RS", bool isCandidateScreen = true,
                                                                  string emailAddress = "maniv@titan-techs.com", string uploadPath = "", string jsonPath = "")
    {
        string _activities = "[]";

        await using SqlConnection _connection = new(Start.ConnectionString);
        try
        {
            await using SqlCommand _command = new("SaveCandidateActivity", _connection);
            _command.CommandType = CommandType.StoredProcedure;
            _command.Int("SubmissionId", activity.ID);
            _command.Int("CandidateID", candidateID);
            _command.Int("RequisitionID", activity.RequisitionID);
            _command.Varchar("Notes", 1000, activity.Notes);
            _command.Char("Status", 3, activity.NewStatusCode.NullOrWhiteSpace() ? activity.StatusCode : activity.NewStatusCode);
            _command.Varchar("User", 10, user);
            _command.Bit("ShowCalendar", activity.ShowCalendar);
            _command.Date("DateTime", activity.DateTimeInterview == DateTime.MinValue ? DBNull.Value : activity.DateTimeInterview);
            _command.Char("Type", 1, activity.TypeOfInterview);
            _command.Varchar("PhoneNumber", 20, activity.PhoneNumber);
            _command.Varchar("InterviewDetails", 2000, activity.InterviewDetails);
            _command.Bit("UpdateSchedule", false);
            _command.Bit("CandScreen", isCandidateScreen);
            _command.Char("RoleID", 2, roleID);
            await _connection.OpenAsync();
            await using SqlDataReader _reader = await _command.ExecuteReaderAsync();
            while (await _reader.ReadAsync())
            {
                _activities = _reader.NString(0);
                /*_activities.Add(new(_reader.GetString(0), _reader.GetDateTime(1), _reader.GetString(2), _reader.GetInt32(3), _reader.GetInt32(4),
                                    _reader.GetString(5), _reader.GetString(6), _reader.GetInt32(7), _reader.GetBoolean(8), _reader.GetString(9), _reader.GetString(10),
                                    _reader.GetString(11), _reader.GetBoolean(12), _reader.GetString(13), _reader.GetInt32(14), _reader.GetString(15),
                                    _reader.GetInt32(16), _reader.GetString(17), _reader.GetBoolean(18), _reader.NDateTime(19), _reader.GetString(20),
                                    _reader.NString(21), _reader.NString(22), _reader.GetBoolean(23)));*/
            }

            await _reader.NextResultAsync();
            // string _firstName = "", _lastName = "", _reqCode = "", _reqTitle = "", _company = ""; //, _original = "", _originalInternal = "", _formatted = "", _formattedInternal = "";
            //bool _firstTime = false;
            await _reader.ReadAsync();
            (string _firstName, string _lastName, string _reqCode, string _reqTitle, string _company) = (_reader.NString(0), _reader.NString(1), _reader.NString(2), _reader.NString(3),
                                                                                                         _reader.NString(8));
            /*_firstName = _reader.NString(0);
            _lastName = _reader.NString(1);
            _reqCode = _reader.NString(2);
            _reqTitle = _reader.NString(3);
            //_original = _reader.NString(4);
            //_originalInternal = _reader.NString(5);
            //_formatted = _reader.NString(6);
            //_formattedInternal = _reader.NString(7);
            //_firstTime = _reader.GetBoolean(8);
            _company = _reader.GetString(8);*/

            List<EmailTemplates> _templates = [];
            Dictionary<string, string> _emailAddresses = new();
            Dictionary<string, string> _emailCC = new();

            await _reader.NextResultAsync();
            while (await _reader.ReadAsync())
            {
                _templates.Add(new(_reader.NString(0), _reader.NString(1), _reader.NString(2)));
            }

            await _reader.NextResultAsync();
            while (await _reader.ReadAsync())
            {
                _emailAddresses.Add(_reader.NString(0), _reader.NString(1));
            }

            await _reader.CloseAsync();

            if (_templates.Count != 0)
            {
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
                                                         .Replace("$FULL_NAME$", $"{_firstName} {_lastName}")
                                                         .Replace("$FIRST_NAME$", _firstName)
                                                         .Replace("$LAST_NAME$", _lastName)
                                                         .Replace("$REQ_ID$", _reqCode)
                                                         .Replace("$REQ_TITLE$", _reqTitle)
                                                         .Replace("$COMPANY$", _company)
                                                         .Replace("$SUBMISSION_NOTES$", activity.Notes)
                                                         .Replace("$SUBMISSION_STATUS$", activity.Status)
                                                         .Replace("$LOGGED_USER$", user);

                _templateSingle.Template = _templateSingle.Template.Replace("$TODAY$", DateTime.Today.CultureDate())
                                                          .Replace("$FULL_NAME$", $"{_firstName} {_lastName}")
                                                          .Replace("$FIRST_NAME$", _firstName)
                                                          .Replace("$LAST_NAME$", _lastName)
                                                          .Replace("$REQ_ID$", _reqCode)
                                                          .Replace("$REQ_TITLE$", _reqTitle)
                                                          .Replace("$COMPANY$", _company)
                                                          .Replace("$SUBMISSION_NOTES$", activity.Notes)
                                                          .Replace("$SUBMISSION_STATUS$", activity.Status)
                                                          .Replace("$LOGGED_USER$", user);

                List<string> _attachments = [];
                //string _pathDest = "";
                //if (_firstTime)
                //{
                //    string _path = "";
                //    if (!_formatted.NullOrWhiteSpace())
                //    {
                //        _path = Path.Combine(uploadPath, "Uploads", "Candidate", candidateID.ToString(), _formattedInternal);
                //        _pathDest = Path.Combine(uploadPath, "Uploads", "Candidate", candidateID.ToString(), _formatted);
                //    }
                //    else
                //    {
                //        _path = Path.Combine(uploadPath, "Uploads", "Candidate", candidateID.ToString(), _originalInternal);
                //        _pathDest = Path.Combine(uploadPath, "Uploads", "Candidate", candidateID.ToString(), _original);
                //    }

                //    if (!_path.NullOrWhiteSpace() && !_pathDest.NullOrWhiteSpace() && System.IO.File.Exists(_path))
                //    {
                //        System.IO.File.Copy(_path, _pathDest, true);
                //        _attachments.Add(_pathDest);
                //    }
                //}

                //GMailSend.SendEmail(jsonPath, emailAddress, _emailCC, _emailAddresses, _templateSingle.Subject, _templateSingle.Template, _attachments);
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error saving candidate activity. {ExceptionMessage}", ex.Message);
            return StatusCode(500, ex.Message);
            //
        }
        finally
        {
            await _connection.CloseAsync();
        }

        return Ok(_activities);
    }

    [HttpPost, SuppressMessage("ReSharper", "CollectionNeverQueried.Local")]
    public async Task<ActionResult<string>> SaveCandidateWithResume(CandidateDetailsResume candidateDetailsResume, string userName = "")
    {
        if (candidateDetailsResume?.CandidateDetails == null || candidateDetailsResume.ParsedCandidate == null)
        {
            return NotFound(-1);
        }

        string _internalFileName = Guid.NewGuid().ToString("N");
        CandidateDetails _candidateDetails = candidateDetailsResume.CandidateDetails;
        await using SqlConnection _connection = new(Start.ConnectionString);
        await using SqlCommand _command = new("SaveCandidateWithSubmissions", _connection);
        _command.CommandType = CommandType.StoredProcedure;
        _command.Int("@ID", _candidateDetails.CandidateID, true);
        _command.Varchar("@FirstName", 50, _candidateDetails.FirstName);
        _command.Varchar("@MiddleName", 50, _candidateDetails.MiddleName);
        _command.Varchar("@LastName", 50, _candidateDetails.LastName);
        _command.Varchar("@Title", 200, _candidateDetails.Title);
        _command.Int("@Eligibility", _candidateDetails.EligibilityID);
        _command.Decimal("@HourlyRate", 6, 2, _candidateDetails.HourlyRate);
        _command.Decimal("@HourlyRateHigh", 6, 2, _candidateDetails.HourlyRateHigh);
        _command.Decimal("@SalaryLow", 9, 2, _candidateDetails.SalaryLow);
        _command.Decimal("@SalaryHigh", 9, 2, _candidateDetails.SalaryHigh);
        _command.Int("@Experience", _candidateDetails.ExperienceID);
        _command.Bit("@Relocate", _candidateDetails.Relocate);
        _command.Varchar("@JobOptions", 50, _candidateDetails.JobOptions);
        _command.Char("@Communication", 1, _candidateDetails.Communication);
        _command.Varchar("@Keywords", 500, _candidateDetails.Keywords.Length > 500 ? _candidateDetails.Keywords[..500] : _candidateDetails.Keywords);
        _command.Varchar("@Status", 3, "AVL");
        _command.Varchar("@TextResume", -1, _candidateDetails.TextResume);
        _command.Varchar("@OriginalResume", 255, candidateDetailsResume.ParsedCandidate.FileName);
        _command.Varchar("@FormattedResume", 255, _candidateDetails.FormattedResume);
        _command.Varchar("@OriginalFileID", 50, _internalFileName);
        _command.Varchar("@FormattedFileID", 50, DBNull.Value);
        _command.Varchar("@Address1", 255, _candidateDetails.Address1);
        _command.Varchar("@Address2", 255, _candidateDetails.Address2);
        _command.Varchar("@City", 50, _candidateDetails.City);
        _command.Int("@StateID", _candidateDetails.StateID);
        _command.Varchar("@ZipCode", 20, _candidateDetails.ZipCode);
        _command.Varchar("@Email", 255, _candidateDetails.Email);
        _command.Varchar("@Phone1", 15, _candidateDetails.Phone1.StripPhoneNumber());
        _command.Varchar("@Phone2", 15, _candidateDetails.Phone2.StripPhoneNumber());
        _command.Varchar("@Phone3", 15, _candidateDetails.Phone3.StripPhoneNumber());
        _command.SmallInt("@Phone3Ext", _candidateDetails.PhoneExt.ToInt16());
        _command.Varchar("@LinkedIn", 255, _candidateDetails.LinkedIn);
        _command.Varchar("@Facebook", 255, _candidateDetails.Facebook);
        _command.Varchar("@Twitter", 255, _candidateDetails.Twitter);
        _command.Varchar("@Google", 255, _candidateDetails.GooglePlus);
        _command.Bit("@Refer", _candidateDetails.Refer);
        _command.Varchar("@ReferAccountMgr", 10, _candidateDetails.ReferAccountManager);
        _command.Varchar("@TaxTerm", 10, _candidateDetails.TaxTerm);
        _command.Bit("@Background", _candidateDetails.Background);
        _command.Varchar("@Summary", -1, _candidateDetails.Summary);
        _command.Varchar("@Objective", -1, "");
        _command.Bit("@EEO", _candidateDetails.EEO);
        _command.Varchar("@RelocNotes", 200, _candidateDetails.RelocationNotes);
        _command.Varchar("@SecurityClearanceNotes", 200, _candidateDetails.SecurityNotes);
        _command.Varchar("@User", 10, userName);
        _command.Int("@RequisitionID", candidateDetailsResume.ParsedCandidate.RequisitionID);
        _command.Varchar("@SubmissionNotes", 1000, candidateDetailsResume.ParsedCandidate.SubmissionNotes);

        try
        {
            await _connection.OpenAsync();
            await using SqlDataReader _reader = await _command.ExecuteReaderAsync();

            List<EmailTemplates> _templates = [];
            List<KeyValues> _emailAddresses = [];
            List<KeyValues> _emailCC = [];

            while (await _reader.ReadAsync())
            {
                _templates.Add(new(_reader.NString(0), _reader.NString(1), _reader.NString(2)));
            }

            await _reader.NextResultAsync();
            while (await _reader.ReadAsync())
            {
                _emailAddresses.Add(new() {KeyValue = _reader.NString(0), Text = _reader.NString(1)});
            }

            string _stateName = "";
            await _reader.NextResultAsync();
            while (await _reader.ReadAsync())
            {
                _stateName = _reader.GetString(0);
            }

            await _reader.NextResultAsync();
            int _candidateID = 0;
            while (await _reader.ReadAsync())
            {
                _candidateID = _reader.NInt32(0);
            }

            await _reader.CloseAsync();
            await General.UploadToBlob(candidateDetailsResume.ParsedCandidate.DocumentBytes, $"{Start.AzureBlobContainer}/Candidate/{_candidateID}/{_internalFileName}");

            if (_templates.Count > 0)
            {
                foreach (EmailTemplates _emailTemplates in _templates)
                {
                    if (_emailTemplates.CC.NotNullOrWhiteSpace())
                    {
                        string[] _ccArray = _emailTemplates.CC!.Split(",");
                        _emailCC.AddRange(_ccArray.Select(cc => new KeyValues {KeyValue = cc, Text = cc}));
                    }

                    _emailTemplates.Subject = _emailTemplates.Subject!.Replace("{TODAY}", DateTime.Today.CultureDate())
                                                             .Replace("{FullName}", $"{_candidateDetails.FirstName} {_candidateDetails.LastName}")
                                                             .Replace("{FirstName}", _candidateDetails.FirstName)
                                                             .Replace("{LastName}", _candidateDetails.LastName)
                                                             .Replace("{CandidateLocation}", GetCandidateLocation(_candidateDetails, _stateName))
                                                             .Replace("{CandidatePhone}", _candidateDetails.Phone1.StripAndFormatPhoneNumber())
                                                             .Replace("{CandidateSummary}", _candidateDetails.Summary)
                                                             .Replace("{LoggedInUser}", userName);

                    _emailTemplates.Template = _emailTemplates.Template!.Replace("{TODAY}", DateTime.Today.CultureDate())
                                                              .Replace("{FullName}", $"{_candidateDetails.FirstName} {_candidateDetails.LastName}")
                                                              .Replace("{FirstName}", _candidateDetails.FirstName)
                                                              .Replace("{LastName}", _candidateDetails.LastName)
                                                              .Replace("{CandidateLocation}", GetCandidateLocation(_candidateDetails, _stateName))
                                                              .Replace("{CandidatePhone}", _candidateDetails.Phone1.StripAndFormatPhoneNumber())
                                                              .Replace("{CandidateSummary}", _candidateDetails.Summary)
                                                              .Replace("{LoggedInUser}", userName);

                    using SmtpClient _smtpClient = new(Start.EmailHost, Start.Port);
                    _smtpClient.Credentials = new NetworkCredential(Start.EmailUsername, Start.EmailPassword);
                    _smtpClient.EnableSsl = true;

                    MailMessage _mailMessage = new()
                                               {
                                                   From = new("maniv@hire-titan.com", "Mani-Meow"),
                                                   Subject = _emailTemplates.Subject,
                                                   Body = _emailTemplates.Template,
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
                    MemoryStream _resumeStream = new(candidateDetailsResume.ParsedCandidate.DocumentBytes);
                    if (_emailTemplates.Subject.Contains("Submitted"))
                    {
                        //Attach the Resume
                        Attachment _resumeAttachment = new(_resumeStream, candidateDetailsResume.ParsedCandidate.FileName, GetMimeType(candidateDetailsResume.ParsedCandidate.Mime));
                        _mailMessage.Attachments.Add(_resumeAttachment);
                    }

                    await _smtpClient.SendMailAsync(_mailMessage);
                    _resumeStream.Close();
                    await _resumeStream.DisposeAsync();
                }
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error saving candidate. {ExceptionMessage}", ex.Message);
            return StatusCode(500, ex.Message);
        }
        finally
        {
            await _connection.CloseAsync();
        }

        return Ok(0);
    }

    [HttpPost]
    public async Task<ActionResult<string>> SaveEducation(CandidateEducation education, int candidateID, string user)
    {
        if (education == null)
        {
            return Ok("[]");
        }

        return await ExecuteQueryAsync("SaveEducation", command =>
                                                        {
                                                            command.Int("Id", education.ID);
                                                            command.Int("CandidateID", candidateID);
                                                            command.Varchar("Degree", 100, education.Degree);
                                                            command.Varchar("College", 255, education.College);
                                                            command.Varchar("State", 100, education.State);
                                                            command.Varchar("Country", 100, education.Country);
                                                            command.Varchar("Year", 10, education.Year);
                                                            command.Varchar("User", 10, user);
                                                        }, "SaveEducation", "Error saving candidate education.");

        /*await using SqlConnection _connection = new(Start.ConnectionString);
        await using SqlCommand _command = new("SaveEducation", _connection);
        _command.CommandType = CommandType.StoredProcedure;
        _command.Int("Id", education.ID);
        _command.Int("CandidateID", candidateID);
        _command.Varchar("Degree", 100, education.Degree);
        _command.Varchar("College", 255, education.College);
        _command.Varchar("State", 100, education.State);
        _command.Varchar("Country", 100, education.Country);
        _command.Varchar("Year", 10, education.Year);
        _command.Varchar("User", 10, user);
        try
        {
            await _connection.OpenAsync();
            _returnVal = (await _command.ExecuteScalarAsync())?.ToString();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error saving education. {ExceptionMessage}", ex.Message);
            return StatusCode(500, ex.Message);
        }
        finally
        {
            await _connection.CloseAsync();
        }

        return Ok(_returnVal);*/
    }

    [HttpPost]
    public async Task<ActionResult<string>> SaveExperience(CandidateExperience experience, int candidateID, string user)
    {
        if (experience == null)
        {
            return Ok("[]");
        }

        return await ExecuteQueryAsync("SaveExperience", command =>
                                                         {
                                                             command.Int("Id", experience.ID);
                                                             command.Int("CandidateID", candidateID);
                                                             command.Varchar("Employer", 100, experience.Employer);
                                                             command.Varchar("Start", 10, experience.Start);
                                                             command.Varchar("End", 10, experience.End);
                                                             command.Varchar("Location", 100, experience.Location);
                                                             command.Varchar("Description", 1000, experience.Description);
                                                             command.Varchar("Title", 1000, experience.Title);
                                                             command.Varchar("User", 10, user);
                                                         }, "SaveExperience", "Error saving candidate experience.");

        /*await using SqlConnection _connection = new(Start.ConnectionString);
        await using SqlCommand _command = new("SaveExperience", _connection);
        _command.CommandType = CommandType.StoredProcedure;
        _command.Int("Id", experience.ID);
        _command.Int("CandidateID", candidateID);
        _command.Varchar("Employer", 100, experience.Employer);
        _command.Varchar("Start", 10, experience.Start);
        _command.Varchar("End", 10, experience.End);
        _command.Varchar("Location", 100, experience.Location);
        _command.Varchar("Description", 1000, experience.Description);
        _command.Varchar("Title", 1000, experience.Title);
        _command.Varchar("User", 10, user);
        try
        {
            await _connection.OpenAsync();
            _returnVal = (await _command.ExecuteScalarAsync())?.ToString();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error saving experience. {ExceptionMessage}", ex.Message);
            return StatusCode(500, ex.Message);
        }
        finally
        {
            await _connection.CloseAsync();
        }

        return Ok(_returnVal);*/
    }

    [HttpPost]
    public async Task<Dictionary<string, object>> SaveMPC(CandidateRatingMPC mpc, string user)
    {
        string _mpc = "[]";
        try
        {
            if (mpc == null)
            {
                return new()
                       {
                           {
                               "MPCList", _mpc
                           },
                           {
                               "FirstMPC", null
                           }
                       };
            }

            await using SqlConnection _connection = new(Start.ConnectionString);
            await _connection.OpenAsync();
            await using SqlCommand _command = new("ChangeMPC", _connection);
            _command.CommandType = CommandType.StoredProcedure;
            _command.Int("@CandidateId", mpc.ID);
            _command.Bit("@MPC", mpc.MPC);
            _command.Varchar("@Notes", -1, mpc.MPCComments);
            _command.Varchar("@From", 10, user);
            string _mpcNotes = (await _command.ExecuteScalarAsync())?.ToString();

            await _connection.CloseAsync();

            bool _mpcFirst = false;
            string _mpcComments = "";
            if (_mpcNotes != null)
            {
                JArray _mpcNotesArray = JArray.Parse(_mpcNotes);

                if (_mpcNotesArray.Any())
                {
                    JArray _mpcSortedArray = new(_mpcNotesArray.OrderByDescending(obj => DateTime.Parse(obj["DateTime"]!.ToString())));

                    JToken _mpcFirstCandidate = _mpcSortedArray.FirstOrDefault();
                    if (_mpcFirstCandidate != null)
                    {
                        _mpcFirst = _mpcFirstCandidate["MPC"].ToBoolean();
                        _mpcComments = _mpcFirstCandidate["Comment"]?.ToString();
                    }

                    _mpc = _mpcSortedArray.ToString();
                }
            }

            mpc.MPC = _mpcFirst;
            mpc.MPCComments = _mpcComments;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error saving MPC. {ExceptionMessage}", ex.Message);
            //
        }

        return new()
               {
                   {
                       "MPCList", _mpc
                   },
                   {
                       "FirstMPC", mpc
                   }
               };
    }

    [HttpPost]
    public async Task<ActionResult<string>> SaveNotes(CandidateNotes candidateNote, int candidateID, string user)
    {
        if (candidateNote == null)
        {
            return Ok("[]");
        }

        return await ExecuteQueryAsync("SaveNote", command =>
                                                   {
                                                       command.Int("Id", candidateNote.ID);
                                                       command.Int("CandidateID", candidateID);
                                                       command.Varchar("Note", -1, candidateNote.Notes);
                                                       command.Bit("IsPrimary", false);
                                                       command.Varchar("EntityType", 5, "CND");
                                                       command.Varchar("User", 10, user);
                                                   }, "SaveNotes", "Error saving candidate notes.");

        /*await using SqlConnection _connection = new(Start.ConnectionString);
        await using SqlCommand _command = new("SaveNote", _connection);
        _command.CommandType = CommandType.StoredProcedure;
        _command.Int("Id", candidateNote.ID);
        _command.Int("CandidateID", candidateID);
        _command.Varchar("Note", -1, candidateNote.Notes);
        _command.Bit("IsPrimary", false);
        _command.Varchar("EntityType", 5, "CND");
        _command.Varchar("User", 10, user);
        try
        {
            await _connection.OpenAsync();
            _returnVal = (await _command.ExecuteScalarAsync())?.ToString();
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

        return Ok(_returnVal);*/
    }

    [HttpPost]
    public async Task<Dictionary<string, object>> SaveRating(CandidateRatingMPC rating, string user)
    {
        string _rating = "[]";
        try
        {
            if (rating == null)
            {
                return new()
                       {
                           {
                               "RatingList", _rating
                           },
                           {
                               "FirstRating", null
                           }
                       };
            }

            await using SqlConnection _connection = new(Start.ConnectionString);
            await _connection.OpenAsync();
            await using SqlCommand _command = new("ChangeRating", _connection);
            _command.CommandType = CommandType.StoredProcedure;
            _command.Int("@CandidateId", rating.ID);
            _command.TinyInt("@Rating", rating.Rating);
            _command.Varchar("@Notes", -1, rating.RatingComments);
            _command.Varchar("@From", 10, user);
            string _ratingNotes = (await _command.ExecuteScalarAsync())?.ToString();

            await _connection.CloseAsync();

            byte _ratingFirst = 1;
            string _ratingComments = "";
            if (_ratingNotes != null)
            {
                JArray _ratingNotesArray = JArray.Parse(_ratingNotes);

                if (_ratingNotesArray.Any())
                {
                    JArray _ratingSortedArray = new(_ratingNotesArray.OrderByDescending(obj => DateTime.Parse(obj["DateTime"]!.ToString())));

                    JToken _ratingFirstCandidate = _ratingSortedArray.FirstOrDefault();
                    if (_ratingFirstCandidate != null)
                    {
                        _ratingFirst = _ratingFirstCandidate["Rating"].ToByte();
                        _ratingComments = _ratingFirstCandidate["Comment"]?.ToString();
                    }

                    _rating = _ratingSortedArray.ToString();
                }
            }

            rating.Rating = _ratingFirst;
            rating.RatingComments = _ratingComments;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error saving rating. {ExceptionMessage}", ex.Message);
            //
        }

        return new()
               {
                   {
                       "RatingList", _rating
                   },
                   {
                       "FirstRating", rating
                   }
               };
    }

    [HttpPost]
    public async Task<ActionResult<string>> SaveSkill(CandidateSkills skill, int candidateID, string user)
    {
        if (skill == null)
        {
            return Ok("[]");
        }

        return await ExecuteQueryAsync("SaveSkill", command =>
                                                    {
                                                        command.Int("EntitySkillId", skill.ID);
                                                        command.Varchar("Skill", 100, skill.Skill);
                                                        command.Int("CandidateID", candidateID);
                                                        command.SmallInt("LastUsed", skill.LastUsed);
                                                        command.SmallInt("ExpMonth", skill.ExpMonth);
                                                        command.Varchar("User", 10, user);
                                                    }, "SaveSkill", "Error saving candidate skill.");

        /*await using SqlConnection _connection = new(Start.ConnectionString);
        await using SqlCommand _command = new("SaveSkill", _connection);
        _command.CommandType = CommandType.StoredProcedure;
        _command.Int("EntitySkillId", skill.ID);
        _command.Varchar("Skill", 100, skill.Skill);
        _command.Int("CandidateID", candidateID);
        _command.SmallInt("LastUsed", skill.LastUsed);
        _command.SmallInt("ExpMonth", skill.ExpMonth);
        _command.Varchar("User", 10, user);
        try
        {
            await _connection.OpenAsync();
            _returnVal = (await _command.ExecuteScalarAsync())?.ToString();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error saving skill. {ExceptionMessage}", ex.Message);
            return StatusCode(500, ex.Message);
        }
        finally
        {
            await _connection.CloseAsync();
        }

        return Ok(_returnVal);*/
    }

    [HttpGet]
    public async Task<ActionResult<string>> SearchCandidates(string filter)
    {
        return await ExecuteQueryAsync("SearchCandidates", command => { command.Varchar("Name", 30, filter); }, "SearchCandidates", "Error searching candidates.");
        /*await using SqlConnection _connection = new(Start.ConnectionString);
        await using SqlCommand _command = new("SearchCandidates", _connection);
        _command.CommandType = CommandType.StoredProcedure;
        _command.Varchar("Name", 30, filter);

        string _candidates = "[]";
        try
        {
            await _connection.OpenAsync();

            _candidates = (await _command.ExecuteScalarAsync())?.ToString();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error searching candidates. {ExceptionMessage}", ex.Message);
            return StatusCode(500, ex.Message);
        }
        finally
        {
            await _connection.CloseAsync();
        }

        return Ok(_candidates);*/
    }

    [HttpPost]
    public async Task<ActionResult<bool>> SubmitCandidateRequisition(int requisitionID, int candidateID, string notes = "", string user = "", string roleID = "RS")
    {
        if (candidateID == 0 || requisitionID == 0)
        {
            return BadRequest("Candidate ID and/or Requisition ID is not valid.");
        }

        await using SqlConnection _connection = new(Start.ConnectionString);
        await using SqlCommand _command = new("SubmitCandidateRequisition", _connection);
        _command.CommandType = CommandType.StoredProcedure;
        _command.Int("RequisitionID", requisitionID);
        _command.Int("CandidateID", candidateID);
        _command.Varchar("Notes", 1000, notes);
        _command.Char("RoleID", 2, roleID);
        _command.Char("User", 10, user);
        try
        {
            await _connection.OpenAsync();
            await using SqlDataReader _reader = await _command.ExecuteReaderAsync();

            await _reader.ReadAsync();
            string _firstName = _reader.NString(0);
            string _lastName = _reader.NString(1);
            string _reqCode = _reader.NString(2);
            string _reqTitle = _reader.NString(3);
            string _original = _reader.NString(4);
            string _originalInternal = _reader.NString(5);
            string _formatted = _reader.NString(6);
            string _formattedInternal = _reader.NString(7);
            string _company = _reader.GetString(8);

            List<EmailTemplates> _templates = [];
            Dictionary<string, string> _emailAddresses = new(), _emailCC = new();

            await _reader.NextResultAsync();
            while (await _reader.ReadAsync())
            {
                _templates.Add(new(_reader.NString(0), _reader.NString(1), _reader.NString(2)));
            }

            await _reader.NextResultAsync();
            while (await _reader.ReadAsync())
            {
                _emailAddresses.Add(_reader.NString(0), _reader.NString(1));
            }

            await _reader.CloseAsync();

            if (_templates.Count > 0)
            {
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
                                                         .Replace("$FULL_NAME$", $"{_firstName} {_lastName}")
                                                         .Replace("$FIRST_NAME$", _firstName)
                                                         .Replace("$LAST_NAME$", _lastName)
                                                         .Replace("$REQ_ID$", _reqCode)
                                                         .Replace("$REQ_TITLE$", _reqTitle)
                                                         .Replace("$COMPANY$", _company)
                                                         .Replace("$SUBMISSION_NOTES$", notes)
                                                         .Replace("$LOGGED_USER$", user);

                _templateSingle.Template = _templateSingle.Template.Replace("$TODAY$", DateTime.Today.CultureDate())
                                                          .Replace("$FULL_NAME$", $"{_firstName} {_lastName}")
                                                          .Replace("$FIRST_NAME$", _firstName)
                                                          .Replace("$LAST_NAME$", _lastName)
                                                          .Replace("$REQ_ID$", _reqCode)
                                                          .Replace("$REQ_TITLE$", _reqTitle)
                                                          .Replace("$COMPANY$", _company)
                                                          .Replace("$SUBMISSION_NOTES$", notes)
                                                          .Replace("$LOGGED_USER$", user);

                if (_original.NotNullOrWhiteSpace() || _formatted.NotNullOrWhiteSpace())
                {
                    return Ok(false);
                }

                byte[] _originalBytesArray = null;
                if (_originalInternal.NotNullOrWhiteSpace())
                {
                    _originalBytesArray = await General.ReadFromBlob($"{Start.AzureBlobContainer}/Candidate/{candidateID}/{_formattedInternal}");
                }

                byte[] _formattedBytesArray = null;
                if (_formattedInternal.NotNullOrWhiteSpace())
                {
                    _formattedBytesArray = await General.ReadFromBlob($"{Start.AzureBlobContainer}/Candidate/{candidateID}/{_formattedInternal}");
                }

                using SmtpClient _smtpClient = new(Start.EmailHost, Start.Port);
                _smtpClient.Credentials = new NetworkCredential(Start.EmailUsername, Start.EmailPassword);
                _smtpClient.EnableSsl = true;

                MailMessage _mailMessage = new()
                                           {
                                               From = new("jolly@hire-titan.com", "Mani Bhai"),
                                               Subject = _templateSingle.Subject,
                                               Body = _templateSingle.Template,
                                               IsBodyHtml = true
                                           };
                _mailMessage.To.Add("manivenkit@gmail.com");

                // Add attachments from byte arrays

                if (!TryAddAttachment(_formattedBytesArray, _formatted, _mailMessage))
                {
                    TryAddAttachment(_originalBytesArray, _original, _mailMessage);
                }

                _smtpClient.Send(_mailMessage);
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error submitting candidate requisition. {ExceptionMessage}", ex.Message);
            return StatusCode(500, ex.Message);
        }
        finally
        {
            await _connection.CloseAsync();
        }

        return Ok(true);
    }

    private bool TryAddAttachment(byte[] array, string name, MailMessage mailMessage)
    {
        if (array is null)
        {
            return false;
        }

        using MemoryStream _formattedStream = new(array);
        Attachment _formattedAttachment = new(_formattedStream, name, GetMimeType(name));
        mailMessage.Attachments.Add(_formattedAttachment);

        return true;
    }

    [HttpPost]
    public async Task<ActionResult<string>> UndoCandidateActivity(int submissionID, string user, string roleID = "RS", bool isCandidateScreen = true)
    {
        if (submissionID == 0)
        {
            return NotFound("Submission ID is not valid.");
        }

        return await ExecuteQueryAsync("UndoCandidateActivity", command =>
                                                                {
                                                                    command.Int("Id", submissionID);
                                                                    command.Varchar("User", 10, user);
                                                                    command.Bit("CandScreen", isCandidateScreen);
                                                                    command.Char("RoleID", 2, roleID);
                                                                }, "UndoCandidateActivity", "Error undoing candidate activity.");
        /*
        await using SqlConnection _connection = new(Start.ConnectionString);
        await using SqlCommand _command = new("UndoCandidateActivity", _connection);
        _command.CommandType = CommandType.StoredProcedure;
        _command.Int("Id", submissionID);
        _command.Varchar("User", 10, user);
        _command.Bit("CandScreen", isCandidateScreen);
        _command.Char("RoleID", 2, roleID);

        try
        {
            await _connection.OpenAsync();
            _activities = (await _command.ExecuteScalarAsync())?.ToString() ?? "[]";
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error undoing candidate activity. {ExceptionMessage}", ex.Message);
            return StatusCode(500, ex.Message);
        }
        finally
        {
            await _connection.CloseAsync();
        }

        return Ok(_activities);*/
    }

    [HttpPost, RequestSizeLimit(62_914_560)] //60 MB
    public async Task<ActionResult<string>> UpdateResume(IFormFile file)
    {
        string _fileName = file.FileName;
        string _candidateID = Request.Form["candidateID"].ToString();
        string _resumeType = Request.Form["type"].ToString();
        string _internalFileName = Guid.NewGuid().ToString("N");
        bool _updateTextResume = Request.Form["updateTextResume"].ToBoolean();
        string _textResume = "";

        if (_updateTextResume)
        {
            if (Path.GetExtension(_fileName).Equals(".pdf", StringComparison.CurrentCultureIgnoreCase))
            {
                _textResume = General.ExtractTextFromPdf(file);
            }
            else if (Path.GetExtension(_fileName).Equals(".doc", StringComparison.CurrentCultureIgnoreCase)
                     || Path.GetExtension(_fileName).Equals(".docx", StringComparison.CurrentCultureIgnoreCase)
                     || Path.GetExtension(_fileName).Equals(".rtf", StringComparison.CurrentCultureIgnoreCase))
            {
                _textResume = General.ExtractTextFromWord(file);
            }
        }

        // Create the folder path
        string _blobPath = $"{Start.AzureBlobContainer}/Candidate/{_candidateID}/{_internalFileName}";

        await General.UploadToBlob(file, _blobPath);

        await using SqlConnection _connection = new(Start.ConnectionString);

        try
        {
            await using SqlCommand _command = new("UpdateResume", _connection);
            _command.CommandType = CommandType.StoredProcedure;
            _command.Int("CandidateId", _candidateID.ToInt32());
            _command.Varchar("InternalName", 50, _internalFileName);
            _command.Varchar("FileName", 255, _fileName);
            _command.Int("Type", Request.Form["type"].ToInt32());
            _command.Varchar("User", 10, Request.Form["user"].ToString());
            _command.Varchar("TextResume", -1, _textResume);
            await _connection.OpenAsync().ConfigureAwait(false);
            await _command.ExecuteNonQueryAsync().ConfigureAwait(false);

            return Ok(_textResume);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error saving candidate document. {ExceptionMessage}", ex.Message);
            return StatusCode(500, ex.Message);
        }
        finally
        {
            await _connection.CloseAsync().ConfigureAwait(false);
        }
    }

    [HttpPost, RequestSizeLimit(62_914_560)] //60 MB
    public async Task<ActionResult<string>> UploadDocument(IFormFile file)
    {
        string _fileName = file.FileName;
        string _candidateID = Request.Form["candidateID"].ToString();
        string _internalFileName = Guid.NewGuid().ToString("N");

        // Create the folder path
        string _blobPath = $"{Start.AzureBlobContainer}/Candidate/{_candidateID}/{_internalFileName}";

        await General.UploadToBlob(file, _blobPath);
        /*// Create a BlobStorage instance
        IAzureBlobStorage _storage = StorageFactory.Blobs.AzureBlobStorageWithSharedKey(Start.AccountName, Start.AzureKey);

        await using (Stream stream = file.OpenReadStream())
        {
            await _storage.WriteAsync(_blobPath, stream);
        }*/

        await using SqlConnection _connection = new(Start.ConnectionString);
        string _returnVal = "[]";
        try
        {
            await using SqlCommand _command = new("SaveCandidateDocuments", _connection);
            _command.CommandType = CommandType.StoredProcedure;
            _command.Int("CandidateId", _candidateID.ToInt32());
            _command.Varchar("DocumentName", 255, Request.Form["name"].ToString());
            _command.Varchar("DocumentLocation", 255, _fileName);
            _command.Varchar("DocumentNotes", 2000, Request.Form["notes"].ToString());
            _command.Varchar("InternalFileName", 50, _internalFileName);
            _command.Int("DocumentType", Request.Form["type"].ToInt32());
            _command.Varchar("DocsUser", 10, Request.Form["user"].ToString());
            await _connection.OpenAsync();
            await using SqlDataReader _reader = await _command.ExecuteReaderAsync();

            while (await _reader.ReadAsync())
            {
                _returnVal = _reader.NString(0, "[]");
            }

            await _reader.CloseAsync();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error saving candidate document. {ExceptionMessage}", ex.Message);
            return StatusCode(500, ex.Message);
        }

        return Ok(_returnVal);
    }
}