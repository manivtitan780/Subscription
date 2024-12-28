#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.Client
// File Name:           Program.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu
// Created On:          2-7-2024 15:1
// Last Updated On:     4-17-2024 21:5
// *****************************************/

#endregion

WebAssemblyHostBuilder _builder = WebAssemblyHostBuilder.CreateDefault(args);
_builder.Services.AddBlazoredSessionStorage(); // Session storage
_builder.Services.AddMemoryCache();
_builder.Services.AddScoped<SfDialogService>();
_builder.Services.AddSyncfusionBlazor();
//_builder.Services.AddSubtleCrypto(opt => opt.Key = "~1@3$5^7*9)-+QwErTyUiOpAsDfGhJkL");
SyncfusionLicenseProvider.RegisterLicense("MzE2MTcyNUAzMjM1MmUzMDJlMzBLM3NDRWpvWjNoczZxREUwRFBBbW42YWNmanh3bm4yU2FqTEZFU2NoTVlRPQ==");

bool _isLocal = false;

WebAssemblyHostConfiguration _config = _builder.Configuration;

_isLocal = _config["EnvironmentLocal"].ToBoolean();
if (_isLocal)
{
	Start.APIHost = _config["APIHost"];
	Start.CacheServer = _config["Garnet:HostName"];
	Start.CachePort = _config["Garnet:SslPort"];
	Start.Access = _config["Garnet:Access"];
}
else
{
	Start.APIHost = _config["APIHostServer"];
	Start.CacheServer = _config["GarnetServer:HostName"];
	Start.CachePort = _config["GarnetServer:SslPort"];
	Start.Access = _config["GarnetServer:Access"];
}

await _builder.Build().RunAsync();