#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.Server
// File Name:           Users.razor.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu, Brijesh Dubey
// Created On:          03-21-2025 21:03
// Last Updated On:     05-02-2025 15:21
// *****************************************/

#endregion

#region Using

using JsonSerializer = System.Text.Json.JsonSerializer;
using Role = Subscription.Model.Role;

#endregion

namespace Subscription.Server.Components.Pages.Admin;

public partial class Users : ComponentBase
{
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    private bool AdminScreens { get; set; }

    private List<User> DataSource { get; set; } = [];

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
    private SfDialogService DialogService { get; set; }

    private SfGrid<User> Grid { get; set; }

    /// <summary>
    ///     Gets or sets the instance of the ILocalStorageService. This service is used for managing the local storage of the
    ///     browser.
    ///     It is used in this class to retrieve and store User-specific data, such as the "autoUser" item and the
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

    private List<IntValues> Roles { get; set; } = [];

    /// <summary>
    ///     Gets or sets the instance of the ILocalStorageService. This service is used for managing the local storage of the
    ///     browser.
    ///     It is used in this class to retrieve and store User-specific data, such as the "autoUser" item and the
    ///     `LoginCookyUser` object.
    /// </summary>
    [Inject]
    private ISessionStorageService SessionStorage { get; set; }

    /// <summary>
    ///     Gets or sets the User of the User Dialog in the administrative context.
    ///     The User changes based on the action being performed on the User record - "Add" when a new User is being added,
    ///     and "Edit" when an existing User's details are being modified.
    /// </summary>
    private string Title { get; set; } = "Edit";

    private string User { get; set; }

    private string UserAuto { get; set; }

    /// <summary>
    ///     Gets or sets the 'UserDialog' instance used for managing User information in the administrative context.
    ///     This dialog is used for both creating new User and editing existing User.
    /// </summary>
    private UserDialog UserDialog { get; set; }

    /// <summary>
    ///     Gets or sets the UserRecord property of the User class.
    ///     The UserRecord property represents a single User in the application.
    ///     It is used to hold the data of the selected User in the User grid.
    ///     The data is encapsulated in a User object, which is defined in the ProfSvc_Classes namespace.
    /// </summary>
    private User UserRecord { get; set; } = new();

    /// <summary>
    ///     Gets or sets the clone of a User record. This property is used to hold a copy of a User record for
    ///     operations like editing or adding a User.
    ///     When adding a new User, a new instance of User is created and assigned to this property.
    ///     When editing an existing User, a copy of the User record to be edited is created and assigned to this property.
    /// </summary>
    private User UserRecordClone { get; set; } = new();

    private bool VisibleSpinner { get; set; }

    private async Task DataBound(object arg)
    {
        if (Grid.TotalItemCount > 0)
        {
            await Grid.SelectRowAsync(0);
        }
    }

