#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.Server
// File Name:           Candidates.razor.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu, Brijesh Dubey
// Created On:          02-06-2025 19:02
// Last Updated On:     03-03-2025 21:03
// *****************************************/

#endregion

namespace Subscription.Server.Components.Pages;

public partial class Candidates
{
    private const string StorageName = "CandidatesGrid";
    private static TaskCompletionSource<bool> _initializationTaskSource;
    private List<CandidateActivity> _candidateActivityObject = [];
    private CandidateDetails _candidateDetailsObject = new(), _candidateDetailsObjectClone = new();
    private List<CandidateDocument> _candidateDocumentsObject = [];
    private List<CandidateEducation> _candidateEducationObject = [];
    private List<CandidateExperience> _candidateExperienceObject = [];
    private List<CandidateMPC> _candidateMPCObject = [];
    private List<CandidateNotes> _candidateNotesObject = [];
    private List<CandidateRating> _candidateRatingObject = [];
    private List<CandidateSkills> _candidateSkillsObject = [];
    private List<IntValues> _eligibility = [], _experience = [], _states, _documentTypes = [];
    private bool _formattedExists, _originalExists;

    private List<KeyValues> _jobOptions = [], _taxTerms = [], _communication = [], _statusCodes = [];
    private List<AppWorkflow> _workflow = [];

    private Query _query = new();

    //private CandidateRatingMPC _ratingMPC = new();
    private List<Role> _roles;
    private int _selectedTab;

    private readonly SemaphoreSlim _semaphoreMainPage = new(1, 1);

    private Candidate _target;

    private readonly List<ToolbarItemModel> _tools1 =
    [
        new() {Name = "Original", TooltipText = "Show Original Resume"},
        new() {Name = "Formatted", TooltipText = "Show Formatted Resume"}
    ];

    private ActivityPanel ActivityPanel
    {
        get;
        set;
    }

    /// <summary>
    ///     Represents the address of the candidate.
    /// </summary>
    /// <remarks>
    ///     This property is a markup string that holds the address of the candidate.
    ///     The address is constructed from various address fields of the candidate's details.
    /// </remarks>
    private MarkupString Address
    {
        get;
        set;
    }

    /// <summary>
    ///     Represents the candidate's communication rating.
    /// </summary>
    /// <remarks>
    ///     This property is a markup string that holds the communication rating of the candidate.
    ///     The communication rating is converted to a more descriptive string.
    ///     The conversion is as follows:
    ///     - "G" => "Good"
    ///     - "A" => "Average"
    ///     - "X" => "Excellent"
    ///     - Any other value => "Fair"
    /// </remarks>
    private MarkupString CandidateCommunication
    {
        get;
        set;
    }

    private EditCandidateDialog CandidateDialog
    {
        get;
        set;
    }

    /// <summary>
    ///     Represents the dialog component for editing a candidate's education details.
    /// </summary>
    /// <remarks>
    ///     This component provides a user interface for editing the education details of a candidate.
    ///     It includes fields for entering the degree, college, location, year, and other relevant information.
    ///     The dialog can be opened in either add or edit mode, depending on whether an existing education record is being
    ///     edited or a new one is being added.
    /// </remarks>
    private EditEducationDialog CandidateEducationDialog
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
    ///     This property is in the `SetEligibility()` method and in the `BuildRenderTree()` method of the `Candidate`
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

    /// <summary>
    ///     Represents the dialog component for editing a candidate's experience details.
    /// </summary>
    /// <remarks>
    ///     This component provides a user interface for editing the experience details of a candidate.
    ///     It includes fields for entering the job title, company, location, duration, and other relevant information.
    ///     The dialog can be opened in either add or edit mode, depending on whether an existing experience record is being
    ///     edited or a new one is being added.
    /// </remarks>
    private EditExperienceDialog CandidateExperienceDialog
    {
        get;
        set;
    }

    /// <summary>
    ///     Represents the job options for the candidate.
    /// </summary>
    /// <remarks>
    ///     This property is a markup string that holds the job options of the candidate.
    ///     The job options are constructed from the candidate's details and the `_jobOptions` collection.
    /// </remarks>
    private MarkupString CandidateJobOptions
    {
        get;
        set;
    }

    private EditNotesDialog CandidateNotesDialog
    {
        get;
        set;
    }

    /// <summary>
    ///     Represents the dialog component for editing a candidate's skills.
    /// </summary>
    /// <remarks>
    ///     This component provides a user interface for editing the skills of a candidate.
    ///     It includes fields for entering skill details and can be opened in either add or edit mode.
    /// </remarks>
    private EditSkillDialog CandidateSkillDialog
    {
        get;
        set;
    }

    /// <summary>
    ///     Represents the tax terms for the candidate.
    /// </summary>
    /// <remarks>
    ///     This property is a markup string that holds the tax terms of the candidate.
    ///     The tax terms are constructed from the candidate's details and the `_taxTerms` collection.
    /// </remarks>
    private MarkupString CandidateTaxTerms
    {
        get;
        set;
    }

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
    ///     Gets or sets the dialog for adding a candidate document.
    /// </summary>
    /// <remarks>
    ///     This property is used to manage the dialog for adding a candidate document in the Candidate page.
    ///     It is an instance of the `AddCandidateDocument` component, which allows the user to upload a document to a
    ///     company.
    ///     The dialog is shown when the `AddDocument` method is called.
    /// </remarks>
    private AddDocumentDialog DialogDocument
    {
        get;
        set;
    }

    private bool DownloadFormatted
    {
        get;
        set;
    }

    private bool DownloadOriginal
    {
        get;
        set;
    }

    private DownloadsPanel DownloadsPanel
    {
        get;
        set;
    }

    public EditContext EditConEducation
    {
        get;
        set;
    }

    public EditContext EditConExperience
    {
        get;
        set;
    }

