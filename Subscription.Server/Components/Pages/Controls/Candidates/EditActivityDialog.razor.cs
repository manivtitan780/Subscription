#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.Server
// File Name:           EditActivityDialog.razor.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu, Brijesh Dubey
// Created On:          03-04-2025 20:03
// Last Updated On:     05-21-2025 15:00
// *****************************************/

#endregion

namespace Subscription.Server.Components.Pages.Controls.Candidates;

/// <summary>
///     Represents a dialog for editing candidate activities.
/// </summary>
/// <remarks>
///     This class is used in the context of both the `Candidate` and `CompanyRequisitions` pages.
///     It provides functionality to show a dialog where the user can edit the details of a candidate's activity.
///     The class contains several parameters including `Cancel`, `IsCandidate`, `Model`, `ModelSteps`, `Save`, `Status`,
///     and `StatusCodes`.
/// </remarks>
public partial class EditActivityDialog : IDisposable
{
    /// <summary>
    ///     Gets or sets the event callback that is invoked when the cancel action is triggered.
    /// </summary>
    /// <value>
    ///     The event callback for the cancel action.
    /// </value>
    /// <remarks>
    ///     This event callback is used to handle the cancellation of the dialog. It is invoked in the `CancelDialog` method
    ///     where it is passed to the `CallCancelMethod` of the `General` class. The `CallCancelMethod` then invokes this
    ///     callback asynchronously, hides the spinner and the dialog, and enables the dialog buttons.
    /// </remarks>
    [Parameter]
    public EventCallback<MouseEventArgs> Cancel { get; set; }

    private EditActivityValidator CandidateActivityValidator { get; } = new();

    private EditContext Context { get; set; }

    /// <summary>
    ///     Gets or sets the Syncfusion Blazor Dialog control used in the EditActivityDialog component.
    /// </summary>
    /// <value>
    ///     The Syncfusion Blazor Dialog control.
    /// </value>
    /// <remarks>
    ///     This property is used to control the visibility and behavior of the dialog in the EditActivityDialog component.
    ///     It is used in the `ShowDialog` method to display the dialog and in the `CancelDialog` method to hide the dialog.
    ///     The dialog's reference is set in the `BuildRenderTree` method of the EditActivityDialog component.
    /// </remarks>
    private SfDialog Dialog { get; set; }

    /// <summary>
    ///     Represents the form used for editing a candidate's activity within the dialog.
    /// </summary>
    /// <remarks>
    ///     This form is used within the `EditActivityDialog` to capture and validate the user's input when editing a
    ///     candidate's activity.
    ///     The form is bound to the `Model` property of the `EditActivityDialog` and uses data annotations for validation.
    /// </remarks>
    private SfDataForm EditActivityForm { get; set; }

    /// <summary>
    ///     Gets or sets the list of interview types available for selection in the activity dialog.
    /// </summary>
    /// <value>
    ///     The list of interview types is represented as a list of <see cref="KeyValues" /> instances. Each instance
    ///     represents a type of interview with a key as the interview type name and a value as its short form.
    ///     The list includes types such as "In-Person Interview", "Telephonic Interview", "Others", and "None".
    /// </value>
    private IEnumerable<KeyValues> InterviewTypes { get; } =
    [
        new() {Text = "In-Person Interview", KeyValue = "I"}, new() {Text = "Telephonic Interview", KeyValue = "P"}, new() {Text = "Others", KeyValue = "O"}, new() {Text = "None", KeyValue = ""}
    ];

    /// <summary>
    ///     Gets or sets a value indicating whether the dialog is for a candidate.
    /// </summary>
    /// <value>
    ///     true if this dialog is for a candidate; otherwise, false. The default value is true.
    /// </value>
    /// <remarks>
    ///     This property is used to determine the context in which the dialog is being used. If `IsCandidate` is true,
    ///     the dialog is being used in the context of a candidate's activity. If `IsCandidate` is false, the dialog is
    ///     being used in a different context.
    /// </remarks>
    [Parameter]
    public bool IsCandidate { get; set; } = true;

