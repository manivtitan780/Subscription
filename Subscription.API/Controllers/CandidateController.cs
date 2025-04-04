#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.API
// File Name:           CandidateController.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu, Brijesh Dubey
// Created On:          02-06-2025 16:02
// Last Updated On:     04-02-2025 18:04
// *****************************************/

#endregion

#region Using

using StackExchange.Redis;

#endregion

namespace Subscription.API.Controllers;

[ApiController, Route("api/[controller]/[action]")]
public class CandidateController(OpenAIClient openClient) : ControllerBase
{
    private static readonly string[] JSONSerializable = ["Skill"];

    private void CreateToolFunction(ChatCompletionOptions options)
    {
        options.Tools.Add(ChatTool.CreateFunctionTool("extract_candidate",
                                                      "Extract candidate details from resume",
                                                      BinaryData.FromObjectAsJson(new
                                                                                  {
                                                                                      type = "object",
                                                                                      properties = new
                                                                                                   {
                                                                                                       FirstName = new
                                                                                                                   {
                                                                                                                       type = "string", description = "First name of the candidate"
                                                                                                                   },
                                                                                                       LastName = new {type = "string", description = "Last name of the candidate"},
                                                                                                       PhoneNumbers = new
                                                                                                                      {
                                                                                                                          type = "string", description = "Phone number of the candidate"
                                                                                                                      },
                                                                                                       EmailAddresses = new
                                                                                                                        {
                                                                                                                            type = "string", description = "Email address of the candidate"
                                                                                                                        },
                                                                                                       PostalAddress = new
                                                                                                                       {
                                                                                                                           type = "object",
                                                                                                                           properties = new
                                                                                                                                        {
                                                                                                                                            Street = new
                                                                                                                                                     {
                                                                                                                                                         type = "string",
                                                                                                                                                         description = "Street address of the candidate"
                                                                                                                                                     },
                                                                                                                                            City = new
                                                                                                                                                   {
                                                                                                                                                       type = "string",
                                                                                                                                                       description = "City of the candidate"
                                                                                                                                                   },
                                                                                                                                            State = new
                                                                                                                                                    {
                                                                                                                                                        type = "string",
                                                                                                                                                        description = "State of the candidate"
                                                                                                                                                    },
                                                                                                                                            Zip = new
                                                                                                                                                  {
                                                                                                                                                      type = "string",
                                                                                                                                                      description = "Zip code of the candidate"
                                                                                                                                                  }
                                                                                                                                        }
                                                                                                                       },
                                                                                                       CandidateSummary = new
                                                                                                                          {
                                                                                                                              type = "string",
                                                                                                                              description = "Summary of the candidate"
                                                                                                                          },
                                                                                                       CandidateKeywords = new
                                                                                                                           {
                                                                                                                               type = "string",
                                                                                                                               description = "Keywords related to the candidate"
                                                                                                                           },
                                                                                                       LinkedInProfile = new
                                                                                                                         {
                                                                                                                             type = "string",
                                                                                                                             description = "LinkedIn profile of the candidate"
                                                                                                                         },
                                                                                                       TotalExperienceInMonths =
                                                                                                           new
                                                                                                           {
                                                                                                               type = "integer",
                                                                                                               description = "Total experience of the candidate"
                                                                                                           },
                                                                                                       EducationInfo = new
                                                                                                                       {
                                                                                                                           type = "array",
                                                                                                                           items = new
                                                                                                                                   {
                                                                                                                                       Course = new
                                                                                                                                                   {
                                                                                                                                                       type = "string",
                                                                                                                                                       description = "Course name of the candidate"
                                                                                                                                                   },
                                                                                                                                        School = new
                                                                                                                                                    {
                                                                                                                                                         type = "string",
                                                                                                                                                         description = "School name of the candidate"
                                                                                                                                                    },
                                                                                                                                        Period = new
                                                                                                                                                   {
                                                                                                                                                       type = "string",
                                                                                                                                                       description = "Period of the education"
                                                                                                                                                   },
                                                                                                                                        State = new
                                                                                                                                                   {
                                                                                                                                                       type = "string",
                                                                                                                                                       description = "State of the education"
                                                                                                                                                   },
                                                                                                                                        Country = new
                                                                                                                                                   {
                                                                                                                                                       type = "string",
                                                                                                                                                       description = "Country of the education"
                                                                                                                                                   }
                                                                                                                                   },
                                                                                                                           description = "Education information of the candidate"
                                                                                                                       },
                                                                                                       EmploymentInfo = new
                                                                                                                        {
                                                                                                                            type = "array",
                                                                                                                            items = new
                                                                                                                                    {
                                                                                                                                        Company = new
                                                                                                                                                  {
                                                                                                                                                      type = "string",
                                                                                                                                                      description = "Company name of the candidate"
                                                                                                                                                  },
                                                                                                                                        Start = new
                                                                                                                                                {
                                                                                                                                                    type = "string",
                                                                                                                                                    description = "Start date"
                                                                                                                                                },
                                                                                                                                        End = new
                                                                                                                                              {
                                                                                                                                                  type = "string",
                                                                                                                                                  description = "End date"
                                                                                                                                              },
                                                                                                                                        Location = new
                                                                                                                                                   {
                                                                                                                                                       type = "string",
                                                                                                                                                       description = "Location of the company"
                                                                                                                                                   },
                                                                                                                                        Title = new
                                                                                                                                                {
                                                                                                                                                    type = "string",
                                                                                                                                                    description = "Title of the job profile"
                                                                                                                                                }
                                                                                                                                    },
                                                                                                                            description = "Employment information of the candidate"
                                                                                                                        },
                                                                                                       Skills = new
                                                                                                                {
                                                                                                                    type = "array", items = new
                                                                                                                                            {
                                                                                                                                                type = "object",
                                                                                                                                                properties = new
                                                                                                                                                             {
                                                                                                                                                                 Skill = new
                                                                                                                                                                         {
                                                                                                                                                                             type = "string",
                                                                                                                                                                             description =
                                                                                                                                                                                 "Skill of the candidate"
                                                                                                                                                                         },
                                                                                                                                                                 Period = new
                                                                                                                                                                          {
                                                                                                                                                                              type =
                                                                                                                                                                                  "string",
                                                                                                                                                                              description =
                                                                                                                                                                                  "Period worked from to end the skill. If currently working end will be Present."
                                                                                                                                                                          },
                                                                                                                                                                 Month = new
                                                                                                                                                                         {
                                                                                                                                                                             type =
                                                                                                                                                                                 "integer",
                                                                                                                                                                             description =
                                                                                                                                                                                 "Number of Months worked in the skill"
                                                                                                                                                                         }
                                                                                                                                                             },
                                                                                                                                                required = JSONSerializable
                                                                                                                                            },
                                                                                                                    description = "Skills of the candidate"
                                                                                                                }
                                                                                                   }
                                                                                  })));
        ;
    }

