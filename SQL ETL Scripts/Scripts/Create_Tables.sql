CREATE TABLE [dbo].[AuditTrail](
	[Id] [int] IDENTITY(1,1) NOT NULL,[Action] [varchar](100) NULL,[Page] [varchar](30) NULL,[Description] [varchar](7000) NULL,[InitiatedBy] [varchar](10) NULL,[InitiatedOn] [datetime] NOT NULL
) ON [PRIMARY]

CREATE TABLE [dbo].[Candidate](
	[ID] [int] IDENTITY(1,1) NOT NULL,[FirstName] [varchar](50) NULL,[LastName] [varchar](50) NULL,[MiddleName] [varchar](50) NULL,[Title] [varchar](200) NULL,[Address1] [varchar](255) NULL,[Address2] [varchar](255) NULL,[City] [varchar](50) NULL,[StateId] [int] NULL,[ZipCode] [varchar](20) NULL,[Email] [varchar](255) NULL,[Phone1] [varchar](15) NULL,[Phone2] [varchar](15) NULL,[Phone3] [varchar](15) NULL,[Phone3Ext] [int] NULL,[EligibilityId] [int] NOT NULL,[ExperienceId] [int] NOT NULL,[Experience] [int] NULL,[JobOptions] [varchar](50) NULL,[Communication] [char](1) NOT NULL,[TaxTerm] [varchar](30) NULL,[SalaryLow] [numeric](9, 2) NULL,[SalaryHigh] [numeric](9, 2) NULL,[HourlyRate] [numeric](6, 2) NULL,[HourlyRateHigh] [numeric](6, 2) NULL,[VendorId] [int] NULL,[Relocate] [bit] NOT NULL,[RelocNotes] [varchar](200) NULL,[Background] [bit] NOT NULL,[SecurityClearanceNotes] [varchar](200) NULL,[Keywords] [varchar](500) NULL,[Summary] [varchar](max) NOT NULL,[ExperienceSummary] [varchar](max) NOT NULL,[Objective] [varchar](max) NOT NULL,[RateCandidate] [tinyint] NOT NULL,[RateNotes] [varchar](max) NULL,[MPC] [bit] NOT NULL,[MPCNotes] [varchar](max) NULL,[Status] [char](3) NOT NULL,[TextResume] [varchar](max) NULL,[OriginalResume] [varchar](255) NULL,[FormattedResume] [varchar](255) NULL,[OriginalFileId] [varchar](50) NULL,[FormattedFileId] [varchar](50) NULL,[LinkedIn] [varchar](255) NULL,[Facebook] [varchar](255) NULL,[Twitter] [varchar](255) NULL,[Google] [varchar](255) NULL,[ReferAccountMgr] [varchar](10) NULL,[Refer] [bit] NOT NULL,[EEO] [bit] NOT NULL,[ParsedXML] [xml] NULL,[JsonFileName] [varchar](255) NULL,[CreatedBy] [varchar](10) NULL,[CreatedDate] [smalldatetime] NOT NULL,[UpdatedBy] [varchar](10) NULL,[UpdatedDate] [smalldatetime] NOT NULL,
 CONSTRAINT [PK_CONSULTANT_p] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PS_Candidate]([ID])
) ON [PS_Candidate]([ID])
GO

