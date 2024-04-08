#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.Client
// File Name:           Program.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu
// Created On:          2-7-2024 15:1
// Last Updated On:     2-7-2024 15:49
// *****************************************/

#endregion

WebAssemblyHostBuilder _builder = WebAssemblyHostBuilder.CreateDefault(args);
_builder.Services.AddBlazoredSessionStorage(); // Session storage
_builder.Services.AddMemoryCache();
_builder.Services.AddScoped<SfDialogService>();
_builder.Services.AddSyncfusionBlazor();
_builder.Services.AddSubtleCrypto(opt => opt.Key = "~1@3$5^7*9)-+QwErTyUiOpAsDfGhJkL");
SyncfusionLicenseProvider.RegisterLicense("MzE2MTcyNUAzMjM1MmUzMDJlMzBLM3NDRWpvWjNoczZxREUwRFBBbW42YWNmanh3bm4yU2FqTEZFU2NoTVlRPQ==");

string _baseURL = _builder.HostEnvironment.BaseAddress;

Start.APIHost = _builder.Configuration.GetValue(typeof(string), _baseURL.Contains("localhost") ? "APIHost" : "APIHostServer")?.ToString();

await _builder.Build().RunAsync();