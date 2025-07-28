#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.API
// File Name:           AdminController.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu, Brijesh Dubey
// Created On:          07-15-2025 16:07
// Last Updated On:     07-28-2025 15:40
// *****************************************/

#endregion

namespace Subscription.API.Controllers;

[ApiController, Route("api/[controller]/[action]")]
public class AdminController(RedisService redisService) : ControllerBase
{
    // Adding constant for ADMIN user to reduce string allocations and improve performance
    private const string AdminUser = "ADMIN";

    [HttpPost]
    public async Task<ActionResult<bool>> CheckJobCode(string code, int id = 0)
    {
        return await ExecuteValidationAsync("Admin_CheckJobCode", command =>
                                                                  {
                                                                      command.Varchar("Code", 10, code);
                                                                      command.Int("ID", id);
                                                                  });
    }

    [HttpPost]
    public async Task<ActionResult<bool>> CheckJobOption(string option, int id = 0)
    {
        return await ExecuteValidationAsync("Admin_CheckJobOption", command =>
                                                                    {
                                                                        command.Varchar("Option", 50, option);
                                                                        command.Int("ID", id);
                                                                    });
    }

    [HttpPost]
    public async Task<ActionResult<bool>> CheckRole(string roleName, int id = 0)
    {
        return await ExecuteValidationAsync("Admin_CheckRole", command =>
                                                               {
                                                                   command.Varchar("RoleName", 10, roleName);
                                                                   command.Int("ID", id);
                                                               });
    }

    [HttpPost]
    public async Task<ActionResult<bool>> CheckRoleID(int roleId, int id = 0)
    {
        return await ExecuteValidationAsync("Admin_CheckRoleID", command =>
                                                                 {
                                                                     command.Int("RoleID", roleId);
                                                                     command.Int("ID", id);
                                                                 });
    }

    [HttpPost]
    public async Task<ActionResult<bool>> CheckState(string stateName, int id = 0)
    {
        return await ExecuteValidationAsync("Admin_CheckState", command =>
                                                                {
                                                                    command.Varchar("StateName", 50, stateName);
                                                                    command.Int("ID", id);
                                                                });
    }

    [HttpPost]
    public async Task<ActionResult<bool>> CheckStateCode(string code, int id = 0)
    {
        return await ExecuteValidationAsync("Admin_CheckStateCode", command =>
                                                                    {
                                                                        command.Varchar("Code", 2, code);
                                                                        command.Int("ID", id);
                                                                    });
    }

    [HttpPost]
    public async Task<ActionResult<bool>> CheckTaxTermCode(string code, int id = 0)
    {
        return await ExecuteValidationAsync("Admin_CheckTaxTermCode", command =>
                                                                      {
                                                                          command.Varchar("Code", 10, code);
                                                                          command.Int("ID", id);
                                                                      });
    }

    [HttpPost]
    public async Task<ActionResult<bool>> CheckText(string text, int id = 0, string entity = "", string code = "")
    {
        return await ExecuteValidationAsync("Admin_CheckText", command =>
                                                               {
                                                                   command.Varchar("Text", 100, text);
                                                                   command.Int("ID", id);
                                                                   command.Varchar("Entity", 50, entity);
                                                                   command.Varchar("Code", 10, code);
                                                               });
    }

    [HttpGet]
    public async Task<ActionResult<string>> CheckZip(string zip = "00000")
    {
        return await ExecuteQueryAsync("GetZipCityState", command => { command.Varchar("Zip", 10, zip); }, "CityState", "Error fetching City/State.");
    }

    private async Task<ActionResult<string>> ExecuteQueryAsync(string procedureName, Action<SqlCommand> parameterBinder, string logContext, string errorMessage)
    {
        await using SqlConnection _connection = new(Start.ConnectionString);
        await using SqlCommand _command = new(procedureName, _connection);
        _command.CommandType = CommandType.StoredProcedure;

        parameterBinder(_command);

        string _result = "[]";
        try
        {
            await _connection.OpenAsync();
            _result = await _command.ExecuteScalarAsync() as string ?? "[]";
        }
        catch (SqlException ex)
        {
            Log.Error(ex, "Error executing {logContext} query. {ExceptionMessage}", logContext, ex.Message);
            return StatusCode(500, errorMessage);
        }

        return Ok(_result);
    }

