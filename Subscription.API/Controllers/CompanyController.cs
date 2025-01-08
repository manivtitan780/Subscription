#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.API
// File Name:           CompanyController.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu
// Created On:          02-08-2024 15:02
// Last Updated On:     01-07-2025 16:01
// *****************************************/

#endregion

namespace Subscription.API.Controllers;

/// <summary>
///     Represents a controller for handling company related requests.
/// </summary>
/*/// <param name="configuration">
///     The application configuration, injected by the ASP.NET Core DI container.
/// </param>*/
[ApiController, Route("api/[controller]/[action]")]
public class CompanyController : ControllerBase
{
    /// <summary>
    ///     Asynchronously checks if a company's Employer Identification Number (EIN) exists in the database.
    ///     <para>
    ///         <br />This method, Add, is designed to calculate the sum of two integers.
    ///         It takes two parameters, each representing an integer. The method
    ///         performs the addition operation on these two integers and returns
    ///         the result. This result is an integer representing the sum of the
    ///         input parameters.
    ///     </para>
    /// </summary>
    /// <param name="companyID">
    ///     The ID of the company to check.
    /// </param>
    /// <param name="ein">
    ///     The Employer Identification Number (EIN) to check.
    /// </param>
    /// <returns>
    ///     A task that represents the asynchronous operation. The task result contains a boolean value indicating whether
    ///     the EIN exists (true) or not (false).
    /// </returns>
    [HttpGet]
    public async Task<bool> CheckEIN(int companyID, string ein)
    {
        await using SqlConnection _connection = new(Start.ConnectionString);
        await using SqlCommand _command = new("CheckEIN", _connection);
        _command.CommandType = CommandType.StoredProcedure;
        _command.Int("ID", companyID);
        _command.Varchar("EIN", 10, ein);
        await _connection.OpenAsync();
        bool _returnValue = false;
        try
        {
            _returnValue = _command.ExecuteScalar().ToBoolean(); //true means the EIN exists and false means the EIN doesn't exist.
        }
        catch
        {
            //
        }

        await _connection.CloseAsync();

        return _returnValue;
    }

    /// <summary>
    ///     Retrieves the details of a company based on the provided company ID and user.
    ///     <para>
    ///         <br />This asynchronous method, GetCompanyDetails, is designed to retrieve the details of a specific company
    ///         based on
    ///         the provided company ID and user. It interacts with the database to fetch the company's details, its contacts,
    ///         requisitions, and documents. The method returns a dictionary containing these details. If the company ID or
    ///         user is not found, the method will return an empty dictionary.
    ///     </para>
    /// </summary>
    /// <summary>
    /// </summary>
    /// <param name="companyID">
    ///     The ID of the company whose details are to be retrieved.
    /// </param>
    /// <param name="user">
    ///     The user requesting the company details.
    /// </param>
    /// <returns>
    ///     A dictionary containing the details of the company, its contacts, requisitions, and documents.
    /// </returns>
    [HttpGet]
    public async Task<ActionResult<ReturnCompanyDetails>> GetCompanyDetails(int companyID, string user)
    {
        await using SqlConnection _connection = new(Start.ConnectionString);

        await using SqlCommand _command = new("GetCompanyDetails", _connection);
        _command.CommandType = CommandType.StoredProcedure;
        _command.Int("CompanyID", companyID);
        _command.Varchar("User", 10, user);

        await _connection.OpenAsync();

        string _company = "[]", _locations = "[]", _contacts = "[]", _documents = "[]";
        try
        {
            await using SqlDataReader _reader = await _command.ExecuteReaderAsync();
            while (await _reader.ReadAsync()) //Company Details
            {
                _company = _reader.NString(0);
            }

            await _reader.NextResultAsync(); //Company Locations
            while (await _reader.ReadAsync())
            {
                _locations = _reader.NString(0);
            }

            await _reader.NextResultAsync(); //Company Contacts
            while (await _reader.ReadAsync())
            {
                _contacts = _reader.NString(0);
            }

            await _reader.NextResultAsync(); //Company Documents
            while (await _reader.ReadAsync())
            {
                _documents = _reader.NString(0);
            }

            await _reader.CloseAsync();

            await _connection.CloseAsync();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error getting company details. {ExceptionMessage}", ex.Message);
            return StatusCode(500, ex.Message);
        }

        return Ok(new ReturnCompanyDetails {Company = _company, Locations = _locations, Contacts = _contacts, Documents = _documents});
    }

