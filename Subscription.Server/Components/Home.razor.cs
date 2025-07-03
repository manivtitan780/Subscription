#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.Server
// File Name:           Home.razor.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu, Brijesh Dubey
// Created On:          06-13-2025 20:06
// Last Updated On:     06-24-2025 20:32
// *****************************************/

#endregion

namespace Subscription.Server.Components;

public partial class Home : ComponentBase
{
    // Data models - these would match your API response classes
    private DashboardData _dashboardData;
    private readonly string[] _groupColumns = ["Company"];
    private bool _isLoading = true;
    private List<HiredPlacement> Placements = [];
    private List<RecentActivity> RecentActivity = [];
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

    private List<KeyValues> Users { get; set; }

    private Task ExecuteMethod(Func<Task> task) => General.ExecuteMethod(_semaphoreMainPage, task);

    private string GetHireToOfferRatio()
    {
        /*TimeMetric metric = _timeMetrics.FirstOrDefault(m => m.DateRange == _selectedTimePeriod);
        return metric?.HireToOfferRatio.ToString("P0") ?? "0%";*/
        return "0%";
    }

    private Task GetMetData(ChangeEventArgs<string, string> arg)
    {
        StateHasChanged();
        return Task.CompletedTask;
    }

    private string GetMetricValue(string metricName)
    {
        /*TimeMetric metric = _timeMetrics.FirstOrDefault(m => m.DateRange == GetPeriod(_selectedTimePeriod));
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
               };*/
        return "0";
    }

    private static string GetPeriod(string period, bool reverse = false)
    {
        if (!reverse)
        {
            return period switch
                   {
                       "7 days" => "Last 7 Days",
                       "MTD" => "Month To Date",
                       "QTD" => "Quarter To Date",
                       "HYTD" => "Half Year To Date",
                       "YTD" => "Year To Date",
                       _ => "All Time"
                   };
        }

        return period switch
               {
                   "Last 7 Days" => "7 days",
                   "Month To Date" => "MTD",
                   "Quarter To Date" => "QTD",
                   "Half Year To Date" => "HYTD",
                   "Year To Date" => "YTD",
                   _ => "ALL"
               };
    }

    private string GetRoleName()
    {
        return RoleName switch
               {
                   "AD" => "Administrator",
                   "RS" => "Accounts Manager",
                   "RC" => "Recruiter",
                   _ => "Full Desk"
               };
    }
private string _selectedUser = "";

