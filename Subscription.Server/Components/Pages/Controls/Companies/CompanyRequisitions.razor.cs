#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            ProfSvc_AppTrack
// Project:             ProfSvc_AppTrack
// File Name:           CompanyRequisitions.razor.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja
// Created On:          12-09-2022 15:57
// Last Updated On:     09-28-2023 21:15
// *****************************************/

#endregion

#region Using

/*
using ActivityPanelRequisition = Profsvc_AppTrack.Client.Pages.Controls.Requisitions.ActivityPanelRequisition;
using AddRequisitionDocument = Profsvc_AppTrack.Client.Pages.Controls.Requisitions.AddRequisitionDocument;
using DocumentsPanel = Profsvc_AppTrack.Client.Pages.Controls.Requisitions.DocumentsPanel;
*/

#endregion

namespace Subscription.Server.Components.Pages.Controls.Companies;

/// <summary>
///     Represents a component that manages company requisitions.
/// </summary>
/// <remarks>
///     This class contains parameters for various data related to company requisitions, such as companies list, company
///     contacts, education, eligibility, experience, job options, model object, recruiters, role ID, row height, row
///     height activity, skills, states, status codes, user, and workflows. It also includes a dialog for editing
///     requisitions and methods for handling data and detail row collapse.
/// </remarks>
public partial class CompanyRequisitions
{
    private List<CandidateActivity> _candidateActivityObject = new();
    private RequisitionDetails _modelObjectClone = new();
    private MarkupString _requisitionDetailSkills = "".ToMarkupString();
    private RequisitionDetails _requisitionDetailsObject;
    private RequisitionDetails _requisitionDetailsObjectClone;
    private List<RequisitionDocuments> _requisitionDocumentsObject = new();
    private int _selectedReqTab;

    private List<IntValues> _states = new();

    private Requisition _targetRequisitions;

    private readonly List<ToolbarItemModel> _tools1 = new()
                                                      {
                                                          new()
                                                          {
                                                              Command = ToolbarCommand.Bold
                                                          },
                                                          new()
                                                          {
                                                              Command = ToolbarCommand.Italic
                                                          },
                                                          new()
                                                          {
                                                              Command = ToolbarCommand.Underline
                                                          },
                                                          new()
                                                          {
                                                              Command = ToolbarCommand.StrikeThrough
                                                          },
                                                          new()
                                                          {
                                                              Command = ToolbarCommand.LowerCase
                                                          },
                                                          new()
                                                          {
                                                              Command = ToolbarCommand.UpperCase
                                                          },
                                                          new()
                                                          {
                                                              Command = ToolbarCommand.SuperScript
                                                          },
                                                          new()
                                                          {
                                                              Command = ToolbarCommand.SubScript
                                                          },
                                                          new()
                                                          {
                                                              Command = ToolbarCommand.Separator
                                                          },
                                                          new()
                                                          {
                                                              Command = ToolbarCommand.ClearFormat
                                                          },
                                                          new()
                                                          {
                                                              Command = ToolbarCommand.Separator
                                                          },
                                                          new()
                                                          {
                                                              Command = ToolbarCommand.Undo
                                                          },
                                                          new()
                                                          {
                                                              Command = ToolbarCommand.Redo
                                                          }
                                                      };

    /// <summary>
    ///     Gets or sets the ActivityPanelRequisition component used for managing requisition activities within the company.
    /// </summary>
    /// <value>
    ///     The ActivityPanelRequisition component.
    /// </value>
    /// <remarks>
    ///     This property is used to interact with the ActivityPanelRequisition component, which provides functionality for
    ///     managing requisition activities. It includes parameters for various data related to requisition activities and
    ///     methods for handling these activities.
    /// </remarks>
    private ActivityPanelRequisition ActivityPanel
    {
        get;
        set;
    }

    /// <summary>
    ///     Gets the MemoryStream instance representing the document added to the company requisitions.
    /// </summary>
    /// <remarks>
    ///     This property is used to store the document data in memory after it has been uploaded.
    ///     It is used in the 'SaveDocument' method where the document data is sent to the server
    ///     and in the 'UploadDocument' method where the uploaded file's data is copied into this MemoryStream.
    /// </remarks>
    private MemoryStream AddedDocument
    {
        get;
    } = new();

    /// <summary>
    ///     Gets or sets the list of companies.
    /// </summary>
    /// <value>
    ///     The list of companies.
    /// </value>
    /// <remarks>
    ///     This parameter is used to store and retrieve the list of companies associated with the company requisitions.
    /// </remarks>
    [Parameter]
    public List<Company> CompaniesList
    {
        get;
        set;
    }

    /// <summary>
    ///     Gets or sets the list of company contacts.
    /// </summary>
    /// <value>
    ///     The list of company contacts.
    /// </value>
    /// <remarks>
    ///     This parameter is used to store and retrieve the list of contacts associated with the company requisitions.
    /// </remarks>
    [Parameter]
    public List<CompanyContacts> CompanyContacts
    {
        get;
        set;
    }

    /// <summary>
    ///     Gets or sets the dialog for editing activities related to company requisitions.
    /// </summary>
    /// <value>
    ///     The dialog for editing activities.
    /// </value>
    /// <remarks>
    ///     This property is of type <see cref="EditActivityDialog" />, which is used to show a dialog where the user can edit
    ///     the details of a candidate's activity.
    ///     The dialog is used in the context of both the `Candidate` and `CompanyRequisitions` pages.
    /// </remarks>
    private EditActivityDialog DialogActivity
    {
        get;
        set;
    }

