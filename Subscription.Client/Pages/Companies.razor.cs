#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.Client
// File Name:           Companies.razor.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu
// Created On:          2-7-2024 19:53
// Last Updated On:     3-19-2024 20:27
// *****************************************/

#endregion

#region Using

using Microsoft.AspNetCore.Components.Forms;

using Syncfusion.Blazor.Buttons;

#endregion

namespace Subscription.Client.Pages;

public partial class Companies
{
    private const string StorageName = "CompaniesGrid";
    private static TaskCompletionSource<bool> _initializationTaskSource;
    private List<CompanyContacts> _companyContacts = [];

    private CompanyDetails _companyDetails = new(), _companyDetailsClone = new();
    private List<CompanyDocuments> _companyDocuments = [];
    private List<CompanyLocations> _companyLocations = [];
    private int _selectedTab;
    private readonly SemaphoreSlim _semaphoreMainPage = new(1, 1);

    private Company _target;

    private MarkupString Address
    {
        get;
        set;
    }

    private EditCompany CompanyEditDialog
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

    public EditContext EditCon
    {
        get;
        set;
    }

    private static SfGrid<Company> Grid
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

    private static List<IntValues> NAICS
    {
        get;
        set;
    }

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

    /// <summary>
    ///     Gets or sets a clone of the CompanySearch model. This clone is used to manage the state of the search functionality
    ///     in the Companies page. It holds the search parameters and criteria used to filter and display the company data in
    ///     the grid view.
    /// </summary>
    private CompanySearch SearchModelClone
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
    }

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
                                                                                                                                             {"user", "ADMIN"}
                                                                                                                                         };
                                                                                                Dictionary<string, object> _restResponse =
                                                                                                    await General.GetRest<Dictionary<string, object>>("Company/GetCompanyDetails",
                                                                                                                                                      _parameters);

                                                                                                if (_restResponse != null)
                                                                                                {
                                                                                                    _companyDetails =
                                                                                                        General.DeserializeObject<CompanyDetails>(_restResponse["Company"]?.ToString() ?? string.Empty);
                                                                                                    EditCon = new(_companyDetails);
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

    private Task EditCompany() => ExecuteMethod(async () =>
                                                {
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

                                                    await CompanyEditDialog.ShowDialog();
                                                    //StateHasChanged();
                                                });

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

    private Task OnActionBegin(ActionEventArgs<Company> company) => ExecuteMethod(async () =>
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
                                                                                  });

    protected override async Task OnInitializedAsync()
    {
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
                                                  User = "ADMIN"
                                              };
                                return Task.CompletedTask;
                            });
        _initializationTaskSource.SetResult(true);
        await Task.Delay(1000);
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

    private Task RowSelected(RowSelectEventArgs<Company> company) => null;

    private async Task SaveCompany(EditContext editContext) => await ExecuteMethod(async () =>
                                                                                   {
                                                                                       using RestClient _client = new("http://localhost/subscriptionapi/api/");
                                                                                       RestRequest _request = new("Company/SaveCompany")
                                                                                                              {
                                                                                                                  RequestFormat = DataFormat.Json
                                                                                                              };

                                                                                       _request.AddJsonBody(_companyDetailsClone);
                                                                                       _request.AddQueryParameter("userID", "ADMIN");

                                                                                       await _client.PostAsync<int>(_request);

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

    private void SetupAddress()
    {
        //NumberOfLines = 1;
        string _generateAddress = _companyDetails.StreetName;

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
                    _generateAddress = $"<strong>{_state.Key.Trim()[7..]}</strong>";
                }
                else
                {
                    try //Because sometimes the default values are not getting set. It's so random that it can't be debugged. And it never fails during debugging session.
                    {
                        _generateAddress += $", <strong>{_state.Key.Trim()[7..]}</strong>";
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

        if (_generateAddress != null && _generateAddress.StartsWith(","))
        {
            _generateAddress = _generateAddress[1..].Trim();
        }

        Address = _generateAddress.ToMarkupString();
    }

    private string SetupTargetAddress()
    {
        string _address = _companyDetails.City;

        if (_companyDetails.StateID > 0)
        {
            IntValues _state = State.FirstOrDefault(state => state.Value == _companyDetails.StateID);

            if (_state != null)
            {
                if (_address == "")
                {
                    _address = $"{_state.Key.Trim().Substring(1, 2)}";
                }
                else
                {
                    try //Because sometimes the default values are not getting set. It's so random that it can't be debugged. And it never fails during debugging session.
                    {
                        _address += $", {_state.Key.Trim().Substring(1, 2)}";
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
                break;
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
                        NAICS = JsonConvert.DeserializeObject<List<IntValues>>(_restResponse["NAICS"].ToString() ?? string.Empty);
                        State = JsonConvert.DeserializeObject<List<IntValues>>(_restResponse["States"].ToString() ?? string.Empty);
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