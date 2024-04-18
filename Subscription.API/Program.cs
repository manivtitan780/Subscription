#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.API
// File Name:           Program.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu
// Created On:          2-7-2024 16:21
// Last Updated On:     4-16-2024 20:47
// *****************************************/

#endregion
#region Using


using Microsoft.Extensions.DependencyInjection;

using StackExchange.Redis;

using Subscription.API;

#endregion

WebApplicationBuilder _builder = WebApplication.CreateBuilder(args);

// Add services to the container.

_builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
_builder.Services.AddEndpointsApiExplorer();
_builder.Services.AddSwaggerGen();
ConfigurationManager _config = _builder.Configuration;

WebApplication _app = _builder.Build();

// Configure the HTTP request pipeline.
if (_app.Environment.IsDevelopment())
{
	_app.UseSwagger();
	_app.UseSwaggerUI();
}

_app.UseHttpsRedirection();

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

				 await General.SetCache();
				 _isSet = true;
			 }

			 await next.Invoke();
		 });

_app.Run();