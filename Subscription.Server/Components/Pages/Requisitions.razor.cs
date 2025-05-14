#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.Server
// File Name:           Requisitions.razor.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu, Brijesh Dubey
// Created On:          02-06-2025 19:02
// Last Updated On:     05-12-2025 20:17
// *****************************************/

#endregion

namespace Subscription.Server.Components.Pages;

public partial class Requisitions
{
    private const string StorageName = "RequisitionGrid";

    private List<CandidateActivity> _candActivityObject = [];
    private List<IntValues> _education = [], _eligibility = [], _experience = [];
    private List<JobOptions> /*_companies = [], */ _jobOptions = [];

    private Preferences _preference;
    private List<KeyValues> _recruiters;
    private MarkupString _reqDetailSkills = "".ToMarkupString();
    private RequisitionDetails _reqDetailsObject = new(), _reqDetailsObjectClone = new();
    private List<RequisitionDocuments> _reqDocumentsObject = [];

    private List<string> _search = [];

    private int _selectedTab;
    private readonly SemaphoreSlim _semaphoreMainPage = new(1, 1);
    private List<StateCache> _states = [];

    private List<StatusCode> _statusCodes;

    // private List<KeyValues> _statusSearch = [];

    private Requisition _target;

    private readonly List<Workflow> _workflow = [];
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

    private AddRequisitionDocument DialogDocument { get; set; }

    private AdvancedRequisitionSearch DialogSearch { get; set; }

    private DocumentsPanel DocumentsPanel { get; set; }

    private SfGrid<Requisition> Grid { get; set; }

    private bool HasEditRights { get; set; }

    public bool HasViewRights { get; set; }

    [Inject]
    private IJSRuntime JsRuntime { get; set; }

    [Inject]
    private ILocalStorageService LocalStorage { get; set; }

    [Inject]
    private NavigationManager NavManager { get; set; }

    private RequisitionDocuments NewDocument { get; set; } = new();

    private List<KeyValues> NextSteps { get; } = [];

    [Inject]
    private RedisService RedisService { get; set; }

    private int RequisitionID { get; set; }

    private int RoleID { get; } = 5;

    private string RoleName { get; set; }

    private RequisitionSearch SearchModel { get; set; } = new();

    private RequisitionSearch SearchModelClone { get; set; } = new();

    private CandidateActivity SelectedActivity { get; set; } = new();

    private RequisitionDocuments SelectedDownload { get; set; } = new();

    [Inject]
    private ISessionStorageService SessionStorage { get; set; }

    private List<IntValues> Skills { get; set; } = [];

    private List<KeyValues> StatusList { get; set; } = [];

    private string Title { get; set; } = "Edit";

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

