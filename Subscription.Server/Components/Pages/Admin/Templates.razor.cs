#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Profsvc_AppTrack
// Project:             Profsvc_AppTrack.Client
// File Name:           Templates.razor.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu
// Created On:          1-31-2024 16:7
// Last Updated On:     2-1-2024 16:8
// *****************************************/

#endregion

#region Using

//using TemplateDialog = Profsvc_AppTrack.Client.Pages.Admin.Controls.TemplateDialog;

#endregion

namespace Subscription.Server.Components.Pages.Admin;

/// <summary>
///     Represents the Templates page in the Admin section of the ProfSvc_AppTrack application.
/// </summary>
/// <remarks>
///     This class is responsible for managing the Templates page, which is accessible only to administrators.
///     It includes properties for managing the user's login information, navigation, and local storage.
///     It also includes methods for initializing the page and rendering it after initialization.
/// </remarks>
public partial class Templates
{
    private static TaskCompletionSource<bool> _initializationTaskSource;
    private int _selectedID;
    private readonly SemaphoreSlim _semaphore = new(1, 1);
    private byte _toggleValue;

    /// <summary>
    ///     Gets or sets the AdminGrid component for managing Template data in the Templates page.
    /// </summary>
    /// <value>
    ///     The AdminGrid component of type <see cref="AppTemplate" />.
    /// </value>
    /// <remarks>
    ///     This property is used to manage the display, sorting, filtering, and other operations of Template data in the
    ///     Templates page.
    ///     The AdminGrid component is a generic component that can manage data of any type, in this case, Template data.
    /// </remarks>
    private AdminGrid<AppTemplate> AdminGrid
    {
        get;
        set;
    }

    /// <summary>
    ///     Gets or sets the TemplateRecord property in the Templates class.
    /// </summary>
    /// <value>
    ///     The TemplateRecord property represents a single template record in the Templates page of the Admin section.
    ///     This property is of type Template, which is defined in the ProfSvc_Classes namespace.
    /// </value>
    /// <remarks>
    ///     The TemplateRecord property is used to manage the currently selected template in the Templates page.
    ///     It is initialized with a new instance of the Template class.
    /// </remarks>
    private AppTemplate AppTemplateRecord
    {
        get;
        set;
    } = new();

    /// <summary>
    ///     Gets or sets the clone of the template record.
    /// </summary>
    /// <remarks>
    ///     This property is used to hold a clone of the template record. It is used in various operations such as editing and
    ///     saving a template.
    ///     The clone is created to avoid direct modifications to the original template record during these operations.
    /// </remarks>
    private AppTemplate AppTemplateRecordClone
    {
        get;
        set;
    } = new();

    [Inject]
    private IConfiguration Configuration
    {
        get;
        set;
    }

    /// <summary>
    ///     Gets or sets the count of templates currently displayed in the grid on the Templates page.
    /// </summary>
    /// <value>
    ///     The count of templates.
    /// </value>
    /// <remarks>
    ///     This property is used to track the number of templates in the grid. It is updated each time the data in the grid is
    ///     bound or refreshed.
    /// </remarks>
    private static int Count
    {
        get;
        set;
    }

    [Inject]
    private ICryptoService Crypto
    {
        get;
        set;
    }

    /// <summary>
    ///     Gets or sets the filter string used in the Templates page.
    /// </summary>
    /// <value>
    ///     The filter string.
    /// </value>
    /// <remarks>
    ///     This property is used to store the current filter value for the Templates page.
    ///     It is used when fetching the list of templates from the server.
    ///     The filter value is processed and formatted by the FilterSet method in the General class before being used.
    /// </remarks>
    private static string Filter
    {
        get;
        set;
    }

    /// <summary>
    ///     Gets or sets the JavaScript runtime instance for the Templates page.
    /// </summary>
    /// <remarks>
    ///     This property is used to interact with JavaScript from C# code. It is used for invoking JavaScript functions and
    ///     methods directly from C# code.
    ///     For example, it is used in the `SaveTemplate` method to scroll to a specific index in the grid, and in the
    ///     `ToggleStatusAsync` method to toggle the status of a template.
    /// </remarks>
    [Inject]
    private IJSRuntime JsRuntime
    {
        get;
        set;
    }

    /// <summary>
    ///     Gets or sets the instance of the local storage service.
    /// </summary>
    /// <value>
    ///     The instance of the local storage service.
    /// </value>
    /// <remarks>
    ///     This property is used to interact with the local storage of the browser.
    ///     It is used to store and retrieve data that persists across browser sessions.
    ///     For example, it is used to retrieve the "autoTemplate" value in the `OnAfterRenderAsync` method,
    ///     and to get the user's login information in the `OnInitializedAsync` method.
    /// </remarks>
    [Inject]
    private ILocalStorageService LocalStorage
    {
        get;
        set;
    }

