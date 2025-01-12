#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.Server
// File Name:           Requisitions.razor.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu
// Created On:          05-01-2024 15:05
// Last Updated On:     01-11-2025 19:01
// *****************************************/

#endregion

namespace Subscription.Server.Components.Pages;

public partial class Requisitions
{
    private const string StorageName = "RequisitionGrid";
    private static int _currentPage = 1;

    private static TaskCompletionSource<bool> _initializationTaskSource;
    private List<KeyValues> _companies;
    private List<IntValues> _education;
    private List<IntValues> _eligibility;
    private List<IntValues> _experience;
    private List<KeyValues> _jobOptions;
    private Preferences _preference;

    private List<Role> _roles;
    private readonly SemaphoreSlim _semaphoreMainPage = new(1, 1);
    private List<IntValues> _skills;
    private List<IntValues> _states;
    private List<StatusCode> _statusCodes;

    private readonly List<KeyValues> _statusSearch = [];
    private List<AppWorkflow> _workflows;

    /// <summary>
    ///     Gets or sets the AutocompleteValue property of the Requisition.
    /// </summary>
    /// <remarks>
    ///     The AutocompleteValue is used to store the title of the SearchModel during the initialization of the Requisition
    ///     component.
    ///     It is also updated when clearing filters or selecting all alphabet options.
    /// </remarks>
    /// <value>
    ///     The title of the SearchModel.
    /// </value>
    private string AutocompleteValue
    {
        get;
        set;
    }

    /// <summary>
    ///     Gets or sets the list of companies associated with the requisition.
    /// </summary>
    /// <value>
    ///     The list of companies.
    /// </value>
    internal static List<Company> Companies
    {
        get;
        set;
    } = [];

    /// <summary>
    ///     Gets or sets a list of company contacts associated with the requisition.
    /// </summary>
    /// <value>
    ///     The list of company contacts.
    /// </value>
    internal static List<CompanyContacts> CompanyContacts
    {
        get;
        set;
    } = [];

    /// <summary>
    ///     Represents the configuration for the application.
    /// </summary>
    [Inject]
    private IConfiguration Configuration
    {
        get;
        set;
    }

    /// <summary>
    ///     Gets or sets the count of items.
    /// </summary>
    /// <remarks>
    ///     This property is used to store the total number of items in the data source.
    ///     It is updated whenever the data source is refreshed or a new set of items is loaded.
    /// </remarks>
    internal static int Count
    {
        get;
        set;
    }

    /// <summary>
    ///     Gets or sets the end record of the requisition. The end record is calculated as the start record plus the count of
    ///     data source items.
    ///     This property is used to determine the range of records displayed on the current page of the requisition grid.
    /// </summary>
    internal static int EndRecord
    {
        get;
        set;
    }

    private static SfGrid<Requisition> Grid
    {
        get;
        set;
    }

    public bool HasEditRights
    {
        get;
        set;
    }

    public bool HasViewRights
    {
        get;
        set;
    }

    /// <summary>
    ///     Gets or sets the JavaScript runtime instance. This instance is used to invoke JavaScript functions from C# code.
    ///     For example, it is used in the DownloadDocument method to open a new browser tab for document download.
    /// </summary>
    [Inject]
    private IJSRuntime JsRuntime
    {
        get;
        set;
    }

    /// <summary>
    ///     Gets or sets the instance of the ILocalStorageService used in the application.
    /// </summary>
    /// <remarks>
    ///     This service is used to manage local storage in the browser.
    ///     It is used to store and retrieve data such as user preferences and application state.
    /// </remarks>
    [Inject]
    private ILocalStorageService LocalStorage
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

    internal static int PageCount
    {
        get;
        set;
    }

    /// <summary>
    ///     Gets or sets the ID of the requisition. This ID is used to uniquely identify a requisition in the system.
    /// </summary>
    private static int RequisitionID
    {
        get;
        set;
    }

    /// <summary>
    ///     Gets or sets the search model for the requisition. This model is used to store the search parameters for finding
    ///     requisitions.
    /// </summary>
    private static RequisitionSearch SearchModel
    {
        get;
    } = new();

    /// <summary>
    ///     Gets or sets the search model for the requisition. This model is used to store the search parameters for finding
    ///     requisitions.
    /// </summary>
    private static RequisitionSearch SearchModelClone
    {
        get;
        set;
    } = new();

