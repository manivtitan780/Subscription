﻿#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.API
// File Name:           GlobalUsings.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu
// Created On:          02-08-2024 15:02
// Last Updated On:     12-19-2024 15:12
// *****************************************/

#endregion

#region Using

global using Microsoft.AspNetCore.Mvc;

global using System.Data;

global using Microsoft.Data.SqlClient;

global using Extensions;

global using FluentStorage;
global using FluentStorage.Blobs;
global using FluentStorage.Azure.Blobs;

global using Subscription.Model;

global using Serilog;
global using Serilog.Sinks.MSSqlServer;

global using Subscription.API;
global using Subscription.API.Code;

#endregion