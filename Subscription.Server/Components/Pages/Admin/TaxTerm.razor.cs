#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.Server
// File Name:           TaxTerm.razor.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu, Brijesh Dubey
// Created On:          03-21-2025 19:03
// Last Updated On:     05-01-2025 21:50
// *****************************************/

#endregion

#region Using

using JsonSerializer = System.Text.Json.JsonSerializer;

#endregion

namespace Subscription.Server.Components.Pages.Admin;

public partial class TaxTerm : ComponentBase
{
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    private bool AdminScreens { get; set; }

    private List<AdminList> DataSource { get; set; } = [];

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

    private SfGrid<AdminList> Grid { get; set; }

    /// <summary>
    ///     Gets or sets the instance of the ILocalStorageService. This service is used for managing the local storage of the
    ///     browser.
    ///     It is used in this class to retrieve and store TaxTerm-specific data, such as the "autoTaxTerm" item and the
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
    ///     It is used in this class to retrieve and store TaxTerm-specific data, such as the "autoTaxTerm" item and the
    ///     `LoginCookyUser` object.
    /// </summary>
    [Inject]
    private ISessionStorageService SessionStorage { get; set; }

    private string TaxTermAuto { get; set; }

    /// <summary>
    ///     Gets or sets the 'AdminListDialog' instance used for managing TaxTerm information in the administrative context.
    ///     This dialog is used for both creating new TaxTerm and editing existing TaxTerm.
    /// </summary>
    private TaxTermDialog TaxTermDialog { get; set; }

    /// <summary>
    ///     Gets or sets the TaxTermRecord property of the TaxTerm class.
    ///     The TaxTermRecord property represents a single TaxTerm in the application.
    ///     It is used to hold the data of the selected TaxTerm in the TaxTerm grid.
    ///     The data is encapsulated in a AdminList object, which is defined in the ProfSvc_Classes namespace.
    /// </summary>
    private AdminList TaxTermRecord { get; set; } = new();

    /// <summary>
    ///     Gets or sets the clone of a TaxTerm record. This property is used to hold a copy of a TaxTerm record for
    ///     operations like editing or adding a TaxTerm.
    ///     When adding a new TaxTerm, a new instance of TaxTerm is created and assigned to this property.
    ///     When editing an existing TaxTerm, a copy of the TaxTerm record to be edited is created and assigned to this property.
    /// </summary>
    private AdminList TaxTermRecordClone { get; set; } = new();

