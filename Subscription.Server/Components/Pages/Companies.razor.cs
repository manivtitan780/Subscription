﻿#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.Server
// File Name:           Companies.razor.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu, Brijesh Dubey
// Created On:          02-06-2025 19:02
// Last Updated On:     05-20-2025 20:11
// *****************************************/

#endregion

namespace Subscription.Server.Components.Pages;

public partial class Companies
{
    private const string StorageName = "CompaniesGrid";

    // private static TaskCompletionSource<bool> _initializationTaskSource;
    private List<CompanyContacts> _companyContacts = [];

    private CompanyDetails _companyDetails = new(), _companyDetailsClone = new();
    private List<CompanyDocuments> _companyDocuments = []; /**/
    private List<CompanyLocations> _companyLocations = [];
    private List<Requisition> _companyRequisitions = [];

    private int _selectedRowIndex = -1;
    private int _selectedTab;
    private readonly SemaphoreSlim _semaphoreMainPage = new(1, 1);

    private Company _target;

    private MarkupString Address { get; set; }

    private EditContact CompanyContactDialog { get; set; }

    private EditCompany CompanyEditDialog { get; set; }

    private EditLocation CompanyLocationDialog { get; set; }

    [Inject]
    private IConfiguration Configuration { get; set; }

    private int Count { get; set; }

    private List<Company> DataSource { get; set; }

    private AddCompanyDocument DialogDocument { get; set; }

    private DocumentPanel DocumentPanel { get; set; }

    public EditContext EditConCompany { get; set; }

    public EditContext EditConContact { get; set; }

    public EditContext EditConLocation { get; set; }

    private SfGrid<Company> Grid { get; set; }

    private bool HasEditRights { get; set; }

    private bool HasRendered { get; set; }

    private bool HasViewRights { get; set; }

    [Inject]
    private IJSRuntime JsRuntime { get; set; }

    [Inject]
    private ILocalStorageService LocalStorage { get; set; }

    private List<IntValues> NAICS { get; set; } = [];

    [Inject]
    private NavigationManager NavManager { get; set; }

    private CompanyDocuments NewDocument { get; } = new();

    private ContactPanel PanelContacts { get; set; }

    private LocationPanel PanelLocations { get; set; }

    [Inject]
    private RedisService RedisService { get; set; }

    private int RoleID { get; } = 5;

    private string RoleName { get; set; }

    private List<IntValues> Roles { get; set; } = [];

    private CompanySearch SearchModel { get; set; } = new();

    private CompanyContacts SelectedContact { get; set; } = new();

    private CompanyDocuments SelectedDownload { get; set; }

    private CompanyLocations SelectedLocation { get; set; } = new();

    [Inject]
    private ISessionStorageService SessionStorage { get; set; }

    private SfSpinner Spinner { get; set; } = new();

    private List<StateCache> State { get; set; } = [];

    private string Title { get; set; } = "Edit";

    private string User { get; set; }

    private bool VisibleSpinner { get; set; }

    private Task AddDocument() => ExecuteMethod(() =>
                                                {
                                                    NewDocument.Clear();
                                                    return DialogDocument.ShowDialog();
                                                });

    private Task AllAlphabets(MouseEventArgs args) => ExecuteMethod(async () =>
                                                                    {
                                                                        SearchModel.CompanyName = "";
                                                                        SearchModel.Page = 1;
                                                                        await Task.WhenAll(SaveStorage(), SetDataSource()).ConfigureAwait(false);
                                                                    });

    private Task AutocompleteValueChange(ChangeEventArgs<string, KeyValues> filter) => ExecuteMethod(async () =>
                                                                                                     {
                                                                                                         SearchModel.CompanyName = filter.Value;
                                                                                                         SearchModel.Page = 1;
                                                                                                         await Task.WhenAll(SaveStorage(), SetDataSource()).ConfigureAwait(false);
                                                                                                     });

