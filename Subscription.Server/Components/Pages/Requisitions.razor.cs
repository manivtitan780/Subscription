#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.Server
// File Name:           Requisitions.razor.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu, Brijesh Dubey
// Created On:          02-06-2025 19:02
// Last Updated On:     04-11-2025 19:04
// *****************************************/

#endregion

namespace Subscription.Server.Components.Pages;

public partial class Requisitions
{
    private const string StorageName = "RequisitionGrid";

    // private static TaskCompletionSource<bool> _initializationTaskSource;

    private List<CandidateActivity> _candActivityObject = [];
    private List<IntValues> _education = [], _eligibility = [], _experience = [], _states = [];
    private List<KeyValues> /*_companies = [], */ _jobOptions = [];

    private Preferences _preference;
    private List<KeyValues> _recruiters;
    private MarkupString _reqDetailSkills = string.Empty.ToMarkupString();
    private RequisitionDetails _reqDetailsObject = new(), _reqDetailsObjectClone = new();
    private List<RequisitionDocuments> _reqDocumentsObject = [];

    //private List<Role> _roles;
    private int _selectedTab;
    private readonly SemaphoreSlim _semaphoreMainPage = new(1, 1);

    private List<StatusCode> _statusCodes;
    //private readonly List<KeyValues> _statusSearch = [];

    private Requisition _target;

    private readonly List<Workflow> _workflow = [];
    private List<Workflow> _workflows;

    private ActivityPanelRequisition ActivityPanel { get; set; }

    /*
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
    */

    /// <summary>
    ///     Gets or sets the list of companies associated with the requisition.
    /// </summary>
    /// <value>
    ///     The list of companies.
    /// </value>
    private List<Company> Companies { get; set; } = [];

    /// <summary>
    ///     Gets or sets a list of company contacts associated with the requisition.
    /// </summary>
    /// <value>
    ///     The list of company contacts.
    /// </value>
    private List<CompanyContacts> CompanyContacts { get; } = [];

    /// <summary>
    ///     Represents the configuration for the application.
    /// </summary>
    [Inject]
    private IConfiguration Configuration { get; set; }

    private int Count { get; set; }

    private List<Requisition> DataSource { get; set; }

    private RequisitionDetailsPanel DetailsRequisition { get; set; }

    private EditActivityDialog DialogActivity { get; set; }

    private AddRequisitionDocument DialogDocument { get; set; }

    private DocumentsPanel DocumentsPanel { get; set; }
    /*

    [Inject]
    private Container ContainerObject
    {
        get;
        set;
    }
    */

    private static SfGrid<Requisition> Grid { get; set; }

    private bool HasEditRights { get; set; }

    /*
    private bool HasRendered
    {
        get;
        set;
    }

    */
    public bool HasViewRights { get; set; }

    /// <summary>
    ///     Gets or sets the JavaScript runtime instance. This instance is used to invoke JavaScript functions from C# code.
    ///     For example, it is used in the DownloadDocument method to open a new browser tab for document download.
    /// </summary>
    [Inject]
    private IJSRuntime JsRuntime { get; set; }

    /// <summary>
    ///     Gets or sets the instance of the ILocalStorageService used in the application.
    /// </summary>
    /// <remarks>
    ///     This service is used to manage local storage in the browser.
    ///     It is used to store and retrieve data such as user preferences and application state.
    /// </remarks>
    [Inject]
    private ILocalStorageService LocalStorage { get; set; }

    /// <summary>
    ///     Gets or sets the instance of the NavigationManager service used in the Companies page.
    ///     This service provides methods and properties to manage and interact with the URI of the application.
    ///     It is used for tasks such as navigating to different pages and constructing URIs for use within the application.
    ///     For example, it is used in the `DownloadDocument` method to construct a URI for downloading a document.
    /// </summary>
    [Inject]
    private NavigationManager NavManager { get; set; }

    /// <summary>
    ///     Gets or sets a new document for the requisition. This property is used to store the details of a new document
    ///     that is being added to the requisition. If the new document is null, a new instance of RequisitionDocuments is
    ///     created.
    ///     If the new document already exists, its data is cleared before adding new data.
    /// </summary>
    private RequisitionDocuments NewDocument { get; set; } = new();

    /// <summary>
    ///     Gets or sets a list of KeyValues instances representing the next steps for the candidate.
    /// </summary>
    private List<KeyValues> NextSteps { get; } = [];

    private int Page { get; set; } = 1;

    /// <summary>
    ///     Gets or sets the ID of the requisition. This ID is used to uniquely identify a requisition in the system.
    /// </summary>
    private int RequisitionID { get; set; }

    private int RoleID { get; } = 5;

    private string RoleName { get; set; }

