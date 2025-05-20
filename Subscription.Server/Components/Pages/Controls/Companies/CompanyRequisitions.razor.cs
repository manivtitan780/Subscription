#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.Server
// File Name:           CompanyRequisitions.razor.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu, Brijesh Dubey
// Created On:          05-20-2025 15:05
// Last Updated On:     05-20-2025 18:46
// *****************************************/

#endregion

namespace Subscription.Server.Components.Pages.Controls.Companies;

public partial class CompanyRequisitions
{
    private int _selectedReqTab = 0;

    [Parameter]
    public List<Company> CompaniesList { get; set; }

    [Parameter]
    public List<CompanyContacts> CompanyContacts { get; set; }

    [Parameter]
    public List<IntValues> Education { get; set; }

    [Parameter]
    public List<IntValues> Eligibility { get; set; }

    [Parameter]
    public List<IntValues> Experience { get; set; }

    private bool FirstRender { get; set; }

    private SfGrid<Requisition> GridInnerRequisition { get; set; }

    [Parameter]
    public List<JobOptions> JobOptions { get; set; }

    [Inject]
    private IJSRuntime JsRuntime { get; set; }

    [Parameter]
    public List<Requisition> Model { get; set; }

    [Inject]
    private NavigationManager NavManager { get; set; }

    [Parameter]
    public List<KeyValues> Recruiters { get; set; }

    [Parameter]
    public int RoleID { get; set; }

    [Parameter]
    public double RowHeight { get; set; }

    [Parameter]
    public int RowHeightActivity { get; set; }

    [Parameter]
    public List<IntValues> Skills { get; set; }

    private SfSpinner Spinner { get; set; }

    [Parameter]
    public List<StateCache> States { get; set; }

    [Parameter]
    public List<StatusCode> StatusCodes { get; set; }

    [Parameter]
    public string User { get; set; } = "JOLLY";

    [Parameter]
    public List<Workflow> Workflows { get; set; } = new();

    private async Task DataHandler(object obj)
    {
        /*DotNetObjectReference<CompanyRequisitions> _dotNetReference = DotNetObjectReference.Create(this); // create dotnet ref
        await Runtime.InvokeAsync<string>("detail", _dotNetReference);*/
        //  send the dotnet ref to JS side
        FirstRender = false;
        //Count = Count;
        if (GridInnerRequisition.TotalItemCount > 0)
        {
            await GridInnerRequisition.SelectRowAsync(0);
        }
    }
}