    /// <summary>
    ///     Gets or sets the session storage service used for storing and retrieving session data.
    ///     This service is used to manage session data such as the requisition grid state and the requisition ID from the
    ///     dashboard.
    /// </summary>
    /// <value>
    ///     The session storage service.
    /// </value>
    [Inject]
    private ISessionStorageService SessionStorage
    {
        get;
        set;
    }

    /// <summary>
    ///     Gets or sets the skills associated with the requisition.
    ///     This is a list of <see cref="IntValues" /> where each value represents a unique skill.
    /// </summary>
    internal static List<IntValues> Skills
    {
        get;
        set;
    }

    /// <summary>
    ///     Gets or sets the sort direction for the requisition grid. This property is used to determine the order in which
    ///     requisitions are displayed in the grid.
    ///     The sort direction can be either ascending or descending.
    /// </summary>
    private SortDirection SortDirectionProperty
    {
        get;
        set;
    }

    /// <summary>
    ///     Gets or sets the field by which the requisition data is sorted.
    /// </summary>
    /// <value>
    ///     The field name used for sorting. Possible values include "Code", "Title", "Company", "Option", "Status", "DueEnd",
    ///     and "Updated".
    /// </value>
    private string SortField
    {
        get;
        set;
    }

    private SfSpinner SpinnerTop
    {
        get;
        set;
    }

    internal static int StartRecord
    {
        get;
        set;
    }

    /// <summary>
    ///     Gets or sets the list of status values associated with the requisition.
    ///     This list is populated with the 'StatusCount' data from the API response.
    ///     Each status is represented by a 'KeyValues' object.
    /// </summary>
    internal static List<KeyValues> StatusList
    {
        get;
        set;
    } = [];

    private static string User
    {
        get;
        set;
    }

