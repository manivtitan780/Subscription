#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.API
// File Name:           General.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu
// Created On:          04-20-2024 20:04
// Last Updated On:     12-13-2024 19:12
// *****************************************/

#endregion

#region Using

using System.Security.Cryptography;
using System.Text;

#endregion

namespace Subscription.API.Code;

public static class General
{
    public static byte[] ComputeHashWithSalt(string input, byte[] salt)
    {
        byte[] inputBytes = Encoding.UTF8.GetBytes(input);
        byte[] inputWithSalt = new byte[inputBytes.Length + salt.Length];
        Buffer.BlockCopy(inputBytes, 0, inputWithSalt, 0, inputBytes.Length);
        Buffer.BlockCopy(salt, 0, inputWithSalt, inputBytes.Length, salt.Length);

        return SHA512PasswordHash(inputWithSalt);
    }

    public static async Task SetCache()
    {
        RedisService _service = new(Start.CacheServer, Start.CachePort.ToInt32(), Start.Access, false);
        bool _keyExists = await _service.CheckKeyExists(CacheObjects.Companies.ToString());

        if (!_keyExists)
        {
            await using SqlConnection _connection = new(Start.ConnectionString);
            await _connection.OpenAsync();

            await using SqlCommand _command = new("SetCache", _connection);
            _command.CommandType = CommandType.StoredProcedure;
            _command.Bit("@Companies", true);
            _command.Bit("@CompanyContact", true);
            _command.Bit("@Designations", true);
            _command.Bit("@DocumentType", true);
            _command.Bit("@Education", true);
            _command.Bit("@Eligibility", true);
            _command.Bit("@Experience", true);
            _command.Bit("@JobOptions", true);
            _command.Bit("@LeadIndustry", true);
            _command.Bit("@LeadSource", true);
            _command.Bit("@LeadStatus", true);
            _command.Bit("@NAICS", true);
            _command.Bit("@Roles", true);
            _command.Bit("@Skills", true);
            _command.Bit("@States", true);
            _command.Bit("@Status", true);
            _command.Bit("@TaxTerms", true);
            _command.Bit("@Users", true);
            _command.Bit("@Workflow", true);
            _command.Bit("@ZipCodes", true);

            await using SqlDataReader _reader = await _command.ExecuteReaderAsync();

            List<IntValues> _companies = [];
            _companies = await SetIntValues(_reader);

            await _reader.NextResultAsync();
            List<CompanyContactList> _companyContacts = await _reader.FillList<CompanyContactList>(contact => new()
                                                                                                              {
                                                                                                                  ID = contact.GetInt32(0),
                                                                                                                  ContactName = contact.GetString(2),
                                                                                                                  CompanyID = contact.GetInt32(1)
                                                                                                              }).ToListAsync();

            await _reader.NextResultAsync();
            List<IntValues> _titles = [];
            _titles = await SetIntValues(_reader);

            await _reader.NextResultAsync();
            List<IntValues> _documentTypes = [];
            _documentTypes = await SetIntValues(_reader);

            await _reader.NextResultAsync();
            List<IntValues> _educations = [];
            _educations = await SetIntValues(_reader);

            await _reader.NextResultAsync();
            List<IntValues> _eligibilities = [];
            _eligibilities = await SetIntValues(_reader);

            await _reader.NextResultAsync();
            List<IntValues> _experiences = [];
            _experiences = await SetIntValues(_reader);

            await _reader.NextResultAsync();
            List<KeyValues> _jobOptions = [];
            _jobOptions = await SetKeyValues(_reader);

            await _reader.NextResultAsync();
            List<IntValues> _leadIndustries = [];
            _leadIndustries = await SetIntValues(_reader, 2);

            await _reader.NextResultAsync();
            List<IntValues> _leadSources = [];
            _leadSources = await SetIntValues(_reader, 2);

            await _reader.NextResultAsync();
            List<IntValues> _leadStatuses = [];
            _leadStatuses = await SetIntValues(_reader, 2);

            await _reader.NextResultAsync();
            List<IntValues> _naics = [];
            _naics = await SetIntValues(_reader);

            await _reader.NextResultAsync();
            List<Role> _roles = await _reader.FillList<Role>(role => new()
                                                                     {
                                                                         ID = role.GetByte(0),
                                                                         RoleName = role.GetString(1),
                                                                         Description = role.GetString(2),
                                                                         CreateOrEditCompany = role.GetBoolean(3),
                                                                         CreateOrEditCandidate = role.GetBoolean(4),
                                                                         ViewAllCompanies = role.GetBoolean(5),
                                                                         ViewMyCompanyProfile = role.GetBoolean(6),
                                                                         EditMyCompanyProfile = role.GetBoolean(7),
                                                                         CreateOrEditRequisition = role.GetBoolean(8),
                                                                         ViewOnlyMyCandidates = role.GetBoolean(9),
                                                                         ViewAllCandidates = role.GetBoolean(10),
                                                                         ManageSubmittedCandidates = role.GetBoolean(11),
                                                                         DownloadOriginal = role.GetBoolean(12),
                                                                         DownloadFormatted = role.GetBoolean(13),
                                                                         ViewRequisitions = role.GetBoolean(14)
                                                                     }).ToListAsync();

            await _reader.NextResultAsync();
            List<IntValues> _skills = [];
            _skills = await SetIntValues(_reader);

            await _reader.NextResultAsync();
            List<IntValues> _states = [];
            _states = await SetIntValues(_reader);

            await _reader.NextResultAsync();
            List<StatusCode> _statusCodes = await _reader.FillList<StatusCode>(status => new()
                                                                                         {
                                                                                             ID = status.GetInt32(6),
                                                                                             Code = status.GetString(0),
                                                                                             Status = status.GetString(1),
                                                                                             Icon = status.NString(2),
                                                                                             AppliesToCode = status.GetString(3),
                                                                                             SubmitCandidate = status.GetBoolean(4),
                                                                                             ShowCommission = status.GetBoolean(5)
                                                                                         }).ToListAsync();

            await _reader.NextResultAsync();
            List<KeyValues> _taxTerms = [];
            _taxTerms = await SetKeyValues(_reader);

            await _reader.NextResultAsync();
            List<UserList> _users = await _reader.FillList<UserList>(user => new()
                                                                             {
                                                                                 UserName = user.GetString(0),
                                                                                 Role = user.GetByte(1)
                                                                             }).ToListAsync();

            await _reader.NextResultAsync();
            List<AppWorkflow> _workflows = await _reader.FillList<AppWorkflow>(workflow => new()
                                                                                           {
                                                                                               ID = workflow.GetInt32(0),
                                                                                               Step = workflow.GetString(1),
                                                                                               Next = workflow.NString(2),
                                                                                               IsLast = workflow.GetBoolean(3),
                                                                                               RoleIDs = workflow.GetString(4),
                                                                                               Schedule = workflow.GetBoolean(5),
                                                                                               AnyStage = workflow.GetBoolean(6),
                                                                                               NextFull = "",
                                                                                               RoleFull = ""
                                                                                           }).ToListAsync();

            await _reader.NextResultAsync();
            List<Zip> _zips = await _reader.FillList<Zip>(zip => new()
                                                                 {
                                                                     ZipCode = zip.GetString(0),
                                                                     City = zip.GetString(1),
                                                                     State = zip.GetString(2),
                                                                     StateID = zip.GetInt32(3)
                                                                 }).ToListAsync();

            List<KeyValues> _communications =
            [
                new() {Key = "A", Value = "Average"},
                new() {Key = "X", Value = "Excellent"},
                new() {Key = "F", Value = "Fair"},
                new() {Key = "G", Value = "Good"}
            ];

            await _reader.CloseAsync();

            await _connection.CloseAsync();

            List<string> _keys =
            [
                CacheObjects.Companies.ToString(), CacheObjects.CompanyContacts.ToString(), CacheObjects.Titles.ToString(), CacheObjects.DocumentTypes.ToString(), CacheObjects.Education.ToString(),
                CacheObjects.Eligibility.ToString(), CacheObjects.Experience.ToString(), CacheObjects.JobOptions.ToString(), CacheObjects.LeadIndustries.ToString(),
                CacheObjects.LeadSources.ToString(), CacheObjects.LeadStatus.ToString(), CacheObjects.NAICS.ToString(), CacheObjects.Roles.ToString(), CacheObjects.Skills.ToString(),
                CacheObjects.States.ToString(), CacheObjects.StatusCodes.ToString(), CacheObjects.TaxTerms.ToString(), CacheObjects.Users.ToString(), CacheObjects.Workflow.ToString(),
                CacheObjects.Zips.ToString(), CacheObjects.Communications.ToString()
            ];

            List<object> _values =
            [
                _companies, _companyContacts, _titles, _documentTypes, _educations, _eligibilities, _experiences, _jobOptions, _leadIndustries, _leadSources, _leadStatuses, _naics, _roles, _skills,
                _states, _statusCodes, _taxTerms, _users, _workflows, _zips, _communications
            ];

            await _service.CreateBatchSet(_keys, _values);
        }
    }