    /// <summary>
    ///     Asynchronously retrieves a paginated list of companies based on the provided search model.
    /// </summary>
    /// <param name="searchModel">
    ///     The search model containing the search parameters.
    /// </param>
    /// <param name="getMasterTables">
    ///     A boolean value indicating whether to retrieve related master table data (true) or not
    ///     (false). Default is true.
    /// </param>
    /// <returns>
    ///     A task that represents the asynchronous operation. The task result contains a dictionary with the following keys:
    ///     - "Companies": A list of companies matching the search parameters.
    ///     - "Count": The total number of companies matching the search parameters.
    /// </returns>
    /*public async Task<Dictionary<string, object>> GetGridCompanies([FromBody] CompanySearch searchModel, bool getMasterTables = true)*/
    [HttpGet]
    public async Task<ActionResult<ReturnGrid>> GetGridCompanies([FromBody] CompanySearch searchModel, bool getMasterTables = true)
    {
        await using SqlConnection _connection = new(Start.ConnectionString);
        await using SqlCommand _command = new("GetCompanies", _connection);
        int _count = 0;
        string _companies = "[]";
        try
        {
            _command.CommandType = CommandType.StoredProcedure;
            _command.Int("RecordsPerPage", searchModel.ItemCount);
            _command.Int("PageNumber", searchModel.Page);
            _command.Int("SortColumn", searchModel.SortField);
            _command.TinyInt("SortDirection", searchModel.SortDirection);
            _command.Varchar("Name", 30, searchModel.CompanyName);
            //_command.Varchar("Phone", 20, searchModel.Phone);
            //_command.Varchar("Email", 255, searchModel.EmailAddress);
            //_command.Varchar("State", 255, searchModel.State);
            //_command.Bit("MyCompanies", searchModel.MyCompanies);
            //_command.Varchar("Status", 50, searchModel.Status);
            _command.Varchar("UserName", 10, ""); //searchModel.User);

            await _connection.OpenAsync();
            await using SqlDataReader _reader = await _command.ExecuteReaderAsync();

            await _reader.ReadAsync();
            _count = _reader.GetInt32(0);

            await _reader.NextResultAsync();
            while (await _reader.ReadAsync())
            {
                _companies = _reader.NString(0);
            }

            await _reader.CloseAsync();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error getting company details. {ExceptionMessage}", ex.Message);
            return StatusCode(500, ex.Message);
        }
        finally
        {
            await _connection.CloseAsync();
        }

        return Ok(new ReturnGrid {Data = _companies, Count = _count});
    }

    /// <summary>
    ///     Asynchronously retrieves a list of locations for a specific company from the database.
    /// </summary>
    /// <param name="companyID">
    ///     The ID of the company for which to retrieve locations.
    /// </param>
    /// <returns>
    ///     A task that represents the asynchronous operation. The task result contains a list of locations for the
    ///     specified company.
    /// </returns>
    [HttpGet]
    public async Task<ActionResult<string>> GetLocationList(int companyID)
    {
        await using SqlConnection _connection = new(Start.ConnectionString);
        string _returnValue = "[]";
        try
        {
            await using SqlCommand _command = new("GetLocationList", _connection);
            _command.CommandType = CommandType.StoredProcedure;
            _command.Int("CompanyID", companyID);

            await _connection.OpenAsync();

            // Execute the command asynchronously and get the data reader
            object _result = _command.ExecuteScalar()?.ToString();
            if (_result != null)
            {
                _returnValue = _result.ToString();
            }

            await _connection.CloseAsync();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error getting company details. {ExceptionMessage}", ex.Message);
            return StatusCode(500, ex.Message);
        }

        return Ok(_returnValue);
    }

