#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.API
// File Name:           CandidateController.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu
// Created On:          12-28-2024 19:12
// Last Updated On:     12-30-2024 20:12
// *****************************************/

#endregion

namespace Subscription.API.Controllers;

[ApiController, Route("api/[controller]/[action]")]
public class CandidateController : ControllerBase
{
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
    public async Task<DocumentDetails> DownloadFile(int documentID)
    {
        try
        {
            await using SqlConnection _connection = new(Start.ConnectionString);
            await using SqlCommand _command = new("Professional.dbo.GetCandidateDocumentDetails", _connection);
            _command.CommandType = CommandType.StoredProcedure;
            _command.Int("DocumentID", documentID);

            await _connection.OpenAsync();
            await using SqlDataReader _reader = await _command.ExecuteReaderAsync();

            DocumentDetails _documentDetails = null;
            while (_reader.Read())
            {
                _documentDetails = new(_reader.GetInt32(0), _reader.NString(1), _reader.NString(2), _reader.NString(3));
            }

            await _reader.CloseAsync();

            await _connection.CloseAsync();

            return _documentDetails;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error downloading file. {ExceptionMessage}", ex.Message);
            return null;
        }
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
    public async Task<DocumentDetails> DownloadResume(int candidateID, string resumeType)
    {
        try
        {
            await using SqlConnection _connection = new(Start.ConnectionString);
            await using SqlCommand _command = new("DownloadCandidateResume", _connection);
            _command.CommandType = CommandType.StoredProcedure;
            _command.Int("CandidateID", candidateID);
            _command.Varchar("ResumeType", 20, resumeType);

            await _connection.OpenAsync();
            await using SqlDataReader _reader = await _command.ExecuteReaderAsync();

            DocumentDetails _documentDetails = null;

            while (_reader.Read())
            {
                _documentDetails = new()
                                   {
                                       EntityID = _reader.GetInt32(0),
                                       DocumentName = _reader.NString(1),
                                       DocumentLocation = _reader.NString(2),
                                       InternalFileName = _reader.NString(3)
                                   };
            }

            await _reader.CloseAsync();

            await _connection.CloseAsync();

            return _documentDetails;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error downloading resume. {ExceptionMessage}", ex.Message);
            return null;
        }
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
    public async Task<ActionResult<Dictionary<string, object>>> GetCandidateDetails(int candidateID, string roleID)
    {
        try
        {
            await using SqlConnection _connection = new(Start.ConnectionString);
            CandidateDetails _candidate = null;
            string _candRating = "", _candMPC = "";

            await using SqlCommand _command = new("GetDetailCandidate", _connection);
            _command.CommandType = CommandType.StoredProcedure;
            _command.Int("@CandidateID", candidateID);
            _command.Char("@RoleID", 2, roleID);

            await _connection.OpenAsync();
            await using SqlDataReader _reader = await _command.ExecuteReaderAsync();
            if (_reader.HasRows) //Candidate Details
            {
                _reader.Read();
                _candidate = new()
                             {
                                 FirstName = _reader.NString(0), MiddleName = _reader.NString(1), LastName = _reader.NString(2), Address1 = _reader.NString(3), Address2 = _reader.NString(4),
                                 City = _reader.NString(5), StateID = _reader.GetInt32(6), ZipCode = _reader.NString(7), Email = _reader.NString(8), Phone1 = _reader.NString(9),
                                 Phone2 = _reader.NString(10), Phone3 = _reader.NString(11), PhoneExt = _reader.NInt16(12).ToString(), LinkedIn = _reader.NString(13), Facebook = _reader.NString(14),
                                 Twitter = _reader.NString(15), Title = _reader.NString(16), EligibilityID = _reader.GetInt32(17), Relocate = _reader.GetBoolean(18),
                                 Background = _reader.GetBoolean(19), JobOptions = _reader.NString(20), TaxTerm = _reader.NString(21), OriginalResume = _reader.NString(22),
                                 FormattedResume = _reader.NString(23), TextResume = _reader.NString(24), Keywords = _reader.NString(25), Communication = _reader.NString(26),
                                 RateCandidate = _reader.GetByte(27), RateNotes = _reader.NString(28), MPC = _reader.GetBoolean(29), MPCNotes = _reader.NString(30),
                                 ExperienceID = _reader.GetInt32(31), HourlyRate = _reader.GetDecimal(32), HourlyRateHigh = _reader.GetDecimal(33), SalaryHigh = _reader.GetDecimal(34),
                                 SalaryLow = _reader.GetDecimal(35), RelocationNotes = _reader.NString(36), SecurityNotes = _reader.NString(37), Refer = _reader.GetBoolean(38),
                                 ReferAccountManager = _reader.NString(39), EEO = _reader.GetBoolean(40), EEOFile = _reader.NString(41), Summary = _reader.NString(42),
                                 GooglePlus = _reader.NString(43), Created = _reader.NString(44), Updated = _reader.NString(45), CandidateID = candidateID, Status = _reader.NString(46)
                             };
                _candRating = _reader.NString(28);
                _candMPC = _reader.NString(30);
            }

            _reader.NextResult(); //Notes
            string _notes = "[]";
            while (_reader.Read())
            {
                _notes = _reader.NString(0);
            }

            /*List<CandidateNotes> _notes = await _reader.FillList<CandidateNotes>(note => new()
                                                                                         {
                                                                                             ID = note.GetInt32(0), UpdatedDate = note.GetDateTime(1), UpdatedBy = note.GetString(2),
                                                                                             Notes = note.GetString(3)
                                                                                         }).ToListAsync();*/
            _reader.NextResult(); //Skills
            string _skills = "[]";
            while (_reader.Read())
            {
                _skills = _reader.NString(0);
            }

            /*List<CandidateSkills> _skills = await _reader.FillList<CandidateSkills>(skill => new()
                                                                                             {
                                                                                                 ID = skill.GetInt32(0), Skill = skill.GetString(1), LastUsed = skill.GetInt16(2),
                                                                                                 ExpMonth = skill.GetInt16(3), UpdatedBy = skill.GetString(4)
                                                                                             }).ToListAsync();*/
            _reader.NextResult(); //Education
            string _education = "[]";
            while (_reader.Read())
            {
                _education = _reader.NString(0);
            }

            /*List<CandidateEducation> _education = await _reader.FillList<CandidateEducation>(education => new()
                                                                                                          {
                                                                                                              ID = education.GetInt32(0), Degree = education.GetString(1), College = education.GetString(2),
                                                                                                              State = education.GetString(3), Country = education.GetString(4),
                                                                                                              Year = education.GetString(5), UpdatedBy = education.GetString(6)
                                                                                                          }).ToListAsync();*/
            _reader.NextResult(); //Experience
            string _experience = "[]";
            while (_reader.Read())
            {
                _experience = _reader.NString(0);
            }

            /*List<CandidateExperience> _experience = await _reader.FillList<CandidateExperience>(experience => new()
                                                                                                              {
                                                                                                                  ID = experience.GetInt32(0), Employer = experience.GetString(1),
                                                                                                                  Start = experience.GetString(2), End = experience.GetString(3),
                                                                                                                  Location = experience.GetString(4), Description = experience.GetString(5),
                                                                                                                  UpdatedBy = experience.GetString(6), Title = experience.GetString(7)
                                                                                                              }).ToListAsync();*/
            _reader.NextResult(); //Activity
            List<CandidateActivity> _activity = await _reader.FillList<CandidateActivity>(activity => new()
                                                                                                      {
                                                                                                          Requisition = activity.GetString(0), UpdatedDate = activity.GetDateTime(1),
                                                                                                          UpdatedBy = activity.GetString(2), Positions = activity.GetInt32(3),
                                                                                                          PositionFilled = activity.GetInt32(4), Status = activity.GetString(5),
                                                                                                          Notes = activity.GetString(6), ID = activity.GetInt32(7), Schedule = activity.GetBoolean(8),
                                                                                                          AppliesTo = activity.GetString(9), Color = activity.GetString(10), Icon = activity.GetString(11),
                                                                                                          DoRoleHaveRight = activity.GetBoolean(12), LastActionBy = activity.GetString(13),
                                                                                                          RequisitionID = activity.GetInt32(14), CandidateUpdatedBy = activity.GetString(15),
                                                                                                          CountSubmitted = activity.GetInt32(16), StatusCode = activity.GetString(17),
                                                                                                          ShowCalendar = activity.GetBoolean(18), DateTimeInterview = activity.NDateTime(19),
                                                                                                          TypeOfInterview = activity.GetString(20), PhoneNumber = activity.NString(21),
                                                                                                          InterviewDetails = activity.NString(22), Undone = activity.GetBoolean(23)
                                                                                                      }).ToListAsync();
            _reader.NextResult(); //Managers

            _reader.NextResult(); //Documents
            string _documents = "[]";
            while (_reader.Read())
            {
                _documents = _reader.NString(0);
            }

            /*List<CandidateDocument> _documents = await _reader.FillList<CandidateDocument>(document => new()
                                                                                                       {
                                                                                                           ID = document.GetInt32(0), Name = document.GetString(1), Location = document.GetString(2),
                                                                                                           Notes = document.GetString(3), UpdatedBy = $"{document.NDateTime(4)} [{document.NString(5)}]",
                                                                                                           DocumentType = document.GetString(6), InternalFileName = document.GetString(7),
                                                                                                           DocumentTypeID = document.GetInt32(8)
                                                                                                       }).ToListAsync();*/
            await _reader.CloseAsync();

            await _connection.CloseAsync();

            //Candidate Rating
            List<CandidateRating> _rating = [];
            if (!_candRating.NullOrWhiteSpace())
            {
                string[] _ratingArray = _candRating.Split('?');
                _rating.AddRange(_ratingArray
                                .Select(str => new
                                               {
                                                   _str = str,
                                                   _innerArray = str.Split('^')
                                               })
                                .Where(t => t._innerArray.Length == 4)
                                .Select(t => new CandidateRating(t._innerArray[0].Replace("  ", " ").ToDateTime("M/d/yy h:mm:ss tt"), t._innerArray[1],
                                                                 t._innerArray[2].ToByte(), t._innerArray[3])));

                _rating = _rating.OrderByDescending(x => x.Date).ToList();
            }

            //Candidate MPC
            List<CandidateMPC> _mpc = [];
            if (_candMPC.NullOrWhiteSpace())
            {
                return new Dictionary<string, object>
                       {
                           {
                               "Candidate", _candidate
                           },
                           {
                               "Notes", _notes
                           },
                           {
                               "Skills", _skills
                           },
                           {
                               "Education", _education
                           },
                           {
                               "Experience", _experience
                           },
                           {
                               "Activity", _activity
                           },
                           {
                               "Rating", _rating
                           },
                           {
                               "MPC", _mpc
                           },
                           {
                               "RatingMPC", null
                           },
                           {
                               "Document", _documents
                           }
                       };
            }

            string[] _mpcArray = _candMPC.Split('?');
            _mpc.AddRange(_mpcArray
                         .Select(str => new
                                        {
                                            _str = str,
                                            _innerArray = str.Split('^')
                                        })
                         .Where(t => t._innerArray.Length == 4)
                         .Select(t => new CandidateMPC(t._innerArray[0].Replace("  ", " ").ToDateTime("M/d/yy h:mm:ss tt"), t._innerArray[1], t._innerArray[2].ToBoolean(),
                                                       t._innerArray[3])));

            _mpc = _mpc.OrderByDescending(x => x.Date).ToList();

            int _ratingFirst = 0;
            bool _mpcFirst = false;
            string _ratingComments = "", _mpcComments = "";
            if (!_candRating.NullOrWhiteSpace())
            {
                CandidateRating _ratingFirstCandidate = _rating.FirstOrDefault();
                if (_ratingFirstCandidate != null)
                {
                    _ratingFirst = _ratingFirstCandidate.Rating;
                    _ratingComments = _ratingFirstCandidate.Comments;
                }
            }

            if (!_candMPC.NullOrWhiteSpace())
            {
                CandidateMPC _mpcFirstCandidate = _mpc.FirstOrDefault();
                if (_mpcFirstCandidate != null)
                {
                    _mpcFirst = _mpcFirstCandidate.MPC;
                    _mpcComments = _mpcFirstCandidate.Comments;
                }
            }

            CandidateRatingMPC _ratingMPC = new(candidateID, _ratingFirst, _ratingComments, _mpcFirst, _mpcComments);

            return new Dictionary<string, object>
                   {
                       {
                           "Candidate", _candidate
                       },
                       {
                           "Notes", _notes
                       },
                       {
                           "Skills", _skills
                       },
                       {
                           "Education", _education
                       },
                       {
                           "Experience", _experience
                       },
                       {
                           "Activity", _activity
                       },
                       {
                           "Rating", _rating
                       },
                       {
                           "MPC", _mpc
                       },
                       {
                           "RatingMPC", _ratingMPC
                       },
                       {
                           "Document", _documents
                       }
                   };
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error getting candidate details. {ExceptionMessage}", ex.Message);
            return null;
        }
    }

    private static string GetCandidateLocation(CandidateDetails candidateDetails, string stateName)
    {
        string _location = "";

        if (!candidateDetails.City.NullOrWhiteSpace())
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
    public async Task<Dictionary<string, object>> GetGridCandidates([FromBody] CandidateSearch searchModel = null)
    {
        try
        {
            await using SqlConnection _connection = new(Start.ConnectionString);
            List<Candidate> _candidates = [];

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

            await _connection.OpenAsync();
            await using SqlDataReader _reader = await _command.ExecuteReaderAsync();

            await _reader.ReadAsync();
            int _count = _reader.GetInt32(0);

            await _reader.NextResultAsync();
            while (await _reader.ReadAsync())
            {
                string _location = _reader.GetString(4);
                if (_location.StartsWith(","))
                {
                    _location = _location[1..].Trim();
                }

                _candidates.Add(new()
                                {
                                    ID = _reader.GetInt32(0), Name = _reader.GetString(1), Phone = _reader.GetString(2), Email = _reader.GetString(3), Location = _location, Updated = _reader.GetString(5),
                                    Status = _reader.GetString(6), MPC = _reader.GetBoolean(7), Rating = _reader.GetByte(8), OriginalResume = false, FormattedResume = false
                                });
            }

            await _reader.CloseAsync();

            await _connection.CloseAsync();

            return new()
                   {
                       {
                           "Candidates", _candidates
                       },
                       {
                           "Count", _count
                       }
                   };
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error getting grid candidates. {ExceptionMessage}", ex.Message);
            return null;
        }
    }

    /// <summary>
    ///     Asynchronously saves the details of a candidate to the database.
    /// </summary>
    /// <param name="candidateDetails">The details of the candidate to be saved.</param>
    /// <param name="jsonPath">The path to the JSON file containing the email template.</param>
    /// <param name="userName">The username of the user performing the operation. Default is an empty string.</param>
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
    [HttpPost]
    public async Task<int> SaveCandidate(CandidateDetails candidateDetails, string jsonPath, string userName = "", string emailAddress = "maniv@titan-techs.com")
    {
        try
        {
            if (candidateDetails == null)
            {
                return -1;
            }

            await using SqlConnection _connection = new(Start.ConnectionString);
            await _connection.OpenAsync();
            try
            {
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
                    if (!_templateSingle.CC.NullOrWhiteSpace())
                    {
                        string[] _ccArray = _templateSingle.CC.Split(",");
                        foreach (string _cc in _ccArray)
                        {
                            _emailCC.Add(_cc, _cc);
                        }
                    }

                    _templateSingle.Subject = _templateSingle.Subject.Replace("$TODAY$", DateTime.Today.CultureDate())
                                                             .Replace("$FULL_NAME$", $"{candidateDetails.FirstName} {candidateDetails.LastName}")
                                                             .Replace("$FIRST_NAME$", candidateDetails.FirstName)
                                                             .Replace("$LAST_NAME$", candidateDetails.LastName)
                                                             .Replace("$CAND_LOCATION$", GetCandidateLocation(candidateDetails, _stateName))
                                                             .Replace("$CAND_PHONE_PRIMARY$", candidateDetails.Phone1.StripPhoneNumber().FormatPhoneNumber())
                                                             .Replace("$CAND_SUMMARY$", candidateDetails.Summary)
                                                             .Replace("$LOGGED_USER$", userName);
                    _templateSingle.Template = _templateSingle.Template.Replace("$TODAY$", DateTime.Today.CultureDate())
                                                              .Replace("$FULL_NAME$", $"{candidateDetails.FirstName} {candidateDetails.LastName}")
                                                              .Replace("$FIRST_NAME$", candidateDetails.FirstName)
                                                              .Replace("$LAST_NAME$", candidateDetails.LastName)
                                                              .Replace("$CAND_LOCATION$", GetCandidateLocation(candidateDetails, _stateName))
                                                              .Replace("$CAND_PHONE_PRIMARY$", candidateDetails.Phone1.StripPhoneNumber().FormatPhoneNumber())
                                                              .Replace("$CAND_SUMMARY$", candidateDetails.Summary)
                                                              .Replace("$LOGGED_USER$", userName);

                    // GMailSend.SendEmail(jsonPath, emailAddress, _emailCC, _emailAddresses, _templateSingle.Subject, _templateSingle.Template, null);
                }
            }
            catch
            {
                // ignored
            }

            await _connection.CloseAsync();

            return 0;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error saving candidate. {ExceptionMessage}", ex.Message);
            return -1;
        }
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
    public async Task<string> SaveEducation(CandidateEducation education, int candidateID, string user)
    {
        string _returnVal = "[]";
        if (education == null)
        {
            return _returnVal;
        }

        await using SqlConnection _connection = new(Start.ConnectionString);
        await _connection.OpenAsync();
        try
        {
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
            await using SqlDataReader _reader = await _command.ExecuteReaderAsync();

            while (_reader.Read())
            {
                _returnVal = _reader.NString(0);
            }

            await _reader.CloseAsync();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error saving education. {ExceptionMessage}", ex.Message);
            //
        }

        await _connection.CloseAsync();

        return _returnVal;
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
    public async Task<string> SaveExperience(CandidateExperience experience, int candidateID, string user)
    {
        //List<CandidateExperience> _experiences = [];
        string _returnVal = "[]";
        if (experience == null)
        {
            return _returnVal;
        }

        await using SqlConnection _connection = new(Start.ConnectionString);
        await _connection.OpenAsync();
        try
        {
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
            await using SqlDataReader _reader = await _command.ExecuteReaderAsync();

            while (_reader.Read())
            {
                /*_experiences.Add(new()
                                 {
                                     ID = _reader.GetInt32(0),
                                     Employer = _reader.GetString(1),
                                     Start = _reader.GetString(2),
                                     End = _reader.GetString(3),
                                     Location = _reader.GetString(4),
                                     Description = _reader.GetString(5),
                                     UpdatedBy = _reader.GetString(6),
                                     Title = _reader.GetString(7)
                                 });*/
                _returnVal = _reader.NString(0);
            }

            await _reader.CloseAsync();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error saving experience. {ExceptionMessage}", ex.Message);
            //
        }

        await _connection.CloseAsync();

        return _returnVal;
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
    public async Task<string> SaveNotes(CandidateNotes candidateNote, int candidateID, string user)
    {
        string _returnVal = "[]";
        if (candidateNote == null)
        {
            return _returnVal;
        }

        await using SqlConnection _connection = new(Start.ConnectionString);
        await _connection.OpenAsync();
        try
        {
            await using SqlCommand _command = new("SaveNote", _connection);
            _command.CommandType = CommandType.StoredProcedure;
            _command.Int("Id", candidateNote.ID);
            _command.Int("CandidateID", candidateID);
            _command.Varchar("Note", -1, candidateNote.Notes);
            _command.Bit("IsPrimary", false);
            _command.Varchar("EntityType", 5, "CND");
            _command.Varchar("User", 10, user);
            await using SqlDataReader _reader = await _command.ExecuteReaderAsync();

            while (_reader.Read())
            {
                _returnVal = _reader.NString(0);
            }

            await _reader.CloseAsync();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error saving notes. {ExceptionMessage}", ex.Message);
            //
        }

        await _connection.CloseAsync();

        return _returnVal;
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
    public async Task<string> SaveSkill(CandidateSkills skill, int candidateID, string user)
    {
        string _returnVal = "[]";
        if (skill == null)
        {
            return _returnVal;
        }

        await using SqlConnection _connection = new(Start.ConnectionString);
        await _connection.OpenAsync();
        try
        {
            await using SqlCommand _command = new("SaveSkill", _connection);
            _command.CommandType = CommandType.StoredProcedure;
            _command.Int("EntitySkillId", skill.ID);
            _command.Varchar("Skill", 100, skill.Skill);
            _command.Int("CandidateID", candidateID);
            _command.SmallInt("LastUsed", skill.LastUsed);
            _command.SmallInt("ExpMonth", skill.ExpMonth);
            _command.Varchar("User", 10, user);
            await using SqlDataReader _reader = await _command.ExecuteReaderAsync();

            while (_reader.Read())
            {
                _returnVal = _reader.NString(0);
            }

            await _reader.CloseAsync();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error saving skill. {ExceptionMessage}", ex.Message);
        }

        await _connection.CloseAsync();

        return _returnVal;
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
    public async Task<List<KeyValues>> SearchCandidates(string filter)
    {
        await using SqlConnection _connection = new(Start.ConnectionString);
        List<KeyValues> companyNames = [];
        try
        {
            await using SqlCommand _command = new("SearchCandidates", _connection);
            _command.CommandType = CommandType.StoredProcedure;
            _command.Varchar("Name", 30, filter);

            await _connection.OpenAsync();

            await using SqlDataReader _reader = await _command.ExecuteReaderAsync();

            companyNames = await _reader.FillList<KeyValues>(keyValue => new()
                                                                         {
                                                                             Key = keyValue.GetString(0),
                                                                             Value = keyValue.GetString(0)
                                                                         }).ToListAsync();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error searching companies. {ExceptionMessage}", ex.Message);
        }

        await _connection.CloseAsync();

        return companyNames;
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
    [HttpPost, RequestSizeLimit(62_914_560)]
    public async Task<string> UploadDocument(IFormFile file)
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
        await _connection.OpenAsync();
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
            await using SqlDataReader _reader = await _command.ExecuteReaderAsync();

            while (_reader.Read())
            {
                _returnVal = _reader.NString(0);
            }

            await _reader.CloseAsync();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error saving candidate document. {ExceptionMessage}", ex.Message);
        }

        return _returnVal;
    }
}