    public EditContext EditConNotes
    {
        get;
        set;
    }

    public EditContext EditConSkill
    {
        get;
        set;
    }

    private EducationPanel EducationPanel
    {
        get;
        set;
    }

    private ExperiencePanel ExperiencePanel
    {
        get;
        set;
    }

    /// <summary>
    ///     Gets or sets the name of the file being uploaded.
    /// </summary>
    /// <remarks>
    ///     This property is used to store the name of the file that is being uploaded in the `Candidate.UploadDocument()`
    ///     method.
    ///     It is then used in the `Candidate.SaveDocument()` method to add the file to the request for the API call.
    /// </remarks>
    private string FileName
    {
        get;
        set;
    }

    public bool FormattedExists
    {
        get;
        set;
    }

    /// <summary>
    ///     Gets or sets the instance of the Syncfusion grid component used to display candidates.
    /// </summary>
    /// <remarks>
    ///     This grid is used to display the list of candidates and their details.
    ///     It supports various operations such as sorting, filtering, and paging.
    /// </remarks>
    private SfGrid<Candidate> Grid
    {
        get;
        set;
    }

    /// <summary>
    ///     Gets or sets a value indicating whether the user has edit rights.
    /// </summary>
    /// <remarks>
    ///     This property is used to determine if the current user has the necessary permissions to edit the candidates.
    ///     It is set based on the user's claims retrieved during the initialization of the component.
    /// </remarks>
    private bool HasEditRights
    {
        get;
        set;
    } = true;

