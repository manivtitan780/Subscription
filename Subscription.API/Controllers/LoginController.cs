#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.API
// File Name:           LoginController.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu
// Created On:          4-20-2024 20:30
// Last Updated On:     4-20-2024 20:34
// *****************************************/

#endregion

using Subscription.API.Code;

namespace Subscription.API.Controllers;

[ApiController, Route("api/[controller]/[action]")]
public class LoginController(IConfiguration configuration) : ControllerBase
{
    //private readonly IConfiguration _configuration = configuration;

    /// <summary>
    ///     Performs user login operation.
    /// </summary>
    /// <param name="userName">The username of the user.</param>
    /// <param name="password">The password of the user in base64 format.</param>
    /// <returns>
    ///     A task that represents the asynchronous operation. The task result contains the LoginCooky object with user
    ///     details if login is successful, null otherwise.
    /// </returns>
    [HttpPost("Login")]
    public async Task<LoginCooky> Login(string userName, string password)
    {
        //await Task.Yield();
        byte[] _password = General.SHA512PasswordHash(password);
        //byte[] _password = Convert.FromBase64String(password);
        await using SqlConnection _connection = new(configuration.GetConnectionString("DBConnect"));
        await using SqlCommand _command = new("ValidateLogin", _connection);
        _command.CommandType = CommandType.StoredProcedure;
        _command.Varchar("@User", 10, userName);
        _command.Binary("@Password", 16, _password);
        _connection.Open();
        await using SqlDataReader _reader = await _command.ExecuteReaderAsync();
        if (!_reader.HasRows)
        {
            return null;
        }

        _reader.Read();
        return new(userName, _reader.GetString(0), _reader.GetString(1), _reader.GetString(2), _reader.GetString(5), _reader.GetString(3),
                   _reader.IsDBNull(4) ? DateTime.MinValue : _reader.GetDateTime(4), _reader.NString(6));
    }
}