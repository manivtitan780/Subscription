#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.Server
// File Name:           Companies.razor.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu
// Created On:          04-22-2024 15:04
// Last Updated On:     10-29-2024 15:10
// *****************************************/

#endregion

namespace Subscription.Server.Components.Pages;

public partial class Companies
{
    // private const string StorageName = "CompaniesGrid";
    private static TaskCompletionSource<bool> _initializationTaskSource;
    private List<CompanyContacts> _companyContacts = [];

    private CompanyDetails _companyDetails = new(), _companyDetailsClone = new();
    private List<CompanyDocuments> _companyDocuments = [];/**/
    private List<CompanyLocations> _companyLocations = [];
    private int _selectedTab;
    private readonly SemaphoreSlim _semaphoreMainPage = new(1, 1);

    private Company _target;

	/// <summary>
	///     Gets or sets the instance of the Address component. This component is used to display the address of the selected.
	/// </summary>
	private MarkupString Address
    {
        get;
        set;
    }

    private EditContact CompanyContactDialog
    {
        get;
        set;
    }

	/// <summary>
	///     Sets or gets the instance of the EditCompany component. This component is used to edit company details.
	/// </summary>
	private EditCompany CompanyEditDialog
    {
        get;
        set;
    }

    private EditLocation CompanyLocationDialog
    {
        get;
        set;
    }

    [Inject]
    private IConfiguration Configuration
    {
        get;
        set;
    }

	/// <summary>
	///     Gets or sets the total count of Companies.
	/// </summary>
	/// <remarks>
	///     This property is used to store the total count of companies retrieved from the API response in the
	///     `General.GetCompanyReadAdaptor()` method.
	/// </remarks>
	private static int Count
    {
        get;
        set;
    }

    public EditContext EditConCompany
    {
        get;
        set;
    }

    public EditContext EditConContact
    {
        get;
        set;
    }

    public EditContext EditConLocation
    {
        get;
        set;
    }

    private static SfGrid<Company> Grid
    {
        get;
        set;
    }

    private bool HasViewRights
    {
        get;
        set;
    }

	/// <summary>
	///     Gets or sets the instance of the IJSRuntime interface. This interface provides methods for interacting with
	///     JavaScript from .NET code.
	/// </summary>
	/// <remarks>
	///     The IJSRuntime instance is used to invoke JavaScript functions from .NET code. In the Companies class, it is used
	///     to open a new browser tab for document download in the `Companies.DownloadDocument()` method.
	/// </remarks>
	[Inject]
    private IJSRuntime JsRuntime
    {
        get;
        set;
    }

    [Inject]
    private ILocalStorageService LocalStorage
    {
        get;
        set;
    }

    private static List<IntValues> NAICS
    {
        get;
        set;
    } = [];

	/// <summary>
	///     Gets or sets the instance of the NavigationManager service used in the Companies page.
	///     This service provides methods and properties to manage and interact with the URI of the application.
	///     It is used for tasks such as navigating to different pages and constructing URIs for use within the application.
	///     For example, it is used in the `DownloadDocument` method to construct a URI for downloading a document.
	/// </summary>
	[Inject]
    private NavigationManager NavManager
    {
        get;
        set;
    }

    private ContactPanel PanelContacts
    {
        get;
        set;
    }

    private LocationPanel PanelLocations
    {
        get;
        set;
    }

    private static List<IntValues> Roles
    {
        get;
        set;
    } = [];

	/// <summary>
	///     Gets or sets the SearchModel property of the Companies class. This property is of type CompaniesSearch and is used
	///     to manage
	///     search criteria for companies.
	/// </summary>
	private static CompanySearch SearchModel
    {
        get;
        set;
    } = new();

	/*/// <summary>
	///     Gets or sets a clone of the CompanySearch model. This clone is used to manage the state of the search functionality
	///     in the Companies page. It holds the search parameters and criteria used to filter and display the company data in
	///     the grid view.
	/// </summary>
	private CompanySearch SearchModelClone
    {
        get;
        set;
    } = new();*/