    /// <summary>
    ///     Gets or sets a value indicating whether the dialog should be displayed in full screen mode.
    /// </summary>
    /// <value>
    ///     <c>true</c> if the dialog should be displayed in full screen mode; otherwise, <c>false</c>.
    /// </value>
    /// <remarks>
    ///     This property is used to control the height and minimum height of the dialog. When set to <c>true</c>, the dialog's
    ///     height and minimum height are set to "98vh", otherwise they are set to "460px".
    /// </remarks>
    private bool IsShow { get; set; }

    /// <summary>
    ///     Gets or sets the maximum date for the interview scheduling.
    /// </summary>
    /// <value>
    ///     The maximum date is set to three months from today's date during initialization.
    ///     This property is used in the `DateTimeControl` in the `EditActivityDialog` Razor component to limit the selection
    ///     of interview dates.
    /// </value>
    private DateTime Max { get; set; }

    /// <summary>
    ///     Gets or sets the minimum date for the interview scheduling.
    /// </summary>
    /// <value>
    ///     The minimum date is set to one month before today's date during initialization.
    ///     This property is used in the `DateTimeControl` in the `EditActivityDialog` Razor component to limit the selection
    ///     of interview dates.
    /// </value>
    private DateTime Min { get; set; }

    /// <summary>
    ///     Gets or sets the model representing a candidate's activity.
    /// </summary>
    /// <value>
    ///     The model is of type <see cref="CandidateActivity" />, which contains the details of a candidate's activity.
    /// </value>
    /// <remarks>
    ///     This model is used as a data source for the form in the dialog, and its properties are bound to various controls in
    ///     the form.
    /// </remarks>
    [Parameter]
    public CandidateActivity Model { get; set; } = new();

    /// <summary>
    ///     Gets or sets the list of key-value pairs representing the steps in the model.
    /// </summary>
    /// <value>
    ///     The list of key-value pairs representing the steps in the model.
    /// </value>
    /// <remarks>
    ///     This property is used as a data source for the drop-down control in the dialog, allowing the user to set a new
    ///     status for the activity.
    ///     Each key-value pair in the list represents a possible status that the activity can have.
    /// </remarks>
    [Parameter]
    public List<KeyValues> ModelSteps { get; set; }

    /// <summary>
    ///     Gets or sets the event callback that is invoked when the save action is triggered.
    /// </summary>
    /// <value>
    ///     The event callback for the save action.
    /// </value>
    /// <remarks>
    ///     This event callback is used to handle the saving of the dialog. It is invoked in the `SaveActivityDialog` method
    ///     where it is passed to the `CallSaveMethod` of the `General` class. The `CallSaveMethod` then invokes this
    ///     callback asynchronously, hides the spinner and the dialog, and enables the dialog buttons.
    /// </remarks>
    [Parameter]
    public EventCallback<EditContext> Save { get; set; }

    /// <summary>
    ///     Gets or sets the list of application workflow steps.
    /// </summary>
    /// <value>
    ///     The list of application workflow steps.
    /// </value>
    /// <remarks>
    ///     This property represents the workflow steps that an activity can go through in the application.
    ///     Each item in the list is an instance of the `AppWorkflow` class, which encapsulates the details of a workflow step.
    ///     This list is used in the `ChangeStatus` method to determine whether to show certain UI elements based on the
    ///     current workflow step.
    /// </remarks>
    [Parameter]
    public List<Workflow> Status { get; set; } = [];

    /// <summary>
    ///     Gets or sets the list of application workflow steps.
    /// </summary>
    /// <value>
    ///     The list of application workflow steps.
    /// </value>
    /// <remarks>
    ///     This property represents the workflow steps that an activity can go through in the application.
    ///     Each item in the list is an instance of the `AppWorkflow` class, which encapsulates the details of a workflow step.
    ///     This list is used in the `ChangeStatus` method to determine whether to show certain UI elements based on the
    ///     current workflow step.
    /// </remarks>
    [Parameter]
    public List<StatusCode> StatusCodes { get; set; }

    private bool VisibleSpinner { get; set; }

    public void Dispose()
    {
        if (Context is not null)
        {
            Context.OnFieldChanged -= Context_OnFieldChanged;
        }

        GC.SuppressFinalize(this);
    }

