#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.API
// File Name:           CompanyController.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu, Brijesh Dubey
// Created On:          07-15-2025 21:07
// Last Updated On:     07-15-2025 21:02
// *****************************************/

#endregion

namespace Subscription.API.Controllers;

/// <summary>
///     Represents a controller for handling company related requests.
/// </summary>
/*/// <param name="configuration">
///     The application configuration, injected by the ASP.NET Core DI container.
/// </param>*/
[ApiController, Route("api/[controller]/[action]"), SuppressMessage("ReSharper", "UnusedParameter.Global")]
public partial class CompanyController : ControllerBase
{
    /// <summary>
    ///     Asynchronously checks if a company's Employer Identification Number (EIN) exists in the database.
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
    public async Task<ActionResult<bool>> CheckEIN(int companyID, string ein)
    {
        return await ExecuteBooleanAsync("CheckEIN", command =>
                                                     {
                                                         command.Int("ID", companyID);
                                                         command.Varchar("EIN", 10, ein);
                                                     }, "CheckEIN", "Error checking EIN existence."); //true means the EIN exists and false means the EIN doesn't exist.
    }

    [HttpPost]
    public async Task<ActionResult<string>> SaveNotes(CandidateNotes candidateNote, int companyID, string user)
    {
        if (candidateNote == null)
        {
            return NotFound("No Note information is found");
        }

        return await ExecuteScalarAsync("SaveNote", command =>
                                                   {
                                                       command.Int("Id", candidateNote.ID);
                                                       command.Int("CandidateID", companyID);
                                                       command.Varchar("Note", -1, candidateNote.Notes);
                                                       command.Bit("IsPrimary", false);
                                                       command.Varchar("EntityType", 5, "CLI");
                                                       command.Varchar("User", 10, user);
                                                   }, "SaveNotes", "Error saving company notes.");
    }

    [HttpPost]
    public async Task<ActionResult<string>> DeleteCompanyDocument([FromQuery] int documentID, [FromQuery] string user)
    {
        //TODO: make sure you delete the associated document from Azure filesystem too.
        return await ExecuteScalarAsync("DeleteCompanyDocument", command =>
                                                                 {
                                                                     command.Int("ID", documentID);
                                                                     command.Varchar("User", 10, user);
                                                                 }, "DeleteCompanyDocument", "Error deleting company document.");
    }

    private async Task<ActionResult<bool>> ExecuteBooleanAsync(string procedureName, Action<SqlCommand> parameterBinder, string logContext, string errorMessage)
    {
        await using SqlConnection _connection = new(Start.ConnectionString);
        await using SqlCommand _command = new(procedureName, _connection);
        _command.CommandType = CommandType.StoredProcedure;

        parameterBinder(_command);

        bool _result = false;
        try
        {
            await _connection.OpenAsync();
            _result = (await _command.ExecuteScalarAsync()).ToBoolean();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error executing {logContext} query. {ExceptionMessage}", logContext, ex.Message);
            return StatusCode(500, errorMessage);
        }

        return Ok(_result);
    }

    private async Task<ActionResult<string>> ExecuteScalarAsync(string procedureName, Action<SqlCommand> parameterBinder, string logContext, string errorMessage)
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
        catch (Exception ex)
        {
            Log.Error(ex, "Error executing {logContext} query. {ExceptionMessage}", logContext, ex.Message);
            return StatusCode(500, errorMessage);
        }

