#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.Server
// File Name:           General.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu
// Created On:          04-22-2024 15:04
// Last Updated On:     10-29-2024 15:10
// *****************************************/

#endregion

#region Using

using System.IdentityModel.Tokens.Jwt;

#endregion

namespace Subscription.Server.Code;

public class General
{
    /// <summary>
    ///     Deserializes a JSON string to an object of a specified type.
    /// </summary>
    /// <typeparam name="T">The type of object to deserialize to.</typeparam>
    /// <param name="array">The JSON string representing the object to be deserialized.</param>
    /// <returns>The deserialized object of type T.</returns>
    internal static T DeserializeObject<T>(object array) => JsonConvert.DeserializeObject<T>(array?.ToString() ?? string.Empty);

    internal static DialogOptions DialogOptions(string contentText)
    {
        return new()
               {
                   ChildContent = content => { content.AddContent(0, contentText.ToMarkupString()); },
                   CloseOnEscape = true
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
            catch (Exception)
            {
                //logger.LogError(_ex, $"Exception occurred at: [{DateTime.Today.ToString(CultureInfo.InvariantCulture)}]{Environment.NewLine}{new('-', 40)}{Environment.NewLine}");
            }
            finally
            {
                semaphore.Release();
            }
        }
    }

    /// <summary>
    ///     Asynchronously retrieves autocomplete data for a specific method and parameter.
    /// </summary>
    /// <param name="endpoint">The endpoint to connect to, to fetch the list of items to display.</param>
    /// <param name="dm">The DataManagerRequest object containing additional request parameters.</param>
    /// <returns>
    ///     A task that represents the asynchronous operation. The task result contains a list of autocomplete options
    ///     in the form of KeyValues objects if any matches are found, or an empty list if no matches are found.
    ///     If the DataManagerRequest object requires counts, the task result is a DataResult object containing the
    ///     autocomplete options and their count.
    /// </returns>
    /// <remarks>
    ///     This method makes an asynchronous request to the 'Admin/GetSearchDropDown' endpoint of the API,
    ///     passing the method name, parameter name, and filter value as query parameters.
    ///     The response is a list of strings which are then converted into KeyValues objects and returned.
    ///     If an exception occurs during the operation, an empty list or a DataResult object with a count of 0 is returned.
    /// </remarks>
    internal static async Task<object> GetAutocompleteAsync(string endpoint, DataManagerRequest dm)
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
            Dictionary<string, string> _parameters = new()
                                                     {
                                                         {"filter", dm.Where[0].value.ToString()}
                                                     };
            List<KeyValues> _response = await GetRest<List<KeyValues>>(endpoint, _parameters);

            int _count = 0;
            if (_response == null)
            {
                return dm.RequiresCounts ? new DataResult
                                           {
                                               Result = _dataSource,
                                               Count = _count
                                           } : _dataSource;
            }

            _count = _response.Count;

            return dm.RequiresCounts ? new DataResult
                                       {
                                           Result = _response,
                                           Count = _count
                                       } : _response;
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

        _response = _response?.Replace("\"", string.Empty);
        JwtSecurityTokenHandler handler = new();
        JwtSecurityToken jwtToken = handler.ReadJwtToken(_response);
        IEnumerable<Claim> _claims = jwtToken.Claims.ToList();

        return _claims;
    }

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

        return await _client.GetAsync<T>(_request);
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
}