#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.Server
// File Name:           Home.razor.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu, Brijesh Dubey
// Created On:          06-13-2025 20:06
// Last Updated On:     06-18-2025 20:54
// *****************************************/

#endregion

namespace Subscription.Server.Components;

public partial class Home : ComponentBase
{
    // Data models - these would match your API response classes
    private DashboardData? _dashboardData;
    private bool _isLoading = true;
    private List<HiredPlacement> _placements = [];
    private List<RecentActivity> _recentActivity = [];
    private string _selectedTimePeriod = "MTD";
    private readonly string _selectedUser = "DAVE";
    private List<TimeMetric> _timeMetrics = [];
    private readonly List<string> _timePeriods = ["7 days", "MTD", "QTD", "HYTD", "YTD"];

    private string GetHireToOfferRatio()
    {
        TimeMetric metric = _timeMetrics.FirstOrDefault(m => m.DateRange == _selectedTimePeriod);
        return metric?.HireToOfferRatio.ToString("P0") ?? "0%";
    }

    private string GetMetricValue(string metricName)
    {
        TimeMetric metric = _timeMetrics.FirstOrDefault(m => m.DateRange == _selectedTimePeriod);
        if (metric == null)
        {
            return "0";
        }

        return metricName switch
               {
                   "Total Requisitions" => metric.TotalRequisitions.ToString(),
                   "Active Requisitions" => metric.ActiveRequisitions.ToString(),
                   "Candidates in Interview" => metric.CandidatesInInterview.ToString(),
                   "Offers Extended" => metric.OffersExtended.ToString(),
                   "Candidates Hired" => metric.CandidatesHired.ToString(),
                   _ => "0"
               };
    }

    // Mock data methods - replace with actual API calls
    private DashboardData GetMockDashboardData() => new() {User = _selectedUser};

    private List<HiredPlacement> GetMockPlacements() =>
    [
        new()
        {
            Company = "Enterprise Corp", RequisitionNumber = "REQ001", NumPosition = 1, Title = "Senior Architect", CandidateName = "Alex Brown", DateHired = DateTime.Now.AddDays(-30),
            SalaryOffered = 120000, PlacementFee = 24000,
            CommissionPercent = 0.05m, CommissionEarned = 1200
        },
        new()
        {
            Company = "Growth LLC", RequisitionNumber = "REQ002", NumPosition = 2, Title = "Data Scientist", CandidateName = "Lisa Chen", DateHired = DateTime.Now.AddDays(-45),
            SalaryOffered = 110000, PlacementFee = 22000,
            CommissionPercent = 0.05m, CommissionEarned = 1100
        }
    ];

    private List<RecentActivity> GetMockRecentActivity() =>
    [
        new()
        {
            Company = "Tech Solutions Inc", Requisition = "REQ001", NumPosition = 2, Title = "Senior Developer", CandidateName = "John Smith", CurrentStatus = "INT",
            DateFirstSubmitted = DateTime.Now.AddDays(-5),
            LastActivityDate = DateTime.Now.AddDays(-1), ActivityNotes = "Second round interview scheduled"
        },
        new()
        {
            Company = "Digital Innovations", Requisition = "REQ002", NumPosition = 1, Title = "Product Manager", CandidateName = "Sarah Johnson", CurrentStatus = "OEX",
            DateFirstSubmitted = DateTime.Now.AddDays(-10),
            LastActivityDate = DateTime.Now.AddDays(-2), ActivityNotes = "Offer extended, awaiting response"
        },
        new()
        {
            Company = "StartupCorp", Requisition = "REQ003", NumPosition = 3, Title = "Frontend Developer", CandidateName = "Mike Wilson", CurrentStatus = "HIR",
            DateFirstSubmitted = DateTime.Now.AddDays(-15),
            LastActivityDate = DateTime.Now.AddDays(-3), ActivityNotes = "Offer accepted, start date confirmed"
        }
    ];

    private async Task LoadDashboardData()
    {
        _isLoading = true;
        try
        {
            // Call your WebAPI endpoint
            // dashboardData = await Http.GetFromJsonAsync<DashboardData>($"api/dashboard/{selectedUser}");

            // For demo purposes, using mock data
            await Task.Delay(1000); // Simulate API call
            _dashboardData = GetMockDashboardData();

            ProcessDashboardData();
        }
        catch (Exception ex)
        {
            // Handle error
            Console.WriteLine($"Error loading dashboard data: {ex.Message}");
            _dashboardData = null;
        }
        finally
        {
            _isLoading = false;
            StateHasChanged();
        }
    }

    protected override async Task OnInitializedAsync()
    {
        await LoadDashboardData();
    }

    private void ProcessDashboardData()
    {
        if (_dashboardData == null)
        {
            return;
        }

        // Process time metrics for the summary grid
        _timeMetrics =
        [
            new() {DateRange = "7 days", TotalRequisitions = 5, ActiveRequisitions = 4, CandidatesInInterview = 2, OffersExtended = 1, CandidatesHired = 1, HireToOfferRatio = 1.0m},
            new() {DateRange = "MTD", TotalRequisitions = 12, ActiveRequisitions = 9, CandidatesInInterview = 4, OffersExtended = 2, CandidatesHired = 2, HireToOfferRatio = 1.0m},
            new() {DateRange = "QTD", TotalRequisitions = 28, ActiveRequisitions = 18, CandidatesInInterview = 7, OffersExtended = 4, CandidatesHired = 3, HireToOfferRatio = 0.75m},
            new() {DateRange = "HYTD", TotalRequisitions = 45, ActiveRequisitions = 25, CandidatesInInterview = 9, OffersExtended = 6, CandidatesHired = 4, HireToOfferRatio = 0.67m},
            new() {DateRange = "YTD", TotalRequisitions = 67, ActiveRequisitions = 32, CandidatesInInterview = 12, OffersExtended = 8, CandidatesHired = 6, HireToOfferRatio = 0.75m}
        ];

        // Mock recent activity data
        _recentActivity = GetMockRecentActivity();

        // Mock placement data
        _placements = GetMockPlacements();
    }

    private async Task RefreshData()
    {
        await LoadDashboardData();
    }
}