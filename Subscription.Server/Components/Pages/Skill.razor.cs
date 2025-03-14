#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.Server
// File Name:           Skill.razor.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu, Brijesh Dubey
// Created On:          03-13-2025 20:03
// Last Updated On:     03-13-2025 20:03
// *****************************************/

#endregion

namespace Subscription.Server.Components.Pages;

public partial class Skill : ComponentBase
{
    private static TaskCompletionSource<bool> _initializationTaskSource;

    private Query _query = new();
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    /// <summary>
    ///     Gets or sets the 'AdminListDialog' instance used for managing Skill information in the administrative context.
    ///     This dialog is used for both creating new Skill and editing existing Skill.
    /// </summary>
    private AdminListDialog AdminDialog
    {
        get;
        set;
    }

    public AdminGrid AdminGrid
    {
        get;
        set;
    }

    private bool AdminScreens
    {
        get;
        set;
    }

    [Inject]
    private IConfiguration Configuration
    {
        get;
        set;
    }

    /// <summary>
    ///     Gets or sets the dialog service used for displaying confirmation dialogs.
    /// </summary>
    /// <value>
    ///     An instance of <see cref="SfDialogService" /> that provides methods for showing dialogs and handling user
    ///     interactions
    ///     with those dialogs.
    /// </value>
    /// <remarks>
    ///     The <see cref="SfDialogService" /> is used to display confirmation dialogs to the user. It provides methods such as
    ///     <see cref="SfDialogService.ConfirmAsync" /> to show a confirmation dialog and await the user's response.
    /// </remarks>
    [Inject]
    private SfDialogService DialogService
    {
        get;
        set;
    }

    private string SkillAuto
    {
        get;
        set;
    }

    /// <summary>
    ///     Gets or sets the SkillRecord property of the Skill class.
    ///     The SkillRecord property represents a single Skill in the application.
    ///     It is used to hold the data of the selected Skill in the Skill grid.
    ///     The data is encapsulated in a AdminList object, which is defined in the ProfSvc_Classes namespace.
    /// </summary>
    private AdminList SkillRecord
    {
        get;
        set;
    } = new();

    /// <summary>
    ///     Gets or sets the clone of a Skill record. This property is used to hold a copy of a Skill record for
    ///     operations like editing or adding a Skill.
    ///     When adding a new Skill, a new instance of Skill is created and assigned to this property.
    ///     When editing an existing Skill, a copy of the Skill record to be edited is created and assigned to this property.
    /// </summary>
    private AdminList SkillRecordClone
    {
        get;
        set;
    } = new();

    /// <summary>
    ///     Gets or sets the filter value for the application Skill in the administrative context.
    ///     This static property is used to filter the Skill based on certain criteria in the administrative context.
    /// </summary>
    private static string Filter
    {
        get;
        set;
    }

    private SfGrid<AdminList> Grid
    {
        get;
        set;
    }

    /// <summary>
    ///     Gets or sets the instance of the ILocalStorageService. This service is used for managing the local storage of the
    ///     browser.
    ///     It is used in this class to retrieve and store Skill-specific data, such as the "autoSkill" item and the
    ///     `LoginCookyUser` object.
    /// </summary>
    [Inject]
    private ILocalStorageService LocalStorage
    {
        get;
        set;
    }

    /// <summary>
    ///     Gets or sets the ILogger instance used for logging in the Skill class.
    /// </summary>
    /// <remarks>
    ///     This property is used to log information about the execution of tasks and methods within the Skill class.
    ///     It is injected at runtime by the dependency injection system.
    /// </remarks>
    [Inject]
    private ILogger<Skill> Logger
    {
        get;
        set;
    }

    /// <summary>
    ///     Gets or sets the `LoginCooky` object for the current Skill.
    ///     This object contains information about the user's login session, including their ID, name, email address, role,
    ///     last login date, and login IP.
    ///     It is used to manage user authentication and authorization within the application.
    /// </summary>
    private LoginCooky LoginCookyUser
    {
        get;
        set;
    }