    /// <summary>
    ///     Gets or sets the ILogger instance used for logging in the Workflow class.
    /// </summary>
    /// <remarks>
    ///     This property is used to log information about the execution of tasks and methods within the Workflow class.
    ///     It is injected at runtime by the dependency injection system.
    /// </remarks>
    [Inject]
    private ILogger<Workflow> Logger
    {
        get;
        set;
    }

    /// <summary>
    ///     Gets or sets the instance of the LoginCooky class.
    /// </summary>
    /// <value>
    ///     The instance of the LoginCooky class.
    /// </value>
    /// <remarks>
    ///     This property is used to store and retrieve the user's login information.
    ///     It is populated in the `OnInitializedAsync` method by calling the `RedirectInner` method of the `LoginCheck` class.
    ///     The `RoleID` property of this instance is then used to check if the user has administrator rights.
    /// </remarks>
    private LoginCooky LoginCookyUser
    {
        get;
        set;
    }

    /// <summary>
    ///     Gets or sets the instance of the NavigationManager.
    /// </summary>
    /// <value>
    ///     The instance of the NavigationManager.
    /// </value>
    /// <remarks>
    ///     This property is used to manage navigation within the application.
    ///     It is used in the `OnInitializedAsync` method to redirect the user to the home page if they do not have
    ///     administrator rights.
    /// </remarks>
    [Inject]
    private NavigationManager NavManager
    {
        get;
        set;
    }

    /// <summary>
    ///     Gets or sets the RoleID associated with the current user.
    /// </summary>
    /// <remarks>
    ///     This property is used to store the RoleID of the currently logged-in user.
    ///     It is set during the initialization of the Templates page and is used to determine
    ///     the user's access rights and permissions within the application.
    /// </remarks>
    private string RoleID
    {
        get;
        set;
    }

    /// <summary>
    ///     Gets or sets the TemplateDialog instance used for managing templates in the Templates page.
    /// </summary>
    /// <value>
    ///     The TemplateDialog instance.
    /// </value>
    /// <remarks>
    ///     This property is used to interact with the TemplateDialog, which provides functionality for handling
    ///     template-related operations such as saving and cancelling. It also provides an interface for setting and
    ///     getting properties like HeaderString, Model, OriginalTemplateName, and event callbacks like BlurSubject, Cancel,
    ///     and Save.
    /// </remarks>
    private TemplateDialog TemplateDialog
    {
        get;
        set;
    }

    /// <summary>
    ///     Gets or sets the title for the Template dialog.
    /// </summary>
    /// <remarks>
    ///     The title is determined by the context of the dialog, either "Add" when creating a new template, or "Edit" when
    ///     modifying an existing template.
    /// </remarks>
    private string Title
    {
        get;
        set;
    }

    /// <summary>
    ///     Handles the data binding for the grid in the Templates page.
    /// </summary>
    /// <remarks>
    ///     This method is called when the data in the grid is bound. It sets the count of templates currently displayed in the
    ///     grid
    ///     and selects the first row if the count is greater than zero.
    /// </remarks>
    private Task DataHandler()
    {
        return ExecuteMethod(async () => { Count = await General.SetCountAndSelect(AdminGrid.Grid); });
    }

