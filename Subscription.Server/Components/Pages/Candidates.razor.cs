#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.Server
// File Name:           Candidates.razor.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu
// Created On:          05-01-2024 15:05
// Last Updated On:     05-11-2024 19:05
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
    private List<CandidateActivity> _candidateActivityObject = new();
    private CandidateDetails _candidateDetailsObject = new();
    private List<CandidateDocument> _candidateDocumentsObject = new();
    private List<CandidateEducation> _candidateEducationObject = new();
    private List<CandidateExperience> _candidateExperienceObject = new();
    private List<CandidateMPC> _candidateMPCObject = new();
    private List<CandidateNotes> _candidateNotesObject = new();
    private List<CandidateRating> _candidateRatingObject = new();
    private List<CandidateSkills> _candidateSkillsObject = new();
    private List<IntValues> _eligibility = [], _experience = [];
    private List<KeyValues> _jobOptions = [], _taxTerms = [];
    private List<Role> _roles;
    private int _selectedTab;

    private readonly SemaphoreSlim _semaphoreMainPage = new(1, 1);
    private List<IntValues> _states;

    private List<KeyValues> _statusCodes = [], _workflow = [], _communication = [], _documentTypes = [];

    private Candidate _target;
    private bool FormattedExists;
    private bool OriginalExists;
    private CandidateRatingMPC RatingMPC = new();

    private MarkupString Address
    {
        get;
        set;
    }

    private MarkupString CandidateCommunication
    {
        get;
        set;
    }

    /// <summary>
    ///     Gets or sets the eligibility status of the candidate.
    /// </summary>
    /// <remarks>
    ///     This property is used to store the eligibility status of a candidate. The eligibility status is determined based on
    ///     the `EligibilityID` of the candidate.
    ///     If the `EligibilityID` is greater than 0, the eligibility status is set to the corresponding value from the
    ///     `_eligibility` collection.
    ///     If the `EligibilityID` is not greater than 0, the eligibility status is set to an empty string.
    ///     This property is used in the `SetEligibility()` method and in the `BuildRenderTree()` method of the `Candidate`
    ///     component.
    /// </remarks>
    private MarkupString CandidateEligibility
    {
        get;
        set;
    }

    /// <summary>
    ///     Gets or sets the Candidate's experience.
    /// </summary>
    /// <remarks>
    ///     This property is used to store the Candidate's experience retrieved from the
    ///     `_candidateDetailsObject.ExperienceID`.
    ///     If the `ExperienceID` is greater than 0, it fetches the corresponding experience from the `_experience` collection.
    ///     Otherwise, it is set to an empty string.
    /// </remarks>
    private MarkupString CandidateExperience
    {
        get;
        set;
    }

    private MarkupString CandidateJobOptions
    {
        get;
        set;
    }

    private MarkupString CandidateTaxTerms
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

    private MarkupString MPCDate
    {
        get;
        set;
    }

    private MarkupString MPCNote
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

    private MarkupString RatingDate
    {
        get;
        set;
    }

    private MarkupString RatingNote
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
                                 try
                                 {
                                     if (Spinner != null)
                                     {
                                         await Spinner.ShowAsync();
                                     }
                                 }
                                 catch
                                 {
                                     //Ignore the error.
                                 }

                                 Dictionary<string, string> _parameters = new()
                                                                          {
                                                                              {"candidateID", _target.ID.ToString()},
                                                                              {"roleID", "RS"}
                                                                          };
                                 Dictionary<string, object> _response = await General.GetRest<Dictionary<string, object>>("Candidate/GetCandidateDetails", _parameters);

                                 if (_response != null)
                                 {
                                     _candidateDetailsObject = General.DeserializeObject<CandidateDetails>(_response["Candidate"]);
                                     _candidateSkillsObject = General.DeserializeObject<List<CandidateSkills>>(_response["Skills"]);
                                     _candidateEducationObject = General.DeserializeObject<List<CandidateEducation>>(_response["Education"]);
                                     _candidateExperienceObject = General.DeserializeObject<List<CandidateExperience>>(_response["Experience"]);
                                     _candidateActivityObject = General.DeserializeObject<List<CandidateActivity>>(_response["Activity"]);
                                     _candidateNotesObject = General.DeserializeObject<List<CandidateNotes>>(_response["Notes"]);
                                     _candidateRatingObject = General.DeserializeObject<List<CandidateRating>>(_response["Rating"]);
                                     _candidateMPCObject = General.DeserializeObject<List<CandidateMPC>>(_response["MPC"]);
                                     _candidateDocumentsObject = General.DeserializeObject<List<CandidateDocument>>(_response["Document"]);
                                     RatingMPC = General.DeserializeObject<CandidateRatingMPC>(_response["RatingMPC"]) ?? new();
                                     GetMPCDate();
                                     GetMPCNote();
                                     GetRatingDate();
                                     GetRatingNote();
                                     SetupAddress();
                                     SetCommunication();
                                     SetEligibility();
                                     SetJobOption();
                                     SetTaxTerm();
                                     SetExperience();
                                 }

                                 _selectedTab = _candidateActivityObject.Count > 0 ? 7 : 0;
                                 FormattedExists = _target.FormattedResume;
                                 OriginalExists = _target.OriginalResume;

                                 try
                                 {
                                     if (Spinner != null)
                                     {
                                         await Spinner.HideAsync();
                                     }
                                 }
                                 catch
                                 {
                                     //Ignore the error.
                                 }
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

    /// <summary>
    ///     Retrieves the most recent date from the CandidateMPC list and converts it to a MarkupString.
    ///     The method first checks if the MpcNotes property of the CandidateDetails object is empty.
    ///     If it is, an empty MarkupString is assigned to the MPCDate property.
    ///     Then, the method finds the CandidateMPC object with the latest date.
    ///     If such an object exists, the method formats its date and assigns it to the MPCDate property.
    /// </summary>
    private void GetMPCDate()
    {
        string _mpcDate = "";
        if (_candidateDetailsObject.MPCNotes == "")
        {
            MPCDate = _mpcDate.ToMarkupString();
        }

        CandidateMPC _candidateMPCObjectFirst = _candidateMPCObject.MaxBy(x => x.Date);
        if (_candidateMPCObjectFirst != null)
        {
            _mpcDate = $"{_candidateMPCObjectFirst.Date.CultureDate()} [{string.Concat(_candidateMPCObjectFirst.User.Where(char.IsLetter))}]";
        }

        MPCDate = _mpcDate.ToMarkupString();
    }

    /// <summary>
    ///     The GetMPCNote method is responsible for retrieving the most recent note from the CandidateMPC object list.
    ///     If the MpcNotes property of the _candidateDetailsObject is empty, an empty string is converted to a MarkupString
    ///     and assigned to the MPCNote property.
    ///     The method then finds the CandidateMPC object with the latest date and assigns its Comments property to the
    ///     _mpcNote variable.
    ///     Finally, the _mpcNote is converted to a MarkupString and assigned to the MPCNote property.
    /// </summary>
    private void GetMPCNote()
    {
        string _mpcNote = "";
        if (_candidateDetailsObject.MPCNotes == "")
        {
            MPCNote = _mpcNote.ToMarkupString();
        }

        CandidateMPC _candidateMPCObjectFirst = _candidateMPCObject.MaxBy(x => x.Date);
        if (_candidateMPCObjectFirst != null)
        {
            _mpcNote = _candidateMPCObjectFirst.Comments;
        }

        MPCNote = _mpcNote.ToMarkupString();
    }

    /// <summary>
    ///     Retrieves the rating date for the candidate.
    /// </summary>
    /// <remarks>
    ///     This method fetches the maximum (latest) date from the candidate's rating list and formats it into a string.
    ///     The formatted string includes the date and the user's initials. If the candidate's rating notes are empty,
    ///     an empty string is returned. The result is converted into a MarkupString and stored in the RatingDate property.
    /// </remarks>
    private void GetRatingDate()
    {
        string _ratingDate = "";
        if (_candidateDetailsObject.RateNotes == "")
        {
            RatingDate = _ratingDate.ToMarkupString();
        }

        CandidateRating _candidateRatingObjectFirst = _candidateRatingObject.MaxBy(x => x.Date);
        if (_candidateRatingObjectFirst != null)
        {
            _ratingDate =
                $"{_candidateRatingObjectFirst.Date.CultureDate()} [{string.Concat(_candidateRatingObjectFirst.User.Where(char.IsLetter))}]";
        }

        RatingDate = _ratingDate.ToMarkupString();
    }

    /// <summary>
    ///     Retrieves the rating note for a candidate.
    /// </summary>
    /// <remarks>
    ///     This method checks if the candidate's rating notes are empty. If they are, it sets the RatingNote property to an
    ///     empty string.
    ///     If the candidate has rating notes, it retrieves the most recent rating note based on the date and sets the
    ///     RatingNote property to this value.
    ///     The RatingNote property is then converted to a MarkupString for display purposes.
    /// </remarks>
    private void GetRatingNote()
    {
        string _ratingNote = "";
        if (_candidateDetailsObject.RateNotes == "")
        {
            RatingNote = _ratingNote.ToMarkupString();
        }

        CandidateRating _candidateRatingObjectFirst = _candidateRatingObject.MaxBy(x => x.Date);
        if (_candidateRatingObjectFirst != null)
        {
            _ratingNote = _candidateRatingObjectFirst.Comments;
        }

        RatingNote = _ratingNote.ToMarkupString();
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
        _initializationTaskSource = new();

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

                                    HasViewRights = _enumerable.Any(claim => claim.Type == "Permission" && claim.Value == "ViewAllCompanies");
                                }

                                if (Start.APIHost.NullOrWhiteSpace())
                                {
                                    Start.APIHost = Configuration[NavManager.BaseUri.Contains("localhost") ? "APIHost" : "APIHostServer"];
                                }

                                List<string> _keys =
                                [
                                    CacheObjects.Roles.ToString(), CacheObjects.States.ToString(), CacheObjects.Eligibility.ToString(), CacheObjects.Experience.ToString(),
                                    CacheObjects.TaxTerms.ToString(), CacheObjects.JobOptions.ToString(), CacheObjects.StatusCodes.ToString(), CacheObjects.Workflow.ToString(),
                                    CacheObjects.Communication.ToString(), CacheObjects.DocumentTypes.ToString()
                                ];

                                RedisService _service = new(Start.CacheServer, Start.CachePort.ToInt32(), Start.Access, false);

                                Dictionary<string, string> _cacheValues = await _service.BatchGet(_keys);

                                _roles = General.DeserializeObject<List<Role>>(_cacheValues[CacheObjects.Roles.ToString()]);
                                _states = General.DeserializeObject<List<IntValues>>(_cacheValues[CacheObjects.States.ToString()]);
                                _eligibility = General.DeserializeObject<List<IntValues>>(_cacheValues[CacheObjects.Eligibility.ToString()]);
                                _experience = General.DeserializeObject<List<IntValues>>(_cacheValues[CacheObjects.Experience.ToString()]);
                                _taxTerms = General.DeserializeObject<List<KeyValues>>(_cacheValues[CacheObjects.TaxTerms.ToString()]);
                                _jobOptions = General.DeserializeObject<List<KeyValues>>(_cacheValues[CacheObjects.JobOptions.ToString()]);
                                _statusCodes = General.DeserializeObject<List<KeyValues>>(_cacheValues[CacheObjects.StatusCodes.ToString()]);
                                _workflow = General.DeserializeObject<List<KeyValues>>(_cacheValues[CacheObjects.Workflow.ToString()]);
                                _communication = General.DeserializeObject<List<KeyValues>>(_cacheValues[CacheObjects.Communication.ToString()]);
                                _documentTypes = General.DeserializeObject<List<KeyValues>>(_cacheValues[CacheObjects.DocumentTypes.ToString()]);

                                SearchModel.Clear();
                            });
        
        _initializationTaskSource.SetResult(true);

        await base.OnInitializedAsync();
    }

    /// <summary>
    ///     Sets the communication rating of the candidate.
    /// </summary>
    /// <remarks>
    ///     This method retrieves the communication rating from the candidate details object and converts it to a more
    ///     descriptive string.
    ///     The conversion is as follows:
    ///     - "G" => "Good"
    ///     - "A" => "Average"
    ///     - "X" => "Excellent"
    ///     - Any other value => "Fair"
    ///     The resulting string is then assigned to the CandidateCommunication property.
    /// </remarks>
    private void SetCommunication()
    {
        string _returnValue = _candidateDetailsObject.Communication switch
                              {
                                  "G" => "Good",
                                  "A" => "Average",
                                  "X" => "Excellent",
                                  _ => "Fair"
                              };

        CandidateCommunication = _returnValue.ToMarkupString();
    }

    /// <summary>
    ///     Sets the eligibility status of the candidate.
    /// </summary>
    /// <remarks>
    ///     This method checks if the eligibility list has any items. If it does, it sets the CandidateEligibility property to
    ///     the eligibility value of the candidate details object if it exists. If the eligibility ID of the candidate details
    ///     object is not greater than 0, it sets the CandidateEligibility property to an empty string.
    /// </remarks>
    private void SetEligibility()
    {
        if (_eligibility is {Count: > 0})
        {
            CandidateEligibility = _candidateDetailsObject.EligibilityID > 0
                                       ? _eligibility.FirstOrDefault(eligibility => eligibility.Value == _candidateDetailsObject.EligibilityID)!.Text.ToMarkupString()
                                       : "".ToMarkupString();
        }
    }

    /// <summary>
    ///     Sets the experience of the candidate.
    /// </summary>
    /// <remarks>
    ///     This method checks if the experience list is not null and has more than zero elements.
    ///     If the candidate's ExperienceID is greater than zero, it sets the CandidateExperience
    ///     to the corresponding experience value from the experience list.
    ///     If the ExperienceID is not greater than zero, it sets the CandidateExperience to an empty string.
    /// </remarks>
    private void SetExperience()
    {
        if (_experience is {Count: > 0})
        {
            CandidateExperience = _candidateDetailsObject.ExperienceID > 0
                                      ? _experience.FirstOrDefault(experience => experience.Value == _candidateDetailsObject.ExperienceID)!.Text.ToMarkupString()
                                      : "".ToMarkupString();
        }
    }

    /// <summary>
    ///     Sets the job options for the candidate.
    /// </summary>
    /// <remarks>
    ///     This method performs the following steps:
    ///     - Checks if the job options list is not null and has more than zero elements.
    ///     - Splits the job options from the candidate details object by comma.
    ///     - Iterates through each split job option.
    ///     - If the split job option is not an empty string, it finds the corresponding job option in the job options list and
    ///     appends it to the return value.
    ///     - Finally, it converts the return value to a markup string and sets it as the candidate's job options.
    /// </remarks>
    private void SetJobOption()
    {
        string _returnValue = "";
        if (_jobOptions is {Count: > 0})
        {
            string[] _splitJobOptions = _candidateDetailsObject.JobOptions.Split(',');
            foreach (string _str in _splitJobOptions)
            {
                if (_str == "")
                {
                    continue;
                }

                if (_returnValue != "")
                {
                    _returnValue += ", " + _jobOptions.FirstOrDefault(jobOption => jobOption.Key == _str)?.Value;
                }
                else
                {
                    _returnValue = _jobOptions.FirstOrDefault(jobOption => jobOption.Key == _str)?.Value;
                }
            }
        }

        CandidateJobOptions = _returnValue.ToMarkupString();
    }

    /// <summary>
    ///     Sets the tax terms for the candidate.
    /// </summary>
    /// <remarks>
    ///     This method performs the following steps:
    ///     - Checks if the tax terms list is not null and has more than zero items.
    ///     - Splits the candidate's tax term string by comma.
    ///     - Iterates through each split tax term.
    ///     - If the tax term is not an empty string, it finds the corresponding tax term from the tax terms list and appends
    ///     it to the return value.
    ///     - Sets the `CandidateTaxTerms` property with the return value converted to a markup string.
    /// </remarks>
    private void SetTaxTerm()
    {
        string _returnValue = "";

        if (_taxTerms is {Count: > 0})
        {
            string[] _splitTaxTerm = _candidateDetailsObject.TaxTerm.Split(',');
            foreach (string _str in _splitTaxTerm)
            {
                if (_str == "")
                {
                    continue;
                }

                if (_returnValue != "")
                {
                    _returnValue += ", " + _taxTerms.FirstOrDefault(taxTerm => taxTerm.Key == _str)?.Value;
                }
                else
                {
                    _returnValue = _taxTerms.FirstOrDefault(taxTerm => taxTerm.Key == _str)?.Value;
                }
            }
        }

        CandidateTaxTerms = _returnValue.ToMarkupString();
    }

    /// <summary>
    ///     Sets up the address for the candidate by concatenating the address fields.
    /// </summary>
    /// <remarks>
    ///     This method concatenates the Address1, Address2, City, StateID, and ZipCode fields of the candidate's details.
    ///     Each part of the address is separated by a comma or a line break.
    ///     If a part of the address is empty, it is skipped.
    ///     If the generated address starts with a comma, it is removed.
    ///     The final address is converted to a markup string and stored in the Address field.
    /// </remarks>
    private void SetupAddress()
    {
        string _generateAddress = _candidateDetailsObject.Address1;

        if (_generateAddress == "")
        {
            _generateAddress = _candidateDetailsObject.Address2;
        }
        else
        {
            _generateAddress += _candidateDetailsObject.Address2 == "" ? "" : "<br/>" + _candidateDetailsObject.Address2;
        }

        if (_generateAddress == "")
        {
            _generateAddress = _candidateDetailsObject.City;
        }
        else
        {
            _generateAddress += _candidateDetailsObject.City == "" ? "" : "<br/>" + _candidateDetailsObject.City;
        }

        if (_candidateDetailsObject.StateID > 0)
        {
            if (_generateAddress == "")
            {
                _generateAddress = _states.FirstOrDefault(state => state.Value == _candidateDetailsObject.StateID)?.Text?.Split('-')[0].Trim();
            }
            else
            {
                try //Because sometimes the default values are not getting set. It's so random that it can't be debugged. And it never fails during debugging session.
                {
                    _generateAddress += ", " + _states.FirstOrDefault(state => state.Value == _candidateDetailsObject.StateID)?.Text?.Split('-')[0].Trim();
                }
                catch
                {
                    //
                }
            }
        }

        if (_candidateDetailsObject.ZipCode != "")
        {
            if (_generateAddress == "")
            {
                _generateAddress = _candidateDetailsObject.ZipCode;
            }
            else
            {
                _generateAddress += ", " + _candidateDetailsObject.ZipCode;
            }
        }

        if (_generateAddress.StartsWith(","))
        {
            _generateAddress = _generateAddress[1..].Trim();
        }

        Address = _generateAddress.ToMarkupString();
    }

    private Task SpeedDialItemClicked(SpeedDialItemEventArgs arg) => null;

    private void TabSelected(SelectEventArgs tab) => _selectedTab = tab.SelectedIndex;

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