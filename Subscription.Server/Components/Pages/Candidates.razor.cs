#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.Server
// File Name:           Candidates.razor.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu, Brijesh Dubey
// Created On:          07-11-2025 19:07
// Last Updated On:     07-18-2025 20:28
// *****************************************/

#endregion

#region Using

using System.Text;

#endregion

namespace Subscription.Server.Components.Pages;

public sealed partial class Candidates : IDisposable
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

    private bool _disposed;

    private DotNetObjectReference<Candidates> _dotNetReference;

    private List<IntValues> _eligibility = [], _experience = [], _documentTypes = [];
    //private bool _formattedExists, _originalExists;

    private List<KeyValues> _jobOptions = [], _taxTerms = [], _communication = [];
    private Dictionary<string, string> _jobOptionsDict;

    private readonly Dictionary<string, string> _reusableParameters = new();

    private int _selectedTab;

    private readonly SemaphoreSlim _semaphoreMainPage = new(1, 1);
    private List<StateCache> _states = [];
    private List<StatusCode> _statusCodes = [];

    private readonly StringBuilder _stringBuilder = new();

    //private readonly Stopwatch _stopwatch = new();
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

    private AdvancedCandidateSearch DialogSearch { get; set; }

    [Inject]
    private SfDialogService DialogService { get; set; }

    private bool DownloadFormatted { get; set; }

    private bool DownloadOriginal { get; set; }

    //public EditContext EditConEducation { get; set; }

    //public EditContext EditConExperience { get; set; }

    //public EditContext EditConNotes { get; set; }

    //public EditContext EditConSkill { get; set; }

    private EducationPanel EducationPanel { get; set; }

    private ExperiencePanel ExperiencePanel { get; set; }

    //private string FileName { get; set; }

    private bool FormattedExists { get; set; }

    private SfGrid<Candidate> Grid { get; set; }

    private bool HasEditRights { get; set; }

    //private bool HasRendered { get; set; }

    private bool HasViewRights { get; set; }

    private bool IsFromCompany { get; set; }

    [Inject]
    private IJSRuntime JsRuntime { get; set; }

    [Inject]
    private ILocalStorageService LocalStorage { get; set; }

    //private string Mime { get; set; }

    private MarkupString MPCDate { get; set; }

    private MPCCandidateDialog MPCDialog { get; set; }

    private MarkupString MPCNote { get; set; }

    [Inject]
    private NavigationManager NavManager { get; set; }

    private CandidateDocument NewDocument { get; } = new();

    private CandidateResume NewResume { get; } = new();

    private List<KeyValues> NextSteps { get; set; } = [];

    private NotesPanel NotesPanel { get; set; }

    private bool OriginalExists { get; set; }

    //private int Page { get; set; } = 1;

    private DownloadsPanel PanelDownload { get; set; }

    private MarkupString RatingDate { get; set; }

    private RatingCandidateDialog RatingDialog { get; set; }

    private CandidateRatingMPC RatingMPC { get; set; } = new();

    private MarkupString RatingNote { get; set; }

    [Inject]
    private RedisService RedisService { get; set; }

    private int RequisitionID { get; set; }

    private string ResumeType { get; set; }

    private UpdateResume ResumeUpdate { get; set; }

    private int RoleID { get; } = 5;

    private string RoleName { get; set; }

    private CandidateSearch SearchModel { get; set; } = new();

    private CandidateSearch SearchModelClone { get; set; } = new();

    private CandidateActivity SelectedActivity { get; set; } = new();

    private CandidateDocument SelectedDownload { get; set; } = new();

    private CandidateEducation SelectedEducation { get; set; } = new();

    //private CandidateEducation SelectedEducationClone { get; set; } = new();

    private CandidateExperience SelectedExperience { get; set; } = new();

    private CandidateNotes SelectedNotes { get; set; } = new();

    private CandidateSkills SelectedSkill { get; set; } = new();

    [Inject]
    private ISessionStorageService SessionStorage { get; set; }

    private SkillPanel SkillPanel { get; set; }

    /*private SfSpinner Spinner { get; set; }*/

    private SubmitCandidate SubmitDialog { get; set; }

    private UploadCandidate UploadCandidateDialog { get; set; }

    private string User { get; set; }

    private bool VisibleSpinner { get; set; }

    /*[Inject]
    private ZipCodeService ZipCodeService { get; set; }*/

    public void Dispose()
    {
        if (!_disposed)
        {
            // Dispose managed resources
            _semaphoreMainPage?.Dispose();
            _dotNetReference?.Dispose();

            // Clear large collections
            ClearAllCollections();

            _disposed = true;
        }

        //GC.SuppressFinalize();
    }

    private async Task AddCandidate(MouseEventArgs arg)
    {
        if (await DialogService.ConfirmAsync(null, "Auto-Fill Candidate?", General.DialogOptions("Do you want to auto-fill the candidate details?")))
        {
            await UploadCandidateDialog.ShowDialog();
        }
        else
        {
            await Grid.SelectRowAsync(-1);
            await EditCandidate();
        }
    }

    private Task AddDocument() => ExecuteMethod(() =>
                                                {
                                                    NewDocument.Clear();
                                                    return DialogDocument.ShowDialog();
                                                });

    private void AdvancedSearch() => ExecuteMethod(async () =>
                                                   {
                                                       SearchModelClone = SearchModel.Copy();
                                                       if (SearchModelClone.Relocate.NullOrWhiteSpace())
                                                       {
                                                           SearchModelClone.Relocate = "";
                                                       }

                                                       if (SearchModelClone.SecurityClearance.NullOrWhiteSpace())
                                                       {
                                                           SearchModelClone.SecurityClearance = "";
                                                       }

                                                       await DialogSearch.ShowDialog();
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

    private Task ChangeStatus() => ExecuteMethod(async () =>
                                                 {
                                                     if (await DialogService.ConfirmAsync(null, "Change Candidate Status?",
                                                                                          General.DialogOptions("Do you want to change the status of this candidate?")).ConfigureAwait(false))
                                                     {
                                                         await Grid.ShowSpinnerAsync();
                                                         Dictionary<string, string> _parameters = new()
                                                                                                  {
                                                                                                      {"candidateID", _target.ID.ToString()},
                                                                                                      {"user", User}
                                                                                                  };

                                                         string _response = await General.ExecuteRest<string>("Candidate/ChangeStatus", _parameters);

                                                         if (_response.NotNullOrWhiteSpace() && _response != "[]")
                                                         {
                                                             _target.Status = _response;
                                                         }

                                                         await Grid.HideSpinnerAsync();
                                                     }
                                                 });

    private void ClearAllCollections()
    {
        _candActivityObject?.Clear();
        _candDocumentsObject?.Clear();
        _candEducationObject?.Clear();
        _candExperienceObject?.Clear();
        _candidateNotesObject?.Clear();
        _candMPCObject?.Clear();
        _candRatingObject?.Clear();
        _candSkillsObject?.Clear();
        DataSource?.Clear();
    }

    private Task ClearFilter() => ExecuteMethod(async () =>
                                                {
                                                    SearchModel.Clear();
                                                    SearchModel.User = User;
                                                    await Task.WhenAll(SaveStorage(), SetDataSource()).ConfigureAwait(false);
                                                });

    private async Task CloseUploadCandidate()
    {
        await Refresh().ConfigureAwait(false);
    }

    private Dictionary<string, string> CreateParameters(int id)
    {
        _reusableParameters.Clear();
        _reusableParameters["id"] = id.ToString();
        _reusableParameters["candidateID"] = _target.ID.ToString();
        _reusableParameters["user"] = User;
        return _reusableParameters;

        /*return new()
               {
                   {"id", id.ToString()},
                   {"candidateID", _target.ID.ToString()},
                   {"user", User}
               };*/
    }

    private Task DataHandler() => ExecuteMethod(async () =>
                                                {
                                                    _dotNetReference = DotNetObjectReference.Create(this); // create dotnet ref
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

                                                              _candDocumentsObject = General.DeserializeObject<List<CandidateDocument>>(_response, false);
                                                          });

    private Task DeleteEducation(int id) => ExecuteMethod(async () =>
                                                          {
                                                              Dictionary<string, string> _parameters = CreateParameters(id);
                                                              string _response = await General.ExecuteRest<string>("Candidate/DeleteEducation", _parameters);

                                                              _candEducationObject = General.DeserializeObject<List<CandidateEducation>>(_response, false);
                                                          });

    private Task DeleteExperience(int id) => ExecuteMethod(async () =>
                                                           {
                                                               Dictionary<string, string> _parameters = CreateParameters(id);
                                                               string _response = await General.ExecuteRest<string>("Candidate/DeleteExperience", _parameters);

                                                               _candExperienceObject = General.DeserializeObject<List<CandidateExperience>>(_response, false);
                                                           });

    private Task DeleteNotes(int id) => ExecuteMethod(async () =>
                                                      {
                                                          Dictionary<string, string> _parameters = CreateParameters(id);
                                                          string _response = await General.ExecuteRest<string>("Candidate/DeleteNotes", _parameters);

                                                          _candidateNotesObject = General.DeserializeObject<List<CandidateNotes>>(_response, false);
                                                      });

    private Task DeleteSkill(int id) => ExecuteMethod(async () =>
                                                      {
                                                          Dictionary<string, string> _parameters = CreateParameters(id);
                                                          string _response = await General.ExecuteRest<string>("Candidate/DeleteSkill", _parameters);

                                                          _candSkillsObject = General.DeserializeObject<List<CandidateSkills>>(_response, false);
                                                      });

    private Task DetailDataBind(DetailDataBoundEventArgs<Candidate> candidate)
    {
        return ExecuteMethod(async () =>
                             {
                                 int _index = await Grid.GetRowIndexByPrimaryKeyAsync(candidate.Data.ID);
                                 if (_index != Grid.SelectedRowIndex)
                                 {
                                     await Grid.SelectRowAsync(_index);
                                 }

                                 await Grid.CollapseAllDetailRowAsync();
                                 await Grid.ExpandCollapseDetailRowAsync(candidate.Data);

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
                                 FormattedExists = _target.FormattedResume;
                                 OriginalExists = _target.OriginalResume;

                                 // await Task.Delay(100);
                                 VisibleSpinner = false;
                             });
    }

    [JSInvokable("DetailCollapse")]
    public void DetailRowCollapse() => _target = null;

    private Task DownloadDocument(int arg) => ExecuteMethod(async () =>
                                                            {
                                                                SelectedDownload = PanelDownload.SelectedRow;
                                                                string _queryString = $"{SelectedDownload.InternalFileName}^{_target.ID}^{SelectedDownload.Location}^0".ToBase64String();
                                                                await JsRuntime.InvokeVoidAsync("open", $"{NavManager.BaseUri}Download/{_queryString}", "_blank");
                                                            });

    private async Task DuplicateCandidate()
    {
        if (await DialogService.ConfirmAsync(null, "Duplicate Candidate?", General.DialogOptions("Are you sure you want to duplicate this candidate?")).ConfigureAwait(false))
        {
            Dictionary<string, string> _parameters = new() {{"candidateID", _target.ID.ToString()}, {"user", User}};

            int _duplicateCandidateID = await General.ExecuteRest<int>("Candidate/DuplicateCandidate", _parameters).ConfigureAwait(false);

            if (_duplicateCandidateID > 0)
            {
                SearchModel.Page = 1;
                SearchModel.Name = _target.Name[.._target.Name.IndexOf('(')].Trim();
                SearchModel.ActiveRequisitionsOnly = false;
                SearchModel.ShowArchive = false;
                await Task.WhenAll(SaveStorage(), SetDataSource()).ConfigureAwait(false);
            }
        }
    }

    private Task EditActivity(int id) => ExecuteMethod(async () =>
                                                       {
                                                           SelectedActivity = ActivityPanel.SelectedRow;
                                                           NextSteps.Clear();
                                                           try
                                                           {
                                                               HashSet<string> nextCodes = _workflow.Where(flow => flow.Step == SelectedActivity.StatusCode)
                                                                                                    .SelectMany(flow => flow.Next.Split(','))
                                                                                                    .Distinct().ToHashSet();
                                                               NextSteps = _statusCodes.Where(status => nextCodes.Contains(status.Code) && status.AppliesToCode == "SCN")
                                                                                       .Select(status => new KeyValues {Text = status.Status, KeyValue = status.Code})
                                                                                       .Prepend(new() {Text = "No Change", KeyValue = "0"}).ToList();
                                                               await DialogActivity.ShowDialog();
                                                           }
                                                           catch (Exception ex)
                                                           {
                                                               Console.WriteLine(ex.Message);
                                                               //Ignore this error. No need to log this error.
                                                           }
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
                                                                SelectedEducation?.Clear();
                                                            }
                                                            else
                                                            {
                                                                SelectedEducation = EducationPanel.SelectedRow != null ? EducationPanel.SelectedRow.Copy() : new();
                                                            }

                                                            //EditConEducation = new(SelectedEducation!);
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

                                                             //EditConExperience = new(SelectedExperience!);
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

                                                        //EditConNotes = new(SelectedNotes!);
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

                                                           //EditConSkill = new(SelectedSkill!);
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
        if (_candDetailsObject.MPCNotes.NullOrWhiteSpace())
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
        if (_candDetailsObject.MPCNotes.NullOrWhiteSpace())
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
        if (_candDetailsObject.RateNotes.NullOrWhiteSpace())
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
        if (_candDetailsObject.RateNotes.NullOrWhiteSpace())
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
                                                                          Dictionary<string, string> _parameters = new()
                                                                                                                   {
                                                                                                                       {"candidateID", _target.ID.ToString()},
                                                                                                                       {"resumeType", resumeType}
                                                                                                                   };
                                                                          string _restResponse = await General.ExecuteRest<string>("Candidate/DownloadResume", _parameters, null, false);

                                                                          if (_restResponse != null && _restResponse != "[]")
                                                                          {
                                                                              DocumentDetails _response = General.DeserializeObject<DocumentDetails>(_restResponse);
                                                                              try
                                                                              {
                                                                                  await PanelDownload.ShowResume(_response.DocumentLocation, _target.ID, "Original Resume", _response.InternalFileName);
                                                                              }
                                                                              catch (Exception ex)
                                                                              {
                                                                                  Console.WriteLine(ex.Message);
                                                                              }
                                                                          }
                                                                      });

    private Task GridPageChanging(GridPageChangingEventArgs page) => ExecuteMethod(async () =>
                                                                                   {
                                                                                       await Task.Delay(2000);
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

    private void InitializeJobOptionsDictionary()
    {
        _jobOptionsDict = _jobOptions?.ToDictionary(x => x.KeyValue, x => x.Text) ?? new();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            if (RequisitionID == 0)
            {
                if (await SessionStorage.ContainKeyAsync(StorageName))
                {
                    SearchModel = await SessionStorage.GetItemAsync<CandidateSearch>(StorageName).ConfigureAwait(false);
                }
                else
                {
                    SearchModel.Clear();
                    SearchModel.User = User;
                    await SaveStorage().ConfigureAwait(false);
                }
            }
            else
            {
                SearchModel.Clear();
            }

            //_stopwatch.Start();
            await SetDataSource().ConfigureAwait(false);
            //_stopwatch.Stop();
            //await File.WriteAllTextAsync(@"C:\Logs\ZipLog.txt",$"Elapsed time: {_stopwatch.ElapsedMilliseconds} ms\n");
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
                                    nameof(CacheObjects.Roles), nameof(CacheObjects.States), nameof(CacheObjects.Eligibility), nameof(CacheObjects.Experience),
                                    nameof(CacheObjects.TaxTerms), nameof(CacheObjects.JobOptions), nameof(CacheObjects.StatusCodes), nameof(CacheObjects.Workflow),
                                    nameof(CacheObjects.Communications), nameof(CacheObjects.DocumentTypes), nameof(CacheObjects.Users)
                                ];

                                Dictionary<string, string> _cacheValues = await RedisService.BatchGet(_keys);

                                // Deserialize configuration data into master objects
                                //_roles = General.DeserializeObject<List<Role>>(_cacheValues[nameof(CacheObjects.Roles)]);
                                _states = General.DeserializeObject<List<StateCache>>(_cacheValues[nameof(CacheObjects.States)]);
                                _eligibility = General.DeserializeObject<List<IntValues>>(_cacheValues[nameof(CacheObjects.Eligibility)]);
                                _experience = General.DeserializeObject<List<IntValues>>(_cacheValues[nameof(CacheObjects.Experience)]);
                                _taxTerms = General.DeserializeObject<List<KeyValues>>(_cacheValues[nameof(CacheObjects.TaxTerms)]);
                                _jobOptions = General.DeserializeObject<List<KeyValues>>(_cacheValues[nameof(CacheObjects.JobOptions)]);
                                _statusCodes = General.DeserializeObject<List<StatusCode>>(_cacheValues[nameof(CacheObjects.StatusCodes)]);
                                _workflow = General.DeserializeObject<List<Workflow>>(_cacheValues[nameof(CacheObjects.Workflow)]);
                                _communication = General.DeserializeObject<List<KeyValues>>(_cacheValues[nameof(CacheObjects.Communications)]);
                                _documentTypes = General.DeserializeObject<List<IntValues>>(_cacheValues[nameof(CacheObjects.DocumentTypes)]);
                                InitializeJobOptionsDictionary();
                            });

        await base.OnInitializedAsync();
    }

    private Task OriginalClick(MouseEventArgs arg) => GetResumeOnClick("Original");

    private async Task PageChanging(PageChangedEventArgs page)
    {
        //Page = page.CurrentPage;
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
        _target = candidate.Data;
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
                                                                                                   {"userName", User}
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

                                                      _target.Updated = DateTime.Today.CultureDate() + "[" + User + "]";
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

                                                                             if (_response.NotNullOrWhiteSpace() && _response != "[]")
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

                                                                               if (_response.NotNullOrWhiteSpace() && _response != "[]")
                                                                               {
                                                                                   _candEducationObject = General.DeserializeObject<List<CandidateEducation>>(_response);
                                                                               }
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
                                                                               RatingMPC = JsonConvert.DeserializeObject<CandidateRatingMPC>(_response["FirstMPC"]?.ToString() ?? "");
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
                                                                                  RatingMPC = JsonConvert.DeserializeObject<CandidateRatingMPC>(_response["FirstRating"]?.ToString() ?? "");
                                                                                  _candDetailsObject.RateCandidate = RatingMPC.Rating.ToInt32();
                                                                                  GetRatingDate();
                                                                                  GetRatingNote();
                                                                              }
                                                                          }
                                                                      });

    private Task SaveResume(EditContext resume) => ExecuteMethod(async () =>
                                                                 {
                                                                     if (resume.Model is CandidateResume _resume)
                                                                     {
                                                                         Dictionary<string, string> _parameters = new()
                                                                                                                  {
                                                                                                                      {"filename", ResumeUpdate.FileName},
                                                                                                                      {"mime", ResumeUpdate.Mime},
                                                                                                                      {"candidateID", _target.ID.ToString()},
                                                                                                                      {"user", User},
                                                                                                                      {"type", ResumeType},
                                                                                                                      {"updateTextResume", _resume.UpdateTextResume.ToString()}
                                                                                                                  };

                                                                         string _response = await General.ExecuteRest<string>("Candidate/UpdateResume", _parameters, null, true,
                                                                                                                              ResumeUpdate.AddedDocument.ToStreamByteArray(), ResumeUpdate.FileName);

                                                                         if (_response.NotNullOrWhiteSpace() && _response != "[]" && _resume.UpdateTextResume)
                                                                         {
                                                                             _candDetailsObject.TextResume = _response;
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
            SearchModel.User = User;
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

    private async Task SearchCandidate(EditContext arg)
    {
        SearchModel = SearchModelClone.Copy();
        SearchModel.Page = 1;
        await Task.WhenAll(SaveStorage(), SetDataSource()).ConfigureAwait(false);
    }

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
        /*
        _stopwatch.Reset();
        _stopwatch.Start();
        */
        await Grid.ShowSpinnerAsync();
        (string _data, Count) = await General.ExecuteRest<ReturnGrid>("Candidate/GetGridCandidates", null, SearchModel, false).ConfigureAwait(false);
        DataSource = JsonConvert.DeserializeObject<List<Candidate>>(_data);
        await Grid.Refresh();
        await Grid.HideSpinnerAsync();
        /*
        _stopwatch.Stop();
        await File.WriteAllTextAsync(@"C:\Logs\ZipLog.txt",$"Elapsed time: {_stopwatch.ElapsedMilliseconds} ms\n");
    */
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
        _stringBuilder.Clear(); // Reuse same instance

        if (_jobOptionsDict?.Count > 0 && !string.IsNullOrWhiteSpace(_candDetailsObject.JobOptions))
        {
            string[] keys = _candDetailsObject.JobOptions.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            foreach (string key in keys)
            {
                if (!_jobOptionsDict.TryGetValue(key, out string text))
                {
                    continue;
                }

                if (_stringBuilder.Length > 0)
                {
                    _stringBuilder.Append(", ");
                }

                _stringBuilder.Append(text);
            }

            CandidateJobOptions = _stringBuilder.ToString().ToMarkupString();
        }
        else
        {
            CandidateJobOptions = string.Empty.ToMarkupString();
        }

        /*if (_jobOptions is {Count: > 0} && !string.IsNullOrWhiteSpace(_candDetailsObject.JobOptions))
        {
            IEnumerable<string> _matchedTexts = _candDetailsObject.JobOptions.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                                                                  .Select(key => _jobOptions.FirstOrDefault(option => option.KeyValue == key)?.Text)
                                                                  .Where(text => text.NotNullOrWhiteSpace());

            CandidateJobOptions = string.Join(", ", _matchedTexts).ToMarkupString();
        }
        else
        {
            CandidateJobOptions = string.Empty.ToMarkupString();
        }*/
    }

    private void SetTaxTerm()
    {
        if (_taxTerms is {Count: > 0} && !string.IsNullOrWhiteSpace(_candDetailsObject.TaxTerm))
        {
            IEnumerable<string> _selected = _candDetailsObject.TaxTerm.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                                                              .Select(code => _taxTerms.FirstOrDefault(t => t.KeyValue == code)?.Text)
                                                              .Where(text => !string.IsNullOrWhiteSpace(text));

            CandidateTaxTerms = string.Join(", ", _selected).ToMarkupString();
        }
        else
        {
            CandidateTaxTerms = string.Empty.ToMarkupString();
        }
    }

    private void SetupAddress()
    {
        List<string> addressParts = [];
        List<string> locationParts = [];

        if (_candDetailsObject.Address1.NotNullOrWhiteSpace())
        {
            addressParts.Add(_candDetailsObject.Address1);
        }

        if (_candDetailsObject.Address2.NotNullOrWhiteSpace())
        {
            addressParts.Add(_candDetailsObject.Address2);
        }

        if (_candDetailsObject.City.NotNullOrWhiteSpace())
        {
            locationParts.Add(_candDetailsObject.City);
        }

        if (_candDetailsObject.StateID > 0)
        {
            try
            {
                string stateName = SplitState(_candDetailsObject.StateID).Name;
                if (stateName.NotNullOrWhiteSpace())
                {
                    locationParts.Add(stateName);
                }
            }
            catch
            {
                // Log or ignore random SplitState failures
            }
        }

        if (_candDetailsObject.ZipCode.NotNullOrWhiteSpace())
        {
            locationParts.Add(_candDetailsObject.ZipCode);
        }

        string line1 = string.Join(", ", addressParts);
        string line2 = string.Join(", ", locationParts);

        Address = string.Join("<br/>", new[] {line1, line2}.Where(line => line.NotNullOrWhiteSpace())).ToMarkupString();
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
                return UpdateCandidateResume(true);
            //return AddResume(0);
            case "itemFormattedResume":
                _selectedTab = 5;
                return UpdateCandidateResume(false);
            //return AddResume(1);
            case "itemChangeStatus":
                return ChangeStatus();
            case "itemDuplicateCandidate":
                return DuplicateCandidate();
        }

        return Task.CompletedTask;
    }

    private (string Code, string Name) SplitState(int stateID)
    {
        StateCache _stateSplit = _states.FirstOrDefault(state => state.KeyValue == stateID);
        return (_stateSplit.Code, _stateSplit.Text);
    }

    private async Task SubmitSelectedCandidate(MouseEventArgs arg)
    {
        _submitCandidateModel.Clear();
        _submitCandidateModel.CandidateID = _target.ID;
        _submitCandidateModel.RequisitionID = RequisitionID;
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

    private async Task UpdateCandidateResume(bool isOriginal)
    {
        ResumeType = isOriginal ? "Original" : "Formatted";
        await ResumeUpdate.ShowDialog().ConfigureAwait(false);
    }
}