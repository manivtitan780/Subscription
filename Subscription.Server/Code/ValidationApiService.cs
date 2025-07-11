using Microsoft.Extensions.Configuration;
using RestSharp;
using Subscription.Model; // To use IValidationApiService and other models
using System.Threading.Tasks;

namespace Subscription.Server.Code;

public class ValidationApiService : IValidationApiService
{
    private readonly IConfiguration _configuration;
    private readonly RestClient _restClient;

    public ValidationApiService(IConfiguration configuration)
    {
        _configuration = configuration;
        string apiHost = GetApiHost();
        _restClient = new RestClient(apiHost);
    }

    private string GetApiHost()
    {
        // Logic to determine APIHost or APIHostServer from appsettings.json
        // This is a simplified example, actual logic might be more complex
        string apiHost = _configuration["APIHost"] ?? _configuration["APIHostServer"];
        if (string.IsNullOrEmpty(apiHost))
        {
            throw new InvalidOperationException("APIHost or APIHostServer is not configured in appsettings.json.");
        }
        return apiHost;
    }

    public async Task<bool> CheckEINExistsAsync(string ein, int companyID)
    {
        RestRequest _request = new("Company/CheckEIN")
        {
            RequestFormat = DataFormat.Json
        };
        _request.AddQueryParameter("ein", ein);
        _request.AddQueryParameter("companyID", companyID);
        var response = await _restClient.GetAsync<bool>(_request);
        return !response; // Assuming API returns true if exists, so we negate for validation
    }

    // Implement other Check*ExistsAsync methods here
    public async Task<bool> CheckAdminListCodeExistsAsync(string code)
    {
        RestRequest _request = new("Admin/CheckTaxTermCode") // Assuming this is the endpoint for AdminList code
        {
            RequestFormat = DataFormat.Json
        };
        _request.AddQueryParameter("code", code);
        var response = await _restClient.GetAsync<bool>(_request);
        return !response;
    }

    public async Task<bool> CheckAdminListTextExistsAsync(string text, int id, string entity, string code)
    {
        RestRequest _request = new("Admin/CheckText")
        {
            RequestFormat = DataFormat.Json
        };
        _request.AddQueryParameter("id", id);
        _request.AddQueryParameter("text", text);
        _request.AddQueryParameter("entity", entity);
        _request.AddQueryParameter("code", code);
        var response = await _restClient.GetAsync<bool>(_request);
        return !response;
    }

    public async Task<bool> CheckDocumentTypeExistsAsync(string docType, int id)
    {
        RestRequest _request = new("Admin/CheckText") // Assuming Admin/CheckText is used for DocumentType
        {
            RequestFormat = DataFormat.Json
        };
        _request.AddQueryParameter("id", id);
        _request.AddQueryParameter("text", docType);
        _request.AddQueryParameter("entity", "Document Type"); // Hardcoded entity name
        var response = await _restClient.GetAsync<bool>(_request);
        return !response;
    }

    public async Task<bool> CheckJobCodeExistsAsync(string jobCode)
    {
        RestRequest _request = new("Admin/CheckJobCode")
        {
            RequestFormat = DataFormat.Json
        };
        _request.AddQueryParameter("id", jobCode); // Assuming 'id' is used for jobCode
        var response = await _restClient.GetAsync<bool>(_request);
        return !response;
    }

    public async Task<bool> CheckJobOptionExistsAsync(string code, string jobOption)
    {
        RestRequest _request = new("Admin/CheckJobOption")
        {
            RequestFormat = DataFormat.Json
        };
        _request.AddQueryParameter("code", code);
        _request.AddQueryParameter("text", jobOption);
        var response = await _restClient.GetAsync<bool>(_request);
        return !response;
    }

    public async Task<bool> CheckRoleIDExistsAsync(string roleID)
    {
        RestRequest _request = new("Admin/CheckRoleID")
        {
            RequestFormat = DataFormat.Json
        };
        _request.AddQueryParameter("id", roleID);
        var response = await _restClient.GetAsync<bool>(_request);
        return !response;
    }

    public async Task<bool> CheckRoleExistsAsync(string roleID, string role)
    {
        RestRequest _request = new("Admin/CheckRole")
        {
            RequestFormat = DataFormat.Json
        };
        _request.AddQueryParameter("id", roleID);
        _request.AddQueryParameter("text", role);
        var response = await _restClient.GetAsync<bool>(_request);
        return !response;
    }

    public async Task<bool> CheckStateCodeExistsAsync(string stateCode)
    {
        RestRequest _request = new("Admin/CheckStateCode")
        {
            RequestFormat = DataFormat.Json
        };
        _request.AddQueryParameter("code", stateCode);
        var response = await _restClient.GetAsync<bool>(_request);
        return !response;
    }

    public async Task<bool> CheckStateExistsAsync(string stateCode, string state)
    {
        RestRequest _request = new("Admin/CheckState")
        {
            RequestFormat = DataFormat.Json
        };
        _request.AddQueryParameter("code", stateCode);
        _request.AddQueryParameter("text", state);
        var response = await _restClient.GetAsync<bool>(_request);
        return !response;
    }

    public async Task<bool> CheckUserNameExistsAsync(string userName)
    {
        RestRequest _request = new("Admin/CheckText") // Assuming Admin/CheckText is used for UserName
        {
            RequestFormat = DataFormat.Json
        };
        _request.AddQueryParameter("id", 0); // Assuming ID is 0 for new user check
        _request.AddQueryParameter("text", userName);
        _request.AddQueryParameter("entity", "User Name"); // Hardcoded entity name
        var response = await _restClient.GetAsync<bool>(_request);
        return !response;
    }
}