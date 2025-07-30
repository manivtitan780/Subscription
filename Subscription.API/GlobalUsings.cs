#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.API
// File Name:           GlobalUsings.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu, Brijesh Dubey
// Created On:          02-06-2025 16:02
// Last Updated On:     07-23-2025 19:12
// *****************************************/

#endregion

#region Using

global using System.Data;
global using System.Diagnostics.CodeAnalysis;
global using System.Net;
global using System.Net.Mail;
global using System.Runtime.CompilerServices;
global using System.Security.Cryptography;
global using System.Text;
global using System.Text.Json;
global using System.Text.Json.Nodes;
global using Azure;
global using Azure.AI.OpenAI;
global using Azure.Storage.Blobs;
global using Azure.Storage.Blobs.Models;

global using Extensions;
global using Extensions.Memory;
global using FluentStorage;
//global using FluentStorage.Blobs;
global using FluentStorage.Azure.Blobs;
global using FluentStorage.Blobs;

global using Microsoft.AspNetCore.Mvc;
global using Microsoft.Data.SqlClient;
global using Microsoft.IO;

global using Newtonsoft.Json.Linq;
global using OpenAI;
global using OpenAI.Chat;
global using Serilog;
global using Serilog.Sinks.MSSqlServer;
global using Subscription.API;
global using Subscription.API.Code;
global using Subscription.Model;
global using Subscription.Model.Return;
global using Syncfusion.DocIO;
global using Syncfusion.DocIO.DLS;
global using Syncfusion.Pdf;
global using Syncfusion.Pdf.Parsing;

#endregion