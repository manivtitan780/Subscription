#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.Server
// File Name:           MasterDataCacheService.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu, Brijesh Dubey
// Created On:          07-10-2025 20:07
// Last Updated On:     07-10-2025 20:46
// *****************************************/

#endregion

/*
    Code Added by Gemini.
    Reason: This service acts as a singleton to cache application-wide master data.
    It prevents components from repeatedly fetching the same data from Redis, improving performance and reducing memory usage.
*/
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

        // Deserialize all the data
        States = General.DeserializeObject<List<StateCache>>(_cacheValues[nameof(CacheObjects.States)]);
        Eligibility = General.DeserializeObject<List<IntValues>>(_cacheValues[nameof(CacheObjects.Eligibility)]);
        Education = General.DeserializeObject<List<IntValues>>(_cacheValues[nameof(CacheObjects.Education)]);
        Experience = General.DeserializeObject<List<IntValues>>(_cacheValues[nameof(CacheObjects.Experience)]);
        JobOptions = General.DeserializeObject<List<JobOptions>>(_cacheValues[nameof(CacheObjects.JobOptions)]);
        Skills = General.DeserializeObject<List<IntValues>>(_cacheValues[nameof(CacheObjects.Skills)]);
        StatusCodes = General.DeserializeObject<List<StatusCode>>(_cacheValues[nameof(CacheObjects.StatusCodes)]);
        Preferences = General.DeserializeObject<Preferences>(_cacheValues[nameof(CacheObjects.Preferences)]);
        Companies = General.DeserializeObject<List<CompaniesList>>(_cacheValues[nameof(CacheObjects.Companies)]);
        CompanyContacts = General.DeserializeObject<List<CompanyContacts>>(_cacheValues[nameof(CacheObjects.CompanyContacts)]);
        Workflows = General.DeserializeObject<List<Workflow>>(_cacheValues[nameof(CacheObjects.Workflow)]);

        // Pre-compute derived data
        if (Skills != null)
        {
            SkillDictionary = Skills.ToDictionary(s => s.KeyValue, s => s.Text);
        }

        List<UserList> _users = General.DeserializeObject<List<UserList>>(_cacheValues[nameof(CacheObjects.Users)]);
        if (_users != null)
        {
            Recruiters = _users.Where(user => user.Role is 2 or 4 or 5 or 6)
                               .Select(user => new KeyValues {KeyValue = user.UserName, Text = user.UserName})
                               .ToList();
        }

        _isInitialized = true;
    }
}