    /// <summary>
    ///     Gets or sets the dialog for adding a requisition document.
    /// </summary>
    /// <value>
    ///     The dialog for adding a requisition document.
    /// </value>
    /// <remarks>
    ///     This property is used to manage the dialog that allows users to add new requisition documents to a company's
    ///     requisitions.
    /// </remarks>
    private AddRequisitionDocument DialogDocument
    {
        get;
        set;
    }

    /// <summary>
    ///     Gets or sets the dialog for editing requisitions.
    /// </summary>
    /// <value>
    ///     The dialog for editing requisitions.
    /// </value>
    /// <remarks>
    ///     This property is used to manage the dialog that allows users to edit requisitions within the company requisitions
    ///     component.
    /// </remarks>
    public RequisitionDetailsPanel DialogEditRequisition
    {
        get;
        set;
    }

    /// <summary>
    ///     Gets or sets the dialog for editing requisitions.
    /// </summary>
    /// <value>
    ///     The dialog for editing requisitions.
    /// </value>
    /// <remarks>
    ///     This property is used to manage the dialog that allows users to edit requisitions within the company requisitions
    ///     component.
    /// </remarks>
    private DocumentsPanel DocumentsPanel
    {
        get;
        set;
    }

    /// <summary>
    ///     Gets or sets the list of education requirements for company requisitions.
    /// </summary>
    /// <value>
    ///     The list of education requirements.
    /// </value>
    /// <remarks>
    ///     This parameter is used to store and retrieve the list of education requirements associated with the company
    ///     requisitions.
    ///     Each item in the list represents an education requirement as an integer value.
    /// </remarks>
    [Parameter]
    public List<IntValues> Education
    {
        get;
        set;
    }

    /// <summary>
    ///     Gets or sets the eligibility criteria for company requisitions.
    /// </summary>
    /// <value>
    ///     The eligibility criteria for company requisitions.
    /// </value>
    /// <remarks>
    ///     This parameter is used to store and retrieve the eligibility criteria associated with the company requisitions.
    ///     The eligibility criteria are represented as a list of <see cref="IntValues" />.
    /// </remarks>
    [Parameter]
    public List<IntValues> Eligibility
    {
        get;
        set;
    }

    /// <summary>
    ///     Gets or sets the experience values for the company requisitions.
    /// </summary>
    /// <value>
    ///     The experience values for the company requisitions.
    /// </value>
    /// <remarks>
    ///     This parameter is used to store and retrieve the experience values associated with the company requisitions.
    ///     The experience values are represented by a list of <see cref="IntValues" /> instances.
    /// </remarks>
    [Parameter]
    public List<IntValues> Experience
    {
        get;
        set;
    }

    /// <summary>
    ///     Gets or sets the name of the file to be uploaded.
    /// </summary>
    /// <value>
    ///     The name of the file to be uploaded.
    /// </value>
    /// <remarks>
    ///     This property is used to store the name of the file when a document is being uploaded in the `UploadDocument`
    ///     method.
    ///     It is then used in the `SaveDocument` method when making a POST request to the "Requisition/UploadDocument"
    ///     endpoint.
    /// </remarks>
    private string FileName
    {
        get;
        set;
    } = "";

    /// <summary>
    ///     Gets or sets a value indicating whether the component is being rendered for the first time.
    /// </summary>
    /// <value>
    ///     <c>true</c> if this is the first render; otherwise, <c>false</c>.
    /// </value>
    /// <remarks>
    ///     This property is used in the <see cref="DataHandler" /> method to control the behavior of the component during its
    ///     initial rendering.
    ///     When the component is rendered for the first time, this property is set to <c>true</c>, and then it is set to
    ///     <c>false</c> to prevent certain operations from being performed again in subsequent renderings.
    /// </remarks>
    private bool FirstRender
    {
        get;
        set;
    }

    /// <summary>
    ///     Gets or sets the inner grid of company requisitions.
    /// </summary>
    /// <value>
    ///     The inner grid of company requisitions.
    /// </value>
    /// <remarks>
    ///     This property is used to manage and manipulate the grid of company requisitions. It is used in various methods such
    ///     as data handling and detail row binding.
    /// </remarks>
    private SfGrid<Requisition> GridInnerRequisition
    {
        get;
        set;
    }

    /// <summary>
    ///     Gets or sets the list of job options.
    /// </summary>
    /// <value>
    ///     The list of job options.
    /// </value>
    /// <remarks>
    ///     This parameter is used to store and retrieve the list of job options associated with the company requisitions.
    ///     Each job option is represented by a KeyValues object.
    /// </remarks>
    [Parameter]
    public List<KeyValues> JobOptions
    {
        get;
        set;
    }

    /// <summary>
    ///     Gets or sets the JavaScript runtime instance.
    /// </summary>
    /// <value>
    ///     The JavaScript runtime instance.
    /// </value>
    /// <remarks>
    ///     This property is used to invoke JavaScript functions from the .NET environment. It is injected into the component
    ///     and can be used to perform operations that require direct interaction with the JavaScript runtime, such as opening
    ///     a new browser tab as shown in the DownloadDocument method.
    /// </remarks>
    [Inject]
    private IJSRuntime JsRuntime
    {
        get;
        set;
    }