    /// <summary>
    ///     Gets or sets the instance of the NavigationManager. This service is used for managing navigation across the
    ///     application.
    ///     It is used in this class to redirect the user to different pages based on their role and authentication status.
    ///     For example, if the user's role is not "AD" (Administrator), the user is redirected to the Dashboard page.
    /// </summary>
    [Inject]
    private NavigationManager NavManager
    {
        get;
        set;
    }

    /// <summary>
    ///     Gets or sets the RoleID for the current user. The RoleID is used to determine the user's permissions within the
    ///     application.
    /// </summary>
    private string RoleID
    {
        get;
        set;
    }

    private string RoleName
    {
        get;
        set;
    }

    /// <summary>
    ///     Gets or sets the instance of the ILocalStorageService. This service is used for managing the local storage of the
    ///     browser.
    ///     It is used in this class to retrieve and store Skill-specific data, such as the "autoSkill" item and the
    ///     `LoginCookyUser` object.
    /// </summary>
    [Inject]
    private ISessionStorageService SessionStorage
    {
        get;
        set;
    }

    private SfSpinner Spinner
    {
        get;
        set;
    }

    /// <summary>
    ///     Gets or sets the Skill of the Skill Dialog in the administrative context.
    ///     The Skill changes based on the action being performed on the Skill record - "Add" when a new Skill is being added,
    ///     and "Edit" when an existing Skill's details are being modified.
    /// </summary>
    private string Title
    {
        get;
        set;
    } = "Edit";

    private string User
    {
        get;
        set;
    }

    private bool VisibleSpinner
    {
        get;
        set;
    }

    private async Task DataBound(object arg)
    {
        if (Grid.TotalItemCount > 0)
        {
            await Grid.SelectRowAsync(0);
        }
    }

    /// <summary>
    ///     Asynchronously edits the skill with the given ID. If the ID is 0, a new skill is created.
    /// </summary>
    /// <param name="id">The ID of the skill to edit. If this parameter is 0, a new skill is created.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    /// <remarks>
    ///     This method performs the following steps:
    ///     - Retrieves the selected records from the grid.
    ///     - If the first selected record's ID does not match the given ID, it selects the row with the given ID in the grid.
    ///     - If the ID is 0, it sets the title to "Add" and initializes a new skill record clone if it does not exist,
    ///     or clears its data if it does.
    ///     - If the ID is not 0, it sets the title to "Edit" and copies the current skill record to the clone.
    ///     - Sets the entity of the skill record clone to "Skill".
    ///     - Triggers a state change.
    ///     - Shows the admin dialog.
    /// </remarks>
    private Task EditSkillAsync(int id = 0) => ExecuteMethod(async () =>
                                                                   {
                                                                       VisibleSpinner = true;
                                                                       if (id != 0)
                                                                       {
                                                                           List<AdminList> _selectedList = await Grid.GetSelectedRecordsAsync();
                                                                           if (_selectedList.Count == 0 || _selectedList.First().ID != id)
                                                                           {
                                                                               int _index = await Grid.GetRowIndexByPrimaryKeyAsync(id);
                                                                               await Grid.SelectRowAsync(_index);
                                                                           }
                                                                       }

                                                                       if (id == 0)
                                                                       {
                                                                           Title = "Add";
                                                                           if (SkillRecordClone == null)
                                                                           {
                                                                               SkillRecordClone = new();
                                                                           }
                                                                           else
                                                                           {
                                                                               SkillRecordClone.Clear();
                                                                           }
                                                                       }
                                                                       else
                                                                       {
                                                                           Title = "Edit";
                                                                           SkillRecordClone = SkillRecord.Copy();
                                                                       }

                                                                       VisibleSpinner = false;
                                                                       SkillRecordClone.Entity = "Skill";
                                                                       await AdminDialog.ShowDialog();
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
    ///     Handles the filtering of the grid based on the provided skill.
    ///     This method is triggered when a skill is selected in the grid.
    ///     It sets the filter value to the selected skill and refreshes the grid to update the displayed data.
    ///     The method ensures that the grid is not refreshed multiple times simultaneously by using a toggling flag.
    /// </summary>
    /// <param name="skill">The selected skill in the grid, encapsulated in a ChangeEventArgs object.</param>
    /// <returns>A Task representing the asynchronous operation of refreshing the grid.</returns>
    private Task FilterGrid(ChangeEventArgs<string, KeyValues> skill)
    {
        return ExecuteMethod(async () =>
                             {
                                 await FilterSet(skill.Value.NullOrWhiteSpace() ? string.Empty : skill.Value);
                                 await Grid.Refresh(true);
                                 //Count = await General.SetCountAndSelect(AdminGrid.Grid);
                             });
    }

    /// <summary>
    ///     Sets the filter value for the Skill component.
    ///     This method is used to update the static Filter property with the passed value.
    ///     The passed value is processed by the General.FilterSet method before being assigned to the Filter property.
    /// </summary>
    /// <param name="value">The value to be set as the filter.</param>
    private async Task FilterSet(string value)
    {
        SkillAuto = value;
        _query ??= new();
        _query.AddParams("Filter", value);
        await LocalStorage.SetItemAsStringAsync("autoSkill", value);
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            string _result = await LocalStorage.GetItemAsStringAsync("autoSkill");

            SkillAuto = _result.NotNullOrWhiteSpace() && _result != "null" ? _result : string.Empty;
            _query ??= new();
            _query.AddParams("Filter", SkillAuto);

            try
            {
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
                                    IEnumerable<Claim> _enumerable = _claims as Claim[] ?? _claims.ToArray();
                                    User = _enumerable.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value.ToUpperInvariant();
                                    RoleName = _enumerable.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value.ToUpperInvariant();
                                    if (User.NullOrWhiteSpace())
                                    {
                                        NavManager.NavigateTo($"{NavManager.BaseUri}login", true);
                                    }

                                    // Set user permissions
                                    AdminScreens = _enumerable.Any(claim => claim.Type == "Permission" && claim.Value == "AdminScreens");
                                }
                            });

        //_initializationTaskSource.SetResult(true);
        await base.OnInitializedAsync();
    }