    /// <summary>
    ///     Initiates the advanced search process for requisitions. This method is invoked when the advanced search option is
    ///     selected.
    ///     It creates a copy of the current search model for backup purposes and then opens the advanced search dialog.
    /// </summary>
    /// <param name="args">The mouse event arguments associated with the advanced search invocation.</param>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    private Task AdvancedSearch(MouseEventArgs args) => ExecuteMethod(async () =>
                                                                      {
                                                                          SearchModelClone = SearchModel.Copy();
                                                                          await Task.CompletedTask;
                                                                          //await DialogSearch.ShowDialog();
                                                                      });

    /// <summary>
    ///     Handles the click event for the "All Alphabet" button in the requisition grid.
    /// </summary>
    /// <remarks>
    ///     This method resets the search model's title and page properties, clears the autocomplete value, and refreshes the
    ///     grid.
    ///     It also ensures that the method's actions are not performed if a previous action is still in progress.
    /// </remarks>
    /// <param name="args">The <see cref="MouseEventArgs" /> instance containing the event data.</param>
    private Task AllAlphabets(MouseEventArgs args) => ExecuteMethod(async () =>
                                                                    {
                                                                        SearchModel.Title = "";
                                                                        _currentPage = 1;
                                                                        SearchModel.Page = 1;
                                                                        await SessionStorage.SetItemAsync(StorageName, SearchModel);
                                                                        AutocompleteValue = "";
                                                                        await Grid.Refresh();
                                                                    });

    private static async Task AutocompleteValueChange(ChangeEventArgs<string, KeyValues> filter)
    {
        SearchModel.Title = filter.Value;
        SearchModel.Page = 1;
        await Grid.Refresh();
    }

    /// <summary>
    ///     Asynchronously changes the item count of the requisition.
    /// </summary>
    /// <param name="item">The change event arguments containing the new item count.</param>
    /// <remarks>
    ///     This method updates the item count of the requisition and refreshes the grid.
    ///     It also saves the updated search model to the session storage.
    ///     This method is not executed if an action is already in progress.
    /// </remarks>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    private Task ChangeItemCount(ChangeEventArgs<int, IntValues> item) => ExecuteMethod(async () =>
                                                                                        {
                                                                                            SearchModel.Page = 1;
                                                                                            SearchModel.ItemCount = item.Value;

                                                                                            await SessionStorage.SetItemAsync(StorageName, SearchModel);
                                                                                            await Grid.Refresh();
                                                                                            StateHasChanged();
                                                                                        });

    /// <summary>
    ///     Clears the filter applied to the requisitions.
    /// </summary>
    /// <remarks>
    ///     This function is called when the "Clear Filter" button is clicked.
    ///     It resets the filter values and reloads the requisitions.
    /// </remarks>
    private async Task ClearFilter()
    {
        SearchModel.Clear();
        SearchModel.User = User;
        await Grid.Refresh();
    }

    /// <summary>
    ///     Handles the data processing for the requisition. This method is responsible for creating a reference to the current
    ///     instance of the Requisition class,
    ///     invoking a JavaScript function to manage detail rows, and managing the selection and expansion of rows in the
    ///     requisition grid based on the RequisitionID.
    ///     If the total item count in the grid is greater than zero, it checks if the RequisitionID is greater than zero. If
    ///     so, it selects and expands the corresponding row.
    ///     If the RequisitionID is not greater than zero, it selects the first row. After the operations, it resets the
    ///     RequisitionID to zero.
    /// </summary>
    /// <param name="obj">The object that triggers the data handling.</param>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    private Task DataHandler(object obj) => ExecuteMethod(async () =>
                                                          {
                                                              DotNetObjectReference<Requisitions> _dotNetReference = DotNetObjectReference.Create(this); // create dotnet ref
                                                              await JsRuntime.InvokeAsync<string>("detail", _dotNetReference);
                                                              //  send the dotnet ref to JS side
                                                              if (Grid.TotalItemCount > 0)
                                                              {
                                                                  if (RequisitionID > 0)
                                                                  {
                                                                      int _index = await Grid.GetRowIndexByPrimaryKeyAsync(RequisitionID);
                                                                      if (_index != Grid.SelectedRowIndex)
                                                                      {
                                                                          await Grid.SelectRowAsync(_index);
                                                                          foreach (Requisition _requisition in Grid.CurrentViewData.OfType<Requisition>()
                                                                                                                   .Where(requisitions => requisitions.ID == RequisitionID))
                                                                          {
                                                                              await Grid.ExpandCollapseDetailRowAsync(_requisition);
                                                                              break;
                                                                          }
                                                                      }

                                                                      await SessionStorage.SetItemAsync(StorageName, SearchModel);
                                                                      await SessionStorage.RemoveItemAsync("RequisitionIDFromDashboard");
                                                                  }
                                                                  else
                                                                  {
                                                                      await Grid.SelectRowAsync(0);
                                                                  }
                                                              }

                                                              RequisitionID = 0;
                                                          });

    private Task DetailDataBind(DetailDataBoundEventArgs<Requisition> arg) => Task.CompletedTask;

    /// <summary>
    ///     Executes the provided task within a semaphore lock. If the semaphore is currently locked, the method will return
    ///     immediately.
    ///     If an exception occurs during the execution of the task, it will be logged using the provided logger.
    /// </summary>
    /// <param name="task">
    ///     The task to be executed.
    /// </param>
    /// <returns>
    ///     A task that represents the asynchronous operation.
    /// </returns>
    private Task ExecuteMethod(Func<Task> task) => General.ExecuteMethod(_semaphoreMainPage, task);

    private async Task GetAlphabets(char alphabet)
    {
        await ExecuteMethod(async () =>
                            {
                                SearchModel.Title = alphabet.ToString();
                                SearchModel.Page = 1;
                                await SessionStorage.SetItemAsync(StorageName, SearchModel);
                                await Grid.Refresh();
                            });
    }

    private Task GridPageChanging(GridPageChangingEventArgs page) => ExecuteMethod(async () =>
                                                                                   {

                                                                                       if (page.CurrentPageSize != SearchModel.ItemCount)
                                                                                       {
                                                                                           SearchModel.ItemCount = page.CurrentPageSize;
                                                                                           SearchModel.Page = 1;
                                                                                           await Grid.GoToPageAsync(1);
                                                                                       }
                                                                                       else
                                                                                       {
                                                                                           SearchModel.Page = page.CurrentPage;
                                                                                           await Grid.Refresh();
                                                                                       }

                                                                                       await SessionStorage.SetItemAsync(StorageName, SearchModel);

                                                                                   });

    /// <summary>
    ///     Asynchronously initializes the Requisition component.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         This method is invoked when the component is first initialized. It performs several operations:
    ///     </para>
    ///     <para>
    ///         - Checks if all required objects are not null, otherwise throws an ArgumentNullException.
    ///     </para>
    ///     <para>
    ///         - Retrieves and sets the LoginCookyUser from the NavigationManager.
    ///     </para>
    ///     <para>
    ///         - Retrieves the RequisitionGrid and RequisitionIDFromDashboard from the SessionStorage.
    ///     </para>
    ///     <para>
    ///         - If the RequisitionID is not set, it deserializes the RequisitionSearch from the RequisitionGrid.
    ///     </para>
    ///     <para>
    ///         - If the RequisitionID is set, it initializes a new RequisitionSearch with default values.
    ///     </para>
    ///     <para>
    ///         - Retrieves various data from the MemoryCache, such as States, Eligibility, Education, Experience, JobOptions,
    ///         Recruiters, Skills, StatusCodes, Preferences, Companies,
    ///         and Workflow.
    ///     </para>
    ///     <para>
    ///         - Sets the SortDirectionProperty and SortField based on the SearchModel.
    ///     </para>
    ///     <para>
    ///         - Sets the AutocompleteValue to the Title of the SearchModel.
    ///     </para>
    ///     <para>
    ///         - Marks the component as loaded and refreshes the Grid.
    ///     </para>
    /// </remarks>
    /// <returns>A Task that represents the asynchronous operation.</returns>
    protected override async Task OnInitializedAsync()
    {
        _initializationTaskSource = new();
        await ExecuteMethod(async () =>
                            {
                                // Get user claims
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

                                    // Set user permissions
                                    HasViewRights = _enumerable.Any(claim => claim.Type == "Permission" && claim.Value == "ViewRequisitions");
                                    HasEditRights = _enumerable.Any(claim => claim.Type == "Permission" && claim.Value == "CreateOrEditRequisitions");
                                }

                                if (Start.APIHost.NullOrWhiteSpace())
                                {
                                    Start.APIHost = Configuration[NavManager.BaseUri.Contains("localhost") ? "APIHost" : "APIHostServer"];
                                }

                                //General.CheckStart(NavManager, Configuration);
                                //LoginCookyUser = await NavManager.RedirectInner(LocalStorage, Crypto);

                                List<string> _keys =
                                [
                                    CacheObjects.Roles.ToString(), CacheObjects.States.ToString(), CacheObjects.Eligibility.ToString(), CacheObjects.Education.ToString(),
                                    CacheObjects.Experience.ToString(), CacheObjects.JobOptions.ToString(), CacheObjects.Users.ToString(), CacheObjects.Skills.ToString(),
                                    CacheObjects.StatusCodes.ToString(), CacheObjects.Preferences.ToString(), CacheObjects.Companies.ToString(), CacheObjects.Workflow.ToString()
                                ];

                                Dictionary<string, string> _parameters = new()
                                                                         {
                                                                             {"value", JsonConvert.SerializeObject(_keys)}
                                                                         };
                                Dictionary<string, object> _cacheValues = await General.PostRest("Redis/BatchGet", _parameters);
                                _roles = General.DeserializeObject<List<Role>>(_cacheValues[CacheObjects.Roles.ToString()]); //await Redis.GetAsync<List<Role>>("Roles");

                                while (_states == null)
                                {
                                    _states = General.DeserializeObject<List<IntValues>>(_cacheValues[CacheObjects.States.ToString()]);
                                }

                                while (_eligibility == null)
                                {
                                    _eligibility = General.DeserializeObject<List<IntValues>>(_cacheValues[CacheObjects.Eligibility.ToString()]);
                                }

                                while (_education == null)
                                {
                                    _education = General.DeserializeObject<List<IntValues>>(_cacheValues[CacheObjects.Education.ToString()]);
                                }

                                while (_experience == null)
                                {
                                    _experience = General.DeserializeObject<List<IntValues>>(_cacheValues[CacheObjects.Experience.ToString()]);
                                }

                                while (_jobOptions == null)
                                {
                                    _jobOptions = General.DeserializeObject<List<KeyValues>>(_cacheValues[CacheObjects.JobOptions.ToString()]);
                                }

                                /*while (_recruiters == null)
                                {
                                    List<UserList> _users = General.DeserializeObject<List<UserList>>(_cacheValues[CacheObjects.Users.ToString()]);
                                    if (_users == null)
                                    {
                                        continue;
                                    }

                                    _recruiters = [];
                                    foreach (User _user in _users.Where(user => user.Role is "Recruiter" or "Recruiter & Sales Manager"))
                                    {
                                        _recruiters?.Add(new(_user.UserName, _user.UserName));
                                    }
                                }*/

                                while (_skills == null)
                                {
                                    _skills = General.DeserializeObject<List<IntValues>>(_cacheValues[CacheObjects.Skills.ToString()]);
                                }

                                _statusCodes = General.DeserializeObject<List<StatusCode>>(_cacheValues[CacheObjects.StatusCodes.ToString()]);
                                _preference = General.DeserializeObject<Preferences>(_cacheValues[CacheObjects.Preferences.ToString()]);

                                if (_statusCodes is {Count: > 0})
                                {
                                    foreach (StatusCode _statusCode in _statusCodes.Where(statusCode => statusCode.AppliesToCode == "REQ"))
                                    {
                                        _statusSearch.Add(new()
                                                          {
                                                              Key = _statusCode.Status,
                                                              Value = _statusCode.Code
                                                          });
                                    }
                                }

                                List<Company> _companyList = General.DeserializeObject<List<Company>>(_cacheValues[CacheObjects.Companies.ToString()]);
                                _companies.Add(new()
                                               {
                                                   Key = "All Companies",
                                                   Value = "%"
                                               });
                                if (_companyList != null)
                                {
                                    foreach (Company _company in _companyList.Where(company => company.UpdatedBy == User || company.UpdatedBy == "ADMIN"))
                                    {
                                        _companies.Add(new()
                                                       {
                                                           Key = _company.CompanyName,
                                                           Value = _company.CompanyName
                                                       });
                                    }
                                }

                                _workflows = General.DeserializeObject<List<AppWorkflow>>(_cacheValues[CacheObjects.Workflow.ToString()]);

                                SortDirectionProperty = SearchModel.SortDirection == 1 ? SortDirection.Ascending : SortDirection.Descending;
                                SortField = SearchModel.SortField switch
                                            {
                                                2 => "Code",
                                                3 => "Title",
                                                4 => "Company",
                                                5 => "Option",
                                                6 => "Status",
                                                8 => "DueEnd",
                                                _ => "Updated"
                                            };
                                AutocompleteValue = SearchModel.Title;

                                await Grid.Refresh();
                            });

        _initializationTaskSource.SetResult(true);
        await base.OnInitializedAsync();
    }

    /// <summary>
    ///     The RequisitionAdaptor class is a custom data adaptor for the Requisitions page.
    ///     It inherits from the DataAdaptor class and overrides the ReadAsync method.
    /// </summary>
    /// <remarks>
    ///     The ReadAsync method is used to asynchronously read data for the Requisitions page.
    ///     It checks if a read operation is already in progress, and if not, it initiates a new read operation.
    ///     The method retrieves lead data using the General.GetRequisitionReadAdaptor method and the provided
    ///     DataManagerRequest.
    ///     If there are any requisitions, it selects the first one. The method returns the retrieved requisitions data.
    /// </remarks>
    internal class RequisitionAdaptor : DataAdaptor
    {
        private readonly SemaphoreSlim _semaphoreSlim = new(1, 1);

        /// <summary>
        ///     Asynchronously reads data from the Requisition data source.
        /// </summary>
        /// <param name="dm">The DataManagerRequest object that contains the parameters for the data request.</param>
        /// <param name="key">An optional key to identify a specific data record.</param>
        /// <remarks>
        ///     This method checks if the data is already being read or if it has not been loaded yet. If either of these
        ///     conditions is true, it returns null.
        ///     Otherwise, it sets the _reading flag to true and proceeds with the data read operation. It also checks if the
        ///     Companies list is not null and has more than 0 items.
        ///     If so, it sets the _getInformation flag to true. Then it calls the GetRequisitionReadAdaptor method from the
        ///     General class, passing the SearchModel, dm, _getInformation, RequisitionID, and true as parameters.
        ///     The result of this method call is stored in the _requisitionReturn object. After the data read operation, it sets
        ///     the _currentPage to the Page property of the SearchModel and the _reading flag back to false.
        /// </remarks>
        /// <returns>
        ///     A Task that represents the asynchronous operation. The task result contains the data retrieved from the data
        ///     source.
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
                bool _getInformation = true;
                if (Companies != null)
                {
                    _getInformation = Companies.Count == 0;
                }

                object _requisitionReturn = await General.GetRequisitionReadAdaptor(SearchModel, dm, _getInformation, RequisitionID, true, User);

                _currentPage = SearchModel.Page;

                return _requisitionReturn;
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