#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.API
// File Name:           General.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu, Brijesh Dubey
// Created On:          02-06-2025 19:02
// Last Updated On:     03-16-2025 20:03
// *****************************************/

#endregion

#region Using

using System.Security.Cryptography;
using System.Text;

using FluentStorage.Blobs;

using Newtonsoft.Json;

#endregion

namespace Subscription.API.Code;

[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
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

    /// <summary>
    ///     Deserializes a JSON string to an object of a specified type.
    /// </summary>
    /// <typeparam name="T">The type of object to deserialize to.</typeparam>
    /// <param name="array">The JSON string representing the object to be deserialized.</param>
    /// <returns>The deserialized object of type T.</returns>
    internal static T DeserializeObject<T>(object array) => JsonConvert.DeserializeObject<T>(array?.ToString() ?? string.Empty);

    
    internal static byte[] GenerateRandomString(int length = 8)
    {
        //string _randomString = Path.GetRandomFileName().Replace(".", string.Empty)[..length];
        StringBuilder _result = new(length);
        while (_result.Length < length)
        {
            string _randomString = Path.GetRandomFileName().Replace(".", string.Empty);
            _result.Append(_randomString);
        }
        return Encoding.UTF8.GetBytes(_result.ToString());
    }
    public static async Task SetCache()
    {
        RedisService _service = new(Start.CacheServer, Start.CachePort!.ToInt32(), Start.Access, false);
        bool _keyExists = await _service.CheckKeyExists(nameof(CacheObjects.Companies));

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

            string _companies = "[]";
            while (await _reader.ReadAsync())
            {
                _companies = _reader.NString(0);
            }

            //_companies = await SetIntValues(_reader);

            await _reader.NextResultAsync();
            string _companyContacts = "[]";
            while (await _reader.ReadAsync())
            {
                _companyContacts = _reader.NString(0);
            }

            // _companyContacts = await _reader.FillList<CompanyContactList>(contact => new()
            //                                                                          {
            //                                                                              ID = contact.GetInt32(0),
            //                                                                              ContactName = contact.GetString(2),
            //                                                                              CompanyID = contact.GetInt32(1)
            //                                                                          }).ToListAsync();

            await _reader.NextResultAsync();
            string _titles = "[]";
            while (await _reader.ReadAsync())
            {
                _titles = _reader.NString(0);
            }
            // _titles = await SetIntValues(_reader);

            await _reader.NextResultAsync();
            string _documentTypes = "[]";
            while (await _reader.ReadAsync())
            {
                _documentTypes = _reader.NString(0);
            }
            // _documentTypes = await SetIntValues(_reader);

            await _reader.NextResultAsync();
            string _educations = "[]";
            while (await _reader.ReadAsync())
            {
                _educations = _reader.NString(0);
            }
            // _educations = await SetIntValues(_reader);

            await _reader.NextResultAsync();
            string _eligibilities = "[]";
            while (await _reader.ReadAsync())
            {
                _eligibilities = _reader.NString(0);
            }
            // _eligibilities = await SetIntValues(_reader);

            await _reader.NextResultAsync();
            string _experiences = "[]";
            while (await _reader.ReadAsync())
            {
                _experiences = _reader.NString(0, "[]");
            }
            // _experiences = await SetIntValues(_reader);

            await _reader.NextResultAsync();
            string _jobOptions = "[]";
            while (await _reader.ReadAsync())
            {
                _jobOptions = _reader.NString(0, "[]");
            }
            // _jobOptions = await SetKeyValues(_reader);

            await _reader.NextResultAsync();
            string _leadIndustries = "[]";
            while (await _reader.ReadAsync())
            {
                _leadIndustries = _reader.NString(0, "[]");
            }
            // _leadIndustries = await SetIntValues(_reader, 2);

            await _reader.NextResultAsync();
            string _leadSources = "[]";
            while (await _reader.ReadAsync())
            {
                _leadSources = _reader.NString(0, "[]");
            }
            // _leadSources = await SetIntValues(_reader, 2);

            await _reader.NextResultAsync();
            string _leadStatuses = "[]";
            while (await _reader.ReadAsync())
            {
                _leadStatuses = _reader.NString(0, "[]");
            }
            // _leadStatuses = await SetIntValues(_reader, 2);

            await _reader.NextResultAsync();
            string _naics = "[]";
            while (await _reader.ReadAsync())
            {
                _naics = _reader.NString(0, "[]");
            }
            // _naics = await SetIntValues(_reader);

            await _reader.NextResultAsync();
            string _roles = "[]";
            while (await _reader.ReadAsync())
            {
                _roles = _reader.NString(0, "[]");
            }
            // _roles = await _reader.FillList<Role>(role => new()
            //                                               {
            //                                                   ID = role.GetByte(0),
            //                                                   RoleName = role.GetString(1),
            //                                                   Description = role.GetString(2),
            //                                                   CreateOrEditCompany = role.GetBoolean(3),
            //                                                   CreateOrEditCandidate = role.GetBoolean(4),
            //                                                   ViewAllCompanies = role.GetBoolean(5),
            //                                                   ViewMyCompanyProfile = role.GetBoolean(6),
            //                                                   EditMyCompanyProfile = role.GetBoolean(7),
            //                                                   CreateOrEditRequisition = role.GetBoolean(8),
            //                                                   ViewOnlyMyCandidates = role.GetBoolean(9),
            //                                                   ViewAllCandidates = role.GetBoolean(10),
            //                                                   ManageSubmittedCandidates = role.GetBoolean(11),
            //                                                   DownloadOriginal = role.GetBoolean(12),
            //                                                   DownloadFormatted = role.GetBoolean(13),
            //                                                   ViewRequisitions = role.GetBoolean(14)
            //                                               }).ToListAsync();

            await _reader.NextResultAsync();
            string _skills = "[]";
            while (await _reader.ReadAsync())
            {
                _skills = _reader.NString(0, "[]");
            }
            // _skills = await SetIntValues(_reader);

            await _reader.NextResultAsync();
            string _states = "[]";
            while (await _reader.ReadAsync())
            {
                _states = _reader.NString(0, "[]");
            }
            // _states = await SetIntValues(_reader);

            await _reader.NextResultAsync();
            string _statusCodes = "[]";
            while (await _reader.ReadAsync())
            {
                _statusCodes = _reader.NString(0, "[]");
            }
            // _statusCodes = await _reader.FillList<StatusCode>(status => new()
            //                                                             {
            //                                                                 ID = status.GetInt32(6),
            //                                                                 Code = status.GetString(0),
            //                                                                 Status = status.GetString(1),
            //                                                                 Icon = status.NString(2),
            //                                                                 AppliesToCode = status.GetString(3),
            //                                                                 SubmitCandidate = status.GetBoolean(4),
            //                                                                 ShowCommission = status.GetBoolean(5)
            //                                                             }).ToListAsync();

            await _reader.NextResultAsync();
            string _taxTerms = "[]";
            while (await _reader.ReadAsync())
            {
                _taxTerms = _reader.NString(0, "[]");
            }
            // _taxTerms = await SetKeyValues(_reader);

            await _reader.NextResultAsync();
            string _users = "[]";
            while (await _reader.ReadAsync())
            {
                _users = _reader.NString(0, "[]");
            }
            // _users = await _reader.FillList<UserList>(user => new()
            //                                                   {
            //                                                       UserName = user.GetString(0),
            //                                                       Role = user.GetByte(1)
            //                                                   }).ToListAsync();

            await _reader.NextResultAsync();
            string _workflows = "[]";
            while (await _reader.ReadAsync())
            {
                _workflows = _reader.NString(0, "[]");
            }
            // _workflows = await _reader.FillList<AppWorkflow>(workflow => new()
            //                                                              {
            //                                                                  ID = workflow.GetInt32(0),
            //                                                                  Step = workflow.GetString(1),
            //                                                                  Next = workflow.NString(2),
            //                                                                  IsLast = workflow.GetBoolean(3),
            //                                                                  RoleIDs = workflow.GetString(4),
            //                                                                  Schedule = workflow.GetBoolean(5),
            //                                                                  AnyStage = workflow.GetBoolean(6),
            //                                                                  NextFull = "",
            //                                                                  RoleFull = ""
            //                                                              }).ToListAsync();

            await _reader.NextResultAsync();
            string _zips = "[]";
            await _reader.ReadAsync();
            _zips = ((byte[])_reader[0]).DecompressGZip();
            /*byte[]* _zipCompressed = (byte[])_reader[0];
            using (MemoryStream _memStreamReader = new(_zipCompressed))
            {
                await using (GZipStream _gZipStream = new(_memStreamReader, CompressionMode.Decompress))
                {
                    using (MemoryStream _memStream = new())
                    {
                        await _gZipStream.CopyToAsync(_memStream);
                        _zips = Encoding.UTF8.GetString(_memStream.ToArray());
                    }
                }
            }*/

            //string* _zipCodes = JsonConvert.SerializeObject(_zips); 

            await _reader.NextResultAsync();
            string _preferences = "[]";
            if (_reader.HasRows)
            {
                await _reader.ReadAsync();
                _preferences = _reader.NString(0, "[]");
            }

            List<KeyValues> _communications =
            [
                new() {KeyValue = "A", Text = "Average"},
                new() {KeyValue = "X", Text = "Excellent"},
                new() {KeyValue = "F", Text = "Fair"},
                new() {KeyValue = "G", Text = "Good"}
            ];

            string _comms = JsonConvert.SerializeObject(_communications);

            await _reader.CloseAsync();

            await _connection.CloseAsync();

            string[] _keys =
            [
                nameof(CacheObjects.Companies), nameof(CacheObjects.CompanyContacts), nameof(CacheObjects.Titles), nameof(CacheObjects.DocumentTypes), nameof(CacheObjects.Education), nameof(CacheObjects.Eligibility),
                nameof(CacheObjects.Experience), nameof(CacheObjects.JobOptions), nameof(CacheObjects.LeadIndustries), nameof(CacheObjects.LeadSources), nameof(CacheObjects.LeadStatus), nameof(CacheObjects.NAICS), 
                nameof(CacheObjects.Roles), nameof(CacheObjects.Skills), nameof(CacheObjects.States), nameof(CacheObjects.StatusCodes), nameof(CacheObjects.TaxTerms), nameof(CacheObjects.Users), 
                nameof(CacheObjects.Workflow), nameof(CacheObjects.Zips), nameof(CacheObjects.Communications), nameof(CacheObjects.Preferences)
            ];

            string[] _values =
            [
                _companies, _companyContacts, _titles, _documentTypes, _educations, _eligibilities, _experiences, _jobOptions, _leadIndustries, _leadSources, _leadStatuses, _naics, _roles, _skills,
                _states, _statusCodes, _taxTerms, _users, _workflows, _zips, _comms, _preferences
            ];

            await _service.CreateBatchSet(_keys, _values);
        }
    }

    internal static async Task UploadToBlob(IFormFile file, string blobPath)
    {
        IAzureBlobStorage _storage = StorageFactory.Blobs.AzureBlobStorageWithSharedKey(Start.AccountName, Start.AzureKey);

        await using Stream stream = file.OpenReadStream();
        await _storage.WriteAsync(blobPath, stream);
    }

    internal static async Task<byte[]> ReadFromBlob(string blobPath)
    {
        //Connect to the Azure Blob Storage
        IAzureBlobStorage _storage = StorageFactory.Blobs.AzureBlobStorageWithSharedKey(Start.AccountName, Start.AzureKey);

        //Read the file into a Bytes Array
        byte[] _memBytes = await _storage.ReadBytesAsync(blobPath);
        
        return _memBytes;
    }
    
    // ReSharper disable once UnusedMember.Local
    private static async Task<List<IntValues>> SetIntValues(SqlDataReader reader, byte keyType = 0) //0-Int32, 1=Int16, 2=Byte
    {
        return await reader.FillList<IntValues>(intValue => new()
                                                            {
                                                                KeyValue = keyType switch
                                                                           {
                                                                               0 => intValue.GetInt32(0),
                                                                               1 => intValue.GetInt16(0),
                                                                               2 => intValue.GetByte(0),
                                                                               _ => 0
                                                                           },
                                                                Text = intValue.GetString(1)
                                                            }).ToListAsync();
    }

    // ReSharper disable once UnusedMember.Local
    private static async Task<List<KeyValues>> SetKeyValues(SqlDataReader reader)
    {
        return await reader.FillList<KeyValues>(keyValue => new()
                                                            {
                                                                KeyValue = keyValue.GetString(0),
                                                                Text = keyValue.GetString(1)
                                                            }).ToListAsync();
    }

    /// <summary>
    ///     Computes the SHA-512 hash of the input text.
    /// </summary>
    /// <param name="inputText">The text to be hashed.</param>
    /// <returns>A byte array representing the SHA-512 hash of the input text.</returns>
    public static byte[] SHA512PasswordHash(string inputText) => SHA512.HashData(new UTF8Encoding().GetBytes(inputText));

    /// <summary>
    ///     Computes the SHA-512 hash of the input text.
    /// </summary>
    /// <param name="inputText">The text to be hashed.</param>
    /// <returns>A byte array representing the SHA-512 hash of the input text.</returns>
    public static byte[] SHA512PasswordHash(byte[] inputText) => SHA512.HashData(inputText);
}