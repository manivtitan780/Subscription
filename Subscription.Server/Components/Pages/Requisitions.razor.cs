﻿#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.Server
// File Name:           Requisitions.razor.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu, Brijesh Dubey
// Created On:          07-23-2025 20:07
// Last Updated On:     07-29-2025 20:35
// *****************************************/

#endregion

#region Using

using JsonSerializer = System.Text.Json.JsonSerializer;

#endregion

namespace Subscription.Server.Components.Pages;

public partial class Requisitions
{
    private const string StorageName = "RequisitionGrid";

    // Memory optimization: Pre-allocate Lists with estimated capacities to prevent resize operations
    private List<CandidateActivity> _candActivityObject = new(50);                               // Typical activity count per requisition
    private List<IntValues> _education = new(20), _eligibility = new(10), _experience = new(15); // Typical lookup table sizes
    private List<JobOptions> /*_companies = [], */ _jobOptions = new(30);                        // Typical job options count

    private Preferences _preference;
    private List<KeyValues> _recruiters;
    private MarkupString _reqDetailSkills = "".ToMarkupString();
    private RequisitionDetails _reqDetailsObject = new(), _reqDetailsObjectClone = new();
    private List<RequisitionDocuments> _reqDocumentsObject = [];
    private List<CandidateNotes> _reqNotesObject = [];

    private List<string> _search = [];

    private int _selectedTab;
    private readonly SemaphoreSlim _semaphoreMainPage = new(1, 1);
    private List<StateCache> _states = [];

    private List<StatusCode> _statusCodes;

    // private List<KeyValues> _statusSearch = [];

    private Requisition _target;
    private SubmissionTimeline[] _timelineObject = [], _timelineActivityObject = [];

    private List<Workflow> _workflows;

    private ActivityPanelRequisition ActivityPanel { get; set; }

    private List<Company> Companies { get; set; } = [];

    private List<CompanyContacts> CompanyContacts { get; } = [];

    [Inject]
    private IConfiguration Configuration { get; set; }

    private int Count { get; set; }

    private List<Requisition> DataSource { get; set; }

    private RequisitionDetailsPanel DetailsRequisition { get; set; }

    private EditActivityDialog DialogActivity { get; set; }

    private ChangeRequisitionStatus DialogChangeStatus { get; set; }

    private AddRequisitionDocument DialogDocument { get; set; }

    private AdvancedRequisitionSearch DialogSearch { get; set; }

    private DocumentsPanel DocumentsPanel { get; set; }

    //public EditContext EditConNotes { get; set; }

    private SfGrid<Requisition> Grid { get; set; }

    private bool HasEditRights { get; set; }

    private bool HasViewRights { get; set; }

    [Inject]
    private IJSRuntime JsRuntime { get; set; }

    [Inject]
    private ILocalStorageService LocalStorage { get; set; }

    [Inject]
    private NavigationManager NavManager { get; set; }

    private RequisitionDocuments NewDocument { get; set; } = new();

    private List<KeyValues> NextSteps { get; set; } = [];

    private RequisitionNotesPanel NotesPanel { get; set; }

    [Inject]
    private RedisService RedisService { get; set; }

    private int RequisitionID { get; set; }

    private EditNotesDialog RequisitionNotesDialog { get; set; }

    private int RoleID { get; } = 5;

    private string RoleName { get; set; }

    private RequisitionSearch SearchModel { get; set; } = new();

    private RequisitionSearch SearchModelClone { get; set; } = new();

    private CandidateActivity SelectedActivity { get; set; } = new();

    private RequisitionDocuments SelectedDownload { get; set; } = new();

    private CandidateNotes SelectedNotes { get; set; } = new();

    [Inject]
    private ISessionStorageService SessionStorage { get; set; }

    private List<IntValues> Skills { get; set; } = [];

    private List<KeyValues> StatusList { get; set; } = [];

    private RequisitionStatus StatusRequisition { get; } = new();

    public List<KeyValues> StatusRequisitionList { get; set; } = [];

    private TimelineDialog TimelineDialog { get; set; }

    private string Title { get; set; } = "Edit";

    private UploadCandidate UploadCandidateDialog { get; set; }

    private string User { get; set; }