    /// <summary>
    ///     Gets or sets the MIME type of the file being uploaded.
    /// </summary>
    /// <value>
    ///     The MIME type of the file.
    /// </value>
    /// <remarks>
    ///     This property is used to store and retrieve the MIME type of the file being uploaded in the company requisitions.
    ///     The MIME type is determined when a file is selected for upload and is used when making a POST request to upload the
    ///     file.
    /// </remarks>
    private string MIME
    {
        get;
        set;
    } = "";

    /// <summary>
    ///     Gets or sets the list of company requisitions.
    /// </summary>
    /// <value>
    ///     The list of requisitions of type <see cref="Requisition" />.
    /// </value>
    /// <remarks>
    ///     This property is used to hold the data related to the company requisitions. It is a parameter that can be supplied
    ///     when the component is used in a Razor page or component.
    /// </remarks>
    [Parameter]
    public List<Requisition> Model
    {
        get;
        set;
    }

    /// <summary>
    ///     Gets or sets the NavigationManager instance.
    /// </summary>
    /// <value>
    ///     The NavigationManager instance.
    /// </value>
    /// <remarks>
    ///     This property is used to manage navigation for the CompanyRequisitions component. It provides methods to navigate
    ///     to different URIs,
    ///     build URIs, and trigger URI change notifications. It is injected into the component via dependency injection.
    /// </remarks>
    [Inject]
    private NavigationManager NavManager
    {
        get;
        set;
    }

    /// <summary>
    ///     Gets an instance of the RequisitionDocuments class.
    /// </summary>
    /// <value>
    ///     An instance of the RequisitionDocuments class.
    /// </value>
    /// <remarks>
    ///     This property is used to manage the documents related to the company requisitions.
    /// </remarks>
    private RequisitionDocuments NewDocument
    {
        get;
    } = new();

    /// <summary>
    ///     Gets the list of next steps for the activity.
    /// </summary>
    /// <value>
    ///     The list of next steps.
    /// </value>
    /// <remarks>
    ///     This property is used to store the possible next steps that can be taken for an activity in the company
    ///     requisitions process.
    ///     Each step is represented as a `KeyValues` object. The list is initially empty and is populated when an activity is
    ///     being edited.
    /// </remarks>
    private List<KeyValues> NextSteps
    {
        get;
    } = new();

    /// <summary>
    ///     Gets or sets the list of recruiters.
    /// </summary>
    /// <value>
    ///     The list of recruiters.
    /// </value>
    /// <remarks>
    ///     This parameter is used to store and retrieve the list of recruiters associated with the company requisitions.
    /// </remarks>
    [Parameter]
    public List<KeyValues> Recruiters
    {
        get;
        set;
    }

    /// <summary>
    ///     Gets or sets the role ID associated with the company requisitions.
    /// </summary>
    /// <value>
    ///     The role ID as a string.
    /// </value>
    /// <remarks>
    ///     This parameter is used to store and retrieve the role ID associated with the company requisitions.
    ///     The role ID is used to determine the permissions and access level of the user within the company requisitions
    ///     component.
    /// </remarks>
    [Parameter]
    public string RoleID
    {
        get;
        set;
    }

    /// <summary>
    ///     Gets or sets the height of the row in the company requisitions list.
    /// </summary>
    /// <value>
    ///     The height of the row in pixels.
    /// </value>
    /// <remarks>
    ///     This parameter is used to control the display of the company requisitions list.
    ///     It affects the height of each row in the list, and therefore the overall layout and readability of the list.
    /// </remarks>
    [Parameter]
    public double RowHeight
    {
        get;
        set;
    }

    /// <summary>
    ///     Gets or sets the row height for the activity in the company requisitions.
    /// </summary>
    /// <value>
    ///     The height of the row for the activity in the company requisitions.
    /// </value>
    /// <remarks>
    ///     This parameter is used to manage the height of the row for the activity in the company requisitions component.
    ///     It is used in the rendering process of the component, specifically in the `BuildRenderTree` method.
    /// </remarks>
    [Parameter]
    public int RowHeightActivity
    {
        get;
        set;
    }

    /// <summary>
    ///     Gets or sets the selected activity of a candidate in the company requisitions.
    /// </summary>
    /// <value>
    ///     The selected activity of a candidate.
    /// </value>
    /// <remarks>
    ///     This property is used to store and retrieve the selected activity of a candidate when managing company
    ///     requisitions.
    ///     It is used in the EditActivity method to update the activity status and next steps based on the workflows.
    /// </remarks>
    private CandidateActivity SelectedActivity
    {
        get;
        set;
    } = new();

    /// <summary>
    ///     Gets or sets the selected requisition document for download.
    /// </summary>
    /// <value>
    ///     The selected requisition document of type <see cref="ProfSvc_Classes.RequisitionDocuments" />.
    /// </value>
    /// <remarks>
    ///     This property is used to hold the data related to the selected requisition document that the user wants to
    ///     download. It is set in the DownloadDocument method.
    /// </remarks>
    private RequisitionDocuments SelectedDownload
    {
        get;
        set;
    } = new();

    /// <summary>
    ///     Gets or sets the list of skills required for the company requisitions.
    /// </summary>
    /// <value>
    ///     The list of skills represented as a list of <see cref="IntValues" />.
    /// </value>
    /// <remarks>
    ///     This parameter is used to store and manage the skills required for the company requisitions.
    ///     Each skill is represented by an instance of the <see cref="IntValues" /> class.
    /// </remarks>
    [Parameter]
    public List<IntValues> Skills
    {
        get;
        set;
    }