    /// <summary>
    ///     Deletes a candidate's document from the database.
    /// </summary>
    /// <param name="documentID">The ID of the document to be deleted.</param>
    /// <param name="user">The user who is performing the delete operation.</param>
    /// <returns>A dictionary containing the status of the operation and any relevant data.</returns>
    /// <remarks>
    ///     This method connects to the database, executes a stored procedure to delete the document,
    ///     and returns a dictionary containing the result of the operation.
    ///     If the operation is successful, the dictionary will contain a list of remaining documents for the candidate.
    /// </remarks>
    [HttpPost]
    public async Task<ActionResult<string>> DeleteCandidateDocument(int documentID, string user)
    {
        await using SqlConnection _connection = new(Start.ConnectionString);
        string _documents = "[]";
        await using SqlCommand _command = new("DeleteCandidateDocument", _connection);
        _command.CommandType = CommandType.StoredProcedure;
        _command.Int("CandidateDocumentId", documentID);
        _command.Varchar("User", 10, user); //TODO: make sure you delete the associated document from Azure filesystem too.
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

        return Ok(_documents);
    }

    /// <summary>
    ///     Deletes a candidate's education record from the database.
    /// </summary>
    /// <param name="id">The ID of the education record to be deleted.</param>
    /// <param name="candidateID">The ID of the candidate whose education record is to be deleted.</param>
    /// <param name="user">The user who is performing the delete operation.</param>
    /// <returns>A dictionary containing the updated list of education records for the candidate.</returns>
    /// <remarks>
    ///     This method connects to the database, executes a stored procedure to delete the education record,
    ///     and returns a dictionary containing the updated list of education records.
    ///     If the operation is successful, the dictionary will contain a list of remaining education records for the
    ///     candidate.
    /// </remarks>
    [HttpPost]
    public async Task<ActionResult<string>> DeleteEducation(int id, int candidateID, string user)
    {
        await Task.Delay(1);
        string _education = "[]";
        if (id == 0)
        {
            return Ok("[]");
        }

        await using SqlConnection _connection = new(Start.ConnectionString);

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

        return Ok(_education);
    }

    /// <summary>
    ///     Deletes a candidate's experience record from the database.
    /// </summary>
    /// <param name="id">The ID of the experience record to be deleted.</param>
    /// <param name="candidateID">The ID of the candidate whose experience record is to be deleted.</param>
    /// <param name="user">The user who is performing the delete operation.</param>
    /// <returns>A dictionary containing the updated list of experience records for the candidate.</returns>
    /// <remarks>
    ///     This method connects to the database, executes a stored procedure to delete the experience record,
    ///     and returns a dictionary containing the updated list of experience records.
    ///     If the operation is successful, the dictionary will contain a list of remaining experience records for the
    ///     candidate.
    /// </remarks>
    [HttpPost]
    public async Task<ActionResult<string>> DeleteExperience(int id, int candidateID, string user)
    {
        await Task.Delay(1);
        string _experiences = "[]";
        if (id == 0)
        {
            return Ok(_experiences);
        }

        await using SqlConnection _connection = new(Start.ConnectionString);
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

        return Ok(_experiences);
    }

