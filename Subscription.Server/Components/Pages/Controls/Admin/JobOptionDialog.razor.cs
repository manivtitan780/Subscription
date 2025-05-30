#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.Server
// File Name:           JobOptionDialog.razor.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu, Brijesh Dubey
// Created On:          03-17-2025 19:03
// Last Updated On:     05-02-2025 15:48
// *****************************************/

#endregion

namespace Subscription.Server.Components.Pages.Controls.Admin;

public partial class JobOptionDialog : ComponentBase, IDisposable
{
    private readonly JobOptionsValidator _jobOptionsValidator = new();
    private List<string> _statusList;

    private List<string> SelectedTaxTerms
    {
        get => _statusList ??= Model.Tax.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                      .Select(s => s.Trim())
                                      .ToList() ?? [];
        set
        {
            _statusList = value;
            Model.Tax = string.Join(",", _statusList);
        }
    }

    /// <summary>
    ///     Gets or sets the Cancel event callback that is triggered when the Cancel operation is performed in the
    ///     AdminListDialog.
    /// </summary>
    /// <remarks>
    ///     This event callback is of type <see cref="EventCallback{MouseEventArgs}" /> and it is invoked when the user clicks
    ///     on the Cancel button in the dialog.
    /// </remarks>
    [Parameter]
    public EventCallback<MouseEventArgs> Cancel { get; set; }

    private EditContext Context { get; set; }

    /// <summary>
    ///     Gets or sets the SfDialog control for the AdminListDialog.
    /// </summary>
    /// <remarks>
    ///     This property is used to control the dialog box in the AdminListDialog. It is used to show or hide the dialog box.
    ///     The SfDialog control is a part of Syncfusion Blazor UI components and provides a modal dialog box functionality.
    ///     The value of this property is bound to the Dialog attribute of the SfDialog component in the Razor markup.
    /// </remarks>
    private SfDialog Dialog { get; set; }

    /// <summary>
    ///     Gets or sets the EditForm component for the AdminListDialog.
    /// </summary>
    /// <remarks>
    ///     This property represents the EditForm component in the dialog, which is used for editing the details of the
    ///     AdminList.
    ///     The form includes a TextBoxControl for input and a SwitchControl for toggling the status of the AdminList.
    ///     The form's Model is bound to the AdminList and its OnValidSubmit event is bound to the SaveAdminList method.
    /// </remarks>
    private EditForm EditAdminForm { get; set; }

    private SfDataForm EditJobOptionsForm { get; set; }

    /// <summary>
    ///     Gets or sets the header string for the AdminListDialog.
    /// </summary>
    /// <remarks>
    ///     This property is used to set the title of the dialog. It is displayed at the top of the dialog box.
    ///     The value of this property is bound to the Header attribute of the SfDialog component in the Razor markup.
    /// </remarks>
    [Parameter]
    public string HeaderString { get; set; }

    /// <summary>
    ///     Gets or sets the Model for the AdminListDialog.
    /// </summary>
    /// <remarks>
    ///     This property is used to bind the data of the AdminList to the dialog. It is used in the EditForm component in the
    ///     Razor markup.
    ///     The value of this property is bound to the Model attribute of the EditForm component.
    /// </remarks>
    [Parameter]
    public JobOptions Model { get; set; }

    /// <summary>
    ///     Gets or sets the placeholder text for the TextBoxControl in the AdminListDialog.
    /// </summary>
    /// <remarks>
    ///     This property is used to set the placeholder text of the TextBoxControl in the dialog. It provides a short hint
    ///     describing the expected value of the input field.
    ///     The value of this property is bound to the Placeholder attribute of the TextBoxControl in the Razor markup.
    /// </remarks>
    [Parameter]
    public string Placeholder { get; set; }

    /// <summary>
    ///     Gets or sets the Save event callback that is triggered when the Save operation is performed in the AdminListDialog.
    /// </summary>
    /// <remarks>
    ///     This event callback is of type <see cref="EventCallback{EditContext}" /> and it is invoked when the user clicks on
    ///     the Save button in the dialog.
    /// </remarks>
    [Parameter]
    public EventCallback<EditContext> Save { get; set; }

    [Parameter]
    public List<KeyValues> TaxTerms { get; set; }

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
    ///     Asynchronously cancels the administrative list operation.
    /// </summary>
    /// <param name="args">The mouse event arguments associated with the cancel action.</param>
    /// <remarks>
    ///     This method is invoked when the user clicks on the Cancel button in the AdminListDialog.
    /// </remarks>
    /// <returns>A task that represents the asynchronous operation.</returns>
    private async Task CancelJobOptions(MouseEventArgs args)
    {
        VisibleSpinner = true;
        await Cancel.InvokeAsync(args);
        await Dialog.HideAsync();
        VisibleSpinner = false;
    }

    private void Context_OnFieldChanged(object sender, FieldChangedEventArgs e) => Context.Validate();

    private void MultiSelectValueChange(MultiSelectChangeEventArgs<List<string>> taxTerms)
    {
        Model.Tax = string.Join(",", taxTerms.Value);
        Context.NotifyFieldChanged(Context.Field(nameof(Model.Tax)));
    }

    protected override void OnParametersSet()
    {
        Context = new(Model);
        Context.OnFieldChanged += Context_OnFieldChanged;
        base.OnParametersSet();
    }

    /// <summary>
    ///     Asynchronously opens the dialog before it is displayed.
    /// </summary>
    /// <param name="arg">The arguments for the BeforeOpen event of the dialog.</param>
    /// <remarks>
    ///     This method is invoked when the dialog is about to be opened. It yields control back to the caller before
    ///     validating the EditForm's context.
    /// </remarks>
    private void OpenDialog(BeforeOpenEventArgs arg)
    {
        /*if (Model.Tax.NotNullOrWhiteSpace())
        {
            _taxTerms = Model.Tax.Split(',').Select(term => term.Trim()).ToList();
        }*/
        Context.Validate();
    }

    /// <summary>
    ///     Asynchronously saves the administrative list in the AdminListDialog.
    /// </summary>
    /// <param name="editContext">The edit context associated with the save action.</param>
    /// <remarks>
    ///     It also controls the spinner animation and the dialog buttons during the save operation.
    /// </remarks>
    /// <returns>A task that represents the asynchronous operation.</returns>
    private async Task SaveJobOptions(EditContext editContext)
    {
        VisibleSpinner = true;
        await Save.InvokeAsync(editContext);
        await Dialog.HideAsync();
        VisibleSpinner = false;
    }

    /// <summary>
    ///     Asynchronously displays the AdminListDialog.
    /// </summary>
    /// <remarks>
    ///     This method is used to display the AdminListDialog when it is required, such as during the editing of
    ///     administrative records like Designation, Education, and Eligibility. It uses the ShowAsync method of the SfDialog
    ///     control to display the dialog.
    /// </remarks>
    /// <returns>A Task representing the asynchronous operation of showing the dialog.</returns>
    public Task ShowDialog() => Dialog.ShowAsync();
}