        return Ok(_result);
    }

    private async Task<ActionResult<T>> ExecuteReaderAsync<T>(string procedureName, Action<SqlCommand> parameterBinder, Func<SqlDataReader, Task<T>> resultProcessor, string logContext, string errorMessage)
    {
        await using SqlConnection _connection = new(Start.ConnectionString);
        await using SqlCommand _command = new(procedureName, _connection);
        _command.CommandType = CommandType.StoredProcedure;

        parameterBinder(_command);

        try
        {
            await _connection.OpenAsync();
            await using SqlDataReader _reader = await _command.ExecuteReaderAsync();
            
            T _result = await resultProcessor(_reader);
            return Ok(_result);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error executing {logContext} query. {ExceptionMessage}", logContext, ex.Message);
            return StatusCode(500, errorMessage);
        }
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
        return await ExecuteReaderAsync("GetCompanyDetails", command =>
        {
            command.Int("CompanyID", companyID);
            command.Varchar("User", 10, user);
        }, async reader =>
        {
            // Memory optimization: Interned string constants for optimal performance
            const string emptyArray = "[]";

            string _company = "[]", _locations = "[]", _contacts = "[]", _documents = "[]", _requisitions = "[]", _notes = "[]";

            // Company Details
            _company = await ReadNextResultAsync();
            if (_company == emptyArray)
            {
                _company = "{}";
            }
            // Company Locations
            await reader.NextResultAsync();
            _locations = await ReadNextResultAsync();

            // Company Contacts
            await reader.NextResultAsync();
            _contacts = await ReadNextResultAsync();
            
            // Company Notes
            await reader.NextResultAsync();
            _notes = await ReadNextResultAsync();

            // Company Documents
            await reader.NextResultAsync();
            _documents = await ReadNextResultAsync();

            // Company Requisitions
            await reader.NextResultAsync();
            _requisitions = await ReadNextResultAsync();

            return new ReturnCompanyDetails(_company, _contacts, _locations, _documents, _requisitions, _notes);

            // Memory optimization: Local function eliminates code duplication for result reading
            async Task<string> ReadNextResultAsync()
            {
                return await reader.ReadAsync() ? reader.NString(0, emptyArray) : emptyArray;
            }
        }, "GetCompanyDetails", "Error fetching company details.");
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
    [HttpPost]
    public async Task<ActionResult<ReturnGrid>> GetGridCompanies(CompanySearch searchModel, bool getMasterTables = true)
    {
        return await ExecuteReaderAsync("GetCompanies", command =>
        {
            command.Int("RecordsPerPage", searchModel.ItemCount);
            command.Int("PageNumber", searchModel.Page);
            command.Int("SortColumn", searchModel.SortField);
            command.TinyInt("SortDirection", searchModel.SortDirection);
            command.Varchar("Name", 30, searchModel.CompanyName);
            //_command.Varchar("Phone", 20, searchModel.Phone);
            //_command.Varchar("Email", 255, searchModel.EmailAddress);
            //_command.Varchar("State", 255, searchModel.State);
            //_command.Bit("MyCompanies", searchModel.MyCompanies);
            //_command.Varchar("Status", 50, searchModel.Status);
            command.Varchar("UserName", 10, ""); //searchModel.User);
        }, async reader =>
        {
            int _count = 0;
            string _companies = "[]";
            
            // Count
            await reader.ReadAsync();
            _count = reader.GetInt32(0);

            // Companies
            await reader.NextResultAsync();
            if (await reader.ReadAsync())
            {
                _companies = reader.NString(0);
            }

            return new ReturnGrid(_companies, _count);
        }, "GetGridCompanies", "Error fetching companies grid data.");
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
        return await ExecuteScalarAsync("GetLocationList", command => { command.Int("CompanyID", companyID); }, "GetLocationList", "Error fetching company location list.");
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
    public async Task<ActionResult<ReturnCompanyDetails>> SaveCompany(CompanyDetails company, string user)
    {
        if (company == null)
        {
            return StatusCode(500, "An internal error occurred while saving the company details.");
        }

        return await ExecuteReaderAsync("SaveCompany", command =>
        {
            command.Int("ID", company.ID);
            command.Varchar("CompanyName", 100, company.Name);
            command.Varchar("EIN", 9, company.EIN);
            command.Varchar("WebsiteURL", 255, company.Website);
            command.Varchar("DUN", 20, company.DUNS);
            command.Varchar("NAICSCode", 6, company.NAICSCode);
            command.Bit("Status", company.Status);
            command.Varchar("Notes", 2000, company.Notes);
            command.Varchar("StreetName", 500, company.StreetName);
            command.Varchar("City", 100, company.City);
            command.TinyInt("StateID", company.StateID);
            command.Varchar("ZipCode", 10, company.ZipCode);
            command.Varchar("CompanyEmail", 255, company.EmailAddress);
            command.Varchar("Phone", 20, company.Phone);
            command.Varchar("Extension", 10, company.Extension);
            command.Varchar("Fax", 20, company.Fax);
            command.Varchar("LocationNotes", 2000, company.LocationNotes);
            command.Varchar("User", 10, user);
        }, async reader =>
        {
            string _companyDetails = "[]", _companyLocations = "[]";
            
            // Company Details
            if (await reader.ReadAsync())
            {
                _companyDetails = reader.NString(0);
            }

            // Company Locations
            await reader.NextResultAsync();
            if (await reader.ReadAsync())
            {
                _companyLocations = reader.NString(0);
            }

            return new ReturnCompanyDetails(_companyDetails, "[]", _companyLocations, "[]", "[]", "[]");
        }, "SaveCompany", "Error saving company details.");
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

        return await ExecuteScalarAsync("SaveCompanyContact", command =>
                                                              {
                                                                  command.Int("ID", contact.ID);
                                                                  command.Int("CompanyID", contact.CompanyID);
                                                                  command.Varchar("Prefix", 10, contact.Prefix);
                                                                  command.Varchar("FirstName", 50, contact.FirstName);
                                                                  command.Varchar("MiddleInitial", 10, contact.MiddleInitial);
                                                                  command.Varchar("LastName", 50, contact.LastName);
                                                                  command.Varchar("Suffix", 10, contact.Suffix);
                                                                  command.Int("CompanyLocationID", contact.LocationID);
                                                                  command.Varchar("Email", 255, contact.EmailAddress);
                                                                  command.Varchar("Phone", 20, contact.Phone);
                                                                  command.Varchar("Extension", 10, contact.Extension);
                                                                  command.Varchar("Fax", 20, contact.Fax);
                                                                  command.Varchar("Designation", 255, contact.Title);
                                                                  command.Varchar("Department", 255, contact.Department);
                                                                  command.TinyInt("Role", contact.RoleID);
                                                                  command.Varchar("ContactNotes", 2000, contact.Notes);
                                                                  command.Bit("IsPrimaryContact", contact.PrimaryContact);
                                                                  command.Varchar("User", 10, user);
                                                              }, "SaveCompanyContact", "Error saving company contact.");
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

        return await ExecuteScalarAsync("SaveCompanyLocation", command =>
                                                               {
                                                                   command.Int("ID", location.ID);
                                                                   command.Int("CompanyID", location.CompanyID);
                                                                   command.Varchar("StreetName", 500, location.StreetName);
                                                                   command.Varchar("City", 100, location.City);
                                                                   command.TinyInt("StateID", location.StateID);
                                                                   command.Varchar("ZipCode", 10, location.ZipCode);
                                                                   command.Varchar("CompanyEmail", 255, location.EmailAddress);
                                                                   command.Varchar("Phone", 20, location.Phone);
                                                                   command.Varchar("Extension", 10, location.Extension);
                                                                   command.Varchar("Fax", 20, location.Fax);
                                                                   command.Varchar("LocationNotes", 2000, location.Notes);
                                                                   command.Bit("isPrimaryLocation", location.PrimaryLocation);
                                                                   command.Varchar("User", 10, user);
                                                               }, "SaveCompanyLocation", "Error saving company location.");
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
        return await ExecuteScalarAsync("SearchCompanies", command => { command.Varchar("Company", 30, filter); }, "SearchCompanies", "Error searching companies.");

        /*while (await _reader.ReadAsync())
        {
            companyNames.Add(new()
                             {
                                 Key = _reader.GetString(0),
                                 Value = _reader.GetString(0)
                             });
        }*/
    }

    [HttpPost, RequestSizeLimit(62_914_560)] //60 MB
    public async Task<ActionResult<string>> UploadDocument(IFormFile file)
    {
        string _fileName = file.FileName;
        string _companyID = Request.Form["companyID"].ToString();
        string _internalFileName = Guid.NewGuid().ToString("N");

        // Create the folder path
        string _blobPath = $"{Start.AzureBlobContainer}/Company/{_companyID}/{_internalFileName}";

        await General.UploadToBlob(file, _blobPath);
        /*// Create a BlobStorage instance
        IAzureBlobStorage _storage = StorageFactory.Blobs.AzureBlobStorageWithSharedKey(Start.AccountName, Start.AzureKey);

        await using (Stream stream = file.OpenReadStream())
        {
            await _storage.WriteAsync(_blobPath, stream);
        }*/

        return await ExecuteScalarAsync("SaveCompanyDocuments", command =>
                                                                {
                                                                    command.Int("CompanyId", _companyID.ToInt32());
                                                                    command.Varchar("DocumentName", 255, Request.Form["name"].ToString());
                                                                    command.Varchar("OriginalFileName", 255, _fileName);
                                                                    command.Varchar("InternalFileName", 50, _internalFileName);
                                                                    command.Varchar("Notes", 2000, Request.Form["notes"].ToString());
                                                                    command.Varchar("User", 10, Request.Form["user"].ToString());
                                                                }, "UploadDocument", "Error saving company document.");
    }
}