    /// <summary>
    ///     Refreshes the grid component of the Skill page.
    ///     This method is used to update the grid component and reflect any changes made to the data.
    /// </summary>
    /// <returns>A Task that represents the asynchronous operation.</returns>
    private Task RefreshGrid() => Grid.Refresh(true);

    /// <summary>
    ///     Handles the event of a row being selected in the Skill grid.
    /// </summary>
    /// <param name="skill">The selected row data encapsulated in a RowSelectEventArgs object.</param>
    private void RowSelected(RowSelectingEventArgs<AdminList> skill) => SkillRecord = skill.Data;

    /// <summary>
    ///     Saves the changes made to the skill record.
    /// </summary>
    /// <param name="context">The context for the form being edited.</param>
    /// <returns>A Task that represents the asynchronous operation.</returns>
    /// <remarks>
    ///     This method calls the General.SaveAdminListAsync method, passing in the necessary parameters to save the changes
    ///     made to the SkillRecordClone.
    ///     After the save operation, it refreshes the grid and selects the updated row.
    /// </remarks>
    private Task SaveSkill(EditContext context) => ExecuteMethod(async () =>
                                                                       {
                                                                           Dictionary<string, string> _parameters = new()
                                                                                                                    {
                                                                                                                        {"methodName", "Admin_SaveSkill"},
                                                                                                                        {"parameterName", "Skill"},
                                                                                                                        {"containDescription", "false"},
                                                                                                                        {"isString", "false"},
                                                                                                                        {"cacheName", CacheObjects.Skills.ToString()}
                                                                                                                    };
                                                                           string _response = await General.ExecuteRest<string>("Admin/SaveAdminList", _parameters,
                                                                                                                                SkillRecordClone);
                                                                           if (SkillRecordClone != null)
                                                                           {
                                                                               SkillRecord = SkillRecordClone.Copy();
                                                                           }

                                                                           await Grid.Refresh(true);

                                                                           /*int _index = await Grid.GetRowIndexByPrimaryKeyAsync(_response.ToInt32());
                                                                           await Grid.SelectRowAsync(_index);*/
                                                                       });

