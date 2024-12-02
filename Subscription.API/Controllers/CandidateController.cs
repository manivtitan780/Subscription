#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.API
// File Name:           CandidateController.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu
// Created On:          05-06-2024 20:05
// Last Updated On:     12-02-2024 20:12
// *****************************************/

#endregion

namespace Subscription.API.Controllers;

[ApiController, Route("api/[controller]/[action]")]
public class CandidateController : ControllerBase
{
	/// <summary>
	///     Retrieves the detailed information of a candidate by their ID.
	/// </summary>
	/// <param name="candidateID">The ID of the candidate.</param>
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
        List<CandidateNotes> _notes = await _reader.FillList<CandidateNotes>(note => new()
                                                                                     {
                                                                                         ID = note.GetInt32(0), UpdatedDate = note.GetDateTime(1), UpdatedBy = note.GetString(2),
                                                                                         Notes = note.GetString(3)
                                                                                     }).ToListAsync();
        _reader.NextResult(); //Skills
        List<CandidateSkills> _skills = await _reader.FillList<CandidateSkills>(skill => new()
                                                                                         {
                                                                                             ID = skill.GetInt32(0), Skill = skill.GetString(1), LastUsed = skill.GetInt16(2),
                                                                                             ExpMonth = skill.GetInt16(3), UpdatedBy = skill.GetString(4)
                                                                                         }).ToListAsync();
        _reader.NextResult(); //Education
        List<CandidateEducation> _education = await _reader.FillList<CandidateEducation>(education => new()
                                                                                                      {
                                                                                                          ID = education.GetInt32(0), Degree = education.GetString(1), College = education.GetString(2),
                                                                                                          State = education.GetString(3), Country = education.GetString(4),
                                                                                                          Year = education.GetString(5), UpdatedBy = education.GetString(6)
                                                                                                      }).ToListAsync();
        _reader.NextResult(); //Experience
        List<CandidateExperience> _experience = await _reader.FillList<CandidateExperience>(experience => new()
                                                                                                          {
                                                                                                              ID = experience.GetInt32(0), Employer = experience.GetString(1),
                                                                                                              Start = experience.GetString(2), End = experience.GetString(3),
                                                                                                              Location = experience.GetString(4), Description = experience.GetString(5),
                                                                                                              UpdatedBy = experience.GetString(6), Title = experience.GetString(7)
                                                                                                          }).ToListAsync();
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
        List<CandidateDocument> _documents = await _reader.FillList<CandidateDocument>(document => new()
                                                                                                   {
                                                                                                       ID = document.GetInt32(0), Name = document.GetString(1), Location = document.GetString(2),
                                                                                                       Notes = document.GetString(3), UpdatedBy = $"{document.NDateTime(4)} [{document.NString(5)}]",
                                                                                                       DocumentType = document.GetString(6), InternalFileName = document.GetString(7),
                                                                                                       DocumentTypeID = document.GetInt32(8)
                                                                                                   }).ToListAsync();
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
        await using SqlConnection _connection = new(Start.ConnectionString);
        List<Candidate> _candidates = [];

        await using SqlCommand _command = new("GetCandidates", _connection);
        _command.CommandType = CommandType.StoredProcedure;
        _command.Int("RecordsPerPage", searchModel.ItemCount);
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

	/// <summary>
	///     Saves a candidate's education record to the database.
	/// </summary>
	/// <param name="education">The education record of the candidate to be saved.</param>
	/// <param name="candidateID">The ID of the candidate whose education record is to be saved.</param>
	/// <param name="user">The user who is performing the save operation.</param>
	/// <returns>A dictionary containing the updated list of education records for the candidate.</returns>
	/// <remarks>
	///     This method connects to the database, executes a stored procedure to save the education record,
	///     and returns a dictionary containing the updated list of education records.
	///     If the operation is successful, the dictionary will contain a list of the candidate's education records.
	/// </remarks>
	[HttpPost]
    public async Task<Dictionary<string, object>> SaveEducation(CandidateEducation education, int candidateID, string user)
    {
        List<CandidateEducation> _education = [];
        if (education == null)
        {
            return new()
                   {
                       {
                           "Education", _education
                       }
                   };
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
            if (_reader.HasRows)
            {
                while (_reader.Read())
                {
                    _education.Add(new(_reader.GetInt32(0), _reader.GetString(1), _reader.GetString(2), _reader.GetString(3), _reader.GetString(4), 
                                       _reader.GetString(5), _reader.GetString(6)));
                }
            }

            await _reader.CloseAsync();
        }
        catch
        {
            //
        }

        await _connection.CloseAsync();

        return new()
               {
                   {
                       "Education", _education
                   }
               };
    }

