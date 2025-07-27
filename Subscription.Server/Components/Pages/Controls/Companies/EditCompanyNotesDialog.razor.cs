#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.Server
// File Name:           EditCompanyNotesDialog.razor.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu, Brijesh Dubey
// Created On:          07-26-2025 16:07
// Last Updated On:     07-26-2025 19:27
// *****************************************/

#endregion

namespace Subscription.Server.Components.Pages.Controls.Companies;

public partial class EditCompanyNotesDialog : ComponentBase, IDisposable
{
    private readonly CandidateNotesValidator _candidateNotesValidator = new();

    [Parameter]
    public EventCallback<MouseEventArgs> Cancel { get; set; }

    private EditContext Context { get; set; }

    private SfDialog Dialog { get; set; }

    private SfDataForm EditNotesForm { get; set; }

    [Parameter]
    public string Entity { get; set; } = "Company";

    [Parameter]
    public CandidateNotes Model { get; set; } = new();

    [Parameter]
    public EventCallback<EditContext> Save { get; set; }

    [Parameter]
    public string Title { get; set; }

    private bool VisibleSpinner { get; set; }

    public void Dispose()
    {
        // No event handlers to dispose after optimization
        GC.SuppressFinalize(this);
    }

    private async Task CancelNotesDialog(MouseEventArgs args)
    {
        VisibleSpinner = true;
        await Cancel.InvokeAsync(args);
        await Dialog.HideAsync();
        VisibleSpinner = false;
    }

    protected override void OnParametersSet()
    {
        // Memory optimization: Explicit cleanup before creating new EditContext
        if (Context?.Model != Model)
        {
            Context = null; // Immediate reference cleanup for GC
            Context = new(Model);
        }

        base.OnParametersSet();
    }

    private void OpenDialog() => Context.Validate();

    private async Task SaveNotesDialog(EditContext editContext)
    {
        VisibleSpinner = true;
        await Save.InvokeAsync(editContext);
        await Dialog.HideAsync();
        VisibleSpinner = false;
    }

    public async Task ShowDialog() => await Dialog.ShowAsync();
}