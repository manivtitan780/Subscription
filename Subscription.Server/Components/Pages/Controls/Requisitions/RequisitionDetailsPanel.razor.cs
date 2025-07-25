#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.Server
// File Name:           RequisitionDetailsPanel.razor.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu, Brijesh Dubey
// Created On:          02-26-2025 16:02
// Last Updated On:     05-14-2025 21:11
// *****************************************/

#endregion

#region Using

using Syncfusion.Blazor.Calendars;

#endregion

namespace Subscription.Server.Components.Pages.Controls.Requisitions;

/// <summary>
///     Represents a panel for displaying and managing requisition details in the application.
/// </summary>
/// <remarks>
///     This class is a part of the ProfSvc_AppTrack.Pages.Controls.Requisitions namespace and is used to manage various
///     aspects of a requisition,
///     including associated companies, contacts, education details, eligibility criteria, experience requirements, job
///     options, recruiters, skills, and states.
///     It also provides event callbacks for handling actions such as Cancel, Save, File Upload, and StateID changes.
/// </remarks>
public partial class RequisitionDetailsPanel : IDisposable
{
    private bool _actionProgress;

    private bool _companyFirst = true;

    private bool _duration, _rate, _salary, _percent, _benefits, _hours, _expenses, _placementFee;

    // Removed: Unused _editContext field

    /// <summary>
    ///     Gets or sets the filtered list of company contacts associated with the requisition.
    /// </summary>
    /// <value>
    ///     The filtered list of company contacts.
    /// </value>
    /// <remarks>
    ///     This field is used to store the filtered list of company contacts based on certain criteria.
    ///     It is used in the RequisitionDetailsPanel for displaying only the relevant contacts to the user.
    /// </remarks>
    // Memory optimization: Initial capacity hint for filtered contacts (typically 3-8 contacts per company)
    private List<CompanyContacts> _filteredCompanyContacts = new(8);

    private readonly RequisitionDetailsValidator _requisitionDetailsValidator = new();