    private bool HasRendered { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether the user has view rights.
    /// </summary>
    /// <remarks>
    ///     This property is used to determine if the current user has the necessary permissions to view the candidates.
    ///     It is set based on the user's claims retrieved during the initialization of the component.
    /// </remarks>
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
    ///     Gets or sets the MIME type of the file being uploaded.
    /// </summary>
    /// <remarks>
    ///     This property is used to store the MIME type of the file being uploaded in the `Candidate.UploadDocument()` method.
    ///     The MIME type is retrieved from the FileInfo of the uploaded file.
    ///     It is then used as a parameter in the API request in the `Candidate.SaveDocument()` method.
    /// </remarks>
    private string Mime
    {
        get;
        set;
    }

    /// <summary>
    ///     Represents the date of the candidate's MPC.
    /// </summary>
    /// <remarks>
    ///     This property is a markup string that holds the date of the candidate's MPC.
    ///     The date is retrieved from the `_candidateMPCObject` collection and formatted.
    /// </remarks>
    private MarkupString MPCDate
    {
        get;
        set;
    }

    private MPCCandidateDialog MPCDialog
    {
        get;
        set;
    }

    /// <summary>
    ///     Represents the note of the candidate's MPC.
    /// </summary>
    /// <remarks>
    ///     This property is a markup string that holds the note of the candidate's MPC.
    ///     The note is retrieved from the `_candidateMPCObject` collection.
    /// </remarks>
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

    /// <summary>
    ///     Gets or sets a new instance of the CandidateDocuments class.
    /// </summary>
    private CandidateDocument NewDocument
    {
        get;
    } = new();

    /// <summary>
    ///     Gets or sets a list of KeyValues instances representing the next steps for the candidate.
    /// </summary>
    private List<KeyValues> NextSteps
    {
        get;
    } = [];

    private NotesPanel NotesPanel
    {
        get;
        set;
    }

    public bool OriginalExists
    {
        get;
        set;
    }

    /// <summary>
    ///     Represents the date of the candidate's rating.
    /// </summary>
    /// <remarks>
    ///     This property is a markup string that holds the date of the candidate's rating.
    ///     The date is retrieved from the `_candidateRatingObject` collection and formatted.
    /// </remarks>
    private MarkupString RatingDate
    {
        get;
        set;
    }

    private RatingCandidateDialog RatingDialog
    {
        get;
        set;
    }

    /// <summary>
    ///     Gets or sets the RatingMPC property of the Candidate class. This property represents an instance of the
    ///     CandidateRatingMPC class.
    /// </summary>
    private CandidateRatingMPC RatingMPC
    {
        get;
        set;
    } = new();

    /// <summary>
    ///     Represents the note of the candidate's rating.
    /// </summary>
    /// <remarks>
    ///     This property is a markup string that holds the note of the candidate's rating.
    ///     The note is retrieved from the `_candidateRatingObject` collection.
    /// </remarks>
    private MarkupString RatingNote
    {
        get;
        set;
    }

    /// <summary>
    ///     Gets or sets the Requisition ID for which to submit the Candidate for.
    /// </summary>
    private int RequisitionID
    {
        get;
        set;
    }

    private int RoleID
    {
        get;
        set;
    }

    /// <summary>
    ///     Gets or sets the search model used for filtering candidates.
    /// </summary>
    /// <remarks>
    ///     This model contains the search parameters used to filter the list of candidates displayed in the grid.
    ///     It includes properties such as Name and Page for pagination and filtering.
    /// </remarks>
    private CandidateSearch SearchModel
    {
        get;
        set;
    } = new();

    /// <summary>
    ///     Gets or sets the selected activity for the candidate.
    /// </summary>
    /// <value>
    ///     The selected activity is of type <see cref="CandidateActivity" /> and it represents the current
    ///     activity selected for the candidate.
    /// </value>
    private CandidateActivity SelectedActivity
    {
        get;
        set;
    } = new();

    /// <summary>
    ///     Gets or sets the selected document for download associated with a candidate.
    /// </summary>
    /// <remarks>
    ///     This property is used to store the document selected by the user for download.
    ///     The document is selected in the DownloadsPanel and used in the `DownloadDocument` method.
    /// </remarks>
    private CandidateDocument SelectedDownload
    {
        get;
        set;
    } = new();

    /// <summary>
    ///     Gets or sets the selected education for the candidate. This property is of type
    ///     <see cref="Subscription.Model.CandidateEducation" />.
    /// </summary>
    private CandidateEducation SelectedEducation
    {
        get;
        set;
    } = new();

    private CandidateEducation SelectedEducationClone
    {
        get;
        set;
    } = new();

    /// <summary>
    ///     Gets or sets the selected education for the candidate. This property is of type
    ///     <see cref="Subscription.Model.CandidateExperience" />.
    /// </summary>
    private CandidateExperience SelectedExperience
    {
        get;
        set;
    } = new();

    private CandidateNotes SelectedNotes
    {
        get;
        set;
    } = new();

    /// <summary>
    ///     Gets or sets the selected skill for the candidate.
    /// </summary>
    /// <value>
    ///     The selected skill of type <see cref="CandidateSkills" />.
    /// </value>
    private CandidateSkills SelectedSkill
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

    /// <summary>
    ///     Gets or sets the SkillPanel instance associated with the Candidate.
    /// </summary>
    /// <remarks>
    ///     This property is used to interact with the SkillPanel component, which provides functionality for managing
    ///     candidate skills.
    ///     It includes methods for editing and deleting skills, and properties for managing the display and behavior of the
    ///     skill grid.
    /// </remarks>
    private SkillPanel SkillPanel
    {
        get;
        set;
    }

    /// <summary>
    ///     Gets or sets the instance of the Syncfusion spinner control used in the application.
    /// </summary>
    /// <remarks>
    ///     This spinner is displayed during long-running operations such as data loading or saving.
    ///     It provides visual feedback to the user that an operation is in progress.
    /// </remarks>
    private SfSpinner Spinner
    {
        get;
        set;
    }

    /// <summary>
    ///     Gets or sets the username of the current user.
    /// </summary>
    /// <remarks>
    ///     This property is used to store the username of the user currently logged into the application.
    ///     It is retrieved from the user's claims during the initialization of the component.
    /// </remarks>
    private string User
    {
        get;
        set;
    }

    private bool VisibleSpin
    {
        get;
        set;
    }

    private EditActivityDialog DialogActivity
    {
        get;
        set;
    }

    /// <summary>
    ///     This method is used to add a new document to the candidate's profile.
    ///     It first checks if a new document instance exists, if not, it creates a new one.
    ///     If an instance already exists, it clears the data from the previous instance.
    ///     After preparing the new document instance, it opens the document dialog for the user to add the document details.
    /// </summary>
    /// <returns>A Task that represents the asynchronous operation.</returns>
    private Task AddDocument() => ExecuteMethod(() =>
                                                {
                                                    NewDocument.Clear();
                                                    return DialogDocument.ShowDialog();
                                                });

    private Task AllAlphabets(MouseEventArgs args) => ExecuteMethod(async () =>
                                                                    {
                                                                        SearchModel.Name = "";
                                                                        SearchModel.Page = 1;
                                                                        await SaveStorage();
                                                                    });

    private Task AutocompleteValueChange(ChangeEventArgs<string, KeyValues> filter) => ExecuteMethod(async () =>
                                                                                                     {
                                                                                                         SearchModel.Name = filter.Value;
                                                                                                         SearchModel.Page = 1;
                                                                                                         await SaveStorage();
                                                                                                     });

    /// <summary>
    ///     Clears the filter applied to the candidates.
    /// </summary>
    /// <remarks>
    ///     This function is called when the "Clear Filter" button is clicked.
    ///     It resets the filter values and reloads the candidates.
    /// </remarks>
    private Task ClearFilter() => ExecuteMethod(async () =>
                                                {
                                                    SearchModel.Clear();
                                                    SearchModel.User = User;
                                                    await SaveStorage();
                                                });

    private Dictionary<string, string> CreateParameters(int id) => new()
                                                                   {
                                                                       {"id", id.ToString()},
                                                                       {"candidateID", _target.ID.ToString()},
                                                                       {"user", User}
                                                                   };

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

    /// <summary>
    ///     Asynchronously deletes a document associated with a candidate.
    /// </summary>
    /// <param name="arg">The ID of the document to be deleted.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    /// <remarks>
    ///     This method sends a POST request to the Candidates/DeleteCandidateDocument endpoint with the document ID and user
    ///     ID as parameters.
    ///     If the action is successful, the candidate's documents are updated.
    /// </remarks>
    private Task DeleteDocument(int arg) => ExecuteMethod(async () =>
                                                          {
                                                              Dictionary<string, string> _parameters = new()
                                                                                                       {
                                                                                                           {"documentID", arg.ToString()},
                                                                                                           {"user", User}
                                                                                                       };

                                                              string _response = await General.ExecuteRest<string>("Candidate/DeleteCandidateDocument", _parameters);

                                                              _candidateDocumentsObject = General.DeserializeObject<List<CandidateDocument>>(_response);
                                                          });

    /// <summary>
    ///     Asynchronously deletes the education record of a candidate.
    /// </summary>
    /// <param name="id">The identifier of the education record to be deleted.</param>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    /// <remarks>
    ///     This method sends a POST request to the "Candidates/DeleteEducation" endpoint with the education record's ID, the
    ///     candidate's ID, and the user's ID as parameters.
    ///     If the action is not in progress, it sets the action in progress, sends the request, and updates the candidate's
    ///     education object with the response.
    ///     If the response is null, the method returns immediately. If an exception occurs during the request, it is caught
    ///     and ignored.
    ///     After the request is completed, the action progress is set to false.
    /// </remarks>
    private Task DeleteEducation(int id) => ExecuteMethod(async () =>
                                                          {
                                                              Dictionary<string, string> _parameters = CreateParameters(id);
                                                              string _response = await General.ExecuteRest<string>("Candidate/DeleteEducation", _parameters);

                                                              _candidateEducationObject = General.DeserializeObject<List<CandidateEducation>>(_response);
                                                          });

    /// <summary>
    ///     Asynchronously deletes the experience record of a candidate.
    /// </summary>
    /// <param name="id">The identifier of the education record to be deleted.</param>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    /// <remarks>
    ///     This method sends a POST request to the "Candidates/DeleteExperience" endpoint with the experience record's ID, the
    ///     candidate's ID, and the user's ID as parameters.
    ///     If the action is not in progress, it sets the action in progress, sends the request, and updates the candidate's
    ///     experience object with the response.
    ///     If the response is null, the method returns immediately. If an exception occurs during the request, it is caught
    ///     and ignored.
    ///     After the request is completed, the action progress is set to false.
    /// </remarks>
    private Task DeleteExperience(int id) => ExecuteMethod(async () =>
                                                           {
                                                               Dictionary<string, string> _parameters = CreateParameters(id);
                                                               string _response = await General.ExecuteRest<string>("Candidate/DeleteExperience", _parameters);

                                                               _candidateExperienceObject = General.DeserializeObject<List<CandidateExperience>>(_response);
                                                           });

    private Task DeleteNotes(int id) => ExecuteMethod(async () =>
                                                      {
                                                          Dictionary<string, string> _parameters = CreateParameters(id);
                                                          string _response = await General.ExecuteRest<string>("Candidate/DeleteNotes", _parameters);

                                                          _candidateNotesObject = General.DeserializeObject<List<CandidateNotes>>(_response);
                                                      });

    /// <summary>
    ///     Asynchronously deletes a skill from a candidate's profile.
    /// </summary>
    /// <param name="id">The unique identifier of the skill to be deleted.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    /// <remarks>
    ///     This method sends a POST request to the "Candidates/DeleteSkill" endpoint with the skill's ID,
    ///     the candidate's ID, and the current user's ID as parameters. If the current user is not logged in,
    ///     the user ID is set to "JOLLY". The method also sets a flag to prevent multiple simultaneous requests.
    ///     If the request is successful, the method updates the candidate's skills list with the response from the server.
    /// </remarks>
    private Task DeleteSkill(int id) => ExecuteMethod(async () =>
                                                      {
                                                          Dictionary<string, string> _parameters = CreateParameters(id);
                                                          string _response = await General.ExecuteRest<string>("Candidate/DeleteSkill", _parameters);

                                                          _candidateSkillsObject = General.DeserializeObject<List<CandidateSkills>>(_response);
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

                                 VisibleSpin = true;

                                 Dictionary<string, string> _parameters = new()
                                                                          {
                                                                              {"candidateID", _target.ID.ToString()},
                                                                              {"roleID", "RS"}
                                                                          };
                                 ReturnCandidateDetails _response = await General.ExecuteRest<ReturnCandidateDetails>("Candidate/GetCandidateDetails", _parameters,
                                                                                                                      null, false);

                                 _candidateDetailsObject = General.DeserializeObject<CandidateDetails>(_response.Candidate) ?? new();
                                 _candidateSkillsObject = General.DeserializeObject<List<CandidateSkills>>(_response.Skills) ?? [];
                                 _candidateEducationObject = General.DeserializeObject<List<CandidateEducation>>(_response.Education) ?? [];
                                 _candidateExperienceObject = General.DeserializeObject<List<CandidateExperience>>(_response.Experience) ?? [];
                                 _candidateNotesObject = General.DeserializeObject<List<CandidateNotes>>(_response.Notes) ?? [];
                                 _candidateDocumentsObject = General.DeserializeObject<List<CandidateDocument>>(_response.Documents) ?? [];
                                 _candidateActivityObject = General.DeserializeObject<List<CandidateActivity>>(_response.Activity) ?? [];

                                 _candidateRatingObject = _response.Rating;
                                 _candidateMPCObject = _response.MPC;
                                 RatingMPC = _response.RatingMPC;
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

                                 _selectedTab = _candidateActivityObject is {Count: > 0} ? 7 : 0;
                                 _formattedExists = _target.FormattedResume;
                                 _originalExists = _target.OriginalResume;

                                 await Task.Delay(100);
                                 VisibleSpin = false;
                             });
    }