    private Task ClearFilter() => ExecuteMethod(async () =>
                                                {
                                                    SearchModel.Clear();
                                                    SearchModel.User = User;
                                                    await Task.WhenAll(SaveStorage(), SetDataSource()).ConfigureAwait(false);
                                                });

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

    private Task DeleteCompanyDocument(int arg) => ExecuteMethod(async () =>
                                                                 {
                                                                     _companyDocuments = await General.ExecuteAndDeserialize<CompanyDocuments>("Company/DeleteCompanyDocument",
                                                                                                                                               new() {{"documentID", arg.ToString()}, {"user", User}})
                                                                                                      .ConfigureAwait(false);
                                                                 });

    private Task DetailDataBind(DetailDataBoundEventArgs<Company> company) => ExecuteMethod(async () =>
                                                                                            {
                                                                                                int _index = await Grid.GetRowIndexByPrimaryKeyAsync(company.Data.ID);
                                                                                                if (_index != Grid.SelectedRowIndex)
                                                                                                {
                                                                                                    await Grid.SelectRowAsync(_index);
                                                                                                }

                                                                                                await Grid.CollapseAllDetailRowAsync();
                                                                                                await Grid.ExpandCollapseDetailRowAsync(company.Data);
                                                                                                _target = company.Data;

                                                                                                VisibleSpinner = true;

                                                                                                Dictionary<string, string> _parameters = new()
                                                                                                                                         {
                                                                                                                                             {"companyID", _target.ID.ToString()},
                                                                                                                                             {"user", User}
                                                                                                                                         };
                                                                                                (string _company, string _contacts, string _locations, string _documents, string _requisitions) =
                                                                                                    await General.ExecuteRest<ReturnCompanyDetails>("Company/GetCompanyDetails", _parameters, null,
                                                                                                                                                    false);

                                                                                                EditConCompany = new(_companyDetails);
                                                                                                try
                                                                                                {
                                                                                                    _companyDetails = General.DeserializeObject<CompanyDetails>(_company);
                                                                                                    _companyContacts = General.DeserializeObject<List<CompanyContacts>>(_contacts) ?? [];
                                                                                                    _companyDocuments = General.DeserializeObject<List<CompanyDocuments>>(_documents) ?? [];
                                                                                                    _companyLocations = General.DeserializeObject<List<CompanyLocations>>(_locations) ?? [];
                                                                                                    _companyRequisitions = General.DeserializeObject<List<Requisition>>(_requisitions) ?? [];
                                                                                                    SetupAddress();
                                                                                                }
                                                                                                catch (Exception ex)
                                                                                                {
                                                                                                    Console.WriteLine(ex.Message);
                                                                                                }

                                                                                                _selectedTab = 0;

                                                                                                VisibleSpinner = false;
                                                                                            });

    private Task DownloadDocument(int arg) => ExecuteMethod(async () =>
                                                            {
                                                                SelectedDownload = DocumentPanel.SelectedRow;
                                                                string _queryString = $"{SelectedDownload.InternalFileName}^{_target.ID}^{SelectedDownload.FileName}^2".ToBase64String();
                                                                await JsRuntime.InvokeVoidAsync("open", $"{NavManager.BaseUri}Download/{_queryString}", "_blank");
                                                            });

    private Task EditCompany(bool isAdd = false) => ExecuteMethod(async () =>
                                                                  {
                                                                      VisibleSpinner = true;

                                                                      if (isAdd)
                                                                      {
                                                                          Title = "Add";
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
                                                                          Title = "Edit";
                                                                          _companyDetailsClone = _companyDetails.Copy();
                                                                          _companyDetailsClone.IsAdd = false;
                                                                      }

                                                                      VisibleSpinner = false;
                                                                      await CompanyEditDialog.ShowDialog();
                                                                  });

    private Task EditCompanyContact(int contact) => ExecuteMethod(async () =>
                                                                  {
                                                                      VisibleSpinner = true;
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

                                                                      EditConContact = new(SelectedContact!);
                                                                      VisibleSpinner = false;
                                                                      await CompanyContactDialog.ShowDialog();
                                                                  });

