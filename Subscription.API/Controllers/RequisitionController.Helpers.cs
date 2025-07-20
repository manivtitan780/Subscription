#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.API
// File Name:           RequisitionController.Helpers.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu, Brijesh Dubey
// Created On:          07-19-2025 20:07
// Last Updated On:     07-19-2025 20:25
// *****************************************/

#endregion

namespace Subscription.API.Controllers;

public partial class RequisitionController
{
    private async Task<ActionResult<string>> ExecuteQueryAsync(string procedureName, Action<SqlCommand> parameterBinder, string logContext, string errorMessage)
    {
        await using SqlConnection _connection = new(Start.ConnectionString);
        await using SqlCommand _command = new(procedureName, _connection);
        _command.CommandType = CommandType.StoredProcedure;

        parameterBinder(_command);

        string _result = "[]";
        try
        {
            await _connection.OpenAsync();
            _result = (await _command.ExecuteScalarAsync())?.ToString() ?? "[]";
        }
        catch (SqlException ex)
        {
            Log.Error(ex, "Error executing {logContext} query. {ExceptionMessage}", logContext, ex.Message);
            return StatusCode(500, errorMessage);
        }

        return Ok(_result);
    }

    private static string GetPriority(byte priority)
    {
        return priority switch
               {
                   0 => "Low",
                   2 => "High",
                   _ => "Medium"
               };
    }

    private async Task<ActionResult<T>> ExecuteReaderAsync<T>(string procedureName, Action<SqlCommand> parameterBinder, Func<SqlDataReader, Task<T>> readerProcessor, string logContext,
                                                              string errorMessage)
    {
        await using SqlConnection _connection = new(Start.ConnectionString);
        await using SqlCommand _command = new(procedureName, _connection);
        _command.CommandType = CommandType.StoredProcedure;

        parameterBinder(_command);

        try
        {
            await _connection.OpenAsync();
            await using SqlDataReader _reader = await _command.ExecuteReaderAsync();
            T _result = await readerProcessor(_reader);
            return Ok(_result);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error executing {logContext} query. {ExceptionMessage}", logContext, ex.Message);
            return StatusCode(500, errorMessage);
        }
    }

    private async Task<ActionResult<T>> ExecuteScalarAsync<T>(string procedureName, Action<SqlCommand> parameterBinder, string logContext, string errorMessage)
    {
        await using SqlConnection _connection = new(Start.ConnectionString);
        await using SqlCommand _command = new(procedureName, _connection);
        _command.CommandType = CommandType.StoredProcedure;

        parameterBinder(_command);

        try
        {
            await _connection.OpenAsync();
            object _result = await _command.ExecuteScalarAsync();
            return Ok((T)Convert.ChangeType(_result ?? default(T), typeof(T)));
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error executing {logContext} query. {ExceptionMessage}", logContext, ex.Message);
            return StatusCode(500, errorMessage);
        }
    }

    private static string GenerateLocation(RequisitionDetails requisition, string stateName)
    {
        List<string> _parts = [];

        if (requisition.City.NotNullOrWhiteSpace() && !stateName.Contains(requisition.City))
        {
            _parts.Add(requisition.City);
        }

        if (stateName.NotNullOrWhiteSpace())
        {
            _parts.Add(stateName);
        }

        if (requisition.ZipCode.NotNullOrWhiteSpace())
        {
            _parts.Add(requisition.ZipCode);
        }

        return string.Join(", ", _parts);
    }
}