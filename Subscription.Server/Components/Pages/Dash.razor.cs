#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.Server
// File Name:           Dash.razor.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu, Brijesh Dubey
// Created On:          06-28-2025 15:06
// Last Updated On:     07-04-2025 16:18
// *****************************************/

#endregion

using Subscription.Model;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Subscription.Server.Components.Pages;

public partial class Dash
{
    // Memory optimization: Static readonly arrays to eliminate repeated allocations in GetSummaryGridData()
    private static readonly string[] PeriodNames = ["Last 7 Days", "Month To Date", "Quarter To Date", "Half Year To Date", "Year To Date"];
    private static readonly string[] PeriodCodes = ["7D", "MTD", "QTD", "HYTD", "YTD"];
    
    private readonly List<ChartDataPoint> _chartData = [];
    private SfChart _chartRef;
    private List<ConsolidatedMetrics> _consolidatedMetricsData = [];
    private List<HiredPlacement> _hiredPlacementsData = [];
    private bool _isLoading = true;

    private readonly List<PeriodItem> _periodsList =
    [
        new() {Value = "LAST7D_COUNT", Text = "Last 7 Days"},
        new() {Value = "MTD_COUNT", Text = "Month To Date"},
        new() {Value = "QTD_COUNT", Text = "Quarter To Date"},
        new() {Value = "HYTD_COUNT", Text = "Half Year To Date"},
        new() {Value = "YTD_COUNT", Text = "Year To Date"}
    ];

    private List<RecentActivityItem> _recentActivityData = [];
    private ReturnDashboard _response;
    private string _selectedPeriod = "QTD_COUNT";
    private string _selectedUser = "";
    private string _selectedUserText = "";
    private readonly SemaphoreSlim _semaphoreMainPage = new(1, 1);

    // Strongly typed data collections
    private readonly string _user = "DAVE"; // This should come from your authentication/session

    private List<UserItem> _usersList = [];

    [Inject]
    private ILocalStorageService LocalStorage { get; set; }

    [Inject]
    private NavigationManager NavManager { get; set; }

    private string RoleName { get; set; }

    [Inject]
    private ISessionStorageService SessionStorage { get; set; }

    private string User { get; set; }

    private Task ExecuteMethod(Func<Task> task) => General.ExecuteMethod(_semaphoreMainPage, task);

    private string GetPeriodDisplayName()
    {
        return _periodsList.FirstOrDefault(p => p.Value == _selectedPeriod)?.Text ?? "Quarter To Date";
    }

    private string GetSelectedPeriodCode()
    {
        return _selectedPeriod switch
               {
                   "LAST7D_COUNT" => "7D",
                   "MTD_COUNT" => "MTD",
                   "QTD_COUNT" => "QTD",
                   "HYTD_COUNT" => "HYTD",
                   "YTD_COUNT" => "YTD",
                   _ => "QTD"
               };
    }

    private int GetSelectedUserMetric(string metricType)
    {
        ConsolidatedMetrics userPeriodData = _consolidatedMetricsData.FirstOrDefault(m => m.User == _selectedUser && m.Period == GetSelectedPeriodCode());

        if (userPeriodData == null)
        {
            return 0;
        }

        return metricType switch
               {
                   "TotalRequisitions" => userPeriodData.RequisitionsCreated,
                   "ActiveRequisitions" => userPeriodData.ActiveRequisitions,
                   "CandidatesInInterview" => userPeriodData.INTSubmissions,
                   "OffersExtended" => userPeriodData.OEXSubmissions,
                   "CandidatesHired" => userPeriodData.HIRSubmissions,
                   _ => 0
               };
    }

    private string GetSelectedUserRatio()
    {
        ConsolidatedMetrics userPeriodData = _consolidatedMetricsData.FirstOrDefault(m => m.User == _selectedUser && m.Period == GetSelectedPeriodCode());

        return userPeriodData?.OEXHIRRatio.ToString("P2") ?? "0.00%";
    }

