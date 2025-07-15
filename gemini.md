# GEMINI.md

This file provides guidance to Gemini CLI when working with code in this repository.

## Project Overview

This is a .NET 9.0 enterprise subscription management system built with Blazor Server and ASP.NET Core Web API. The application manages candidates, companies, requisitions, and related business processes for a staffing/recruitment platform.

## Solution Architecture

The solution consists of 5 main projects:

- **Subscription.Server** - Main Blazor Server application (UI)
- **Subscription.API** - REST API backend with controllers
- **Subscription.Model** - Shared models, validators, and business logic
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
The WebAPI exists in Subscription.API.
In the Subscription.Server we use General.ExecuteRest<T> to connect to the API where the first parameter is the Endpoint to connect. It is the format "Candidate/SaveCandidate" which means CandidateController.cs and the second part of / is the endpoint method. Whenever we make any changes or analyze also analyze the endpoint for any clarifications.
If there are any code that uses General.PostRest or Generat.GetRest which are legacy code they should be suggested to be changed to General.ExecuteRest. Before that check the endpoint (as per rule defined above), whether the endpoint returns a ActionResult<T> or just T. Please evaluate if the decision to change to ActionResult<T> would be better in that situation.
Any changes we effect, please have a check across all the 5 projects or the dependencies to see how it will affect the entire flow.
If any breaking changes PLEASE HIGLIGHT IN UPPERCASE.
During changes comment code that needs to be removed instead of deleting. I will have them removed later during code cleanup.
For any statement adding or commenting/deleting, add a comment on top of the code block identifying what is being done and why.
Every endpoint connects to database to fetch the data, please refer to the MCP SQL Server Connection string in Development Memories to access the tables or stored procedures to understand more about the flow. But before ask if I can provide the Stored Procedure data so that time to connect and fetch can be reduced.

## Development Memories

### SQL Server Connection Testing
- Test the MCP connection using the following command:
  ```
  mcp__sqlserver__test_connection --connectionString "Server=192.168.1.3\SQL2022;Database=Subscription;User Id=sa;Password=Password$100;"
  ```

## Pending Implementation: Dynamic Validation Infrastructure

### Overview
The FluentValidation validators have commented-out server-side validation checks that need to be restored. This functionality validates data uniqueness by making API calls to check if values already exist in the database before allowing create/update operations.

### Current State (Partially Implemented)

#### ✅ **Infrastructure Already Exists:**
- **`IValidationApiService`** interface in `Subscription.Model/IValidationApiService.cs`
- **`ValidationApiService`** implementation in `Subscription.Server/Code/ValidationApiService.cs` 
- **Configuration Logic** for API host detection (localhost vs server) in `ValidationApiService.GetApiHost()`
- **11 Check Methods** already implemented covering major validation scenarios

#### ❌ **Missing Integration:**
- **Validator Integration**: Commented validation rules in validators not connected to service
- **Dependency Injection**: Service not registered in DI container  
- **API Endpoints**: Some validation endpoints may be missing from Controllers
- **Testing**: Validation service integration needs testing

### Implementation Requirements

#### **1. Commented Validation Examples Found:**

**AdminListValidator.cs** (Lines 55, 73-115):
```csharp
// COMMENTED OUT:
//.Must((obj, text) => CheckTextExists(text, obj.ID, obj.Entity, obj.Code))
//  .WithMessage(x => $"{x.Entity} already exists. Enter another {x.Entity}");

// COMMENTED METHODS:
private static bool CheckCodeExists(string code)
{
    RestClient _restClient = new(GeneralClass.ApiHost ?? "");
    RestRequest _request = new("Admin/CheckTaxTermCode");
    // ... API call to check tax term code uniqueness
}

private static bool CheckTextExists(string text, int id, string entity, string code)
{
    RestClient _restClient = new(GeneralClass.ApiHost ?? "");
    RestRequest _request = new("Admin/CheckText");
    // ... API call to check text uniqueness for entity
}
```

**CompanyDetailsValidator.cs** (Lines 81-84):
```csharp
// ACTIVE VALIDATION (needs DI conversion):
private static bool CheckEINExists(string ein, int companyID)
{
    using RestClient _client = new(GeneralModel.APIHost ?? "");
    RestRequest _request = new("Company/CheckEIN");
    // ... Currently using static API access
}
```