    /// <summary>
    ///     Deletes a candidate's note from the database.
    /// </summary>
    /// <param name="id">The ID of the note to be deleted.</param>
    /// <param name="candidateID">The ID of the candidate whose note is to be deleted.</param>
    /// <param name="user">The user who is performing the delete operation.</param>
    /// <returns>A dictionary containing the updated list of notes for the candidate.</returns>
    /// <remarks>
    ///     This method connects to the database, executes a stored procedure to delete the note,
    ///     and returns a dictionary containing the updated list of notes.
    ///     If the operation is successful, the dictionary will contain a list of remaining notes for the candidate.
    /// </remarks>
    [HttpPost]
    public async Task<ActionResult<string>> DeleteNotes(int id, int candidateID, string user)
    {
        string _notes = "[]";
        if (id == 0)
        {
            return Ok("[]");
        }

        await using SqlConnection _connection = new(Start.ConnectionString);
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

        return Ok(_notes);
    }

    /// <summary>
    ///     Deletes a candidate's skill record from the database.
    /// </summary>
    /// <param name="id">The ID of the skill record to be deleted.</param>
    /// <param name="candidateID">The ID of the candidate whose skill record is to be deleted.</param>
    /// <param name="user">The user who is performing the delete operation.</param>
    /// <returns>A dictionary containing the updated list of skill records for the candidate.</returns>
    /// <remarks>
    ///     This method connects to the database, executes a stored procedure to delete the skill record,
    ///     and returns a dictionary containing the updated list of skill records.
    ///     If the operation is successful, the dictionary will contain a list of remaining skill records for the candidate.
    /// </remarks>
    [HttpPost]
    public async Task<ActionResult<string>> DeleteSkill(int id, int candidateID, string user)
    {
        string _skills = "[]";
        if (id == 0)
        {
            return Ok(_skills);
        }

        await using SqlConnection _connection = new(Start.ConnectionString);
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

        return Ok(_skills);
    }

    /// <summary>
    ///     Downloads a file associated with a specific document ID.
    /// </summary>
    /// <param name="documentID">
    ///     The ID of the document to be downloaded.
    /// </param>
    /// <returns>
    ///     A <see cref="DocumentDetails" /> object containing details of the downloaded document.
    /// </returns>
    /// <remarks>
    ///     This method connects to the database, executes a stored procedure to fetch the document details,
    ///     and returns a <see cref="DocumentDetails" /> object containing the details of the document.
    ///     If the document does not exist, null is returned.
    /// </remarks>
    [HttpGet]
    public async Task<ActionResult<string>> DownloadFile(int documentID)
    {
        await using SqlConnection _connection = new(Start.ConnectionString);
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

        return Ok(_documentDetails);
    }

    /// <summary>
    ///     Downloads the resume of a candidate.
    /// </summary>
    /// <param name="candidateID">
    ///     The ID of the candidate whose resume is to be downloaded.
    /// </param>
    /// <param name="resumeType">
    ///     The type of the resume to be downloaded, Original or Formatted.
    /// </param>
    /// <returns>
    ///     A <see cref="DocumentDetails" /> object containing the details of the downloaded resume.
    /// </returns>
    /// <remarks>
    ///     This method connects to the database using a stored procedure to download the candidate's resume.
    ///     The resume details are then encapsulated in a <see cref="DocumentDetails" /> object and returned.
    /// </remarks>
    [HttpGet]
    public async Task<ActionResult<string>> DownloadResume(int candidateID, string resumeType)
    {
        await using SqlConnection _connection = new(Start.ConnectionString);
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

        return Ok(_documentDetails);
    }

    private static DataTable Education(JsonArray education)
    {
        DataTable _tableEducation = new();
        _tableEducation.Columns.Add("Degree", typeof(string));
        _tableEducation.Columns.Add("College", typeof(string));
        _tableEducation.Columns.Add("State", typeof(string));
        _tableEducation.Columns.Add("Country", typeof(string));
        _tableEducation.Columns.Add("Year", typeof(string));

        foreach (JsonNode _education in education)
        {
            if (_education == null)
            {
                continue;
            }

            DataRow _row = _tableEducation.NewRow();
            _row["Degree"] = _education["Course"]?.ToString() ?? string.Empty;
            _row["College"] = _education["School"]?.ToString() ?? string.Empty;
            _row["State"] = _education["State"]?.ToString() ?? string.Empty;
            _row["Country"] = _education["Country"]?.ToString() ?? string.Empty;
            _row["Year"] = _education["Period"]?.ToString() ?? string.Empty;
            _tableEducation.Rows.Add(_row);
        }

        return _tableEducation;
    }