    private Task EditLocation(int location) => ExecuteMethod(async () =>
                                                             {
                                                                 if (location == 0)
                                                                 {
                                                                     VisibleSpinner = true;
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

                                                                 EditConLocation = new(SelectedLocation!);
                                                                 VisibleSpinner = false;
                                                                 await CompanyLocationDialog.ShowDialog();
                                                             });

    private Task ExecuteMethod(Func<Task> task) => General.ExecuteMethod(_semaphoreMainPage, task);

    public static string FormatDUNS(string input)
    {
        if (input.NullOrWhiteSpace() || input.Length != 9)
        {
            return input;
        }

        return $"{input[..2]}-{input.Substring(2, 3)}-{input[5..]}";
    }

    public static string FormatEIN(string input)
    {
        if (input.NullOrWhiteSpace() || input.Length != 9)
        {
            return input;
        }

        return $"{input[..2]}-{input[2..]}";
    }

    private async Task GetAlphabets(char alphabet) => await ExecuteMethod(async () =>
                                                                          {
                                                                              SearchModel.CompanyName = alphabet.ToString();
                                                                              SearchModel.Page = 1;
                                                                              await Task.WhenAll(SaveStorage(), SetDataSource()).ConfigureAwait(false);
                                                                          });

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            if (await SessionStorage.ContainKeyAsync(StorageName))
            {
                SearchModel = await SessionStorage.GetItemAsync<CompanySearch>(StorageName);
            }
            else
            {
                SearchModel.Clear();
            }

            await Task.WhenAll(SaveStorage(), SetDataSource()).ConfigureAwait(false);
            await Task.Delay(100);
        }
    }

    protected override async Task OnInitializedAsync()
    {
        await ExecuteMethod(async () =>
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

                                    RoleName = _enumerable.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value.ToUpperInvariant();
                                    HasViewRights = _enumerable.Any(claim => claim.Type == "Permission" && claim.Value == "ViewAllCompanies");
                                    HasEditRights = _enumerable.Any(claim => claim.Type == "Permission" && claim.Value == "CreateOrEditRequisitions");
                                }

                                if (Start.APIHost.NullOrWhiteSpace())
                                {
                                    Start.APIHost = Configuration[NavManager.BaseUri.Contains("localhost") ? "APIHost" : "APIHostServer"];
                                }

                                if (NAICS.Count == 0 && State.Count == 0 && Roles.Count == 0)
                                {
                                    List<string> _keys = [nameof(CacheObjects.NAICS), nameof(CacheObjects.States), nameof(CacheObjects.Roles)];

                                    Dictionary<string, string> _values = await RedisService.BatchGet(_keys);
                                    NAICS = General.DeserializeObject<List<IntValues>>(_values[nameof(CacheObjects.NAICS)]);
                                    State = General.DeserializeObject<List<StateCache>>(_values[nameof(CacheObjects.States)]);
                                    Roles = General.DeserializeObject<List<IntValues>>(_values[nameof(CacheObjects.Roles)]);
                                }
                            });

        await base.OnInitializedAsync();
    }

    private async Task PageChanged(PageChangedEventArgs page)
    {
        SearchModel.Page = page.CurrentPage;
        await Task.WhenAll(SaveStorage(), SetDataSource()).ConfigureAwait(false);
    }

    private async Task PageSizeChanged(PageSizeChangedArgs page)
    {
        SearchModel.ItemCount = page.CurrentPageSize;
        SearchModel.Page = 1;
        await Task.WhenAll(SaveStorage(), SetDataSource()).ConfigureAwait(false);
    }

    private async Task Refresh(MouseEventArgs arg) => await SetDataSource().ConfigureAwait(false);

    private async Task RowSelected(RowSelectEventArgs<Company> company)
    {
        if (_selectedRowIndex != -1 && _selectedRowIndex != company.RowIndex)
        {
            await Grid.CollapseAllDetailRowAsync();
            await Grid.ExpandCollapseDetailRowAsync(company.Data);
        }

        _selectedRowIndex = company.RowIndex;
    }

    private async Task SaveCompany() => await ExecuteMethod(async () =>
                                                            {
                                                                // Dictionary<string, string> _parameters = new() {{"user", User}};

                                                                (string _company, _, string _locations, _, _) = await General.ExecuteRest<ReturnCompanyDetails>("Company/SaveCompany", UserParameters(),
                                                                                                                                                                _companyDetailsClone);
                                                                _companyDetails = General.DeserializeObject<CompanyDetails>(_company);
                                                                _companyLocations = General.DeserializeObject<List<CompanyLocations>>(_locations);

                                                                if (_target != null)
                                                                {
                                                                    /* This will work only if the columns are template else this will fail without warning. */
                                                                    if (_companyDetails != null)
                                                                    {
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
                                                                }
                                                                else
                                                                {
                                                                    await SetDataSource().ConfigureAwait(false);
                                                                }

                                                                StateHasChanged();
                                                            });

    private async Task SaveContact() => await ExecuteMethod(async () =>
                                                            {
                                                                // Dictionary<string, string> _parameters = new() {{"user", User}};
                                                                string _response = await General.ExecuteRest<string>("Company/SaveCompanyContact", UserParameters(), SelectedContact);

                                                                _companyContacts = General.DeserializeObject<List<CompanyContacts>>(_response);

                                                                if (_target == null)
                                                                {
                                                                    await SetDataSource().ConfigureAwait(false);
                                                                }
                                                            });

    private Task SaveDocument(EditContext document) => ExecuteMethod(async () =>
                                                                     {
                                                                         if (document.Model is CompanyDocuments _document)
                                                                         {
                                                                             Dictionary<string, string> _parameters = new()
                                                                                                                      {
                                                                                                                          {"filename", DialogDocument.FileName},
                                                                                                                          {"mime", DialogDocument.Mime},
                                                                                                                          {"name", _document.DocumentName},
                                                                                                                          {"notes", _document.Notes},
                                                                                                                          {"companyID", _target.ID.ToString()},
                                                                                                                          {"user", User}
                                                                                                                      };

                                                                             string _response = await General.ExecuteRest<string>("Company/UploadDocument", _parameters, null, true,
                                                                                                                                  DialogDocument.AddedDocument.ToStreamByteArray(),
                                                                                                                                  DialogDocument.FileName);

                                                                             _companyDocuments = General.DeserializeObject<List<CompanyDocuments>>(_response);
                                                                         }
                                                                     });

    private async Task SaveLocation() => await ExecuteMethod(async () =>
                                                             {
                                                                 string _response = await General.ExecuteRest<string>("Company/SaveCompanyLocation", UserParameters(), SelectedLocation);

                                                                 _companyLocations = General.DeserializeObject<List<CompanyLocations>>(_response);

                                                                 if (_target != null)
                                                                 {
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
                                                                     await SetDataSource().ConfigureAwait(false);
                                                                 }
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
        (string _data, Count) = await General.ExecuteRest<ReturnGrid>("Company/GetGridCompanies", null, SearchModel, false);
        DataSource = Count > 0 ? General.DeserializeObject<List<Company>>(_data) : [];
        await Grid.Refresh().ConfigureAwait(false);
    }

    private void SetupAddress(bool useLocation = false)
    {
        List<string> _parts = [];
        if (!useLocation)
        {
            if (_companyDetails.StreetName.NotNullOrWhiteSpace())
            {
                _parts.Add(_companyDetails.StreetName);
            }

            if (_companyDetails.City.NotNullOrWhiteSpace())
            {
                _parts.Add(_companyDetails.City);
            }

            if (_companyDetails.StateID > 0)
            {
                StateCache _state = State.FirstOrDefault(state => state.KeyValue == _companyDetails.StateID);

                if (_state is {Text: not null})
                {
                    _parts.Add($"<strong>{_state.Text}</strong>");
                }
            }

            if (_companyDetails.ZipCode.NotNullOrWhiteSpace())
            {
                _parts.Add(_companyDetails.ZipCode);
            }
        }
        else
        {
            CompanyLocations _loc = _companyLocations.FirstOrDefault(location => location.PrimaryLocation);
            if (_loc != null)
            {
                if (_loc.StreetName.NotNullOrWhiteSpace())
                {
                    _parts.Add(_loc.StreetName);
                }

                if (_loc.City.NotNullOrWhiteSpace())
                {
                    _parts.Add(_loc.City);
                }

                if (_loc.StateID > 0)
                {
                    StateCache _state = State.FirstOrDefault(state => state.KeyValue == _loc.StateID);

                    if (_state is {Text: not null})
                    {
                        _parts.Add($"<strong>{_state.Text}</strong>");
                    }
                }

                if (_loc.ZipCode.NotNullOrWhiteSpace())
                {
                    _parts.Add(_loc.ZipCode);
                }
            }
            else
            {
                Address = "".ToMarkupString();
            }
        }

        if (_parts.Count > 0)
        {
            Address = string.Join(", ", _parts).ToMarkupString();
        }
    }

    private string SetupTargetAddress(bool useLocation = false)
    {
        List<string> _parts = [];
        if (!useLocation)
        {
            _parts.Add(_companyDetails.City);

            if (_companyDetails.StateID > 0)
            {
                StateCache _state = State.FirstOrDefault(state => state.KeyValue == _companyDetails.StateID);

                if (_state is {Code: not null})
                {
                    _parts.Add(_state.Code);
                }
            }

            if (_companyDetails.ZipCode != "")
            {
                _parts.Add(_companyDetails.ZipCode);
            }
        }
        else
        {
            CompanyLocations _loc = _companyLocations.FirstOrDefault(location => location.PrimaryLocation);
            if (_loc == null)
            {
                return "";
            }

            _parts.Add(_loc.City);

            if (_loc.StateID > 0)
            {
                StateCache _state = State.FirstOrDefault(state => state.KeyValue == _loc.StateID);

                if (_state is {Code: not null})
                {
                    _parts.Add(_state.Code);
                }
            }

            if (_loc.ZipCode != "")
            {
                _parts.Add(_loc.ZipCode);
            }
        }

        return _parts.Count > 0 ? string.Join(", ", _parts) : "";
    }

    private async Task SpeedDialItemClicked(SpeedDialItemEventArgs args)
    {
        switch (args.Item.ID)
        {
            case "itemEditCompany":
            {
                _selectedTab = 0;
                await EditCompany();
                break;
            }
            case "itemAddLocation":
            {
                _selectedTab = 1;
                await EditLocation(0);
                break;
            }
            case "itemAddContact":
            {
                _selectedTab = 2;
                await EditCompanyContact(0);
                break;
            }
            case "itemAddDocument":
            {
                _selectedTab = 3;
                await AddDocument();
                break;
            }
            case "itemEditAccount":
            {
                _selectedTab = 4;
                break;
            }
            case "itemManageRequisition":
            {
                _selectedTab = 5;
                RequisitionSearch _requisitionSearch = new();
                if (await SessionStorage.ContainKeyAsync("RequisitionGrid"))
                {
                    _requisitionSearch = await SessionStorage.GetItemAsync<RequisitionSearch>(StorageName);
                }

                int _itemCount = _requisitionSearch.ItemCount;
                _requisitionSearch.Clear();
                _requisitionSearch.ItemCount = _itemCount;
                _requisitionSearch.Company = _target.CompanyName;
                _requisitionSearch.Status = "";
                await SessionStorage.SetItemAsync("RequisitionGrid", _requisitionSearch);
                NavManager.NavigateTo($"{NavManager.BaseUri}requisition", true);
                
                break;
            }
        }
    }

    private void TabSelected(SelectEventArgs args) => _selectedTab = args.SelectedIndex;

    private Dictionary<string, string> UserParameters() => new() {{"user", User}};
}