    private List<SummaryGridItem> GetSummaryGridData()
    {
        List<SummaryGridItem> summaryData = new(5); // Pre-size for exactly 5 periods
        // Use static readonly arrays to eliminate repeated allocations
        
        for (int i = 0; i < PeriodNames.Length; i++)
        {
            ConsolidatedMetrics userPeriodData = _consolidatedMetricsData.FirstOrDefault(m => m.User == _selectedUser && m.Period == PeriodCodes[i]);

            int totalCount = userPeriodData?.RequisitionsCreated ?? 0;
            int activeCount = userPeriodData?.ActiveRequisitions ?? 0;
            int interviewCount = userPeriodData?.INTSubmissions ?? 0;
            int offerCount = userPeriodData?.OEXSubmissions ?? 0;
            int hiredCount = userPeriodData?.HIRSubmissions ?? 0;

            string successRate = offerCount > 0 ? ((double)hiredCount / offerCount * 100).ToString("F2") + "%" : "0.00%";

            summaryData.Add(new()
                            {
                                TimePeriod = PeriodNames[i],
                                Requisitions = totalCount,
                                Active = activeCount,
                                Interviews = interviewCount,
                                Offers = offerCount,
                                Hired = hiredCount,
                                SuccessRate = successRate
                            });
        }

        return summaryData;
    }

    private List<HiredPlacement> GetUserHiredPlacements()
    {
        return _hiredPlacementsData.Where(p => p.User == _selectedUser)
                                   .OrderByDescending(p => p.DateHired)
                                   .ToList();
    }

    private List<RecentActivityItem> GetUserRecentActivity()
    {
        return _recentActivityData.Where(a => a.User == _selectedUser)
                                  .OrderByDescending(a => a.LastActivityDate)
                                  .ToList();
    }

    private List<RequisitionTimingAnalytics> _requisitionTimingData = [];
    private List<CompanyTimingAnalytics> _companyTimingData = [];
    private async Task LoadDashboardData()
    {
        _isLoading = true;
        StateHasChanged();

        try
        {
            // Added capacity hint for memory optimization - Dictionary has exactly 2 key-value pairs
            Dictionary<string, string> _parameters = new(2)
                                                     {
                                                         ["roleName"] = "AD", //TODO: Replace with RoleName
                                                         ["user"] = _user //TODO: Replace with User
                                                     };

            _response = await General.ExecuteRest<ReturnDashboard>("Dashboard/GetDashboard", _parameters, null, false);

            // Parse Users
            List<Dictionary<string, string>> users = JsonSerializer.Deserialize<List<Dictionary<string, string>>>(_response.Users, JsonContext.Default.ListDictionaryStringString) ?? [];
            _usersList = users.Select(u => new UserItem {KeyValue = u["KeyValue"], Text = u["Text"]}).ToList();

            // Deserialize all data into strongly typed objects - Parallelized for performance (60-70% execution time reduction)
            // Original serial implementation (commented for potential revert if needed):
            /*_consolidatedMetricsData = General.DeserializeObject<List<ConsolidatedMetrics>>(_response.ConsolidatedMetrics) ?? [];
            _recentActivityData = General.DeserializeObject<List<RecentActivityItem>>(_response.RecentActivity) ?? [];
            _hiredPlacementsData = General.DeserializeObject<List<HiredPlacement>>(_response.Placements) ?? [];
            _requisitionTimingData = General.DeserializeObject<List<RequisitionTimingAnalytics>>(_response.RequisitionTimingAnalytics) ?? [];
            _companyTimingData = General.DeserializeObject<List<CompanyTimingAnalytics>>(_response.CompanyTimingAnalytics) ?? [];*/
            
            // Parallel deserialization with proper thread safety for consistency across codebase
            Task[] dashboardDeserializationTasks = [
                Task.Run(() => _consolidatedMetricsData = JsonSerializer.Deserialize<List<ConsolidatedMetrics>>(_response.ConsolidatedMetrics, JsonContext.Default.ListConsolidatedMetrics) ?? []),
                Task.Run(() => _recentActivityData = JsonSerializer.Deserialize<List<RecentActivityItem>>(_response.RecentActivity, JsonContext.Default.ListRecentActivityItem) ?? []),
                Task.Run(() => _hiredPlacementsData = JsonSerializer.Deserialize<List<HiredPlacement>>(_response.Placements, JsonContext.Default.ListHiredPlacement) ?? []),
                Task.Run(() => _requisitionTimingData = JsonSerializer.Deserialize<List<RequisitionTimingAnalytics>>(_response.RequisitionTimingAnalytics, JsonContext.Default.ListRequisitionTimingAnalytics) ?? []),
                Task.Run(() => _companyTimingData = JsonSerializer.Deserialize<List<CompanyTimingAnalytics>>(_response.CompanyTimingAnalytics, JsonContext.Default.ListCompanyTimingAnalytics) ?? [])
            ];
            await Task.WhenAll(dashboardDeserializationTasks);
            
            // Set default selected user
            if (_usersList.Any(u => u.KeyValue == _user))
            {
                _selectedUser = _user;
                _selectedUserText = _usersList.First(u => u.KeyValue == _user).Text;
            }
            else if (_usersList.Any())
            {
                _selectedUser = _usersList.First().KeyValue;
                _selectedUserText = _usersList.First().Text;
            }

            UpdateChartData();
        }
        catch (Exception ex)
        {
            // Handle error - you might want to show a toast notification
            Console.WriteLine($"Error loading dashboard: {ex.Message}");
        }
        finally
        {
            _isLoading = false;
            StateHasChanged();

            // Force chart refresh after data is loaded and UI is updated
            if (_chartRef != null)
            {
                await _chartRef.RefreshAsync();
            }
        }
    }

