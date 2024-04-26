#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription
// File Name:           Program.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu
// Created On:          2-7-2024 15:1
// Last Updated On:     2-7-2024 15:42
// *****************************************/

#endregion

using Extensions;

using Subscription.Client.Code;

WebApplicationBuilder _builder = WebApplication.CreateBuilder(args);

// Add services to the container.
_builder.Services.AddRazorComponents()
		.AddInteractiveServerComponents()
		.AddInteractiveWebAssemblyComponents();

_builder.Services.AddBlazoredSessionStorage(); // Session storage
_builder.Services.AddMemoryCache();
_builder.Services.AddSignalR(e =>
							 {
								 e.MaximumReceiveMessageSize = 10485760;
								 e.EnableDetailedErrors = true;
							 });
_builder.Services.AddScoped<SfDialogService>();
_builder.Services.AddSyncfusionBlazor();
_builder.Services.AddResponseCompression(options =>
										 {
											 options.Providers.Add<BrotliCompressionProvider>();
											 options.Providers.Add<GzipCompressionProvider>();
											 options.MimeTypes =
												 ResponseCompressionDefaults.MimeTypes.Concat(new[] { "image/svg+xml" });
										 });
_builder.Services.AddServerSideBlazor().AddCircuitOptions(option => { option.DetailedErrors = true; });
_builder.Services.Configure<BrotliCompressionProviderOptions>(options => { options.Level = CompressionLevel.Optimal; });
_builder.Services.Configure<GzipCompressionProviderOptions>(options => { options.Level = CompressionLevel.Optimal; });
//_builder.Services.AddSubtleCrypto(opt => opt.Key = "~1@3$5^7*9)-+QwErTyUiOpAsDfGhJkL");

WebApplication _app = _builder.Build();

// Configure the HTTP request pipeline.
if (_app.Environment.IsDevelopment())
{
	_app.UseWebAssemblyDebugging();
}
else
{
	_app.UseExceptionHandler("/Error", true);
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	_app.UseHsts();
}

SyncfusionLicenseProvider.RegisterLicense("MzE2MTcyNUAzMjM1MmUzMDJlMzBLM3NDRWpvWjNoczZxREUwRFBBbW42YWNmanh3bm4yU2FqTEZFU2NoTVlRPQ==");

_app.UseHttpsRedirection();

_app.UseStaticFiles();
_app.UseAntiforgery();

_app.MapRazorComponents<App>()
   .AddInteractiveServerRenderMode()
   .AddInteractiveWebAssemblyRenderMode()
   .AddAdditionalAssemblies(typeof(Subscription.Client._Imports).Assembly);


ConfigurationManager _config = _builder.Configuration;
bool _isLocal = false;

_app.Use(async (context, next) =>
		 {
			 if (context.Request.Host.Host.Contains("localhost") || context.Request.Host.Host.Contains("127.0.0.1"))
			 {
				 _isLocal = true;
			 }

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

			 await next.Invoke();
		 });

//bool _isLocal = _config["EnvironmentLocal"].ToBoolean();

//string _baseURL = _builder.Environment.WebRootPath;

//Start.APIHost = _builder.Configuration.GetValue(typeof(string), _baseURL.Contains("localhost") ? "APIHost" : "APIHostServer")?.ToString();

_app.Run();