    private static async Task<List<IntValues>> SetIntValues(SqlDataReader reader, byte keyType = 0) //0-Int32, 1=Int16, 2=Byte
    {
        return await reader.FillList<IntValues>(intValue => new()
                                                            {
                                                                Value = keyType switch
                                                                        {
                                                                            0 => intValue.GetInt32(0),
                                                                            1 => intValue.GetInt16(0),
                                                                            2 => intValue.GetByte(0),
                                                                            _ => 0
                                                                        },
                                                                Text = intValue.GetString(1)
                                                            }).ToListAsync();
    }

    private static async Task<List<KeyValues>> SetKeyValues(SqlDataReader reader)
    {
        return await reader.FillList<KeyValues>(keyValue => new()
                                                            {
                                                                Key = keyValue.GetString(0),
                                                                Value = keyValue.GetString(1)
                                                            }).ToListAsync();
    }

    /// <summary>
    ///     Computes the SHA-512 hash of the input text.
    /// </summary>
    /// <param name="inputText">The text to be hashed.</param>
    /// <returns>A byte array representing the SHA-512 hash of the input text.</returns>
    public static byte[] SHA512PasswordHash(string inputText) => SHA512.Create().ComputeHash(new UTF8Encoding().GetBytes(inputText));

    /// <summary>
    ///     Computes the SHA-512 hash of the input text.
    /// </summary>
    /// <param name="inputText">The text to be hashed.</param>
    /// <returns>A byte array representing the SHA-512 hash of the input text.</returns>
    public static byte[] SHA512PasswordHash(byte[] inputText) => SHA512.Create().ComputeHash(inputText);
}