    /// <summary>
    ///     Gets or sets the TaxTerm of the TaxTerm Dialog in the administrative context.
    ///     The TaxTerm changes based on the action being performed on the TaxTerm record - "Add" when a new TaxTerm is being added,
    ///     and "Edit" when an existing TaxTerm's details are being modified.
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
    ///     Asynchronously edits the TaxTerm with the given ID. If the ID is 0, a new TaxTerm is created.
    /// </summary>
    /// <param name="id">The ID of the TaxTerm to edit. If this parameter is 0, a new TaxTerm is created.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    /// <remarks>
    ///     This method performs the following steps:
    ///     - Retrieves the selected records from the grid.
    ///     - If the first selected record's ID does not match the given ID, it selects the row with the given ID in the grid.
    ///     - If the ID is 0, it sets the title to "Add" and initializes a new TaxTerm record clone if it does not exist,
    ///     or clears its data if it does.
    ///     - If the ID is not 0, it sets the title to "Edit" and copies the current TaxTerm record to the clone.
    ///     - Sets the entity of the TaxTerm record clone to "TaxTerm".
    ///     - Triggers a state change.
    ///     - Shows the admin dialog.
    /// </remarks>
    private Task EditTaxTermAsync(string id = "") => ExecuteMethod(async () =>
                                                                   {
                                                                       VisibleSpinner = true;
                                                                       if (id.NotNullOrWhiteSpace())
                                                                       {
                                                                           List<AdminList> _selectedList = await Grid.GetSelectedRecordsAsync();
                                                                           if (_selectedList.Count == 0 || _selectedList.First().Code != id)
                                                                           {
                                                                               int _index = await Grid.GetRowIndexByPrimaryKeyAsync(id);
                                                                               await Grid.SelectRowAsync(_index);
                                                                           }
                                                                       }

                                                                       if (id.NullOrWhiteSpace())
                                                                       {
                                                                           Title = "Add";
                                                                           if (TaxTermRecordClone == null)
                                                                           {
                                                                               TaxTermRecordClone = new();
                                                                           }
                                                                           else
                                                                           {
                                                                               TaxTermRecordClone.Clear();
                                                                           }

                                                                           TaxTermRecordClone.IsAdd = true;
                                                                       }
                                                                       else
                                                                       {
                                                                           Title = "Edit";
                                                                           TaxTermRecordClone = TaxTermRecord.Copy();
                                                                           TaxTermRecordClone.IsAdd = false;
                                                                       }

                                                                       VisibleSpinner = false;
                                                                       TaxTermRecordClone.Entity = "TaxTerm";
                                                                       await TaxTermDialog.ShowDialog();
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
    ///     Handles the filtering of the grid based on the provided TaxTerm.
    ///     This method is triggered when a TaxTerm is selected in the grid.
    ///     It sets the filter value to the selected TaxTerm and refreshes the grid to update the displayed data.
    ///     The method ensures that the grid is not refreshed multiple times simultaneously by using a toggling flag.
    /// </summary>
    /// <param name="taxTerm">The selected TaxTerm in the grid, encapsulated in a ChangeEventArgs object.</param>
    /// <returns>A Task representing the asynchronous operation of refreshing the grid.</returns>
    private Task FilterGrid(ChangeEventArgs<string, KeyValues> taxTerm)
    {
        return ExecuteMethod(async () =>
                             {
                                 await FilterSet(taxTerm.Value.NullOrWhiteSpace() ? "" : taxTerm.Value);
                                 await SetDataSource();
                             });
    }

    /// <summary>
    ///     Sets the filter value for the TaxTerm component.
    ///     This method is used to update the static Filter property with the passed value.
    ///     The passed value is processed by the General.FilterSet method before being assigned to the Filter property.
    /// </summary>
    /// <param name="value">The value to be set as the filter.</param>
    private async Task FilterSet(string value)
    {
        TaxTermAuto = value;
        await LocalStorage.SetItemAsStringAsync("autoTaxTerm", value);
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            string _result = await LocalStorage.GetItemAsStringAsync("autoTaxTerm");

            TaxTermAuto = _result.NotNullOrWhiteSpace() && _result != "null" ? _result : "";

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
    ///     Refreshes the grid component of the TaxTerm page.
    ///     This method is used to update the grid component and reflect any changes made to the data.
    /// </summary>
    /// <returns>A Task that represents the asynchronous operation.</returns>
    private async Task RefreshGrid() => await SetDataSource();

    /// <summary>
    ///     Handles the event of a row being selected in the TaxTerm grid.
    /// </summary>
    /// <param name="taxTerm">The selected row data encapsulated in a RowSelectEventArgs object.</param>
    private void RowSelected(RowSelectingEventArgs<AdminList> taxTerm) => TaxTermRecord = taxTerm.Data;

    /// <summary>
    ///     Saves the changes made to the TaxTerm record.
    /// </summary>
    /// <param name="context">The context for the form being edited.</param>
    /// <returns>A Task that represents the asynchronous operation.</returns>
    /// <remarks>
    ///     This method calls the General.SaveAdminListAsync method, passing in the necessary parameters to save the changes
    ///     made to the TaxTermRecordClone.
    ///     After the save operation, it refreshes the grid and selects the updated row.
    /// </remarks>
    private Task SaveTaxTerm(EditContext context) => ExecuteMethod(async () =>
                                                                   {
                                                                       Dictionary<string, string> _parameters = new()
                                                                                                                {
                                                                                                                    {"methodName", "Admin_SaveTaxTerm"},
                                                                                                                    {"parameterName", "TaxTerm"},
                                                                                                                    {"containDescription", "true"},
                                                                                                                    {"isString", "true"},
                                                                                                                    {"cacheName", nameof(CacheObjects.TaxTerms)}
                                                                                                                };
                                                                       string _response = await General.ExecuteRest<string>("Admin/SaveAdminList", _parameters,
                                                                                                                            TaxTermRecordClone);
                                                                       if (TaxTermRecordClone != null)
                                                                       {
                                                                           TaxTermRecord = TaxTermRecordClone.Copy();
                                                                       }

                                                                       if (_response.NotNullOrWhiteSpace() && _response != "[]")
                                                                       {
                                                                           await FilterSet("");
                                                                           // Convert from General.DeserializeObject to JsonContext source generation for optimal performance
                                                                           DataSource = JsonSerializer.Deserialize(_response, JsonContext.CaseInsensitive.ListAdminList) ?? [];
                                                                       }

                                                                       /*int _index = await Grid.GetRowIndexByPrimaryKeyAsync(_response.ToInt32());
                                                                       await Grid.SelectRowAsync(_index);*/
                                                                   });

    private async Task SetDataSource()
    {
        Dictionary<string, string> _parameters = new()
                                                 {
                                                     {"methodName", "Admin_GetTaxTerms"},
                                                     {"filter", TaxTermAuto ?? ""}
                                                 };
        string _returnValue = await General.ExecuteRest<string>("Admin/GetAdminList", _parameters, null, false);
        // Convert from General.DeserializeObject to JsonContext source generation for optimal performance
        DataSource = JsonSerializer.Deserialize(_returnValue, JsonContext.CaseInsensitive.ListAdminList) ?? [];
        
        await Grid.Refresh();
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
    private Task ToggleMethod(string id, bool enabled) => ExecuteMethod(async () =>
                                                                        {
                                                                            List<AdminList> _selectedList = await Grid.GetSelectedRecordsAsync();
                                                                            if (_selectedList.Count == 0 || _selectedList.First().Code != id)
                                                                            {
                                                                                int _index = await Grid.GetRowIndexByPrimaryKeyAsync(id);
                                                                                await Grid.SelectRowAsync(_index);
                                                                            }

                                                                            if (await DialogService.ConfirmAsync(null, enabled ? "Disable TaxTerm?" : "Enable TaxTerm?",
                                                                                                                 General.DialogOptions("Are you sure you want to <strong>"
                                                                                                                                       + (enabled ? "disable" : "enable") + "</strong> " +
                                                                                                                                       "this <i>TaxTerm</i>?")))
                                                                            {
                                                                                Dictionary<string, string> _parameters = new()
                                                                                                                         {
                                                                                                                             {"methodName", "Admin_ToggleTaxTermStatus"},
                                                                                                                             {"idIsString", "true"},
                                                                                                                             {"id", id}
                                                                                                                         };
                                                                                string _response = await General.ExecuteRest<string>("Admin/ToggleAdminList", _parameters);

                                                                                if (_response.NotNullOrWhiteSpace() && _response != "[]")
                                                                                {
                                                                                    await FilterSet("");
                                                                                    // Convert from General.DeserializeObject to JsonContext source generation for optimal performance
                                                                                    DataSource = JsonSerializer.Deserialize(_response, JsonContext.CaseInsensitive.ListAdminList) ?? [];
                                                                                }
                                                                            }
                                                                        });
}