	/// <summary>
	///     Saves a candidate's experience record to the database.
	/// </summary>
	/// <param name="experience">The experience record of the candidate to be saved.</param>
	/// <param name="candidateID">The ID of the candidate whose experience record is to be saved.</param>
	/// <param name="user">The user who is performing the save operation.</param>
	/// <returns>A dictionary containing the updated list of experience records for the candidate.</returns>
	/// <remarks>
	///     This method connects to the database, executes a stored procedure to save the experience record,
	///     and returns a dictionary containing the updated list of experience records.
	///     If the operation is successful, the dictionary will contain a list of updated experience records for the
	///     candidate.
	/// </remarks>
	[HttpPost]
    public async Task<Dictionary<string, object>> SaveExperience(CandidateExperience experience, int candidateID, string user)
    {
        List<CandidateExperience> _experiences = [];
        if (experience == null)
        {
            return new()
                   {
                       {
                           "Experience", _experiences
                       }
                   };
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
            if (_reader.HasRows)
            {
                while (_reader.Read())
                {
                    _experiences.Add(new(_reader.GetInt32(0), _reader.GetString(1), _reader.GetString(2), _reader.GetString(3), _reader.GetString(4),
                                         _reader.GetString(5), _reader.GetString(6), _reader.GetString(7)));
                }
            }

            await _reader.CloseAsync();
        }
        catch
        {
            //
        }

        await _connection.CloseAsync();

        return new()
               {
                   {
                       "Experience", _experiences
                   }
               };
    }

	/// <summary>
	///     Saves a candidate's skill to the database.
	/// </summary>
	/// <param name="skill">The skill of the candidate to be saved.</param>
	/// <param name="candidateID">The ID of the candidate whose skill is to be saved.</param>
	/// <param name="user">The user who is performing the save operation.</param>
	/// <returns>A dictionary containing the updated list of skills for the candidate.</returns>
	/// <remarks>
	///     This method connects to the database, executes a stored procedure to save the skill,
	///     and returns a dictionary containing the updated list of skills.
	///     If the operation is successful, the dictionary will contain a list of remaining skills for the
	///     candidate.
	/// </remarks>
	[HttpPost]
    public async Task<Dictionary<string, object>> SaveSkill(CandidateSkills skill, int candidateID, string user)
    {
        await Task.Delay(1);
        List<CandidateSkills> _skills = [];
        if (skill == null)
        {
            return new()
                   {
                       {
                           "Skills", _skills
                       }
                   };
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
            if (_reader.HasRows)
            {
                while (_reader.Read())
                {
                    _skills.Add(new(_reader.GetInt32(0), _reader.GetString(1), _reader.GetInt16(2), _reader.GetInt16(3), _reader.GetString(4)));
                }
            }

            await _reader.CloseAsync();
        }
        catch
        {
            //TODO: Log the exception
        }

        await _connection.CloseAsync();

        return new()
               {
                   {
                       "Skills", _skills
                   }
               };
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
        await using SqlCommand _command = new("SearchCandidates", _connection);
        _command.CommandType = CommandType.StoredProcedure;
        _command.Varchar("Name", 30, filter);

        await _connection.OpenAsync();

        await using SqlDataReader _reader = await _command.ExecuteReaderAsync();

        List<KeyValues> companyNames = await _reader.FillList<KeyValues>(keyValue => new()
                                                                                     {
                                                                                         Key = keyValue.GetString(0),
                                                                                         Value = keyValue.GetString(0)
                                                                                     }).ToListAsync();
        await _connection.CloseAsync();

        return companyNames;
    }
}