    // Adding validation endpoints for future validation infrastructure
    private async Task<ActionResult<bool>> ExecuteValidationAsync(string procedureName, Action<SqlCommand> parameterBinder)
    {
        await using SqlConnection _connection = new(Start.ConnectionString);
        await using SqlCommand _command = new(procedureName, _connection);
        _command.CommandType = CommandType.StoredProcedure;

        parameterBinder(_command);

        try
        {
            await _connection.OpenAsync();
            object result = await _command.ExecuteScalarAsync();
            // Correctly checks for DBNull and converts the result to a boolean.
            // The logic is inverted because a validation success (true) means the item does NOT exist.
            bool exists = result != null && result != DBNull.Value && Convert.ToBoolean(result);
            return Ok(!exists);
        }
        catch (SqlException ex)
        {
            Log.Error(ex, "Error executing validation query {ProcedureName}", procedureName);
            return StatusCode(500, "Validation check failed");
        }
    }

    [HttpGet]
    public async Task<ActionResult<string>> GetAdminList(string methodName, string filter = "")
    {
        return await ExecuteQueryAsync(methodName, command =>
                                                   {
                                                       if (filter.NotNullOrWhiteSpace())
                                                       {
                                                           command.Varchar("Filter", 100, filter);
                                                       }
                                                   }, methodName, $"Error saving {methodName}.");
    }

    [HttpGet]
    public async Task<ActionResult<string>> GetSearch(string methodName = "", string paramName = "", string filter = "")
    {
        return await ExecuteQueryAsync(methodName, command => { command.Varchar(paramName, 100, filter); }, paramName, $"Error fetching {paramName} search.");
    }

    [HttpPost]
    public async Task<ActionResult<string>> SaveAdminList([FromBody] AdminList adminList, string methodName, string parameterName, bool containDescription, bool isString, string cacheName = "")
    {
        return await SaveEntityAsync(methodName, (command, list) =>
                                                 {
                                                     if (isString)
                                                     {
                                                         command.Char("Code", 1, list.Code.DBNull());
                                                     }
                                                     else
                                                     {
                                                         command.Int("ID", list.ID.DBNull());
                                                     }

                                                     command.Varchar(parameterName, 100, list.Text);

                                                     if (containDescription)
                                                     {
                                                         command.Varchar("Desc", 500, list.Text);
                                                     }

                                                     command.Varchar("User", 10, AdminUser);
                                                     command.Bit("Enabled", list.IsEnabled);
                                                 }, cacheName, adminList, parameterName);
    }

    [HttpPost]
    public async Task<ActionResult<string>> SaveDocumentType([FromBody] DocumentTypes documentType, string cacheName = nameof(CacheObjects.DocumentTypes))
    {
        return await SaveEntityAsync("Admin_SaveDocumentType", (command, entity) =>
                                                               {
                                                                   command.Int("ID", entity.KeyValue.DBNull());
                                                                   command.Varchar("DocumentType", 100, entity.Text);
                                                               }, cacheName, documentType, "Document Type");
    }