    private async Task LoadDashboardData()
    {
        _isLoading = true;
        try
        {
            // Call your WebAPI endpoint
            Dictionary<string, string> _parameters = new()
                                                     {
                                                         {"roleName", "AD"},
                                                         {"user", "DAVE"}
                                                     };
            ReturnDashboard _response = await General.ExecuteRest<ReturnDashboard>("Dashboard/GetAccountsManagerDashboard", _parameters, null, false);

            _dashboardData ??= new();
            _dashboardData.UserName = User;
            Users = General.DeserializeObject<List<KeyValues>>(_response.Users);
            /*List<DateCount> _dateCountsTotalRequisition = General.DeserializeObject<List<DateCount>>(_response.TotalRequisitions);
            List<DateCount> _dateCountsActiveRequisition = General.DeserializeObject<List<DateCount>>(_response.ActiveRequisitions);
            List<DateCount> _dateCountsCandidatesInInterview = General.DeserializeObject<List<DateCount>>(_response.CandidatesInInterview);
            List<DateCount> _dateCountsOffersExtended = General.DeserializeObject<List<DateCount>>(_response.OffersExtended);
            List<DateCount> _dateCountsCandidatesHired = General.DeserializeObject<List<DateCount>>(_response.CandidatesHired);
            List<RatioCount> _ratioCounts = General.DeserializeObject<List<RatioCount>>(_response.HireToOfferRatio);*/
            _consolidatedMetricsData = General.DeserializeObject<List<ConsolidatedMetrics>>(_response.ConsolidatedMetrics) ?? [];
            List<RecentActivity> _recentActivity = General.DeserializeObject<List<RecentActivity>>(_response.RecentActivity) ?? [];
            List<HiredPlacement> _placements = General.DeserializeObject<List<HiredPlacement>>(_response.Placements) ?? [];

            string[] _periods = ["7 days", "MTD", "QTD", "HYTD", "YTD"];

            foreach (KeyValues _user in Users)
            {
                DateCount _total = _dateCountsTotalRequisition.FirstOrDefault(d => d.User == _user.KeyValue);
                DateCount _active = _dateCountsActiveRequisition.FirstOrDefault(d => d.User == _user.KeyValue);
                DateCount _interview = _dateCountsCandidatesInInterview.FirstOrDefault(d => d.User == _user.KeyValue);
                DateCount _offers = _dateCountsOffersExtended.FirstOrDefault(d => d.User == _user.KeyValue);
                DateCount _hired = _dateCountsCandidatesHired.FirstOrDefault(d => d.User == _user.KeyValue);
                RatioCount _ratio = _ratioCounts.FirstOrDefault(d => d.User == _user.KeyValue);

                foreach (string _period in _periods)
                {
                    TimeMetric _timeData = new();
                    _timeData.DateRange.Add(GetPeriod(_period));
                    _timeData.TotalRequisitions.Add(_total.Count);
                    _timeData.ActiveRequisitions.Add(_active.Count);
                    _timeData.CandidatesInInterview.Add(_interview.Count);
                    _timeData.OffersExtended.Add(_offers.Count);
                    _timeData.CandidatesHired.Add(_hired.Count);
                    _timeData.HireToOfferRatio.Add(_ratio.Ratio);
                    _timeData.RecentActivities.AddRange(_recentActivity.Where(d => d.User == _user.KeyValue));
                    _timeData.Placements.AddRange(_placements.Where(d => d.User == _user.KeyValue));
                    _timeData.User.Add(_user.KeyValue);
                    _timeMetrics.Add(_timeData);
                }
            }

            foreach (string period in _periods)
            {
                IEnumerable<DateCount> _total = _dateCountsTotalRequisition.Where(d => d.Period == period);
                IEnumerable<DateCount> _active = _dateCountsActiveRequisition.Where(d => d.Period == period);
                IEnumerable<DateCount> _interview = _dateCountsCandidatesInInterview.Where(d => d.Period == period);
                IEnumerable<DateCount> _offers = _dateCountsOffersExtended.Where(d => d.Period == period);
                IEnumerable<DateCount> _hired = _dateCountsCandidatesHired.Where(d => d.Period == period);
                IEnumerable<RatioCount> _ratio = _ratioCounts.Where(d => d.Period == period);

                foreach(DateCount _t in _total)
                {
                    TimeMetric _timeData = new();
                    _timeData.DateRange.Add(GetPeriod(period));
                    _timeData.TotalRequisitions.Add(_t.Count);
                    _timeData.ActiveRequisitions.Add(_active.FirstOrDefault(d => d.User == _t.User).Count);
                    _timeData.CandidatesInInterview.Add(_interview.FirstOrDefault(d => d.User == _t.User).Count);
                    _timeData.OffersExtended.Add(_offers.FirstOrDefault(d => d.User == _t.User).Count);
                    _timeData.CandidatesHired.Add(_hired.FirstOrDefault(d => d.User == _t.User).Count);
                    _timeData.HireToOfferRatio.Add(_ratio.FirstOrDefault(d => d.User == _t.User).Ratio);
                    _timeData.User.Add(_t.User);
                    _timeMetrics.Add(_timeData);
                }

                /*_timeMetrics.Add(new());
                TimeMetric _time = new();
                _time.DateRange.Add(GetPeriod(period));
                                      _time.TotalRequisitions.Add(_total.Count);
                                      _time.ActiveRequisitions.Add(_active.To.Count);
                                     _time. CandidatesInInterview.Add( = _interview.Count,
                                     _time. OffersExtended.Add( = _offers.Count,
                                     _time. CandidatesHired.Add( = _hired.Count,
                                     _time. HireToOfferRatio.Add( = _ratio.Ratio
                                 });*/
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

                                    _selectedUser = User;
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