#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.Server
// File Name:           Candidates.razor.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu, Brijesh Dubey
// Created On:          02-06-2025 19:02
// Last Updated On:     04-16-2025 16:04
// *****************************************/

#endregion

#region Using

using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;

using Role = Subscription.Model.Role;

#endregion

namespace Subscription.Server.Components.Pages;

public partial class Candidates
{
    private const string StorageName = "CandidatesGrid";
    private List<CandidateActivity> _candActivityObject = [];
    private CandidateDetails _candDetailsObject = new(), _candDetailsObjectClone = new();
    private List<CandidateDocument> _candDocumentsObject = [];
    private List<CandidateEducation> _candEducationObject = [];
    private List<CandidateExperience> _candExperienceObject = [];
    private List<CandidateNotes> _candidateNotesObject = [];
    private List<CandidateMPC> _candMPCObject = [];
    private List<CandidateRating> _candRatingObject = [];
    private List<CandidateSkills> _candSkillsObject = [];
    private List<IntValues> _eligibility = [], _experience = [], _states, _documentTypes = [];
    private bool _formattedExists, _originalExists;

    private List<KeyValues> _jobOptions = [], _taxTerms = [], _communication = [];

    private Query _query = new();

    //private CandidateRatingMPC _ratingMPC = new();
    private List<Role> _roles;
    private int _selectedTab;

    private readonly SemaphoreSlim _semaphoreMainPage = new(1, 1);
    private List<StatusCode> _statusCodes = [];
    private readonly SubmitCandidateRequisition _submitCandidateModel = new();

    private Candidate _target;

    private readonly List<ToolbarItemModel> _tools1 =
    [
        new() {Name = "Original", TooltipText = "Show Original Resume"},
        new() {Name = "Formatted", TooltipText = "Show Formatted Resume"}
    ];

    private List<Workflow> _workflow = [];

    private ActivityPanel ActivityPanel { get; set; }

    private MarkupString Address { get; set; }

    private MarkupString CandidateCommunication { get; set; }

    private EditCandidateDialog CandidateDialog { get; set; }

    private EditEducationDialog CandidateEducationDialog { get; set; }

    private MarkupString CandidateEligibility { get; set; }

    private MarkupString CandidateExperience { get; set; }

    private EditExperienceDialog CandidateExperienceDialog { get; set; }

    private MarkupString CandidateJobOptions { get; set; }

    private EditNotesDialog CandidateNotesDialog { get; set; }

    private EditSkillDialog CandidateSkillDialog { get; set; }

    private MarkupString CandidateTaxTerms { get; set; }

    [Inject]
    private IConfiguration Configuration { get; set; }

    private int Count { get; set; }

    private List<Candidate> DataSource { get; set; }

    private EditActivityDialog DialogActivity { get; set; }

    private AddDocumentDialog DialogDocument { get; set; }

    private bool DownloadFormatted { get; set; }

    private bool DownloadOriginal { get; set; }

    private DownloadsPanel DownloadsPanel { get; set; }

    public EditContext EditConEducation { get; set; }

    public EditContext EditConExperience { get; set; }

    public EditContext EditConNotes { get; set; }

    public EditContext EditConSkill { get; set; }

    private EducationPanel EducationPanel { get; set; }

    private ExperiencePanel ExperiencePanel { get; set; }

    private string FileName { get; set; }

    public bool FormattedExists { get; set; }

    private SfGrid<Candidate> Grid { get; set; }

    private bool HasEditRights { get; set; } = true;

    private bool HasRendered { get; set; }

    private bool HasViewRights { get; set; } = true;

    private bool IsFromCompany { get; set; }

    [Inject]
    private IJSRuntime JsRuntime { get; set; }

    [Inject]
    private ILocalStorageService LocalStorage { get; set; }

    private string Mime { get; set; }

    private MarkupString MPCDate { get; set; }

    private MPCCandidateDialog MPCDialog { get; set; }

    private MarkupString MPCNote { get; set; }

    [Inject]
    private NavigationManager NavManager { get; set; }

    private CandidateDocument NewDocument { get; } = new();

    private List<KeyValues> NextSteps { get; } = [];

    private NotesPanel NotesPanel { get; set; }

    public bool OriginalExists { get; set; }

    private int Page { get; set; } = 1;

    private MarkupString RatingDate { get; set; }

    private RatingCandidateDialog RatingDialog { get; set; }

    private CandidateRatingMPC RatingMPC { get; set; } = new();

    private MarkupString RatingNote { get; set; }

    [Inject]
    private RedisService RedisService { get; set; }

    private int RequisitionID { get; set; }

    private int RoleID { get; set; }

    private string RoleName { get; set; }

    private CandidateSearch SearchModel { get; set; } = new();

    private CandidateActivity SelectedActivity { get; set; } = new();

    private CandidateDocument SelectedDownload { get; set; } = new();

    private CandidateEducation SelectedEducation { get; set; } = new();

    private CandidateEducation SelectedEducationClone { get; set; } = new();

    private CandidateExperience SelectedExperience { get; set; } = new();

    private CandidateNotes SelectedNotes { get; set; } = new();

    private CandidateSkills SelectedSkill { get; set; } = new();

