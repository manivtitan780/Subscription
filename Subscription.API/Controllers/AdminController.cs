#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.API
// File Name:           AdminController.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu, Brijesh Dubey
// Created On:          03-10-2025 15:03
// Last Updated On:     03-26-2025 20:03
// *****************************************/

#endregion

namespace Subscription.API.Controllers;

[ApiController, Route("api/[controller]/[action]")]
public class AdminController(RedisService redisService) : ControllerBase
{
    // private RedisService _redisService = redisService;

    /// <summary>
    ///     Retrieves a list of administrative items based on the specified method and filter.
    /// </summary>
    /// <param name="methodName">The name of the stored procedure to be executed.</param>
    /// <param name="filter">An optional filter to apply to the list. If not provided, all items are returned.</param>
    /// <param name="isString">A flag indicating whether the filter is a string. If false, the filter is treated as an integer.</param>
    /// <returns>
    ///     A dictionary containing a list of administrative items and the total count of items.
    ///     The "GeneralItems" key contains a list of items, each represented as an instance of the AdminList class.
    ///     The "Count" key contains the total count of items.
    /// </returns>
    /// <remarks>
    ///     This method establishes a connection to the database using a connection string from the configuration.
    ///     It then creates a SQL command using the provided method name and sets the command type to stored procedure.
    ///     If a filter is provided, it adds a character parameter 'Filter' to the command.
    ///     After executing the command, it reads the results into a list of AdminList instances and counts the total number of
    ///     items.
    ///     The method then returns a dictionary containing the list of items and the total count.
    /// </remarks>
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
            _generalItems = (await _command.ExecuteScalarAsync())?.ToString();
            /*if (isString)
             {
                 _generalItems.Add(new(_reader.GetString(0), _reader.GetString(1), _reader.GetString(2), _reader.GetString(3), _reader.GetString(4),
                                       _reader.GetBoolean(5)));
             }
             else
             {
                 _generalItems.Add(new(_reader.GetDataTypeName(0) == "tinyint" ? _reader.GetByte(0) : _reader.GetInt32(0), _reader.GetString(1), _reader.GetString(2),
                                       _reader.GetString(3), _reader.GetString(4), _reader.GetBoolean(5)));
             }*/

            /*await _reader.NextResultAsync();
            await _reader.ReadAsync();
            int _count = _reader.GetInt32(0);*/

