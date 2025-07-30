#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.Server
// File Name:           States.razor.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu, Brijesh Dubey
// Created On:          03-21-2025 14:03
// Last Updated On:     05-01-2025 21:40
// *****************************************/

#endregion

using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Subscription.Server.Components.Pages.Admin;

public partial class States : ComponentBase
{
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    private bool AdminScreens { get; set; }

    private List<State> DataSource { get; set; } = [];

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

    private SfGrid<State> Grid { get; set; }

    /// <summary>
    ///     Gets or sets the instance of the ILocalStorageService. This service is used for managing the local storage of the
    ///     browser.
    ///     It is used in this class to retrieve and store State-specific data, such as the "autoState" item and the
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
    ///     Gets or sets the RoleID for the current user. The RoleID is used to determine the user's permissions within the
    ///     application.
    /// </summary>
    private string RoleID { get; set; }

    private string RoleName { get; set; }

    /// <summary>
    ///     Gets or sets the instance of the ILocalStorageService. This service is used for managing the local storage of the
    ///     browser.
    ///     It is used in this class to retrieve and store State-specific data, such as the "autoState" item and the
    ///     `LoginCookyUser` object.
    /// </summary>
    [Inject]
    private ISessionStorageService SessionStorage { get; set; }

    private string StateAuto { get; set; }

    /// <summary>
    ///     Gets or sets the 'StateDialog' instance used for managing State information in the administrative context.
    ///     This dialog is used for both creating new State and editing existing State.
    /// </summary>
    private StateDialog StateDialog { get; set; }

    /// <summary>
    ///     Gets or sets the StateRecord property of the State class.
    ///     The StateRecord property represents a single State in the application.
    ///     It is used to hold the data of the selected State in the State grid.
    ///     The data is encapsulated in a State object, which is defined in the ProfSvc_Classes namespace.
    /// </summary>
    private State StateRecord { get; set; } = new();

    /// <summary>
    ///     Gets or sets the clone of a State record. This property is used to hold a copy of a State record for
    ///     operations like editing or adding a State.
    ///     When adding a new State, a new instance of State is created and assigned to this property.
    ///     When editing an existing State, a copy of the State record to be edited is created and assigned to this property.
    /// </summary>
    private State StateRecordClone { get; set; } = new();