    private CompanyContacts SelectedContact
    {
        get;
        set;
    } = new();

    private CompanyLocations SelectedLocation
    {
        get;
        set;
    } = new();

	/// <summary>
	///     Gets or sets an instance of the ILocalStorageService.
	///     This service is used for managing local storage in the application.
	///     It is used to store and retrieve the state of the Companies grid, including search parameters and pagination.
	/// </summary>
	[Inject]
    private ISessionStorageService SessionStorage
    {
        get;
        set;
    }

	/// <summary>
	///     Gets or sets the instance of the SfSpinner component.
	/// </summary>
	/// <remarks>
	///     This property is used to manage the spinner in the Companies page.
	///     The spinner is shown by calling the `ShowAsync` method and hidden by calling the `HideAsync` method of this
	///     property.
	///     For example, it is used in the `Companies.DetailDataBind()` and `Companies.EditCompany()` methods to indicate a
	///     loading state
	///     while performing asynchronous operations.
	/// </remarks>
	private SfSpinner Spinner
    {
        get;
        set;
    } = new();

    private static List<IntValues> State
    {
        get;
        set;
    } = [];

    private string User
    {
        get;
        set;
    }

    private static async Task AllAlphabets()
    {
        SearchModel.CompanyName = "";
        SearchModel.Page = 1;
        await Grid.Refresh();
    }

    private static async Task AutocompleteValueChange(ChangeEventArgs<string, KeyValues> filter)
    {
        SearchModel.CompanyName = filter.Value;
        SearchModel.Page = 1;
        await Grid.Refresh();
    }

    private async Task ClearFilter()
    {
        SearchModel.Clear();
        SearchModel.User = User;
        await Grid.Refresh();
    }

	/// <summary>
	///     Handles the OnInitializedAsync lifecycle event of the Companies page.
	/// </summary>
	/// <returns></returns>
	private Task DataHandler() => ExecuteMethod(async () =>
												{
													DotNetObjectReference<Companies> _dotNetReference = DotNetObjectReference.Create(this); // create dotnet ref
													await JsRuntime.InvokeAsync<string>("detail", _dotNetReference);
													//  send the dotnet ref to JS side
													if (Grid.TotalItemCount > 0)
													{
														await Grid.SelectRowAsync(0);
													}
												});

	/// <summary>
	///     Fires when the detail row is expanded in the Companies page grid view. This method is invoked from JavaScript.
	/// </summary>
	/// <param name="company"></param>
	/// <returns></returns>
	private Task DetailDataBind(DetailDataBoundEventArgs<Company> company) => ExecuteMethod(async () =>
																							{
																								if (_target != null && _target != company.Data)
																								{
																									// return when target is equal to args.data
																									await Grid.ExpandCollapseDetailRowAsync(_target);
																								}

																								int _index = await Grid.GetRowIndexByPrimaryKeyAsync(company.Data.ID);
																								if (_index != Grid.SelectedRowIndex)
																								{
																									await Grid.SelectRowAsync(_index);
																								}

																								_target = company.Data;

																								try
																								{
																									if (Spinner != null)
																									{
																										await Spinner.ShowAsync();
																									}
																								}
																								catch
																								{
																									//
																								}

																								Dictionary<string, string> _parameters = new()
																																		 {
																																			 {"companyID", _target.ID.ToString()},
																																			 {"user", User}
																																		 };
																								Dictionary<string, object> _restResponse =
																									await General.GetRest<Dictionary<string, object>>("Company/GetCompanyDetails",
																																					  _parameters);

																								if (_restResponse != null)
																								{
																									_companyDetails =
																										General.DeserializeObject<CompanyDetails>(_restResponse["Company"]?.ToString() ?? string.Empty);
																									EditConCompany = new(_companyDetails);
																									_companyContacts = General.DeserializeObject<List<CompanyContacts>>(_restResponse["Contacts"]);
																									_companyDocuments =
																										General.DeserializeObject<List<CompanyDocuments>>(_restResponse["Documents"]);
																									_companyLocations =
																										General.DeserializeObject<List<CompanyLocations>>(_restResponse["Locations"]);
																									SetupAddress();
																								}

																								_selectedTab = 0;

																								try
																								{
																									if (Spinner != null)
																									{
																										await Spinner.HideAsync();
																									}
																								}
																								catch
																								{
																									//
																								}
																							});