    /// <summary>
    ///     Represents a collection of toolbar items used in the RequisitionDetailsPanel.
    /// </summary>
    /// <remarks>
    ///     Each toolbar item in this collection corresponds to a specific command, such as Bold, Italic, Underline, etc.
    ///     These commands are used to manipulate the text in the RequisitionDetailsPanel.
    /// </remarks>
    private readonly List<ToolbarItemModel> _tools1 =
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
        new() {Command = ToolbarCommand.Redo}
    ];

    private bool _zipChanging;

    // Removed: Legacy AutoCompleteButton control - functionality moved to modern components

    /// <summary>
    ///     Gets or sets the event callback that is triggered when the Cancel action is performed.
    /// </summary>
    /// <value>
    ///     The event callback containing the MouseEventArgs associated with the Cancel action.
    /// </value>
    /// <remarks>
    ///     This property is used to manage the Cancel action in the RequisitionDetailsPanel.
    ///     The MouseEventArgs provides detailed information and functionality for the Cancel action.
    /// </remarks>
    [Parameter]
    public EventCallback<MouseEventArgs> Cancel { get; set; }

    /// <summary>
    ///     Gets or sets the list of companies associated with the requisition.
    /// </summary>
    /// <value>
    ///     The list of companies.
    /// </value>
    [Parameter]
    public List<Company> Companies { get; set; }

    /// <summary>
    ///     Gets or sets the list of company contacts associated with the requisition.
    /// </summary>
    /// <value>
    ///     The list of company contacts.
    /// </value>
    [Parameter]
    public List<CompanyContacts> CompanyContacts { get; set; }

    // Memory optimization: Using List with capacity hint instead of empty enumerable
    private IEnumerable<CompanyContacts> CompanyContactsFiltered { get; set; } = new List<CompanyContacts>(8);

    private EditContext Context { get; set; }

    /// <summary>
    ///     Gets or sets the CSS class name for the description field in the requisition details panel.
    /// </summary>
    /// <value>
    ///     The CSS class name. The default value is "success". It changes to "error" if there are any validation errors in the
    ///     description field.
    /// </value>
    /// <remarks>
    ///     This property is used to change the visual representation of the description field based on the validation state.
    /// </remarks>
    private string CssClassName { get; set; } = "success";

    /// <summary>
    ///     Gets or sets the Dialog property, which represents a dialog in the RequisitionDetailsPanel.
    /// </summary>
    /// <value>
    ///     The Dialog property gets/sets the value of a SfDialog object.
    /// </value>
    /// <remarks>
    ///     This property is used for managing the dialog display in the RequisitionDetailsPanel.
    ///     The SfDialog object provides detailed control over the dialog's behavior and appearance.
    /// </remarks>
    private SfDialog Dialog { get; set; }

    /// <summary>
    ///     Gets a list of duration codes. Each code is represented as a <see cref="KeyValues" /> object,
    ///     where the key is the code and the value is the description of the duration (e.g., "M" for months, "Y" for years).
    /// </summary>
    /// <remarks>
    ///     Memory optimization: Static readonly to prevent repeated allocations across component instances.
    /// </remarks>
    private static readonly KeyValues[] DurationCode =
        [new() {KeyValue = "D", Text = "Days"}, new() {KeyValue = "W", Text = "Weeks"}, new() {KeyValue = "M", Text = "Months"}, new() {KeyValue = "Y", Text = "Years"}];

    /// <summary>
    ///     Gets or sets the EditForm for the RequisitionDetailsPanel.
    ///     This form is used to edit the details of a requisition.
    /// </summary>
    private SfDataForm EditRequisitionForm { get; set; }

    /// <summary>
    ///     Gets or sets the education details for the requisition.
    /// </summary>
    /// <value>
    ///     The education details represented as a list of <see cref="IntValues" />.
    /// </value>
    [Parameter]
    public List<IntValues> Education { get; set; }

    /// <summary>
    ///     Gets or sets the eligibility criteria as a list of integer values.
    /// </summary>
    /// <value>
    ///     The eligibility criteria.
    /// </value>
    [Parameter]
    public List<IntValues> Eligibility { get; set; }

    /// <summary>
    ///     Gets or sets the experience requirements for the requisition.
    /// </summary>
    /// <value>
    ///     The experience requirements are represented as a list of <see cref="IntValues" />.
    /// </value>
    [Parameter]
    public List<IntValues> Experience { get; set; }

    /// <summary>
    ///     Gets or sets the job options for the requisition details panel.
    /// </summary>
    /// <value>
    ///     The job options are represented as a list of <see cref="KeyValues" /> instances.
    /// </value>
    [Parameter]
    public List<JobOptions> JobOptions { get; set; }

    /// <summary>
    ///     Gets or sets the JavaScript runtime instance.
    /// </summary>
    /// <value>
    ///     The JavaScript runtime instance used for invoking JavaScript functions from .NET code.
    /// </value>
    /// <remarks>
    ///     This property is injected and used to perform operations that require JavaScript interop.
    /// </remarks>
    [Inject]
    private IJSRuntime JsRuntime { get; set; }

    /// <summary>
    ///     Gets or sets the model for the Requisition Details Panel.
    /// </summary>
    /// <value>
    ///     The model representing the details of a requisition.
    /// </value>
    [Parameter]
    public RequisitionDetails Model { get; set; } = new();

    /// <summary>
    ///     Gets or sets the event callback that is triggered when a file is uploaded.
    /// </summary>
    /// <value>
    ///     The event callback containing the UploadChangeEventArgs associated with the file upload action.
    /// </value>
    /// <remarks>
    ///     This property is used to manage the file upload action in the RequisitionDetailsPanel.
    ///     The UploadChangeEventArgs provides detailed information and functionality for the file upload action.
    /// </remarks>
    [Parameter]
    public EventCallback<UploadChangeEventArgs> OnFileUpload { get; set; }

    /// <summary>
    ///     Gets the list of priority levels for the requisition.
    /// </summary>
    /// <value>
    ///     The list of priority levels, represented as <see cref="IntValues" />, where the integer key represents the priority
    ///     level and the string value represents the priority description.
    /// </value>
    /// <summary>
    /// Memory optimization: Static readonly to prevent repeated allocations across component instances.
    /// </summary>
    private static readonly List<IntValues> Priority = [new() {KeyValue = 0, Text = "Low"}, new() {KeyValue = 1, Text = "Medium"}, new() {KeyValue = 2, Text = "High"}];

    /// <summary>
    ///     Gets or sets the list of recruiters associated with the requisition.
    /// </summary>
    /// <value>
    ///     The list of recruiters, represented as instances of the KeyValues class.
    /// </value>
    [Parameter]
    public List<KeyValues> Recruiters { get; set; }

    /// <summary>
    ///     Gets or sets the event callback that is triggered when the Save action is performed.
    /// </summary>
    /// <value>
    ///     The event callback containing the EditContext associated with the Save action.
    /// </value>
    /// <remarks>
    ///     This property is used to manage the Save action in the RequisitionDetailsPanel.
    ///     The EditContext provides detailed information and functionality for the Save action.
    /// </remarks>
    [Parameter]
    public EventCallback<EditContext> Save { get; set; }

    /// <summary>
    ///     Gets or sets the list of skills associated with the requisition.
    /// </summary>
    /// <value>
    ///     The list of skills, each represented as an instance of the <see cref="IntValues" /> class.
    /// </value>
    [Parameter]
    public List<IntValues> Skills { get; set; }

    /// <summary>
    ///     Gets or sets the SfSpinner control of the RequisitionDetailsPanel.
    /// </summary>
    /// <value>
    ///     The SfSpinner control.
    /// </value>
    /// <remarks>
    ///     This property is used to display a spinner animation during asynchronous operations.
    /// </remarks>
    private SfSpinner Spinner { get; set; }

    /// <summary>
    ///     Gets or sets the list of states for the RequisitionDetailsPanel.
    /// </summary>
    /// <value>
    ///     The list of states, represented as instances of the IntValues class.
    /// </value>
    [Parameter]
    public List<StateCache> States { get; set; }

    /// <summary>
    ///     Gets or sets the title of the RequisitionDetailsPanel.
    /// </summary>
    /// <value>
    ///     The title of the RequisitionDetailsPanel.
    /// </value>
    /// <remarks>
    ///     This property is used to set the header of the RequisitionDetailsPanel dialog.
    /// </remarks>
    [Parameter]
    public string Title { get; set; }

    private bool VisibleSpinner { get; set; }

    /// <summary>
    ///     Handles the cancellation of the dialog.
    /// </summary>
    /// <param name="args">The mouse event arguments associated with the cancellation event.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    private async Task CancelDialog(MouseEventArgs args)
    {
        VisibleSpinner = true;
        await Cancel.InvokeAsync(args);
        await Dialog.HideAsync();
        VisibleSpinner = false;
    }

    // Removed: Legacy General.CallCancelMethod reference
    /// <summary>
    ///     Gets or sets the Redis service used for caching data.
    /// </summary>
    /// <value>
    ///     The Redis service.
    /// </value>
    /// <remarks>
    ///     This property is injected and used for operations like retrieving or storing data in Redis cache.
    /// </remarks>
    /// <summary>
    ///     Gets or sets the Redis service for cache operations.
    ///     Using DI singleton to avoid connection leaks.
    /// </summary>
    [Inject]
    private RedisService RedisService { get; set; }

    /// <summary>
    ///     Handles the event when the selected company changes in the Requisition Details Panel.
    /// </summary>
    /// <param name="company">
    ///     The ChangeEventArgs object containing the new selected company's ID and the company object
    ///     itself.
    /// </param>
    /// <returns>
    ///     A Task that represents the asynchronous operation.
    /// </returns>
    /// <remarks>
    ///     This method updates the CompanyContactsFiltered based on the new company's ID. If the company data is not null, it also
    ///     updates the Model's company-related properties.
    ///     It uses the memory cache to retrieve the company list and find the new company's details.
    /// </remarks>
    private async Task CompanyChanged(ChangeEventArgs<int, Company> company)
    {
        if (!company.Value.NullOrWhiteSpace())
        {
            _companyFirst = false;
            CompanyContactsFiltered = CompanyContacts.Where(x => x.CompanyID == company.Value).ToList();
            if (company.ItemData != null)
            {
                Model.CompanyName = company.ItemData.CompanyName;
                // Using injected RedisService singleton instead of creating new instances to avoid connection leaks
                RedisValue _cacheValues = await RedisService.GetAsync("Companies");
                string _returnValue = _cacheValues.ToString();

                List<Company> _companyList = null;
                if (!_returnValue.NullOrWhiteSpace())
                {
                    _companyList = General.DeserializeObject<List<Company>>(_returnValue);
                }

                if (_companyList is {Count: > 0})
                {
                    Company _company = _companyList.First(x => x.ID == company.ItemData.ID);
                    Model.CompanyCity = _company.City;
                    Model.CompanyState = _company.State;
                    Model.CompanyZip = _company.ZipCode;
                }
            }
        }
    }

    private void ContactChanged(ChangeEventArgs<int, CompanyContacts> arg) => Model.ContactName = arg.ItemData?.ContactName;

    private void ContactDataBound(DataBoundEventArgs args)
    {
        if (_companyFirst)
        {
            return;
        }

        Model.ContactID = CompanyContactsFiltered.Any() ? CompanyContactsFiltered.First().ID : 0;
    }

    // Removed: Legacy Context_OnFieldChanged - replaced with specific EditContext_OnFieldChanged handler

    /// <summary>
    ///     Asynchronously opens the dialog of the requisition details panel.
    /// </summary>
    /// <param name="args">The arguments for the BeforeOpen event of the dialog.</param>
    /// <remarks>
    ///     This method performs several actions:
    ///     - Yields the current task, allowing other tasks to execute.
    ///     - If no action is in progress, it sets the action progress flag and updates the CompanyContactsFiltered based on the CompanyID
    ///     of the Model.
    ///     - It validates the edit context of the RequisitionForm and subscribes to the OnFieldChanged event.
    ///     - It checks for validation messages for the Description field of the Model and sets the CssClassName accordingly.
    ///     - Finally, it signals the component to re-render and resets the action progress flag.
    /// </remarks>
    /// <returns>
    ///     A task that represents the asynchronous operation.
    /// </returns>
    private async Task DialogOpen(BeforeOpenEventArgs args)
    {
        await Task.Yield();
        if (!_actionProgress)
        {
            _actionProgress = true;
            CompanyContactsFiltered = CompanyContacts.Where(x => x.CompanyID == Model.CompanyID).ToList();
            if (CompanyContactsFiltered.Any())
            {
                foreach (CompanyContacts _contact in CompanyContactsFiltered)
                {
                    if (_contact.ContactName == Model.ContactName)
                    {
                        Model.ContactID = _contact.ID;
                        break;
                    }
                }

                /*if (CompanyContactsFiltered.Any(x => x.ContactName != Model.ContactName))
                {
                    Model.ContactID = CompanyContactsFiltered.First().ID;
                }*/
            }
            else
            {
                Model.ContactID = 0;
            }

            Context = EditRequisitionForm.EditContext;
            Context?.Validate();
            // Subscribe to field change events for Description validation styling
            // Memory optimization: Prevent multiple subscriptions by unsubscribing first
            if (Context != null)
            {
                Context.OnFieldChanged -= EditContext_OnFieldChanged; // Remove any existing subscription
                Context.OnFieldChanged += EditContext_OnFieldChanged; // Add new subscription
            }

            FieldIdentifier _fieldIdentifier = new(Model, nameof(Model.Description));
            IEnumerable<string> _errorMessages = Context?.GetValidationMessages(_fieldIdentifier);
            CssClassName = _errorMessages != null && _errorMessages.Any() ? "error" : "success";
            StateHasChanged();
            _actionProgress = false;
        }
    }

    // Removed: Duplicate ContactChanged method - active implementation at line 393

    /// <summary>
    ///     Handles the change of value in a dropdown.
    /// </summary>
    /// <param name="option">The option that has been changed, encapsulating the new value.</param>
    /// <param name="type">
    ///     The type of the dropdown that has been changed. 0 for Priority, 1 for Eligibility, 2 for Experience,
    ///     and 3 for Education.
    /// </param>
    /// <returns>
    ///     A Task representing the asynchronous operation.
    /// </returns>
    private async Task DropValueChanged(ChangeEventArgs<int, IntValues> option, byte type)
    {
        await Task.Yield();
        switch (type)
        {
            case 0:
                Model.Priority = option.ItemData.KeyValue.ToString();
                break;
            case 1:
                Model.Eligibility = option.ItemData.KeyValue.ToString();
                break;
            case 2:
                Model.Experience = option.ItemData.KeyValue.ToString();
                break;
            case 3:
                Model.Education = option.ItemData.KeyValue.ToString();
                break;
        }
    }

    /// <summary>
    ///     Handles the event when the due date value changes.
    /// </summary>
    /// <param name="args">The arguments containing the new due date.</param>
    /// <returns>
    ///     A task that represents the asynchronous operation.
    /// </returns>
    /// <remarks>
    ///     If the expected start date is not the minimum value and is greater than or equal to the new due date,
    ///     the expected start date is set to be 7 days before the new due date.
    ///     After the due date change, it notifies that the DueDate field has changed and requests a UI update.
    /// </remarks>
    private async Task DueDateValueChange(ChangedEventArgs<DateTime> args)
    {
        await Task.Yield();
        if (Model.ExpectedStart != DateTime.MinValue && Model.ExpectedStart >= args.Value)
        {
            Model.ExpectedStart = args.Value.AddDays(-7);
        }

        Context?.NotifyFieldChanged(Context.Field(nameof(Model.DueDate)));
        StateHasChanged();
    }

    /// <summary>
    ///     Handles the field changed event of the EditContext.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">An instance of <see cref="FieldChangedEventArgs" /> containing event data.</param>
    /// <remarks>
    ///     This method is triggered when a field in the EditContext changes. If the field that changed is not 'Description',
    ///     the method returns immediately.
    ///     If the 'Description' field is the one that changed, the method validates the field and updates the 'CssClassName'
    ///     property based on whether any validation errors exist.
    /// </remarks>
    private void EditContext_OnFieldChanged(object sender, FieldChangedEventArgs e)
    {
        if (e.FieldIdentifier.FieldName != "Description")
        {
            return;
        }

        FieldIdentifier _fieldIdentifier = new(Model, nameof(Model.Description));
        IEnumerable<string> _errorMessages = Context.GetValidationMessages(_fieldIdentifier);
        CssClassName = _errorMessages.Any() ? "error" : "success";
        StateHasChanged();
    }

    /// <summary>
    ///     Handles the change of the expected start date.
    /// </summary>
    /// <param name="args">The arguments of the change event, which contain the new date.</param>
    /// <returns>
    ///     A <see cref="Task" /> representing the asynchronous operation.
    /// </returns>
    /// <remarks>
    ///     If the due date is not the minimum value for <see cref="DateTime" /> and is less than or equal to the new start
    ///     date,
    ///     the due date is set to be one week after the new start date. After the due date is potentially changed,
    ///     the method notifies that the due date field has changed and requests a UI update.
    /// </remarks>
    private async Task ExpectedStartValueChange(ChangedEventArgs<DateTime> args)
    {
        await Task.Yield();
        if (Model.DueDate != DateTime.MinValue && Model.DueDate <= args.Value)
        {
            Model.DueDate = args.Value.AddDays(7);
        }

        Context?.NotifyFieldChanged(Context.Field(nameof(Model.DueDate)));
        StateHasChanged();
    }

    /// <summary>
    ///     Asynchronously hides the dialog.
    /// </summary>
    /// <remarks>
    ///     This method is used to hide the dialog of the RequisitionDetailsPanel component.
    /// </remarks>
    /// <returns>
    ///     A Task representing the asynchronous operation.
    /// </returns>
    public Task HideDialog() => Dialog.HideAsync();

    // Removed: Legacy JobOptionChange method - replaced with modern JobOptionChanged implementation

    private void JobOptionChanged(ChangeEventArgs<string, JobOptions> option)
    {
        _duration = option.ItemData.Duration;
        _rate = option.ItemData.Rate;
        _salary = option.ItemData.Sal;
        _percent = option.ItemData.ShowPercent;
        _benefits = option.ItemData.Benefits;
        _hours = option.ItemData.ShowHours;
        _expenses = option.ItemData.Exp;
        _placementFee = option.ItemData.PlaceFee;
    }

    /// <summary>
    ///     Ensures that only numeric input is allowed in the specified field.
    /// </summary>
    /// <param name="args">The event arguments associated with the input field.</param>
    /// <returns>
    ///     A Task that represents the asynchronous operation.
    /// </returns>
    private async Task NumbersOnly(object args) => await JsRuntime.InvokeVoidAsync("onCreate", "textDuration", true);

    /// <summary>
    ///     Executes after the component has been rendered.
    /// </summary>
    /// <param name="firstRender">
    ///     A boolean value that indicates whether this is the first time the component is being
    ///     rendered.
    /// </param>
    /// <remarks>
    ///     If this is not the first render and the Model's CompanyID is greater than 0,
    /// </remarks>
    protected override void OnAfterRender(bool firstRender)
    {
        if (!firstRender && Model is {CompanyID: > 0})
        {
            CompanyContactsFiltered = CompanyContacts.Where(x => x.CompanyID == Model.CompanyID).ToList();
        }

        base.OnAfterRender(firstRender);
    }

    /// <summary>
    ///     Handles the creation event.
    /// </summary>
    /// <param name="arg">The argument passed to the creation event.</param>
    /// <returns>A Task that represents the asynchronous operation.</returns>
    private async Task OnCreate(object arg) => await JsRuntime.InvokeVoidAsync("onCreate", "autoLocZip", true);

    protected override void OnParametersSet()
    {
        // Memory optimization: Explicit cleanup before creating new EditContext
        if (Context?.Model != Model)
        {
            Context = null;  // Immediate reference cleanup for GC
            Context = new(Model);
        }
        base.OnParametersSet();
    }

    /// <summary>
    ///     Asynchronously saves the changes made in the requisition details panel.
    /// </summary>
    /// <param name="editContext">The edit context associated with the save action.</param>
    /// <returns>
    ///     A task that represents the asynchronous operation.
    /// </returns>
    private async Task SaveRequisitionDialog(EditContext editContext)
    {
        VisibleSpinner = true;
        await Save.InvokeAsync(editContext);
        await Dialog.HideAsync();
        VisibleSpinner = false;
    }

    // Removed: Legacy General.CallSaveMethod reference
    /// <summary>
    ///     Asynchronously shows the dialog of the requisition details panel.
    /// </summary>
    /// <returns>
    ///     A task that represents the asynchronous operation.
    /// </returns>
    public Task ShowDialog() => Dialog.ShowAsync();

    /// <summary>
    /// Memory optimization: Clean disposal pattern for event handler cleanup
    /// </summary>
    public void Dispose()
    {
        // Clean up EditContext event handler subscription to prevent memory leaks
        if (Context != null)
        {
            Context.OnFieldChanged -= EditContext_OnFieldChanged;
        }
        GC.SuppressFinalize(this);
    }

    // Removed: Legacy tooltip handling - not currently in use

    // Removed: Legacy ZipChange method - ZIP code lookup functionality moved to autocomplete implementation

    // Removed: Legacy ZipDropDownAdaptor class - replaced with modern autocomplete implementation
}