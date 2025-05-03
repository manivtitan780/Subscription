#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.Server
// File Name:           WorkflowDialog.razor.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu, Brijesh Dubey
// Created On:          03-24-2025 20:03
// Last Updated On:     05-02-2025 20:55
// *****************************************/

#endregion

namespace Subscription.Server.Components.Pages.Controls.Admin;

public partial class WorkflowDialog : ComponentBase, IDisposable
{
    private readonly WorkflowValidator _workflowValidator = new();

    [Parameter]
    public EventCallback<MouseEventArgs> Cancel { get; set; }

    private EditContext Context { get; set; }

    private SfDialog Dialog { get; set; }

    private SfDataForm EditWorkflowForm { get; set; }

    [Parameter]
    public string HeaderString { get; set; }

    [Parameter]
    public Workflow Model { get; set; } = new();

    [Parameter]
    public List<KeyValues> Next { get; set; } = [];

    [Parameter]
    public List<KeyValues> Role { get; set; } = [];

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

    private async Task CancelJobOptions(MouseEventArgs args)
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

    private void OpenDialog(BeforeOpenEventArgs arg)
    {
        Context.Validate();
    }

    private async Task SaveJobOptions(EditContext args)
    {
        VisibleSpinner = true;
        await Save.InvokeAsync(args);
        await Dialog.HideAsync();
        VisibleSpinner = false;
    }

    public Task ShowDialog() => Dialog.ShowAsync();
}