    [Inject]
    private ISessionStorageService SessionStorage { get; set; }

    private SkillPanel SkillPanel { get; set; }

    private SfSpinner Spinner { get; set; }

    private SubmitCandidate SubmitDialog { get; set; }

    private string User { get; set; }

    private bool VisibleSpinner { get; set; }

    private async Task AddCandidate(MouseEventArgs arg)
    {
        await Grid.SelectRowAsync(-1);
        await EditCandidate();
    }

    private Task AddDocument() => ExecuteMethod(() =>
                                                {
                                                    NewDocument.Clear();
                                                    return DialogDocument.ShowDialog();
                                                });

    private Task AllAlphabets(MouseEventArgs args) => ExecuteMethod(async () =>
                                                                    {
                                                                        SearchModel.Name = "";
                                                                        SearchModel.Page = 1;
                                                                        await Task.WhenAll(SaveStorage(), SetDataSource()).ConfigureAwait(false);
                                                                    });

    private Task AutocompleteValueChange(ChangeEventArgs<string, KeyValues> filter) => ExecuteMethod(async () =>
                                                                                                     {
                                                                                                         SearchModel.Name = filter.Value;
                                                                                                         SearchModel.Page = 1;
                                                                                                         await Task.WhenAll(SaveStorage(), SetDataSource()).ConfigureAwait(false);
                                                                                                     });

    private Task ClearFilter() => ExecuteMethod(async () =>
                                                {
                                                    SearchModel.Clear();
                                                    SearchModel.User = User;
                                                    await Task.WhenAll(SaveStorage(), SetDataSource()).ConfigureAwait(false);
                                                });

    private Dictionary<string, string> CreateParameters(int id) => new()
                                                                   {
                                                                       {"id", id.ToString()},
                                                                       {"candidateID", _target.ID.ToString()},
                                                                       {"user", User}
                                                                   };

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

    private Task DeleteDocument(int arg) => ExecuteMethod(async () =>
                                                          {
                                                              Dictionary<string, string> _parameters = new()
                                                                                                       {
                                                                                                           {"documentID", arg.ToString()},
                                                                                                           {"user", User}
                                                                                                       };

                                                              string _response = await General.ExecuteRest<string>("Candidate/DeleteCandidateDocument", _parameters);

                                                              _candDocumentsObject = General.DeserializeObject<List<CandidateDocument>>(_response);
                                                          });

    private Task DeleteEducation(int id) => ExecuteMethod(async () =>
                                                          {
                                                              Dictionary<string, string> _parameters = CreateParameters(id);
                                                              string _response = await General.ExecuteRest<string>("Candidate/DeleteEducation", _parameters);

                                                              _candEducationObject = General.DeserializeObject<List<CandidateEducation>>(_response);
                                                          });

    private Task DeleteExperience(int id) => ExecuteMethod(async () =>
                                                           {
                                                               Dictionary<string, string> _parameters = CreateParameters(id);
                                                               string _response = await General.ExecuteRest<string>("Candidate/DeleteExperience", _parameters);

                                                               _candExperienceObject = General.DeserializeObject<List<CandidateExperience>>(_response);
                                                           });

    private Task DeleteNotes(int id) => ExecuteMethod(async () =>
                                                      {
                                                          Dictionary<string, string> _parameters = CreateParameters(id);
                                                          string _response = await General.ExecuteRest<string>("Candidate/DeleteNotes", _parameters);

                                                          _candidateNotesObject = General.DeserializeObject<List<CandidateNotes>>(_response);
                                                      });

