#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.Server
// File Name:           EditCandidateDialog.razor.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu, Brijesh Dubey
// Created On:          02-06-2025 19:02
// Last Updated On:     05-06-2025 19:36
// *****************************************/

#endregion

#region Using

using System.Text.Json;

#endregion

namespace Subscription.Server.Components.Pages.Controls.Candidates;

/// <summary>
///     Represents a dialog for editing candidate details in the application.
/// </summary>
/// <remarks>
///     This class provides properties for various parameters required for editing candidate details, such as
///     communication, eligibility, experience, job options, tax terms, and more.
///     It also provides events for handling actions like saving changes or cancelling the edit operation.
///     The dialog is displayed when the `ShowDialog` method is called.
/// </remarks>
public partial class EditCandidateDialog
{
    private readonly CandidateDetailsValidator _candidateDetailsValidator = new();

    private string _existingValue = "";

    /// <summary>
    ///     A list of toolbar items for the rich text editor in the candidate edit dialog.
    /// </summary>
    /// <remarks>
    ///     This list includes commands for text formatting such as bold, italic, underline, strikethrough, lower case, upper
    ///     case, superscript, subscript, and clear format. It also includes commands for undo and redo actions.
    /// </remarks>
    private readonly List<ToolbarItemModel> _tools =
    [
        new() {Command = ToolbarCommand.Bold},
        new() {Command = ToolbarCommand.Italic},
        new() {Command = ToolbarCommand.Underline},
        new() {Command = ToolbarCommand.StrikeThrough},
        new() {Command = ToolbarCommand.LowerCase},
        new() {Command = ToolbarCommand.UpperCase},
        new() {Command = ToolbarCommand.SuperScript},
        new() {Command = ToolbarCommand.SubScript},
        new() {Command = ToolbarCommand.Separator},
        new() {Command = ToolbarCommand.ClearFormat},
        new() {Command = ToolbarCommand.Separator},
        new() {Command = ToolbarCommand.Undo},
        new() {Command = ToolbarCommand.Redo} /*new ToolbarItemModel() {Name = "Tokens",  TooltipText = "Insert Token"}*/
    ];

    /// <summary>
    ///     Gets or sets the event to be triggered when the cancel action is performed in the dialog.
    /// </summary>
    /// <remarks>
    ///     This event is triggered when the user clicks the cancel button in the dialog. The associated method will hide the
    ///     dialog, hide the spinner, and enable the dialog buttons.
    /// </remarks>
    [Parameter]
    public EventCallback<MouseEventArgs> Cancel { get; set; }

    /// <summary>
    ///     Gets or sets the collection of communication details for the candidate.
    /// </summary>
    /// <value>
    ///     The collection of `KeyValues` that represents the communication details for the candidate.
    /// </value>
    /// <remarks>
    ///     This property is used to populate the communication details in the edit dialog. Each `KeyValues` object in the
    ///     collection represents a specific communication detail.
    /// </remarks>
    [Parameter]
    public IEnumerable<KeyValues> Communication { get; set; }

    private string Content { get; set; } = "Generate Keywords / Summary";

    private EditContext Context { get; set; }

    /// <summary>
    ///     Gets or sets the instance of the Syncfusion Blazor Dialog component used in the EditCandidateDialog.
    /// </summary>
    /// <remarks>
    ///     This instance of the SfDialog component is used to display the dialog for editing candidate details.
    ///     It is shown when the `ShowDialog` method is called and hidden when the `CancelDialog` or `SaveCandidateDialog`
    ///     methods are called.
    /// </remarks>
    private SfDialog Dialog { get; set; }

    /// <summary>
    ///     Gets or sets the form used for editing candidate details.
    /// </summary>
    /// <value>
    ///     The form used for editing candidate details.
    /// </value>
    /// <remarks>
    ///     This property is used to manage the data and actions related to the form for editing candidate details.
    /// </remarks>
    private SfDataForm EditCandidateForm { get; set; }

    /// <summary>
    ///     Gets or sets the eligibility of the candidate.
    /// </summary>
    /// <value>
    ///     The eligibility of the candidate, represented as a collection of <see cref="IntValues" />.
    /// </value>
    [Parameter]
    public IEnumerable<IntValues> Eligibility { get; set; }

