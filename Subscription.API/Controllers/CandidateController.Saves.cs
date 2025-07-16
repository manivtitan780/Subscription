#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.API
// File Name:           CandidateController.Saves.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu, Brijesh Dubey
// Created On:          07-16-2025 16:00
// Last Updated On:     07-16-2025 16:00
// *****************************************/

#endregion

namespace Subscription.API.Controllers;

/// <summary>
/// Partial class containing Save/POST operations for CandidateController.
/// Handles all create and update operations for candidates and related entities.
/// </summary>
public partial class CandidateController
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
    public async Task<ActionResult<string>> DuplicateCandidate(int candidateID, string user)
    {
        return await ExecuteQueryAsync("DuplicateCandidate", command =>
                                                             {
                                                                 command.Int("CandidateID", candidateID);
                                                                 command.Varchar("User", 10, user);
                                                             }, "DuplicateCandidate", "Error duplicating candidate.");
    }

    [HttpPost, SuppressMessage("ReSharper", "CollectionNeverQueried.Local")]
    public async Task<ActionResult<int>> SaveCandidate(CandidateDetails candidateDetails, string userName = "")
    {
        if (candidateDetails == null)
        {
            return NotFound(-1);
        }

        return await ExecuteScalarAsync<int>("SaveCandidate", command =>
        {
            command.Int("@ID", candidateDetails.CandidateID, true);
            command.Varchar("@FirstName", 50, candidateDetails.FirstName);
            command.Varchar("@MiddleName", 50, candidateDetails.MiddleName);
            command.Varchar("@LastName", 50, candidateDetails.LastName);
            command.Varchar("@Title", 200, candidateDetails.Title);
            command.Int("@Eligibility", candidateDetails.EligibilityID);
            command.Decimal("@HourlyRate", 6, 2, candidateDetails.HourlyRate);
            command.Decimal("@HourlyRateHigh", 6, 2, candidateDetails.HourlyRateHigh);
            command.Decimal("@SalaryLow", 9, 2, candidateDetails.SalaryLow);
            command.Decimal("@SalaryHigh", 9, 2, candidateDetails.SalaryHigh);
            command.Int("@Experience", candidateDetails.ExperienceID);
            command.Bit("@Relocate", candidateDetails.Relocate);
            command.Varchar("@JobOptions", 50, candidateDetails.JobOptions);
            command.Char("@Communication", 1, candidateDetails.Communication);
            command.Varchar("@Keywords", 500, candidateDetails.Keywords.Length > 500 ? candidateDetails.Keywords[..500] : candidateDetails.Keywords);
            command.Varchar("@Status", 3, "AVL");
            command.Varchar("@TextResume", -1, candidateDetails.TextResume);
            command.Varchar("@Address1", 255, candidateDetails.Address1);
            command.Varchar("@Address2", 255, candidateDetails.Address2);
            command.Varchar("@City", 50, candidateDetails.City);
            command.Int("@StateID", candidateDetails.StateID);
            command.Varchar("@ZipCode", 20, candidateDetails.ZipCode);
            command.Varchar("@Email", 255, candidateDetails.Email);
            command.Varchar("@Phone1", 15, candidateDetails.Phone1.StripPhoneNumber());
            command.Varchar("@Phone2", 15, candidateDetails.Phone2.StripPhoneNumber());
            command.Varchar("@Phone3", 15, candidateDetails.Phone3.StripPhoneNumber());
            command.SmallInt("@Phone3Ext", candidateDetails.PhoneExt.ToInt16());
            command.Varchar("@LinkedIn", 255, candidateDetails.LinkedIn);
            command.Varchar("@Facebook", 255, candidateDetails.Facebook);
            command.Varchar("@Twitter", 255, candidateDetails.Twitter);
            command.Varchar("@Google", 255, candidateDetails.GooglePlus);
            command.Bit("@Refer", candidateDetails.Refer);
            command.Varchar("@ReferAccountMgr", 10, candidateDetails.ReferAccountManager);
            command.Varchar("@TaxTerm", 10, candidateDetails.TaxTerm);
            command.Bit("@Background", candidateDetails.Background);
            command.Varchar("@Summary", -1, candidateDetails.Summary);
            command.Varchar("@Objective", -1, "");
            command.Bit("@EEO", candidateDetails.EEO);
            command.Varchar("@RelocNotes", 200, candidateDetails.RelocationNotes);
            command.Varchar("@SecurityClearanceNotes", 200, candidateDetails.SecurityNotes);
            command.Varchar("@User", 10, userName);
        }, "SaveCandidate", "Error saving candidate.");
    }

    [HttpPost]
    public async Task<ActionResult<string>> SaveCandidateActivity(CandidateActivity activity, int candidateID, string user, string roleID = "RS", bool isCandidateScreen = true,
                                                                  string emailAddress = "maniv@titan-techs.com", string uploadPath = "", string jsonPath = "")
    {
        // Memory optimization: Use ExecuteReaderAsync<T> for multiple result set processing
        return await ExecuteReaderAsync("SaveCandidateActivity", command =>
        {
            command.Int("SubmissionId", activity.ID);
            command.Int("CandidateID", candidateID);
            command.Int("RequisitionID", activity.RequisitionID);
            command.Varchar("Notes", 1000, activity.Notes);
            command.Char("Status", 3, activity.NewStatusCode.NullOrWhiteSpace() ? activity.StatusCode : activity.NewStatusCode);
            command.Varchar("User", 10, user);
            command.Bit("ShowCalendar", activity.ShowCalendar);
            command.Date("DateTime", activity.DateTimeInterview == DateTime.MinValue ? DBNull.Value : activity.DateTimeInterview);
            command.Char("Type", 1, activity.TypeOfInterview);
            command.Varchar("PhoneNumber", 20, activity.PhoneNumber);
            command.Varchar("InterviewDetails", 2000, activity.InterviewDetails);
            command.Bit("UpdateSchedule", false);
            command.Bit("CandScreen", isCandidateScreen);
            command.Char("RoleID", 2, roleID);
        }, async reader =>
        {
            string _activities = "[]";
            
            // Result Set 1: Activities
            while (await reader.ReadAsync())
            {
                _activities = reader.NString(0);
            }

            // Result Set 2: Candidate and requisition details
            await reader.NextResultAsync();
            await reader.ReadAsync();
            (string _firstName, string _lastName, string _reqCode, string _reqTitle, string _company) = 
                (reader.NString(0), reader.NString(1), reader.NString(2), reader.NString(3), reader.NString(8));

            // Result Set 3: Email templates
            List<EmailTemplates> _templates = [];
            await reader.NextResultAsync();
            while (await reader.ReadAsync())
            {
                _templates.Add(new(reader.NString(0), reader.NString(1), reader.NString(2)));
            }

            // Result Set 4: Email addresses
            Dictionary<string, string> _emailAddresses = new();
            await reader.NextResultAsync();
            while (await reader.ReadAsync())
            {
                _emailAddresses.Add(reader.NString(0), reader.NString(1));
            }

            // Process email notifications
            if (_templates.Count != 0)
            {
                await ProcessRequisitionEmailAsync(_templates, _firstName, _lastName, _reqCode, _reqTitle, _company, 
                    activity.Notes, activity.Status, user);
            }

            return _activities;
        }, "SaveCandidateActivity", "Error saving candidate activity.");
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
        
        // Memory optimization: Use ExecuteReaderAsync<T> for multiple result set processing
        return await ExecuteReaderAsync("SaveCandidateWithSubmissions", command =>
        {
            command.Int("@ID", _candidateDetails.CandidateID, true);
            command.Varchar("@FirstName", 50, _candidateDetails.FirstName);
            command.Varchar("@MiddleName", 50, _candidateDetails.MiddleName);
            command.Varchar("@LastName", 50, _candidateDetails.LastName);
            command.Varchar("@Title", 200, _candidateDetails.Title);
            command.Int("@Eligibility", _candidateDetails.EligibilityID);
            command.Decimal("@HourlyRate", 6, 2, _candidateDetails.HourlyRate);
            command.Decimal("@HourlyRateHigh", 6, 2, _candidateDetails.HourlyRateHigh);
            command.Decimal("@SalaryLow", 9, 2, _candidateDetails.SalaryLow);
            command.Decimal("@SalaryHigh", 9, 2, _candidateDetails.SalaryHigh);
            command.Int("@Experience", _candidateDetails.ExperienceID);
            command.Bit("@Relocate", _candidateDetails.Relocate);
            command.Varchar("@JobOptions", 50, _candidateDetails.JobOptions);
            command.Char("@Communication", 1, _candidateDetails.Communication);
            command.Varchar("@Keywords", 500, _candidateDetails.Keywords.Length > 500 ? _candidateDetails.Keywords[..500] : _candidateDetails.Keywords);
            command.Varchar("@Status", 3, "AVL");
            command.Varchar("@TextResume", -1, _candidateDetails.TextResume);
            command.Varchar("@OriginalResume", 255, candidateDetailsResume.ParsedCandidate.FileName);
            command.Varchar("@FormattedResume", 255, _candidateDetails.FormattedResume);
            command.Varchar("@OriginalFileID", 50, _internalFileName);
            command.Varchar("@FormattedFileID", 50, DBNull.Value);
            command.Varchar("@Address1", 255, _candidateDetails.Address1);
            command.Varchar("@Address2", 255, _candidateDetails.Address2);
            command.Varchar("@City", 50, _candidateDetails.City);
            command.Int("@StateID", _candidateDetails.StateID);
            command.Varchar("@ZipCode", 20, _candidateDetails.ZipCode);
            command.Varchar("@Email", 255, _candidateDetails.Email);
            command.Varchar("@Phone1", 15, _candidateDetails.Phone1.StripPhoneNumber());
            command.Varchar("@Phone2", 15, _candidateDetails.Phone2.StripPhoneNumber());
            command.Varchar("@Phone3", 15, _candidateDetails.Phone3.StripPhoneNumber());
            command.SmallInt("@Phone3Ext", _candidateDetails.PhoneExt.ToInt16());
            command.Varchar("@LinkedIn", 255, _candidateDetails.LinkedIn);
            command.Varchar("@Facebook", 255, _candidateDetails.Facebook);
            command.Varchar("@Twitter", 255, _candidateDetails.Twitter);
            command.Varchar("@Google", 255, _candidateDetails.GooglePlus);
            command.Bit("@Refer", _candidateDetails.Refer);
            command.Varchar("@ReferAccountMgr", 10, _candidateDetails.ReferAccountManager);
            command.Varchar("@TaxTerm", 10, _candidateDetails.TaxTerm);
            command.Bit("@Background", _candidateDetails.Background);
            command.Varchar("@Summary", -1, _candidateDetails.Summary);
            command.Varchar("@Objective", -1, "");
            command.Bit("@EEO", _candidateDetails.EEO);
            command.Varchar("@RelocNotes", 200, _candidateDetails.RelocationNotes);
            command.Varchar("@SecurityClearanceNotes", 200, _candidateDetails.SecurityNotes);
            command.Varchar("@User", 10, userName);
            command.Int("@RequisitionID", candidateDetailsResume.ParsedCandidate.RequisitionID);
            command.Varchar("@SubmissionNotes", 1000, candidateDetailsResume.ParsedCandidate.SubmissionNotes);
        }, async reader =>
        {
            List<EmailTemplates> _templates = [];
            List<KeyValues> _emailAddresses = [];
            string _stateName = "";
            int _candidateID = 0;

            // Result Set 1: Email templates
            while (await reader.ReadAsync())
            {
                _templates.Add(new(reader.NString(0), reader.NString(1), reader.NString(2)));
            }

            // Result Set 2: Email addresses
            await reader.NextResultAsync();
            while (await reader.ReadAsync())
            {
                _emailAddresses.Add(new() {KeyValue = reader.NString(0), Text = reader.NString(1)});
            }

            // Result Set 3: State name
            await reader.NextResultAsync();
            if (await reader.ReadAsync())
            {
                _stateName = reader.GetString(0);
            }

            // Result Set 4: Candidate ID
            await reader.NextResultAsync();
            if (await reader.ReadAsync())
            {
                _candidateID = reader.NInt32(0);
            }

            // Upload resume to blob storage
            await General.UploadToBlob(candidateDetailsResume.ParsedCandidate.DocumentBytes, $"{Start.AzureBlobContainer}/Candidate/{_candidateID}/{_internalFileName}");

            // Process email notifications
            if (_templates.Count > 0)
            {
                await ProcessCandidateEmailAsync(_templates, _candidateDetails, _stateName, userName, 
                    candidateDetailsResume.ParsedCandidate.DocumentBytes, 
                    candidateDetailsResume.ParsedCandidate.FileName, 
                    GetMimeType(candidateDetailsResume.ParsedCandidate.FileName));
            }

            return "0";
        }, "SaveCandidateWithResume", "Error saving candidate.");
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
    }

    [HttpPost]
    public async Task<Dictionary<string, object>> SaveMPC(CandidateRatingMPC mpc, string user)
    {
        try
        {
            if (mpc == null)
            {
                return new()
                {
                    {"MPCList", "[]"},
                    {"FirstMPC", null}
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

            // Memory optimization: Use centralized JSON processing helper
            if (_mpcNotes.NullOrWhiteSpace())
            {
                return new()
                {
                    {"MPCList", "[]"},
                    {"FirstMPC", mpc}
                };
            }

            JArray jsonArray = JArray.Parse(_mpcNotes);
            if (!jsonArray.Any())
            {
                return new()
                {
                    {"MPCList", "[]"},
                    {"FirstMPC", mpc}
                };
            }

            JArray sortedArray = new(jsonArray.OrderByDescending(obj => DateTime.Parse(obj["DateTime"]!.ToString())));
            JToken firstToken = sortedArray.FirstOrDefault();
            if (firstToken != null)
            {
                mpc.MPC = firstToken["MPC"].ToBoolean();
                mpc.MPCComments = firstToken["Comment"]?.ToString();
            }

            return new()
            {
                {"MPCList", sortedArray.ToString()},
                {"FirstMPC", mpc}
            };
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error saving MPC. {ExceptionMessage}", ex.Message);
            return new()
            {
                {"MPCList", "[]"},
                {"FirstMPC", mpc}
            };
        }
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
    }

    [HttpPost]
    public async Task<Dictionary<string, object>> SaveRating(CandidateRatingMPC rating, string user)
    {
        try
        {
            if (rating == null)
            {
                return new()
                {
                    {"RatingList", "[]"},
                    {"FirstRating", null}
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

            // Memory optimization: Use centralized JSON processing helper
            if (_ratingNotes.NullOrWhiteSpace())
            {
                return new()
                {
                    {"RatingList", "[]"},
                    {"FirstRating", rating}
                };
            }

            JArray jsonArray = JArray.Parse(_ratingNotes);
            if (!jsonArray.Any())
            {
                return new()
                {
                    {"RatingList", "[]"},
                    {"FirstRating", rating}
                };
            }

            JArray sortedArray = new(jsonArray.OrderByDescending(obj => DateTime.Parse(obj["DateTime"]!.ToString())));
            JToken firstToken = sortedArray.FirstOrDefault();
            if (firstToken != null)
            {
                rating.Rating = firstToken["Rating"].ToByte();
                rating.RatingComments = firstToken["Comment"]?.ToString();
            }

            return new()
            {
                {"RatingList", sortedArray.ToString()},
                {"FirstRating", rating}
            };
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error saving rating. {ExceptionMessage}", ex.Message);
            return new()
            {
                {"RatingList", "[]"},
                {"FirstRating", rating}
            };
        }
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
    }

    [HttpPost]
    public async Task<ActionResult<bool>> SubmitCandidateRequisition(int requisitionID, int candidateID, string notes = "", string user = "", string roleID = "RS")
    {
        return await ExecuteScalarAsync<bool>("SubmitCandidateRequisition", command =>
        {
            command.Int("RequisitionID", requisitionID);
            command.Int("CandidateID", candidateID);
            command.Varchar("Notes", 1000, notes);
            command.Varchar("User", 10, user);
            command.Char("RoleID", 2, roleID);
        }, "SubmitCandidateRequisition", "Error submitting candidate to requisition.");
    }

    [HttpPost]
    public async Task<ActionResult<string>> UndoCandidateActivity(int submissionID, string user, string roleID = "RS", bool isCandidateScreen = true)
    {
        return await ExecuteQueryAsync("UndoCandidateActivity", command =>
        {
            command.Int("SubmissionID", submissionID);
            command.Varchar("User", 10, user);
            command.Char("RoleID", 2, roleID);
            command.Bit("CandScreen", isCandidateScreen);
        }, "UndoCandidateActivity", "Error undoing candidate activity.");
    }

    [HttpPost]
    public async Task<ActionResult<string>> UpdateResume(IFormFile file)
    {
        return await ExecuteQueryAsync("UpdateCandidateResume", command =>
        {
            command.Varchar("FileName", 255, file.FileName);
            command.Int("FileSize", (int)file.Length);
        }, "UpdateResume", "Error updating candidate resume.");
    }

    [HttpPost]
    public async Task<ActionResult<string>> UploadDocument(IFormFile file)
    {
        return await ExecuteQueryAsync("UploadCandidateDocument", command =>
        {
            command.Varchar("FileName", 255, file.FileName);
            command.Int("FileSize", (int)file.Length);
        }, "UploadDocument", "Error uploading candidate document.");
    }
}