    /// <summary>
    ///     Gets or sets the spinner control for the CompanyRequisitions component.
    /// </summary>
    /// <value>
    ///     The spinner control.
    /// </value>
    /// <remarks>
    ///     This property is used to control the visibility of a spinner during asynchronous operations.
    ///     The spinner is shown before the start of an operation and hidden after its completion.
    /// </remarks>
    private SfSpinner Spinner
    {
        get;
        set;
    }

    /// <summary>
    ///     Gets or sets the list of states associated with the company requisitions.
    /// </summary>
    /// <value>
    ///     The list of states.
    /// </value>
    /// <remarks>
    ///     This parameter is used to store and retrieve the list of states associated with the company requisitions. Each
    ///     state is represented by an instance of the IntValues class.
    /// </remarks>
    [Parameter]
    public List<IntValues> States
    {
        get;
        set;
    }

    /// <summary>
    ///     Gets or sets the list of status codes.
    /// </summary>
    /// <value>
    ///     The list of status codes.
    /// </value>
    /// <remarks>
    ///     This parameter is used to store and retrieve the list of status codes associated with the company requisitions.
    ///     Each status code is represented by an instance of the <see cref="StatusCode" /> class.
    /// </remarks>
    [Parameter]
    public List<StatusCode> StatusCodes
    {
        get;
        set;
    }

    /// <summary>
    ///     Gets or sets the title for the requisition.
    /// </summary>
    /// <value>
    ///     The title of the requisition.
    /// </value>
    /// <remarks>
    ///     This property is used to set the title of the requisition when editing a requisition.
    ///     It is also used to retrieve the current title of the requisition.
    /// </remarks>
    private string TitleRequisition
    {
        get;
        set;
    }

    /// <summary>
    ///     Gets or sets the username of the current user.
    /// </summary>
    /// <value>
    ///     The username of the current user. This value is used as a parameter in various API requests.
    /// </value>
    /// <remarks>
    ///     The username is converted to uppercase invariant before being used in API requests.
    /// </remarks>
    [Parameter]
    public string User
    {
        get;
        set;
    } = "JOLLY";

    /// <summary>
    ///     Gets or sets the list of application workflows.
    /// </summary>
    /// <value>
    ///     The list of application workflows.
    /// </value>
    /// <remarks>
    ///     This parameter is used to store and retrieve the list of workflows associated with the company requisitions.
    ///     Each workflow represents a sequence of steps or activities in the requisition process.
    /// </remarks>
    [Parameter]
    public List<Workflow> Workflows
    {
        get;
        set;
    } = new();

    /// <summary>
    ///     Asynchronously adds a new document to the company requisitions.
    /// </summary>
    /// <param name="arg">The mouse event arguments.</param>
    /// <remarks>
    ///     This method clears the data of the new document, then shows the dialog for adding a requisition document.
    /// </remarks>
    private async Task AddDocument(MouseEventArgs arg)
    {
        await Task.Yield();

        NewDocument.Clear();

        await DialogDocument.ShowDialog();
    }

     /*/// <summary>
    ///     Handles the event after a document is processed in the requisition document dialog.
    /// </summary>
    /// <param name="arg">
    ///     The event arguments of type <see cref="Syncfusion.Blazor.Inputs.ActionCompleteEventArgs" />.
    /// </param>
    /// <remarks>
    ///     This method is called after a document is processed in the requisition document dialog. It enables the buttons in
    ///     the dialog.
    /// </remarks>
   private void AfterDocument(ActionCompleteEventArgs arg)
    {
        DialogDocument.EnableButtons();
    }*/

    /*/// <summary>
    ///     Handles the event before a document upload.
    /// </summary>
    /// <param name="arg">
    ///     The arguments associated with the upload event.
    /// </param>
    /// <remarks>
    ///     This method is called before a document is uploaded. It disables the buttons in the
    ///     <see cref="AddRequisitionDocument" /> dialog to prevent further user interaction during the upload process.
    /// </remarks>
    private void BeforeDocument(BeforeUploadEventArgs arg)
    {
        DialogDocument.DisableButtons();
    }*/

    /// <summary>
    ///     Handles the data for the company requisitions component.
    /// </summary>
    /// <param name="obj">The object containing the data to be handled.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <remarks>
    ///     This method creates a .NET reference to the current instance of the <see cref="CompanyRequisitions" /> class and
    ///     sends it to the JavaScript side.
    ///     It also sets the <see cref="FirstRender" /> property to false and selects the first row of the
    ///     <see cref="GridInnerRequisition" /> if it contains any items.
    /// </remarks>
    private async Task DataHandler(object obj)
    {
        DotNetObjectReference<CompanyRequisitions> _dotNetReference = DotNetObjectReference.Create(this); // create dotnet ref
        await Runtime.InvokeAsync<string>("detail", _dotNetReference);
        //  send the dotnet ref to JS side
        FirstRender = false;
        //Count = Count;
        if (GridInnerRequisition.TotalItemCount > 0)
        {
            await GridInnerRequisition.SelectRowAsync(0);
        }
    }

