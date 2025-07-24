#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.Server
// File Name:           ChangeRequisitionStatus.razor.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu, Brijesh Dubey
// Created On:          07-24-2025 20:07
// Last Updated On:     07-24-2025 20:07
// *****************************************/

#endregion

namespace Subscription.Server.Components.Pages.Controls.Requisitions;

public partial class ChangeRequisitionStatus : ComponentBase, IDisposable
{
    [Parameter]
    public EventCallback<MouseEventArgs> Cancel { get; set; }

    private EditContext Context { get; set; }

    private SfDialog Dialog { get; set; }

    private SfDataForm EditRequisitionForm { get; set; }

    [Parameter]
    public RequisitionStatus Model { get; set; }

    [Parameter]
    public List<KeyValues> ReqStatus { get; set; } = [];

    [Parameter]
    public EventCallback<EditContext> Save { get; set; }

    [Parameter]
    public bool VisibleSpinner { get; set; }

    // Removed: Unnecessary Context_OnFieldChanged event handler - validation handled by form validation

    /// <summary>
    ///     Memory optimization: Clean disposal pattern
    /// </summary>
    public void Dispose()
    {
        // No event handlers to dispose after optimization
        GC.SuppressFinalize(this);
    }

    private async Task CancelDialog(MouseEventArgs args)
    {
        VisibleSpinner = true;
        await Cancel.InvokeAsync(args);
        await Dialog.HideAsync();
        VisibleSpinner = false;
    }

    protected override void OnParametersSet()
    {
        // Memory optimization: Only create new EditContext if Model reference changed
        if (Context?.Model != Model)
        {
            Context = new(Model);
        }

        base.OnParametersSet();
    }

    private void OpenDialog() => Context?.Validate();

    private async Task SaveRequisitionDialog(EditContext editContext)
    {
        VisibleSpinner = true;
        await Save.InvokeAsync(editContext);
        await Dialog.HideAsync();
        VisibleSpinner = false;
    }

    public async Task ShowDialog() => await Dialog.ShowAsync();
}