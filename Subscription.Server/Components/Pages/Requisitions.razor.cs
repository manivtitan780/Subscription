#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.Server
// File Name:           Requisitions.razor.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu, Brijesh Dubey
// Created On:          02-06-2025 19:02
// Last Updated On:     04-13-2025 15:04
// *****************************************/

#endregion

namespace Subscription.Server.Components.Pages;

public partial class Requisitions
{
    private const string StorageName = "RequisitionGrid";

    private List<CandidateActivity> _candActivityObject = [];
    private List<IntValues> _education = [], _eligibility = [], _experience = [], _states = [];
    private List<KeyValues> /*_companies = [], */ _jobOptions = [];

    private Preferences _preference;
    private List<KeyValues> _recruiters;
    private MarkupString _reqDetailSkills = string.Empty.ToMarkupString();
    private RequisitionDetails _reqDetailsObject = new(), _reqDetailsObjectClone = new();
    private List<RequisitionDocuments> _reqDocumentsObject = [];

    private int _selectedTab;
    private readonly SemaphoreSlim _semaphoreMainPage = new(1, 1);

    private List<StatusCode> _statusCodes;

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

    private int Page { get; set; } = 1;

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

    private SfSpinner Spinner { get; set; } = new();

    private SfSpinner SpinnerTop { get; set; }

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

    private Task AdvancedSearch(MouseEventArgs args) => ExecuteMethod(async () =>
                                                                      {
                                                                          SearchModelClone = SearchModel.Copy();
                                                                          await Task.CompletedTask;
                                                                          //await DialogSearch.ShowDialog();
                                                                      });

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

    private Task ClearFilter() => ExecuteMethod(async () =>
                                                {
                                                    SearchModel.Clear();
                                                    SearchModel.User = User;
                                                    await Task.WhenAll(SaveStorage(), SetDataSource()).ConfigureAwait(false);
                                                    //await Grid.Refresh();
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

    private async Task PageChanging(PageChangingEventArgs page)
    {
        Page = page.CurrentPage;
        SearchModel.Page = page.CurrentPage;
        await Task.WhenAll(SaveStorage(), SetDataSource()).ConfigureAwait(false);
    }

    private async Task PageSizeChanging(PageSizeChangingArgs page)
    {
        SearchModel.ItemCount = page.SelectedPageSize.ToInt32();
        SearchModel.Page = 1;
        await Grid.GoToPageAsync(1);
        await Task.WhenAll(SaveStorage(), SetDataSource()).ConfigureAwait(false);
    }

    private async Task Refresh(MouseEventArgs arg) => await SetDataSource().ConfigureAwait(false);

    private void RowSelected(RowSelectingEventArgs<Requisition> arg)
    {
        _target = arg.Data;
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

    private async Task SetDataSource()
    {
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

        Count = _count;
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