    /// <summary>
    ///     Asynchronously deletes a document associated with a company requisition.
    /// </summary>
    /// <param name="args">
    ///     The ID of the document to be deleted.
    /// </param>
    /// <remarks>
    ///     This method sends a POST request to the "Requisition/DeleteRequisitionDocument" endpoint with the document ID and
    ///     user details as query parameters.
    ///     If the request is successful, it updates the list of requisition documents.
    /// </remarks>
    private async Task DeleteDocument(int args)
    {
        await Task.Yield();
        try
        {
            RestClient _client = new($"{Start.APIHost}");
            RestRequest _request = new("Requisition/DeleteRequisitionDocument", Method.Post)
                                   {
                                       RequestFormat = DataFormat.Json
                                   };
            _request.AddQueryParameter("documentID", args.ToString());
            _request.AddQueryParameter("user", User.ToUpperInvariant());

            Dictionary<string, object> _response = await _client.PostAsync<Dictionary<string, object>>(_request);
            if (_response == null)
            {
                return;
            }

            _requisitionDocumentsObject = General.DeserializeObject<List<RequisitionDocuments>>(_response["Document"]);
        }
        catch
        {
            //
        }

        await Task.Yield();
    }

    /// <summary>
    ///     Asynchronously binds data to the detail row of a requisition in the grid.
    /// </summary>
    /// <param name="requisition">The detail data bound event arguments containing the requisition data.</param>
    /// <remarks>
    ///     This method performs several operations:
    ///     - It checks if the target requisition is not null and if it's different from the current requisition data, it
    ///     expands or collapses the detail row accordingly.
    ///     - It sets the target requisition to the current requisition data.
    ///     - It gets the row index of the current requisition data and selects the row if it's not already selected.
    ///     - It sends a GET request to the "Requisition/GetRequisitionDetails" endpoint with the ID of the target requisition
    ///     as a query parameter.
    ///     - If the response is not null, it deserializes the requisition details, documents, and activity data and sets the
    ///     skills.
    ///     - It sets the selected requisition tab based on whether there are any candidate activities.
    /// </remarks>
    /// <returns>
    ///     A task that represents the asynchronous operation.
    /// </returns>
    private async Task DetailDataBind(DetailDataBoundEventArgs<Requisition> requisition)
    {
        //VisibleProperty = true;

        if (_targetRequisitions != null)
        {
            if (_targetRequisitions != requisition.Data) // return when target is equal to args.data
            {
                await GridInnerRequisition.ExpandCollapseDetailRowAsync(_targetRequisitions);
            }
        }

        _targetRequisitions = requisition.Data;

        int _index = await GridInnerRequisition.GetRowIndexByPrimaryKeyAsync(requisition.Data.ID);
        if (_index.ToInt64() != GridInnerRequisition.SelectedRowIndex.ToInt64())
        {
            await GridInnerRequisition.SelectRowAsync(_index);
        }

        await Task.Yield();
        await Spinner.ShowAsync();
        //await Task.Delay(1000);
        RestClient _restClient = new($"{Start.APIHost}");
        RestRequest request = new("Requisition/GetRequisitionDetails");
        request.AddQueryParameter("requisitionID", _targetRequisitions.ID);

        Dictionary<string, object> _restResponse = await _restClient.GetAsync<Dictionary<string, object>>(request);

        if (_restResponse != null)
        {
            _requisitionDetailsObject = JsonConvert.DeserializeObject<RequisitionDetails>(_restResponse["Requisition"]?.ToString() ?? string.Empty);
            _requisitionDocumentsObject = General.DeserializeObject<List<RequisitionDocuments>>(_restResponse["Documents"]);
            _candidateActivityObject = General.DeserializeObject<List<CandidateActivity>>(_restResponse["Activity"]);
            SetSkills();
        }

        _selectedReqTab = _candidateActivityObject.Count > 0 ? 2 : 0;

        await Spinner.HideAsync();
    }

    /// <summary>
    ///     Collapses the detail row in the company requisitions component.
    /// </summary>
    /// <remarks>
    ///     This method is invoked from JavaScript and sets the target requisition to null, effectively collapsing the detail
    ///     row.
    /// </remarks>
    [JSInvokable("DetailCollapse")]
    public void DetailRowCollapse() => _targetRequisitions = null;

    /// <summary>
    ///     Initiates the download of a selected requisition document.
    /// </summary>
    /// <param name="args">
    ///     The argument is not used in the current implementation.
    /// </param>
    /// <returns>
    ///     A task that represents the asynchronous operation.
    /// </returns>
    /// <remarks>
    ///     This method sets the `SelectedDownload` property to the `SelectedRow` of the `DocumentsPanel`. It then constructs a
    ///     query string
    ///     using the `DocumentFileName`, `ID`, and `OriginalFileName` of the `SelectedDownload` and the `ID` of the
    ///     `_targetRequisitions`.
    ///     The query string is Base64 encoded and appended to the base URI of the `NavManager` to form the download URL.
    ///     The method then invokes the JavaScript function `open` to open the download URL in a new browser tab.
    /// </remarks>
    private async Task DownloadDocument(int args)
    {
        await Task.Yield();
        SelectedDownload = DocumentsPanel.SelectedRow;
        string _queryString = (SelectedDownload.Location + "^" + _targetRequisitions.ID + "^" + SelectedDownload.InternalFileName + "^1").ToBase64String();
        //NavManager.NavigateTo(NavManager.BaseUri + "Download/" + _queryString);
        await JsRuntime.InvokeVoidAsync("open", $"{NavManager.BaseUri}Download/{_queryString}", "_blank");
        /*string _filePath = Path.Combine(Environment.WebRootPath, "Uploads", "Candidate", _target.ID.ToString(), SelectedDownload.InternalFileName).ToBase64String();
        byte[] _fileBytes = await File.ReadAllBytesAsync(_filePath);
        return File(_fileBytes, "application/force-download", _decodedStringArray[2]);*/
    }

