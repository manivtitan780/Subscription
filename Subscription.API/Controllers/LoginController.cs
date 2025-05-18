#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.API
// File Name:           LoginController.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu, Brijesh Dubey
// Created On:          02-06-2025 16:02
// Last Updated On:     05-18-2025 18:55
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
public class LoginController(IConfiguration configuration, RedisService redisService) : ControllerBase
{
    private string GenerateToken(string username, List<string> permissions, string roleName = "RC")
    {
        string _keyString = configuration["JWTSecretKey"] ?? "";
        if (_keyString.NullOrWhiteSpace())
        {
            return "";
        }

        List<Claim> _claims = [new(ClaimTypes.Name, username), new(ClaimTypes.Role, roleName)];
        _claims.AddRange(permissions.Select(permission => new Claim("Permission", permission)));

        SymmetricSecurityKey _key = new(Encoding.UTF8.GetBytes(_keyString));

        SigningCredentials _credentials = new(_key, SecurityAlgorithms.HmacSha256);

        JwtSecurityToken token = new(configuration["JWTIssuer"],
                                     configuration["JWTAudience"],
                                     _claims,
                                     expires: DateTime.UtcNow.AddDays(14),
                                     signingCredentials: _credentials);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

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
    public async Task<ActionResult<string>> LoginPage(string userName, string password)
    {
        await using SqlConnection _connection = new(configuration.GetConnectionString("DBConnect"));
        await using SqlCommand _command = new("ValidateLogin", _connection);
        _command.CommandType = CommandType.StoredProcedure;
        _command.Varchar("@User", 10, userName);
        try
        {
            await _connection.OpenAsync();
            await using SqlDataReader _reader = await _command.ExecuteReaderAsync();
            while (await _reader.ReadAsync())
            {
                byte[] _salt = (byte[])_reader["Salt"];
                byte[] _sqlPassword = (byte[])_reader["Password"];
                if (!PasswordHasher.VerifyPassword(password, _sqlPassword, _salt))
                {
                    continue;
                }

                int _roleID = (byte)_reader["Role"];

                RedisValue _roles = await redisService.GetAsync("Roles");
                string _roleString = _roles.ToString();
                List<Role> _rolesList = JsonConvert.DeserializeObject<List<Role>>(_roleString);
                Role _userRole = _rolesList!.FirstOrDefault(role => role.ID == _roleID)!;
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

                if (_userRole is {CreateOrEditRequisitions: true})
                {
                    _permissions.Add(nameof(_userRole.CreateOrEditRequisitions));
                }

                if (_userRole is {ViewOnlyMyCandidates: true})
                {
                    _permissions.Add(nameof(_userRole.ViewOnlyMyCandidates));
                }

                if (_userRole is {ViewAllCandidates: true})
                {
                    _permissions.Add(nameof(_userRole.ViewAllCandidates));
                }

                if (_userRole is {ViewRequisitions: true})
                {
                    _permissions.Add(nameof(_userRole.ViewRequisitions));
                }

                if (_userRole is {EditRequisitions: true})
                {
                    _permissions.Add(nameof(_userRole.EditRequisitions));
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

                if (_userRole is {AdminScreens: true})
                {
                    _permissions.Add(nameof(_userRole.AdminScreens));
                }

                return Ok(GenerateToken(userName, _permissions, _userRole.RoleName));
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error logging in. {ExceptionMessage}", ex.Message);
            return StatusCode(500, ex.Message);
        }
        finally
        {
            await _connection.CloseAsync();
        }

        return BadRequest("Invalid Credentials");
    }
}