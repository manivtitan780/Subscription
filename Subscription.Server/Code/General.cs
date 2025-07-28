#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.Server
// File Name:           General.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu, Brijesh Dubey
// Created On:          07-22-2025 21:07
// Last Updated On:     07-28-2025 19:32
// *****************************************/

#endregion

#region Using

using JsonSerializer = System.Text.Json.JsonSerializer;

#endregion

namespace Subscription.Server.Code;

public class General //(Container container)
{
    // private Container _container = container;

    // Dynamic RestClient creation using current Start.APIHost (set by middleware based on request context)
    // This allows proper localhost vs server detection per request

    // Standardized JsonSerializerOptions for consistent case-insensitive JSON handling across the application
    internal static readonly JsonSerializerOptions JsonOptions = new()
                                                                 {
                                                                     PropertyNameCaseInsensitive = true,
                                                                     PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                                                                     ReadCommentHandling = JsonCommentHandling.Skip,
                                                                     AllowTrailingCommas = true
                                                                 };

    // Updated to use System.Text.Json with case-insensitive options for better performance
    /// <summary>
    ///     Deserializes a JSON string to an object of a specified type using System.Text.Json.
    /// </summary>
    /// <typeparam name="T">The type of object to deserialize to.</typeparam>
    /// <param name="array">The JSON string representing the object to be deserialized.</param>
    /// <param name="checkForNullOrEmptyArray">Should check if array converted to string is null/empty/whitespace or an empty json array.</param>
    /// <returns>The deserialized object of type T.</returns>
    internal static T DeserializeObject<T>(object array, bool checkForNullOrEmptyArray = true)
    {
        if (!checkForNullOrEmptyArray)
        {
            return JsonSerializer.Deserialize<T>(array?.ToString() ?? "", JsonOptions);
        }

        string _stringArray = array?.ToString() ?? "";
        if (_stringArray.NotNullOrWhiteSpace() && _stringArray != "[]")
        {
            return JsonSerializer.Deserialize<T>(_stringArray, JsonOptions);
        }

        return default;
    }

    // public General() : this(null)
    // {
    // }
    /*private static IServiceProvider _serviceProvider;
    // private static Dictionary<string, object> _restResponse;
    public static void SetServiceProvider(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    public static async Task SetValues(string someValue)
    {
        ISessionStorageService _sessionStorage = _serviceProvider.GetService<ISessionStorageService>();
        await _sessionStorage.SetItemAsync("PageState", someValue);
        // return Task.CompletedTask;
    }
    /// <summary>
    ///     Asynchronously executes the provided cancel method, hides the spinner and dialog, and enables the dialog buttons.
    ///     This method is designed to be used as a common cancellation routine for various dialogs in the application.
    /// </summary>
    /// <param name="args">The mouse event arguments associated with the cancel action.</param>
    /// <param name="spinner">The spinner control to be hidden when the cancel method completes.</param>
    /// <param name="footer">The dialog footer containing the buttons to be enabled when the cancel method completes.</param>
    /// <param name="dialog">The dialog to be hidden when the cancel method completes.</param>
    /// <param name="cancelMethod">The cancel method to be invoked asynchronously.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    internal static async Task CallCancelMethod(MouseEventArgs args, SfSpinner spinner, DialogFooter footer, SfDialog dialog, EventCallback<MouseEventArgs> cancelMethod)
    {
        await cancelMethod.InvokeAsync(args);
        footer.EnableButtons();
        await spinner.HideAsync();
        await dialog.HideAsync();
    }

    /// <summary>
    ///     Asynchronously executes the provided save method, shows the spinner, disables the dialog buttons, and then hides the spinner and dialog, and enables the
    ///     dialog buttons. This method is designed to be used as a common save routine for various dialogs in the application.
    /// </summary>
    /// <param name="editContext">The edit context associated with the save action.</param>
    /// <param name="spinner">
    ///     The spinner control to be shown when the save method starts and hidden when the save method
    ///     completes.
    /// </param>
    /// <param name="footer">
    ///     The dialog footer containing the buttons to be disabled when the save method starts and enabled
    ///     when the save method completes.
    /// </param>
    /// <param name="dialog">The dialog to be hidden when the save method completes.</param>
    /// <param name="saveMethod">The save method to be invoked asynchronously.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    internal static async Task CallSaveMethod(EditContext editContext, SfSpinner spinner, DialogFooter footer, SfDialog dialog, EventCallback<EditContext> saveMethod)
    {
        if (!footer.ButtonsDisabled())
        {
            await spinner.ShowAsync();
            footer.DisableButtons();
            await saveMethod.InvokeAsync(editContext);
            footer.EnableButtons();
            await spinner.HideAsync();
            await dialog.HideAsync();
        }
    }*/

