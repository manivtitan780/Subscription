#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.Server
// File Name:           MasterDataCacheService.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu, Brijesh Dubey
// Created On:          07-29-2025 15:07
// Last Updated On:     07-29-2025 15:58
// *****************************************/

#endregion

#region Using

using JsonSerializer = System.Text.Json.JsonSerializer;

#endregion

namespace Subscription.Server.Code;

public class MasterDataCacheService(RedisService redisService)
{
    private bool _isInitialized;

    public List<CompaniesList> Companies { get; private set; } = [];

    public List<CompanyContacts> CompanyContacts { get; private set; } = [];

    // Properties to hold the cached data
    public List<IntValues> Education { get; private set; } = [];

    public List<IntValues> Eligibility { get; private set; } = [];

    public List<IntValues> Experience { get; private set; } = [];

    public List<JobOptions> JobOptions { get; private set; } = [];

    public Preferences Preferences { get; private set; } = new();

    public List<KeyValues> Recruiters { get; private set; } = [];

    public Dictionary<int, string> SkillDictionary { get; private set; } = [];

    public List<IntValues> Skills { get; private set; } = [];

    public List<StateCache> States { get; private set; } = [];

    public List<StatusCode> StatusCodes { get; private set; } = [];

    public List<Workflow> Workflows { get; private set; } = [];

    public async Task InitializeAsync()
    {
        if (_isInitialized)
        {
            return;
        }

        List<string> _keys =
        [
            nameof(CacheObjects.States), nameof(CacheObjects.Eligibility), nameof(CacheObjects.Education),
            nameof(CacheObjects.Experience), nameof(CacheObjects.JobOptions), nameof(CacheObjects.Users), nameof(CacheObjects.Skills),
            nameof(CacheObjects.StatusCodes), nameof(CacheObjects.Preferences), nameof(CacheObjects.Companies), nameof(CacheObjects.Workflow),
            nameof(CacheObjects.CompanyContacts)
        ];

        Dictionary<string, string> _cacheValues = await redisService.BatchGet(_keys);

        // Deserialize all the data using JsonContext source generation for optimal performance
        States = JsonSerializer.Deserialize(_cacheValues[nameof(CacheObjects.States)], JsonContext.CaseInsensitive.ListStateCache) ?? [];
        Eligibility = JsonSerializer.Deserialize(_cacheValues[nameof(CacheObjects.Eligibility)], JsonContext.CaseInsensitive.ListIntValues) ?? [];
        Education = JsonSerializer.Deserialize(_cacheValues[nameof(CacheObjects.Education)], JsonContext.CaseInsensitive.ListIntValues) ?? [];
        Experience = JsonSerializer.Deserialize(_cacheValues[nameof(CacheObjects.Experience)], JsonContext.CaseInsensitive.ListIntValues) ?? [];
        JobOptions = JsonSerializer.Deserialize(_cacheValues[nameof(CacheObjects.JobOptions)], JsonContext.CaseInsensitive.ListJobOptions) ?? [];
        Skills = JsonSerializer.Deserialize(_cacheValues[nameof(CacheObjects.Skills)], JsonContext.CaseInsensitive.ListIntValues) ?? [];
        StatusCodes = JsonSerializer.Deserialize(_cacheValues[nameof(CacheObjects.StatusCodes)], JsonContext.CaseInsensitive.ListStatusCode) ?? [];
        Preferences = JsonSerializer.Deserialize(_cacheValues[nameof(CacheObjects.Preferences)], JsonContext.CaseInsensitive.Preferences) ?? new();
        Companies = JsonSerializer.Deserialize(_cacheValues[nameof(CacheObjects.Companies)], JsonContext.CaseInsensitive.ListCompaniesList) ?? [];
        CompanyContacts = JsonSerializer.Deserialize(_cacheValues[nameof(CacheObjects.CompanyContacts)], JsonContext.CaseInsensitive.ListCompanyContacts) ?? [];
        Workflows = JsonSerializer.Deserialize(_cacheValues[nameof(CacheObjects.Workflow)], JsonContext.CaseInsensitive.ListWorkflow) ?? [];

        // Pre-compute derived data
        if (Skills != null)
        {
            SkillDictionary = Skills.ToDictionary(s => s.KeyValue, s => s.Text);
        }

        List<UserList> _users = JsonSerializer.Deserialize(_cacheValues[nameof(CacheObjects.Users)], JsonContext.CaseInsensitive.ListUserList) ?? [];
        if (_users != null)
        {
            Recruiters = _users.Where(user => user.Role is 2 or 4 or 5 or 6)
                               .Select(user => new KeyValues {KeyValue = user.UserName, Text = user.UserName})
                               .ToList();
        }

        _isInitialized = true;
    }
}