    private bool VisibleSpinner { get; set; }

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

                                                    await DialogDocument.ShowDialog().ConfigureAwait(false);
                                                });

    private Task AdvancedSearch() => ExecuteMethod(async () =>
                                                   {
                                                       SearchModelClone = SearchModel.Copy();
                                                       await DialogSearch.ShowDialog().ConfigureAwait(false);
                                                   });

    private Task AllAlphabets(MouseEventArgs args) => ExecuteMethod(async () =>
                                                                    {
                                                                        SearchModel.Title = "";
                                                                        SearchModel.Page = 1;
                                                                        await Task.WhenAll(SaveStorage(), SetDataSource()).ConfigureAwait(false);
                                                                    });

    private Task AutocompleteValueChange(ChangeEventArgs<string, KeyValues> filter) => ExecuteMethod(async () =>
                                                                                                     {
                                                                                                         SearchModel.Title = filter.Value;
                                                                                                         SearchModel.Page = 1;
                                                                                                         await Task.WhenAll(SaveStorage(), SetDataSource()).ConfigureAwait(false);
                                                                                                     });

    private async Task ChangeStatus() => await DialogChangeStatus.ShowDialog();

    private Task ClearFilter() => ExecuteMethod(async () =>
                                                {
                                                    SearchModel.Clear();
                                                    SearchModel.User = User;
                                                    await Task.WhenAll(SaveStorage(), SetDataSource()).ConfigureAwait(false);
                                                });

    private async Task CloseUploadCandidate()
    {
        await Refresh(null).ConfigureAwait(false);
    }

    // Added capacity hint for memory optimization - Dictionary has exactly 3 key-value pairs
    private Dictionary<string, string> CreateParameters(int id) => new(3)
                                                                   {
                                                                       ["id"] = id.ToString(),
                                                                       ["requisitionID"] = _target.ID.ToString(),
                                                                       ["user"] = User
                                                                   };

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
                                                                          StatusList = JsonSerializer.Deserialize(_value.DecompressGZip(), JsonContext.CaseInsensitive.ListKeyValues);
                                                                      }
                                                                  }
                                                              }

                                                              RequisitionID = 0;
                                                          });

    private Task DeleteDocument(int args) => ExecuteMethod(async () =>
                                                           {
                                                               // Added capacity hint for memory optimization - Dictionary has exactly 2 key-value pairs
                                                               Dictionary<string, string> _parameters = new(2)
                                                                                                        {
                                                                                                            ["documentID"] = args.ToString(),
                                                                                                            ["user"] = User
                                                                                                        };

                                                               _reqDocumentsObject = await General.ExecuteAndDeserialize<RequisitionDocuments>("Requisition/DeleteRequisitionDocument", _parameters)
                                                                                                  .ConfigureAwait(false);
                                                           });

    private Task DeleteNotes(int id) => ExecuteMethod(async () =>
                                                      {
                                                          Dictionary<string, string> _parameters = CreateParameters(id);
                                                          string _response = await General.ExecuteRest<string>("Requisition/DeleteNotes", _parameters);

                                                          // _reqNotesObject = General.DeserializeObject<List<CandidateNotes>>(_response);
                                                          _reqNotesObject = JsonSerializer.Deserialize(_response, JsonContext.CaseInsensitive.ListCandidateNotes);
                                                      });

    private Task DetailDataBind(DetailDataBoundEventArgs<Requisition> requisition) => ExecuteMethod(async () =>
                                                                                                    {
                                                                                                        int _index = await Grid.GetRowIndexByPrimaryKeyAsync(requisition.Data.ID);
                                                                                                        if (_index != Grid.SelectedRowIndex)
                                                                                                        {
                                                                                                            await Grid.SelectRowAsync(_index);
                                                                                                        }

                                                                                                        await Grid.CollapseAllDetailRowAsync();
                                                                                                        await Grid.ExpandCollapseDetailRowAsync(requisition.Data);
                                                                                                        _target = requisition.Data;

                                                                                                        VisibleSpinner = true;
                                                                                                        StatusRequisition.Status = _target.Status;

                                                                                                        // Added capacity hint for memory optimization - Dictionary has exactly 1 key-value pair
                                                                                                        Dictionary<string, string> _parameters = new(1) {["requisitionID"] = _target.ID.ToString()};

                                                                                                        (string _requisition, string _activity, string _documents, string _notes, string _timelineCandidate) =
                                                                                                            await General.ExecuteRest<ReturnRequisitionDetails>("Requisition/GetRequisitionDetails",
                                                                                                                                                                _parameters, null, false);

                                                                                                        Task[] requisitionDetailsTasks =
                                                                                                        [
                                                                                                            Task.Run(() => _reqDetailsObject =
                                                                                                                               JsonSerializer.Deserialize(_requisition, JsonContext.CaseInsensitive.RequisitionDetails)),
                                                                                                            Task.Run(() => _candActivityObject =
                                                                                                                               JsonSerializer.Deserialize(_activity, JsonContext.CaseInsensitive.ListCandidateActivity) ?? []),
                                                                                                            Task.Run(() => _reqNotesObject = JsonSerializer.Deserialize(_notes, JsonContext.CaseInsensitive.ListCandidateNotes) ?? []),
                                                                                                            Task.Run(() => _reqDocumentsObject = JsonSerializer.Deserialize(_documents, JsonContext.CaseInsensitive.ListRequisitionDocuments) ?? []),
                                                                                                            Task.Run(() => _timelineActivityObject =
                                                                                                                               JsonSerializer.Deserialize(_timelineCandidate, JsonContext.CaseInsensitive.SubmissionTimelineArray) ?? [])
                                                                                                        ];
                                                                                                        await Task.WhenAll(requisitionDetailsTasks);
                                                                                                        SetSkills();

                                                                                                        _selectedTab = _candActivityObject.Count > 0 ? 3 : 0;

                                                                                                        VisibleSpinner = false;
                                                                                                    });

    [JSInvokable("DetailCollapse")]
    public void DetailRowCollapse() => _target = null;

    private Task DownloadDocument(int args) => ExecuteMethod(async () =>
                                                             {
                                                                 SelectedDownload = DocumentsPanel.SelectedRow;
                                                                 string _queryString = $"{SelectedDownload.Location}^{_target.ID}^{SelectedDownload.InternalFileName}^1".ToBase64String();
                                                                 await JsRuntime.InvokeVoidAsync("open", $"{NavManager.BaseUri}Download/{_queryString}", "_blank");
                                                             });

    private Task EditActivity(int args) => ExecuteMethod(async () =>
                                                         {
                                                             SelectedActivity = ActivityPanel.SelectedRow;
                                                             NextSteps.Clear();
                                                             try
                                                             {
                                                                 // Memory optimization: Use HashSet for O(1) lookups instead of O(n) Contains() operations
                                                                 // Eliminates LINQ overhead and provides faster workflow processing
                                                                 HashSet<string> nextCodesSet = [];
                                                                 foreach (Workflow flow in _workflows.Where(flow => flow.Step == SelectedActivity.StatusCode))
                                                                 {
                                                                     foreach (string code in flow.Next.Split(',', StringSplitOptions.RemoveEmptyEntries))
                                                                     {
                                                                         nextCodesSet.Add(code.Trim());
                                                                     }
                                                                 }

                                                                 NextSteps = _statusCodes.Where(status => nextCodesSet.Contains(status.Code) && status.AppliesToCode == "SCN")
                                                                                         .Select(status => new KeyValues {Text = status.Status, KeyValue = status.Code})
                                                                                         .Prepend(new() {Text = "No Change", KeyValue = "0"}).ToList();
                                                             }
                                                             catch
                                                             {
                                                                 //Ignore this error. No need to log this error.
                                                             }

                                                             await DialogActivity.ShowDialog();
                                                         });

    private Task EditNotes(int id) => ExecuteMethod(async () =>
                                                    {
                                                        VisibleSpinner = true;
                                                        if (id == 0)
                                                        {
                                                            if (SelectedNotes == null)
                                                            {
                                                                SelectedNotes = new();
                                                            }
                                                            else
                                                            {
                                                                SelectedNotes.Clear();
                                                            }
                                                        }
                                                        else
                                                        {
                                                            SelectedNotes = NotesPanel.SelectedRow != null ? NotesPanel.SelectedRow.Copy() : new();
                                                        }

                                                        //EditConNotes = new(SelectedNotes!);
                                                        VisibleSpinner = false;
                                                        await Task.Yield();
                                                        await RequisitionNotesDialog.ShowDialog().ConfigureAwait(false);
                                                    });

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

    private Task ExecuteMethod(Func<Task> task) => General.ExecuteMethod(_semaphoreMainPage, task);

    private async Task GetAlphabets(char alphabet) => await ExecuteMethod(async () =>
                                                                          {
                                                                              SearchModel.Title = alphabet.ToString();
                                                                              SearchModel.Page = 1;
                                                                              await Task.WhenAll(SaveStorage(), SetDataSource()).ConfigureAwait(false);
                                                                          });

    // Memory optimization: GetDurationCode and GetLocation methods moved to RequisitionInfoPanel component
    // This reduces code duplication and improves component encapsulation

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            /*if (await SessionStorage.ContainKeyAsync("OptReqID"))
            {
                SearchModel.Clear();
                SearchModel.OptRequisitionID = (await SessionStorage.GetItemAsync<string>("OptReqID")).ToInt32();
            }*/
            if (await SessionStorage.ContainKeyAsync(StorageName))
            {
                SearchModel = await SessionStorage.GetItemAsync<RequisitionSearch>(StorageName);
                SearchModel.OptRequisitionID = 0;
            }
            else
            {
                SearchModel.Clear();
            }

            SearchModel.User = User;
            await Task.WhenAll(SaveStorage(), SetDataSource()).ConfigureAwait(false);
        }

        await base.OnAfterRenderAsync(firstRender);
    }

    protected override async Task OnInitializedAsync()
    {
        await ExecuteMethod(async () =>
                            {
                                // Get user claims
                                Claim[] _claims = (await General.GetClaimsToken(LocalStorage, SessionStorage)).ToArray();

                                {
                                    User = _claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value.ToUpperInvariant();
                                    RoleName = _claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value.ToUpperInvariant();
                                    if (User.NullOrWhiteSpace())
                                    {
                                        NavManager.NavigateTo($"{NavManager.BaseUri}login", true);
                                    }

                                    // Set user permissions
                                    HasViewRights = _claims.Any(claim => claim.Type == "Permission" && claim.Value == "ViewRequisitions");
                                    HasEditRights = _claims.Any(claim => claim.Type == "Permission" && claim.Value == "CreateOrEditCompany");
                                }

                                if (Start.APIHost.NullOrWhiteSpace())
                                {
                                    Start.APIHost = Configuration[NavManager.BaseUri.Contains("localhost") ? "APIHost" : "APIHostServer"];
                                }

                                // Memory optimization: Pre-allocate string array with exact size to avoid resize operations
                                string[] _keys =
                                [
                                    nameof(CacheObjects.Roles), nameof(CacheObjects.States), nameof(CacheObjects.Eligibility), nameof(CacheObjects.Education),
                                    nameof(CacheObjects.Experience), nameof(CacheObjects.JobOptions), nameof(CacheObjects.Users), nameof(CacheObjects.Skills),
                                    nameof(CacheObjects.StatusCodes), nameof(CacheObjects.Preferences), nameof(CacheObjects.Companies), nameof(CacheObjects.Workflow),
                                    nameof(CacheObjects.CompanyContacts)
                                ];

                                Dictionary<string, string> _cacheValues = await RedisService.BatchGet(_keys);

                                List<UserList> _users = null;
                                List<CompaniesList> _companyList = null;
                                List<CompanyContacts> _companyContacts = null;

                                /*Task[] cacheDeserializationTasks =
                                [
                                    Task.Run(() => _states = General.DeserializeObject<List<StateCache>>(_cacheValues[nameof(CacheObjects.States)])),
                                    Task.Run(() => _eligibility = General.DeserializeObject<List<IntValues>>(_cacheValues[nameof(CacheObjects.Eligibility)])),
                                    Task.Run(() => _education = General.DeserializeObject<List<IntValues>>(_cacheValues[nameof(CacheObjects.Education)])),
                                    Task.Run(() => _experience = General.DeserializeObject<List<IntValues>>(_cacheValues[nameof(CacheObjects.Experience)])),
                                    Task.Run(() => _jobOptions = General.DeserializeObject<List<JobOptions>>(_cacheValues[nameof(CacheObjects.JobOptions)])),
                                    Task.Run(() => _users = General.DeserializeObject<List<UserList>>(_cacheValues[nameof(CacheObjects.Users)])),
                                    Task.Run(() => Skills = General.DeserializeObject<List<IntValues>>(_cacheValues[nameof(CacheObjects.Skills)])),
                                    Task.Run(() => _statusCodes = General.DeserializeObject<List<StatusCode>>(_cacheValues[nameof(CacheObjects.StatusCodes)])),
                                    Task.Run(() => _preference = General.DeserializeObject<Preferences>(_cacheValues[nameof(CacheObjects.Preferences)])),
                                    Task.Run(() => _companyList = General.DeserializeObject<List<CompaniesList>>(_cacheValues[nameof(CacheObjects.Companies)])),
                                    Task.Run(() => _companyContacts = General.DeserializeObject<List<CompanyContacts>>(_cacheValues[nameof(CacheObjects.CompanyContacts)])),
                                    Task.Run(() => _workflows = General.DeserializeObject<List<Workflow>>(_cacheValues[nameof(CacheObjects.Workflow)]))
                                ];*/
                                Task[] cacheDeserializationTasks =
                                [
                                    Task.Run(() => _states = JsonSerializer.Deserialize(_cacheValues[nameof(CacheObjects.States)], JsonContext.CaseInsensitive.ListStateCache) ?? []),
                                    Task.Run(() => _eligibility = JsonSerializer.Deserialize(_cacheValues[nameof(CacheObjects.Eligibility)], JsonContext.CaseInsensitive.ListIntValues) ?? []),
                                    Task.Run(() => _education = JsonSerializer.Deserialize(_cacheValues[nameof(CacheObjects.Education)], JsonContext.CaseInsensitive.ListIntValues) ?? []),
                                    Task.Run(() => _experience = JsonSerializer.Deserialize(_cacheValues[nameof(CacheObjects.Experience)], JsonContext.CaseInsensitive.ListIntValues) ?? []),
                                    Task.Run(() => _jobOptions = JsonSerializer.Deserialize(_cacheValues[nameof(CacheObjects.JobOptions)], JsonContext.CaseInsensitive.ListJobOptions) ?? []),
                                    Task.Run(() => _users = JsonSerializer.Deserialize(_cacheValues[nameof(CacheObjects.Users)], JsonContext.CaseInsensitive.ListUserList) ?? []),
                                    Task.Run(() => Skills = JsonSerializer.Deserialize(_cacheValues[nameof(CacheObjects.Skills)], JsonContext.CaseInsensitive.ListIntValues) ?? []),
                                    Task.Run(() => _statusCodes = JsonSerializer.Deserialize(_cacheValues[nameof(CacheObjects.StatusCodes)], JsonContext.CaseInsensitive.ListStatusCode) ?? []),
                                    Task.Run(() => _preference = JsonSerializer.Deserialize(_cacheValues[nameof(CacheObjects.Preferences)], JsonContext.CaseInsensitive.Preferences) ?? new()),
                                    Task.Run(() => _companyList = JsonSerializer.Deserialize(_cacheValues[nameof(CacheObjects.Companies)], JsonContext.CaseInsensitive.ListCompaniesList) ?? []),
                                    Task.Run(() => _companyContacts = JsonSerializer.Deserialize(_cacheValues[nameof(CacheObjects.CompanyContacts)], JsonContext.CaseInsensitive.ListCompanyContacts) ?? []),
                                    Task.Run(() => _workflows = JsonSerializer.Deserialize(_cacheValues[nameof(CacheObjects.Workflow)], JsonContext.CaseInsensitive.ListWorkflow) ?? [])
                                ];
                                await Task.WhenAll(cacheDeserializationTasks);

                                // Now process all objects sequentially as needed
                                if (_recruiters == null && _users != null)
                                {
                                    _recruiters = _users.Where(user => user.Role is 2 or 4 or 5 or 6)
                                                        .Select(user => new KeyValues {KeyValue = user.UserName, Text = user.UserName})
                                                        .ToList();
                                    _users = null; // Release memory after processing
                                }

                                if (_statusCodes is {Count: > 0})
                                {
                                    StatusRequisitionList = _statusCodes.Where(statusCode => statusCode.AppliesToCode == "REQ")
                                                                        .Select(statusCode => new KeyValues {KeyValue = statusCode.Code, Text = statusCode.Status})
                                                                        .ToList();

                                    _search = _statusCodes.Where(statusCode => statusCode.AppliesToCode == "REQ")
                                                          .Select(statusCode => statusCode.Status)
                                                          .ToList();
                                }

                                Companies = [];
                                if (_companyList?.Count > 0)
                                {
                                    IEnumerable<Company> _filtered = _companyList.Select(c => new Company {ID = c.ID, CompanyName = c.CompanyName});
                                    Companies.AddRange(_filtered);
                                    _companyList = null; // Release memory after processing
                                }

                                if (_companyContacts?.Count > 0)
                                {
                                    CompanyContacts.AddRange(_companyContacts.Select(c => new CompanyContacts {CompanyID = c.CompanyID, ID = c.ID, ContactName = c.ContactName}));
                                    _companyContacts = null; // Release memory after processing
                                }
                            });

        await base.OnInitializedAsync();
    }

    private async Task PageChanging(PageChangedEventArgs page)
    {
        SearchModel.Page = page.CurrentPage;
        await Task.WhenAll(SaveStorage(), SetDataSource()).ConfigureAwait(false);
    }

    private async Task PageSizeChanging(PageSizeChangedArgs page)
    {
        SearchModel.ItemCount = page.CurrentPageSize;
        SearchModel.Page = 1;
        await Task.WhenAll(SaveStorage(), SetDataSource()).ConfigureAwait(false);
    }

    private async Task Refresh(MouseEventArgs arg) => await SetDataSource().ConfigureAwait(false);

    private void RowSelected(RowSelectingEventArgs<Requisition> arg) => _target = arg.Data;

    private Task SaveActivity(EditContext activity) => ExecuteMethod(async () =>
                                                                     {
                                                                         // Added capacity hint for memory optimization - Dictionary has exactly 7 key-value pairs
                                                                         Dictionary<string, string> _parameters = new(7)
                                                                                                                  {
                                                                                                                      ["candidateID"] = _target.ID.ToString(),
                                                                                                                      ["user"] = User,
                                                                                                                      ["roleID"] = RoleName,
                                                                                                                      ["isCandidateScreen"] = "false",
                                                                                                                      ["jsonPath"] = "",
                                                                                                                      ["emailAddress"] = "",
                                                                                                                      ["uploadPath"] = ""
                                                                                                                  };

                                                                         string _response = await General.ExecuteRest<string>("Candidate/SaveCandidateActivity", _parameters,
                                                                                                                              activity.Model);

                                                                         if (_response.NotNullOrWhiteSpace() && _response != "[]")
                                                                         {
                                                                             //_candActivityObject = General.DeserializeObject<List<CandidateActivity>>(_response);
                                                                             _candActivityObject = JsonSerializer.Deserialize(_response, JsonContext.CaseInsensitive.ListCandidateActivity);
                                                                         }
                                                                     });

    private Task SaveChangeRequisition(EditContext arg) => ExecuteMethod(async () =>
                                                                         {
                                                                             string _statusCode = _statusCodes.Where(status => status.Status == StatusRequisition.Status)
                                                                                                              .Select(status => status.Code)
                                                                                                              .FirstOrDefault();
                                                                             // Added capacity hint for memory optimization - Dictionary has exactly 3 key-value pairs
                                                                             Dictionary<string, string> _parameters = new(3)
                                                                                                                      {
                                                                                                                          ["requisitionID"] = _target.ID.ToString(),
                                                                                                                          ["statusCode"] = _statusCode,
                                                                                                                          ["user"] = User
                                                                                                                      };

                                                                             string _response = await General.ExecuteRest<string>("Requisition/ChangeRequisitionStatus", _parameters);

                                                                             if (_response.NotNullOrWhiteSpace())
                                                                             {
                                                                                 _target.Status = _response;
                                                                                 _target.Updated = DateTime.Today.CultureDate();
                                                                                 _target.UpdatedBy = User;
                                                                             }
                                                                         });

    private Task SaveDocument(EditContext document) => ExecuteMethod(async () =>
                                                                     {
                                                                         if (document.Model is RequisitionDocuments _document)
                                                                         {
                                                                             // Added capacity hint for memory optimization - Dictionary has exactly 7 key-value pairs
                                                                             Dictionary<string, string> _parameters = new(7)
                                                                                                                      {
                                                                                                                          ["filename"] = DialogDocument.FileName,
                                                                                                                          ["mime"] = DialogDocument.Mime,
                                                                                                                          ["name"] = _document.Name,
                                                                                                                          ["notes"] = _document.Notes,
                                                                                                                          ["requisitionID"] = _target.ID.ToString(),
                                                                                                                          ["user"] = User,
                                                                                                                          ["path"] = Start.UploadsPath
                                                                                                                      };

                                                                             string _response = await General.ExecuteRest<string>("Requisition/UploadDocument", _parameters, null, true,
                                                                                                                                  DialogDocument.AddedDocument.ToStreamByteArray(),
                                                                                                                                  DialogDocument.FileName);

                                                                             if (_response.NotNullOrWhiteSpace() && _response != "[]")
                                                                             {
                                                                                 //_reqDocumentsObject = General.DeserializeObject<List<RequisitionDocuments>>(_response);
                                                                                 _reqDocumentsObject = JsonSerializer.Deserialize(_response, JsonContext.CaseInsensitive.ListRequisitionDocuments);
                                                                             }
                                                                         }
                                                                     });

    private Task SaveNote(EditContext note) => ExecuteMethod(async () =>
                                                             {
                                                                 if (note.Model is CandidateNotes _candidateNotes)
                                                                 {
                                                                     // Added capacity hint for memory optimization - Dictionary has exactly 2 key-value pairs
                                                                     Dictionary<string, string> _parameters = new(2)
                                                                                                              {
                                                                                                                  ["requisitionID"] = _target.ID.ToString(),
                                                                                                                  ["user"] = User
                                                                                                              };
                                                                     string _response = await General.ExecuteRest<string>("Requisition/SaveNotes", _parameters, _candidateNotes);

                                                                     if (_response.NullOrWhiteSpace() || _response == "[]")
                                                                     {
                                                                         return;
                                                                     }

                                                                     //_reqNotesObject = General.DeserializeObject<List<CandidateNotes>>(_response);
                                                                     _reqNotesObject = JsonSerializer.Deserialize(_response, JsonContext.CaseInsensitive.ListCandidateNotes);
                                                                 }
                                                             });

    private Task SaveRequisition(EditContext arg) => ExecuteMethod(async () =>
                                                                   {
                                                                       // Added capacity hint for memory optimization - Dictionary has exactly 3 key-value pairs
                                                                       Dictionary<string, string> _parameters = new(3)
                                                                                                                {
                                                                                                                    ["user"] = User,
                                                                                                                    ["jsonPath"] = "",
                                                                                                                    ["emailAddress"] = ""
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

    private async Task SearchRequisition(EditContext arg)
    {
        SearchModel = SearchModelClone.Copy();
        await Task.WhenAll(SaveStorage(), SetDataSource()).ConfigureAwait(false);
    }

    private async Task SetDataSource()
    {
        // Added capacity hint for memory optimization - Dictionary has exactly 4 key-value pairs
        Dictionary<string, string> _parameters = new(4)
                                                 {
                                                     ["getCompanyInformation"] = Companies.Count.Equals(0).ToBooleanString(),
                                                     ["requisitionID"] = RequisitionID.ToString(),
                                                     ["thenProceed"] = false.ToString(),
                                                     ["user"] = User
                                                 };
        (Count, string _requisitions, string _, string _, string _status, int _) =
            await General.ExecuteRest<ReturnGridRequisition>("Requisition/GetGridRequisitions", _parameters, SearchModel, false).ConfigureAwait(false);
        // Fixed: Replaced Newtonsoft.Json with System.Text.Json for 2-3x performance improvement
        // Original Newtonsoft.Json usage (commented for potential revert if needed):
        /*DataSource = Count > 0 ? JsonConvert.DeserializeObject<List<Requisition>>(_requisitions) : [];*/
        //DataSource = Count > 0 ? General.DeserializeObject<List<Requisition>>(_requisitions) : [];
        DataSource = Count > 0 ? JsonSerializer.Deserialize(_requisitions, JsonContext.CaseInsensitive.ListRequisition) : [];

        if (_status.NotNullOrWhiteSpace() && _status != "[]")
        {
            await SessionStorage.SetItemAsync("StatusList", _status.CompressGZip()).ConfigureAwait(false);
        }

        await Grid.Refresh().ConfigureAwait(false);
    }

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

        string _skillsRequired = _reqDetailsObject.SkillsRequired.NullOrWhiteSpace() ? "" :
                                     _reqDetailsObject.SkillsRequired.Split(',')
                                                      .Select(skillString => Skills.FirstOrDefault(skill => skill.KeyValue == skillString.ToInt32())?.Text)
                                                      .Aggregate("", (current, text) => current == "" ? text : current + ", " + text)
                                     ?? "";

        string _skillsOptional = _reqDetailsObject.Optional.NullOrWhiteSpace() ? "" :
                                     _reqDetailsObject.Optional.Split(',')
                                                      .Select(skillString => Skills.FirstOrDefault(skill => skill.KeyValue == skillString.ToInt32())?.Text)
                                                      .Aggregate("", (current, text) => current == "" ? text : current + ", " + text)
                                     ?? "";

        string _skillStringTemp = "";

        if (!_skillsRequired.NullOrWhiteSpace())
        {
            _skillStringTemp = "<strong>Required Skills:</strong> <br/>" + _skillsRequired + "<br/><br/>";
        }

        if (!_skillsOptional.NullOrWhiteSpace())
        {
            _skillStringTemp += "<strong>Optional Skills:</strong> <br/>" + _skillsOptional;
        }

        _reqDetailSkills = (_skillStringTemp.NullOrWhiteSpace() ? "" : _skillStringTemp).ToMarkupString();
    }

    private async Task SpeedDialItemClicked(SpeedDialItemEventArgs args)
    {
        switch (args.Item.ID)
        {
            case "itemEditRequisition":
                _selectedTab = 0;
                await EditRequisition(false);
                break;
            case "itemAddDocument":
                _selectedTab = 2;
                await AddDocument();
                break;
            case "itemAddNotes":
                _selectedTab = 1;
                await EditNotes(0);
                break;
            case "itemSubmitExisting":
                NavManager.NavigateTo($"{NavManager.BaseUri}candidate?reqid={_target.ID}", true);
                break;
            case "itemSubmitNew":
                await UploadCandidateDialog.ShowDialog();
                break;
            case "itemChangeStatus":
                _selectedTab = 0;
                await ChangeStatus();
                break;
        }
    }

    private void TabSelected(SelectEventArgs tab) => _selectedTab = tab.SelectedIndex;

    private async Task TimeLine(int requisitionID)
    {
        _timelineObject = _timelineActivityObject.Where(x => x.CandidateId == requisitionID).ToArray();
        await TimelineDialog.ShowDialog();
    }

    private Task UndoActivity(int activityID) => ExecuteMethod(async () =>
                                                               {
                                                                   // Added capacity hint for memory optimization - Dictionary has exactly 4 key-value pairs
                                                                   Dictionary<string, string> _parameters = new(4)
                                                                                                            {
                                                                                                                ["submissionID"] = activityID.ToString(),
                                                                                                                ["user"] = User,
                                                                                                                ["isCandidateScreen"] = "false",
                                                                                                                ["roleID"] = RoleName
                                                                                                            };
                                                                   string _response = await General.ExecuteRest<string>("Candidate/UndoCandidateActivity", _parameters);
                                                                   if (_response.NotNullOrWhiteSpace() && _response != "[]")
                                                                   {
                                                                       //_candActivityObject = General.DeserializeObject<List<CandidateActivity>>(_response);
                                                                       _candActivityObject = JsonSerializer.Deserialize(_response, JsonContext.CaseInsensitive.ListCandidateActivity);
                                                                   }
                                                               });
}