    /// <summary>
    ///     Asynchronously edits the User with the given ID. If the ID is 0, a new User is created.
    /// </summary>
    /// <param name="id">The ID of the User to edit. If this parameter is 0, a new User is created.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    /// <remarks>
    ///     This method performs the following steps:
    ///     - Retrieves the selected records from the grid.
    ///     - If the first selected record's ID does not match the given ID, it selects the row with the given ID in the grid.
    ///     - If the ID is 0, it sets the title to "Add" and initializes a new User record clone if it does not exist,
    ///     or clears its data if it does.
    ///     - If the ID is not 0, it sets the title to "Edit" and copies the current User record to the clone.
    ///     - Sets the entity of the User record clone to "User".
    ///     - Triggers a state change.
    ///     - Shows the admin dialog.
    /// </remarks>
    private Task EditUserAsync(string id = "") => ExecuteMethod(async () =>
                                                                {
                                                                    VisibleSpinner = true;
                                                                    if (id.NotNullOrWhiteSpace())
                                                                    {
                                                                        List<User> _selectedList = await Grid.GetSelectedRecordsAsync();
                                                                        if (_selectedList.Count == 0 || _selectedList.First().UserName != id)
                                                                        {
                                                                            int _index = await Grid.GetRowIndexByPrimaryKeyAsync(id);
                                                                            await Grid.SelectRowAsync(_index);
                                                                        }
                                                                    }

                                                                    if (id.NullOrWhiteSpace())
                                                                    {
                                                                        Title = "Add";
                                                                        if (UserRecordClone == null)
                                                                        {
                                                                            UserRecordClone = new();
                                                                        }
                                                                        else
                                                                        {
                                                                            UserRecordClone.Clear();
                                                                        }

                                                                        UserRecordClone.IsAdd = true;
                                                                    }
                                                                    else
                                                                    {
                                                                        Title = "Edit";
                                                                        UserRecordClone = UserRecord.Copy();
                                                                        UserRecordClone.IsAdd = false;
                                                                    }

                                                                    VisibleSpinner = false;
                                                                    await UserDialog.ShowDialog();
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
    ///     Handles the filtering of the grid based on the provided User.
    ///     This method is triggered when a User is selected in the grid.
    ///     It sets the filter value to the selected User and refreshes the grid to update the displayed data.
    ///     The method ensures that the grid is not refreshed multiple times simultaneously by using a toggling flag.
    /// </summary>
    /// <param name="user">The selected User in the grid, encapsulated in a ChangeEventArgs object.</param>
    /// <returns>A Task representing the asynchronous operation of refreshing the grid.</returns>
    private Task FilterGrid(ChangeEventArgs<string, KeyValues> user) => ExecuteMethod(async () =>
                                                                                      {
                                                                                          await FilterSet(user.Value.NullOrWhiteSpace() ? "" : user.Value);
                                                                                          await SetDataSource();
                                                                                      });

    /// <summary>
    ///     Sets the filter value for the User component.
    ///     This method is used to update the static Filter property with the passed value.
    ///     The passed value is processed by the General.FilterSet method before being assigned to the Filter property.
    /// </summary>
    /// <param name="value">The value to be set as the filter.</param>
    private async Task FilterSet(string value)
    {
        UserAuto = value;
        await LocalStorage.SetItemAsStringAsync("autoUser", value);
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            string _result = await LocalStorage.GetItemAsStringAsync("autoUser");
            UserAuto = _result.NotNullOrWhiteSpace() && _result != "null" ? _result : "";
            await SetDataSource();
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
                                    
                                    if (!AdminScreens)
                                    {
                                        NavManager.NavigateTo($"{NavManager.BaseUri}dash", true);
                                    }

                                    // Using injected RedisService singleton instead of creating new instances to avoid connection leaks
                                    RedisValue _value = await RedisService.GetAsync(nameof(CacheObjects.Roles));
                                    string _roleString = _value.ToString();
                                    try
                                    {
                                        if (_roleString.NotNullOrWhiteSpace() && _roleString != "[]")
                                        {
                                            // Convert from General.DeserializeObject to JsonContext source generation for optimal performance
                                            List<Role> _roles = JsonSerializer.Deserialize(_roleString, JsonContext.CaseInsensitive.ListRole) ?? [];
                                            foreach (Role _role in _roles)
                                            {
                                                Roles.Add(new() {KeyValue = _role.ID, Text = $"[{_role.RoleName}] - {_role.Description}"});
                                            }
                                        }
                                    }
                                    catch //(Exception ex)
                                    {
                                        Roles = [];
                                    }
                                }
                            });

