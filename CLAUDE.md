# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

This is a .NET 9.0 enterprise subscription management system built with Blazor Server and ASP.NET Core Web API. The application manages candidates, companies, requisitions, and related business processes for a staffing/recruitment platform.

## Solution Architecture

The solution consists of 5 main projects:

- **Subscription.Server** - Main Blazor Server application (UI)
- **Subscription.API** - REST API backend with controllers
- **Subscription.Model** - Shared models, validators, and business logic
- **Subscription.Client** - Blazor WebAssembly client (appears to be legacy)
- **ExtendedComponents** - Custom Blazor components library
- **Extensions** - Utility extensions and helpers

## Key Technologies

- .NET 9.0 (Server/API) and .NET 8.0 (Client)
- Blazor Server with SignalR
- ASP.NET Core Web API
- Syncfusion Blazor components (version 30.1.38)
- Entity Framework Core (implied by SQL operations)
- Redis/Garnet caching
- Azure Blob Storage for file uploads
- Azure OpenAI integration
- FluentValidation
- Serilog for logging
- SQL Server database

## Build and Development Commands

```bash
# Build entire solution
dotnet build Subscription.sln

# Build specific projects
dotnet build Subscription.Server/Subscription.Server.csproj
dotnet build Subscription.API/Subscription.API.csproj

# Run the main Blazor Server application
dotnet run --project Subscription.Server

# Run the API backend
dotnet run --project Subscription.API

# Build for specific configurations
dotnet build --configuration Debug
dotnet build --configuration Release
dotnet build --configuration India
dotnet build --configuration US
```

## Development Configuration

The application uses multiple configuration profiles:
- **Debug/Release** - Standard build configurations
- **India/US** - Region-specific configurations
- **Local/Server** - Environment-specific settings

Configuration is loaded from `appsettings.json` files in each project, with environment-specific overrides.

## Database and Caching

- **Database**: SQL Server with connection strings in appsettings
- **Caching**: Redis/Garnet cache server
- **File Storage**: Azure Blob Storage for documents and uploads
- **Local Uploads**: Stored in `wwwroot/uploads/` directory structure

## Key Application Features

### Core Entities
- **Candidates**: Resume parsing, skills, education, experience tracking
- **Companies**: Company management with contacts and locations
- **Requisitions**: Job postings and candidate submissions
- **Administration**: User management, roles, document types, workflows

### File Management
- Document upload and storage (Azure Blob + local filesystem)
- Resume parsing with AI integration
- PDF viewing capabilities
- Supported file types controlled by `AllowedExtensions` config

### UI Architecture
- Syncfusion Blazor components for grids, forms, and dialogs
- Custom component library in `ExtendedComponents`
- Responsive layouts with multiple layout templates
- Real-time updates via SignalR

## Important Notes

### IIS Integration
The project files include pre/post build events for IIS app pool management:
- Stops app pools before build
- Starts app pools after build
- Targets "subscription AppPool 2" and "subscriptionapi AppPool"

### Security
- FluentValidation for model validation
- Global usings defined in `GlobalUsings.cs`
- Nullable reference types disabled project-wide
- Authentication and authorization implemented

### Performance
- Response compression (Brotli/Gzip)
- Memory caching
- Redis distributed caching
- Optimized build configurations

## Common Development Patterns

### Controllers
Located in `Subscription.API/Controllers/` - include Admin, Candidate, Company, Dashboard, Login, and Requisition controllers.

### Razor Components
Organized in `Components/Pages/` with separate code-behind files (`.razor.cs`)
Control components organized in `Components/Pages/Controls/` by feature area.

### Models and Validation
All models in `Subscription.Model/` with corresponding FluentValidation validators in `Validators/` subfolder.

### File Uploads
Handled through dedicated upload directories organized by entity type (Candidate, Company, Requisition).

### Instructions
Analyze any code as instructed only for Memory Optimizations and Leaks, GC Pressures and improvements and general performance optimizations. Refactor should be suggested only if the code maintenance significantly improves. Minor suggestions should not be made. 
Always discuss any changes and only after approval should code changes be effected, and one file at a time.
Don't output information on screen about Reading files or internal system prompts. Just display your findings, expected changes and other relevant information for implementation.

## Development Memories

### SQL Server Connection Testing
- Test the MCP connection using the following command:
  ```
  mcp__sqlserver__test_connection --connectionString "Server=192.168.80.1\SQL2022;Database=Subscription;User Id=sa;Password=Password$100;"
  ```