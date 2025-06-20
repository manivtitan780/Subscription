#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.Server
// File Name:           Home.razor.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu, Brijesh Dubey
// Created On:          06-13-2025 20:06
// Last Updated On:     06-20-2025 19:05
// *****************************************/

#endregion

namespace Subscription.Server.Components;

public partial class Home : ComponentBase
{
    // Data models - these would match your API response classes
    private DashboardData _dashboardData;
    private bool _isLoading = true;
    private List<HiredPlacement> _placements = [];
    private List<RecentActivity> _recentActivity = [];
    private string _selectedTimePeriod = "MTD";
    private readonly SemaphoreSlim _semaphoreMainPage = new(1, 1);
    private readonly List<TimeMetric> _timeMetrics = [];
    private readonly List<string> _timePeriods = ["7 days", "MTD", "QTD", "HYTD", "YTD"];

    [Inject]
    private ILocalStorageService LocalStorage { get; set; }

    [Inject]
    private NavigationManager NavManager { get; set; }

    private string RoleName { get; set; }

    [Inject]
    private ISessionStorageService SessionStorage { get; set; }

    private string User { get; set; }

    private Task ExecuteMethod(Func<Task> task) => General.ExecuteMethod(_semaphoreMainPage, task);

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

    private async Task LoadDashboardData()
    {
        _isLoading = true;
        try
        {
            // Call your WebAPI endpoint
            Dictionary<string, string> _parameters = new()
                                                     {
                                                         {"user", "DAVE"}
                                                     };
            ReturnDashboard _response = await General.ExecuteRest<ReturnDashboard>("Dashboard/GetAccountsManagerDashboard", _parameters, null, false);

            _dashboardData ??= new();
            _dashboardData.UserName = User;
            List<DateCount> _dateCountsTotalRequisition = General.DeserializeObject<List<DateCount>>(_response.TotalRequisitions);
            List<DateCount> _dateCountsActiveRequisition = General.DeserializeObject<List<DateCount>>(_response.ActiveRequisitions);
            List<DateCount> _dateCountsCandidatesInInterview = General.DeserializeObject<List<DateCount>>(_response.CandidatesInInterview);
            List<DateCount> _dateCountsOffersExtended = General.DeserializeObject<List<DateCount>>(_response.OffersExtended);
            List<DateCount> _dateCountsCandidatesHired = General.DeserializeObject<List<DateCount>>(_response.CandidatesHired);
            List<RatioCount> _ratioCounts = General.DeserializeObject<List<RatioCount>>(_response.HireToOfferRatio);
            _recentActivity = General.DeserializeObject<List<RecentActivity>>(_response.RecentActivity);
            _placements = General.DeserializeObject<List<HiredPlacement>>(_response.Placements);

            // For demo purposes, using mock data
            /*await Task.Delay(1000); // Simulate API call
            _dashboardData = GetMockDashboardData();*/

            string[] periods = ["7 days", "MTD", "QTD", "HYTD", "YTD"];

            foreach (string period in periods)
            {
                DateCount _total = _dateCountsTotalRequisition.FirstOrDefault(d => d.Period == period);
                DateCount _active = _dateCountsActiveRequisition.FirstOrDefault(d => d.Period == period);
                DateCount _interview = _dateCountsCandidatesInInterview.FirstOrDefault(d => d.Period == period);
                DateCount _offers = _dateCountsOffersExtended.FirstOrDefault(d => d.Period == period);
                DateCount _hired = _dateCountsCandidatesHired.FirstOrDefault(d => d.Period == period);
                RatioCount _ratio = _ratioCounts.FirstOrDefault(d => d.Period == period);

                _timeMetrics.Add(new()
                                 {
                                     DateRange = period,
                                     TotalRequisitions = _total.Count,
                                     ActiveRequisitions = _active.Count,
                                     CandidatesInInterview = _interview.Count,
                                     OffersExtended = _offers.Count,
                                     CandidatesHired = _hired.Count,
                                     HireToOfferRatio = _ratio.Ratio
                                 });
            }
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
        await ExecuteMethod(async () =>
                            {
                                // Get user claims
                                IEnumerable<Claim> _claims = await General.GetClaimsToken(LocalStorage, SessionStorage);

                                if (_claims == null)
                                {
                                    NavManager.NavigateTo($"{NavManager.BaseUri}login", true);
                                }
                                else
                                {
                                    IEnumerable<Claim> _enumerable = _claims as Claim[] ?? _claims.ToArray();
                                    User = _enumerable.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value.ToUpperInvariant();
                                    RoleName = _enumerable.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value.ToUpperInvariant();
                                    if (User.NullOrWhiteSpace())
                                    {
                                        NavManager.NavigateTo($"{NavManager.BaseUri}login", true);
                                    }
                                }

                                await LoadDashboardData();
                            });

        await base.OnInitializedAsync();
    }

    private async Task RefreshData()
    {
        await LoadDashboardData();
    }
}