    /// <summary>
    ///     Gets or sets the collection of experience values for the candidate.
    /// </summary>
    /// <value>
    ///     The collection of experience values represented as instances of the IntValues class.
    /// </value>
    /// <remarks>
    ///     This property is used to populate the experience field in the edit candidate dialog.
    /// </remarks>
    [Parameter]
    public IEnumerable<IntValues> Experience { get; set; }

    private List<string> FieldTokens { get; set; } = ["One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine", "Ten"];

    private SfButton GenerateButton { get; set; }

    /// <summary>
    ///     Gets or sets the job options for the candidate.
    /// </summary>
    /// <value>
    ///     The job options are represented as a collection of `KeyValues` instances.
    ///     Each `KeyValues` instance represents a single job option.
    /// </value>
    [Parameter]
    public IEnumerable<KeyValues> JobOptions { get; set; }

    /// <summary>
    ///     Gets or sets the model representing the details of a candidate.
    /// </summary>
    /// <value>
    ///     The model is of type <see cref="CandidateDetails" />, which holds various details about a candidate.
    /// </value>
    /// <remarks>
    ///     This model is used to bind the data in the dialog for editing candidate details.
    ///     It is populated when the dialog is opened and updated when changes are saved.
    /// </remarks>
    [Parameter]
    public CandidateDetails Model { get; set; }

    /// <summary>
    ///     Gets or sets the event to be triggered when the save action is performed in the dialog.
    /// </summary>
    /// <remarks>
    ///     This event is triggered when the user clicks the save button in the dialog. The associated method will validate and
    ///     save the changes made to the candidate details, hide the dialog, hide the spinner, and enable the dialog buttons.
    /// </remarks>
    [Parameter]
    public EventCallback<EditContext> Save { get; set; }

    /// <summary>
    ///     Gets or sets the instance of the Syncfusion spinner control used in the dialog.
    /// </summary>
    /// <value>
    ///     The instance of the Syncfusion spinner control.
    /// </value>
    /// <remarks>
    ///     The spinner is used to indicate a loading state while the dialog is performing operations like saving changes or
    ///     cancelling the edit operation.
    /// </remarks>
    private SfSpinner Spinner { get; set; }

    /// <summary>
    ///     Gets or sets the list of states associated with the candidate.
    /// </summary>
    /// <value>
    ///     The list of states is represented as a collection of `IntValues` instances.
    /// </value>
    /// <remarks>
    ///     This property is used to populate the state selection in the edit dialog. Each `IntValues` instance represents a
    ///     state.
    /// </remarks>
    [Parameter]
    public List<StateCache> States { get; set; }

    /// <summary>
    ///     Gets or sets the collection of tax terms associated with the candidate.
    /// </summary>
    /// <value>
    ///     The collection of tax terms, represented as instances of <see cref="KeyValues" />.
    /// </value>
    /// <remarks>
    ///     This property is used to populate the tax terms section in the candidate editing dialog.
    /// </remarks>
    [Parameter]
    public IEnumerable<KeyValues> TaxTerms { get; set; }

    /// <summary>
    ///     Cancels the candidate editing operation and closes the dialog.
    /// </summary>
    /// <param name="args">The mouse event arguments associated with the cancel action.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    /// <remarks>
    ///     This method is triggered when the user clicks on the cancel button in the dialog.
    ///     It calls the `General.CallCancelMethod` to handle the cancellation and closing of the dialog.
    /// </remarks>
    private async Task CancelDialog(MouseEventArgs args)
    {
        await General.DisplaySpinner(Spinner);
        await Cancel.InvokeAsync(args);
        await Dialog.HideAsync();
        await General.DisplaySpinner(Spinner, false);
    }

    private async Task CheckZip(MaskBlurEventArgs args)
    {
        if (args.Value.Length != 5 || args.Value == _existingValue)
        {
            return;
        }

        Dictionary<string, string> _parameters = new()
                                                 {
                                                     {"zip", args.Value}
                                                 };

        string _response = await General.ExecuteRest<string>("Admin/CheckZip", _parameters, null, false);

        if (_response.NotNullOrWhiteSpace() && _response != "[]")
        {
            (string city, int state) = General.DeserializeObject<CityZip>(_response);
            Model.City = city;
            Model.StateID = state;
            Context.NotifyFieldChanged(Context.Field(nameof(Model.City)));
            Context.NotifyFieldChanged(Context.Field(nameof(Model.StateID)));
        }
    }