    /// <summary>
    ///     Asynchronously saves the details of a company to the database.
    /// </summary>
    /// <param name="company">
    ///     The details of the company to save.
    /// </param>
    /// <param name="user">
    ///     The user performing the save operation.
    /// </param>
    /// <returns>
    ///     A task that represents the asynchronous operation. The task result contains an integer value indicating the result
    ///     of the save operation.
    ///     A return value of -1 indicates that the company parameter was null. Any other value is the return code from the
    ///     database operation.
    /// </returns>
    [HttpPost]
    public async Task<ActionResult<int>> SaveCompany(CompanyDetails company, string user)
    {
        if (company == null)
        {
            return StatusCode(500, "An internal error occurred while saving the company details.");
        }

        await using SqlConnection _connection = new(Start.ConnectionString);
        int _returnCode = 0;
        await using SqlCommand _command = new("SaveCompany", _connection);
        _command.CommandType = CommandType.StoredProcedure;
        _command.Int("ID", company.ID);
        _command.Varchar("CompanyName", 100, company.Name);
        _command.Varchar("EIN", 9, company.EIN);
        _command.Varchar("WebsiteURL", 255, company.Website);
        _command.Varchar("DUN", 20, company.DUNS);
        _command.Varchar("NAICSCode", 6, company.NAICSCode);
        _command.Bit("Status", company.Status);
        _command.Varchar("Notes", 2000, company.Notes);
        _command.Varchar("StreetName", 500, company.StreetName);
        _command.Varchar("City", 100, company.City);
        _command.TinyInt("StateID", company.StateID);
        _command.Varchar("ZipCode", 10, company.ZipCode);
        _command.Varchar("CompanyEmail", 255, company.EmailAddress);
        _command.Varchar("Phone", 20, company.Phone);
        _command.Varchar("Extension", 10, company.Extension);
        _command.Varchar("Fax", 20, company.Fax);
        _command.Varchar("LocationNotes", 2000, company.LocationNotes);
        _command.Varchar("User", 10, user);
        try
        {
            await _connection.OpenAsync();
            _returnCode = (await _command.ExecuteScalarAsync()).ToInt32();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error saving company details. {ExceptionMessage}", ex.Message);
            return StatusCode(500, ex.Message);
        }
        finally
        {
            await _connection.CloseAsync();
        }

        return Ok(_returnCode);
    }

    /// <summary>
    ///     Asynchronously saves the details of a company's contact to the database.
    /// </summary>
    /// <param name="contact">
    ///     The details of the company's contact to save.
    /// </param>
    /// <param name="user">
    ///     The user performing the save operation.
    /// </param>
    /// <returns>
    ///     A task that represents the asynchronous operation. The task result contains a list of company contacts.
    ///     If the contact parameter is null, the method returns null.
    /// </returns>
    [HttpPost]
    public async Task<ActionResult<string>> SaveCompanyContact(CompanyContacts contact, string user)
    {
        if (contact == null)
        {
            return Ok("[]");
        }

        await using SqlConnection _connection = new(Start.ConnectionString);

        await using SqlCommand _command = new("SaveCompanyContact", _connection);
        _command.CommandType = CommandType.StoredProcedure;
        _command.Int("ID", contact.ID);
        _command.Int("CompanyID", contact.CompanyID);
        _command.Varchar("Prefix", 10, contact.Prefix);
        _command.Varchar("FirstName", 50, contact.FirstName);
        _command.Varchar("MiddleInitial", 10, contact.MiddleInitial);
        _command.Varchar("LastName", 50, contact.LastName);
        _command.Varchar("Suffix", 10, contact.Suffix);
        _command.Int("CompanyLocationID", contact.LocationID);
        _command.Varchar("Email", 255, contact.EmailAddress);
        _command.Varchar("Phone", 20, contact.Phone);
        _command.Varchar("Extension", 10, contact.Extension);
        _command.Varchar("Fax", 20, contact.Fax);
        _command.Varchar("Designation", 255, contact.Title);
        _command.Varchar("Department", 255, contact.Department);
        _command.TinyInt("Role", contact.RoleID);
        _command.Varchar("ContactNotes", 2000, contact.Notes);
        _command.Bit("IsPrimaryContact", contact.PrimaryContact);
        _command.Varchar("User", 10, user);

        string _contacts = "[]";

        try
        {
            await _connection.OpenAsync();
            _contacts = (await _command.ExecuteScalarAsync())?.ToString();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error saving company contact. {ExceptionMessage}", ex.Message);
            return StatusCode(500, ex.Message);
        }
        finally
        {
            await _connection.CloseAsync();
        }

        return Ok(_contacts);
    }

