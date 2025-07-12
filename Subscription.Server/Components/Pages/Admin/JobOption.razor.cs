#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.Server
// File Name:           JobOption.razor.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu, Brijesh Dubey
// Created On:          03-16-2025 19:03
// Last Updated On:     05-01-2025 21:01
// *****************************************/

#endregion

namespace Subscription.Server.Components.Pages.Admin;

public partial class JobOption : ComponentBase
{
    private static TaskCompletionSource<bool> _initializationTaskSource;

    private readonly SemaphoreSlim _semaphore = new(1, 1);

    private bool AdminScreens { get; set; }

    private List<JobOptions> DataSource { get; set; } = [];

    private SfGrid<JobOptions> Grid { get; set; }

    private string JobOptionAuto { get; set; }

    /// <summary>
    ///     Gets or sets the 'JobOptionsDialog' instance used for managing JobOption information in the administrative context.
    ///     This dialog is used for both creating new JobOption and editing existing JobOption.
    /// </summary>
    private JobOptionDialog JobOptionDialog { get; set; }

    /// <summary>
    ///     Gets or sets the JobOptionRecord property of the JobOption class.
    ///     The JobOptionRecord property represents a single JobOption in the application.
    ///     It is used to hold the data of the selected JobOption in the JobOption grid.
    ///     The data is encapsulated in a JobOptions object, which is defined in the ProfSvc_Classes namespace.
    /// </summary>
    private JobOptions JobOptionRecord { get; set; } = new();

    /// <summary>
    ///     Gets or sets the clone of a JobOption record. This property is used to hold a copy of a JobOption record for
    ///     operations like editing or adding a JobOption.
    ///     When adding a new JobOption, a new instance of JobOption is created and assigned to this property.
    ///     When editing an existing JobOption, a copy of the JobOption record to be edited is created and assigned to this property.
    /// </summary>
    private JobOptions JobOptionRecordClone { get; set; } = new();

    /// <summary>
    ///     Gets or sets the instance of the ILocalStorageService. This service is used for managing the local storage of the
    ///     browser.
    ///     It is used in this class to retrieve and store JobOption-specific data, such as the "autoJobOption" item and the
    ///     `LoginCookyUser` object.
    /// </summary>
    [Inject]
    private ILocalStorageService LocalStorage { get; set; }

    /// <summary>
    ///     Gets or sets the instance of the NavigationManager. This service is used for managing navigation across the
    ///     application.
    ///     It is used in this class to redirect the user to different pages based on their role and authentication status.
    ///     For example, if the user's role is not "AD" (Administrator), the user is redirected to the Dashboard page.
    /// </summary>
    [Inject]
    private NavigationManager NavManager { get; set; }

    /// <summary>
    ///     Gets or sets the Redis service for cache operations.
    ///     Using DI singleton to avoid connection leaks.
    /// </summary>
    [Inject]
    private RedisService RedisService { get; set; }

    /// <summary>
    ///     Gets or sets the RoleID for the current user. The RoleID is used to determine the user's permissions within the
    ///     application.
    /// </summary>
    private string RoleID { get; set; }

    private string RoleName { get; set; }

    /// <summary>
    ///     Gets or sets the instance of the ILocalStorageService. This service is used for managing the local storage of the
    ///     browser.
    ///     It is used in this class to retrieve and store JobOption-specific data, such as the "autoJobOption" item and the
    ///     `LoginCookyUser` object.
    /// </summary>
    [Inject]
    private ISessionStorageService SessionStorage { get; set; }

    private List<KeyValues> TaxTerms { get; set; } = [];

    /// <summary>
    ///     Gets or sets the JobOption of the JobOption Dialog in the administrative context.
    ///     The JobOption changes based on the action being performed on the JobOption record - "Add" when a new JobOption is being added,
    ///     and "Edit" when an existing JobOption's details are being modified.
    /// </summary>
    private string Title { get; set; } = "Edit";

    private string User { get; set; }

    private bool VisibleSpinner { get; set; }

    private async Task DataBound(object arg)
    {
        if (Grid.TotalItemCount > 0)
        {
            await Grid.SelectRowAsync(0);
        }
    }