	/// <summary>
	///     Collapses the detail row in the Companies page grid view. This method is invoked from JavaScript.
	/// </summary>
	[JSInvokable("DetailCollapse")]
    public void DetailRowCollapse() => _target = null;

	/// <summary>
	///     Opens the company edit dialog for adding a new company or editing an existing one.
	/// </summary>
	/// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
	private Task EditCompany() => ExecuteMethod(async () =>
												{
													await General.DisplaySpinner(Spinner);

													if (_target == null || _target.ID == 0)
													{
														if (_companyDetailsClone == null)
														{
															_companyDetailsClone = new();
														}
														else
														{
															_companyDetailsClone.Clear();
														}

														_companyDetailsClone.IsAdd = true;
													}
													else
													{
														_companyDetailsClone = _companyDetails.Copy();
														_companyDetailsClone.IsAdd = false;
													}

													await General.DisplaySpinner(Spinner, false);

													await CompanyEditDialog.ShowDialog();
													//StateHasChanged();
												});

    private Task EditCompanyContact(int contact)
    {
        return ExecuteMethod(async () =>
                             {
                                 if (contact == 0)
                                 {
                                     if (SelectedContact == null)
                                     {
                                         SelectedContact = new();
                                     }
                                     else
                                     {
                                         SelectedContact.Clear();
                                     }

                                     SelectedContact.CompanyID = _target.ID;
                                 }
                                 else
                                 {
                                     SelectedContact = PanelContacts.SelectedRow != null ? PanelContacts.SelectedRow.Copy() : new();
                                 }

                                 EditConContact = new(SelectedLocation);
                                 await CompanyContactDialog.ShowDialog();
                             });
    }

    private Task EditLocation(int location)
    {
        return ExecuteMethod(async () =>
                             {
                                 if (location == 0)
                                 {
                                     if (SelectedLocation == null)
                                     {
                                         SelectedLocation = new();
                                     }
                                     else
                                     {
                                         SelectedLocation.Clear();
                                     }

                                     SelectedLocation.CompanyID = _target.ID;
                                 }
                                 else
                                 {
                                     SelectedLocation = PanelLocations.SelectedRow != null ? PanelLocations.SelectedRow.Copy() : new();
                                 }

                                 EditConLocation = new(SelectedLocation);
                                 await CompanyLocationDialog.ShowDialog();
                             });
    }

	/// <summary>
	///     Executes the provided task within a semaphore lock. If the semaphore is currently locked, the method will return
	///     immediately.
	///     If an exception occurs during the execution of the task, it will be logged using the provided logger.
	/// </summary>
	/// <param name="task">The task to be executed.</param>
	/// <returns>
	///     A task that represents the asynchronous operation.
	/// </returns>
	private Task ExecuteMethod(Func<Task> task) => General.ExecuteMethod(_semaphoreMainPage, task);

    private async Task GetAlphabets(char alphabet)
    {
        await ExecuteMethod(async () =>
                            {
                                SearchModel.CompanyName = alphabet.ToString();
                                SearchModel.Page = 1;
                                await Grid.Refresh();
                            });
    }

    private async Task GridPageChanging(GridPageChangingEventArgs page)
    {
        await ExecuteMethod(async () =>
                            {
                                if (page.CurrentPageSize != SearchModel.ItemCount)
                                {
                                    SearchModel.ItemCount = page.CurrentPageSize;
                                    SearchModel.Page = 1;
                                    await Grid.GoToPageAsync(1);
                                    await Task.Yield();
                                }
                                else
                                {
                                    SearchModel.Page = page.CurrentPage;
                                    await Grid.Refresh();
                                }
                            });
    }

