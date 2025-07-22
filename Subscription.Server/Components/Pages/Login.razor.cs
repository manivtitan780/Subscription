#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.Server
// File Name:           Login.razor.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu, Brijesh Dubey
// Created On:          04-22-2024 15:04
// Last Updated On:     05-12-2025 20:29
// *****************************************/

#endregion

/*using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Newtonsoft.Json.Linq;*/

namespace Subscription.Server.Components.Pages;

public partial class Login
{
    [Inject]
    private SfDialogService DialogService { get; set; }

    [Inject]
    private ILocalStorageService LocalStorage { get; set; }

    private LoginModel LoginModel { get; } = new();

    [Inject]
    private NavigationManager NavigationManager { get; set; }

    [Inject]
    private ISessionStorageService SessionStorage { get; set; }

    private async Task LoginToApplication(EditContext arg)
    {
        // Memory optimization: Pre-allocate Dictionary with capacity hint for better performance
        Dictionary<string, string> _parameters = new(2)
                                                 {
                                                     ["userName"] = LoginModel.UserName,
                                                     ["password"] = LoginModel.Password
                                                 };
        string _response = await General.ExecuteRest<string>("Login/LoginPage", _parameters);

        if (_response.NullOrWhiteSpace())
        {
            await DialogService.AlertAsync(null, "Invalid Credentials", General.DialogOptions("Invalid username or password."));
        }
        else
        {
            if (LoginModel.RememberMe)
            {
                await LocalStorage.SetItemAsync("PageState", _response);
            }
            else
            {
                await SessionStorage.SetItemAsync("PageState", _response);
            }

            //JwtSecurityTokenHandler handler = new();
            //JwtSecurityToken jwtToken = handler.ReadJwtToken(_response);
            //IEnumerable<Claim> claim = jwtToken.Claims;
            //string userName = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            //bool hasPermission = jwtToken.Claims.Any(c => c.Type == "Permission" && c.Value == "ViewAllCompanies");

            NavigationManager.NavigateTo($"{NavigationManager.BaseUri}company");
        }
    }

    protected override async Task OnInitializedAsync()
    {
        IEnumerable<Claim> _claims = await General.GetClaimsToken(LocalStorage, SessionStorage);
        if (_claims != null)
        {
            string _userName = _claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            if (!_userName.NullOrWhiteSpace())
            {
                NavigationManager.NavigateTo($"{NavigationManager.BaseUri}company", true);
            }
        }

        await base.OnInitializedAsync();
    }
}