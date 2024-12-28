#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.API
// File Name:           Program.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu
// Created On:          02-07-2024 16:02
// Last Updated On:     12-28-2024 19:12
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

                 Start.AzureBlob = _config.GetConnectionString("AzureBlob");
                 Start.AzureBlobContainer = _config["AzureBlobContainer"];
                 Start.AzureKey = _config["AzureKey"];
                 Start.AccountName = _config["AccountName"];

                 await General.SetCache();
                 _isSet = true;
             }

             await next.Invoke();
         });

_app.Run();