    /*private Task OnActionBegin(ActionEventArgs<Company> company) => ExecuteMethod(async () =>
                                                                                  {
                                                                                      if (company.RequestType == GridAction.Sorting)
                                                                                      {
                                                                                          SearchModel.SortField = company.ColumnName switch
                                                                                                                  {
                                                                                                                      "CompanyName" => 1,
                                                                                                                      "Address" => 2,
                                                                                                                      "UpdatedDate" => 3,
                                                                                                                      _ => 3
                                                                                                                  };
                                                                                          SearchModel.SortDirection = company.Direction == SortDirection.Ascending ? (byte)1 : (byte)0;
                                                                                          //await SessionStorage.SetItemAsync(StorageName, SearchModel);
                                                                                          await Grid.Refresh();
                                                                                      }
                                                                                  });*/

    protected override async Task OnInitializedAsync()
    {
        IEnumerable<Claim> _claims = await General.GetClaimsToken(LocalStorage, SessionStorage);

        if (_claims == null)
        {
            NavManager.NavigateTo($"{NavManager.BaseUri}login", true);
        }
        else
        {
            IEnumerable<Claim> _enumerable = _claims as Claim[] ?? _claims.ToArray();
            User = _enumerable.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value.ToUpperInvariant();
            if (User.NullOrWhiteSpace())
            {
                NavManager.NavigateTo($"{NavManager.BaseUri}login", true);
            }

            HasViewRights = _enumerable.Any(claim => claim.Type == "Permission" && claim.Value == "ViewAllCompanies");
        }

        if (Start.APIHost.NullOrWhiteSpace())
        {
            Start.APIHost = Configuration[NavManager.BaseUri.Contains("localhost") ? "APIHost" : "APIHostServer"];
        }

        _initializationTaskSource = new();
        await ExecuteMethod(() =>
                            {
                                SearchModel = new()
                                              {
                                                  CompanyName = "",
                                                  ItemCount = 25,
                                                  Page = 5,
                                                  SortDirection = 1,
                                                  SortField = 1,
                                                  User = User
                                              };
                                return Task.CompletedTask;
                            });
        _initializationTaskSource.SetResult(true);
        //await Task.Delay(1000);
        await base.OnInitializedAsync();
    }

    private static async Task PageNumberClick(PagerItemClickEventArgs page)
    {
        SearchModel.Page = page.CurrentPage;
        await Grid.Refresh();
    }

    private static async Task PageSizeChanged(PageSizeChangedArgs pageSize)
    {
        SearchModel.ItemCount = pageSize.CurrentPageSize;
        SearchModel.Page = 1;
        await Grid.Refresh();
    }

    /*private Task RowSelected(RowSelectEventArgs<Company> company) => null;*/

    private async Task SaveCompany() => await ExecuteMethod(async () =>
                                                            {
                                                                Dictionary<string, string> _parameters = new()
                                                                                                         {
                                                                                                             {"userID", User}
                                                                                                         };

                                                                await General.PostRest<int>("Company/SaveCompany", _parameters, _companyDetailsClone);
                                                                _companyDetails = _companyDetailsClone.Copy();

                                                                if (_target != null)
                                                                {
                                                                    /* This will work only if the columns are template else this will fail without warning. */
                                                                    _target.CompanyName = _companyDetails.Name;
                                                                    _target.Email = _companyDetails.EmailAddress;
                                                                    _target.Phone = _companyDetails.Phone;
                                                                    _target.Address = SetupTargetAddress();
                                                                    SetupAddress();
                                                                    _target.Website = _companyDetails.Website;
                                                                    _target.Status = _companyDetails.Status;
                                                                    _target.UpdatedBy = _companyDetails.UpdatedBy;
                                                                    _target.UpdatedDate = _companyDetails.UpdatedDate;
                                                                }
                                                                else
                                                                {
                                                                    await Grid.Refresh();
                                                                }

                                                                StateHasChanged();
                                                            });

