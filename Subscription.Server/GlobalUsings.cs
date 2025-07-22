#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.Server
// File Name:           GlobalUsings.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu
// Created On:          04-22-2024 15:04
// Last Updated On:     10-29-2024 15:10
// *****************************************/

#endregion

#region Using

global using System.IdentityModel.Tokens.Jwt;
global using System.IO.Compression;
global using System.Security.Claims;
global using System.Text.RegularExpressions;
global using Blazored.LocalStorage;
global using Blazored.SessionStorage;
global using Extensions;
global using FluentStorage;
global using FluentStorage.Azure.Blobs;
global using FluentStorage.Blobs;
global using Microsoft.AspNetCore.Components;
// global using GridAction = Syncfusion.Blazor.Grids.Action;
global using Microsoft.AspNetCore.Components.Forms;
global using Microsoft.AspNetCore.Components.Web;
global using Microsoft.AspNetCore.WebUtilities;
global using Microsoft.Extensions.Caching.Memory;
global using Microsoft.Extensions.Primitives;
global using Microsoft.JSInterop;
// Updated: Added System.Text.Json and Serilog for performance optimizations
global using System.Text.Json;
global using Serilog;
global using Newtonsoft.Json; // TODO: Remove after complete migration to System.Text.Json
//global using Microsoft.Extensions.Logging;
//global using Subscription.Server.Components.Pages.Controls.Common;
global using RestSharp;
global using StackExchange.Redis;
global using Subscription.Model;
global using Subscription.Model.Return;
global using Subscription.Model.Validators;
global using Subscription.Server.Code;
global using Subscription.Server.Components.Pages;
global using Subscription.Server.Components.Pages.Controls.Admin;
global using Subscription.Server.Components.Pages.Controls.Candidates;
global using Subscription.Server.Components.Pages.Controls.Common;
global using Subscription.Server.Components.Pages.Controls.Companies;
global using Subscription.Server.Components.Pages.Controls.Requisitions;
//global using Subscription.Server.Components.Pages;
global using Syncfusion.Blazor;
global using Syncfusion.Blazor.Buttons;
global using Syncfusion.Blazor.Cards;
global using Syncfusion.Blazor.Charts;
global using Syncfusion.Blazor.Data;
global using Syncfusion.Blazor.DataForm;
global using Syncfusion.Blazor.DropDowns;
global using Syncfusion.Blazor.Grids;
//global using Syncfusion.Blazor.DocumentEditor;
global using Syncfusion.Blazor.Inputs;
// global using Syncfusion.Blazor.Diagrams;
global using Syncfusion.Blazor.Navigations;
global using Syncfusion.Blazor.Popups;
global using Syncfusion.Blazor.RichTextEditor;
global using Syncfusion.Blazor.SfPdfViewer;
global using Syncfusion.Blazor.Spinner;
global using Syncfusion.DocIORenderer;
global using Syncfusion.Licensing;
global using Syncfusion.Pdf;
global using BeforeOpenEventArgs = Syncfusion.Blazor.Popups.BeforeOpenEventArgs;

//global using Syncfusion.Blazor.SfPdfViewer;
#endregion