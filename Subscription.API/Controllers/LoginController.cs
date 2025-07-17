#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.API
// File Name:           LoginController.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu, Brijesh Dubey
// Created On:          07-15-2025 19:07
// Last Updated On:     07-15-2025 19:56
// *****************************************/

#endregion

#region Using

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using Microsoft.IdentityModel.Tokens;

using StackExchange.Redis;

using Role = Subscription.Model.Role;

#endregion

namespace Subscription.API.Controllers;

[ApiController, Route("api/[controller]/[action]")]
public class LoginController(IConfiguration configuration, RedisService redisService) : ControllerBase
{
    // Static caching for roles to avoid repeated JSON deserialization
    private static List<Role> _cachedRoles;
    private static readonly SemaphoreSlim CacheUpdateSemaphore = new(1, 1);

    // Static caching for JWT key bytes to avoid repeated UTF8 encoding
    private static byte[] _cachedKeyBytes;
    private static string _cachedKeyString;

    private string GenerateToken(string username, List<string> permissions, string roleName = "RC")
    {
        string _keyString = configuration["JWTSecretKey"] ?? "";
        if (_keyString.NullOrWhiteSpace())
        {
            return "";
        }

        List<Claim> _claims = [new(ClaimTypes.Name, username), new(ClaimTypes.Role, roleName)];
        _claims.AddRange(permissions.Select(permission => new Claim("Permission", permission)));

        // Optimized: Cache UTF8 bytes to avoid repeated encoding allocation
        if (_cachedKeyBytes == null || _cachedKeyString != _keyString)
        {
            _cachedKeyString = _keyString;
            _cachedKeyBytes = Encoding.UTF8.GetBytes(_keyString);
        }

        SymmetricSecurityKey _key = new(_cachedKeyBytes);

        SigningCredentials _credentials = new(_key, SecurityAlgorithms.HmacSha256);

        JwtSecurityToken token = new(configuration["JWTIssuer"], configuration["JWTAudience"], _claims, expires: DateTime.UtcNow.AddDays(14), signingCredentials: _credentials);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private async Task<List<Role>> GetCachedRolesAsync()
    {
        if (_cachedRoles != null)
        {
            return _cachedRoles;
        }

        await CacheUpdateSemaphore.WaitAsync();
        try
        {
            if (_cachedRoles != null)
            {
                return _cachedRoles;
            }

            RedisValue _roles = await redisService.GetAsync("Roles");
            string _roleString = _roles.ToString();

            // Optimized: Using System.Text.Json instead of Newtonsoft.Json for better performance
            _cachedRoles = JsonSerializer.Deserialize<List<Role>>(_roleString) ?? [];
            return _cachedRoles;
        }
        finally
        {
            CacheUpdateSemaphore.Release();
        }
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
        // Fixed: Using Start.ConnectionString for consistency with other controllers
        await using SqlConnection _connection = new(Start.ConnectionString);
        await using SqlCommand _command = new("ValidateLogin", _connection);
        _command.CommandType = CommandType.StoredProcedure;
        _command.Varchar("@User", 10, userName);
        try
        {
            await _connection.OpenAsync();
            await using SqlDataReader _reader = await _command.ExecuteReaderAsync();
            // Optimized: Using if instead of while since exactly 1 user is expected
            if (await _reader.ReadAsync())
            {
                byte[] _salt = (byte[])_reader["Salt"];
                byte[] _sqlPassword = (byte[])_reader["Password"];
                if (!PasswordHasher.VerifyPassword(password, _sqlPassword, _salt))
                {
                    return BadRequest("Invalid Credentials");
                }

                int _roleID = (byte)_reader["Role"];

                // Optimized: Using cached roles instead of repeated JSON deserialization
                List<Role> _rolesList = await GetCachedRolesAsync();
                Role _userRole = _rolesList.FirstOrDefault(role => role.ID == _roleID)!;

                // Optimized: Using LINQ to build permissions list efficiently
                List<string> _permissions = new[]
                                            {
                                                (nameof(_userRole.CreateOrEditCompany), _userRole.CreateOrEditCompany), (nameof(_userRole.CreateOrEditCandidate), _userRole.CreateOrEditCandidate),
                                                (nameof(_userRole.ViewAllCompanies), _userRole.ViewAllCompanies), (nameof(_userRole.ViewMyCompanyProfile), _userRole.ViewMyCompanyProfile),
                                                (nameof(_userRole.EditMyCompanyProfile), _userRole.EditMyCompanyProfile),
                                                (nameof(_userRole.CreateOrEditRequisitions), _userRole.CreateOrEditRequisitions),
                                                (nameof(_userRole.ViewOnlyMyCandidates), _userRole.ViewOnlyMyCandidates), (nameof(_userRole.ViewAllCandidates), _userRole.ViewAllCandidates),
                                                (nameof(_userRole.ViewRequisitions), _userRole.ViewRequisitions), (nameof(_userRole.EditRequisitions), _userRole.EditRequisitions),
                                                (nameof(_userRole.ManageSubmittedCandidates), _userRole.ManageSubmittedCandidates), (nameof(_userRole.DownloadOriginal), _userRole.DownloadOriginal),
                                                (nameof(_userRole.DownloadFormatted), _userRole.DownloadFormatted), (nameof(_userRole.AdminScreens), _userRole.AdminScreens)
                                            }
                                           .Where(permission => permission.Item2)
                                           .Select(permission => permission.Item1)
                                           .ToList();

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