    private async Task<ActionResult<string>> SaveEntityAsync<T>(string procedureName, Action<SqlCommand, T> parameterBinder, string cacheName, T entity, string logContext)
    {
        await using SqlConnection _con = new(Start.ConnectionString);
        string _returnCode = "";
        try
        {
            await using SqlCommand _command = new(procedureName, _con);
            _command.CommandType = CommandType.StoredProcedure;

            parameterBinder(_command, entity);

            await _con.OpenAsync();
            await using SqlDataReader _reader = await _command.ExecuteReaderAsync();

            // Optimized: Using if instead of while since exactly 1 row is returned
            if (await _reader.ReadAsync())
            {
                _returnCode = _reader.NString(0, "[]");
            }

            await _reader.NextResultAsync();
            string _cacheValue = "[]";
            // Optimized: Using if instead of while since exactly 1 row is returned
            if (await _reader.ReadAsync())
            {
                _cacheValue = _reader.NString(0, "[]");
            }

            // Update Redis cache only if a valid cache name and non-empty, non-default JSON value are present.
            if (cacheName.NotNullOrWhiteSpace() && _cacheValue.Length > 2) // A simple check for "[]"
            {
                await redisService.CreateAsync(cacheName, _cacheValue);
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error saving {LogContext}. {ExceptionMessage}", logContext, ex.Message);
            return StatusCode(500, $"Error saving {logContext}.");
        }

        return Ok(_returnCode);
    }

    [HttpPost]
    public async Task<ActionResult<string>> SaveJobOptions([FromBody] JobOptions jobOption, string cacheName = nameof(CacheObjects.JobOptions))
    {
        return await SaveEntityAsync("Admin_SaveJobOptions", (command, entity) =>
                                                             {
                                                                 command.Char("Code", 1, entity.KeyValue);
                                                                 command.Varchar("JobOptions", 50, entity.Text);
                                                                 command.Varchar("Desc", 500, entity.Description);
                                                                 command.Bit("Duration", entity.Duration);
                                                                 command.Bit("Rate", entity.Rate);
                                                                 command.Bit("Sal", entity.Sal);
                                                                 command.Varchar("TaxTerms", 20, entity.Tax);
                                                                 command.Bit("Expenses", entity.Exp);
                                                                 command.Bit("PlaceFee", entity.PlaceFee);
                                                                 command.Bit("Benefits", entity.Benefits);
                                                                 command.Bit("ShowHours", entity.ShowHours);
                                                                 command.Varchar("RateText", 255, entity.RateText);
                                                                 command.Varchar("PercentText", 255, entity.PercentText);
                                                                 command.Decimal("CostPercent", 5, 2, entity.CostPercent);
                                                                 command.Bit("ShowPercent", entity.ShowPercent);
                                                                 command.Varchar("User", 10, AdminUser);
                                                             }, cacheName, jobOption, "Job Options");
    }

    [HttpPost]
    public async Task<ActionResult<string>> SaveNAICS([FromBody] NAICS naics, string cacheName = nameof(CacheObjects.NAICS))
    {
        return await SaveEntityAsync("Admin_SaveNAICS", (command, entity) =>
                                                        {
                                                            command.Int("ID", entity.ID.DBNull());
                                                            command.Varchar("NAICS", 100, entity.Title);
                                                        }, cacheName, naics, "NAICS");
    }

    [HttpPost]
    public async Task<ActionResult<string>> SaveRole([FromBody] Role role, string cacheName = nameof(CacheObjects.Roles))
    {
        return await SaveEntityAsync("Admin_SaveRole", (command, entity) =>
                                                       {
                                                           command.Int("ID", entity.ID.DBNull());
                                                           command.Varchar("RoleName", 10, entity.RoleName);
                                                           command.Varchar("RoleDescription", 255, entity.Description);
                                                           command.Bit("CreateOrEditCompany", entity.CreateOrEditCompany);
                                                           command.Bit("CreateOrEditCandidate", entity.CreateOrEditCandidate);
                                                           command.Bit("ViewAllCompanies", entity.ViewAllCompanies);
                                                           command.Bit("ViewMyCompanyProfile", entity.ViewMyCompanyProfile);
                                                           command.Bit("EditMyCompanyProfile", entity.EditMyCompanyProfile);
                                                           command.Bit("CreateOrEditRequisitions", entity.CreateOrEditRequisitions);
                                                           command.Bit("ViewOnlyMyCandidates", entity.ViewOnlyMyCandidates);
                                                           command.Bit("ViewAllCandidates", entity.ViewAllCandidates);
                                                           command.Bit("ViewRequisitions", entity.ViewRequisitions);
                                                           command.Bit("EditRequisitions", entity.EditRequisitions);
                                                           command.Bit("ManageSubmittedCandidates", entity.ManageSubmittedCandidates);
                                                           command.Bit("DownloadOriginal", entity.DownloadOriginal);
                                                           command.Bit("DownloadFormatted", entity.DownloadFormatted);
                                                           command.Bit("AdminScreens", entity.AdminScreens);
                                                           command.Varchar("User", 10, AdminUser);
                                                       }, cacheName, role, "Role");
    }

    [HttpPost]
    public async Task<ActionResult<string>> SaveState([FromBody] State state, string cacheName = nameof(CacheObjects.States))
    {
        return await SaveEntityAsync("Admin_SaveState", (command, entity) =>
                                                        {
                                                            command.Int("ID", entity.ID.DBNull());
                                                            command.Varchar("Code", 2, entity.Code);
                                                            command.Varchar("State", 50, entity.StateName);
                                                            command.Varchar("Country", 50, "USA");
                                                            command.Varchar("User", 10, AdminUser);
                                                        }, cacheName, state, "State");
    }

    [HttpPost]
    public async Task<ActionResult<string>> SaveTemplate([FromBody] AppTemplate template, string cacheName = nameof(CacheObjects.Templates))
    {
        return await SaveEntityAsync("Admin_SaveTemplate", (command, entity) =>
                                                           {
                                                               command.Int("ID", entity.ID.DBNull());
                                                               command.Varchar("TemplateName", 50, entity.TemplateName);
                                                               command.Varchar("CC", 2000, entity.CC);
                                                               command.Varchar("Subject", 255, entity.Subject);
                                                               command.Varchar("Template", -1, entity.TemplateContent);
                                                               command.Varchar("Notes", 500, entity.Notes);
                                                               command.Varchar("SendTo", 200, entity.SendTo);
                                                               command.TinyInt("Action", entity.Action);
                                                               command.Varchar("User", 10, AdminUser);
                                                               command.Bit("Enabled", entity.IsEnabled);
                                                           }, cacheName, template, "Template");
    }

    [HttpPost]
    public async Task<ActionResult<string>> SaveUser([FromBody] User user, string cacheName = nameof(CacheObjects.Users))
    {
        // Fail fast: Password is required for user creation/update
        if (user.Password.NullOrWhiteSpace())
        {
            return BadRequest("Password is required for user operations.");
        }

        // Generate password hash and salt only when password is valid
        //byte[] _salt = General.GenerateRandomString(64);
        (byte[] Hash, byte[] Salt) _password = PasswordHasher.HashPassword(user.Password);

        return await SaveEntityAsync("Admin_SaveUser", (command, entity) =>
                                                       {
                                                           command.Varchar("UserName", 10, entity.UserName);
                                                           command.Varchar("FirstName", 50, entity.FirstName);
                                                           command.Varchar("LastName", 200, entity.LastName);
                                                           command.Varchar("Email", 200, entity.EmailAddress);
                                                           command.TinyInt("Role", entity.RoleID);
                                                           command.Bit("Status", entity.StatusEnabled);
                                                           command.Varchar("User", 10, AdminUser);
                                                           command.Binary("Salt", 64, _password.Salt);
                                                           command.Binary("Password", 64, _password.Hash);
                                                       }, cacheName, user, "User");
    }

    [HttpPost]
    public async Task<ActionResult<string>> SaveWorkflow([FromBody] Workflow workflow, string cacheName = nameof(CacheObjects.Workflow))
    {
        return await SaveEntityAsync("Admin_SaveWorkflow", (command, entity) =>
                                                           {
                                                               command.Int("ID", entity.ID.DBNull());
                                                               command.Varchar("Next", 100, entity.Next);
                                                               command.Bit("IsLast", entity.IsLast);
                                                               command.Varchar("Role", 50, entity.RoleIDs);
                                                               command.Bit("Schedule", entity.Schedule);
                                                               command.Bit("AnyStage", entity.AnyStage);
                                                               command.Varchar("User", 10, AdminUser);
                                                           }, cacheName, workflow, "Workflow");
    }

    [HttpPost]
    public async Task<ActionResult<string>> ToggleAdminList(string methodName, string id, string userName = AdminUser, bool idIsString = false, bool isUser = false)
    {
        // Refactored to use ExecuteQueryAsync pattern for consistency and better resource management
        return await ExecuteQueryAsync(methodName, command =>
                                                   {
                                                       if (!idIsString)
                                                       {
                                                           command.Int("ID", id.ToInt32());
                                                       }
                                                       else if (!isUser)
                                                       {
                                                           command.Char("Code", 1, id);
                                                       }
                                                       else
                                                       {
                                                           command.Varchar("Code", 10, id);
                                                       }

                                                       command.Varchar("User", 10, userName);
                                                   }, methodName, $"Error saving {methodName}.");
    }
}