    /// <summary>
    ///     Toggles the status of an AdminList item and shows a confirmation dialog.
    /// </summary>
    /// <param name="id">The ID of the AdminList item to toggle.</param>
    /// <param name="enabled">
    ///     The new status to set for the AdminList item. If true, the status is set to 2, otherwise it is
    ///     set to 1.
    /// </param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    private Task ToggleMethod(int id, bool enabled) => ExecuteMethod(async () =>
                                                                     {
                                                                         /*_selectedID = id;
                                                                         _toggleValue = enabled ? (byte)2 : (byte)1;*/
                                                                         List<AdminList> _selectedList = await Grid.GetSelectedRecordsAsync();
                                                                         if (_selectedList.Count == 0 || _selectedList.First().ID != id)
                                                                         {
                                                                             int _index = await Grid.GetRowIndexByPrimaryKeyAsync(id);
                                                                             await Grid.SelectRowAsync(_index);
                                                                         }

                                                                         if (await DialogService.ConfirmAsync(null, enabled ? "Disable Skill?" : "Enable Skill?",
                                                                                                              General.DialogOptions("Are you sure you want to <strong>"
                                                                                                                                    + (enabled ? "disable" : "enable") + "</strong> " +
                                                                                                                                    "this <i>Skill</i>?")))
                                                                         {
                                                                             Dictionary<string, string> _parameters = new()
                                                                                                                      {
                                                                                                                          {"methodName", "Admin_ToggleSkillStatus"},
                                                                                                                          {"id", id.ToString()}
                                                                                                                      };
                                                                             _ = await General.ExecuteRest<string>("Admin/ToggleAdminList", _parameters);

                                                                             await Grid.Refresh(true);

                                                                             int _index = await Grid.GetRowIndexByPrimaryKeyAsync(id);
                                                                             await Grid.SelectRowAsync(_index);
                                                                         }
                                                                         // await AdminGrid.DialogConfirm.ShowDialog();
                                                                     });

    /// <summary>
    ///     The AdminSkillAdaptor class is a data adaptor for the Admin Skill page.
    ///     It inherits from the DataAdaptor class and overrides the ReadAsync method.
    /// </summary>
    /// <remarks>
    ///     This class is used to handle data operations for the Admin Skill page.
    ///     It communicates with the server to fetch data based on the DataManagerRequest and a key.
    ///     The ReadAsync method is used to asynchronously fetch data from the server.
    ///     It uses the General.GetReadAsync method to perform the actual data fetching.
    /// </remarks>
    public class AdminSkillAdaptor : DataAdaptor
    {
        private readonly SemaphoreSlim _semaphoreSlim = new(1, 1);

        /// <summary>
        ///     Asynchronously fetches data for the Admin Skill page from the server.
        /// </summary>
        /// <param name="dm">The DataManagerRequest object that contains the parameters for the data request.</param>
        /// <param name="key">An optional key used to fetch specific data. Default is null.</param>
        /// <returns>A Task that represents the asynchronous operation. The Task result contains the fetched data as an object.</returns>
        /// <remarks>
        ///     This method uses the General.GetReadAsync method to fetch data from the server.
        ///     It sets a flag to prevent multiple simultaneous reads.
        /// </remarks>
        public override async Task<object> ReadAsync(DataManagerRequest dm, string key = null)
        {
            if (!await _semaphoreSlim.WaitAsync(TimeSpan.Zero))
            {
                return null;
            }

            if (_initializationTaskSource == null)
            {
                return null;
            }

            await _initializationTaskSource.Task;
            try
            {
                Dictionary<string, string> _parameters = new()
                                                         {
                                                             {"methodName", "Admin_GetSkills"},
                                                             {"filter", dm.Params["Filter"]?.ToString() ?? string.Empty}
                                                         };
                string _returnValue = await General.ExecuteRest<string>("Admin/GetAdminList", _parameters, null, false);
                List<AdminList> _adminList = JsonConvert.DeserializeObject<List<AdminList>>(_returnValue);
                return dm.RequiresCounts ? new DataResult
                                           {
                                               Result = _adminList,
                                               Count = _adminList.Count
                                           }
                           : _adminList;
            }
            catch
            {
                return null;
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }
    }
}