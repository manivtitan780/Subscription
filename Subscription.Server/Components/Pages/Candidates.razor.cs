#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.Server
// File Name:           Candidates.razor.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu
// Created On:          05-01-2024 15:05
// Last Updated On:     05-10-2024 19:05
// *****************************************/

#endregion

#region Using

using System.Security.Claims;

#endregion

namespace Subscription.Server.Components.Pages;

public partial class Candidates
{
    private const string StorageName = "CandidatesGrid";
    private static TaskCompletionSource<bool> _initializationTaskSource;

    private readonly SemaphoreSlim _semaphoreMainPage = new(1, 1);

    private Candidate _target;

    [Inject]
    private IConfiguration Configuration
    {
        get;
        set;
    }

    private static int Count
    {
        get;
        set;
    }

    private static SfGrid<Candidate> Grid
    {
        get;
        set;
    }

    private bool HasViewRights
    {
        get;
        set;
    } = true;

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

    public static CandidateSearch SearchModel
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

    private SfSpinner Spinner
    {
        get;
        set;
    }

    private string User
    {
        get;
        set;
    }

    private static async Task AllAlphabets()
    {
        SearchModel.Name = "";
        SearchModel.Page = 1;
        await Grid.Refresh();
    }

    private static async Task AutocompleteValueChange(ChangeEventArgs<string, KeyValues> filter)
    {
        SearchModel.Name = filter.Value;
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
                                                    DotNetObjectReference<Candidates> _dotNetReference = DotNetObjectReference.Create(this); // create dotnet ref
                                                    await JsRuntime.InvokeAsync<string>("detail", _dotNetReference);
                                                    //  send the dotnet ref to JS side
                                                    if (Grid.TotalItemCount > 0)
                                                    {
                                                        await Grid.SelectRowAsync(0);
                                                    }
                                                });

    private Task DetailDataBind(DetailDataBoundEventArgs<Candidate> candidate)
    {
        return ExecuteMethod(async () =>
                             {
                                 if (_target != null && _target != candidate.Data)
                                 {
                                     // return when target is equal to args.data
                                     await Grid.ExpandCollapseDetailRowAsync(_target);
                                 }

                                 int _index = await Grid.GetRowIndexByPrimaryKeyAsync(candidate.Data.ID);
                                 if (_index != Grid.SelectedRowIndex)
                                 {
                                     await Grid.SelectRowAsync(_index);
                                 }

                                 _target = candidate.Data;
                                 //try
                                 //{
                                 //    if (Spinner != null)
                                 //    {
                                 //        await Spinner.ShowAsync();
                                 //    }
                                 //}
                                 //catch
                                 //{
                                 //    //Ignore the error.
                                 //}

                                 //Dictionary<string, string> _parameters = new()
                                 //                                         {
                                 //                                             {"candidateID", _target.ID.ToString()},
                                 //                                             {"roleID", General.GetRoleID(LoginCookyUser)}
                                 //                                         };
                                 //Dictionary<string, object> _response = await General.GetRest<Dictionary<string, object>>("Candidates/GetCandidateDetails", _parameters);

                                 //if (_response != null)
                                 //{
                                 //    _candidateDetailsObject = General.DeserializeObject<CandidateDetails>(_response["Candidate"]);
                                 //    _candidateSkillsObject = General.DeserializeObject<List<CandidateSkills>>(_response["Skills"]);
                                 //    _candidateEducationObject = General.DeserializeObject<List<CandidateEducation>>(_response["Education"]);
                                 //    _candidateExperienceObject = General.DeserializeObject<List<CandidateExperience>>(_response["Experience"]);
                                 //    _candidateActivityObject = General.DeserializeObject<List<CandidateActivity>>(_response["Activity"]);
                                 //    _candidateNotesObject = General.DeserializeObject<List<CandidateNotes>>(_response["Notes"]);
                                 //    _candidateRatingObject = General.DeserializeObject<List<CandidateRating>>(_response["Rating"]);
                                 //    _candidateMPCObject = General.DeserializeObject<List<CandidateMPC>>(_response["MPC"]);
                                 //    _candidateDocumentsObject = General.DeserializeObject<List<CandidateDocument>>(_response["Document"]);
                                 //    RatingMPC = General.DeserializeObject<CandidateRatingMPC>(_response["RatingMPC"]) ?? new();
                                 //    GetMPCDate();
                                 //    GetMPCNote();
                                 //    GetRatingDate();
                                 //    GetRatingNote();
                                 //    SetupAddress();
                                 //    SetCommunication();
                                 //    SetEligibility();
                                 //    SetJobOption();
                                 //    SetTaxTerm();
                                 //    SetExperience();
                                 //}

                                 //_selectedTab = _candidateActivityObject.Count > 0 ? 7 : 0;
                                 //FormattedExists = _target.FormattedResume;
                                 //OriginalExists = _target.OriginalResume;

                                 //try
                                 //{
                                 //    if (Spinner != null)
                                 //    {
                                 //        await Spinner.HideAsync();
                                 //    }
                                 //}
                                 //catch
                                 //{
                                 //    //Ignore the error.
                                 //}
                             });
    }

    /// <summary>
    ///     Collapses the detail row in the Companies page grid view. This method is invoked from JavaScript.
    /// </summary>
    [JSInvokable("DetailCollapse")]
    public void DetailRowCollapse()
    {
        _target = null;
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
                                SearchModel.Name = alphabet.ToString();
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
                                SearchModel.Clear();
                                return Task.CompletedTask;
                            });
        _initializationTaskSource.SetResult(true);

        await base.OnInitializedAsync();
    }

    private Task SpeedDialItemClicked(SpeedDialItemEventArgs arg) => null;

    private Task TabSelected(SelectEventArgs arg) => null;

    public class CandidateAdaptor : DataAdaptor
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
                List<Candidate> _dataSource = [];

                object _candidateReturn = null;
                try
                {
                    Dictionary<string, object> _restResponse = await General.GetRest<Dictionary<string, object>>("Candidate/GetGridCandidates", null, SearchModel);

                    if (_restResponse == null)
                    {
                        _candidateReturn = dm.RequiresCounts ? new DataResult
                                                               {
                                                                   Result = _dataSource,
                                                                   Count = 0 /*_count*/
                                                               } : _dataSource;
                    }
                    else
                    {
                        //if (NAICS is not { Count: not 0 } || State is not { Count: not 0 } || Roles is not { Count: not 0 })
                        //{
                        //    RedisService _service = new(Start.CacheServer, Start.CachePort.ToInt32(), Start.Access, false);
                        //    List<string> _keys = [CacheObjects.NAICS.ToString(), CacheObjects.States.ToString(), CacheObjects.Roles.ToString()];

                        //    Dictionary<string, string> _values = await _service.BatchGet(_keys);
                        //    NAICS = JsonConvert.DeserializeObject<List<IntValues>>(_values["NAICS"] ?? string.Empty);
                        //    State = JsonConvert.DeserializeObject<List<IntValues>>(_values["States"] ?? string.Empty);
                        //    Roles = JsonConvert.DeserializeObject<List<IntValues>>(_values["Roles"] ?? string.Empty);
                        //}

                        _dataSource = JsonConvert.DeserializeObject<List<Candidate>>(_restResponse["Candidates"].ToString() ?? string.Empty);
                        int _count = _restResponse["Count"].ToInt32();
                        Count = _count;
                        if (_dataSource == null)
                        {
                            _candidateReturn = dm.RequiresCounts ? new DataResult
                                                                   {
                                                                       Result = null,
                                                                       Count = 1
                                                                   } : null;
                        }
                        else
                        {
                            _candidateReturn = dm.RequiresCounts ? new DataResult
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
                        _candidateReturn = dm.RequiresCounts ? new DataResult
                                                               {
                                                                   Result = null,
                                                                   Count = 1
                                                               } : null;
                    }
                    else
                    {
                        _dataSource.Add(new());

                        _candidateReturn = dm.RequiresCounts ? new DataResult
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

                return _candidateReturn;
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