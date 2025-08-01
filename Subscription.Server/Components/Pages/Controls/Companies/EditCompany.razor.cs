﻿#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.Server
// File Name:           EditCompany.razor.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu, Brijesh Dubey
// Created On:          02-05-2025 20:02
// Last Updated On:     04-25-2025 19:25
// *****************************************/

#endregion

namespace Subscription.Server.Components.Pages.Controls.Companies;

public partial class EditCompany : IDisposable
{
    private readonly CompanyDetailsValidator _companyValidator = new();

    // Memory optimization: Track model changes to avoid unnecessary Context recreation
    private CompanyDetails _currentModel;

    [Parameter]
    public EventCallback<MouseEventArgs> Cancel { get; set; }

    private SfDataForm CompanyEditForm { get; set; }

    private EditContext Context { get; set; }

    private SfDialog Dialog { get; set; }

    [Parameter]
    public CompanyDetails Model { get; set; } = new();

    [Parameter]
    public List<IntValues> NAICS { get; set; } = [];

    [Parameter]
    public EventCallback<EditContext> Save { get; set; }

    [Parameter]
    public List<StateCache> State { get; set; } = [];

    private bool VisibleSpinner { get; set; }

    public void Dispose()
    {
        // Memory optimization: No event handlers to unsubscribe from
        // EditContext handles validation automatically
        GC.SuppressFinalize(this);
    }

    private async Task CancelForm(MouseEventArgs args)
    {
        VisibleSpinner = true;
        await Cancel.InvokeAsync(args);
        await Dialog.HideAsync();
        VisibleSpinner = false;
    }

    // Memory optimization: Removed redundant Context_OnFieldChanged event handler
    // EditContext already handles field validation automatically

    private void DialogOpen(BeforeOpenEventArgs args) => CompanyEditForm.EditContext?.Validate();

    protected override void OnParametersSet()
    {
        // Memory optimization: Only create new Context if Model reference has changed
        // ReSharper disable once InvertIf
        if (_currentModel != Model)
        {
            _currentModel = Model;
            Context = new(Model);
            // Memory optimization: No event handler needed - EditContext handles validation automatically
        }
        
        base.OnParametersSet();
    }

    private async Task SaveCompany(EditContext args)
    {
        VisibleSpinner = true;
        await Save.InvokeAsync(args);
        await Dialog.HideAsync();
        VisibleSpinner = false;
    }

    internal async Task ShowDialog() => await Dialog.ShowAsync();
}