    /// <summary>
    ///     Asynchronously saves the details of a company's location to the database.
    ///     <para>
    ///         <br />This method is responsible for saving the details of a company's location to the database.
    ///         It takes as parameters a CompanyLocations object, which contains the details of the location to be saved,
    ///         and a string representing the user performing the operation.
    ///         The method first checks if the location parameter is null, and if it is, it returns null.
    ///         Otherwise, it creates a new SQL connection and opens it.
    ///         It then creates a new SQL command with the stored procedure name "SaveCompanyLocation" and adds the details of
    ///         the
    ///         location and user to the command parameters.
    ///         The method then executes the command asynchronously and reads the returned rows, adding each row to a list of
    ///         CompanyLocations objects.
    ///         Finally, it closes the SQL connection and returns the list of CompanyLocations objects.
    ///         This method is asynchronous and returns a Task that wraps a list of CompanyLocations objects.
    ///     </para>
    /// </summary>
    /// <param name="location">
    ///     The details of the company's location to save.
    /// </param>
    /// <param name="user">
    ///     The user performing the save operation.
    /// </param>
    /// <returns>
    ///     A task that represents the asynchronous operation. The task result contains a list of company locations.
    ///     If the location parameter is null, the method returns null.
    /// </returns>
    [HttpPost]
    public async Task<ActionResult<string>> SaveCompanyLocation(CompanyLocations location, string user)
    {
        if (location == null)
        {
            return Ok("[]");
        }

        await using SqlConnection _connection = new(Start.ConnectionString);

        await using SqlCommand _command = new("SaveCompanyLocation", _connection);
        _command.CommandType = CommandType.StoredProcedure;
        _command.Int("ID", location.ID);
        _command.Varchar("CompanyID", 100, location.CompanyID);
        _command.Varchar("StreetName", 500, location.StreetName);
        _command.Varchar("City", 100, location.City);
        _command.TinyInt("StateID", location.StateID);
        _command.Varchar("ZipCode", 10, location.ZipCode);
        _command.Varchar("CompanyEmail", 255, location.EmailAddress);
        _command.Varchar("Phone", 20, location.Phone);
        _command.Varchar("Extension", 10, location.Extension);
        _command.Varchar("Fax", 20, location.Fax);
        _command.Varchar("LocationNotes", 2000, location.Notes);
        _command.Bit("isPrimaryLocation", location.PrimaryLocation);
        _command.Varchar("User", 10, user);

        string _locations = "[]";
        try
        {
            await _connection.OpenAsync();
            _locations = (await _command.ExecuteScalarAsync())?.ToString();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error saving company location. {ExceptionMessage}", ex.Message);
            return StatusCode(500, ex.Message);
        }
        finally
        {
            await _connection.CloseAsync();
        }

        return _locations;
    }

    /// <summary>
    ///     This asynchronous method, SearchCompanies, is designed to interact with the database to fetch a list of companies
    ///     based on a provided search string.
    ///     It accepts a single parameter, which is a string representing the company name or part of it.
    ///     The method calls a stored procedure named 'SearchCompanies' in the database, passing the search string as a
    ///     parameter.
    ///     The stored procedure is expected to return a list of company names that match the search string.
    ///     The method then reads these company names and returns them as a list of strings.
    ///     If no matches are found, the method will return an empty list.
    /// </summary>
    /// <param name="filter">
    ///     The company name or part of it to search for.
    /// </param>
    /// <returns>
    ///     A task that represents the asynchronous operation. The task result contains a list of company names matching the
    ///     search string.
    /// </returns>
    [HttpGet]
    public async Task<ActionResult<string>> SearchCompanies(string filter)
    {
        await using SqlConnection _connection = new(Start.ConnectionString);
        await using SqlCommand _command = new("SearchCompanies", _connection);
        _command.CommandType = CommandType.StoredProcedure;
        _command.Varchar("Company", 30, filter);

        string _companies = "[]";
        try
        {
            await _connection.OpenAsync();

            _companies = (await _command.ExecuteScalarAsync())?.ToString();

            /*while (await _reader.ReadAsync())
            {
                companyNames.Add(new()
                                 {
                                     Key = _reader.GetString(0),
                                     Value = _reader.GetString(0)
                                 });
            }*/
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error searching companies. {ExceptionMessage}", ex.Message);
            return StatusCode(500, ex.Message);
        }
        finally
        {
            await _connection.CloseAsync();
        }

        return _companies;
    }
}