            await _connection.CloseAsync();
        }
        catch (SqlException ex)
        {
            Log.Error(ex, "Error saving education. {ExceptionMessage}", ex.Message);
            return StatusCode(500, "Error saving education.");
        }
        finally
        {
            await _connection.CloseAsync();
        }

        return Ok(_generalItems);
    }

    /// <summary>
    ///     Retrieves a list of search options from the database using a specified stored procedure.
    /// </summary>
    /// <param name="methodName">The name of the stored procedure to be executed.</param>
    /// <param name="paramName">The name of the parameter to be passed to the stored procedure.</param>
    /// <param name="filter">The filter to be applied on the search options.</param>
    /// <returns>A list of strings representing the search options.</returns>
    /// <remarks>
    ///     This method establishes a connection to the database using a connection string from the configuration.
    ///     It then creates a SQL command using the provided method name and sets the command type to stored procedure.
    ///     It adds a varchar parameter to the command with the provided parameter name and filter.
    ///     After executing the command, it reads the result into a list of strings and returns this list.
    /// </remarks>
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
            _connection.Open();
            _listOptions = (await _command.ExecuteScalarAsync())?.ToString();
        }
        catch (SqlException ex)
        {
            Log.Error(ex, "Error saving education. {ExceptionMessage}", ex.Message);
            return StatusCode(500, "Error saving education.");
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
        await using SqlConnection _con = new(Start.ConnectionString);
        string _returnCode = "";
        await using SqlCommand _command = new(methodName, _con);
        _command.CommandType = CommandType.StoredProcedure;
        if (isString)
        {
            _command.Char("Code", 1, adminList.Code.DBNull());
        }
        else
        {
            _command.Int("ID", adminList.ID.DBNull());
        }

        _command.Varchar(parameterName, 100, adminList.Text);
        if (containDescription)
        {
            _command.Varchar("Desc", 500, adminList.Text);
        }

        _command.Varchar("User", 10, "ADMIN");
        _command.Bit("Enabled", adminList.IsEnabled);
        try
        {
            await _con.OpenAsync();

            await using SqlDataReader _reader = await _command.ExecuteReaderAsync();
            while (_reader.Read())
            {
                _returnCode = _reader.NString(0, "[]");
            }

            string _cacheValue = "[]";
            await _reader.NextResultAsync();
            while (_reader.Read())
            {
                _cacheValue = _reader.NString(0, "[]");
            }

            /*_returnCode = (await _command.ExecuteScalarAsync())?.ToString() ?? "";*/

            if (_cacheValue.NotNullOrWhiteSpace() && _cacheValue != "[]" && cacheName.NotNullOrWhiteSpace())
            {
                RedisService _service = new(Start.CacheServer, Start.CachePort!.ToInt32(), Start.Access, false);
                await _service.CreateAsync(cacheName, _cacheValue);
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error saving " + parameterName + ". {ExceptionMessage}", ex.Message);
            return StatusCode(500, $"Error saving {parameterName}.");
        }
        finally
        {
            await _con.CloseAsync();
        }

        return Ok(_returnCode);
    }

    [HttpPost]
    public async Task<ActionResult<string>> SaveDocumentType([FromBody] DocumentTypes documentType, string cacheName = "")
    {
        await using SqlConnection _con = new(Start.ConnectionString);
        string _returnCode = "";
        await using SqlCommand _command = new("Admin_SaveDocumentType", _con);
        _command.CommandType = CommandType.StoredProcedure;
        _command.Int("ID", documentType.KeyValue.DBNull());

        _command.Varchar("DocumentType", 100, documentType.Text);

        try
        {
            await _con.OpenAsync();

            _returnCode = (await _command.ExecuteScalarAsync())?.ToString() ?? "";

            if (_returnCode.NotNullOrWhiteSpace() && _returnCode != "[]" && cacheName.NotNullOrWhiteSpace())
            {
                RedisService _service = new(Start.CacheServer, Start.CachePort!.ToInt32(), Start.Access, false);
                await _service.CreateAsync(cacheName, _returnCode);
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error saving Document Type. {ExceptionMessage}", ex.Message);
        }
        finally
        {
            await _con.CloseAsync();
        }

        return Ok(_returnCode);
    }

    [HttpPost]
    public async Task<ActionResult<string>> SaveJobOptions([FromBody] JobOptions jobOption, string cacheName = "")
    {
        await using SqlConnection _con = new(Start.ConnectionString);
        string _returnCode = "";
        await using SqlCommand _command = new("Admin_SaveJobOptions", _con);
        _command.CommandType = CommandType.StoredProcedure;
        _command.Char("Code", 1, jobOption.KeyValue);
        _command.Varchar("JobOptions", 50, jobOption.Text);
        _command.Varchar("Desc", 500, jobOption.Description);
        _command.Bit("Duration", jobOption.Duration);
        _command.Bit("Rate", jobOption.Rate);
        _command.Bit("Sal", jobOption.Sal);
        _command.Varchar("TaxTerms", 20, jobOption.Tax);
        _command.Bit("Expenses", jobOption.Exp);
        _command.Bit("PlaceFee", jobOption.PlaceFee);
        _command.Bit("Benefits", jobOption.Benefits);
        _command.Bit("ShowHours", jobOption.ShowHours);
        _command.Varchar("RateText", 255, jobOption.RateText);
        _command.Varchar("PercentText", 255, jobOption.PercentText);
        _command.Decimal("CostPercent", 5, 2, jobOption.CostPercent);
        _command.Bit("ShowPercent", jobOption.ShowPercent);
        _command.Varchar("User", 10, "ADMIN");

        try
        {
            await _con.OpenAsync();
            _returnCode = (await _command.ExecuteScalarAsync())?.ToString() ?? "";

            if (_returnCode.NotNullOrWhiteSpace() && _returnCode != "[]" && cacheName.NotNullOrWhiteSpace())
            {
                RedisService _service = new(Start.CacheServer, Start.CachePort!.ToInt32(), Start.Access, false);
                await _service.CreateAsync(cacheName, _returnCode);
            }
        }
        catch
        {
            // ignored
        }

        return Ok(_returnCode);
    }

    [HttpPost]
    public async Task<ActionResult<string>> SaveNAICS([FromBody] NAICS naics, string cacheName = "NAICS")
    {
        await using SqlConnection _con = new(Start.ConnectionString);
        string _returnCode = "";
        await using SqlCommand _command = new("Admin_SaveNAICS", _con);
        _command.CommandType = CommandType.StoredProcedure;
        _command.Int("ID", naics.ID.DBNull());

        _command.Varchar("NAICS", 100, naics.Title);

        try
        {
            await _con.OpenAsync();

            _returnCode = (await _command.ExecuteScalarAsync())?.ToString() ?? "";

            if (_returnCode.NotNullOrWhiteSpace() && _returnCode != "[]" && cacheName.NotNullOrWhiteSpace())
            {
                RedisService _service = new(Start.CacheServer, Start.CachePort!.ToInt32(), Start.Access, false);
                await _service.CreateAsync(cacheName, _returnCode);
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error saving NAICS. {ExceptionMessage}", ex.Message);
        }
        finally
        {
            await _con.CloseAsync();
        }

        return Ok(_returnCode);
    }

    [HttpPost]
    public async Task<ActionResult<string>> SaveRole([FromBody] Role role, string cacheName = "Roles")
    {
        await using SqlConnection _con = new(Start.ConnectionString);
        string _returnCode = "";
        await using SqlCommand _command = new("Admin_SaveRole", _con);
        _command.CommandType = CommandType.StoredProcedure;
        _command.Int("ID", role.ID.DBNull());
        _command.Varchar("RoleName", 10, role.RoleName);
        _command.Varchar("RoleDescription", 255, role.Description);
        _command.Bit("@CreateOrEditCompany", role.CreateOrEditCompany);
        _command.Bit("@CreateOrEditCandidate", role.CreateOrEditCandidate);
        _command.Bit("@ViewAllCompanies", role.ViewAllCompanies);
        _command.Bit("@ViewMyCompanyProfile", role.ViewMyCompanyProfile);
        _command.Bit("@EditMyCompanyProfile", role.EditMyCompanyProfile);
        _command.Bit("@CreateOrEditRequisitions", role.CreateOrEditRequisitions);
        _command.Bit("@ViewOnlyMyCandidates", role.ViewOnlyMyCandidates);
        _command.Bit("@ViewAllCandidates", role.ViewAllCandidates);
        _command.Bit("@ViewRequisitions", role.ViewRequisitions);
        _command.Bit("@EditRequisitions", role.EditRequisitions);
        _command.Bit("@ManageSubmittedCandidates", role.ManageSubmittedCandidates);
        _command.Bit("@DownloadOriginal", role.DownloadOriginal);
        _command.Bit("@DownloadFormatted", role.DownloadFormatted);
        _command.Bit("@AdminScreens", role.AdminScreens);
        _command.Varchar("User", 10, "ADMIN");

        try
        {
            await _con.OpenAsync();
            _returnCode = (await _command.ExecuteScalarAsync())?.ToString() ?? "";

            if (_returnCode.NotNullOrWhiteSpace() && _returnCode != "[]" && cacheName.NotNullOrWhiteSpace())
            {
                RedisService _service = new(Start.CacheServer, Start.CachePort!.ToInt32(), Start.Access, false);
                await _service.CreateAsync(cacheName, _returnCode);
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error saving roles. {ExceptionMessage}", ex.Message);
            return StatusCode(500, "Error saving roles.");
        }

        return Ok(_returnCode);
    }

    [HttpPost]
    public async Task<ActionResult<string>> SaveState([FromBody] State state, string cacheName = "States")
    {
        await using SqlConnection _con = new(Start.ConnectionString);
        string _returnCode = "";
        await using SqlCommand _command = new("Admin_SaveState", _con);
        _command.CommandType = CommandType.StoredProcedure;
        _command.Int("ID", state.ID.DBNull());
        _command.Varchar("Code", 2, state.Code);
        _command.Varchar("State", 50, state.StateName);
        _command.Varchar("Country", 50, "USA");
        _command.Varchar("User", 10, "ADMIN");

        try
        {
            await _con.OpenAsync();
            _returnCode = (await _command.ExecuteScalarAsync())?.ToString() ?? "";

            if (_returnCode.NotNullOrWhiteSpace() && _returnCode != "[]" && cacheName.NotNullOrWhiteSpace())
            {
                RedisService _service = new(Start.CacheServer, Start.CachePort!.ToInt32(), Start.Access, false);
                await _service.CreateAsync(cacheName, _returnCode);
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error saving state. {ExceptionMessage}", ex.Message);
            return StatusCode(500, "Error saving state.");
        }

        return Ok(_returnCode);
    }

    [HttpPost]
    public async Task<ActionResult<string>> SaveUser([FromBody] User user, string cacheName = "Users")
    {
        await using SqlConnection _con = new(Start.ConnectionString);

        string _returnCode = "";
        byte[] _salt = user.Password.NullOrWhiteSpace() ? new byte[64] : General.GenerateRandomString(64);
        byte[] _password = user.Password.NullOrWhiteSpace() ? new byte[64] : General.ComputeHashWithSalt(user.Password, _salt);
        General.GenerateRandomString();
        await using SqlCommand _command = new("Admin_SaveUser", _con);
        _command.CommandType = CommandType.StoredProcedure;
        _command.Varchar("UserName", 10, user.UserName);
        _command.Varchar("FirstName", 50, user.FirstName);
        _command.Varchar("LastName", 200, user.LastName);
        _command.Varchar("Email", 200, user.EmailAddress);
        _command.TinyInt("Role", user.RoleID);
        _command.Bit("Status", user.StatusEnabled);
        _command.Varchar("User", 10, "ADMIN");
        _command.Binary("Salt", 64, user.Password.NullOrWhiteSpace() ? DBNull.Value : _salt);
        _command.Binary("Password", 64, user.Password.NullOrWhiteSpace() ? DBNull.Value : _password);
        //_command.Varchar("Passwd", 30, user.Password.NullOrWhiteSpace() ? DBNull.Value : user.Password);
        try
        {
            await _con.OpenAsync();
            _returnCode = (await _command.ExecuteScalarAsync())?.ToString() ?? "";

            if (_returnCode.NotNullOrWhiteSpace() && _returnCode != "[]" && cacheName.NotNullOrWhiteSpace())
            {
                RedisService _service = new(Start.CacheServer, Start.CachePort!.ToInt32(), Start.Access, false);
                await _service.CreateAsync(cacheName, _returnCode);
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error saving user. {ExceptionMessage}", ex.Message);
            return StatusCode(500, "Error saving user.");
            // ignored
        }

        return Ok(_returnCode);
    }

    [HttpPost]
    public async Task<ActionResult<string>> SaveWorkflow([FromBody] Workflow workflow, string cacheName = nameof(CacheObjects.Workflow))
    {
        await using SqlConnection _con = new(Start.ConnectionString);

        string _returnCode = "", _cacheValue = "";
        await using SqlCommand _command = new("Admin_SaveWorkflow", _con);
        _command.CommandType = CommandType.StoredProcedure;
        _command.Int("ID", workflow.ID.DBNull());
        _command.Varchar("Next", 100, workflow.Next);
        _command.Bit("IsLast", workflow.IsLast);
        _command.Varchar("Role", 50, workflow.RoleIDs);
        _command.Bit("Schedule", workflow.Schedule);
        _command.Bit("AnyStage", workflow.AnyStage);
        _command.Varchar("User", 10, "ADMIN");
        try
        {
            await _con.OpenAsync();
            await using SqlDataReader _reader = await _command.ExecuteReaderAsync();
            while (await _reader.ReadAsync())
            {
                _returnCode = _reader.NString(0, "[]");
            }

            //_returnCode = (await _command.ExecuteScalarAsync())?.ToString() ?? "";
            
            await _reader.NextResultAsync();
            while (await _reader.ReadAsync())
            {
                _cacheValue = _reader.NString(0, "[]");
            }

            if (_cacheValue.NotNullOrWhiteSpace() && _cacheValue != "[]" && cacheName.NotNullOrWhiteSpace())
            {
                //RedisService _service = new(Start.CacheServer, Start.CachePort!.ToInt32(), Start.Access, false);
                await redisService.CreateAsync(cacheName, _returnCode);
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error saving user. {ExceptionMessage}", ex.Message);
            return StatusCode(500, "Error saving user.");
            // ignored
        }

        return Ok(_returnCode);
    }
    
    /// <summary>
    ///     Toggles the administrative list based on the provided method name, ID, and username.
    /// </summary>
    /// <param name="methodName">The name of the stored procedure to be executed.</param>
    /// <param name="id">The ID or code to be processed. It can be an integer or a string.</param>
    /// <param name="userName">The username to be processed.</param>
    /// <param name="idIsString">
    ///     A flag indicating whether the provided ID is a string. If false, the ID is treated as an
    ///     integer.
    /// </param>
    /// <param name="isUser">
    ///     A flag indicating whether the provided ID is a user. If true, the ID is treated as a user code.
    /// </param>
    /// <returns>The ID or code that was processed.</returns>
    /// <remarks>
    ///     This method establishes a connection to the database using a connection string from the configuration.
    ///     It then creates a SQL command using the provided method name and sets the command type to stored procedure.
    ///     Depending on the 'idIsString' and 'isUser' flags, it either adds an integer parameter 'ID', a character parameter
    ///     'Code', or a varchar parameter 'Code' to the
    ///     command.
    ///     It also adds a varchar parameter 'User' to the command with the provided username.
    ///     After executing the command, it returns the ID or code that was processed.
    /// </remarks>
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
            Log.Error(ex, "Error toggling " + methodName + ". {ExceptionMessage}", ex.Message);
            return StatusCode(500, $"Error toggling {methodName}.");
        }
        finally
        {
            await _con.CloseAsync();
        }

        {
            // ignored
        }

        return Ok(_returnCode);
    }
}