    private static DataTable Experience(JsonArray experience)
    {
        DataTable _tableExperience = new();
        _tableExperience.Columns.Add("Employer", typeof(string));
        _tableExperience.Columns.Add("Start", typeof(string));
        _tableExperience.Columns.Add("End", typeof(string));
        _tableExperience.Columns.Add("Location", typeof(string));
        _tableExperience.Columns.Add("Title", typeof(string));
        _tableExperience.Columns.Add("Description", typeof(string));

        foreach (JsonNode _experience in experience)
        {
            if (_experience == null)
            {
                continue;
            }

            DataRow _row = _tableExperience.NewRow();
            _row["Employer"] = _experience["Company"]?.ToString() ?? string.Empty;
            //string[] _period = _experience?["Period"]?.ToString().Split('-');
            _row["Start"] = _experience["Start"]?.ToString() ?? string.Empty;
            _row["End"] = _experience["End"]?.ToString() ?? string.Empty;
            _row["Location"] = _experience["Location"]?.ToString() ?? string.Empty;
            _row["Title"] = _experience["Title"]?.ToString() ?? string.Empty;
            _row["Description"] = _experience["Description"]?.ToString() ?? string.Empty;
            _tableExperience.Rows.Add(_row);
        }

        return _tableExperience;
    }

    /// <summary>
    ///     Retrieves the detailed information of a candidate by their ID.
    /// </summary>
    /// <param name="candidateID">
    ///     The ID of the candidate.
    /// </param>
    /// <param name="roleID">
    ///     The role ID associated with the user making the request. This is used for access control and
    ///     permissions management.
    /// </param>
    /// <returns>
    ///     A dictionary containing the candidate's details, notes, skills, education, experience, activity, rating, MPC,
    ///     RatingMPC, and documents.
    /// </returns>
    /// <remarks>
    ///     This method performs a database operation using a stored procedure named "GetDetailCandidate".
    ///     It reads multiple result sets from the database to populate various aspects of the candidate's information.
    /// </remarks>
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

                _candRating = _candidateJson["RateNotes"]?.ToString(); // _reader.NString(28);
                _candMPC = _candidateJson["MPCNotes"]?.ToString();     //_reader.NString(30);
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

    /// <summary>
    ///     Creates a location string based on the provided candidate details and state name.
    /// </summary>
    /// <param name="candidateDetails">
    ///     An instance of <see cref="CandidateDetails" /> containing the city and zip code details of the candidate.
    /// </param>
    /// <param name="stateName">
    ///     The name of the state.
    /// </param>
    /// <returns>
    ///     A string representing the location, in the format of "City, State, ZipCode". If any part is not available, it
    ///     will be omitted from the string.
    /// </returns>
    private static string GetCandidateLocation(CandidateDetails candidateDetails, string stateName)
    {
        string _location = "";

        if (!candidateDetails!.City.NullOrWhiteSpace())
        {
            _location = candidateDetails.City;
        }

        if (!stateName.NullOrWhiteSpace())
        {
            _location += ", " + stateName;
        }
        else
        {
            _location = stateName;
        }

        if (!candidateDetails.ZipCode.NullOrWhiteSpace())
        {
            _location += ", " + candidateDetails.ZipCode;
        }
        else
        {
            _location = candidateDetails.ZipCode;
        }

        return _location;
    }

    /// <summary>
    ///     Retrieves a list of candidates based on the provided search model.
    /// </summary>
    /// <param name="searchModel">
    ///     The search model containing the criteria for filtering candidates. This includes parameters such as
    ///     item count, page number, sort field, sort direction, name, keywords, skills, city, state, proximity, eligibility,
    ///     relocation, job options, security clearance, and user.
    /// </param>
    /// <returns>
    ///     A dictionary containing the list of candidates and the total count of candidates matching the search criteria.
    /// </returns>
    /// <remarks>
    ///     This method performs a database operation using a stored procedure named "GetCandidates".
    ///     It reads the result set from the database to populate the list of candidates and the total count.
    /// </remarks>
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
        _command.Varchar("Name", 30, searchModel.Name);
        //_command.Varchar("Phone", 20, searchModel.Phone);
        //_command.Varchar("Email", 255, searchModel.EmailAddress);
        _command.Bit("MyCandidates", !searchModel.AllCandidates);
        _command.Bit("IncludeAdmin", searchModel.IncludeAdmin);
        _command.Varchar("Keywords", 2000, searchModel.Keywords);
        _command.Varchar("Skill", 2000, searchModel.Skills);
        _command.Bit("SearchState", !searchModel.CityZip);
        _command.Varchar("City", 30, searchModel.CityName);
        _command.Varchar("State", 1000, searchModel.StateID);
        _command.Int("Proximity", searchModel.Proximity);
        _command.TinyInt("ProximityUnit", searchModel.ProximityUnit);
        _command.Varchar("Eligibility", 10, searchModel.Eligibility);
        _command.Varchar("Reloc", 10, searchModel.Relocate);
        _command.Varchar("JobOptions", 10, searchModel.JobOptions);
        //_command.Varchar("Communications",10, searchModel.Communication);
        _command.Varchar("Security", 10, searchModel.SecurityClearance);
        _command.Varchar("User", 10, searchModel.User);
        //_command.Bit("ActiveRequisitionsOnly", searchModel.ActiveRequisitionsOnly);
        //_command.Int("OptionalCandidateID", candidateID);
        //_command.Bit("ThenProceed", thenProceed);

