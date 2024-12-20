#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.Server
// File Name:           Requisitions.razor.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu
// Created On:          05-01-2024 15:05
// Last Updated On:     12-20-2024 20:12
// *****************************************/

#endregion

namespace Subscription.Server.Components.Pages;

public partial class Requisitions
{
    private const string StorageName = "RequisitionGrid";
    private static int _currentPage = 1;

    private static TaskCompletionSource<bool> _initializationTaskSource;
    private readonly SemaphoreSlim _semaphoreMainPage = new(1, 1);

    /// <summary>
    ///     Gets or sets the AutocompleteValue property of the Requisition.
    /// </summary>
    /// <remarks>
    ///     The AutocompleteValue is used to store the title of the SearchModel during the initialization of the Requisition
    ///     component.
    ///     It is also updated when clearing filters or selecting all alphabet options.
    /// </remarks>
    /// <value>
    ///     The title of the SearchModel.
    /// </value>
    private string AutocompleteValue
    {
        get;
        set;
    }

    /// <summary>
    ///     Gets or sets the list of companies associated with the requisition.
    /// </summary>
    /// <value>
    ///     The list of companies.
    /// </value>
    internal static List<Company> Companies
    {
        get;
        set;
    } = [];

    /// <summary>
    ///     Gets or sets a list of company contacts associated with the requisition.
    /// </summary>
    /// <value>
    ///     The list of company contacts.
    /// </value>
    internal static List<CompanyContacts> CompanyContacts
    {
        get;
        set;
    } = [];

    /// <summary>
    ///     Gets or sets the count of items.
    /// </summary>
    /// <remarks>
    ///     This property is used to store the total number of items in the data source.
    ///     It is updated whenever the data source is refreshed or a new set of items is loaded.
    /// </remarks>
    internal static int Count
    {
        get;
        set;
    }

    /// <summary>
    ///     Gets or sets the end record of the requisition. The end record is calculated as the start record plus the count of
    ///     data source items.
    ///     This property is used to determine the range of records displayed on the current page of the requisition grid.
    /// </summary>
    internal static int EndRecord
    {
        get;
        set;
    }

    private static SfGrid<Requisition> Grid
    {
        get;
        set;
    }

    internal static int PageCount
    {
        get;
        set;
    }

    /// <summary>
    ///     Gets or sets the search model for the requisition. This model is used to store the search parameters for finding
    ///     requisitions.
    /// </summary>
    private static RequisitionSearch SearchModel
    {
        get;
    } = new();
    
   /// <summary>
    ///     Gets or sets the search model for the requisition. This model is used to store the search parameters for finding
    ///     requisitions.
    /// </summary>
    private static RequisitionSearch SearchModelClone
    {
        get;
        set;
    } = new();

    /// <summary>
    ///     Gets or sets the session storage service used for storing and retrieving session data.
    ///     This service is used to manage session data such as the requisition grid state and the requisition ID from the
    ///     dashboard.
    /// </summary>
    /// <value>
    ///     The session storage service.
    /// </value>
    [Inject]
    private ISessionStorageService SessionStorage
    {
        get;
        set;
    }

    /// <summary>
    ///     Gets or sets the skills associated with the requisition.
    ///     This is a list of <see cref="IntValues" /> where each value represents a unique skill.
    /// </summary>
    internal static List<IntValues> Skills
    {
        get;
        set;
    }

    private SfSpinner SpinnerTop
    {
        get;
        set;
    }

    internal static int StartRecord
    {
        get;
        set;
    }

    /// <summary>
    ///     Gets or sets the list of status values associated with the requisition.
    ///     This list is populated with the 'StatusCount' data from the API response.
    ///     Each status is represented by a 'KeyValues' object.
    /// </summary>
    internal static List<KeyValues> StatusList
    {
        get;
        set;
    } = [];

    private static string User
    {
        get;
        set;
    }

