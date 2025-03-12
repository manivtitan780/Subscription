#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.API
// File Name:           AdminController.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu, Brijesh Dubey
// Created On:          03-10-2025 15:03
// Last Updated On:     03-11-2025 20:03
// *****************************************/

#endregion

namespace Subscription.API.Controllers;

[ApiController, Route("api/[controller]/[action]")]
public class AdminController : ControllerBase
{
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
        string? _generalItems = "[]";

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

        string? _listOptions = "[]";
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

    /// <summary>
    ///     Saves the administrative list to the database.
    /// </summary>
    /// <param name="methodName">The name of the stored procedure to be executed.</param>
    /// <param name="parameterName">The name of the parameter to be passed to the stored procedure.</param>
    /// <param name="containDescription">A flag indicating whether the list contains a description.</param>
    /// <param name="isString">
    ///     A flag indicating whether the provided code is a string. If false, the code is treated as an
    ///     integer.
    /// </param>
    /// <param name="adminList">The administrative list to be saved.</param>
    /// <returns>A string value indicating the return code of the operation.</returns>
    /// <remarks>
    ///     This method establishes a connection to the database using a connection string from the configuration.
    ///     It then creates a SQL command using the provided method name and sets the command type to stored procedure.
    ///     Depending on the 'isString' flag, it either adds an integer parameter 'ID' or a character parameter 'Code' to the
    ///     command.
    ///     It also adds a varchar parameter with the provided parameter name and the text from the admin list.
    ///     If 'containDescription' is true, it adds another varchar parameter 'Desc' with the text from the admin list.
    ///     It also adds a varchar parameter 'User' with the value 'ADMIN' and a bit parameter 'Enabled' with the enabled
    ///     status from the admin list.
    ///     After executing the command, it reads the return code from the first column of the first row in the result set and
    ///     returns this value.
    ///     If an exception occurs during the execution of the command, it is caught and ignored, and an empty string is
    ///     returned.
    /// </remarks>
    [HttpPost]
    public async Task<ActionResult<string>> SaveAdminList(string methodName, string parameterName, bool containDescription, bool isString, [FromBody] AdminList adminList)
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

            _returnCode = (await _command.ExecuteScalarAsync())?.ToString() ?? "";
        }
        catch (SqlException ex)
        {
            Log.Error(ex, "Error saving " + parameterName + ". {ExceptionMessage}", ex.Message);
            return StatusCode(500, $"Error toggling {parameterName}.");
        }
        finally
        {
            await _con.CloseAsync();
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
            await _command.ExecuteNonQueryAsync();
        }
        catch (SqlException ex)
        {
            Log.Error(ex, "Error toggling "+ methodName + ". {ExceptionMessage}", ex.Message);
            return StatusCode(500, $"Error toggling {methodName}.");
        }
        finally
        {
            await _con.CloseAsync();
        }
        {
            // ignored
        }

        return Ok(id);
    }

}