    // Updated to use System.Text.Json with case-insensitive options for better performance

    /// <summary>
    ///     Creates and returns a DialogOptions object with the specified content text.
    /// </summary>
    /// <param name="contentText">The text content to be displayed in the dialog.</param>
    /// <returns>A DialogOptions object configured with the provided content text.</returns>
    internal static DialogOptions DialogOptions(string contentText)
    {
        return new()
               {
                   ChildContent = content => { content.AddContent(0, contentText.ToMarkupString()); },
                   CloseOnEscape = true,
                   AnimationSettings = new() {Effect = DialogEffect.Fade, Duration = 300},
                   PrimaryButtonOptions = new() {Content = "Yes"},
                   CancelButtonOptions = new() {Content = "No"},
                   ZIndex = 100000
               };
    }

    internal static async Task DisplaySpinner(SfSpinner spinner, bool show = true)
    {
        try
        {
            if (spinner != null)
            {
                if (show)
                {
                    await spinner.ShowAsync();
                }
                else
                {
                    await spinner.HideAsync();
                }
            }
        }
        catch
        {
            //
        }
    }

    public static async Task<List<T>> ExecuteAndDeserialize<T>(string endpoint, Dictionary<string, string> parameters)
    {
        string _response = await ExecuteRest<string>(endpoint, parameters);

        if (_response.NotNullOrWhiteSpace() && _response != "[]")
        {
            return DeserializeObject<List<T>>(_response);
        }

        return [];
    }

    /// <summary>
    ///     Executes the provided task within a semaphore lock. If the semaphore is currently locked, the method will return
    ///     immediately.
    ///     If an exception occurs during the execution of the task, it will be logged using the provided logger.
    /// </summary>
    /// <param name="semaphore">The semaphore used to control access to the task.</param>
    /// <param name="task">The task to be executed.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /*/// <param name="logger">The logger used to log any exceptions that occur during the execution of the task.</param>*/
    internal static async Task ExecuteMethod(SemaphoreSlim semaphore, Func<Task> task) //, ILogger logger)
    {
        if (await semaphore.WaitAsync(TimeSpan.Zero))
        {
            try
            {
                await task();
            }
            catch (Exception ex)
            {
                // Log exception for debugging while maintaining existing swallow behavior
                Log.Error(ex, "Error occurred in ExecuteMethod. {ExceptionMessage}", ex.Message);
            }
            finally
            {
                semaphore.Release();
            }
        }
    }

    public static async Task<T> ExecuteRest<T>(string endpoint, Dictionary<string, string> parameters = null, object jsonBody = null, bool isPost = true, byte[] fileArray = null, string fileName = "",
                                               string parameterName = "file")
    {
        // Use current Start.APIHost (set by middleware for localhost detection)
        using RestClient _client = new(Start.APIHost);
        RestRequest _request = new(endpoint, isPost ? Method.Post : Method.Get)
                               {
                                   RequestFormat = DataFormat.Json
                               };
        if (fileName.NotNullOrWhiteSpace())
        {
            _request.AlwaysMultipartFormData = true;
        }

        if (jsonBody is not null)
        {
            _request.AddJsonBody(jsonBody);
        }

        if (parameters is not null)
        {
            foreach (KeyValuePair<string, string> _parameter in parameters)
            {
                if (fileArray is null)
                {
                    _request.AddQueryParameter(_parameter.Key, _parameter.Value);
                }
                else
                {
                    _request.AddParameter(_parameter.Key, _parameter.Value, ParameterType.GetOrPost);
                }
            }
        }

        if (fileArray is not null)
        {
            _request.AddFile(parameterName, fileArray, fileName, "application/octet-stream");
        }

        RestResponse<T> response = null;
        try
        {
            response = await _client.ExecuteAsync<T>(_request);
        }
        catch (Exception ex)
        {
            // Log exception for debugging while maintaining existing swallow behavior
            Log.Error(ex, "Error in ExecuteRest for endpoint {Endpoint}. {ExceptionMessage}", endpoint, ex.Message);
            response = null;
        }

        return response!.IsSuccessful ? response.Data : default;
    }

