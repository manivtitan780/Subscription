#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.Server
// File Name:           Dash.razor.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu, Brijesh Dubey
// Created On:          06-28-2025 15:06
// Last Updated On:     06-28-2025 16:11
// *****************************************/

#endregion

#region Using

using Syncfusion.Blazor.Charts;

#endregion

namespace Subscription.Server.Components.Pages;

public partial class Dash
{
    private List<DateCounts> _activeRequisitionsData = [];
    private List<DateCounts> _candidatesHiredData = [];
    private List<DateCounts> _candidatesInInterviewData = [];
    private List<ChartDataPoint> _chartData = [];
    private SfChart _chartRef;

    private List<HiredPlacement> _hiredPlacementsData = [];
    private List<RatioCounts> _hireToOfferRatioData = [];
    private bool _isLoading = true;
    private List<DateCounts> _offersExtendedData = [];

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

    // Strongly typed data collections
    private List<DateCounts> _totalRequisitionsData = [];
    private readonly string _user = "DAVE"; // This should come from your authentication/session

    private List<UserItem> _usersList = [];

    private int GetCountByPeriod(DateCounts data)
    {
        return _selectedPeriod switch
               {
                   "LAST7D_COUNT" => data.LAST7D_COUNT,
                   "MTD_COUNT" => data.MTD_COUNT,
                   "QTD_COUNT" => data.QTD_COUNT,
                   "HYTD_COUNT" => data.HYTD_COUNT,
                   "YTD_COUNT" => data.YTD_COUNT,
                   _ => 0
               };
    }

    private string GetPeriodDisplayName()
    {
        return _periodsList.FirstOrDefault(p => p.Value == _selectedPeriod)?.Text ?? "Quarter To Date";
    }

    private int GetSelectedUserMetric(string metricType)
    {
        DateCounts userData = metricType switch
                              {
                                  "TotalRequisitions" => _totalRequisitionsData.FirstOrDefault(d => d.User == _selectedUser),
                                  "ActiveRequisitions" => _activeRequisitionsData.FirstOrDefault(d => d.User == _selectedUser),
                                  "CandidatesInInterview" => _candidatesInInterviewData.FirstOrDefault(d => d.User == _selectedUser),
                                  "OffersExtended" => _offersExtendedData.FirstOrDefault(d => d.User == _selectedUser),
                                  "CandidatesHired" => _candidatesHiredData.FirstOrDefault(d => d.User == _selectedUser),
                                  _ => default
                              };

        if (userData.Equals(default))
        {
            return 0;
        }

        return _selectedPeriod switch
               {
                   "LAST7D_COUNT" => userData.LAST7D_COUNT,
                   "MTD_COUNT" => userData.MTD_COUNT,
                   "QTD_COUNT" => userData.QTD_COUNT,
                   "HYTD_COUNT" => userData.HYTD_COUNT,
                   "YTD_COUNT" => userData.YTD_COUNT,
                   _ => 0
               };
    }

    private string GetSelectedUserRatio()
    {
        RatioCounts userData = _hireToOfferRatioData.FirstOrDefault(d => d.User == _selectedUser);

        if (userData.Equals(default))
        {
            return "0.00%";
        }

        float ratio = _selectedPeriod switch
                      {
                          "LAST7D_COUNT" => userData.LAST7D_RATIO,
                          "MTD_COUNT" => userData.MTD_RATIO,
                          "QTD_COUNT" => userData.QTD_RATIO,
                          "HYTD_COUNT" => userData.HYTD_RATIO,
                          "YTD_COUNT" => userData.YTD_RATIO,
                          _ => 0f
                      };

        return $"{ratio:P2}";
    }

