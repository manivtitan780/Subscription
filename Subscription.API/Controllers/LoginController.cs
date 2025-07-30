#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.API
// File Name:           LoginController.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu, Brijesh Dubey
// Created On:          07-15-2025 19:07
// Last Updated On:     07-28-2025 15:51
// *****************************************/

#endregion

#region Using

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

using Microsoft.IdentityModel.Tokens;

using StackExchange.Redis;

using Role = Subscription.Model.Role;
using Subscription.Model;

#endregion

namespace Subscription.API.Controllers;

[ApiController, Route("api/[controller]/[action]")]
public class LoginController(IConfiguration configuration, RedisService redisService) : ControllerBase
{
    // Static caching for roles to avoid repeated JSON deserialization
    private static List<Role> _cachedRoles;
    private static readonly SemaphoreSlim CacheUpdateSemaphore = new(1, 1);
    private static readonly JwtSecurityTokenHandler TokenHandler = new();

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
        return TokenHandler.WriteToken(token);
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
            if (!_roles.HasValue)
            {
                _cachedRoles = [];
                return _cachedRoles;
            }

            string _roleString = _roles.ToString();

            // Optimized: Using System.Text.Json instead of Newtonsoft.Json for better performance
            _cachedRoles = JsonSerializer.Deserialize<List<Role>>(_roleString, JsonContext.CaseInsensitive.ListRole) ?? [];
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

                List<Role> _rolesList = await GetCachedRolesAsync();
                Role _userRole = _rolesList.FirstOrDefault(role => role.ID == _roleID);

                if (_userRole == null)
                {
                    Log.Error($"Role with ID {_roleID} not found in cache for user {userName}.");
                    return StatusCode(500, "User role configuration error.");
                }

                // More efficient permission list construction
                List<string> _permissions = [];
                if (_userRole.CreateOrEditCompany)
                {
                    _permissions.Add(nameof(_userRole.CreateOrEditCompany));
                }

                if (_userRole.CreateOrEditCandidate)
                {
                    _permissions.Add(nameof(_userRole.CreateOrEditCandidate));
                }

                if (_userRole.ViewAllCompanies)
                {
                    _permissions.Add(nameof(_userRole.ViewAllCompanies));
                }

                if (_userRole.ViewMyCompanyProfile)
                {
                    _permissions.Add(nameof(_userRole.ViewMyCompanyProfile));
                }

                if (_userRole.EditMyCompanyProfile)
                {
                    _permissions.Add(nameof(_userRole.EditMyCompanyProfile));
                }

                if (_userRole.CreateOrEditRequisitions)
                {
                    _permissions.Add(nameof(_userRole.CreateOrEditRequisitions));
                }

                if (_userRole.ViewOnlyMyCandidates)
                {
                    _permissions.Add(nameof(_userRole.ViewOnlyMyCandidates));
                }

                if (_userRole.ViewAllCandidates)
                {
                    _permissions.Add(nameof(_userRole.ViewAllCandidates));
                }

                if (_userRole.ViewRequisitions)
                {
                    _permissions.Add(nameof(_userRole.ViewRequisitions));
                }

                if (_userRole.EditRequisitions)
                {
                    _permissions.Add(nameof(_userRole.EditRequisitions));
                }

                if (_userRole.ManageSubmittedCandidates)
                {
                    _permissions.Add(nameof(_userRole.ManageSubmittedCandidates));
                }

                if (_userRole.DownloadOriginal)
                {
                    _permissions.Add(nameof(_userRole.DownloadOriginal));
                }

                if (_userRole.DownloadFormatted)
                {
                    _permissions.Add(nameof(_userRole.DownloadFormatted));
                }

                if (_userRole.AdminScreens)
                {
                    _permissions.Add(nameof(_userRole.AdminScreens));
                }

                return Ok(GenerateToken(userName, _permissions, _userRole.RoleName));
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error logging in. {ExceptionMessage}", ex.Message);
            return StatusCode(500, "An unexpected error occurred during login.");
        }

        return BadRequest("Invalid Credentials");
    }
}