    private void Context_OnFieldChanged(object sender, FieldChangedEventArgs e) => Context.Validate();

    /// <summary>
    ///     Opens the dialog for editing candidate details.
    /// </summary>
    /// <remarks>
    ///     This method is called when the dialog is opened. It validates the form context of the dialog.
    /// </remarks>
    private void DialogOpen() => EditCandidateForm.EditContext?.Validate();

    [Inject]
    private SfDialogService DialogService { get; set; }

    private async Task GenerateSummary()
    {
        if (Model.TextResume.NullOrWhiteSpace())
        {
            await DialogService.AlertAsync("Please enter the resume text before generating the summary.", "Text Resume required.");
            return;
        }
        
        Content = "Generating…";
        RestClient client = new(Start.AzureOpenAIEndpoint);
        RestRequest request = new("", Method.Post);
        request.AddHeader("Content-Type", "application/json");
        request.AddHeader("api-key", Start.AzureOpenAIKey);
        var requestBody = new
                          {
                              messages = new[]
                                         {
                                             new {role = "system", content = "You are a concise resume summarizer."},
                                             new
                                             {
                                                 role = "user",
                                                 content = $"{Start.Prompt}{Model.TextResume}"
                                             }
                                         },
                              temperature = 0.3,
                              max_tokens = 1000
                          };

        request.AddJsonBody(requestBody);

        RestResponse response = await client.ExecuteAsync(request);

        if (!response.IsSuccessful)
        {
            throw new ApplicationException($"Error from Azure OpenAI: {response.StatusCode} - {response.Content}");
        }

        using JsonDocument _doc = JsonDocument.Parse(response.Content ?? string.Empty);
        string _content = _doc.RootElement.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString();
        if (_content != null)
        {
            JsonDocument _json = JsonDocument.Parse(_content);
            JsonElement _root = _json.RootElement;
            Model.Keywords = _root.GetProperty("Keywords").GetString();
            Model.Summary = _root.GetProperty("Summary").GetString();
            Model.Title = _root.GetProperty("Title").GetString();
            Model.FirstName = _root.GetProperty("FirstName").GetString();
            Model.LastName = _root.GetProperty("LastName").GetString();
            Model.Address1 = _root.GetProperty("Address").GetString();
            Model.City = _root.GetProperty("City").GetString();
            Model.ZipCode = _root.GetProperty("Zip").GetString();
            Model.Email = _root.GetProperty("Email").GetString();
            Model.Phone1 = _root.GetProperty("Phone").GetString();
        }

        Content = "Generate Keywords / Summary";
    }

    protected override void OnParametersSet()
    {
        Context = new(Model);
        Context.OnFieldChanged += Context_OnFieldChanged;
        base.OnParametersSet();
    }

    private void OpenDialog() => Context.Validate();

    /// <summary>
    ///     Asynchronously saves the changes made in the candidate dialog.
    /// </summary>
    /// <param name="editContext">
    ///     The edit context associated with the save action. This context contains the candidate details that are being
    ///     edited.
    /// </param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <remarks>
    ///     This method calls the `General.CallSaveMethod` utility method, passing in the edit context, spinner, dialog footer,
    ///     dialog, and save method.
    ///     The spinner is shown and the dialog buttons are disabled when the save operation starts, and hidden and enabled
    ///     respectively when the save operation completes.
    /// </remarks>
    private async Task SaveCandidateDialog(EditContext editContext)
    {
        await General.DisplaySpinner(Spinner);
        await Save.InvokeAsync(editContext);
        await Dialog.HideAsync();
        await General.DisplaySpinner(Spinner, false);
    }

    private void SaveValue(MaskFocusEventArgs args)
    {
        _existingValue = args.Value;
    }

    /// <summary>
    ///     Displays the dialog for editing candidate details.
    /// </summary>
    /// <remarks>
    ///     This method is responsible for displaying the dialog that allows users to edit candidate details.
    ///     It does this by calling the `ShowAsync` method on the `Dialog` instance of the `SfDialog` component.
    /// </remarks>
    public async Task ShowDialog() => await Dialog.ShowAsync();
}