    /// <summary>
    ///     Cancels the current dialog operation.
    /// </summary>
    /// <param name="args">The mouse event arguments.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <remarks>
    ///     This method is invoked when the user clicks the cancel button in the dialog.
    ///     It hides the dialog and stops any ongoing operations.
    /// </remarks>
    private async Task CancelActivityDialog(MouseEventArgs args)
    {
        VisibleSpinner = true;
        await Cancel.InvokeAsync(args);
        await Dialog.HideAsync();
        VisibleSpinner = false;
    }

    /// <summary>
    ///     Changes the status of the activity.
    /// </summary>
    /// <param name="status">
    ///     The new status to be set, encapsulated in a `ChangeEventArgs` object which also contains the previous status.
    /// </param>
    /// <remarks>
    ///     This method is asynchronous and is primarily used to update the UI based on the new status.
    ///     It first sets `IsShow` to false, then checks if the new status requires the dialog to be shown.
    ///     If so, `IsShow` is set to true. Finally, it triggers a UI update by calling `StateHasChanged`.
    /// </remarks>
    private void ChangeStatus(ChangeEventArgs<string, KeyValues> status)
    {
        if (Status != null)
        {
            IsShow = Status.Any(workflow => workflow.Step == status.Value && workflow.Schedule);
        }
    }

    private void Context_OnFieldChanged(object sender, FieldChangedEventArgs e) => Context?.Validate();

    /// <summary>
    ///     Asynchronously initializes the dialog after all parameters have been set.
    /// </summary>
    /// <returns>
    ///     A <see cref="Task" /> representing the asynchronous operation.
    /// </returns>
    /// <remarks>
    ///     This method is called when the component is first initialized. It sets the minimum and maximum dates for the
    ///     interview scheduling.
    ///     The minimum date is set to one month before the current date, and the maximum date is set to three months after the
    ///     current date.
    /// </remarks>
    protected override void OnInitialized()
    {
        Min = DateTime.Today.AddMonths(-1);
        Max = DateTime.Today.AddMonths(3);
        base.OnInitialized();
    }

    protected override void OnParametersSet()
    {
        // Memory optimization: Only create new EditContext if Model reference changed
        if (Context?.Model != Model)
        {
            // Dispose previous context event handler to prevent memory leaks
            if (Context != null)
            {
                Context.OnFieldChanged -= Context_OnFieldChanged;
            }
            Context = new(Model);
            Context.OnFieldChanged += Context_OnFieldChanged;
        }
        base.OnParametersSet();
    }

    /// <summary>
    ///     Opens the dialog for editing a candidate's activity.
    /// </summary>
    /// <remarks>
    ///     This method is responsible for opening the `EditActivityDialog`. It is triggered when the dialog is about to be
    ///     opened.
    ///     It validates the `EditContext` of the `EditActivityForm`, ensuring that the form is in a valid state before the
    ///     dialog is opened.
    /// </remarks>
    private void OpenDialog()
    {
        Model.NewStatusCode = "0";
        Context.Validate();
    }

    /// <summary>
    ///     Asynchronously saves the changes made in the EditActivityDialog.
    /// </summary>
    /// <param name="editContext">
    ///     The edit context associated with the save action. This context contains the model being edited.
    /// </param>
    /// <returns>
    ///     A task that represents the asynchronous operation.
    /// </returns>
    /// <remarks>
    ///     This method is responsible for saving the changes made in the EditActivityDialog. It calls the `CallSaveMethod`
    ///     of the `General` class, passing in the `editContext`, `Spinner`, `FooterDialog`, `Dialog`, and `Save` callback.
    ///     The `CallSaveMethod` then executes the provided save method, shows the spinner, disables the dialog buttons,
    ///     and then hides the spinner and dialog, and enables the dialog buttons.
    /// </remarks>
    private async Task SaveActivityDialog(EditContext editContext)
    {
        VisibleSpinner = true;
        await Save.InvokeAsync(editContext);
        await Dialog.HideAsync();
        VisibleSpinner = false;
    }

    /// <summary>
    ///     Asynchronously shows the dialog for editing a candidate's activity.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <remarks>
    ///     This method is used to display the dialog for editing a candidate's activity. It is invoked in the `EditActivity`
    ///     methods
    ///     of the `Candidate`, `CompanyRequisitions`, and `Requisition` classes. The method uses the `ShowAsync` method of the
    ///     `Syncfusion Blazor Dialog` control to display the dialog.
    /// </remarks>
    public async Task ShowDialog() => await Dialog.ShowAsync();
}