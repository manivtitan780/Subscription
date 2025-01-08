#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.Server
// File Name:           Program.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu
// Created On:          04-22-2024 15:04
// Last Updated On:     12-12-2024 19:12
// *****************************************/

#endregion

#region Using

using Microsoft.AspNetCore.ResponseCompression;

using Serilog;
using Serilog.Sinks.MSSqlServer;

using Subscription.Server.Components;

#endregion

WebApplicationBuilder _builder = WebApplication.CreateBuilder(args);

// Add services to the container.
_builder.Services.AddRazorComponents()
        .AddInteractiveServerComponents();

_builder.Services.AddBlazoredSessionStorage(); // Session storage
_builder.Services.AddBlazoredLocalStorage();   // Local storage
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
                                                 ResponseCompressionDefaults.MimeTypes.Concat(["image/svg+xml"]);
                                         });
_builder.Services.AddServerSideBlazor().AddCircuitOptions(option => { option.DetailedErrors = true; });
_builder.Services.Configure<BrotliCompressionProviderOptions>(options => { options.Level = CompressionLevel.Optimal; });
_builder.Services.Configure<GzipCompressionProviderOptions>(options => { options.Level = CompressionLevel.Optimal; });

/*
Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Error()
            .WriteTo.Console()
            .WriteTo.MSSqlServer(_config.GetConnectionString("DBConnect"),
                                 new MSSqlServerSinkOptions {TableName = "Logs", AutoCreateSqlTable = true},
                                 columnOptions: columnOptions).CreateLogger();

_builder.Host.UseSerilog();*/

WebApplication _app = _builder.Build();

// Configure the HTTP request pipeline.
if (!_app.Environment.IsDevelopment())
{
    _app.UseExceptionHandler("/Error", true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    _app.UseHsts();
}

/*SyncfusionLicenseProvider.RegisterLicense("MzU1ODExM0AzMjM3MmUzMDJlMzBtNU84aGQ5RDhlYXNPZ0tpZ2U3bkloRnI4YThTb1hVUVFnOEpHTVdCYlRnPQ==");*/
SyncfusionLicenseProvider.RegisterLicense("MzYyMDc2MUAzMjM4MmUzMDJlMzBldVNkeUdtdkk5ZC91Y0VZVXd2dU1uNU0rK1gxT0VrenBuNFc2cGRLVWJBPQ==");

_app.UseHttpsRedirection();

_app.UseStaticFiles();
_app.UseAntiforgery();

_app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

ConfigurationManager _config = _builder.Configuration;
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
             }

             await next.Invoke();
         });

_app.Run();