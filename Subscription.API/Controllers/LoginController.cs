#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.API
// File Name:           LoginController.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu
// Created On:          04-20-2024 20:04
// Last Updated On:     10-29-2024 15:10
// *****************************************/

#endregion

#region Using

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using Microsoft.IdentityModel.Tokens;

using Newtonsoft.Json;

using StackExchange.Redis;

using Role = Subscription.Model.Role;

#endregion

namespace Subscription.API.Controllers;

[ApiController, Route("api/[controller]/[action]")]
public class LoginController(IConfiguration configuration) : ControllerBase
{
    public string GenerateToken(string username, List<string> permissions)
    {
        List<Claim> _claims = [new(ClaimTypes.Name, username)];
        _claims.AddRange(permissions.Select(permission => new Claim("Permission", permission)));

        SymmetricSecurityKey _key = new(Encoding.UTF8.GetBytes(configuration["JWTSecretKey"] ?? "SomeKey"));
        SigningCredentials _credentials = new(_key, SecurityAlgorithms.HmacSha256);

        JwtSecurityToken token = new(configuration["JWTIssuer"],
                                     configuration["JWTAudience"],
                                     _claims,
                                     expires: DateTime.Now.AddDays(14),
                                     signingCredentials: _credentials);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
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
    [HttpPost]
    public async Task<string> LoginPage(string userName, string password)
    {
        //await Task.Yield();
        //byte[] _password = Convert.FromBase64String(password);
        await using SqlConnection _connection = new(configuration.GetConnectionString("DBConnect"));
        await using SqlCommand _command = new("ValidateLogin", _connection);
        _command.CommandType = CommandType.StoredProcedure;
        _command.Varchar("@User", 10, userName);
        await _connection.OpenAsync();
        await using SqlDataReader _reader = await _command.ExecuteReaderAsync();
        while (await _reader.ReadAsync())
        {
            byte[] _salt = (byte[])_reader["Salt"];
            byte[] _sqlPassword = (byte[])_reader["Password"];
            byte[] _password = General.ComputeHashWithSalt(password, _salt);
            int _roleID = (byte)_reader["Role"];
            if (!_sqlPassword.SequenceEqual(_password))
            {
                continue;
            }

            RedisService _service = new(Start.CacheServer, Start.CachePort.ToInt32(), Start.Access, false);
            RedisValue _roles = await _service.GetAsync("Roles");
            string _roleString = _roles.ToString();
            List<Role> _rolesList = JsonConvert.DeserializeObject<List<Role>>(_roleString);
            Role _userRole = _rolesList.FirstOrDefault(role => role.ID == _roleID);
            List<string> _permissions = [];

            if (_userRole is {CreateOrEditCompany: true})
            {
                _permissions.Add(nameof(_userRole.CreateOrEditCompany));
            }

            if (_userRole is {CreateOrEditCandidate: true})
            {
                _permissions.Add(nameof(_userRole.CreateOrEditCandidate));
            }

            if (_userRole is {ViewAllCompanies: true})
            {
                _permissions.Add(nameof(_userRole.ViewAllCompanies));
            }

            if (_userRole is {ViewMyCompanyProfile: true})
            {
                _permissions.Add(nameof(_userRole.ViewMyCompanyProfile));
            }

            if (_userRole is {EditMyCompanyProfile: true})
            {
                _permissions.Add(nameof(_userRole.EditMyCompanyProfile));
            }

            if (_userRole is {CreateOrEditRequisition: true})
            {
                _permissions.Add(nameof(_userRole.CreateOrEditRequisition));
            }

            if (_userRole is {ViewOnlyMyCandidates: true})
            {
                _permissions.Add(nameof(_userRole.ViewOnlyMyCandidates));
            }

            if (_userRole is {ViewAllCandidates: true})
            {
                _permissions.Add(nameof(_userRole.ViewAllCandidates));
            }

            if (_userRole is {ManageSubmittedCandidates: true})
            {
                _permissions.Add(nameof(_userRole.ManageSubmittedCandidates));
            }

            if (_userRole is {DownloadOriginal: true})
            {
                _permissions.Add(nameof(_userRole.DownloadOriginal));
            }

            if (_userRole is {DownloadFormatted: true})
            {
                _permissions.Add(nameof(_userRole.DownloadFormatted));
            }

            return GenerateToken(userName, _permissions);
        }

        return "";
    }
}