    /// <summary>
    ///     Asynchronously edits the JobOption with the given ID. If the ID is 0, a new JobOption is created.
    /// </summary>
    /// <param name="id">The ID of the JobOption to edit. If this parameter is 0, a new JobOption is created.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    /// <remarks>
    ///     This method performs the following steps:
    ///     - Retrieves the selected records from the grid.
    ///     - If the first selected record's ID does not match the given ID, it selects the row with the given ID in the grid.
    ///     - If the ID is 0, it sets the title to "Add" and initializes a new JobOption record clone if it does not exist,
    ///     or clears its data if it does.
    ///     - If the ID is not 0, it sets the title to "Edit" and copies the current JobOption record to the clone.
    ///     - Sets the entity of the JobOption record clone to "JobOption".
    ///     - Triggers a state change.
    ///     - Shows the admin dialog.
    /// </remarks>
    private Task EditJobOptionAsync(string id = "") => ExecuteMethod(async () =>
                                                                     {
                                                                         VisibleSpinner = true;
                                                                         if (id.NotNullOrWhiteSpace())
                                                                         {
                                                                             List<JobOptions> _selectedList = await Grid.GetSelectedRecordsAsync();
                                                                             if (_selectedList.Count == 0 || _selectedList.First().KeyValue != id)
                                                                             {
                                                                                 int _index = await Grid.GetRowIndexByPrimaryKeyAsync(id);
                                                                                 await Grid.SelectRowAsync(_index);
                                                                             }
                                                                         }

                                                                         if (id.NullOrWhiteSpace())
                                                                         {
                                                                             Title = "Add";
                                                                             if (JobOptionRecordClone == null)
                                                                             {
                                                                                 JobOptionRecordClone = new();
                                                                             }
                                                                             else
                                                                             {
                                                                                 JobOptionRecordClone.Clear();
                                                                             }
                                                                         }
                                                                         else
                                                                         {
                                                                             Title = "Edit";
                                                                             JobOptionRecordClone = JobOptionRecord.Copy();
                                                                         }

                                                                         VisibleSpinner = false;
                                                                         await JobOptionDialog.ShowDialog();
                                                                     });

    /// <summary>
    ///     Executes the provided task within a semaphore lock. If the semaphore is currently locked, the method will return
    ///     immediately.
    ///     If an exception occurs during the execution of the task, it will be logged using the provided logger.
    /// </summary>
    /// <param name="task">The task to be executed.</param>
    /// <returns>
    ///     A task that represents the asynchronous operation.
    /// </returns>
    private Task ExecuteMethod(Func<Task> task) => General.ExecuteMethod(_semaphore, task);

    /// <summary>
    ///     Handles the filtering of the grid based on the provided JobOption.
    ///     This method is triggered when a JobOption is selected in the grid.
    ///     It sets the filter value to the selected JobOption and refreshes the grid to update the displayed data.
    ///     The method ensures that the grid is not refreshed multiple times simultaneously by using a toggling flag.
    /// </summary>
    /// <param name="jobOption">The selected JobOption in the grid, encapsulated in a ChangeEventArgs object.</param>
    /// <returns>A Task representing the asynchronous operation of refreshing the grid.</returns>
    private Task FilterGrid(ChangeEventArgs<string, KeyValues> jobOption)
    {
        return ExecuteMethod(async () =>
                             {
                                 await FilterSet(jobOption.Value.NullOrWhiteSpace() ? "" : jobOption.Value);
                                 await SetDataSource();
                             });
    }

