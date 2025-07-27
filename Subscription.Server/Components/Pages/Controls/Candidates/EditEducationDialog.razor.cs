#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.Server
// File Name:           EditEducationDialog.razor.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu, Brijesh Dubey
// Created On:          12-02-2024 14:12
// Last Updated On:     07-26-2025 15:27
// *****************************************/

#endregion

namespace Subscription.Server.Components.Pages.Controls.Candidates;

/// <summary>
///     Represents a dialog for editing the education details of a candidate.
/// </summary>
/// <remarks>
///     This class is a Blazor component, which is used in the Candidate page. It provides functionality to edit the
///     education details of a candidate.
///     It contains parameters for handling the events of saving and canceling the edit operation, and a model representing
///     the candidate's education details.
///     It also contains a method to show the dialog.
/// </remarks>
public partial class EditEducationDialog
{
    private readonly CandidateEducationValidator _candidateEducationValidator = new();

	/// <summary>
	///     Gets or sets the event callback that is invoked when the cancel action is triggered in the dialog.
	/// </summary>
	/// <remarks>
	///     This event callback is used to handle the cancel action, which typically involves hiding the dialog and performing
	///     any necessary cleanup.
	///     The cancel action is usually triggered by a user interaction, such as clicking a cancel button.
	/// </remarks>
	[Parameter]
    public EventCallback<MouseEventArgs> Cancel { get; set; }

    private EditContext Context { get; set; }

	/// <summary>
	///     Gets or sets the dialog control of the component.
	/// </summary>
	/// <value>
	///     The dialog of type <see cref="SfDialog" /> which provides a modal dialog for editing the education details of a
	///     candidate.
	/// </value>
	/// <remarks>
	///     This property is used to control the visibility and the content of the dialog.
	///     It is bound to the SfDialog component in the Razor markup.
	/// </remarks>
	private SfDialog Dialog { get; set; }

	/// <summary>
	///     Gets or sets the EditForm instance used for editing the education details of a candidate.
	/// </summary>
	/// <value>
	///     The EditForm of type <see cref="EditForm" /> which encapsulates the form used for editing the education details.
	/// </value>
	/// <remarks>
	///     This property is used to manage the form fields and handle the form submission for editing the education details of
	///     a candidate.
	///     It is also used to perform form validation when the dialog is opened.
	/// </remarks>
	private SfDataForm EditEducationForm { get; set; }

	/// <summary>
	///     Gets or sets the JavaScript runtime instance for the component.
	/// </summary>
	/// <value>
	///     The instance of type <see cref="IJSRuntime" /> which provides a way to run JavaScript code from .NET.
	/// </value>
	/// <remarks>
	///     This property is used to invoke JavaScript functions from the .NET code. It is used in the TextOnly method to call
	///     the 'onCreate' JavaScript function.
	/// </remarks>
	[Inject]
    private IJSRuntime JsRuntime { get; set; }

	/// <summary>
	///     Gets or sets the model representing the education details of a candidate.
	/// </summary>
	/// <value>
	///     The model of type <see cref="CandidateEducation" /> which holds the education details of a candidate.
	/// </value>
	/// <remarks>
	///     This property is used as a Parameter in the Blazor component, and it is bound to the form fields in the dialog.
	///     It is used to get the current education details of the candidate and to set the updated education details.
	/// </remarks>
	[Parameter]
    public CandidateEducation Model { get; set; } = new();

	/// <summary>
	///     Gets or sets the event callback that is invoked when the save action is triggered in the dialog.
	/// </summary>
	/// <remarks>
	///     This event callback is used to handle the save action, which typically involves validating the form data,
	///     updating the candidate's education details, and hiding the dialog.
	///     The save action is usually triggered by a user interaction, such as clicking a save button.
	/// </remarks>
	[Parameter]
    public EventCallback<EditContext> Save { get; set; }

	private bool VisibleSpinner { get; set; }

	/// <summary>
	///     Asynchronously cancels the education dialog operation.
	/// </summary>
	/// <param name="args">The mouse event arguments associated with the cancel action.</param>
	/// <returns>A task that represents the asynchronous operation.</returns>
	/// <remarks>
	///     This method is triggered when the user clicks the cancel button in the education dialog.
	///     It calls the 'CallCancelMethod' from the 'General' class to handle the cancel operation,
	///     which includes hiding the spinner and dialog, and enabling the dialog buttons.
	/// </remarks>
	private async Task CancelEducationDialog(MouseEventArgs args)
    {
        VisibleSpinner = true;
        await Cancel.InvokeAsync(args);
        await Dialog.HideAsync();
        VisibleSpinner = false;
    }

    protected override void OnParametersSet()
    {
		if (Context?.Model != Model)
		{
			Context = null; // Immediate reference cleanup for GC
			Context = new(Model);
		}
        base.OnParametersSet();
    }

	/// <summary>
	///     Validates the form context of the dialog for editing the education details of a candidate.
	/// </summary>
	/// <remarks>
	///     This method is used to validate the form fields in the dialog when it is opened.
	///     It is typically called when the dialog is about to be displayed to the user.
	/// </remarks>
	private void OpenDialog() => Context.Validate();

	/// <summary>
	///     Asynchronously saves the education details of a candidate.
	/// </summary>
	/// <param name="editContext">The edit context associated with the save action.</param>
	/// <remarks>
	///     This method is invoked when the Save button in the dialog is clicked. It calls the `General.CallSaveMethod`
	///     to perform the save operation, which includes showing the spinner, disabling the dialog buttons,
	///     executing the save operation, and then hiding the spinner and dialog, and enabling the dialog buttons.
	/// </remarks>
	/// <returns>A task that represents the asynchronous operation.</returns>
	private async Task SaveEducationDialog(EditContext editContext)
    {
        VisibleSpinner = true;
        await Save.InvokeAsync(editContext);
        await Dialog.HideAsync();
        VisibleSpinner = false;
    }

	/// <summary>
	///     Asynchronously shows the dialog for editing the education details of a candidate.
	/// </summary>
	/// <returns>A <see cref="Task" /> that represents the asynchronous operation.</returns>
	/// <remarks>
	///     This method is used to display the dialog which allows the user to edit the education details of a candidate.
	///     It is typically called when the user triggers an edit action on the Candidate page.
	/// </remarks>
	public async Task ShowDialog() => await Dialog.ShowAsync();

	/// <summary>
	///     Invokes a JavaScript function to create a text box with specific input restrictions.
	/// </summary>
	/// <param name="textBox">The ID of the text box.</param>
	/// <param name="numbers">A boolean value indicating whether the text box should accept numbers.</param>
	/// <remarks>
	///     This method is used to create a text box in the dialog that only accepts text input. If the 'numbers' parameter is
	///     set to false, the text box will not accept number input.
	///     It uses the JavaScript runtime to invoke the 'onCreate' JavaScript function, passing the ID of the text box and the
	///     'numbers' parameter as arguments.
	/// </remarks>
	private async Task TextOnly(string textBox, bool numbers) => await JsRuntime.InvokeVoidAsync("onCreate", textBox, numbers);
}