    private Task DeleteSkill(int id) => ExecuteMethod(async () =>
                                                      {
                                                          Dictionary<string, string> _parameters = CreateParameters(id);
                                                          string _response = await General.ExecuteRest<string>("Candidate/DeleteSkill", _parameters);

                                                          _candSkillsObject = General.DeserializeObject<List<CandidateSkills>>(_response);
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

                                 VisibleSpinner = true;

                                 Dictionary<string, string> _parameters = new()
                                                                          {
                                                                              {"candidateID", _target.ID.ToString()},
                                                                              {"roleID", "RS"}
                                                                          };
                                 (string _candidate, string _notes, string _skills, string _education, string _s, string _activity, List<CandidateRating> _candidateRatings,
                                  List<CandidateMPC> _candidateMPC, CandidateRatingMPC _candidateRatingMPC, string _documents) =
                                     await General.ExecuteRest<ReturnCandidateDetails>("Candidate/GetCandidateDetails", _parameters, null, false);

                                 _candDetailsObject = General.DeserializeObject<CandidateDetails>(_candidate) ?? new();
                                 _candSkillsObject = General.DeserializeObject<List<CandidateSkills>>(_skills) ?? [];
                                 _candEducationObject = General.DeserializeObject<List<CandidateEducation>>(_education) ?? [];
                                 _candExperienceObject = General.DeserializeObject<List<CandidateExperience>>(_s) ?? [];
                                 _candidateNotesObject = General.DeserializeObject<List<CandidateNotes>>(_notes) ?? [];
                                 _candDocumentsObject = General.DeserializeObject<List<CandidateDocument>>(_documents) ?? [];
                                 _candActivityObject = General.DeserializeObject<List<CandidateActivity>>(_activity) ?? [];

                                 _candRatingObject = _candidateRatings;
                                 _candMPCObject = _candidateMPC;
                                 RatingMPC = _candidateRatingMPC;
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

                                 _selectedTab = _candActivityObject is {Count: > 0} ? 7 : 0;
                                 _formattedExists = _target.FormattedResume;
                                 _originalExists = _target.OriginalResume;

                                 await Task.Delay(100);
                                 VisibleSpinner = false;
                             });
    }

    [JSInvokable("DetailCollapse")]
    public void DetailRowCollapse() => _target = null;

    private Task DownloadDocument(int arg) => ExecuteMethod(async () =>
                                                            {
                                                                SelectedDownload = DownloadsPanel.SelectedRow;
                                                                string _queryString = $"{SelectedDownload.InternalFileName}^{_target.ID}^{SelectedDownload.Location}^0".ToBase64String();
                                                                await JsRuntime.InvokeVoidAsync("open", $"{NavManager.BaseUri}Download/{_queryString}", "_blank");
                                                            });

    private Task EditActivity(int id) => ExecuteMethod(async () =>
                                                       {
                                                           SelectedActivity = ActivityPanel.SelectedRow;
                                                           NextSteps.Clear();
                                                           NextSteps.Add(new() {Text = "No Change", KeyValue = "0"});
                                                           try
                                                           {
                                                               foreach (string[] _next in _workflow.Where(flow => flow.Step == SelectedActivity.StatusCode)
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
                                                               //Ignore this error. No need to log this error.
                                                           }

                                                           await DialogActivity.ShowDialog();
                                                       });

    private Task EditCandidate() => ExecuteMethod(async () =>
                                                  {
                                                      VisibleSpinner = true;

                                                      if (_target == null || _target.ID == 0)
                                                      {
                                                          if (_candDetailsObjectClone == null)
                                                          {
                                                              _candDetailsObjectClone = new();
                                                          }
                                                          else
                                                          {
                                                              _candDetailsObjectClone.Clear();
                                                          }

                                                          _candDetailsObjectClone.IsAdd = true;
                                                      }
                                                      else
                                                      {
                                                          _candDetailsObjectClone = _candDetailsObject.Copy();

                                                          _candDetailsObjectClone.IsAdd = false;
                                                      }

                                                      VisibleSpinner = false;

                                                      await CandidateDialog.ShowDialog();
                                                      StateHasChanged();
                                                  });

    private Task EditEducation(int id) => ExecuteMethod(async () =>
                                                        {
                                                            VisibleSpinner = true;
                                                            if (id == 0)
                                                            {
                                                                if (SelectedEducation == null)
                                                                {
                                                                    SelectedEducationClone = new();
                                                                }
                                                                else
                                                                {
                                                                    SelectedEducation.Clear();
                                                                }
                                                            }
                                                            else
                                                            {
                                                                SelectedEducation = EducationPanel.SelectedRow != null ? EducationPanel.SelectedRow.Copy() : new();
                                                            }

                                                            EditConEducation = new(SelectedEducation!);
                                                            VisibleSpinner = false;
                                                            await CandidateEducationDialog.ShowDialog();
                                                        });

    private Task EditExperience(int id) => ExecuteMethod(async () =>
                                                         {
                                                             VisibleSpinner = true;
                                                             if (id == 0)
                                                             {
                                                                 if (SelectedExperience == null)
                                                                 {
                                                                     SelectedExperience = new();
                                                                 }
                                                                 else
                                                                 {
                                                                     SelectedExperience.Clear();
                                                                 }
                                                             }
                                                             else
                                                             {
                                                                 SelectedExperience = ExperiencePanel.SelectedRow != null ? ExperiencePanel.SelectedRow.Copy() : new();
                                                             }

                                                             EditConExperience = new(SelectedExperience!);
                                                             VisibleSpinner = false;
                                                             await CandidateExperienceDialog.ShowDialog();
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

                                                        EditConNotes = new(SelectedNotes!);
                                                        VisibleSpinner = false;
                                                        await CandidateNotesDialog.ShowDialog();
                                                    });

    private Task EditSkill(int skill) => ExecuteMethod(async () =>
                                                       {
                                                           VisibleSpinner = true;
                                                           if (skill == 0)
                                                           {
                                                               if (SelectedSkill == null)
                                                               {
                                                                   SelectedSkill = new();
                                                               }
                                                               else
                                                               {
                                                                   SelectedSkill.Clear();
                                                               }
                                                           }
                                                           else
                                                           {
                                                               SelectedSkill = SkillPanel.SelectedRow != null ? SkillPanel.SelectedRow.Copy() : new();
                                                           }

                                                           EditConSkill = new(SelectedSkill!);
                                                           VisibleSpinner = false;
                                                           await CandidateSkillDialog.ShowDialog();
                                                       });

    private Task ExecuteMethod(Func<Task> task) => General.ExecuteMethod(_semaphoreMainPage, task);

    private Task FormattedClick(MouseEventArgs arg) => GetResumeOnClick("Formatted");

    private async Task GetAlphabets(char alphabet) => await ExecuteMethod(async () =>
                                                                          {
                                                                              SearchModel.Name = alphabet.ToString();
                                                                              SearchModel.Page = 1;
                                                                              await Task.WhenAll(SaveStorage(), SetDataSource()).ConfigureAwait(false);
                                                                          });

    private void GetMPCDate()
    {
        string _mpcDate = "";
        if (_candDetailsObject.MPCNotes == "")
        {
            MPCDate = _mpcDate.ToMarkupString();
            return;
        }

        CandidateMPC _candidateMPCObjectFirst = _candMPCObject.MaxBy(x => x.DateTime);
        _mpcDate = $"{_candidateMPCObjectFirst.DateTime.CultureDate()} [{string.Concat(_candidateMPCObjectFirst.Name.Where(char.IsLetter))}]";

        MPCDate = _mpcDate.ToMarkupString();
    }

    private void GetMPCNote()
    {
        string _mpcNote = "";
        if (_candDetailsObject.MPCNotes == "")
        {
            MPCNote = _mpcNote.ToMarkupString();
            return;
        }

        CandidateMPC _candidateMPCObjectFirst = _candMPCObject.MaxBy(x => x.DateTime);
        _mpcNote = _candidateMPCObjectFirst.Comment;

        MPCNote = _mpcNote.ToMarkupString();
    }

    private void GetRatingDate()
    {
        string _ratingDate = "";
        if (_candDetailsObject.RateNotes == "")
        {
            RatingDate = _ratingDate.ToMarkupString();
            return;
        }

        CandidateRating _candidateRatingObjectFirst = _candRatingObject.MaxBy(x => x.DateTime);
        _ratingDate = $"{_candidateRatingObjectFirst.DateTime.CultureDate()} [{string.Concat(_candidateRatingObjectFirst.Name.Where(char.IsLetter))}]";

        RatingDate = _ratingDate.ToMarkupString();
    }

    private void GetRatingNote()
    {
        string _ratingNote = "";
        if (_candDetailsObject.RateNotes == "")
        {
            RatingNote = _ratingNote.ToMarkupString();
            return;
        }

        CandidateRating _candidateRatingObjectFirst = _candRatingObject.MaxBy(x => x.DateTime);
        _ratingNote = _candidateRatingObjectFirst.Comment;

        RatingNote = _ratingNote.ToMarkupString();
    }

    private Task GetResumeOnClick(string resumeType) => ExecuteMethod(async () =>
                                                                      {
                                                                          /*Dictionary<string, string> _parameters = new()
                                                                                                                   {
                                                                                                                       {"candidateID", _target.ID.ToString()},
                                                                                                                       {"resumeType", resumeType}
                                                                                                                   };
                                                                          DocumentDetails _restResponse = await General.GetRest<DocumentDetails>("Candidates/DownloadResume", _parameters);

                                                                          if (_restResponse != null)
                                                                          {
                                                                              await DownloadsPanel.ShowResume(_restResponse.DocumentLocation, _target.ID, "Original Resume",
                                                                                                              _restResponse.InternalFileName);
                                                                          }*/
                                                                          await Task.CompletedTask;
                                                                      });

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
                                                                                       }

                                                                                       await Task.WhenAll(SaveStorage(), SetDataSource()).ConfigureAwait(false);
                                                                                   });

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            if (RequisitionID == 0)
            {
                if (await SessionStorage.ContainKeyAsync(StorageName))
                {
                    SearchModel = await SessionStorage.GetItemAsync<CandidateSearch>(StorageName);
                }
                else
                {
                    SearchModel.Clear();
                }
            }
            else
            {
                SearchModel.Clear();
            }

            await SetDataSource();
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
                                    IEnumerable<Claim> _enumerable = _claims as Claim[] ?? _claims.ToArray();
                                    User = _enumerable.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value.ToUpperInvariant();
                                    RoleName = _enumerable.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value.ToUpperInvariant();
                                    if (User.NullOrWhiteSpace())
                                    {
                                        NavManager.NavigateTo($"{NavManager.BaseUri}login", true);
                                    }

                                    // Set user permissions
                                    HasViewRights = _enumerable.Any(claim => claim.Type == "Permission" && claim.Value == "ViewAllCandidates");
                                    HasEditRights = _enumerable.Any(claim => claim.Type == "Permission" && claim.Value == "CreateOrEditCandidate");
                                    DownloadOriginal = _enumerable.Any(claim => claim.Type == "Permission" && claim.Value == "DownloadOriginal");
                                    DownloadFormatted = _enumerable.Any(claim => claim.Type == "Permission" && claim.Value == "DownloadFormatted");
                                }

                                Uri _uri = NavManager.ToAbsoluteUri(NavManager.Uri);
                                if (QueryHelpers.ParseQuery(_uri.Query).TryGetValue("reqid", out StringValues _tempRequisitionID))
                                {
                                    RequisitionID = _tempRequisitionID.ToInt32();
                                }

                                if (QueryHelpers.ParseQuery(_uri.Query).TryGetValue("company", out StringValues _isFromCompany))
                                {
                                    IsFromCompany = _isFromCompany.ToInt32() > 0;
                                }

                                if (Start.APIHost.NullOrWhiteSpace())
                                {
                                    Start.APIHost = Configuration[NavManager.BaseUri.Contains("localhost") ? "APIHost" : "APIHostServer"];
                                }

                                // Get configuration data from cache server
                                List<string> _keys =
                                [
                                    CacheObjects.Roles.ToString(), CacheObjects.States.ToString(), CacheObjects.Eligibility.ToString(), CacheObjects.Experience.ToString(),
                                    CacheObjects.TaxTerms.ToString(), CacheObjects.JobOptions.ToString(), CacheObjects.StatusCodes.ToString(), CacheObjects.Workflow.ToString(),
                                    CacheObjects.Communications.ToString(), CacheObjects.DocumentTypes.ToString(), CacheObjects.Users.ToString()
                                ];

                                Dictionary<string, string> _cacheValues = await RedisService.BatchGet(_keys);

                                // Deserialize configuration data into master objects
                                _roles = General.DeserializeObject<List<Role>>(_cacheValues[CacheObjects.Roles.ToString()]);
                                _states = General.DeserializeObject<List<IntValues>>(_cacheValues[CacheObjects.States.ToString()]);
                                _eligibility = General.DeserializeObject<List<IntValues>>(_cacheValues[CacheObjects.Eligibility.ToString()]);
                                _experience = General.DeserializeObject<List<IntValues>>(_cacheValues[CacheObjects.Experience.ToString()]);
                                _taxTerms = General.DeserializeObject<List<KeyValues>>(_cacheValues[CacheObjects.TaxTerms.ToString()]);
                                _jobOptions = General.DeserializeObject<List<KeyValues>>(_cacheValues[CacheObjects.JobOptions.ToString()]);
                                _statusCodes = General.DeserializeObject<List<StatusCode>>(_cacheValues[CacheObjects.StatusCodes.ToString()]);
                                _workflow = General.DeserializeObject<List<Workflow>>(_cacheValues[CacheObjects.Workflow.ToString()]);
                                _communication = General.DeserializeObject<List<KeyValues>>(_cacheValues[CacheObjects.Communications.ToString()]);
                                _documentTypes = General.DeserializeObject<List<IntValues>>(_cacheValues[CacheObjects.DocumentTypes.ToString()]);
                            });

        await base.OnInitializedAsync();
    }

    private Task OriginalClick(MouseEventArgs arg) => GetResumeOnClick("Original");

    private async Task PageChanging(PageChangedEventArgs page)
    {
        Page = page.CurrentPage;
        SearchModel.Page = page.CurrentPage;
        await Task.WhenAll(SaveStorage(), SetDataSource()).ConfigureAwait(false);
    }

    private async Task PageSizeChanging(PageSizeChangedArgs page)
    {
        SearchModel.ItemCount = page.CurrentPageSize.ToInt32();
        SearchModel.Page = 1;
        await Grid.GoToPageAsync(1);
        await Task.WhenAll(SaveStorage(), SetDataSource()).ConfigureAwait(false);
    }

    private async Task Refresh() => await Grid.Refresh(true);

    private void RowSelected(RowSelectingEventArgs<Candidate> candidate)
    {
        if (_target != null && _target != candidate.Data)
        {
            _target = candidate.Data;
        }
    }

    private Task SaveActivity(EditContext activity) => ExecuteMethod(async () =>
                                                                     {
                                                                         Dictionary<string, string> _parameters = new()
                                                                                                                  {
                                                                                                                      {"candidateID", _target.ID.ToString()},
                                                                                                                      {"user", User},
                                                                                                                      {"roleID", RoleName},
                                                                                                                      {"isCandidateScreen", true.ToString()},
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

    private Task SaveCandidate() => ExecuteMethod(async () =>
                                                  {
                                                      Dictionary<string, string> _parameters = new()
                                                                                               {
                                                                                                   {"jsonPath", ""},
                                                                                                   {"userName", User},
                                                                                                   {"emailAddress", "maniv@titan-techs.com"}
                                                                                               };

                                                      await General.ExecuteRest<int>("Candidate/SaveCandidate", _parameters, _candDetailsObjectClone);

                                                      _candDetailsObject = _candDetailsObjectClone.Copy();
                                                      if (_candDetailsObject != null)
                                                      {
                                                          _target.Name = $"{_candDetailsObject.FirstName} {_candDetailsObject.LastName}";
                                                          _target.Phone = _candDetailsObject.Phone1.FormatPhoneNumber();
                                                          _target.Email = _candDetailsObject.Email;
                                                          _target.Location = $"{_candDetailsObject.City}, {SplitState(_candDetailsObject.StateID).Code}, {_candDetailsObject.ZipCode}";
                                                      }

                                                      _target.Updated = DateTime.Today.CultureDate() + "[ADMIN]";
                                                      _target.Status = "Available";
                                                      SetupAddress();
                                                      SetCommunication();
                                                      SetEligibility();
                                                      SetJobOption();
                                                      SetTaxTerm();
                                                      SetExperience();
                                                      StateHasChanged();
                                                  });

    private Task SaveDocument(EditContext document) => ExecuteMethod(async () =>
                                                                     {
                                                                         if (document.Model is CandidateDocument _document)
                                                                         {
                                                                             Dictionary<string, string> _parameters = new()
                                                                                                                      {
                                                                                                                          {"filename", DialogDocument.FileName},
                                                                                                                          {"mime", DialogDocument.Mime},
                                                                                                                          {"name", _document.Name},
                                                                                                                          {"notes", _document.Notes},
                                                                                                                          {"candidateID", _target.ID.ToString()},
                                                                                                                          {"user", User},
                                                                                                                          {"path", Start.UploadsPath},
                                                                                                                          {"type", _document.DocumentTypeID.ToString()}
                                                                                                                      };

                                                                             string _response = await General.ExecuteRest<string>("Candidate/UploadDocument", _parameters, null, true,
                                                                                                                                  DialogDocument.AddedDocument.ToStreamByteArray(),
                                                                                                                                  DialogDocument.FileName);

                                                                             if (_response.NotNullOrWhiteSpace() && _response == "[]")
                                                                             {
                                                                                 _candDocumentsObject = General.DeserializeObject<List<CandidateDocument>>(_response);
                                                                             }
                                                                         }
                                                                     });

    private Task SaveEducation(EditContext education) => ExecuteMethod(async () =>
                                                                       {
                                                                           if (education.Model is CandidateEducation _candidateEducation)
                                                                           {
                                                                               Dictionary<string, string> _parameters = new()
                                                                                                                        {
                                                                                                                            {"candidateID", _target.ID.ToString()},
                                                                                                                            {"user", User}
                                                                                                                        };
                                                                               string _response = await General.ExecuteRest<string>("Candidate/SaveEducation", _parameters,
                                                                                                                                    _candidateEducation);

                                                                               if (_response.NullOrWhiteSpace() || _response == "[]")
                                                                               {
                                                                                   return;
                                                                               }

                                                                               _candEducationObject = General.DeserializeObject<List<CandidateEducation>>(_response);
                                                                           }
                                                                       });

    private Task SaveExperience(EditContext experience) => ExecuteMethod(async () =>
                                                                         {
                                                                             if (experience.Model is CandidateExperience _candidateExperience)
                                                                             {
                                                                                 Dictionary<string, string> _parameters = new()
                                                                                                                          {
                                                                                                                              {"candidateID", _target.ID.ToString()},
                                                                                                                              {"user", User}
                                                                                                                          };
                                                                                 string _response = await General.ExecuteRest<string>("Candidate/SaveExperience", _parameters, _candidateExperience);

                                                                                 if (_response.NullOrWhiteSpace() || _response == "[]")
                                                                                 {
                                                                                     return;
                                                                                 }

                                                                                 _candExperienceObject = General.DeserializeObject<List<CandidateExperience>>(_response);
                                                                             }
                                                                         });

    private Task SaveMPC(EditContext editContext) => ExecuteMethod(async () =>
                                                                   {
                                                                       if (editContext.Model is CandidateRatingMPC _mpc)
                                                                       {
                                                                           Dictionary<string, string> _parameters = new()
                                                                                                                    {
                                                                                                                        {"user", User}
                                                                                                                    };
                                                                           Dictionary<string, object> _response = await General.PostRest("Candidate/SaveMPC", _parameters, _mpc);

                                                                           if (_response != null)
                                                                           {
                                                                               _candMPCObject = General.DeserializeObject<List<CandidateMPC>>(_response["MPCList"]);
                                                                               RatingMPC = JsonConvert.DeserializeObject<CandidateRatingMPC>(_response["FirstMPC"]?.ToString() ?? string.Empty);
                                                                               GetMPCDate();
                                                                               GetMPCNote();
                                                                           }
                                                                       }
                                                                   });

    private Task SaveNote(EditContext note) => ExecuteMethod(async () =>
                                                             {
                                                                 if (note.Model is CandidateNotes _candidateNotes)
                                                                 {
                                                                     Dictionary<string, string> _parameters = new()
                                                                                                              {
                                                                                                                  {"candidateID", _target.ID.ToString()},
                                                                                                                  {"user", User}
                                                                                                              };
                                                                     string _response = await General.ExecuteRest<string>("Candidate/SaveNotes", _parameters, _candidateNotes);

                                                                     if (_response.NullOrWhiteSpace() || _response == "[]")
                                                                     {
                                                                         return;
                                                                     }

                                                                     _candidateNotesObject = General.DeserializeObject<List<CandidateNotes>>(_response);
                                                                 }
                                                             });

    private Task SaveRating(EditContext editContext) => ExecuteMethod(async () =>
                                                                      {
                                                                          if (editContext.Model is CandidateRatingMPC _rating)
                                                                          {
                                                                              Dictionary<string, string> _parameters = new()
                                                                                                                       {
                                                                                                                           {"user", User}
                                                                                                                       };
                                                                              Dictionary<string, object> _response = await General.PostRest("Candidate/SaveRating", _parameters, _rating);

                                                                              if (_response != null)
                                                                              {
                                                                                  _candRatingObject = General.DeserializeObject<List<CandidateRating>>(_response["RatingList"]);
                                                                                  RatingMPC = JsonConvert.DeserializeObject<CandidateRatingMPC>(_response["FirstRating"]?.ToString() ?? string.Empty);
                                                                                  _candDetailsObject.RateCandidate = RatingMPC.Rating.ToInt32();
                                                                                  GetRatingDate();
                                                                                  GetRatingNote();
                                                                              }
                                                                          }
                                                                      });

    private Task SaveSkill(EditContext skill) => ExecuteMethod(async () =>
                                                               {
                                                                   if (skill.Model is CandidateSkills _skill)
                                                                   {
                                                                       Dictionary<string, string> _parameters = new()
                                                                                                                {
                                                                                                                    {"candidateID", _target.ID.ToString()},
                                                                                                                    {"user", User}
                                                                                                                };

                                                                       string _response = await General.ExecuteRest<string>("Candidate/SaveSkill", _parameters, _skill);

                                                                       if (_response.NullOrWhiteSpace() || _response == "[]")
                                                                       {
                                                                           return;
                                                                       }

                                                                       _candSkillsObject = General.DeserializeObject<List<CandidateSkills>>(_response);
                                                                   }
                                                               });

    private async Task SaveStorage(bool refreshGrid = true)
    {
        if (RequisitionID == 0)
        {
            await SessionStorage.SetItemAsync(StorageName, SearchModel);
        }

        if (refreshGrid)
        {
            await Grid.Refresh(true);
        }
    }

    private Task SaveSubmitCandidate(EditContext arg) => ExecuteMethod(async () =>
                                                                       {
                                                                           Dictionary<string, string> _parameters = new()
                                                                                                                    {
                                                                                                                        {"requisitionID", RequisitionID.ToString()},
                                                                                                                        {"candidateID", _target.ID.ToString()},
                                                                                                                        {"notes", _submitCandidateModel.Text},
                                                                                                                        {"roleID", RoleName},
                                                                                                                        {"user", User} /*,
                                                                                                                        {"jsonPath", Start.JsonFilePath},
                                                                                                                        {"emailAddress", General.GetEmail(LoginCookyUser)},
                                                                                                                        {"uploadPath", Start.UploadsPath}*/
                                                                                                                    };

                                                                           _ = await General.ExecuteRest<bool>("Candidate/SubmitCandidateRequisition", _parameters);

                                                                           if (RequisitionID > 0)
                                                                           {
                                                                               await SessionStorage.SetItemAsync("OptReqID", RequisitionID.ToString());
                                                                               NavManager.NavigateTo(NavManager.BaseUri + (IsFromCompany ? "company" : "requisition"));
                                                                           }
                                                                       });

    private void SetCommunication()
    {
        string _returnValue = _candDetailsObject.Communication switch
                              {
                                  "G" => "Good",
                                  "A" => "Average",
                                  "X" => "Excellent",
                                  _ => "Fair"
                              };

        CandidateCommunication = _returnValue.ToMarkupString();
    }

    private async Task SetDataSource()
    {
        (string _data, Count) = await General.ExecuteRest<ReturnGrid>("Candidate/GetGridCandidates", null, SearchModel, false);

        DataSource = JsonConvert.DeserializeObject<List<Candidate>>(_data);
        await Grid.Refresh().ConfigureAwait(false);
    }

    private void SetEligibility()
    {
        if (_eligibility is {Count: > 0})
        {
            CandidateEligibility = _candDetailsObject.EligibilityID > 0
                                       ? _eligibility.FirstOrDefault(eligibility => eligibility.KeyValue == _candDetailsObject.EligibilityID)!.Text.ToMarkupString()
                                       : "".ToMarkupString();
        }
    }

    private void SetExperience()
    {
        if (_experience is {Count: > 0})
        {
            CandidateExperience = (_candDetailsObject.ExperienceID > 0
                                       ? _experience.FirstOrDefault(experience => experience.KeyValue == _candDetailsObject.ExperienceID)!.Text
                                       : "").ToMarkupString();
        }
    }

    private void SetJobOption()
    {
        string _returnValue = "";
        if (_jobOptions is {Count: > 0})
        {
            string[] _splitJobOptions = _candDetailsObject.JobOptions.Split(',');
            foreach (string _str in _splitJobOptions)
            {
                if (_str == "")
                {
                    continue;
                }

                if (_returnValue != "")
                {
                    _returnValue += ", " + _jobOptions.FirstOrDefault(jobOption => jobOption.KeyValue == _str)?.Text;
                }
                else
                {
                    _returnValue = _jobOptions.FirstOrDefault(jobOption => jobOption.KeyValue == _str)?.Text;
                }
            }
        }

        CandidateJobOptions = _returnValue.ToMarkupString();
    }

    private void SetTaxTerm()
    {
        string _returnValue = "";

        if (_taxTerms is {Count: > 0})
        {
            string[] _splitTaxTerm = _candDetailsObject.TaxTerm.Split(',');
            foreach (string _str in _splitTaxTerm)
            {
                if (_str == "")
                {
                    continue;
                }

                if (_returnValue != "")
                {
                    _returnValue += ", " + _taxTerms.FirstOrDefault(taxTerm => taxTerm.KeyValue == _str)?.Text;
                }
                else
                {
                    _returnValue = _taxTerms.FirstOrDefault(taxTerm => taxTerm.KeyValue == _str)?.Text;
                }
            }
        }

        CandidateTaxTerms = _returnValue.ToMarkupString();
    }

    private void SetupAddress()
    {
        string _generateAddress = _candDetailsObject.Address1;

        if (_generateAddress == "")
        {
            _generateAddress = _candDetailsObject.Address2;
        }
        else
        {
            _generateAddress += _candDetailsObject.Address2 == "" ? "" : "<br/>" + _candDetailsObject.Address2;
        }

        if (_generateAddress == "")
        {
            _generateAddress = _candDetailsObject.City;
        }
        else
        {
            _generateAddress += _candDetailsObject.City == "" ? "" : "<br/>" + _candDetailsObject.City;
        }

        if (_candDetailsObject.StateID > 0)
        {
            if (_generateAddress == "")
            {
                _generateAddress = SplitState(_candDetailsObject.StateID).Name; // _states.FirstOrDefault(state => state.Value == _candidateDetailsObject.StateID)?.Text?.Split('-')[0].Trim();
            }
            else
            {
                try //Because sometimes the default values are not getting set. It's so random that it can't be debugged. And it never fails during debugging session.
                {
                    _generateAddress += ", " + SplitState(_candDetailsObject.StateID)
                                           .Name; //_states.FirstOrDefault(state => state.Value == _candidateDetailsObject.StateID)?.Text?.Split('-')[0].Trim();
                }
                catch
                {
                    //
                }
            }
        }

        if (_candDetailsObject.ZipCode != "")
        {
            if (_generateAddress == "")
            {
                _generateAddress = _candDetailsObject.ZipCode;
            }
            else
            {
                _generateAddress += ", " + _candDetailsObject.ZipCode;
            }
        }

        if (_generateAddress is {Length: > 1} && _generateAddress.StartsWith(','))
        {
            _generateAddress = _generateAddress[1..].Trim();
        }

        Address = _generateAddress.ToMarkupString();
    }

    private Task SpeedDialItemClicked(SpeedDialItemEventArgs args)
    {
        switch (args.Item.ID)
        {
            case "itemEditCandidate":
                _selectedTab = 0;
                return EditCandidate();
            case "itemEditRating":
                _selectedTab = 0;
                StateHasChanged();
                return RatingDialog.ShowDialog();
            case "itemEditMPC":
                _selectedTab = 0;
                StateHasChanged();
                return MPCDialog.ShowDialog();
            case "itemAddSkill":
                _selectedTab = 1;
                return EditSkill(0);
            case "itemAddEducation":
                _selectedTab = 2;
                return EditEducation(0);
            case "itemAddExperience":
                _selectedTab = 3;
                return EditExperience(0);
            case "itemAddNotes":
                _selectedTab = 4;
                return EditNotes(0);
            case "itemAddAttachment":
                _selectedTab = 6;
                return AddDocument();
            case "itemOriginalResume":
                _selectedTab = 5;
                return Task.CompletedTask;
            //return AddResume(0);
            case "itemFormattedResume":
                _selectedTab = 5;
                return Task.CompletedTask;
            //return AddResume(1);
        }

        return Task.CompletedTask;
    }

    private (string Code, string Name) SplitState(int stateID)
    {
        string _stateName = _states.FirstOrDefault(state => state.KeyValue == stateID)?.Text!;
        if (_stateName == null)
        {
            return ("", "");
        }

        if (!_stateName.Contains(" - "))
        {
            return ("", _stateName);
        }

        string[] _parts = _stateName.Split([" - "], StringSplitOptions.TrimEntries);
        return _parts.Length != 2 ? ("", "") : (_parts[0].Trim('[', ']'), _parts[1]);

        // Remove the brackets from the code
    }

    private async Task SubmitSelectedCandidate(MouseEventArgs arg)
    {
        _submitCandidateModel.Clear();
        await SubmitDialog.ShowDialog();
    }

    private void TabSelected(SelectEventArgs tab) => _selectedTab = tab.SelectedIndex;

    private Task UndoActivity(int activityID) => ExecuteMethod(async () =>
                                                               {
                                                                   Dictionary<string, string> _parameters = new()
                                                                                                            {
                                                                                                                {"submissionID", activityID.ToString()},
                                                                                                                {"user", User},
                                                                                                                {"isCandidateScreen", "true"},
                                                                                                                {"roleID", RoleName}
                                                                                                            };
                                                                   string _response = await General.ExecuteRest<string>("Candidate/UndoCandidateActivity", _parameters);

                                                                   if (_response.NotNullOrWhiteSpace() && _response != "[]")
                                                                   {
                                                                       _candActivityObject = General.DeserializeObject<List<CandidateActivity>>(_response);
                                                                   }
                                                               });
}
/*
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
                     CandidateSearch _searchModel = General.DeserializeObject<CandidateSearch>(dm.Params["SearchModel"].ToString());

                     (string _data, int _count) = await General.ExecuteRest<ReturnGrid>("Candidate/GetGridCandidates", null, _searchModel, false);

                     _dataSource = JsonConvert.DeserializeObject<List<Candidate>>(_data);

                     _candidateReturn = dm.RequiresCounts ? new DataResult
                                                            {
                                                                Result = _dataSource,
                                                                Count = _count /*_count#1#
                                                            } : _dataSource;
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
 */