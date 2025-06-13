using Microsoft.AspNetCore.Components;

namespace Subscription.Server.Components.Pages.Controls.Requisitions;

public partial class EditRequisitionNotesDialog : ComponentBase
{
    private readonly CandidateNotesValidator _candidateNotesValidator = new();

    [Parameter]
    public EventCallback<MouseEventArgs> Cancel { get; set; }

    private EditContext Context { get; set; }

    private SfDialog Dialog { get; set; }

    private SfDataForm EditNotesForm { get; set; }

    [Parameter]
    public string Entity { get; set; } = "Requisition";

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