    private async Task SaveContact() => await ExecuteMethod(async () =>
                                                            {
                                                                Dictionary<string, string> _parameters = new()
                                                                                                         {
                                                                                                             {"user", User}
                                                                                                         };
                                                                List<CompanyContacts> _response =
                                                                    await General.PostRest<List<CompanyContacts>>("Company/SaveCompanyContact", _parameters, SelectedContact);

                                                                _companyContacts = _response;

                                                                if (_target == null)
                                                                {
                                                                    await Grid.Refresh();
                                                                }

                                                                StateHasChanged();
                                                            });

    private async Task SaveLocation() => await ExecuteMethod(async () =>
                                                             {
                                                                 Dictionary<string, string> _parameters = new()
                                                                                                          {
                                                                                                              {"user", User}
                                                                                                          };
                                                                 List<CompanyLocations> _response =
                                                                     await General.PostRest<List<CompanyLocations>>("Company/SaveCompanyLocation", _parameters, SelectedLocation);

                                                                 _companyLocations = _response;

                                                                 if (_target != null)
                                                                 {
                                                                     /* This will work only if the columns are template else this will fail without warning. */
                                                                     _target.CompanyName = _companyDetails.Name;
                                                                     _target.Email = _companyDetails.EmailAddress;
                                                                     _target.Phone = _companyDetails.Phone;
                                                                     _target.Address = SetupTargetAddress(true);
                                                                     SetupAddress(true);
                                                                     _target.Website = _companyDetails.Website;
                                                                     _target.Status = _companyDetails.Status;
                                                                     _target.UpdatedBy = _companyDetails.UpdatedBy;
                                                                     _target.UpdatedDate = _companyDetails.UpdatedDate;
                                                                 }
                                                                 else
                                                                 {
                                                                     await Grid.Refresh();
                                                                 }

                                                                 //await Grid.Refresh();
                                                                 StateHasChanged();
                                                             });

    private void SetupAddress(bool useLocation = false)
    {
        //NumberOfLines = 1;
        string _generateAddress = "";

        if (!useLocation)
        {
            _generateAddress = _companyDetails.StreetName;

            if (_generateAddress == "")
            {
                _generateAddress = _companyDetails.City;
            }
            else
            {
                _generateAddress += _companyDetails.City == "" ? "" : $"<br/>{_companyDetails.City}";
            }

            if (_companyDetails.StateID > 0)
            {
                IntValues _state = State.FirstOrDefault(state => state.Value == _companyDetails.StateID);

                if (_state != null)
                {
                    if (_generateAddress == "")
                    {
                        _generateAddress = $"<strong>{_state.Text.Trim()[7..]}</strong>";
                    }
                    else
                    {
                        try //Because sometimes the default values are not getting set. It's so random that it can't be debugged. And it never fails during debugging session.
                        {
                            _generateAddress += $", <strong>{_state.Text.Trim()[7..]}</strong>";
                        }
                        catch
                        {
                            //
                        }
                    }
                }
            }

            if (_companyDetails.ZipCode != "")
            {
                if (_generateAddress == "")
                {
                    _generateAddress = _companyDetails.ZipCode;
                }
                else
                {
                    _generateAddress += ", " + _companyDetails.ZipCode;
                }
            }
        }
        else
        {
            CompanyLocations _loc = _companyLocations.FirstOrDefault(location => location.PrimaryLocation);
            if (_loc != null)
            {
                _generateAddress = _loc.StreetName;

                if (_generateAddress == "")
                {
                    _generateAddress = _loc.City;
                }
                else
                {
                    _generateAddress += _loc.City == "" ? "" : $"<br/>{_loc.City}";
                }

                if (_companyDetails.StateID > 0)
                {
                    IntValues _state = State.FirstOrDefault(state => state.Value == _loc.StateID);

                    if (_state != null)
                    {
                        if (_generateAddress == "")
                        {
                            _generateAddress = $"<strong>{_state.Text.Trim()[7..]}</strong>";
                        }
                        else
                        {
                            try //Because sometimes the default values are not getting set. It's so random that it can't be debugged. And it never fails during debugging session.
                            {
                                _generateAddress += $", <strong>{_state.Text.Trim()[7..]}</strong>";
                            }
                            catch
                            {
                                //
                            }
                        }
                    }
                }

                if (_loc.ZipCode != "")
                {
                    if (_generateAddress == "")
                    {
                        _generateAddress = _loc.ZipCode;
                    }
                    else
                    {
                        _generateAddress += ", " + _loc.ZipCode;
                    }
                }
            }
        }

        if (_generateAddress != null && _generateAddress.StartsWith(","))
        {
            _generateAddress = _generateAddress[1..].Trim();
        }

        Address = _generateAddress.ToMarkupString();
    }