    /// <summary>
    ///     Asynchronously edits the selected activity in the company requisitions.
    /// </summary>
    /// <param name="args">
    ///     The argument used in the editing process. Currently not used in the method.
    /// </param>
    /// <remarks>
    ///     This method is responsible for editing the selected activity in the company requisitions. It first clears the
    ///     `NextSteps` list and adds a default "No Change" option. Then, it iterates over the workflows, and for each workflow
    ///     that matches the status code of the selected activity, it splits the next steps into an array and adds them to the
    ///     `NextSteps` list. If an error occurs during this process, it is caught and ignored. Finally, it shows the activity
    ///     dialog.
    /// </remarks>
    /// <returns>
    ///     A <see cref="Task" /> representing the asynchronous operation.
    /// </returns>
    private async Task EditActivity(int args)
    {
        await Task.Yield();

        SelectedActivity = ActivityPanel.SelectedRow;
        NextSteps.Clear();
        NextSteps.Add(new("No Change", ""));
        try
        {
            foreach (string[] _next in Workflows.Where(flow => flow.Step == SelectedActivity.StatusCode).Select(flow => flow.Next.Split(',')))
            {
                foreach (string _nextString in _next)
                {
                    foreach (StatusCode _status in StatusCodes.Where(status => status.Code == _nextString && status.AppliesToCode == "SCN"))
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
    }

    /// <summary>
    ///     Initiates the process of editing a requisition.
    /// </summary>
    /// <param name="isAdd">
    ///     A boolean value indicating whether a new requisition is being added.
    ///     If true, a new requisition is being added; if false, an existing requisition is being edited.
    /// </param>
    /// <remarks>
    ///     This method is responsible for showing a spinner, setting the title of the requisition,
    ///     creating a copy of the requisition details, and showing the dialog for editing the requisition.
    ///     After the dialog is shown, the spinner is hidden.
    /// </remarks>
    private async Task EditRequisition(bool isAdd)
    {
        await Task.Yield();
        await Spinner.ShowAsync();

        TitleRequisition = "Edit";
        _requisitionDetailsObjectClone = _requisitionDetailsObject.Copy();

        StateHasChanged();
        await DialogEditRequisition.ShowDialog();
        await Spinner.HideAsync();
    }

    /// <summary>
    ///     Handles the action at the beginning of a process.
    /// </summary>
    /// <param name="arg">
    ///     The arguments related to the action, which includes information about the requisition.
    /// </param>
    /// <returns>
    ///     A task that represents the asynchronous operation.
    /// </returns>
    /// <remarks>
    ///     This method is called at the beginning of an action related to a requisition. It's an asynchronous method, meaning
    ///     it returns a Task to avoid blocking the main thread.
    /// </remarks>
    private async Task OnActionBegin(ActionEventArgs<Requisition> arg) => await Task.Yield();

    /// <summary>
    ///     Handles the completion of an action related to company requisitions.
    /// </summary>
    /// <param name="arg">
    ///     The event arguments containing information about the completed action on a requisition.
    /// </param>
    /// <returns>
    ///     A task that represents the asynchronous operation.
    /// </returns>
    /// <remarks>
    ///     This method is triggered when an action related to a company requisition, such as adding, editing, or deleting, is
    ///     completed.
    /// </remarks>
    private async Task OnActionComplete(ActionEventArgs<Requisition> arg) => await Task.Yield();

    /// <summary>
    ///     Asynchronously saves the activity of a company requisition.
    /// </summary>
    /// <param name="activity">
    ///     The edit context of the activity to be saved.
    /// </param>
    /// <remarks>
    ///     This method sends a POST request to the "Candidates/SaveCandidateActivity" endpoint with the activity model, user,
    ///     candidate ID, and a flag indicating whether the screen is a candidate screen. The response is expected to be a
    ///     dictionary containing the saved activity. If the response is null, the method returns without doing anything. If an
    ///     exception occurs during the process, it is caught and ignored.
    /// </remarks>
    /// <returns>
    ///     A task that represents the asynchronous operation.
    /// </returns>
    private async Task SaveActivity(EditContext activity)
    {
        await Task.Yield();

        try
        {
            RestClient _client = new($"{Start.APIHost}");
            RestRequest _request = new("Candidates/SaveCandidateActivity", Method.Post)
                                   {
                                       RequestFormat = DataFormat.Json
                                   };
            _request.AddJsonBody(activity.Model);
            _request.AddQueryParameter("user", User.ToUpperInvariant());
            _request.AddQueryParameter("candidateID", _targetRequisitions.ID);
            _request.AddQueryParameter("isCandidateScreen", false);

            Dictionary<string, object> _response = await _client.PostAsync<Dictionary<string, object>>(_request);
            if (_response == null)
            {
                return;
            }

            _candidateActivityObject = General.DeserializeObject<List<CandidateActivity>>(_response["Activity"]);
        }
        catch
        {
            //
        }

        await Task.Yield();
    }

    /// <summary>
    ///     Asynchronously saves a document related to a company requisition.
    /// </summary>
    /// <param name="document">The context of the document to be saved.</param>
    /// <remarks>
    ///     This method initiates an asynchronous operation that sends a POST request to the 'Requisition/UploadDocument'
    ///     endpoint.
    ///     The request includes multipart form data, which contains the document file and associated parameters such as
    ///     filename, mime type, document name, notes, requisition ID, user, and path.
    ///     If the request is successful, the response is deserialized into a list of 'RequisitionDocuments' objects.
    /// </remarks>
    /// <returns>A task that represents the asynchronous operation.</returns>
    private async Task SaveDocument(EditContext document)
    {
        await Task.Yield();
        try
        {
            if (document.Model is RequisitionDocuments _document)
            {
                RestClient _client = new($"{Start.APIHost}");
                RestRequest _request = new("Requisition/UploadDocument", Method.Post)
                                       {
                                           AlwaysMultipartFormData = true
                                       };
                _request.AddFile("file", AddedDocument.ToStreamByteArray(), FileName);
                _request.AddParameter("filename", FileName, ParameterType.GetOrPost);
                _request.AddParameter("mime", MIME, ParameterType.GetOrPost);
                _request.AddParameter("name", _document.Location, ParameterType.GetOrPost);
                _request.AddParameter("notes", _document.Notes, ParameterType.GetOrPost);
                _request.AddParameter("requisitionID", _targetRequisitions.ID.ToString(), ParameterType.GetOrPost);
                _request.AddParameter("user", User.ToUpperInvariant(), ParameterType.GetOrPost);
                _request.AddParameter("path", Start.UploadsPath, ParameterType.GetOrPost);

                Dictionary<string, object> _response = await _client.PostAsync<Dictionary<string, object>>(_request);
                if (_response == null)
                {
                    return;
                }

                _requisitionDocumentsObject = General.DeserializeObject<List<RequisitionDocuments>>(_response["Document"]);
            }
        }
        catch
        {
            //
        }

        await Task.Yield();
    }

    /// <summary>
    ///     Asynchronously saves the changes made to a requisition.
    /// </summary>
    /// <param name="arg">The context of the edit operation.</param>
    /// <remarks>
    ///     This method sends a POST request to the "Requisition/SaveRequisition" endpoint with the updated requisition
    ///     details.
    ///     It also manages the visibility and functionality of the dialog buttons during the save operation.
    /// </remarks>
    private async Task SaveRequisition(EditContext arg)
    {
        //SpinnerVisible = true;
        await Spinner.ShowAsync();
        // DialogEditRequisition.FooterDialog.DisableButtons();
        await Task.Yield();

        RestClient _client = new($"{Start.APIHost}");
        RestRequest _request = new("Requisition/SaveRequisition", Method.Post)
                               {
                                   RequestFormat = DataFormat.Json
                               };
        _request.AddJsonBody(_requisitionDetailsObjectClone);
        _request.AddQueryParameter("fileName", "");
        _request.AddQueryParameter("mimeType", "");

        await _client.PostAsync<int>(_request);
        _requisitionDetailsObject = _requisitionDetailsObjectClone.Copy();

        /*_target.Name = _candidateDetailsObject.FirstName + " " + _candidateDetailsObject.LastName;
        _target.Phone = $"{_candidateDetailsObject.Phone1.ToInt64(): (###) ###-####}";
        _target.Email = _candidateDetailsObject.Email;
        _target.Location = _candidateDetailsObject.City + ", " + GetState(_candidateDetailsObject.StateID) + ", " + _candidateDetailsObject.ZipCode;
        _target.Updated = DateTime.Today.CultureDate() + "[ADMIN]";
        _target.Status = "Available";
        SetupAddress();
        SetCommunication();
        SetEligibility();
        SetJobOption();
        SetTaxTerm();
        SetExperience();*/
        //SpinnerVisible = false;
        await Spinner.HideAsync();
        // DialogEditRequisition.FooterDialog.DisableButtons();
        await DialogEditRequisition.HideDialog();
        await Task.Yield();
        //VisibleCandidateInfo = false;
        StateHasChanged();
    }
    //private List<IntValues> _skills;

    /// <summary>
    ///     Sets the skills required and optional for the company requisitions.
    /// </summary>
    /// <remarks>
    ///     This method is used to set the skills required and optional for the company requisitions.
    ///     It splits the required and optional skills from the `RequisitionDetails` object and matches them with the `Skills`
    ///     list.
    ///     The matched skills are then formatted and converted to a `MarkupString` which is set to `_requisitionDetailSkills`.
    /// </remarks>
    private void SetSkills()
    {
        if (_requisitionDetailsObject == null || Skills == null)
        {
            return;
        }

        if (_requisitionDetailsObject.SkillsRequired.NullOrWhiteSpace() && _requisitionDetailsObject.Optional.NullOrWhiteSpace())
        {
            return;
        }

        _requisitionDetailSkills = "".ToMarkupString();

        string[] _skillRequiredStrings = { }, _skillOptionalStrings = { };
        if (_requisitionDetailsObject.SkillsRequired != "")
        {
            _skillRequiredStrings = _requisitionDetailsObject.SkillsRequired.Split(',');
        }

        if (_requisitionDetailsObject.Optional != "")
        {
            _skillOptionalStrings = _requisitionDetailsObject.Optional.Split(',');
        }

        string _skillsRequired = "", _skillsOptional = "";
        foreach (string _skillString in _skillRequiredStrings)
        {
            IntValues _skill = Skills.FirstOrDefault(skill => skill.KeyValue == _skillString.ToInt32());
            if (_skill == null)
            {
                continue;
            }

            if (_skillsRequired == "")
            {
                _skillsRequired = _skill.Text;
            }
            else
            {
                _skillsRequired += ", " + _skill.Text;
            }
        }

        foreach (string _skillString in _skillOptionalStrings)
        {
            IntValues _skill = Skills.FirstOrDefault(skill => skill.KeyValue == _skillString.ToInt32());
            if (_skill == null)
            {
                continue;
            }

            if (_skillsOptional == "")
            {
                _skillsOptional = _skill.Text;
            }
            else
            {
                _skillsOptional += ", " + _skill.Text;
            }
        }

        string _skillStringTemp = "";

        if (!_skillsRequired.NullOrWhiteSpace())
        {
            _skillStringTemp = "Required Skills: <br/>" + _skillsRequired + "<br/>";
        }

        if (!_skillsOptional.NullOrWhiteSpace())
        {
            _skillStringTemp += "Optional Skills: <br/>" + _skillsOptional;
        }

        _requisitionDetailSkills = _skillStringTemp.ToMarkupString();
    }

    /// <summary>
    ///     Handles the event when a speed dial item is clicked.
    /// </summary>
    /// <param name="args">The arguments related to the speed dial item event.</param>
    /// <remarks>
    ///     This method is triggered when a speed dial item is clicked. It performs different actions based on the ID of the
    ///     clicked item.
    ///     If the "itemEditRequisition" is clicked, it initiates the process of editing a requisition.
    ///     If the "itemAddDocument" is clicked, it initiates the process of adding a new document to the company requisitions.
    ///     For all other items, it initiates the process of submitting a candidate.
    /// </remarks>
    private async Task SpeedDialItemClicked(SpeedDialItemEventArgs args)
    {
        await Task.Yield();
        switch (args.Item.ID)
        {
            case "itemEditRequisition":
                await EditRequisition(false);
                break;
            case "itemAddDocument":
                await AddDocument(null);
                break;
            default:
                await SubmitCandidate(null);
                break;
        }
    }

    /// <summary>
    ///     Initiates the process of submitting a candidate for a requisition.
    /// </summary>
    /// <param name="arg">The mouse event arguments. This parameter is not currently used in the method.</param>
    /// <remarks>
    ///     This method navigates to the candidate submission page for the target requisition. The requisition ID and company
    ///     ID are passed as query parameters in the URL.
    ///     The navigation is forced, meaning the current page state will be discarded.
    /// </remarks>
    private async Task SubmitCandidate(MouseEventArgs arg)
    {
        await Task.Yield();
        NavManager.NavigateTo($"{NavManager.BaseUri}candidate?requisition={_targetRequisitions.ID}&company=1", true);
    }

 /// <summary>
///     Handles the event when a tab is selected in the company requisitions component.
/// </summary>
/// <param name="args">
///     Contains information about the selected tab.
/// </param>
/// <remarks>
///     This method is invoked when a tab is selected in the company requisitions component. It updates the 
///     <see cref="_selectedReqTab"/> field with the index of the selected tab.
/// </remarks>
  private async Task TabSelected(SelectEventArgs args)
    {
        await Task.Yield();
        _selectedReqTab = args.SelectedIndex;
    }

  /// <summary>
///     Asynchronously undoes a specific activity related to a company requisition.
/// </summary>
/// <param name="activityID">
///     The ID of the activity to be undone.
/// </param>
/// <remarks>
///     This method sends a POST request to the "Candidates/UndoCandidateActivity" endpoint with the activity ID and user information.
///     If the request is successful, it updates the candidate activity object with the response data.
/// </remarks>
  private async Task UndoActivity(int activityID)
    {
        await Task.Yield();

        try
        {
            RestClient _client = new($"{Start.APIHost}");
            RestRequest _request = new("Candidates/UndoCandidateActivity", Method.Post)
                                   {
                                       RequestFormat = DataFormat.Json
                                   };
            _request.AddQueryParameter("submissionID", activityID);
            _request.AddQueryParameter("user", User.ToUpperInvariant());
            _request.AddQueryParameter("isCandidateScreen", false);

            Dictionary<string, object> _response = await _client.PostAsync<Dictionary<string, object>>(_request);
            if (_response == null)
            {
                return;
            }

            _candidateActivityObject = General.DeserializeObject<List<CandidateActivity>>(_response["Activity"]);
        }
        catch
        {
            //
        }

        await Task.Yield();
    }

  /// <summary>
///     Asynchronously uploads a document file.
/// </summary>
/// <param name="file">
///     The file to be uploaded, represented as an instance of <see cref="UploadChangeEventArgs"/>.
/// </param>
/// <remarks>
///     This method reads the file stream from the uploaded file, copies it to a target stream, and then closes the source stream.
///     It also sets the file name and MIME type of the uploaded document.
/// </remarks>
  private async Task UploadDocument(UploadChangeEventArgs file)
    {
        await Task.Yield();
        foreach (UploadFiles _file in file.Files)
        {
            Stream _str = _file.File.OpenReadStream(20 * 1024 * 1024);
            await _str.CopyToAsync(AddedDocument);
            FileName = _file.FileInfo.Name;
            MIME = _file.FileInfo.MimeContentType;
            AddedDocument.Position = 0;
            _str.Close();
        }
    }
}