#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.API
// File Name:           CompanyController.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu
// Created On:          2-8-2024 15:54
// Last Updated On:     3-14-2024 19:37
// *****************************************/

#endregion

namespace Subscription.API.Controllers;

[ApiController, Route("api/[controller]/[action]")]
public class CompanyController : ControllerBase
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="CompanyController" /> class.
    /// </summary>
    /// <param name="configuration">The application configuration, injected by the ASP.NET Core DI container.</param>
    public CompanyController(IConfiguration configuration) => _configuration = configuration;

    private readonly IConfiguration _configuration;

    [HttpGet]
    public async Task<bool> CheckEIN(int companyID, string ein)
    {
        await using SqlConnection _connection = new(_configuration.GetConnectionString("DBConnect"));
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

    [HttpPost]
    public async Task<int> SaveCompany(CompanyDetails company, string user)
    {
        if (company == null)
        {
            return -1;
        }

        await using SqlConnection _connection = new(_configuration.GetConnectionString("DBConnect"));
        await _connection.OpenAsync();
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

        await using SqlDataReader _reader = await _command.ExecuteReaderAsync();

        if (_reader.HasRows)
        {
            await _reader.ReadAsync();
            _returnCode = _reader.GetInt32(0);
        }

        await _connection.CloseAsync();
        return _returnCode;
    }

    [HttpPost]
    public async Task<List<CompanyLocations>> SaveCompanyLocation(CompanyLocations location, string user)
    {
        if (location == null)
        {
            return null;
        }

        await using SqlConnection _connection = new(_configuration.GetConnectionString("DBConnect"));
        await _connection.OpenAsync();

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

        await using SqlDataReader _reader = await _command.ExecuteReaderAsync();

        List<CompanyLocations> _locations = [];

        while (_reader.Read())
        {
            _locations.Add(new()
                           {
                               ID = _reader.GetInt32(0),
                               CompanyID = _reader.GetInt32(1),
                               CompanyName = _reader.GetString(2),
                               StreetName = _reader.GetString(3),
                               City = _reader.GetString(4),
                               StateID = _reader.GetByte(5),
                               State = _reader.GetString(6),
                               ZipCode = _reader.GetString(7),
                               EmailAddress = _reader.GetString(8),
                               Phone = _reader.GetString(9),
                               Extension = _reader.GetString(10),
                               Fax = _reader.GetString(11),
                               PrimaryLocation = _reader.GetBoolean(12),
                               Notes = _reader.GetString(13),
                               CreatedBy = _reader.GetString(14),
                               CreatedDate = _reader.GetDateTime(15),
                               UpdatedBy = _reader.GetString(16),
                               UpdatedDate = _reader.GetDateTime(17)
                           });
        }

        await _connection.CloseAsync();
        return _locations;
    }

    /// <summary>
    ///     Retrieves the details of a company based on the provided company ID and user.
    /// </summary>
    /// <param name="companyID">The ID of the company whose details are to be retrieved.</param>
    /// <param name="user">The user requesting the company details.</param>
    /// <returns>A dictionary containing the details of the company, its contacts, requisitions, and documents.</returns>
    [HttpGet]
    public async Task<ActionResult<Dictionary<string, object>>> GetCompanyDetails(int companyID, string user)
    {
        await using SqlConnection _connection = new(_configuration.GetConnectionString("DBConnect"));
        CompanyDetails _company = null;
        string _companyName = "";

        await using SqlCommand _command = new("GetCompanyDetails", _connection);
        _command.CommandType = CommandType.StoredProcedure;
        _command.Int("CompanyID", companyID);
        _command.Varchar("User", 10, user);

        await _connection.OpenAsync();
        List<CompanyLocations> _locations = [];
        List<CompanyContacts> _contacts = [];
        List<CompanyDocuments> _documents = [];
        try
        {
            await using SqlDataReader _reader = await _command.ExecuteReaderAsync();
            if (_reader.HasRows) //Company Details
            {
                _reader.Read();
                _company = new()
                           {
                               ID = _reader.GetInt32(0),
                               Name = _reader.GetString(1),
                               EmailAddress = _reader.GetString(2),
                               EIN = _reader.GetString(3),
                               Phone = _reader.GetString(4),
                               Extension = _reader.GetString(5),
                               Fax = _reader.GetString(6),
                               StreetName = _reader.GetString(7),
                               City = _reader.GetString(8),
                               StateID = _reader.GetByte(9),
                               State = _reader.GetString(10),
                               ZipCode = _reader.GetString(11),
                               Website = _reader.GetString(12),
                               DUNS = _reader.GetString(13),
                               NAICSCode = _reader.GetString(14).ToInt32(),
                               Status = _reader.GetBoolean(15),
                               Notes = _reader.GetString(16),
                               LocationNotes = _reader.GetString(17),
                               CreatedBy = _reader.GetString(18),
                               CreatedDate = _reader.GetDateTime(19),
                               UpdatedBy = _reader.GetString(20),
                               UpdatedDate = _reader.GetDateTime(21)
                           };
                _companyName = _reader.GetString(1);
            }

            _reader.NextResult(); //Company Locations
            while (_reader.Read())
            {
                _locations.Add(new()
                               {
                                   ID = _reader.GetInt32(0),
                                   CompanyID = _reader.GetInt32(1),
                                   CompanyName = _companyName,
                                   StreetName = _reader.GetString(2),
                                   City = _reader.GetString(3),
                                   StateID = _reader.GetByte(4),
                                   State = _reader.GetString(5),
                                   ZipCode = _reader.GetString(6),
                                   EmailAddress = _reader.GetString(7),
                                   Phone = _reader.GetString(8),
                                   Extension = _reader.GetString(9),
                                   Fax = _reader.GetString(10),
                                   PrimaryLocation = _reader.GetBoolean(11),
                                   Notes = _reader.GetString(12),
                                   CreatedBy = _reader.GetString(13),
                                   CreatedDate = _reader.GetDateTime(14),
                                   UpdatedBy = _reader.GetString(15),
                                   UpdatedDate = _reader.GetDateTime(16)
                               });
            }

            _reader.NextResult(); //Company Contacts
            while (_reader.Read())
            {
                _contacts.Add(new()
                              {
                                  ID = _reader.GetInt32(0),
                                  CompanyID = _reader.GetInt32(1),
                                  CompanyName = _companyName,
                                  Prefix = _reader.GetString(2),
                                  FirstName = _reader.GetString(3),
                                  MiddleInitial = _reader.GetString(4),
                                  LastName = _reader.GetString(5),
                                  Suffix = _reader.GetString(6),
                                  StreetName = _reader.GetString(7),
                                  City = _reader.GetString(8),
                                  StateID = _reader.GetByte(9),
                                  State = _reader.GetString(10),
                                  ZipCode = _reader.GetString(11),
                                  EmailAddress = _reader.GetString(12),
                                  Phone = _reader.GetString(13),
                                  Extension = _reader.GetString(14),
                                  Fax = _reader.GetString(15),
                                  Title = _reader.GetString(16),
                                  Department = _reader.GetString(17),
                                  CreatedBy = _reader.GetString(18),
                                  CreatedDate = _reader.GetDateTime(19),
                                  UpdatedBy = _reader.GetString(20),
                                  UpdatedDate = _reader.GetDateTime(21)
                              });
            }

            _reader.NextResult(); //Company Documents
            if (_reader.HasRows)
            {
                while (_reader.Read())
                {
                    _documents.Add(new()
                                   {
                                       ID = _reader.GetInt32(0),
                                       CompanyID = _reader.GetInt32(1),
                                       CompanyName = _companyName,
                                       DocumentName = _reader.GetString(2),
                                       FileName = _reader.GetString(3),
                                       Notes = _reader.GetString(4),
                                       UpdatedBy = _reader.GetString(20),
                                       UpdatedDate = _reader.GetDateTime(21)
                                   });
                }
            }

            await _reader.CloseAsync();

            await _connection.CloseAsync();
        }
        catch (Exception)
        {
            //
        }

        return new Dictionary<string, object>
               {
                   {
                       "Company", _company
                   },
                   {
                       "Contacts", _contacts
                   },
                   {
                       "Locations", _locations
                   },
                   {
                       "Documents", _documents
                   }
               };
    }

    [HttpGet]
    public async Task<Dictionary<string, object>> GetGridCompanies([FromBody] CompanySearch searchModel, bool getMasterTables = true)
    {
        await using SqlConnection _connection = new(_configuration.GetConnectionString("DBConnect"));
        List<Company> _companies = [];
        await using SqlCommand _command = new("GetCompanies", _connection);
        _command.CommandType = CommandType.StoredProcedure;
        _command.Int("RecordsPerPage", searchModel.ItemCount);
        _command.Int("PageNumber", searchModel.Page);
        _command.Int("SortColumn", searchModel.SortField);
        _command.TinyInt("SortDirection", searchModel.SortDirection);
        //_command.Varchar("Name", 255, searchModel.CompanyName);
        //_command.Varchar("Phone", 20, searchModel.Phone);
        //_command.Varchar("Email", 255, searchModel.EmailAddress);
        //_command.Varchar("State", 255, searchModel.State);
        //_command.Bit("MyCompanies", searchModel.MyCompanies);
        //_command.Varchar("Status", 50, searchModel.Status);
        _command.Varchar("UserName", 10, searchModel.User);
        _command.Bit("GetMasterTables", getMasterTables);

        await _connection.OpenAsync();
        await using SqlDataReader _reader = await _command.ExecuteReaderAsync();

        int _count = 0;

        List<IntValues> _naics = [], _states = [];

        _naics.Add(new()
                   {
                       Key = "--Select",
                       Value = 0
                   });
        while (await _reader.ReadAsync())
        {
            _naics.Add(new()
                       {
                           Value = _reader.GetInt32(0),
                           Key = _reader.GetString(1)
                       });
        }

        await _reader.NextResultAsync();
        while (await _reader.ReadAsync())
        {
            _states.Add(new()
                        {
                            Value = _reader.GetInt32(0),
                            Key = _reader.GetString(1)
                        });
        }

        await _reader.NextResultAsync();
        while (await _reader.ReadAsync())
        {
            _companies.Add(new()
                           {
                               ID = _reader.GetInt32(0),
                               CompanyName = _reader.GetString(1),
                               Email = _reader.GetString(2),
                               Phone = _reader.GetString(3),
                               Address = _reader.GetString(4),
                               Website = _reader.GetString(5),
                               Status = _reader.GetBoolean(6),
                               ContactsCount = _reader.GetInt32(7),
                               LocationsCount = _reader.GetInt32(8),
                               UpdatedBy = _reader.GetString(9),
                               UpdatedDate = _reader.GetDateTime(10)
                           });
            if (_count == 0)
            {
                _count = _reader.GetInt32(11);
            }
        }

        await _reader.CloseAsync();

        await _connection.CloseAsync();

        return new()
               {
                   {
                       "Companies", _companies
                   },
                   {
                       "Count", _count
                   },
                   {
                       "NAICS", _naics
                   },
                   {
                       "States", _states
                   }
               };
    }
}