    private string SetupTargetAddress(bool useLocation = false)
    {
        string _address = "";
        if (!useLocation)
        {
            _address = _companyDetails.City;

            if (_companyDetails.StateID > 0)
            {
                IntValues _state = State.FirstOrDefault(state => state.Value == _companyDetails.StateID);

                if (_state != null)
                {
                    if (_address == "")
                    {
                        _address = $"{_state.Text.Trim().Substring(1, 2)}";
                    }
                    else
                    {
                        try //Because sometimes the default values are not getting set. It's so random that it can't be debugged. And it never fails during debugging session.
                        {
                            _address += $", {_state.Text.Trim().Substring(1, 2)}";
                        }
                        catch
                        {
                            //
                        }
                    }
                }
            }

            if (_companyDetails.ZipCode != "")
            {
                if (_address == "")
                {
                    _address = _companyDetails.ZipCode;
                }
                else
                {
                    _address += ", " + _companyDetails.ZipCode;
                }
            }
        }
        else
        {
            CompanyLocations _loc = _companyLocations.FirstOrDefault(location => location.PrimaryLocation);
            if (_loc != null)
            {
                _address = _loc.City;

                if (_loc.StateID > 0)
                {
                    IntValues _state = State.FirstOrDefault(state => state.Value == _loc.StateID);

                    if (_state != null)
                    {
                        if (_address == "")
                        {
                            _address = $"{_state.Text.Trim().Substring(1, 2)}";
                        }
                        else
                        {
                            try //Because sometimes the default values are not getting set. It's so random that it can't be debugged. And it never fails during debugging session.
                            {
                                _address += $", {_state.Text.Trim().Substring(1, 2)}";
                            }
                            catch
                            {
                                //
                            }
                        }
                    }
                }

                if (_loc.ZipCode != "")
                {
                    if (_address == "")
                    {
                        _address = _loc.ZipCode;
                    }
                    else
                    {
                        _address += ", " + _loc.ZipCode;
                    }
                }
            }
            else
            {
                _address = "";
            }
        }

        if (_address != null && _address.StartsWith(","))
        {
            _address = _address[1..].Trim();
        }

        return _address;
    }

    private Task SpeedDialItemClicked(SpeedDialItemEventArgs args)
    {
        switch (args.Item.ID)
        {
            case "itemEditCompany":
                _selectedTab = 0;
                return EditCompany();
            case "itemAddLocation":
                _selectedTab = 1;
                return EditLocation(0);
            case "itemAddContact":
                _selectedTab = 2;
                break;
            case "itemAddDocument":
                _selectedTab = 3;
                break;
            case "itemEditAccount":
                _selectedTab = 4;
                break;
        }

        return Task.CompletedTask;
    }

    private void TabSelected(SelectEventArgs args) => _selectedTab = args.SelectedIndex;

	/// <summary>
	///     The CompanyAdaptor class is a custom data adaptor for the Companies page grid view.
	///     It extends the DataAdaptor class and overrides the ReadAsync method to provide a custom data retrieval mechanism.
	///     The ReadAsync method retrieves company data for the grid view. If the CompaniesList is not null and contains data,
	///     the method does not retrieve new data. Otherwise, it calls the GetCompanyReadAdaptor method to retrieve company
	///     data.
	///     If there are any companies in the retrieved data, the first row in the grid view is selected.
	/// </summary>
	public class CompanyAdaptor : DataAdaptor
    {
        private readonly SemaphoreSlim _semaphoreSlim = new(1, 1);

