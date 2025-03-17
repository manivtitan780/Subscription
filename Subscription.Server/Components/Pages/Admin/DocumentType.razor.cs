using Microsoft.AspNetCore.Components;

namespace Subscription.Server.Components.Pages.Admin;

public partial class DocumentType : ComponentBase
{
    private static TaskCompletionSource<bool> _initializationTaskSource;

    private Query _query = new();
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    /*
    /// <summary>
    ///     Gets or sets the 'DocumentTypeDialog' instance used for managing DocumentType information in the administrative context.
    ///     This dialog is used for both creating new DocumentType and editing existing DocumentType.
    /// </summary>
    private DocumentTypeDialog AdminDialog
    {
        get;
        set;
    }
    */

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

    private string DocumentTypeAuto
    {
        get;
        set;
    }

    /// <summary>
    ///     Gets or sets the DocumentTypeRecord property of the DocumentType class.
    ///     The DocumentTypeRecord property represents a single DocumentType in the application.
    ///     It is used to hold the data of the selected DocumentType in the DocumentType grid.
    ///     The data is encapsulated in a DocumentType object, which is defined in the ProfSvc_Classes namespace.
    /// </summary>
    private DocumentTypes DocumentTypeRecord
    {
        get;
        set;
    } = new();

    /// <summary>
    ///     Gets or sets the clone of a DocumentType record. This property is used to hold a copy of a DocumentType record for
    ///     operations like editing or adding a DocumentType.
    ///     When adding a new DocumentType, a new instance of DocumentType is created and assigned to this property.
    ///     When editing an existing DocumentType, a copy of the DocumentType record to be edited is created and assigned to this property.
    /// </summary>
    private DocumentTypes DocumentTypeRecordClone
    {
        get;
        set;
    } = new();

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

    /// <summary>
    ///     Gets or sets the filter value for the application DocumentType in the administrative context.
    ///     This static property is used to filter the DocumentType based on certain criteria in the administrative context.
    /// </summary>
    private static string Filter
    {
        get;
        set;
    }

    private SfGrid<DocumentTypes> Grid
    {
        get;
        set;
    }

    /// <summary>
    ///     Gets or sets the instance of the ILocalStorageService. This service is used for managing the local storage of the
    ///     browser.
    ///     It is used in this class to retrieve and store DocumentType-specific data, such as the "autoDocumentType" item and the
    ///     `LoginCookyUser` object.
    /// </summary>
    [Inject]
    private ILocalStorageService LocalStorage
    {
        get;
        set;
    }

    /// <summary>
    ///     Gets or sets the ILogger instance used for logging in the DocumentType class.
    /// </summary>
    /// <remarks>
    ///     This property is used to log information about the execution of tasks and methods within the DocumentType class.
    ///     It is injected at runtime by the dependency injection system.
    /// </remarks>
    [Inject]
    private ILogger<DocumentType> Logger
    {
        get;
        set;
    }

    /// <summary>
    ///     Gets or sets the `LoginCooky` object for the current DocumentType.
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
    ///     It is used in this class to retrieve and store DocumentType-specific data, such as the "autoDocumentType" item and the
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
    ///     Gets or sets the DocumentType of the DocumentType Dialog in the administrative context.
    ///     The DocumentType changes based on the action being performed on the DocumentType record - "Add" when a new DocumentType is being added,
    ///     and "Edit" when an existing DocumentType's details are being modified.
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