    /// <summary>
    ///     Collapses the detail row in the Companies page grid view. This method is invoked from JavaScript.
    /// </summary>
    [JSInvokable("DetailCollapse")]
    public void DetailRowCollapse() => _target = null;

    /// <summary>
    ///     Initiates the download of a document associated with a candidate.
    /// </summary>
    /// <param name="arg">The identifier of the document to be downloaded.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    /// <remarks>
    ///     This method first checks if another download operation is in progress. If not, it sets the SelectedDownload
    ///     property to the document selected in the DownloadsPanel.
    ///     It then constructs a query string by concatenating the internal file name of the document, the ID of the target
    ///     candidate, the location of the document, and a zero, separated by "^" characters.
    ///     The query string is then Base64 encoded. The method then invokes a JavaScript function to open a new browser tab
    ///     with a URL constructed from the BaseUri of the NavigationManager, the string "Download/", and the encoded query
    ///     string.
    /// </remarks>
    private Task DownloadDocument(int arg) => ExecuteMethod(async () =>
                                                            {
                                                                SelectedDownload = DownloadsPanel.SelectedRow;
                                                                string _queryString = $"{SelectedDownload.InternalFileName}^{_target.ID}^{SelectedDownload.Location}^0".ToBase64String();
                                                                await JsRuntime.InvokeVoidAsync("open", $"{NavManager.BaseUri}Download/{_queryString}", "_blank");
                                                            });

