#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.API
// File Name:           AdminController.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu, Brijesh Dubey
// Created On:          03-10-2025 15:03
// Last Updated On:     05-04-2025 20:27
// *****************************************/

#endregion

namespace Subscription.API.Controllers;

[ApiController, Route("api/[controller]/[action]")]
public class AdminController(RedisService redisService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<string>> GetAdminList(string methodName, string filter = "", bool isString = true)
    {
        await using SqlConnection _connection = new(Start.ConnectionString);
        string _generalItems = "[]";

        await using SqlCommand _command = new(methodName, _connection);
        _command.CommandType = CommandType.StoredProcedure;

        if (filter.NotNullOrWhiteSpace())
        {
            _command.Varchar("Filter", 100, filter);
        }

        try
        {
            // Open the connection
            await _connection.OpenAsync();
            _generalItems = (await _command.ExecuteScalarAsync())?.ToString() ?? "[]";
        }
        catch (SqlException ex)
        {
            Log.Error(ex, "Error saving {methodName} search. {ExceptionMessage}", methodName, ex.Message);
            return StatusCode(500, "Error saving education.");
        }
        finally
        {
            await _connection.CloseAsync();
        }

        return Ok(_generalItems);
    }

    [HttpGet]
    public async Task<ActionResult<string>> GetSearch(string methodName = "", string paramName = "", string filter = "")
    {
        await using SqlConnection _connection = new(Start.ConnectionString);
        await using SqlCommand _command = new(methodName, _connection);
        _command.CommandType = CommandType.StoredProcedure;
        _command.Varchar(paramName, 100, filter);

        string _listOptions = "[]";
        try
        {
            await _connection.OpenAsync();
            _listOptions = (await _command.ExecuteScalarAsync())?.ToString() ?? "[]";
        }
        catch (SqlException ex)
        {
            Log.Error(ex, "Error saving {paramName} search. {ExceptionMessage}", paramName, ex.Message);
            return StatusCode(500, $"Error fetching {paramName} search.");
        }
        finally
        {
            await _connection.CloseAsync();
        }

        return Ok(_listOptions ?? "[]");
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

                                                     command.Varchar("User", 10, "ADMIN");
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

            while (await _reader.ReadAsync())
            {
                _returnCode = _reader.NString(0, "[]");
            }

            await _reader.NextResultAsync();
            string _cacheValue = "[]";
            while (await _reader.ReadAsync())
            {
                _cacheValue = _reader.NString(0, "[]");
            }

            if (_cacheValue.NotNullOrWhiteSpace() && _cacheValue != "[]" && cacheName.NotNullOrWhiteSpace())
            {
                await redisService.CreateAsync(cacheName, _cacheValue);
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error saving {LogContext}. {ExceptionMessage}", logContext, ex.Message);
            return StatusCode(500, $"Error saving {logContext}.");
        }
        finally
        {
            await _con.CloseAsync();
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
                                                                   command.Varchar("User", 10, "ADMIN");
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
                                                                   command.Varchar("User", 10, "ADMIN");
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
                                                                   command.Varchar("User", 10, "ADMIN");
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
                                                                   command.Varchar("User", 10, "ADMIN");
                                                                   command.Bit("Enabled", entity.IsEnabled);
                                                               }, cacheName, template, "Template");
    }

    [HttpPost]
    public async Task<ActionResult<string>> SaveUser([FromBody] User user, string cacheName = nameof(CacheObjects.Users))
    {
        byte[] _salt = user.Password.NullOrWhiteSpace() ? new byte[64] : General.GenerateRandomString(64);
        byte[] _password = user.Password.NullOrWhiteSpace() ? new byte[64] : General.ComputeHashWithSalt(user.Password, _salt);
        return await SaveEntityAsync("Admin_SaveUser", (command, entity) =>
                                                       {
                                                           command.Varchar("UserName", 10, entity.UserName);
                                                           command.Varchar("FirstName", 50, entity.FirstName);
                                                           command.Varchar("LastName", 200, entity.LastName);
                                                           command.Varchar("Email", 200, entity.EmailAddress);
                                                           command.TinyInt("Role", entity.RoleID);
                                                           command.Bit("Status", entity.StatusEnabled);
                                                           command.Varchar("User", 10, "ADMIN");
                                                           command.Binary("Salt", 64, entity.Password.NullOrWhiteSpace() ? DBNull.Value : _salt);
                                                           command.Binary("Password", 64, entity.Password.NullOrWhiteSpace() ? DBNull.Value : _password);
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
                                                                   command.Varchar("User", 10, "ADMIN");
                                                               }, cacheName, workflow, "Workflow");
    }

    [HttpPost]
    public async Task<ActionResult<string>> ToggleAdminList(string methodName, string id, string userName = "ADMIN", bool idIsString = false, bool isUser = false)
    {
        await using SqlConnection _con = new(Start.ConnectionString);
        _con.Open();
        string _returnCode = "[]";
        try
        {
            await using SqlCommand _command = new(methodName, _con);
            _command.CommandType = CommandType.StoredProcedure;
            if (!idIsString)
            {
                _command.Int("ID", id.ToInt32());
            }
            else if (!isUser)
            {
                _command.Char("Code", 1, id);
            }
            else
            {
                _command.Varchar("Code", 10, id);
            }

            _command.Varchar("User", 10, userName);
            _returnCode = (await _command.ExecuteScalarAsync())?.ToString() ?? "[]";
        }
        catch (SqlException ex)
        {
#pragma warning disable CA2254
            Log.Error(ex, "Error toggling " + methodName + ". {ExceptionMessage}", ex.Message);
#pragma warning restore CA2254
            return StatusCode(500, $"Error toggling {methodName}.");
        }
        finally
        {
            await _con.CloseAsync();
        }

        return Ok(_returnCode);
    }
}