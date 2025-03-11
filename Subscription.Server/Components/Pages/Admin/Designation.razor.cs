#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Profsvc_AppTrack
// Project:             Profsvc_AppTrack.Client
// File Name:           Designation.razor.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu
// Created On:          1-31-2024 16:7
// Last Updated On:     1-31-2024 21:17
// *****************************************/

#endregion

namespace Subscription.Server.Components.Pages.Admin;

/// <summary>
///     The Designation class is a part of the ProfSvc_AppTrack.Pages.Admin namespace.
///     It is used to manage the designations in the administrative context of the application.
///     This class includes properties for managing the local storage of the browser, user's login session, navigation
///     across the application, and user's permissions.
///     It also includes methods for handling data, editing designations, filtering the grid based on the provided
///     designation, saving the changes made to the designation record, and toggling the status of an AdminList item.
/// </summary>
public partial class Designation
{
    private static TaskCompletionSource<bool> _initializationTaskSource;
    private int _selectedID;
    private readonly SemaphoreSlim _semaphore = new(1, 1);
    private byte _toggleValue;

    /// <summary>
    ///     Gets or sets the 'AdminListDialog' instance used for managing title information in the administrative context.
    ///     This dialog is used for both creating new title and editing existing title.
    /// </summary>
    private AdminListDialog AdminDialog
    {
        get;
        set;
    }

    /*
    /// <summary>
    ///     Gets or sets the AdminGrid property of the Designation class.
    ///     This property is of type AdminGrid{AdminList} and is used to manage the administrative list in the grid format.
    ///     It provides functionalities such as selecting, filtering, and refreshing the grid.
    /// </summary>
    private AdminGrid AdminGrid
    {
        get;
        set;
    }
    */

    private Query _query = new();

    [Inject]
    private IConfiguration Configuration
    {
        get;
        set;
    }

    /// <summary>
    ///     Gets or sets the count of titles currently displayed in the grid view on the Designation page.
    ///     This property is used in the 'DataHandler' method to store the count of title and
    ///     to select the first title if the count is more than zero.
    /// </summary>
    private static int Count
    {
        get;
        set;
    } = 24;

    /*
    [Inject]
    private ICryptoService Crypto
    {
        get;
        set;
    }
    */

    /// <summary>
    ///     Gets or sets the DesignationRecord property of the Designation class.
    ///     The DesignationRecord property represents a single title in the application.
    ///     It is used to hold the data of the selected title in the title grid.
    ///     The data is encapsulated in a AdminList object, which is defined in the ProfSvc_Classes namespace.
    /// </summary>
    private AdminList DesignationRecord
    {
        get;
        set;
    } = new();

    /// <summary>
    ///     Gets or sets the clone of a Designation record. This property is used to hold a copy of a Designation record for
    ///     operations like editing or adding a title.
    ///     When adding a new title, a new instance of Title is created and assigned to this property.
    ///     When editing an existing title, a copy of the Title record to be edited is created and assigned to this property.
    /// </summary>
    private AdminList DesignationRecordClone
    {
        get;
        set;
    } = new();

    /// <summary>
    ///     Gets or sets the filter value for the application titles in the administrative context.
    ///     This static property is used to filter the titles based on certain criteria in the administrative context.
    /// </summary>
    private static string Filter
    {
        get;
        set;
    }

    /// <summary>
    ///     Gets or sets the JavaScript runtime instance. The JavaScript runtime provides a mechanism for running JavaScript in
    ///     the context of the component.
    ///     This property is injected into the component and is used to call JavaScript functions from .NET code.
    ///     For example, it is used in the 'Save' method to scroll to a specific row in the grid, and in the
    ///     'ToggleStatusAsync' method to toggle the status of a title.
    /// </summary>
    [Inject]
    private IJSRuntime JsRuntime
    {
        get;
        set;
    }

    /// <summary>
    ///     Gets or sets the instance of the ILocalStorageService. This service is used for managing the local storage of the
    ///     browser.
    ///     It is used in this class to retrieve and store title-specific data, such as the "autoTitle" item and the
    ///     `LoginCookyUser` object.
    /// </summary>
    [Inject]
    private ILocalStorageService LocalStorage
    {
        get;
        set;
    }

    /// <summary>
    ///     Gets or sets the ILogger instance used for logging in the Designation class.
    /// </summary>
    /// <remarks>
    ///     This property is used to log information about the execution of tasks and methods within the Designation class.
    ///     It is injected at runtime by the dependency injection system.
    /// </remarks>
    [Inject]
    private ILogger<Designation> Logger
    {
        get;
        set;
    }

    /// <summary>
    ///     Gets or sets the `LoginCooky` object for the current title.
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