    /// <summary>
    ///     Initiates the advanced search process for requisitions. This method is invoked when the advanced search option is
    ///     selected.
    ///     It creates a copy of the current search model for backup purposes and then opens the advanced search dialog.
    /// </summary>
    /// <param name="args">The mouse event arguments associated with the advanced search invocation.</param>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    private Task AdvancedSearch(MouseEventArgs args) => ExecuteMethod(async () =>
                                                                      {
                                                                          SearchModelClone = SearchModel.Copy();
                                                                          await Task.CompletedTask;
                                                                          //await DialogSearch.ShowDialog();
                                                                      });

    /// <summary>
    ///     Asynchronously changes the item count of the requisition.
    /// </summary>
    /// <param name="item">The change event arguments containing the new item count.</param>
    /// <remarks>
    ///     This method updates the item count of the requisition and refreshes the grid.
    ///     It also saves the updated search model to the session storage.
    ///     This method is not executed if an action is already in progress.
    /// </remarks>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    private Task ChangeItemCount(ChangeEventArgs<int, IntValues> item) => ExecuteMethod(async () =>
                                                                                        {
                                                                                            SearchModel.Page = 1;
                                                                                            SearchModel.ItemCount = item.Value;

                                                                                            await SessionStorage.SetItemAsync(StorageName, SearchModel);
                                                                                            await Grid.Refresh();
                                                                                            StateHasChanged();
                                                                                        });

    /// <summary>
    ///     Handles the click event for the "All Alphabet" button in the requisition grid.
    /// </summary>
    /// <remarks>
    ///     This method resets the search model's title and page properties, clears the autocomplete value, and refreshes the
    ///     grid.
    ///     It also ensures that the method's actions are not performed if a previous action is still in progress.
    /// </remarks>
    /// <param name="args">The <see cref="MouseEventArgs" /> instance containing the event data.</param>
    private Task AllAlphabets(MouseEventArgs args) => ExecuteMethod(async () =>
                                                                    {
                                                                        SearchModel.Title = "";
                                                                        _currentPage = 1;
                                                                        SearchModel.Page = 1;
                                                                        await SessionStorage.SetItemAsync(StorageName, SearchModel);
                                                                        AutocompleteValue = "";
                                                                        await Grid.Refresh();
                                                                    });

    private static async Task AutocompleteValueChange(ChangeEventArgs<string, KeyValues> filter)
    {
        SearchModel.Title = filter.Value;
        SearchModel.Page = 1;
        await Grid.Refresh();
    }

    /// <summary>
    ///     Clears the filter applied to the requisitions.
    /// </summary>
    /// <remarks>
    ///     This function is called when the "Clear Filter" button is clicked.
    ///     It resets the filter values and reloads the requisitions.
    /// </remarks>
    private async Task ClearFilter()
    {
        SearchModel.Clear();
        SearchModel.User = User;
        await Grid.Refresh();
    }

    /// <summary>
    ///     Gets or sets the JavaScript runtime instance. This instance is used to invoke JavaScript functions from C# code.
    ///     For example, it is used in the DownloadDocument method to open a new browser tab for document download.
    /// </summary>
    [Inject]
    private IJSRuntime JsRuntime
    {
        get;
        set;
    }

    /// <summary>
    ///     Gets or sets the ID of the requisition. This ID is used to uniquely identify a requisition in the system.
    /// </summary>
    private static int RequisitionID
    {
        get;
        set;
    }

    /// <summary>
    ///     Handles the data processing for the requisition. This method is responsible for creating a reference to the current
    ///     instance of the Requisition class,
    ///     invoking a JavaScript function to manage detail rows, and managing the selection and expansion of rows in the
    ///     requisition grid based on the RequisitionID.
    ///     If the total item count in the grid is greater than zero, it checks if the RequisitionID is greater than zero. If
    ///     so, it selects and expands the corresponding row.
    ///     If the RequisitionID is not greater than zero, it selects the first row. After the operations, it resets the
    ///     RequisitionID to zero.
    /// </summary>
    /// <param name="obj">The object that triggers the data handling.</param>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    private Task DataHandler(object obj) => ExecuteMethod(async () =>
                                                          {
                                                              DotNetObjectReference<Requisitions> _dotNetReference = DotNetObjectReference.Create(this); // create dotnet ref
                                                              await JsRuntime.InvokeAsync<string>("detail", _dotNetReference);
                                                              //  send the dotnet ref to JS side
                                                              if (Grid.TotalItemCount > 0)
                                                              {
                                                                  if (RequisitionID > 0)
                                                                  {
                                                                      int _index = await Grid.GetRowIndexByPrimaryKeyAsync(RequisitionID);
                                                                      if (_index != Grid.SelectedRowIndex)
                                                                      {
                                                                          await Grid.SelectRowAsync(_index);
                                                                          foreach (Requisition _requisition in Grid.CurrentViewData.OfType<Requisition>()
                                                                                                                    .Where(requisitions => requisitions.ID == RequisitionID))
                                                                          {
                                                                              await Grid.ExpandCollapseDetailRowAsync(_requisition);
                                                                              break;
                                                                          }
                                                                      }

                                                                      await SessionStorage.SetItemAsync(StorageName, SearchModel);
                                                                      await SessionStorage.RemoveItemAsync("RequisitionIDFromDashboard");
                                                                  }
                                                                  else
                                                                  {
                                                                      await Grid.SelectRowAsync(0);
                                                                  }
                                                              }

                                                              RequisitionID = 0;
                                                          });

    /// <summary>
    ///     Executes the provided task within a semaphore lock. If the semaphore is currently locked, the method will return
    ///     immediately.
    ///     If an exception occurs during the execution of the task, it will be logged using the provided logger.
    /// </summary>
    /// <param name="task">
    ///     The task to be executed.
    /// </param>
    /// <returns>
    ///     A task that represents the asynchronous operation.
    /// </returns>
    private Task ExecuteMethod(Func<Task> task) => General.ExecuteMethod(_semaphoreMainPage, task);

    private async Task GetAlphabets(char alphabet)
    {
        await ExecuteMethod(async () =>
                            {
                                SearchModel.Title = alphabet.ToString();
                                SearchModel.Page = 1;
                                await Grid.Refresh();
                            });
    }

    /// <summary>
    ///     The RequisitionAdaptor class is a custom data adaptor for the Requisitions page.
    ///     It inherits from the DataAdaptor class and overrides the ReadAsync method.
    /// </summary>
    /// <remarks>
    ///     The ReadAsync method is used to asynchronously read data for the Requisitions page.
    ///     It checks if a read operation is already in progress, and if not, it initiates a new read operation.
    ///     The method retrieves lead data using the General.GetRequisitionReadAdaptor method and the provided
    ///     DataManagerRequest.
    ///     If there are any requisitions, it selects the first one. The method returns the retrieved requisitions data.
    /// </remarks>
    internal class RequisitionAdaptor : DataAdaptor
    {
        private readonly SemaphoreSlim _semaphoreSlim = new(1, 1);

        /// <summary>
        ///     Asynchronously reads data from the Requisition data source.
        /// </summary>
        /// <param name="dm">The DataManagerRequest object that contains the parameters for the data request.</param>
        /// <param name="key">An optional key to identify a specific data record.</param>
        /// <remarks>
        ///     This method checks if the data is already being read or if it has not been loaded yet. If either of these
        ///     conditions is true, it returns null.
        ///     Otherwise, it sets the _reading flag to true and proceeds with the data read operation. It also checks if the
        ///     Companies list is not null and has more than 0 items.
        ///     If so, it sets the _getInformation flag to true. Then it calls the GetRequisitionReadAdaptor method from the
        ///     General class, passing the SearchModel, dm, _getInformation, RequisitionID, and true as parameters.
        ///     The result of this method call is stored in the _requisitionReturn object. After the data read operation, it sets
        ///     the _currentPage to the Page property of the SearchModel and the _reading flag back to false.
        /// </remarks>
        /// <returns>
        ///     A Task that represents the asynchronous operation. The task result contains the data retrieved from the data
        ///     source.
        /// </returns>
        public override async Task<object> ReadAsync(DataManagerRequest dm, string key = null)
        {
            if (!await _semaphoreSlim.WaitAsync(TimeSpan.Zero))
            {
                return null;
            }

            await _initializationTaskSource.Task;
            try
            {
                bool _getInformation = true;
                if (Companies != null)
                {
                    _getInformation = Companies.Count == 0;
                }

                object _requisitionReturn = await General.GetRequisitionReadAdaptor(SearchModel, dm, _getInformation, RequisitionID, true, User);

                _currentPage = SearchModel.Page;

                return _requisitionReturn;
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