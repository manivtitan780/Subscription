namespace Subscription.Model;

public interface IValidationApiService
{
    Task<bool> CheckEINExistsAsync(string ein, int companyID);
    Task<bool> CheckAdminListCodeExistsAsync(string code);
    Task<bool> CheckAdminListTextExistsAsync(string text, int id, string entity, string code);
    Task<bool> CheckDocumentTypeExistsAsync(string docType, int id);
    Task<bool> CheckJobCodeExistsAsync(string jobCode);
    Task<bool> CheckJobOptionExistsAsync(string code, string jobOption);
    Task<bool> CheckRoleIDExistsAsync(string roleID);
    Task<bool> CheckRoleExistsAsync(string roleID, string role);
    Task<bool> CheckStateCodeExistsAsync(string stateCode);
    Task<bool> CheckStateExistsAsync(string stateCode, string state);
    Task<bool> CheckUserNameExistsAsync(string userName);
}