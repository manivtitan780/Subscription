# .NET Solution Class Diagrams

**Generated:** 2025-07-28  
**Solution:** Subscription Management System  

## Table of Contents

1. [High-Level System Architecture](#high-level-system-architecture)
2. [Project Dependencies](#project-dependencies)
3. [Subscription.Server Class Diagram](#subscriptionserver-class-diagram)
4. [Subscription.API Class Diagram](#subscriptionapi-class-diagram)
5. [Subscription.Model Domain Structure](#subscriptionmodel-domain-structure)
6. [ExtendedComponents Library](#extendedcomponents-library)
7. [Extensions Utility Foundation](#extensions-utility-foundation)
8. [Entity Relationships](#entity-relationships)
9. [Service Dependencies](#service-dependencies)
10. [Data Flow Diagrams](#data-flow-diagrams)

## High-Level System Architecture

```mermaid
graph TB
    subgraph "Client Browser"
        UI[Blazor Server UI]
    end
    
    subgraph "Subscription.Server (.NET 9.0)"
        Pages[Razor Pages/Components]
        General_S[General.cs - HTTP Orchestration]
        ValidationSvc[ValidationApiService]
        Redis_S[RedisService Client]
    end
    
    subgraph "ExtendedComponents"
        CustomControls[Custom Blazor Controls]
        SyncfusionWrappers[Syncfusion Component Wrappers]
    end
    
    subgraph "Subscription.API (.NET 9.0)"
        Controllers[REST Controllers]
        General_A[General.cs - Data Access]
        Business[Business Logic]
    end
    
    subgraph "Subscription.Model (.NET 9.0)"
        Entities[Domain Entities]
        Validators[FluentValidation Rules]
        DTOs[Data Transfer Objects]
        Services[Domain Services]
    end
    
    subgraph "Extensions (.NET 9.0)"
        SqlExtensions[SQL Parameter Extensions]
        MemoryMgmt[Memory Management]
        Utilities[Type Conversions & Utilities]
    end
    
    subgraph "External Systems"
        DB[(SQL Server Database)]
        Redis[(Redis/Garnet Cache)]
        Azure[Azure Blob Storage]
        OpenAI[Azure OpenAI]
        Email[SMTP Email Service]
    end
    
    UI -->|SignalR| Pages
    Pages --> General_S
    Pages --> ValidationSvc
    Pages --> CustomControls
    General_S -->|HTTP REST| Controllers
    Controllers --> General_A
    Controllers --> Business
    General_A --> DB
    Controllers --> Redis
    Controllers --> Azure
    Controllers --> OpenAI
    Controllers --> Email
    Redis_S --> Redis
    
    Pages -.-> Entities
    Controllers -.-> Entities
    Business -.-> Validators
    General_A -.-> SqlExtensions
    General_A -.-> MemoryMgmt
    
    classDef serverLayer fill:#e1f5fe
    classDef apiLayer fill:#f3e5f5
    classDef modelLayer fill:#e8f5e8
    classDef extensionLayer fill:#fff3e0
    classDef external fill:#ffebee
    
    class Pages,General_S,ValidationSvc,Redis_S serverLayer
    class Controllers,General_A,Business apiLayer
    class Entities,Validators,DTOs,Services modelLayer
    class SqlExtensions,MemoryMgmt,Utilities extensionLayer
    class DB,Redis,Azure,OpenAI,Email external
```

## Project Dependencies

```mermaid
graph TD
    A[Subscription.Server<br/>Blazor Server UI<br/>.NET 9.0] --> B[ExtendedComponents<br/>Custom Blazor Components<br/>.NET 9.0]
    A --> C[Extensions<br/>Utilities & Memory Mgmt<br/>.NET 9.0]
    A --> D[Subscription.Model<br/>Business Logic & Models<br/>.NET 9.0]
    
    E[Subscription.API<br/>REST API Backend<br/>.NET 9.0] --> C
    E --> D
    
    D --> C
    
    classDef server fill:#e1f5fe,stroke:#01579b,stroke-width:2px
    classDef api fill:#f3e5f5,stroke:#4a148c,stroke-width:2px
    classDef model fill:#e8f5e8,stroke:#1b5e20,stroke-width:2px
    classDef components fill:#fff8e1,stroke:#e65100,stroke-width:2px
    classDef extensions fill:#fce4ec,stroke:#880e4f,stroke-width:2px
    
    class A server
    class E api
    class D model
    class B components
    class C extensions
```

## Subscription.Server Class Diagram

```mermaid
classDiagram
    class Program {
        +Main(string[] args) void
        +ConfigureServices() void
        +ConfigureApp() void
    }
    
    class General {
        +ExecuteRest~T~(string endpoint, Dictionary parameters) Task~T~
        +ExecuteAndDeserialize~T~(string endpoint, Dictionary parameters) Task~T~
        +DeserializeObject~T~(string json) T
        +GetClaimsToken(LocalStorage localStorage, SessionStorage sessionStorage) Task~IEnumerable~Claim~~
        +DialogOptions(string message) DialogSettings
        +ExecuteMethod(SemaphoreSlim semaphore, Func task) Task
    }
    
    class ValidationApiService {
        -IConfiguration _configuration
        -HttpClient _httpClient
        +CheckAdminListTextExistsAsync(string text, int id, string entity, string code) Task~bool~
        +CheckCompanyEINExistsAsync(string ein, int companyId) Task~bool~
        +CheckTaxTermCodeExistsAsync(string code) Task~bool~
        +GetApiHost() string
    }
    
    class Start {
        +APIHost string
        +UploadsPath string
        +ConfigureApplication() void
    }
    
    class Candidates {
        -CandidateDetails _candDetailsObject
        -CandidateNotes[] _candidateNotesObject
        -SubmissionTimeline[] _timelineActivityObject
        -SubmissionTimeline[] _timelineObject
        -SemaphoreSlim _semaphoreMainPage
        +EditCandidate() Task
        +SaveCandidate() Task
        +DeleteNotes(int id) Task
        +TimeLine(int requisitionID) Task
        +Dispose() void
    }
    
    class Companies {
        -CompanyDetails _companyDetails
        -CompanyContacts[] _companyContacts
        -CandidateNotes[] _companyNotesObject
        -SemaphoreSlim _semaphoreMainPage
        +EditCompany(bool isAdd) Task
        +SaveCompany() Task
        +EditNotes(int id) Task
        +DeleteNotes(int id) Task
        +SetupAddress(bool useLocation) void
    }
    
    class TimelineDialog {
        +SubmissionTimeline[] Model
        +EventCallback~MouseEventArgs~ Cancel
        -SfDialog Dialog
        -bool VisibleSpinner
        +ShowDialog() Task
        +CloseDialog(MouseEventArgs args) Task
        +DismissDialog() Task
        +Dispose() void
    }
    
    class Login {
        -LoginModel _loginModel
        -bool _loginSpinner
        +ValidateLogin() Task
        +ForgotPassword() Task
        +CreateCookie(LoginCooky loginCooky) Task
    }
    
    class Dash {
        -DashboardData _dashboardData
        -bool _isLoading
        +LoadDashboardData() Task
        +RefreshData() Task
    }
    
    Program --> General
    Program --> Start
    Candidates --> General
    Candidates --> ValidationApiService
    Candidates --> TimelineDialog
    Companies --> General
    Companies --> TimelineDialog
    Login --> General
    Dash --> General
```

## Subscription.API Class Diagram

```mermaid
classDiagram
    class CandidateController {
        +GetGridCandidates(CandidateSearch search) ActionResult~ReturnGrid~
        +GetCandidateDetails(int candidateID, string roleID) ActionResult~ReturnCandidateDetails~
        +SaveCandidate(CandidateDetails candidate, string userName) ActionResult~int~
        +DeleteCandidate(int candidateID, string user) ActionResult~bool~
        +UploadDocument(IFormFile file, CandidateDocument document) ActionResult~string~
        +SaveNotes(CandidateNotes notes, int candidateID, string user) ActionResult~string~
        +DeleteNotes(int id, int candidateID, string user) ActionResult~string~
    }
    
    class CompanyController {
        +SearchCompanies(CompanySearch search) ActionResult~ReturnGrid~
        +GetCompanyDetails(int companyID, string user) ActionResult~ReturnCompanyDetails~
        +SaveCompany(CompanyDetails company, string user) ActionResult~ReturnCompanyDetails~
        +SaveNotes(CandidateNotes notes, int companyID, string user) ActionResult~string~
        +DeleteNotes(int id, int companyID, string user) ActionResult~string~
        +CheckEIN(string ein, int companyID) ActionResult~bool~
    }
    
    class RequisitionController {
        +GetGridRequisitions(RequisitionSearch search) ActionResult~ReturnGridRequisition~
        +GetRequisitionDetails(int requisitionID, string user) ActionResult~ReturnRequisitionDetails~
        +SaveRequisition(RequisitionDetails requisition, string user) ActionResult~ReturnRequisitionDetails~
        +SubmitCandidate(SubmitCandidateRequisition submission) ActionResult~bool~
        +ChangeStatus(int requisitionID, string status, string user) ActionResult~bool~
    }
    
    class AdminController {
        +GetAdminList(string entity, int page, int pageSize) ActionResult~ReturnGrid~
        +SaveAdminList(AdminList adminList, string user) ActionResult~string~
        +DeleteAdminList(int id, string entity, string user) ActionResult~string~
        +CheckText(string text, int id, string entity, string code) ActionResult~bool~
        +CheckTaxTermCode(string code) ActionResult~bool~
        +CheckJobCode(string code) ActionResult~bool~
    }
    
    class DashboardController {
        +GetDashboardData(string user, string role) ActionResult~ReturnDashboard~
        +GetMetrics(DateTime startDate, DateTime endDate) ActionResult~ConsolidatedMetrics~
        +GetRecentActivity(string user, int count) ActionResult~RecentActivity[]~
    }
    
    class LoginController {
        +ValidateLogin(LoginModel login) ActionResult~LoginCooky~
        +ForgotPassword(string email) ActionResult~bool~
        +ChangePassword(string user, string oldPassword, string newPassword) ActionResult~bool~
    }
    
    class General_API {
        +ExecuteSQL(string storedProcedure, Dictionary parameters) Task~DataTable~
        +ExecuteSQLWithParameters~T~(string storedProcedure, Dictionary parameters) Task~T~
        +UploadToBlob(byte[] file, string blobPath) Task~bool~
        +ExtractTextFromPdf(byte[] file) string
        +ExtractTextFromWord(byte[] file) string
        +SendEmail(string to, string subject, string body, byte[] attachment) Task~bool~
        +LoadZipCodes() Task~List~Zip~~
        +GetConnectionString() string
    }
    
    class PasswordHasher {
        +HashPassword(string password) string
        +VerifyPassword(string password, string hash) bool
        +GenerateSalt() string
    }
    
    CandidateController --> General_API
    CompanyController --> General_API
    RequisitionController --> General_API
    AdminController --> General_API
    DashboardController --> General_API
    LoginController --> General_API
    LoginController --> PasswordHasher
```

## Subscription.Model Domain Structure

```mermaid
classDiagram
    class Candidate {
        +int ID
        +string Name
        +string Email
        +string Phone
        +string Location
        +string Status
        +string Owner
        +string Updated
        +int Rating
        +bool MPC
        +bool FormattedResume
        +bool OriginalResume
    }
    
    class CandidateDetails {
        +int ID
        +string FirstName
        +string LastName
        +string Email
        +string Phone1
        +string Phone2
        +string Address1
        +string Address2
        +string City
        +int StateID
        +string ZipCode
        +string TextResume
        +DateTime CreatedDate
        +string CreatedBy
        +DateTime UpdatedDate
        +string UpdatedBy
        +bool IsAdd
        +Copy() CandidateDetails
        +Clear() void
    }
    
    class Company {
        +int ID
        +string CompanyName
        +string Email
        +string Phone
        +string Address
        +string Website
        +bool Status
        +DateTime UpdatedDate
        +string UpdatedBy
    }
    
    class CompanyDetails {
        +int ID
        +string Name
        +string EmailAddress
        +string Phone
        +string StreetName
        +string City
        +int StateID
        +string ZipCode
        +string Website
        +bool Status
        +DateTime CreatedDate
        +string CreatedBy
        +DateTime UpdatedDate
        +string UpdatedBy
        +bool IsAdd
        +Copy() CompanyDetails
        +Clear() void
    }
    
    class SubmissionTimeline {
        +int Id
        +int RequisitionId
        +int CandidateId
        +string Status
        +string StatusName
        +string Notes
        +string CreatedBy
        +DateTime CreatedDate
        +string CandidateName
        +string RequisitionName
        +DateTime? InterviewDateTime
        +string PhoneNumber
        +string InterviewDetails
        +bool IsInterview
        +string FormattedDate
        +string FormattedInterviewDate
    }
    
    class CandidateNotes {
        +int ID
        +int CandidateID
        +string Notes
        +DateTime CreatedDate
        +string CreatedBy
        +DateTime UpdatedDate
        +string UpdatedBy
        +Copy() CandidateNotes
        +Clear() void
    }
    
    class CandidateEducation {
        +int ID
        +int CandidateID
        +string Degree
        +string Institution
        +string Major
        +DateTime GraduationDate
        +decimal GPA
        +Copy() CandidateEducation
        +Clear() void
    }
    
    class CandidateSkills {
        +int ID
        +int CandidateID
        +string Skill
        +int ExpMonths
        +DateTime LastUsed
        +Copy() CandidateSkills
        +Clear() void
    }
    
    class RedisService {
        -IConnectionMultiplexer _redis
        -IDatabase _database
        +GetAsync(string key) Task~string~
        +SetAsync(string key, string value, TimeSpan? expiry) Task~bool~
        +DeleteAsync(string key) Task~bool~
        +BatchGet(string[] keys) Task~Dictionary~string,string~~
        +BatchSet(Dictionary values) Task~bool~
        +Dispose() void
    }
    
    class IValidationApiService {
        +CheckAdminListTextExistsAsync(string text, int id, string entity, string code) Task~bool~
        +CheckCompanyEINExistsAsync(string ein, int companyId) Task~bool~
        +CheckTaxTermCodeExistsAsync(string code) Task~bool~
        +CheckJobCodeExistsAsync(string code) Task~bool~
        +CheckRoleIDExistsAsync(int roleId) Task~bool~
        +CheckStateCodeExistsAsync(string code) Task~bool~
    }
    
    Candidate ||--o{ CandidateDetails : has
    Candidate ||--o{ CandidateNotes : has
    Candidate ||--o{ CandidateEducation : has
    Candidate ||--o{ CandidateSkills : has
    Candidate ||--o{ SubmissionTimeline : submits_to
    Company ||--o{ CompanyDetails : has
    Company ||--o{ SubmissionTimeline : receives
```

## ExtendedComponents Library

```mermaid
classDiagram
    class TextBox {
        +string Value
        +string Placeholder
        +bool Required
        +bool Disabled
        +string CssClass
        +EventCallback~string~ ValueChanged
        +EventCallback~FocusEventArgs~ OnFocus
        +EventCallback~FocusEventArgs~ OnBlur
        +Focus() Task
        +Blur() Task
    }
    
    class TextArea {
        +string Value
        +string Placeholder
        +int Rows
        +int MaxLength
        +bool Required
        +bool Disabled
        +string CssClass
        +EventCallback~string~ ValueChanged
        +Focus() Task
    }
    
    class DropDown {
        +object DataSource
        +string TextField
        +string ValueField
        +object Value
        +string Placeholder
        +bool AllowFiltering
        +bool Required
        +string CssClass
        +EventCallback~ChangeEventArgs~ ValueChanged
        +EventCallback~object~ OnSelect
        +Refresh() Task
    }
    
    class MultiSelect {
        +object DataSource
        +string TextField
        +string ValueField
        +object[] Value
        +string Placeholder
        +string Mode
        +bool ShowSelectAll
        +string CssClass
        +EventCallback~MultiSelectChangeEventArgs~ ValueChanged
        +SelectAll() Task
        +ClearAll() Task
    }
    
    class Upload {
        +string[] AllowedExtensions
        +long MaxFileSize
        +bool Multiple
        +bool AutoUpload
        +string UploadUrl
        +Dictionary~string,object~ AsyncSettings
        +EventCallback~UploadingEventArgs~ OnFileSelected
        +EventCallback~SuccessEventArgs~ OnUploadComplete
        +EventCallback~FailureEventArgs~ OnUploadFailed
        +Upload() Task
        +Remove() Task
    }
    
    class MaskedTextBox {
        +string Mask
        +string Value
        +string Placeholder
        +char PromptChar
        +bool Required
        +string CssClass
        +EventCallback~MaskChangeEventArgs~ ValueChanged
        +EventCallback~FocusEventArgs~ OnFocus
        +GetMaskedValue() string
        +GetUnmaskedValue() string
    }
    
    class NumericTextBox {
        +decimal? Value
        +decimal Min
        +decimal Max
        +int Decimals
        +string Format
        +decimal Step
        +bool Required
        +string CssClass
        +EventCallback~NumericChangeEventArgs~ ValueChanged
        +Increment() Task
        +Decrement() Task
    }
```

## Extensions Utility Foundation

```mermaid
classDiagram
    class Extensions_Add {
        +AddWithValue(SqlCommand cmd, string paramName, object value) SqlCommand
        +AddBooleanParameter(SqlCommand cmd, string paramName, bool value) SqlCommand
        +AddDateTimeParameter(SqlCommand cmd, string paramName, DateTime value) SqlCommand
        +AddIntParameter(SqlCommand cmd, string paramName, int value) SqlCommand
        +AddStringParameter(SqlCommand cmd, string paramName, string value) SqlCommand
        +AddDecimalParameter(SqlCommand cmd, string paramName, decimal value) SqlCommand
    }
    
    class Extensions_To {
        +ToInt32(string value) int
        +ToInt32(object value) int
        +ToDateTime(string value) DateTime
        +ToDateTime(object value) DateTime
        +ToDecimal(string value) decimal
        +ToBool(string value) bool
        +ToMarkupString(string value) MarkupString
        +CultureDate(DateTime date) string
        +FormatPhoneNumber(string phone) string
        +StripPhoneNumber(string phone) string
        +StripAndFormatPhoneNumber(string phone) string
        +ToBase64String(byte[] bytes) string
        +FromBase64String(string base64) byte[]
    }
    
    class Extensions_Streams {
        +ToStreamByteArray(MemoryStream stream) byte[]
        +ToStream(byte[] bytes) MemoryStream
        +CopyToAsync(Stream source, Stream destination) Task
        +ReadAllBytesAsync(Stream stream) Task~byte[]~
    }
    
    class ReusableMemoryStream {
        -byte[] _buffer
        -int _position
        -int _length
        -bool _disposed
        +Write(byte[] buffer, int offset, int count) void
        +Read(byte[] buffer, int offset, int count) int
        +Seek(long offset, SeekOrigin origin) long
        +SetLength(long value) void
        +ToArray() byte[]
        +Reset() void
        +Dispose() void
    }
    
    class MemoryStreamManager {
        -ObjectPool~ReusableMemoryStream~ _pool
        -int _maxRetained
        -int _initialCapacity
        +GetStream() ReusableMemoryStream
        +ReturnStream(ReusableMemoryStream stream) void
        +GetStream(int initialCapacity) ReusableMemoryStream
        +Dispose() void
    }
    
    class Base64Helper {
        +Encode(byte[] bytes) string
        +Encode(string text) string
        +Decode(string base64) byte[]
        +DecodeToString(string base64) string
        +IsValidBase64(string base64) bool
    }
    
    Extensions_Add --> SqlCommand
    Extensions_To --> MarkupString
    Extensions_Streams --> MemoryStream
    MemoryStreamManager --> ReusableMemoryStream
    MemoryStreamManager --> ObjectPool
```

## Entity Relationships

```mermaid
erDiagram
    CANDIDATE ||--o{ CANDIDATE_DETAILS : has
    CANDIDATE ||--o{ CANDIDATE_EDUCATION : has
    CANDIDATE ||--o{ CANDIDATE_EXPERIENCE : has
    CANDIDATE ||--o{ CANDIDATE_SKILLS : has
    CANDIDATE ||--o{ CANDIDATE_DOCUMENTS : has
    CANDIDATE ||--o{ CANDIDATE_NOTES : has
    CANDIDATE ||--o{ CANDIDATE_RATING : has
    CANDIDATE ||--o{ CANDIDATE_MPC : has
    CANDIDATE ||--o{ SUBMISSION_TIMELINE : submits_to
    
    COMPANY ||--o{ COMPANY_DETAILS : has
    COMPANY ||--o{ COMPANY_CONTACTS : has
    COMPANY ||--o{ COMPANY_LOCATIONS : has
    COMPANY ||--o{ COMPANY_DOCUMENTS : has
    COMPANY ||--o{ REQUISITION_DETAILS : posts
    
    REQUISITION_DETAILS ||--o{ REQUISITION_DOCUMENTS : has
    REQUISITION_DETAILS ||--o{ SUBMISSION_TIMELINE : receives
    
    SUBMISSION_TIMELINE }o--|| STATUS_CODE : references
    SUBMISSION_TIMELINE }o--|| CANDIDATE : references
    SUBMISSION_TIMELINE }o--|| REQUISITION_DETAILS : references
    
    USER ||--o{ CANDIDATE_NOTES : creates
    USER ||--o{ CANDIDATE_RATING : creates
    USER ||--o{ SUBMISSION_TIMELINE : creates
    USER ||--o{ COMPANY_DOCUMENTS : uploads
    USER ||--o{ CANDIDATE_DOCUMENTS : uploads
    
    STATE_CACHE ||--o{ CANDIDATE_DETAILS : location
    STATE_CACHE ||--o{ COMPANY_DETAILS : location
    STATE_CACHE ||--o{ COMPANY_LOCATIONS : location
    
    CANDIDATE {
        int ID PK
        string Name
        string Email
        string Phone
        string Location
        string Status
        string Owner
        datetime Updated
        int Rating
        bool MPC
        bool FormattedResume
        bool OriginalResume
    }
    
    CANDIDATE_DETAILS {
        int ID PK
        int CandidateID FK
        string FirstName
        string LastName
        string Email
        string Phone1
        string Phone2
        string Address1
        string Address2
        string City
        int StateID FK
        string ZipCode
        text TextResume
        datetime CreatedDate
        string CreatedBy
        datetime UpdatedDate
        string UpdatedBy
        bool IsAdd
    }
    
    SUBMISSION_TIMELINE {
        int Id PK
        int RequisitionId FK
        int CandidateId FK
        string Status
        string StatusName
        varchar Notes
        string CreatedBy
        datetime CreatedDate
        string CandidateName
        string RequisitionName
        datetime InterviewDateTime
        varchar PhoneNumber
        varchar InterviewDetails
        bool Undone
    }
    
    COMPANY {
        int ID PK
        string CompanyName
        string Email
        string Phone
        string Address
        string Website
        bool Status
        datetime UpdatedDate
        string UpdatedBy
    }
    
    COMPANY_DETAILS {
        int ID PK
        int CompanyID FK
        string Name
        string EmailAddress
        string Phone
        string StreetName
        string City
        int StateID FK
        string ZipCode
        string Website
        bool Status
        datetime CreatedDate
        string CreatedBy
        datetime UpdatedDate
        string UpdatedBy
    }
```

## Service Dependencies

```mermaid
classDiagram
    class Controllers {
        +GetData() ActionResult
        +SaveData() ActionResult
        +DeleteData() ActionResult
    }
    
    class General_API {
        -IConfiguration _configuration
        -string _connectionString
        +ExecuteSQL(string storedProcedure, Dictionary parameters) Task~DataTable~
        +ExecuteSQLWithParameters~T~(string storedProcedure, Dictionary parameters) Task~T~
        +UploadToBlob(byte[] file, string blobPath) Task~bool~
        +ExtractTextFromPdf(byte[] file) string
        +ExtractTextFromWord(byte[] file) string
        +SendEmail(string to, string subject, string body, byte[] attachment) Task~bool~
        +GetConnectionString() string
    }
    
    class RedisService {
        -IConnectionMultiplexer _redis
        -IDatabase _database
        -string _connectionString
        +GetAsync(string key) Task~string~
        +SetAsync(string key, string value, TimeSpan? expiry) Task~bool~
        +BatchGet(string[] keys) Task~Dictionary~string,string~~
        +BatchSet(Dictionary values) Task~bool~
        +FlushDatabase() Task
        +Dispose() void
    }
    
    class SqlExtensions {
        +AddWithValue(SqlCommand cmd, string paramName, object value) SqlCommand
        +AddBooleanParameter(SqlCommand cmd, string paramName, bool value) SqlCommand
        +AddDateTimeParameter(SqlCommand cmd, string paramName, DateTime value) SqlCommand
        +AddIntParameter(SqlCommand cmd, string paramName, int value) SqlCommand
        +AddStringParameter(SqlCommand cmd, string paramName, string value) SqlCommand
    }
    
    class ReusableMemoryStream {
        -byte[] _buffer
        -int _position
        -int _length
        +Write(byte[] buffer, int offset, int count) void
        +Read(byte[] buffer, int offset, int count) int
        +ToArray() byte[]
        +Reset() void
        +Dispose() void
    }
    
    class MemoryStreamManager {
        -ObjectPool~ReusableMemoryStream~ _pool
        +GetStream() ReusableMemoryStream
        +ReturnStream(ReusableMemoryStream stream) void
        +Dispose() void
    }
    
    class AzureOpenAIClient {
        -string _endpoint
        -AzureKeyCredential _credential
        +GetChatCompletionsAsync(ChatCompletionsOptions options) Task~Response~ChatCompletions~~
        +ExtractTextFromDocument(byte[] document) Task~string~
    }
    
    class SmtpClient {
        -string _host
        -int _port
        -NetworkCredential _credentials
        +SendMailAsync(MailMessage message) Task
        +Dispose() void
    }
    
    Controllers --> General_API
    General_API --> RedisService
    General_API --> SqlExtensions
    General_API --> ReusableMemoryStream
    General_API --> MemoryStreamManager
    General_API --> AzureOpenAIClient
    General_API --> SmtpClient
    MemoryStreamManager --> ReusableMemoryStream
```

## Data Flow Diagrams

### Request Processing Flow

```mermaid
sequenceDiagram
    participant UI as Blazor Server
    participant API as REST API
    participant Cache as Redis Cache
    participant DB as SQL Server
    participant Blob as Azure Blob
    
    UI->>+API: HTTP REST Request
    Note over UI,API: General.ExecuteRest<T>()
    
    API->>+Cache: Check Cache
    Note over API,Cache: RedisService.GetAsync()
    
    alt Cache Hit
        Cache-->>-API: Cached Data
    else Cache Miss
        API->>+DB: Execute SQL
        Note over API,DB: General.ExecuteSQL()
        DB-->>-API: Raw Data
        API->>Cache: Store in Cache
        Note over API,Cache: RedisService.SetAsync()
    end
    
    opt File Operation
        API->>+Blob: Upload/Download File
        Note over API,Blob: Azure Blob Storage
        Blob-->>-API: File URL/Data
    end
    
    API-->>-UI: JSON Response
    Note over API,UI: System.Text.Json
    
    UI->>UI: Update Components
    Note over UI: SignalR Real-time Updates
```

### Memory Management Flow

```mermaid
sequenceDiagram
    participant API as API Controller
    participant GM as General_API
    participant MSM as MemoryStreamManager
    participant RMS as ReusableMemoryStream
    participant Pool as ObjectPool
    
    API->>+GM: Process File Upload
    GM->>+MSM: GetStream()
    MSM->>+Pool: Get()
    Pool-->>-MSM: ReusableMemoryStream
    MSM-->>-GM: Stream Instance
    
    GM->>+RMS: Write(bytes)
    RMS-->>-GM: Success
    
    GM->>+RMS: ToArray()
    RMS-->>-GM: byte[]
    
    GM->>+MSM: ReturnStream(stream)
    MSM->>+RMS: Reset()
    RMS-->>-MSM: Reset Complete
    MSM->>+Pool: Return(stream)
    Pool-->>-MSM: Returned to Pool
    MSM-->>-GM: Success
    
    GM-->>-API: Processing Complete
```

### Validation Flow

```mermaid
sequenceDiagram
    participant UI as Blazor Component
    participant Form as EditContext
    participant FV as FluentValidator
    participant VAS as ValidationApiService
    participant API as Validation API
    
    UI->>+Form: Submit Form
    Form->>+FV: Validate()
    
    FV->>FV: Basic Validation Rules
    
    opt Server-side Validation
        FV->>+VAS: CheckUniqueAsync()
        VAS->>+API: HTTP Request
        API-->>-VAS: Validation Result
        VAS-->>-FV: IsUnique
    end
    
    FV-->>-Form: ValidationResult
    Form-->>-UI: IsValid / Errors
    
    alt Valid
        UI->>API: Save Data
    else Invalid
        UI->>UI: Display Errors
    end
```

---

**Generated:** 2025-07-28  
**File Type:** Mermaid Class Diagrams  
**Total Diagrams:** 12 comprehensive diagrams  
**Coverage:** Complete solution architecture  