        await base.OnInitializedAsync();
    }

    /// <summary>
    ///     Refreshes the grid component of the User page.
    ///     This method is used to update the grid component and reflect any changes made to the data.
    /// </summary>
    /// <returns>A Task that represents the asynchronous operation.</returns>
    private async Task RefreshGrid() => await SetDataSource();

    /// <summary>
    ///     Handles the event of a row being selected in the User grid.
    /// </summary>
    /// <param name="user">The selected row data encapsulated in a RowSelectEventArgs object.</param>
    private void RowSelected(RowSelectingEventArgs<User> user) => UserRecord = user.Data;

    /// <summary>
    ///     Saves the changes made to the User record.
    /// </summary>
    /// <param name="context">The context for the form being edited.</param>
    /// <returns>A Task that represents the asynchronous operation.</returns>
    /// <remarks>
    ///     This method calls the General.SaveUserAsync method, passing in the necessary parameters to save the changes
    ///     made to the UserRecordClone.
    ///     After the save operation, it refreshes the grid and selects the updated row.
    /// </remarks>
    private Task SaveUser(EditContext context) => ExecuteMethod(async () =>
                                                                {
                                                                    Dictionary<string, string> _parameters = new()
                                                                                                             {
                                                                                                                 {"cacheName", nameof(CacheObjects.Users)}
                                                                                                             };
                                                                    string _response = await General.ExecuteRest<string>("Admin/SaveUser", _parameters,
                                                                                                                         UserRecordClone);
                                                                    if (UserRecordClone != null)
                                                                    {
                                                                        UserRecord = UserRecordClone.Copy();
                                                                    }

                                                                    if (_response.NotNullOrWhiteSpace() && _response != "[]")
                                                                    {
                                                                        await FilterSet("");
                                                                        // Convert from General.DeserializeObject to JsonContext source generation for optimal performance
                                                                        DataSource = JsonSerializer.Deserialize(_response, JsonContext.CaseInsensitive.ListUser) ?? [];
                                                                    }
                                                                });

    private async Task SetDataSource()
    {
        Dictionary<string, string> _parameters = new()
                                                 {
                                                     {"methodName", "Admin_GetUsers"},
                                                     {"filter", UserAuto ?? ""}
                                                 };
        string _returnValue = await General.ExecuteRest<string>("Admin/GetAdminList", _parameters, null, false);
        // Convert from General.DeserializeObject to JsonContext source generation for optimal performance
        DataSource = JsonSerializer.Deserialize(_returnValue, JsonContext.CaseInsensitive.ListUser) ?? [];

        await Grid.Refresh();
    }

    /// <summary>
    ///     Toggles the status of an User item and shows a confirmation dialog.
    /// </summary>
    /// <param name="id">The ID of the User item to toggle.</param>
    /// <param name="enabled">
    ///     The new status to set for the User item. If true, the status is set to 2, otherwise it is
    ///     set to 1.
    /// </param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    private Task ToggleMethod(string id, bool enabled) => ExecuteMethod(async () =>
                                                                        {
                                                                            List<User> _selectedList = await Grid.GetSelectedRecordsAsync();
                                                                            if (_selectedList.Count == 0 || _selectedList.First().UserName != id)
                                                                            {
                                                                                int _index = await Grid.GetRowIndexByPrimaryKeyAsync(id);
                                                                                await Grid.SelectRowAsync(_index);
                                                                            }

                                                                            if (await DialogService.ConfirmAsync(null, enabled ? "Disable User?" : "Enable User?",
                                                                                                                 General.DialogOptions("Are you sure you want to <strong>"
                                                                                                                                       + (enabled ? "disable" : "enable") + "</strong> " +
                                                                                                                                       "this <i>User</i>?")))
                                                                            {
                                                                                Dictionary<string, string> _parameters = new()
                                                                                                                         {
                                                                                                                             {"methodName", "Admin_ToggleUserStatus"},
                                                                                                                             {"id", id},
                                                                                                                             {"idIsString", "true"},
                                                                                                                             {"isUser", "true"}
                                                                                                                         };
                                                                                string _response = await General.ExecuteRest<string>("Admin/ToggleAdminList", _parameters);

                                                                                if (_response.NotNullOrWhiteSpace() && _response != "[]")
                                                                                {
                                                                                    await FilterSet("");
                                                                                    // Convert from General.DeserializeObject to JsonContext source generation for optimal performance
                                                                                    DataSource = JsonSerializer.Deserialize(_response, JsonContext.CaseInsensitive.ListUser) ?? [];
                                                                                }
                                                                            }
                                                                        });
}