    internal static async Task<object> GetAutocompleteAsync(string endpoint, DataManagerRequest dm, string methodName = "", string paramName = "")
    {
        List<KeyValues> _dataSource = [];
        if (dm.Where is not {Count: > 0} || dm.Where[0].value.NullOrWhiteSpace())
        {
            return dm.RequiresCounts ? new DataResult
                                       {
                                           Result = _dataSource,
                                           Count = 0
                                       } : _dataSource;
        }

        try
        {
            Dictionary<string, string> _parameters = new(1)
                                                     {
                                                         ["filter"] = dm.Where[0].value.ToString()
                                                     };

            if (methodName.NotNullOrWhiteSpace())
            {
                _parameters.Add("methodName", methodName);
                _parameters.Add("paramName", paramName);
            }

            string _response = await ExecuteRest<string>(endpoint, _parameters, null, false);

            // Updated to use System.Text.Json with standardized options
            if (_response.NotNullOrWhiteSpace() && _response != "[]")
            {
                _dataSource = JsonSerializer.Deserialize(_response, JsonContext.Default.ListKeyValues);
            }

            int _count = _dataSource.Count;

            return dm.RequiresCounts ? new DataResult
                                       {
                                           Result = _dataSource,
                                           Count = _count
                                       } : _dataSource;
        }
        catch
        {
            return dm.RequiresCounts ? new DataResult
                                       {
                                           Result = _dataSource,
                                           Count = 0
                                       } : _dataSource;
        }
    }

    internal static async Task<IEnumerable<Claim>> GetClaimsToken(ILocalStorageService local, ISessionStorageService session)
    {
        string _response = await local.GetItemAsStringAsync("PageState");
        if (_response.NullOrWhiteSpace())
        {
            _response = await session.GetItemAsStringAsync("PageState");
        }

        if (_response.NullOrWhiteSpace())
        {
            return null;
        }

        _response = _response?.Replace("\"", "");
        JwtSecurityTokenHandler handler = new();
        JwtSecurityToken jwtToken = handler.ReadJwtToken(_response);
        return jwtToken.Claims;
        //return _claims;
    }

    /*/// <summary>
    ///     Asynchronously retrieves a list of requisitions based on the provided search model and data manager request.
    ///     This method also has the ability to fetch additional company information if required.
    /// </summary>
    /// <param name="searchModel">The model containing the search parameters for the requisitions.</param>
    /// <param name="dm">The data manager request object.</param>
    /// <param name="thenProceed">
    ///     Optional parameter. If set to true, the method will continue processing even after
    ///     encountering an error. Default is false.
    /// </param>
    /// <returns>
    ///     A task that represents the asynchronous operation. The task result contains a list of requisitions or a data result
    ///     object if counts are required.
    /// </returns>
    internal static async Task<object> GetRequisitionReadAdaptor(RequisitionSearch searchModel, DataManagerRequest dm, bool thenProceed = false)
    {
        List<Requisition> _dataSource = [];

        /*int _itemCount = searchModel.ItemCount;
        int _page = searchModel.Page;#1#
        try
        {
            Dictionary<string, string> _parameters = new(4)
                                                     {
                                                         ["getCompanyInformation"] = dm.Params["GetInformation"].ToString(),

                                                         {"getCompanyInformation", dm.Params["GetInformation"].ToString()},
                                                         {"requisitionID", dm.Params["RequisitionID"].ToString()},
                                                         {"thenProceed", thenProceed.ToString()},
                                                         {"user", dm.Params["User"].ToString()}
                                                     };
            (int _count, string _requisitions, string _companies, string _companyContacts, string _status, int _pageNumber) =
                await ExecuteRest<ReturnGridRequisition>("Requisition/GetGridRequisitions", _parameters, searchModel, false);

            // Updated to use System.Text.Json with standardized options
            if (_count > 0)
            {
                _dataSource = JsonSerializer.Deserialize<List<Requisition>>(_requisitions, JsonOptions);
            }
            else
            {
                _dataSource = [];
            }
            searchModel.Page = _pageNumber;
            //_page = searchModel.Page;

            if (_dataSource == null)
            {
                return dm.RequiresCounts ? new DataResult
                                           {
                                               Result = null,
                                               Count = 0 /*_count#1#
                                           } : null;
            }

            if (!dm.Params["GetInformation"].ToBoolean())
            {
                if (_status.NotNullOrWhiteSpace())
                {
                    dm.Params["StatusList"] = "";
                }

                return dm.RequiresCounts ? new DataResult
                                           {
                                               Result = _dataSource,
                                               Count = _count /*_count#1#
                                           } : _dataSource;
            }

            //TODO: Use Cache
            // Requisitions.Skills = JsonConvert.DeserializeObject<List<IntValues>>(_restResponse["Skills"].ToString() ?? "");
            if (_status.NotNullOrWhiteSpace())
            {
                dm.Params.Add("StatusList", _status);
            }

            return dm.RequiresCounts ? new DataResult
                                       {
                                           Result = _dataSource,
                                           Count = _count /*_count#1#
                                       } : _dataSource;
        }
        catch (Exception)
        {
            if (_dataSource == null)
            {
                return dm.RequiresCounts ? new DataResult
                                           {
                                               Result = null,
                                               Count = 1
                                           } : null;
            }

            _dataSource.Add(new());

            return dm.RequiresCounts ? new DataResult
                                       {
                                           Result = _dataSource,
                                           Count = 1
                                       } : _dataSource;
        }
    }*/