    /// <summary>
    ///     Gets or sets the State of the State Dialog in the administrative context.
    ///     The State changes based on the action being performed on the State record - "Add" when a new State is being added,
    ///     and "Edit" when an existing State's details are being modified.
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
    ///     Asynchronously edits the State with the given ID. If the ID is 0, a new State is created.
    /// </summary>
    /// <param name="id">The ID of the State to edit. If this parameter is 0, a new State is created.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    /// <remarks>
    ///     This method performs the following steps:
    ///     - Retrieves the selected records from the grid.
    ///     - If the first selected record's ID does not match the given ID, it selects the row with the given ID in the grid.
    ///     - If the ID is 0, it sets the title to "Add" and initializes a new State record clone if it does not exist,
    ///     or clears its data if it does.
    ///     - If the ID is not 0, it sets the title to "Edit" and copies the current State record to the clone.
    ///     - Sets the entity of the State record clone to "State".
    ///     - Triggers a state change.
    ///     - Shows the admin dialog.
    /// </remarks>
    private Task EditStateAsync(int id = 0) => ExecuteMethod(async () =>
                                                             {
                                                                 VisibleSpinner = true;
                                                                 if (id != 0)
                                                                 {
                                                                     List<State> _selectedList = await Grid.GetSelectedRecordsAsync();
                                                                     if (_selectedList.Count == 0 || _selectedList.First().ID != id)
                                                                     {
                                                                         int _index = await Grid.GetRowIndexByPrimaryKeyAsync(id);
                                                                         await Grid.SelectRowAsync(_index);
                                                                     }
                                                                 }

                                                                 if (id == 0)
                                                                 {
                                                                     Title = "Add";
                                                                     if (StateRecordClone == null)
                                                                     {
                                                                         StateRecordClone = new();
                                                                     }
                                                                     else
                                                                     {
                                                                         StateRecordClone.Clear();
                                                                     }
                                                                 }
                                                                 else
                                                                 {
                                                                     Title = "Edit";
                                                                     StateRecordClone = StateRecord.Copy();
                                                                 }

                                                                 VisibleSpinner = false;
                                                                 //StateRecordClone.Entity = "State";
                                                                 await StateDialog.ShowDialog();
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
    ///     Handles the filtering of the grid based on the provided State.
    ///     This method is triggered when a State is selected in the grid.
    ///     It sets the filter value to the selected State and refreshes the grid to update the displayed data.
    ///     The method ensures that the grid is not refreshed multiple times simultaneously by using a toggling flag.
    /// </summary>
    /// <param name="state">The selected State in the grid, encapsulated in a ChangeEventArgs object.</param>
    /// <returns>A Task representing the asynchronous operation of refreshing the grid.</returns>
    private Task FilterGrid(ChangeEventArgs<string, KeyValues> state)
    {
        return ExecuteMethod(async () =>
                             {
                                 await FilterSet(state.Value.NullOrWhiteSpace() ? "" : state.Value);
                                 await SetDataSource();
                             });
    }

    /// <summary>
    ///     Sets the filter value for the State component.
    ///     This method is used to update the static Filter property with the passed value.
    ///     The passed value is processed by the General.FilterSet method before being assigned to the Filter property.
    /// </summary>
    /// <param name="value">The value to be set as the filter.</param>
    private async Task FilterSet(string value)
    {
        StateAuto = value;
        await LocalStorage.SetItemAsStringAsync("autoState", value);
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            string _result = await LocalStorage.GetItemAsStringAsync("autoState");

            StateAuto = _result.NotNullOrWhiteSpace() && _result != "null" ? _result : "";

            try
            {
                await SetDataSource();
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

        await base.OnInitializedAsync();
    }

    /// <summary>
    ///     Refreshes the grid component of the State page.
    ///     This method is used to update the grid component and reflect any changes made to the data.
    /// </summary>
    /// <returns>A Task that represents the asynchronous operation.</returns>
    private async Task RefreshGrid() => await SetDataSource();

    /// <summary>
    ///     Handles the event of a row being selected in the State grid.
    /// </summary>
    /// <param name="state">The selected row data encapsulated in a RowSelectEventArgs object.</param>
    private void RowSelected(RowSelectingEventArgs<State> state) => StateRecord = state.Data;

    /// <summary>
    ///     Saves the changes made to the State record.
    /// </summary>
    /// <param name="context">The context for the form being edited.</param>
    /// <returns>A Task that represents the asynchronous operation.</returns>
    /// <remarks>
    ///     This method calls the General.SaveStateAsync method, passing in the necessary parameters to save the changes
    ///     made to the StateRecordClone.
    ///     After the save operation, it refreshes the grid and selects the updated row.
    /// </remarks>
    private Task SaveState(EditContext context) => ExecuteMethod(async () =>
                                                                 {
                                                                     Dictionary<string, string> _parameters = new()
                                                                                                              {
                                                                                                                  {"cacheName", nameof(CacheObjects.States)}
                                                                                                              };
                                                                     string _response = await General.ExecuteRest<string>("Admin/SaveState", _parameters,
                                                                                                                          StateRecordClone);
                                                                     if (StateRecordClone != null)
                                                                     {
                                                                         StateRecord = StateRecordClone.Copy();
                                                                     }

                                                                     //await Grid.Refresh(true);
                                                                     if (_response.NotNullOrWhiteSpace() && _response != "[]")
                                                                     {
                                                                         await FilterSet("");
                                                                         // Convert from General.DeserializeObject to JsonContext source generation for optimal performance
                                                                         DataSource = JsonSerializer.Deserialize(_response, JsonContext.CaseInsensitive.ListState) ?? [];
                                                                     }

                                                                     /*int _index = await Grid.GetRowIndexByPrimaryKeyAsync(_response.ToInt32());
                                                                     await Grid.SelectRowAsync(_index);*/
                                                                 });

    private async Task SetDataSource()
    {
        Dictionary<string, string> _parameters = new()
                                                 {
                                                     {"methodName", "Admin_GetStates"},
                                                     {"filter", StateAuto ?? ""}
                                                 };
        string _returnValue = await General.ExecuteRest<string>("Admin/GetAdminList", _parameters, null, false);
        // Convert from General.DeserializeObject to JsonContext source generation for optimal performance
        DataSource = JsonSerializer.Deserialize(_returnValue, JsonContext.CaseInsensitive.ListState) ?? [];
        
        await Grid.Refresh();
    }
}