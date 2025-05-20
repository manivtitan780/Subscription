#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.Server
// File Name:           SubmitCandidate.razor.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu, Brijesh Dubey
// Created On:          04-05-2025 16:04
// Last Updated On:     05-20-2025 15:26
// *****************************************/

#endregion

namespace Subscription.Server.Components.Pages.Controls.Candidates;

/// <summary>
///     Represents a dialog for submitting a candidate.
/// </summary>
/// <remarks>
///     This dialog is used to handle the submission process of a candidate.
///     It contains a text box for submission notes and a footer with cancel and submit options.
/// </remarks>
/// <explain>
///     This class represents a dialog used for submitting a candidate. It contains a text box for entering submission
///     notes and a footer with cancel and submit options. The dialog is modal, meaning it requires user interaction before
///     other parts of the application can be interacted with. The dialog is initially invisible and its visibility is
///     controlled programmatically. The dialog also includes validation for the text box, ensuring that the entered notes
///     are between 5 and 1000 characters long.
/// </explain>
public partial class SubmitCandidate
{
    private readonly SubmitCandidateRequisitionValidator _submitCandidateRequisitionValidator = new();

    /// <summary>
    ///     Gets or sets the cancel event callback that is invoked when the user clicks the cancel button in the
    ///     SubmitCandidate dialog.
    /// </summary>
    /// <value>
    ///     The cancel event callback.
    /// </value>
    /// <remarks>
    ///     This event callback is used to handle the cancellation of the candidate submission process.
    ///     It is invoked in the CancelDialog method, which is called when the user clicks the cancel button in the
    ///     SubmitCandidate dialog.
    ///     The CancelDialog method also hides the dialog and the spinner, and enables the dialog buttons.
    /// </remarks>
    [Parameter]
    public EventCallback<MouseEventArgs> Cancel { get; set; }

    private string Content { get; set; } = "Generate Summary";

    private EditContext Context { get; set; }

    /// <summary>
    ///     Gets or sets the dialog used for submitting a candidate.
    /// </summary>
    /// <value>
    ///     The dialog used for submitting a candidate.
    /// </value>
    /// <remarks>
    ///     This dialog is used to handle the submission process of a candidate. It contains a text box for entering submission
    ///     notes and a footer with cancel and submit options. The dialog is modal, meaning it requires user interaction before
    ///     other parts of the application can be interacted with. The dialog is initially invisible and its visibility is
    ///     controlled programmatically. The dialog also includes validation for the text box, ensuring that the entered notes
    ///     are between 5 and 1000 characters long.
    /// </remarks>
    private SfDialog Dialog { get; set; }

    private bool Disabled { get; set; }

    /// <summary>
    ///     Gets or sets the EditForm instance for the candidate submission dialog.
    /// </summary>
    /// <value>
    ///     The EditForm instance.
    /// </value>
    /// <remarks>
    ///     This EditForm instance is used to handle the form submission process within the candidate submission dialog.
    ///     It includes validation for the text box, ensuring that the entered notes are between 5 and 1000 characters long.
    /// </remarks>
    private SfDataForm EditSubmitForm { get; set; }

    /// <summary>
    ///     Gets or sets the model for the SubmitCandidate dialog.
    /// </summary>
    /// <value>
    ///     The model of type SubmitCandidateRequisition.
    /// </value>
    /// <remarks>
    ///     This model is used to bind the data in the SubmitCandidate dialog.
    ///     It contains a string property 'Text' which is used to hold the submission notes entered by the user.
    ///     The 'Text' property is validated to ensure that the entered notes are between 5 and 1000 characters long.
    /// </remarks>
    [Parameter]
    public SubmitCandidateRequisition Model { get; set; }

    /// <summary>
    ///     Gets or sets the save event callback that is invoked when the user submits a candidate.
    /// </summary>
    /// <value>
    ///     The save event callback.
    /// </value>
    /// <remarks>
    ///     This event callback is used to handle the submission process of a candidate.
    ///     It is invoked in the SubmitCandidateToRequisitionDialog method, which is called when the user submits a candidate.
    ///     The SubmitCandidateToRequisitionDialog method also calls the General.CallSaveMethod, which executes the provided
    ///     save method, shows the spinner, disables the dialog buttons, and then hides the spinner and dialog, and enables the
    ///     dialog buttons.
    /// </remarks>
    [Parameter]
    public EventCallback<EditContext> Save { get; set; }

    private bool VisibleSpinner { get; set; }

    private async Task CancelCandidateSubmit(MouseEventArgs args)
    {
        VisibleSpinner = true;
        await Cancel.InvokeAsync(args);
        await Dialog.HideAsync();
        VisibleSpinner = false;
    }

    private void Context_OnFieldChanged(object sender, FieldChangedEventArgs e) => Context.Validate();

    private async Task GenerateSummary()
    {
        Content = "Generating…";
        Disabled = true;
        Dictionary<string, string> _parameters = new() {{"candidateID", Model.CandidateID.ToString()}, {"requisitionID", Model.RequisitionID.ToString()}};
        string _response = await General.ExecuteRest<string>("Candidate/GenerateSummary", _parameters, null, false);
        if (_response.NotNullOrWhiteSpace())
        {
            Model.Text = _response;
            Context.NotifyFieldChanged(Context.Field(nameof(Model.Text)));
        }

        Disabled = false;
        Content = "Generate Summary";
    }

    protected override void OnParametersSet()
    {
        Context = new(Model);
        Context.OnFieldChanged += Context_OnFieldChanged;
        base.OnParametersSet();
    }

    /// <summary>
    ///     Opens the dialog for submitting a candidate.
    /// </summary>
    /// <remarks>
    ///     This method is responsible for opening the SubmitCandidate dialog.
    ///     It triggers the validation of the EditForm instance associated with the dialog.
    /// </remarks>
    /// <explain>
    ///     This method is part of the SubmitCandidate class, which represents a dialog used for submitting a candidate. The
    ///     OpenDialog method triggers the validation of the EditForm instance associated with the dialog. The EditForm
    ///     instance is used to handle the form submission process within the candidate submission dialog and includes
    ///     validation for the text box, ensuring that the entered notes are between 5 and 1000 characters long.
    /// </explain>
    private void OpenDialog() => Context.Validate();

    /// <summary>
    ///     Asynchronously submits a candidate to a requisition.
    /// </summary>
    /// <param name="editContext">The edit context associated with the submission action.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <remarks>
    ///     This method is responsible for handling the submission process of a candidate to a requisition.
    ///     It calls the General.CallSaveMethod, which executes the provided save method, shows the spinner,
    ///     disables the dialog buttons, and then hides the spinner and dialog, and enables the dialog buttons.
    /// </remarks>
    private async Task SaveCandidateSubmit(EditContext editContext)
    {
        VisibleSpinner = true;
        await Save.InvokeAsync(editContext);
        await Dialog.HideAsync();
        VisibleSpinner = false;
    }

    /// <summary>
    ///     Asynchronously shows the SubmitCandidate dialog.
    /// </summary>
    /// <returns>
    ///     A Task that represents the asynchronous operation.
    /// </returns>
    /// <remarks>
    ///     This method is used to display the SubmitCandidate dialog which is used for the submission process of a candidate.
    ///     The dialog is initially invisible and its visibility is controlled programmatically by this method.
    /// </remarks>
    public async Task ShowDialog() => await Dialog.ShowAsync();
}