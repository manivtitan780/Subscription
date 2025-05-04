#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.Server
// File Name:           TemplateDialog.razor.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu, Brijesh Dubey
// Created On:          05-03-2025 20:05
// Last Updated On:     05-04-2025 15:13
// *****************************************/

#endregion

namespace Subscription.Server.Components.Pages.Controls.Admin;

public partial class TemplateDialog : ComponentBase, IDisposable
{
    private readonly List<ToolbarItemModel> _tools =
    [
        new() {Command = ToolbarCommand.Bold}, new() {Command = ToolbarCommand.Italic}, new() {Command = ToolbarCommand.Underline}, new() {Command = ToolbarCommand.StrikeThrough},
        new() {Command = ToolbarCommand.LowerCase}, new() {Command = ToolbarCommand.UpperCase}, new() {Command = ToolbarCommand.Separator}, new() {Command = ToolbarCommand.ClearFormat},
        new() {Command = ToolbarCommand.Separator}, new() {Command = ToolbarCommand.Undo}, new() {Command = ToolbarCommand.Redo}, new() {Name = "Tokens", TooltipText = "Insert Token"}
    ];

    private List<IntValues> ActionList { get; } =
    [
        new() {KeyValue = 1, Text = "Candidate Created"}, new() {KeyValue = 2, Text = "Candidate Updated"}, new() {KeyValue = 3, Text = "Candidate Submitted"},
        new() {KeyValue = 4, Text = "Candidate Deleted"}, new() {KeyValue = 5, Text = "Candidate Status Changed"}, new() {KeyValue = 6, Text = "Requisition Created"},
        new() {KeyValue = 7, Text = "Requisition Updated"}, new() {KeyValue = 8, Text = "Requisition Status Changed"}, new() {KeyValue = 9, Text = "Candidate Submission Updated"},
        new() {KeyValue = 10, Text = "No Action"}
    ];

    [Parameter]
    public EventCallback<MouseEventArgs> Cancel { get; set; }

    private EditContext Context { get; set; }

    private SfDialog Dialog { get; set; }

    private SfDataForm EditTemplateForm { get; set; }

    private IEnumerable<KeyValues> FieldTokens { get; } =
    [
        new() {KeyValue = "{TODAY}", Text = "Today's Date"},
        new() {KeyValue = "{CandidateID}", Text = "Candidate ID"},
        new() {KeyValue = "{FullName}", Text = "Candidate Full Name"},
        new() {KeyValue = "{FirstName}", Text = "Candidate First Name"},
        new() {KeyValue = "{LastName}", Text = "Candidate Last Name"},
        new() {KeyValue = "{CandidateLocation}", Text = "Candidate Location"},
        new() {KeyValue = "{CandidatePhone}", Text = "Candidate Phone"},
        new() {KeyValue = "{CandidateSummary}", Text = "Candidate Summary"},
        new() {KeyValue = "{RequisitionID}", Text = "Requisition ID"},
        new() {KeyValue = "{RequisitionTitle}", Text = "Requisition Title"},
        new() {KeyValue = "{Company}", Text = "Company Name"},
        new() {KeyValue = "{RequisitionLocation}", Text = "Requisition Location"},
        new() {KeyValue = "{RequisitionDescription}", Text = "Requisition Description"},
        new() {KeyValue = "{SubmissionNotes}", Text = "Candidate Submission Notes"},
        new() {KeyValue = "{LoggedInUser}", Text = "Logged In User"}
    ];

    [Parameter]
    public string HeaderString { get; set; }

    [Parameter]
    public AppTemplate Model { get; set; } = new();

    [Parameter]
    public EventCallback<EditContext> Save { get; set; }

    private bool VisibleSpinner { get; set; }

    public void Dispose()
    {
        if (Context is not null)
        {
            Context.OnFieldChanged -= Context_OnFieldChanged;
        }

        GC.SuppressFinalize(this);
    }

    private async Task CancelTemplate(MouseEventArgs args)
    {
        VisibleSpinner = true;
        await Cancel.InvokeAsync(args);
        await Dialog.HideAsync();
        VisibleSpinner = false;
    }

    private void Context_OnFieldChanged(object sender, FieldChangedEventArgs e) => Context.Validate();

    protected override void OnParametersSet()
    {
        Context = new(Model);
        Context.OnFieldChanged += Context_OnFieldChanged;
        base.OnParametersSet();
    }

    private void OpenDialog() => Context.Validate();

    private async Task SaveTemplate(EditContext editContext)
    {
        VisibleSpinner = true;
        await Save.InvokeAsync(editContext);
        await Dialog.HideAsync();
        VisibleSpinner = false;
    }

    public async Task ShowDialog() => await Dialog.ShowAsync();
}