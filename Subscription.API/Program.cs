#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.API
// File Name:           Program.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu, Brijesh Dubey
// Created On:          02-05-2025 20:02
// Last Updated On:     07-06-2025 20:13
// *****************************************/

#endregion

WebApplicationBuilder _builder = WebApplication.CreateBuilder(args);

// Add services to the container.

_builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
_builder.Services.AddEndpointsApiExplorer();
_builder.Services.AddSwaggerGen();
ConfigurationManager _config = _builder.Configuration;

// Configure Serilog
ColumnOptions columnOptions = new();
columnOptions.Store.Remove(StandardColumn.Properties);
columnOptions.Store.Remove(StandardColumn.MessageTemplate);
columnOptions.Store.Add(StandardColumn.LogEvent);

Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Error()
            .WriteTo.Console()
            .WriteTo.MSSqlServer(_config.GetConnectionString("DBConnect"),
                                 new MSSqlServerSinkOptions {TableName = "Logs", AutoCreateSqlTable = true},
                                 columnOptions: columnOptions).CreateLogger();
_builder.Host.UseSerilog();
_builder.Services.AddSingleton<RedisService>(_ =>
                                             {
                                                 string host = _config["Garnet:HostName"];
                                                 int port = (_config["Garnet:SslPort"] ?? "").ToInt32();
                                                 string access = _config["GarnetServer:Access"];

                                                 return new(host, port, access, false);
                                             });

_builder.Services.AddSingleton<OpenAIClient>(_ =>
                                             {
                                                 string _apiKey = _config["AzureOpenAI:APIKey"];
                                                 string _endpoint = _config["AzureOpenAI:Endpoint"];
                                                 OpenAIClientOptions _options = new()
                                                                                {
                                                                                    Endpoint = new(_endpoint ?? "")
                                                                                };
                                                 return new(new(_apiKey ?? ""), _options);
                                             });

_builder.Services.AddScoped(_ =>
                            {
                                SmtpClient client = new(_config["Email:Host"], (_config["Email:Port"] ?? string.Empty).ToInt32(587));
                                client.Credentials = new NetworkCredential(_config["Email:UserName"], _config["Email:Password"]);
                                client.EnableSsl = true;
                                return client;
                            });

_builder.Services.AddSingleton<CacheInitializerService>();

/*GraphSenderOptions _graphOptions = new()
                                   {
                                       Secret = _config["Email:Secret"],
                                       ClientId = _config["Email:ClientID"],
                                       TenantId = _config["Email:TenantID"]
                                   };*/
/*
_builder.Services.AddFluentEmail("dummy@emailprovider.com")
        .AddSmtpSender(_config["Email:Host"], _config["Email:Port"].ToInt32(), _config["Email:Username"], _config["Email:Password"]);
*/
WebApplication _app = _builder.Build();

// Configure the HTTP request pipeline.
if (_app.Environment.IsDevelopment())
{
    _app.UseSwagger();
    _app.UseSwaggerUI();
}

_app.UseHttpsRedirection();

_app.UseSerilogRequestLogging();

_app.UseAuthorization();

_app.MapControllers();

bool _isLocal = false, _isSet = false;
_app.Use(async (context, next) =>
         {
             if (!_isSet)
             {
                 if (context.Request.Host.Host.Contains("localhost") || context.Request.Host.Host.Contains("127.0.0.1"))
                 {
                     _isLocal = true;
                 }

                 if (_isLocal)
                 {
                     Start.APIHost = _config["APIHost"];
                     Start.ConnectionString = _config.GetConnectionString("DBConnect");
                     Start.CacheServer = _config["Garnet:HostName"];
                     Start.CachePort = _config["Garnet:SslPort"];
                     Start.Access = _config["Garnet:Access"];
                 }
                 else
                 {
                     Start.APIHost = _config["APIHostServer"];
                     Start.ConnectionString = _config.GetConnectionString("DBConnectServer");
                     Start.CacheServer = _config["GarnetServer:HostName"];
                     Start.CachePort = _config["GarnetServer:SslPort"];
                     Start.Access = _config["GarnetServer:Access"];
                 }

                 Start.AzureKey = _config["AzureKey"];
                 Start.AzureBlob = _config.GetConnectionString("AzureBlob");
                 Start.AzureBlobContainer = _config["AzureBlobContainer"];
                 Start.AzureOpenAIKey = _config["AzureOpenAI:APIKey"];
                 Start.AzureOpenAIEndpoint = _config["AzureOpenAI:Endpoint"];
                 Start.AccountName = _config["AccountName"];
                 Start.DeploymentName = _config["AzureOpenAI:DeploymentName"];
                 Start.SystemChatMessage = _config["AzureOpenAI:SystemChatMessage"];
                 Start.Prompt = _config["AzureOpenAI:Prompt"];
                 Start.EmailHost = _config["Email:Host"];
                 Start.Port = (_config["Email:Port"] ?? string.Empty).ToInt32(587);
                 Start.EmailUsername = _config["Email:UserName"];
                 Start.EmailPassword = _config["Email:Password"];
                 Start.EmailSecret = _config["Email:Secret"];
                 Start.EmailClientID = _config["Email:ClientID"];
                 Start.TenantID = _config["Email:TenantID"];

                 CacheInitializerService cacheInitializer = _app.Services.GetRequiredService<CacheInitializerService>();
                 await cacheInitializer.InitializeCacheAsync(Start.ConnectionString);
                 _isSet = true;
             }

             await next.Invoke();
         });

_app.Run();