    /// <summary>
    ///     Gets or sets the title of the Designation Dialog in the administrative context.
    ///     The title changes based on the action being performed on the title record - "Add" when a new title is being added,
    ///     and "Edit" when an existing title's details are being modified.
    /// </summary>
    private string Title
    {
        get;
        set;
    } = "Edit";

    private SfGrid<AdminList> Grid
    {
        get;
        set;
    }

    public AdminGrid AdminGrid
    {
        get;
        set;
    }

    /// <summary>
    ///     Handles the data for the Designation page. It counts the number of records in the current view of the grid.
    ///     If there are records, it selects the first row.
    /// </summary>
    /// <returns>A Task that represents the asynchronous operation.</returns>
    private Task DataHandler()
    {
        return Task.CompletedTask;
        //return ExecuteMethod(async () => Count = await General.SetCountAndSelect(AdminGrid.Grid));
    }

    /// <summary>
    ///     Asynchronously edits the designation with the given ID. If the ID is 0, a new designation is created.
    /// </summary>
    /// <param name="id">The ID of the designation to edit. If this parameter is 0, a new designation is created.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    /// <remarks>
    ///     This method performs the following steps:
    ///     - Retrieves the selected records from the grid.
    ///     - If the first selected record's ID does not match the given ID, it selects the row with the given ID in the grid.
    ///     - If the ID is 0, it sets the title to "Add" and initializes a new designation record clone if it does not exist,
    ///     or clears its data if it does.
    ///     - If the ID is not 0, it sets the title to "Edit" and copies the current designation record to the clone.
    ///     - Sets the entity of the designation record clone to "Title".
    ///     - Triggers a state change.
    ///     - Shows the admin dialog.
    /// </remarks>
    private Task EditDesignationAsync(int id = 0)
    {
        return ExecuteMethod(async () =>
                             {
                                 List<AdminList> _selectedList = await Grid.GetSelectedRecordsAsync();
                                 if (_selectedList.Any() && _selectedList.First().ID != id)
                                 {
                                     int _index = await Grid.GetRowIndexByPrimaryKeyAsync(id);
                                     await Grid.SelectRowAsync(_index);
                                 }

                                 if (id == 0)
                                 {
                                     Title = "Add";
                                     if (DesignationRecordClone == null)
                                     {
                                         DesignationRecordClone = new();
                                     }
                                     else
                                     {
                                         DesignationRecordClone.Clear();
                                     }
                                 }
                                 else
                                 {
                                     Title = "Edit";
                                     DesignationRecordClone = DesignationRecord.Copy();
                                 }

                                 DesignationRecordClone.Entity = "Title";

                                 StateHasChanged();
                                 await AdminDialog.ShowDialog();
                             });
    }

    /// <summary>
    ///     Executes the provided task within a semaphore lock. If the semaphore is currently locked, the method will return
    ///     immediately.
    ///     If an exception occurs during the execution of the task, it will be logged using the provided logger.
    /// </summary>
    /// <param name="task">The task to be executed.</param>
    /// <returns>
    ///     A task that represents the asynchronous operation.
    /// </returns>
    private Task ExecuteMethod(Func<Task> task)
    {
        return General.ExecuteMethod(_semaphore, task);
    }

    /// <summary>
    ///     Handles the filtering of the grid based on the provided designation.
    ///     This method is triggered when a designation is selected in the grid.
    ///     It sets the filter value to the selected designation and refreshes the grid to update the displayed data.
    ///     The method ensures that the grid is not refreshed multiple times simultaneously by using a toggling flag.
    /// </summary>
    /// <param name="designation">The selected designation in the grid, encapsulated in a ChangeEventArgs object.</param>
    /// <returns>A Task representing the asynchronous operation of refreshing the grid.</returns>
    private Task FilterGrid(ChangeEventArgs<string, KeyValues> designation)
    {
        return ExecuteMethod(async () =>
                             {
                                 FilterSet(designation.Value);
                                 await Grid.Refresh();
                                 //Count = await General.SetCountAndSelect(AdminGrid.Grid);
                             });
    }