    private DocumentTypeDialog DocumentTypeDialog
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
    ///     Asynchronously edits the documentType with the given ID. If the ID is 0, a new documentType is created.
    /// </summary>
    /// <param name="id">The ID of the documentType to edit. If this parameter is 0, a new documentType is created.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    /// <remarks>
    ///     This method performs the following steps:
    ///     - Retrieves the selected records from the grid.
    ///     - If the first selected record's ID does not match the given ID, it selects the row with the given ID in the grid.
    ///     - If the ID is 0, it sets the title to "Add" and initializes a new documentType record clone if it does not exist,
    ///     or clears its data if it does.
    ///     - If the ID is not 0, it sets the title to "Edit" and copies the current documentType record to the clone.
    ///     - Sets the entity of the documentType record clone to "DocumentType".
    ///     - Triggers a state change.
    ///     - Shows the admin dialog.
    /// </remarks>
    private Task EditDocumentTypeAsync(int id = 0) => ExecuteMethod(async () =>
                                                                    {
                                                                        VisibleSpinner = true;
                                                                        if (id != 0)
                                                                        {
                                                                            List<DocumentTypes> _selectedList = await Grid.GetSelectedRecordsAsync();
                                                                            if (_selectedList.Count == 0 || _selectedList.First().KeyValue != id)
                                                                            {
                                                                                int _index = await Grid.GetRowIndexByPrimaryKeyAsync(id);
                                                                                await Grid.SelectRowAsync(_index);
                                                                            }
                                                                        }

                                                                        if (id == 0)
                                                                        {
                                                                            Title = "Add";
                                                                            if (DocumentTypeRecordClone == null)
                                                                            {
                                                                                DocumentTypeRecordClone = new();
                                                                            }
                                                                            else
                                                                            {
                                                                                DocumentTypeRecordClone.Clear();
                                                                            }
                                                                        }
                                                                        else
                                                                        {
                                                                            Title = "Edit";
                                                                            DocumentTypeRecordClone = DocumentTypeRecord.Copy();
                                                                        }

                                                                        VisibleSpinner = false;
                                                                        await DocumentTypeDialog.ShowDialog();
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
    ///     Handles the filtering of the grid based on the provided documentType.
    ///     This method is triggered when a documentType is selected in the grid.
    ///     It sets the filter value to the selected documentType and refreshes the grid to update the displayed data.
    ///     The method ensures that the grid is not refreshed multiple times simultaneously by using a toggling flag.
    /// </summary>
    /// <param name="documentType">The selected documentType in the grid, encapsulated in a ChangeEventArgs object.</param>
    /// <returns>A Task representing the asynchronous operation of refreshing the grid.</returns>
    private Task FilterGrid(ChangeEventArgs<string, KeyValues> documentType)
    {
        return ExecuteMethod(async () =>
                             {
                                 await FilterSet(documentType.Value.NullOrWhiteSpace() ? string.Empty : documentType.Value);
                                 await SetDataSource();
                                 //await Grid.Refresh(true);
                                 //Count = await General.SetCountAndSelect(AdminGrid.Grid);
                             });
    }

    /// <summary>
    ///     Sets the filter value for the DocumentType component.
    ///     This method is used to update the static Filter property with the passed value.
    ///     The passed value is processed by the General.FilterSet method before being assigned to the Filter property.
    /// </summary>
    /// <param name="value">The value to be set as the filter.</param>
    private async Task FilterSet(string value)
    {
        DocumentTypeAuto = value;
        _query ??= new();
        _query.AddParams("Filter", value);
        await LocalStorage.SetItemAsStringAsync("autoDocumentType", value);
    }

    private List<DocumentTypes> DataSource
    {
        get;
        set;
    } = [];

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            string _result = await LocalStorage.GetItemAsStringAsync("autoDocumentType");

            DocumentTypeAuto = _result.NotNullOrWhiteSpace() && _result != "null" ? _result : string.Empty;
            _query ??= new();
            _query.AddParams("Filter", DocumentTypeAuto);

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

    private async Task SetDataSource()
    {
        Dictionary<string, string> _parameters = new()
                                                 {
                                                     {"methodName", "Admin_GetDocumentTypes"},
                                                     {"filter", DocumentTypeAuto ?? string.Empty}
                                                 };
        string _returnValue = await General.ExecuteRest<string>("Admin/GetAdminList", _parameters, null, false);
        DataSource = JsonConvert.DeserializeObject<List<DocumentTypes>>(_returnValue);
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
    ///     Refreshes the grid component of the DocumentType page.
    ///     This method is used to update the grid component and reflect any changes made to the data.
    /// </summary>
    /// <returns>A Task that represents the asynchronous operation.</returns>
    private Task RefreshGrid() => Grid.Refresh(true);

    /// <summary>
    ///     Handles the event of a row being selected in the DocumentType grid.
    /// </summary>
    /// <param name="documentType">The selected row data encapsulated in a RowSelectEventArgs object.</param>
    private void RowSelected(RowSelectingEventArgs<DocumentTypes> documentType) => DocumentTypeRecord = documentType.Data;

    /// <summary>
    ///     Saves the changes made to the documentType record.
    /// </summary>
    /// <param name="context">The context for the form being edited.</param>
    /// <returns>A Task that represents the asynchronous operation.</returns>
    /// <remarks>
    ///     This method calls the General.SaveDocumentTypeAsync method, passing in the necessary parameters to save the changes
    ///     made to the DocumentTypeRecordClone.
    ///     After the save operation, it refreshes the grid and selects the updated row.
    /// </remarks>
    private Task SaveDocumentType(EditContext context) => ExecuteMethod(async () =>
                                                                       {
                                                                           Dictionary<string, string> _parameters = new()
                                                                                                                    {
                                                                                                                        {"cacheName", CacheObjects.DocumentTypes.ToString()}
                                                                                                                    };
                                                                           string _response = await General.ExecuteRest<string>("Admin/SaveDocumentType", _parameters,
                                                                                                                                DocumentTypeRecordClone);
                                                                           if (DocumentTypeRecordClone != null)
                                                                           {
                                                                               DocumentTypeRecord = DocumentTypeRecordClone.Copy();
                                                                           }

                                                                           DataSource = General.DeserializeObject<List<DocumentTypes>>(_response);
                                                                           //await Grid.Refresh(false);

                                                                           /*int _index = await Grid.GetRowIndexByPrimaryKeyAsync(_response.ToInt32());
                                                                           await Grid.SelectRowAsync(_index);*/
                                                                       });

    public class AdminDocumentTypeAdaptor : DataAdaptor
    {
        private readonly SemaphoreSlim _semaphoreSlim = new(1, 1);

        /// <summary>
        ///     Asynchronously fetches data for the Admin DocumentType page from the server.
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
                                                             {"methodName", "Admin_GetDocumentTypes"},
                                                             {"filter", dm.Params["Filter"]?.ToString() ?? string.Empty}
                                                         };
                string _returnValue = await General.ExecuteRest<string>("Admin/GetAdminList", _parameters, null, false);
                List<DocumentTypes> _documentType = JsonConvert.DeserializeObject<List<DocumentTypes>>(_returnValue);
                return dm.RequiresCounts ? new DataResult
                                           {
                                               Result = _documentType,
                                               Count = _documentType.Count
                                           }
                           : _documentType;
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