#### **2. API Endpoints Required:**

Based on `ValidationApiService.cs`, these endpoints should exist:

**Admin Controller:**
- `Admin/CheckText` - Check text uniqueness for entities
- `Admin/CheckTaxTermCode` - Check tax term code uniqueness
- `Admin/CheckJobCode` - Check job code uniqueness  
- `Admin/CheckJobOption` - Check job option uniqueness
- `Admin/CheckRoleID` - Check role ID uniqueness
- `Admin/CheckRole` - Check role name uniqueness
- `Admin/CheckStateCode` - Check state code uniqueness
- `Admin/CheckState` - Check state name uniqueness

**Company Controller:**
- `Company/CheckEIN` - Check EIN uniqueness (already referenced)

#### **3. Configuration Architecture:**

**Current API Host Detection** (in `ValidationApiService.GetApiHost()`):
```csharp
private string GetApiHost()
{
    // Logic to determine APIHost or APIHostServer from appsettings.json
    string apiHost = _configuration["APIHost"] ?? _configuration["APIHostServer"];
    if (string.IsNullOrEmpty(apiHost))
    {
        throw new InvalidOperationException("APIHost or APIHostServer is not configured.");
    }
    return apiHost;
}
```

**Required Enhancement**: The service needs localhost/127.0.0.1 detection logic:
```csharp
// NEEDS IMPLEMENTATION:
// If URL contains localhost/127.0.0.1 → use APIHost
// Otherwise → use APIHostServer
```

#### **4. Validator Integration Pattern:**

**Current Pattern** (static, commented):
```csharp
.Must((obj, text) => CheckTextExists(text, obj.ID, obj.Entity, obj.Code))
```

**Target Pattern** (DI-based, async):
```csharp
.MustAsync(async (obj, text, cancellation) => 
    await _validationApiService.CheckAdminListTextExistsAsync(text, obj.ID, obj.Entity, obj.Code))
```

### Implementation Steps

#### **Phase 1: Restore API Endpoints**
1. **Verify existing endpoints** in Controllers
2. **Restore missing endpoints** from backup
3. **Ensure all 11 validation endpoints** are functional

#### **Phase 2: Complete Service Integration**
1. **Register ValidationApiService** in DI container (`Program.cs`)
2. **Update API host detection** to include localhost/127.0.0.1 logic
3. **Add environment-specific configuration** reading

#### **Phase 3: Update Validators**
1. **Inject IValidationApiService** into validators requiring DI
2. **Convert static Must() calls** to async MustAsync() calls
3. **Uncomment and update validation rules** across all validators
4. **Replace static API calls** with service calls

#### **Phase 4: Testing and Validation**
1. **Test localhost detection** logic
2. **Test server environment** detection
3. **Verify all validation endpoints** respond correctly
4. **Test validator integration** with actual data

### Breaking Changes Considerations

**⚠️ POTENTIAL BREAKING CHANGES:**
- **Validator Constructors**: May need DI parameters for service injection
- **Async Validation**: Converting Must() to MustAsync() changes validation behavior
- **API Dependencies**: Validators will now require API availability for validation

### Files Requiring Updates

**Model Project:**
- `/Validators/*.cs` - Update validation rules to use service
- `/IValidationApiService.cs` - May need additional methods

**Server Project:**  
- `/Code/ValidationApiService.cs` - Enhance host detection logic
- `/Program.cs` - Register service in DI container

**API Project:**
- `/Controllers/AdminController.cs` - Ensure all Check* endpoints exist
- `/Controllers/CompanyController.cs` - Verify CheckEIN endpoint
- `/Controllers/*Controller.cs` - Add missing validation endpoints

### Success Criteria
- ✅ All commented validation rules restored and functional
- ✅ Environment-specific API host detection working
- ✅ Async validation integrated without performance issues  
- ✅ All validation endpoints responding correctly
- ✅ No breaking changes to existing validation behavior

### Implementation Timeline
**Recommended**: Implement after completing API and Server project reviews to understand:
- Complete endpoint inventory
- Existing DI patterns
- Configuration architecture
- Error handling strategies

**Documentation Updated**: 2025-07-12
**Status**: Awaiting API/Server project review completion