    /// <summary>
    ///     Gets or sets the search model for the requisition. This model is used to store the search parameters for finding
    ///     requisitions.
    /// </summary>
    private RequisitionSearch SearchModel { get; set; } = new();

    /// <summary>
    ///     Gets or sets the search model for the requisition. This model is used to store the search parameters for finding
    ///     requisitions.
    /// </summary>
    private RequisitionSearch SearchModelClone { get; set; } = new();

    /// <summary>
    ///     Gets or sets the selected activity for the candidate.
    /// </summary>
    /// <value>
    ///     The selected activity is of type <see cref="CandidateActivity" /> and it represents the current
    ///     activity selected for the candidate.
    /// </value>
    private CandidateActivity SelectedActivity { get; set; } = new();

    /// <summary>
    ///     Gets or sets the selected document for download from the requisition's documents panel.
    ///     This property is used in the DownloadDocument method to generate a query string for downloading the selected
    ///     document.
    /// </summary>
    private RequisitionDocuments SelectedDownload { get; set; } = new();

    /// <summary>
    ///     Gets or sets the session storage service used for storing and retrieving session data.
    ///     This service is used to manage session data such as the requisition grid state and the requisition ID from the
    ///     dashboard.
    /// </summary>
    /// <value>
    ///     The session storage service.
    /// </value>
    [Inject]
    private ISessionStorageService SessionStorage { get; set; }

    /// <summary>
    ///     Gets or sets the skills associated with the requisition.
    ///     This is a list of <see cref="IntValues" /> where each value represents a unique skill.
    /// </summary>
    private List<IntValues> Skills { get; set; } = [];

    /*
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
    */

    private SfSpinner Spinner { get; set; } = new();

    private SfSpinner SpinnerTop { get; set; }

    /// <summary>
    ///     Gets or sets the list of status values associated with the requisition.
    ///     This list is populated with the 'StatusCount' data from the API response.
    ///     Each status is represented by a 'KeyValues' object.
    /// </summary>
    private List<KeyValues> StatusList { get; set; } = [];

    /// <summary>
    ///     Gets or sets the title of the requisition. The title is used to distinguish between "Add" and "Edit" modes in the
    ///     requisition form.
    ///     When a new requisition is being added, the title is set to "Add". When an existing requisition is being edited, the
    ///     title is set to "Edit".
    /// </summary>
    private static string Title { get; set; } = "Edit";

    private static string User { get; set; }

    private bool VisibleSpinner { get; set; }