    /// <summary>
    ///     Asynchronously edits the activity with the specified ID.
    ///     If the action is not in progress or is a speed dial, it sets the selected activity to the selected row of the
    ///     ActivityPanel,
    ///     clears the NextSteps list and adds a new "No Change" option. Then, it iterates through the workflows to find the
    ///     next steps
    ///     based on the status code of the selected activity. If a next step is found, it is added to the NextSteps list.
    ///     After all operations are done, it resets the action progress and shows the DialogActivity.
    /// </summary>
    /// <param name="id">The ID of the activity to be edited.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    private Task EditActivity(int id) => ExecuteMethod(async () =>
                                                       {
                                                           await Task.Yield();
                                                           SelectedActivity = ActivityPanel.SelectedRow;
                                                           NextSteps.Clear();
                                                           NextSteps.Add(new() {Text = "No Change", KeyValue = "0"});
                                                           try
                                                           {
                                                               /*foreach (string[] _next in _workflows.Where(flow => flow.Step == SelectedActivity.StatusCode)
                                                                                                    .Select(flow => flow.Next.Split(',')))
                                                               {
                                                                   foreach (string _nextString in _next)
                                                                   {
                                                                       foreach (KeyValues _status in _statusCodes.Where(status => status.Code == _nextString && status.AppliesToCode == "SCN"))
                                                                       {
                                                                           //NextSteps.Add(new(_status.Status, _nextString));
                                                                           break;
                                                                       }
                                                                   }

                                                                   break;
                                                               }*/
                                                           }
                                                           catch
                                                           {
                                                               //Ignore this error. No need to log this error.
                                                           }

                                                           await DialogActivity.ShowDialog();
                                                       });

    /// <summary>
    ///     Asynchronously edits the details of a candidate. If the candidate is not selected or is new,
    ///     it prepares the system to add a new candidate. Otherwise, it prepares the system to edit the existing candidate's
    ///     details.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    /// <remarks>
    ///     This method first checks if an action is already in progress or if the speed dial is active, if so it returns
    ///     immediately.
    ///     Then it shows a spinner indicating the action is in progress.
    ///     If the target candidate is null or new, it prepares the system to add a new candidate.
    ///     Otherwise, it prepares the system to edit the existing candidate's details.
    ///     Finally, it hides the spinner and shows the dialog to edit the candidate.
    /// </remarks>
    private Task EditCandidate() => ExecuteMethod(async () =>
                                                  {
                                                      VisibleSpin = true;

                                                      if (_target == null || _target.ID == 0)
                                                      {
                                                          if (_candidateDetailsObjectClone == null)
                                                          {
                                                              _candidateDetailsObjectClone = new();
                                                          }
                                                          else
                                                          {
                                                              _candidateDetailsObjectClone.Clear();
                                                          }

                                                          _candidateDetailsObjectClone.IsAdd = true;
                                                      }
                                                      else
                                                      {
                                                          _candidateDetailsObjectClone = _candidateDetailsObject.Copy();

                                                          _candidateDetailsObjectClone.IsAdd = false;
                                                      }

                                                      VisibleSpin = false;

                                                      await CandidateDialog.ShowDialog();
                                                      StateHasChanged();
                                                  });

    /// <summary>
    ///     Asynchronously edits the education details of a candidate. If the education record is not selected or is new,
    ///     it prepares the system to add a new education record. Otherwise, it prepares the system to edit the existing
    ///     education record.
    /// </summary>
    /// <param name="id">The ID of the education record to be edited. If the ID is 0, a new education record will be prepared.</param>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    /// <remarks>
    ///     This method first checks if an action is already in progress or if the speed dial is active, if so it returns
    ///     immediately.
    ///     Then it sets the action in progress.
    ///     If the target education record is null or new, it prepares the system to add a new education record.
    ///     Otherwise, it prepares the system to edit the existing education record.
    ///     Finally, it resets the action progress and shows the dialog to edit the education record.
    /// </remarks>
    private Task EditEducation(int id) => ExecuteMethod(async () =>
                                                        {
                                                            VisibleSpin = true;
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
                                                            VisibleSpin = false;
                                                            await CandidateEducationDialog.ShowDialog();
                                                        });

    /// <summary>
    ///     Asynchronously edits the experience details of a candidate. If the experience record is not selected or is new,
    ///     it prepares the system to add a new experience record. Otherwise, it prepares the system to edit the existing
    ///     experience record.
    /// </summary>
    /// <param name="id">
    ///     The ID of the experience record to be edited. If the ID is 0, a new experience record will be
    ///     prepared.
    /// </param>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    /// <remarks>
    ///     This method first checks if an action is already in progress or if the speed dial is active, if so it returns
    ///     immediately.
    ///     Then it sets the action in progress.
    ///     If the target experience record is null or new, it prepares the system to add a new experience record.
    ///     Otherwise, it prepares the system to edit the existing experience record.
    ///     Finally, it resets the action progress and shows the dialog to edit the experience record.
    /// </remarks>
    private Task EditExperience(int id) => ExecuteMethod(async () =>
                                                         {
                                                             VisibleSpin = true;
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
                                                             VisibleSpin = false;
                                                             await CandidateExperienceDialog.ShowDialog();
                                                         });

    private Task EditNotes(int id) => ExecuteMethod(async () =>
                                                    {
                                                        VisibleSpin = true;
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
                                                        VisibleSpin = false;
                                                        await CandidateNotesDialog.ShowDialog();
                                                    });

    /// <summary>
    ///     Asynchronously edits the skill of a candidate. If the skill is not selected or is new,
    ///     it prepares the system to add a new skill. Otherwise, it prepares the system to edit the existing skill.
    /// </summary>
    /// <param name="skill">The identifier of the skill to be edited.</param>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    /// <remarks>
    ///     This method first checks if an action is already in progress or if the speed dial is active, if so it returns
    ///     immediately.
    ///     Then it sets the action in progress.
    ///     If the target skill is null or new, it prepares the system to add a new skill.
    ///     Otherwise, it prepares the system to edit the existing skill.
    ///     Finally, it ends the action in progress and shows the dialog to edit the skill.
    /// </remarks>
    private Task EditSkill(int skill) => ExecuteMethod(async () =>
                                                       {
                                                           VisibleSpin = true;
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
                                                           VisibleSpin = false;
                                                           await CandidateSkillDialog.ShowDialog();
                                                       });

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

    /// <summary>
    ///     Handles the click event on the Formatted button in the Candidate page.
    ///     This method triggers the retrieval of the formatted resume of a candidate.
    /// </summary>
    /// <param name="arg">The mouse event arguments.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    private Task FormattedClick(MouseEventArgs arg) => GetResumeOnClick("Formatted");

    private async Task GetAlphabets(char alphabet) => await ExecuteMethod(async () =>
                                                                          {
                                                                              SearchModel.Name = alphabet.ToString();
                                                                              SearchModel.Page = 1;
                                                                              await SaveStorage();
                                                                          });

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
            return;
        }

        CandidateMPC _candidateMPCObjectFirst = _candidateMPCObject.MaxBy(x => x.DateTime);
        _mpcDate = $"{_candidateMPCObjectFirst.DateTime.CultureDate()} [{string.Concat(_candidateMPCObjectFirst.Name.Where(char.IsLetter))}]";

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
            return;
        }

        CandidateMPC _candidateMPCObjectFirst = _candidateMPCObject.MaxBy(x => x.DateTime);
        _mpcNote = _candidateMPCObjectFirst.Comment;

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
            return;
        }

        CandidateRating _candidateRatingObjectFirst = _candidateRatingObject.MaxBy(x => x.DateTime);
        _ratingDate = $"{_candidateRatingObjectFirst.DateTime.CultureDate()} [{string.Concat(_candidateRatingObjectFirst.Name.Where(char.IsLetter))}]";

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
            return;
        }

        CandidateRating _candidateRatingObjectFirst = _candidateRatingObject.MaxBy(x => x.DateTime);
        _ratingNote = _candidateRatingObjectFirst.Comment;

        RatingNote = _ratingNote.ToMarkupString();
    }

    /// <summary>
    ///     Handles the retrieval of a candidate's resume based on the specified resume type.
    /// </summary>
    /// <param name="resumeType">The type of the resume to retrieve. This can be "Original" or "Formatted".</param>
    /// <returns>A Task representing the asynchronous operation of retrieving the resume.</returns>
    /// <remarks>
    ///     This method sends a GET request to the "Candidates/DownloadResume" endpoint with the candidate's ID and the resume
    ///     type as query parameters.
    ///     If the request is successful, it shows the retrieved resume in the DownloadsPanel.
    /// </remarks>
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

                                                                                       await SaveStorage(false);
                                                                                   });

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            if (await SessionStorage.ContainKeyAsync(StorageName))
            {
                SearchModel = await SessionStorage.GetItemAsync<CandidateSearch>(StorageName);
            }
            else
            {
                SearchModel.Clear();
            }

            _query ??= new();
            _query = _query.AddParams("SearchModel", SearchModel);
            HasRendered = true;
            try
            {
                _initializationTaskSource.SetResult(true);
            }
            catch
            {
                //
            }
        }

        await base.OnAfterRenderAsync(firstRender);
    }

    /// <summary>
    ///     Handles the asynchronous initialization of the Candidates component.
    /// </summary>
    /// <remarks>
    ///     This method performs the following steps:
    ///     <list type="number">
    ///         <item>Initializes the task completion source.</item>
    ///         <item>Retrieves user claims and sets user permissions.</item>
    ///         <item>Fetches configuration data from the cache server.</item>
    ///         <item>Clears the search model.</item>
    ///     </list>
    ///     If the user is not authenticated, they are redirected to the login page.
    /// </remarks>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
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
                                    HasViewRights = _enumerable.Any(claim => claim.Type == "Permission" && claim.Value == "ViewAllCandidates");
                                    HasEditRights = _enumerable.Any(claim => claim.Type == "Permission" && claim.Value == "CreateOrEditCandidate");
                                    DownloadOriginal = _enumerable.Any(claim => claim.Type == "Permission" && claim.Value == "DownloadOriginal");
                                    DownloadFormatted = _enumerable.Any(claim => claim.Type == "Permission" && claim.Value == "DownloadFormatted");
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
                                    CacheObjects.Communications.ToString(), CacheObjects.DocumentTypes.ToString()
                                ];

                                RedisService _service = new(Start.CacheServer, Start.CachePort.ToInt32(), Start.Access, false);

                                Dictionary<string, string> _cacheValues = await _service.BatchGet(_keys);

                                // Deserialize configuration data into master objects
                                _roles = General.DeserializeObject<List<Role>>(_cacheValues[CacheObjects.Roles.ToString()]);
                                _states = General.DeserializeObject<List<IntValues>>(_cacheValues[CacheObjects.States.ToString()]);
                                _eligibility = General.DeserializeObject<List<IntValues>>(_cacheValues[CacheObjects.Eligibility.ToString()]);
                                _experience = General.DeserializeObject<List<IntValues>>(_cacheValues[CacheObjects.Experience.ToString()]);
                                _taxTerms = General.DeserializeObject<List<KeyValues>>(_cacheValues[CacheObjects.TaxTerms.ToString()]);
                                _jobOptions = General.DeserializeObject<List<KeyValues>>(_cacheValues[CacheObjects.JobOptions.ToString()]);
                                _statusCodes = General.DeserializeObject<List<KeyValues>>(_cacheValues[CacheObjects.StatusCodes.ToString()]);
                                _workflow = General.DeserializeObject<List<AppWorkflow>>(_cacheValues[CacheObjects.Workflow.ToString()]);
                                _communication = General.DeserializeObject<List<KeyValues>>(_cacheValues[CacheObjects.Communications.ToString()]);
                                _documentTypes = General.DeserializeObject<List<IntValues>>(_cacheValues[CacheObjects.DocumentTypes.ToString()]);
                            });

        await base.OnInitializedAsync();
    }

    /// <summary>
    ///     Handles the click event for retrieving the original resume of a candidate.
    /// </summary>
    /// <param name="arg">The Mouse Event Arguments associated with the click event.</param>
    /// <returns>A Task representing the asynchronous operation of retrieving the original resume.</returns>
    /// <remarks>
    ///     This method calls the GetResumeOnClick method with "Original" as the argument, which sends a GET request to the
    ///     "Candidates/DownloadResume" endpoint to retrieve the original resume.
    /// </remarks>
    private Task OriginalClick(MouseEventArgs arg) => GetResumeOnClick("Original");

    /// <summary>
    ///     Asynchronously saves the candidate details.
    /// </summary>
    /// <remarks>
    ///     This method performs the following steps:
    ///     - Yields the current thread of execution.
    ///     - Creates a new RestClient instance with the API host URL.
    ///     - Creates a new RestRequest instance for the "Candidates/SaveCandidate" endpoint, using the POST method and JSON
    ///     request format.
    ///     - Adds the cloned candidate details object to the request body as JSON.
    ///     - Adds the JSON file path, username, and email address as query parameters to the request. If the username or
    ///     email address is null or whitespace, default values are used.
    ///     - Sends the request asynchronously and awaits the response.
    ///     - Updates the candidate details object with the cloned object.
    ///     - Updates the target candidate's name, phone, email, location, updated date, and status based on the candidate
    ///     details object.
    ///     - Calls methods to set up the address, communication, eligibility, job option, tax term, and experience.
    ///     - Yields the current thread of execution again.
    ///     - Triggers a UI refresh by calling StateHasChanged.
    /// </remarks>
    private Task SaveCandidate() => ExecuteMethod(async () =>
                                                  {
                                                      Dictionary<string, string> _parameters = new()
                                                                                               {
                                                                                                   {"jsonPath", ""},
                                                                                                   {"userName", User},
                                                                                                   {"emailAddress", "maniv@titan-techs.com"}
                                                                                               };

                                                      await General.ExecuteRest<int>("Candidate/SaveCandidate", _parameters, _candidateDetailsObjectClone);

                                                      _candidateDetailsObject = _candidateDetailsObjectClone.Copy();
                                                      if (_candidateDetailsObject != null)
                                                      {
                                                          _target.Name = $"{_candidateDetailsObject.FirstName} {_candidateDetailsObject.LastName}";
                                                          _target.Phone = _candidateDetailsObject.Phone1.FormatPhoneNumber();
                                                          _target.Email = _candidateDetailsObject.Email;
                                                          _target.Location = $"{_candidateDetailsObject.City}, {SplitState(_candidateDetailsObject.StateID).Code}, {_candidateDetailsObject.ZipCode}";
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

    /// <summary>
    ///     Asynchronously saves the document related to a candidate.
    /// </summary>
    /// <param name="document">The edit context of the document to be saved.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    /// <remarks>
    ///     This method attempts to save a document related to a candidate. The document is represented by an EditContext.
    ///     If the model of the EditContext is a CandidateDocument, it will be uploaded to a specified API endpoint.
    ///     The method uses RestSharp to send a POST request to the API, including the document and additional parameters.
    ///     If the upload is successful, the response is deserialized into a list of CandidateDocument objects.
    /// </remarks>
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
                                                                                 _candidateDocumentsObject = General.DeserializeObject<List<CandidateDocument>>(_response);
                                                                             }
                                                                         }
                                                                     });

    /// <summary>
    ///     Asynchronously saves the education details of a candidate.
    /// </summary>
    /// <param name="education">
    ///     The edit context containing the candidate's education details.
    /// </param>
    /// <returns>
    ///     A task that represents the asynchronous operation.
    /// </returns>
    /// <remarks>
    ///     This method sends a POST request to the "Candidate/SaveEducation" endpoint with the candidate's education details.
    ///     The user ID of the logged-in user or "JOLLY" (if no user is logged in) and the candidate's ID are added as query
    ///     parameters to the request.
    ///     If the response is not null, the education details from the response are deserialized and stored in the
    ///     _candidateEducationObject.
    /// </remarks>
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

                                                                               _candidateEducationObject = General.DeserializeObject<List<CandidateEducation>>(_response);
                                                                           }
                                                                       });

    /// <summary>
    ///     Asynchronously saves the candidate's experience.
    /// </summary>
    /// <param name="experience">
    ///     The EditContext object containing the candidate's experience to be saved.
    /// </param>
    /// <returns>
    ///     A Task representing the asynchronous operation.
    /// </returns>
    /// <remarks>
    ///     This method sends a POST request to the "Candidates/SaveExperience" endpoint of the API.
    ///     The request includes the experience model in the body and user and candidateID as query parameters.
    ///     If the response is not null, it deserializes the "Experience" field of the response into a List of
    ///     CandidateExperience objects.
    /// </remarks>
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

                                                                                 _candidateExperienceObject = General.DeserializeObject<List<CandidateExperience>>(_response);
                                                                             }
                                                                         });

    /// <summary>
    ///     This method is used to save the CandidateMPC object. It makes a POST request to the "Candidates/SaveMPC" endpoint.
    ///     The method takes an EditContext object as a parameter, which is used to form the body of the POST request.
    ///     The user ID from the LoginCookyUser object is added as a query parameter to the request.
    ///     If the request is successful, the response is deserialized into a dictionary and used to update the
    ///     _candidateMPCObject and RatingMPC properties.
    ///     The GetMPCDate and GetMPCNote methods are then called to update the MPC date and note.
    /// </summary>
    /// <param name="editContext">The EditContext object that contains the data to be saved.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
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
                                                                               _candidateMPCObject = General.DeserializeObject<List<CandidateMPC>>(_response["MPCList"]);
                                                                               RatingMPC = JsonConvert.DeserializeObject<CandidateRatingMPC>(_response["FirstMPC"]?.ToString() ?? string.Empty);
                                                                               GetMPCDate();
                                                                               GetMPCNote();
                                                                           }
                                                                       }
                                                                   });

    /// <summary>
    ///     Asynchronously saves a note for a candidate.
    /// </summary>
    /// <param name="note">
    ///     The edit context containing the candidate's note details.
    /// </param>
    /// <returns>
    ///     A task that represents the asynchronous operation.
    /// </returns>
    /// <remarks>
    ///     This method sends a POST request to the "Candidate/SaveNotes" endpoint with the candidate's note details.
    ///     If the response is not null, the note details from the response are deserialized and stored in the
    ///     _candidateNotesObject.
    /// </remarks>
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

    /// <summary>
    ///     This method is responsible for saving the rating of a candidate.
    ///     It sends a POST request to the "Candidates/SaveRating" endpoint with the rating information.
    ///     If the operation is successful, it updates the candidate's rating information in the application.
    /// </summary>
    /// <param name="editContext">The context for the form that contains the rating information to be saved.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
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
                                                                                  _candidateRatingObject = General.DeserializeObject<List<CandidateRating>>(_response["RatingList"]);
                                                                                  RatingMPC = JsonConvert.DeserializeObject<CandidateRatingMPC>(_response["FirstRating"]?.ToString() ?? string.Empty);
                                                                                  _candidateDetailsObject.RateCandidate = RatingMPC.Rating.ToInt32();
                                                                                  GetRatingDate();
                                                                                  GetRatingNote();
                                                                              }
                                                                          }
                                                                      });

    /// <summary>
    ///     Asynchronously saves the skill of a candidate.
    /// </summary>
    /// <param name="skill">
    ///     The context of the skill to be saved.
    /// </param>
    /// <returns>
    ///     A Task representing the asynchronous operation.
    /// </returns>
    /// <remarks>
    ///     This method creates a new RestClient and RestRequest to make a POST request to the "Candidates/SaveSkill" endpoint.
    ///     The skill model is added to the request body in JSON format.
    ///     The user ID from the LoginCookyUser and the candidate ID are added as query parameters.
    ///     The response from the server is expected to be a dictionary containing the updated skills.
    ///     If the response is not null, it deserializes the "Skills" value from the response into a list of CandidateSkills
    ///     and assigns it to the _candidateSkillsObject.
    /// </remarks>
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

                                                                       _candidateSkillsObject = General.DeserializeObject<List<CandidateSkills>>(_response);
                                                                   }
                                                               });

    private async Task SaveStorage(bool refreshGrid = true)
    {
        await SessionStorage.SetItemAsync(StorageName, SearchModel);
        if (refreshGrid)
        {
            await Grid.Refresh(true);
        }
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
                                       ? _eligibility.FirstOrDefault(eligibility => eligibility.KeyValue == _candidateDetailsObject.EligibilityID)!.Text.ToMarkupString()
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
            CandidateExperience = (_candidateDetailsObject.ExperienceID > 0
                                       ? _experience.FirstOrDefault(experience => experience.KeyValue == _candidateDetailsObject.ExperienceID)!.Text
                                       : "").ToMarkupString();
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
                _generateAddress = SplitState(_candidateDetailsObject.StateID).Name; // _states.FirstOrDefault(state => state.Value == _candidateDetailsObject.StateID)?.Text?.Split('-')[0].Trim();
            }
            else
            {
                try //Because sometimes the default values are not getting set. It's so random that it can't be debugged. And it never fails during debugging session.
                {
                    _generateAddress += ", " + SplitState(_candidateDetailsObject.StateID)
                                           .Name; //_states.FirstOrDefault(state => state.Value == _candidateDetailsObject.StateID)?.Text?.Split('-')[0].Trim();
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

        if (_generateAddress is {Length: > 1} && _generateAddress.StartsWith(','))
        {
            _generateAddress = _generateAddress[1..].Trim();
        }

        Address = _generateAddress.ToMarkupString();
    }

    /// <summary>
    ///     Handles the event when a speed dial item is clicked.
    /// </summary>
    /// <param name="args">The arguments related to the speed dial item event.</param>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    /// <remarks>
    ///     This method first checks if an action is already in progress. If not, it sets the action in progress and the speed
    ///     dial to active.
    ///     Depending on the ID of the clicked speed dial item, it performs different actions such as editing a candidate,
    ///     rating, adding a skill, education, experience, notes, attachment, or resume.
    ///     After the action is performed, it sets the speed dial to inactive and ends the action in progress.
    /// </remarks>
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
        string[] parts = _stateName?.Split([" - "], StringSplitOptions.TrimEntries);
        if (parts?.Length != 2)
        {
            return ("", "");
        }

        // Remove the brackets from the code
        string _code = parts[0].Trim('[', ']');
        string _state = parts[1];

        return (_code, _state);
    }

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
                    CandidateSearch _searchModel = General.DeserializeObject<CandidateSearch>(dm.Params["SearchModel"].ToString());

                    (string _data, int _count) = await General.ExecuteRest<ReturnGrid>("Candidate/GetGridCandidates", null, _searchModel, false);

                    _dataSource = JsonConvert.DeserializeObject<List<Candidate>>(_data);

                    _candidateReturn = dm.RequiresCounts ? new DataResult
                                                           {
                                                               Result = _dataSource,
                                                               Count = _count /*_count*/
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

    private Task UndoActivity(int arg)
    {
        return null;
    }

    private Task SaveActivity(EditContext arg)
    {
        return Task.CompletedTask;
    }
}