		/// <summary>
		///     Asynchronously reads company data for the grid view on the Companies page.
		///     This method checks if the CompaniesList is not null and contains data, in which case it does not retrieve new data.
		///     If the CompaniesList is null or empty, it calls the GetCompanyReadAdaptor method to retrieve company data.
		///     If there are any companies in the retrieved data, it selects the first row in the grid view.
		/// </summary>
		/// <param name="dm">The DataManagerRequest object that contains the parameters for the data request.</param>
		/// <param name="key">An optional key to identify a specific data item. Default is null.</param>
		/// <returns>
		///     A Task that represents the asynchronous read operation. The value of the TResult parameter contains the
		///     retrieved data.
		/// </returns>
		public override async Task<object> ReadAsync(DataManagerRequest dm, string key = null)
        {
            if (!await _semaphoreSlim.WaitAsync(TimeSpan.Zero))
            {
                return null;
            }

            if (_initializationTaskSource == null)
            {
                return null;
            }

            await _initializationTaskSource.Task;
            try
            {
                List<Company> _dataSource = [];

                object _companyReturn = null;
                try
                {
                    Dictionary<string, object> _restResponse = await General.GetRest<Dictionary<string, object>>("Company/GetGridCompanies", null, SearchModel);

                    if (_restResponse == null)
                    {
                        _companyReturn = dm.RequiresCounts ? new DataResult
                                                             {
                                                                 Result = _dataSource,
                                                                 Count = 0 /*_count*/
                                                             } : _dataSource;
                    }
                    else
                    {
                        if (NAICS is not {Count: not 0} || State is not {Count: not 0} || Roles is not {Count: not 0})
                        {
                            RedisService _service = new(Start.CacheServer, Start.CachePort.ToInt32(), Start.Access, false);
                            List<string> _keys = [CacheObjects.NAICS.ToString(), CacheObjects.States.ToString(), CacheObjects.Roles.ToString()];

                            Dictionary<string, string> _values = await _service.BatchGet(_keys);
                            NAICS = JsonConvert.DeserializeObject<List<IntValues>>(_values["NAICS"] ?? string.Empty);
                            State = JsonConvert.DeserializeObject<List<IntValues>>(_values["States"] ?? string.Empty);
                            Roles = JsonConvert.DeserializeObject<List<IntValues>>(_values["Roles"] ?? string.Empty);
                        }

                        _dataSource = JsonConvert.DeserializeObject<List<Company>>(_restResponse["Companies"].ToString() ?? string.Empty);
                        int _count = _restResponse["Count"].ToInt32();
                        Count = _count;
                        if (_dataSource == null)
                        {
                            _companyReturn = dm.RequiresCounts ? new DataResult
                                                                 {
                                                                     Result = null,
                                                                     Count = 1
                                                                 } : null;
                        }
                        else
                        {
                            _companyReturn = dm.RequiresCounts ? new DataResult
                                                                 {
                                                                     Result = _dataSource,
                                                                     Count = _count /*_count*/
                                                                 } : _dataSource;
                        }
                    }
                }
                catch
                {
                    if (_dataSource == null)
                    {
                        _companyReturn = dm.RequiresCounts ? new DataResult
                                                             {
                                                                 Result = null,
                                                                 Count = 1
                                                             } : null;
                    }
                    else
                    {
                        _dataSource.Add(new());

                        _companyReturn = dm.RequiresCounts ? new DataResult
                                                             {
                                                                 Result = _dataSource,
                                                                 Count = 1
                                                             } : _dataSource;
                    }
                }

                if (Count > 0)
                {
                    await Grid.SelectRowAsync(0);
                }

                return _companyReturn;
            }
            catch
            {
                return null;
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }
    }
}