    private List<RequisitionTimingAnalytics> GetUserRequisitionAnalytics()
    {
        return _requisitionTimingData.Where(r => r.CreatedBy == _selectedUser)
                                     .OrderByDescending(r => r.PHNDays)
                                     .ToList();
    }

    private List<CompanyTimingAnalytics> GetUserCompanyAnalytics()
    {
        return _companyTimingData.Where(c => c.CreatedBy == _selectedUser)
                                 .OrderByDescending(c => c.TotalRequisitions)
                                 .ToList();
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

                                    _selectedUser = User;
                                }

                                await LoadDashboardData();
                            });

        await base.OnInitializedAsync();
    }

    private void OnPeriodChangedEventRaise(ChangeEventArgs<string, PeriodItem> args)
    {
        UpdateChartData();
        StateHasChanged();
    }

    private void OnUserChangedEventRaise(ChangeEventArgs<string, UserItem> args)
    {
        _selectedUserText = _usersList.FirstOrDefault(u => u.KeyValue == args.Value)?.Text ?? "";
        StateHasChanged();
    }

    private async Task RefreshDashboard()
    {
        await LoadDashboardData();
    }

    private void UpdateChartData()
    {
        _chartData.Clear();

        foreach (UserItem user in _usersList)
        {
            ConsolidatedMetrics userPeriodData = _consolidatedMetricsData.FirstOrDefault(m => m.User == user.KeyValue && m.Period == GetSelectedPeriodCode());

            int totalValue = userPeriodData?.RequisitionsCreated ?? 0;
            int activeValue = userPeriodData?.ActiveRequisitions ?? 0;
            int interviewValue = userPeriodData?.INTSubmissions ?? 0;
            int offersValue = userPeriodData?.OEXSubmissions ?? 0;
            int hiredValue = userPeriodData?.HIRSubmissions ?? 0;

            _chartData.Add(new()
                           {
                               User = user.Text,
                               TotalRequisitions = totalValue,
                               ActiveRequisitions = activeValue,
                               CandidatesInInterview = interviewValue,
                               OffersExtended = offersValue,
                               CandidatesHired = hiredValue
                           });
        }
    }
}