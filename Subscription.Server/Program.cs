#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.Server
// File Name:           Program.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu, Brijesh Dubey
// Created On:          02-06-2025 16:02
// Last Updated On:     05-12-2025 20:27
// *****************************************/

#endregion

#region Using

using Microsoft.AspNetCore.ResponseCompression;
using Serilog;
// Database sink removed - Server project doesn't access DB directly

using Subscription.Server.Components;

#endregion

WebApplicationBuilder _builder = WebApplication.CreateBuilder(args);

// Add services to the container.
_builder.Services.AddRazorComponents()
        .AddInteractiveServerComponents();
ConfigurationManager _config = _builder.Configuration;

_builder.Services.AddBlazoredSessionStorage(); // Session storage
_builder.Services.AddBlazoredLocalStorage();   // Local storage
_builder.Services.AddMemoryCache();
_builder.Services.AddSignalR(e =>
                             {
                                 e.MaximumReceiveMessageSize = 10485760;
                                 e.EnableDetailedErrors = true;
                             });
_builder.Services.AddScoped<SfDialogService>();
// _builder.Services.AddScoped<Container>();
// _builder.Services.AddScoped<Requisitions.RequisitionAdaptor>();
_builder.Services.AddSyncfusionBlazor();
_builder.Services.AddResponseCompression(options =>
                                         {
                                             options.Providers.Add<BrotliCompressionProvider>();
                                             options.Providers.Add<GzipCompressionProvider>();
                                             options.MimeTypes =
                                                 ResponseCompressionDefaults.MimeTypes.Concat(["image/svg+xml"]);
                                         });
_builder.Services.AddServerSideBlazor().AddCircuitOptions(option => { option.DetailedErrors = true; });
_builder.Services.Configure<BrotliCompressionProviderOptions>(options => { options.Level = CompressionLevel.Optimal; });
_builder.Services.Configure<GzipCompressionProviderOptions>(options => { options.Level = CompressionLevel.Optimal; });
/*_builder.Services.AddSingleton<OpenAIClient>(sp =>
                                             {
                                                 // IConfiguration _configService = sp.GetRequiredService<IConfiguration>();
                                                 string _apiKey = _config["AzureOpenAI:APIKey"];
                                                 string _endpoint = _config["AzureOpenAI:Endpoint"];
                                                 OpenAIClientOptions _options = new()
                                                                                {
                                                                                    Endpoint = new Uri(_endpoint ?? "")
                                                                                };
                                                 return new(new(_apiKey ?? ""), _options);
                                             });*/
_builder.Services.AddSingleton<RedisService>(_ =>
                                             {
                                                 string host = _config["Garnet:HostName"];
                                                 int port = (_config["Garnet:SslPort"] ?? "").ToInt32();
                                                 string access = _config["GarnetServer:Access"];

                                                 return new(host, port, access, false);
                                             });

_builder.Services.AddSingleton<ZipCodeService>();

// Note: RestClient singleton removed - will use dynamic host detection in ExecuteRest
// This allows proper localhost vs server host detection per request

// Enable Serilog for comprehensive exception logging

Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Error()
            .WriteTo.Console()
            .WriteTo.File(Path.Combine(Directory.GetCurrentDirectory(), "logs", "subscription-server-.log"), 
                         rollingInterval: RollingInterval.Day,
                         retainedFileCountLimit: 7,
                         outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
            .CreateLogger();
            // Database logging removed - this project doesn't have direct DB access
            // All DB logging should be handled by the API project
_builder.Host.UseSerilog();

WebApplication _app = _builder.Build();

// RestClient will be created dynamically in ExecuteRest using Start.APIHost
// No DI initialization needed

// Configure the HTTP request pipeline.
if (!_app.Environment.IsDevelopment())
{
    _app.UseExceptionHandler("/Error", true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    _app.UseHsts();
}

/*SyncfusionLicenseProvider.RegisterLicense("MzU1ODExM0AzMjM3MmUzMDJlMzBtNU84aGQ5RDhlYXNPZ0tpZ2U3bkloRnI4YThTb1hVUVFnOEpHTVdCYlRnPQ==");*/
SyncfusionLicenseProvider.RegisterLicense("MzkyNjE3M0AzMzMwMmUzMDJlMzAzYjMzMzAzYmpIVkptM0R6TXJXV0lFeUVtamFHNCsvTDNVMmc0cWVhOE1DRTVGbU10bUk9");

_app.UseHttpsRedirection();

_app.UseStaticFiles();
_app.UseAntiforgery();

_app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

bool _isLocal = false;

_app.Use(async (context, next) =>
         {
             if (Start.APIHost.NullOrWhiteSpace())
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

                 Start.AzureBlob = _config.GetConnectionString("AzureBlob");
                 Start.AzureBlobContainer = _config["AzureBlobContainer"];
                 Start.AzureKey = _config["AzureKey"];
                 Start.AccountName = _config["AccountName"];
                 Start.UploadsPath = _builder.Environment.WebRootPath;
                 Start.UploadPath = _config["RootPath"];
                 Start.AzureOpenAIKey = _config["AzureOpenAI:APIKey"];
                 Start.AzureOpenAIEndpoint = _config["AzureOpenAI:Endpoint"];
                 Start.Model = _config["AzureOpenAI:Model"];
                 Start.Prompt = _config["AzureOpenAI:Prompt"];
                 Start.DeploymentName = _config["AzureOpenAI:DeploymentName"];
                 Start.AllowedExtensions = _config["AllowedExtensions"];
             }

             await next.Invoke();
         });

await _app.RunAsync();