CREATE TABLE [dbo].[CandidateDocument](
	[CandidateDocId] [int] IDENTITY(1,1) NOT NULL,[CandidateId] [int] NULL,[DocumentName] [varchar](255) NULL,[DocumentLocation] [varchar](255) NULL,[DocumentType] [int] NOT NULL,[Notes] [varchar](2000) NULL,[InternalFileName] [varchar](50) NULL,[LastUpdatedDate] [datetime] NULL,[LastUpdatedBy] [varchar](10) NULL,
 CONSTRAINT [PK_CandidateDocument] PRIMARY KEY CLUSTERED 
(
	[CandidateDocId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[CandidateEducation](
	[Id] [int] IDENTITY(1,1) NOT NULL,[CandidateId] [int] NOT NULL,[Degree] [varchar](100) NULL,[College] [varchar](255) NULL,[State] [varchar](100) NULL,[Country] [varchar](100) NULL,[Year] [varchar](50) NULL,[CreatedDate] [datetime] NOT NULL,[CreatedBy] [varchar](10) NULL,[UpdatedDate] [datetime] NOT NULL,[UpdatedBy] [varchar](10) NULL,
 CONSTRAINT [PK_CandidateEducation] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[CandidateEmployer](
	[Id] [int] IDENTITY(1,1) NOT NULL,[CandidateId] [int] NOT NULL,[Employer] [varchar](100) NULL,[Start] [varchar](10) NULL,[End] [varchar](10) NULL,[Location] [varchar](100) NULL,[Title] [varchar](255) NULL,[Description] [varchar](max) NULL,[CreatedDate] [datetime] NOT NULL,[CreatedBy] [varchar](10) NULL,[UpdatedDate] [datetime] NOT NULL,[UpdatedBy] [varchar](10) NULL,
 CONSTRAINT [PK_CandidateEmployer] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

CREATE TABLE [dbo].[CandidateView]
(
	[ID] [int] NOT NULL,[MPC] [bit] NULL,[UpdatedDate] [smalldatetime] NULL,[FirstName] [varchar](50) NULL,[LastName] [varchar](50) NULL,[Phone1] [varchar](15) NULL,[Email] [varchar](255) NULL,[City] [varchar](50) NULL,[Code] [varchar](2) NULL,[UpdatedBy] [varchar](10) NULL,[Status] [char](3) NULL,[RateCandidate] [tinyint] NULL,[Keywords] [varchar](500) NULL,[ZipCode] [varchar](10) NULL,[StateID] [tinyint] NULL,[EligibilityID] [int] NULL,[Relocate] [bit] NULL,[JobOptions] [varchar](50) NULL,[Communication] [char](1) NULL,[Background] [bit] NULL,[FullName]  AS (ltrim(rtrim((isnull([FirstName],'')+case when [FirstName] IS NOT NULL AND [LastName] IS NOT NULL then ' ' else '' end)+isnull([LastName],'')))) PERSISTED,[RequisitionCount] [int] NULL,[RequisitionCountAll] [int] NOT NULL,[OriginalFile] [bit] NULL,[FormattedFile] [bit] NULL,

INDEX [AllColumn] NONCLUSTERED 
(
	[MPC] ASC,[UpdatedDate] ASC,[FullName] ASC,[Phone1] ASC,[Email] ASC,[City] ASC,[Code] ASC,[UpdatedBy] ASC,[Status] ASC,[RateCandidate] ASC,[Keywords] ASC,[EligibilityID] ASC,[Relocate] ASC,[JobOptions] ASC,[Communication] ASC,[Background] ASC
),
 CONSTRAINT [CandidateView_primaryKey]  PRIMARY KEY NONCLUSTERED HASH 
(
	[ID]
)WITH ( BUCKET_COUNT = 16384),
INDEX [ZipState] NONCLUSTERED 
(
	[ZipCode] ASC,
	[StateID] ASC
)
)WITH ( MEMORY_OPTIMIZED = ON , DURABILITY = SCHEMA_AND_DATA )
GO

CREATE TABLE [dbo].[Companies](
	[ID] [int] IDENTITY(1000,1) NOT NULL,[CompanyName] [varchar](100) NULL,[EIN] [varchar](9) NULL,[WebsiteURL] [varchar](255) NULL,[DUN] [varchar](20) NULL,[NAICSCode] [varchar](6) NULL,[Status] [bit] NOT NULL,[Notes] [varchar](2000) NULL,[CreatedBy] [varchar](10) NULL,[CreatedDate] [smalldatetime] NULL,[UpdatedBy] [varchar](10) NULL,[UpdatedDate] [smalldatetime] NULL,[CompanyName_BIN2]  AS (([CompanyName]) collate Latin1_General_BIN2),
 CONSTRAINT [PK_Companies] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

CREATE TABLE [dbo].[CompanyContacts](
	[ID] [int] IDENTITY(1,1) NOT NULL,[CompanyID] [int] NOT NULL,[ContactPrefix] [varchar](10) NULL,[ContactFirstName] [varchar](50) NULL,[ContactMiddleInitial] [varchar](10) NULL,[ContactLastName] [varchar](50) NULL,[ContactSuffix] [varchar](10) NULL,[CompanyLocationID] [int] NULL,[ContactEmailAddress] [varchar](255) NULL,[ContactPhone] [varchar](20) NULL,[ContactPhoneExtension] [varchar](10) NULL,[ContactFax] [varchar](20) NULL,[Designation] [varchar](255) NULL,[Department] [varchar](255) NULL,[Role] [tinyint] NOT NULL,[Notes] [varchar](2000) NULL,[PrimaryContact] [bit] NOT NULL,[CreatedBy] [varchar](10) NULL,[CreatedDate] [smalldatetime] NULL,[UpdatedBy] [varchar](10) NULL,[UpdatedDate] [smalldatetime] NULL,
 CONSTRAINT [PK_CompanyContacts] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

CREATE TABLE [dbo].[CompanyDocuments](
	[ID] [int] IDENTITY(1,1) NOT NULL,[CompanyID] [int] NOT NULL,[DocumentName] [varchar](255) NULL,[OriginalFileName] [varchar](255) NULL,[InternalFileName] [varchar](255) NULL,[Notes] [varchar](2000) NULL,[UpdatedDate] [smalldatetime] NOT NULL,[UpdatedBy] [varchar](10) NULL,
 CONSTRAINT [PK_CompanyDocuments] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[CompanyLocations](
	[ID] [int] IDENTITY(100,1) NOT NULL,[CompanyID] [int] NOT NULL,[StreetName] [varchar](500) NULL,[City] [varchar](100) NULL,[StateID] [tinyint] NOT NULL,[ZipCode] [varchar](10) NULL,[CompanyEmail] [varchar](255) NULL,[Phone] [varchar](20) NULL,[Extension] [varchar](10) NULL,[Fax] [varchar](20) NULL,[Notes] [varchar](2000) NULL,[IsPrimaryLocation] [bit] NOT NULL,[CreatedBy] [varchar](10) NULL,[CreatedDate] [smalldatetime] NULL,[UpdatedBy] [varchar](10) NULL,[UpdatedDate] [smalldatetime] NULL,
 CONSTRAINT [PK_CompanyLocations] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

CREATE TABLE [dbo].[Designation](
	[Id] [int] IDENTITY(1,1) NOT NULL,[Designation] [varchar](100) NULL,[CreatedBy] [varchar](10) NULL,[CreatedDate] [smalldatetime] NOT NULL,[UpdatedBy] [varchar](10) NULL,[UpdatedDate] [smalldatetime] NOT NULL,[Enabled] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

CREATE TABLE [dbo].[DocumentType](
	[Id] [int] IDENTITY(1,1) NOT NULL,[DocumentType] [varchar](50) NULL,[LastUpdatedDate] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

CREATE TABLE [dbo].[Education](
	[Id] [int] IDENTITY(1,1) NOT NULL,[Education] [varchar](50) NULL,[CreatedBy] [varchar](10) NULL,[CreatedOn] [datetime] NULL,[UpdatedBy] [varchar](10) NULL,[UpdatedOn] [datetime] NULL,[Enabled] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

CREATE TABLE [dbo].[Eligibility](
	[Id] [int] IDENTITY(1,1) NOT NULL,[Eligibility] [varchar](100) NULL,[CreatedBy] [varchar](10) NULL,[CreatedDate] [smalldatetime] NOT NULL,[UpdatedBy] [varchar](10) NULL,[UpdatedDate] [smalldatetime] NOT NULL,[Enabled] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

CREATE TABLE [dbo].[EntitySkills](
	[Id] [int] IDENTITY(1,1) NOT NULL,[EntityId] [int] NOT NULL,[EntityType] [varchar](5) NULL,[SkillId] [int] NOT NULL,[LastUsed] [smallint] NOT NULL,[ExpMonth] [smallint] NOT NULL,[CreatedBy] [varchar](10) NULL,[CreatedDate] [smalldatetime] NOT NULL,[UpdatedBy] [varchar](10) NULL,[UpdatedDate] [smalldatetime] NOT NULL
) ON [PRIMARY]

CREATE TABLE [dbo].[Experience](
	[Id] [int] IDENTITY(1,1) NOT NULL,[Experience] [varchar](100) NULL,[CreatedBy] [varchar](10) NULL,[CreatedDate] [smalldatetime] NOT NULL,[UpdatedBy] [varchar](10) NULL,[UpdatedDate] [smalldatetime] NOT NULL,[Enabled] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

CREATE TABLE [dbo].[JobOptions](
	[JobCode] [char](1) NOT NULL,[JobOptions] [varchar](50) NULL,[Description] [varchar](500) NULL,[DurationReq] [bit] NOT NULL,[RateReq] [bit] NOT NULL,[SalReq] [bit] NOT NULL,[ExpReq] [bit] NOT NULL,[PlaceFeeReq] [bit] NOT NULL,[BenefitsReq] [bit] NOT NULL,[TaxTerms] [varchar](20) NULL,[ShowHours] [bit] NOT NULL,[RateText] [varchar](255) NULL,[PercentText] [varchar](255) NULL,[CostPercent] [numeric](5, 2) NOT NULL,[ShowPercent] [bit] NOT NULL,[UpdateDate] [datetime] NOT NULL,
 CONSTRAINT [PK_JobOptions] PRIMARY KEY CLUSTERED 
(
	[JobCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

CREATE TABLE [dbo].[LeadDocuments](
	[Id] [int] IDENTITY(1,1) NOT NULL,[LeadId] [int] NOT NULL,[OriginalFileName] [varchar](255) NULL,[DocumentName] [varchar](255) NULL,[DocumentLocation] [varchar](255) NULL,[Notes] [varchar](2000) NULL,[UpdatedBy] [varchar](10) NULL,[UpdatedDate] [smalldatetime] NULL,
 CONSTRAINT [PK_LeadDocuments] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

CREATE TABLE [dbo].[LeadIndustry](
	[Id] [tinyint] IDENTITY(1,1) NOT NULL,[Industry] [varchar](100) NULL,[CreatedBy] [varchar](10) NULL,[CreatedDate] [smalldatetime] NULL,[UpdatedBy] [varchar](10) NULL,[UpdatedDate] [smalldatetime] NULL,[Enabled] [bit] NOT NULL
) ON [PRIMARY]

CREATE TABLE [dbo].[Leads](
	[Id] [int] IDENTITY(1,1) NOT NULL,[UserId] [varchar](10) NULL,[Company] [varchar](100) NULL,[FirstName] [varchar](50) NULL,[LastName] [varchar](50) NULL,[Status] [tinyint] NOT NULL,[Phone] [varchar](20) NULL,[Email] [varchar](255) NULL,[Street] [varchar](1000) NULL,[City] [varchar](100) NULL,[State] [int] NULL,[ZipCode] [varchar](20) NULL,[Website] [varchar](255) NULL,[NoEmployees] [int] NOT NULL,[Revenue] [numeric](18, 2) NOT NULL,[LeadSource] [tinyint] NOT NULL,[Industry] [tinyint] NOT NULL,[Description] [varchar](8000) NULL,[CreatedBy] [varchar](10) NULL,[CreatedDate] [smalldatetime] NOT NULL,[UpdatedBy] [varchar](10) NULL,[UpdatedDate] [smalldatetime] NOT NULL,
 CONSTRAINT [PK_Leads] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

CREATE TABLE [dbo].[LeadSource](
	[Id] [tinyint] IDENTITY(1,1) NOT NULL,[LeadSource] [varchar](50) NULL,[CreatedBy] [varchar](10) NULL,[CreatedDate] [smalldatetime] NULL,[UpdatedBy] [varchar](10) NULL,[UpdatedDate] [smalldatetime] NULL,[Enabled] [bit] NOT NULL,
 CONSTRAINT [PK_LeadSource] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

CREATE TABLE [dbo].[LeadStatus](
	[Id] [tinyint] IDENTITY(1,1) NOT NULL,[LeadStatus] [varchar](25) NULL,[CreatedBy] [varchar](10) NULL,[CreatedDate] [smalldatetime] NOT NULL,[UpdatedBy] [varchar](10) NULL,[UpdatedDate] [smalldatetime] NOT NULL,[Enabled] [bit] NOT NULL,
 CONSTRAINT [PK_LeadStatus] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

CREATE TABLE [dbo].[Logs](
	[Id] [int] IDENTITY(1,1) NOT NULL,[Message] [nvarchar](max) NULL,[Level] [nvarchar](max) NULL,[TimeStamp] [datetime] NULL,[Exception] [nvarchar](max) NULL,[LogEvent] [nvarchar](max) NULL,
 CONSTRAINT [PK_Logs] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

CREATE TABLE [dbo].[NAICS](
	[ID] [int] NOT NULL,[NAICSTitle] [varchar](255) NULL,[CreatedBy] [varchar](10) NULL,[CreatedDate] [smalldatetime] NULL,[UpdatedBy] [varchar](10) NULL,[UpdatedDate] [smalldatetime] NULL,
 CONSTRAINT [PK_NAICS] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

CREATE TABLE [dbo].[Notes](
	[Id] [int] IDENTITY(1,1) NOT NULL,[EntityId] [int] NOT NULL,[EntityType] [varchar](5) NULL,[Notes] [varchar](max) NOT NULL,[IsPrimary] [bit] NOT NULL,[CreatedBy] [varchar](10) NULL,[CreatedDate] [smalldatetime] NOT NULL,[UpdatedBy] [varchar](10) NULL,[UpdatedDate] [smalldatetime] NOT NULL,
 CONSTRAINT [PK_ENTITY_Notes] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

CREATE TABLE [dbo].[Preferences](
	[Id] [int] IDENTITY(1,1) NOT NULL,[ReqPriorityHigh] [varchar](7) NULL,[ReqPriorityNormal] [varchar](7) NULL,[ReqPriorityLow] [varchar](7) NULL,[AllRecruitersSubmitCandidate] [bit] NOT NULL,[AdminCandidates] [bit] NOT NULL,[AdminRequisitions] [bit] NOT NULL,[ReqStatusChange] [tinyint] NOT NULL,[CandStatusChange] [tinyint] NOT NULL,[ChangeCandidateSubmissionStatus] [tinyint] NOT NULL,[PageSize] [tinyint] NOT NULL,[SortReqonPriority] [bit] NOT NULL,
 CONSTRAINT [PK_Preferences] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

CREATE TABLE [dbo].[RequisitionDocument](
	[RequisitionDocId] [int] IDENTITY(1,1) NOT NULL,[RequisitionId] [int] NULL,[DocumentName] [varchar](255) NULL,[DocumentLocation] [varchar](255) NULL,[InternalFileName] [varchar](255) NULL,[Notes] [varchar](2000) NULL,[LastUpdatedDate] [datetime] NULL,[LastUpdatedBy] [varchar](10) NULL,
 CONSTRAINT [PK_RequisitionDocument] PRIMARY KEY CLUSTERED 
(
	[RequisitionDocId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

CREATE TABLE [dbo].[Requisitions](
	[Id] [int] IDENTITY(1,1) NOT NULL,[Code] [varchar](15) NULL,[CompanyId] [int] NOT NULL,[HiringMgrId] [int] NOT NULL,[PosTitle] [varchar](200) NULL,[Description] [varchar](max) NOT NULL,[Positions] [int] NOT NULL,[Duration] [varchar](50) NULL,[DurationCode] [char](1) NOT NULL,[Location] [int] NULL,[ExperienceId] [int] NOT NULL,[ExpRateLow] [numeric](9, 2) NOT NULL,[ExpRateHigh] [numeric](9, 2) NOT NULL,[ExpLoadLow] [numeric](9, 2) NOT NULL,[ExpLoadHigh] [numeric](9, 2) NOT NULL,[PlacementFee] [numeric](9, 2) NOT NULL,[PlacementType] [bit] NOT NULL,[JobOption] [char](1) NOT NULL,[ReportTo] [varchar](255) NULL,[SalaryLow] [numeric](10, 2) NOT NULL,[SalaryHigh] [numeric](10, 2) NOT NULL,[ExpPaid] [bit] NOT NULL,[ExpStart] [smalldatetime] NULL,[Status] [char](3) NOT NULL,[AlertOn] [bit] NULL,[AlertTimeout] [int] NOT NULL,[AlertRepFreq] [int] NOT NULL,[AlertEnd] [smalldatetime] NULL,[AlertMsg] [varchar](1000) NULL,[AlertMail] [bit] NULL,[IsHot] [tinyint] NOT NULL,[CreatedBy] [varchar](10) NULL,[CreatedDate] [smalldatetime] NOT NULL,[UpdatedBy] [varchar](10) NULL,[UpdatedDate] [smalldatetime] NOT NULL,[SkillsReq] [varchar](2000) NULL,[Education] [int] NULL,[Eligibility] [int] NULL,[SecurityClearance] [bit] NOT NULL,[SecurityType] [int] NOT NULL,[Benefits] [bit] NOT NULL,[BenefitsNotes] [varchar](max) NULL,[OFCCP] [bit] NOT NULL,[AttachName] [varchar](255) NULL,[AttachFileType] [varchar](10) NULL,[AttachContentType] [varchar](100) NULL,[Due] [smalldatetime] NULL,[BenefitName] [varchar](255) NULL,[BenefitFileType] [varchar](10) NULL,[BenefitContentType] [varchar](100) NULL,[City] [varchar](50) NULL,[StateId] [int] NULL,[Zip] [varchar](10) NULL,[AlertLastSent] [datetime] NULL,[AttachName2] [varchar](255) NULL,[AttachFileType2] [varchar](10) NULL,[AttachContentType2] [varchar](100) NULL,[AttachName3] [varchar](255) NULL,[AttachFileType3] [varchar](10) NULL,[AttachContentType3] [varchar](100) NULL,[Attachments] [varchar](255) NULL,[BenefitsAttach] [varchar](255) NULL,[Attachments2] [varchar](255) NULL,[Attachments3] [varchar](255) NULL,[AssignedRecruiter] [varchar](550) NULL,[SecondaryRecruiter] [varchar](10) NULL,[MandatoryRequirement] [varchar](8000) NULL,[PreferredRequirement] [varchar](8000) NULL,[OptionalRequirement] [varchar](8000) NULL,
 CONSTRAINT [PK_Requisitions] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

CREATE TABLE [dbo].[RequisitionStatus](
	[Id] [int] IDENTITY(1,1) NOT NULL,[RequisitionId] [int] NOT NULL,[Status] [char](3) NOT NULL,[Notes] [varchar](5000) NULL,[CreatedBy] [varchar](10) NULL,[CreatedOn] [datetime] NOT NULL,[UpdatedBy] [varchar](10) NULL,[UpdatedOn] [datetime] NOT NULL,
 CONSTRAINT [PK_RequisitionStatus] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

CREATE TABLE [dbo].[RequisitionView]
(
	[Id] [int] NOT NULL,[Code] [varchar](50) NULL,[Title] [varchar](255) NULL,[Company] [varchar](255) NULL,[JobOption] [char](1) NULL,[JobOptions] [varchar](255) NULL,[StatusCode] [char](3) NULL,[Status] [varchar](20) NULL,[Created] [smalldatetime] NULL,[Updated] [smalldatetime] NULL,[UpdatedBy] [varchar](10) NULL,[CreatedBy] [varchar](10) NULL,[DueDate] [smalldatetime] NULL,[Icon] [varchar](20) NULL,[IsHot] [tinyint] NULL,[SubmitCandidate] [bit] NULL,[CanUpdate] [bit] NULL,[ChangeStatus] [bit] NULL,[PriorityColor] [varchar](20) NULL,[AssignedRecruiter] [varchar](255) NULL,[RoleID] [int] NULL,[SubmissionCount] [int] NOT NULL,
CONSTRAINT [RequisitionView_primaryKey]  PRIMARY KEY NONCLUSTERED HASH 
(
	[Id]
)WITH ( BUCKET_COUNT = 512),
INDEX [ReqView_All] NONCLUSTERED 
(
	[Code] ASC,[Title] ASC,[Company] ASC,[JobOption] ASC,[JobOptions] ASC,[Status] ASC,[Updated] ASC,[UpdatedBy] ASC,[CreatedBy] ASC,[DueDate] ASC,[Icon] ASC,[IsHot] ASC,[SubmitCandidate] ASC,[CanUpdate] ASC,[ChangeStatus] ASC,[PriorityColor] ASC,[AssignedRecruiter] ASC,[RoleID] ASC
)
)WITH ( MEMORY_OPTIMIZED = ON , DURABILITY = SCHEMA_AND_DATA )

CREATE TABLE [dbo].[Roles](
	[ID] [tinyint] IDENTITY(1,1) NOT NULL,[RoleName] [varchar](10) NULL,[RoleDescription] [varchar](255) NULL,[CreateOrEditCompany] [bit] NOT NULL,[CreateOrEditCandidate] [bit] NOT NULL,[ViewAllCompanies] [bit] NOT NULL,[ViewMyCompanyProfile] [bit] NOT NULL,[EditMyCompanyProfile] [bit] NOT NULL,[CreateOrEditRequisitions] [bit] NOT NULL,[ViewOnlyMyCandidates] [bit] NOT NULL,[ViewAllCandidates] [bit] NOT NULL,[ViewRequisitions] [bit] NOT NULL,[EditRequisitions] [bit] NOT NULL,[ManageSubmittedCandidates] [bit] NOT NULL,[DownloadOriginal] [bit] NOT NULL,[DownloadFormatted] [bit] NOT NULL,[AdminScreens] [bit] NOT NULL,[CreatedBy] [varchar](10) NULL,[CreatedDate] [smalldatetime] NULL,[UpdatedBy] [varchar](10) NULL,[UpdatedDate] [smalldatetime] NULL,
 CONSTRAINT [PK_Roles] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

CREATE TABLE [dbo].[SentMails](
	[Id] [int] IDENTITY(1,1) NOT NULL,[CandidateId] [int] NOT NULL,[Subject] [varchar](255) NULL,[Cc] [varchar](2000) NULL,[Body] [varchar](max) NOT NULL,[SentDate] [datetime] NOT NULL,[Sender] [varchar](30) NULL,
 CONSTRAINT [PK_SentMails] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

CREATE TABLE [dbo].[Skills](
	[Id] [int] IDENTITY(1,1) NOT NULL,[Skill] [varchar](100) NULL,[CreatedBy] [varchar](10) NULL,[CreatedDate] [smalldatetime] NOT NULL,[UpdatedBy] [varchar](10) NULL,[UpdatedDate] [smalldatetime] NOT NULL,[Enabled] [bit] NOT NULL,
 CONSTRAINT [PK_Skills] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

CREATE TABLE [dbo].[State](
	[Id] [int] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,[Code] [varchar](2) NULL,[State] [varchar](50) NULL,[Country] [varchar](50) NULL,[CreatedBy] [varchar](10) NULL,[CreatedDate] [smalldatetime] NOT NULL,[UpdatedBy] [varchar](10) NULL,[UpdatedDate] [smalldatetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

CREATE TABLE [dbo].[StatusCode](
	[Id] [int] IDENTITY(1,1) NOT NULL,[StatusCode] [char](3) NOT NULL,[Status] [varchar](50) NULL,[Description] [varchar](100) NULL,[AppliesTo] [char](3) NOT NULL,[DisplayOrder] [int] NOT NULL,[CreatedBy] [varchar](10) NULL,[CreatedDate] [smalldatetime] NOT NULL,[UpdatedBy] [varchar](10) NULL,[UpdatedDate] [smalldatetime] NOT NULL,[Icon] [nvarchar](255) NULL,[SubmitCandidate] [bit] NOT NULL,[Color] [varchar](10) NULL,[ShowCommission] [bit] NOT NULL,
 CONSTRAINT [PK_ENTITY_Status] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

CREATE TABLE [dbo].[SubmissionCommission](
	[Id] [int] IDENTITY(1,1) NOT NULL,[SubmissionId] [int] NOT NULL,[RequirementId] [int] NOT NULL,[JobOptions] [char](1) NOT NULL,[ClientRate] [numeric](10, 2) NOT NULL,[CommissionPercent] [numeric](7, 2) NOT NULL,[Hours] [smallint] NOT NULL,[CostPercent] [tinyint] NOT NULL,[CandidatePayRate] [numeric](7, 2) NOT NULL,[Spread] [numeric](5, 2) NOT NULL,[CommissionSpread] [numeric](5, 2) NOT NULL,[Commission] [numeric](6, 2) NOT NULL,[Points] [tinyint] NOT NULL,[StartDate] [date] NOT NULL,
 CONSTRAINT [PK_SubmissionCommission] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

CREATE TABLE [dbo].[Submissions](
	[Id] [int] IDENTITY(1,1) NOT NULL,[RequisitionId] [int] NOT NULL,[CandidateId] [int] NOT NULL,[Status] [char](3) NOT NULL,[Notes] [varchar](1000) NULL,[CreatedBy] [varchar](10) NULL,[CreatedDate] [datetime] NOT NULL,[UpdatedBy] [varchar](10) NULL,[UpdatedDate] [datetime] NOT NULL,[StatusId] [int] NOT NULL,[ShowCalendar] [bit] NOT NULL,[DateTime] [datetime] NULL,[Type] [char](1) NOT NULL,[PhoneNumber] [varchar](20) NULL,[InterviewDetails] [varchar](2000) NULL,[Undone] [bit] NOT NULL,
 CONSTRAINT [PK_Submissions] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

CREATE TABLE [dbo].[SubmissionStatus](
	[Id] [int] IDENTITY(1,1) NOT NULL,[SubmissionId] [int] NOT NULL,[Status] [char](3) NOT NULL,[Notes] [varchar](1000) NULL,[ShowCalendar] [bit] NOT NULL,[DateTime] [datetime] NULL,[Type] [char](1) NOT NULL,[PhoneNumber] [varchar](20) NULL,[InterviewDetails] [varchar](2000) NULL,[CreatedDate] [datetime] NOT NULL,[CreatedBy] [varchar](10) NULL,[UpdatedDate] [datetime] NOT NULL,[UpdatedBy] [varchar](10) NULL,
 CONSTRAINT [PK_SubmissionStatus] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

CREATE TABLE [dbo].[TaxTerm](
	[TaxTermCode] [char](1) NOT NULL,[TaxTerm] [varchar](50) NULL,[Description] [varchar](500) NULL,[UpdateDate] [datetime] NOT NULL,[Enabled] [bit] NOT NULL,
 CONSTRAINT [PK_TaxTerm] PRIMARY KEY CLUSTERED 
(
	[TaxTermCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

CREATE TABLE [dbo].[Templates](
	[Id] [int] IDENTITY(1,1) NOT NULL,[TemplateName] [varchar](50) NULL,[Cc] [varchar](2000) NULL,[Subject] [varchar](255) NULL,[Template] [varchar](max) NOT NULL,[Notes] [varchar](500) NULL,[Action] [tinyint] NOT NULL,[SendTo] [varchar](200) NULL,[CreatedDate] [datetime] NOT NULL,[CreatedBy] [varchar](30) NULL,[UpdatedDate] [datetime] NOT NULL,[UpdatedBy] [varchar](30) NULL,[IncludeResume] [bit] NOT NULL,[Enabled] [bit] NOT NULL,
 CONSTRAINT [PK_Templates] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

CREATE TABLE [dbo].[Users](
	[UserName] [varchar](10) NOT NULL,[Password] [binary](64) NULL,[Salt] [binary](64) NULL,[Prefix] [varchar](10) NULL,[FirstName] [varchar](50) NULL,[MiddleInitial] [varchar](10) NULL,[LastName] [varchar](50) NULL,[Suffix] [varchar](10) NULL,[EmailAddress] [varchar](255) NULL,[Role] [tinyint] NOT NULL,[Status] [bit] NOT NULL,[CreatedBy] [varchar](10) NULL,[CreatedDate] [smalldatetime] NULL,[UpdatedBy] [varchar](10) NULL,[UpdatedDate] [smalldatetime] NULL,
 CONSTRAINT [PK_Users_1] PRIMARY KEY CLUSTERED 
(
	[UserName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

CREATE TABLE [dbo].[VariableCommission](
	[Id] [int] IDENTITY(1,1) NOT NULL,[NoofHours] [smallint] NOT NULL,[OverHeadCost] [tinyint] NOT NULL,[W2TaxLoadingRate] [tinyint] NOT NULL,[1099CostRate] [tinyint] NOT NULL,[FTERateOffered] [tinyint] NOT NULL,
 CONSTRAINT [PK_VariableCommission] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

CREATE TABLE [dbo].[WorkflowActivity](
	[Id] [int] IDENTITY(1,1) NOT NULL,[Step] [varchar](3) NULL,[Next] [varchar](100) NULL,[IsLast] [bit] NOT NULL,[Role] [varchar](50) NULL,[Schedule] [bit] NOT NULL,[AnyStage] [bit] NOT NULL,
 CONSTRAINT [PK_WorkflowActivity] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

CREATE TABLE [dbo].[ZipCodes](
	[Country] [varchar](2) NULL,[ZipCode] [varchar](5) NOT NULL,[City] [varchar](200) NULL,[STATE] [varchar](50) NULL,[StateAbbreviation] [varchar](2) NULL,[County] [varchar](50) NULL,[Latitude] [decimal](8, 5) NOT NULL,[Longitude] [decimal](8, 5) NOT NULL,[GeogCol1] [geography] NULL,[UpdatedDate] [datetime] NOT NULL,[GeomCol1] [geometry] NULL,
 CONSTRAINT [PK_ZipCodes] PRIMARY KEY CLUSTERED 
(
	[ZipCode] ASC,
	[Latitude] ASC,
	[Longitude] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

CREATE NONCLUSTERED INDEX [Candidate_Multiple_Fields_p] ON [dbo].[Candidate]([Status] ASC)
INCLUDE([FirstName],[LastName],[City],[StateId],[ZipCode],[UpdatedBy],[UpdatedDate],[Email],[Phone1],[RateCandidate],[MPC],[OriginalResume],[FormattedResume]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]

CREATE NONCLUSTERED INDEX [IDX_Candidate_FirstName__LastName_p] ON [dbo].[Candidate]([FirstName] ASC)
INCLUDE([LastName]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO

CREATE NONCLUSTERED INDEX [IDX_Candidate_SID_p] ON [dbo].[Candidate]([StateId] ASC)
INCLUDE([LastName],[FirstName],[Keywords],[Status],[UpdatedBy],[UpdatedDate],[RateCandidate],[MPC],[City],[Email],[Phone1]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]

CREATE NONCLUSTERED INDEX [IX_Candidate_5_p] ON [dbo].[Candidate]([Status] ASC,[UpdatedBy] ASC,[UpdatedDate] ASC,[RateCandidate] ASC,[City] ASC,[Email] ASC,[Phone1] ASC)
INCLUDE([ID],[LastName],[FirstName],[StateId]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]

CREATE NONCLUSTERED INDEX [IX_Candidate_Various_p] ON [dbo].[Candidate]([JobOptions] ASC,[Communication] ASC,[Status] ASC,[UpdatedBy] ASC,[UpdatedDate] ASC,[RateCandidate] ASC,[Email] ASC,[Phone1] ASC)
INCLUDE([EligibilityId],[Relocate],[City],[StateId],[ZipCode],[ID],[LastName],[FirstName]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]

CREATE NONCLUSTERED INDEX [IX_Keywords_Candidate_p] ON [dbo].[Candidate]([Keywords] ASC)
INCLUDE([ID]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]

CREATE NONCLUSTERED INDEX [SomeIndex_p] ON [dbo].[Candidate]([LastName] ASC)
INCLUDE([FirstName]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]

CREATE UNIQUE NONCLUSTERED INDEX [UI_Candidate_Keywords_p] ON [dbo].[Candidate]([ID] ASC)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]

CREATE NONCLUSTERED INDEX [idx_Companies_Status_CreatedBy_UpdatedBy] ON [dbo].[Companies]([Status] ASC,[CreatedBy] ASC,[UpdatedBy] ASC)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]

CREATE NONCLUSTERED INDEX [IX_Companies_CompanyName_BIN2] ON [dbo].[Companies]([CompanyName_BIN2] ASC)
INCLUDE([UpdatedBy],[UpdatedDate],[ID]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]

CREATE NONCLUSTERED INDEX [IX_Location_Primary] ON [dbo].[CompanyLocations]([CompanyID] ASC,[CompanyEmail] ASC,[City] ASC,[ZipCode] ASC,[Phone] ASC)
WHERE ([IsPrimaryLocation]=(1))
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]

CREATE NONCLUSTERED INDEX [ES_ET_SID] ON [dbo].[EntitySkills]([EntityType] ASC,[SkillId] ASC)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]

CREATE NONCLUSTERED INDEX [IDX_EntityName_EntityType] ON [dbo].[EntitySkills]([EntityType] ASC)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]

CREATE NONCLUSTERED INDEX [IDX_EntitySkills_ERSkID] ON [dbo].[EntitySkills]([EntityType] ASC)
INCLUDE([EntityId],[SkillId]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]

CREATE NONCLUSTERED INDEX [IX_Notes_Id_Type] ON [dbo].[Notes]([EntityId] ASC,[EntityType] ASC)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]

CREATE NONCLUSTERED INDEX [IX_Req_CompanyId_All] ON [dbo].[Requisitions]([CompanyId] ASC)
INCLUDE([Id],[Code],[PosTitle],[JobOption],[Status],[UpdatedDate],[Due],[AssignedRecruiter],[SecondaryRecruiter],[CreatedBy],[CreatedDate],[UpdatedBy]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]

CREATE NONCLUSTERED INDEX [IX_Requisition_Code_PosTile_Job_Updated_Due] ON [dbo].[Requisitions]([Code] ASC,[PosTitle] ASC,[JobOption] ASC,[UpdatedDate] ASC,[Due] ASC)
INCLUDE([CompanyId],[Status],[IsHot],[CreatedBy],[UpdatedBy],[AssignedRecruiter]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]

CREATE NONCLUSTERED INDEX [IX_Requisitions_CreatedBy_CreatedDate] ON [dbo].[Requisitions]([CreatedBy] ASC)
INCLUDE([CreatedDate]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]

CREATE NONCLUSTERED INDEX [IX_Requisitions_Status_AllOthers] ON [dbo].[Requisitions]([Status] ASC)
INCLUDE([Id],[Code],[CompanyId],[PosTitle],[JobOption],[CreatedBy],[CreatedDate],[UpdatedBy],[UpdatedDate],[Due],[AssignedRecruiter]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]

CREATE NONCLUSTERED INDEX [IX_Requssitions_Code_Title_Status] ON [dbo].[Requisitions]([Code] ASC,[PosTitle] ASC,[Status] ASC,[CreatedBy] ASC,[CreatedDate] ASC,[Due] ASC)
INCLUDE([Id],[CompanyId],[JobOption]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]

CREATE NONCLUSTERED INDEX [IX_Submission_CandidateId] ON [dbo].[Submissions]([CandidateId] ASC)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]

CREATE NONCLUSTERED INDEX [IX_Submission_ReqId_Status] ON [dbo].[Submissions]([RequisitionId] ASC,[Status] ASC)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]

CREATE NONCLUSTERED INDEX [IX_Submissions_CreatedBy_CreatedDate] ON [dbo].[Submissions]([CreatedBy] ASC)
INCLUDE([CreatedDate]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]

CREATE NONCLUSTERED INDEX [IX_Submissions_Status_Calendar_DateTime] ON [dbo].[Submissions]([Status] ASC,[ShowCalendar] ASC,[DateTime] ASC)
INCLUDE([RequisitionId],[CandidateId],[CreatedBy],[UpdatedBy]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]

CREATE NONCLUSTERED INDEX [<Name of Missing Index, sysname,>] ON [dbo].[ZipCodes]([StateAbbreviation] ASC)
INCLUDE([City]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]

ALTER TABLE [dbo].[Candidate] ADD CONSTRAINT [DF__Candidate__Addre__2A164134_p]  DEFAULT ('') FOR [Address1]
ALTER TABLE [dbo].[Candidate] ADD CONSTRAINT [DF__Candidate__Email__2B0A656D_p]  DEFAULT ('') FOR [Email]
ALTER TABLE [dbo].[Candidate] ADD CONSTRAINT [DF__Candidate__Phone__2BFE89A6_p]  DEFAULT ('') FOR [Phone1]
ALTER TABLE [dbo].[Candidate] ADD CONSTRAINT [DF_Candidate_EligibilityId_p]  DEFAULT ((3)) FOR [EligibilityId]
ALTER TABLE [dbo].[Candidate] ADD CONSTRAINT [DF_Candidate_ExperienceId_p]  DEFAULT ((1)) FOR [ExperienceId]
ALTER TABLE [dbo].[Candidate] ADD CONSTRAINT [DF_Candidate_Experience_p]  DEFAULT ((0)) FOR [Experience]
ALTER TABLE [dbo].[Candidate] ADD CONSTRAINT [DF_Candidate_JobOptions_p]  DEFAULT ('N') FOR [JobOptions]
ALTER TABLE [dbo].[Candidate] ADD CONSTRAINT [DF_Candidate_Communication_p]  DEFAULT ('G') FOR [Communication]
ALTER TABLE [dbo].[Candidate] ADD CONSTRAINT [DF__Candidate__TaxTe__00AA174D_p]  DEFAULT ('') FOR [TaxTerm]
ALTER TABLE [dbo].[Candidate] ADD CONSTRAINT [DF_Candidate_SalaryLow_p]  DEFAULT ((0)) FOR [SalaryLow]
ALTER TABLE [dbo].[Candidate] ADD CONSTRAINT [DF_Candidate_SalaryHigh_p]  DEFAULT ((0)) FOR [SalaryHigh]
ALTER TABLE [dbo].[Candidate] ADD CONSTRAINT [DF_Candidate_HourlyRate_p]  DEFAULT ((0)) FOR [HourlyRate]
ALTER TABLE [dbo].[Candidate] ADD CONSTRAINT [DF__Candidate__Hourl__075714DC_p]  DEFAULT ((0)) FOR [HourlyRateHigh]
ALTER TABLE [dbo].[Candidate] ADD CONSTRAINT [DF_Candidate_VendorId_p]  DEFAULT ((0)) FOR [VendorId]
ALTER TABLE [dbo].[Candidate] ADD CONSTRAINT [DF_Candidate_Relocate_p]  DEFAULT ((0)) FOR [Relocate]
ALTER TABLE [dbo].[Candidate] ADD CONSTRAINT [DF__Candidate__Backg__038683F8_p]  DEFAULT ((0)) FOR [Background]
ALTER TABLE [dbo].[Candidate] ADD CONSTRAINT [DF_Candidate_Keywords_p]  DEFAULT ('') FOR [Keywords]
ALTER TABLE [dbo].[Candidate] ADD CONSTRAINT [DF__Candidate__Summa__1699586C_p]  DEFAULT ('') FOR [Summary]
ALTER TABLE [dbo].[Candidate] ADD CONSTRAINT [DF_Candidate_ExperienceSummary_p]  DEFAULT ('') FOR [ExperienceSummary]
ALTER TABLE [dbo].[Candidate] ADD CONSTRAINT [DF__Candidate__Objec__178D7CA5_p]  DEFAULT ('') FOR [Objective]
ALTER TABLE [dbo].[Candidate] ADD CONSTRAINT [DF_Candidate_RateCandidate_p]  DEFAULT ((3)) FOR [RateCandidate]
ALTER TABLE [dbo].[Candidate] ADD CONSTRAINT [DF_Candidate_MPC_p]  DEFAULT ((0)) FOR [MPC]
ALTER TABLE [dbo].[Candidate] ADD CONSTRAINT [DF_Candidate_Status_p]  DEFAULT ('AVL') FOR [Status]
ALTER TABLE [dbo].[Candidate] ADD CONSTRAINT [DF__Candidate__Refer__65F62111_p]  DEFAULT ((0)) FOR [Refer]
ALTER TABLE [dbo].[Candidate] ADD CONSTRAINT [DF__Candidate__EEO__1881A0DE_p]  DEFAULT ((0)) FOR [EEO]
ALTER TABLE [dbo].[Candidate] ADD CONSTRAINT [DF_Candidate_JsonFileName_p]  DEFAULT ('') FOR [JsonFileName]
ALTER TABLE [dbo].[Candidate] ADD CONSTRAINT [DF_CONSULTANT_CreatedDate_p]  DEFAULT (getdate()) FOR [CreatedDate]
ALTER TABLE [dbo].[Candidate] ADD CONSTRAINT [DF_CONSULTANT_UpdatedDate_p]  DEFAULT (getdate()) FOR [UpdatedDate]
ALTER TABLE [dbo].[CandidateDocument] ADD CONSTRAINT [DF_CandidateDocument_DocumentType]  DEFAULT ((5)) FOR [DocumentType]
ALTER TABLE [dbo].[CandidateDocument] ADD CONSTRAINT [DF_CandidateDocument_InternalFileName]  DEFAULT (replace(newid(),'-','')) FOR [InternalFileName]
ALTER TABLE [dbo].[CandidateDocument] ADD CONSTRAINT [DF_CandidateDocument_UpdatedDate]  DEFAULT (getdate()) FOR [LastUpdatedDate]
ALTER TABLE [dbo].[CandidateEducation] ADD CONSTRAINT [DF__Candidate__Creat__625A9A57]  DEFAULT (getdate()) FOR [CreatedDate]
ALTER TABLE [dbo].[CandidateEducation] ADD CONSTRAINT [DF__Candidate__Updat__634EBE90]  DEFAULT (getdate()) FOR [UpdatedDate]
ALTER TABLE [dbo].[CandidateEmployer] ADD CONSTRAINT [DF_CandidateEmployer_Employer]  DEFAULT ('') FOR [Employer]
ALTER TABLE [dbo].[CandidateEmployer] ADD CONSTRAINT [DF_CandidateEmployer_CreatedDate]  DEFAULT (getdate()) FOR [CreatedDate]
ALTER TABLE [dbo].[CandidateEmployer] ADD CONSTRAINT [DF_CandidateEmployer_UpdatedDate]  DEFAULT (getdate()) FOR [UpdatedDate]
ALTER TABLE [dbo].[Companies] ADD CONSTRAINT [DF_Companies_EIN#]  DEFAULT ('') FOR [EIN]
ALTER TABLE [dbo].[Companies] ADD CONSTRAINT [DF_Companies_Status]  DEFAULT ((1)) FOR [Status]
ALTER TABLE [dbo].[Companies] ADD CONSTRAINT [DF_Companies_CreatedBy]  DEFAULT ('ADMIN') FOR [CreatedBy]
ALTER TABLE [dbo].[Companies] ADD CONSTRAINT [DF_Companies_CreatedDate]  DEFAULT (getdate()) FOR [CreatedDate]
ALTER TABLE [dbo].[Companies] ADD CONSTRAINT [DF_Companies_UpdatedBy]  DEFAULT ('ADMIN') FOR [UpdatedBy]
ALTER TABLE [dbo].[Companies] ADD CONSTRAINT [DF_Companies_UpdatedDate]  DEFAULT (getdate()) FOR [UpdatedDate]
ALTER TABLE [dbo].[CompanyContacts] ADD CONSTRAINT [DF_CompanyContacts_PrimaryContact]  DEFAULT ((0)) FOR [PrimaryContact]
ALTER TABLE [dbo].[CompanyContacts] ADD CONSTRAINT [DF_CompanyContacts_CreatedBy]  DEFAULT ('ADMIN') FOR [CreatedBy]
ALTER TABLE [dbo].[CompanyContacts] ADD CONSTRAINT [DF_CompanyContacts_CreatedDate]  DEFAULT (getdate()) FOR [CreatedDate]
ALTER TABLE [dbo].[CompanyContacts] ADD CONSTRAINT [DF_CompanyContacts_UpdatedBy]  DEFAULT ('ADMIN') FOR [UpdatedBy]
ALTER TABLE [dbo].[CompanyContacts] ADD CONSTRAINT [DF_CompanyContacts_UpdatedDate]  DEFAULT (getdate()) FOR [UpdatedDate]
ALTER TABLE [dbo].[CompanyDocuments] ADD CONSTRAINT [DF_CompanyDocuments_CompanyID]  DEFAULT ((0)) FOR [CompanyID]
ALTER TABLE [dbo].[CompanyDocuments] ADD CONSTRAINT [DF_CompanyDocuments_DocumentName]  DEFAULT ('') FOR [DocumentName]
ALTER TABLE [dbo].[CompanyDocuments] ADD CONSTRAINT [DF_CompanyDocuments_OriginalFileName]  DEFAULT ('') FOR [OriginalFileName]
ALTER TABLE [dbo].[CompanyDocuments] ADD CONSTRAINT [DF_CompanyDocuments_InternalFileName]  DEFAULT ('') FOR [InternalFileName]
ALTER TABLE [dbo].[CompanyDocuments] ADD CONSTRAINT [DF_CompanyDocuments_Notes]  DEFAULT ('') FOR [Notes]
ALTER TABLE [dbo].[CompanyDocuments] ADD CONSTRAINT [DF_Table_1_LastUpdatedDate]  DEFAULT (getdate()) FOR [UpdatedDate]
ALTER TABLE [dbo].[CompanyDocuments] ADD CONSTRAINT [DF_CompanyDocuments_UpdatedBy]  DEFAULT ('ADMIN') FOR [UpdatedBy]
ALTER TABLE [dbo].[CompanyLocations] ADD CONSTRAINT [DF_CompanyLocations_IsPrimaryLocation]  DEFAULT ((1)) FOR [IsPrimaryLocation]
ALTER TABLE [dbo].[CompanyLocations] ADD CONSTRAINT [DF_CompanyLocations_CreatedBy]  DEFAULT ('ADMIN') FOR [CreatedBy]
ALTER TABLE [dbo].[CompanyLocations] ADD CONSTRAINT [DF_CompanyLocations_CreatedDate]  DEFAULT (getdate()) FOR [CreatedDate]
ALTER TABLE [dbo].[CompanyLocations] ADD CONSTRAINT [DF_CompanyLocations_UpdatedBy]  DEFAULT ('ADMIN') FOR [UpdatedBy]
ALTER TABLE [dbo].[CompanyLocations] ADD CONSTRAINT [DF_CompanyLocations_UpdatedDate]  DEFAULT (getdate()) FOR [UpdatedDate]
ALTER TABLE [dbo].[LeadDocuments] ADD CONSTRAINT [DF_LeadDocuments_UpdatedBy]  DEFAULT ('ADMIN') FOR [UpdatedBy]
ALTER TABLE [dbo].[LeadDocuments] ADD CONSTRAINT [DF_LeadDocuments_UpdatedDate]  DEFAULT (getdate()) FOR [UpdatedDate]
ALTER TABLE [dbo].[Leads] ADD CONSTRAINT [DF_Leads_Status]  DEFAULT ((1)) FOR [Status]
ALTER TABLE [dbo].[Leads] ADD CONSTRAINT [DF_Leads_Website]  DEFAULT ('') FOR [Website]
ALTER TABLE [dbo].[Leads] ADD CONSTRAINT [DF_Leads_NoEmployees]  DEFAULT ((0)) FOR [NoEmployees]
ALTER TABLE [dbo].[Leads] ADD CONSTRAINT [DF_Leads_Revenue]  DEFAULT ((0)) FOR [Revenue]
ALTER TABLE [dbo].[Leads] ADD CONSTRAINT [DF_Leads_LeadSource]  DEFAULT ((0)) FOR [LeadSource]
ALTER TABLE [dbo].[Leads] ADD CONSTRAINT [DF_Leads_Industry]  DEFAULT ((0)) FOR [Industry]
ALTER TABLE [dbo].[Leads] ADD CONSTRAINT [DF_Leads_CreatedDate]  DEFAULT (getdate()) FOR [CreatedDate]
ALTER TABLE [dbo].[Leads] ADD CONSTRAINT [DF_Leads_UpdatedDate]  DEFAULT (getdate()) FOR [UpdatedDate]
ALTER TABLE [dbo].[Notes] ADD CONSTRAINT [DF_ENTITY_Notes_IS_PRIMARY]  DEFAULT ((0)) FOR [IsPrimary]
ALTER TABLE [dbo].[Notes] ADD CONSTRAINT [DF_Notes_CreatedDate]  DEFAULT (getdate()) FOR [CreatedDate]
ALTER TABLE [dbo].[Notes] ADD CONSTRAINT [DF_ENTITY_Notes_UpdatedDate]  DEFAULT (getdate()) FOR [UpdatedDate]
ALTER TABLE [dbo].[Preferences] ADD CONSTRAINT [DF_Preferences_ReqPriorityHigh]  DEFAULT ('#ff0000') FOR [ReqPriorityHigh]
ALTER TABLE [dbo].[Preferences] ADD CONSTRAINT [DF_Preferences_ReqPriorityNormal]  DEFAULT ('#000000') FOR [ReqPriorityNormal]
ALTER TABLE [dbo].[Preferences] ADD CONSTRAINT [DF_Preferences_ReqPriorityLow]  DEFAULT ('#008000') FOR [ReqPriorityLow]
ALTER TABLE [dbo].[Preferences] ADD CONSTRAINT [DF_Preferences_AllRecruiters]  DEFAULT ((1)) FOR [AllRecruitersSubmitCandidate]
ALTER TABLE [dbo].[Preferences] ADD CONSTRAINT [DF_Preferences_AdminCandidates]  DEFAULT ((1)) FOR [AdminCandidates]
ALTER TABLE [dbo].[Preferences] ADD CONSTRAINT [DF_Preferences_AdminRequisitions]  DEFAULT ((1)) FOR [AdminRequisitions]
ALTER TABLE [dbo].[Preferences] ADD CONSTRAINT [DF_Preferences_ReqStatusChange]  DEFAULT ((1)) FOR [ReqStatusChange]
ALTER TABLE [dbo].[Preferences] ADD CONSTRAINT [DF_Preferences_CandStatusChange]  DEFAULT ((1)) FOR [CandStatusChange]
ALTER TABLE [dbo].[Preferences] ADD CONSTRAINT [DF_Preferences_ChangeCandidateSubmissionStatus]  DEFAULT ((1)) FOR [ChangeCandidateSubmissionStatus]
ALTER TABLE [dbo].[Preferences] ADD CONSTRAINT [DF_Preferences_PageSize]  DEFAULT ((50)) FOR [PageSize]
ALTER TABLE [dbo].[Preferences] ADD CONSTRAINT [DF_Preferences_SortReqonPriority]  DEFAULT ((0)) FOR [SortReqonPriority]
ALTER TABLE [dbo].[RequisitionDocument] ADD CONSTRAINT [DF_RequisitionDocument_UpdatedDate]  DEFAULT (getdate()) FOR [LastUpdatedDate]
ALTER TABLE [dbo].[Requisitions] ADD CONSTRAINT [DF__Requisiti__Hirin__5DB5E0CB]  DEFAULT ((0)) FOR [HiringMgrId]
ALTER TABLE [dbo].[Requisitions] ADD CONSTRAINT [DF__Requisiti__Posit__5EAA0504]  DEFAULT ((1)) FOR [Positions]
ALTER TABLE [dbo].[Requisitions] ADD CONSTRAINT [DF__Requisiti__Durat__5F9E293D]  DEFAULT ('M') FOR [DurationCode]
ALTER TABLE [dbo].[Requisitions] ADD CONSTRAINT [DF__Requisiti__ExpRa__60924D76]  DEFAULT ((0)) FOR [ExpRateLow]
ALTER TABLE [dbo].[Requisitions] ADD CONSTRAINT [DF__Requisiti__ExpRa__618671AF]  DEFAULT ((0)) FOR [ExpRateHigh]
ALTER TABLE [dbo].[Requisitions] ADD CONSTRAINT [DF__Requisiti__ExpLo__627A95E8]  DEFAULT ((0)) FOR [ExpLoadLow]
ALTER TABLE [dbo].[Requisitions] ADD CONSTRAINT [DF__Requisiti__ExpLo__636EBA21]  DEFAULT ((0)) FOR [ExpLoadHigh]
ALTER TABLE [dbo].[Requisitions] ADD CONSTRAINT [DF__Requisiti__Place__6462DE5A]  DEFAULT ((0)) FOR [PlacementFee]
ALTER TABLE [dbo].[Requisitions] ADD CONSTRAINT [DF__Requisiti__Place__65570293]  DEFAULT ((0)) FOR [PlacementType]
ALTER TABLE [dbo].[Requisitions] ADD CONSTRAINT [DF__Requisiti__JobOp__664B26CC]  DEFAULT ('C') FOR [JobOption]
ALTER TABLE [dbo].[Requisitions] ADD CONSTRAINT [DF__Requisiti__Salar__673F4B05]  DEFAULT ((0)) FOR [SalaryLow]
ALTER TABLE [dbo].[Requisitions] ADD CONSTRAINT [DF__Requisiti__Salar__68336F3E]  DEFAULT ((0)) FOR [SalaryHigh]
ALTER TABLE [dbo].[Requisitions] ADD CONSTRAINT [DF__Requisiti__ExpPa__69279377]  DEFAULT ((0)) FOR [ExpPaid]
ALTER TABLE [dbo].[Requisitions] ADD CONSTRAINT [DF__Requisiti__ExpSt__6A1BB7B0]  DEFAULT (getdate()+(3)) FOR [ExpStart]
ALTER TABLE [dbo].[Requisitions] ADD CONSTRAINT [DF__Requisiti__Statu__6B0FDBE9]  DEFAULT ('NEW') FOR [Status]
ALTER TABLE [dbo].[Requisitions] ADD CONSTRAINT [DF__Requisiti__Alert__6C040022]  DEFAULT ((0)) FOR [AlertOn]
ALTER TABLE [dbo].[Requisitions] ADD CONSTRAINT [DF__Requisiti__Alert__6CF8245B]  DEFAULT ((24)) FOR [AlertTimeout]
ALTER TABLE [dbo].[Requisitions] ADD CONSTRAINT [DF__Requisiti__Alert__6DEC4894]  DEFAULT ((24)) FOR [AlertRepFreq]
ALTER TABLE [dbo].[Requisitions] ADD CONSTRAINT [DF__Requisiti__Alert__6EE06CCD]  DEFAULT (getdate()) FOR [AlertEnd]
ALTER TABLE [dbo].[Requisitions] ADD CONSTRAINT [DF__Requisiti__Alert__6FD49106]  DEFAULT ((0)) FOR [AlertMail]
ALTER TABLE [dbo].[Requisitions] ADD CONSTRAINT [DF__Requisiti__IsHot__70C8B53F]  DEFAULT ((1)) FOR [IsHot]
ALTER TABLE [dbo].[Requisitions] ADD CONSTRAINT [DF__Requisiti__Creat__71BCD978]  DEFAULT (getdate()) FOR [CreatedDate]
ALTER TABLE [dbo].[Requisitions] ADD CONSTRAINT [DF__Requisiti__Updat__72B0FDB1]  DEFAULT (getdate()) FOR [UpdatedDate]
ALTER TABLE [dbo].[Requisitions] ADD CONSTRAINT [DF__Requisiti__Secur__74994623]  DEFAULT ((0)) FOR [SecurityClearance]
ALTER TABLE [dbo].[Requisitions] ADD CONSTRAINT [DF__Requisiti__Secur__758D6A5C]  DEFAULT ((0)) FOR [SecurityType]
ALTER TABLE [dbo].[Requisitions] ADD CONSTRAINT [DF__Requisiti__Benef__76818E95]  DEFAULT ((0)) FOR [Benefits]
ALTER TABLE [dbo].[Requisitions] ADD CONSTRAINT [DF__Requisiti__OFCCP__7775B2CE]  DEFAULT ((0)) FOR [OFCCP]
ALTER TABLE [dbo].[Requisitions] ADD CONSTRAINT [DF__Requisiti__Attac__7869D707]  DEFAULT ('.doc') FOR [AttachFileType]
ALTER TABLE [dbo].[Requisitions] ADD CONSTRAINT [DF__Requisiti__Benef__795DFB40]  DEFAULT ('.doc') FOR [BenefitFileType]
ALTER TABLE [dbo].[Requisitions] ADD CONSTRAINT [DF__Requisiti__State__7A521F79]  DEFAULT ((1)) FOR [StateId]
ALTER TABLE [dbo].[Requisitions] ADD CONSTRAINT [DF__Requisiti__Attac__7B4643B2]  DEFAULT ('.doc') FOR [AttachFileType2]
ALTER TABLE [dbo].[Requisitions] ADD CONSTRAINT [DF__Requisiti__Attac__7C3A67EB]  DEFAULT ('.doc') FOR [AttachFileType3]
ALTER TABLE [dbo].[Requisitions] ADD CONSTRAINT [DF_Requisitions_PrimaryRecruiter]  DEFAULT ('') FOR [AssignedRecruiter]
ALTER TABLE [dbo].[Requisitions] ADD CONSTRAINT [DF_Requisitions_SecondaryRecruiter]  DEFAULT ((0)) FOR [SecondaryRecruiter]
ALTER TABLE [dbo].[Requisitions] ADD CONSTRAINT [DF_Requisitions_MandatoryRequirement]  DEFAULT ('') FOR [MandatoryRequirement]
ALTER TABLE [dbo].[Requisitions] ADD CONSTRAINT [DF_Requisitions_PreferredRequirement]  DEFAULT ('') FOR [PreferredRequirement]
ALTER TABLE [dbo].[Requisitions] ADD CONSTRAINT [DF_Requisitions_OptionalRequirement]  DEFAULT ('') FOR [OptionalRequirement]
ALTER TABLE [dbo].[RequisitionStatus] ADD CONSTRAINT [DF_RequisitionStatus_CreatedOn]  DEFAULT (getdate()) FOR [CreatedOn]
ALTER TABLE [dbo].[RequisitionStatus] ADD CONSTRAINT [DF_RequisitionStatus_UpdatedOn]  DEFAULT (getdate()) FOR [UpdatedOn]
ALTER TABLE [dbo].[RequisitionView] ADD  DEFAULT ((0)) FOR [SubmissionCount]
ALTER TABLE [dbo].[Roles] ADD CONSTRAINT [DF_Roles_CreateOrEditCompany]  DEFAULT ((0)) FOR [CreateOrEditCompany]
ALTER TABLE [dbo].[Roles] ADD CONSTRAINT [DF_Roles_CreateOrEditCandidate]  DEFAULT ((0)) FOR [CreateOrEditCandidate]
ALTER TABLE [dbo].[Roles] ADD CONSTRAINT [DF_Roles_ViewAllCompanies]  DEFAULT ((0)) FOR [ViewAllCompanies]
ALTER TABLE [dbo].[Roles] ADD CONSTRAINT [DF_Roles_ViewMyCompanyProfile]  DEFAULT ((0)) FOR [ViewMyCompanyProfile]
ALTER TABLE [dbo].[Roles] ADD CONSTRAINT [DF_Roles_EditMyCompanyProfile]  DEFAULT ((0)) FOR [EditMyCompanyProfile]
ALTER TABLE [dbo].[Roles] ADD CONSTRAINT [DF_Roles_CreateOrEditRequisitions]  DEFAULT ((0)) FOR [CreateOrEditRequisitions]
ALTER TABLE [dbo].[Roles] ADD CONSTRAINT [DF_Roles_ViewOnlyMyCandidates]  DEFAULT ((1)) FOR [ViewOnlyMyCandidates]
ALTER TABLE [dbo].[Roles] ADD CONSTRAINT [DF_Roles_ViewAllCandidates]  DEFAULT ((0)) FOR [ViewAllCandidates]
ALTER TABLE [dbo].[Roles] ADD CONSTRAINT [DF_Roles_ViewRequisitions]  DEFAULT ((0)) FOR [ViewRequisitions]
ALTER TABLE [dbo].[Roles] ADD CONSTRAINT [DF_Roles_EditRequisitions]  DEFAULT ((0)) FOR [EditRequisitions]
ALTER TABLE [dbo].[Roles] ADD CONSTRAINT [DF_Roles_ManageSubmittedCandidates]  DEFAULT ((0)) FOR [ManageSubmittedCandidates]
ALTER TABLE [dbo].[Roles] ADD CONSTRAINT [DEFAULT_Roles_DownloadOriginal]  DEFAULT ((0)) FOR [DownloadOriginal]
ALTER TABLE [dbo].[Roles] ADD CONSTRAINT [DEFAULT_Roles_DownloadFormatted]  DEFAULT ((0)) FOR [DownloadFormatted]
ALTER TABLE [dbo].[Roles] ADD CONSTRAINT [DF_Roles_AdminScreens]  DEFAULT ((0)) FOR [AdminScreens]
ALTER TABLE [dbo].[Roles] ADD CONSTRAINT [DF_Roles_CreatedBy]  DEFAULT ('ADMIN') FOR [CreatedBy]
ALTER TABLE [dbo].[Roles] ADD CONSTRAINT [DF_Roles_CreatedDate]  DEFAULT (getdate()) FOR [CreatedDate]
ALTER TABLE [dbo].[Roles] ADD CONSTRAINT [DF_Roles_UpdatedBy]  DEFAULT ('ADMIN') FOR [UpdatedBy]
ALTER TABLE [dbo].[Roles] ADD CONSTRAINT [DF_Roles_UpdatedDate]  DEFAULT (getdate()) FOR [UpdatedDate]
ALTER TABLE [dbo].[SentMails] ADD CONSTRAINT [DF_SentMails_SentDate]  DEFAULT (getdate()) FOR [SentDate]
ALTER TABLE [dbo].[SubmissionCommission] ADD CONSTRAINT [DF_SubmissionCommission_JobOptions]  DEFAULT ('T') FOR [JobOptions]
ALTER TABLE [dbo].[SubmissionCommission] ADD CONSTRAINT [DF_SubmissionCommission_ClientRate]  DEFAULT ((0)) FOR [ClientRate]
ALTER TABLE [dbo].[SubmissionCommission] ADD CONSTRAINT [DF_SubmissionCommission_HourlyRate]  DEFAULT ((0)) FOR [CommissionPercent]
ALTER TABLE [dbo].[SubmissionCommission] ADD CONSTRAINT [DF_SubmissionCommission_Hours]  DEFAULT ((1)) FOR [Hours]
ALTER TABLE [dbo].[SubmissionCommission] ADD CONSTRAINT [DF_SubmissionCommission_CostPercent]  DEFAULT ((1)) FOR [CostPercent]
ALTER TABLE [dbo].[SubmissionCommission] ADD CONSTRAINT [DF_SubmissionCommission_CandidatePayRate]  DEFAULT ((0)) FOR [CandidatePayRate]
ALTER TABLE [dbo].[SubmissionCommission] ADD CONSTRAINT [DF_SubmissionCommission_Spread]  DEFAULT ((1)) FOR [Spread]
ALTER TABLE [dbo].[SubmissionCommission] ADD CONSTRAINT [DF_SubmissionCommission_CommissionSpread]  DEFAULT ((0)) FOR [CommissionSpread]
ALTER TABLE [dbo].[SubmissionCommission] ADD CONSTRAINT [DF_SubmissionCommission_Commission]  DEFAULT ((0)) FOR [Commission]
ALTER TABLE [dbo].[SubmissionCommission] ADD CONSTRAINT [DF_SubmissionCommission_Points]  DEFAULT ((0)) FOR [Points]
ALTER TABLE [dbo].[SubmissionCommission] ADD CONSTRAINT [DF_SubmissionCommission_StartDate]  DEFAULT (getdate()) FOR [StartDate]
ALTER TABLE [dbo].[Submissions] ADD CONSTRAINT [DF_Submissions_CreatedDate]  DEFAULT (getdate()) FOR [CreatedDate]
ALTER TABLE [dbo].[Submissions] ADD CONSTRAINT [DF_Submissions_UpdatedDate]  DEFAULT (getdate()) FOR [UpdatedDate]
ALTER TABLE [dbo].[Submissions] ADD CONSTRAINT [DF_Submissions_StatusId]  DEFAULT ((1)) FOR [StatusId]
ALTER TABLE [dbo].[Submissions] ADD CONSTRAINT [DF_Submissions_ShowCalendar]  DEFAULT ((0)) FOR [ShowCalendar]
ALTER TABLE [dbo].[Submissions] ADD CONSTRAINT [DF_Submissions_Type]  DEFAULT ('P') FOR [Type]
ALTER TABLE [dbo].[Submissions] ADD CONSTRAINT [DF_Submissions_Undone]  DEFAULT ((0)) FOR [Undone]
ALTER TABLE [dbo].[SubmissionStatus] ADD CONSTRAINT [DF_SubmissionStatus_ShowCalendar]  DEFAULT ((0)) FOR [ShowCalendar]
ALTER TABLE [dbo].[SubmissionStatus] ADD CONSTRAINT [DF_SubmissionStatus_Type]  DEFAULT ('P') FOR [Type]
ALTER TABLE [dbo].[SubmissionStatus] ADD CONSTRAINT [DF_SubmissionStatus_CreatedDate]  DEFAULT (getdate()) FOR [CreatedDate]
ALTER TABLE [dbo].[SubmissionStatus] ADD CONSTRAINT [DF_SubmissionStatus_UpdatedBy]  DEFAULT (getdate()) FOR [UpdatedDate]
ALTER TABLE [dbo].[Templates] ADD CONSTRAINT [DF_Templates_Action]  DEFAULT ((1)) FOR [Action]
ALTER TABLE [dbo].[Templates] ADD CONSTRAINT [DF_Templates_SendTo]  DEFAULT ('Administrator,Sales Manager,Recruiter,Full Desk,Requisition Owner,Candidate Owner,Requisition Assigned,Everyone,Others') FOR [SendTo]
ALTER TABLE [dbo].[Templates] ADD CONSTRAINT [DF_Templates_CreatedOn]  DEFAULT (getdate()) FOR [CreatedDate]
ALTER TABLE [dbo].[Templates] ADD CONSTRAINT [DF_Templates_UpdatedOn]  DEFAULT (getdate()) FOR [UpdatedDate]
ALTER TABLE [dbo].[Templates] ADD CONSTRAINT [DF_Templates_IncludeResume]  DEFAULT ((0)) FOR [IncludeResume]
ALTER TABLE [dbo].[Templates] ADD CONSTRAINT [DF_Templates_Enabled]  DEFAULT ((1)) FOR [Enabled]
ALTER TABLE [dbo].[Users] ADD CONSTRAINT [DF_Users_Prefix]  DEFAULT ('') FOR [Prefix]
ALTER TABLE [dbo].[Users] ADD CONSTRAINT [DF_Users_MiddleInitial]  DEFAULT ('') FOR [MiddleInitial]
ALTER TABLE [dbo].[Users] ADD CONSTRAINT [DF_Users_Suffix]  DEFAULT ('') FOR [Suffix]
ALTER TABLE [dbo].[Users] ADD CONSTRAINT [DF_Users_Status]  DEFAULT ((1)) FOR [Status]
ALTER TABLE [dbo].[Users] ADD CONSTRAINT [DF_Users_CreatedBy]  DEFAULT ('ADMIN') FOR [CreatedBy]
ALTER TABLE [dbo].[Users] ADD CONSTRAINT [DF_Users_CreatedDate]  DEFAULT (getdate()) FOR [CreatedDate]
ALTER TABLE [dbo].[Users] ADD CONSTRAINT [DF_Users_UpdatedBy]  DEFAULT ('ADMIN') FOR [UpdatedBy]
ALTER TABLE [dbo].[Users] ADD CONSTRAINT [DF_Users_UpdatedDate]  DEFAULT (getdate()) FOR [UpdatedDate]
ALTER TABLE [dbo].[VariableCommission] ADD CONSTRAINT [DF_VariableCommission_NoofHours]  DEFAULT ((1920)) FOR [NoofHours]
ALTER TABLE [dbo].[VariableCommission] ADD CONSTRAINT [DF_VariableCommission_OverHeadCost]  DEFAULT ((24)) FOR [OverHeadCost]
ALTER TABLE [dbo].[VariableCommission] ADD CONSTRAINT [DF_VariableCommission_W2TaxLoadingRate]  DEFAULT ((12)) FOR [W2TaxLoadingRate]
ALTER TABLE [dbo].[VariableCommission] ADD CONSTRAINT [DF_VariableCommission_1099CostRate]  DEFAULT ((3)) FOR [1099CostRate]
ALTER TABLE [dbo].[VariableCommission] ADD CONSTRAINT [DF_VariableCommission_FTERateOffered]  DEFAULT ((15)) FOR [FTERateOffered]
ALTER TABLE [dbo].[Users] WITH CHECK ADD CONSTRAINT [FK_Users_Roles] FOREIGN KEY([Role])
REFERENCES [dbo].[Roles] ([ID]) ON UPDATE CASCADE ON DELETE CASCADE
ALTER TABLE [dbo].[Users] CHECK CONSTRAINT [FK_Users_Roles]
ALTER TABLE [dbo].[Candidate] WITH NOCHECK ADD CONSTRAINT [CK_Candidate_Communication_p] CHECK  (([Communication]='A' OR [Communication]='F' OR [Communication]='G' OR [Communication]='X'))
ALTER TABLE [dbo].[Candidate] CHECK CONSTRAINT [CK_Candidate_Communication_p]
ALTER TABLE [dbo].[Candidate] WITH NOCHECK ADD CONSTRAINT [CK_Candidate_p] CHECK  (([RateCandidate]=(5) OR [RateCandidate]=(4) OR [RateCandidate]=(3) OR [RateCandidate]=(2) OR [RateCandidate]=(1)))
ALTER TABLE [dbo].[Candidate] CHECK CONSTRAINT [CK_Candidate_p]

CREATE TRIGGER [dbo].[InsertCandidateTrigger_p]
   ON  [dbo].[Candidate]
   AFTER INSERT, UPDATE
AS 
BEGIN
		SET NOCOUNT ON;

		IF CURSOR_STATUS('global', 'cur') >= 0
		BEGIN
			CLOSE cur;
			DEALLOCATE cur;
		END

		-- Step 1: Fetch candidates into Cursor

		DECLARE @ID int, @MPC bit, @UpdatedDate smalldatetime, @FirstName varchar(50), @LastName varchar(50), @Phone1 varchar(15), @Email varchar(255), @City varchar(50),
			@Code varchar(2), @UpdatedBy varchar(10), @Status char(3), @RateCandidate tinyint, @Keywords varchar(500), @ZipCode varchar(10), @StateID tinyint,
			@EligibilityID int, @Relocate bit, @JobOptions varchar(50), @Communication char(1), @Background bit, @SubmissionCount int, @SubmissionCountAll int, @Formatted bit, @Original bit;

		DECLARE cur CURSOR LOCAL FAST_FORWARD FOR
			SELECT 
				I.ID, I.MPC, I.UpdatedDate, I.FirstName, I.LastName, I.Phone1, I.Email, I.City, B.Code, I.UpdatedBy, I.Status, I.RateCandidate, I.Keywords, I.ZipCode, I.StateID, I.EligibilityID, I.Relocate, 
				I.JobOptions, I.Communication, I.Background, CASE WHEN I.OriginalFileId <> '' THEN 1 ELSE 0 END [OriginalFileID], CASE WHEN I.FormattedFileId <> '' THEN 1 ELSE 0 END [FormattedFileID]
			FROM 
				INSERTED I LEFT JOIN State B ON I.StateID=B.ID

		-- Step 2: Open Curseor and fetch First Record
		OPEN cur;
		FETCH NEXT FROM cur INTO @ID, @MPC, @UpdatedDate, @FirstName, @LastName, @Phone1, @Email, @City, @Code, @UpdatedBy, @Status, @RateCandidate, @Keywords, @ZipCode, @StateID, @EligibilityID, @Relocate, 
				@JobOptions, @Communication, @Background, @Original, @Formatted;

		WHILE @@FETCH_STATUS = 0
			BEGIN
				-- Get SubmissionCount for this candidate
				SELECT 
					@SubmissionCount = COUNT(DISTINCT S.RequisitionID)
				FROM 
					dbo.Submissions S INNER JOIN dbo.Requisitions R ON S.RequisitionId = R.ID
				WHERE 
					S.CandidateId = @ID AND R.Status IN ('OPN', 'NEW', 'PAR');

				SELECT 
					@SubmissionCountAll = COUNT(DISTINCT S.RequisitionID)
				FROM 
					dbo.Submissions S INNER JOIN dbo.Requisitions R ON S.RequisitionId = R.ID
				WHERE 
					S.CandidateId = @ID;
				--Check if Candidate Exists then update
				if EXISTS(SELECT 1 from CandidateView WHERE ID = @ID)
					BEGIN
						UPDATE
							CandidateView
						SET 
							MPC = @MPC,
							UpdatedDate = @UpdatedDate,
							FirstName = @FirstName,
							LastName = @LastName,
							Phone1 = @Phone1,
							Email = @Email,
							City = @City,
							Code = @Code,
							UpdatedBy = @UpdatedBy,
							[Status] = @Status,
							RateCandidate = @RateCandidate,
							Keywords = @Keywords,
							ZipCode = @ZipCode,
							StateID = @StateID,
							EligibilityID = @EligibilityID,
							Relocate = @Relocate,
							JobOptions = @JobOptions,
							Communication = @Communication,
							Background = @Background,
							RequisitionCount = @SubmissionCount,
							RequisitionCountAll = @SubmissionCountAll,
							OriginalFile = @Original,
							FormattedFile = @Formatted
						WHERE
							ID = @ID;
					END
				else --Insert the Candidate
					BEGIN
						INSERT INTO
							dbo.CandidateView
							(ID, MPC, UpdatedDate, FirstName, LastName, Phone1, Email, City, Code, UpdatedBy, Status, RateCandidate, Keywords, ZipCode, StateID, EligibilityID, Relocate, JobOptions, Communication, 
							Background, RequisitionCount, RequisitionCountAll, OriginalFile, FormattedFile)
						VALUES
							(@ID, @MPC, @UpdatedDate, @FirstName, @LastName, @Phone1, @Email, @City, @Code, @UpdatedBy, @Status, @RateCandidate, @Keywords, @ZipCode, @StateID, @EligibilityID, @Relocate, @JobOptions, 
							@Communication, @Background, @SubmissionCount, @SubmissionCountAll, @Original, @Formatted);
					END

				--Fetch Next record into the variables from Cursor
				FETCH NEXT FROM cur INTO @ID, @MPC, @UpdatedDate, @FirstName, @LastName, @Phone1, @Email, @City, @Code, @UpdatedBy, @Status, @RateCandidate, @Keywords, @ZipCode, @StateID, @EligibilityID, @Relocate, 
										@JobOptions, @Communication, @Background, @Original, @Formatted;
			END

		--Step 3: Close the Cursor
		CLOSE cur;
		DEALLOCATE cur;
	END

ALTER TABLE [dbo].[Candidate] ENABLE TRIGGER [InsertCandidateTrigger_p]

CREATE   TRIGGER [dbo].[InsertRequisitionTrigger]
   ON  [dbo].[Requisitions]
   AFTER INSERT,UPDATE
AS 
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

		IF CURSOR_STATUS('global', 'cur') >= 0
		BEGIN
			CLOSE cur;
			DEALLOCATE cur;
		END

		-- Step 1: Fetch candidates into Cursor
		DECLARE @High varchar(10), @Normal varchar(10), @Low varchar(10);
		SELECT
			@High = A.ReqPriorityHigh, @Normal = A.ReqPriorityNormal, @Low = A.ReqPriorityLow
		FROM 
			dbo.Preferences A

		DECLARE @ID int, @Code varchar(50), @Title varchar(255), @Company varchar(255), @JobOption char(1), @JobOptions varchar(255), @StatusCode char(3), @Status varchar(20), @Updated smalldatetime, 
				@UpdatedBy varchar(10), @CreatedBy varchar(10), @DueDate smalldatetime, @Icon varchar(20), @IsHot tinyint, @SubmitCandidate bit, @CanUpdate bit, @ChangeStatus bit, @PriorityColor varchar(20), @AssignedRecruiter varchar(255), @RoleID int, @SubmissionCount int, @SubmissionCountAll int,@CreatedDate smalldatetime;

		DECLARE cur CURSOR LOCAL FAST_FORWARD FOR
		SELECT 
			I.ID, I.Code, I.PosTitle, B.CompanyName, I.JobOption, C.JobOptions, I.Status, D.Status, I.UpdatedDate, I.UpdatedBy, I.CreatedBy, I.Due, D.Icon, I.IsHot, 1, 1, 1, 
			CASE IsHot WHEN 2 THEN @High WHEN 1 THEN @Normal WHEN 0 THEN @Low END, I.AssignedRecruiter, 5, (SELECT COUNT(DISTINCT CandidateID) FROM Submissions S WHERE S.RequisitionId = I.Id),I.CreatedDate
		FROM 
			INSERTED I INNER JOIN Companies B ON I.CompanyId=B.ID INNER JOIN JobOptions C ON I.JobOption=C.JobCode INNER JOIN dbo.StatusCode D ON I.Status=D.StatusCode AND D.AppliesTo='REQ' --State B ON I.StateID=B.ID

		-- Step 2: Open Cursor and fetch First Record
		OPEN cur;
		FETCH NEXT FROM cur INTO @ID, @Code, @Title, @Company, @JobOption, @JobOptions, @StatusCode, @Status, @Updated, @UpdatedBy, @CreatedBy, @DueDate, @Icon, @IsHot, @SubmitCandidate, @CanUpdate, @ChangeStatus, 
								@PriorityColor, @AssignedRecruiter, @RoleID, @SubmissionCount,@CreatedDate

		WHILE @@FETCH_STATUS = 0
			BEGIN
				-- Get SubmissionCount for this candidate
				if EXISTS(SELECT 1 from RequisitionView WHERE ID = @ID)
					BEGIN
						UPDATE
							RequisitionView
						SET 
							Code = @Code,
							Title = @Title,
							Company = @Company,
							JobOption = @JobOption,
							JobOptions = @JobOptions,
							StatusCode = @StatusCode,
							[Status] = @Status,
							Updated = @Updated,
							UpdatedBy = @UpdatedBy,
							CreatedBy = @CreatedBy,
							DueDate = @DueDate,
							Icon = @Icon,
							IsHot = IsHot,
							SubmitCandidate = @SubmitCandidate,
							CanUpdate = @CanUpdate,
							ChangeStatus = @ChangeStatus,
							PriorityColor = @PriorityColor,
							AssignedRecruiter = @AssignedRecruiter,
							RoleID = @RoleID,
							SubmissionCount = @SubmissionCount,
							Created = @CreatedDate
						WHERE
							ID = @ID;
					END
				else --Insert the Candidate
					BEGIN
						INSERT INTO
							dbo.RequisitionView
							(ID, Code, Title, Company, JobOption, JobOptions, StatusCode, [Status], Updated, UpdatedBy, CreatedBy, DueDate, Icon, IsHot, SubmitCandidate, CanUpdate, ChangeStatus, PriorityColor,AssignedRecruiter, RoleID, SubmissionCount, Created)
						VALUES
							(@ID, @Code, @Title, @Company, @JobOption, @JobOptions, @StatusCode, @Status, @Updated, @UpdatedBy, @CreatedBy, @DueDate, @Icon, @IsHot, @SubmitCandidate, @CanUpdate, @ChangeStatus, @PriorityColor, @AssignedRecruiter, @RoleID, @SubmissionCount, @CreatedDate);
					END

				--Fetch Next record into the variables from Cursor
				FETCH NEXT FROM cur INTO @ID, @Code, @Title, @Company, @JobOption, @JobOptions, @StatusCode, @Status, @Updated, @UpdatedBy, @CreatedBy, @DueDate, @Icon, @IsHot, @SubmitCandidate, @CanUpdate, @ChangeStatus, @PriorityColor, @AssignedRecruiter, @RoleID, @SubmissionCount, @CreatedDate
			END

		--Step 3: Close the Cursor
		CLOSE cur;
		DEALLOCATE cur;
END

ALTER TABLE [dbo].[Requisitions] ENABLE TRIGGER [InsertRequisitionTrigger]

CREATE SPATIAL INDEX [SIndx_SpatialTable_geography_col1] ON [dbo].[ZipCodes]
(
	[GeogCol1]
)USING  GEOGRAPHY_AUTO_GRID 
WITH (
CELLS_PER_OBJECT = 16, PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