    private List<SummaryGridItem> GetSummaryGridData()
    {
        List<SummaryGridItem> summaryData = [];
        string[] periodNames = ["Last 7 Days", "Month To Date", "Quarter To Date", "Half Year To Date", "Year To Date"];

        DateCounts userTotalReq = _totalRequisitionsData.FirstOrDefault(d => d.User == _selectedUser);
        DateCounts userActiveReq = _activeRequisitionsData.FirstOrDefault(d => d.User == _selectedUser);
        DateCounts userInterview = _candidatesInInterviewData.FirstOrDefault(d => d.User == _selectedUser);
        DateCounts userOffers = _offersExtendedData.FirstOrDefault(d => d.User == _selectedUser);
        DateCounts userHired = _candidatesHiredData.FirstOrDefault(d => d.User == _selectedUser);

        int[] totalCounts = [userTotalReq.LAST7D_COUNT, userTotalReq.MTD_COUNT, userTotalReq.QTD_COUNT, userTotalReq.HYTD_COUNT, userTotalReq.YTD_COUNT];
        int[] activeCounts = [userActiveReq.LAST7D_COUNT, userActiveReq.MTD_COUNT, userActiveReq.QTD_COUNT, userActiveReq.HYTD_COUNT, userActiveReq.YTD_COUNT];
        int[] interviewCounts = [userInterview.LAST7D_COUNT, userInterview.MTD_COUNT, userInterview.QTD_COUNT, userInterview.HYTD_COUNT, userInterview.YTD_COUNT];
        int[] offerCounts = [userOffers.LAST7D_COUNT, userOffers.MTD_COUNT, userOffers.QTD_COUNT, userOffers.HYTD_COUNT, userOffers.YTD_COUNT];
        int[] hiredCounts = [userHired.LAST7D_COUNT, userHired.MTD_COUNT, userHired.QTD_COUNT, userHired.HYTD_COUNT, userHired.YTD_COUNT];

        for (int i = 0; i < periodNames.Length; i++)
        {
            string successRate = offerCounts[i] > 0 ? ((double)hiredCounts[i] / offerCounts[i] * 100).ToString("F2") + "%" : "0.00%";

            summaryData.Add(new()
                            {
                                TimePeriod = periodNames[i],
                                Requisitions = totalCounts[i],
                                Active = activeCounts[i],
                                Interviews = interviewCounts[i],
                                Offers = offerCounts[i],
                                Hired = hiredCounts[i],
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

    private async Task LoadDashboardData()
    {
        _isLoading = true;
        StateHasChanged();

        try
        {
            Dictionary<string, string> _parameters = new()
                                                     {
                                                         {"roleName", "FD"},
                                                         {"user", _user}
                                                     };

            _response = await General.ExecuteRest<ReturnDashboard>("Dashboard/GetAccountsManagerDashboard", _parameters, null, false);

            // Parse Users
            List<Dictionary<string, string>> users = General.DeserializeObject<List<Dictionary<string, string>>>(_response.Users);
            _usersList = users.Select(u => new UserItem {KeyValue = u["KeyValue"], Text = u["Text"]}).ToList();

            // Deserialize all data into strongly typed objects
            _totalRequisitionsData = General.DeserializeObject<List<DateCounts>>(_response.TotalRequisitions) ?? [];
            _activeRequisitionsData = General.DeserializeObject<List<DateCounts>>(_response.ActiveRequisitions) ?? [];
            _candidatesInInterviewData = General.DeserializeObject<List<DateCounts>>(_response.CandidatesInInterview) ?? [];
            _offersExtendedData = General.DeserializeObject<List<DateCounts>>(_response.OffersExtended) ?? [];
            _candidatesHiredData = General.DeserializeObject<List<DateCounts>>(_response.CandidatesHired) ?? [];
            _hireToOfferRatioData = General.DeserializeObject<List<RatioCounts>>(_response.HireToOfferRatio) ?? [];
            _recentActivityData = General.DeserializeObject<List<RecentActivityItem>>(_response.RecentActivity) ?? [];
            _hiredPlacementsData = General.DeserializeObject<List<HiredPlacement>>(_response.Placements) ?? [];
            
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
            /*if (_chartRef != null)
            {
                 await Task.Delay(1000); // Small delay to ensure DOM is updated
                await _chartRef.RefreshAsync();
            }*/
        }
    }
    private Task ExecuteMethod(Func<Task> task) => General.ExecuteMethod(_semaphoreMainPage, task);
    private readonly SemaphoreSlim _semaphoreMainPage = new(1, 1);

    [Inject]
    private ILocalStorageService LocalStorage { get; set; }

    [Inject]
    private NavigationManager NavManager { get; set; }

    private string RoleName { get; set; }

    [Inject]
    private ISessionStorageService SessionStorage { get; set; }

    private string User { get; set; }
    
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
            DateCounts totalReq = _totalRequisitionsData.FirstOrDefault(d => d.User == user.KeyValue);
            DateCounts activeReq = _activeRequisitionsData.FirstOrDefault(d => d.User == user.KeyValue);
            DateCounts interview = _candidatesInInterviewData.FirstOrDefault(d => d.User == user.KeyValue);
            DateCounts offers = _offersExtendedData.FirstOrDefault(d => d.User == user.KeyValue);
            DateCounts hired = _candidatesHiredData.FirstOrDefault(d => d.User == user.KeyValue);

            int totalValue = !totalReq.Equals(default) ? GetCountByPeriod(totalReq) : 0;
            int activeValue = !activeReq.Equals(default) ? GetCountByPeriod(activeReq) : 0;
            int interviewValue = !interview.Equals(default) ? GetCountByPeriod(interview) : 0;
            int offersValue = !offers.Equals(default) ? GetCountByPeriod(offers) : 0;
            int hiredValue = !hired.Equals(default) ? GetCountByPeriod(hired) : 0;

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