    /// <summary>
    ///     Sends a REST request to the specified endpoint and returns the response as an object of type T.
    /// </summary>
    /// <param name="endpoint">The endpoint to which the REST request is sent.</param>
    /// <param name="parameters">A dictionary of query parameters to be included in the REST request.</param>
    /// <param name="jsonBody">An optional JSON body to be included in the REST request.</param>
    /// <returns>
    ///     A task that represents the asynchronous operation. The task result is the response from the REST request as an
    ///     object of type T.
    /// </returns>
    /// <remarks>
    ///     This method uses the RestClient class to send the REST request. If a JSON body is provided, it is included in the
    ///     request.
    ///     If a dictionary of parameters is provided, they are added as query parameters to the request.
    ///     The method then sends the request and awaits the response. The response is returned as an object of type T.
    /// </remarks>
    internal static async Task<T> GetRest<T>(string endpoint, Dictionary<string, string> parameters = null, object jsonBody = null)
    {
        using RestClient _client = new(Start.APIHost);
        RestRequest _request = new(endpoint)
                               {
                                   RequestFormat = DataFormat.Json
                               };

        if (jsonBody != null)
        {
            _request.AddJsonBody(jsonBody);
        }

        if (parameters == null)
        {
            return await _client.GetAsync<T>(_request);
        }

        foreach (KeyValuePair<string, string> _parameter in parameters)
        {
            _request.AddQueryParameter(_parameter.Key, _parameter.Value);
        }

        try
        {
            return await _client.GetAsync<T>(_request);
        }
        catch (Exception ex)
        {
            // Log exception for debugging while maintaining existing swallow behavior
            Log.Error(ex, "Error in GetRest for endpoint {Endpoint}. {ExceptionMessage}", endpoint, ex.Message);
            return default;
        }
    }

    public static async Task<List<T>> LoadDataAsync<T>(string methodName, string filter)
    {
        Dictionary<string, string> parameters = new(2)
                                                {
                                                    ["methodName"] = methodName,
                                                    ["filter"] = filter ?? string.Empty
                                                };

        string response = await ExecuteRest<string>("Admin/GetAdminList", parameters, null, false);

        // Updated to use System.Text.Json with standardized options
        List<T> result = DeserializeObject<List<T>>(response);

        return result;
    }

    /// <summary>
    ///     Sends a POST request to the specified endpoint with the provided parameters and JSON body.
    /// </summary>
    /// <param name="endpoint">The API endpoint to which the request is sent.</param>
    /// <param name="parameters">The parameters to be included in the request.</param>
    /// <param name="jsonBody">The JSON body to be included in the request. Default is null.</param>
    /// <param name="fileArray">Array of bytes containing the file contents. Default is null.</param>
    /// <param name="fileName">Name of the file to be uploaded. Default is blank and will be used in fileArray is not null.</param>
    /// <param name="parameterName">Name of the parameter to pass to the RESTful API. Default is `file`.</param>
    /// <returns>
    ///     A task that represents the asynchronous operation. The task result contains a dictionary with the response data.
    /// </returns>
    /// <remarks>
    ///     This method uses the RestClient to send a POST request to the API.
    ///     If a JSON body is provided, it is added to the request.
    ///     All key-value pairs in the parameters dictionary are added as query parameters to the request.
    /// </remarks>
    internal static Task<Dictionary<string, object>> PostRest(string endpoint, Dictionary<string, string> parameters, object jsonBody = null, byte[] fileArray = null, string fileName = "",
                                                              string parameterName = "file") => PostRest<Dictionary<string, object>>(endpoint, parameters, jsonBody, fileArray, fileName, parameterName);