    /// <summary>
    ///     Asynchronously adds a new document to the requisition.
    ///     This method first checks if a new document instance exists. If not, it creates a new instance.
    ///     If an instance already exists, it clears the existing data.
    ///     After preparing the new document, it opens the dialog for adding a new requisition document.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    private Task AddDocument() => ExecuteMethod(async () =>
                                                {
                                                    if (NewDocument == null)
                                                    {
                                                        NewDocument = new();
                                                    }
                                                    else
                                                    {
                                                        NewDocument.Clear();
                                                    }

                                                    await DialogDocument.ShowDialog();
                                                });

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
                                                                        SearchModel.Title = string.Empty;
                                                                        SearchModel.Page = 1;
                                                                        await Task.WhenAll(SaveStorage(), SetDataSource()).ConfigureAwait(false);
                                                                        //await Grid.Refresh();
                                                                    });

    private Task AutocompleteValueChange(ChangeEventArgs<string, KeyValues> filter) => ExecuteMethod(async () =>
                                                                                                     {
                                                                                                         SearchModel.Title = filter.Value;
                                                                                                         SearchModel.Page = 1;
                                                                                                         await Task.WhenAll(SaveStorage(), SetDataSource()).ConfigureAwait(false);
                                                                                                         //await Grid.Refresh();
                                                                                                     });

    /// <summary>
    ///     Clears the filter applied to the requisitions.
    /// </summary>
    /// <remarks>
    ///     This function is called when the "Clear Filter" button is clicked.
    ///     It resets the filter values and reloads the requisitions.
    /// </remarks>
    private Task ClearFilter() => ExecuteMethod(async () =>
                                                {
                                                    SearchModel.Clear();
                                                    SearchModel.User = User;
                                                    await Task.WhenAll(SaveStorage(), SetDataSource()).ConfigureAwait(false);
                                                    //await Grid.Refresh();
                                                });

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
                                                              // Grid.TotalItemCount = Count;
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

                                                                      await SaveStorage(false); // SessionStorage.SetItemAsync(StorageName, SearchModel);
                                                                      await SessionStorage.RemoveItemAsync("RequisitionIDFromDashboard");
                                                                  }
                                                                  else
                                                                  {
                                                                      await Grid.SelectRowAsync(0);
                                                                  }

                                                                  if (StatusList.Count == 0)
                                                                  {
                                                                      byte[] _value = await SessionStorage.GetItemAsync<byte[]>("StatusList");
                                                                      if (_value != null)
                                                                      {
                                                                          StatusList = General.DeserializeObject<List<KeyValues>>(_value.DecompressGZip());
                                                                      }
                                                                  }
                                                              }

                                                              RequisitionID = 0;
                                                          });

    /// <summary>
    ///     Asynchronously deletes a document associated with a requisition.
    /// </summary>
    /// <param name="args">The ID of the document to be deleted.</param>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    /// <remarks>
    ///     This method sends a POST request to the 'Requisition/DeleteRequisitionDocument' endpoint of the API.
    ///     The document ID and user ID are passed as query parameters in the request.
    ///     If the deletion is successful, the list of requisition documents is updated.
    ///     This method is safe to call concurrently. If the method is already in progress, subsequent calls will return
    ///     immediately.
    /// </remarks>
    private Task DeleteDocument(int args) => ExecuteMethod(async () =>
                                                           {
                                                               Dictionary<string, string> _parameters = new()
                                                                                                        {
                                                                                                            {"documentID", args.ToString()},
                                                                                                            {"user", User}
                                                                                                        };

                                                               Dictionary<string, object> _response = await General.PostRest("Requisition/DeleteRequisitionDocument", _parameters);
                                                               if (_response == null)
                                                               {
                                                                   return;
                                                               }

                                                               _reqDocumentsObject = General.DeserializeObject<List<RequisitionDocuments>>(_response["Document"]);
                                                           });

    private Task DetailDataBind(DetailDataBoundEventArgs<Requisition> requisition)
    {
        return ExecuteMethod(async () =>
                             {
                                 if (_target != null && _target != requisition.Data)
                                 {
                                     // return when target is equal to args.data
                                     await Grid.ExpandCollapseDetailRowAsync(_target);
                                 }

                                 int _index = await Grid.GetRowIndexByPrimaryKeyAsync(requisition.Data.ID);
                                 if (_index != Grid.SelectedRowIndex)
                                 {
                                     await Grid.SelectRowAsync(_index);
                                 }

                                 _target = requisition.Data;

                                 VisibleSpinner = true;

                                 Dictionary<string, string> _parameters = new()
                                                                          {
                                                                              {"requisitionID", _target.ID.ToString()}
                                                                          };

                                 (string _requisition, string _activity, string _documents) = await General.ExecuteRest<ReturnRequisitionDetails>("Requisition/GetRequisitionDetails", _parameters,
                                                                                                                                                  null, false);

                                 _reqDetailsObject = General.DeserializeObject<RequisitionDetails>(_requisition);
                                 _candActivityObject = General.DeserializeObject<List<CandidateActivity>>(_activity) ?? [];
                                 _reqDocumentsObject = General.DeserializeObject<List<RequisitionDocuments>>(_documents) ?? [];
                                 SetSkills();

                                 _selectedTab = _candActivityObject.Count > 0 ? 2 : 0;

                                 await Task.Delay(100);
                                 VisibleSpinner = false;
                             });
    }

    /// <summary>
    ///     Collapses the detail row in the Companies page grid view. This method is invoked from JavaScript.
    /// </summary>
    [JSInvokable("DetailCollapse")]
    public void DetailRowCollapse() => _target = null;

    /// <summary>
    ///     Initiates the download of a document associated with a requisition.
    /// </summary>
    /// <param name="args">The identifier of the document to be downloaded.</param>
    /// <remarks>
    ///     This method is asynchronous and may not complete immediately. It first checks if a download action is already in
    ///     progress,
    ///     and if not, it sets the selected download to the document corresponding to the provided identifier.
    ///     It then constructs a query string and invokes a JavaScript function to open the download link in a new browser tab.
    ///     After the download is initiated, the method resets the action progress indicator.
    /// </remarks>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    private Task DownloadDocument(int args) => ExecuteMethod(async () =>
                                                             {
                                                                 SelectedDownload = DocumentsPanel.SelectedRow;
                                                                 string _queryString = $"{SelectedDownload.Location}^{_target.ID}^{SelectedDownload.InternalFileName}^1".ToBase64String();
                                                                 await JsRuntime.InvokeVoidAsync("open", $"{NavManager.BaseUri}Download/{_queryString}", "_blank");
                                                             });

    /// <summary>
    ///     Initiates the editing process for an activity associated with a requisition.
    ///     This method is asynchronous and can be awaited. It first checks if an edit is already in progress,
    ///     if not, it sets the selected activity for editing, clears the next steps, and populates it with the possible
    ///     next steps based on the current status of the activity. It then shows the dialog for editing the activity.
    /// </summary>
    /// <param name="args">
    ///     The identifier of the activity to be edited.
    /// </param>
    /// <returns>
    ///     A <see cref="Task" /> representing the asynchronous operation.
    /// </returns>
    private Task EditActivity(int args) => ExecuteMethod(async () =>
                                                         {
                                                             SelectedActivity = ActivityPanel.SelectedRow;
                                                             NextSteps.Clear();
                                                             NextSteps.Add(new("No Change", ""));
                                                             try
                                                             {
                                                                 foreach (string[] _next in _workflows.Where(flow => flow.Step == SelectedActivity.StatusCode)
                                                                                                      .Select(flow => flow.Next.Split(',')))
                                                                 {
                                                                     foreach (string _nextString in _next)
                                                                     {
                                                                         foreach (StatusCode _status in _statusCodes.Where(status => status.Code == _nextString && status.AppliesToCode == "SCN"))
                                                                         {
                                                                             NextSteps.Add(new(_status.Status, _nextString));
                                                                             break;
                                                                         }
                                                                     }

                                                                     break;
                                                                 }
                                                             }
                                                             catch
                                                             {
                                                                 //
                                                             }

                                                             await DialogActivity.ShowDialog();
                                                         });

    /// <summary>
    ///     Initiates the process of editing a requisition. This method is asynchronous.
    /// </summary>
    /// <param name="isAdd">
    ///     A boolean value that determines whether a new requisition is being added or an existing one is being edited.
    ///     If true, a new requisition is being added. If false, an existing requisition is being edited.
    /// </param>
    /// <remarks>
    ///     This method performs several actions:
    ///     - It shows a spinner to indicate that a process is running.
    ///     - It sets the title of the dialog box based on whether a new requisition is being added or an existing one is being
    ///     edited.
    ///     - It creates a new requisition or clears the data of an existing one based on the value of the isAdd parameter.
    ///     - It triggers a state change in the component.
    ///     - It shows the dialog box for editing the requisition.
    ///     - It hides the spinner once the process is complete.
    /// </remarks>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    private Task EditRequisition(bool isAdd) => ExecuteMethod(async () =>
                                                              {
                                                                  VisibleSpinner = true;

                                                                  if (isAdd)
                                                                  {
                                                                      Title = "Add";
                                                                      if (_reqDetailsObjectClone == null)
                                                                      {
                                                                          _reqDetailsObjectClone = new();
                                                                      }
                                                                      else
                                                                      {
                                                                          _reqDetailsObjectClone.Clear();
                                                                      }
                                                                  }
                                                                  else
                                                                  {
                                                                      Title = "Edit";
                                                                      _reqDetailsObjectClone = _reqDetailsObject.Copy();
                                                                  }

                                                                  await DetailsRequisition.ShowDialog();
                                                                  VisibleSpinner = false;
                                                              });

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

    private async Task GetAlphabets(char alphabet) => await ExecuteMethod(async () =>
                                                                          {
                                                                              SearchModel.Title = alphabet.ToString();
                                                                              SearchModel.Page = 1;
                                                                              /*_query.Queries.Params["GetInformation"] = _companies.Count == 0;*/
                                                                              await Task.WhenAll(SaveStorage(), SetDataSource()).ConfigureAwait(false);
                                                                              //await Grid.Refresh();
                                                                          });

    private string GetDurationCode(string durationCode)
    {
        return durationCode.ToLower() switch
               {
                   "m" => "months",
                   "w" => "weeks",
                   "d" => "days",
                   _ => "years"
               };
    }

    private string GetLocation()
    {
        /*if (_states == null || location.ToInt32() == 0)
        {
            return location;
        }

        foreach (IntValues _intValues in _states.Where(intValues => location.ToInt32() == intValues.KeyValue))
        {
            return _intValues.Text;
        }

        return location;*/
        string _location = string.Empty;
        if (_reqDetailsObject == null)
        {
            return _location;
        }

        if (_reqDetailsObject.City.NotNullOrWhiteSpace())
        {
            _location = _reqDetailsObject.City;
        }

        if (_reqDetailsObject.StateID.ToInt32() != 0)
        {
            foreach (IntValues _intValues in _states.Where(intValues => _reqDetailsObject.StateID.ToInt32() == intValues.KeyValue))
            {
                _location = $"{_location}, {_intValues.Text}";
                break;
            }
        }

        if (_reqDetailsObject.ZipCode.NotNullOrWhiteSpace())
        {
            _location = $"{_location}, {_reqDetailsObject.ZipCode}";
        }

        return _location;
    }

    private Task GridPageChanging(GridPageChangingEventArgs page) => ExecuteMethod(async () =>
                                                                                   {
                                                                                       Page = page.CurrentPage;
                                                                                       SearchModel.Page = page.CurrentPage;

                                                                                       await Task.WhenAll(SaveStorage(), SetDataSource()).ConfigureAwait(false);
                                                                                       //await Grid.Refresh();
                                                                                   });

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            if (await SessionStorage.ContainKeyAsync("OptReqID"))
            {
                SearchModel.Clear();
                SearchModel.OptRequisitionID = await SessionStorage.GetItemAsync<int>("OptReqID");
            }
            else if (await SessionStorage.ContainKeyAsync(StorageName))
            {
                SearchModel = await SessionStorage.GetItemAsync<RequisitionSearch>(StorageName);
                SearchModel.OptRequisitionID = 0;
            }
            else
            {
                SearchModel.Clear();
            }

            await SetDataSource().ConfigureAwait(false);
            Grid.TotalItemCount = 378;
            //await Grid.Refresh().ConfigureAwait(false);
        }
    }

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
        // _initializationTaskSource = new();
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
                                    RoleName = _enumerable.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value.ToUpperInvariant();
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
                                    CacheObjects.StatusCodes.ToString(), CacheObjects.Preferences.ToString(), CacheObjects.Companies.ToString(), CacheObjects.Workflow.ToString(),
                                    CacheObjects.CompanyContacts.ToString()
                                ];

                                RedisService _service = new(Start.CacheServer, Start.CachePort.ToInt32(), Start.Access, false);

                                Dictionary<string, string> _cacheValues = await _service.BatchGet(_keys);

                                //_roles = General.DeserializeObject<List<Role>>(_cacheValues[CacheObjects.Roles.ToString()]); //await Redis.GetAsync<List<Role>>("Roles");

                                _states = General.DeserializeObject<List<IntValues>>(_cacheValues[CacheObjects.States.ToString()]);
                                _eligibility = General.DeserializeObject<List<IntValues>>(_cacheValues[CacheObjects.Eligibility.ToString()]);
                                _education = General.DeserializeObject<List<IntValues>>(_cacheValues[CacheObjects.Education.ToString()]);
                                _experience = General.DeserializeObject<List<IntValues>>(_cacheValues[CacheObjects.Experience.ToString()]);
                                _jobOptions = General.DeserializeObject<List<KeyValues>>(_cacheValues[CacheObjects.JobOptions.ToString()]);

                                while (_recruiters == null)
                                {
                                    List<UserList> _users = General.DeserializeObject<List<UserList>>(_cacheValues[CacheObjects.Users.ToString()]);
                                    if (_users == null)
                                    {
                                        continue;
                                    }

                                    _recruiters = [];
                                    foreach (UserList _user in _users.Where(user => user.Role is 2 or 4 or 5 or 6))
                                    {
                                        _recruiters.Add(new() {KeyValue = _user.UserName, Text = _user.UserName});
                                    }
                                }

                                Skills = General.DeserializeObject<List<IntValues>>(_cacheValues[CacheObjects.Skills.ToString()]);

                                _statusCodes = General.DeserializeObject<List<StatusCode>>(_cacheValues[CacheObjects.StatusCodes.ToString()]);
                                _preference = General.DeserializeObject<Preferences>(_cacheValues[CacheObjects.Preferences.ToString()]);

                                /*if (_statusCodes is {Count: > 0})
                                {
                                    foreach (StatusCode _statusCode in _statusCodes.Where(statusCode => statusCode.AppliesToCode == "REQ"))
                                    {
                                        _statusSearch.Add(new()
                                                          {
                                                              KeyValue = _statusCode.Status,
                                                              Text = _statusCode.Code
                                                          });
                                    }
                                }*/

                                List<CompaniesList> _companyList = General.DeserializeObject<List<CompaniesList>>(_cacheValues[CacheObjects.Companies.ToString()]);

                                //_companies = [];
                                Companies = [];
                                /*_companies.Add(new()
                                               {
                                                   KeyValue = "All Companies",
                                                   Text = "%"
                                               });*/
                                if (_companyList != null)
                                {
                                    foreach (CompaniesList _company in _companyList.Where(company => company.UpdatedBy == User || company.UpdatedBy == "ADMIN"))
                                    {
                                        /*_companies.Add(new()
                                                       {
                                                           KeyValue = _company.CompanyName,
                                                           Text = _company.CompanyName
                                                       });*/

                                        Companies.Add(new()
                                                      {
                                                          ID = _company.ID,
                                                          CompanyName = _company.CompanyName
                                                      });
                                    }
                                }

                                List<CompanyContacts> _companyContacts = General.DeserializeObject<List<CompanyContacts>>(_cacheValues[CacheObjects.CompanyContacts.ToString()]);
                                foreach (CompanyContacts _companyContact in _companyContacts) //.Where(companyContact => _company.ID == companyContact.CompanyID))
                                {
                                    CompanyContacts.Add(new() {CompanyID = _companyContact.CompanyID, ID = _companyContact.ID, ContactName = _companyContact.ContactName});
                                    // break;
                                }

                                _workflows = General.DeserializeObject<List<Workflow>>(_cacheValues[CacheObjects.Workflow.ToString()]);
                            });

        await base.OnInitializedAsync();
    }

    private async Task Refresh(MouseEventArgs arg)
    {
        await SetDataSource().ConfigureAwait(false);
        //await Grid.Refresh().ConfigureAwait(false);
    }

    private Task SaveActivity(EditContext activity) => ExecuteMethod(async () =>
                                                                     {
                                                                         Dictionary<string, string> _parameters = new()
                                                                                                                  {
                                                                                                                      {"candidateID", _target.ID.ToString()},
                                                                                                                      {"user", User},
                                                                                                                      {"roleID", RoleName},
                                                                                                                      {"isCandidateScreen", "false"},
                                                                                                                      {"jsonPath", ""},
                                                                                                                      {"emailAddress", ""},
                                                                                                                      {"uploadPath", ""}
                                                                                                                  };

                                                                         string _response = await General.ExecuteRest<string>("Candidate/SaveCandidateActivity", _parameters,
                                                                                                                              activity.Model);

                                                                         if (_response.NotNullOrWhiteSpace() && _response != "[]")
                                                                         {
                                                                             _candActivityObject = General.DeserializeObject<List<CandidateActivity>>(_response);
                                                                         }
                                                                     });

    /// <summary>
    ///     Asynchronously saves the document associated with the requisition.
    /// </summary>
    /// <param name="document">
    ///     The <see cref="EditContext" /> instance that contains the document to be saved.
    /// </param>
    /// <returns>
    ///     A <see cref="Task" /> that represents the asynchronous operation.
    /// </returns>
    /// <remarks>
    ///     This method uses the REST API to upload the document. The document is converted to a byte array and sent as a file
    ///     in a multipart form data request. Additional parameters, such as filename, mime type, document name, notes,
    ///     requisition ID, user, and path are also sent with the request.
    /// </remarks>
    private Task SaveDocument(EditContext document) => ExecuteMethod(async () =>
                                                                     {
                                                                         if (document.Model is RequisitionDocuments _document)
                                                                         {
                                                                             Dictionary<string, string> _parameters = new()
                                                                                                                      {
                                                                                                                          {"filename", DialogDocument.FileName},
                                                                                                                          {"mime", DialogDocument.Mime},
                                                                                                                          {"name", _document.Name},
                                                                                                                          {"notes", _document.Notes},
                                                                                                                          {"requisitionID", _target.ID.ToString()},
                                                                                                                          {"user", User},
                                                                                                                          {"path", Start.UploadsPath}
                                                                                                                      };
                                                                             string _response = await General.ExecuteRest<string>("Requisition/UploadDocument", _parameters, null, true,
                                                                                                                                  DialogDocument.AddedDocument.ToStreamByteArray(),
                                                                                                                                  DialogDocument.FileName);
                                                                             if (_response.NotNullOrWhiteSpace() && _response != "[]")
                                                                             {
                                                                                 _reqDocumentsObject = General.DeserializeObject<List<RequisitionDocuments>>(_response);
                                                                             }
                                                                         }
                                                                     });

    /// <summary>
    ///     Asynchronously saves the requisition details.
    /// </summary>
    /// <param name="arg">The edit context of the requisition details.</param>
    /// <remarks>
    ///     This method performs the following steps:
    ///     1. Creates a new REST client and request.
    ///     2. Adds the cloned requisition details object to the request body in JSON format.
    ///     3. Adds the user ID, JSON file path, and email address to the request as query parameters.
    ///     4. Sends the request to the server.
    ///     5. Updates the requisition details object with the cloned object.
    ///     6. If the requisition ID is greater than 0, updates the target's title, company, job options, status, and priority
    ///     color.
    ///     7. If the requisition ID is not greater than 0, clears the search model data and refreshes the grid.
    ///     8. Triggers a state change to update the UI.
    /// </remarks>
    /// <returns>A task that represents the asynchronous operation.</returns>
    private Task SaveRequisition(EditContext arg) => ExecuteMethod(async () =>
                                                                   {
                                                                       Dictionary<string, string> _parameters = new()
                                                                                                                {
                                                                                                                    {"user", User},
                                                                                                                    {"jsonPath", ""},
                                                                                                                    {"emailAddress", ""}
                                                                                                                };

                                                                       _ = await General.ExecuteRest<int>("Requisition/SaveRequisition", _parameters, _reqDetailsObjectClone);

                                                                       _reqDetailsObject = _reqDetailsObjectClone.Copy();

                                                                       if (_reqDetailsObject.RequisitionID > 0)
                                                                       {
                                                                           _target.Title = $"{_reqDetailsObject.PositionTitle} ({_candActivityObject.Count})";
                                                                           _target.Company = _reqDetailsObject.CompanyName;
                                                                           _target.JobOptions = _reqDetailsObject.JobOptions;
                                                                           _target.Status = _reqDetailsObject.Status;
                                                                           _target.PriorityColor = _reqDetailsObject.Priority.ToUpperInvariant() switch
                                                                                                   {
                                                                                                       "HIGH" => _preference.HighPriorityColor,
                                                                                                       "LOW" => _preference.LowPriorityColor,
                                                                                                       _ => _preference.NormalPriorityColor
                                                                                                   };
                                                                       }
                                                                       else
                                                                       {
                                                                           SearchModel.Clear();
                                                                           await Grid.Refresh();
                                                                       }

                                                                       StateHasChanged();
                                                                   });

    private async Task SaveStorage(bool refreshGrid = true)
    {
        await SessionStorage.SetItemAsync(StorageName, SearchModel);
        if (refreshGrid)
        {
            await SetDataSource().ConfigureAwait(false);
        }
    }

    private async Task SetDataSource()
    {
        // await Task.CompletedTask;
        Dictionary<string, string> _parameters = new()
                                                 {
                                                     {"getCompanyInformation", Companies.Count.Equals(0).ToBooleanString()},
                                                     {"requisitionID", RequisitionID.ToString()},
                                                     {"thenProceed", false.ToString()},
                                                     {"user", User}
                                                 };
        //(int _count, string _requisitions, string _companies, string _companyContacts, string _status, int _pageNumber) =
        (int _count, string _requisitions, string _, string _, string _status, int _) =
            await General.ExecuteRest<ReturnGridRequisition>("Requisition/GetGridRequisitions", _parameters, SearchModel, false).ConfigureAwait(false);
        DataSource = _count > 0 ? JsonConvert.DeserializeObject<List<Requisition>>(_requisitions) : [];
        Grid.TotalItemCount = _count;
        Count = _count;
        if (_status.NotNullOrWhiteSpace() && _status != "[]")
        {
            await SessionStorage.SetItemAsync("StatusList", _status.CompressGZip()).ConfigureAwait(false);
        }

        await Grid.Refresh().ConfigureAwait(false);
    }

    /// <summary>
    ///     Sets the skills for the requisition details object.
    /// </summary>
    /// <remarks>
    ///     This method sets the required and optional skills for the requisition details object.
    ///     The skills are retrieved from the SkillsRequired and Optional properties of the requisition details object.
    ///     The skills are then formatted into a string and converted to a MarkupString which is stored in the
    ///     _requisitionDetailSkills field.
    ///     If the requisition details object is null or both the SkillsRequired and Optional properties are null or
    ///     whitespace, the method will return without setting the skills.
    /// </remarks>
    private void SetSkills()
    {
        if (_reqDetailsObject == null)
        {
            return;
        }

        if (_reqDetailsObject.SkillsRequired.NullOrWhiteSpace() && _reqDetailsObject.Optional.NullOrWhiteSpace())
        {
            return;
        }

        string _skillsRequired = _reqDetailsObject.SkillsRequired.NullOrWhiteSpace() ? string.Empty :
                                     _reqDetailsObject.SkillsRequired.Split(',')
                                                      .Select(skillString => Skills.FirstOrDefault(skill => skill.KeyValue == skillString.ToInt32())?.Text)
                                                      .Aggregate(string.Empty, (current, text) => current == string.Empty ? text : current + ", " + text)
                                     ?? string.Empty;

        string _skillsOptional = _reqDetailsObject.Optional.NullOrWhiteSpace() ? string.Empty :
                                     _reqDetailsObject.Optional.Split(',')
                                                      .Select(skillString => Skills.FirstOrDefault(skill => skill.KeyValue == skillString.ToInt32())?.Text)
                                                      .Aggregate(string.Empty, (current, text) => current == string.Empty ? text : current + ", " + text)
                                     ?? string.Empty;

        string _skillStringTemp = string.Empty;

        if (!_skillsRequired.NullOrWhiteSpace())
        {
            _skillStringTemp = "<strong>Required Skills:</strong> <br/>" + _skillsRequired + "<br/><br/>";
        }

        if (!_skillsOptional.NullOrWhiteSpace())
        {
            _skillStringTemp += "<strong>Optional Skills:</strong> <br/>" + _skillsOptional;
        }

        _reqDetailSkills = (_skillStringTemp.NullOrWhiteSpace() ? string.Empty : _skillStringTemp).ToMarkupString();
    }

    private Task SpeedDialItemClicked(SpeedDialItemEventArgs args)
    {
        switch (args.Item.ID)
        {
            case "itemEditRequisition":
                _selectedTab = 0;
                return EditRequisition(false);
            case "itemAddDocument":
                _selectedTab = 1;
                return AddDocument();
            case "itemSubmitExisting":
                NavManager.NavigateTo($"{NavManager.BaseUri}candidate?reqid={_target.ID}", true);
                break;
        }

        return Task.CompletedTask;
    }

    private void TabSelected(SelectEventArgs tab) => _selectedTab = tab.SelectedIndex;

    /// <summary>
    ///     Asynchronously undoes a candidate activity based on the provided activity ID.
    /// </summary>
    /// <param name="activityID">The ID of the candidate activity to undo.</param>
    /// <remarks>
    ///     This method sends a POST request to the "Candidates/UndoCandidateActivity" endpoint with the activity ID, user ID,
    ///     and a flag indicating it's from the candidate screen.
    ///     If the user is not logged in, "JOLLY" is used as the user ID. The response is expected to be a dictionary
    ///     containing the activity data, which is then deserialized into a list of CandidateActivity objects.
    ///     If the response is null or an exception occurs during the process, the method will return immediately.
    /// </remarks>
    /// <returns>A Task representing the asynchronous operation.</returns>
    private Task UndoActivity(int activityID) => ExecuteMethod(async () =>
                                                               {
                                                                   Dictionary<string, string> _parameters = new()
                                                                                                            {
                                                                                                                {"submissionID", activityID.ToString()},
                                                                                                                {"user", User},
                                                                                                                {"isCandidateScreen", "false"},
                                                                                                                {"roleID", RoleName}
                                                                                                            };
                                                                   string _response = await General.ExecuteRest<string>("Candidate/UndoCandidateActivity", _parameters);
                                                                   if (_response.NotNullOrWhiteSpace() && _response != "[]")
                                                                   {
                                                                       _candActivityObject = General.DeserializeObject<List<CandidateActivity>>(_response);
                                                                   }
                                                               });

    /*/// <summary>
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
    public class RequisitionAdaptor(ISessionStorageService sessionStorage) : DataAdaptor
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

            /*if (_initializationTaskSource == null)
            {
                return null;
            }

            await _initializationTaskSource.Task;#1#
            try
            {
                RequisitionSearch _searchModel = General.DeserializeObject<RequisitionSearch>(dm.Params["SearchModel"].ToString());
                List<Requisition> _dataSource = [];

                try
                {
                    Dictionary<string, string> _parameters = new()
                                                             {
                                                                 {"getCompanyInformation", dm.Params["GetInformation"].ToString()},
                                                                 {"requisitionID", dm.Params["RequisitionID"].ToString()},
                                                                 {"thenProceed", false.ToString()},
                                                                 {"user", dm.Params["User"].ToString()}
                                                             };
                    (int _count, string _requisitions, string _companies, string _companyContacts, string _status, int _pageNumber) =
                        await General.ExecuteRest<ReturnGridRequisition>("Requisition/GetGridRequisitions", _parameters, _searchModel, false);

                    _dataSource = _count > 0 ? JsonConvert.DeserializeObject<List<Requisition>>(_requisitions) : [];

                    if (_dataSource == null)
                    {
                        return dm.RequiresCounts ? new DataResult
                                                   {
                                                       Result = null,
                                                       Count = 0 /*_count#1#
                                                   } : null;
                    }

                    if (!dm.Params["GetInformation"].ToBoolean())
                    {
                        return dm.RequiresCounts ? new DataResult
                                                   {
                                                       Result = _dataSource,
                                                       Count = _count /*_count#1#
                                                   } : _dataSource;
                    }

                    if (_status.NullOrWhiteSpace())
                    {
                        return dm.RequiresCounts ? new DataResult
                                                   {
                                                       Result = _dataSource,
                                                       Count = _count /*_count#1#
                                                   } : _dataSource;
                    }

                    await sessionStorage.SetItemAsync("StatusList", _status.CompressGZip());

                    return dm.RequiresCounts ? new DataResult
                                               {
                                                   Result = _dataSource,
                                                   Count = _count /*_count#1#
                                               } : _dataSource;
                }
                catch (Exception)
                {
                    if (_dataSource == null)
                    {
                        return dm.RequiresCounts ? new DataResult
                                                   {
                                                       Result = null,
                                                       Count = 1
                                                   } : null;
                    }

                    _dataSource.Add(new());

                    return dm.RequiresCounts ? new DataResult
                                               {
                                                   Result = _dataSource,
                                                   Count = 1
                                               } : _dataSource;
                }
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
    }*/
}