    /// <summary>
    ///     Sets the filter value for the Designation component.
    ///     This method is used to update the static Filter property with the passed value.
    ///     The passed value is processed by the General.FilterSet method before being assigned to the Filter property.
    /// </summary>
    /// <param name="value">The value to be set as the filter.</param>
    private static void FilterSet(string value)
    {
        //Filter = General.FilterSet(Filter, value);
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
                                /*General.CheckStart(NavManager, Configuration);
                                LoginCookyUser = await NavManager.RedirectInner(LocalStorage, Crypto);
                                RoleID = LoginCookyUser.RoleID;
                                string _result = await LocalStorage.GetItemAsStringAsync("autoTitle");
                                FilterSet(_result);
                                if (!LoginCookyUser.IsAdmin()) //Administrator only has the rights.
                                {
                                    NavManager.NavigateTo($"{NavManager.BaseUri}home", true);
                                }*/
                            });

        _initializationTaskSource.SetResult(true);
        await base.OnInitializedAsync();
    }

    /// <summary>
    ///     Refreshes the grid component of the Designation page.
    ///     This method is used to update the grid component and reflect any changes made to the data.
    /// </summary>
    /// <returns>A Task that represents the asynchronous operation.</returns>
    private Task RefreshGrid() => Grid.Refresh();

    /// <summary>
    ///     Handles the event of a row being selected in the Designation grid.
    /// </summary>
    /// <param name="designation">The selected row data encapsulated in a RowSelectEventArgs object.</param>
    private void RowSelected(RowSelectEventArgs<AdminList> designation) => DesignationRecord = designation.Data;

    /// <summary>
    ///     Saves the changes made to the designation record.
    /// </summary>
    /// <param name="context">The context for the form being edited.</param>
    /// <returns>A Task that represents the asynchronous operation.</returns>
    /// <remarks>
    ///     This method calls the General.SaveAdminListAsync method, passing in the necessary parameters to save the changes
    ///     made to the DesignationRecordClone.
    ///     After the save operation, it refreshes the grid and selects the updated row.
    /// </remarks>
    private Task SaveDesignation(EditContext context)
    {
        /*return ExecuteMethod(() => General.SaveAdminListAsync("Admin_SaveDesignation", "Designation", false, false, DesignationRecordClone, AdminGrid.Grid,
                                                              DesignationRecord, JsRuntime));*/
        return Task.CompletedTask;
    }

    /// <summary>
    ///     Toggles the status of an AdminList item and shows a confirmation dialog.
    /// </summary>
    /// <param name="id">The ID of the AdminList item to toggle.</param>
    /// <param name="enabled">
    ///     The new status to set for the AdminList item. If true, the status is set to 2, otherwise it is
    ///     set to 1.
    /// </param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    private Task ToggleMethod(int id, bool enabled)
    {
        return ExecuteMethod(async () =>
                             {
                                 _selectedID = id;
                                 _toggleValue = enabled ? (byte)2 : (byte)1;
                                 List<AdminList> _selectedList = await Grid.GetSelectedRecordsAsync();
                                 if (_selectedList.Any() && _selectedList.First().ID != id)
                                 {
                                     int _index = await Grid.GetRowIndexByPrimaryKeyAsync(id);
                                     await Grid.SelectRowAsync(_index);
                                 }

                                 // await AdminGrid.DialogConfirm.ShowDialog();
                             });
    }

    /// <summary>
    ///     Toggles the status of a designation asynchronously.
    /// </summary>
    /// <param name="designationID">The ID of the designation whose status is to be toggled.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    /// <remarks>
    ///     This method posts a toggle request to the "Admin_ToggleDesignationStatus" endpoint with the provided designation
    ///     ID.
    ///     The status toggle operation is not performed if it is already in progress.
    ///     After the status is toggled, the method refreshes the grid and selects the row with the toggled designation.
    /// </remarks>
    private Task ToggleStatusAsync(int designationID)
    {
        //return ExecuteMethod(() => General.PostToggleAsync("Admin_ToggleDesignationStatus", designationID, "ADMIN", false, AdminGrid.Grid, runtime: JsRuntime));
        return Task.CompletedTask;
    }

    /// <summary>
    ///     The AdminDesignationAdaptor class is a data adaptor for the Admin Designation page.
    ///     It inherits from the DataAdaptor class and overrides the ReadAsync method.
    /// </summary>
    /// <remarks>
    ///     This class is used to handle data operations for the Admin Designation page.
    ///     It communicates with the server to fetch data based on the DataManagerRequest and a key.
    ///     The ReadAsync method is used to asynchronously fetch data from the server.
    ///     It uses the General.GetReadAsync method to perform the actual data fetching.
    /// </remarks>
    public class AdminDesignationAdaptor : DataAdaptor
    {
        private readonly SemaphoreSlim _semaphoreSlim = new(1, 1);

        /// <summary>
        ///     Asynchronously fetches data for the Admin Designation page from the server.
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
                                                            {"methodName", "Admin_GetDesignations"},
                                                            {"filter", ""}
                                                        };
                string _returnValue = await General.ExecuteRest<string>("Admin/GetAdminList", _parameters, null, false);
                List<AdminList> _adminList = JsonConvert.DeserializeObject<List<AdminList>>(_returnValue);
                return dm.RequiresCounts ? new DataResult
                                          {
                                              Result = _adminList,
                                              Count = _adminList.Count
                                          }
                                          : _adminList;
                //object _returnValue = await General.GetReadAsync("Admin_GetDesignations", Filter, dm, false);
            }
            catch (Exception ex)
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