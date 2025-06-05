#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.Server
// File Name:           ChangeRequisitionStatus.razor.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu, Brijesh Dubey
// Created On:          06-02-2025 19:06
// Last Updated On:     06-02-2025 20:02
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

    private async Task CancelDialog(MouseEventArgs args)
    {
        VisibleSpinner = true;
        await Cancel.InvokeAsync(args);
        await Dialog.HideAsync();
        VisibleSpinner = false;
    }

    private void Context_OnFieldChanged(object sender, FieldChangedEventArgs e) => Context?.Validate();

    public void Dispose()
    {
        if (Context is not null)
        {
            Context.OnFieldChanged -= Context_OnFieldChanged;
        }

        GC.SuppressFinalize(this);
    }

    protected override void OnParametersSet()
    {
        Context = new(Model);
        Context.OnFieldChanged += Context_OnFieldChanged;
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