    /// <summary>
    ///     Sets the filter value for the JobOption component.
    ///     This method is used to update the static Filter property with the passed value.
    ///     The passed value is processed by the General.FilterSet method before being assigned to the Filter property.
    /// </summary>
    /// <param name="value">The value to be set as the filter.</param>
    private async Task FilterSet(string value)
    {
        JobOptionAuto = value;
        await LocalStorage.SetItemAsStringAsync("autoJobOption", value);
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            string _result = await LocalStorage.GetItemAsStringAsync("autoJobOption");

            JobOptionAuto = _result.NotNullOrWhiteSpace() && _result != "null" ? _result : "";

            try
            {
                await SetDataSource();
                _initializationTaskSource.SetResult(true);
            }
            catch
            {
                //
            }
        }
    }

    /// <summary>
    ///     This method is called when the component is initialized.
    ///     It retrieves the user's login information from the local storage and checks the user's role.
    ///     If the user's role is not "AD" (Administrator), it redirects the user to the home page.
    /// </summary>
    /// <returns>A Task that represents the asynchronous operation.</returns>
    protected override async Task OnInitializedAsync()
    {
        _initializationTaskSource = new();
        await ExecuteMethod(async () =>
                            {
                                IEnumerable<Claim> _claims = await General.GetClaimsToken(LocalStorage, SessionStorage);

                                if (_claims == null)
                                {
                                    NavManager.NavigateTo($"{NavManager.BaseUri}login", true);
                                }
                                else
                                {
                                    Claim[] _enumerable = _claims as Claim[] ?? _claims.ToArray();
                                    User = _enumerable.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value.ToUpperInvariant();
                                    RoleName = _enumerable.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value.ToUpperInvariant();
                                    if (User.NullOrWhiteSpace())
                                    {
                                        NavManager.NavigateTo($"{NavManager.BaseUri}login", true);
                                    }

                                    // Set user permissions
                                    AdminScreens = _enumerable.Any(claim => claim.Type == "Permission" && claim.Value == "AdminScreens");
                                    // Using injected RedisService singleton instead of creating new instances to avoid connection leaks
                                    RedisValue _cacheValues = await RedisService.GetAsync(nameof(CacheObjects.TaxTerms));
                                    TaxTerms = _cacheValues.IsNull ? [] : JsonConvert.DeserializeObject<List<KeyValues>>(_cacheValues.ToString());
                                }
                            });

        await base.OnInitializedAsync();
    }

    /// <summary>
    ///     Refreshes the grid component of the JobOption page.
    ///     This method is used to update the grid component and reflect any changes made to the data.
    /// </summary>
    /// <returns>A Task that represents the asynchronous operation.</returns>
    private async Task RefreshGrid() => await SetDataSource();

    /// <summary>
    ///     Handles the event of a row being selected in the JobOption grid.
    /// </summary>
    /// <param name="jobOption">The selected row data encapsulated in a RowSelectEventArgs object.</param>
    private void RowSelected(RowSelectingEventArgs<JobOptions> jobOption) => JobOptionRecord = jobOption.Data;

    /// <summary>
    ///     Saves the changes made to the JobOption record.
    /// </summary>
    /// <param name="context">The context for the form being edited.</param>
    /// <returns>A Task that represents the asynchronous operation.</returns>
    /// <remarks>
    ///     This method calls the General.SaveJobOptionsAsync method, passing in the necessary parameters to save the changes
    ///     made to the JobOptionRecordClone.
    ///     After the save operation, it refreshes the grid and selects the updated row.
    /// </remarks>
    private Task SaveJobOption(EditContext context) => ExecuteMethod(async () =>
                                                                     {
                                                                         Dictionary<string, string> _parameters = new()
                                                                                                                  {
                                                                                                                      {"cacheName", nameof(CacheObjects.JobOptions)}
                                                                                                                  };
                                                                         string _response = await General.ExecuteRest<string>("Admin/SaveJobOptions", _parameters,
                                                                                                                              JobOptionRecordClone);
                                                                         if (JobOptionRecordClone != null)
                                                                         {
                                                                             JobOptionRecord = JobOptionRecordClone.Copy();
                                                                         }

                                                                         if (_response.NotNullOrWhiteSpace() && _response != "[]")
                                                                         {
                                                                             await FilterSet("");
                                                                             DataSource = General.DeserializeObject<List<JobOptions>>(_response);
                                                                             await Grid.Refresh();
                                                                         }
                                                                     });

    private async Task SetDataSource()
    {
        Dictionary<string, string> _parameters = new()
                                                 {
                                                     {"methodName", "Admin_GetJobOptions"},
                                                     {"filter", JobOptionAuto ?? ""}
                                                 };
        string _returnValue = await General.ExecuteRest<string>("Admin/GetAdminList", _parameters, null, false);
        DataSource = JsonConvert.DeserializeObject<List<JobOptions>>(_returnValue);
        
        await Grid.Refresh();
    }
}