    /// <summary>
    ///     Sends a POST request to the specified endpoint with the provided parameters and JSON body.
    /// </summary>
    /// <param name="endpoint">The API endpoint to which the request is sent.</param>
    /// <param name="parameters">The parameters to be included in the request.</param>
    /// <param name="jsonBody">The JSON body to be included in the request. Default is null.</param>
    /// <param name="fileArray">Array of bytes containing the file contents. Default is null.</param>
    /// <param name="fileName">Name of the file to be uploaded. Default is blank and will be used in fileArray is not null.</param>
    /// <param name="parameterName">Name of the parameter to pass to the RESTful API. Default is `file`.</param>
    /// <returns>
    ///     A task that represents the asynchronous operation. The task result contains a dictionary with the response data.
    /// </returns>
    /// <remarks>
    ///     This method uses the RestClient to send a POST request to the API.
    ///     If a JSON body is provided, it is added to the request.
    ///     All key-value pairs in the parameters dictionary are added as query parameters to the request.
    /// </remarks>
    internal static async Task<T> PostRest<T>(string endpoint, Dictionary<string, string> parameters = null, object jsonBody = null, byte[] fileArray = null, string fileName = "",
                                              string parameterName = "file")
    {
        //string host = ConfigManager.Get
        using RestClient _client = new(Start.APIHost);
        RestRequest _request = new(endpoint, Method.Post)
                               {
                                   RequestFormat = DataFormat.Json
                               };

        if (jsonBody != null)
        {
            _request.AddJsonBody(jsonBody);
        }

        if (parameters != null)
        {
            foreach (KeyValuePair<string, string> _parameter in parameters)
            {
                _request.AddQueryParameter(_parameter.Key, _parameter.Value);
            }
        }

        if (fileArray != null)
        {
            _request.AddFile(parameterName, fileArray, fileName);
        }

        return await _client.PostAsync<T>(_request);
    }

    /// <summary>
    ///     Sends a POST request to the specified endpoint with the provided parameters and JSON body.
    /// </summary>
    /// <param name="endpoint">The API endpoint to which the request is sent.</param>
    /// <param name="parameters">The parameters to be included in the request.</param>
    /// <param name="jsonBody">The JSON body to be included in the request. Default is null.</param>
    /// <param name="fileArray">Array of bytes containing the file contents. Default is null.</param>
    /// <param name="fileName">Name of the file to be uploaded. Default is blank and will be used in fileArray is not null.</param>
    /// <param name="parameterName">Name of the parameter to pass to the RESTful API. Default is `file`.</param>
    /// <returns>
    ///     A task that represents the asynchronous operation. The task result contains a dictionary with the response data.
    /// </returns>
    /// <remarks>
    ///     This method uses the RestClient to send a POST request to the API.
    ///     If a JSON body is provided, it is added to the request.
    ///     All key-value pairs in the parameters dictionary are added as query parameters to the request.
    /// </remarks>
    internal static async Task<T> PostRestParameter<T>(string endpoint, Dictionary<string, string> parameters = null, object jsonBody = null, byte[] fileArray = null, string fileName = "",
                                                       string parameterName = "file")
    {
        using RestClient _client = new(Start.APIHost);
        RestRequest _request = new(endpoint, Method.Post)
                               {
                                   AlwaysMultipartFormData = true
                               };

        if (jsonBody is not null)
        {
            _request.AddJsonBody(jsonBody);
        }

        if (parameters is not null)
        {
            foreach (KeyValuePair<string, string> _parameter in parameters)
            {
                _request.AddParameter(_parameter.Key, _parameter.Value, ParameterType.GetOrPost);
            }
        }

        if (fileArray is not null)
        {
            _request.AddFile(parameterName, fileArray, fileName);
        }

        return await _client.PostAsync<T>(_request);
    }

    internal static async Task<byte[]> ReadFromBlob(string blobPath)
    {
        //Connect to the Azure Blob Storage
        IAzureBlobStorage _storage = StorageFactory.Blobs.AzureBlobStorageWithSharedKey(Start.AccountName, Start.AzureKey);

        //Read the file into a Bytes Array
        byte[] _memBytes = await _storage.ReadBytesAsync(blobPath);

        return _memBytes;
    }

    
}