                                                    await DialogDocument.ShowDialog();
                                                });

    private Task AdvancedSearch() => ExecuteMethod(async () =>
                                                   {
                                                       SearchModelClone = SearchModel.Copy();
                                                       await DialogSearch.ShowDialog();
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

    private Task ClearFilter() => ExecuteMethod(async () =>
                                                {
                                                    SearchModel.Clear();
                                                    SearchModel.User = User;
                                                    await Task.WhenAll(SaveStorage(), SetDataSource()).ConfigureAwait(false);
                                                });

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

                                                                                                        Dictionary<string, string> _parameters = new()
                                                                                                                                                 {
                                                                                                                                                     {"requisitionID", _target.ID.ToString()}
                                                                                                                                                 };

                                                                                                        (string _requisition, string _activity, string _documents) =
                                                                                                            await General.ExecuteRest<ReturnRequisitionDetails>("Requisition/GetRequisitionDetails",
                                                                                                                                                                _parameters, null, false);

                                                                                                        _reqDetailsObject = General.DeserializeObject<RequisitionDetails>(_requisition);
                                                                                                        _candActivityObject = General.DeserializeObject<List<CandidateActivity>>(_activity) ?? [];
                                                                                                        _reqDocumentsObject = General.DeserializeObject<List<RequisitionDocuments>>(_documents) ?? [];
                                                                                                        SetSkills();

                                                                                                        _selectedTab = _candActivityObject.Count > 0 ? 2 : 0;

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
        // string _location = "";
        if (_reqDetailsObject == null)
        {
            return "";
        }

        List<string> _parts = [];
        if (_reqDetailsObject.City.NotNullOrWhiteSpace())
        {
            _parts.Add(_reqDetailsObject.City);
        }

        if (_reqDetailsObject.StateID.ToInt32() != 0)
        {
            StateCache _state = _states.FirstOrDefault(s => s.KeyValue == _reqDetailsObject.StateID.ToInt32());
            _parts.Add(_state.Code);
        }

        if (_reqDetailsObject.ZipCode.NotNullOrWhiteSpace())
        {
            _parts.Add(_reqDetailsObject.ZipCode);
        }

        return string.Join(", ", _parts);
    }

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
                                IEnumerable<Claim> _claims = await General.GetClaimsToken(LocalStorage, SessionStorage);

                                if (_claims == null)
                                {
                                    NavManager.NavigateTo($"{NavManager.BaseUri}login", true);
                                }
                                else
                                {
                                    Claim[] _enumerable = _claims as Claim[] ?? _claims.ToArray();
                                    User = _enumerable.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value.ToUpperInvariant();
                                    RoleName = _enumerable.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value.ToUpperInvariant();
                                    if (User.NullOrWhiteSpace())
                                    {
                                        NavManager.NavigateTo($"{NavManager.BaseUri}login", true);
                                    }

                                    // Set user permissions
                                    HasViewRights = _enumerable.Any(claim => claim.Type == "Permission" && claim.Value == "ViewRequisitions");
                                    HasEditRights = _enumerable.Any(claim => claim.Type == "Permission" && claim.Value == "CreateOrEditCompany");
                                }

                                if (Start.APIHost.NullOrWhiteSpace())
                                {
                                    Start.APIHost = Configuration[NavManager.BaseUri.Contains("localhost") ? "APIHost" : "APIHostServer"];
                                }

                                //General.CheckStart(NavManager, Configuration);
                                //LoginCookyUser = await NavManager.RedirectInner(LocalStorage, Crypto);

                                List<string> _keys =
                                [
                                    nameof(CacheObjects.Roles), nameof(CacheObjects.States), nameof(CacheObjects.Eligibility), nameof(CacheObjects.Education),
                                    nameof(CacheObjects.Experience), nameof(CacheObjects.JobOptions), nameof(CacheObjects.Users), nameof(CacheObjects.Skills),
                                    nameof(CacheObjects.StatusCodes), nameof(CacheObjects.Preferences), nameof(CacheObjects.Companies), nameof(CacheObjects.Workflow),
                                    nameof(CacheObjects.CompanyContacts)
                                ];

                                Dictionary<string, string> _cacheValues = await RedisService.BatchGet(_keys);

                                //_roles = General.DeserializeObject<List<Role>>(_cacheValues[CacheObjects.Roles.ToString()]); //await Redis.GetAsync<List<Role>>("Roles");

                                _states = General.DeserializeObject<List<StateCache>>(_cacheValues[nameof(CacheObjects.States)]);
                                _eligibility = General.DeserializeObject<List<IntValues>>(_cacheValues[nameof(CacheObjects.Eligibility)]);
                                _education = General.DeserializeObject<List<IntValues>>(_cacheValues[nameof(CacheObjects.Education)]);
                                _experience = General.DeserializeObject<List<IntValues>>(_cacheValues[nameof(CacheObjects.Experience)]);
                                _jobOptions = General.DeserializeObject<List<JobOptions>>(_cacheValues[nameof(CacheObjects.JobOptions)]);

                                if (_recruiters == null)
                                {
                                    List<UserList> _users = General.DeserializeObject<List<UserList>>(_cacheValues[nameof(CacheObjects.Users)]);

                                    if (_users != null)
                                    {
                                        _recruiters = _users
                                                     .Where(user => user.Role is 2 or 4 or 5 or 6)
                                                     .Select(user => new KeyValues {KeyValue = user.UserName, Text = user.UserName})
                                                     .ToList();
                                    }
                                }

                                Skills = General.DeserializeObject<List<IntValues>>(_cacheValues[nameof(CacheObjects.Skills)]);

                                _statusCodes = General.DeserializeObject<List<StatusCode>>(_cacheValues[nameof(CacheObjects.StatusCodes)]);
                                _preference = General.DeserializeObject<Preferences>(_cacheValues[nameof(CacheObjects.Preferences)]);

                                if (_statusCodes is {Count: > 0})
                                {
                                    _search = _statusCodes.Where(statusCode => statusCode.AppliesToCode == "REQ")
                                                          .Select(statusCode => statusCode.Status)
                                                          .ToList();
                                    /*_statusSearch = _statusCodes.Where(statusCode => statusCode.AppliesToCode == "REQ")
                                                                .Select(statusCode => new KeyValues {Text = statusCode.Status, KeyValue = statusCode.Code})
                                                                .ToList();*/
                                }

                                List<CompaniesList> _companyList = General.DeserializeObject<List<CompaniesList>>(_cacheValues[nameof(CacheObjects.Companies)]);

                                Companies = [];

                                if (_companyList?.Count > 0)
                                {
                                    IEnumerable<Company> _filtered = _companyList.Where(c => c.UpdatedBy == User || c.UpdatedBy == "ADMIN")
                                                                                 .Select(c => new Company
                                                                                              {
                                                                                                  ID = c.ID, CompanyName = c.CompanyName
                                                                                              });

                                    Companies.AddRange(_filtered);
                                }

                                List<CompanyContacts> _companyContacts = General.DeserializeObject<List<CompanyContacts>>(_cacheValues[nameof(CacheObjects.CompanyContacts)]);
                                if (_companyContacts?.Count > 0)
                                {
                                    CompanyContacts.AddRange(_companyContacts.Select(c => new CompanyContacts
                                                                                          {
                                                                                              CompanyID = c.CompanyID, ID = c.ID, ContactName = c.ContactName
                                                                                          }));
                                }

                                _workflows = General.DeserializeObject<List<Workflow>>(_cacheValues[nameof(CacheObjects.Workflow)]);
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

    private async Task SearchRequisition(EditContext arg)
    {
        SearchModel = SearchModelClone.Copy();
        await Task.WhenAll(SaveStorage(), SetDataSource()).ConfigureAwait(false);
    }

    private async Task SetDataSource()
    {
        Dictionary<string, string> _parameters = new()
                                                 {
                                                     {"getCompanyInformation", Companies.Count.Equals(0).ToBooleanString()},
                                                     {"requisitionID", RequisitionID.ToString()},
                                                     {"thenProceed", false.ToString()},
                                                     {"user", User}
                                                 };
        (Count, string _requisitions, string _, string _, string _status, int _) =
            await General.ExecuteRest<ReturnGridRequisition>("Requisition/GetGridRequisitions", _parameters, SearchModel, false).ConfigureAwait(false);
        DataSource = Count > 0 ? JsonConvert.DeserializeObject<List<Requisition>>(_requisitions) : [];

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
}