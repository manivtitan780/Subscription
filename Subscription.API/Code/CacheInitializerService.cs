#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.API
// File Name:           CacheInitializerService.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu, Brijesh Dubey
// Created On:          07-28-2025 16:07
// Last Updated On:     07-28-2025 16:39
// *****************************************/

#endregion

#region Using

using Newtonsoft.Json;

#endregion

namespace Subscription.API.Code;

public class CacheInitializerService(RedisService redisService)
{
    [SuppressMessage("ReSharper", "CollectionNeverQueried.Local")]
    public async Task InitializeCacheAsync(string connectionString)
    {
        //RedisService _service = redisService;
        bool _keyExists = await redisService.CheckKeyExists(nameof(CacheObjects.Companies));

        if (!_keyExists)
        {
            await using SqlConnection _connection = new(connectionString);
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

            async Task<string> ReadCurrentResultAsync(SqlDataReader reader)
            {
                if (await reader.ReadAsync())
                {
                    return reader.NString(0, "[]");
                }
                return "[]";
            }

            string _companies = await ReadCurrentResultAsync(_reader);

            await _reader.NextResultAsync();
            string _companyContacts = await ReadCurrentResultAsync(_reader);

            await _reader.NextResultAsync();
            string _titles = await ReadCurrentResultAsync(_reader);

            await _reader.NextResultAsync();
            string _documentTypes = await ReadCurrentResultAsync(_reader);

            await _reader.NextResultAsync();
            string _educations = await ReadCurrentResultAsync(_reader);

            await _reader.NextResultAsync();
            string _eligibilities = await ReadCurrentResultAsync(_reader);

            await _reader.NextResultAsync();
            string _experiences = await ReadCurrentResultAsync(_reader);

            await _reader.NextResultAsync();
            string _jobOptions = await ReadCurrentResultAsync(_reader);

            await _reader.NextResultAsync();
            string _leadIndustries = await ReadCurrentResultAsync(_reader);

            await _reader.NextResultAsync();
            string _leadSources = await ReadCurrentResultAsync(_reader);

            await _reader.NextResultAsync();
            string _leadStatuses = await ReadCurrentResultAsync(_reader);

            await _reader.NextResultAsync();
            string _naics = await ReadCurrentResultAsync(_reader);

            await _reader.NextResultAsync();
            string _roles = await ReadCurrentResultAsync(_reader);

            await _reader.NextResultAsync();
            string _skills = await ReadCurrentResultAsync(_reader);

            await _reader.NextResultAsync();
            string _states = await ReadCurrentResultAsync(_reader);

            await _reader.NextResultAsync();
            string _statusCodes = await ReadCurrentResultAsync(_reader);

            await _reader.NextResultAsync();
            string _taxTerms = await ReadCurrentResultAsync(_reader);

            await _reader.NextResultAsync();
            string _users = await ReadCurrentResultAsync(_reader);

            await _reader.NextResultAsync();
            string _workflows = await ReadCurrentResultAsync(_reader);

            await _reader.NextResultAsync();
            string _zips = "[]";
            await _reader.ReadAsync();
            _zips = ((byte[])_reader[0]).DecompressGZip();

            await _reader.NextResultAsync();
            string _preferences = "[]";
            if (_reader.HasRows)
            {
                _preferences = await ReadCurrentResultAsync(_reader);
            }
            string _comms = "[{\"KeyValue\":\"A\",\"Text\":\"Average\"},{\"KeyValue\":\"X\",\"Text\":\"Excellent\"},{\"KeyValue\":\"F\",\"Text\":\"Fair\"},{\"KeyValue\":\"G\",\"Text\":\"Good\"}]";

            string[] _keys =
            [
                nameof(CacheObjects.Companies), nameof(CacheObjects.CompanyContacts), nameof(CacheObjects.Titles), nameof(CacheObjects.DocumentTypes), nameof(CacheObjects.Education), nameof(CacheObjects.Eligibility),
                nameof(CacheObjects.Experience), nameof(CacheObjects.JobOptions), nameof(CacheObjects.LeadIndustries), nameof(CacheObjects.LeadSources), nameof(CacheObjects.LeadStatus), nameof(CacheObjects.NAICS),
                nameof(CacheObjects.Roles), nameof(CacheObjects.Skills), nameof(CacheObjects.States), nameof(CacheObjects.StatusCodes), nameof(CacheObjects.TaxTerms), nameof(CacheObjects.Users), nameof(CacheObjects.Workflow),
                nameof(CacheObjects.Zips), nameof(CacheObjects.Communications), nameof(CacheObjects.Preferences)
            ];

            string[] _values =
            [
                _companies, _companyContacts, _titles, _documentTypes, _educations, _eligibilities, _experiences, _jobOptions, _leadIndustries, _leadSources, _leadStatuses, _naics, _roles, _skills,
                _states, _statusCodes, _taxTerms, _users, _workflows, _zips, _comms, _preferences
            ];

            await redisService.CreateBatchSet(_keys, _values);
        }
    }
}