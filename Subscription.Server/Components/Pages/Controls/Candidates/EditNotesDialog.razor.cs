#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.Server
// File Name:           EditNotesDialog.razor.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu, Brijesh Dubey
// Created On:          12-04-2024 20:12
// Last Updated On:     04-14-2025 19:04
// *****************************************/

#endregion

namespace Subscription.Server.Components.Pages.Controls.Candidates;

public partial class EditNotesDialog
{
    private readonly CandidateNotesValidator _candidateNotesValidator = new();

    private readonly Dictionary<string, object> _textBoxAttributes = new()
                                                                     {
                                                                         {"MaxLength", "1000"},
                                                                         {"MinLength", "1"}
                                                                     };

    [Parameter]
    public EventCallback<MouseEventArgs> Cancel { get; set; }

    private EditContext Context { get; set; }

    private SfDialog Dialog { get; set; }

    private SfDataForm EditNotesForm { get; set; }

    [Parameter]
    public string Entity { get; set; } = "Candidate";

    [Parameter]
    public CandidateNotes Model { get; set; }

    [Parameter]
    public EventCallback<EditContext> Save { get; set; }

    private SfSpinner Spinner { get; set; }

    [Parameter]
    public string Title { get; set; }

    private async Task CancelNotesDialog(MouseEventArgs args)
    {
        await General.DisplaySpinner(Spinner);
        await Cancel.InvokeAsync(args);
        await Dialog.HideAsync();
        await General.DisplaySpinner(Spinner, false);
    }

    private void Context_OnFieldChanged(object sender, FieldChangedEventArgs e) => Context.Validate();

    protected override void OnParametersSet()
    {
        Context = new(Model);
        Context.OnFieldChanged += Context_OnFieldChanged;
        base.OnParametersSet();
    }

    private void OpenDialog() => Context.Validate();

    private async Task SaveNotesDialog(EditContext editContext)
    {
        await General.DisplaySpinner(Spinner);
        await Save.InvokeAsync(editContext);
        await Dialog.HideAsync();
        await General.DisplaySpinner(Spinner, false);
    }

    public async Task ShowDialog() => await Dialog.ShowAsync();
}