        int _count = -1;
        try
        {
            await _connection.OpenAsync();
            await using SqlDataReader _reader = await _command.ExecuteReaderAsync();

            await _reader.ReadAsync();
            _count = _reader.GetInt32(0);

            await _reader.NextResultAsync();
            while (await _reader.ReadAsync())
            {
                _candidates = _reader.NString(0);
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

    public async Task<ActionResult<string>> ParseCandidate(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("No file has been uploaded.");
        }

        string _fileContent = string.Empty;
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

        string _parsedJSON = string.Empty;
        string _tempJSONFileName = Path.Combine($"{Guid.NewGuid():N}.json");
        try
        {
            ChatCompletion _completeChatAsync = await _chatClient.CompleteChatAsync(_messages, _chatOptions);
            _parsedJSON = _completeChatAsync.Content[0].Text;
            //JsonSerializer.Serialize(_completeChatAsync);

            /* Parse JSON to Objects */
            JsonNode _rootNode = JsonNode.Parse(_parsedJSON)!;
            if (_rootNode != null)
            {
                string _firstName = _rootNode["FirstName"]?.ToString() ?? string.Empty;
                string _lastName = _rootNode["LastName"]?.ToString() ?? string.Empty;
                string _phone = _rootNode["PhoneNumbers"]?[0]?.ToString() ?? string.Empty;
                string _email = _rootNode["EmailAddresses"]?[0]?.ToString() ?? string.Empty;
                string _street = _rootNode["PostalAddress"]?["Street"]?.ToString() ?? string.Empty;
                string _city = _rootNode["PostalAddress"]?["City"]?.ToString() ?? string.Empty;
                string _stateName = _rootNode["PostalAddress"]?["State"]?.ToString() ?? string.Empty;
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

                string _zip = _rootNode["PostalAddress"]?["Zip"]?.ToString() ?? string.Empty;
                string _summary = _rootNode["CandidateSummary"]?.ToString() ?? string.Empty;
                string _keywords = _rootNode["CandidateKeywords"]?.ToString() ?? string.Empty;

                /*Education*/
                DataTable _tableEducation = Education(_rootNode["EducationInfo"] as JsonArray);

                /*Experience*/
                DataTable _tableExperience = Experience(_rootNode["EmploymentInfo"] as JsonArray);

                /* Skills */
                DataTable _tableSkills = Skills(_rootNode["Skills"] as JsonArray);
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error parsing candidate. {ExceptionMessage}", ex.Message);
            return StatusCode(500, ex.Message);
        }

        return Ok(_parsedJSON);
    }

    /// <summary>
    ///     Asynchronously saves the details of a candidate to the database.
    /// </summary>
    /// <param name="candidateDetails">
    ///     The details of the candidate to be saved.
    /// </param>
    /// <param name="jsonPath">
    ///     The path to the JSON file containing the email template.
    /// </param>
    /// <param name="userName">
    ///     The username of the user performing the operation. Default is an empty string.
    /// </param>
    /// <param name="emailAddress">
    ///     The email address to which the operation result should be sent. Default is
    ///     "maniv@titan-techs.com".
    /// </param>
    /// <returns>
    ///     Returns an integer indicating the result of the operation. A return value of -1 indicates that the candidate
    ///     details were null.
    /// </returns>
    /// <remarks>
    ///     This method performs the following operations:
    ///     - Opens a connection to the database.
    ///     - Creates a new SQL command with the stored procedure "SaveCandidate".
    ///     - Adds the details of the candidate as parameters to the SQL command.
    ///     - Executes the SQL command and reads the result.
    ///     - If there are any email templates, it sends an email with the operation result.
    ///     - Closes the connection to the database.
    ///     - Returns the result of the operation.
    /// </remarks>
    [HttpPost, SuppressMessage("ReSharper", "CollectionNeverQueried.Local")]
    public async Task<ActionResult<int>> SaveCandidate(CandidateDetails candidateDetails, string jsonPath = "", string userName = "", string emailAddress = "maniv@titan-techs.com")
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

                _templateSingle.Subject = _templateSingle.Subject!.Replace("$TODAY$", DateTime.Today.CultureDate())
                                                         .Replace("$FULL_NAME$", $"{candidateDetails.FirstName} {candidateDetails.LastName}")
                                                         .Replace("$FIRST_NAME$", candidateDetails.FirstName)
                                                         .Replace("$LAST_NAME$", candidateDetails.LastName)
                                                         .Replace("$CAND_LOCATION$", GetCandidateLocation(candidateDetails, _stateName))
                                                         .Replace("$CAND_PHONE_PRIMARY$", candidateDetails.Phone1.StripPhoneNumber().FormatPhoneNumber())
                                                         .Replace("$CAND_SUMMARY$", candidateDetails.Summary)
                                                         .Replace("$LOGGED_USER$", userName);
                _templateSingle.Template = _templateSingle.Template!.Replace("$TODAY$", DateTime.Today.CultureDate())
                                                          .Replace("$FULL_NAME$", $"{candidateDetails.FirstName} {candidateDetails.LastName}")
                                                          .Replace("$FIRST_NAME$", candidateDetails.FirstName)
                                                          .Replace("$LAST_NAME$", candidateDetails.LastName)
                                                          .Replace("$CAND_LOCATION$", GetCandidateLocation(candidateDetails, _stateName))
                                                          .Replace("$CAND_PHONE_PRIMARY$", candidateDetails.Phone1.StripPhoneNumber().FormatPhoneNumber())
                                                          .Replace("$CAND_SUMMARY$", candidateDetails.Summary)
                                                          .Replace("$LOGGED_USER$", userName);

                /*SendResponse? _email = await Email.From("maniv@hire-titan.com")
                                                  .To("manivenkit@gmail.com", "Mani Bhai")
                                                  .Subject("Chup chaap accept kar")
                                                  .Body("Bhai ka message aayela hain. Accept karne ka, samjha kya?")
                                                  .SendAsync();*/
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
                _smtpClient.Send(_mailMessage);
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

    /// <summary>
    ///     Saves a candidate's activity to the database.
    /// </summary>
    /// <param name="activity">The activity of the candidate to be saved.</param>
    /// <param name="candidateID">The ID of the candidate.</param>
    /// <param name="user">The user who is performing the save operation.</param>
    /// <param name="roleID">The role ID of the user, default is "RS".</param>
    /// <param name="isCandidateScreen">
    ///     A flag indicating whether the operation is performed from the candidate screen, default
    ///     is true.
    /// </param>
    /// <param name="emailAddress">The email address to which notifications will be sent, default is "maniv@titan-techs.com".</param>
    /// <param name="uploadPath">The path where files will be uploaded, default is an empty string.</param>
    /// <param name="jsonPath">The path where JSON files will be stored, default is an empty string.</param>
    /// <returns>A dictionary containing the status of the operation and any relevant data.</returns>
    /// <remarks>
    ///     This method connects to the database, executes a stored procedure to save the activity,
    ///     and returns a dictionary containing the result of the operation.
    ///     If the operation is successful, the dictionary will contain a list of activities for the candidate.
    /// </remarks>
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

    /// <summary>
    ///     Saves a candidate's education record to the database.
    /// </summary>
    /// <param name="education">
    ///     The education record of the candidate to be saved.
    /// </param>
    /// <param name="candidateID">
    ///     The ID of the candidate whose education record is to be saved.
    /// </param>
    /// <param name="user">
    ///     The user who is performing the save operation.
    /// </param>
    /// <returns>
    ///     A JSON formatted string containing the updated list of education records for the candidate.
    /// </returns>
    /// <remarks>
    ///     This method connects to the database, executes a stored procedure to save the education record,
    ///     and returns a JSON formatted string containing the updated list of education records.
    ///     If the operation is successful, the JSON formatted string will contain a list of the candidate's education records.
    /// </remarks>
    [HttpPost]
    public async Task<ActionResult<string>> SaveEducation(CandidateEducation education, int candidateID, string user)
    {
        string _returnVal = "[]";
        if (education == null)
        {
            return Ok(_returnVal);
        }

        await using SqlConnection _connection = new(Start.ConnectionString);
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

        return Ok(_returnVal);
    }

    /// <summary>
    ///     Saves a candidate's experience record to the database.
    /// </summary>
    /// <param name="experience">
    ///     The experience record of the candidate to be saved.
    /// </param>
    /// <param name="candidateID">
    ///     The ID of the candidate whose experience record is to be saved.
    /// </param>
    /// <param name="user">
    ///     The user who is performing the save operation.
    /// </param>
    /// <returns>
    ///     A JSON formatted containing the updated list of experience records for the candidate.
    /// </returns>
    /// <remarks>
    ///     This method connects to the database, executes a stored procedure to save the experience record,
    ///     and returns a JSON formatted string containing the updated list of experience records.
    ///     If the operation is successful, the JSON formatted string will contain a list of updated experience records for the
    ///     candidate.
    /// </remarks>
    [HttpPost]
    public async Task<ActionResult<string>> SaveExperience(CandidateExperience experience, int candidateID, string user)
    {
        string _returnVal = "[]";
        if (experience == null)
        {
            return Ok(_returnVal);
        }

        await using SqlConnection _connection = new(Start.ConnectionString);
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

        return Ok(_returnVal);
    }

    /// <summary>
    ///     Saves the MPC (Most Placeable Candidate) rating for a candidate.
    /// </summary>
    /// <param name="mpc">
    ///     An instance of <see cref="CandidateRatingMPC" /> representing the MPC rating and comments for a candidate.
    /// </param>
    /// <param name="user">
    ///     A string representing the user who is saving the MPC rating.
    /// </param>
    /// <returns>
    ///     A dictionary containing a list of all MPC ratings for the candidate and the first MPC rating.
    /// </returns>
    /// <remarks>
    ///     This method connects to the database, executes a stored procedure to save the MPC rating, and retrieves all MPC
    ///     ratings for the candidate.
    ///     If the provided MPC rating is null, it returns a dictionary with an empty list and null as the first MPC.
    ///     The method handles any exceptions that occur during the database operations and continues execution.
    /// </remarks>
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

    /// <summary>
    ///     Saves the notes of a candidate in the database.
    /// </summary>
    /// <param name="candidateNote">
    ///     An instance of <see cref="CandidateNotes" /> containing the note details.
    /// </param>
    /// <param name="candidateID">
    ///     The ID of the candidate for whom the note is to be saved.
    /// </param>
    /// <param name="user">
    ///     The user who is performing the save operation.
    /// </param>
    /// <returns>
    ///     A JSON formatted containing the updated list of notes for the candidate.
    /// </returns>
    /// <remarks>
    ///     This method connects to the database, executes a stored procedure to save the note,
    ///     and returns a JSON formatted containing the updated list of notes.
    ///     If the operation is successful, the JSON formatted will contain a list of notes for the candidate.
    ///     If the candidateNote parameter is null, an empty list of notes is returned.
    /// </remarks>
    [HttpPost]
    public async Task<ActionResult<string>> SaveNotes(CandidateNotes candidateNote, int candidateID, string user)
    {
        string _returnVal = "[]";
        if (candidateNote == null)
        {
            return Ok(_returnVal);
        }

        await using SqlConnection _connection = new(Start.ConnectionString);
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

        return Ok(_returnVal);
    }

    /// <summary>
    ///     Asynchronously saves the rating of a candidate.
    /// </summary>
    /// <param name="rating">An instance of <see cref="CandidateRatingMPC" /> representing the rating to be saved.</param>
    /// <param name="user">A string representing the user who is saving the rating.</param>
    /// <returns>
    ///     A task that represents the asynchronous operation. The task result contains a dictionary with two keys:
    ///     "RatingList" - a list of <see cref="CandidateRating" /> objects representing the rating history of the candidate,
    ///     and "FirstRating" - an instance of <see cref="CandidateRatingMPC" /> representing the first rating in the rating
    ///     history.
    /// </returns>
    /// <remarks>
    ///     This method performs a database operation using a stored procedure named "ChangeRating".
    ///     If the rating parameter is null, the method returns a dictionary with "RatingList" key set to an empty list and
    ///     "FirstRating" key set to null.
    ///     If the rating parameter is not null, the method executes the stored procedure with the provided rating and user
    ///     parameters,
    ///     then parses the result to create a list of <see cref="CandidateRating" /> objects and an instance of
    ///     <see cref="CandidateRatingMPC" />.
    ///     These are then returned in a dictionary.
    /// </remarks>
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

    /// <summary>
    ///     Saves a candidate's skill to the database.
    /// </summary>
    /// <param name="skill">
    ///     The skill of the candidate to be saved.
    /// </param>
    /// <param name="candidateID">
    ///     The ID of the candidate whose skill is to be saved.
    /// </param>
    /// <param name="user">
    ///     The user who is performing the save operation.
    /// </param>
    /// <returns>
    ///     A JSON formatted containing the updated list of skills for the candidate.
    /// </returns>
    /// <remarks>
    ///     This method connects to the database, executes a stored procedure to save the skill,
    ///     and returns a JSON formatted containing the updated list of skills.
    ///     If the operation is successful, the JSON formatted will contain a list of remaining skills for the
    ///     candidate.
    /// </remarks>
    [HttpPost]
    public async Task<ActionResult<string>> SaveSkill(CandidateSkills skill, int candidateID, string user)
    {
        string _returnVal = "[]";
        if (skill == null)
        {
            return Ok(_returnVal);
        }

        await using SqlConnection _connection = new(Start.ConnectionString);
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

        return Ok(_returnVal);
    }

    /// <summary>
    ///     This asynchronous method, SearchCompanies, is designed to interact with the database to fetch a list of companies
    ///     based on a provided search string.
    ///     It accepts a single parameter, which is a string representing the company name or part of it.
    ///     The method calls a stored procedure named 'SearchCompanies' in the database, passing the search string as a
    ///     parameter.
    ///     The stored procedure is expected to return a list of company names that match the search string.
    ///     The method then reads these company names and returns them as a list of strings.
    ///     If no matches are found, the method will return an empty list.
    /// </summary>
    /// <param name="filter">
    ///     The company name or part of it to search for.
    /// </param>
    /// <returns>
    ///     A task that represents the asynchronous operation. The task result contains a list of company names matching the
    ///     search string.
    /// </returns>
    [HttpGet]
    public async Task<ActionResult<string>> SearchCandidates(string filter)
    {
        await using SqlConnection _connection = new(Start.ConnectionString);
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

        return Ok(_candidates);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static DataTable Skills(JsonArray skills)
    {
        DataTable _tableSkills = new();
        _tableSkills.Columns.Add("Skill", typeof(string));
        _tableSkills.Columns.Add("LastUsed", typeof(int));
        _tableSkills.Columns.Add("Month", typeof(int));

        foreach (JsonNode _skill in skills)
        {
            if (_skill == null)
            {
                continue;
            }

            DataRow _row = _tableSkills.NewRow();
            _row["Skill"] = _skill["Skill"]?.ToString() ?? string.Empty;
            _row["LastUsed"] = _skill["PeriodOfUsage"]?.ToInt32() ?? 0;
            _row["Month"] = _skill["MonthsOfUsage"]?.ToInt32() ?? 0;
            _tableSkills.Rows.Add(_row);
        }

        return _tableSkills;
    }

    /// <summary>
    ///     Reverts the last activity of a candidate.
    /// </summary>
    /// <param name="submissionID">The ID of the submission related to the candidate's activity.</param>
    /// <param name="user">The user who is performing the undo operation.</param>
    /// <param name="roleID">The role ID of the user, default is "RS".</param>
    /// <param name="isCandidateScreen">
    ///     A boolean value indicating if the operation is performed from the candidate screen,
    ///     default is true.
    /// </param>
    /// <returns>A dictionary containing the list of remaining activities for the candidate.</returns>
    /// <remarks>
    ///     This method connects to the database, executes a stored procedure to undo the candidate's last activity,
    ///     and returns a dictionary containing the updated list of activities.
    ///     If the operation is successful, the dictionary will contain a list of remaining activities for the candidate.
    /// </remarks>
    [HttpPost]
    public async Task<ActionResult<string>> UndoCandidateActivity(int submissionID, string user, string roleID = "RS", bool isCandidateScreen = true)
    {
        string _activities = "[]";
        if (submissionID == 0)
        {
            return NotFound("Submission ID is not valid.");
        }

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

        return Ok(_activities);
    }

    /// <summary>
    ///     Uploads a document for a candidate.
    /// </summary>
    /// <param name="file">
    ///     The file to be uploaded.
    /// </param>
    /// <returns>
    ///     A JSON formatted containing the status of the operation and any relevant data.
    /// </returns>
    /// <remarks>
    ///     This method creates a directory for the candidate's documents if it doesn't exist,
    ///     saves the uploaded file to the server, and then saves the document details to the database
    ///     using a stored procedure. If the operation is successful, the JSON formatted will contain a list
    ///     of documents for the candidate.
    /// </remarks>
    [HttpPost, RequestSizeLimit(62_914_560)] //60 MB
    public async Task<ActionResult<string>> UploadDocument(IFormFile file)
    {
        string _fileName = file.FileName;
        string _candidateID = Request.Form["candidateID"].ToString();
        string _internalFileName = Guid.NewGuid().ToString("N");

        // Create a BlobStorage instance
        IAzureBlobStorage _storage = StorageFactory.Blobs.AzureBlobStorageWithSharedKey(Start.AccountName, Start.AzureKey);

        // Create the folder path
        string _blobPath = $"{Start.AzureBlobContainer}/Candidate/{_candidateID}/{_internalFileName}";

        await using (Stream stream = file.OpenReadStream())
        {
            await _storage.WriteAsync(_blobPath, stream);
        }

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
                _returnVal = _reader.NString(0);
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