    /// <summary>
    ///     Asynchronously edits a template.
    /// </summary>
    /// <param name="id">The ID of the template to edit. If the ID is 0, a new template is created.</param>
    /// <remarks>
    ///     This method is responsible for editing a template. If the ID is 0, it initializes a new template.
    ///     Otherwise, it finds the template with the given ID and prepares it for editing.
    ///     After the template is prepared, it triggers the UI to open the template dialog for user interaction.
    /// </remarks>
    private Task EditTemplateAsync(int id = 0)
    {
        return ExecuteMethod(async () =>
                             {
                                 List<AppTemplate> _selectedList = await AdminGrid.Grid.GetSelectedRecordsAsync();
                                 if (_selectedList.Any() && _selectedList.First().ID != id)
                                 {
                                     int _index = await AdminGrid.Grid.GetRowIndexByPrimaryKeyAsync(id);
                                     await AdminGrid.Grid.SelectRowAsync(_index);
                                 }

                                 if (id == 0)
                                 {
                                     Title = "Add";
                                     if (AppTemplateRecordClone == null)
                                     {
                                         AppTemplateRecordClone = new();
                                     }
                                     else
                                     {
                                         AppTemplateRecordClone.Clear();
                                     }
                                 }
                                 else
                                 {
                                     Title = "Edit";
                                     AppTemplateRecordClone = AppTemplateRecord.Copy();
                                 }

                                 StateHasChanged();
                                 await TemplateDialog.ShowDialog();
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
    private Task ExecuteMethod(Func<Task> task) => General.ExecuteMethod(_semaphore, task, Logger);

    /// <summary>
    ///     Asynchronously filters the grid based on the provided template.
    /// </summary>
    /// <param name="template">
    ///     The template containing key-value pairs used for filtering the grid.
    /// </param>
    /// <remarks>
    ///     This method sets the filter based on the value of the provided template, refreshes the grid to apply the filter,
    ///     and prevents multiple simultaneous filter operations using a toggling mechanism.
    /// </remarks>
    private Task FilterGrid(ChangeEventArgs<string, KeyValues> template)
    {
        return ExecuteMethod(async () =>
                             {
                                 FilterSet(template.Value);
                                 await AdminGrid.Grid.Refresh();
                                 Count = await General.SetCountAndSelect(AdminGrid.Grid);
                             });
    }

    /// <summary>
    ///     Sets the filter value for the Templates page.
    /// </summary>
    /// <param name="value">The new value to be set as the filter.</param>
    /// <remarks>
    ///     This method sets the filter value for the Templates page.
    ///     It uses the FilterSet method from the General class to process and format the passed value before setting it as the
    ///     filter.
    /// </remarks>
    private static void FilterSet(string value) => Filter = General.FilterSet(Filter, value);

    /// <summary>
    ///     Asynchronously initializes the Templates page.
    /// </summary>
    /// <remarks>
    ///     This method is called when the Templates page is first initialized.
    ///     It retrieves the user's login information from local storage and checks if the user has administrator rights.
    ///     If the user does not have administrator rights, they are redirected to the home page.
    /// </remarks>
    protected override async Task OnInitializedAsync()
    {
        _initializationTaskSource = new();
        await ExecuteMethod(async () =>
                            {
                                General.CheckStart(NavManager, Configuration);
                                LoginCookyUser = await NavManager.RedirectInner(LocalStorage, Crypto);
                                RoleID = LoginCookyUser.RoleID;
                                string _result = await LocalStorage.GetItemAsStringAsync("autoTemplate");
                                FilterSet(_result);
                                if (!LoginCookyUser.IsAdmin()) //Administrator only has the rights.
                                {
                                    NavManager.NavigateTo($"{NavManager.BaseUri}home", true);
                                }
                            });

        _initializationTaskSource.SetResult(true);
        await base.OnInitializedAsync();
    }

    /// <summary>
    ///     Asynchronously refreshes the grid component on the Templates page.
    /// </summary>
    /// <remarks>
    ///     This method is used to refresh the Syncfusion Blazor Grid component, which displays and manages the templates in
    ///     the Admin section.
    ///     It is typically called when the underlying data has changed and the UI needs to be updated to reflect these
    ///     changes.
    /// </remarks>
    /// <returns>
    ///     A Task representing the asynchronous operation.
    /// </returns>
    private Task RefreshGrid() => AdminGrid.Grid.Refresh();

    /// <summary>
    ///     Handles the row selection event in the Templates page grid.
    /// </summary>
    /// <param name="template">
    ///     The event arguments containing the data of the selected row, which is of type Template.
    /// </param>
    /// <remarks>
    ///     This method is triggered when a row in the Templates page grid is selected.
    ///     It updates the TemplateRecord property with the data of the selected row.
    /// </remarks>
    private void RowSelected(RowSelectEventArgs<AppTemplate> template) => AppTemplateRecord = template.Data;

    /// <summary>
    ///     Asynchronously saves the template record.
    /// </summary>
    /// <param name="arg">The context of the edit operation.</param>
    /// <remarks>
    ///     This method is responsible for saving the template record to the server. It sends a POST request to the
    ///     "Admin/SaveTemplate" endpoint with the cloned template record as the body.
    ///     After the request is completed, it refreshes the grid and selects the row of the saved template. If the saved
    ///     template is not the first one in the selected list, it scrolls to the saved template's row.
    /// </remarks>
    private Task SaveTemplate(EditContext arg)
    {
        return ExecuteMethod(async () =>
                             {
                                 int _response = await General.PostRest<int>("Admin/SaveTemplate", null, AppTemplateRecordClone);

                                 if (_response.NullOrWhiteSpace())
                                 {
                                     _response = AppTemplateRecordClone.ID;
                                 }

                                 AppTemplateRecord = AppTemplateRecordClone.Copy();

                                 await AdminGrid.Grid.Refresh();
                                 int _index = await AdminGrid.Grid.GetRowIndexByPrimaryKeyAsync(_response);
                                 await AdminGrid.Grid.SelectRowAsync(_index);

                                 await JsRuntime.InvokeVoidAsync("scroll", _index);
                             });
    }

    /// <summary>
    ///     Toggles the enabled status of a template.
    /// </summary>
    /// <param name="id">
    ///     The ID of the template to be toggled.
    /// </param>
    /// <param name="enabled">
    ///     The new enabled status of the template.
    /// </param>
    /// <returns>
    ///     A Task that represents the asynchronous operation.
    /// </returns>
    /// <remarks>
    ///     This method is used to toggle the enabled status of a template in the Admin section of the ProfSvc_AppTrack
    ///     application.
    ///     It sets the selected ID and the toggle value based on the provided parameters.
    ///     If the selected template is not the same as the template with the provided ID, it selects the correct template in
    ///     the grid.
    ///     Finally, it shows a confirmation dialog to the user.
    /// </remarks>
    private Task ToggleMethod(int id, bool enabled)
    {
        return ExecuteMethod(async () =>
                             {
                                 _selectedID = id;
                                 _toggleValue = enabled ? (byte)2 : (byte)1;
                                 List<AppTemplate> _selectedList = await AdminGrid.Grid.GetSelectedRecordsAsync();
                                 if (_selectedList.Any() && _selectedList.First().ID != id)
                                 {
                                     int _index = await AdminGrid.Grid.GetRowIndexByPrimaryKeyAsync(id);
                                     await AdminGrid.Grid.SelectRowAsync(_index);
                                 }

                                 await AdminGrid.DialogConfirm.ShowDialog();
                             });
    }

    /// <summary>
    ///     Toggles the status of a template asynchronously.
    /// </summary>
    /// <param name="designationID">
    ///     The ID of the template whose status is to be toggled.
    /// </param>
    /// <returns>
    ///     A Task that represents the asynchronous operation.
    /// </returns>
    /// <remarks>
    ///     This method sends a POST request to toggle the status of a template.
    ///     The method ensures that the status of a template can only be toggled once at a time by using a flag (_toggling).
    ///     The status of the template is toggled by calling the General.PostToggleAsync method with the appropriate
    ///     parameters.
    ///     After the status has been toggled, the flag (_toggling) is reset to false.
    /// </remarks>
    private Task ToggleStatusAsync(int designationID)
    {
        return ExecuteMethod(() => General.PostToggleAsync("Admin_ToggleTemplateStatus", designationID, "ADMIN", false, AdminGrid.Grid, runtime: JsRuntime));
    }

    /// <summary>
    ///     Represents the data adaptor for the Admin Templates page in the ProfSvc_AppTrack application.
    /// </summary>
    /// <remarks>
    ///     This class is responsible for managing the data retrieval for the Templates page, which is accessible only to
    ///     administrators.
    ///     It includes a method for reading data asynchronously from the data manager.
    /// </remarks>
    public class AdminTemplateAdaptor : DataAdaptor
    {
        private readonly SemaphoreSlim _semaphoreSlim = new(1, 1);

        /// <summary>
        ///     Asynchronously reads data from the data manager using the provided DataManagerRequest and filter.
        /// </summary>
        /// <param name="dm">The DataManagerRequest object that contains the parameters for the server request.</param>
        /// <param name="key">An optional key to be used in the data retrieval. Default is null.</param>
        /// <returns>
        ///     A task that represents the asynchronous operation. The task result contains the data retrieved from the server.
        ///     If the method is already reading data, the task result is null.
        /// </returns>
        /// <remarks>
        ///     This method is used to fetch data for the Templates page in the Admin section of the application.
        ///     It uses the General.GetTemplateReadAdaptor method to fetch the data from the server.
        ///     The method ensures that only one read operation is performed at a time by checking and setting the _reading flag.
        /// </remarks>
        public override async Task<object> ReadAsync(DataManagerRequest dm, string key = null)
        {
            if (!await _semaphoreSlim.WaitAsync(TimeSpan.Zero))
            {
                return null;
            }

            await _initializationTaskSource.Task;
            try
            {
                object _returnValue = await General.GetTemplateReadAdaptor(dm, Filter);
                return _returnValue;
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