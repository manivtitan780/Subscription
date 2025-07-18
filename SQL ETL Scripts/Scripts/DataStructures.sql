USE [Subscription]
GO
CREATE TYPE [CandEducation] AS TABLE(
	[Degree] [varchar](100) COLLATE Latin1_General_CI_AI NULL,
	[College] [varchar](255) COLLATE Latin1_General_CI_AI NULL,
	[State] [varchar](100) COLLATE Latin1_General_CI_AI NULL,
	[Country] [varchar](100) COLLATE Latin1_General_CI_AI NULL,
	[Year] [varchar](50) COLLATE Latin1_General_CI_AI NULL
)
GO
CREATE TYPE [CandEmployer] AS TABLE(
	[Employer] [varchar](100) COLLATE Latin1_General_CI_AI NULL,
	[Start] [varchar](10) COLLATE Latin1_General_CI_AI NULL,
	[End] [varchar](10) COLLATE Latin1_General_CI_AI NULL,
	[Location] [varchar](100) COLLATE Latin1_General_CI_AI NULL,
	[Title] [varchar](255) COLLATE Latin1_General_CI_AI NULL,
	[Description] [varchar](max) COLLATE Latin1_General_CI_AI NULL
)
GO
CREATE TYPE [CandidateType] AS TABLE(
	[ID] [int] NOT NULL,
	[MPC] [bit] NULL,
	[UpdateddDate] [smalldatetime] NULL,
	[FirstName] [varchar](50) COLLATE Latin1_General_CI_AI NULL,
	[LastName] [varchar](50) COLLATE Latin1_General_CI_AI NULL,
	[Phone1] [varchar](15) COLLATE Latin1_General_CI_AI NULL,
	[Email] [varchar](255) COLLATE Latin1_General_CI_AI NULL,
	[City] [varchar](50) COLLATE Latin1_General_CI_AI NULL,
	[StateId] [int] NULL,
	[ZipCode] [varchar](20) COLLATE Latin1_General_CI_AI NULL,
	[UpdatedBy] [varchar](10) COLLATE Latin1_General_CI_AI NULL,
	[Status] [char](3) COLLATE Latin1_General_CI_AI NULL,
	[RateCandidate] [tinyint] NULL,
	[Keywords] [varchar](500) COLLATE Latin1_General_CI_AI NULL,
	PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (IGNORE_DUP_KEY = OFF)
)
GO
CREATE TYPE [CandSkills] AS TABLE(
	[Skill] [varchar](255) COLLATE Latin1_General_CI_AI NULL,
	[LastUsed] [int] NULL,
	[Month] [int] NULL
)
GO
CREATE TYPE [PagingTable] AS TABLE(
	[Row] [int] IDENTITY(1,1) NOT NULL,
	[ID] [int] NULL,
	[Count] [int] NULL,
	INDEX [IDX_Paging_ID] NONCLUSTERED HASH 
(
	[ID]
)WITH ( BUCKET_COUNT = 131072),
	 PRIMARY KEY NONCLUSTERED 
(
	[Row] ASC
)
)
WITH ( MEMORY_OPTIMIZED = ON )
GO
CREATE TYPE [TempTable] AS TABLE(
	[Row] [int] IDENTITY(1,1) NOT NULL,
	[ID] [int] NOT NULL,
	[Count] [int] NULL,
	 PRIMARY KEY NONCLUSTERED 
(
	[ID] ASC
),
	INDEX [TempTable_ID] NONCLUSTERED HASH 
(
	[ID]
)WITH ( BUCKET_COUNT = 131072)
)
WITH ( MEMORY_OPTIMIZED = ON )
GO
CREATE TYPE [TempTable2] AS TABLE(
	[ID] [int] NOT NULL,
	[Count] [int] NULL,
	 PRIMARY KEY NONCLUSTERED 
(
	[ID] ASC
),
	INDEX [TempTable2_ID] NONCLUSTERED HASH 
(
	[ID]
)WITH ( BUCKET_COUNT = 131072)
)
WITH ( MEMORY_OPTIMIZED = ON )
GO
CREATE TYPE [WordTable] AS TABLE(
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Term] [varchar](100) COLLATE Latin1_General_CI_AI NULL,
	INDEX [IDX_WordTable] NONCLUSTERED HASH 
(
	[Term]
)WITH ( BUCKET_COUNT = 131072),
	 PRIMARY KEY NONCLUSTERED 
(
	[ID] ASC
)
)
WITH ( MEMORY_OPTIMIZED = ON )
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [BigSplit]
(
	@Delimiter char(1),
	@String varchar(max) 
)       
RETURNS @t table (s varchar(255))       
AS       
BEGIN       
    DECLARE @idx int
    DECLARE @slice varchar(max)       
      
    SET @idx = 1       
    if (LEN(@String)<1 OR @String IS NULL)  
		RETURN;
      
    while (@idx!= 0)
		BEGIN       
			SET @idx = CHARINDEX(@Delimiter, @String)       
			if (@idx!=0)
				SET @slice = LEFT(@String,@idx - 1)       
			else       
				SET @slice = @String       
	          
			if (LEN(@slice)>0)  
				INSERT INTO @t
					(s) 
				VALUES
					( LTRIM(RTRIM(@slice)) )       
	  
			SET @String = RIGHT(@String, LEN(@String) - @idx)       
			if (LEN(@String) = 0) 
				BREAK;
		END   
	RETURN
END


GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [ConcatenateSkills]
(
	@RequisitionId int = 1,
	@Delimiter char(1) = ';'
)
RETURNS varchar(8000)
AS
BEGIN
	DECLARE @List varchar(8000);

	SELECT
		@List = COALESCE(@List + @Delimiter + ' ', '') + B.[Skill]
	FROM
		[Professional].dbo.[EntitySkills] A INNER JOIN [ProfessionalMaster].dbo.[Skills] B ON A.[SkillId] = B.[Id]
	WHERE
		A.[EntityId] = @RequisitionId AND A.[EntityType] = 'REQ';


	RETURN @List;
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [GetDistance](@Lat1 FLOAT, @Lon1 FLOAT, @Lat2 FLOAT, @Lon2 FLOAT)
RETURNS FLOAT
AS
BEGIN
    DECLARE @R FLOAT = 3959; -- Earth's radius in miles
    DECLARE @dLat FLOAT = RADIANS(@Lat2 - @Lat1);
    DECLARE @dLon FLOAT = RADIANS(@Lon2 - @Lon1);
    DECLARE @a FLOAT, @c FLOAT, @d FLOAT;

    SET @a = SIN(@dLat / 2) * SIN(@dLat / 2) +
              COS(RADIANS(@Lat1)) * COS(RADIANS(@Lat2)) *
              SIN(@dLon / 2) * SIN(@dLon / 2);
    SET @c = 2 * ATN2(SQRT(@a), SQRT(1 - @a));
    SET @d = @R * @c;

    RETURN @d;
END;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   FUNCTION [GetUniqueAggregatedNames] (@input NVARCHAR(MAX))
RETURNS NVARCHAR(MAX)
AS
BEGIN
    DECLARE @result NVARCHAR(MAX) = '';
    DECLARE @json NVARCHAR(MAX);
    DECLARE @name NVARCHAR(255);

    -- Convert the comma-separated string to JSON
    SET @json = '[{"name":"' + REPLACE(@input, ', ', '"},{"name":"') + '"}]';

    -- Extract unique names and aggregate them
    SELECT @result = STRING_AGG(name, ', ')
    FROM (
        SELECT DISTINCT JSON_VALUE(TRIM(value), '$.name') AS name
        FROM OPENJSON(@json)
    ) AS UniqueNames;

    RETURN @result;
END;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [StripNonNumeric] (@input NVARCHAR(50))
RETURNS NVARCHAR(50)
AS
BEGIN
    DECLARE @output NVARCHAR(50) = ''
    DECLARE @i INT = 1
    DECLARE @len INT = LEN(@input)
    DECLARE @char NCHAR(1)

    WHILE @i <= @len
    BEGIN
        SET @char = SUBSTRING(@input, @i, 1)
        IF @char >= '0' AND @char <= '9'
            SET @output = @output + @char

        SET @i += 1
    END

    RETURN @output
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   FUNCTION [StripPhoneNumber] 
(
	@PhoneNumber varchar(20)
)
RETURNS varchar(20)
AS
BEGIN
	DECLARE @Phone varchar(20) = '';
	SET @Phone = REPLACE(REPLACE(REPLACE(REPLACE(@PhoneNumber, '(', ''), ')', ''), ' ', ''), '-', '');
	IF (TRIM(@Phone) = '0')
		SET @Phone = '';

	RETURN @Phone;
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

create FUNCTION [udfProperCase]
(
@input varchar(8000)
)
--This function returns the proper case string of varchar type
RETURNS varchar(8000)
AS
BEGIN
	IF @input IS NULL 
	BEGIN
		RETURN NULL
	END
	
	DECLARE @output varchar(8000)
	DECLARE @ctr int, @len int, @found_at int
	DECLARE @LOWER_CASE_a int, @LOWER_CASE_z int, @Delimiter char(3), @UPPER_CASE_A int, @UPPER_CASE_Z int
	
	SET @ctr = 1
	SET @len = LEN(@input)
	SET @output = ''
	SET @LOWER_CASE_a = 97
	SET @LOWER_CASE_z = 122
	SET @Delimiter = ' ,-'
	SET @UPPER_CASE_A = 65
	SET @UPPER_CASE_Z = 90
	
	WHILE @ctr <= @len
	BEGIN
		WHILE CHARINDEX(SUBSTRING(@input,@ctr,1), @Delimiter) > 0
		BEGIN
			SET @output = @output + SUBSTRING(@input,@ctr,1)
			SET @ctr = @ctr + 1
		END

		IF ASCII(SUBSTRING(@input,@ctr,1)) BETWEEN @LOWER_CASE_a AND @LOWER_CASE_z
		BEGIN
			SET @output = @output + UPPER(SUBSTRING(@input,@ctr,1))
		END
		ELSE
		BEGIN
			SET @output = @output + SUBSTRING(@input,@ctr,1)
		END
		
		SET @ctr = @ctr + 1

		WHILE CHARINDEX(SUBSTRING(@input,@ctr,1), @Delimiter) = 0 AND (@ctr <= @len)
		BEGIN
			IF ASCII(SUBSTRING(@input,@ctr,1)) BETWEEN @UPPER_CASE_A AND @UPPER_CASE_Z
			BEGIN
				SET @output = @output + LOWER(SUBSTRING(@input,@ctr,1))
			END
			ELSE
			BEGIN
				SET @output = @output + SUBSTRING(@input,@ctr,1)
			END
			SET @ctr = @ctr + 1
		END
		
	END
RETURN @output
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [EntitySkills](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[EntityId] [int] NOT NULL,
	[EntityType] [varchar](5) COLLATE Latin1_General_CI_AI NULL,
	[SkillId] [int] NOT NULL,
	[LastUsed] [smallint] NOT NULL,
	[ExpMonth] [smallint] NOT NULL,
	[CreatedBy] [varchar](10) COLLATE Latin1_General_CI_AI NULL,
	[CreatedDate] [smalldatetime] NOT NULL,
	[UpdatedBy] [varchar](10) COLLATE Latin1_General_CI_AI NULL,
	[UpdatedDate] [smalldatetime] NOT NULL,
 CONSTRAINT [PK_EntitySkills] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Skills](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Skill] [varchar](100) COLLATE Latin1_General_CI_AI NULL,
	[CreatedBy] [varchar](10) COLLATE Latin1_General_CI_AI NULL,
	[CreatedDate] [smalldatetime] NOT NULL,
	[UpdatedBy] [varchar](10) COLLATE Latin1_General_CI_AI NULL,
	[UpdatedDate] [smalldatetime] NOT NULL,
	[Enabled] [bit] NOT NULL,
 CONSTRAINT [PK_Skills] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [SkillsView]
AS
SELECT B.Skill, A.Id, A.EntityId
FROM dbo.EntitySkills AS A INNER JOIN dbo.Skills AS B ON A.SkillId = B.Id
WHERE (A.EntityType = 'CND')
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   FUNCTION [IsSearchMatch]
(
    @Input varchar(200),
    @Target varchar(200)
)
RETURNS TABLE
AS
RETURN
(
    SELECT Value = CASE
        WHEN TRIM(ISNULL(@Input, '')) = '' THEN 1
        WHEN @Target LIKE '%' + TRIM(@Input) + '%' THEN 1
        ELSE 0
    END
);

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [ActiveUsersView]
(
	[CreatedBy] [varchar](10) COLLATE Latin1_General_CI_AI NOT NULL,
	[FullName] [varchar](101) COLLATE Latin1_General_CI_AI NULL,
	[Role] [tinyint] NOT NULL,
	[Status] [bit] NOT NULL,
	[LastRequisitionDate] [date] NULL,
	[RequisitionCount] [smallint] NOT NULL,
	[RefreshDate] [smalldatetime] NOT NULL,

 CONSTRAINT [PK_ActiveUsersView]  PRIMARY KEY NONCLUSTERED 
(
	[CreatedBy] ASC
)
)WITH ( MEMORY_OPTIMIZED = ON , DURABILITY = SCHEMA_AND_DATA )
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [AuditTrail](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Action] [varchar](100) COLLATE Latin1_General_CI_AI NULL,
	[Page] [varchar](30) COLLATE Latin1_General_CI_AI NULL,
	[Description] [varchar](7000) COLLATE Latin1_General_CI_AI NULL,
	[InitiatedBy] [varchar](10) COLLATE Latin1_General_CI_AI NULL,
	[InitiatedOn] [datetime] NOT NULL
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Candidate](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[FirstName] [varchar](50) COLLATE Latin1_General_CI_AI NULL,
	[LastName] [varchar](50) COLLATE Latin1_General_CI_AI NULL,
	[MiddleName] [varchar](50) COLLATE Latin1_General_CI_AI NULL,
	[Title] [varchar](200) COLLATE Latin1_General_CI_AI NULL,
	[Address1] [varchar](255) COLLATE Latin1_General_CI_AI NULL,
	[Address2] [varchar](255) COLLATE Latin1_General_CI_AI NULL,
	[City] [varchar](50) COLLATE Latin1_General_CI_AI NULL,
	[StateId] [int] NULL,
	[ZipCode] [varchar](20) COLLATE Latin1_General_CI_AI NULL,
	[Email] [varchar](255) COLLATE Latin1_General_CI_AI NULL,
	[Phone1] [varchar](15) COLLATE Latin1_General_CI_AI NULL,
	[Phone2] [varchar](15) COLLATE Latin1_General_CI_AI NULL,
	[Phone3] [varchar](15) COLLATE Latin1_General_CI_AI NULL,
	[Phone3Ext] [int] NULL,
	[EligibilityId] [int] NOT NULL,
	[ExperienceId] [int] NOT NULL,
	[Experience] [int] NULL,
	[JobOptions] [varchar](50) COLLATE Latin1_General_CI_AI NULL,
	[Communication] [char](1) COLLATE Latin1_General_CI_AI NOT NULL,
	[TaxTerm] [varchar](30) COLLATE Latin1_General_CI_AI NULL,
	[SalaryLow] [numeric](9, 2) NULL,
	[SalaryHigh] [numeric](9, 2) NULL,
	[HourlyRate] [numeric](6, 2) NULL,
	[HourlyRateHigh] [numeric](6, 2) NULL,
	[VendorId] [int] NULL,
	[Relocate] [bit] NOT NULL,
	[RelocNotes] [varchar](200) COLLATE Latin1_General_CI_AI NULL,
	[Background] [bit] NOT NULL,
	[SecurityClearanceNotes] [varchar](200) COLLATE Latin1_General_CI_AI NULL,
	[Keywords] [varchar](500) COLLATE Latin1_General_CI_AI NULL,
	[Summary] [varchar](max) COLLATE Latin1_General_CI_AI NOT NULL,
	[ExperienceSummary] [varchar](max) COLLATE Latin1_General_CI_AI NOT NULL,
	[Objective] [varchar](max) COLLATE Latin1_General_CI_AI NOT NULL,
	[RateCandidate] [tinyint] NOT NULL,
	[RateNotes] [varchar](max) COLLATE Latin1_General_CI_AI NULL,
	[MPC] [bit] NOT NULL,
	[MPCNotes] [varchar](max) COLLATE Latin1_General_CI_AI NULL,
	[Status] [char](3) COLLATE Latin1_General_CI_AI NOT NULL,
	[TextResume] [varchar](max) COLLATE Latin1_General_CI_AI NULL,
	[OriginalResume] [varchar](255) COLLATE Latin1_General_CI_AI NULL,
	[FormattedResume] [varchar](255) COLLATE Latin1_General_CI_AI NULL,
	[OriginalFileId] [varchar](50) COLLATE Latin1_General_CI_AI NULL,
	[FormattedFileId] [varchar](50) COLLATE Latin1_General_CI_AI NULL,
	[LinkedIn] [varchar](255) COLLATE Latin1_General_CI_AI NULL,
	[Facebook] [varchar](255) COLLATE Latin1_General_CI_AI NULL,
	[Twitter] [varchar](255) COLLATE Latin1_General_CI_AI NULL,
	[Google] [varchar](255) COLLATE Latin1_General_CI_AI NULL,
	[ReferAccountMgr] [varchar](10) COLLATE Latin1_General_CI_AI NULL,
	[Refer] [bit] NOT NULL,
	[EEO] [bit] NOT NULL,
	[ParsedXML] [xml] NULL,
	[JsonFileName] [varchar](255) COLLATE Latin1_General_CI_AI NULL,
	[CreatedBy] [varchar](10) COLLATE Latin1_General_CI_AI NULL,
	[CreatedDate] [smalldatetime] NOT NULL,
	[UpdatedBy] [varchar](10) COLLATE Latin1_General_CI_AI NULL,
	[UpdatedDate] [smalldatetime] NOT NULL,
 CONSTRAINT [PK_CONSULTANT_p] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PS_Candidate]([ID])
) ON [PS_Candidate]([ID])
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [CandidateDocument](
	[CandidateDocId] [int] IDENTITY(1,1) NOT NULL,
	[CandidateId] [int] NULL,
	[DocumentName] [varchar](255) COLLATE Latin1_General_CI_AI NULL,
	[DocumentLocation] [varchar](255) COLLATE Latin1_General_CI_AI NULL,
	[DocumentType] [int] NOT NULL,
	[Notes] [varchar](2000) COLLATE Latin1_General_CI_AI NULL,
	[InternalFileName] [varchar](50) COLLATE Latin1_General_CI_AI NULL,
	[LastUpdatedDate] [datetime] NULL,
	[LastUpdatedBy] [varchar](10) COLLATE Latin1_General_CI_AI NULL,
 CONSTRAINT [PK_CandidateDocument] PRIMARY KEY CLUSTERED 
(
	[CandidateDocId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [CandidateEducation](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CandidateId] [int] NOT NULL,
	[Degree] [varchar](100) COLLATE Latin1_General_CI_AI NULL,
	[College] [varchar](255) COLLATE Latin1_General_CI_AI NULL,
	[State] [varchar](100) COLLATE Latin1_General_CI_AI NULL,
	[Country] [varchar](100) COLLATE Latin1_General_CI_AI NULL,
	[Year] [varchar](50) COLLATE Latin1_General_CI_AI NULL,
	[CreatedDate] [datetime] NOT NULL,
	[CreatedBy] [varchar](10) COLLATE Latin1_General_CI_AI NULL,
	[UpdatedDate] [datetime] NOT NULL,
	[UpdatedBy] [varchar](10) COLLATE Latin1_General_CI_AI NULL,
 CONSTRAINT [PK_CandidateEducation] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [CandidateEmployer](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CandidateId] [int] NOT NULL,
	[Employer] [varchar](100) COLLATE Latin1_General_CI_AI NULL,
	[Start] [varchar](10) COLLATE Latin1_General_CI_AI NULL,
	[End] [varchar](10) COLLATE Latin1_General_CI_AI NULL,
	[Location] [varchar](100) COLLATE Latin1_General_CI_AI NULL,
	[Title] [varchar](255) COLLATE Latin1_General_CI_AI NULL,
	[Description] [varchar](max) COLLATE Latin1_General_CI_AI NULL,
	[CreatedDate] [datetime] NOT NULL,
	[CreatedBy] [varchar](10) COLLATE Latin1_General_CI_AI NULL,
	[UpdatedDate] [datetime] NOT NULL,
	[UpdatedBy] [varchar](10) COLLATE Latin1_General_CI_AI NULL,
 CONSTRAINT [PK_CandidateEmployer] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [CandidateQualityMetricsView]
(
	[CreatedBy] [varchar](10) COLLATE Latin1_General_CI_AI NOT NULL,
	[MetricDate] [date] NOT NULL,
	[Period] [varchar](10) COLLATE Latin1_General_CI_AI NOT NULL,
	[Total_Submissions] [smallint] NOT NULL,
	[Reached_INT] [smallint] NOT NULL,
	[Reached_OEX] [smallint] NOT NULL,
	[Reached_HIR] [smallint] NOT NULL,
	[PEN_to_INT_Ratio] [decimal](5, 2) NOT NULL,
	[INT_to_OEX_Ratio] [decimal](5, 2) NOT NULL,
	[OEX_to_HIR_Ratio] [decimal](5, 2) NOT NULL,
	[RefreshDate] [smalldatetime] NOT NULL,

INDEX [IX_CandidateQualityMetricsView_CreatedBy] NONCLUSTERED 
(
	[CreatedBy] ASC
),
 CONSTRAINT [PK_CandidateQualityMetricsView]  PRIMARY KEY NONCLUSTERED 
(
	[CreatedBy] ASC,
	[Period] ASC,
	[MetricDate] ASC
)
)WITH ( MEMORY_OPTIMIZED = ON , DURABILITY = SCHEMA_AND_DATA )
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [CandidateView]
(
	[ID] [int] NOT NULL,
	[MPC] [bit] NULL,
	[UpdatedDate] [smalldatetime] NULL,
	[FirstName] [varchar](50) COLLATE Latin1_General_CI_AI NULL,
	[LastName] [varchar](50) COLLATE Latin1_General_CI_AI NULL,
	[Phone1] [varchar](15) COLLATE Latin1_General_CI_AI NULL,
	[Email] [varchar](255) COLLATE Latin1_General_CI_AI NULL,
	[City] [varchar](50) COLLATE Latin1_General_CI_AI NULL,
	[Code] [varchar](2) COLLATE Latin1_General_CI_AI NULL,
	[UpdatedBy] [varchar](10) COLLATE Latin1_General_CI_AI NULL,
	[Status] [char](3) COLLATE Latin1_General_CI_AI NULL,
	[RateCandidate] [tinyint] NULL,
	[Keywords] [varchar](500) COLLATE Latin1_General_CI_AI NULL,
	[ZipCode] [varchar](10) COLLATE Latin1_General_CI_AI NULL,
	[StateID] [tinyint] NULL,
	[EligibilityID] [int] NULL,
	[Relocate] [bit] NULL,
	[JobOptions] [varchar](50) COLLATE Latin1_General_CI_AI NULL,
	[Communication] [char](1) COLLATE Latin1_General_CI_AI NULL,
	[Background] [bit] NULL,
	[FullName]  AS (ltrim(rtrim((isnull([FirstName],'')+case when [FirstName] IS NOT NULL AND [LastName] IS NOT NULL then ' ' else '' end)+isnull([LastName],'')))) PERSISTED,
	[RequisitionCount] [int] NULL,
	[RequisitionCountAll] [int] NOT NULL,
	[OriginalFile] [bit] NULL,
	[FormattedFile] [bit] NULL,

INDEX [AllColumn] NONCLUSTERED 
(
	[MPC] ASC,
	[UpdatedDate] ASC,
	[FullName] ASC,
	[Phone1] ASC,
	[Email] ASC,
	[City] ASC,
	[Code] ASC,
	[UpdatedBy] ASC,
	[Status] ASC,
	[RateCandidate] ASC,
	[Keywords] ASC,
	[EligibilityID] ASC,
	[Relocate] ASC,
	[JobOptions] ASC,
	[Communication] ASC,
	[Background] ASC
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
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [CandidateView_old](
	[ID] [int] NOT NULL,
	[MPC] [bit] NULL,
	[UpdatedDate] [smalldatetime] NULL,
	[FirstName] [varchar](50) COLLATE Latin1_General_CI_AI NULL,
	[LastName] [varchar](50) COLLATE Latin1_General_CI_AI NULL,
	[Phone1] [varchar](15) COLLATE Latin1_General_CI_AI NULL,
	[Email] [varchar](255) COLLATE Latin1_General_CI_AI NULL,
	[City] [varchar](50) COLLATE Latin1_General_CI_AI NULL,
	[Code] [varchar](2) COLLATE Latin1_General_CI_AI NULL,
	[UpdatedBy] [varchar](10) COLLATE Latin1_General_CI_AI NULL,
	[Status] [char](3) COLLATE Latin1_General_CI_AI NULL,
	[RateCandidate] [tinyint] NULL,
	[Keywords] [varchar](500) COLLATE Latin1_General_CI_AI NULL,
	[ZipCode] [varchar](10) COLLATE Latin1_General_CI_AI NULL,
	[StateID] [tinyint] NULL,
	[EligibilityID] [int] NULL,
	[Relocate] [bit] NULL,
	[JobOptions] [varchar](50) COLLATE Latin1_General_CI_AI NULL,
	[Communication] [char](1) COLLATE Latin1_General_CI_AI NULL,
	[Background] [bit] NULL,
	[FullName] [varchar](101) COLLATE Latin1_General_CI_AI NULL,
 CONSTRAINT [PK_CandidateView2] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Companies](
	[ID] [int] IDENTITY(1000,1) NOT NULL,
	[CompanyName] [varchar](100) COLLATE Latin1_General_CI_AI NULL,
	[EIN] [varchar](9) COLLATE Latin1_General_CI_AI NULL,
	[WebsiteURL] [varchar](255) COLLATE Latin1_General_CI_AI NULL,
	[DUN] [varchar](20) COLLATE Latin1_General_CI_AI NULL,
	[NAICSCode] [varchar](6) COLLATE Latin1_General_CI_AI NULL,
	[Status] [bit] NOT NULL,
	[Notes] [varchar](2000) COLLATE Latin1_General_CI_AI NULL,
	[CreatedBy] [varchar](10) COLLATE Latin1_General_CI_AI NULL,
	[CreatedDate] [smalldatetime] NULL,
	[UpdatedBy] [varchar](10) COLLATE Latin1_General_CI_AI NULL,
	[UpdatedDate] [smalldatetime] NULL,
	[CompanyName_BIN2]  AS (([CompanyName]) collate Latin1_General_BIN2),
 CONSTRAINT [PK_Companies] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [CompanyContacts](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[CompanyID] [int] NOT NULL,
	[ContactPrefix] [varchar](10) COLLATE Latin1_General_CI_AI NULL,
	[ContactFirstName] [varchar](50) COLLATE Latin1_General_CI_AI NULL,
	[ContactMiddleInitial] [varchar](10) COLLATE Latin1_General_CI_AI NULL,
	[ContactLastName] [varchar](50) COLLATE Latin1_General_CI_AI NULL,
	[ContactSuffix] [varchar](10) COLLATE Latin1_General_CI_AI NULL,
	[CompanyLocationID] [int] NULL,
	[ContactEmailAddress] [varchar](255) COLLATE Latin1_General_CI_AI NULL,
	[ContactPhone] [varchar](20) COLLATE Latin1_General_CI_AI NULL,
	[ContactPhoneExtension] [varchar](10) COLLATE Latin1_General_CI_AI NULL,
	[ContactFax] [varchar](20) COLLATE Latin1_General_CI_AI NULL,
	[Designation] [varchar](255) COLLATE Latin1_General_CI_AI NULL,
	[Department] [varchar](255) COLLATE Latin1_General_CI_AI NULL,
	[Role] [tinyint] NOT NULL,
	[Notes] [varchar](2000) COLLATE Latin1_General_CI_AI NULL,
	[PrimaryContact] [bit] NOT NULL,
	[CreatedBy] [varchar](10) COLLATE Latin1_General_CI_AI NULL,
	[CreatedDate] [smalldatetime] NULL,
	[UpdatedBy] [varchar](10) COLLATE Latin1_General_CI_AI NULL,
	[UpdatedDate] [smalldatetime] NULL,
 CONSTRAINT [PK_CompanyContacts] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [CompanyDocuments](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[CompanyID] [int] NOT NULL,
	[DocumentName] [varchar](255) COLLATE Latin1_General_CI_AI NULL,
	[OriginalFileName] [varchar](255) COLLATE Latin1_General_CI_AI NULL,
	[InternalFileName] [varchar](255) COLLATE Latin1_General_CI_AI NULL,
	[Notes] [varchar](2000) COLLATE Latin1_General_CI_AI NULL,
	[UpdatedDate] [smalldatetime] NOT NULL,
	[UpdatedBy] [varchar](10) COLLATE Latin1_General_CI_AI NULL,
 CONSTRAINT [PK_CompanyDocuments] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [CompanyLocations](
	[ID] [int] IDENTITY(100,1) NOT NULL,
	[CompanyID] [int] NOT NULL,
	[StreetName] [varchar](500) COLLATE Latin1_General_CI_AI NULL,
	[City] [varchar](100) COLLATE Latin1_General_CI_AI NULL,
	[StateID] [tinyint] NOT NULL,
	[ZipCode] [varchar](10) COLLATE Latin1_General_CI_AI NULL,
	[CompanyEmail] [varchar](255) COLLATE Latin1_General_CI_AI NULL,
	[Phone] [varchar](20) COLLATE Latin1_General_CI_AI NULL,
	[Extension] [varchar](10) COLLATE Latin1_General_CI_AI NULL,
	[Fax] [varchar](20) COLLATE Latin1_General_CI_AI NULL,
	[Notes] [varchar](2000) COLLATE Latin1_General_CI_AI NULL,
	[IsPrimaryLocation] [bit] NOT NULL,
	[CreatedBy] [varchar](10) COLLATE Latin1_General_CI_AI NULL,
	[CreatedDate] [smalldatetime] NULL,
	[UpdatedBy] [varchar](10) COLLATE Latin1_General_CI_AI NULL,
	[UpdatedDate] [smalldatetime] NULL,
 CONSTRAINT [PK_CompanyLocations] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [DateRangeLookup]
(
	[Period] [varchar](10) COLLATE Latin1_General_CI_AI NOT NULL,
	[StartDate] [date] NOT NULL,
	[EndDate] [date] NOT NULL,
	[RefreshDate] [smalldatetime] NOT NULL,

 CONSTRAINT [PK_DateRangeLookup]  PRIMARY KEY NONCLUSTERED 
(
	[Period] ASC
)
)WITH ( MEMORY_OPTIMIZED = ON , DURABILITY = SCHEMA_AND_DATA )
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Designation](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Designation] [varchar](100) COLLATE Latin1_General_CI_AI NULL,
	[CreatedBy] [varchar](10) COLLATE Latin1_General_CI_AI NULL,
	[CreatedDate] [smalldatetime] NOT NULL,
	[UpdatedBy] [varchar](10) COLLATE Latin1_General_CI_AI NULL,
	[UpdatedDate] [smalldatetime] NOT NULL,
	[Enabled] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [DocumentType](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[DocumentType] [varchar](50) COLLATE Latin1_General_CI_AI NULL,
	[LastUpdatedDate] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Education](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Education] [varchar](50) COLLATE Latin1_General_CI_AI NULL,
	[CreatedBy] [varchar](10) COLLATE Latin1_General_CI_AI NULL,
	[CreatedOn] [datetime] NULL,
	[UpdatedBy] [varchar](10) COLLATE Latin1_General_CI_AI NULL,
	[UpdatedOn] [datetime] NULL,
	[Enabled] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Eligibility](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Eligibility] [varchar](100) COLLATE Latin1_General_CI_AI NULL,
	[CreatedBy] [varchar](10) COLLATE Latin1_General_CI_AI NULL,
	[CreatedDate] [smalldatetime] NOT NULL,
	[UpdatedBy] [varchar](10) COLLATE Latin1_General_CI_AI NULL,
	[UpdatedDate] [smalldatetime] NOT NULL,
	[Enabled] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Experience](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Experience] [varchar](100) COLLATE Latin1_General_CI_AI NULL,
	[CreatedBy] [varchar](10) COLLATE Latin1_General_CI_AI NULL,
	[CreatedDate] [smalldatetime] NOT NULL,
	[UpdatedBy] [varchar](10) COLLATE Latin1_General_CI_AI NULL,
	[UpdatedDate] [smalldatetime] NOT NULL,
	[Enabled] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [JobOptions](
	[JobCode] [char](1) COLLATE Latin1_General_CI_AI NOT NULL,
	[JobOptions] [varchar](50) COLLATE Latin1_General_CI_AI NULL,
	[Description] [varchar](500) COLLATE Latin1_General_CI_AI NULL,
	[DurationReq] [bit] NOT NULL,
	[RateReq] [bit] NOT NULL,
	[SalReq] [bit] NOT NULL,
	[ExpReq] [bit] NOT NULL,
	[PlaceFeeReq] [bit] NOT NULL,
	[BenefitsReq] [bit] NOT NULL,
	[TaxTerms] [varchar](20) COLLATE Latin1_General_CI_AI NULL,
	[ShowHours] [bit] NOT NULL,
	[RateText] [varchar](255) COLLATE Latin1_General_CI_AI NULL,
	[PercentText] [varchar](255) COLLATE Latin1_General_CI_AI NULL,
	[CostPercent] [numeric](5, 2) NOT NULL,
	[ShowPercent] [bit] NOT NULL,
	[UpdateDate] [datetime] NOT NULL,
 CONSTRAINT [PK_JobOptions] PRIMARY KEY CLUSTERED 
(
	[JobCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [LeadDocuments](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[LeadId] [int] NOT NULL,
	[OriginalFileName] [varchar](255) COLLATE Latin1_General_CI_AI NULL,
	[DocumentName] [varchar](255) COLLATE Latin1_General_CI_AI NULL,
	[DocumentLocation] [varchar](255) COLLATE Latin1_General_CI_AI NULL,
	[Notes] [varchar](2000) COLLATE Latin1_General_CI_AI NULL,
	[UpdatedBy] [varchar](10) COLLATE Latin1_General_CI_AI NULL,
	[UpdatedDate] [smalldatetime] NULL,
 CONSTRAINT [PK_LeadDocuments] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [LeadIndustry](
	[Id] [tinyint] IDENTITY(1,1) NOT NULL,
	[Industry] [varchar](100) COLLATE Latin1_General_CI_AI NULL,
	[CreatedBy] [varchar](10) COLLATE Latin1_General_CI_AI NULL,
	[CreatedDate] [smalldatetime] NULL,
	[UpdatedBy] [varchar](10) COLLATE Latin1_General_CI_AI NULL,
	[UpdatedDate] [smalldatetime] NULL,
	[Enabled] [bit] NOT NULL
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Leads](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [varchar](10) COLLATE Latin1_General_CI_AI NULL,
	[Company] [varchar](100) COLLATE Latin1_General_CI_AI NULL,
	[FirstName] [varchar](50) COLLATE Latin1_General_CI_AI NULL,
	[LastName] [varchar](50) COLLATE Latin1_General_CI_AI NULL,
	[Status] [tinyint] NOT NULL,
	[Phone] [varchar](20) COLLATE Latin1_General_CI_AI NULL,
	[Email] [varchar](255) COLLATE Latin1_General_CI_AI NULL,
	[Street] [varchar](1000) COLLATE Latin1_General_CI_AI NULL,
	[City] [varchar](100) COLLATE Latin1_General_CI_AI NULL,
	[State] [int] NULL,
	[ZipCode] [varchar](20) COLLATE Latin1_General_CI_AI NULL,
	[Website] [varchar](255) COLLATE Latin1_General_CI_AI NULL,
	[NoEmployees] [int] NOT NULL,
	[Revenue] [numeric](18, 2) NOT NULL,
	[LeadSource] [tinyint] NOT NULL,
	[Industry] [tinyint] NOT NULL,
	[Description] [varchar](8000) COLLATE Latin1_General_CI_AI NULL,
	[CreatedBy] [varchar](10) COLLATE Latin1_General_CI_AI NULL,
	[CreatedDate] [smalldatetime] NOT NULL,
	[UpdatedBy] [varchar](10) COLLATE Latin1_General_CI_AI NULL,
	[UpdatedDate] [smalldatetime] NOT NULL,
 CONSTRAINT [PK_Leads] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [LeadSource](
	[Id] [tinyint] IDENTITY(1,1) NOT NULL,
	[LeadSource] [varchar](50) COLLATE Latin1_General_CI_AI NULL,
	[CreatedBy] [varchar](10) COLLATE Latin1_General_CI_AI NULL,
	[CreatedDate] [smalldatetime] NULL,
	[UpdatedBy] [varchar](10) COLLATE Latin1_General_CI_AI NULL,
	[UpdatedDate] [smalldatetime] NULL,
	[Enabled] [bit] NOT NULL,
 CONSTRAINT [PK_LeadSource] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [LeadStatus](
	[Id] [tinyint] IDENTITY(1,1) NOT NULL,
	[LeadStatus] [varchar](25) COLLATE Latin1_General_CI_AI NULL,
	[CreatedBy] [varchar](10) COLLATE Latin1_General_CI_AI NULL,
	[CreatedDate] [smalldatetime] NOT NULL,
	[UpdatedBy] [varchar](10) COLLATE Latin1_General_CI_AI NULL,
	[UpdatedDate] [smalldatetime] NOT NULL,
	[Enabled] [bit] NOT NULL,
 CONSTRAINT [PK_LeadStatus] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Logs](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Message] [nvarchar](max) COLLATE Latin1_General_CI_AI NULL,
	[Level] [nvarchar](max) COLLATE Latin1_General_CI_AI NULL,
	[TimeStamp] [datetime] NULL,
	[Exception] [nvarchar](max) COLLATE Latin1_General_CI_AI NULL,
	[LogEvent] [nvarchar](max) COLLATE Latin1_General_CI_AI NULL,
 CONSTRAINT [PK_Logs] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [NAICS](
	[ID] [int] NOT NULL,
	[NAICSTitle] [varchar](255) COLLATE Latin1_General_CI_AI NULL,
	[CreatedBy] [varchar](10) COLLATE Latin1_General_CI_AI NULL,
	[CreatedDate] [smalldatetime] NULL,
	[UpdatedBy] [varchar](10) COLLATE Latin1_General_CI_AI NULL,
	[UpdatedDate] [smalldatetime] NULL,
 CONSTRAINT [PK_NAICS] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Notes](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[EntityId] [int] NOT NULL,
	[EntityType] [varchar](5) COLLATE Latin1_General_CI_AI NULL,
	[Notes] [varchar](max) COLLATE Latin1_General_CI_AI NOT NULL,
	[IsPrimary] [bit] NOT NULL,
	[CreatedBy] [varchar](10) COLLATE Latin1_General_CI_AI NULL,
	[CreatedDate] [smalldatetime] NOT NULL,
	[UpdatedBy] [varchar](10) COLLATE Latin1_General_CI_AI NULL,
	[UpdatedDate] [smalldatetime] NOT NULL,
 CONSTRAINT [PK_ENTITY_Notes] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [PlacementReportView]
(
	[CreatedBy] [varchar](10) COLLATE Latin1_General_CI_AI NOT NULL,
	[RequisitionId] [int] NOT NULL,
	[CandidateId] [int] NOT NULL,
	[CompanyName] [varchar](255) COLLATE Latin1_General_CI_AI NOT NULL,
	[RequisitionNumber] [varchar](50) COLLATE Latin1_General_CI_AI NOT NULL,
	[NumPosition] [smallint] NOT NULL,
	[RequisitionTitle] [varchar](255) COLLATE Latin1_General_CI_AI NOT NULL,
	[CandidateName] [varchar](101) COLLATE Latin1_General_CI_AI NOT NULL,
	[DateHired] [date] NOT NULL,
	[SalaryOffered] [decimal](9, 2) NOT NULL,
	[StartDate] [date] NOT NULL,
	[DateInvoiced] [date] NOT NULL,
	[DatePaid] [date] NOT NULL,
	[PlacementFee] [decimal](9, 2) NOT NULL,
	[CommissionPercent] [decimal](5, 2) NOT NULL,
	[CommissionEarned] [decimal](9, 2) NOT NULL,
	[RefreshDate] [smalldatetime] NOT NULL,

INDEX [IX_PlacementReport_Company_Date] NONCLUSTERED 
(
	[CompanyName] ASC,
	[DateHired] DESC,
	[CreatedBy] ASC
),
INDEX [IX_PlacementReport_User] NONCLUSTERED HASH 
(
	[CreatedBy]
)WITH ( BUCKET_COUNT = 64),
 CONSTRAINT [PK_PlacementReportView]  PRIMARY KEY NONCLUSTERED 
(
	[CreatedBy] ASC,
	[RequisitionId] ASC,
	[CandidateId] ASC
)
)WITH ( MEMORY_OPTIMIZED = ON , DURABILITY = SCHEMA_AND_DATA )
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Preferences](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ReqPriorityHigh] [varchar](7) COLLATE Latin1_General_CI_AI NULL,
	[ReqPriorityNormal] [varchar](7) COLLATE Latin1_General_CI_AI NULL,
	[ReqPriorityLow] [varchar](7) COLLATE Latin1_General_CI_AI NULL,
	[AllRecruitersSubmitCandidate] [bit] NOT NULL,
	[AdminCandidates] [bit] NOT NULL,
	[AdminRequisitions] [bit] NOT NULL,
	[ReqStatusChange] [tinyint] NOT NULL,
	[CandStatusChange] [tinyint] NOT NULL,
	[ChangeCandidateSubmissionStatus] [tinyint] NOT NULL,
	[PageSize] [tinyint] NOT NULL,
	[SortReqonPriority] [bit] NOT NULL,
 CONSTRAINT [PK_Preferences] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [RecentActivityView]
(
	[CreatedBy] [varchar](10) COLLATE Latin1_General_CI_AI NOT NULL,
	[RequisitionId] [int] NOT NULL,
	[CandidateId] [int] NOT NULL,
	[CompanyName] [varchar](255) COLLATE Latin1_General_CI_AI NOT NULL,
	[RequisitionCode] [varchar](50) COLLATE Latin1_General_CI_AI NOT NULL,
	[RequisitionTitle] [varchar](255) COLLATE Latin1_General_CI_AI NOT NULL,
	[Positions] [smallint] NOT NULL,
	[CandidateName] [varchar](101) COLLATE Latin1_General_CI_AI NOT NULL,
	[CurrentStatus] [char](3) COLLATE Latin1_General_CI_AI NOT NULL,
	[DateFirstSubmitted] [date] NOT NULL,
	[LastActivityDate] [date] NOT NULL,
	[ActivityNotes] [varchar](5000) COLLATE Latin1_General_CI_AI NULL,
	[SubmissionCount] [smallint] NOT NULL,
	[RefreshDate] [smalldatetime] NOT NULL,

INDEX [IX_RecentActivity_Company] NONCLUSTERED 
(
	[CompanyName] ASC,
	[CreatedBy] ASC
),
INDEX [IX_RecentActivity_User] NONCLUSTERED HASH 
(
	[CreatedBy]
)WITH ( BUCKET_COUNT = 64),
 CONSTRAINT [PK_RecentActivityView]  PRIMARY KEY NONCLUSTERED 
(
	[CreatedBy] ASC,
	[RequisitionId] ASC,
	[CandidateId] ASC
)
)WITH ( MEMORY_OPTIMIZED = ON , DURABILITY = SCHEMA_AND_DATA )
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [RequisitionDocument](
	[RequisitionDocId] [int] IDENTITY(1,1) NOT NULL,
	[RequisitionId] [int] NULL,
	[DocumentName] [varchar](255) COLLATE Latin1_General_CI_AI NULL,
	[DocumentLocation] [varchar](255) COLLATE Latin1_General_CI_AI NULL,
	[InternalFileName] [varchar](255) COLLATE Latin1_General_CI_AI NULL,
	[Notes] [varchar](2000) COLLATE Latin1_General_CI_AI NULL,
	[LastUpdatedDate] [datetime] NULL,
	[LastUpdatedBy] [varchar](10) COLLATE Latin1_General_CI_AI NULL,
 CONSTRAINT [PK_RequisitionDocument] PRIMARY KEY CLUSTERED 
(
	[RequisitionDocId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Requisitions](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Code] [varchar](15) COLLATE Latin1_General_CI_AI NULL,
	[CompanyId] [int] NOT NULL,
	[HiringMgrId] [int] NOT NULL,
	[PosTitle] [varchar](200) COLLATE Latin1_General_CI_AI NULL,
	[Description] [varchar](max) COLLATE Latin1_General_CI_AI NOT NULL,
	[Positions] [int] NOT NULL,
	[Duration] [varchar](50) COLLATE Latin1_General_CI_AI NULL,
	[DurationCode] [char](1) COLLATE Latin1_General_CI_AI NOT NULL,
	[Location] [int] NULL,
	[ExperienceId] [int] NOT NULL,
	[ExpRateLow] [numeric](9, 2) NOT NULL,
	[ExpRateHigh] [numeric](9, 2) NOT NULL,
	[ExpLoadLow] [numeric](9, 2) NOT NULL,
	[ExpLoadHigh] [numeric](9, 2) NOT NULL,
	[PlacementFee] [numeric](9, 2) NOT NULL,
	[PlacementType] [bit] NOT NULL,
	[JobOption] [char](1) COLLATE Latin1_General_CI_AI NOT NULL,
	[ReportTo] [varchar](255) COLLATE Latin1_General_CI_AI NULL,
	[SalaryLow] [numeric](10, 2) NOT NULL,
	[SalaryHigh] [numeric](10, 2) NOT NULL,
	[ExpPaid] [bit] NOT NULL,
	[ExpStart] [smalldatetime] NULL,
	[Status] [char](3) COLLATE Latin1_General_CI_AI NOT NULL,
	[AlertOn] [bit] NULL,
	[AlertTimeout] [int] NOT NULL,
	[AlertRepFreq] [int] NOT NULL,
	[AlertEnd] [smalldatetime] NULL,
	[AlertMsg] [varchar](1000) COLLATE Latin1_General_CI_AI NULL,
	[AlertMail] [bit] NULL,
	[IsHot] [tinyint] NOT NULL,
	[CreatedBy] [varchar](10) COLLATE Latin1_General_CI_AI NULL,
	[CreatedDate] [smalldatetime] NOT NULL,
	[UpdatedBy] [varchar](10) COLLATE Latin1_General_CI_AI NULL,
	[UpdatedDate] [smalldatetime] NOT NULL,
	[SkillsReq] [varchar](2000) COLLATE Latin1_General_CI_AI NULL,
	[Education] [int] NULL,
	[Eligibility] [int] NULL,
	[SecurityClearance] [bit] NOT NULL,
	[SecurityType] [int] NOT NULL,
	[Benefits] [bit] NOT NULL,
	[BenefitsNotes] [varchar](max) COLLATE Latin1_General_CI_AI NULL,
	[OFCCP] [bit] NOT NULL,
	[AttachName] [varchar](255) COLLATE Latin1_General_CI_AI NULL,
	[AttachFileType] [varchar](10) COLLATE Latin1_General_CI_AI NULL,
	[AttachContentType] [varchar](100) COLLATE Latin1_General_CI_AI NULL,
	[Due] [smalldatetime] NULL,
	[BenefitName] [varchar](255) COLLATE Latin1_General_CI_AI NULL,
	[BenefitFileType] [varchar](10) COLLATE Latin1_General_CI_AI NULL,
	[BenefitContentType] [varchar](100) COLLATE Latin1_General_CI_AI NULL,
	[City] [varchar](50) COLLATE Latin1_General_CI_AI NULL,
	[StateId] [int] NULL,
	[Zip] [varchar](10) COLLATE Latin1_General_CI_AI NULL,
	[AlertLastSent] [datetime] NULL,
	[AttachName2] [varchar](255) COLLATE Latin1_General_CI_AI NULL,
	[AttachFileType2] [varchar](10) COLLATE Latin1_General_CI_AI NULL,
	[AttachContentType2] [varchar](100) COLLATE Latin1_General_CI_AI NULL,
	[AttachName3] [varchar](255) COLLATE Latin1_General_CI_AI NULL,
	[AttachFileType3] [varchar](10) COLLATE Latin1_General_CI_AI NULL,
	[AttachContentType3] [varchar](100) COLLATE Latin1_General_CI_AI NULL,
	[Attachments] [varchar](255) COLLATE Latin1_General_CI_AI NULL,
	[BenefitsAttach] [varchar](255) COLLATE Latin1_General_CI_AI NULL,
	[Attachments2] [varchar](255) COLLATE Latin1_General_CI_AI NULL,
	[Attachments3] [varchar](255) COLLATE Latin1_General_CI_AI NULL,
	[AssignedRecruiter] [varchar](550) COLLATE Latin1_General_CI_AI NULL,
	[SecondaryRecruiter] [varchar](10) COLLATE Latin1_General_CI_AI NULL,
	[MandatoryRequirement] [varchar](8000) COLLATE Latin1_General_CI_AI NULL,
	[PreferredRequirement] [varchar](8000) COLLATE Latin1_General_CI_AI NULL,
	[OptionalRequirement] [varchar](8000) COLLATE Latin1_General_CI_AI NULL,
 CONSTRAINT [PK_Requisitions] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [RequisitionStatus](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[RequisitionId] [int] NOT NULL,
	[Status] [char](3) COLLATE Latin1_General_CI_AI NOT NULL,
	[Notes] [varchar](5000) COLLATE Latin1_General_CI_AI NULL,
	[CreatedBy] [varchar](10) COLLATE Latin1_General_CI_AI NULL,
	[CreatedOn] [datetime] NOT NULL,
	[UpdatedBy] [varchar](10) COLLATE Latin1_General_CI_AI NULL,
	[UpdatedOn] [datetime] NOT NULL,
 CONSTRAINT [PK_RequisitionStatus] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [RequisitionView]
(
	[Id] [int] NOT NULL,
	[Code] [varchar](50) COLLATE Latin1_General_CI_AI NULL,
	[Title] [varchar](255) COLLATE Latin1_General_CI_AI NULL,
	[Company] [varchar](255) COLLATE Latin1_General_CI_AI NULL,
	[JobOption] [char](1) COLLATE Latin1_General_CI_AI NULL,
	[JobOptions] [varchar](255) COLLATE Latin1_General_CI_AI NULL,
	[StatusCode] [char](3) COLLATE Latin1_General_CI_AI NULL,
	[Status] [varchar](20) COLLATE Latin1_General_CI_AI NULL,
	[Updated] [smalldatetime] NULL,
	[UpdatedBy] [varchar](10) COLLATE Latin1_General_CI_AI NULL,
	[CreatedBy] [varchar](10) COLLATE Latin1_General_CI_AI NULL,
	[DueDate] [smalldatetime] NULL,
	[Icon] [varchar](20) COLLATE Latin1_General_CI_AI NULL,
	[IsHot] [tinyint] NULL,
	[SubmitCandidate] [bit] NULL,
	[CanUpdate] [bit] NULL,
	[ChangeStatus] [bit] NULL,
	[PriorityColor] [varchar](20) COLLATE Latin1_General_CI_AI NULL,
	[AssignedRecruiter] [varchar](255) COLLATE Latin1_General_CI_AI NULL,
	[RoleID] [int] NULL,
	[SubmissionCount] [int] NOT NULL,
	[Created] [smalldatetime] NULL,

 CONSTRAINT [RequisitionView_primaryKey]  PRIMARY KEY NONCLUSTERED HASH 
(
	[Id]
)WITH ( BUCKET_COUNT = 512),
INDEX [ReqView_All] NONCLUSTERED 
(
	[Code] ASC,
	[Title] ASC,
	[Company] ASC,
	[JobOption] ASC,
	[JobOptions] ASC,
	[Status] ASC,
	[Updated] ASC,
	[UpdatedBy] ASC,
	[CreatedBy] ASC,
	[DueDate] ASC,
	[Icon] ASC,
	[IsHot] ASC,
	[SubmitCandidate] ASC,
	[CanUpdate] ASC,
	[ChangeStatus] ASC,
	[PriorityColor] ASC,
	[AssignedRecruiter] ASC,
	[RoleID] ASC
)
)WITH ( MEMORY_OPTIMIZED = ON , DURABILITY = SCHEMA_AND_DATA )
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [RequisitionView_old](
	[Id] [int] NOT NULL,
	[Code] [varchar](50) COLLATE Latin1_General_CI_AI NULL,
	[Title] [varchar](255) COLLATE Latin1_General_CI_AI NULL,
	[Company] [varchar](255) COLLATE Latin1_General_CI_AI NULL,
	[JobOption] [char](1) COLLATE Latin1_General_CI_AI NULL,
	[JobOptions] [varchar](255) COLLATE Latin1_General_CI_AI NULL,
	[StatusCode] [char](3) COLLATE Latin1_General_CI_AI NULL,
	[Status] [varchar](20) COLLATE Latin1_General_CI_AI NULL,
	[Updated] [smalldatetime] NULL,
	[UpdatedBy] [varchar](10) COLLATE Latin1_General_CI_AI NULL,
	[CreatedBy] [varchar](10) COLLATE Latin1_General_CI_AI NULL,
	[DueDate] [smalldatetime] NULL,
	[Icon] [varchar](20) COLLATE Latin1_General_CI_AI NULL,
	[IsHot] [tinyint] NULL,
	[SubmitCandidate] [bit] NULL,
	[CanUpdate] [bit] NULL,
	[ChangeStatus] [bit] NULL,
	[PriorityColor] [varchar](20) COLLATE Latin1_General_CI_AI NULL,
	[AssignedRecruiter] [varchar](255) COLLATE Latin1_General_CI_AI NULL,
	[RoleID] [int] NULL,
 CONSTRAINT [PK_RequisitionView] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Roles](
	[ID] [tinyint] IDENTITY(1,1) NOT NULL,
	[RoleName] [varchar](10) COLLATE Latin1_General_CI_AI NULL,
	[RoleDescription] [varchar](255) COLLATE Latin1_General_CI_AI NULL,
	[CreateOrEditCompany] [bit] NOT NULL,
	[CreateOrEditCandidate] [bit] NOT NULL,
	[ViewAllCompanies] [bit] NOT NULL,
	[ViewMyCompanyProfile] [bit] NOT NULL,
	[EditMyCompanyProfile] [bit] NOT NULL,
	[CreateOrEditRequisitions] [bit] NOT NULL,
	[ViewOnlyMyCandidates] [bit] NOT NULL,
	[ViewAllCandidates] [bit] NOT NULL,
	[ViewRequisitions] [bit] NOT NULL,
	[EditRequisitions] [bit] NOT NULL,
	[ManageSubmittedCandidates] [bit] NOT NULL,
	[DownloadOriginal] [bit] NOT NULL,
	[DownloadFormatted] [bit] NOT NULL,
	[AdminScreens] [bit] NOT NULL,
	[CreatedBy] [varchar](10) COLLATE Latin1_General_CI_AI NULL,
	[CreatedDate] [smalldatetime] NULL,
	[UpdatedBy] [varchar](10) COLLATE Latin1_General_CI_AI NULL,
	[UpdatedDate] [smalldatetime] NULL,
 CONSTRAINT [PK_Roles] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [SentMails](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CandidateId] [int] NOT NULL,
	[Subject] [varchar](255) COLLATE Latin1_General_CI_AI NULL,
	[Cc] [varchar](2000) COLLATE Latin1_General_CI_AI NULL,
	[Body] [varchar](max) COLLATE Latin1_General_CI_AI NOT NULL,
	[SentDate] [datetime] NOT NULL,
	[Sender] [varchar](30) COLLATE Latin1_General_CI_AI NULL,
 CONSTRAINT [PK_SentMails] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [State](
	[Id] [int] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[Code] [varchar](2) COLLATE Latin1_General_CI_AI NULL,
	[State] [varchar](50) COLLATE Latin1_General_CI_AI NULL,
	[Country] [varchar](50) COLLATE Latin1_General_CI_AI NULL,
	[CreatedBy] [varchar](10) COLLATE Latin1_General_CI_AI NULL,
	[CreatedDate] [smalldatetime] NOT NULL,
	[UpdatedBy] [varchar](10) COLLATE Latin1_General_CI_AI NULL,
	[UpdatedDate] [smalldatetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [StatusCode](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[StatusCode] [char](3) COLLATE Latin1_General_CI_AI NOT NULL,
	[Status] [varchar](50) COLLATE Latin1_General_CI_AI NULL,
	[Description] [varchar](100) COLLATE Latin1_General_CI_AI NULL,
	[AppliesTo] [char](3) COLLATE Latin1_General_CI_AI NOT NULL,
	[DisplayOrder] [int] NOT NULL,
	[CreatedBy] [varchar](10) COLLATE Latin1_General_CI_AI NULL,
	[CreatedDate] [smalldatetime] NOT NULL,
	[UpdatedBy] [varchar](10) COLLATE Latin1_General_CI_AI NULL,
	[UpdatedDate] [smalldatetime] NOT NULL,
	[Icon] [nvarchar](255) COLLATE Latin1_General_CI_AI NULL,
	[SubmitCandidate] [bit] NOT NULL,
	[Color] [varchar](10) COLLATE Latin1_General_CI_AI NULL,
	[ShowCommission] [bit] NOT NULL,
 CONSTRAINT [PK_ENTITY_Status] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [SubmissionCommission](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[SubmissionId] [int] NOT NULL,
	[RequirementId] [int] NOT NULL,
	[JobOptions] [char](1) COLLATE Latin1_General_CI_AI NOT NULL,
	[ClientRate] [numeric](10, 2) NOT NULL,
	[CommissionPercent] [numeric](7, 2) NOT NULL,
	[Hours] [smallint] NOT NULL,
	[CostPercent] [tinyint] NOT NULL,
	[CandidatePayRate] [numeric](7, 2) NOT NULL,
	[Spread] [numeric](5, 2) NOT NULL,
	[CommissionSpread] [numeric](5, 2) NOT NULL,
	[Commission] [numeric](6, 2) NOT NULL,
	[Points] [tinyint] NOT NULL,
	[StartDate] [date] NOT NULL,
 CONSTRAINT [PK_SubmissionCommission] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [SubmissionMetricsView]
(
	[CreatedBy] [varchar](10) COLLATE Latin1_General_CI_AI NOT NULL,
	[MetricDate] [date] NOT NULL,
	[Period] [varchar](10) COLLATE Latin1_General_CI_AI NOT NULL,
	[INT_Count] [smallint] NOT NULL,
	[OEX_Count] [smallint] NOT NULL,
	[HIR_Count] [smallint] NOT NULL,
	[Total_Submissions] [smallint] NOT NULL,
	[OEX_HIR_Ratio] [decimal](5, 2) NOT NULL,
	[Requisitions_Created] [smallint] NOT NULL,
	[Active_Requisitions_Updated] [smallint] NOT NULL,
	[RefreshDate] [smalldatetime] NOT NULL,

INDEX [IX_SubmissionMetricsView_CreatedBy] NONCLUSTERED 
(
	[CreatedBy] ASC
),
 CONSTRAINT [PK_SubmissionMetricsView]  PRIMARY KEY NONCLUSTERED 
(
	[CreatedBy] ASC,
	[Period] ASC,
	[MetricDate] ASC
)
)WITH ( MEMORY_OPTIMIZED = ON , DURABILITY = SCHEMA_AND_DATA )
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Submissions](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[RequisitionId] [int] NOT NULL,
	[CandidateId] [int] NOT NULL,
	[Status] [char](3) COLLATE Latin1_General_CI_AI NOT NULL,
	[Notes] [varchar](1000) COLLATE Latin1_General_CI_AI NULL,
	[CreatedBy] [varchar](10) COLLATE Latin1_General_CI_AI NULL,
	[CreatedDate] [datetime] NOT NULL,
	[UpdatedBy] [varchar](10) COLLATE Latin1_General_CI_AI NULL,
	[UpdatedDate] [datetime] NOT NULL,
	[StatusId] [int] NOT NULL,
	[ShowCalendar] [bit] NOT NULL,
	[DateTime] [datetime] NULL,
	[Type] [char](1) COLLATE Latin1_General_CI_AI NOT NULL,
	[PhoneNumber] [varchar](20) COLLATE Latin1_General_CI_AI NULL,
	[InterviewDetails] [varchar](2000) COLLATE Latin1_General_CI_AI NULL,
	[Undone] [bit] NOT NULL,
 CONSTRAINT [PK_Submissions] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [SubmissionStatus](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[SubmissionId] [int] NOT NULL,
	[Status] [char](3) COLLATE Latin1_General_CI_AI NOT NULL,
	[Notes] [varchar](1000) COLLATE Latin1_General_CI_AI NULL,
	[ShowCalendar] [bit] NOT NULL,
	[DateTime] [datetime] NULL,
	[Type] [char](1) COLLATE Latin1_General_CI_AI NOT NULL,
	[PhoneNumber] [varchar](20) COLLATE Latin1_General_CI_AI NULL,
	[InterviewDetails] [varchar](2000) COLLATE Latin1_General_CI_AI NULL,
	[CreatedDate] [datetime] NOT NULL,
	[CreatedBy] [varchar](10) COLLATE Latin1_General_CI_AI NULL,
	[UpdatedDate] [datetime] NOT NULL,
	[UpdatedBy] [varchar](10) COLLATE Latin1_General_CI_AI NULL,
 CONSTRAINT [PK_SubmissionStatus] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [TaxTerm](
	[TaxTermCode] [char](1) COLLATE Latin1_General_CI_AI NOT NULL,
	[TaxTerm] [varchar](50) COLLATE Latin1_General_CI_AI NULL,
	[Description] [varchar](500) COLLATE Latin1_General_CI_AI NULL,
	[UpdateDate] [datetime] NOT NULL,
	[Enabled] [bit] NOT NULL,
 CONSTRAINT [PK_TaxTerm] PRIMARY KEY CLUSTERED 
(
	[TaxTermCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Templates](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[TemplateName] [varchar](50) COLLATE Latin1_General_CI_AI NULL,
	[Cc] [varchar](2000) COLLATE Latin1_General_CI_AI NULL,
	[Subject] [varchar](255) COLLATE Latin1_General_CI_AI NULL,
	[Template] [varchar](max) COLLATE Latin1_General_CI_AI NOT NULL,
	[Notes] [varchar](500) COLLATE Latin1_General_CI_AI NULL,
	[Action] [tinyint] NOT NULL,
	[SendTo] [varchar](200) COLLATE Latin1_General_CI_AI NULL,
	[CreatedDate] [datetime] NOT NULL,
	[CreatedBy] [varchar](30) COLLATE Latin1_General_CI_AI NULL,
	[UpdatedDate] [datetime] NOT NULL,
	[UpdatedBy] [varchar](30) COLLATE Latin1_General_CI_AI NULL,
	[IncludeResume] [bit] NOT NULL,
	[Enabled] [bit] NOT NULL,
 CONSTRAINT [PK_Templates] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [TimingAnalyticsView]
(
	[Context] [varchar](10) COLLATE Latin1_General_CI_AI NOT NULL,
	[CreatedBy] [varchar](10) COLLATE Latin1_General_CI_AI NOT NULL,
	[RequisitionId] [int] NOT NULL,
	[RequisitionCode] [varchar](50) COLLATE Latin1_General_CI_AI NOT NULL,
	[CompanyName] [varchar](255) COLLATE Latin1_General_CI_AI NOT NULL,
	[Title] [varchar](255) COLLATE Latin1_General_CI_AI NOT NULL,
	[CandidateId] [int] NOT NULL,
	[RequisitionCreatedDate] [date] NOT NULL,
	[RequisitionUpdatedDate] [date] NULL,
	[RequisitionStatus] [char](3) COLLATE Latin1_General_CI_AI NOT NULL,
	[TimeToFill_Days] [smallint] NULL,
	[TimeToHire_Days] [smallint] NULL,
	[FirstSubmissionDate] [date] NULL,
	[HireDate] [date] NULL,
	[PEN_Days] [smallint] NOT NULL,
	[REJ_Days] [smallint] NOT NULL,
	[HLD_Days] [smallint] NOT NULL,
	[PHN_Days] [smallint] NOT NULL,
	[URW_Days] [smallint] NOT NULL,
	[INT_Days] [smallint] NOT NULL,
	[RHM_Days] [smallint] NOT NULL,
	[DEC_Days] [smallint] NOT NULL,
	[NOA_Days] [smallint] NOT NULL,
	[OEX_Days] [smallint] NOT NULL,
	[ODC_Days] [smallint] NOT NULL,
	[HIR_Days] [smallint] NOT NULL,
	[WDR_Days] [smallint] NOT NULL,
	[RefreshDate] [smalldatetime] NOT NULL,

INDEX [IX_TimingAnalytics_Company] NONCLUSTERED 
(
	[CompanyName] ASC,
	[Context] ASC,
	[CreatedBy] ASC
),
INDEX [IX_TimingAnalytics_ContextUser] NONCLUSTERED HASH 
(
	[Context],
	[CreatedBy]
)WITH ( BUCKET_COUNT = 128),
 CONSTRAINT [PK_TimingAnalyticsView]  PRIMARY KEY NONCLUSTERED 
(
	[Context] ASC,
	[CreatedBy] ASC,
	[RequisitionId] ASC,
	[CandidateId] ASC
)
)WITH ( MEMORY_OPTIMIZED = ON , DURABILITY = SCHEMA_AND_DATA )
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Users](
	[UserName] [varchar](10) COLLATE Latin1_General_CI_AI NOT NULL,
	[Password] [binary](64) NULL,
	[Salt] [binary](64) NULL,
	[Prefix] [varchar](10) COLLATE Latin1_General_CI_AI NULL,
	[FirstName] [varchar](50) COLLATE Latin1_General_CI_AI NULL,
	[MiddleInitial] [varchar](10) COLLATE Latin1_General_CI_AI NULL,
	[LastName] [varchar](50) COLLATE Latin1_General_CI_AI NULL,
	[Suffix] [varchar](10) COLLATE Latin1_General_CI_AI NULL,
	[EmailAddress] [varchar](255) COLLATE Latin1_General_CI_AI NULL,
	[Role] [tinyint] NOT NULL,
	[Status] [bit] NOT NULL,
	[CreatedBy] [varchar](10) COLLATE Latin1_General_CI_AI NULL,
	[CreatedDate] [smalldatetime] NULL,
	[UpdatedBy] [varchar](10) COLLATE Latin1_General_CI_AI NULL,
	[UpdatedDate] [smalldatetime] NULL,
 CONSTRAINT [PK_Users_1] PRIMARY KEY CLUSTERED 
(
	[UserName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [VariableCommission](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[NoofHours] [smallint] NOT NULL,
	[OverHeadCost] [tinyint] NOT NULL,
	[W2TaxLoadingRate] [tinyint] NOT NULL,
	[1099CostRate] [tinyint] NOT NULL,
	[FTERateOffered] [tinyint] NOT NULL,
 CONSTRAINT [PK_VariableCommission] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [WorkflowActivity](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Step] [varchar](3) COLLATE Latin1_General_CI_AI NULL,
	[Next] [varchar](100) COLLATE Latin1_General_CI_AI NULL,
	[IsLast] [bit] NOT NULL,
	[Role] [varchar](50) COLLATE Latin1_General_CI_AI NULL,
	[Schedule] [bit] NOT NULL,
	[AnyStage] [bit] NOT NULL,
 CONSTRAINT [PK_WorkflowActivity] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [ZipCodes](
	[Country] [varchar](2) COLLATE Latin1_General_CI_AI NULL,
	[ZipCode] [varchar](5) COLLATE Latin1_General_CI_AI NOT NULL,
	[City] [varchar](200) COLLATE Latin1_General_CI_AI NULL,
	[STATE] [varchar](50) COLLATE Latin1_General_CI_AI NULL,
	[StateAbbreviation] [varchar](2) COLLATE Latin1_General_CI_AI NULL,
	[County] [varchar](50) COLLATE Latin1_General_CI_AI NULL,
	[Latitude] [decimal](8, 5) NOT NULL,
	[Longitude] [decimal](8, 5) NOT NULL,
	[GeogCol1] [geography] NULL,
	[UpdatedDate] [datetime] NOT NULL,
	[GeomCol1] [geometry] NULL,
 CONSTRAINT [PK_ZipCodes] PRIMARY KEY CLUSTERED 
(
	[ZipCode] ASC,
	[Latitude] ASC,
	[Longitude] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
CREATE NONCLUSTERED INDEX [Candidate_Multiple_Fields_p] ON [Candidate]
(
	[Status] ASC
)
INCLUDE([FirstName],[LastName],[City],[StateId],[ZipCode],[UpdatedBy],[UpdatedDate],[Email],[Phone1],[RateCandidate],[MPC],[OriginalResume],[FormattedResume]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
CREATE NONCLUSTERED INDEX [IDX_Candidate_FirstName__LastName_p] ON [Candidate]
(
	[FirstName] ASC
)
INCLUDE([LastName]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IDX_Candidate_SID_p] ON [Candidate]
(
	[StateId] ASC
)
INCLUDE([LastName],[FirstName],[Keywords],[Status],[UpdatedBy],[UpdatedDate],[RateCandidate],[MPC],[City],[Email],[Phone1]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
CREATE NONCLUSTERED INDEX [IX_Candidate_5_p] ON [Candidate]
(
	[Status] ASC,
	[UpdatedBy] ASC,
	[UpdatedDate] ASC,
	[RateCandidate] ASC,
	[City] ASC,
	[Email] ASC,
	[Phone1] ASC
)
INCLUDE([ID],[LastName],[FirstName],[StateId]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
CREATE NONCLUSTERED INDEX [IX_Candidate_Various_p] ON [Candidate]
(
	[JobOptions] ASC,
	[Communication] ASC,
	[Status] ASC,
	[UpdatedBy] ASC,
	[UpdatedDate] ASC,
	[RateCandidate] ASC,
	[Email] ASC,
	[Phone1] ASC
)
INCLUDE([EligibilityId],[Relocate],[City],[StateId],[ZipCode],[ID],[LastName],[FirstName]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
CREATE NONCLUSTERED INDEX [IX_Keywords_Candidate_p] ON [Candidate]
(
	[Keywords] ASC
)
INCLUDE([ID]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
CREATE NONCLUSTERED INDEX [SomeIndex_p] ON [Candidate]
(
	[LastName] ASC
)
INCLUDE([FirstName]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE UNIQUE NONCLUSTERED INDEX [UI_Candidate_Keywords_p] ON [Candidate]
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
CREATE NONCLUSTERED INDEX [idx_Companies_Status_CreatedBy_UpdatedBy] ON [Companies]
(
	[Status] ASC,
	[CreatedBy] ASC,
	[UpdatedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ARITHABORT ON
SET CONCAT_NULL_YIELDS_NULL ON
SET QUOTED_IDENTIFIER ON
SET ANSI_NULLS ON
SET ANSI_PADDING ON
SET ANSI_WARNINGS ON
SET NUMERIC_ROUNDABORT OFF
GO
CREATE NONCLUSTERED INDEX [IX_Companies_CompanyName_BIN2] ON [Companies]
(
	[CompanyName_BIN2] ASC
)
INCLUDE([UpdatedBy],[UpdatedDate],[ID]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
CREATE NONCLUSTERED INDEX [IX_Location_Primary] ON [CompanyLocations]
(
	[CompanyID] ASC,
	[CompanyEmail] ASC,
	[City] ASC,
	[ZipCode] ASC,
	[Phone] ASC
)
WHERE ([IsPrimaryLocation]=(1))
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
CREATE NONCLUSTERED INDEX [ES_ET_SID] ON [EntitySkills]
(
	[EntityType] ASC,
	[SkillId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
CREATE NONCLUSTERED INDEX [IDX_EntityName_EntityType] ON [EntitySkills]
(
	[EntityType] ASC,
	[EntityId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
CREATE NONCLUSTERED INDEX [IDX_EntitySkills_ERSkID] ON [EntitySkills]
(
	[EntityType] ASC
)
INCLUDE([EntityId],[SkillId]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
CREATE NONCLUSTERED INDEX [IX_Notes_Id_Type] ON [Notes]
(
	[EntityId] ASC,
	[EntityType] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Req_CompanyId_All] ON [Requisitions]
(
	[CompanyId] ASC
)
INCLUDE([Id],[Code],[PosTitle],[JobOption],[Status],[UpdatedDate],[Due],[AssignedRecruiter],[SecondaryRecruiter],[CreatedBy],[CreatedDate],[UpdatedBy]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
CREATE NONCLUSTERED INDEX [IX_Reqs_Missing] ON [Requisitions]
(
	[CreatedBy] ASC,
	[CreatedDate] ASC
)
INCLUDE([Code],[CompanyId],[PosTitle],[Status],[UpdatedDate]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
CREATE NONCLUSTERED INDEX [IX_Requisition_Code_PosTile_Job_Updated_Due] ON [Requisitions]
(
	[Code] ASC,
	[PosTitle] ASC,
	[JobOption] ASC,
	[UpdatedDate] ASC,
	[Due] ASC
)
INCLUDE([CompanyId],[Status],[IsHot],[CreatedBy],[UpdatedBy],[AssignedRecruiter]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
CREATE NONCLUSTERED INDEX [IX_Requisitions_CreatedBy_CreatedDate] ON [Requisitions]
(
	[CreatedBy] ASC
)
INCLUDE([CreatedDate]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
CREATE NONCLUSTERED INDEX [IX_Requisitions_Status_AllOthers] ON [Requisitions]
(
	[Status] ASC
)
INCLUDE([Id],[Code],[CompanyId],[PosTitle],[JobOption],[CreatedBy],[CreatedDate],[UpdatedBy],[UpdatedDate],[Due],[AssignedRecruiter]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
CREATE NONCLUSTERED INDEX [IX_Requssitions_Code_Title_Status] ON [Requisitions]
(
	[Code] ASC,
	[PosTitle] ASC,
	[Status] ASC,
	[CreatedBy] ASC,
	[CreatedDate] ASC,
	[Due] ASC
)
INCLUDE([Id],[CompanyId],[JobOption]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
CREATE NONCLUSTERED INDEX [ReqView_All] ON [RequisitionView_old]
(
	[Code] ASC,
	[Title] ASC,
	[Company] ASC,
	[JobOption] ASC,
	[JobOptions] ASC,
	[Status] ASC,
	[Updated] ASC,
	[UpdatedBy] ASC,
	[CreatedBy] ASC,
	[DueDate] ASC,
	[Icon] ASC,
	[IsHot] ASC,
	[SubmitCandidate] ASC,
	[CanUpdate] ASC,
	[ChangeStatus] ASC,
	[PriorityColor] ASC,
	[AssignedRecruiter] ASC,
	[RoleID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
CREATE NONCLUSTERED INDEX [IX_Submissing_Missing_Dash] ON [Submissions]
(
	[CreatedBy] ASC,
	[CreatedDate] ASC
)
INCLUDE([RequisitionId]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Submission_CandidateId] ON [Submissions]
(
	[CandidateId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
CREATE NONCLUSTERED INDEX [IX_Submission_ReqId_Status] ON [Submissions]
(
	[RequisitionId] ASC,
	[Status] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
CREATE NONCLUSTERED INDEX [IX_Submissions_CreatedBy_CreatedDate] ON [Submissions]
(
	[CreatedBy] ASC
)
INCLUDE([CreatedDate]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
CREATE NONCLUSTERED INDEX [IX_Submissions_Missing_Dash2] ON [Submissions]
(
	[CreatedBy] ASC
)
INCLUDE([RequisitionId],[CandidateId],[CreatedDate]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
CREATE NONCLUSTERED INDEX [IX_Submissions_Status_Calendar_DateTime] ON [Submissions]
(
	[Status] ASC,
	[ShowCalendar] ASC,
	[DateTime] ASC
)
INCLUDE([RequisitionId],[CandidateId],[CreatedBy],[UpdatedBy]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
CREATE NONCLUSTERED INDEX [<Name of Missing Index, sysname,>] ON [ZipCodes]
(
	[StateAbbreviation] ASC
)
INCLUDE([City]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [ActiveUsersView] ADD  DEFAULT ((0)) FOR [RequisitionCount]
GO
ALTER TABLE [ActiveUsersView] ADD  DEFAULT (getdate()) FOR [RefreshDate]
GO
ALTER TABLE [Candidate] ADD  CONSTRAINT [DF__Candidate__Addre__2A164134_p]  DEFAULT ('') FOR [Address1]
GO
ALTER TABLE [Candidate] ADD  CONSTRAINT [DF__Candidate__Email__2B0A656D_p]  DEFAULT ('') FOR [Email]
GO
ALTER TABLE [Candidate] ADD  CONSTRAINT [DF__Candidate__Phone__2BFE89A6_p]  DEFAULT ('') FOR [Phone1]
GO
ALTER TABLE [Candidate] ADD  CONSTRAINT [DF_Candidate_EligibilityId_p]  DEFAULT ((3)) FOR [EligibilityId]
GO
ALTER TABLE [Candidate] ADD  CONSTRAINT [DF_Candidate_ExperienceId_p]  DEFAULT ((1)) FOR [ExperienceId]
GO
ALTER TABLE [Candidate] ADD  CONSTRAINT [DF_Candidate_Experience_p]  DEFAULT ((0)) FOR [Experience]
GO
ALTER TABLE [Candidate] ADD  CONSTRAINT [DF_Candidate_JobOptions_p]  DEFAULT ('N') FOR [JobOptions]
GO
ALTER TABLE [Candidate] ADD  CONSTRAINT [DF_Candidate_Communication_p]  DEFAULT ('G') FOR [Communication]
GO
ALTER TABLE [Candidate] ADD  CONSTRAINT [DF__Candidate__TaxTe__00AA174D_p]  DEFAULT ('') FOR [TaxTerm]
GO
ALTER TABLE [Candidate] ADD  CONSTRAINT [DF_Candidate_SalaryLow_p]  DEFAULT ((0)) FOR [SalaryLow]
GO
ALTER TABLE [Candidate] ADD  CONSTRAINT [DF_Candidate_SalaryHigh_p]  DEFAULT ((0)) FOR [SalaryHigh]
GO
ALTER TABLE [Candidate] ADD  CONSTRAINT [DF_Candidate_HourlyRate_p]  DEFAULT ((0)) FOR [HourlyRate]
GO
ALTER TABLE [Candidate] ADD  CONSTRAINT [DF__Candidate__Hourl__075714DC_p]  DEFAULT ((0)) FOR [HourlyRateHigh]
GO
ALTER TABLE [Candidate] ADD  CONSTRAINT [DF_Candidate_VendorId_p]  DEFAULT ((0)) FOR [VendorId]
GO
ALTER TABLE [Candidate] ADD  CONSTRAINT [DF_Candidate_Relocate_p]  DEFAULT ((0)) FOR [Relocate]
GO
ALTER TABLE [Candidate] ADD  CONSTRAINT [DF__Candidate__Backg__038683F8_p]  DEFAULT ((0)) FOR [Background]
GO
ALTER TABLE [Candidate] ADD  CONSTRAINT [DF_Candidate_Keywords_p]  DEFAULT ('') FOR [Keywords]
GO
ALTER TABLE [Candidate] ADD  CONSTRAINT [DF__Candidate__Summa__1699586C_p]  DEFAULT ('') FOR [Summary]
GO
ALTER TABLE [Candidate] ADD  CONSTRAINT [DF_Candidate_ExperienceSummary_p]  DEFAULT ('') FOR [ExperienceSummary]
GO
ALTER TABLE [Candidate] ADD  CONSTRAINT [DF__Candidate__Objec__178D7CA5_p]  DEFAULT ('') FOR [Objective]
GO
ALTER TABLE [Candidate] ADD  CONSTRAINT [DF_Candidate_RateCandidate_p]  DEFAULT ((3)) FOR [RateCandidate]
GO
ALTER TABLE [Candidate] ADD  CONSTRAINT [DF_Candidate_MPC_p]  DEFAULT ((0)) FOR [MPC]
GO
ALTER TABLE [Candidate] ADD  CONSTRAINT [DF_Candidate_Status_p]  DEFAULT ('AVL') FOR [Status]
GO
ALTER TABLE [Candidate] ADD  CONSTRAINT [DF__Candidate__Refer__65F62111_p]  DEFAULT ((0)) FOR [Refer]
GO
ALTER TABLE [Candidate] ADD  CONSTRAINT [DF__Candidate__EEO__1881A0DE_p]  DEFAULT ((0)) FOR [EEO]
GO
ALTER TABLE [Candidate] ADD  CONSTRAINT [DF_Candidate_JsonFileName_p]  DEFAULT ('') FOR [JsonFileName]
GO
ALTER TABLE [Candidate] ADD  CONSTRAINT [DF_CONSULTANT_CreatedDate_p]  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [Candidate] ADD  CONSTRAINT [DF_CONSULTANT_UpdatedDate_p]  DEFAULT (getdate()) FOR [UpdatedDate]
GO
ALTER TABLE [CandidateDocument] ADD  CONSTRAINT [DF_CandidateDocument_DocumentType]  DEFAULT ((5)) FOR [DocumentType]
GO
ALTER TABLE [CandidateDocument] ADD  CONSTRAINT [DF_CandidateDocument_InternalFileName]  DEFAULT (replace(newid(),'-','')) FOR [InternalFileName]
GO
ALTER TABLE [CandidateDocument] ADD  CONSTRAINT [DF_CandidateDocument_UpdatedDate]  DEFAULT (getdate()) FOR [LastUpdatedDate]
GO
ALTER TABLE [CandidateEducation] ADD  CONSTRAINT [DF__Candidate__Creat__625A9A57]  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [CandidateEducation] ADD  CONSTRAINT [DF__Candidate__Updat__634EBE90]  DEFAULT (getdate()) FOR [UpdatedDate]
GO
ALTER TABLE [CandidateEmployer] ADD  CONSTRAINT [DF_CandidateEmployer_Employer]  DEFAULT ('') FOR [Employer]
GO
ALTER TABLE [CandidateEmployer] ADD  CONSTRAINT [DF_CandidateEmployer_CreatedDate]  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [CandidateEmployer] ADD  CONSTRAINT [DF_CandidateEmployer_UpdatedDate]  DEFAULT (getdate()) FOR [UpdatedDate]
GO
ALTER TABLE [CandidateQualityMetricsView] ADD  DEFAULT ((0)) FOR [Total_Submissions]
GO
ALTER TABLE [CandidateQualityMetricsView] ADD  DEFAULT ((0)) FOR [Reached_INT]
GO
ALTER TABLE [CandidateQualityMetricsView] ADD  DEFAULT ((0)) FOR [Reached_OEX]
GO
ALTER TABLE [CandidateQualityMetricsView] ADD  DEFAULT ((0)) FOR [Reached_HIR]
GO
ALTER TABLE [CandidateQualityMetricsView] ADD  DEFAULT ((0.00)) FOR [PEN_to_INT_Ratio]
GO
ALTER TABLE [CandidateQualityMetricsView] ADD  DEFAULT ((0.00)) FOR [INT_to_OEX_Ratio]
GO
ALTER TABLE [CandidateQualityMetricsView] ADD  DEFAULT ((0.00)) FOR [OEX_to_HIR_Ratio]
GO
ALTER TABLE [CandidateQualityMetricsView] ADD  DEFAULT (getdate()) FOR [RefreshDate]
GO
ALTER TABLE [Companies] ADD  CONSTRAINT [DF_Companies_EIN#]  DEFAULT ('') FOR [EIN]
GO
ALTER TABLE [Companies] ADD  CONSTRAINT [DF_Companies_Status]  DEFAULT ((1)) FOR [Status]
GO
ALTER TABLE [Companies] ADD  CONSTRAINT [DF_Companies_CreatedBy]  DEFAULT ('ADMIN') FOR [CreatedBy]
GO
ALTER TABLE [Companies] ADD  CONSTRAINT [DF_Companies_CreatedDate]  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [Companies] ADD  CONSTRAINT [DF_Companies_UpdatedBy]  DEFAULT ('ADMIN') FOR [UpdatedBy]
GO
ALTER TABLE [Companies] ADD  CONSTRAINT [DF_Companies_UpdatedDate]  DEFAULT (getdate()) FOR [UpdatedDate]
GO
ALTER TABLE [CompanyContacts] ADD  CONSTRAINT [DF_CompanyContacts_PrimaryContact]  DEFAULT ((0)) FOR [PrimaryContact]
GO
ALTER TABLE [CompanyContacts] ADD  CONSTRAINT [DF_CompanyContacts_CreatedBy]  DEFAULT ('ADMIN') FOR [CreatedBy]
GO
ALTER TABLE [CompanyContacts] ADD  CONSTRAINT [DF_CompanyContacts_CreatedDate]  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [CompanyContacts] ADD  CONSTRAINT [DF_CompanyContacts_UpdatedBy]  DEFAULT ('ADMIN') FOR [UpdatedBy]
GO
ALTER TABLE [CompanyContacts] ADD  CONSTRAINT [DF_CompanyContacts_UpdatedDate]  DEFAULT (getdate()) FOR [UpdatedDate]
GO
ALTER TABLE [CompanyDocuments] ADD  CONSTRAINT [DF_CompanyDocuments_CompanyID]  DEFAULT ((0)) FOR [CompanyID]
GO
ALTER TABLE [CompanyDocuments] ADD  CONSTRAINT [DF_CompanyDocuments_DocumentName]  DEFAULT ('') FOR [DocumentName]
GO
ALTER TABLE [CompanyDocuments] ADD  CONSTRAINT [DF_CompanyDocuments_OriginalFileName]  DEFAULT ('') FOR [OriginalFileName]
GO
ALTER TABLE [CompanyDocuments] ADD  CONSTRAINT [DF_CompanyDocuments_InternalFileName]  DEFAULT ('') FOR [InternalFileName]
GO
ALTER TABLE [CompanyDocuments] ADD  CONSTRAINT [DF_CompanyDocuments_Notes]  DEFAULT ('') FOR [Notes]
GO
ALTER TABLE [CompanyDocuments] ADD  CONSTRAINT [DF_Table_1_LastUpdatedDate]  DEFAULT (getdate()) FOR [UpdatedDate]
GO
ALTER TABLE [CompanyDocuments] ADD  CONSTRAINT [DF_CompanyDocuments_UpdatedBy]  DEFAULT ('ADMIN') FOR [UpdatedBy]
GO
ALTER TABLE [CompanyLocations] ADD  CONSTRAINT [DF_CompanyLocations_IsPrimaryLocation]  DEFAULT ((1)) FOR [IsPrimaryLocation]
GO
ALTER TABLE [CompanyLocations] ADD  CONSTRAINT [DF_CompanyLocations_CreatedBy]  DEFAULT ('ADMIN') FOR [CreatedBy]
GO
ALTER TABLE [CompanyLocations] ADD  CONSTRAINT [DF_CompanyLocations_CreatedDate]  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [CompanyLocations] ADD  CONSTRAINT [DF_CompanyLocations_UpdatedBy]  DEFAULT ('ADMIN') FOR [UpdatedBy]
GO
ALTER TABLE [CompanyLocations] ADD  CONSTRAINT [DF_CompanyLocations_UpdatedDate]  DEFAULT (getdate()) FOR [UpdatedDate]
GO
ALTER TABLE [DateRangeLookup] ADD  DEFAULT (getdate()) FOR [RefreshDate]
GO
ALTER TABLE [LeadDocuments] ADD  CONSTRAINT [DF_LeadDocuments_UpdatedBy]  DEFAULT ('ADMIN') FOR [UpdatedBy]
GO
ALTER TABLE [LeadDocuments] ADD  CONSTRAINT [DF_LeadDocuments_UpdatedDate]  DEFAULT (getdate()) FOR [UpdatedDate]
GO
ALTER TABLE [Leads] ADD  CONSTRAINT [DF_Leads_Status]  DEFAULT ((1)) FOR [Status]
GO
ALTER TABLE [Leads] ADD  CONSTRAINT [DF_Leads_Website]  DEFAULT ('') FOR [Website]
GO
ALTER TABLE [Leads] ADD  CONSTRAINT [DF_Leads_NoEmployees]  DEFAULT ((0)) FOR [NoEmployees]
GO
ALTER TABLE [Leads] ADD  CONSTRAINT [DF_Leads_Revenue]  DEFAULT ((0)) FOR [Revenue]
GO
ALTER TABLE [Leads] ADD  CONSTRAINT [DF_Leads_LeadSource]  DEFAULT ((0)) FOR [LeadSource]
GO
ALTER TABLE [Leads] ADD  CONSTRAINT [DF_Leads_Industry]  DEFAULT ((0)) FOR [Industry]
GO
ALTER TABLE [Leads] ADD  CONSTRAINT [DF_Leads_CreatedDate]  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [Leads] ADD  CONSTRAINT [DF_Leads_UpdatedDate]  DEFAULT (getdate()) FOR [UpdatedDate]
GO
ALTER TABLE [Notes] ADD  CONSTRAINT [DF_ENTITY_Notes_IS_PRIMARY]  DEFAULT ((0)) FOR [IsPrimary]
GO
ALTER TABLE [Notes] ADD  CONSTRAINT [DF_Notes_CreatedDate]  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [Notes] ADD  CONSTRAINT [DF_ENTITY_Notes_UpdatedDate]  DEFAULT (getdate()) FOR [UpdatedDate]
GO
ALTER TABLE [PlacementReportView] ADD  DEFAULT ((0.00)) FOR [SalaryOffered]
GO
ALTER TABLE [PlacementReportView] ADD  DEFAULT (getdate()) FOR [StartDate]
GO
ALTER TABLE [PlacementReportView] ADD  DEFAULT (getdate()) FOR [DateInvoiced]
GO
ALTER TABLE [PlacementReportView] ADD  DEFAULT (getdate()) FOR [DatePaid]
GO
ALTER TABLE [PlacementReportView] ADD  DEFAULT ((0.00)) FOR [PlacementFee]
GO
ALTER TABLE [PlacementReportView] ADD  DEFAULT ((0.00)) FOR [CommissionPercent]
GO
ALTER TABLE [PlacementReportView] ADD  DEFAULT ((0.00)) FOR [CommissionEarned]
GO
ALTER TABLE [PlacementReportView] ADD  DEFAULT (getdate()) FOR [RefreshDate]
GO
ALTER TABLE [Preferences] ADD  CONSTRAINT [DF_Preferences_ReqPriorityHigh]  DEFAULT ('#ff0000') FOR [ReqPriorityHigh]
GO
ALTER TABLE [Preferences] ADD  CONSTRAINT [DF_Preferences_ReqPriorityNormal]  DEFAULT ('#000000') FOR [ReqPriorityNormal]
GO
ALTER TABLE [Preferences] ADD  CONSTRAINT [DF_Preferences_ReqPriorityLow]  DEFAULT ('#008000') FOR [ReqPriorityLow]
GO
ALTER TABLE [Preferences] ADD  CONSTRAINT [DF_Preferences_AllRecruiters]  DEFAULT ((1)) FOR [AllRecruitersSubmitCandidate]
GO
ALTER TABLE [Preferences] ADD  CONSTRAINT [DF_Preferences_AdminCandidates]  DEFAULT ((1)) FOR [AdminCandidates]
GO
ALTER TABLE [Preferences] ADD  CONSTRAINT [DF_Preferences_AdminRequisitions]  DEFAULT ((1)) FOR [AdminRequisitions]
GO
ALTER TABLE [Preferences] ADD  CONSTRAINT [DF_Preferences_ReqStatusChange]  DEFAULT ((1)) FOR [ReqStatusChange]
GO
ALTER TABLE [Preferences] ADD  CONSTRAINT [DF_Preferences_CandStatusChange]  DEFAULT ((1)) FOR [CandStatusChange]
GO
ALTER TABLE [Preferences] ADD  CONSTRAINT [DF_Preferences_ChangeCandidateSubmissionStatus]  DEFAULT ((1)) FOR [ChangeCandidateSubmissionStatus]
GO
ALTER TABLE [Preferences] ADD  CONSTRAINT [DF_Preferences_PageSize]  DEFAULT ((50)) FOR [PageSize]
GO
ALTER TABLE [Preferences] ADD  CONSTRAINT [DF_Preferences_SortReqonPriority]  DEFAULT ((0)) FOR [SortReqonPriority]
GO
ALTER TABLE [RecentActivityView] ADD  DEFAULT (getdate()) FOR [RefreshDate]
GO
ALTER TABLE [RequisitionDocument] ADD  CONSTRAINT [DF_RequisitionDocument_UpdatedDate]  DEFAULT (getdate()) FOR [LastUpdatedDate]
GO
ALTER TABLE [Requisitions] ADD  CONSTRAINT [DF__Requisiti__Hirin__5DB5E0CB]  DEFAULT ((0)) FOR [HiringMgrId]
GO
ALTER TABLE [Requisitions] ADD  CONSTRAINT [DF__Requisiti__Posit__5EAA0504]  DEFAULT ((1)) FOR [Positions]
GO
ALTER TABLE [Requisitions] ADD  CONSTRAINT [DF__Requisiti__Durat__5F9E293D]  DEFAULT ('M') FOR [DurationCode]
GO
ALTER TABLE [Requisitions] ADD  CONSTRAINT [DF__Requisiti__ExpRa__60924D76]  DEFAULT ((0)) FOR [ExpRateLow]
GO
ALTER TABLE [Requisitions] ADD  CONSTRAINT [DF__Requisiti__ExpRa__618671AF]  DEFAULT ((0)) FOR [ExpRateHigh]
GO
ALTER TABLE [Requisitions] ADD  CONSTRAINT [DF__Requisiti__ExpLo__627A95E8]  DEFAULT ((0)) FOR [ExpLoadLow]
GO
ALTER TABLE [Requisitions] ADD  CONSTRAINT [DF__Requisiti__ExpLo__636EBA21]  DEFAULT ((0)) FOR [ExpLoadHigh]
GO
ALTER TABLE [Requisitions] ADD  CONSTRAINT [DF__Requisiti__Place__6462DE5A]  DEFAULT ((0)) FOR [PlacementFee]
GO
ALTER TABLE [Requisitions] ADD  CONSTRAINT [DF__Requisiti__Place__65570293]  DEFAULT ((0)) FOR [PlacementType]
GO
ALTER TABLE [Requisitions] ADD  CONSTRAINT [DF__Requisiti__JobOp__664B26CC]  DEFAULT ('C') FOR [JobOption]
GO
ALTER TABLE [Requisitions] ADD  CONSTRAINT [DF__Requisiti__Salar__673F4B05]  DEFAULT ((0)) FOR [SalaryLow]
GO
ALTER TABLE [Requisitions] ADD  CONSTRAINT [DF__Requisiti__Salar__68336F3E]  DEFAULT ((0)) FOR [SalaryHigh]
GO
ALTER TABLE [Requisitions] ADD  CONSTRAINT [DF__Requisiti__ExpPa__69279377]  DEFAULT ((0)) FOR [ExpPaid]
GO
ALTER TABLE [Requisitions] ADD  CONSTRAINT [DF__Requisiti__ExpSt__6A1BB7B0]  DEFAULT (getdate()+(3)) FOR [ExpStart]
GO
ALTER TABLE [Requisitions] ADD  CONSTRAINT [DF__Requisiti__Statu__6B0FDBE9]  DEFAULT ('NEW') FOR [Status]
GO
ALTER TABLE [Requisitions] ADD  CONSTRAINT [DF__Requisiti__Alert__6C040022]  DEFAULT ((0)) FOR [AlertOn]
GO
ALTER TABLE [Requisitions] ADD  CONSTRAINT [DF__Requisiti__Alert__6CF8245B]  DEFAULT ((24)) FOR [AlertTimeout]
GO
ALTER TABLE [Requisitions] ADD  CONSTRAINT [DF__Requisiti__Alert__6DEC4894]  DEFAULT ((24)) FOR [AlertRepFreq]
GO
ALTER TABLE [Requisitions] ADD  CONSTRAINT [DF__Requisiti__Alert__6EE06CCD]  DEFAULT (getdate()) FOR [AlertEnd]
GO
ALTER TABLE [Requisitions] ADD  CONSTRAINT [DF__Requisiti__Alert__6FD49106]  DEFAULT ((0)) FOR [AlertMail]
GO
ALTER TABLE [Requisitions] ADD  CONSTRAINT [DF__Requisiti__IsHot__70C8B53F]  DEFAULT ((1)) FOR [IsHot]
GO
ALTER TABLE [Requisitions] ADD  CONSTRAINT [DF__Requisiti__Creat__71BCD978]  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [Requisitions] ADD  CONSTRAINT [DF__Requisiti__Updat__72B0FDB1]  DEFAULT (getdate()) FOR [UpdatedDate]
GO
ALTER TABLE [Requisitions] ADD  CONSTRAINT [DF__Requisiti__Secur__74994623]  DEFAULT ((0)) FOR [SecurityClearance]
GO
ALTER TABLE [Requisitions] ADD  CONSTRAINT [DF__Requisiti__Secur__758D6A5C]  DEFAULT ((0)) FOR [SecurityType]
GO
ALTER TABLE [Requisitions] ADD  CONSTRAINT [DF__Requisiti__Benef__76818E95]  DEFAULT ((0)) FOR [Benefits]
GO
ALTER TABLE [Requisitions] ADD  CONSTRAINT [DF__Requisiti__OFCCP__7775B2CE]  DEFAULT ((0)) FOR [OFCCP]
GO
ALTER TABLE [Requisitions] ADD  CONSTRAINT [DF__Requisiti__Attac__7869D707]  DEFAULT ('.doc') FOR [AttachFileType]
GO
ALTER TABLE [Requisitions] ADD  CONSTRAINT [DF__Requisiti__Benef__795DFB40]  DEFAULT ('.doc') FOR [BenefitFileType]
GO
ALTER TABLE [Requisitions] ADD  CONSTRAINT [DF__Requisiti__State__7A521F79]  DEFAULT ((1)) FOR [StateId]
GO
ALTER TABLE [Requisitions] ADD  CONSTRAINT [DF__Requisiti__Attac__7B4643B2]  DEFAULT ('.doc') FOR [AttachFileType2]
GO
ALTER TABLE [Requisitions] ADD  CONSTRAINT [DF__Requisiti__Attac__7C3A67EB]  DEFAULT ('.doc') FOR [AttachFileType3]
GO
ALTER TABLE [Requisitions] ADD  CONSTRAINT [DF_Requisitions_PrimaryRecruiter]  DEFAULT ('') FOR [AssignedRecruiter]
GO
ALTER TABLE [Requisitions] ADD  CONSTRAINT [DF_Requisitions_SecondaryRecruiter]  DEFAULT ((0)) FOR [SecondaryRecruiter]
GO
ALTER TABLE [Requisitions] ADD  CONSTRAINT [DF_Requisitions_MandatoryRequirement]  DEFAULT ('') FOR [MandatoryRequirement]
GO
ALTER TABLE [Requisitions] ADD  CONSTRAINT [DF_Requisitions_PreferredRequirement]  DEFAULT ('') FOR [PreferredRequirement]
GO
ALTER TABLE [Requisitions] ADD  CONSTRAINT [DF_Requisitions_OptionalRequirement]  DEFAULT ('') FOR [OptionalRequirement]
GO
ALTER TABLE [RequisitionStatus] ADD  CONSTRAINT [DF_RequisitionStatus_CreatedOn]  DEFAULT (getdate()) FOR [CreatedOn]
GO
ALTER TABLE [RequisitionStatus] ADD  CONSTRAINT [DF_RequisitionStatus_UpdatedOn]  DEFAULT (getdate()) FOR [UpdatedOn]
GO
ALTER TABLE [RequisitionView] ADD  DEFAULT ((0)) FOR [SubmissionCount]
GO
ALTER TABLE [Roles] ADD  CONSTRAINT [DF_Roles_CreateOrEditCompany]  DEFAULT ((0)) FOR [CreateOrEditCompany]
GO
ALTER TABLE [Roles] ADD  CONSTRAINT [DF_Roles_CreateOrEditCandidate]  DEFAULT ((0)) FOR [CreateOrEditCandidate]
GO
ALTER TABLE [Roles] ADD  CONSTRAINT [DF_Roles_ViewAllCompanies]  DEFAULT ((0)) FOR [ViewAllCompanies]
GO
ALTER TABLE [Roles] ADD  CONSTRAINT [DF_Roles_ViewMyCompanyProfile]  DEFAULT ((0)) FOR [ViewMyCompanyProfile]
GO
ALTER TABLE [Roles] ADD  CONSTRAINT [DF_Roles_EditMyCompanyProfile]  DEFAULT ((0)) FOR [EditMyCompanyProfile]
GO
ALTER TABLE [Roles] ADD  CONSTRAINT [DF_Roles_CreateOrEditRequisitions]  DEFAULT ((0)) FOR [CreateOrEditRequisitions]
GO
ALTER TABLE [Roles] ADD  CONSTRAINT [DF_Roles_ViewOnlyMyCandidates]  DEFAULT ((1)) FOR [ViewOnlyMyCandidates]
GO
ALTER TABLE [Roles] ADD  CONSTRAINT [DF_Roles_ViewAllCandidates]  DEFAULT ((0)) FOR [ViewAllCandidates]
GO
ALTER TABLE [Roles] ADD  CONSTRAINT [DF_Roles_ViewRequisitions]  DEFAULT ((0)) FOR [ViewRequisitions]
GO
ALTER TABLE [Roles] ADD  CONSTRAINT [DF_Roles_EditRequisitions]  DEFAULT ((0)) FOR [EditRequisitions]
GO
ALTER TABLE [Roles] ADD  CONSTRAINT [DF_Roles_ManageSubmittedCandidates]  DEFAULT ((0)) FOR [ManageSubmittedCandidates]
GO
ALTER TABLE [Roles] ADD  CONSTRAINT [DEFAULT_Roles_DownloadOriginal]  DEFAULT ((0)) FOR [DownloadOriginal]
GO
ALTER TABLE [Roles] ADD  CONSTRAINT [DEFAULT_Roles_DownloadFormatted]  DEFAULT ((0)) FOR [DownloadFormatted]
GO
ALTER TABLE [Roles] ADD  CONSTRAINT [DF_Roles_AdminScreens]  DEFAULT ((0)) FOR [AdminScreens]
GO
ALTER TABLE [Roles] ADD  CONSTRAINT [DF_Roles_CreatedBy]  DEFAULT ('ADMIN') FOR [CreatedBy]
GO
ALTER TABLE [Roles] ADD  CONSTRAINT [DF_Roles_CreatedDate]  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [Roles] ADD  CONSTRAINT [DF_Roles_UpdatedBy]  DEFAULT ('ADMIN') FOR [UpdatedBy]
GO
ALTER TABLE [Roles] ADD  CONSTRAINT [DF_Roles_UpdatedDate]  DEFAULT (getdate()) FOR [UpdatedDate]
GO
ALTER TABLE [SentMails] ADD  CONSTRAINT [DF_SentMails_SentDate]  DEFAULT (getdate()) FOR [SentDate]
GO
ALTER TABLE [SubmissionCommission] ADD  CONSTRAINT [DF_SubmissionCommission_JobOptions]  DEFAULT ('T') FOR [JobOptions]
GO
ALTER TABLE [SubmissionCommission] ADD  CONSTRAINT [DF_SubmissionCommission_ClientRate]  DEFAULT ((0)) FOR [ClientRate]
GO
ALTER TABLE [SubmissionCommission] ADD  CONSTRAINT [DF_SubmissionCommission_HourlyRate]  DEFAULT ((0)) FOR [CommissionPercent]
GO
ALTER TABLE [SubmissionCommission] ADD  CONSTRAINT [DF_SubmissionCommission_Hours]  DEFAULT ((1)) FOR [Hours]
GO
ALTER TABLE [SubmissionCommission] ADD  CONSTRAINT [DF_SubmissionCommission_CostPercent]  DEFAULT ((1)) FOR [CostPercent]
GO
ALTER TABLE [SubmissionCommission] ADD  CONSTRAINT [DF_SubmissionCommission_CandidatePayRate]  DEFAULT ((0)) FOR [CandidatePayRate]
GO
ALTER TABLE [SubmissionCommission] ADD  CONSTRAINT [DF_SubmissionCommission_Spread]  DEFAULT ((1)) FOR [Spread]
GO
ALTER TABLE [SubmissionCommission] ADD  CONSTRAINT [DF_SubmissionCommission_CommissionSpread]  DEFAULT ((0)) FOR [CommissionSpread]
GO
ALTER TABLE [SubmissionCommission] ADD  CONSTRAINT [DF_SubmissionCommission_Commission]  DEFAULT ((0)) FOR [Commission]
GO
ALTER TABLE [SubmissionCommission] ADD  CONSTRAINT [DF_SubmissionCommission_Points]  DEFAULT ((0)) FOR [Points]
GO
ALTER TABLE [SubmissionCommission] ADD  CONSTRAINT [DF_SubmissionCommission_StartDate]  DEFAULT (getdate()) FOR [StartDate]
GO
ALTER TABLE [SubmissionMetricsView] ADD  DEFAULT ((0)) FOR [INT_Count]
GO
ALTER TABLE [SubmissionMetricsView] ADD  DEFAULT ((0)) FOR [OEX_Count]
GO
ALTER TABLE [SubmissionMetricsView] ADD  DEFAULT ((0)) FOR [HIR_Count]
GO
ALTER TABLE [SubmissionMetricsView] ADD  DEFAULT ((0)) FOR [Total_Submissions]
GO
ALTER TABLE [SubmissionMetricsView] ADD  DEFAULT ((0)) FOR [OEX_HIR_Ratio]
GO
ALTER TABLE [SubmissionMetricsView] ADD  DEFAULT ((0)) FOR [Requisitions_Created]
GO
ALTER TABLE [SubmissionMetricsView] ADD  DEFAULT ((0)) FOR [Active_Requisitions_Updated]
GO
ALTER TABLE [SubmissionMetricsView] ADD  DEFAULT (getdate()) FOR [RefreshDate]
GO
ALTER TABLE [Submissions] ADD  CONSTRAINT [DF_Submissions_CreatedDate]  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [Submissions] ADD  CONSTRAINT [DF_Submissions_UpdatedDate]  DEFAULT (getdate()) FOR [UpdatedDate]
GO
ALTER TABLE [Submissions] ADD  CONSTRAINT [DF_Submissions_StatusId]  DEFAULT ((1)) FOR [StatusId]
GO
ALTER TABLE [Submissions] ADD  CONSTRAINT [DF_Submissions_ShowCalendar]  DEFAULT ((0)) FOR [ShowCalendar]
GO
ALTER TABLE [Submissions] ADD  CONSTRAINT [DF_Submissions_Type]  DEFAULT ('P') FOR [Type]
GO
ALTER TABLE [Submissions] ADD  CONSTRAINT [DF_Submissions_Undone]  DEFAULT ((0)) FOR [Undone]
GO
ALTER TABLE [SubmissionStatus] ADD  CONSTRAINT [DF_SubmissionStatus_ShowCalendar]  DEFAULT ((0)) FOR [ShowCalendar]
GO
ALTER TABLE [SubmissionStatus] ADD  CONSTRAINT [DF_SubmissionStatus_Type]  DEFAULT ('P') FOR [Type]
GO
ALTER TABLE [SubmissionStatus] ADD  CONSTRAINT [DF_SubmissionStatus_CreatedDate]  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [SubmissionStatus] ADD  CONSTRAINT [DF_SubmissionStatus_UpdatedBy]  DEFAULT (getdate()) FOR [UpdatedDate]
GO
ALTER TABLE [Templates] ADD  CONSTRAINT [DF_Templates_Action]  DEFAULT ((1)) FOR [Action]
GO
ALTER TABLE [Templates] ADD  CONSTRAINT [DF_Templates_SendTo]  DEFAULT ('Administrator,Sales Manager,Recruiter,Full Desk,Requisition Owner,Candidate Owner,Requisition Assigned,Everyone,Others') FOR [SendTo]
GO
ALTER TABLE [Templates] ADD  CONSTRAINT [DF_Templates_CreatedOn]  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [Templates] ADD  CONSTRAINT [DF_Templates_UpdatedOn]  DEFAULT (getdate()) FOR [UpdatedDate]
GO
ALTER TABLE [Templates] ADD  CONSTRAINT [DF_Templates_IncludeResume]  DEFAULT ((0)) FOR [IncludeResume]
GO
ALTER TABLE [Templates] ADD  CONSTRAINT [DF_Templates_Enabled]  DEFAULT ((1)) FOR [Enabled]
GO
ALTER TABLE [TimingAnalyticsView] ADD  DEFAULT ((0)) FOR [PEN_Days]
GO
ALTER TABLE [TimingAnalyticsView] ADD  DEFAULT ((0)) FOR [REJ_Days]
GO
ALTER TABLE [TimingAnalyticsView] ADD  DEFAULT ((0)) FOR [HLD_Days]
GO
ALTER TABLE [TimingAnalyticsView] ADD  DEFAULT ((0)) FOR [PHN_Days]
GO
ALTER TABLE [TimingAnalyticsView] ADD  DEFAULT ((0)) FOR [URW_Days]
GO
ALTER TABLE [TimingAnalyticsView] ADD  DEFAULT ((0)) FOR [INT_Days]
GO
ALTER TABLE [TimingAnalyticsView] ADD  DEFAULT ((0)) FOR [RHM_Days]
GO
ALTER TABLE [TimingAnalyticsView] ADD  DEFAULT ((0)) FOR [DEC_Days]
GO
ALTER TABLE [TimingAnalyticsView] ADD  DEFAULT ((0)) FOR [NOA_Days]
GO
ALTER TABLE [TimingAnalyticsView] ADD  DEFAULT ((0)) FOR [OEX_Days]
GO
ALTER TABLE [TimingAnalyticsView] ADD  DEFAULT ((0)) FOR [ODC_Days]
GO
ALTER TABLE [TimingAnalyticsView] ADD  DEFAULT ((0)) FOR [HIR_Days]
GO
ALTER TABLE [TimingAnalyticsView] ADD  DEFAULT ((0)) FOR [WDR_Days]
GO
ALTER TABLE [TimingAnalyticsView] ADD  DEFAULT (getdate()) FOR [RefreshDate]
GO
ALTER TABLE [Users] ADD  CONSTRAINT [DF_Users_Prefix]  DEFAULT ('') FOR [Prefix]
GO
ALTER TABLE [Users] ADD  CONSTRAINT [DF_Users_MiddleInitial]  DEFAULT ('') FOR [MiddleInitial]
GO
ALTER TABLE [Users] ADD  CONSTRAINT [DF_Users_Suffix]  DEFAULT ('') FOR [Suffix]
GO
ALTER TABLE [Users] ADD  CONSTRAINT [DF_Users_Status]  DEFAULT ((1)) FOR [Status]
GO
ALTER TABLE [Users] ADD  CONSTRAINT [DF_Users_CreatedBy]  DEFAULT ('ADMIN') FOR [CreatedBy]
GO
ALTER TABLE [Users] ADD  CONSTRAINT [DF_Users_CreatedDate]  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [Users] ADD  CONSTRAINT [DF_Users_UpdatedBy]  DEFAULT ('ADMIN') FOR [UpdatedBy]
GO
ALTER TABLE [Users] ADD  CONSTRAINT [DF_Users_UpdatedDate]  DEFAULT (getdate()) FOR [UpdatedDate]
GO
ALTER TABLE [VariableCommission] ADD  CONSTRAINT [DF_VariableCommission_NoofHours]  DEFAULT ((1920)) FOR [NoofHours]
GO
ALTER TABLE [VariableCommission] ADD  CONSTRAINT [DF_VariableCommission_OverHeadCost]  DEFAULT ((24)) FOR [OverHeadCost]
GO
ALTER TABLE [VariableCommission] ADD  CONSTRAINT [DF_VariableCommission_W2TaxLoadingRate]  DEFAULT ((12)) FOR [W2TaxLoadingRate]
GO
ALTER TABLE [VariableCommission] ADD  CONSTRAINT [DF_VariableCommission_1099CostRate]  DEFAULT ((3)) FOR [1099CostRate]
GO
ALTER TABLE [VariableCommission] ADD  CONSTRAINT [DF_VariableCommission_FTERateOffered]  DEFAULT ((15)) FOR [FTERateOffered]
GO
ALTER TABLE [Users]  WITH CHECK ADD  CONSTRAINT [FK_Users_Roles] FOREIGN KEY([Role])
REFERENCES [Roles] ([ID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [Users] CHECK CONSTRAINT [FK_Users_Roles]
GO
ALTER TABLE [Candidate]  WITH NOCHECK ADD  CONSTRAINT [CK_Candidate_Communication_p] CHECK  (([Communication]='A' OR [Communication]='F' OR [Communication]='G' OR [Communication]='X'))
GO
ALTER TABLE [Candidate] CHECK CONSTRAINT [CK_Candidate_Communication_p]
GO
ALTER TABLE [Candidate]  WITH NOCHECK ADD  CONSTRAINT [CK_Candidate_p] CHECK  (([RateCandidate]=(5) OR [RateCandidate]=(4) OR [RateCandidate]=(3) OR [RateCandidate]=(2) OR [RateCandidate]=(1)))
GO
ALTER TABLE [Candidate] CHECK CONSTRAINT [CK_Candidate_p]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [AddAuditTrail]
	@Action varchar(100),
	@Page varchar(30),
	@Description varchar(7000),
	@InitiatedBy varchar(10)
AS
BEGIN
	SET NOCOUNT ON;
	
	INSERT INTO
		[AuditTrail]
		([Action], [Page], [Description], [InitiatedBy], [InitiatedOn])
	VALUES
		(@Action, @Page, @Description, @InitiatedBy, GETDATE())
END



GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [Admin_CheckDocumentType]
	@ID int = 0,
	@Text varchar(100) = ''
AS
BEGIN
	SET NOCOUNT ON;

	if (@ID = NULL OR @ID = 0)
		BEGIN
			if (EXISTS(SELECT * FROM [Subscription].dbo.[DocumentType] A WHERE A.[DocumentType] = @Text))
				SELECT CAST(1 as bit);
			else
				SELECT CAST(0 as bit);
		END
	else
		BEGIN
			if (EXISTS(SELECT * FROM [Subscription].dbo.[DocumentType] A WHERE A.[DocumentType] = @Text AND A.[Id] <> @ID))
				SELECT CAST(1 as bit);
			else
				SELECT CAST(0 as bit);
		END
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE     PROCEDURE [Admin_CheckEducation]
	@ID int = 0,
	@Text varchar(50) = ''
AS
BEGIN
	SET NOCOUNT ON;

	if (@ID = NULL OR @ID = 0)
		BEGIN
			if (EXISTS(SELECT * FROM [Subscription].dbo.[Education] A WHERE A.[Education] = @Text))
				SELECT CAST(1 as bit);
			else
				SELECT CAST(0 as bit);
		END
	else
		BEGIN
			if (EXISTS(SELECT * FROM [Subscription].dbo.[Education] A WHERE A.[Education] = @Text AND A.[Id] <> @ID))
				SELECT CAST(1 as bit);
			else
				SELECT CAST(0 as bit);
		END
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE     PROCEDURE [Admin_CheckEligibility]
	@ID int = 0,
	@Text varchar(50) = ''
AS
BEGIN
	SET NOCOUNT ON;

	if (@ID = NULL OR @ID = 0)
		BEGIN
			if (EXISTS(SELECT * FROM [Subscription].dbo.[Eligibility] A WHERE A.[Eligibility] = @Text))
				SELECT CAST(1 as bit);
			else
				SELECT CAST(0 as bit);
		END
	else
		BEGIN
			if (EXISTS(SELECT * FROM [Subscription].dbo.[Eligibility] A WHERE A.[Eligibility] = @Text AND A.[Id] <> @ID))
				SELECT CAST(1 as bit);
			else
				SELECT CAST(0 as bit);
		END
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE     PROCEDURE [Admin_CheckExperience]
	@ID int = 0,
	@Text varchar(50) = ''
AS
BEGIN
	SET NOCOUNT ON;

	if (@ID = NULL OR @ID = 0)
		BEGIN
			if (EXISTS(SELECT * FROM [Subscription].dbo.[Experience] A WHERE A.[Experience] = @Text))
				SELECT CAST(1 as bit);
			else
				SELECT CAST(0 as bit);
		END
	else
		BEGIN
			if (EXISTS(SELECT * FROM [Subscription].dbo.[Experience] A WHERE A.[Experience] = @Text AND A.[Id] <> @ID))
				SELECT CAST(1 as bit);
			else
				SELECT CAST(0 as bit);
		END
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE     procedure [Admin_CheckJobCode]
	@Code char(1) = '#'
AS
BEGIN
	SET NOCOUNT ON;

	IF (EXISTS (SELECT * FROM [Subscription].dbo.[JobOptions] A WHERE A.[JobCode] = @Code))
		BEGIN
			SELECT CAST(1 as bit);
		END
	ELSE
		BEGIN
			SELECT CAST(0 as bit);
		END
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE       PROCEDURE [Admin_CheckJobOption]
	@Code char(1) = '',
	@Text varchar(50) = ''
AS
BEGIN
	SET NOCOUNT ON;

	if (@Code = NULL OR @Code = '')
		BEGIN
			if (EXISTS(SELECT * FROM [Subscription].dbo.[JobOptions] A WHERE A.[JobOptions] = @Text))
				SELECT CAST(1 as bit);
			else
				SELECT CAST(0 as bit);
		END
	else
		BEGIN
			if (EXISTS(SELECT * FROM [Subscription].dbo.[JobOptions] A WHERE A.[JobOptions] = @Text AND A.[JobCode] <> @Code))
				SELECT CAST(1 as bit);
			else
				SELECT CAST(0 as bit);
		END
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE     PROCEDURE [Admin_CheckLeadIndustry]
	@ID int = 0,
	@Text varchar(50) = ''
AS
BEGIN
	SET NOCOUNT ON;

	if (@ID = NULL OR @ID = 0)
		BEGIN
			if (EXISTS(SELECT * FROM [Subscription].dbo.[LeadIndustry] A WHERE A.[Industry] = @Text))
				SELECT CAST(1 as bit);
			else
				SELECT CAST(0 as bit);
		END
	else
		BEGIN
			if (EXISTS(SELECT * FROM [Subscription].dbo.[LeadIndustry] A WHERE A.[Industry] = @Text AND A.[Id] <> @ID))
				SELECT CAST(1 as bit);
			else
				SELECT CAST(0 as bit);
		END
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE     PROCEDURE [Admin_CheckLeadSource]
	@ID int = 0,
	@Text varchar(50) = ''
AS
BEGIN
	SET NOCOUNT ON;

	if (@ID = NULL OR @ID = 0)
		BEGIN
			if (EXISTS(SELECT * FROM [ProfessionalMaster].dbo.[LeadSource] A WHERE A.[LeadSource] = @Text))
				SELECT CAST(1 as bit);
			else
				SELECT CAST(0 as bit);
		END
	else
		BEGIN
			if (EXISTS(SELECT * FROM [ProfessionalMaster].dbo.[LeadSource] A WHERE A.[LeadSource] = @Text AND A.[Id] <> @ID))
				SELECT CAST(1 as bit);
			else
				SELECT CAST(0 as bit);
		END
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE     PROCEDURE [Admin_CheckLeadStatus]
	@ID int = 0,
	@Text varchar(50) = ''
AS
BEGIN
	SET NOCOUNT ON;

	if (@ID = NULL OR @ID = 0)
		BEGIN
			if (EXISTS(SELECT * FROM [ProfessionalMaster].dbo.[LeadStatus] A WHERE A.[LeadStatus] = @Text))
				SELECT CAST(1 as bit);
			else
				SELECT CAST(0 as bit);
		END
	else
		BEGIN
			if (EXISTS(SELECT * FROM [ProfessionalMaster].dbo.[LeadStatus] A WHERE A.[LeadStatus] = @Text AND A.[Id] <> @ID))
				SELECT CAST(1 as bit);
			else
				SELECT CAST(0 as bit);
		END
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE       PROCEDURE [Admin_CheckRole]
	@ID char(2) = 'FS',
	@Text varchar(50) = 'Administrator'
AS
BEGIN
	SET NOCOUNT ON;

	if (@ID = NULL OR @ID = '')
		BEGIN
			if (EXISTS(SELECT * FROM [Subscription].dbo.[Roles] A WHERE A.[RoleName] = @Text))
				SELECT CAST(1 as bit);
			else
				SELECT CAST(0 as bit);
		END
	else
		BEGIN
			if (EXISTS(SELECT * FROM [Subscription].dbo.[Roles] A WHERE A.[RoleName] = @Text AND A.[ID] <> @ID))
				SELECT CAST(1 as bit);
			else
				SELECT CAST(0 as bit);
		END
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE       procedure [Admin_CheckRoleId]
	@ID char(2) ='SD'
AS
BEGIN
	SET NOCOUNT ON;

	IF (EXISTS (SELECT * FROM [ProfessionalMaster].dbo.[Roles] A WHERE A.[Id] = @ID))
		BEGIN
			SELECT CAST(1 as bit);
		END
	ELSE
		BEGIN
			SELECT CAST(0 as bit);
		END
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE       PROCEDURE [Admin_CheckSkill]
	@ID int = 0,
	@Text varchar(50) = ''
AS
BEGIN
	SET NOCOUNT ON;

	if (@ID = NULL OR @ID = 0)
		BEGIN
			if (EXISTS(SELECT * FROM [ProfessionalMaster].dbo.[Skills] A WHERE A.[Skill] = @Text))
				SELECT CAST(1 as bit);
			else
				SELECT CAST(0 as bit);
		END
	else
		BEGIN
			if (EXISTS(SELECT * FROM [ProfessionalMaster].dbo.[Skills] A WHERE A.[Skill] = @Text AND A.[Id] <> @ID))
				SELECT CAST(1 as bit);
			else
				SELECT CAST(0 as bit);
		END
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE         PROCEDURE [Admin_CheckState]
	@Code char(2) = '',
	@Text varchar(50) = ''
AS
BEGIN
	SET NOCOUNT ON;

	if (@Code = NULL OR @Code = '')
		BEGIN
			if (EXISTS(SELECT * FROM [Subscription].dbo.[State] A WHERE A.[State] = @Text))
				SELECT CAST(1 as bit);
			else
				SELECT CAST(0 as bit);
		END
	else
		BEGIN
			if (EXISTS(SELECT * FROM [Subscription].dbo.[State] A WHERE A.[State] = @Text AND A.[Code] <> @Code))
				SELECT CAST(1 as bit);
			else
				SELECT CAST(0 as bit);
		END
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE     procedure [Admin_CheckStateCode]
	@Code char(2) = 'AQ'
AS
BEGIN
	SET NOCOUNT ON;

	IF (EXISTS (SELECT * FROM [ProfessionalMaster].dbo.[State] A WHERE A.[Code] = @Code))
		BEGIN
			SELECT CAST(1 as bit);
		END
	ELSE
		BEGIN
			SELECT CAST(0 as bit);
		END
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE         PROCEDURE [Admin_CheckStatus]
	@Code char(3) = '',
	@Text varchar(50) = 'Available',
	@AppliesTo char(3) = 'CND'
AS
BEGIN
	SET NOCOUNT ON;

	if (@Code = NULL OR @Code = '')
		BEGIN
			if (EXISTS(SELECT * FROM [ProfessionalMaster].dbo.[StatusCode] A WHERE A.[Status] = @Text AND A.[AppliesTo] = @AppliesTo ))
				SELECT CAST(1 as bit);
			else
				SELECT CAST(0 as bit);
		END
	else
		BEGIN
			if (EXISTS(SELECT * FROM [ProfessionalMaster].dbo.[StatusCode] A WHERE A.[Status] = @Text AND A.[AppliesTo] = @AppliesTo AND A.[StatusCode] <> @Code))
				SELECT CAST(1 as bit);
			else
				SELECT CAST(0 as bit);
		END
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE     procedure [Admin_CheckStatusCode]
	@Code char(3) = 'ACT',
	@AppliesTo char(3) = 'CLI'
AS
BEGIN
	SET NOCOUNT ON;

	IF (EXISTS (SELECT * FROM [ProfessionalMaster].dbo.[StatusCode] A WHERE A.[StatusCode] = @Code AND A.[AppliesTo] = @AppliesTo))
		BEGIN
			SELECT CAST(1 as bit);
		END
	ELSE
		BEGIN
			SELECT CAST(0 as bit);
		END
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [Admin_CheckTaxTerm]
	@Code char(10) = '1',
	@Text varchar(50) = '1099'
AS
BEGIN
	SET NOCOUNT ON;

	if (@Code = NULL OR @Code = '')
		BEGIN
			if (EXISTS(SELECT * FROM [Subscription].dbo.[TaxTerm] A WHERE A.[TaxTerm] = @Text))
				SELECT CAST(1 as bit);
			else
				SELECT CAST(0 as bit);
		END
	else
		BEGIN
			if (EXISTS(SELECT * FROM [Subscription].dbo.[TaxTerm] A WHERE A.[TaxTerm] = @Text AND A.[TaxTermCode] <> @Code))
				SELECT CAST(1 as bit);
			else
				SELECT CAST(0 as bit);
		END
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE     procedure [Admin_CheckTaxTermCode]
	@Code char(1) = '2'
AS
BEGIN
	SET NOCOUNT ON;

	IF (EXISTS (SELECT * FROM [Subscription].dbo.[TaxTerm] A WHERE A.[TaxTermCode] = @Code))
		BEGIN
			SELECT CAST(1 as bit);
		END
	ELSE
		BEGIN
			SELECT CAST(0 as bit);
		END
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [Admin_CheckTemplateName]
	@ID int = 2002,
	@TemplateName varchar(50) = 'First Template'
AS
BEGIN
	SET NOCOUNT ON;

	if (@ID = NULL OR @ID = '')
		BEGIN
			if (EXISTS(SELECT * FROM dbo.[Templates] A WHERE A.[TemplateName] = @TemplateName))
				SELECT CAST(1 as bit);
			else
				SELECT CAST(0 as bit);
		END
	else
		BEGIN
			if (EXISTS(SELECT * FROM dbo.[Templates] A WHERE A.[TemplateName] = @TemplateName AND A.[Id] <> @ID))
				SELECT CAST(1 as bit);
			else
				SELECT CAST(0 as bit);
		END
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [Admin_CheckTitle]
	@ID int = 0,
	@Text varchar(50) = ''
AS
BEGIN
	SET NOCOUNT ON;

	if (@ID = NULL OR @ID = 0)
		BEGIN
			if (EXISTS(SELECT * FROM [ProfessionalMaster].dbo.Designation A WHERE A.[Designation] = @Text))
				SELECT CAST(1 as bit);
			else
				SELECT CAST(0 as bit);
		END
	else
		BEGIN
			if (EXISTS(SELECT * FROM [ProfessionalMaster].dbo.Designation A WHERE A.[Designation] = @Text AND A.[Id] <> @ID))
				SELECT CAST(1 as bit);
			else
				SELECT CAST(0 as bit);
		END
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [Admin_CheckUserName]
	@ID int = 0, --ID is only for compatability purpose
	@Text varchar(100) = ''
AS
BEGIN
	SET NOCOUNT ON;

	if (EXISTS(SELECT * FROM dbo.[Users] A WHERE A.[UserName] = @Text))
		SELECT CAST(1 as bit);
	else
		SELECT CAST(0 as bit);
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE procedure [Admin_DeleteCandidate]
	@Id int,
	@User varchar(10)
as
BEGIN
	SET NOCOUNT ON;

	DECLARE @Description varchar(7000), @Action varchar(30), @Enabled bit
	DECLARE @SubmissionCount int;
	DECLARE @Status char(3);

	SELECT
		@SubmissionCount = COUNT(*)
	FROM
		dbo.Submissions A
	WHERE
		A.[CandidateId] = @Id;

	if (@SubmissionCount = 0)
		BEGIN
			DECLARE @Name varchar(200);
			SELECT
				@Name = A.[FirstName] + ' ' + A.[LastName]
			FROM
				dbo.[Candidate] A
			WHERE
				A.[Id] = @Id
			DELETE FROM
				dbo.[Candidate]
			WHERE
				[Id] = @Id;
			SET @Description = 'Deleted Candidate: ' + @Name + ' [Id:' + CAST(@Id as varchar(5)) + ']';

			SET @Action = 'DELETE CANDIDATE';
						
			execute [dbo].[AddAuditTrail] @Action, 'Admin Candidate', @Description, @User;

			SELECT 1;
		END
	else
		BEGIN
			SELECT 2;
	END

END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE Procedure [Admin_EnableDisableUser]
	@Id varchar(10),
	@User varchar(10),
	@Act tinyint
AS
BEGIN
	SET NOCOUNT ON;

	if (EXISTS(SELECT * FROM dbo.[Users] WHERE [UserName] = @Id))
		BEGIN
			DECLARE @Description varchar(7000), @Action varchar(30)
			DECLARE @Stat varchar(5);

			SELECT
				@Stat = A.[Status]
			FROM
				dbo.[Users] A
			WHERE
				[UserName] = @Id;


			UPDATE
				dbo.[Users]
			SET
				[Status] = CASE @Stat WHEN 'ACT' THEN 'INA' ELSE 'ACT' END,
				[UpdatedBy] = @User,
				[UpdatedDate] = GETDATE()
			WHERE
				[UserName] = @Id;
			SET @Description = 'Changed Status for User: ' + @Id + ' to ' + CASE @Stat WHEN 'INA' THEN 'Active' ELSE 'Inactive' END;

			SET @Action = 'CHANGE STATUS';
						
			execute [dbo].[AddAuditTrail] @Action, 'Admin Users', @Description, @User;
		END
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE procedure [Admin_GetDesignationDetails]
	@Id int
as
BEGIN
	SET NOCOUNT ON;

	SELECT
		A.[Designation], CASE A.[Enabled] WHEN 1 THEN 'Active' ELSE 'Inactive' END
	FROM
		[ProfessionalMaster].dbo.[Designation] A
	WHERE
		A.[Id] = @Id
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE procedure [Admin_GetDesignations]
	@Filter varchar(100) = ''
as
BEGIN
	SET NOCOUNT ON;

	if (LTRIM(RTRIM(@Filter)) = '')
		SET @Filter = '%';
	else
		SET @Filter = '%' + @Filter + '%';

	DECLARE @return varchar(max);

	SELECT @return =
	(SELECT 
		A.[Id] [ID], A.[Designation] [Text], CONVERT(varchar(20), A.[CreatedDate], 101) + ' [' + A.[CreatedBy] + ']' [CreatedDate], CONVERT(varchar(20), A.[UpdatedDate], 101) + ' [' + A.[UpdatedBy] + ']' [UpdatedDate], 
		CASE A.[Enabled] WHEN 1 THEN 'Active' ELSE 'Inactive' END [Enabled], A.[Enabled] [IsEnabled]
	FROM
		dbo.[Designation] A
	WHERE
		A.[Designation] LIKE @Filter
	ORDER BY
		A.[Designation] ASC
	FOR JSON PATH);

	SELECT @return;
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE   procedure [Admin_GetDocumentTypes]
	@Filter varchar(100) = ''
AS
BEGIN
	SET NOCOUNT ON;

	if (LTRIM(RTRIM(@Filter)) = '')
		SET @Filter = '%';
	else
		SET @Filter = '%' + @Filter + '%';
	
	DECLARE @return varchar(max);

	SELECT @return =
	(SELECT 
		A.[Id] [KeyValue], A.[DocumentType] [Text], CONVERT(varchar(20), A.[LastUpdatedDate], 101) [LastUpdatedDate]
	FROM
		dbo.[DocumentType] A	
	WHERE
		A.[DocumentType] LIKE @Filter
	ORDER BY
		A.[DocumentType] ASC, A.[LastUpdatedDate] ASC
	FOR JSON PATH);

	SELECT @return;
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE procedure [Admin_GetEducation]
	@Filter varchar(100) = ''
as
BEGIN
	SET NOCOUNT ON;

	if (LTRIM(RTRIM(@Filter)) = '')
		SET @Filter = '%';
	else
		SET @Filter = '%' + @Filter + '%';

	DECLARE @return varchar(max);

	SELECT @return =
	(SELECT
		A.[Id] [ID], A.[Education] [Text], CONVERT(varchar(20), A.[CreatedOn], 101) + ' [' + A.[CreatedBy] + ']' [CreatedDate], CONVERT(varchar(20), A.[UpdatedOn], 101) + ' [' + A.[UpdatedBy] + ']' [UpdatedDate], 
		CASE A.[Enabled] WHEN 1 THEN 'Active' ELSE 'Inactive' END [Enabled], A.[Enabled] [IsEnabled]
	FROM
		dbo.[Education] A
	WHERE
		A.[Education] LIKE @Filter
	ORDER BY
		A.[Education] ASC
	FOR JSON PATH);

	SELECT @return;

END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE procedure [Admin_GetEducationDetails]
	@Id int
as
BEGIN
	SET NOCOUNT ON;

	SELECT
		A.[Education], CASE A.[Enabled] WHEN 1 THEN 'Active' ELSE 'Inactive' END
	FROM
		[ProfessionalMaster].dbo.[Education] A
	WHERE
		A.[Id] = @Id
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE procedure [Admin_GetEligibility]
	@Filter varchar(100) = ''
as
BEGIN
	SET NOCOUNT ON;

	if (LTRIM(RTRIM(@Filter)) = '')
		SET @Filter = '%';
	else
		SET @Filter = '%' + @Filter + '%';

	DECLARE @return varchar(max);

	SELECT @return =
	(SELECT
		A.[Id] [ID], A.[Eligibility] [Text], CONVERT(varchar(20), A.[CreatedDate], 101) + ' [' + A.[CreatedBy] + ']' [CreatedDate], CONVERT(varchar(20), A.[UpdatedDate], 101) + ' [' + A.[UpdatedBy] + ']' [UpdatedDate],
		CASE A.[Enabled] WHEN 1 THEN 'Active' ELSE 'Inactive' END [Enabled], A.[Enabled] [IsEnabled]
	FROM
		dbo.[Eligibility] A
	WHERE
		A.[Eligibility] LIKE @Filter
	ORDER BY
		A.[Eligibility] ASC
	FOR JSON PATH);

	SELECT @return;

END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE procedure [Admin_GetEligibilityDetails]
	@Id int
as
BEGIN
	SET NOCOUNT ON;

	SELECT
		A.[Eligibility], CASE A.[Enabled] WHEN 1 THEN 'Active' ELSE 'Inactive' END
	FROM
		[ProfessionalMaster].dbo.[Eligibility] A
	WHERE
		A.[Id] = @Id
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE procedure [Admin_GetExperience]
	@Filter varchar(100) = ''
as
BEGIN
	SET NOCOUNT ON;

	if (LTRIM(RTRIM(@Filter)) = '')
		SET @Filter = '%';
	else
		SET @Filter = '%' + @Filter + '%';

	DECLARE @return varchar(max);

	SELECT @return =
	(SELECT
		A.[Id] [ID], A.[Experience] [Text], CONVERT(varchar(20), A.[CreatedDate], 101) + ' [' + A.[CreatedBy] + ']' [CreatedDate], CONVERT(varchar(20), A.[UpdatedDate], 101) + ' [' + A.[UpdatedBy] + ']' [UpdatedDate],
		CASE A.[Enabled] WHEN 1 THEN 'Active' ELSE 'Inactive' END [Enabled], A.[Enabled] [IsEnabled]
	FROM
		dbo.[Experience] A
	WHERE
		A.[Experience] LIKE @Filter
	ORDER BY
		A.[Experience] ASC
	FOR JSON PATH);

	SELECT @return;
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE procedure [Admin_GetExperienceDetails]
	@Id int
as
BEGIN
	SET NOCOUNT ON;

	SELECT
		A.[Experience], CASE A.[Enabled] WHEN 1 THEN 'Active' ELSE 'Inactive' END
	FROM
		[ProfessionalMaster].dbo.[Experience] A
	WHERE
		A.[Id] = @Id
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [Admin_GetIndustries]
	@Filter varchar(100) = ''
as
BEGIN
	SET NOCOUNT ON;

	if (LTRIM(RTRIM(@Filter)) = '')
		SET @Filter = '%';
	else
		SET @Filter = '%' + @Filter + '%';

	DECLARE @return varchar(max);

	SELECT @return =
	(SELECT
		A.[Id] [ID], A.[Industry] [Text], CONVERT(varchar(20), A.[CreatedDate], 101) + ' [' + A.[CreatedBy] + ']' [CreatedDate], CONVERT(varchar(20), A.[UpdatedDate], 101) + ' [' + A.[UpdatedBy] + ']' [UpdatedDate],
		CASE A.[Enabled] WHEN 1 THEN 'Active' ELSE 'Inactive' END [Enabled], A.[Enabled] [IsEnabled]
	FROM
		dbo.[LeadIndustry] A
	WHERE
		A.[Industry] LIKE @Filter
	ORDER BY
		A.[Industry] ASC
	FOR JSON PATH);

	SELECT @return;
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE procedure [Admin_GetJobOptionDetails]
	@Code char(1) = 'F'
as
BEGIN
	SET NOCOUNT ON;

	SELECT
		A.[JobCode], A.[JobOptions], A.[DurationReq], A.[RateReq], A.[SalReq], A.[TaxTerms], A.[ExpReq], A.[PlaceFeeReq], A.[BenefitsReq], A.[Description]
	FROM
		[ProfessionalMaster].dbo.[JobOptions] A
	WHERE
		A.[JobCode] = @Code
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE   PROCEDURE [Admin_GetJobOptions]
	@Filter varchar(100) = ''
AS
BEGIN
	SET NOCOUNT ON;

	if (LTRIM(RTRIM(@Filter)) = '')
		SET @Filter = '%';
	else
		SET @Filter = '%' + @Filter + '%';

	DECLARE @return varchar(max);

	SELECT @return =
	(
	SELECT 
		J.[JobCode] [KeyValue], J.[JobOptions] [Text], J.[Description], CONVERT(varchar(20), J.[UpdateDate], 101) [UpdatedDate], J.[DurationReq] [Duration], J.[RateReq] [Rate], J.[SalReq] [Sal], J.[TaxTerms] [Tax], J.[ExpReq] [Exp], 
		J.[PlaceFeeReq] [PlaceFee], J.[BenefitsReq] [Benefits], J.[RateText], J.[PercentText], J.[ShowHours], ISNULL([CostPercent], 0) / 100 [CostPercent], [ShowPercent], STRING_AGG(V.Flag, ', ') [Flags]
	FROM JobOptions J
		CROSS APPLY
		(
			VALUES
				(CASE WHEN J.DurationReq = 1 THEN 'Duration' END),
				(CASE WHEN J.RateReq = 1 THEN 'Rate' END),
				(CASE WHEN J.SalReq = 1 THEN 'Salary' END),
				(CASE WHEN J.ExpReq = 1 THEN 'Expenses' END),
				(CASE WHEN J.PlaceFeeReq = 1 THEN 'Placement Fee' END),
				(CASE WHEN J.BenefitsReq = 1 THEN 'Benefits' END),
				(CASE WHEN J.ShowHours = 1 THEN 'Hours' END),
				(CASE WHEN J.ShowPercent = 1 THEN 'Percent' END)
		) V(Flag)
	WHERE 
		V.Flag IS NOT NULL
	GROUP BY  
		J.JobCode, J.JobOptions, J.[Description], J.[UpdateDate], J.[DurationReq], J.[RateReq], J.[SalReq], J.[TaxTerms], J.[ExpReq], J.[PlaceFeeReq], J.[BenefitsReq], J.[RateText], J.[PercentText], J.[ShowHours], [CostPercent], [ShowPercent]
	ORDER BY
		J.[JobOptions] ASC
	FOR JSON PATH);

	SELECT @return;
END

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE     procedure [Admin_GetLeadSources]
	@Filter varchar(100) = ''
as
BEGIN
	SET NOCOUNT ON;

	if (LTRIM(RTRIM(@Filter)) = '')
		SET @Filter = '%';
	else
		SET @Filter = '%' + @Filter + '%';

	DECLARE @return varchar(max);

	SELECT @return =
	(SELECT 
		A.[Id] [ID], A.[LeadSource] [Text], CONVERT(varchar(20), A.[CreatedDate], 101) + ' [' + A.[CreatedBy] + ']' [CreatedDate], CONVERT(varchar(20), A.[UpdatedDate], 101) + ' [' + A.[UpdatedBy] + ']' [UpdatedDate], 
		CASE A.[Enabled] WHEN 1 THEN 'Active' ELSE 'Inactive' END [Enabled], A.[Enabled] [IsEnabled]
	FROM
		dbo.[LeadSource] A
	WHERE
		A.[LeadSource] LIKE @Filter
	ORDER BY
		A.[LeadSource] ASC
	FOR JSON PATH);

	SELECT @return;
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE     procedure [Admin_GetLeadStatuses]
	@Filter varchar(100) = ''
as
BEGIN
	SET NOCOUNT ON;

	if (LTRIM(RTRIM(@Filter)) = '')
		SET @Filter = '%';
	else
		SET @Filter = '%' + @Filter + '%';

	DECLARE @return varchar(max);

	SELECT @return =
	(SELECT
		A.[Id] [ID], A.[LeadStatus] [Text], CONVERT(varchar(20), A.[CreatedDate], 101) + ' [' + A.[CreatedBy] + ']' [CreatedDate], CONVERT(varchar(20), A.[UpdatedDate], 101) + ' [' + A.[UpdatedBy] + ']' [UpdatedDate],
		CASE A.[Enabled] WHEN 1 THEN 'Active' ELSE 'Inactive' END [Enabled], A.[Enabled] [IsEnabled]
	FROM
		dbo.[LeadStatus] A
	WHERE
		A.[LeadStatus] LIKE @Filter
	ORDER BY
		A.[LeadStatus] ASC
	FOR JSON PATH);

	SELECT @return;
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE   procedure [Admin_GetNAICS]
	@Filter varchar(100) = ''
as
BEGIN
	SET NOCOUNT ON;

	if (LTRIM(RTRIM(@Filter)) = '')
		SET @Filter = '%';
	else
		SET @Filter = '%' + @Filter + '%';

	DECLARE @return varchar(max);

	SELECT @return =
	(SELECT 
		A.[Id] [ID], A.[NAICSTitle] [Title], CONVERT(varchar(20), A.[CreatedDate], 101) + ' [' + A.[CreatedBy] + ']' [CreatedDate], CONVERT(varchar(20), A.[UpdatedDate], 101) + ' [' + A.[UpdatedBy] + ']' [UpdatedDate]
	FROM
		dbo.[NAICS] A
	WHERE
		A.[NAICSTitle] LIKE @Filter
	ORDER BY
		A.[NAICSTitle] ASC
	FOR JSON PATH);

	SELECT @return;
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [Admin_GetRoleDetails]
	@Id char(2)
AS
BEGIN
	SET NOCOUNT ON;

	SELECT 
		A.[Id], A.[Role], A.[ViewCandidate], A.[ViewRequisition], A.[EditCandidate], A.[EditRequisition], A.[ChangeCandidateStatus], 
		A.[ChangeRequisitionStatus], A.[SendEmailCandidate], A.[ForwardResume], A.[DownloadResume], A.[SubmitCandidates], 
		A.[ViewClients], A.[EditClients], A.[Description]
	FROM 
		[ProfessionalMaster].[dbo].[Roles] A
	WHERE
		A.[Id] = @Id;
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE   procedure [Admin_GetRoles]
	@Filter varchar(100) = ''
as
BEGIN
	SET NOCOUNT ON;

	if (LTRIM(RTRIM(@Filter)) = '')
		SET @Filter = '%';
	else
		SET @Filter = '%' + @Filter + '%';

	DECLARE @return varchar(max);

	SELECT @return =
	(SELECT 
		[ID], [RoleName], [RoleDescription] [Description], CONVERT(varchar(20), A.[CreatedDate], 101) + ' [' + A.[CreatedBy] + ']' [CreatedDate], 
		CONVERT(varchar(20), A.[UpdatedDate], 101) + ' [' + A.[UpdatedBy] + ']' [UpdatedDate], [RoleDescription], [CreateOrEditCompany], [CreateOrEditCandidate], [ViewAllCompanies], [ViewMyCompanyProfile], 
		[EditMyCompanyProfile], [CreateOrEditRequisitions], [ViewOnlyMyCandidates], [ViewAllCandidates], [ViewRequisitions], [EditRequisitions], [ManageSubmittedCandidates], [DownloadOriginal],
		[DownloadFormatted], [AdminScreens]
	FROM 
		[dbo].[Roles] A
	WHERE
		A.[RoleDescription] LIKE @Filter
	ORDER BY
		A.[RoleDescription] ASC
	FOR JSON PATH);

	SELECT @return;
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE procedure [Admin_GetSkillDetails]
	@Id int
as
BEGIN
	SET NOCOUNT ON;

	SELECT
		A.[Skill], CASE A.[Enabled] WHEN 1 THEN 'Active' ELSE 'Inactive' END
	FROM
		[ProfessionalMaster].dbo.[Skills] A
	WHERE
		A.[Id] = @Id
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [Admin_GetSkills]
	@Filter varchar(100) = ''
AS
BEGIN
	SET NOCOUNT ON;

	if (LTRIM(RTRIM(@Filter)) = '')
		SET @Filter = '%';
	else
		SET @Filter = '%' + @Filter + '%';

	DECLARE @return varchar(max);

	SELECT @return =
	(SELECT 
		A.[Id] [ID], A.[Skill] [Text], CONVERT(varchar(20), A.[CreatedDate], 101) + ' [' + A.[CreatedBy] + ']' [CreatedDate], CONVERT(varchar(20), A.[UpdatedDate], 101) + ' [' + A.[UpdatedBy] + ']' [UpdatedDate], 
		CASE A.[Enabled] WHEN 1 THEN 'Active' ELSE 'Inactive' END [Enabled], A.[Enabled] [IsEnabled]
	FROM
		dbo.[Skills] A
	WHERE
		A.[Skill] LIKE @Filter
	ORDER BY
		A.[Skill] ASC
	FOR JSON PATH);

	SELECT @return;
END

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE procedure [Admin_GetStage]
AS
BEGIN;
	SET NOCOUNT ON;

	SELECT
		A.[Id], A.[Step], B.[Status], A.[Next], A.[IsLast], A.[Role], A.[Schedule], A.[AnyStage]
	FROM
		[ProfessionalMaster].dbo.[WorkflowActivity] A INNER JOIN [ProfessionalMaster].dbo.[StatusCode] B ON A.[Step] = B.[StatusCode]
		AND B.[AppliesTo] = 'SCN';

	SELECT
		A.[StatusCode], A.[Status]
	FROM
		[ProfessionalMaster].dbo.[StatusCode] A
	WHERE
		A.[AppliesTo] = 'SCN';

	SELECT
		A.[Id], A.[Role]
	FROM
		[ProfessionalMaster].dbo.[Roles] A;
END;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE procedure [Admin_GetStageDetails]
	@Id char(3)
AS
BEGIN;
	SET NOCOUNT ON;

	SELECT
		A.[Id], A.[Step], B.[Status], A.[Next], A.[IsLast], A.[Role], A.[Schedule], A.[AnyStage]
	FROM
		[ProfessionalMaster].dbo.[WorkflowActivity] A INNER JOIN [ProfessionalMaster].dbo.[StatusCode] B ON A.[Step] = B.[StatusCode]
		AND B.[AppliesTo] = 'SCN'
	WHERE
		A.[Step] = @Id;
END;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE procedure [Admin_GetStateDetails]
	@Id int
AS 
BEGIN
	SET NOCOUNT ON;
	
	SELECT
		A.[Id], A.[Code], A.[State], A.[Country], A.[CreatedDate], A.[UpdatedDate]
	FROM
		[ProfessionalMaster].dbo.[State] A
	WHERE
		A.[Id] = @Id;
END


GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE   procedure [Admin_GetStates]
	@Filter varchar(100) = ''
AS 
BEGIN
	SET NOCOUNT ON;

	if (LTRIM(RTRIM(@Filter)) = '')
		SET @Filter = '%';
	else
		SET @Filter = '%' + @Filter + '%';

	DECLARE @return varchar(max);

	SELECT @return =
	(SELECT 
		A.[Id], A.[State] [StateName], A.[Code], CONVERT(varchar(20), A.[CreatedDate], 101) + ' [' + A.[CreatedBy] + ']' [CreatedDate], CONVERT(varchar(20), A.[UpdatedDate], 101) + ' [' + A.[UpdatedBy] + ']' [UpdatedDate]
	FROM 
		[dbo].[State] A
	WHERE
		A.[State] LIKE @Filter
	ORDER BY
		A.[State] ASC
	FOR JSON PATH);

	SELECT @return;
END


GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE Procedure [Admin_GetStatus]
AS 
BEGIN
	SET NOCOUNT ON;
	
	SELECT
		A.[Id], A.[StatusCode], A.[Status], '' [Description], CASE A.[AppliesTo] WHEN 'CLI' THEN 'Client'
															WHEN 'CND' THEN 'Candidate'
															WHEN 'REQ' THEN 'Requirement'
															WHEN 'SUB' THEN 'Submission'
															WHEN 'USR' THEN 'User'
															WHEN 'VND' THEN 'Vendor'
															WHEN 'SCN' THEN 'Submission'
														END [AppliesTo], A.[DisplayOrder], A.[CreatedDate], A.[UpdatedDate], A.[Icon]
	FROM
		[ProfessionalMaster].dbo.[StatusCode] A
	WHERE
		A.[AppliesTo] <> 'SUB'
	ORDER BY
		A.[AppliesTo] ASC, A.[DisplayOrder] ASC;
END


GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE procedure [Admin_GetStatusCodes]
	@Filter varchar(100) = ''
AS
BEGIN
	SET NOCOUNT ON;

	if (LTRIM(RTRIM(@Filter)) = '')
		SET @Filter = '%';
	else
		SET @Filter = '%' + @Filter + '%';
	
	SELECT
		A.[Id], A.[StatusCode], A.[Status], A.[Description], A.[AppliesTo],
		CASE 
			A.[AppliesTo] 
				WHEN 'CLI' THEN 'Client' WHEN 'CND' THEN 'Candidate' WHEN 'REQ' THEN 'Requisition' WHEN 'SCN' THEN 'Candidate Submission'
				WHEN 'SUB' THEN 'Submission' WHEN'USR' THEN 'User' WHEN 'VND' Then 'Vendor'
		END,
		ISNULL(A.[Icon], ''), A.[SubmitCandidate], A.[ShowCommission], A.[Color], 
		CONVERT(varchar(20), A.[CreatedDate], 101) + ' [' + A.[CreatedBy] + ']', CONVERT(varchar(20), A.[UpdatedDate], 101) + ' [' + A.[UpdatedBy] + ']'
	FROM
		[ProfessionalMaster].dbo.[StatusCode] A	
	WHERE
		A.[Status] LIKE @Filter
	ORDER BY
		A.[AppliesTo] ASC, A.[Status] ASC

	SELECT @@ROWCOUNT

END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE Procedure [Admin_GetStatusDetails]
	@Id int=1
AS 
BEGIN
	SET NOCOUNT ON;
	
	SELECT
		A.[Id], A.[StatusCode], A.[Status], A.[Description], A.[AppliesTo], A.[DisplayOrder], A.[CreatedDate], A.[UpdatedDate], A.[Icon]
	FROM
		[ProfessionalMaster].dbo.[StatusCode] A
	WHERE
		A.[Id] = @Id;
END


GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE procedure [Admin_GetTaxTermDetails]
	@Code char(1)
as
BEGIN
	SET NOCOUNT ON;

	SELECT
		A.[TaxTermCode], A.[TaxTerm], A.[Description], A.[UpdateDate], CASE A.[Enabled] WHEN 1 THEN 'Active' ELSE 'Inactive' END
	FROM
		[ProfessionalMaster].dbo.[TaxTerm] A
	WHERE
		A.[TaxTermCode] = @Code;
END;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE   procedure [Admin_GetTaxTerms]
	@Filter varchar(100) = ''
as
BEGIN
	SET NOCOUNT ON;

	if (LTRIM(RTRIM(@Filter)) = '')
		SET @Filter = '%';
	else
		SET @Filter = '%' + @Filter + '%';

	DECLARE @return varchar(max);

	SELECT @return =
	(SELECT 
		A.[TaxTermCode] [Code], A.[TaxTerm] [Text], A.[Description], CONVERT(varchar(20), A.[UpdateDate], 101) + ' [ADMIN]' [CreatedDate], CONVERT(varchar(20), A.[UpdateDate], 101) + ' [ADMIN]' [UpdatedDate], 
		CASE A.[Enabled] WHEN 1 THEN 'Active' ELSE 'Inactive' END [Enabled], A.[Enabled] [IsEnabled]
	FROM
		dbo.[TaxTerm] A
	WHERE
		A.[TaxTerm] LIKE @Filter
	ORDER BY
		A.[TaxTerm] ASC
	FOR JSON PATH);

	SELECT @return;
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE procedure [Admin_GetTemplateDetails]
	@Id int
as
BEGIN
	SET NOCOUNT ON;

	SELECT
		A.[Id], A.[TemplateName], A.[Cc], A.[Subject], A.[Template], A.[Notes], A.[CreatedDate], A.[UpdatedDate], 
		CASE A.[Enabled] WHEN 1 THEN 'Active' ELSE 'Inactive' END, A.[Action], A.[SendTo]
	FROM
		dbo.[Templates] A
	WHERE
		A.[Id] = @Id;
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE procedure [Admin_GetTemplates]
	@Filter varchar(100) = ''
as
BEGIN
	SET NOCOUNT ON;

	if (@Filter = '')
		BEGIN
			SET @Filter = '%';
		END
	else
		BEGIN
			SET @Filter = '%' + @Filter + '%';
		END

	DECLARE @return varchar(max);

	SELECT @return =
	(
	SELECT
		A.[Id], A.[TemplateName], '' [CC], A.[Subject], A.[Template] [TemplateContent], A.[Notes], A.[CreatedDate], A.[CreatedBy], A.[UpdatedDate], A.[UpdatedBy],
		CASE A.[Enabled] WHEN 1 THEN 'Active' ELSE 'Inactive' END [Status], A.[Enabled] [IsEnabled], A.[Action], A.[SendTo]
	FROM
		dbo.[Templates] A
	WHERE
		A.[TemplateName] LIKE @Filter
	ORDER BY
		A.[TemplateName], A.[Enabled] DESC, A.[Id] ASC
	FOR JSON PATH
	);

	SELECT @return;
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

create Procedure [Admin_GetUserDetails]
	@Id varchar(10)
AS
BEGIN
	SET NOCOUNT ON;

	SELECT
		A.[FirstName], A.[LastName], A.[MiddleInitial], A.[EmailAddress], A.[Role], A.[Status]
	FROM
		dbo.[Users] A
	WHERE
		A.[UserName] = @Id
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [Admin_GetUsers]
	@Filter varchar(100) = ''
AS
BEGIN
	SET NOCOUNT ON;

	if (@Filter = '')
		BEGIN
			SET @Filter = '%';
		END
	else
		BEGIN
			SET @Filter = '%' + @Filter + '%';
		END

	DECLARE @return varchar(max);

	SELECT @return =
	(SELECT 
		A.[UserName], A.[FirstName] + ' ' + A.[LastName] [FullName], '[' + B.[RoleName] + '] - ' + B.[RoleDescription] [Role], A.[EmailAddress], A.[FirstName], A.[LastName], A.[Role] [RoleID], 
		CASE A.[Status] WHEN 1 THEN 'Active' ELSE 'Inactive' END [Status], A.[Status] [StatusEnabled], CONVERT(varchar(20), A.[CreatedDate], 101) [CreatedDate], A.[UpdatedDate] --, ISNULL(A..[Passwd], '')
	FROM
		dbo.[Users] A INNER JOIN dbo.[Roles] B ON A.[Role] = B.[Id]
	WHERE
		A.[FirstName] + ' ' + A.[LastName] LIKE @Filter OR A.[UserName] LIKE @Filter
	ORDER BY
		A.[FirstName] + ' ' + A.[LastName], A.[UpdatedDate] DESC, B.[RoleName] DESC, A.[Status]
	FOR JSON PATH);

	SELECT @return;
	
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

create   procedure [Admin_GetVariableCommission]
as
BEGIN
	SET NOCOUNT ON;

	SELECT
		A.[Id], A.[NoofHours], A.[OverHeadCost], A.[W2TaxLoadingRate], A.[1099CostRate], A.[FTERateOffered]
	FROM
		dbo.[VariableCommission] A;
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE     PROCEDURE [Admin_GetWorkflow] 
	@Filter varchar(100) = ''
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	if (@Filter = '')
		BEGIN
			SET @Filter = '%';
		END
	else
		BEGIN
			SET @Filter = '%' + @Filter + '%';
		END

	DECLARE @return varchar(max);

	;WITH CTE_Workflow as (
	SELECT
		A.[Id], '[' + A.[Step] + '] - ' + S.Status [Step], A.[Next], A.[IsLast], A.[Role] [RoleIDs], A.[Schedule], A.[AnyStage], 
		CASE WHEN A.[Next] IS NULL OR A.[Next] = '' THEN '' ELSE STRING_AGG(B.[Status], ', ') END AS NextStatusNames,
		CASE WHEN A.[Role] IS NULL OR A.[Role] = '' THEN '' ELSE STRING_AGG(R.[RoleDescription], ', ') END AS RoleDescriptions--, B.[Status]
	FROM
		dbo.[WorkflowActivity] A INNER JOIN dbo.[StatusCode] S ON A.Step=S.StatusCode AND S.AppliesTo = 'SCN' 
		LEFT JOIN  dbo.[StatusCode] B ON ',' + A.[Next] + ',' LIKE '%,' + B.[StatusCode] + ',%' AND B.AppliesTo = 'SCN'
		LEFT JOIN dbo.[Roles] R ON ',' + A.[Role] + ',' LIKE '%,' + R.[RoleName] + ',%'
	GROUP BY
		A.[Id], A.[Step], A.[Next], A.[IsLast], A.[Role], A.[Schedule], A.[AnyStage], S.Status)

	SELECT @return =
	(
	SELECT 
		ID, Step, Next, IsLast, RoleIDs, Schedule, AnyStage, dbo.GetUniqueAggregatedNames(NextStatusNames) [NextFull], dbo.GetUniqueAggregatedNames(RoleDescriptions) [RoleFull]
	FROM 
		CTE_Workflow 
	WHERE
		[Step] LIKE @Filter
	ORDER BY
		[Step] ASC
	FOR JSON PATH
	); --AND B.[AppliesTo] = 'SCN';

	SELECT @return;

END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE Procedure [Admin_SaveDesignation]
	@Id int = 43,
	@Designation varchar(100) = '1Raddiees',
	@User varchar(10) = 'ADMIN',
	@Enabled bit=1
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @Description varchar(7000), @Action varchar(30)

	if (@Id IS NULL OR @Id = 0)
		BEGIN
			INSERT INTO
				dbo.[Designation]
				([Designation], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [Enabled])
			VALUES
				(@Designation, @User, GETDATE(), @User, GETDATE(), @Enabled);

			--SET @Id = IDENT_CURRENT('Designation');

			SET @Description = 'Added Designation: ' + @Designation + ', [ID: ' + CAST(@Id as varchar(5)) + ']';

			SET @Action = 'ADD DESIGNATION';
		END
	else
		BEGIN
			UPDATE
				dbo.[Designation]
			SET
				[Designation] = @Designation,
				[Enabled] = @Enabled,
				[UpdatedBy] = @User,
				[UpdatedDate] = GETDATE()
			WHERE
				[Id] = @Id;

			SET @Description = 'Updated Designation: ' + @Designation + ', [ID: ' + CAST(@Id as varchar(5)) + ']';

			SET @Action = 'UPDATE DESIGNATION';
		END

	execute dbo.[Admin_GetDesignations];

	execute dbo.[GetDesignations];
						
	execute [dbo].[AddAuditTrail] @Action, 'Admin Designation', @Description, @User;
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE   Procedure [Admin_SaveDocumentType]
	@Id varchar(10),
	@DocumentType varchar(50)
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @Description varchar(7000), @Action varchar(30)

	if (@Id IS NULL OR @Id = 0)
		BEGIN
			INSERT INTO
				dbo.[DocumentType]
				([DocumentType], [LastUpdatedDate])
			VALUES
				(@DocumentType, GETDATE());

			--SET @Id = IDENT_CURRENT('DocumentType');

			SET @Description = 'Added Document Type ' + @DocumentType + ', [ID: ' + @Id + ']';

			SET @Action = 'ADD DOCUMENT TYPE';
		END
	else
		BEGIN
			UPDATE
				dbo.[DocumentType]
			SET
				[DocumentType] = @DocumentType,
				[LastUpdatedDate] = GETDATE()
			WHERE
				[Id] = @Id;

			SET @Description = 'Updated Document Type ' + @DocumentType + ', [ID: ' + @Id + ']';

			SET @Action = 'UPDATE Document Type';
		END

	execute dbo.[Admin_GetDocumentTypes];

	execute dbo.[GetDocumentType];
						
	execute [dbo].[AddAuditTrail] @Action, 'Admin Users', @Description, 'ADMIN';
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE Procedure [Admin_SaveEducation]
	@Id int = NULL,
	@Education varchar(50),
	@User varchar(10),
	@Enabled bit
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @Description varchar(7000), @Action varchar(30)

	if (@Id IS NULL OR @Id = 0)
		BEGIN
			INSERT INTO
				dbo.[Education]
				([Education], [CreatedBy], [CreatedOn], [UpdatedBy], [UpdatedOn], [Enabled])
			VALUES
				(@Education, @User, GETDATE(), @User, GETDATE(), @Enabled);

			SET @Description = 'Added Education: ' + @Education + ', [ID: ' + CAST(@Id as varchar(5)) + ']';

			SET @Action = 'ADD EDUCATION';
		END
	else
		BEGIN
			UPDATE
				dbo.[Education]
			SET
				[Education] = @Education,
				[Enabled] = @Enabled,
				[UpdatedBy] = @User,
				[UpdatedOn] = GETDATE()
			WHERE
				[Id] = @Id;

			SET @Description = 'Updated Education: ' + @Education + ', [ID: ' + CAST(@Id as varchar(5)) + ']';

			SET @Action = 'UPDATE EDUCATION';
		END

	execute dbo.[Admin_GetEducation];

	execute dbo.[GetEducation];
						
	execute [dbo].[AddAuditTrail] @Action, 'Admin Education', @Description, @User;
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE Procedure [Admin_SaveEligibility]
	@Id int = NULL,
	@Eligibility varchar(100),
	@User varchar(10),
	@Enabled bit
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @Description varchar(7000), @Action varchar(30)

	if (@Id IS NULL OR @Id = 0)
		BEGIN
			INSERT INTO
				dbo.[Eligibility]
				([Eligibility], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [Enabled])
			VALUES
				(@Eligibility, @User, GETDATE(), @User, GETDATE(), @Enabled);

			--SET @Id = IDENT_CURRENT('Eligibility');

			SET @Description = 'Added Eligibility: ' + @Eligibility + ', [ID: ' + CAST(@Id as varchar(5)) + ']';

			SET @Action = 'ADD ELIGIBILITY';
		END
	else
		BEGIN
			UPDATE
				dbo.[Eligibility]
			SET
				[Eligibility] = @Eligibility,
				[Enabled] = @Enabled,
				[UpdatedBy] = @User,
				[UpdatedDate] = GETDATE()
			WHERE
				[Id] = @Id;

			SET @Description = 'Updated Eligibility: ' + @Eligibility + ', [ID: ' + CAST(@Id as varchar(5)) + ']';

			SET @Action = 'UPDATE ELIGIBILITY';
		END

	execute dbo.[Admin_GetEligibility];

	execute dbo.[GetEligibility];
						
	execute [dbo].[AddAuditTrail] @Action, 'Admin Eligibility', @Description, @User;
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE Procedure [Admin_SaveExperience]
	@Id int = NULL,
	@Experience varchar(100),
	@User varchar(10),
	@Enabled bit
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @Description varchar(7000), @Action varchar(30)

	if (@Id IS NULL OR @Id = 0)
		BEGIN
			INSERT INTO
				dbo.[Experience]
				([Experience], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [Enabled])
			VALUES
				(@Experience, @User, GETDATE(), @User, GETDATE(), @Enabled);

			--SET @Id = IDENT_CURRENT('Experience');

			SET @Description = 'Added Experience: ' + @Experience + ', [ID: ' + CAST(@Id as varchar(5)) + ']';

			SET @Action = 'ADD EXPERIENCE';
		END
	else
		BEGIN
			UPDATE
				dbo.[Experience]
			SET
				[Experience] = @Experience,
				[Enabled] = @Enabled,
				[UpdatedBy] = @User,
				[UpdatedDate] = GETDATE()
			WHERE
				[Id] = @Id;

			SET @Description = 'Updated Experience: ' + @Experience + ', [ID: ' + CAST(@Id as varchar(5)) + ']';

			SET @Action = 'UPDATE EXPERIENCE';
		END

	execute dbo.[Admin_GetExperience];

	execute dbo.[GetExperience];
						
	execute [dbo].[AddAuditTrail] @Action, 'Admin Experience', @Description, @User;
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE   Procedure [Admin_SaveIndustry]
	@Id int = NULL,
	@Industry varchar(100),
	@User varchar(10),
	@Enabled bit
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @Description varchar(7000), @Action varchar(30)

	if (@Id IS NULL OR @Id = 0)
		BEGIN
			INSERT INTO
				dbo.[LeadIndustry]
				([Industry], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [Enabled])
			VALUES
				(@Industry, @User, GETDATE(), @User, GETDATE(), @Enabled);

			--SET @Id = IDENT_CURRENT('LeadIndustry');

			SET @Description = 'Added LeadIndustry: ' + @Industry + ', [ID: ' + CAST(@Id as varchar(5)) + ']';

			SET @Action = 'ADD LeadIndustry';
		END
	else
		BEGIN
			UPDATE
				dbo.[LeadIndustry]
			SET
				[Industry] = @Industry,
				[Enabled] = @Enabled,
				[UpdatedBy] = @User,
				[UpdatedDate] = GETDATE()
			WHERE
				[Id] = @Id;

			SET @Description = 'Updated LeadIndustry: ' + @Industry + ', [ID: ' + CAST(@Id as varchar(5)) + ']';

			SET @Action = 'UPDATE LeadIndustry';
		END

	execute dbo.[Admin_GetIndustries];

	execute dbo.[GetLeadIndustry];
						
	execute [dbo].[AddAuditTrail] @Action, 'Admin LeadIndustry', @Description, @User;
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE Procedure [Admin_SaveJobOptions]
	@Code char(1),
	@JobOptions varchar(50),
	@Desc varchar(500),
	@Duration bit,
	@Rate bit,
	@Sal bit,
	@TaxTerms varchar(20),
	@Expenses bit,
	@PlaceFee bit, 
	@Benefits bit,
	@ShowHours bit,
	@RateText varchar(255),
	@PercentText varchar(255),
	@CostPercent numeric(5, 2),
	@ShowPercent bit,
	@User varchar(10)
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @Description varchar(7000), @Action varchar(30)

	if (NOT EXISTS(SELECT * FROM dbo.[JobOptions] A WHERE A.[JobCode] = @Code))
		BEGIN
			INSERT INTO
				dbo.[JobOptions]
				([JobCode], [JobOptions], [Description], [DurationReq], [RateReq], [SalReq], [TaxTerms], [ExpReq], [PlaceFeeReq], [BenefitsReq], [ShowHours], [RateText],
				[PercentText], [CostPercent], [ShowPercent], [UpdateDate])
			VALUES
				(@Code, @JobOptions, @Desc, @Duration, @Rate, @Sal, @TaxTerms, @Expenses, @PlaceFee, @Benefits, @ShowHours, @RateText, @PercentText, @CostPercent * 100,
				@ShowPercent, GETDATE());

			SET @Description = 'Added Job Option: ' + @JobOptions + ', [Code: ' + @Code + ']';

			SET @Action = 'ADD JOB OPTION';
		END
	else
		BEGIN
			UPDATE
				dbo.[JobOptions]
			SET
				[JobOptions] = @JobOptions,
				[Description] = @Desc,
				[DurationReq] = @Duration,
				[RateReq] = @Rate,
				[SalReq] = @Sal,
				[TaxTerms] = @TaxTerms,
				[ExpReq] = @Expenses,
				[PlaceFeeReq] = @PlaceFee,
				[BenefitsReq] = @Benefits,
				[ShowHours] = @ShowHours,
				[RateText] = @RateText,
				[PercentText] = @PercentText,
				[CostPercent] = @CostPercent * 100,
				[ShowPercent] = @ShowPercent,
				[UpdateDate] = GETDATE()
			WHERE
				[JobCode] = @Code;

			SET @Description = 'Updated Job Option: ' + @JobOptions + ', [Code: ' + @Code + ']';

			SET @Action = 'UPDATE JOB OPTION';
		END

	execute dbo.[Admin_GetJobOptions];

	execute dbo.[GetJobOptions];

	execute [dbo].[AddAuditTrail] @Action, 'Admin Experience', @Description, @User;
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE     Procedure [Admin_SaveLeadSource]
	@Id int = NULL,
	@LeadSource varchar(100),
	@User varchar(10),
	@Enabled bit
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @Description varchar(7000), @Action varchar(30)

	if (@Id IS NULL OR @Id = 0)
		BEGIN
			INSERT INTO
				dbo.[LeadSource]
				([LeadSource], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [Enabled])
			VALUES
				(@LeadSource, @User, GETDATE(), @User, GETDATE(), @Enabled);

			--SET @Id = IDENT_CURRENT('LeadSource');

			SET @Description = 'Added LeadSource: ' + @LeadSource + ', [ID: ' + CAST(@Id as varchar(5)) + ']';

			SET @Action = 'ADD LeadSource';
		END
	else
		BEGIN
			UPDATE
				dbo.[LeadSource]
			SET
				[LeadSource] = @LeadSource,
				[Enabled] = @Enabled,
				[UpdatedBy] = @User,
				[UpdatedDate] = GETDATE()
			WHERE
				[Id] = @Id;

			SET @Description = 'Updated LeadSource: ' + @LeadSource + ', [ID: ' + CAST(@Id as varchar(5)) + ']';

			SET @Action = 'UPDATE LeadSource';
		END

	execute dbo.[Admin_GetLeadSources];

	execute dbo.[GetLeadSource];
						
	execute [dbo].[AddAuditTrail] @Action, 'Admin LeadSource', @Description, @User;
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE     Procedure [Admin_SaveLeadStatus]
	@Id int = NULL,
	@LeadStatus varchar(100),
	@User varchar(10),
	@Enabled bit
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @Description varchar(7000), @Action varchar(30)

	if (@Id IS NULL OR @Id = 0)
		BEGIN
			INSERT INTO
				dbo.[LeadStatus]
				([LeadStatus], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [Enabled])
			VALUES
				(@LeadStatus, @User, GETDATE(), @User, GETDATE(), @Enabled);

			--SET @Id = IDENT_CURRENT('LeadStatus');

			SET @Description = 'Added LeadStatus: ' + @LeadStatus + ', [ID: ' + CAST(@Id as varchar(5)) + ']';

			SET @Action = 'ADD LeadStatus';
		END
	else
		BEGIN
			UPDATE
				dbo.[LeadStatus]
			SET
				[LeadStatus] = @LeadStatus,
				[Enabled] = @Enabled,
				[UpdatedBy] = @User,
				[UpdatedDate] = GETDATE()
			WHERE
				[Id] = @Id;

			SET @Description = 'Updated LeadStatus: ' + @LeadStatus + ', [ID: ' + CAST(@Id as varchar(5)) + ']';

			SET @Action = 'UPDATE LeadStatus';
		END

	execute dbo.[Admin_GetLeadStatuses];

	execute dbo.[GetLeadStatus];
						
	execute [dbo].[AddAuditTrail] @Action, 'Admin LeadStatus', @Description, @User;
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE   Procedure [Admin_SaveNAICS]
	@Id int = 43,
	@NAICS varchar(100) = '1Raddiees',
	@User varchar(10) = 'ADMIN'
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @Description varchar(7000), @Action varchar(30)

	if (@Id IS NULL OR @Id = 0)
		BEGIN
			INSERT INTO
				dbo.[NAICS]
				([NAICSTitle], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate])
			VALUES
				(@NAICS, @User, GETDATE(), @User, GETDATE());

			--SET @Id = IDENT_CURRENT('NAICS');

			SET @Description = 'Added NAICS: ' + @NAICS + ', [ID: ' + CAST(@Id as varchar(5)) + ']';

			SET @Action = 'ADD NAICS';
		END
	else
		BEGIN
			UPDATE
				dbo.[NAICS]
			SET
				[NAICSTitle] = @NAICS,
				[UpdatedBy] = @User,
				[UpdatedDate] = GETDATE()
			WHERE
				[Id] = @Id;

			SET @Description = 'Updated NAICS: ' + @NAICS + ', [ID: ' + CAST(@Id as varchar(5)) + ']';

			SET @Action = 'UPDATE NAICS';
		END

	execute dbo.[Admin_GetNAICS];

	execute dbo.[GetNAICS];
						
	execute [dbo].[AddAuditTrail] @Action, 'Admin NAICS', @Description, @User;
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

create     Procedure [Admin_SavePreferences]
	@ReqPriorityHigh varchar(7),
	@ReqPriorityNormal varchar(7),
	@ReqPriorityLow varchar(7),
	@PageSize tinyint
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @Description varchar(7000), @Action varchar(30)

	UPDATE
		dbo.[Preferences]
	SET
		[ReqPriorityHigh] = @ReqPriorityHigh,
		[ReqPriorityNormal] = @ReqPriorityNormal,
		[ReqPriorityLow] = @ReqPriorityLow,
		[PageSize] = @PageSize

	SET @Description = 'Updated Preferences';
	SET @Action = 'UPDATE PREFERENCES';
						
	execute [dbo].[AddAuditTrail] @Action, 'Admin Preferences', @Description, 'ADMIN';

END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE Procedure [Admin_SaveRole]
	@ID int,
	@RoleName varchar(10),
    @RoleDescription varchar(255),
    @CreateOrEditCompany bit,
    @CreateOrEditCandidate bit,
    @ViewAllCompanies bit,
    @ViewMyCompanyProfile bit,
    @EditMyCompanyProfile bit,
    @CreateOrEditRequisitions bit,
    @ViewOnlyMyCandidates bit,
    @ViewAllCandidates bit,
    @ViewRequisitions bit,
    @EditRequisitions bit,
    @ManageSubmittedCandidates bit,
    @DownloadOriginal bit,
    @DownloadFormatted bit,
    @AdminScreens bit,
	@User varchar(10)
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @Description varchar(7000), @Action varchar(30)

	if (NOT EXISTS(SELECT * FROM dbo.[Roles] A WHERE A.[Id] = @ID))
		BEGIN
			INSERT INTO
				dbo.[Roles]
				([RoleName], [RoleDescription], [CreateOrEditCompany], [CreateOrEditCandidate], [ViewAllCompanies], [ViewMyCompanyProfile], [EditMyCompanyProfile], [CreateOrEditRequisitions], [ViewOnlyMyCandidates],
				[ViewAllCandidates], [ViewRequisitions], [EditRequisitions], [ManageSubmittedCandidates], [DownloadOriginal], [DownloadFormatted], [AdminScreens], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate])
			VALUES
				(@RoleName, @RoleDescription, @CreateOrEditCompany, @CreateOrEditCandidate, @ViewAllCompanies, @ViewMyCompanyProfile, @EditMyCompanyProfile, @CreateOrEditRequisitions, @ViewOnlyMyCandidates, 
				@ViewAllCandidates, @ViewRequisitions, @EditRequisitions, @ManageSubmittedCandidates, @DownloadOriginal, @DownloadFormatted, @AdminScreens, @User, GETDATE(), @User, GETDATE());

			SET @Description = 'Added Role: ' + @RoleDescription + ', [Id: ' + @ID + ']';

			SET @Action = 'ADD ROLE';
		END
	else
		BEGIN
			UPDATE
				dbo.[Roles]
			SET
				[RoleName] = @RoleName,
				[RoleDescription] = @RoleDescription,
				[CreateOrEditCompany] = @CreateOrEditCompany,
				[CreateOrEditCandidate] = @CreateOrEditCandidate,
				[ViewAllCompanies] = @ViewAllCompanies,
				[ViewMyCompanyProfile] = @ViewMyCompanyProfile,
				[EditMyCompanyProfile] = @EditMyCompanyProfile,
				[CreateOrEditRequisitions] = @CreateOrEditRequisitions,
				[ViewOnlyMyCandidates] = @ViewOnlyMyCandidates,
				[ViewAllCandidates] = @ViewAllCandidates,
				[ViewRequisitions] = @ViewRequisitions,
				[EditRequisitions] = @EditRequisitions,
				[ManageSubmittedCandidates] = @ManageSubmittedCandidates,
				[DownloadOriginal] = @DownloadOriginal,
				[DownloadFormatted] = @DownloadFormatted,
				[AdminScreens] = @AdminScreens,
				[UpdatedBy] = @User,
				[UpdatedDate] = GETDATE()
			WHERE
				[Id] = @ID;

			SET @Description = 'Updated Role: ' + @RoleDescription + ', [Id: ' + cast(@ID as varchar(10)) + ']';

			SET @Action = 'UPDATE ROLE';
		END
				
	execute dbo.[Admin_GetRoles];

	execute dbo.[GetRoles];

	execute [dbo].[AddAuditTrail] @Action, 'Admin Role', @Description, @User;
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE Procedure [Admin_SaveSkill]
	@Id int = NULL,
	@Skill varchar(100),
	@User varchar(10),
	@Enabled bit
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @Description varchar(7000), @Action varchar(30)

	if (@Id IS NULL OR @Id = 0)
		BEGIN
			INSERT INTO
				dbo.[Skills]
				([Skill], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [Enabled])
			VALUES
				(@Skill, @User, GETDATE(), @User, GETDATE(), @Enabled);

			--SET @Id = IDENT_CURRENT('Skills');

			SET @Description = 'Added Skill: ' + @Skill + ', [ID: ' + CAST(@Id as varchar(5)) + ']';

			SET @Action = 'ADD Skill';
		END
	else
		BEGIN
			UPDATE
				dbo.[Skills]
			SET
				[Skill] = @Skill,
				[Enabled] = @Enabled,
				[UpdatedBy] = @User,
				[UpdatedDate] = GETDATE()
			WHERE
				[Id] = @Id;

			SET @Description = 'Updated Skill: ' + @Skill + ', [ID: ' + CAST(@Id as varchar(5)) + ']';

			SET @Action = 'UPDATE Skill';
		END
				
	execute dbo.[Admin_GetSkills];

	execute dbo.[GetSkills];

	execute [dbo].[AddAuditTrail] @Action, 'Admin Skill', @Description, @User;
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE procedure [Admin_SaveStage]
	@Step char(3),
	@Next varchar(100),
	@IsLast bit,
	@Role varchar(50),
	@Schedule bit,
	@AnyStage bit
AS
BEGIN;
	SET NOCOUNT ON;

	DECLARE @Description varchar(7000), @Action varchar(30)

	UPDATE
		[ProfessionalMaster].[dbo].[WorkflowActivity]
	SET
		[Next] = @Next,
		[IsLast] = @IsLast,
		[Role] = @Role,
		[Schedule] = @Schedule,
		[AnyStage] = @AnyStage
	WHERE
		[Step] = @Step;


	SET @Description = 'Updated Workflow Activity: ' + @Step;

	SET @Action = 'UPDATE WORKFLOW';

	execute [dbo].[AddAuditTrail] @Action, 'Admin Workflow', @Description, 'ADMIN';
END; 
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE Procedure [Admin_SaveState]
	@Id int = NULL,
	@Code varchar(2),
	@State varchar(50),
	@Country varchar(50),
	@User varchar(10)
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @Description varchar(7000), @Action varchar(30)

	if (NOT EXISTS(SELECT * FROM dbo.[State] A WHERE A.[Code] = @Code))
		BEGIN
			INSERT INTO
				dbo.[State]
				([Code], [State], [Country], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate])
			VALUES
				(@Code, @State, @Country, @User, GETDATE(), @User, GETDATE());

			SET @Id = IDENT_CURRENT('[ProfessionalMaster].dbo.State');

			SET @Description = 'Added State: ' + @State + ', [ID: ' + CAST(@Id as varchar(5)) + ']';

			SET @Action = 'ADD STATE';
		END
	else
		BEGIN
			UPDATE
				dbo.[State]
			SET
				[State] = @State,
				[Country] = @Country,
				[UpdatedBy] = @User,
				[UpdatedDate] = GETDATE()
			WHERE
				[Code] = @Code;

			if (@Id IS NULL)
				BEGIN
					SELECT
						@Id = [Id]
					FROM
						dbo.[State] A
					WHERE
						A.[Code] = @Code;
				END

			SET @Description = 'Updated State: ' + @State + ', [ID: ' + CAST(@Id as varchar(5)) + ']';

			SET @Action = 'UPDATE STATE';
		END

	execute dbo.[Admin_GetStates];

	execute dbo.[GetStates];
						
	execute [dbo].[AddAuditTrail] @Action, 'Admin State', @Description, @User;
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE Procedure [Admin_SaveStatusCode]
	@Id int = NULL,
	@Code char(3),
	@Status varchar(50),
	@Desc varchar(100),
	@AppliesTo char(3),
	@Icon varchar(255),
	@Color varchar(10),
	@SubmitCandidate bit,
	@ShowCommission bit,
	@User varchar(10)
AS
BEGIN;
	SET NOCOUNT ON;

	DECLARE @Description varchar(7000), @Action varchar(30)

	if (NOT EXISTS(SELECT * FROM [ProfessionalMaster].dbo.[StatusCode] A WHERE A.[StatusCode] = @Code AND A.[AppliesTo] = @AppliesTo))
		BEGIN;
			INSERT INTO
				[ProfessionalMaster].dbo.[StatusCode]
				([StatusCode], [Status], [Description], [AppliesTo], [DisplayOrder], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [Icon], [Color], 
				[SubmitCandidate], [ShowCommission])
			VALUES
				(@Code, @Status, @Desc, @AppliesTo, 1, @User, GETDATE(), @User, GETDATE(), @Icon, @Color, @SubmitCandidate, @ShowCommission);

			SET @Id = IDENT_CURRENT('[ProfessionalMaster].dbo.StatusCode');

			if (@AppliesTo = 'SCN')
				BEGIN;
					INSERT INTO
						[ProfessionalMaster].dbo.[WorkflowActivity]
						([Step], [Next], [Role], [IsLast], [Schedule], [AnyStage])
					VALUES
						(@Code, NULL, 'AD', 0, 0, 0);
				END;

			SET @Description = 'Added Status: ' + @Status + ', [ID: ' + CAST(@Id as varchar(5)) + ']';

			SET @Action = 'ADD STATUS CODE';
		END;
	else
		BEGIN;
			UPDATE
				[ProfessionalMaster].dbo.[StatusCode]
			SET
				[Status] = @Status,
				[Description] = @Desc,
				[AppliesTo] = @AppliesTo,
				[UpdatedBy] = @User,
				[UpdatedDate] = GETDATE(),
				[Icon] = @Icon,
				[Color] = @Color,
				[SubmitCandidate] = @SubmitCandidate,
				[ShowCommission] = @ShowCommission
			WHERE
				[StatusCode] = @Code
				AND [AppliesTo] = @AppliesTo;

			if (@Id IS NULL)
				BEGIN
					SELECT
						@Id = [Id]
					FROM
						[ProfessionalMaster].dbo.[StatusCode] A
					WHERE
						A.[StatusCode] = @Code
						AND A.[AppliesTo] = @AppliesTo;
				END

			SET @Description = 'Updated Status: ' + @Status + ', [ID: ' + CAST(@Id as varchar(5)) + ']';

			SET @Action = 'UPDATE STATUS';
		END;
				
	SELECT @Id;


	execute [dbo].[AddAuditTrail] @Action, 'Admin Status', @Description, @User;
END;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE Procedure [Admin_SaveTaxTerm]
	@Code char(1),
	@TaxTerm varchar(50),
	@Desc varchar(500) = '',
	@User varchar(10),
	@Enabled bit
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @Description varchar(7000), @Action varchar(30)

	if (NOT EXISTS(SELECT * FROM dbo.[TaxTerm] A WHERE A.[TaxTermCode] = @Code))
		BEGIN
			INSERT INTO
				dbo.[TaxTerm]
				([TaxTermCode], [TaxTerm], [Description], [Enabled], [UpdateDate])
			VALUES
				(@Code, @TaxTerm, @Desc, @Enabled, GETDATE());

			SET @Description = 'Added Tax Term: ' + @TaxTerm + ', [Code: ' + @Code + ']';

			SET @Action = 'ADD TAX TERM';
		END
	else
		BEGIN
			UPDATE
				dbo.[TaxTerm]
			SET
				[TaxTerm] = @TaxTerm,
				[Description] = @Desc,
				[Enabled] = @Enabled,
				[UpdateDate] = GETDATE()
			WHERE
				[TaxTermCode] = @Code;

			SET @Description = 'Updated Tax Term: ' + @TaxTerm + ', [Code: ' + @Code + ']';

			SET @Action = 'UPDATE TAX TERM';
		END
				
	execute dbo.[Admin_GetTaxTerms];

	execute dbo.[GetTaxTerms];
						
	execute [dbo].[AddAuditTrail] @Action, 'Admin Tax Term', @Description, @User;
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE Procedure [Admin_SaveTemplate]
	@Id int = 2009,
	@TemplateName varchar(50) = 'Create Candidate',
	@Cc varchar(2000) = '',
	@Subject varchar(255)= 'Candidate {FullName} Created.',
	@Template varchar(max) = '<p>Hello,</p><p>This is to inform you that a new candidate has been created/parsed in the PSS system.</p><p>The details of the candidate are:</p><p>Name: <strong>$FULL_NAME$</strong></p><p>Location: <strong>$CAND_LOCATION$</strong></p><p>Phone: <strong>$CAND_PHONE_PRIMARY$</strong></p><p>Summary:&nbsp;</p><p>$CAND_SUMMARY$</p><p><strong><br></strong></p><p>--------------------------------------------------------</p><p>Candidate Created on: <strong>$TODAY$</strong>&nbsp;by: <strong>$LOGGED_USER$</strong></p><p>Thanks,</p><p>PSS Admin System.</p>',
	@Notes varchar(500) = 'Create a new Candidate and send the mails to Administrator/s only.',
	@SendTo varchar(200) = 'Administrator',
	@Action tinyint=1,
	@User varchar(10)='ADMIN',
	@Enabled bit=1
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @Description varchar(7000), @ActionD varchar(30)

	if (NOT EXISTS(SELECT * FROM dbo.[Templates] A WHERE A.[ID] = @Id))
		BEGIN
			INSERT INTO
				dbo.[Templates]
				([TemplateName], [Cc], [Subject], [Template], [Notes], [Action], [SendTo], [Enabled], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate])
			VALUES
				(@TemplateName, @Cc, @Subject, @Template, @Notes, @Action, @SendTo, @Enabled, @User, GETDATE(), @User, GETDATE());

			SET @Id = IDENT_CURRENT('Templates');

			SET @Description = 'Added Template: ' + @TemplateName + ', [Id: ' + CAST(@Id as varchar(5)) + ']';

			SET @ActionD = 'ADD TEMPLATE';
		END
	else
		BEGIN
			UPDATE
				dbo.[Templates]
			SET
				[Cc] = @Cc,
				[Subject]  = @Subject,
				[Template] = @Template,
				[Notes] = @Notes,
				[Action] = @Action,
				[SendTo] = @SendTo,
				[Enabled] = @Enabled,
				[UpdatedDate] = GETDATE(),
				[UpdatedBy] = @User
			WHERE
				[Id] = @Id;

			SET @Description = 'Updated Template: ' + @TemplateName + ', [Id: ' + CAST(@Id as varchar(5)) + ']';

			SET @ActionD = 'UPDATE TEMPLATE';
		END

	--SELECT @Id;
						
	execute [dbo].[AddAuditTrail] @ActionD, 'Admin Template', @Description, @User;

	execute dbo.[Admin_GetTemplates];

	SELECT '[]';
	--execute dbo.[GetTemplates];
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   Procedure [Admin_SaveUser]
	@UserName varchar(10),
	@FirstName varchar(50),
	@LastName varchar(50),
	@Email varchar(200),
	@Role tinyint,
	@Status bit,
	@User varchar(10),
	@Password binary(64),
	@Passwd varchar(300) = '',
	@Salt binary(64)
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @Description varchar(7000), @Action varchar(30)

	if (NOT EXISTS(SELECT * FROM dbo.[Users] WHERE [UserName] = @UserName))
		BEGIN
			INSERT INTO
				dbo.[Users]
				([UserName], [FirstName], [LastName], [MiddleInitial], [EmailAddress], [Role], [Status], [CreatedBy], [CreatedDate], 
				[UpdatedBy], [UpdatedDate], [Password], [Salt])
			VALUES
				(@UserName, @FirstName, @LastName, LEFT(@FirstName, 1) + LEFT(@LastName, 1), @Email, @Role, @Status, @User, GETDATE(),
				@User, GETDATE(), @Password, @Salt);

			SET @Description = 'Added User ' + @FirstName + ' ' + @LastName + ', [ID: ' + @UserName + ']';

			SET @Action = 'ADD USER';
		END
	else
		BEGIN
			UPDATE
				dbo.[Users]
			SET
				[FirstName] = @FirstName,
				[LastName] = @LastName,
				[MiddleInitial] = LEFT(@FirstName, 1) + LEFT(@LastName, 1),
				[EmailAddress] = @Email,
				[Role] = @Role,
				[Status] = @Status,
				[UpdatedBy] = @User,
				[Password] = CASE WHEN @Password IS NULL THEN [Password] ELSE @Password END,
				[Salt] = CASE WHEN @Salt IS NULL THEN [Salt] ELSE @Salt END,
				[UpdatedDate] = GETDATE()
			WHERE
				[UserName] = @UserName;

			SET @Description = 'Updated User ' + @FirstName + ' ' + @LastName + ', [ID: ' + @UserName + '], Status: ' + CASE @Status WHEN 1 THEN 'Enabled' ELSE 'Disabled' END;

			SET @Action = 'UPDATE USER';
		END

	execute dbo.[Admin_GetUsers];

	execute dbo.[GetUsers];
						
	execute [dbo].[AddAuditTrail] @Action, 'Admin Users', @Description, @User;
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

create   Procedure [Admin_SaveVariableCommission]
	@NoofHours smallint,
	@OverHeadCost tinyint,
	@W2TaxLoadingRate tinyint,
	@1099CostRate tinyint,
	@FTERateOffered tinyint
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @Description varchar(7000), @Action varchar(30)

	UPDATE
		dbo.[VariableCommission]
	SET
		[NoofHours] = @NoofHours,
		[OverHeadCost] = @OverHeadCost,
		[W2TaxLoadingRate] = @W2TaxLoadingRate,
		[1099CostRate] = @1099CostRate,
		[FTERateOffered] = @FTERateOffered;

	SET @Description = 'Updated Variable Commission';
	SET @Action = 'UPDATE VARIABLE COMMISSION';
						
	execute [dbo].[AddAuditTrail] @Action, 'Admin Variable Commission', @Description, 'ADMIN';

END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE   Procedure [Admin_SaveWorkflow]
	@Id int=5,
	--@Step varchar(3)='URW',
	@Next varchar(100)='INT,RHM',
	@IsLast bit=0,
	@Role varchar(50)='AD,RS,SM',
	@Schedule bit=0,
	@AnyStage bit=0,
	@User varchar(10)='ADMIN'
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @Description varchar(7000), @Action varchar(30)

	UPDATE
		dbo.[WorkflowActivity]
	SET
		--[Step] = @Step,
		[Next] = @Next,
		[IsLast] = @IsLast,
		[Role] = @Role,
		[Schedule] = @Schedule,
		[AnyStage] = @AnyStage
	WHERE
		[Id] = @Id;

	SET @Description = 'Updated Workflow: [ID: ' + CAST(@Id as varchar(5)) + '], by User: ' + @User;

	SET @Action = 'UPDATE Workflow';
						
	execute [dbo].[AddAuditTrail] @Action, 'Admin Workflow', @Description, @User;

	execute dbo.[Admin_GetWorkflow];

	execute dbo.[GetWorkflow];
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE Procedure [Admin_SearchDesignation]
	@Designation varchar(100) = 'chief'
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @return varchar(max);

	SELECT @return = 
	(SELECT TOP 20
		A.[Designation] [Text], A.[Designation] [KeyValue]
	FROM
		dbo.[Designation] A
	WHERE
		A.[Designation] LIKE '%' + @Designation + '%'
	FOR JSON AUTO);

	SELECT @return
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE     Procedure [Admin_SearchDocumentTypes]
	@DocumentType varchar(100) = 'Letter'
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @return varchar(max);

	SELECT @return = 
	(SELECT TOP 20
		A.[DocumentType] [Text], A.[DocumentType] [KeyValue]
	FROM
		dbo.[DocumentType]  A
	WHERE
		A.[DocumentType] LIKE '%' + @DocumentType + '%'
	ORDER BY
		A.[DocumentType] ASC
	FOR JSON PATH);

	SELECT @return;
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE   Procedure [Admin_SearchEducation]
	@Education varchar(100) = 'kind'
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @return varchar(max);

	SELECT @return = 
	(SELECT TOP 20
		A.[Education] [Text], A.[Education] [KeyValue]
	FROM
		dbo.[Education] A
	WHERE
		A.[Education] LIKE '%' + @Education + '%'
	FOR JSON AUTO);

	SELECT @return
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE   Procedure [Admin_SearchEligibility]
	@Eligibility varchar(100) = ''
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @return varchar(max);

	SELECT @return = 
	(SELECT TOP 20
		A.[Eligibility] [Text], A.[Eligibility] [KeyValue]
	FROM
		dbo.[Eligibility]  A
	WHERE
		A.[Eligibility] LIKE '%' + @Eligibility + '%'
	FOR JSON AUTO);

	SELECT @return
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE   Procedure [Admin_SearchExperience]
	@Experience varchar(100) = ''
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @return varchar(max);

	SELECT @return = 
	(SELECT TOP 20
		A.[Experience] [Text], A.[Experience] [KeyValue]
	FROM
		dbo.[Experience]  A
	WHERE
		A.[Experience] LIKE '%' + @Experience + '%'
	FOR JSON AUTO);

	SELECT @return
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE   Procedure [Admin_SearchIndustry]
	@Industry varchar(100) = 'fin'
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @return varchar(max);

	SELECT @return = 
	(SELECT TOP 20
		A.[Industry] [Text], A.[Industry] [KeyValue]
	FROM
		dbo.[LeadIndustry] A
	WHERE
		A.[Industry] LIKE '%' + @Industry + '%'
	ORDER BY
		A.[Industry] ASC
	FOR JSON PATH);

	SELECT @return;
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE   Procedure [Admin_SearchJobOption]
	@JobOption varchar(100) = 'Contract'
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @return varchar(max);

	SELECT @return = 
	(SELECT TOP 20
		A.[JobOptions] [Text], A.[JobOptions] [KeyValue]
	FROM
		[ProfessionalMaster].dbo.[JobOptions] A
	WHERE
		A.[JobOptions] LIKE '%' + @JobOption + '%'
	FOR JSON PATH);

	SELECT @return;
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE   Procedure [Admin_SearchLeadSource]
	@LeadSource varchar(100) = 'adv'
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @return varchar(max);

	SELECT @return = 
	(SELECT TOP 20
		A.[LeadSource] [Text], A.[LeadSource] [KeyValue]
	FROM
		dbo.[LeadSource] A
	WHERE
		A.[LeadSource] LIKE '%' + @LeadSource + '%'
	ORDER BY
		A.[LeadSource] ASC
	FOR JSON PATH);

	SELECT @return;
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE   PROCEDURE [Admin_SearchLeadStatus]
	@LeadStatus varchar(100) = ''
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @return varchar(max);

	SELECT @return = 
	(SELECT TOP 20
		A.[LeadStatus] [Text], A.[LeadStatus] [KeyValue]
	FROM
		dbo.[LeadStatus] A
	WHERE
		A.[LeadStatus] LIKE '%' + @LeadStatus + '%'
	FOR JSON AUTO);

	SELECT @return
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE   Procedure [Admin_SearchNAICS]
	@NAICS varchar(100) = 'far'
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @return varchar(max);

	SELECT @return = 
	(SELECT TOP 20
		A.[NAICSTitle] [Text], A.[NAICSTitle] [KeyValue]
	FROM
		dbo.[NAICS] A
	WHERE
		A.[NAICSTitle] LIKE '%' + @NAICS + '%'
	FOR JSON AUTO);

	SELECT @return
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE   Procedure [Admin_SearchRole]
	@Role varchar(100) = 'adm'
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @return varchar(max);

	SELECT @return = 
	(SELECT TOP 20
		A.[RoleDescription] [Text], A.[RoleDescription] [KeyValue]
	FROM
		dbo.[Roles] A
	WHERE
		A.[RoleDescription] LIKE '%' + @Role + '%'
	FOR JSON AUTO);

	SELECT @return
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE   Procedure [Admin_SearchSkill]
	@Skill varchar(100) = 'sql'
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @return varchar(max);

	SELECT @return = 
	(SELECT TOP 20
		A.[Skill] [Text], A.[Skill] [KeyValue]
	FROM
		dbo.[Skills] A
	WHERE
		A.[Skill] LIKE '%' + @Skill + '%'
	FOR JSON AUTO);

	SELECT @return
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE   Procedure [Admin_SearchState]
	@State varchar(100) = ''
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @return varchar(max);

	SELECT @return = 
	(SELECT TOP 20
		A.[State] [Text], A.[State] [KeyValue]
	FROM
		dbo.[State] A
	WHERE
		A.[State] LIKE '%' + @State + '%'
	FOR JSON AUTO);

	SELECT @return
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE     Procedure [Admin_SearchStatusCode]
	@StatusCode varchar(100) = 'cand'
AS
BEGIN
	SET NOCOUNT ON;

	SELECT DISTINCT
		A.[Status]
	FROM
		[ProfessionalMaster].dbo.[StatusCode] A
	WHERE
		A.[Status] LIKE '%' + @StatusCode + '%';
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE   Procedure [Admin_SearchTaxTerm]
	@TaxTerm varchar(100) = ''
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @return varchar(max);

	SELECT @return = 
	(SELECT TOP 20
		A.[TaxTerm] [Text], A.[TaxTerm] [KeyValue]
	FROM
		dbo.[TaxTerm] A
	WHERE
		A.[TaxTerm] LIKE '%' + @TaxTerm + '%'
	FOR JSON AUTO);

	SELECT @return
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE     Procedure [Admin_SearchUser]
	@User varchar(100) = ''
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @return varchar(max);

	SELECT @return = 
	(SELECT TOP 20
		A.[UserName] [Text], A.[UserName] [KeyValue]
	FROM
		dbo.[Users] A
	WHERE
		A.[UserName] LIKE '%' + @User + '%'
	ORDER BY
		A.[UserName] ASC
	FOR JSON PATH);

	SELECT @return;
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE   Procedure [Admin_SearchWorkflow]
	@Workflow varchar(100) = 'pen'
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @return varchar(max);

	SELECT @return = 
	(SELECT
		'[' + A.[Step] + '] - ' + B.[Status] [Text], '[' + A.[Step] + '] - ' + B.[Status] [KeyValue]
	FROM
		dbo.[WorkflowActivity] A INNER JOIN dbo.[StatusCode] B ON A.[Step] = B.[StatusCode]
		AND B.[AppliesTo] = 'SCN'
	WHERE
		'[' + A.[Step] + '] - ' + B.[Status] LIKE '%' + @Workflow + '%'
	ORDER BY
		'[' + A.[Step] + '] - ' + B.[Status] ASC
	FOR JSON PATH);

	SELECT @return;
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE procedure [Admin_ToggleCandidateStatus]
	@Id int,
	@User varchar(10)
as
BEGIN
	SET NOCOUNT ON;

	DECLARE @Description varchar(7000), @Action varchar(30), @Enabled bit
	DECLARE @Status char(3);

	SELECT
		@Status = A.[Status]
	FROM
		dbo.[Candidate] A
	WHERE
		A.[Id] = @Id;

	UPDATE
		[Candidate]
	SET 
		[Status] = CASE @Status WHEN 'AVL' THEN 'UAV' ELSE 'AVL' END
	WHERE
		[Id] = @Id;

	SELECT
		@Status = A.[Status]
	FROM
		dbo.[Candidate] A
	WHERE
		A.[Id] = @Id;

	SET @Description = 'Toggled Status for Candidate: ' + CAST(@Id as varchar(5)) + ' to ' + @Status;

	SET @Action = 'CHANGE STATUS';
						
	execute [dbo].[AddAuditTrail] @Action, 'Admin Candidate', @Description, @User;

END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE procedure [Admin_ToggleDesignationStatus]
	@Id int,
	@User varchar(10)
as
BEGIN
	SET NOCOUNT ON;

	DECLARE @Description varchar(7000), @Action varchar(30), @Enabled bit
	
	UPDATE
		dbo.[Designation]
	SET 
		[Enabled] = [Enabled] ^ 1
	WHERE
		[Id] = @Id

	SELECT
		@Enabled = A.[Enabled]
	FROM
		dbo.[Designation] A
	WHERE
		A.[Id] = @Id

	SET @Description = 'Toggled Status for Designation: ' + CAST(@Id as varchar(5)) + ' to ' + CASE @Enabled WHEN 1 THEN 'Enabled' ELSE 'Disabled' END;

	SET @Action = 'CHANGE STATUS';

	execute dbo.[Admin_GetDesignations];
						
	execute [dbo].[AddAuditTrail] @Action, 'Admin Designation', @Description, @User;

END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE procedure [Admin_ToggleEducationStatus]
	@Id int,
	@User varchar(10)
as
BEGIN
	SET NOCOUNT ON;

	DECLARE @Description varchar(7000), @Action varchar(30), @Enabled bit
	
	UPDATE
		dbo.[Education]
	SET 
		[Enabled] = [Enabled] ^ 1
	WHERE
		[Id] = @Id

	SELECT
		@Enabled = A.[Enabled]
	FROM
		dbo.[Education] A
	WHERE
		A.[Id] = @Id

	SET @Description = 'Toggled Status for Education: ' + CAST(@Id as varchar(5)) + ' to ' + CASE @Enabled WHEN 1 THEN 'Enabled' ELSE 'Disabled' END;

	SET @Action = 'CHANGE STATUS';

	execute dbo.[Admin_GetEducation];
						
	execute [dbo].[AddAuditTrail] @Action, 'Admin Education', @Description, @User;

END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE procedure [Admin_ToggleEligibilityStatus]
	@Id int,
	@User varchar(10)
as
BEGIN
	SET NOCOUNT ON;

	DECLARE @Description varchar(7000), @Action varchar(30), @Enabled bit
	
	UPDATE
		dbo.[Eligibility]
	SET 
		[Enabled] = [Enabled] ^ 1
	WHERE
		[Id] = @Id

	SELECT
		@Enabled = A.[Enabled]
	FROM
		dbo.[Eligibility] A
	WHERE
		A.[Id] = @Id

	SET @Description = 'Toggled Status for Eligibility: ' + CAST(@Id as varchar(5)) + ' to ' + CASE @Enabled WHEN 1 THEN 'Enabled' ELSE 'Disabled' END;

	SET @Action = 'CHANGE STATUS';

	execute dbo.[Admin_GetEligibility];
						
	execute [dbo].[AddAuditTrail] @Action, 'Admin Eligibility', @Description, @User;

END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE procedure [Admin_ToggleExperienceStatus]
	@Id int,
	@User varchar(10)
as
BEGIN
	SET NOCOUNT ON;

	DECLARE @Description varchar(7000), @Action varchar(30), @Enabled bit
	
	UPDATE
		dbo.[Experience]
	SET 
		[Enabled] = [Enabled] ^ 1
	WHERE
		[Id] = @Id

	SELECT
		@Enabled = A.[Enabled]
	FROM
		dbo.[Experience] A
	WHERE
		A.[Id] = @Id

	SET @Description = 'Toggled Status for Experience: ' + CAST(@Id as varchar(5)) + ' to ' + CASE @Enabled WHEN 1 THEN 'Enabled' ELSE 'Disabled' END;

	SET @Action = 'CHANGE STATUS';
		
	execute dbo.[Admin_GetExperience];

	execute [dbo].[AddAuditTrail] @Action, 'Admin Experience', @Description, @User;

END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE   procedure [Admin_ToggleIndustryStatus]
	@Id int,
	@User varchar(10)
as
BEGIN
	SET NOCOUNT ON;

	DECLARE @Description varchar(7000), @Action varchar(30), @Enabled bit
	
	UPDATE
		dbo.[LeadIndustry]
	SET 
		[Enabled] = [Enabled] ^ 1
	WHERE
		[Id] = @Id

	SELECT
		@Enabled = A.[Enabled]
	FROM
		dbo.[LeadIndustry] A
	WHERE
		A.[Id] = @Id

	SET @Description = 'Toggled Status for Industry: ' + CAST(@Id as varchar(5)) + ' to ' + CASE @Enabled WHEN 1 THEN 'Enabled' ELSE 'Disabled' END;

	SET @Action = 'CHANGE STATUS';

	execute dbo.[Admin_GetIndustries];
						
	execute [dbo].[AddAuditTrail] @Action, 'Admin Industry', @Description, @User;

END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE     procedure [Admin_ToggleLeadSourceStatus]
	@Id int,
	@User varchar(10)
as
BEGIN
	SET NOCOUNT ON;

	DECLARE @Description varchar(7000), @Action varchar(30), @Enabled bit
	
	UPDATE
		dbo.[LeadSource]
	SET 
		[Enabled] = [Enabled] ^ 1
	WHERE
		[Id] = @Id

	SELECT
		@Enabled = A.[Enabled]
	FROM
		dbo.[LeadSource] A
	WHERE
		A.[Id] = @Id

	SET @Description = 'Toggled Status for LeadSource: ' + CAST(@Id as varchar(5)) + ' to ' + CASE @Enabled WHEN 1 THEN 'Enabled' ELSE 'Disabled' END;

	SET @Action = 'CHANGE STATUS';

	execute dbo.[Admin_GetLeadSources];
						
	execute [dbo].[AddAuditTrail] @Action, 'Admin LeadSource', @Description, @User;

END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE     procedure [Admin_ToggleLeadStatusStatus]
	@Id int,
	@User varchar(10)
as
BEGIN
	SET NOCOUNT ON;

	DECLARE @Description varchar(7000), @Action varchar(30), @Enabled bit
	
	UPDATE
		dbo.[LeadStatus]
	SET 
		[Enabled] = [Enabled] ^ 1
	WHERE
		[Id] = @Id

	SELECT
		@Enabled = A.[Enabled]
	FROM
		dbo.[LeadStatus] A
	WHERE
		A.[Id] = @Id

	SET @Description = 'Toggled Status for LeadStatus: ' + CAST(@Id as varchar(5)) + ' to ' + CASE @Enabled WHEN 1 THEN 'Enabled' ELSE 'Disabled' END;

	SET @Action = 'CHANGE STATUS';

	execute dbo.[Admin_GetLeadStatuses];
						
	execute [dbo].[AddAuditTrail] @Action, 'Admin LeadStatus', @Description, @User;

END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE procedure [Admin_ToggleSkillStatus]
	@Id int,
	@User varchar(10)
as
BEGIN
	SET NOCOUNT ON;

	DECLARE @Description varchar(7000), @Action varchar(30), @Enabled bit
	
	UPDATE
		dbo.[Skills]
	SET 
		[Enabled] = [Enabled] ^ 1
	WHERE
		[Id] = @Id

	SELECT
		@Enabled = A.[Enabled]
	FROM
		dbo.[Skills] A
	WHERE
		A.[Id] = @Id

	SET @Description = 'Toggled Status for Skills: ' + CAST(@Id as varchar(5)) + ' to ' + CASE @Enabled WHEN 1 THEN 'Enabled' ELSE 'Disabled' END;

	SET @Action = 'CHANGE STATUS';
						
	execute dbo.[Admin_GetSkills];

	execute [dbo].[AddAuditTrail] @Action, 'Admin Skills', @Description, @User;

END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE procedure [Admin_ToggleTaxTermStatus]
	@Code char(1),
	@User varchar(10)
as
BEGIN
	SET NOCOUNT ON;

	DECLARE @Description varchar(7000), @Action varchar(30), @Enabled bit
	
	UPDATE
		dbo.[TaxTerm]
	SET 
		[Enabled] = [Enabled] ^ 1,
		[UpdateDate] = GETDATE()
	WHERE
		[TaxTermCode] = @Code

	SELECT
		@Enabled = A.[Enabled]
	FROM
		dbo.[TaxTerm] A
	WHERE
		A.[TaxTermCode] = @Code

	SET @Description = 'Toggled Status for Tax Term: ' + @Code + ' to ' + CASE @Enabled WHEN 1 THEN 'Enabled' ELSE 'Disabled' END;

	SET @Action = 'CHANGE STATUS';

	execute dbo.[Admin_GetTaxTerms];
						
	execute [dbo].[AddAuditTrail] @Action, 'Admin TaxTerm', @Description, @User;

END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE procedure [Admin_ToggleTemplateStatus]
	@Id int,
	@User varchar(10)
as
BEGIN
	SET NOCOUNT ON;

	DECLARE @Description varchar(7000), @Action varchar(30), @Enabled bit
	
	UPDATE
		dbo.[Templates]
	SET 
		[Enabled] = [Enabled] ^ 1
	WHERE
		[Id] = @Id

	SELECT
		@Enabled = A.[Enabled]
	FROM
		dbo.[Templates] A
	WHERE
		A.[Id] = @Id

	SET @Description = 'Toggled Status for Template: ' + CAST(@Id as varchar(5)) + ' to ' + CASE @Enabled WHEN 1 THEN 'Enabled' ELSE 'Disabled' END;

	SET @Action = 'CHANGE STATUS';
						
	execute [dbo].[AddAuditTrail] @Action, 'Admin Template', @Description, @User;

END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE   Procedure [Admin_ToggleUserStatus]
	@Code varchar(10),
	@User varchar(10)
as
BEGIN
	SET NOCOUNT ON;

	DECLARE @Description varchar(7000), @Action varchar(30), @Enabled bit
	
	UPDATE
		dbo.[Users]
	SET 
		[Status] = [Status] ^ 1
	WHERE
		[UserName] = @Code

	SELECT
		@Enabled = A.[Status]
	FROM
		dbo.[Users] A
	WHERE
		A.[UserName] = @Code

	SET @Description = 'Toggled Status for User Name: ' + @Code + ' to ' + CASE @Enabled WHEN 1 THEN 'Enabled' ELSE 'Disabled' END;

	SET @Action = 'CHANGE USER STATUS';

	execute dbo.[Admin_GetUsers];
						
	execute [dbo].[AddAuditTrail] @Action, 'Admin Users', @Description, @User;

END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE   PROCEDURE [ChangeCandidateStatus]
	@CandidateID int = 0,
	@User varchar(10) = 'JOLLY'
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	SET ARITHABORT OFF;

    if (EXISTS(SELECT * FROM dbo.Candidate WHERE ID = @CandidateID))
		BEGIN
			DECLARE @Status char(3) = 'AVL';

			SELECT
				@Status = A.Status
			FROM
				dbo.[Candidate] A
			WHERE
				A.ID = @CandidateID;

			if (@Status = 'AVL')
				BEGIN
					UPDATE
						dbo.Candidate
					SET
						Status = 'UAV'
					WHERE
						ID = @CandidateID;

					SELECT 'Unavailable';
				END
			else
				BEGIN
					UPDATE
						dbo.Candidate
					SET
						Status = 'AVL'
					WHERE
						ID = @CandidateID;

					SELECT 'Available';
				END
		END

END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [ChangeCollationForAllColumns]
AS
BEGIN
    DECLARE @collate NVARCHAR(100);
    SET @collate = 'Latin1_General_CI_AI'; -- Specify your desired collation

    DECLARE @table NVARCHAR(255);
    DECLARE @column_name NVARCHAR(255);
    DECLARE @data_type NVARCHAR(255);
    DECLARE @max_length INT;
    DECLARE @sql NVARCHAR(MAX);

    DECLARE local_table_cursor CURSOR FOR
    SELECT [name] FROM sysobjects WHERE OBJECTPROPERTY(id, N'IsUserTable') = 1;

    OPEN local_table_cursor;
    FETCH NEXT FROM local_table_cursor INTO @table;

    WHILE @@FETCH_STATUS = 0
    BEGIN
        DECLARE local_change_cursor CURSOR FOR
        SELECT c.name AS column_name,
               t.name AS data_type,
               c.max_length
        FROM sys.columns c
        JOIN sys.types t ON c.system_type_id = t.system_type_id
        WHERE c.object_id = OBJECT_ID(@table)
          AND t.name = 'varchar'; -- Filter only VARCHAR columns

        OPEN local_change_cursor;
        FETCH NEXT FROM local_change_cursor INTO @column_name, @data_type, @max_length;

        WHILE @@FETCH_STATUS = 0
        BEGIN
            SET @sql = 'ALTER TABLE [' + @table + '] ALTER COLUMN [' + @column_name + '] ' +
                        @data_type + '(' + CAST(@max_length AS NVARCHAR(100)) + ') COLLATE ' + @collate;
            EXEC sp_executesql @sql;

            FETCH NEXT FROM local_change_cursor INTO @column_name, @data_type, @max_length;
        END;

        CLOSE local_change_cursor;
        DEALLOCATE local_change_cursor;

        FETCH NEXT FROM local_table_cursor INTO @table;
    END;

    CLOSE local_table_cursor;
    DEALLOCATE local_table_cursor;
END;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/*
Retrieve the Current Date and Time: Use GETDATE() to get the current date and time, formatted as a string.

Create a New JSON Entry: Construct a new JSON entry with the provided MPC, Notes, and From values.

Retrieve Existing JSON: Fetch the existing JSON data from the MPCNotes column in the Candidate table.

Check for NULL, Empty, or [] JSON: If the existing JSON is NULL, empty, or [], initialize a new JSON array with the new entry.

Append New JSON Entry: If the existing JSON is not NULL, empty, or [], append the new JSON entry to the existing JSON array using JSON_MODIFY.

Update the Candidate Table: Update the MPC and MPCNotes columns in the Candidate table with the new JSON data.
*/

CREATE     PROCEDURE [ChangeMPC]
    @CandidateId int = 19,
    @MPC bit = 1,
    @Notes varchar(255) = 'Some Valooable candidate',
    @From varchar(10) = 'MANI'
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @DateString varchar(19), @NewJsonEntry varchar(MAX), @ExistingJson varchar(MAX);

    -- Get the current date and time
    SELECT @DateString = CONVERT(VARCHAR(19), GETDATE(), 126);

    -- Create a new JSON entry
    SET @NewJsonEntry = '{"DateTime":"' + @DateString + '","Name":"' + @From + '","MPC":' + IIF(@MPC=1, 'true', 'false') + ',"Comment":"' + @Notes + '"}';

    -- Retrieve the existing JSON from the Candidate table
    SELECT 
		@ExistingJson = MPCNotes
    FROM 
		[dbo].[Candidate]
    WHERE 
		Id = @CandidateId;

    -- Determine if the existing JSON is NULL, empty, or []
    if (@ExistingJson IS NULL OR @ExistingJson = '' OR @ExistingJson = '[]')
		BEGIN
			-- Initialize a new JSON array with the new entry
			SET @ExistingJson = '[' + @NewJsonEntry + ']';
		END
    else
		BEGIN
			-- Append the new JSON entry to the existing JSON array
			SET @ExistingJson = JSON_MODIFY(@ExistingJson, 'append $', JSON_QUERY(@NewJsonEntry));
		END

    -- Update the Candidate table with the new JSON data
    UPDATE 
		[dbo].[Candidate]
    SET
        [MPC] = @MPC,
        [MPCNotes] = @ExistingJson
    WHERE
        [Id] = @CandidateId;

	--Fetch the Candidate Full Name
	DECLARE @Name varchar(100);
	SELECT
		@Name = A.[FirstName] + ' ' + A.[LastName]
	FROM
		[dbo].[Candidate] A
	WHERE
		A.[Id] = @CandidateId;
		
	--Create the Description for Audit Trail and Insert the Audit Trail
	DECLARE @Description varchar(7000);
	SET @Description = 'Changed MPC for ' + @Name + ', [ID: ' + CAST(@CandidateId as varchar(10)) + '] to: ' + 
						IIF(@MPC = 1, 'Yes', 'No');

	execute [dbo].[AddAuditTrail] 'Update Candidate MPC', 'Candidate Details', @Description, @From;
	
	--Fetch the MPC Notes JSON string
	SELECT
		A.[MPCNotes]
	FROM
		dbo.[Candidate] A
	WHERE
		A.[Id] = @CandidateId;

END


GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/*
Retrieve the Current Date and Time: Use GETDATE() to get the current date and time, formatted as a string.

Create a New JSON Entry: Construct a new JSON entry with the provided Rating, Notes, and From values.

Retrieve Existing JSON: Fetch the existing JSON data from the RateNotes column in the Candidate table.

Check for NULL, Empty, or [] JSON: If the existing JSON is NULL, empty, or [], initialize a new JSON array with the new entry.

Append New JSON Entry: If the existing JSON is not NULL, empty, or [], append the new JSON entry to the existing JSON array using JSON_MODIFY.

Update the Candidate Table: Update the RateCandidate and RateNotes columns in the Candidate table with the new JSON data.
*/

CREATE   PROCEDURE [ChangeRating]
    @CandidateId int = 19,
    @Rating tinyint = 1,
    @Notes varchar(255) = 'Some Goooooood candidate',
    @From varchar(10) = 'MANI'
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @DateString varchar(19), @NewJsonEntry varchar(MAX), @ExistingJson varchar(MAX);

    -- Get the current date and time
    SELECT @DateString = CONVERT(VARCHAR(19), GETDATE(), 126);

    -- Create a new JSON entry
    SET @NewJsonEntry = '{"DateTime":"' + @DateString + '","Name":"' + @From + '","Rating":' + CAST(@Rating AS NVARCHAR) + ',"Comment":"' + @Notes + '"}';

    -- Retrieve the existing JSON from the Candidate table
    SELECT 
		@ExistingJson = RateNotes
    FROM 
		[dbo].[Candidate]
    WHERE 
		Id = @CandidateId;

    -- Determine if the existing JSON is NULL, empty, or []
    if (@ExistingJson IS NULL OR @ExistingJson = '' OR @ExistingJson = '[]')
		BEGIN
			-- Initialize a new JSON array with the new entry
			SET @ExistingJson = '[' + @NewJsonEntry + ']';
		END
    else
		BEGIN
			-- Append the new JSON entry to the existing JSON array
			SET @ExistingJson = JSON_MODIFY(@ExistingJson, 'append $', JSON_QUERY(@NewJsonEntry));
		END

    -- Update the Candidate table with the new JSON data
    UPDATE 
		[dbo].[Candidate]
    SET
        [RateCandidate] = @Rating,
        [RateNotes] = @ExistingJson
    WHERE
        [Id] = @CandidateId;

	--Fetch the Candidate Full Name
	DECLARE @Name varchar(100);
	SELECT
		@Name = A.[FirstName] + ' ' + A.[LastName]
	FROM
		[dbo].[Candidate] A
	WHERE
		A.[Id] = @CandidateId;
		
	--Create the Description for Audit Trail and Insert the Audit Trail
	DECLARE @Description varchar(7000);
	SET @Description = 'Changed Rating for ' + @Name + ', [ID: ' + CAST(@CandidateId as varchar(10)) + '] to: ' + 
						CAST(@Rating as varchar(3));

	execute [dbo].[AddAuditTrail] 'Update Candidate Rating', 'Candidate Details', @Description, @From;
	
	--Fetch the MPC Notes JSON string
	SELECT
		A.[RateNotes]
	FROM
		dbo.[Candidate] A
	WHERE
		A.[Id] = @CandidateId;
END


GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE   PROCEDURE [ChangeRequisitionStatus]
	@RequisitionID int = 0,
	@Status char(3) = 'NEW',
	@User varchar(10) = 'JOLLY'
AS
BEGIN
	SET NOCOUNT ON;
	SET ARITHABORT ON;
	SET ANSI_NULLS ON;
	SET QUOTED_IDENTIFIER ON;

	if (@RequisitionID IS NOT NULL AND @RequisitionID <> 0 AND EXISTS(SELECT * FROM dbo.Requisitions WHERE Id = @RequisitionID))
		BEGIN
			UPDATE
				dbo.[Requisitions]
			SET
				[Status] = @Status,
				UpdatedDate = GETDATE(),
				UpdatedBy = @User
			WHERE 
				Id = @RequisitionID;

			SELECT
				A.[Status]
			FROM
				dbo.[StatusCode] A
			WHERE
				StatusCode = @Status
				AND A.[AppliesTo] = 'REQ'
		END
	else
		SELECT ''
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE       PROCEDURE [CheckEIN]
	@ID int = 2,
	@EIN varchar(10) = '123456789'
AS
BEGIN
	SET NOCOUNT ON;

	if (@ID = NULL OR @ID = 0)
		BEGIN
			SELECT CAST(CASE WHEN EXISTS(SELECT * FROM dbo.[Companies] A WHERE A.[EIN] = @EIN) THEN 1 ELSE 0 END AS bit);
		END
	else
		BEGIN
			SELECT CAST(CASE WHEN EXISTS(SELECT * FROM dbo.[Companies] A WHERE A.[EIN] = @EIN AND A.[Id] <> @ID) THEN 1 ELSE 0 END AS bit);
		END
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [Dashboard_Refactor_ActiveUsers]
AS
BEGIN
    SET NOCOUNT ON;
    SET ARITHABORT ON;
    
    DECLARE @RefreshStart DATETIME2 = SYSDATETIME();
    
    BEGIN TRANSACTION;

    BEGIN TRY
        -- Clear existing data
        DELETE FROM [dbo].[ActiveUsersView];
        
        -- Populate ActiveUsersView with all active users who have created requisitions
        INSERT INTO 
            [dbo].[ActiveUsersView] 
            ([CreatedBy], [FullName], [Role], [Status], [LastRequisitionDate], [RequisitionCount], [RefreshDate])
        SELECT 
            U.UserName as CreatedBy, LTRIM(RTRIM(U.FirstName + ' ' + U.LastName)) as FullName, U.Role, U.Status, ISNULL(MAX(R.CreatedDate), '2000-01-01') as LastRequisitionDate,
            COUNT(DISTINCT R.Id) as RequisitionCount, GETDATE() as RefreshDate
        FROM 
            dbo.Users U LEFT JOIN dbo.Requisitions R ON U.UserName = R.CreatedBy AND R.CreatedDate >= DATEADD(YEAR, -1, GETDATE()) -- Only last year's activity
        WHERE 
            U.Status = 1 AND U.UserName IS NOT NULL
        GROUP BY 
            U.UserName, U.FirstName, U.LastName, U.Role, U.Status;
        
        COMMIT TRANSACTION;

        -- Log success
        DECLARE @Duration_MS INT = DATEDIFF(MILLISECOND, @RefreshStart, SYSDATETIME());
        PRINT 'ActiveUsersView refreshed successfully in ' + CAST(@Duration_MS AS VARCHAR(10)) + 'ms';
        
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        -- Log error details
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
        DECLARE @ErrorState INT = ERROR_STATE();
        
        PRINT 'Error refreshing ActiveUsersView: ' + @ErrorMessage;
        RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
    END CATCH
END;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [Dashboard_Refactor_CandidateQualityMetrics]
AS
BEGIN
    SET NOCOUNT ON;
    SET ARITHABORT ON;
    
    DECLARE @RefreshStart DATETIME2 = SYSDATETIME();
    DECLARE @Today DATE = CAST(GETDATE() AS DATE);
    
    BEGIN TRANSACTION;

    BEGIN TRY
        -- Clear existing data
        DELETE FROM [dbo].[CandidateQualityMetricsView];
        
        -- Date calculations
        DECLARE @YearStart DATE = DATEFROMPARTS(YEAR(@Today), 1, 1);
        DECLARE @StartDate7D DATE = DATEADD(DAY, -6, @Today);
        DECLARE @StartDateMTD DATE = DATEADD(DAY, 1, EOMONTH(@Today, -1));
        DECLARE @StartDateQTD DATE = DATEADD(QUARTER, DATEDIFF(QUARTER, 0, @Today), 0);
        DECLARE @StartDateHYTD DATE = CASE 
            WHEN MONTH(@Today) <= 6 THEN DATEFROMPARTS(YEAR(@Today), 1, 1)
            ELSE DATEFROMPARTS(YEAR(@Today), 7, 1)
        END;
        
        -- Get latest submission status per candidate+requisition combo
        WITH LatestSubmissions AS (
            SELECT 
                S.CreatedBy,
                S.RequisitionId,
                S.CandidateId,
                S.Status,
                CAST(S.CreatedDate AS DATE) as SubmissionDate,
                ROW_NUMBER() OVER (
                    PARTITION BY S.CreatedBy, S.RequisitionId, S.CandidateId 
                    ORDER BY S.CreatedDate DESC
                ) as rn
            FROM dbo.Submissions S
            INNER JOIN dbo.ActiveUsersView A ON S.CreatedBy = A.CreatedBy
            WHERE A.Status = 1
        ),
        CurrentStatus AS (
            SELECT 
                CreatedBy,
                RequisitionId,
                CandidateId,
                Status,
                SubmissionDate
            FROM LatestSubmissions 
            WHERE rn = 1
        ),
        QualityMetrics AS (
            SELECT 
                U.CreatedBy,
                -- 7D metrics
                COUNT(DISTINCT CASE WHEN CS.SubmissionDate BETWEEN @StartDate7D AND @Today 
                    THEN CONCAT(CS.RequisitionId, '-', CS.CandidateId) END) as Total_7D,
                COUNT(DISTINCT CASE WHEN CS.SubmissionDate BETWEEN @StartDate7D AND @Today 
                    AND CS.Status IN ('INT', 'RHM', 'DEC', 'NOA', 'OEX', 'ODC', 'HIR', 'WDR')
                    THEN CONCAT(CS.RequisitionId, '-', CS.CandidateId) END) as INT_7D,
                COUNT(DISTINCT CASE WHEN CS.SubmissionDate BETWEEN @StartDate7D AND @Today 
                    AND CS.Status IN ('OEX', 'ODC', 'HIR', 'WDR')
                    THEN CONCAT(CS.RequisitionId, '-', CS.CandidateId) END) as OEX_7D,
                COUNT(DISTINCT CASE WHEN CS.SubmissionDate BETWEEN @StartDate7D AND @Today 
                    AND CS.Status = 'HIR'
                    THEN CONCAT(CS.RequisitionId, '-', CS.CandidateId) END) as HIR_7D,
                
                -- MTD metrics
                COUNT(DISTINCT CASE WHEN CS.SubmissionDate BETWEEN @StartDateMTD AND @Today 
                    THEN CONCAT(CS.RequisitionId, '-', CS.CandidateId) END) as Total_MTD,
                COUNT(DISTINCT CASE WHEN CS.SubmissionDate BETWEEN @StartDateMTD AND @Today 
                    AND CS.Status IN ('INT', 'RHM', 'DEC', 'NOA', 'OEX', 'ODC', 'HIR', 'WDR')
                    THEN CONCAT(CS.RequisitionId, '-', CS.CandidateId) END) as INT_MTD,
                COUNT(DISTINCT CASE WHEN CS.SubmissionDate BETWEEN @StartDateMTD AND @Today 
                    AND CS.Status IN ('OEX', 'ODC', 'HIR', 'WDR')
                    THEN CONCAT(CS.RequisitionId, '-', CS.CandidateId) END) as OEX_MTD,
                COUNT(DISTINCT CASE WHEN CS.SubmissionDate BETWEEN @StartDateMTD AND @Today 
                    AND CS.Status = 'HIR'
                    THEN CONCAT(CS.RequisitionId, '-', CS.CandidateId) END) as HIR_MTD,
                
                -- QTD metrics
                COUNT(DISTINCT CASE WHEN CS.SubmissionDate BETWEEN @StartDateQTD AND @Today 
                    THEN CONCAT(CS.RequisitionId, '-', CS.CandidateId) END) as Total_QTD,
                COUNT(DISTINCT CASE WHEN CS.SubmissionDate BETWEEN @StartDateQTD AND @Today 
                    AND CS.Status IN ('INT', 'RHM', 'DEC', 'NOA', 'OEX', 'ODC', 'HIR', 'WDR')
                    THEN CONCAT(CS.RequisitionId, '-', CS.CandidateId) END) as INT_QTD,
                COUNT(DISTINCT CASE WHEN CS.SubmissionDate BETWEEN @StartDateQTD AND @Today 
                    AND CS.Status IN ('OEX', 'ODC', 'HIR', 'WDR')
                    THEN CONCAT(CS.RequisitionId, '-', CS.CandidateId) END) as OEX_QTD,
                COUNT(DISTINCT CASE WHEN CS.SubmissionDate BETWEEN @StartDateQTD AND @Today 
                    AND CS.Status = 'HIR'
                    THEN CONCAT(CS.RequisitionId, '-', CS.CandidateId) END) as HIR_QTD,
                
                -- HYTD metrics
                COUNT(DISTINCT CASE WHEN CS.SubmissionDate BETWEEN @StartDateHYTD AND @Today 
                    THEN CONCAT(CS.RequisitionId, '-', CS.CandidateId) END) as Total_HYTD,
                COUNT(DISTINCT CASE WHEN CS.SubmissionDate BETWEEN @StartDateHYTD AND @Today 
                    AND CS.Status IN ('INT', 'RHM', 'DEC', 'NOA', 'OEX', 'ODC', 'HIR', 'WDR')
                    THEN CONCAT(CS.RequisitionId, '-', CS.CandidateId) END) as INT_HYTD,
                COUNT(DISTINCT CASE WHEN CS.SubmissionDate BETWEEN @StartDateHYTD AND @Today 
                    AND CS.Status IN ('OEX', 'ODC', 'HIR', 'WDR')
                    THEN CONCAT(CS.RequisitionId, '-', CS.CandidateId) END) as OEX_HYTD,
                COUNT(DISTINCT CASE WHEN CS.SubmissionDate BETWEEN @StartDateHYTD AND @Today 
                    AND CS.Status = 'HIR'
                    THEN CONCAT(CS.RequisitionId, '-', CS.CandidateId) END) as HIR_HYTD,
                
                -- YTD metrics
                COUNT(DISTINCT CASE WHEN CS.SubmissionDate BETWEEN @YearStart AND @Today 
                    THEN CONCAT(CS.RequisitionId, '-', CS.CandidateId) END) as Total_YTD,
                COUNT(DISTINCT CASE WHEN CS.SubmissionDate BETWEEN @YearStart AND @Today 
                    AND CS.Status IN ('INT', 'RHM', 'DEC', 'NOA', 'OEX', 'ODC', 'HIR', 'WDR')
                    THEN CONCAT(CS.RequisitionId, '-', CS.CandidateId) END) as INT_YTD,
                COUNT(DISTINCT CASE WHEN CS.SubmissionDate BETWEEN @YearStart AND @Today 
                    AND CS.Status IN ('OEX', 'ODC', 'HIR', 'WDR')
                    THEN CONCAT(CS.RequisitionId, '-', CS.CandidateId) END) as OEX_YTD,
                COUNT(DISTINCT CASE WHEN CS.SubmissionDate BETWEEN @YearStart AND @Today 
                    AND CS.Status = 'HIR'
                    THEN CONCAT(CS.RequisitionId, '-', CS.CandidateId) END) as HIR_YTD
            FROM dbo.ActiveUsersView U
            LEFT JOIN CurrentStatus CS ON U.CreatedBy = CS.CreatedBy
            WHERE U.Status = 1
            GROUP BY U.CreatedBy
        )
        
        -- Insert calculated metrics for all periods
        INSERT INTO [dbo].[CandidateQualityMetricsView]
        (CreatedBy, MetricDate, Period, Total_Submissions, Reached_INT, Reached_OEX, Reached_HIR, 
         PEN_to_INT_Ratio, INT_to_OEX_Ratio, OEX_to_HIR_Ratio, RefreshDate)
        SELECT 
            CreatedBy, @Today, Period, Total_Submissions, Reached_INT, Reached_OEX, Reached_HIR,
            CASE WHEN Total_Submissions > 0 THEN CAST((Reached_INT * 100.0 / Total_Submissions) AS DECIMAL(5,2)) ELSE 0.00 END,
            CASE WHEN Reached_INT > 0 THEN CAST((Reached_OEX * 100.0 / Reached_INT) AS DECIMAL(5,2)) ELSE 0.00 END,
            CASE WHEN Reached_OEX > 0 THEN CAST((Reached_HIR * 100.0 / Reached_OEX) AS DECIMAL(5,2)) ELSE 0.00 END,
            GETDATE()
        FROM (
            SELECT CreatedBy, '7D' as Period, Total_7D as Total_Submissions, INT_7D as Reached_INT, OEX_7D as Reached_OEX, HIR_7D as Reached_HIR FROM QualityMetrics
            UNION ALL
            SELECT CreatedBy, 'MTD' as Period, Total_MTD, INT_MTD, OEX_MTD, HIR_MTD FROM QualityMetrics
            UNION ALL
            SELECT CreatedBy, 'QTD' as Period, Total_QTD, INT_QTD, OEX_QTD, HIR_QTD FROM QualityMetrics
            UNION ALL
            SELECT CreatedBy, 'HYTD' as Period, Total_HYTD, INT_HYTD, OEX_HYTD, HIR_HYTD FROM QualityMetrics
            UNION ALL
            SELECT CreatedBy, 'YTD' as Period, Total_YTD, INT_YTD, OEX_YTD, HIR_YTD FROM QualityMetrics
        ) AllPeriods;
        
        COMMIT TRANSACTION;

        -- Log success
        DECLARE @Duration_MS INT = DATEDIFF(MILLISECOND, @RefreshStart, SYSDATETIME());
        PRINT 'CandidateQualityMetricsView refreshed successfully in ' + CAST(@Duration_MS AS VARCHAR(10)) + 'ms';
        
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
        DECLARE @ErrorState INT = ERROR_STATE();
        
        PRINT 'Error refreshing CandidateQualityMetricsView: ' + @ErrorMessage;
        RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
    END CATCH
END;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [Dashboard_Refactor_DateRanges]
AS
BEGIN
    SET NOCOUNT ON;
    SET ARITHABORT ON;
    
    DECLARE @RefreshStart DATETIME2 = SYSDATETIME();
    DECLARE @Today DATE = GETDATE();
    
    BEGIN TRY
        -- Clear existing data
        DELETE FROM [dbo].[DateRangeLookup];
        
        -- Populate DateRangeLookup with all period calculations
        INSERT INTO 
            [dbo].[DateRangeLookup] 
            ([Period], [StartDate], [EndDate], [RefreshDate])
        SELECT '7D' as Period, DATEADD(DAY, -7, @Today) as StartDate, @Today as EndDate, GETDATE() as RefreshDate
        UNION ALL
        SELECT 'MTD', DATEADD(MONTH, DATEDIFF(MONTH, 0, @Today), 0), @Today, GETDATE()
        UNION ALL
        SELECT 'QTD', DATEADD(QUARTER, DATEDIFF(QUARTER, 0, @Today), 0), @Today, GETDATE()
        UNION ALL
        SELECT 'HYTD', CASE WHEN MONTH(@Today) <= 6 THEN DATEFROMPARTS(YEAR(@Today), 1, 1) ELSE DATEFROMPARTS(YEAR(@Today), 7, 1) END, @Today, GETDATE()
        UNION ALL
        SELECT 'YTD', DATEADD(YEAR, DATEDIFF(YEAR, 0, @Today), 0), @Today, GETDATE();
        
        -- Log success
        DECLARE @Duration_MS INT = DATEDIFF(MILLISECOND, @RefreshStart, SYSDATETIME());
        PRINT 'DateRangeLookup refreshed successfully in ' + CAST(@Duration_MS AS VARCHAR(10)) + 'ms';
        
    END TRY
    BEGIN CATCH
        -- Log error details
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
        DECLARE @ErrorState INT = ERROR_STATE();
        
        PRINT 'Error refreshing DateRangeLookup: ' + @ErrorMessage;
        RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
    END CATCH
END;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [Dashboard_Refactor_SubmissionMetrics]
AS
BEGIN
    SET NOCOUNT ON;
    SET ARITHABORT ON;
    
    DECLARE @RefreshStart DATETIME2 = SYSDATETIME();
    DECLARE @Today DATE = GETDATE();
    
    -- Date range calculations (same as current dashboard logic)
    DECLARE @StartDate7D DATE = DATEADD(DAY, -7, @Today);
    DECLARE @StartDateMTD DATE = DATEADD(MONTH, DATEDIFF(MONTH, 0, @Today), 0);
    DECLARE @StartDateQTD DATE = DATEADD(QUARTER, DATEDIFF(QUARTER, 0, @Today), 0);
    DECLARE @StartDateHYTD DATE = CASE 
        WHEN MONTH(@Today) <= 6 THEN DATEFROMPARTS(YEAR(@Today), 1, 1)
        ELSE DATEFROMPARTS(YEAR(@Today), 7, 1)
    END;
    DECLARE @YearStart DATE = DATEADD(YEAR, DATEDIFF(YEAR, 0, @Today), 0);
    
    BEGIN TRY
        -- Clear ALL existing data before refresh
        DELETE FROM [dbo].[SubmissionMetricsView];
        
        -- ========================================================================
        -- COMPREHENSIVE SUBMISSION METRICS CALCULATION FOR ALL DASHBOARD QUERIES
        -- Handles: DashboardAccountsManager, DashboardRecruiter, DashboardAdmin
        -- ========================================================================
        
        WITH AllActiveUsers AS (
            -- Get all active users who have created requisitions or submissions
            SELECT DISTINCT U.UserName as CreatedBy, Role
            FROM dbo.Users U
            WHERE U.Status = 1 AND U.UserName IS NOT NULL
        ),
        
        -- ===== REQUISITION METRICS (Query 1 & 2) =====
        RequisitionMetrics AS (
            SELECT 
                R.CreatedBy,
                -- Query 1: Total Requisitions Created (by CreatedDate)
                SUM(CASE WHEN CAST(R.CreatedDate AS DATE) BETWEEN @StartDate7D AND @Today THEN 1 ELSE 0 END) as REQ_CREATED_7D,
                SUM(CASE WHEN CAST(R.CreatedDate AS DATE) BETWEEN @StartDateMTD AND @Today THEN 1 ELSE 0 END) as REQ_CREATED_MTD,
                SUM(CASE WHEN CAST(R.CreatedDate AS DATE) BETWEEN @StartDateQTD AND @Today THEN 1 ELSE 0 END) as REQ_CREATED_QTD,
                SUM(CASE WHEN CAST(R.CreatedDate AS DATE) BETWEEN @StartDateHYTD AND @Today THEN 1 ELSE 0 END) as REQ_CREATED_HYTD,
                SUM(CASE WHEN CAST(R.CreatedDate AS DATE) BETWEEN @YearStart AND @Today THEN 1 ELSE 0 END) as REQ_CREATED_YTD,
                
                -- Query 2: Active Requisitions (by UpdatedDate, specific statuses)
                SUM(CASE WHEN CAST(R.UpdatedDate AS DATE) BETWEEN @StartDate7D AND @Today AND R.Status IN ('OPN','NEW','PAR') THEN 1 ELSE 0 END) as ACTIVE_REQ_7D,
                SUM(CASE WHEN CAST(R.UpdatedDate AS DATE) BETWEEN @StartDateMTD AND @Today AND R.Status IN ('OPN','NEW','PAR') THEN 1 ELSE 0 END) as ACTIVE_REQ_MTD,
                SUM(CASE WHEN CAST(R.UpdatedDate AS DATE) BETWEEN @StartDateQTD AND @Today AND R.Status IN ('OPN','NEW','PAR') THEN 1 ELSE 0 END) as ACTIVE_REQ_QTD,
                SUM(CASE WHEN CAST(R.UpdatedDate AS DATE) BETWEEN @StartDateHYTD AND @Today AND R.Status IN ('OPN','NEW','PAR') THEN 1 ELSE 0 END) as ACTIVE_REQ_HYTD,
                SUM(CASE WHEN CAST(R.UpdatedDate AS DATE) BETWEEN @YearStart AND @Today AND R.Status IN ('OPN','NEW','PAR') THEN 1 ELSE 0 END) as ACTIVE_REQ_YTD
            FROM dbo.Requisitions R
            INNER JOIN AllActiveUsers AU ON R.CreatedBy = AU.CreatedBy
            GROUP BY R.CreatedBy
        ),
        
        -- ===== SUBMISSION METRICS (Role-specific logic) =====
        SubmissionCounts AS (
            SELECT 
                AU.CreatedBy,
                AU.Role,
                
                -- === TOTAL SUBMISSIONS (Role-specific) ===
                CASE 
                    -- Role 2: ADMIN - All submissions (their own submissions)
                    WHEN AU.Role = 2 THEN 
                        (SELECT COUNT(*) FROM Submissions S WHERE S.CreatedBy = AU.CreatedBy 
                         AND CAST(S.CreatedDate AS DATE) BETWEEN @StartDate7D AND @Today)
                    -- Role 4: RECRUITER - Their submissions only
                    WHEN AU.Role = 4 THEN 
                        (SELECT COUNT(*) FROM Submissions S WHERE S.CreatedBy = AU.CreatedBy 
                         AND CAST(S.CreatedDate AS DATE) BETWEEN @StartDate7D AND @Today)
                    -- Role 5: ACCOUNTS MANAGER - Submissions to their requisitions
                    WHEN AU.Role = 5 THEN 
                        (SELECT COUNT(*) FROM Submissions S INNER JOIN Requisitions R ON S.RequisitionId = R.Id 
                         WHERE R.CreatedBy = AU.CreatedBy AND CAST(S.CreatedDate AS DATE) BETWEEN @StartDate7D AND @Today)
                    -- Role 6: FULL DESK - Their submissions + submissions to their requisitions (no double-count)
                    WHEN AU.Role = 6 THEN 
                        (SELECT COUNT(DISTINCT CONCAT(S.RequisitionId, '-', S.CandidateId, '-', S.Id)) 
                         FROM Submissions S LEFT JOIN Requisitions R ON S.RequisitionId = R.Id
                         WHERE (S.CreatedBy = AU.CreatedBy OR R.CreatedBy = AU.CreatedBy)
                         AND CAST(S.CreatedDate AS DATE) BETWEEN @StartDate7D AND @Today)
                    ELSE 0
                END as SUB_TOTAL_7D,
                
                CASE 
                    WHEN AU.Role = 2 THEN 
                        (SELECT COUNT(*) FROM Submissions S WHERE S.CreatedBy = AU.CreatedBy 
                         AND CAST(S.CreatedDate AS DATE) BETWEEN @StartDateMTD AND @Today)
                    WHEN AU.Role = 4 THEN 
                        (SELECT COUNT(*) FROM Submissions S WHERE S.CreatedBy = AU.CreatedBy 
                         AND CAST(S.CreatedDate AS DATE) BETWEEN @StartDateMTD AND @Today)
                    WHEN AU.Role = 5 THEN 
                        (SELECT COUNT(*) FROM Submissions S INNER JOIN Requisitions R ON S.RequisitionId = R.Id 
                         WHERE R.CreatedBy = AU.CreatedBy AND CAST(S.CreatedDate AS DATE) BETWEEN @StartDateMTD AND @Today)
                    WHEN AU.Role = 6 THEN 
                        (SELECT COUNT(DISTINCT CONCAT(S.RequisitionId, '-', S.CandidateId, '-', S.Id)) 
                         FROM Submissions S LEFT JOIN Requisitions R ON S.RequisitionId = R.Id
                         WHERE (S.CreatedBy = AU.CreatedBy OR R.CreatedBy = AU.CreatedBy)
                         AND CAST(S.CreatedDate AS DATE) BETWEEN @StartDateMTD AND @Today)
                    ELSE 0
                END as SUB_TOTAL_MTD,
                
                CASE 
                    WHEN AU.Role = 2 THEN 
                        (SELECT COUNT(*) FROM Submissions S WHERE S.CreatedBy = AU.CreatedBy 
                         AND CAST(S.CreatedDate AS DATE) BETWEEN @StartDateQTD AND @Today)
                    WHEN AU.Role = 4 THEN 
                        (SELECT COUNT(*) FROM Submissions S WHERE S.CreatedBy = AU.CreatedBy 
                         AND CAST(S.CreatedDate AS DATE) BETWEEN @StartDateQTD AND @Today)
                    WHEN AU.Role = 5 THEN 
                        (SELECT COUNT(*) FROM Submissions S INNER JOIN Requisitions R ON S.RequisitionId = R.Id 
                         WHERE R.CreatedBy = AU.CreatedBy AND CAST(S.CreatedDate AS DATE) BETWEEN @StartDateQTD AND @Today)
                    WHEN AU.Role = 6 THEN 
                        (SELECT COUNT(DISTINCT CONCAT(S.RequisitionId, '-', S.CandidateId, '-', S.Id)) 
                         FROM Submissions S LEFT JOIN Requisitions R ON S.RequisitionId = R.Id
                         WHERE (S.CreatedBy = AU.CreatedBy OR R.CreatedBy = AU.CreatedBy)
                         AND CAST(S.CreatedDate AS DATE) BETWEEN @StartDateQTD AND @Today)
                    ELSE 0
                END as SUB_TOTAL_QTD,
                
                CASE 
                    WHEN AU.Role = 2 THEN 
                        (SELECT COUNT(*) FROM Submissions S WHERE S.CreatedBy = AU.CreatedBy 
                         AND CAST(S.CreatedDate AS DATE) BETWEEN @StartDateHYTD AND @Today)
                    WHEN AU.Role = 4 THEN 
                        (SELECT COUNT(*) FROM Submissions S WHERE S.CreatedBy = AU.CreatedBy 
                         AND CAST(S.CreatedDate AS DATE) BETWEEN @StartDateHYTD AND @Today)
                    WHEN AU.Role = 5 THEN 
                        (SELECT COUNT(*) FROM Submissions S INNER JOIN Requisitions R ON S.RequisitionId = R.Id 
                         WHERE R.CreatedBy = AU.CreatedBy AND CAST(S.CreatedDate AS DATE) BETWEEN @StartDateHYTD AND @Today)
                    WHEN AU.Role = 6 THEN 
                        (SELECT COUNT(DISTINCT CONCAT(S.RequisitionId, '-', S.CandidateId, '-', S.Id)) 
                         FROM Submissions S LEFT JOIN Requisitions R ON S.RequisitionId = R.Id
                         WHERE (S.CreatedBy = AU.CreatedBy OR R.CreatedBy = AU.CreatedBy)
                         AND CAST(S.CreatedDate AS DATE) BETWEEN @StartDateHYTD AND @Today)
                    ELSE 0
                END as SUB_TOTAL_HYTD,
                
                CASE 
                    WHEN AU.Role = 2 THEN 
                        (SELECT COUNT(*) FROM Submissions S WHERE S.CreatedBy = AU.CreatedBy 
                         AND CAST(S.CreatedDate AS DATE) BETWEEN @YearStart AND @Today)
                    WHEN AU.Role = 4 THEN 
                        (SELECT COUNT(*) FROM Submissions S WHERE S.CreatedBy = AU.CreatedBy 
                         AND CAST(S.CreatedDate AS DATE) BETWEEN @YearStart AND @Today)
                    WHEN AU.Role = 5 THEN 
                        (SELECT COUNT(*) FROM Submissions S INNER JOIN Requisitions R ON S.RequisitionId = R.Id 
                         WHERE R.CreatedBy = AU.CreatedBy AND CAST(S.CreatedDate AS DATE) BETWEEN @YearStart AND @Today)
                    WHEN AU.Role = 6 THEN 
                        (SELECT COUNT(DISTINCT CONCAT(S.RequisitionId, '-', S.CandidateId, '-', S.Id)) 
                         FROM Submissions S LEFT JOIN Requisitions R ON S.RequisitionId = R.Id
                         WHERE (S.CreatedBy = AU.CreatedBy OR R.CreatedBy = AU.CreatedBy)
                         AND CAST(S.CreatedDate AS DATE) BETWEEN @YearStart AND @Today)
                    ELSE 0
                END as SUB_TOTAL_YTD,
                
                -- === ACTIVE REQUISITIONS WORKED ON (Role-specific) ===
                CASE 
                    -- Role 2: ADMIN - Requisitions they submitted to (admin acting as recruiter)
                    WHEN AU.Role = 2 THEN 
                        (SELECT COUNT(DISTINCT S.RequisitionId) FROM Submissions S 
                         LEFT JOIN Requisitions R ON S.RequisitionId = R.Id
                         WHERE S.CreatedBy = AU.CreatedBy 
                         AND CAST(S.CreatedDate AS DATE) BETWEEN @StartDate7D AND @Today 
                         AND R.Status IN ('NEW', 'OPN', 'PAR'))
                    -- Role 4: RECRUITER - Requisitions they submitted to
                    WHEN AU.Role = 4 THEN 
                        (SELECT COUNT(DISTINCT S.RequisitionId) FROM Submissions S 
                         LEFT JOIN Requisitions R ON S.RequisitionId = R.Id
                         WHERE S.CreatedBy = AU.CreatedBy 
                         AND CAST(S.CreatedDate AS DATE) BETWEEN @StartDate7D AND @Today 
                         AND R.Status IN ('NEW', 'OPN', 'PAR'))
                    -- Role 5: ACCOUNTS MANAGER - Their requisitions that received submissions
                    WHEN AU.Role = 5 THEN 
                        (SELECT COUNT(DISTINCT S.RequisitionId) FROM Submissions S 
                         LEFT JOIN Requisitions R ON S.RequisitionId = R.Id
                         WHERE R.CreatedBy = AU.CreatedBy 
                         AND CAST(S.CreatedDate AS DATE) BETWEEN @StartDate7D AND @Today 
                         AND R.Status IN ('NEW', 'OPN', 'PAR'))
                    -- Role 6: FULL DESK - Both perspectives, no double-counting
                    WHEN AU.Role = 6 THEN 
                        (SELECT COUNT(DISTINCT S.RequisitionId) FROM Submissions S 
                         LEFT JOIN Requisitions R ON S.RequisitionId = R.Id
                         WHERE (S.CreatedBy = AU.CreatedBy OR R.CreatedBy = AU.CreatedBy)
                         AND CAST(S.CreatedDate AS DATE) BETWEEN @StartDate7D AND @Today 
                         AND R.Status IN ('NEW', 'OPN', 'PAR'))
                    ELSE 0
                END as REQ_WORKED_7D,
                
                CASE 
                    WHEN AU.Role = 2 THEN 
                        (SELECT COUNT(DISTINCT S.RequisitionId) FROM Submissions S 
                         LEFT JOIN Requisitions R ON S.RequisitionId = R.Id
                         WHERE S.CreatedBy = AU.CreatedBy 
                         AND CAST(S.CreatedDate AS DATE) BETWEEN @StartDateMTD AND @Today 
                         AND R.Status IN ('NEW', 'OPN', 'PAR'))
                    WHEN AU.Role = 4 THEN 
                        (SELECT COUNT(DISTINCT S.RequisitionId) FROM Submissions S 
                         LEFT JOIN Requisitions R ON S.RequisitionId = R.Id
                         WHERE S.CreatedBy = AU.CreatedBy 
                         AND CAST(S.CreatedDate AS DATE) BETWEEN @StartDateMTD AND @Today 
                         AND R.Status IN ('NEW', 'OPN', 'PAR'))
                    WHEN AU.Role = 5 THEN 
                        (SELECT COUNT(DISTINCT S.RequisitionId) FROM Submissions S 
                         LEFT JOIN Requisitions R ON S.RequisitionId = R.Id
                         WHERE R.CreatedBy = AU.CreatedBy 
                         AND CAST(S.CreatedDate AS DATE) BETWEEN @StartDateMTD AND @Today 
                         AND R.Status IN ('NEW', 'OPN', 'PAR'))
                    WHEN AU.Role = 6 THEN 
                        (SELECT COUNT(DISTINCT S.RequisitionId) FROM Submissions S 
                         LEFT JOIN Requisitions R ON S.RequisitionId = R.Id
                         WHERE (S.CreatedBy = AU.CreatedBy OR R.CreatedBy = AU.CreatedBy)
                         AND CAST(S.CreatedDate AS DATE) BETWEEN @StartDateMTD AND @Today 
                         AND R.Status IN ('NEW', 'OPN', 'PAR'))
                    ELSE 0
                END as REQ_WORKED_MTD,
                
                CASE 
                    WHEN AU.Role = 2 THEN 
                        (SELECT COUNT(DISTINCT S.RequisitionId) FROM Submissions S 
                         LEFT JOIN Requisitions R ON S.RequisitionId = R.Id
                         WHERE S.CreatedBy = AU.CreatedBy 
                         AND CAST(S.CreatedDate AS DATE) BETWEEN @StartDateQTD AND @Today 
                         AND R.Status IN ('NEW', 'OPN', 'PAR'))
                    WHEN AU.Role = 4 THEN 
                        (SELECT COUNT(DISTINCT S.RequisitionId) FROM Submissions S 
                         LEFT JOIN Requisitions R ON S.RequisitionId = R.Id
                         WHERE S.CreatedBy = AU.CreatedBy 
                         AND CAST(S.CreatedDate AS DATE) BETWEEN @StartDateQTD AND @Today 
                         AND R.Status IN ('NEW', 'OPN', 'PAR'))
                    WHEN AU.Role = 5 THEN 
                        (SELECT COUNT(DISTINCT S.RequisitionId) FROM Submissions S 
                         LEFT JOIN Requisitions R ON S.RequisitionId = R.Id
                         WHERE R.CreatedBy = AU.CreatedBy 
                         AND CAST(S.CreatedDate AS DATE) BETWEEN @StartDateQTD AND @Today 
                         AND R.Status IN ('NEW', 'OPN', 'PAR'))
                    WHEN AU.Role = 6 THEN 
                        (SELECT COUNT(DISTINCT S.RequisitionId) FROM Submissions S 
                         LEFT JOIN Requisitions R ON S.RequisitionId = R.Id
                         WHERE (S.CreatedBy = AU.CreatedBy OR R.CreatedBy = AU.CreatedBy)
                         AND CAST(S.CreatedDate AS DATE) BETWEEN @StartDateQTD AND @Today 
                         AND R.Status IN ('NEW', 'OPN', 'PAR'))
                    ELSE 0
                END as REQ_WORKED_QTD,
                
                CASE 
                    WHEN AU.Role = 2 THEN 
                        (SELECT COUNT(DISTINCT S.RequisitionId) FROM Submissions S 
                         LEFT JOIN Requisitions R ON S.RequisitionId = R.Id
                         WHERE S.CreatedBy = AU.CreatedBy 
                         AND CAST(S.CreatedDate AS DATE) BETWEEN @StartDateHYTD AND @Today 
                         AND R.Status IN ('NEW', 'OPN', 'PAR'))
                    WHEN AU.Role = 4 THEN 
                        (SELECT COUNT(DISTINCT S.RequisitionId) FROM Submissions S 
                         LEFT JOIN Requisitions R ON S.RequisitionId = R.Id
                         WHERE S.CreatedBy = AU.CreatedBy 
                         AND CAST(S.CreatedDate AS DATE) BETWEEN @StartDateHYTD AND @Today 
                         AND R.Status IN ('NEW', 'OPN', 'PAR'))
                    WHEN AU.Role = 5 THEN 
                        (SELECT COUNT(DISTINCT S.RequisitionId) FROM Submissions S 
                         LEFT JOIN Requisitions R ON S.RequisitionId = R.Id
                         WHERE R.CreatedBy = AU.CreatedBy 
                         AND CAST(S.CreatedDate AS DATE) BETWEEN @StartDateHYTD AND @Today 
                         AND R.Status IN ('NEW', 'OPN', 'PAR'))
                    WHEN AU.Role = 6 THEN 
                        (SELECT COUNT(DISTINCT S.RequisitionId) FROM Submissions S 
                         LEFT JOIN Requisitions R ON S.RequisitionId = R.Id
                         WHERE (S.CreatedBy = AU.CreatedBy OR R.CreatedBy = AU.CreatedBy)
                         AND CAST(S.CreatedDate AS DATE) BETWEEN @StartDateHYTD AND @Today 
                         AND R.Status IN ('NEW', 'OPN', 'PAR'))
                    ELSE 0
                END as REQ_WORKED_HYTD,
                
                CASE 
                    WHEN AU.Role = 2 THEN 
                        (SELECT COUNT(DISTINCT S.RequisitionId) FROM Submissions S 
                         LEFT JOIN Requisitions R ON S.RequisitionId = R.Id
                         WHERE S.CreatedBy = AU.CreatedBy 
                         AND CAST(S.CreatedDate AS DATE) BETWEEN @YearStart AND @Today 
                         AND R.Status IN ('NEW', 'OPN', 'PAR'))
                    WHEN AU.Role = 4 THEN 
                        (SELECT COUNT(DISTINCT S.RequisitionId) FROM Submissions S 
                         LEFT JOIN Requisitions R ON S.RequisitionId = R.Id
                         WHERE S.CreatedBy = AU.CreatedBy 
                         AND CAST(S.CreatedDate AS DATE) BETWEEN @YearStart AND @Today 
                         AND R.Status IN ('NEW', 'OPN', 'PAR'))
                    WHEN AU.Role = 5 THEN 
                        (SELECT COUNT(DISTINCT S.RequisitionId) FROM Submissions S 
                         LEFT JOIN Requisitions R ON S.RequisitionId = R.Id
                         WHERE R.CreatedBy = AU.CreatedBy 
                         AND CAST(S.CreatedDate AS DATE) BETWEEN @YearStart AND @Today 
                         AND R.Status IN ('NEW', 'OPN', 'PAR'))
                    WHEN AU.Role = 6 THEN 
                        (SELECT COUNT(DISTINCT S.RequisitionId) FROM Submissions S 
                         LEFT JOIN Requisitions R ON S.RequisitionId = R.Id
                         WHERE (S.CreatedBy = AU.CreatedBy OR R.CreatedBy = AU.CreatedBy)
                         AND CAST(S.CreatedDate AS DATE) BETWEEN @YearStart AND @Today 
                         AND R.Status IN ('NEW', 'OPN', 'PAR'))
                    ELSE 0
                END as REQ_WORKED_YTD
                
            FROM AllActiveUsers AU
        ),
        
        -- ===== STATUS-BASED METRICS (Query 3-5: INT, OEX, HIR) =====
        -- Handle Full Desk users: combine AccountsManager + Recruiter perspectives without double-counting
        StatusMetrics AS (
            SELECT 
                StatusSub.UserName,
                -- Query 3: Interview Status (INT) - Latest status per candidate+requisition
                SUM(CASE WHEN CAST(StatusSub.LatestStatusDate AS DATE) BETWEEN @StartDate7D AND @Today AND StatusSub.Status = 'INT' THEN 1 ELSE 0 END) as INT_7D,
                SUM(CASE WHEN CAST(StatusSub.LatestStatusDate AS DATE) BETWEEN @StartDateMTD AND @Today AND StatusSub.Status = 'INT' THEN 1 ELSE 0 END) as INT_MTD,
                SUM(CASE WHEN CAST(StatusSub.LatestStatusDate AS DATE) BETWEEN @StartDateQTD AND @Today AND StatusSub.Status = 'INT' THEN 1 ELSE 0 END) as INT_QTD,
                SUM(CASE WHEN CAST(StatusSub.LatestStatusDate AS DATE) BETWEEN @StartDateHYTD AND @Today AND StatusSub.Status = 'INT' THEN 1 ELSE 0 END) as INT_HYTD,
                SUM(CASE WHEN CAST(StatusSub.LatestStatusDate AS DATE) BETWEEN @YearStart AND @Today AND StatusSub.Status = 'INT' THEN 1 ELSE 0 END) as INT_YTD,
                
                -- Query 4: Offer Extended (OEX)
                SUM(CASE WHEN CAST(StatusSub.LatestStatusDate AS DATE) BETWEEN @StartDate7D AND @Today AND StatusSub.Status = 'OEX' THEN 1 ELSE 0 END) as OEX_7D,
                SUM(CASE WHEN CAST(StatusSub.LatestStatusDate AS DATE) BETWEEN @StartDateMTD AND @Today AND StatusSub.Status = 'OEX' THEN 1 ELSE 0 END) as OEX_MTD,
                SUM(CASE WHEN CAST(StatusSub.LatestStatusDate AS DATE) BETWEEN @StartDateQTD AND @Today AND StatusSub.Status = 'OEX' THEN 1 ELSE 0 END) as OEX_QTD,
                SUM(CASE WHEN CAST(StatusSub.LatestStatusDate AS DATE) BETWEEN @StartDateHYTD AND @Today AND StatusSub.Status = 'OEX' THEN 1 ELSE 0 END) as OEX_HYTD,
                SUM(CASE WHEN CAST(StatusSub.LatestStatusDate AS DATE) BETWEEN @YearStart AND @Today AND StatusSub.Status = 'OEX' THEN 1 ELSE 0 END) as OEX_YTD,
                
                -- Query 5: Hired (HIR)
                SUM(CASE WHEN CAST(StatusSub.LatestStatusDate AS DATE) BETWEEN @StartDate7D AND @Today AND StatusSub.Status = 'HIR' THEN 1 ELSE 0 END) as HIR_7D,
                SUM(CASE WHEN CAST(StatusSub.LatestStatusDate AS DATE) BETWEEN @StartDateMTD AND @Today AND StatusSub.Status = 'HIR' THEN 1 ELSE 0 END) as HIR_MTD,
                SUM(CASE WHEN CAST(StatusSub.LatestStatusDate AS DATE) BETWEEN @StartDateQTD AND @Today AND StatusSub.Status = 'HIR' THEN 1 ELSE 0 END) as HIR_QTD,
                SUM(CASE WHEN CAST(StatusSub.LatestStatusDate AS DATE) BETWEEN @StartDateHYTD AND @Today AND StatusSub.Status = 'HIR' THEN 1 ELSE 0 END) as HIR_HYTD,
                SUM(CASE WHEN CAST(StatusSub.LatestStatusDate AS DATE) BETWEEN @YearStart AND @Today AND StatusSub.Status = 'HIR' THEN 1 ELSE 0 END) as HIR_YTD
            FROM (
                -- Get latest status for each candidate+requisition combination with user attribution
                -- Each RequisitionId+CandidateId+Status combo should be counted only once per user
                SELECT DISTINCT
                    S.RequisitionId,
                    S.CandidateId,
                    S.Status,
                    MAX(S.CreatedDate) as LatestStatusDate,
                    UserMapping.UserName
                FROM dbo.Submissions S
                INNER JOIN dbo.Requisitions R ON R.Id = S.RequisitionId
                INNER JOIN (
                    -- Map each submission to users who should get credit (avoid double-counting)
                    SELECT DISTINCT
                        S2.RequisitionId,
                        S2.CandidateId,
                        S2.Status,
                        CASE 
                            WHEN R2.CreatedBy = S2.CreatedBy THEN R2.CreatedBy  -- Full Desk: owns req AND submitted = count once
                            WHEN R2.CreatedBy <> S2.CreatedBy THEN R2.CreatedBy -- AccountsManager: owns req, someone else submitted
                            ELSE NULL
                        END as UserName
                    FROM dbo.Submissions S2
                    INNER JOIN dbo.Requisitions R2 ON R2.Id = S2.RequisitionId
                    WHERE S2.Status IN ('INT', 'OEX', 'HIR')
                        AND R2.CreatedBy IS NOT NULL
                    
                    UNION
                    
                    -- Recruiter perspective: submitted to someone else's req
                    SELECT DISTINCT
                        S3.RequisitionId,
                        S3.CandidateId,
                        S3.Status,
                        S3.CreatedBy as UserName
                    FROM dbo.Submissions S3
                    INNER JOIN dbo.Requisitions R3 ON R3.Id = S3.RequisitionId
                    WHERE S3.Status IN ('INT', 'OEX', 'HIR')
                        AND S3.CreatedBy <> R3.CreatedBy  -- Only when submitter != req owner
                        AND S3.CreatedBy IS NOT NULL
                ) UserMapping ON S.RequisitionId = UserMapping.RequisitionId 
                    AND S.CandidateId = UserMapping.CandidateId 
                    AND S.Status = UserMapping.Status
                WHERE S.Status IN ('INT', 'OEX', 'HIR')
                    AND UserMapping.UserName IS NOT NULL
                GROUP BY S.RequisitionId, S.CandidateId, S.Status, UserMapping.UserName
            ) StatusSub
            INNER JOIN AllActiveUsers AU ON StatusSub.UserName = AU.CreatedBy
            GROUP BY StatusSub.UserName
        )
        /*
        -- ===== FINAL INSERT: Combine all metrics by user and period =====
        INSERT INTO 
            [dbo].[SubmissionMetricsView] 
            ([CreatedBy], [MetricDate], [Period], [INT_Count], [OEX_Count], [HIR_Count], [Total_Submissions], [OEX_HIR_Ratio], [Requisitions_Created], [Active_Requisitions_Updated], [RefreshDate])
        
        -- Generate rows for each user and each time period
        SELECT 
            AU.CreatedBy,
            @Today as MetricDate,
            P.Period,
            
            -- Status counts by period
            CASE P.Period
                WHEN '7D' THEN ISNULL(SM.INT_7D, 0)
                WHEN 'MTD' THEN ISNULL(SM.INT_MTD, 0)
                WHEN 'QTD' THEN ISNULL(SM.INT_QTD, 0)
                WHEN 'HYTD' THEN ISNULL(SM.INT_HYTD, 0)
                WHEN 'YTD' THEN ISNULL(SM.INT_YTD, 0)
            END as INT_Count,
            
            CASE P.Period
                WHEN '7D' THEN ISNULL(SM.OEX_7D, 0)
                WHEN 'MTD' THEN ISNULL(SM.OEX_MTD, 0)
                WHEN 'QTD' THEN ISNULL(SM.OEX_QTD, 0)
                WHEN 'HYTD' THEN ISNULL(SM.OEX_HYTD, 0)
                WHEN 'YTD' THEN ISNULL(SM.OEX_YTD, 0)
            END as OEX_Count,
            
            CASE P.Period
                WHEN '7D' THEN ISNULL(SM.HIR_7D, 0)
                WHEN 'MTD' THEN ISNULL(SM.HIR_MTD, 0)
                WHEN 'QTD' THEN ISNULL(SM.HIR_QTD, 0)
                WHEN 'HYTD' THEN ISNULL(SM.HIR_HYTD, 0)
                WHEN 'YTD' THEN ISNULL(SM.HIR_YTD, 0)
            END as HIR_Count,
            
            -- Total submissions (for DashboardRecruiter)
            CASE P.Period
                WHEN '7D' THEN ISNULL(SC.SUB_TOTAL_7D, 0)
                WHEN 'MTD' THEN ISNULL(SC.SUB_TOTAL_MTD, 0)
                WHEN 'QTD' THEN ISNULL(SC.SUB_TOTAL_QTD, 0)
                WHEN 'HYTD' THEN ISNULL(SC.SUB_TOTAL_HYTD, 0)
                WHEN 'YTD' THEN ISNULL(SC.SUB_TOTAL_YTD, 0)
            END as Total_Submissions,
            
            -- Pre-calculate OEX/HIR ratio (Query 6)
            CASE 
                WHEN CASE P.Period
                    WHEN '7D' THEN ISNULL(SM.OEX_7D, 0)
                    WHEN 'MTD' THEN ISNULL(SM.OEX_MTD, 0)
                    WHEN 'QTD' THEN ISNULL(SM.OEX_QTD, 0)
                    WHEN 'HYTD' THEN ISNULL(SM.OEX_HYTD, 0)
                    WHEN 'YTD' THEN ISNULL(SM.OEX_YTD, 0)
                END = 0 THEN 0.00
                ELSE CAST(
                    CASE P.Period
                        WHEN '7D' THEN ISNULL(SM.HIR_7D, 0)
                        WHEN 'MTD' THEN ISNULL(SM.HIR_MTD, 0)
                        WHEN 'QTD' THEN ISNULL(SM.HIR_QTD, 0)
                        WHEN 'HYTD' THEN ISNULL(SM.HIR_HYTD, 0)
                        WHEN 'YTD' THEN ISNULL(SM.HIR_YTD, 0)
                    END AS DECIMAL(5,2)
                ) / CAST(
                    CASE P.Period
                        WHEN '7D' THEN ISNULL(SM.OEX_7D, 0)
                        WHEN 'MTD' THEN ISNULL(SM.OEX_MTD, 0)
                        WHEN 'QTD' THEN ISNULL(SM.OEX_QTD, 0)
                        WHEN 'HYTD' THEN ISNULL(SM.OEX_HYTD, 0)
                        WHEN 'YTD' THEN ISNULL(SM.OEX_YTD, 0)
                    END AS DECIMAL(5,2)
                )
            END as OEX_HIR_Ratio,
            
            -- Requisition metrics
            CASE P.Period
                WHEN '7D' THEN ISNULL(RM.REQ_CREATED_7D, 0)
                WHEN 'MTD' THEN ISNULL(RM.REQ_CREATED_MTD, 0)
                WHEN 'QTD' THEN ISNULL(RM.REQ_CREATED_QTD, 0)
                WHEN 'HYTD' THEN ISNULL(RM.REQ_CREATED_HYTD, 0)
                WHEN 'YTD' THEN ISNULL(RM.REQ_CREATED_YTD, 0)
            END as Requisitions_Created,
            
            CASE P.Period
                WHEN '7D' THEN ISNULL(RM.ACTIVE_REQ_7D, 0)
                WHEN 'MTD' THEN ISNULL(RM.ACTIVE_REQ_MTD, 0)
                WHEN 'QTD' THEN ISNULL(RM.ACTIVE_REQ_QTD, 0)
                WHEN 'HYTD' THEN ISNULL(RM.ACTIVE_REQ_HYTD, 0)
                WHEN 'YTD' THEN ISNULL(RM.ACTIVE_REQ_YTD, 0)
            END as Active_Requisitions_Updated,
            
            GETDATE() as RefreshDate
            
        FROM AllActiveUsers AU
        CROSS JOIN (VALUES ('7D'), ('MTD'), ('QTD'), ('HYTD'), ('YTD')) P(Period)
        LEFT JOIN RequisitionMetrics RM ON AU.CreatedBy = RM.CreatedBy
        LEFT JOIN SubmissionCounts SC ON AU.CreatedBy = SC.CreatedBy
        LEFT JOIN StatusMetrics SM ON AU.CreatedBy = SM.UserName;*/

-- Test just the CASE logic for KEVIN
SELECT 
    AU.CreatedBy,
    P.Period,
    SC.REQ_WORKED_7D, SC.REQ_WORKED_MTD, SC.REQ_WORKED_QTD, SC.REQ_WORKED_HYTD, SC.REQ_WORKED_YTD,
    CASE P.Period
        WHEN '7D' THEN ISNULL(SC.REQ_WORKED_7D, 0)
        WHEN 'MTD' THEN ISNULL(SC.REQ_WORKED_MTD, 0)
        WHEN 'QTD' THEN ISNULL(SC.REQ_WORKED_QTD, 0)
        WHEN 'HYTD' THEN ISNULL(SC.REQ_WORKED_HYTD, 0)
        WHEN 'YTD' THEN ISNULL(SC.REQ_WORKED_YTD, 0)
    END as Active_Requisitions_Updated
FROM AllActiveUsers AU
CROSS JOIN (VALUES ('7D'), ('MTD'), ('QTD'), ('HYTD'), ('YTD')) P(Period)
LEFT JOIN SubmissionCounts SC ON AU.CreatedBy = SC.CreatedBy
WHERE AU.CreatedBy = 'KEVIN'
ORDER BY P.Period;        
        -- Log success with performance metrics
        DECLARE @Duration_MS INT = DATEDIFF(MILLISECOND, @RefreshStart, SYSDATETIME());
        DECLARE @RowCount INT = @@ROWCOUNT;
        PRINT 'SubmissionMetricsView refreshed successfully - ' + CAST(@RowCount AS VARCHAR(10)) + ' rows in ' + CAST(@Duration_MS AS VARCHAR(10)) + 'ms';
        
    END TRY
    BEGIN CATCH
        -- Log error details with rollback
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
        DECLARE @ErrorState INT = ERROR_STATE();
        
        PRINT 'Error refreshing SubmissionMetricsView: ' + @ErrorMessage;
        
        -- Clean up any partial data
        DELETE FROM [dbo].[SubmissionMetricsView] WHERE CAST(RefreshDate AS DATE) = @Today;
        
        RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
    END CATCH
END;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [Dashboard_Refactor_SubmissionMetrics_AccountsManager]
AS
BEGIN
    SET NOCOUNT ON;
    SET ARITHABORT ON;
    
    DECLARE @RefreshStart DATETIME2 = SYSDATETIME();
    DECLARE @Today DATE = GETDATE();
    
    BEGIN TRY
        -- ========================================================================
        -- ACCOUNTS MANAGER LOGIC - COPY FROM DASHBOARDACCOUNTSMANAGER
        -- Key difference: Uses R.CreatedBy (requisition owner) vs S.CreatedBy (submitter)
        -- ========================================================================
        
        -- Date calculations (same as DashboardAccountsManager)
        DECLARE @YearStart DATE = DATEADD(YEAR, DATEDIFF(YEAR, 0, @Today), 0);
        DECLARE @StartDate7D DATE = DATEADD(DAY, -7, @Today);
        DECLARE @StartDateMTD DATE = DATEADD(MONTH, DATEDIFF(MONTH, 0, @Today), 0);
        DECLARE @StartDateQTD DATE = DATEADD(QUARTER, DATEDIFF(QUARTER, 0, @Today), 0);
        DECLARE @StartDateHYTD DATE = CASE WHEN MONTH(@Today) <= 6 THEN DATEFROMPARTS(YEAR(@Today), 1, 1) ELSE DATEFROMPARTS(YEAR(@Today), 7, 1) END;
        
        -- Single INSERT using JOINs for all accounts managers and all periods
        WITH AllAccountsManagers AS (
            SELECT 
                CreatedBy As UserName
            FROM 
                dbo.ActiveUsersView 
            WHERE 
                Role IN (5, 6) AND Status = 1  -- Role 5 = AM, Role 6 = Full Desk
        ),
        RequisitionMetrics AS (
            -- #RequisitionMetrics logic - requisitions they created
            SELECT 
                U.UserName as [User],
                -- Total requisitions created (by Updated date)
                ISNULL(SUM(CASE WHEN CAST(R.Updated AS DATE) BETWEEN @StartDate7D AND @Today THEN 1 ELSE 0 END), 0) as TOTAL_LAST7D_COUNT,
                ISNULL(SUM(CASE WHEN CAST(R.Updated AS DATE) BETWEEN @StartDateMTD AND @Today THEN 1 ELSE 0 END), 0) as TOTAL_MTD_COUNT,
                ISNULL(SUM(CASE WHEN CAST(R.Updated AS DATE) BETWEEN @StartDateQTD AND @Today THEN 1 ELSE 0 END), 0) as TOTAL_QTD_COUNT,
                ISNULL(SUM(CASE WHEN CAST(R.Updated AS DATE) BETWEEN @StartDateHYTD AND @Today THEN 1 ELSE 0 END), 0) as TOTAL_HYTD_COUNT,
                ISNULL(SUM(CASE WHEN CAST(R.Updated AS DATE) BETWEEN @YearStart AND @Today THEN 1 ELSE 0 END), 0) as TOTAL_YTD_COUNT,
                -- Active requisitions (OPN, NEW, PAR)
                ISNULL(SUM(CASE WHEN CAST(R.Updated AS DATE) BETWEEN @StartDate7D AND @Today AND R.Status IN ('OPN','NEW','PAR') THEN 1 ELSE 0 END), 0) as ACTIVE_LAST7D_COUNT,
                ISNULL(SUM(CASE WHEN CAST(R.Updated AS DATE) BETWEEN @StartDateMTD AND @Today AND R.Status IN ('OPN','NEW','PAR') THEN 1 ELSE 0 END), 0) as ACTIVE_MTD_COUNT,
                ISNULL(SUM(CASE WHEN CAST(R.Updated AS DATE) BETWEEN @StartDateQTD AND @Today AND R.Status IN ('OPN','NEW','PAR') THEN 1 ELSE 0 END), 0) as ACTIVE_QTD_COUNT,
                ISNULL(SUM(CASE WHEN CAST(R.Updated AS DATE) BETWEEN @StartDateHYTD AND @Today AND R.Status IN ('OPN','NEW','PAR') THEN 1 ELSE 0 END), 0) as ACTIVE_HYTD_COUNT,
                ISNULL(SUM(CASE WHEN CAST(R.Updated AS DATE) BETWEEN @YearStart AND @Today AND R.Status IN ('OPN','NEW','PAR') THEN 1 ELSE 0 END), 0) as ACTIVE_YTD_COUNT
            FROM 
                AllAccountsManagers U LEFT JOIN RequisitionView R ON R.CreatedBy = U.UserName
            GROUP BY 
                U.UserName
        ),
        SubmissionMetrics AS (
            -- #SubmissionMetrics logic - submissions TO their requisitions
            SELECT 
                U.UserName as [User],
                -- Interview status counts (submissions to their requisitions)
                ISNULL(COUNT(DISTINCT CASE WHEN CAST(SS.LatestStatusDate AS DATE) BETWEEN @StartDate7D AND @Today AND SS.Status = 'INT' THEN CAST(SS.RequisitionId AS VARCHAR(10)) + '-' + CAST(SS.CandidateId AS VARCHAR(10)) END), 0) as INT_LAST7D_COUNT,
                ISNULL(COUNT(DISTINCT CASE WHEN CAST(SS.LatestStatusDate AS DATE) BETWEEN @StartDateMTD AND @Today AND SS.Status = 'INT' THEN CAST(SS.RequisitionId AS VARCHAR(10)) + '-' + CAST(SS.CandidateId AS VARCHAR(10)) END), 0) as INT_MTD_COUNT,
                ISNULL(COUNT(DISTINCT CASE WHEN CAST(SS.LatestStatusDate AS DATE) BETWEEN @StartDateQTD AND @Today AND SS.Status = 'INT' THEN CAST(SS.RequisitionId AS VARCHAR(10)) + '-' + CAST(SS.CandidateId AS VARCHAR(10)) END), 0) as INT_QTD_COUNT,
                ISNULL(COUNT(DISTINCT CASE WHEN CAST(SS.LatestStatusDate AS DATE) BETWEEN @StartDateHYTD AND @Today AND SS.Status = 'INT' THEN CAST(SS.RequisitionId AS VARCHAR(10)) + '-' + CAST(SS.CandidateId AS VARCHAR(10)) END), 0) as INT_HYTD_COUNT,
                ISNULL(COUNT(DISTINCT CASE WHEN CAST(SS.LatestStatusDate AS DATE) BETWEEN @YearStart AND @Today AND SS.Status = 'INT' THEN CAST(SS.RequisitionId AS VARCHAR(10)) + '-' + CAST(SS.CandidateId AS VARCHAR(10)) END), 0) as INT_YTD_COUNT,
                -- Offer extended counts
                ISNULL(COUNT(DISTINCT CASE WHEN CAST(SS.LatestStatusDate AS DATE) BETWEEN @StartDate7D AND @Today AND SS.Status = 'OEX' THEN CAST(SS.RequisitionId AS VARCHAR(10)) + '-' + CAST(SS.CandidateId AS VARCHAR(10)) END), 0) as OEX_LAST7D_COUNT,
                ISNULL(COUNT(DISTINCT CASE WHEN CAST(SS.LatestStatusDate AS DATE) BETWEEN @StartDateMTD AND @Today AND SS.Status = 'OEX' THEN CAST(SS.RequisitionId AS VARCHAR(10)) + '-' + CAST(SS.CandidateId AS VARCHAR(10)) END), 0) as OEX_MTD_COUNT,
                ISNULL(COUNT(DISTINCT CASE WHEN CAST(SS.LatestStatusDate AS DATE) BETWEEN @StartDateQTD AND @Today AND SS.Status = 'OEX' THEN CAST(SS.RequisitionId AS VARCHAR(10)) + '-' + CAST(SS.CandidateId AS VARCHAR(10)) END), 0) as OEX_QTD_COUNT,
                ISNULL(COUNT(DISTINCT CASE WHEN CAST(SS.LatestStatusDate AS DATE) BETWEEN @StartDateHYTD AND @Today AND SS.Status = 'OEX' THEN CAST(SS.RequisitionId AS VARCHAR(10)) + '-' + CAST(SS.CandidateId AS VARCHAR(10)) END), 0) as OEX_HYTD_COUNT,
                ISNULL(COUNT(DISTINCT CASE WHEN CAST(SS.LatestStatusDate AS DATE) BETWEEN @YearStart AND @Today AND SS.Status = 'OEX' THEN CAST(SS.RequisitionId AS VARCHAR(10)) + '-' + CAST(SS.CandidateId AS VARCHAR(10)) END), 0) as OEX_YTD_COUNT,
                -- Hired counts
                ISNULL(COUNT(DISTINCT CASE WHEN CAST(SS.LatestStatusDate AS DATE) BETWEEN @StartDate7D AND @Today AND SS.Status = 'HIR' THEN CAST(SS.RequisitionId AS VARCHAR(10)) + '-' + CAST(SS.CandidateId AS VARCHAR(10)) END), 0) as HIR_LAST7D_COUNT,
                ISNULL(COUNT(DISTINCT CASE WHEN CAST(SS.LatestStatusDate AS DATE) BETWEEN @StartDateMTD AND @Today AND SS.Status = 'HIR' THEN CAST(SS.RequisitionId AS VARCHAR(10)) + '-' + CAST(SS.CandidateId AS VARCHAR(10)) END), 0) as HIR_MTD_COUNT,
                ISNULL(COUNT(DISTINCT CASE WHEN CAST(SS.LatestStatusDate AS DATE) BETWEEN @StartDateQTD AND @Today AND SS.Status = 'HIR' THEN CAST(SS.RequisitionId AS VARCHAR(10)) + '-' + CAST(SS.CandidateId AS VARCHAR(10)) END), 0) as HIR_QTD_COUNT,
                ISNULL(COUNT(DISTINCT CASE WHEN CAST(SS.LatestStatusDate AS DATE) BETWEEN @StartDateHYTD AND @Today AND SS.Status = 'HIR' THEN CAST(SS.RequisitionId AS VARCHAR(10)) + '-' + CAST(SS.CandidateId AS VARCHAR(10)) END), 0) as HIR_HYTD_COUNT,
                ISNULL(COUNT(DISTINCT CASE WHEN CAST(SS.LatestStatusDate AS DATE) BETWEEN @YearStart AND @Today AND SS.Status = 'HIR' THEN CAST(SS.RequisitionId AS VARCHAR(10)) + '-' + CAST(SS.CandidateId AS VARCHAR(10)) END), 0) as HIR_YTD_COUNT
            FROM AllAccountsManagers U
            LEFT JOIN (
                -- StatusSubmissions CTE logic
                SELECT 
                    S.RequisitionId, S.CandidateId, S.Status, MAX(S.CreatedDate) as LatestStatusDate, R.CreatedBy as RequisitionOwner
                FROM 
                    Submissions S INNER JOIN Requisitions R ON R.Id = S.RequisitionId
                WHERE 
                    S.Status IN ('INT', 'OEX', 'HIR')
                GROUP BY 
                    S.RequisitionId, S.CandidateId, S.Status, R.CreatedBy
            ) SS ON SS.RequisitionOwner = U.UserName
            GROUP BY 
                U.UserName
        )
        
        -- Generate all period rows for all accounts managers
        INSERT INTO 
            [dbo].[SubmissionMetricsView] 
            ([CreatedBy], [MetricDate], [Period], [INT_Count], [OEX_Count], [HIR_Count], [Total_Submissions], [OEX_HIR_Ratio], [Requisitions_Created], 
            [Active_Requisitions_Updated], [RefreshDate])
        SELECT 
            RM.[User] as CreatedBy, @Today as MetricDate, P.Period,
            CASE P.Period
                WHEN '7D' THEN SM.INT_LAST7D_COUNT
                WHEN 'MTD' THEN SM.INT_MTD_COUNT
                WHEN 'QTD' THEN SM.INT_QTD_COUNT
                WHEN 'HYTD' THEN SM.INT_HYTD_COUNT
                WHEN 'YTD' THEN SM.INT_YTD_COUNT
            END as INT_Count,
            CASE P.Period
                WHEN '7D' THEN SM.OEX_LAST7D_COUNT
                WHEN 'MTD' THEN SM.OEX_MTD_COUNT
                WHEN 'QTD' THEN SM.OEX_QTD_COUNT
                WHEN 'HYTD' THEN SM.OEX_HYTD_COUNT
                WHEN 'YTD' THEN SM.OEX_YTD_COUNT
            END as OEX_Count,
            CASE P.Period
                WHEN '7D' THEN SM.HIR_LAST7D_COUNT
                WHEN 'MTD' THEN SM.HIR_MTD_COUNT
                WHEN 'QTD' THEN SM.HIR_QTD_COUNT
                WHEN 'HYTD' THEN SM.HIR_HYTD_COUNT
                WHEN 'YTD' THEN SM.HIR_YTD_COUNT
            END as HIR_Count,
            0 as Total_Submissions,
            CASE 
                WHEN CASE P.Period
                    WHEN '7D' THEN SM.OEX_LAST7D_COUNT
                    WHEN 'MTD' THEN SM.OEX_MTD_COUNT
                    WHEN 'QTD' THEN SM.OEX_QTD_COUNT
                    WHEN 'HYTD' THEN SM.OEX_HYTD_COUNT
                    WHEN 'YTD' THEN SM.OEX_YTD_COUNT
                END = 0 THEN 0.00
                ELSE CAST(
                    CASE P.Period
                        WHEN '7D' THEN SM.HIR_LAST7D_COUNT
                        WHEN 'MTD' THEN SM.HIR_MTD_COUNT
                        WHEN 'QTD' THEN SM.HIR_QTD_COUNT
                        WHEN 'HYTD' THEN SM.HIR_HYTD_COUNT
                        WHEN 'YTD' THEN SM.HIR_YTD_COUNT
                    END AS DECIMAL(5,2)
                ) / CAST(
                    CASE P.Period
                        WHEN '7D' THEN SM.OEX_LAST7D_COUNT
                        WHEN 'MTD' THEN SM.OEX_MTD_COUNT
                        WHEN 'QTD' THEN SM.OEX_QTD_COUNT
                        WHEN 'HYTD' THEN SM.OEX_HYTD_COUNT
                        WHEN 'YTD' THEN SM.OEX_YTD_COUNT
                    END AS DECIMAL(5,2)
                )
            END as OEX_HIR_Ratio,
            CASE P.Period
                WHEN '7D' THEN RM.TOTAL_LAST7D_COUNT
                WHEN 'MTD' THEN RM.TOTAL_MTD_COUNT
                WHEN 'QTD' THEN RM.TOTAL_QTD_COUNT
                WHEN 'HYTD' THEN RM.TOTAL_HYTD_COUNT
                WHEN 'YTD' THEN RM.TOTAL_YTD_COUNT
            END as Requisitions_Created,
            CASE P.Period
                WHEN '7D' THEN RM.ACTIVE_LAST7D_COUNT
                WHEN 'MTD' THEN RM.ACTIVE_MTD_COUNT
                WHEN 'QTD' THEN RM.ACTIVE_QTD_COUNT
                WHEN 'HYTD' THEN RM.ACTIVE_HYTD_COUNT
                WHEN 'YTD' THEN RM.ACTIVE_YTD_COUNT
            END as Active_Requisitions_Updated, GETDATE() as RefreshDate
        FROM 
            RequisitionMetrics RM CROSS JOIN (VALUES ('7D'), ('MTD'), ('QTD'), ('HYTD'), ('YTD')) P(Period)
            LEFT JOIN SubmissionMetrics SM ON RM.[User] = SM.[User];
        
        -- Log success
        DECLARE @Duration_MS INT = DATEDIFF(MILLISECOND, @RefreshStart, SYSDATETIME());
        DECLARE @RowCount INT = @@ROWCOUNT;
        PRINT 'AccountsManager SubmissionMetricsView refreshed successfully - ' + CAST(@RowCount AS VARCHAR(10)) + ' rows in ' + CAST(@Duration_MS AS VARCHAR(10)) + 'ms';
        
    END TRY
    BEGIN CATCH
        -- Log error details
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
        DECLARE @ErrorState INT = ERROR_STATE();
        
        PRINT 'Error refreshing AccountsManager SubmissionMetricsView: ' + @ErrorMessage;
        RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
    END CATCH
END;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [Dashboard_Refactor_SubmissionMetrics_Admin]
AS
BEGIN
    SET NOCOUNT ON;
    SET ARITHABORT ON;
    
    DECLARE @RefreshStart DATETIME2 = SYSDATETIME();
    DECLARE @Today DATE = GETDATE();
    
    BEGIN TRY
        -- ========================================================================
        -- ADMIN LOGIC - AGGREGATE ALL USERS DATA FOR ADMIN PERSPECTIVE
        -- Key difference: Returns data by user (for individual user analysis) 
        -- but aggregates across all active users
        -- ========================================================================
        
        -- Date calculations (same as DashboardAdmin)
        DECLARE @YearStart DATE = DATEFROMPARTS(YEAR(@Today), 1, 1);
        DECLARE @StartDate7D DATE = DATEADD(DAY, -6, @Today);
        DECLARE @StartDateMTD DATE = DATEADD(DAY, 1, EOMONTH(@Today, -1));
        DECLARE @StartDateQTD DATE = DATEADD(QUARTER, DATEDIFF(QUARTER, 0, @Today), 0);
        DECLARE @StartDateHYTD DATE = CASE 
            WHEN MONTH(@Today) <= 6 THEN DATEFROMPARTS(YEAR(@Today), 1, 1)
            ELSE DATEFROMPARTS(YEAR(@Today), 7, 1)
        END;
        
        -- Single INSERT using JOINs for all users (admin perspective)
        WITH AllUsers AS (
            -- Same logic as DashboardAdmin - all users who created requisitions in last year
            SELECT DISTINCT R.CreatedBy
            FROM dbo.Requisitions R 
            INNER JOIN dbo.Users U ON R.CreatedBy = U.UserName AND U.Status = 1
            WHERE R.CreatedBy IS NOT NULL AND R.CreatedDate >= DATEADD(YEAR, -1, GETDATE())
        ),
        RequisitionMetrics AS (
            -- Query 1: Requisitions Created (by all users)
            SELECT 
                U.CreatedBy as [User],
                -- Total requisitions created
                SUM(CASE WHEN CAST(R.CreatedDate AS DATE) BETWEEN @StartDate7D AND @Today THEN 1 ELSE 0 END) as REQ_CREATED_LAST7D_COUNT,
                SUM(CASE WHEN CAST(R.CreatedDate AS DATE) BETWEEN @StartDateMTD AND @Today THEN 1 ELSE 0 END) as REQ_CREATED_MTD_COUNT,
                SUM(CASE WHEN CAST(R.CreatedDate AS DATE) BETWEEN @StartDateQTD AND @Today THEN 1 ELSE 0 END) as REQ_CREATED_QTD_COUNT,
                SUM(CASE WHEN CAST(R.CreatedDate AS DATE) BETWEEN @StartDateHYTD AND @Today THEN 1 ELSE 0 END) as REQ_CREATED_HYTD_COUNT,
                SUM(CASE WHEN CAST(R.CreatedDate AS DATE) BETWEEN @YearStart AND @Today THEN 1 ELSE 0 END) as REQ_CREATED_YTD_COUNT
            FROM AllUsers U
            LEFT JOIN dbo.Requisitions R ON R.CreatedBy = U.CreatedBy
            GROUP BY U.CreatedBy
        ),
        ActiveRequisitionMetrics AS (
            -- Query 2: Active Requisitions Updated
            SELECT 
                U.CreatedBy as [User],
                -- Active requisitions (OPN, NEW, PAR)
                SUM(CASE WHEN CAST(R.UpdatedDate AS DATE) BETWEEN @StartDate7D AND @Today AND R.Status IN ('NEW', 'OPN', 'PAR') THEN 1 ELSE 0 END) as ACTIVE_LAST7D_COUNT,
                SUM(CASE WHEN CAST(R.UpdatedDate AS DATE) BETWEEN @StartDateMTD AND @Today AND R.Status IN ('NEW', 'OPN', 'PAR') THEN 1 ELSE 0 END) as ACTIVE_MTD_COUNT,
                SUM(CASE WHEN CAST(R.UpdatedDate AS DATE) BETWEEN @StartDateQTD AND @Today AND R.Status IN ('NEW', 'OPN', 'PAR') THEN 1 ELSE 0 END) as ACTIVE_QTD_COUNT,
                SUM(CASE WHEN CAST(R.UpdatedDate AS DATE) BETWEEN @StartDateHYTD AND @Today AND R.Status IN ('NEW', 'OPN', 'PAR') THEN 1 ELSE 0 END) as ACTIVE_HYTD_COUNT,
                SUM(CASE WHEN CAST(R.UpdatedDate AS DATE) BETWEEN @YearStart AND @Today AND R.Status IN ('NEW', 'OPN', 'PAR') THEN 1 ELSE 0 END) as ACTIVE_YTD_COUNT
            FROM AllUsers U
            LEFT JOIN dbo.Requisitions R ON R.CreatedBy = U.CreatedBy
            GROUP BY U.CreatedBy
        ),
        SubmissionMetrics AS (
            -- Queries 3-5: Status metrics using #SubmissionData logic
            SELECT 
                U.CreatedBy as [User],
                -- Interview status counts (using MaxCreatedDate from temp table logic)
                COUNT(DISTINCT CASE WHEN CAST(SD.MaxCreatedDate AS DATE) BETWEEN @StartDate7D AND @Today AND SD.Status = 'INT' THEN CAST(SD.RequisitionId AS VARCHAR(10)) + '-' + CAST(SD.CandidateId AS VARCHAR(10)) END) as INT_LAST7D_COUNT,
                COUNT(DISTINCT CASE WHEN CAST(SD.MaxCreatedDate AS DATE) BETWEEN @StartDateMTD AND @Today AND SD.Status = 'INT' THEN CAST(SD.RequisitionId AS VARCHAR(10)) + '-' + CAST(SD.CandidateId AS VARCHAR(10)) END) as INT_MTD_COUNT,
                COUNT(DISTINCT CASE WHEN CAST(SD.MaxCreatedDate AS DATE) BETWEEN @StartDateQTD AND @Today AND SD.Status = 'INT' THEN CAST(SD.RequisitionId AS VARCHAR(10)) + '-' + CAST(SD.CandidateId AS VARCHAR(10)) END) as INT_QTD_COUNT,
                COUNT(DISTINCT CASE WHEN CAST(SD.MaxCreatedDate AS DATE) BETWEEN @StartDateHYTD AND @Today AND SD.Status = 'INT' THEN CAST(SD.RequisitionId AS VARCHAR(10)) + '-' + CAST(SD.CandidateId AS VARCHAR(10)) END) as INT_HYTD_COUNT,
                COUNT(DISTINCT CASE WHEN CAST(SD.MaxCreatedDate AS DATE) BETWEEN @YearStart AND @Today AND SD.Status = 'INT' THEN CAST(SD.RequisitionId AS VARCHAR(10)) + '-' + CAST(SD.CandidateId AS VARCHAR(10)) END) as INT_YTD_COUNT,
                -- Offer extended counts
                COUNT(DISTINCT CASE WHEN CAST(SD.MaxCreatedDate AS DATE) BETWEEN @StartDate7D AND @Today AND SD.Status = 'OEX' THEN CAST(SD.RequisitionId AS VARCHAR(10)) + '-' + CAST(SD.CandidateId AS VARCHAR(10)) END) as OEX_LAST7D_COUNT,
                COUNT(DISTINCT CASE WHEN CAST(SD.MaxCreatedDate AS DATE) BETWEEN @StartDateMTD AND @Today AND SD.Status = 'OEX' THEN CAST(SD.RequisitionId AS VARCHAR(10)) + '-' + CAST(SD.CandidateId AS VARCHAR(10)) END) as OEX_MTD_COUNT,
                COUNT(DISTINCT CASE WHEN CAST(SD.MaxCreatedDate AS DATE) BETWEEN @StartDateQTD AND @Today AND SD.Status = 'OEX' THEN CAST(SD.RequisitionId AS VARCHAR(10)) + '-' + CAST(SD.CandidateId AS VARCHAR(10)) END) as OEX_QTD_COUNT,
                COUNT(DISTINCT CASE WHEN CAST(SD.MaxCreatedDate AS DATE) BETWEEN @StartDateHYTD AND @Today AND SD.Status = 'OEX' THEN CAST(SD.RequisitionId AS VARCHAR(10)) + '-' + CAST(SD.CandidateId AS VARCHAR(10)) END) as OEX_HYTD_COUNT,
                COUNT(DISTINCT CASE WHEN CAST(SD.MaxCreatedDate AS DATE) BETWEEN @YearStart AND @Today AND SD.Status = 'OEX' THEN CAST(SD.RequisitionId AS VARCHAR(10)) + '-' + CAST(SD.CandidateId AS VARCHAR(10)) END) as OEX_YTD_COUNT,
                -- Hired counts
                COUNT(DISTINCT CASE WHEN CAST(SD.MaxCreatedDate AS DATE) BETWEEN @StartDate7D AND @Today AND SD.Status = 'HIR' THEN CAST(SD.RequisitionId AS VARCHAR(10)) + '-' + CAST(SD.CandidateId AS VARCHAR(10)) END) as HIR_LAST7D_COUNT,
                COUNT(DISTINCT CASE WHEN CAST(SD.MaxCreatedDate AS DATE) BETWEEN @StartDateMTD AND @Today AND SD.Status = 'HIR' THEN CAST(SD.RequisitionId AS VARCHAR(10)) + '-' + CAST(SD.CandidateId AS VARCHAR(10)) END) as HIR_MTD_COUNT,
                COUNT(DISTINCT CASE WHEN CAST(SD.MaxCreatedDate AS DATE) BETWEEN @StartDateQTD AND @Today AND SD.Status = 'HIR' THEN CAST(SD.RequisitionId AS VARCHAR(10)) + '-' + CAST(SD.CandidateId AS VARCHAR(10)) END) as HIR_QTD_COUNT,
                COUNT(DISTINCT CASE WHEN CAST(SD.MaxCreatedDate AS DATE) BETWEEN @StartDateHYTD AND @Today AND SD.Status = 'HIR' THEN CAST(SD.RequisitionId AS VARCHAR(10)) + '-' + CAST(SD.CandidateId AS VARCHAR(10)) END) as HIR_HYTD_COUNT,
                COUNT(DISTINCT CASE WHEN CAST(SD.MaxCreatedDate AS DATE) BETWEEN @YearStart AND @Today AND SD.Status = 'HIR' THEN CAST(SD.RequisitionId AS VARCHAR(10)) + '-' + CAST(SD.CandidateId AS VARCHAR(10)) END) as HIR_YTD_COUNT
            FROM AllUsers U
            LEFT JOIN (
                -- Replicate #SubmissionData temp table logic
                SELECT 
                    S.CandidateId,
                    S.RequisitionId,
                    S.Status,
                    MAX(S.CreatedDate) as MaxCreatedDate,
                    R.CreatedBy
                FROM dbo.Submissions S 
                INNER JOIN dbo.Requisitions R ON S.RequisitionId = R.ID
                WHERE S.Status IN ('INT', 'OEX', 'HIR')
                GROUP BY S.CandidateId, S.RequisitionId, S.Status, R.CreatedBy
                HAVING R.CreatedBy IS NOT NULL
            ) SD ON SD.CreatedBy = U.CreatedBy
            GROUP BY U.CreatedBy
        )
        
        -- Generate all period rows for all users (admin perspective)
        INSERT INTO [dbo].[SubmissionMetricsView] 
        ([CreatedBy], [MetricDate], [Period], [INT_Count], [OEX_Count], [HIR_Count], [Total_Submissions], [OEX_HIR_Ratio], [Requisitions_Created], [Active_Requisitions_Updated], [RefreshDate])
        
        SELECT 
            RM.[User] as CreatedBy,
            @Today as MetricDate,
            P.Period,
            
            -- Status counts by period
            CASE P.Period
                WHEN '7D' THEN ISNULL(SM.INT_LAST7D_COUNT, 0)
                WHEN 'MTD' THEN ISNULL(SM.INT_MTD_COUNT, 0)
                WHEN 'QTD' THEN ISNULL(SM.INT_QTD_COUNT, 0)
                WHEN 'HYTD' THEN ISNULL(SM.INT_HYTD_COUNT, 0)
                WHEN 'YTD' THEN ISNULL(SM.INT_YTD_COUNT, 0)
            END as INT_Count,
            
            CASE P.Period
                WHEN '7D' THEN ISNULL(SM.OEX_LAST7D_COUNT, 0)
                WHEN 'MTD' THEN ISNULL(SM.OEX_MTD_COUNT, 0)
                WHEN 'QTD' THEN ISNULL(SM.OEX_QTD_COUNT, 0)
                WHEN 'HYTD' THEN ISNULL(SM.OEX_HYTD_COUNT, 0)
                WHEN 'YTD' THEN ISNULL(SM.OEX_YTD_COUNT, 0)
            END as OEX_Count,
            
            CASE P.Period
                WHEN '7D' THEN ISNULL(SM.HIR_LAST7D_COUNT, 0)
                WHEN 'MTD' THEN ISNULL(SM.HIR_MTD_COUNT, 0)
                WHEN 'QTD' THEN ISNULL(SM.HIR_QTD_COUNT, 0)
                WHEN 'HYTD' THEN ISNULL(SM.HIR_HYTD_COUNT, 0)
                WHEN 'YTD' THEN ISNULL(SM.HIR_YTD_COUNT, 0)
            END as HIR_Count,
            
            -- Total submissions (0 for admin - this represents per user data but aggregated from admin perspective)
            0 as Total_Submissions,
            
            -- OEX:HIR Ratio
            CASE 
                WHEN CASE P.Period
                    WHEN '7D' THEN ISNULL(SM.OEX_LAST7D_COUNT, 0)
                    WHEN 'MTD' THEN ISNULL(SM.OEX_MTD_COUNT, 0)
                    WHEN 'QTD' THEN ISNULL(SM.OEX_QTD_COUNT, 0)
                    WHEN 'HYTD' THEN ISNULL(SM.OEX_HYTD_COUNT, 0)
                    WHEN 'YTD' THEN ISNULL(SM.OEX_YTD_COUNT, 0)
                END = 0 THEN 0.00
                ELSE CAST(
                    CASE P.Period
                        WHEN '7D' THEN ISNULL(SM.HIR_LAST7D_COUNT, 0)
                        WHEN 'MTD' THEN ISNULL(SM.HIR_MTD_COUNT, 0)
                        WHEN 'QTD' THEN ISNULL(SM.HIR_QTD_COUNT, 0)
                        WHEN 'HYTD' THEN ISNULL(SM.HIR_HYTD_COUNT, 0)
                        WHEN 'YTD' THEN ISNULL(SM.HIR_YTD_COUNT, 0)
                    END AS DECIMAL(5,2)
                ) / CAST(
                    CASE P.Period
                        WHEN '7D' THEN ISNULL(SM.OEX_LAST7D_COUNT, 0)
                        WHEN 'MTD' THEN ISNULL(SM.OEX_MTD_COUNT, 0)
                        WHEN 'QTD' THEN ISNULL(SM.OEX_QTD_COUNT, 0)
                        WHEN 'HYTD' THEN ISNULL(SM.OEX_HYTD_COUNT, 0)
                        WHEN 'YTD' THEN ISNULL(SM.OEX_YTD_COUNT, 0)
                    END AS DECIMAL(5,2)
                )
            END as OEX_HIR_Ratio,
            
            -- Requisitions created by each user
            CASE P.Period
                WHEN '7D' THEN ISNULL(RM.REQ_CREATED_LAST7D_COUNT, 0)
                WHEN 'MTD' THEN ISNULL(RM.REQ_CREATED_MTD_COUNT, 0)
                WHEN 'QTD' THEN ISNULL(RM.REQ_CREATED_QTD_COUNT, 0)
                WHEN 'HYTD' THEN ISNULL(RM.REQ_CREATED_HYTD_COUNT, 0)
                WHEN 'YTD' THEN ISNULL(RM.REQ_CREATED_YTD_COUNT, 0)
            END as Requisitions_Created,
            
            -- Active requisitions updated by each user
            CASE P.Period
                WHEN '7D' THEN ISNULL(ARM.ACTIVE_LAST7D_COUNT, 0)
                WHEN 'MTD' THEN ISNULL(ARM.ACTIVE_MTD_COUNT, 0)
                WHEN 'QTD' THEN ISNULL(ARM.ACTIVE_QTD_COUNT, 0)
                WHEN 'HYTD' THEN ISNULL(ARM.ACTIVE_HYTD_COUNT, 0)
                WHEN 'YTD' THEN ISNULL(ARM.ACTIVE_YTD_COUNT, 0)
            END as Active_Requisitions_Updated,
            
            GETDATE() as RefreshDate
            
        FROM RequisitionMetrics RM
        CROSS JOIN (VALUES ('7D'), ('MTD'), ('QTD'), ('HYTD'), ('YTD')) P(Period)
        LEFT JOIN ActiveRequisitionMetrics ARM ON RM.[User] = ARM.[User]
        LEFT JOIN SubmissionMetrics SM ON RM.[User] = SM.[User];
        
        -- Log success
        DECLARE @Duration_MS INT = DATEDIFF(MILLISECOND, @RefreshStart, SYSDATETIME());
        DECLARE @RowCount INT = @@ROWCOUNT;
        PRINT 'Admin SubmissionMetricsView refreshed successfully - ' + CAST(@RowCount AS VARCHAR(10)) + ' rows in ' + CAST(@Duration_MS AS VARCHAR(10)) + 'ms';
        
    END TRY
    BEGIN CATCH
        -- Log error details
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
        DECLARE @ErrorState INT = ERROR_STATE();
        
        PRINT 'Error refreshing Admin SubmissionMetricsView: ' + @ErrorMessage;
        RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
    END CATCH
END;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [Dashboard_Refactor_SubmissionMetrics_Consolidated]
AS
BEGIN
    SET NOCOUNT ON;
    SET ARITHABORT ON;

    BEGIN TRANSACTION;

    BEGIN TRY

        DELETE FROM SubmissionMetricsView;

        EXEC [Dashboard_Refactor_SubmissionMetrics_Recruiter];
        EXEC [Dashboard_Refactor_SubmissionMetrics_AccountsManager];

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
    END CATCH
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [Dashboard_Refactor_SubmissionMetrics_Recruiter]
AS
BEGIN
    SET NOCOUNT ON;
    SET ARITHABORT ON;
    
    DECLARE @RefreshStart DATETIME2 = SYSDATETIME();
    DECLARE @Today DATE = '2025-06-30';--CAST(GETDATE() as date);
    
    BEGIN TRY
        -- Date calculations (same for all users)
        DECLARE @YearStart DATE = DATEFROMPARTS(YEAR(@Today), 1, 1);
        DECLARE @StartDate7D DATE = DATEADD(DAY, -6, @Today);
        DECLARE @StartDateMTD DATE = DATEADD(DAY, 1, EOMONTH(@Today, -1));
        DECLARE @StartDateQTD DATE = DATEADD(QUARTER, DATEDIFF(QUARTER, 0, @Today), 0);
        DECLARE @StartDateHYTD DATE = CASE 
            WHEN MONTH(@Today) <= 6 THEN DATEFROMPARTS(YEAR(@Today), 1, 1)
            ELSE DATEFROMPARTS(YEAR(@Today), 7, 1)
        END;
        
        -- Single INSERT using JOINs for all recruiters and all periods
        WITH AllRecruiters AS (
            SELECT CreatedBy as UserName, 
                   -- Adjust YearStart per user based on their earliest submission
                   CASE 
                       WHEN (SELECT MIN(CAST(CreatedDate AS DATE)) FROM Submissions WHERE CreatedBy = A.CreatedBy) IS NULL 
                            OR (SELECT MIN(CAST(CreatedDate AS DATE)) FROM Submissions WHERE CreatedBy = A.CreatedBy) > @YearStart 
                       THEN ISNULL((SELECT MIN(CAST(CreatedDate AS DATE)) FROM Submissions WHERE CreatedBy = A.CreatedBy), @YearStart)
                       ELSE @YearStart
                   END as UserYearStart
            FROM dbo.ActiveUsersView A
            WHERE Role = 4 AND Status = 1
        ),
        BaseMetrics AS (
            -- Replicate #BaseMetrics temp table logic for all users
            SELECT 
                U.UserName as [User], U.UserYearStart,
                -- Submission counts
                ISNULL(SUM(CASE WHEN CAST(S.CreatedDate AS DATE) BETWEEN @StartDate7D AND @Today THEN 1 ELSE 0 END), 0) as SUB_LAST7D_COUNT,
                ISNULL(SUM(CASE WHEN CAST(S.CreatedDate AS DATE) BETWEEN @StartDateMTD AND @Today THEN 1 ELSE 0 END), 0) as SUB_MTD_COUNT,
                ISNULL(SUM(CASE WHEN CAST(S.CreatedDate AS DATE) BETWEEN @StartDateQTD AND @Today THEN 1 ELSE 0 END), 0) as SUB_QTD_COUNT,
                ISNULL(SUM(CASE WHEN CAST(S.CreatedDate AS DATE) BETWEEN @StartDateHYTD AND @Today THEN 1 ELSE 0 END), 0) as SUB_HYTD_COUNT,
                ISNULL(SUM(CASE WHEN CAST(S.CreatedDate AS DATE) BETWEEN U.UserYearStart AND @Today THEN 1 ELSE 0 END), 0) as SUB_YTD_COUNT,
                -- Active requisition counts
                ISNULL(COUNT(DISTINCT CASE WHEN CAST(S.CreatedDate AS DATE) BETWEEN @StartDate7D AND @Today AND R.Status IN ('NEW', 'OPN', 'PAR') THEN S.RequisitionId END), 0) as REQ_LAST7D_COUNT,
                ISNULL(COUNT(DISTINCT CASE WHEN CAST(S.CreatedDate AS DATE) BETWEEN @StartDateMTD AND @Today AND R.Status IN ('NEW', 'OPN', 'PAR') THEN S.RequisitionId END), 0) as REQ_MTD_COUNT,
                ISNULL(COUNT(DISTINCT CASE WHEN CAST(S.CreatedDate AS DATE) BETWEEN @StartDateQTD AND @Today AND R.Status IN ('NEW', 'OPN', 'PAR') THEN S.RequisitionId END), 0) as REQ_QTD_COUNT,
                ISNULL(COUNT(DISTINCT CASE WHEN CAST(S.CreatedDate AS DATE) BETWEEN @StartDateHYTD AND @Today AND R.Status IN ('NEW', 'OPN', 'PAR') THEN S.RequisitionId END), 0) as REQ_HYTD_COUNT,
                ISNULL(COUNT(DISTINCT CASE WHEN CAST(S.CreatedDate AS DATE) BETWEEN U.UserYearStart AND @Today AND R.Status IN ('NEW', 'OPN', 'PAR') THEN S.RequisitionId END), 0) as REQ_YTD_COUNT
            FROM 
                AllRecruiters U LEFT JOIN Submissions S ON S.CreatedBy = U.UserName
                LEFT JOIN Requisitions R ON S.RequisitionId = R.Id
            GROUP BY 
                U.UserName, U.UserYearStart
        ),
        StatusMetrics AS (
            -- Replicate #StatusMetrics logic for all users
            SELECT 
                U.UserName as [User], U.UserYearStart,
                -- Interview status counts
                ISNULL(SUM(CASE WHEN CAST(RS.LatestStatusDate AS DATE) BETWEEN @StartDate7D AND @Today AND RS.Status = 'INT' THEN 1 ELSE 0 END), 0) as INT_LAST7D_COUNT,
                ISNULL(SUM(CASE WHEN CAST(RS.LatestStatusDate AS DATE) BETWEEN @StartDateMTD AND @Today AND RS.Status = 'INT' THEN 1 ELSE 0 END), 0) as INT_MTD_COUNT,
                ISNULL(SUM(CASE WHEN CAST(RS.LatestStatusDate AS DATE) BETWEEN @StartDateQTD AND @Today AND RS.Status = 'INT' THEN 1 ELSE 0 END), 0) as INT_QTD_COUNT,
                ISNULL(SUM(CASE WHEN CAST(RS.LatestStatusDate AS DATE) BETWEEN @StartDateHYTD AND @Today AND RS.Status = 'INT' THEN 1 ELSE 0 END), 0) as INT_HYTD_COUNT,
                ISNULL(SUM(CASE WHEN CAST(RS.LatestStatusDate AS DATE) BETWEEN U.UserYearStart AND @Today AND RS.Status = 'INT' THEN 1 ELSE 0 END), 0) as INT_YTD_COUNT,
                -- Offer extended counts
                ISNULL(SUM(CASE WHEN CAST(RS.LatestStatusDate AS DATE) BETWEEN @StartDate7D AND @Today AND RS.Status = 'OEX' THEN 1 ELSE 0 END), 0) as OEX_LAST7D_COUNT,
                ISNULL(SUM(CASE WHEN CAST(RS.LatestStatusDate AS DATE) BETWEEN @StartDateMTD AND @Today AND RS.Status = 'OEX' THEN 1 ELSE 0 END), 0) as OEX_MTD_COUNT,
                ISNULL(SUM(CASE WHEN CAST(RS.LatestStatusDate AS DATE) BETWEEN @StartDateQTD AND @Today AND RS.Status = 'OEX' THEN 1 ELSE 0 END), 0) as OEX_QTD_COUNT,
                ISNULL(SUM(CASE WHEN CAST(RS.LatestStatusDate AS DATE) BETWEEN @StartDateHYTD AND @Today AND RS.Status = 'OEX' THEN 1 ELSE 0 END), 0) as OEX_HYTD_COUNT,
                ISNULL(SUM(CASE WHEN CAST(RS.LatestStatusDate AS DATE) BETWEEN U.UserYearStart AND @Today AND RS.Status = 'OEX' THEN 1 ELSE 0 END), 0) as OEX_YTD_COUNT,
                -- Hired counts
                ISNULL(SUM(CASE WHEN CAST(RS.LatestStatusDate AS DATE) BETWEEN @StartDate7D AND @Today AND RS.Status = 'HIR' THEN 1 ELSE 0 END), 0) as HIR_LAST7D_COUNT,
                ISNULL(SUM(CASE WHEN CAST(RS.LatestStatusDate AS DATE) BETWEEN @StartDateMTD AND @Today AND RS.Status = 'HIR' THEN 1 ELSE 0 END), 0) as HIR_MTD_COUNT,
                ISNULL(SUM(CASE WHEN CAST(RS.LatestStatusDate AS DATE) BETWEEN @StartDateQTD AND @Today AND RS.Status = 'HIR' THEN 1 ELSE 0 END), 0) as HIR_QTD_COUNT,
                ISNULL(SUM(CASE WHEN CAST(RS.LatestStatusDate AS DATE) BETWEEN @StartDateHYTD AND @Today AND RS.Status = 'HIR' THEN 1 ELSE 0 END), 0) as HIR_HYTD_COUNT,
                ISNULL(SUM(CASE WHEN CAST(RS.LatestStatusDate AS DATE) BETWEEN U.UserYearStart AND @Today AND RS.Status = 'HIR' THEN 1 ELSE 0 END), 0) as HIR_YTD_COUNT
            FROM AllRecruiters U
            LEFT JOIN (
                -- Replicate #RecruiterSubmissions logic for all users
                SELECT 
                    FS.RequisitionId, FS.CandidateId, FS.UserName, S.Status, MAX(S.CreatedDate) as LatestStatusDate
                FROM (
                    SELECT 
                        S.RequisitionId, S.CandidateId, U2.UserName, MIN(S.CreatedDate) as FirstSubmissionDate
                    FROM 
                        Submissions S CROSS JOIN AllRecruiters U2
                    WHERE 
                        S.CreatedBy = U2.UserName
                    GROUP BY 
                        S.RequisitionId, S.CandidateId, U2.UserName
                    HAVING 
                        MIN(CASE WHEN S.CreatedBy = U2.UserName THEN S.CreatedDate END) = MIN(S.CreatedDate)
                ) FS
                INNER JOIN Submissions S  ON FS.RequisitionId = S.RequisitionId AND FS.CandidateId = S.CandidateId AND S.Status IN ('INT', 'OEX', 'HIR')
                GROUP BY 
                    FS.RequisitionId, FS.CandidateId, FS.UserName, S.Status
            ) RS ON RS.UserName = U.UserName
            GROUP BY 
                U.UserName, U.UserYearStart
        )
        
        -- Generate all period rows for all users
         INSERT INTO 
            [dbo].[SubmissionMetricsView] 
            ([CreatedBy], [MetricDate], [Period], [INT_Count], [OEX_Count], [HIR_Count], [Total_Submissions], [OEX_HIR_Ratio], [Requisitions_Created], 
            [Active_Requisitions_Updated], [RefreshDate])
       SELECT 
            BM.[User] as CreatedBy, @Today as MetricDate, P.Period,
            CASE P.Period
                WHEN '7D' THEN SM.INT_LAST7D_COUNT
                WHEN 'MTD' THEN SM.INT_MTD_COUNT
                WHEN 'QTD' THEN SM.INT_QTD_COUNT
                WHEN 'HYTD' THEN SM.INT_HYTD_COUNT
                WHEN 'YTD' THEN SM.INT_YTD_COUNT
            END as INT_Count,
            CASE P.Period
                WHEN '7D' THEN SM.OEX_LAST7D_COUNT
                WHEN 'MTD' THEN SM.OEX_MTD_COUNT
                WHEN 'QTD' THEN SM.OEX_QTD_COUNT
                WHEN 'HYTD' THEN SM.OEX_HYTD_COUNT
                WHEN 'YTD' THEN SM.OEX_YTD_COUNT
            END as OEX_Count,
            CASE P.Period
                WHEN '7D' THEN SM.HIR_LAST7D_COUNT
                WHEN 'MTD' THEN SM.HIR_MTD_COUNT
                WHEN 'QTD' THEN SM.HIR_QTD_COUNT
                WHEN 'HYTD' THEN SM.HIR_HYTD_COUNT
                WHEN 'YTD' THEN SM.HIR_YTD_COUNT
            END as HIR_Count,
            CASE P.Period
                WHEN '7D' THEN BM.SUB_LAST7D_COUNT
                WHEN 'MTD' THEN BM.SUB_MTD_COUNT
                WHEN 'QTD' THEN BM.SUB_QTD_COUNT
                WHEN 'HYTD' THEN BM.SUB_HYTD_COUNT
                WHEN 'YTD' THEN BM.SUB_YTD_COUNT
            END as Total_Submissions,
            CASE 
                WHEN CASE P.Period
                    WHEN '7D' THEN SM.OEX_LAST7D_COUNT
                    WHEN 'MTD' THEN SM.OEX_MTD_COUNT
                    WHEN 'QTD' THEN SM.OEX_QTD_COUNT
                    WHEN 'HYTD' THEN SM.OEX_HYTD_COUNT
                    WHEN 'YTD' THEN SM.OEX_YTD_COUNT
                END = 0 THEN 0.00
                ELSE CAST(
                    CASE P.Period
                        WHEN '7D' THEN SM.HIR_LAST7D_COUNT
                        WHEN 'MTD' THEN SM.HIR_MTD_COUNT
                        WHEN 'QTD' THEN SM.HIR_QTD_COUNT
                        WHEN 'HYTD' THEN SM.HIR_HYTD_COUNT
                        WHEN 'YTD' THEN SM.HIR_YTD_COUNT
                    END AS DECIMAL(5,2)
                ) / CAST(
                    CASE P.Period
                        WHEN '7D' THEN SM.OEX_LAST7D_COUNT
                        WHEN 'MTD' THEN SM.OEX_MTD_COUNT
                        WHEN 'QTD' THEN SM.OEX_QTD_COUNT
                        WHEN 'HYTD' THEN SM.OEX_HYTD_COUNT
                        WHEN 'YTD' THEN SM.OEX_YTD_COUNT
                    END AS DECIMAL(5,2)
                )
            END as OEX_HIR_Ratio,
            0 as Requisitions_Created,
            CASE P.Period
                WHEN '7D' THEN BM.REQ_LAST7D_COUNT
                WHEN 'MTD' THEN BM.REQ_MTD_COUNT
                WHEN 'QTD' THEN BM.REQ_QTD_COUNT
                WHEN 'HYTD' THEN BM.REQ_HYTD_COUNT
                WHEN 'YTD' THEN BM.REQ_YTD_COUNT
            END as Active_Requisitions_Updated,
            GETDATE() as RefreshDate
        FROM 
            BaseMetrics BM CROSS JOIN (VALUES ('7D'), ('MTD'), ('QTD'), ('HYTD'), ('YTD')) P(Period)
            LEFT JOIN StatusMetrics SM ON BM.[User] = SM.[User];

        -- Log success
        DECLARE @Duration_MS INT = DATEDIFF(MILLISECOND, @RefreshStart, SYSDATETIME());
        DECLARE @RowCount INT = @@ROWCOUNT;
        PRINT 'Recruiter SubmissionMetricsView refreshed successfully - ' + CAST(@RowCount AS VARCHAR(10)) + ' rows in ' + CAST(@Duration_MS AS VARCHAR(10)) + 'ms';
        
    END TRY
    BEGIN CATCH
        -- Log error details
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
        DECLARE @ErrorState INT = ERROR_STATE();
        
        PRINT 'Error refreshing Recruiter SubmissionMetricsView: ' + @ErrorMessage;
        RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
    END CATCH
END;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE   PROCEDURE [DashboardAccountManager]
    @User VARCHAR(50) = 'DAVE'
AS
BEGIN
	SET NOCOUNT ON;
	SET ARITHABORT ON;
	SET ANSI_NULLS ON;
	SET QUOTED_IDENTIFIER ON;

    DECLARE @Today DATE = CAST(GETDATE() AS DATE);
    DECLARE @YearStart DATE = DATEFROMPARTS(YEAR(@Today), 1, 1);

    DECLARE @Start7  DATE = CASE WHEN DATEADD(DAY, -7, @Today) > @YearStart THEN DATEADD(DAY, -7, @Today) ELSE @YearStart END;
    DECLARE @Start30 DATE = CASE WHEN DATEADD(DAY, -30, @Today) > @YearStart THEN DATEADD(DAY, -30, @Today) ELSE @YearStart END;
    DECLARE @Start90 DATE = CASE WHEN DATEADD(DAY, -90, @Today) > @YearStart THEN DATEADD(DAY, -90, @Today) ELSE @YearStart END;
    DECLARE @Start180 DATE = CASE WHEN DATEADD(DAY, -180, @Today) > @YearStart THEN DATEADD(DAY, -180, @Today) ELSE @YearStart END;
    DECLARE @StartYTD DATE = @YearStart;

    --------------------------------------------------------------------------------
    -- QUERY 1: All Requisitions by Recruiter
    --------------------------------------------------------------------------------
    ;WITH Periods AS (
        SELECT '7' AS Period, @Start7 AS StartDate
        UNION ALL SELECT '30', @Start30
        UNION ALL SELECT '90', @Start90
        UNION ALL SELECT '180', @Start180
        UNION ALL SELECT 'YTD', @StartYTD
    ),
    Base AS (
        SELECT UpdatedDate AS ActionDate
        FROM Requisitions
        WHERE (CreatedBy = @User OR UpdatedBy = @User)
    )
    SELECT @User, Period, COUNT(b.ActionDate) AS ReqCount
    FROM Periods
    LEFT JOIN Base b ON b.ActionDate >= Periods.StartDate
    GROUP BY Period;

    --------------------------------------------------------------------------------
    -- QUERY 2: Open Requisitions
    --------------------------------------------------------------------------------
    ;WITH Periods AS (
        SELECT '7' AS Period, @Start7 AS StartDate
        UNION ALL SELECT '30', @Start30
        UNION ALL SELECT '90', @Start90
        UNION ALL SELECT '180', @Start180
        UNION ALL SELECT 'YTD', @StartYTD
    ),
    BaseOpen AS (
        SELECT UpdatedDate AS ActionDate
        FROM Requisitions
        WHERE (CreatedBy = @User OR UpdatedBy = @User)
        AND Status IN ('NEW', 'OPN', 'PAR')
    )
    SELECT @User, Period, COUNT(b.ActionDate) AS ReqCount
    FROM Periods
    LEFT JOIN BaseOpen b ON b.ActionDate >= Periods.StartDate
    GROUP BY Period;

    --------------------------------------------------------------------------------
    -- QUERY 3: Unique Candidate Submissions
    --------------------------------------------------------------------------------
    ;WITH Periods AS (
        SELECT '7' AS Period, @Start7 AS StartDate
        UNION ALL SELECT '30', @Start30
        UNION ALL SELECT '90', @Start90
        UNION ALL SELECT '180', @Start180
        UNION ALL SELECT 'YTD', @StartYTD
    ),
    FirstSubs AS (
        SELECT s.CandidateId, s.RequisitionId, MIN(s.CreatedDate) AS FirstSubmittedDate
        FROM Submissions s
        JOIN Requisitions r ON r.Id = s.RequisitionId
        WHERE r.CreatedBy = @User OR r.UpdatedBy = @User
        GROUP BY s.CandidateId, s.RequisitionId
    )
    SELECT @User, Period, COUNT(fs.RequisitionId) AS SubmissionCount
    FROM Periods
    LEFT JOIN FirstSubs fs ON fs.FirstSubmittedDate >= Periods.StartDate
    GROUP BY Period;

    --------------------------------------------------------------------------------
    -- QUERY 4: Interviewed Candidates
    --------------------------------------------------------------------------------
    ;WITH Periods AS (
        SELECT '7' AS Period, @Start7 AS StartDate
        UNION ALL SELECT '30', @Start30
        UNION ALL SELECT '90', @Start90
        UNION ALL SELECT '180', @Start180
        UNION ALL SELECT 'YTD', @StartYTD
    ),
    RawInterviews AS (
        SELECT s.CandidateId, s.RequisitionId, MAX(s.CreatedDate) AS LastInterviewDate
        FROM Submissions s
        JOIN Requisitions r ON r.Id = s.RequisitionId
        WHERE s.Status = 'INT' AND (r.CreatedBy = @User OR r.UpdatedBy = @User)
        GROUP BY s.CandidateId, s.RequisitionId
    )
    SELECT @User, Period, COUNT(ri.RequisitionId) AS InterviewCount
    FROM Periods
    LEFT JOIN RawInterviews ri ON ri.LastInterviewDate >= Periods.StartDate
    GROUP BY Period;

    --------------------------------------------------------------------------------
    -- QUERY 5: Offers Extended
    --------------------------------------------------------------------------------
    ;WITH Periods AS (
        SELECT '7' AS Period, @Start7 AS StartDate
        UNION ALL SELECT '30', @Start30
        UNION ALL SELECT '90', @Start90
        UNION ALL SELECT '180', @Start180
        UNION ALL SELECT 'YTD', @StartYTD
    ),
    RawOffers AS (
        SELECT s.CandidateId, s.RequisitionId, MAX(s.CreatedDate) AS LastOfferDate
        FROM Submissions s
        JOIN Requisitions r ON r.Id = s.RequisitionId
        WHERE s.Status = 'OEX' AND (r.CreatedBy = @User OR r.UpdatedBy = @User)
        GROUP BY s.CandidateId, s.RequisitionId
    )
    SELECT @User, Period, COUNT(ro.RequisitionId) AS OfferExtendedCount
    FROM Periods
    LEFT JOIN RawOffers ro ON ro.LastOfferDate >= Periods.StartDate
    GROUP BY Period;

    --------------------------------------------------------------------------------
    -- QUERY 6: Hired Candidates
    --------------------------------------------------------------------------------
    ;WITH Periods AS (
        SELECT '7' AS Period, @Start7 AS StartDate
        UNION ALL SELECT '30', @Start30
        UNION ALL SELECT '90', @Start90
        UNION ALL SELECT '180', @Start180
        UNION ALL SELECT 'YTD', @StartYTD
    ),
    RawHires AS (
        SELECT s.CandidateId, s.RequisitionId, MAX(s.CreatedDate) AS LastHireDate
        FROM Submissions s
        JOIN Requisitions r ON r.Id = s.RequisitionId
        WHERE s.Status = 'HIR' AND (r.CreatedBy = @User OR r.UpdatedBy = @User)
        GROUP BY s.CandidateId, s.RequisitionId
    )
    SELECT @User, Period, COUNT(rh.RequisitionId) AS HiredCount
    FROM Periods
    LEFT JOIN RawHires rh ON rh.LastHireDate >= Periods.StartDate
    GROUP BY Period;

    --------------------------------------------------------------------------------
    -- QUERY 7: Offer-to-Hire Ratio
    --------------------------------------------------------------------------------
    ;WITH Periods AS (
        SELECT '7' AS Period, @Start7 AS StartDate
        UNION ALL SELECT '30', @Start30
        UNION ALL SELECT '90', @Start90
        UNION ALL SELECT '180', @Start180
        UNION ALL SELECT 'YTD', @StartYTD
    ),
    Offers AS (
        SELECT s.CandidateId, s.RequisitionId, MAX(s.CreatedDate) AS OfferDate
        FROM Submissions s
        JOIN Requisitions r ON r.Id = s.RequisitionId
        WHERE s.Status = 'OEX' AND (r.CreatedBy = @User OR r.UpdatedBy = @User)
        GROUP BY s.CandidateId, s.RequisitionId
    ),
    Hires AS (
        SELECT s.CandidateId, s.RequisitionId, MAX(s.CreatedDate) AS HireDate
        FROM Submissions s
        JOIN Requisitions r ON r.Id = s.RequisitionId
        WHERE s.Status = 'HIR' AND (r.CreatedBy = @User OR r.UpdatedBy = @User)
        GROUP BY s.CandidateId, s.RequisitionId
    )
    SELECT @User AS QueryTag, Period,
           COUNT(DISTINCT o.RequisitionId) AS OfferCount,
           COUNT(DISTINCT h.RequisitionId) AS HireCount,
           CASE WHEN COUNT(DISTINCT o.RequisitionId) > 0 THEN CAST(COUNT(DISTINCT h.RequisitionId) AS DECIMAL(5,2)) / COUNT(DISTINCT o.RequisitionId) ELSE 0.00 END AS HireToOfferRatio
    FROM Periods
    LEFT JOIN Offers o ON o.OfferDate >= Periods.StartDate
    LEFT JOIN Hires h ON h.HireDate >= Periods.StartDate
    GROUP BY Period;

    --------------------------------------------------------------------------------
    -- QUERY 8 (Set 2): First-time PHN submissions on requisitions created in last 30 days
    --------------------------------------------------------------------------------
    ;WITH FirstPHN AS (
        SELECT s.CandidateId, s.RequisitionId, MIN(s.CreatedDate) AS SubmittedDate
        FROM Submissions s
        WHERE s.Status = 'PHN'
        GROUP BY s.CandidateId, s.RequisitionId
    ),
    Filtered AS (
        SELECT r.CompanyId, r.Code AS RequisitionCode, r.Positions, r.PosTitle,
               c.FirstName + ' ' + c.LastName AS CandidateName, f.SubmittedDate, s.Notes, r.Id as RequisitionID
        FROM FirstPHN f
        JOIN Requisitions r ON r.Id = f.RequisitionId
        JOIN Submissions s ON s.CandidateId = f.CandidateId AND s.RequisitionId = f.RequisitionId AND s.CreatedDate = f.SubmittedDate
        JOIN Candidate c ON c.ID = f.CandidateId
        WHERE r.CreatedDate >= DATEADD(DAY, -30, CAST(GETDATE() AS DATE))
    )
    SELECT co.CompanyName, f.RequisitionCode, f.Positions, f.PosTitle,
           f.CandidateName, f.SubmittedDate, f.Notes
    FROM Filtered f
    JOIN Companies co ON co.ID = f.CompanyId
    ORDER BY co.CompanyName,
             (SELECT COUNT(*) FROM Submissions s2
              WHERE s2.RequisitionId = f.RequisitionID
              AND s2.Status = 'PHN') DESC,
             f.SubmittedDate DESC;

    --------------------------------------------------------------------------------
    -- QUERY 9 (Set 3): Hired candidates this year with placeholders for future data
    --------------------------------------------------------------------------------
    ;WITH LatestHire AS (
        SELECT s.CandidateId, s.RequisitionId, MAX(s.CreatedDate) AS HireDate
        FROM Submissions s
        WHERE s.Status = 'HIR'
        GROUP BY s.CandidateId, s.RequisitionId
    ),
    HireInfo AS (
        SELECT r.CompanyId, r.Code AS RequisitionCode, r.Positions, r.PosTitle,
               c.FirstName + ' ' + c.LastName AS CandidateName, lh.HireDate
        FROM LatestHire lh
        JOIN Requisitions r ON r.Id = lh.RequisitionId
        JOIN Candidate c ON c.ID = lh.CandidateId
        WHERE lh.HireDate >= DATEFROMPARTS(YEAR(GETDATE()), 1, 1)
    )
    SELECT co.CompanyName, h.RequisitionCode, h.Positions, h.PosTitle,
           h.CandidateName, h.HireDate,
           0.00 AS SalaryOffered,
           GETDATE() AS StartDate,
           GETDATE() AS InvoiceDate,
           GETDATE() AS PaymentDate
    FROM HireInfo h
    JOIN Companies co ON co.ID = h.CompanyId
    ORDER BY h.HireDate DESC;


END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [DashboardAccountsManager]
    @User VARCHAR(10) = 'DAVE'
AS
BEGIN
	SET NOCOUNT ON;
	SET ARITHABORT ON;
	SET ANSI_NULLS ON;
	SET QUOTED_IDENTIFIER ON;
    
    -- Set default user if not provided
    IF @User IS NULL
        SET @User = SYSTEM_USER;
    
    -- Common date variables
    DECLARE @Today DATE = GETDATE();
    DECLARE @YearStart DATE = DATEADD(YEAR, DATEDIFF(YEAR, 0, @Today), 0);
    
    -- Common date calculations
    DECLARE @StartDate7D DATE = DATEADD(DAY, -7, @Today);
    DECLARE @StartDateMTD DATE = DATEADD(MONTH, DATEDIFF(MONTH, 0, @Today), 0);
    DECLARE @StartDateQTD DATE = DATEADD(QUARTER, DATEDIFF(QUARTER, 0, @Today), 0);
    DECLARE @StartDateHYTD DATE = CASE 
        WHEN MONTH(@Today) <= 6 THEN DATEFROMPARTS(YEAR(@Today), 1, 1)
        ELSE DATEFROMPARTS(YEAR(@Today), 7, 1)
    END;
    
    -- Generic Query for Dropdown (Memory-Optimized)
    SELECT 
        CreatedBy as KeyValue, FullName as Text
    FROM 
        dbo.ActiveUsersView 
    WHERE 
        CreatedBy = @User
    FOR JSON AUTO;
    
    /***************************************************************************
    * RESULT SET 1: TIME-BOUND METRICS (Conditional Aggregation)
    * Returns 6 different queries with time period analysis
    ***************************************************************************/
    
    -- Create temp table for requisition metrics
    DROP TABLE IF EXISTS #RequisitionMetrics;
    
    SELECT 
        @User as [User],
        -- Total requisitions created
        ISNULL(SUM(CASE WHEN CAST(R.Updated AS DATE) BETWEEN @StartDate7D AND @Today THEN 1 ELSE 0 END), 0) as TOTAL_LAST7D_COUNT,
        ISNULL(SUM(CASE WHEN CAST(R.Updated AS DATE) BETWEEN @StartDateMTD AND @Today THEN 1 ELSE 0 END), 0) as TOTAL_MTD_COUNT,
        ISNULL(SUM(CASE WHEN CAST(R.Updated AS DATE) BETWEEN @StartDateQTD AND @Today THEN 1 ELSE 0 END), 0) as TOTAL_QTD_COUNT,
        ISNULL(SUM(CASE WHEN CAST(R.Updated AS DATE) BETWEEN @StartDateHYTD AND @Today THEN 1 ELSE 0 END), 0) as TOTAL_HYTD_COUNT,
        ISNULL(SUM(CASE WHEN CAST(R.Updated AS DATE) BETWEEN @YearStart AND @Today THEN 1 ELSE 0 END), 0) as TOTAL_YTD_COUNT,
        -- Active requisitions (OPN, NEW, PAR)
        ISNULL(SUM(CASE WHEN CAST(R.Updated AS DATE) BETWEEN @StartDate7D AND @Today AND R.Status IN ('OPN','NEW','PAR') THEN 1 ELSE 0 END), 0) as ACTIVE_LAST7D_COUNT,
        ISNULL(SUM(CASE WHEN CAST(R.Updated AS DATE) BETWEEN @StartDateMTD AND @Today AND R.Status IN ('OPN','NEW','PAR') THEN 1 ELSE 0 END), 0) as ACTIVE_MTD_COUNT,
        ISNULL(SUM(CASE WHEN CAST(R.Updated AS DATE) BETWEEN @StartDateQTD AND @Today AND R.Status IN ('OPN','NEW','PAR') THEN 1 ELSE 0 END), 0) as ACTIVE_QTD_COUNT,
        ISNULL(SUM(CASE WHEN CAST(R.Updated AS DATE) BETWEEN @StartDateHYTD AND @Today AND R.Status IN ('OPN','NEW','PAR') THEN 1 ELSE 0 END), 0) as ACTIVE_HYTD_COUNT,
        ISNULL(SUM(CASE WHEN CAST(R.Updated AS DATE) BETWEEN @YearStart AND @Today AND R.Status IN ('OPN','NEW','PAR') THEN 1 ELSE 0 END), 0) as ACTIVE_YTD_COUNT
    INTO #RequisitionMetrics
    FROM (SELECT 1 as dummy) d -- Ensures we always have at least one row
    LEFT JOIN RequisitionView R ON R.CreatedBy = @User;
    
    -- Create temp table for submission status metrics
    DROP TABLE IF EXISTS #SubmissionMetrics;
    
    WITH StatusSubmissions AS (
        SELECT 
            S.RequisitionId,
            S.CandidateId,
            S.Status,
            MAX(S.CreatedDate) as LatestStatusDate
        FROM Submissions S
        INNER JOIN Requisitions R ON R.Id = S.RequisitionId AND R.CreatedBy = @User
        WHERE S.Status IN ('INT', 'OEX', 'HIR', 'PHN')
        GROUP BY S.RequisitionId, S.CandidateId, S.Status
    )
    SELECT 
        @User as [User],
        -- Interview status counts
        ISNULL(COUNT(DISTINCT CASE WHEN CAST(SS.LatestStatusDate AS DATE) BETWEEN @StartDate7D AND @Today AND SS.Status = 'INT' THEN CAST(SS.RequisitionId AS VARCHAR(10)) + '-' + CAST(SS.CandidateId AS VARCHAR(10)) END), 0) as INT_LAST7D_COUNT,
        ISNULL(COUNT(DISTINCT CASE WHEN CAST(SS.LatestStatusDate AS DATE) BETWEEN @StartDateMTD AND @Today AND SS.Status = 'INT' THEN CAST(SS.RequisitionId AS VARCHAR(10)) + '-' + CAST(SS.CandidateId AS VARCHAR(10)) END), 0) as INT_MTD_COUNT,
        ISNULL(COUNT(DISTINCT CASE WHEN CAST(SS.LatestStatusDate AS DATE) BETWEEN @StartDateQTD AND @Today AND SS.Status = 'INT' THEN CAST(SS.RequisitionId AS VARCHAR(10)) + '-' + CAST(SS.CandidateId AS VARCHAR(10)) END), 0) as INT_QTD_COUNT,
        ISNULL(COUNT(DISTINCT CASE WHEN CAST(SS.LatestStatusDate AS DATE) BETWEEN @StartDateHYTD AND @Today AND SS.Status = 'INT' THEN CAST(SS.RequisitionId AS VARCHAR(10)) + '-' + CAST(SS.CandidateId AS VARCHAR(10)) END), 0) as INT_HYTD_COUNT,
        ISNULL(COUNT(DISTINCT CASE WHEN CAST(SS.LatestStatusDate AS DATE) BETWEEN @YearStart AND @Today AND SS.Status = 'INT' THEN CAST(SS.RequisitionId AS VARCHAR(10)) + '-' + CAST(SS.CandidateId AS VARCHAR(10)) END), 0) as INT_YTD_COUNT,
        -- Offer extended counts
        ISNULL(COUNT(DISTINCT CASE WHEN CAST(SS.LatestStatusDate AS DATE) BETWEEN @StartDate7D AND @Today AND SS.Status = 'OEX' THEN CAST(SS.RequisitionId AS VARCHAR(10)) + '-' + CAST(SS.CandidateId AS VARCHAR(10)) END), 0) as OEX_LAST7D_COUNT,
        ISNULL(COUNT(DISTINCT CASE WHEN CAST(SS.LatestStatusDate AS DATE) BETWEEN @StartDateMTD AND @Today AND SS.Status = 'OEX' THEN CAST(SS.RequisitionId AS VARCHAR(10)) + '-' + CAST(SS.CandidateId AS VARCHAR(10)) END), 0) as OEX_MTD_COUNT,
        ISNULL(COUNT(DISTINCT CASE WHEN CAST(SS.LatestStatusDate AS DATE) BETWEEN @StartDateQTD AND @Today AND SS.Status = 'OEX' THEN CAST(SS.RequisitionId AS VARCHAR(10)) + '-' + CAST(SS.CandidateId AS VARCHAR(10)) END), 0) as OEX_QTD_COUNT,
        ISNULL(COUNT(DISTINCT CASE WHEN CAST(SS.LatestStatusDate AS DATE) BETWEEN @StartDateHYTD AND @Today AND SS.Status = 'OEX' THEN CAST(SS.RequisitionId AS VARCHAR(10)) + '-' + CAST(SS.CandidateId AS VARCHAR(10)) END), 0) as OEX_HYTD_COUNT,
        ISNULL(COUNT(DISTINCT CASE WHEN CAST(SS.LatestStatusDate AS DATE) BETWEEN @YearStart AND @Today AND SS.Status = 'OEX' THEN CAST(SS.RequisitionId AS VARCHAR(10)) + '-' + CAST(SS.CandidateId AS VARCHAR(10)) END), 0) as OEX_YTD_COUNT,
        -- Hired counts
        ISNULL(COUNT(DISTINCT CASE WHEN CAST(SS.LatestStatusDate AS DATE) BETWEEN @StartDate7D AND @Today AND SS.Status = 'HIR' THEN CAST(SS.RequisitionId AS VARCHAR(10)) + '-' + CAST(SS.CandidateId AS VARCHAR(10)) END), 0) as HIR_LAST7D_COUNT,
        ISNULL(COUNT(DISTINCT CASE WHEN CAST(SS.LatestStatusDate AS DATE) BETWEEN @StartDateMTD AND @Today AND SS.Status = 'HIR' THEN CAST(SS.RequisitionId AS VARCHAR(10)) + '-' + CAST(SS.CandidateId AS VARCHAR(10)) END), 0) as HIR_MTD_COUNT,
        ISNULL(COUNT(DISTINCT CASE WHEN CAST(SS.LatestStatusDate AS DATE) BETWEEN @StartDateQTD AND @Today AND SS.Status = 'HIR' THEN CAST(SS.RequisitionId AS VARCHAR(10)) + '-' + CAST(SS.CandidateId AS VARCHAR(10)) END), 0) as HIR_QTD_COUNT,
        ISNULL(COUNT(DISTINCT CASE WHEN CAST(SS.LatestStatusDate AS DATE) BETWEEN @StartDateHYTD AND @Today AND SS.Status = 'HIR' THEN CAST(SS.RequisitionId AS VARCHAR(10)) + '-' + CAST(SS.CandidateId AS VARCHAR(10)) END), 0) as HIR_HYTD_COUNT,
        ISNULL(COUNT(DISTINCT CASE WHEN CAST(SS.LatestStatusDate AS DATE) BETWEEN @YearStart AND @Today AND SS.Status = 'HIR' THEN CAST(SS.RequisitionId AS VARCHAR(10)) + '-' + CAST(SS.CandidateId AS VARCHAR(10)) END), 0) as HIR_YTD_COUNT
    INTO #SubmissionMetrics
    FROM (SELECT 1 as dummy) d -- Ensures we always have at least one row
    LEFT JOIN StatusSubmissions SS ON 1=1;
    
    /***************************************************************************
    * Query 1: Total Requisitions Created
    ***************************************************************************/
    SELECT [User], TOTAL_LAST7D_COUNT as LAST7D_COUNT, TOTAL_MTD_COUNT as MTD_COUNT, TOTAL_QTD_COUNT as QTD_COUNT, TOTAL_HYTD_COUNT as HYTD_COUNT, TOTAL_YTD_COUNT as YTD_COUNT
    FROM #RequisitionMetrics FOR JSON PATH, WITHOUT_ARRAY_WRAPPER;
    
    /***************************************************************************
    * Query 2: Active Requisitions (OPN, NEW, PAR)
    ***************************************************************************/
    SELECT [User], ACTIVE_LAST7D_COUNT as LAST7D_COUNT, ACTIVE_MTD_COUNT as MTD_COUNT, ACTIVE_QTD_COUNT as QTD_COUNT, ACTIVE_HYTD_COUNT as HYTD_COUNT, ACTIVE_YTD_COUNT as YTD_COUNT
    FROM #RequisitionMetrics FOR JSON PATH, WITHOUT_ARRAY_WRAPPER;
    
    /***************************************************************************
    * Query 3: Candidates in Interview (INT)
    ***************************************************************************/
    SELECT [User], INT_LAST7D_COUNT as LAST7D_COUNT, INT_MTD_COUNT as MTD_COUNT, INT_QTD_COUNT as QTD_COUNT, INT_HYTD_COUNT as HYTD_COUNT, INT_YTD_COUNT as YTD_COUNT
    FROM #SubmissionMetrics FOR JSON PATH, WITHOUT_ARRAY_WRAPPER;
    
    /***************************************************************************
    * Query 4: Offers Extended (OEX)
    ***************************************************************************/
    SELECT [User], OEX_LAST7D_COUNT as LAST7D_COUNT, OEX_MTD_COUNT as MTD_COUNT, OEX_QTD_COUNT as QTD_COUNT, OEX_HYTD_COUNT as HYTD_COUNT, OEX_YTD_COUNT as YTD_COUNT
    FROM #SubmissionMetrics FOR JSON PATH, WITHOUT_ARRAY_WRAPPER;
    
    /***************************************************************************
    * Query 5: Candidates Hired (HIR)
    ***************************************************************************/
    SELECT [User], HIR_LAST7D_COUNT as LAST7D_COUNT, HIR_MTD_COUNT as MTD_COUNT, HIR_QTD_COUNT as QTD_COUNT, HIR_HYTD_COUNT as HYTD_COUNT, HIR_YTD_COUNT as YTD_COUNT
    FROM #SubmissionMetrics FOR JSON PATH, WITHOUT_ARRAY_WRAPPER;
    
    /***************************************************************************
    * Query 6: Hire-to-Offer Ratio (HIR/OEX)
    ***************************************************************************/
    SELECT 
        [User],
        CASE WHEN OEX_LAST7D_COUNT = 0 THEN 0.00 ELSE CAST(HIR_LAST7D_COUNT AS DECIMAL(10,2)) / CAST(OEX_LAST7D_COUNT AS DECIMAL(10,2)) END as LAST7D_RATIO,
        CASE WHEN OEX_MTD_COUNT = 0 THEN 0.00 ELSE CAST(HIR_MTD_COUNT AS DECIMAL(10,2)) / CAST(OEX_MTD_COUNT AS DECIMAL(10,2)) END as MTD_RATIO,
        CASE WHEN OEX_QTD_COUNT = 0 THEN 0.00 ELSE CAST(HIR_QTD_COUNT AS DECIMAL(10,2)) / CAST(OEX_QTD_COUNT AS DECIMAL(10,2)) END as QTD_RATIO,
        CASE WHEN OEX_HYTD_COUNT = 0 THEN 0.00 ELSE CAST(HIR_HYTD_COUNT AS DECIMAL(10,2)) / CAST(OEX_HYTD_COUNT AS DECIMAL(10,2)) END as HYTD_RATIO,
        CASE WHEN OEX_YTD_COUNT = 0 THEN 0.00 ELSE CAST(HIR_YTD_COUNT AS DECIMAL(10,2)) / CAST(OEX_YTD_COUNT AS DECIMAL(10,2)) END as YTD_RATIO
    FROM #SubmissionMetrics FOR JSON PATH, WITHOUT_ARRAY_WRAPPER;
    
    /***************************************************************************
    * RESULT SET 2: RECENT ACTIVITY REPORT
    * Query 7: 30-Day Submission Activity
    ***************************************************************************/
    DECLARE @StartDate30 DATE = CASE 
        WHEN DATEADD(DAY, -30, @Today) < @YearStart THEN @YearStart
        ELSE DATEADD(DAY, -30, @Today)
    END;
    DECLARE @return varchar(max);

    WITH CompanySubmissionCounts AS (
        -- Get submission counts by company for sorting
        SELECT 
            C.CompanyName,
            COUNT(DISTINCT CAST(S.RequisitionId AS VARCHAR(10)) + '-' + CAST(S.CandidateId AS VARCHAR(10))) as SubmissionCount
        FROM Submissions S
        INNER JOIN Requisitions R ON R.Id = S.RequisitionId AND R.CreatedBy = @User
        INNER JOIN Companies C ON C.ID = R.CompanyId
        WHERE S.CreatedDate >= @StartDate30 AND S.CreatedDate <= DATEADD(DAY, 1, @Today)
        GROUP BY C.CompanyName
    ),
    SubmissionSummary AS (
        -- Get first and last activity for each Req+Cand combination
        SELECT 
            S.RequisitionId,
            S.CandidateId,
            MIN(S.CreatedDate) as DateFirstSubmitted,
            MAX(S.CreatedDate) as LastActivityDate
        FROM Submissions S
        INNER JOIN Requisitions R ON R.Id = S.RequisitionId AND R.CreatedBy = @User
        GROUP BY S.RequisitionId, S.CandidateId
        HAVING MIN(S.CreatedDate) >= @StartDate30 AND MIN(S.CreatedDate) <= DATEADD(DAY, 1, @Today)
    ),
    LastActivity AS (
        -- Get the status and notes from the most recent submission
        SELECT 
            S.RequisitionId,
            S.CandidateId,
            S.Status,
            S.Notes,
            S.CreatedDate,
            ROW_NUMBER() OVER (PARTITION BY S.RequisitionId, S.CandidateId ORDER BY S.CreatedDate DESC) as RN
        FROM Submissions S
        INNER JOIN SubmissionSummary SS 
            ON SS.RequisitionId = S.RequisitionId 
            AND SS.CandidateId = S.CandidateId
    )

    SELECT @return =
    (SELECT 
        C.CompanyName + ' - [' + R.Code + '] - ' + CAST(R.Positions as varchar(5)) + ' Positions - ' + R.PosTitle as Company,
        LTRIM(RTRIM(ISNULL(CAND.FirstName, '') + ' ' + ISNULL(CAND.LastName, ''))) as [CandidateName],
        LA.Status as [CurrentStatus],
        CAST(SS.DateFirstSubmitted AS DATE) as [DateFirstSubmitted],
        CAST(SS.LastActivityDate AS DATE) as [LastActivityDate],
        ISNULL(LA.Notes, '') as [ActivityNotes],
        @User [User]
    FROM SubmissionSummary SS
    INNER JOIN LastActivity LA ON LA.RequisitionId = SS.RequisitionId AND LA.CandidateId = SS.CandidateId AND LA.RN = 1
    INNER JOIN Requisitions R ON R.Id = SS.RequisitionId
    INNER JOIN Companies C ON C.ID = R.CompanyId
    INNER JOIN Candidate CAND ON CAND.ID = SS.CandidateId
    INNER JOIN CompanySubmissionCounts CSC ON CSC.CompanyName = C.CompanyName
    ORDER BY 
        CSC.SubmissionCount DESC,  -- Companies with more submissions first
        C.CompanyName ASC,         -- Alphabetical for ties
        R.Id ASC,                  -- RequisitionID within company
        SS.DateFirstSubmitted ASC
    FOR JSON PATH); -- Date First Submitted within requisition
    
    SELECT @return;

    /***************************************************************************
    * RESULT SET 3: PLACEMENT REPORT
    * Query 8: 3-Month Hire Report
    ***************************************************************************/
    DECLARE @StartDate90 DATE = CASE 
        WHEN DATEADD(MONTH, -3, @Today) < @YearStart THEN @YearStart
        ELSE DATEADD(MONTH, -3, @Today)
    END;

    WITH HiredCandidates AS (
        -- Get the latest HIR record for each Req+Cand combination
        SELECT 
            S.RequisitionId,
            S.CandidateId,
            MAX(S.CreatedDate) as DateHired,
            ROW_NUMBER() OVER (PARTITION BY S.RequisitionId, S.CandidateId ORDER BY MAX(S.CreatedDate) DESC) as RN
        FROM Submissions S
        INNER JOIN Requisitions R ON R.Id = S.RequisitionId AND R.CreatedBy = @User
        WHERE S.Status = 'HIR'
        GROUP BY S.RequisitionId, S.CandidateId
        HAVING MAX(S.CreatedDate) >= @StartDate90 AND MAX(S.CreatedDate) <= DATEADD(DAY, 1, @Today)
    )

    SELECT @return = 
    (SELECT 
        C.CompanyName as Company,
        R.Code as [RequisitionNumber],
        R.Positions as [NumPosition],
        R.PosTitle as Title,
        LTRIM(RTRIM(ISNULL(CAND.FirstName, '') + ' ' + ISNULL(CAND.LastName, ''))) as [CandidateName],
        CAST(HC.DateHired AS DATE) as [DateHired],
        CAST(0.00 AS DECIMAL(9,2)) as [SalaryOffered],
        CAST(GETDATE() AS DATE) as [StartDate],
        CAST(GETDATE() AS DATE) as [DateInvoiced],
        CAST(GETDATE() AS DATE) as [DatePaid],
        CAST(0.00 AS DECIMAL(9,2)) as [PlacementFee],
        CAST(0.00 AS DECIMAL(5,2)) as [CommissionPercent],
        CAST(0.00 AS DECIMAL(9,2)) as [CommissionEarned],
        @User [User]
    FROM HiredCandidates HC
    INNER JOIN Requisitions R ON R.Id = HC.RequisitionId
    INNER JOIN Companies C ON C.ID = R.CompanyId
    INNER JOIN Candidate CAND ON CAND.ID = HC.CandidateId
    WHERE HC.RN = 1
    ORDER BY 
        C.CompanyName ASC,
        R.Id ASC,
        HC.DateHired DESC
    FOR JSON PATH);
    
    SELECT @return;
    
    /***************************************************************************
    * RESULT SET 4: TIMING ANALYTICS
    * Query 1: Time to Fill, Time to Hire, and Time in Stage metrics (By Requisition)
    * Query 2: Time to Fill, Time to Hire, and Time in Stage metrics (By Company)
    ***************************************************************************/
    
    -- Create temp table for timing analytics
    DROP TABLE IF EXISTS #TimingAnalytics;
    
    -- Get requisitions from last year
    DECLARE @StartDate365 DATE = DATEADD(YEAR, -1, @Today);
    
    WITH RequisitionBase AS (
        -- Get requisitions created by user in last year
        SELECT 
            R.Id as RequisitionId,
            R.Code as RequisitionCode,
            C.CompanyName,
            R.PosTitle as Title,
            R.CreatedDate as RequisitionCreatedDate,
            R.CreatedBy,
            -- Time to Fill calculation (only for FUL status)
            CASE 
                WHEN R.Status = 'FUL' THEN DATEDIFF(DAY, R.CreatedDate, R.UpdatedDate)
                ELSE NULL 
            END as TimeToFill
        FROM Requisitions R
        INNER JOIN Companies C ON R.CompanyId = C.ID
        WHERE R.CreatedBy = @User 
            AND CAST(R.CreatedDate AS DATE) >= @StartDate365
    ),
    FirstSubmissions AS (
        -- Get first submission date for each candidate+requisition combo
        SELECT 
            S.RequisitionId,
            S.CandidateId,
            MIN(S.CreatedDate) as FirstSubmissionDate
        FROM Submissions S
        INNER JOIN RequisitionBase RB ON S.RequisitionId = RB.RequisitionId
        GROUP BY S.RequisitionId, S.CandidateId
    ),
    TimeToHireCalc AS (
        -- Calculate Time to Hire for hired candidates
        SELECT 
            FS.RequisitionId,
            FS.CandidateId,
            FS.FirstSubmissionDate,
            MAX(CASE WHEN S.Status = 'HIR' THEN S.CreatedDate END) as HireDate,
            MAX(CASE WHEN S.Status = 'HIR' THEN DATEDIFF(DAY, FS.FirstSubmissionDate, S.CreatedDate) END) as TimeToHire
        FROM FirstSubmissions FS
        INNER JOIN Submissions S ON FS.RequisitionId = S.RequisitionId AND FS.CandidateId = S.CandidateId
        GROUP BY FS.RequisitionId, FS.CandidateId, FS.FirstSubmissionDate
    ),
    StageTimings AS (
        -- Calculate time in each stage
        SELECT 
            FS.RequisitionId,
            FS.CandidateId,
            -- Get the latest date for each stage (in case of multiple records for same stage)
            MAX(CASE WHEN S.Status = 'PEN' THEN S.CreatedDate END) as PEN_Date,
            MAX(CASE WHEN S.Status = 'REJ' THEN S.CreatedDate END) as REJ_Date,
            MAX(CASE WHEN S.Status = 'HLD' THEN S.CreatedDate END) as HLD_Date,
            MAX(CASE WHEN S.Status = 'PHN' THEN S.CreatedDate END) as PHN_Date,
            MAX(CASE WHEN S.Status = 'URW' THEN S.CreatedDate END) as URW_Date,
            MAX(CASE WHEN S.Status = 'INT' THEN S.CreatedDate END) as INT_Date,
            MAX(CASE WHEN S.Status = 'RHM' THEN S.CreatedDate END) as RHM_Date,
            MAX(CASE WHEN S.Status = 'DEC' THEN S.CreatedDate END) as DEC_Date,
            MAX(CASE WHEN S.Status = 'NOA' THEN S.CreatedDate END) as NOA_Date,
            MAX(CASE WHEN S.Status = 'OEX' THEN S.CreatedDate END) as OEX_Date,
            MAX(CASE WHEN S.Status = 'ODC' THEN S.CreatedDate END) as ODC_Date,
            MAX(CASE WHEN S.Status = 'HIR' THEN S.CreatedDate END) as HIR_Date,
            MAX(CASE WHEN S.Status = 'WDR' THEN S.CreatedDate END) as WDR_Date
        FROM FirstSubmissions FS
        INNER JOIN Submissions S ON FS.RequisitionId = S.RequisitionId AND FS.CandidateId = S.CandidateId
        GROUP BY FS.RequisitionId, FS.CandidateId
    ),
    StageTimeCalculations AS (
        -- Calculate days spent in each stage
        SELECT 
            ST.RequisitionId,
            ST.CandidateId,
            FS.FirstSubmissionDate,
            -- Calculate time in each stage (from current stage to next stage or end)
            CASE 
                WHEN ST.PEN_Date IS NULL THEN 0
                WHEN ST.REJ_Date IS NOT NULL AND ST.PEN_Date <= ST.REJ_Date THEN DATEDIFF(DAY, ST.PEN_Date, ST.REJ_Date)
                WHEN ST.HLD_Date IS NOT NULL AND ST.PEN_Date <= ST.HLD_Date THEN DATEDIFF(DAY, ST.PEN_Date, ST.HLD_Date)
                WHEN ST.PHN_Date IS NOT NULL AND ST.PEN_Date <= ST.PHN_Date THEN DATEDIFF(DAY, ST.PEN_Date, ST.PHN_Date)
                ELSE DATEDIFF(DAY, ST.PEN_Date, ISNULL(ST.HIR_Date, ISNULL(ST.WDR_Date, ISNULL(ST.ODC_Date, ISNULL(ST.RHM_Date, @Today)))))
            END as PEN_Days,
            
            CASE 
                WHEN ST.REJ_Date IS NULL THEN 0
                ELSE 0 -- REJ is final stage
            END as REJ_Days,
            
            CASE 
                WHEN ST.HLD_Date IS NULL THEN 0
                WHEN ST.PHN_Date IS NOT NULL AND ST.HLD_Date <= ST.PHN_Date THEN DATEDIFF(DAY, ST.HLD_Date, ST.PHN_Date)
                WHEN ST.URW_Date IS NOT NULL AND ST.HLD_Date <= ST.URW_Date THEN DATEDIFF(DAY, ST.HLD_Date, ST.URW_Date)
                ELSE DATEDIFF(DAY, ST.HLD_Date, ISNULL(ST.HIR_Date, ISNULL(ST.WDR_Date, ISNULL(ST.ODC_Date, ISNULL(ST.RHM_Date, @Today)))))
            END as HLD_Days,
            
            CASE 
                WHEN ST.PHN_Date IS NULL THEN 0
                WHEN ST.URW_Date IS NOT NULL AND ST.PHN_Date <= ST.URW_Date THEN DATEDIFF(DAY, ST.PHN_Date, ST.URW_Date)
                WHEN ST.INT_Date IS NOT NULL AND ST.PHN_Date <= ST.INT_Date THEN DATEDIFF(DAY, ST.PHN_Date, ST.INT_Date)
                ELSE DATEDIFF(DAY, ST.PHN_Date, ISNULL(ST.HIR_Date, ISNULL(ST.WDR_Date, ISNULL(ST.ODC_Date, ISNULL(ST.RHM_Date, @Today)))))
            END as PHN_Days,
            
            CASE 
                WHEN ST.URW_Date IS NULL THEN 0
                WHEN ST.INT_Date IS NOT NULL AND ST.URW_Date <= ST.INT_Date THEN DATEDIFF(DAY, ST.URW_Date, ST.INT_Date)
                WHEN ST.DEC_Date IS NOT NULL AND ST.URW_Date <= ST.DEC_Date THEN DATEDIFF(DAY, ST.URW_Date, ST.DEC_Date)
                ELSE DATEDIFF(DAY, ST.URW_Date, ISNULL(ST.HIR_Date, ISNULL(ST.WDR_Date, ISNULL(ST.ODC_Date, ISNULL(ST.RHM_Date, @Today)))))
            END as URW_Days,
            
            CASE 
                WHEN ST.INT_Date IS NULL THEN 0
                WHEN ST.DEC_Date IS NOT NULL AND ST.INT_Date <= ST.DEC_Date THEN DATEDIFF(DAY, ST.INT_Date, ST.DEC_Date)
                WHEN ST.NOA_Date IS NOT NULL AND ST.INT_Date <= ST.NOA_Date THEN DATEDIFF(DAY, ST.INT_Date, ST.NOA_Date)
                ELSE DATEDIFF(DAY, ST.INT_Date, ISNULL(ST.HIR_Date, ISNULL(ST.WDR_Date, ISNULL(ST.ODC_Date, ISNULL(ST.RHM_Date, @Today)))))
            END as INT_Days,
            
            CASE 
                WHEN ST.RHM_Date IS NULL THEN 0
                ELSE 0 -- RHM is final stage
            END as RHM_Days,
            
            CASE 
                WHEN ST.DEC_Date IS NULL THEN 0
                WHEN ST.NOA_Date IS NOT NULL AND ST.DEC_Date <= ST.NOA_Date THEN DATEDIFF(DAY, ST.DEC_Date, ST.NOA_Date)
                WHEN ST.OEX_Date IS NOT NULL AND ST.DEC_Date <= ST.OEX_Date THEN DATEDIFF(DAY, ST.DEC_Date, ST.OEX_Date)
                ELSE DATEDIFF(DAY, ST.DEC_Date, ISNULL(ST.HIR_Date, ISNULL(ST.WDR_Date, ISNULL(ST.ODC_Date, ISNULL(ST.RHM_Date, @Today)))))
            END as DEC_Days,
            
            CASE 
                WHEN ST.NOA_Date IS NULL THEN 0
                WHEN ST.OEX_Date IS NOT NULL AND ST.NOA_Date <= ST.OEX_Date THEN DATEDIFF(DAY, ST.NOA_Date, ST.OEX_Date)
                ELSE DATEDIFF(DAY, ST.NOA_Date, ISNULL(ST.HIR_Date, ISNULL(ST.WDR_Date, ISNULL(ST.ODC_Date, ISNULL(ST.RHM_Date, @Today)))))
            END as NOA_Days,
            
            CASE 
                WHEN ST.OEX_Date IS NULL THEN 0
                WHEN ST.HIR_Date IS NOT NULL AND ST.OEX_Date <= ST.HIR_Date THEN DATEDIFF(DAY, ST.OEX_Date, ST.HIR_Date)
                WHEN ST.ODC_Date IS NOT NULL AND ST.OEX_Date <= ST.ODC_Date THEN DATEDIFF(DAY, ST.OEX_Date, ST.ODC_Date)
                WHEN ST.WDR_Date IS NOT NULL AND ST.OEX_Date <= ST.WDR_Date THEN DATEDIFF(DAY, ST.OEX_Date, ST.WDR_Date)
                ELSE DATEDIFF(DAY, ST.OEX_Date, @Today)
            END as OEX_Days,
            
            CASE 
                WHEN ST.ODC_Date IS NULL THEN 0
                ELSE 0 -- ODC is final stage
            END as ODC_Days,
            
            CASE 
                WHEN ST.HIR_Date IS NULL THEN 0
                ELSE 0 -- HIR is final stage
            END as HIR_Days,
            
            CASE 
                WHEN ST.WDR_Date IS NULL THEN 0
                ELSE 0 -- WDR is final stage
            END as WDR_Days
            
        FROM StageTimings ST
        INNER JOIN FirstSubmissions FS ON ST.RequisitionId = FS.RequisitionId AND ST.CandidateId = FS.CandidateId
    )
    
    -- Query 1: Timing Analytics by Requisition
    SELECT 
        @User as [User],
        RB.RequisitionCode,
        RB.Title,
        -- Average Time to Fill for this requisition (if filled)
        CEILING(AVG(CAST(RB.TimeToFill as FLOAT))) as AvgTimeToFill,
        -- Average Time to Hire for this requisition  
        CEILING(AVG(CAST(TTH.TimeToHire as FLOAT))) as AvgTimeToHire,
        -- Average time in each stage for this requisition (rounded up to next integer)
        CEILING(AVG(CAST(STC.PEN_Days as FLOAT))) as AvgPEN_Days,
        CEILING(AVG(CAST(STC.REJ_Days as FLOAT))) as AvgREJ_Days,
        CEILING(AVG(CAST(STC.HLD_Days as FLOAT))) as AvgHLD_Days,
        CEILING(AVG(CAST(STC.PHN_Days as FLOAT))) as AvgPHN_Days,
        CEILING(AVG(CAST(STC.URW_Days as FLOAT))) as AvgURW_Days,
        CEILING(AVG(CAST(STC.INT_Days as FLOAT))) as AvgINT_Days,
        CEILING(AVG(CAST(STC.RHM_Days as FLOAT))) as AvgRHM_Days,
        CEILING(AVG(CAST(STC.DEC_Days as FLOAT))) as AvgDEC_Days,
        CEILING(AVG(CAST(STC.NOA_Days as FLOAT))) as AvgNOA_Days,
        CEILING(AVG(CAST(STC.OEX_Days as FLOAT))) as AvgOEX_Days,
        CEILING(AVG(CAST(STC.ODC_Days as FLOAT))) as AvgODC_Days,
        CEILING(AVG(CAST(STC.HIR_Days as FLOAT))) as AvgHIR_Days,
        CEILING(AVG(CAST(STC.WDR_Days as FLOAT))) as AvgWDR_Days,
        COUNT(DISTINCT CONCAT(STC.RequisitionId, '-', STC.CandidateId)) as TotalCandidates
    INTO #TimingAnalytics
    FROM RequisitionBase RB
    LEFT JOIN TimeToHireCalc TTH ON RB.RequisitionId = TTH.RequisitionId
    LEFT JOIN StageTimeCalculations STC ON RB.RequisitionId = STC.RequisitionId
    GROUP BY RB.RequisitionCode, RB.Title;
    
    -- Return the timing analytics results by requisition
    SELECT 
        [User],
        RequisitionCode,
        Title,
        ISNULL(AvgTimeToFill, 0) as TimeToFill_Days,
        ISNULL(AvgTimeToHire, 0) as TimeToHire_Days,
        ISNULL(AvgPEN_Days, 0) as PEN_Days,
        ISNULL(AvgREJ_Days, 0) as REJ_Days,
        ISNULL(AvgHLD_Days, 0) as HLD_Days,
        ISNULL(AvgPHN_Days, 0) as PHN_Days,
        ISNULL(AvgURW_Days, 0) as URW_Days,
        ISNULL(AvgINT_Days, 0) as INT_Days,
        ISNULL(AvgRHM_Days, 0) as RHM_Days,
        ISNULL(AvgDEC_Days, 0) as DEC_Days,
        ISNULL(AvgNOA_Days, 0) as NOA_Days,
        ISNULL(AvgOEX_Days, 0) as OEX_Days,
        ISNULL(AvgODC_Days, 0) as ODC_Days,
        ISNULL(AvgHIR_Days, 0) as HIR_Days,
        ISNULL(AvgWDR_Days, 0) as WDR_Days,
        TotalCandidates
    FROM #TimingAnalytics
    ORDER BY RequisitionCode
    FOR JSON PATH
    ;
    
    -- Clean up timing analytics temp table
    DROP TABLE #TimingAnalytics;
    
    -- Query 2: Timing Analytics by Company
    WITH RequisitionBase AS (
        -- Get requisitions created by user in last year
        SELECT 
            R.Id as RequisitionId,
            C.CompanyName,
            R.CreatedDate as RequisitionCreatedDate,
            R.CreatedBy,
            -- Time to Fill calculation (only for FUL status)
            CASE 
                WHEN R.Status = 'FUL' THEN DATEDIFF(DAY, R.CreatedDate, R.UpdatedDate)
                ELSE NULL 
            END as TimeToFill
        FROM Requisitions R
        INNER JOIN Companies C ON R.CompanyId = C.ID
        WHERE R.CreatedBy = @User 
            AND CAST(R.CreatedDate AS DATE) >= @StartDate365
    ),
    FirstSubmissions AS (
        -- Get first submission date for each candidate+requisition combo
        SELECT 
            S.RequisitionId,
            S.CandidateId,
            MIN(S.CreatedDate) as FirstSubmissionDate
        FROM Submissions S
        INNER JOIN RequisitionBase RB ON S.RequisitionId = RB.RequisitionId
        GROUP BY S.RequisitionId, S.CandidateId
    ),
    TimeToHireCalc AS (
        -- Calculate Time to Hire for hired candidates
        SELECT 
            FS.RequisitionId,
            FS.CandidateId,
            FS.FirstSubmissionDate,
            MAX(CASE WHEN S.Status = 'HIR' THEN S.CreatedDate END) as HireDate,
            MAX(CASE WHEN S.Status = 'HIR' THEN DATEDIFF(DAY, FS.FirstSubmissionDate, S.CreatedDate) END) as TimeToHire
        FROM FirstSubmissions FS
        INNER JOIN Submissions S ON FS.RequisitionId = S.RequisitionId AND FS.CandidateId = S.CandidateId
        GROUP BY FS.RequisitionId, FS.CandidateId, FS.FirstSubmissionDate
    ),
    StageTimings AS (
        -- Calculate time in each stage
        SELECT 
            FS.RequisitionId,
            FS.CandidateId,
            -- Get the latest date for each stage (in case of multiple records for same stage)
            MAX(CASE WHEN S.Status = 'PEN' THEN S.CreatedDate END) as PEN_Date,
            MAX(CASE WHEN S.Status = 'REJ' THEN S.CreatedDate END) as REJ_Date,
            MAX(CASE WHEN S.Status = 'HLD' THEN S.CreatedDate END) as HLD_Date,
            MAX(CASE WHEN S.Status = 'PHN' THEN S.CreatedDate END) as PHN_Date,
            MAX(CASE WHEN S.Status = 'URW' THEN S.CreatedDate END) as URW_Date,
            MAX(CASE WHEN S.Status = 'INT' THEN S.CreatedDate END) as INT_Date,
            MAX(CASE WHEN S.Status = 'RHM' THEN S.CreatedDate END) as RHM_Date,
            MAX(CASE WHEN S.Status = 'DEC' THEN S.CreatedDate END) as DEC_Date,
            MAX(CASE WHEN S.Status = 'NOA' THEN S.CreatedDate END) as NOA_Date,
            MAX(CASE WHEN S.Status = 'OEX' THEN S.CreatedDate END) as OEX_Date,
            MAX(CASE WHEN S.Status = 'ODC' THEN S.CreatedDate END) as ODC_Date,
            MAX(CASE WHEN S.Status = 'HIR' THEN S.CreatedDate END) as HIR_Date,
            MAX(CASE WHEN S.Status = 'WDR' THEN S.CreatedDate END) as WDR_Date
        FROM FirstSubmissions FS
        INNER JOIN Submissions S ON FS.RequisitionId = S.RequisitionId AND FS.CandidateId = S.CandidateId
        GROUP BY FS.RequisitionId, FS.CandidateId
    ),
    StageTimeCalculations AS (
        -- Calculate days spent in each stage
        SELECT 
            ST.RequisitionId,
            ST.CandidateId,
            FS.FirstSubmissionDate,
            -- Calculate time in each stage (from current stage to next stage or end)
            CASE 
                WHEN ST.PEN_Date IS NULL THEN 0
                WHEN ST.REJ_Date IS NOT NULL AND ST.PEN_Date <= ST.REJ_Date THEN DATEDIFF(DAY, ST.PEN_Date, ST.REJ_Date)
                WHEN ST.HLD_Date IS NOT NULL AND ST.PEN_Date <= ST.HLD_Date THEN DATEDIFF(DAY, ST.PEN_Date, ST.HLD_Date)
                WHEN ST.PHN_Date IS NOT NULL AND ST.PEN_Date <= ST.PHN_Date THEN DATEDIFF(DAY, ST.PEN_Date, ST.PHN_Date)
                ELSE DATEDIFF(DAY, ST.PEN_Date, ISNULL(ST.HIR_Date, ISNULL(ST.WDR_Date, ISNULL(ST.ODC_Date, ISNULL(ST.RHM_Date, @Today)))))
            END as PEN_Days,
            
            CASE 
                WHEN ST.REJ_Date IS NULL THEN 0
                ELSE 0 -- REJ is final stage
            END as REJ_Days,
            
            CASE 
                WHEN ST.HLD_Date IS NULL THEN 0
                WHEN ST.PHN_Date IS NOT NULL AND ST.HLD_Date <= ST.PHN_Date THEN DATEDIFF(DAY, ST.HLD_Date, ST.PHN_Date)
                WHEN ST.URW_Date IS NOT NULL AND ST.HLD_Date <= ST.URW_Date THEN DATEDIFF(DAY, ST.HLD_Date, ST.URW_Date)
                ELSE DATEDIFF(DAY, ST.HLD_Date, ISNULL(ST.HIR_Date, ISNULL(ST.WDR_Date, ISNULL(ST.ODC_Date, ISNULL(ST.RHM_Date, @Today)))))
            END as HLD_Days,
            
            CASE 
                WHEN ST.PHN_Date IS NULL THEN 0
                WHEN ST.URW_Date IS NOT NULL AND ST.PHN_Date <= ST.URW_Date THEN DATEDIFF(DAY, ST.PHN_Date, ST.URW_Date)
                WHEN ST.INT_Date IS NOT NULL AND ST.PHN_Date <= ST.INT_Date THEN DATEDIFF(DAY, ST.PHN_Date, ST.INT_Date)
                ELSE DATEDIFF(DAY, ST.PHN_Date, ISNULL(ST.HIR_Date, ISNULL(ST.WDR_Date, ISNULL(ST.ODC_Date, ISNULL(ST.RHM_Date, @Today)))))
            END as PHN_Days,
            
            CASE 
                WHEN ST.URW_Date IS NULL THEN 0
                WHEN ST.INT_Date IS NOT NULL AND ST.URW_Date <= ST.INT_Date THEN DATEDIFF(DAY, ST.URW_Date, ST.INT_Date)
                WHEN ST.DEC_Date IS NOT NULL AND ST.URW_Date <= ST.DEC_Date THEN DATEDIFF(DAY, ST.URW_Date, ST.DEC_Date)
                ELSE DATEDIFF(DAY, ST.URW_Date, ISNULL(ST.HIR_Date, ISNULL(ST.WDR_Date, ISNULL(ST.ODC_Date, ISNULL(ST.RHM_Date, @Today)))))
            END as URW_Days,
            
            CASE 
                WHEN ST.INT_Date IS NULL THEN 0
                WHEN ST.DEC_Date IS NOT NULL AND ST.INT_Date <= ST.DEC_Date THEN DATEDIFF(DAY, ST.INT_Date, ST.DEC_Date)
                WHEN ST.NOA_Date IS NOT NULL AND ST.INT_Date <= ST.NOA_Date THEN DATEDIFF(DAY, ST.INT_Date, ST.NOA_Date)
                ELSE DATEDIFF(DAY, ST.INT_Date, ISNULL(ST.HIR_Date, ISNULL(ST.WDR_Date, ISNULL(ST.ODC_Date, ISNULL(ST.RHM_Date, @Today)))))
            END as INT_Days,
            
            CASE 
                WHEN ST.RHM_Date IS NULL THEN 0
                ELSE 0 -- RHM is final stage
            END as RHM_Days,
            
            CASE 
                WHEN ST.DEC_Date IS NULL THEN 0
                WHEN ST.NOA_Date IS NOT NULL AND ST.DEC_Date <= ST.NOA_Date THEN DATEDIFF(DAY, ST.DEC_Date, ST.NOA_Date)
                WHEN ST.OEX_Date IS NOT NULL AND ST.DEC_Date <= ST.OEX_Date THEN DATEDIFF(DAY, ST.DEC_Date, ST.OEX_Date)
                ELSE DATEDIFF(DAY, ST.DEC_Date, ISNULL(ST.HIR_Date, ISNULL(ST.WDR_Date, ISNULL(ST.ODC_Date, ISNULL(ST.RHM_Date, @Today)))))
            END as DEC_Days,
            
            CASE 
                WHEN ST.NOA_Date IS NULL THEN 0
                WHEN ST.OEX_Date IS NOT NULL AND ST.NOA_Date <= ST.OEX_Date THEN DATEDIFF(DAY, ST.NOA_Date, ST.OEX_Date)
                ELSE DATEDIFF(DAY, ST.NOA_Date, ISNULL(ST.HIR_Date, ISNULL(ST.WDR_Date, ISNULL(ST.ODC_Date, ISNULL(ST.RHM_Date, @Today)))))
            END as NOA_Days,
            
            CASE 
                WHEN ST.OEX_Date IS NULL THEN 0
                WHEN ST.HIR_Date IS NOT NULL AND ST.OEX_Date <= ST.HIR_Date THEN DATEDIFF(DAY, ST.OEX_Date, ST.HIR_Date)
                WHEN ST.ODC_Date IS NOT NULL AND ST.OEX_Date <= ST.ODC_Date THEN DATEDIFF(DAY, ST.OEX_Date, ST.ODC_Date)
                WHEN ST.WDR_Date IS NOT NULL AND ST.OEX_Date <= ST.WDR_Date THEN DATEDIFF(DAY, ST.OEX_Date, ST.WDR_Date)
                ELSE DATEDIFF(DAY, ST.OEX_Date, @Today)
            END as OEX_Days,
            
            CASE 
                WHEN ST.ODC_Date IS NULL THEN 0
                ELSE 0 -- ODC is final stage
            END as ODC_Days,
            
            CASE 
                WHEN ST.HIR_Date IS NULL THEN 0
                ELSE 0 -- HIR is final stage
            END as HIR_Days,
            
            CASE 
                WHEN ST.WDR_Date IS NULL THEN 0
                ELSE 0 -- WDR is final stage
            END as WDR_Days
            
        FROM StageTimings ST
        INNER JOIN FirstSubmissions FS ON ST.RequisitionId = FS.RequisitionId AND ST.CandidateId = FS.CandidateId
    )
    
    -- Return the timing analytics results by company
    SELECT 
        @User as [User],
        RB.CompanyName,
        -- Average Time to Fill for this company (if filled)
        ISNULL(CEILING(AVG(CAST(RB.TimeToFill as FLOAT))), 0) as TimeToFill_Days,
        -- Average Time to Hire for this company  
        ISNULL(CEILING(AVG(CAST(TTH.TimeToHire as FLOAT))), 0) as TimeToHire_Days,
        -- Average time in each stage for this company (rounded up to next integer)
        ISNULL(CEILING(AVG(CAST(STC.PEN_Days as FLOAT))), 0) as PEN_Days,
        ISNULL(CEILING(AVG(CAST(STC.REJ_Days as FLOAT))), 0) as REJ_Days,
        ISNULL(CEILING(AVG(CAST(STC.HLD_Days as FLOAT))), 0) as HLD_Days,
        ISNULL(CEILING(AVG(CAST(STC.PHN_Days as FLOAT))), 0) as PHN_Days,
        ISNULL(CEILING(AVG(CAST(STC.URW_Days as FLOAT))), 0) as URW_Days,
        ISNULL(CEILING(AVG(CAST(STC.INT_Days as FLOAT))), 0) as INT_Days,
        ISNULL(CEILING(AVG(CAST(STC.RHM_Days as FLOAT))), 0) as RHM_Days,
        ISNULL(CEILING(AVG(CAST(STC.DEC_Days as FLOAT))), 0) as DEC_Days,
        ISNULL(CEILING(AVG(CAST(STC.NOA_Days as FLOAT))), 0) as NOA_Days,
        ISNULL(CEILING(AVG(CAST(STC.OEX_Days as FLOAT))), 0) as OEX_Days,
        ISNULL(CEILING(AVG(CAST(STC.ODC_Days as FLOAT))), 0) as ODC_Days,
        ISNULL(CEILING(AVG(CAST(STC.HIR_Days as FLOAT))), 0) as HIR_Days,
        ISNULL(CEILING(AVG(CAST(STC.WDR_Days as FLOAT))), 0) as WDR_Days,
        COUNT(DISTINCT CONCAT(STC.RequisitionId, '-', STC.CandidateId)) as TotalCandidates,
        COUNT(DISTINCT RB.RequisitionId) as TotalRequisitions
    FROM RequisitionBase RB
    LEFT JOIN TimeToHireCalc TTH ON RB.RequisitionId = TTH.RequisitionId
    LEFT JOIN StageTimeCalculations STC ON RB.RequisitionId = STC.RequisitionId
    GROUP BY RB.CompanyName
    ORDER BY RB.CompanyName
    FOR JSON PATH
    ;
    
    -- Clean up temp tables
    DROP TABLE #RequisitionMetrics;
    DROP TABLE #SubmissionMetrics;
END;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- 2. Optimized DashboardAccountsManager
CREATE   PROCEDURE [DashboardAccountsManager_Optimized]
    @User VARCHAR(10) = 'DAVE'
AS
BEGIN
    SET NOCOUNT ON;
    SET ARITHABORT ON;
    SET ANSI_NULLS ON;
    SET QUOTED_IDENTIFIER ON;
    
    IF @User IS NULL SET @User = SYSTEM_USER;
    
    -- Generic Query for Dropdown
    SELECT UserName as KeyValue, FirstName + ' ' + LastName as Text
    FROM dbo.Users 
    WHERE UserName = @User
    FOR JSON AUTO;
    
    -- TIME-BOUND METRICS using materialized views
    SELECT 
        @User as [User],
        -- Total requisitions (from materialized view)
        ISNULL((SELECT Requisitions_Created FROM [dbo].[SubmissionMetricsView] WHERE CreatedBy = @User AND Period = '7D' AND MetricDate = CAST(GETDATE() AS DATE)), 0) as TOTAL_LAST7D_COUNT,
        ISNULL((SELECT Requisitions_Created FROM [dbo].[SubmissionMetricsView] WHERE CreatedBy = @User AND Period = 'MTD' AND MetricDate = CAST(GETDATE() AS DATE)), 0) as TOTAL_MTD_COUNT,
        ISNULL((SELECT Requisitions_Created FROM [dbo].[SubmissionMetricsView] WHERE CreatedBy = @User AND Period = 'QTD' AND MetricDate = CAST(GETDATE() AS DATE)), 0) as TOTAL_QTD_COUNT,
        ISNULL((SELECT Requisitions_Created FROM [dbo].[SubmissionMetricsView] WHERE CreatedBy = @User AND Period = 'HYTD' AND MetricDate = CAST(GETDATE() AS DATE)), 0) as TOTAL_HYTD_COUNT,
        ISNULL((SELECT Requisitions_Created FROM [dbo].[SubmissionMetricsView] WHERE CreatedBy = @User AND Period = 'YTD' AND MetricDate = CAST(GETDATE() AS DATE)), 0) as TOTAL_YTD_COUNT,
        -- Active requisitions (from materialized view)
        ISNULL((SELECT Active_Requisitions_Updated FROM [dbo].[SubmissionMetricsView] WHERE CreatedBy = @User AND Period = '7D' AND MetricDate = CAST(GETDATE() AS DATE)), 0) as ACTIVE_LAST7D_COUNT,
        ISNULL((SELECT Active_Requisitions_Updated FROM [dbo].[SubmissionMetricsView] WHERE CreatedBy = @User AND Period = 'MTD' AND MetricDate = CAST(GETDATE() AS DATE)), 0) as ACTIVE_MTD_COUNT,
        ISNULL((SELECT Active_Requisitions_Updated FROM [dbo].[SubmissionMetricsView] WHERE CreatedBy = @User AND Period = 'QTD' AND MetricDate = CAST(GETDATE() AS DATE)), 0) as ACTIVE_QTD_COUNT,
        ISNULL((SELECT Active_Requisitions_Updated FROM [dbo].[SubmissionMetricsView] WHERE CreatedBy = @User AND Period = 'HYTD' AND MetricDate = CAST(GETDATE() AS DATE)), 0) as ACTIVE_HYTD_COUNT,
        ISNULL((SELECT Active_Requisitions_Updated FROM [dbo].[SubmissionMetricsView] WHERE CreatedBy = @User AND Period = 'YTD' AND MetricDate = CAST(GETDATE() AS DATE)), 0) as ACTIVE_YTD_COUNT
    FOR JSON PATH;
    
    -- Other result sets using materialized views similarly...
END;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [DashboardAccountsManager_Refactor]
    @User VARCHAR(10) = 'DAVE'
AS
BEGIN
    SET NOCOUNT ON;
    SET ARITHABORT ON;
    SET ANSI_NULLS ON;
    SET QUOTED_IDENTIFIER ON;
    
    -- Set default user if not provided
    IF @User IS NULL
        SET @User = SYSTEM_USER;
    
    DECLARE @Today DATE = GETDATE();
    
    -- Generic Query for Dropdown
    SELECT 
        CreatedBy as KeyValue, FullName as Text
    FROM 
        dbo.ActiveUsersView 
    WHERE 
        CreatedBy = @User
    FOR JSON AUTO;
    
    /***************************************************************************
    * RESULT SET 1: TIME-BOUND METRICS (Refactored)
    * Uses pre-calculated SubmissionMetricsView instead of CTEs/temp tables
    ***************************************************************************/
    
    -- Single query replaces entire #RequisitionMetrics and #SubmissionMetrics logic
    SELECT ISNULL((
        SELECT 
            [CreatedBy] as [User], [Period], [Total_Submissions] as TotalSubmissions, [Active_Requisitions_Updated] as ActiveRequisitions, [INT_Count] as INTSubmissions,
            [OEX_Count] as OEXSubmissions, [HIR_Count] as HIRSubmissions, [OEX_HIR_Ratio] as OEXHIRRatio, [Requisitions_Created] as RequisitionsCreated
        FROM 
            [dbo].[SubmissionMetricsView]
        WHERE 
            [CreatedBy] = @User
        ORDER BY 
            CASE [Period] 
                WHEN '7D' THEN 1 
                WHEN 'MTD' THEN 2 
                WHEN 'QTD' THEN 3 
                WHEN 'HYTD' THEN 4 
                WHEN 'YTD' THEN 5 
            END
        FOR JSON PATH
    ), '[]');

        /***************************************************************************
    * RESULT SET 2: RECENT ACTIVITY REPORT
    * Query 7: 30-Day Submission Activity
    ***************************************************************************/
    --DECLARE @YearStart DATE = DATEADD(YEAR, DATEDIFF(YEAR, 0, @Today), 0);
    --DECLARE @StartDate30 DATE = CASE 
    --    WHEN DATEADD(DAY, -30, @Today) < @YearStart THEN @YearStart
    --    ELSE DATEADD(DAY, -30, @Today)
    --END;

    --WITH CompanySubmissionCounts AS (
    --    -- Get submission counts by company for sorting
    --    SELECT 
    --        C.CompanyName, COUNT(DISTINCT CAST(S.RequisitionId AS VARCHAR(10)) + '-' + CAST(S.CandidateId AS VARCHAR(10))) as SubmissionCount
    --    FROM 
    --        Submissions S INNER JOIN Requisitions R ON R.Id = S.RequisitionId AND R.CreatedBy = @User
    --        INNER JOIN Companies C ON C.ID = R.CompanyId
    --    WHERE 
    --        S.CreatedDate >= @StartDate30 AND S.CreatedDate <= DATEADD(DAY, 1, @Today)
    --    GROUP BY 
    --        C.CompanyName
    --),
    --SubmissionSummary AS (
    --    -- Get first and last activity for each Req+Cand combination
    --    SELECT 
    --        S.RequisitionId, S.CandidateId,
    --        MIN(S.CreatedDate) as DateFirstSubmitted,
    --        MAX(S.CreatedDate) as LastActivityDate
    --    FROM Submissions S
    --    INNER JOIN Requisitions R ON R.Id = S.RequisitionId AND R.CreatedBy = @User
    --    GROUP BY S.RequisitionId, S.CandidateId
    --    HAVING MIN(S.CreatedDate) >= @StartDate30 AND MIN(S.CreatedDate) <= DATEADD(DAY, 1, @Today)
    --),
    --LastActivity AS (
    --    -- Get the status and notes from the most recent submission
    --    SELECT 
    --        S.RequisitionId,
    --        S.CandidateId,
    --        S.Status,
    --        S.Notes,
    --        S.CreatedDate,
    --        ROW_NUMBER() OVER (PARTITION BY S.RequisitionId, S.CandidateId ORDER BY S.CreatedDate DESC) as RN
    --    FROM Submissions S
    --    INNER JOIN SubmissionSummary SS 
    --        ON SS.RequisitionId = S.RequisitionId 
    --        AND SS.CandidateId = S.CandidateId
    --)

    --SELECT @return =
    --(SELECT 
    --    C.CompanyName + ' - [' + R.Code + '] - ' + CAST(R.Positions as varchar(5)) + ' Positions - ' + R.PosTitle as Company,
    --    LTRIM(RTRIM(ISNULL(CAND.FirstName, '') + ' ' + ISNULL(CAND.LastName, ''))) as [CandidateName],
    --    LA.Status as [CurrentStatus],
    --    CAST(SS.DateFirstSubmitted AS DATE) as [DateFirstSubmitted],
    --    CAST(SS.LastActivityDate AS DATE) as [LastActivityDate],
    --    ISNULL(LA.Notes, '') as [ActivityNotes],
    --    @User [User]
    --FROM SubmissionSummary SS
    --INNER JOIN LastActivity LA ON LA.RequisitionId = SS.RequisitionId AND LA.CandidateId = SS.CandidateId AND LA.RN = 1
    --INNER JOIN Requisitions R ON R.Id = SS.RequisitionId
    --INNER JOIN Companies C ON C.ID = R.CompanyId
    --INNER JOIN Candidate CAND ON CAND.ID = SS.CandidateId
    --INNER JOIN CompanySubmissionCounts CSC ON CSC.CompanyName = C.CompanyName
    --ORDER BY 
    --    CSC.SubmissionCount DESC,  -- Companies with more submissions first
    --    C.CompanyName ASC,         -- Alphabetical for ties
    --    R.Id ASC,                  -- RequisitionID within company
    --    SS.DateFirstSubmitted ASC
    --FOR JSON PATH); -- Date First Submitted within requisition
    DECLARE @return varchar(max);
    SELECT @return =
        (SELECT 
            RAV.CompanyName + ' - [' + RAV.RequisitionCode + '] - ' + CAST(RAV.Positions as varchar(5)) + ' Positions - ' + RAV.RequisitionTitle as Company,
            RAV.CandidateName, RAV.CurrentStatus, RAV.DateFirstSubmitted, RAV.LastActivityDate, RAV.ActivityNotes, @User [User]
        FROM 
            [dbo].[RecentActivityView] RAV
        WHERE 
            RAV.CreatedBy = @User
        ORDER BY 
            RAV.SubmissionCount DESC, RAV.CompanyName ASC, RAV.RequisitionCode ASC, RAV.DateFirstSubmitted ASC
        FOR JSON PATH);

    SELECT ISNULL(@return, '[]');

    /***************************************************************************
    * RESULT SET 3: PLACEMENT REPORT
    * Query 8: 3-Month Hire Report
    ***************************************************************************/
    --DECLARE @StartDate90 DATE = CASE 
    --    WHEN DATEADD(MONTH, -3, @Today) < @YearStart THEN @YearStart
    --    ELSE DATEADD(MONTH, -3, @Today)
    --END;

    --WITH HiredCandidates AS (
    --    -- Get the latest HIR record for each Req+Cand combination
    --    SELECT 
    --        S.RequisitionId,
    --        S.CandidateId,
    --        MAX(S.CreatedDate) as DateHired,
    --        ROW_NUMBER() OVER (PARTITION BY S.RequisitionId, S.CandidateId ORDER BY MAX(S.CreatedDate) DESC) as RN
    --    FROM Submissions S
    --    INNER JOIN Requisitions R ON R.Id = S.RequisitionId AND R.CreatedBy = @User
    --    WHERE S.Status = 'HIR'
    --    GROUP BY S.RequisitionId, S.CandidateId
    --    HAVING MAX(S.CreatedDate) >= @StartDate90 AND MAX(S.CreatedDate) <= DATEADD(DAY, 1, @Today)
    --)

    --SELECT @return = 
    --(SELECT 
    --    C.CompanyName as Company,
    --    R.Code as [RequisitionNumber],
    --    R.Positions as [NumPosition],
    --    R.PosTitle as Title,
    --    LTRIM(RTRIM(ISNULL(CAND.FirstName, '') + ' ' + ISNULL(CAND.LastName, ''))) as [CandidateName],
    --    CAST(HC.DateHired AS DATE) as [DateHired],
    --    CAST(0.00 AS DECIMAL(9,2)) as [SalaryOffered],
    --    CAST(GETDATE() AS DATE) as [StartDate],
    --    CAST(GETDATE() AS DATE) as [DateInvoiced],
    --    CAST(GETDATE() AS DATE) as [DatePaid],
    --    CAST(0.00 AS DECIMAL(9,2)) as [PlacementFee],
    --    CAST(0.00 AS DECIMAL(5,2)) as [CommissionPercent],
    --    CAST(0.00 AS DECIMAL(9,2)) as [CommissionEarned],
    --    @User [User]
    --FROM HiredCandidates HC
    --INNER JOIN Requisitions R ON R.Id = HC.RequisitionId
    --INNER JOIN Companies C ON C.ID = R.CompanyId
    --INNER JOIN Candidate CAND ON CAND.ID = HC.CandidateId
    --WHERE HC.RN = 1
    --ORDER BY 
    --    C.CompanyName ASC,
    --    R.Id ASC,
    --    HC.DateHired DESC
    --FOR JSON PATH);
    SELECT @return =
        (SELECT 
            PRV.CompanyName as Company, PRV.RequisitionNumber, PRV.NumPosition, PRV.RequisitionTitle as Title, PRV.CandidateName, PRV.DateHired,
            PRV.SalaryOffered, PRV.StartDate, PRV.DateInvoiced, PRV.DatePaid, PRV.PlacementFee as [Placementfee], PRV.CommissionPercent,
            PRV.CommissionEarned, @User [User]
        FROM [dbo].[PlacementReportView] PRV
        WHERE PRV.CreatedBy = @User
        ORDER BY 
            PRV.CompanyName ASC, PRV.RequisitionNumber ASC, PRV.DateHired DESC
        FOR JSON PATH);

    SELECT ISNULL(@return, '[]');

    /***************************************************************************
    * RESULT SET 5: ACCOUNTS MANAGER TIMING ANALYTICS (Keep Original Logic)
    * Query 1: Time to Fill, Time to Hire, and Time in Stage metrics (By Requisition) 
    * Query 2: Time to Fill, Time to Hire, and Time in Stage metrics (By Company)
    * Shows data for SPECIFIC USER requisitions only (AccountsManager perspective)
    ***************************************************************************/
    
    -- Get requisitions from last year created by this accounts manager
    --DECLARE @StartDate365 DATE = DATEADD(YEAR, -1, @Today);
    
    ---- Create temp tables for accounts manager timing analytics
    --DROP TABLE IF EXISTS #AccountsManagerRequisitionBase;
    --DROP TABLE IF EXISTS #AccountsManagerFirstSubmissions;
    --DROP TABLE IF EXISTS #AccountsManagerTimeToHireCalc;
    --DROP TABLE IF EXISTS #AccountsManagerStageTimings;
    --DROP TABLE IF EXISTS #AccountsManagerStageTimeCalculations;
    
    ---- Temp table 1: Get requisitions created by this accounts manager in last year
    --SELECT 
    --    R.Id as RequisitionId,
    --    R.Code as RequisitionCode,
    --    ISNULL(C.CompanyName, 'Unknown Company') as CompanyName,
    --    R.PosTitle as Title,
    --    R.CreatedDate as RequisitionCreatedDate,
    --    R.CreatedBy,
    --    -- Time to Fill calculation (only for FUL status)
    --    CASE 
    --        WHEN R.Status = 'FUL' THEN DATEDIFF(DAY, R.CreatedDate, R.UpdatedDate)
    --        ELSE NULL 
    --    END as TimeToFill
    --INTO #AccountsManagerRequisitionBase
    --FROM Requisitions R
    --INNER JOIN Companies C ON R.CompanyId = C.ID
    --WHERE R.CreatedBy = @User 
    --    AND CAST(R.CreatedDate AS DATE) >= @StartDate365;
    
    ---- Temp table 2: Get first submission date for each candidate+requisition combo
    --SELECT 
    --    S.RequisitionId,
    --    S.CandidateId,
    --    MIN(S.CreatedDate) as FirstSubmissionDate
    --INTO #AccountsManagerFirstSubmissions
    --FROM Submissions S
    --INNER JOIN #AccountsManagerRequisitionBase AMRB ON S.RequisitionId = AMRB.RequisitionId
    --GROUP BY S.RequisitionId, S.CandidateId;
    
    ---- Temp table 3: Calculate Time to Hire for hired candidates on this AM's requisitions
    --SELECT 
    --    AMFS.RequisitionId,
    --    AMFS.CandidateId,
    --    AMFS.FirstSubmissionDate,
    --    MAX(CASE WHEN S.Status = 'HIR' THEN S.CreatedDate END) as HireDate,
    --    MAX(CASE WHEN S.Status = 'HIR' THEN DATEDIFF(DAY, AMFS.FirstSubmissionDate, S.CreatedDate) END) as TimeToHire
    --INTO #AccountsManagerTimeToHireCalc
    --FROM #AccountsManagerFirstSubmissions AMFS
    --INNER JOIN Submissions S ON AMFS.RequisitionId = S.RequisitionId AND AMFS.CandidateId = S.CandidateId
    --GROUP BY AMFS.RequisitionId, AMFS.CandidateId, AMFS.FirstSubmissionDate;
    
    ---- Temp table 4: Get stage timing for each submission on this AM's requisitions
    --SELECT 
    --    AMFS.RequisitionId,
    --    AMFS.CandidateId,
    --    MAX(CASE WHEN S.Status = 'PEN' THEN S.CreatedDate END) as PEN_Date,
    --    MAX(CASE WHEN S.Status = 'REJ' THEN S.CreatedDate END) as REJ_Date,
    --    MAX(CASE WHEN S.Status = 'HLD' THEN S.CreatedDate END) as HLD_Date,
    --    MAX(CASE WHEN S.Status = 'PHN' THEN S.CreatedDate END) as PHN_Date,
    --    MAX(CASE WHEN S.Status = 'URW' THEN S.CreatedDate END) as URW_Date,
    --    MAX(CASE WHEN S.Status = 'INT' THEN S.CreatedDate END) as INT_Date,
    --    MAX(CASE WHEN S.Status = 'RHM' THEN S.CreatedDate END) as RHM_Date,
    --    MAX(CASE WHEN S.Status = 'DEC' THEN S.CreatedDate END) as DEC_Date,
    --    MAX(CASE WHEN S.Status = 'NOA' THEN S.CreatedDate END) as NOA_Date,
    --    MAX(CASE WHEN S.Status = 'OEX' THEN S.CreatedDate END) as OEX_Date,
    --    MAX(CASE WHEN S.Status = 'ODC' THEN S.CreatedDate END) as ODC_Date,
    --    MAX(CASE WHEN S.Status = 'HIR' THEN S.CreatedDate END) as HIR_Date,
    --    MAX(CASE WHEN S.Status = 'WDR' THEN S.CreatedDate END) as WDR_Date
    --INTO #AccountsManagerStageTimings
    --FROM #AccountsManagerFirstSubmissions AMFS
    --INNER JOIN Submissions S ON AMFS.RequisitionId = S.RequisitionId AND AMFS.CandidateId = S.CandidateId
    --GROUP BY AMFS.RequisitionId, AMFS.CandidateId;
    
    ---- Temp table 5: Calculate days spent in each stage
    --SELECT 
    --    AMST.RequisitionId,
    --    AMST.CandidateId,
    --    CASE 
    --        WHEN AMST.PEN_Date IS NULL THEN 0
    --        WHEN AMST.REJ_Date IS NOT NULL AND AMST.REJ_Date > AMST.PEN_Date THEN DATEDIFF(DAY, AMST.PEN_Date, AMST.REJ_Date)
    --        WHEN AMST.HLD_Date IS NOT NULL AND AMST.HLD_Date > AMST.PEN_Date THEN DATEDIFF(DAY, AMST.PEN_Date, AMST.HLD_Date)
    --        WHEN AMST.PHN_Date IS NOT NULL AND AMST.PHN_Date > AMST.PEN_Date THEN DATEDIFF(DAY, AMST.PEN_Date, AMST.PHN_Date)
    --        ELSE DATEDIFF(DAY, AMST.PEN_Date, @Today)
    --    END as PEN_Days,
        
    --    CASE 
    --        WHEN AMST.REJ_Date IS NULL THEN 0
    --        ELSE 0 -- REJ is final stage
    --    END as REJ_Days,
        
    --    CASE 
    --        WHEN AMST.HLD_Date IS NULL THEN 0
    --        WHEN AMST.PHN_Date IS NOT NULL AND AMST.PHN_Date > AMST.HLD_Date THEN DATEDIFF(DAY, AMST.HLD_Date, AMST.PHN_Date)
    --        WHEN AMST.URW_Date IS NOT NULL AND AMST.URW_Date > AMST.HLD_Date THEN DATEDIFF(DAY, AMST.HLD_Date, AMST.URW_Date)
    --        ELSE DATEDIFF(DAY, AMST.HLD_Date, @Today)
    --    END as HLD_Days,
        
    --    CASE 
    --        WHEN AMST.PHN_Date IS NULL THEN 0
    --        WHEN AMST.URW_Date IS NOT NULL AND AMST.URW_Date > AMST.PHN_Date THEN DATEDIFF(DAY, AMST.PHN_Date, AMST.URW_Date)
    --        WHEN AMST.INT_Date IS NOT NULL AND AMST.INT_Date > AMST.PHN_Date THEN DATEDIFF(DAY, AMST.PHN_Date, AMST.INT_Date)
    --        ELSE DATEDIFF(DAY, AMST.PHN_Date, @Today)
    --    END as PHN_Days,
        
    --    CASE 
    --        WHEN AMST.URW_Date IS NULL THEN 0
    --        WHEN AMST.INT_Date IS NOT NULL AND AMST.INT_Date > AMST.URW_Date THEN DATEDIFF(DAY, AMST.URW_Date, AMST.INT_Date)
    --        WHEN AMST.RHM_Date IS NOT NULL AND AMST.RHM_Date > AMST.URW_Date THEN DATEDIFF(DAY, AMST.URW_Date, AMST.RHM_Date)
    --        ELSE DATEDIFF(DAY, AMST.URW_Date, @Today)
    --    END as URW_Days,
        
    --    CASE 
    --        WHEN AMST.INT_Date IS NULL THEN 0
    --        WHEN AMST.RHM_Date IS NOT NULL AND AMST.RHM_Date > AMST.INT_Date THEN DATEDIFF(DAY, AMST.INT_Date, AMST.RHM_Date)
    --        WHEN AMST.DEC_Date IS NOT NULL AND AMST.DEC_Date > AMST.INT_Date THEN DATEDIFF(DAY, AMST.INT_Date, AMST.DEC_Date)
    --        ELSE DATEDIFF(DAY, AMST.INT_Date, @Today)
    --    END as INT_Days,
        
    --    CASE 
    --        WHEN AMST.RHM_Date IS NULL THEN 0
    --        WHEN AMST.DEC_Date IS NOT NULL AND AMST.DEC_Date > AMST.RHM_Date THEN DATEDIFF(DAY, AMST.RHM_Date, AMST.DEC_Date)
    --        WHEN AMST.NOA_Date IS NOT NULL AND AMST.NOA_Date > AMST.RHM_Date THEN DATEDIFF(DAY, AMST.RHM_Date, AMST.NOA_Date)
    --        ELSE DATEDIFF(DAY, AMST.RHM_Date, @Today)
    --    END as RHM_Days,
        
    --    CASE 
    --        WHEN AMST.DEC_Date IS NULL THEN 0
    --        WHEN AMST.NOA_Date IS NOT NULL AND AMST.NOA_Date > AMST.DEC_Date THEN DATEDIFF(DAY, AMST.DEC_Date, AMST.NOA_Date)
    --        WHEN AMST.OEX_Date IS NOT NULL AND AMST.OEX_Date > AMST.DEC_Date THEN DATEDIFF(DAY, AMST.DEC_Date, AMST.OEX_Date)
    --        ELSE DATEDIFF(DAY, AMST.DEC_Date, @Today)
    --    END as DEC_Days,
        
    --    CASE 
    --        WHEN AMST.NOA_Date IS NULL THEN 0
    --        WHEN AMST.OEX_Date IS NOT NULL AND AMST.OEX_Date > AMST.NOA_Date THEN DATEDIFF(DAY, AMST.NOA_Date, AMST.OEX_Date)
    --        WHEN AMST.ODC_Date IS NOT NULL AND AMST.ODC_Date > AMST.NOA_Date THEN DATEDIFF(DAY, AMST.NOA_Date, AMST.ODC_Date)
    --        ELSE DATEDIFF(DAY, AMST.NOA_Date, @Today)
    --    END as NOA_Days,
        
    --    CASE 
    --        WHEN AMST.OEX_Date IS NULL THEN 0
    --        WHEN AMST.HIR_Date IS NOT NULL AND AMST.HIR_Date > AMST.OEX_Date THEN DATEDIFF(DAY, AMST.OEX_Date, AMST.HIR_Date)
    --        WHEN AMST.ODC_Date IS NOT NULL AND AMST.ODC_Date > AMST.OEX_Date THEN DATEDIFF(DAY, AMST.OEX_Date, AMST.ODC_Date)
    --        WHEN AMST.WDR_Date IS NOT NULL AND AMST.WDR_Date > AMST.OEX_Date THEN DATEDIFF(DAY, AMST.OEX_Date, AMST.WDR_Date)
    --        ELSE DATEDIFF(DAY, AMST.OEX_Date, @Today)
    --    END as OEX_Days,
        
    --    CASE 
    --        WHEN AMST.ODC_Date IS NULL THEN 0
    --        ELSE 0 -- ODC is final stage
    --    END as ODC_Days,
        
    --    CASE 
    --        WHEN AMST.HIR_Date IS NULL THEN 0
    --        ELSE 0 -- HIR is final stage
    --    END as HIR_Days,
        
    --    CASE 
    --        WHEN AMST.WDR_Date IS NULL THEN 0
    --        ELSE 0 -- WDR is final stage
    --    END as WDR_Days
        
    --INTO #AccountsManagerStageTimeCalculations
    --FROM #AccountsManagerStageTimings AMST;
    
    -- Query 1: Timing Analytics by Requisition (AccountsManager view - user filtered)
    --SELECT ISNULL((
    --    SELECT 
    --        @User as [User],
    --        AMRB.RequisitionCode,
    --        AMRB.Title,
    --        -- Average Time to Fill for this requisition (if filled)
    --        ISNULL(CEILING(AVG(CAST(AMRB.TimeToFill as FLOAT))), 0) as TimeToFill_Days,
    --        -- Average Time to Hire for this requisition  
    --        ISNULL(CEILING(AVG(CAST(AMTTH.TimeToHire as FLOAT))), 0) as TimeToHire_Days,
    --        -- Average time in each stage for this requisition (rounded up to next integer)
    --        ISNULL(CEILING(AVG(CAST(AMSTC.PEN_Days as FLOAT))), 0) as PEN_Days,
    --        ISNULL(CEILING(AVG(CAST(AMSTC.REJ_Days as FLOAT))), 0) as REJ_Days,
    --        ISNULL(CEILING(AVG(CAST(AMSTC.HLD_Days as FLOAT))), 0) as HLD_Days,
    --        ISNULL(CEILING(AVG(CAST(AMSTC.PHN_Days as FLOAT))), 0) as PHN_Days,
    --        ISNULL(CEILING(AVG(CAST(AMSTC.URW_Days as FLOAT))), 0) as URW_Days,
    --        ISNULL(CEILING(AVG(CAST(AMSTC.INT_Days as FLOAT))), 0) as INT_Days,
    --        ISNULL(CEILING(AVG(CAST(AMSTC.RHM_Days as FLOAT))), 0) as RHM_Days,
    --        ISNULL(CEILING(AVG(CAST(AMSTC.DEC_Days as FLOAT))), 0) as DEC_Days,
    --        ISNULL(CEILING(AVG(CAST(AMSTC.NOA_Days as FLOAT))), 0) as NOA_Days,
    --        ISNULL(CEILING(AVG(CAST(AMSTC.OEX_Days as FLOAT))), 0) as OEX_Days,
    --        ISNULL(CEILING(AVG(CAST(AMSTC.ODC_Days as FLOAT))), 0) as ODC_Days,
    --        ISNULL(CEILING(AVG(CAST(AMSTC.HIR_Days as FLOAT))), 0) as HIR_Days,
    --        ISNULL(CEILING(AVG(CAST(AMSTC.WDR_Days as FLOAT))), 0) as WDR_Days,
    --        COUNT(DISTINCT CONCAT(AMSTC.RequisitionId, '-', AMSTC.CandidateId)) as TotalCandidates
    --    FROM #AccountsManagerRequisitionBase AMRB
    --    LEFT JOIN #AccountsManagerTimeToHireCalc AMTTH ON AMRB.RequisitionId = AMTTH.RequisitionId
    --    LEFT JOIN #AccountsManagerStageTimeCalculations AMSTC ON AMRB.RequisitionId = AMSTC.RequisitionId
    --    GROUP BY AMRB.RequisitionCode, AMRB.Title
    --    ORDER BY AMRB.RequisitionCode
    --    FOR JSON PATH
    --), '[]');

    SELECT @return = 
        (SELECT 
            @User as [User], TAV.RequisitionCode, TAV.CompanyName, TAV.Title, ISNULL(CEILING(AVG(CAST(TAV.TimeToFill_Days as FLOAT))), 0) as TimeToFill_Days,
            ISNULL(CEILING(AVG(CAST(TAV.TimeToHire_Days as FLOAT))), 0) as TimeToHire_Days, ISNULL(CEILING(AVG(CAST(TAV.PEN_Days as FLOAT))), 0) as PEN_Days,
            ISNULL(CEILING(AVG(CAST(TAV.REJ_Days as FLOAT))), 0) as REJ_Days, ISNULL(CEILING(AVG(CAST(TAV.HLD_Days as FLOAT))), 0) as HLD_Days,
            ISNULL(CEILING(AVG(CAST(TAV.PHN_Days as FLOAT))), 0) as PHN_Days, ISNULL(CEILING(AVG(CAST(TAV.URW_Days as FLOAT))), 0) as URW_Days,
            ISNULL(CEILING(AVG(CAST(TAV.INT_Days as FLOAT))), 0) as INT_Days, ISNULL(CEILING(AVG(CAST(TAV.RHM_Days as FLOAT))), 0) as RHM_Days,
            ISNULL(CEILING(AVG(CAST(TAV.DEC_Days as FLOAT))), 0) as DEC_Days, ISNULL(CEILING(AVG(CAST(TAV.NOA_Days as FLOAT))), 0) as NOA_Days,
            ISNULL(CEILING(AVG(CAST(TAV.OEX_Days as FLOAT))), 0) as OEX_Days, ISNULL(CEILING(AVG(CAST(TAV.ODC_Days as FLOAT))), 0) as ODC_Days,
            ISNULL(CEILING(AVG(CAST(TAV.HIR_Days as FLOAT))), 0) as HIR_Days, ISNULL(CEILING(AVG(CAST(TAV.WDR_Days as FLOAT))), 0) as WDR_Days,
            COUNT(DISTINCT CONCAT(TAV.RequisitionId, '-', TAV.CandidateId)) as TotalCandidates
        FROM 
            [dbo].[TimingAnalyticsView] TAV
        WHERE 
            TAV.CreatedBy = @User AND TAV.Context = 'AM'  -- AM context only
        GROUP BY 
            TAV.RequisitionCode, TAV.CompanyName, TAV.Title
        ORDER BY 
            TAV.RequisitionCode
        FOR JSON PATH);
    
        SELECT ISNULL(@return, '[]');

    -- Query 2: Timing Analytics by Company (AccountsManager view - user filtered)
    --SELECT ISNULL((
    --    SELECT 
    --        @User as [User],
    --        AMRB.CompanyName,
    --        -- Average Time to Fill for this company (if filled)
    --        ISNULL(CEILING(AVG(CAST(AMRB.TimeToFill as FLOAT))), 0) as TimeToFill_Days,
    --        -- Average Time to Hire for this company  
    --        ISNULL(CEILING(AVG(CAST(AMTTH.TimeToHire as FLOAT))), 0) as TimeToHire_Days,
    --        -- Average time in each stage for this company (rounded up to next integer)
    --        ISNULL(CEILING(AVG(CAST(AMSTC.PEN_Days as FLOAT))), 0) as PEN_Days,
    --        ISNULL(CEILING(AVG(CAST(AMSTC.REJ_Days as FLOAT))), 0) as REJ_Days,
    --        ISNULL(CEILING(AVG(CAST(AMSTC.HLD_Days as FLOAT))), 0) as HLD_Days,
    --        ISNULL(CEILING(AVG(CAST(AMSTC.PHN_Days as FLOAT))), 0) as PHN_Days,
    --        ISNULL(CEILING(AVG(CAST(AMSTC.URW_Days as FLOAT))), 0) as URW_Days,
    --        ISNULL(CEILING(AVG(CAST(AMSTC.INT_Days as FLOAT))), 0) as INT_Days,
    --        ISNULL(CEILING(AVG(CAST(AMSTC.RHM_Days as FLOAT))), 0) as RHM_Days,
    --        ISNULL(CEILING(AVG(CAST(AMSTC.DEC_Days as FLOAT))), 0) as DEC_Days,
    --        ISNULL(CEILING(AVG(CAST(AMSTC.NOA_Days as FLOAT))), 0) as NOA_Days,
    --        ISNULL(CEILING(AVG(CAST(AMSTC.OEX_Days as FLOAT))), 0) as OEX_Days,
    --        ISNULL(CEILING(AVG(CAST(AMSTC.ODC_Days as FLOAT))), 0) as ODC_Days,
    --        ISNULL(CEILING(AVG(CAST(AMSTC.HIR_Days as FLOAT))), 0) as HIR_Days,
    --        ISNULL(CEILING(AVG(CAST(AMSTC.WDR_Days as FLOAT))), 0) as WDR_Days,
    --        COUNT(DISTINCT CONCAT(AMSTC.RequisitionId, '-', AMSTC.CandidateId)) as TotalCandidates,
    --        COUNT(DISTINCT AMRB.RequisitionId) as TotalRequisitions
    --    FROM #AccountsManagerRequisitionBase AMRB
    --    LEFT JOIN #AccountsManagerTimeToHireCalc AMTTH ON AMRB.RequisitionId = AMTTH.RequisitionId
    --    LEFT JOIN #AccountsManagerStageTimeCalculations AMSTC ON AMRB.RequisitionId = AMSTC.RequisitionId
    --    GROUP BY AMRB.CompanyName
    --    ORDER BY AMRB.CompanyName
    --    FOR JSON PATH
    --), '[]');

    SELECT @return = 
        (SELECT 
            @User as [User], TAV.CompanyName, ISNULL(CEILING(AVG(CAST(TAV.TimeToFill_Days as FLOAT))), 0) as TimeToFill_Days,
            ISNULL(CEILING(AVG(CAST(TAV.TimeToHire_Days as FLOAT))), 0) as TimeToHire_Days, ISNULL(CEILING(AVG(CAST(TAV.PEN_Days as FLOAT))), 0) as PEN_Days,
            ISNULL(CEILING(AVG(CAST(TAV.REJ_Days as FLOAT))), 0) as REJ_Days, ISNULL(CEILING(AVG(CAST(TAV.HLD_Days as FLOAT))), 0) as HLD_Days,
            ISNULL(CEILING(AVG(CAST(TAV.PHN_Days as FLOAT))), 0) as PHN_Days, ISNULL(CEILING(AVG(CAST(TAV.URW_Days as FLOAT))), 0) as URW_Days,
            ISNULL(CEILING(AVG(CAST(TAV.INT_Days as FLOAT))), 0) as INT_Days, ISNULL(CEILING(AVG(CAST(TAV.RHM_Days as FLOAT))), 0) as RHM_Days,
            ISNULL(CEILING(AVG(CAST(TAV.DEC_Days as FLOAT))), 0) as DEC_Days, ISNULL(CEILING(AVG(CAST(TAV.NOA_Days as FLOAT))), 0) as NOA_Days,
            ISNULL(CEILING(AVG(CAST(TAV.OEX_Days as FLOAT))), 0) as OEX_Days, ISNULL(CEILING(AVG(CAST(TAV.ODC_Days as FLOAT))), 0) as ODC_Days,
            ISNULL(CEILING(AVG(CAST(TAV.HIR_Days as FLOAT))), 0) as HIR_Days, ISNULL(CEILING(AVG(CAST(TAV.WDR_Days as FLOAT))), 0) as WDR_Days,
            COUNT(DISTINCT CONCAT(TAV.RequisitionId, '-', TAV.CandidateId)) as TotalCandidates, COUNT(DISTINCT TAV.RequisitionId) as TotalRequisitions
        FROM 
            [dbo].[TimingAnalyticsView] TAV
        WHERE 
            TAV.CreatedBy = @User AND TAV.Context = 'AM'  -- AM context only
        GROUP BY 
            TAV.CompanyName
        ORDER BY 
            TAV.CompanyName
        FOR JSON PATH);

    SELECT ISNULL(@return, '[]');

    -- Clean up accounts manager timing temp tables
    --DROP TABLE IF EXISTS #AccountsManagerRequisitionBase;
    --DROP TABLE IF EXISTS #AccountsManagerFirstSubmissions;
    --DROP TABLE IF EXISTS #AccountsManagerTimeToHireCalc;
    --DROP TABLE IF EXISTS #AccountsManagerStageTimings;
    --DROP TABLE IF EXISTS #AccountsManagerStageTimeCalculations;

    /***************************************************************************
    * RESULT SET 6: CANDIDATE QUALITY & CONVERSION METRICS
    * PEN-to-Interview, Interview-to-Offer, Offer-to-Acceptance Ratios
    ***************************************************************************/
    
    SELECT ISNULL((
        SELECT 
            [CreatedBy] as [User], [Period], 
            [PEN_to_INT_Ratio] as [PEN_to_Interview_Ratio],
            [INT_to_OEX_Ratio] as [Interview_to_Offer_Ratio], 
            [OEX_to_HIR_Ratio] as [Offer_to_Acceptance_Ratio],
            [Total_Submissions], [Reached_INT], [Reached_OEX], [Reached_HIR]
        FROM 
            [dbo].[CandidateQualityMetricsView]
        WHERE 
            [CreatedBy] = @User
        ORDER BY 
            CASE [Period] 
                WHEN '7D' THEN 1 
                WHEN 'MTD' THEN 2 
                WHEN 'QTD' THEN 3 
                WHEN 'HYTD' THEN 4 
                WHEN 'YTD' THEN 5 
            END
        FOR JSON PATH
    ), '[]');
    
END;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [DashboardAdmin]
AS
BEGIN
	SET NOCOUNT ON;
	SET ARITHABORT ON;
	SET ANSI_NULLS ON;
	SET QUOTED_IDENTIFIER ON;

    --For Checking the Query execution time.
    DECLARE @StartTime DATETIME2 = SYSDATETIME();

    DECLARE @Today DATE = '2025-06-30';--GETDATE();
    DECLARE @YearStart DATE = DATEFROMPARTS(YEAR(@Today), 1, 1);
    
    -- Generic Query for Dropdown (Memory-Optimized)
    SELECT 
        CreatedBy as KeyValue, FullName as Text
    FROM 
        dbo.ActiveUsersView 
    WHERE 
        Role = 5 AND Status = 1
    FOR JSON AUTO;

    -- Common date calculations
    DECLARE @StartDate7D DATE = DATEADD(DAY, -6, @Today);
    DECLARE @StartDateMTD DATE = DATEADD(DAY, 1, EOMONTH(@Today, -1));
    DECLARE @StartDateQTD DATE = DATEADD(QUARTER, DATEDIFF(QUARTER, 0, @Today), 0);
    DECLARE @StartDateHYTD DATE = CASE 
        WHEN MONTH(@Today) <= 6 THEN DATEFROMPARTS(YEAR(@Today), 1, 1)
        ELSE DATEFROMPARTS(YEAR(@Today), 7, 1)
    END;
    
    -- Query 1: Requisitions Created
    SELECT 
        CreatedBy as [User],
        MAX(CASE WHEN Period = '7D' THEN Requisitions_Created ELSE 0 END) as LAST7D_COUNT,
        MAX(CASE WHEN Period = 'MTD' THEN Requisitions_Created ELSE 0 END) as MTD_COUNT,
        MAX(CASE WHEN Period = 'QTD' THEN Requisitions_Created ELSE 0 END) as QTD_COUNT,
        MAX(CASE WHEN Period = 'HYTD' THEN Requisitions_Created ELSE 0 END) as HYTD_COUNT,
        MAX(CASE WHEN Period = 'YTD' THEN Requisitions_Created ELSE 0 END) as YTD_COUNT
    FROM dbo.SubmissionMetricsView WHERE MetricDate = @Today
    GROUP BY CreatedBy ORDER BY CreatedBy FOR JSON PATH;

    -- Query 2: Active Requisitions
    SELECT 
        CreatedBy as [User],
        MAX(CASE WHEN Period = '7D' THEN Active_Requisitions_Updated ELSE 0 END) as LAST7D_COUNT,
        MAX(CASE WHEN Period = 'MTD' THEN Active_Requisitions_Updated ELSE 0 END) as MTD_COUNT,
        MAX(CASE WHEN Period = 'QTD' THEN Active_Requisitions_Updated ELSE 0 END) as QTD_COUNT,
        MAX(CASE WHEN Period = 'HYTD' THEN Active_Requisitions_Updated ELSE 0 END) as HYTD_COUNT,
        MAX(CASE WHEN Period = 'YTD' THEN Active_Requisitions_Updated ELSE 0 END) as YTD_COUNT
    FROM dbo.SubmissionMetricsView WHERE MetricDate = @Today
    GROUP BY CreatedBy ORDER BY CreatedBy FOR JSON PATH;

    -- Query 3: Interviews (INT)
    SELECT 
        CreatedBy as [User],
        MAX(CASE WHEN Period = '7D' THEN INT_Count ELSE 0 END) as LAST7D_COUNT,
        MAX(CASE WHEN Period = 'MTD' THEN INT_Count ELSE 0 END) as MTD_COUNT,
        MAX(CASE WHEN Period = 'QTD' THEN INT_Count ELSE 0 END) as QTD_COUNT,
        MAX(CASE WHEN Period = 'HYTD' THEN INT_Count ELSE 0 END) as HYTD_COUNT,
        MAX(CASE WHEN Period = 'YTD' THEN INT_Count ELSE 0 END) as YTD_COUNT
    FROM dbo.SubmissionMetricsView WHERE MetricDate = @Today
    GROUP BY CreatedBy ORDER BY CreatedBy FOR JSON PATH;

    -- Query 4: Offers Extended (OEX)
    SELECT 
        CreatedBy as [User],
        MAX(CASE WHEN Period = '7D' THEN OEX_Count ELSE 0 END) as LAST7D_COUNT,
        MAX(CASE WHEN Period = 'MTD' THEN OEX_Count ELSE 0 END) as MTD_COUNT,
        MAX(CASE WHEN Period = 'QTD' THEN OEX_Count ELSE 0 END) as QTD_COUNT,
        MAX(CASE WHEN Period = 'HYTD' THEN OEX_Count ELSE 0 END) as HYTD_COUNT,
        MAX(CASE WHEN Period = 'YTD' THEN OEX_Count ELSE 0 END) as YTD_COUNT
    FROM dbo.SubmissionMetricsView WHERE MetricDate = @Today
    GROUP BY CreatedBy ORDER BY CreatedBy FOR JSON PATH;

    -- Query 5: Hired (HIR)
    SELECT 
        CreatedBy as [User],
        MAX(CASE WHEN Period = '7D' THEN HIR_Count ELSE 0 END) as LAST7D_COUNT,
        MAX(CASE WHEN Period = 'MTD' THEN HIR_Count ELSE 0 END) as MTD_COUNT,
        MAX(CASE WHEN Period = 'QTD' THEN HIR_Count ELSE 0 END) as QTD_COUNT,
        MAX(CASE WHEN Period = 'HYTD' THEN HIR_Count ELSE 0 END) as HYTD_COUNT,
        MAX(CASE WHEN Period = 'YTD' THEN HIR_Count ELSE 0 END) as YTD_COUNT
    FROM dbo.SubmissionMetricsView WHERE MetricDate = @Today
    GROUP BY CreatedBy ORDER BY CreatedBy FOR JSON PATH;

    -- Query 6: OEX:HIR Ratios
    SELECT 
        CreatedBy as [User],
        MAX(CASE WHEN Period = '7D' THEN OEX_HIR_Ratio ELSE 0.00 END) as LAST7D_RATIO,
        MAX(CASE WHEN Period = 'MTD' THEN OEX_HIR_Ratio ELSE 0.00 END) as MTD_RATIO,
        MAX(CASE WHEN Period = 'QTD' THEN OEX_HIR_Ratio ELSE 0.00 END) as QTD_RATIO,
        MAX(CASE WHEN Period = 'HYTD' THEN OEX_HIR_Ratio ELSE 0.00 END) as HYTD_RATIO,
        MAX(CASE WHEN Period = 'YTD' THEN OEX_HIR_Ratio ELSE 0.00 END) as YTD_RATIO
    FROM dbo.SubmissionMetricsView WHERE MetricDate = @Today
    GROUP BY CreatedBy ORDER BY CreatedBy FOR JSON PATH;

    -- SET 2 - Query 1: Company Submission Activity
    DECLARE @StartDateSet2 DATE = CASE 
        WHEN DATEADD(DAY, -30, @Today) < DATEFROMPARTS(YEAR(@Today), 1, 1) 
        THEN DATEFROMPARTS(YEAR(@Today), 1, 1)
        ELSE DATEADD(DAY, -30, @Today)
    END;
    DECLARE @return varchar(max);
    
    WITH SubmissionSummary AS (
        SELECT 
            S.RequisitionId,
            S.CandidateId,
            MIN(S.CreatedDate) as DateFirstSubmitted,
            MAX(S.CreatedDate) as LastActivityDate
        FROM dbo.Submissions S
        INNER JOIN dbo.Requisitions R ON S.RequisitionId = R.Id
        GROUP BY S.RequisitionId, S.CandidateId
        HAVING MIN(S.CreatedDate) >= @StartDateSet2
    ),
    CurrentStatus AS (
        SELECT 
            SS.RequisitionId,
            SS.CandidateId,
            SS.DateFirstSubmitted,
            SS.LastActivityDate,
            S.Status as CurrentStatus,
            S.Notes as ActivityNotes
        FROM SubmissionSummary SS
        INNER JOIN dbo.Submissions S ON S.RequisitionId = SS.RequisitionId 
            AND S.CandidateId = SS.CandidateId 
            AND S.CreatedDate = SS.LastActivityDate
    ),
    CompanySubmissionCounts AS (
        SELECT 
            C.CompanyName,
            COUNT(*) as SubmissionCount
        FROM CurrentStatus CS
        INNER JOIN dbo.Requisitions R ON CS.RequisitionId = R.Id
        INNER JOIN dbo.Companies C ON R.CompanyId = C.ID
        GROUP BY C.CompanyName
    )

    SELECT @return = 
    (SELECT 
        C.CompanyName + ' - [' + R.Code + '] - ' + CAST(R.Positions as varchar(5)) + ' Positions - ' + R.PosTitle as Company,
        LTRIM(RTRIM(ISNULL(CAND.FirstName, '') + ' ' + ISNULL(CAND.LastName, ''))) as [CandidateName],
        CS.CurrentStatus as [CurrentStatus],
        CS.DateFirstSubmitted as [DateFirstSubmitted],
        CS.LastActivityDate as [LastActivityDate],
        ISNULL(CS.ActivityNotes, '') as [ActivityNotes],
        R.CreatedBy [User]
    FROM CurrentStatus CS
    INNER JOIN dbo.Requisitions R ON CS.RequisitionId = R.Id
    INNER JOIN dbo.Companies C ON R.CompanyId = C.ID
    INNER JOIN dbo.Candidate CAND ON CS.CandidateId = CAND.ID
    INNER JOIN CompanySubmissionCounts CSC ON C.CompanyName = CSC.CompanyName
    ORDER BY 
        CSC.SubmissionCount DESC,
        C.CompanyName ASC,
        R.Id ASC,
        CS.DateFirstSubmitted ASC
    FOR JSON PATH);

    SELECT @return;
    
    -- SET 3 - Query 1: Hired Candidates Report
    DECLARE @StartDateSet3 DATE = DATEADD(MONTH, -3, @Today);
    
    WITH HiredCandidates AS (
        SELECT 
            S.RequisitionId,
            S.CandidateId,
            MAX(S.CreatedDate) as DateHired
        FROM dbo.Submissions S
        INNER JOIN dbo.Requisitions R ON S.RequisitionId = R.Id
        WHERE S.Status = 'HIR'
        GROUP BY S.RequisitionId, S.CandidateId, R.CreatedBy
        HAVING MAX(S.CreatedDate) >= @StartDateSet3
    )

    SELECT @return =
    (SELECT 
        C.CompanyName as Company,
        R.Code as [RequisitionNumber],
        R.Positions as [NumPosition],
        R.PosTitle as Title,
        LTRIM(RTRIM(ISNULL(CAND.FirstName, '') + ' ' + ISNULL(CAND.LastName, ''))) as [CandidateName],
        HC.DateHired as [DateHired],
        CAST(0.00 AS DECIMAL(9,2)) as [SalaryOffered],
        @Today as [StartDate],
        @Today as [DateInvoiced],
        @Today as [DatePaid],
        CAST(0.00 AS DECIMAL(9,2)) as [Placementfee],
        CAST(0.00 AS DECIMAL(5,2)) as [CommissionPercent],
        CAST(0.00 AS DECIMAL(9,2)) as [CommissionEarned],
        R.CreatedBy [User]
    FROM HiredCandidates HC
    INNER JOIN dbo.Requisitions R ON HC.RequisitionId = R.Id
    INNER JOIN dbo.Companies C ON R.CompanyId = C.ID
    INNER JOIN dbo.Candidate CAND ON HC.CandidateId = CAND.ID
    ORDER BY 
        R.CreatedBy,
        C.CompanyName,
        R.Code,
        HC.DateHired
    FOR JSON PATH);
    
    SELECT @return
    -- Clean up
    --DROP TABLE #SubmissionData;
    /***************************************************************************
    * RESULT SET 4: ADMIN TIMING ANALYTICS (2 Queries)
    * Query 1: Time to Fill, Time to Hire, and Time in Stage metrics (By Requisition) 
    * Query 2: Time to Fill, Time to Hire, and Time in Stage metrics (By Company)
    * Shows data for ALL USERS (Admin perspective)
    ***************************************************************************/
    
    -- Get requisitions from last year for all users
    DECLARE @StartDate365 DATE = DATEADD(YEAR, -1, @Today);
    
    -- Create temp tables for admin timing analytics
    DROP TABLE IF EXISTS #AdminRequisitionBase;
    DROP TABLE IF EXISTS #AdminFirstSubmissions;
    DROP TABLE IF EXISTS #AdminTimeToHireCalc;
    DROP TABLE IF EXISTS #AdminStageTimings;
    DROP TABLE IF EXISTS #AdminStageTimeCalculations;
    
    -- Temp table 1: Get requisitions created by ALL USERS in last year
    SELECT 
        R.Id as RequisitionId,
        R.Code as RequisitionCode,
        ISNULL(C.CompanyName, 'Unknown Company') as CompanyName,
        R.PosTitle as Title,
        R.CreatedDate as RequisitionCreatedDate,
        R.CreatedBy,
        -- Time to Fill calculation (only for FUL status)
        CASE 
            WHEN R.Status = 'FUL' THEN DATEDIFF(DAY, R.CreatedDate, R.UpdatedDate)
            ELSE NULL 
        END as TimeToFill
    INTO #AdminRequisitionBase
    FROM Requisitions R
    LEFT JOIN Companies C ON R.CompanyId = C.ID
    INNER JOIN dbo.Users U ON R.CreatedBy = U.UserName AND U.Status = 1
    WHERE CAST(R.CreatedDate AS DATE) >= @StartDate365
        AND R.CreatedBy IS NOT NULL;
    
    -- Temp table 2: Get first submission date for each candidate+requisition combo (all users)
    SELECT 
        S.RequisitionId,
        S.CandidateId,
        MIN(S.CreatedDate) as FirstSubmissionDate
    INTO #AdminFirstSubmissions
    FROM Submissions S
    INNER JOIN #AdminRequisitionBase ARB ON S.RequisitionId = ARB.RequisitionId
    GROUP BY S.RequisitionId, S.CandidateId;
    
    -- Temp table 3: Calculate Time to Hire for hired candidates (all users)
    SELECT 
        AFS.RequisitionId,
        AFS.CandidateId,
        AFS.FirstSubmissionDate,
        MAX(CASE WHEN S.Status = 'HIR' THEN S.CreatedDate END) as HireDate,
        MAX(CASE WHEN S.Status = 'HIR' THEN DATEDIFF(DAY, AFS.FirstSubmissionDate, S.CreatedDate) END) as TimeToHire
    INTO #AdminTimeToHireCalc
    FROM #AdminFirstSubmissions AFS
    INNER JOIN Submissions S ON AFS.RequisitionId = S.RequisitionId AND AFS.CandidateId = S.CandidateId
    GROUP BY AFS.RequisitionId, AFS.CandidateId, AFS.FirstSubmissionDate;
    
    -- Temp table 4: Calculate time in each stage for all users
    SELECT 
        AFS.RequisitionId,
        AFS.CandidateId,
        -- Get the latest date for each stage (in case of multiple records for same stage)
        MAX(CASE WHEN S.Status = 'PEN' THEN S.CreatedDate END) as PEN_Date,
        MAX(CASE WHEN S.Status = 'REJ' THEN S.CreatedDate END) as REJ_Date,
        MAX(CASE WHEN S.Status = 'HLD' THEN S.CreatedDate END) as HLD_Date,
        MAX(CASE WHEN S.Status = 'PHN' THEN S.CreatedDate END) as PHN_Date,
        MAX(CASE WHEN S.Status = 'URW' THEN S.CreatedDate END) as URW_Date,
        MAX(CASE WHEN S.Status = 'INT' THEN S.CreatedDate END) as INT_Date,
        MAX(CASE WHEN S.Status = 'RHM' THEN S.CreatedDate END) as RHM_Date,
        MAX(CASE WHEN S.Status = 'DEC' THEN S.CreatedDate END) as DEC_Date,
        MAX(CASE WHEN S.Status = 'NOA' THEN S.CreatedDate END) as NOA_Date,
        MAX(CASE WHEN S.Status = 'OEX' THEN S.CreatedDate END) as OEX_Date,
        MAX(CASE WHEN S.Status = 'ODC' THEN S.CreatedDate END) as ODC_Date,
        MAX(CASE WHEN S.Status = 'HIR' THEN S.CreatedDate END) as HIR_Date,
        MAX(CASE WHEN S.Status = 'WDR' THEN S.CreatedDate END) as WDR_Date
    INTO #AdminStageTimings
    FROM #AdminFirstSubmissions AFS
    INNER JOIN Submissions S ON AFS.RequisitionId = S.RequisitionId AND AFS.CandidateId = S.CandidateId
    GROUP BY AFS.RequisitionId, AFS.CandidateId;
    
    -- Temp table 5: Calculate time spent in each stage for all users
    SELECT 
        AST.RequisitionId,
        AST.CandidateId,
        
        CASE 
            WHEN AST.PEN_Date IS NULL THEN 0
            WHEN AST.REJ_Date IS NOT NULL THEN DATEDIFF(DAY, AST.PEN_Date, AST.REJ_Date)
            WHEN AST.HLD_Date IS NOT NULL THEN DATEDIFF(DAY, AST.PEN_Date, AST.HLD_Date)
            WHEN AST.PHN_Date IS NOT NULL THEN DATEDIFF(DAY, AST.PEN_Date, AST.PHN_Date)
            ELSE DATEDIFF(DAY, AST.PEN_Date, @Today)
        END as PEN_Days,
        
        CASE 
            WHEN AST.REJ_Date IS NULL THEN 0
            ELSE 0 -- REJ is final stage
        END as REJ_Days,
        
        CASE 
            WHEN AST.HLD_Date IS NULL THEN 0
            WHEN AST.PHN_Date IS NOT NULL THEN DATEDIFF(DAY, AST.HLD_Date, AST.PHN_Date)
            WHEN AST.URW_Date IS NOT NULL THEN DATEDIFF(DAY, AST.HLD_Date, AST.URW_Date)
            ELSE DATEDIFF(DAY, AST.HLD_Date, @Today)
        END as HLD_Days,
        
        CASE 
            WHEN AST.PHN_Date IS NULL THEN 0
            WHEN AST.URW_Date IS NOT NULL THEN DATEDIFF(DAY, AST.PHN_Date, AST.URW_Date)
            WHEN AST.INT_Date IS NOT NULL THEN DATEDIFF(DAY, AST.PHN_Date, AST.INT_Date)
            ELSE DATEDIFF(DAY, AST.PHN_Date, @Today)
        END as PHN_Days,
        
        CASE 
            WHEN AST.URW_Date IS NULL THEN 0
            WHEN AST.INT_Date IS NOT NULL THEN DATEDIFF(DAY, AST.URW_Date, AST.INT_Date)
            ELSE DATEDIFF(DAY, AST.URW_Date, @Today)
        END as URW_Days,
        
        CASE 
            WHEN AST.INT_Date IS NULL THEN 0
            WHEN AST.RHM_Date IS NOT NULL THEN DATEDIFF(DAY, AST.INT_Date, AST.RHM_Date)
            WHEN AST.DEC_Date IS NOT NULL THEN DATEDIFF(DAY, AST.INT_Date, AST.DEC_Date)
            ELSE DATEDIFF(DAY, AST.INT_Date, @Today)
        END as INT_Days,
        
        CASE 
            WHEN AST.RHM_Date IS NULL THEN 0
            WHEN AST.DEC_Date IS NOT NULL THEN DATEDIFF(DAY, AST.RHM_Date, AST.DEC_Date)
            ELSE DATEDIFF(DAY, AST.RHM_Date, @Today)
        END as RHM_Days,
        
        CASE 
            WHEN AST.DEC_Date IS NULL THEN 0
            WHEN AST.NOA_Date IS NOT NULL THEN DATEDIFF(DAY, AST.DEC_Date, AST.NOA_Date)
            WHEN AST.OEX_Date IS NOT NULL THEN DATEDIFF(DAY, AST.DEC_Date, AST.OEX_Date)
            ELSE DATEDIFF(DAY, AST.DEC_Date, @Today)
        END as DEC_Days,
        
        CASE 
            WHEN AST.NOA_Date IS NULL THEN 0
            WHEN AST.OEX_Date IS NOT NULL THEN DATEDIFF(DAY, AST.NOA_Date, AST.OEX_Date)
            ELSE DATEDIFF(DAY, AST.NOA_Date, @Today)
        END as NOA_Days,
        
        CASE 
            WHEN AST.OEX_Date IS NULL THEN 0
            WHEN AST.ODC_Date IS NOT NULL THEN DATEDIFF(DAY, AST.OEX_Date, AST.ODC_Date)
            WHEN AST.HIR_Date IS NOT NULL THEN DATEDIFF(DAY, AST.OEX_Date, AST.HIR_Date)
            WHEN AST.WDR_Date IS NOT NULL THEN DATEDIFF(DAY, AST.OEX_Date, AST.WDR_Date)
            ELSE DATEDIFF(DAY, AST.OEX_Date, @Today)
        END as OEX_Days,
        
        CASE 
            WHEN AST.ODC_Date IS NULL THEN 0
            ELSE 0 -- ODC is final stage
        END as ODC_Days,
        
        CASE 
            WHEN AST.HIR_Date IS NULL THEN 0
            ELSE 0 -- HIR is final stage
        END as HIR_Days,
        
        CASE 
            WHEN AST.WDR_Date IS NULL THEN 0
            ELSE 0 -- WDR is final stage
        END as WDR_Days
        
    INTO #AdminStageTimeCalculations
    FROM #AdminStageTimings AST;
    
    -- Query 1: Timing Analytics by Requisition (Admin view - all users)
    SELECT 
        'ALL_USERS' as [User],
        ARB.RequisitionCode,
        ARB.Title,
        ARB.CreatedBy as RequisitionOwner,
        -- Average Time to Fill for this requisition (if filled)
        ISNULL(CEILING(AVG(CAST(ARB.TimeToFill as FLOAT))), 0) as TimeToFill_Days,
        -- Average Time to Hire for this requisition  
        ISNULL(CEILING(AVG(CAST(ATTH.TimeToHire as FLOAT))), 0) as TimeToHire_Days,
        -- Average time in each stage for this requisition (rounded up to next integer)
        ISNULL(CEILING(AVG(CAST(ASTC.PEN_Days as FLOAT))), 0) as PEN_Days,
        ISNULL(CEILING(AVG(CAST(ASTC.REJ_Days as FLOAT))), 0) as REJ_Days,
        ISNULL(CEILING(AVG(CAST(ASTC.HLD_Days as FLOAT))), 0) as HLD_Days,
        ISNULL(CEILING(AVG(CAST(ASTC.PHN_Days as FLOAT))), 0) as PHN_Days,
        ISNULL(CEILING(AVG(CAST(ASTC.URW_Days as FLOAT))), 0) as URW_Days,
        ISNULL(CEILING(AVG(CAST(ASTC.INT_Days as FLOAT))), 0) as INT_Days,
        ISNULL(CEILING(AVG(CAST(ASTC.RHM_Days as FLOAT))), 0) as RHM_Days,
        ISNULL(CEILING(AVG(CAST(ASTC.DEC_Days as FLOAT))), 0) as DEC_Days,
        ISNULL(CEILING(AVG(CAST(ASTC.NOA_Days as FLOAT))), 0) as NOA_Days,
        ISNULL(CEILING(AVG(CAST(ASTC.OEX_Days as FLOAT))), 0) as OEX_Days,
        ISNULL(CEILING(AVG(CAST(ASTC.ODC_Days as FLOAT))), 0) as ODC_Days,
        ISNULL(CEILING(AVG(CAST(ASTC.HIR_Days as FLOAT))), 0) as HIR_Days,
        ISNULL(CEILING(AVG(CAST(ASTC.WDR_Days as FLOAT))), 0) as WDR_Days,
        COUNT(DISTINCT CONCAT(ASTC.RequisitionId, '-', ASTC.CandidateId)) as TotalCandidates
    FROM #AdminRequisitionBase ARB
    LEFT JOIN #AdminTimeToHireCalc ATTH ON ARB.RequisitionId = ATTH.RequisitionId
    LEFT JOIN #AdminStageTimeCalculations ASTC ON ARB.RequisitionId = ASTC.RequisitionId
    GROUP BY ARB.RequisitionCode, ARB.Title, ARB.CreatedBy
    ORDER BY ARB.RequisitionCode
    FOR JSON PATH;
    
    -- Query 2: Timing Analytics by Company (Admin view - all users)
    SELECT 
        'ALL_USERS' as [User],
        ARB.CompanyName,
        -- Average Time to Fill for this company (if filled)
        ISNULL(CEILING(AVG(CAST(ARB.TimeToFill as FLOAT))), 0) as TimeToFill_Days,
        -- Average Time to Hire for this company  
        ISNULL(CEILING(AVG(CAST(ATTH.TimeToHire as FLOAT))), 0) as TimeToHire_Days,
        -- Average time in each stage for this company (rounded up to next integer)
        ISNULL(CEILING(AVG(CAST(ASTC.PEN_Days as FLOAT))), 0) as PEN_Days,
        ISNULL(CEILING(AVG(CAST(ASTC.REJ_Days as FLOAT))), 0) as REJ_Days,
        ISNULL(CEILING(AVG(CAST(ASTC.HLD_Days as FLOAT))), 0) as HLD_Days,
        ISNULL(CEILING(AVG(CAST(ASTC.PHN_Days as FLOAT))), 0) as PHN_Days,
        ISNULL(CEILING(AVG(CAST(ASTC.URW_Days as FLOAT))), 0) as URW_Days,
        ISNULL(CEILING(AVG(CAST(ASTC.INT_Days as FLOAT))), 0) as INT_Days,
        ISNULL(CEILING(AVG(CAST(ASTC.RHM_Days as FLOAT))), 0) as RHM_Days,
        ISNULL(CEILING(AVG(CAST(ASTC.DEC_Days as FLOAT))), 0) as DEC_Days,
        ISNULL(CEILING(AVG(CAST(ASTC.NOA_Days as FLOAT))), 0) as NOA_Days,
        ISNULL(CEILING(AVG(CAST(ASTC.OEX_Days as FLOAT))), 0) as OEX_Days,
        ISNULL(CEILING(AVG(CAST(ASTC.ODC_Days as FLOAT))), 0) as ODC_Days,
        ISNULL(CEILING(AVG(CAST(ASTC.HIR_Days as FLOAT))), 0) as HIR_Days,
        ISNULL(CEILING(AVG(CAST(ASTC.WDR_Days as FLOAT))), 0) as WDR_Days,
        COUNT(DISTINCT CONCAT(ASTC.RequisitionId, '-', ASTC.CandidateId)) as TotalCandidates,
        COUNT(DISTINCT ARB.RequisitionId) as TotalRequisitions,
        COUNT(DISTINCT ARB.CreatedBy) as TotalRecruiters
    FROM #AdminRequisitionBase ARB
    LEFT JOIN #AdminTimeToHireCalc ATTH ON ARB.RequisitionId = ATTH.RequisitionId
    LEFT JOIN #AdminStageTimeCalculations ASTC ON ARB.RequisitionId = ASTC.RequisitionId
    GROUP BY ARB.CompanyName
    ORDER BY ARB.CompanyName
    FOR JSON PATH;

    -- Clean up admin timing temp tables
    DROP TABLE IF EXISTS #AdminRequisitionBase;
    DROP TABLE IF EXISTS #AdminFirstSubmissions;
    DROP TABLE IF EXISTS #AdminTimeToHireCalc;
    DROP TABLE IF EXISTS #AdminStageTimings;
    DROP TABLE IF EXISTS #AdminStageTimeCalculations;
    
    -- Clean up existing temp tables (from current DashboardAdmin)
    DROP TABLE IF EXISTS #RequisitionMetrics;
    DROP TABLE IF EXISTS #SubmissionMetrics;    

    --For checking the query execution time.
    DECLARE @EndTime DATETIME2 = SYSDATETIME();
    DECLARE @TotalDuration_MS INT = DATEDIFF(MILLISECOND, @StartTime, @EndTime);
    
    -- This will show in Messages tab regardless of Results to Grid/Text
    RAISERROR('Total SP execution time: %d ms', 0, 1, @TotalDuration_MS) WITH NOWAIT;

END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [DashboardAdmin_Optimized]
AS
BEGIN
    SET NOCOUNT ON;
    SET ARITHABORT ON;
    SET ANSI_NULLS ON;
    SET QUOTED_IDENTIFIER ON;
    
    DECLARE @Today DATE = CAST(GETDATE() AS DATE);
    
    -- Generic Query for Dropdown
    SELECT UserName as KeyValue, FirstName + ' ' + LastName as Text
    FROM dbo.Users 
    WHERE Role = 5 AND Status = 1
    FOR JSON AUTO;
    
    -- =====================================================
    -- SET 1 - Requisitions Created by All Users (Using Materialized View)
    -- =====================================================
    SELECT 
        AUV.CreatedBy as [User],
        ISNULL((SELECT SMV.Requisitions_Created 
                FROM [dbo].[SubmissionMetricsView] SMV 
                WHERE SMV.CreatedBy = AUV.CreatedBy AND SMV.Period = '7D' AND SMV.MetricDate = @Today), 0) as LAST7D_COUNT,
        ISNULL((SELECT SMV.Requisitions_Created 
                FROM [dbo].[SubmissionMetricsView] SMV 
                WHERE SMV.CreatedBy = AUV.CreatedBy AND SMV.Period = 'MTD' AND SMV.MetricDate = @Today), 0) as MTD_COUNT,
        ISNULL((SELECT SMV.Requisitions_Created 
                FROM [dbo].[SubmissionMetricsView] SMV 
                WHERE SMV.CreatedBy = AUV.CreatedBy AND SMV.Period = 'QTD' AND SMV.MetricDate = @Today), 0) as QTD_COUNT,
        ISNULL((SELECT SMV.Requisitions_Created 
                FROM [dbo].[SubmissionMetricsView] SMV 
                WHERE SMV.CreatedBy = AUV.CreatedBy AND SMV.Period = 'HYTD' AND SMV.MetricDate = @Today), 0) as HYTD_COUNT,
        ISNULL((SELECT SMV.Requisitions_Created 
                FROM [dbo].[SubmissionMetricsView] SMV 
                WHERE SMV.CreatedBy = AUV.CreatedBy AND SMV.Period = 'YTD' AND SMV.MetricDate = @Today), 0) as YTD_COUNT
    FROM [dbo].[ActiveUsersView] AUV
    WHERE AUV.Status = 1
    ORDER BY AUV.CreatedBy
    FOR JSON PATH;
    
    -- =====================================================
    -- SET 2 - Total Submissions by All Users (Using Materialized View)
    -- =====================================================
    SELECT 
        AUV.CreatedBy as [User],
        ISNULL((SELECT SMV.Total_Submissions 
                FROM [dbo].[SubmissionMetricsView] SMV 
                WHERE SMV.CreatedBy = AUV.CreatedBy AND SMV.Period = '7D' AND SMV.MetricDate = @Today), 0) as LAST7D_COUNT,
        ISNULL((SELECT SMV.Total_Submissions 
                FROM [dbo].[SubmissionMetricsView] SMV 
                WHERE SMV.CreatedBy = AUV.CreatedBy AND SMV.Period = 'MTD' AND SMV.MetricDate = @Today), 0) as MTD_COUNT,
        ISNULL((SELECT SMV.Total_Submissions 
                FROM [dbo].[SubmissionMetricsView] SMV 
                WHERE SMV.CreatedBy = AUV.CreatedBy AND SMV.Period = 'QTD' AND SMV.MetricDate = @Today), 0) as QTD_COUNT,
        ISNULL((SELECT SMV.Total_Submissions 
                FROM [dbo].[SubmissionMetricsView] SMV 
                WHERE SMV.CreatedBy = AUV.CreatedBy AND SMV.Period = 'HYTD' AND SMV.MetricDate = @Today), 0) as HYTD_COUNT,
        ISNULL((SELECT SMV.Total_Submissions 
                FROM [dbo].[SubmissionMetricsView] SMV 
                WHERE SMV.CreatedBy = AUV.CreatedBy AND SMV.Period = 'YTD' AND SMV.MetricDate = @Today), 0) as YTD_COUNT
    FROM [dbo].[ActiveUsersView] AUV
    WHERE AUV.Status = 1
    ORDER BY AUV.CreatedBy
    FOR JSON PATH;
    
    -- =====================================================
    -- SET 3 - INT Submissions by All Users (Using Materialized View)
    -- =====================================================
    SELECT 
        AUV.CreatedBy as [User],
        ISNULL((SELECT SMV.INT_Count 
                FROM [dbo].[SubmissionMetricsView] SMV 
                WHERE SMV.CreatedBy = AUV.CreatedBy AND SMV.Period = '7D' AND SMV.MetricDate = @Today), 0) as LAST7D_COUNT,
        ISNULL((SELECT SMV.INT_Count 
                FROM [dbo].[SubmissionMetricsView] SMV 
                WHERE SMV.CreatedBy = AUV.CreatedBy AND SMV.Period = 'MTD' AND SMV.MetricDate = @Today), 0) as MTD_COUNT,
        ISNULL((SELECT SMV.INT_Count 
                FROM [dbo].[SubmissionMetricsView] SMV 
                WHERE SMV.CreatedBy = AUV.CreatedBy AND SMV.Period = 'QTD' AND SMV.MetricDate = @Today), 0) as QTD_COUNT,
        ISNULL((SELECT SMV.INT_Count 
                FROM [dbo].[SubmissionMetricsView] SMV 
                WHERE SMV.CreatedBy = AUV.CreatedBy AND SMV.Period = 'HYTD' AND SMV.MetricDate = @Today), 0) as HYTD_COUNT,
        ISNULL((SELECT SMV.INT_Count 
                FROM [dbo].[SubmissionMetricsView] SMV 
                WHERE SMV.CreatedBy = AUV.CreatedBy AND SMV.Period = 'YTD' AND SMV.MetricDate = @Today), 0) as YTD_COUNT
    FROM [dbo].[ActiveUsersView] AUV
    WHERE AUV.Status = 1
    ORDER BY AUV.CreatedBy
    FOR JSON PATH;
    
    -- =====================================================
    -- SET 4 - OEX Submissions by All Users (Using Materialized View)
    -- =====================================================
    SELECT 
        AUV.CreatedBy as [User],
        ISNULL((SELECT SMV.OEX_Count 
                FROM [dbo].[SubmissionMetricsView] SMV 
                WHERE SMV.CreatedBy = AUV.CreatedBy AND SMV.Period = '7D' AND SMV.MetricDate = @Today), 0) as LAST7D_COUNT,
        ISNULL((SELECT SMV.OEX_Count 
                FROM [dbo].[SubmissionMetricsView] SMV 
                WHERE SMV.CreatedBy = AUV.CreatedBy AND SMV.Period = 'MTD' AND SMV.MetricDate = @Today), 0) as MTD_COUNT,
        ISNULL((SELECT SMV.OEX_Count 
                FROM [dbo].[SubmissionMetricsView] SMV 
                WHERE SMV.CreatedBy = AUV.CreatedBy AND SMV.Period = 'QTD' AND SMV.MetricDate = @Today), 0) as QTD_COUNT,
        ISNULL((SELECT SMV.OEX_Count 
                FROM [dbo].[SubmissionMetricsView] SMV 
                WHERE SMV.CreatedBy = AUV.CreatedBy AND SMV.Period = 'HYTD' AND SMV.MetricDate = @Today), 0) as HYTD_COUNT,
        ISNULL((SELECT SMV.OEX_Count 
                FROM [dbo].[SubmissionMetricsView] SMV 
                WHERE SMV.CreatedBy = AUV.CreatedBy AND SMV.Period = 'YTD' AND SMV.MetricDate = @Today), 0) as YTD_COUNT
    FROM [dbo].[ActiveUsersView] AUV
    WHERE AUV.Status = 1
    ORDER BY AUV.CreatedBy
    FOR JSON PATH;
    
    -- =====================================================
    -- SET 5 - HIR Submissions by All Users (Using Materialized View)
    -- =====================================================
    SELECT 
        AUV.CreatedBy as [User],
        ISNULL((SELECT SMV.HIR_Count 
                FROM [dbo].[SubmissionMetricsView] SMV 
                WHERE SMV.CreatedBy = AUV.CreatedBy AND SMV.Period = '7D' AND SMV.MetricDate = @Today), 0) as LAST7D_COUNT,
        ISNULL((SELECT SMV.HIR_Count 
                FROM [dbo].[SubmissionMetricsView] SMV 
                WHERE SMV.CreatedBy = AUV.CreatedBy AND SMV.Period = 'MTD' AND SMV.MetricDate = @Today), 0) as MTD_COUNT,
        ISNULL((SELECT SMV.HIR_Count 
                FROM [dbo].[SubmissionMetricsView] SMV 
                WHERE SMV.CreatedBy = AUV.CreatedBy AND SMV.Period = 'QTD' AND SMV.MetricDate = @Today), 0) as QTD_COUNT,
        ISNULL((SELECT SMV.HIR_Count 
                FROM [dbo].[SubmissionMetricsView] SMV 
                WHERE SMV.CreatedBy = AUV.CreatedBy AND SMV.Period = 'HYTD' AND SMV.MetricDate = @Today), 0) as HYTD_COUNT,
        ISNULL((SELECT SMV.HIR_Count 
                FROM [dbo].[SubmissionMetricsView] SMV 
                WHERE SMV.CreatedBy = AUV.CreatedBy AND SMV.Period = 'YTD' AND SMV.MetricDate = @Today), 0) as YTD_COUNT
    FROM [dbo].[ActiveUsersView] AUV
    WHERE AUV.Status = 1
    ORDER BY AUV.CreatedBy
    FOR JSON PATH;
    
    -- =====================================================
    -- SET 6 - OEX to HIR Conversion Ratios (Using Materialized View)
    -- =====================================================
    SELECT 
        AUV.CreatedBy as [User],
        ISNULL((SELECT SMV.OEX_HIR_Ratio 
                FROM [dbo].[SubmissionMetricsView] SMV 
                WHERE SMV.CreatedBy = AUV.CreatedBy AND SMV.Period = '7D' AND SMV.MetricDate = @Today), 0.00) as LAST7D_RATIO,
        ISNULL((SELECT SMV.OEX_HIR_Ratio 
                FROM [dbo].[SubmissionMetricsView] SMV 
                WHERE SMV.CreatedBy = AUV.CreatedBy AND SMV.Period = 'MTD' AND SMV.MetricDate = @Today), 0.00) as MTD_RATIO,
        ISNULL((SELECT SMV.OEX_HIR_Ratio 
                FROM [dbo].[SubmissionMetricsView] SMV 
                WHERE SMV.CreatedBy = AUV.CreatedBy AND SMV.Period = 'QTD' AND SMV.MetricDate = @Today), 0.00) as QTD_RATIO,
        ISNULL((SELECT SMV.OEX_HIR_Ratio 
                FROM [dbo].[SubmissionMetricsView] SMV 
                WHERE SMV.CreatedBy = AUV.CreatedBy AND SMV.Period = 'HYTD' AND SMV.MetricDate = @Today), 0.00) as HYTD_RATIO,
        ISNULL((SELECT SMV.OEX_HIR_Ratio 
                FROM [dbo].[SubmissionMetricsView] SMV 
                WHERE SMV.CreatedBy = AUV.CreatedBy AND SMV.Period = 'YTD' AND SMV.MetricDate = @Today), 0.00) as YTD_RATIO
    FROM [dbo].[ActiveUsersView] AUV
    WHERE AUV.Status = 1
    ORDER BY AUV.CreatedBy
    FOR JSON PATH;
    
    -- =====================================================
    -- SET 7 - Active Requisitions Updated (Using Materialized View)
    -- =====================================================
    SELECT 
        AUV.CreatedBy as [User],
        ISNULL((SELECT SMV.Active_Requisitions_Updated 
                FROM [dbo].[SubmissionMetricsView] SMV 
                WHERE SMV.CreatedBy = AUV.CreatedBy AND SMV.Period = '7D' AND SMV.MetricDate = @Today), 0) as LAST7D_COUNT,
        ISNULL((SELECT SMV.Active_Requisitions_Updated 
                FROM [dbo].[SubmissionMetricsView] SMV 
                WHERE SMV.CreatedBy = AUV.CreatedBy AND SMV.Period = 'MTD' AND SMV.MetricDate = @Today), 0) as MTD_COUNT,
        ISNULL((SELECT SMV.Active_Requisitions_Updated 
                FROM [dbo].[SubmissionMetricsView] SMV 
                WHERE SMV.CreatedBy = AUV.CreatedBy AND SMV.Period = 'QTD' AND SMV.MetricDate = @Today), 0) as QTD_COUNT,
        ISNULL((SELECT SMV.Active_Requisitions_Updated 
                FROM [dbo].[SubmissionMetricsView] SMV 
                WHERE SMV.CreatedBy = AUV.CreatedBy AND SMV.Period = 'HYTD' AND SMV.MetricDate = @Today), 0) as HYTD_COUNT,
        ISNULL((SELECT SMV.Active_Requisitions_Updated 
                FROM [dbo].[SubmissionMetricsView] SMV 
                WHERE SMV.CreatedBy = AUV.CreatedBy AND SMV.Period = 'YTD' AND SMV.MetricDate = @Today), 0) as YTD_COUNT
    FROM [dbo].[ActiveUsersView] AUV
    WHERE AUV.Status = 1
    ORDER BY AUV.CreatedBy
    FOR JSON PATH;
    
    -- =====================================================
    -- SET 8 - System-Wide Timing Analytics (Using Materialized View)
    -- =====================================================
    SELECT 
        'ALL_USERS' as [User],
        -- Overall averages across all active users
        ISNULL(AVG(CAST(TAV.TimeToFill_Days as FLOAT)), 0) as TimeToFill_Days,
        ISNULL(AVG(CAST(TAV.TimeToHire_Days as FLOAT)), 0) as TimeToHire_Days,
        -- Stage timing averages
        ISNULL(AVG(CAST(TAV.PEN_Days as FLOAT)), 0) as PEN_Days,
        ISNULL(AVG(CAST(TAV.REJ_Days as FLOAT)), 0) as REJ_Days,
        ISNULL(AVG(CAST(TAV.HLD_Days as FLOAT)), 0) as HLD_Days,
        ISNULL(AVG(CAST(TAV.PHN_Days as FLOAT)), 0) as PHN_Days,
        ISNULL(AVG(CAST(TAV.URW_Days as FLOAT)), 0) as URW_Days,
        ISNULL(AVG(CAST(TAV.INT_Days as FLOAT)), 0) as INT_Days,
        ISNULL(AVG(CAST(TAV.RHM_Days as FLOAT)), 0) as RHM_Days,
        ISNULL(AVG(CAST(TAV.DEC_Days as FLOAT)), 0) as DEC_Days,
        ISNULL(AVG(CAST(TAV.NOA_Days as FLOAT)), 0) as NOA_Days,
        ISNULL(AVG(CAST(TAV.OEX_Days as FLOAT)), 0) as OEX_Days,
        ISNULL(AVG(CAST(TAV.ODC_Days as FLOAT)), 0) as ODC_Days,
        ISNULL(AVG(CAST(TAV.HIR_Days as FLOAT)), 0) as HIR_Days,
        ISNULL(AVG(CAST(TAV.WDR_Days as FLOAT)), 0) as WDR_Days,
        -- Totals
        COUNT(DISTINCT CONCAT(TAV.RequisitionId, '-', TAV.CandidateId)) as TotalCandidates,
        COUNT(DISTINCT TAV.RequisitionId) as TotalRequisitions,
        COUNT(DISTINCT TAV.CreatedBy) as ActiveUsers
    FROM [dbo].[TimingAnalyticsView] TAV
    INNER JOIN [dbo].[ActiveUsersView] AUV ON TAV.CreatedBy = AUV.CreatedBy
    WHERE AUV.Status = 1
    FOR JSON PATH;
    
    -- =====================================================
    -- SET 9 - Top Performing Users Summary (Using Materialized View)
    -- =====================================================
    WITH UserPerformance AS (
        SELECT 
            AUV.CreatedBy,
            AUV.FullName,
            -- Get YTD metrics
            ISNULL((SELECT SMV.Total_Submissions 
                    FROM [dbo].[SubmissionMetricsView] SMV 
                    WHERE SMV.CreatedBy = AUV.CreatedBy AND SMV.Period = 'YTD' AND SMV.MetricDate = @Today), 0) as YTD_Submissions,
            ISNULL((SELECT SMV.HIR_Count 
                    FROM [dbo].[SubmissionMetricsView] SMV 
                    WHERE SMV.CreatedBy = AUV.CreatedBy AND SMV.Period = 'YTD' AND SMV.MetricDate = @Today), 0) as YTD_Hires,
            ISNULL((SELECT SMV.Requisitions_Created 
                    FROM [dbo].[SubmissionMetricsView] SMV 
                    WHERE SMV.CreatedBy = AUV.CreatedBy AND SMV.Period = 'YTD' AND SMV.MetricDate = @Today), 0) as YTD_Requisitions,
            -- Calculate hire rate
            CASE 
                WHEN ISNULL((SELECT SMV.Total_Submissions 
                            FROM [dbo].[SubmissionMetricsView] SMV 
                            WHERE SMV.CreatedBy = AUV.CreatedBy AND SMV.Period = 'YTD' AND SMV.MetricDate = @Today), 0) > 0
                THEN CAST(ISNULL((SELECT SMV.HIR_Count 
                                FROM [dbo].[SubmissionMetricsView] SMV 
                                WHERE SMV.CreatedBy = AUV.CreatedBy AND SMV.Period = 'YTD' AND SMV.MetricDate = @Today), 0) AS FLOAT) / 
                     ISNULL((SELECT SMV.Total_Submissions 
                            FROM [dbo].[SubmissionMetricsView] SMV 
                            WHERE SMV.CreatedBy = AUV.CreatedBy AND SMV.Period = 'YTD' AND SMV.MetricDate = @Today), 1) * 100
                ELSE 0
            END as HireRate
        FROM [dbo].[ActiveUsersView] AUV
        WHERE AUV.Status = 1
    )
    SELECT 
        CreatedBy as [User],
        FullName,
        YTD_Submissions,
        YTD_Hires,
        YTD_Requisitions,
        CAST(HireRate as DECIMAL(5,2)) as HireRatePercent,
        ROW_NUMBER() OVER (ORDER BY YTD_Hires DESC, YTD_Submissions DESC) as PerformanceRank
    FROM UserPerformance
    WHERE YTD_Submissions > 0  -- Only show users with activity
    ORDER BY YTD_Hires DESC, YTD_Submissions DESC
    FOR JSON PATH;
    
    -- =====================================================
    -- SET 10 - Data Freshness Check
    -- =====================================================
    SELECT 
        'SYSTEM_STATUS' as [Type],
        MAX(AUV.RefreshDate) as LastActiveUsersRefresh,
        MAX(SMV.RefreshDate) as LastSubmissionMetricsRefresh,
        MAX(TAV.RefreshDate) as LastTimingAnalyticsRefresh,
        DATEDIFF(MINUTE, MAX(SMV.RefreshDate), GETDATE()) as MinutesSinceLastRefresh,
        CASE 
            WHEN DATEDIFF(MINUTE, MAX(SMV.RefreshDate), GETDATE()) > 25 THEN 'STALE'
            WHEN DATEDIFF(MINUTE, MAX(SMV.RefreshDate), GETDATE()) > 20 THEN 'WARNING'
            ELSE 'FRESH'
        END as DataFreshnessStatus
    FROM [dbo].[ActiveUsersView] AUV
    CROSS JOIN [dbo].[SubmissionMetricsView] SMV
    CROSS JOIN [dbo].[TimingAnalyticsView] TAV
    FOR JSON PATH;
    
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [DashboardAdmin_Refactor]
AS
BEGIN
    SET NOCOUNT ON;
    SET ARITHABORT ON;
    SET ANSI_NULLS ON;
    SET QUOTED_IDENTIFIER ON;
    
    DECLARE @Today DATE = GETDATE();
    
    -- Generic Query for Dropdown (Admin sees all users)
    SELECT 
        CreatedBy as KeyValue, FullName as Text
    FROM 
        dbo.ActiveUsersView
    WHERE 
        Status = 1
    FOR JSON AUTO;
    
    /***************************************************************************
    * RESULT SET 1: TIME-BOUND METRICS - ADMIN AGGREGATED (Refactored)
    * Uses pre-calculated SubmissionMetricsView - SUM across ALL USERS
    ***************************************************************************/
    
    -- Single query replaces entire #RequisitionMetrics and #SubmissionMetrics logic
    -- Admin perspective: Show individual data for ALL active users (no aggregation)
    SELECT ISNULL((
        SELECT 
            [CreatedBy] as [User], [Period], [Total_Submissions] as TotalSubmissions, [Active_Requisitions_Updated] as ActiveRequisitions, [INT_Count] as INTSubmissions,
            [OEX_Count] as OEXSubmissions, [HIR_Count] as HIRSubmissions, [OEX_HIR_Ratio] as OEXHIRRatio, [Requisitions_Created] as RequisitionsCreated
        FROM 
            [dbo].[SubmissionMetricsView]
        WHERE 
            [CreatedBy] IN (SELECT UserName FROM dbo.Users WHERE Status = 1 AND Role IN (4, 5, 6))
        ORDER BY 
            [CreatedBy], CASE [Period] 
                            WHEN '7D' THEN 1 
                            WHEN 'MTD' THEN 2 
                            WHEN 'QTD' THEN 3 
                            WHEN 'HYTD' THEN 4 
                            WHEN 'YTD' THEN 5 
                        END
        FOR JSON PATH
    ), '[]');

    /***************************************************************************
    * RESULT SET 2: ADMIN STATUS REPORT (Keep Original Logic)
    * Query 1: Current submission status across all users
    ***************************************************************************/
    
    -- Current Status Report for all active submissions
    --WITH CompanySubmissionCounts AS (
    --    SELECT 
    --        C.CompanyName,
    --        COUNT(DISTINCT S.RequisitionId) as SubmissionCount
    --    FROM dbo.Submissions S
    --    INNER JOIN dbo.Requisitions R ON S.RequisitionId = R.Id
    --    INNER JOIN dbo.Companies C ON R.CompanyId = C.ID
    --    INNER JOIN dbo.Users U ON R.CreatedBy = U.UserName AND U.Status = 1
    --    GROUP BY C.CompanyName
    --),
    --CurrentStatus AS (
    --    SELECT 
    --        S.RequisitionId,
    --        S.CandidateId,
    --        S.Status as CurrentStatus,
    --        MIN(S.CreatedDate) as DateFirstSubmitted,
    --        MAX(S.CreatedDate) as LastActivityDate,
    --        STRING_AGG(ISNULL(S.Notes, ''), '; ') as ActivityNotes
    --    FROM dbo.Submissions S
    --    INNER JOIN dbo.Requisitions R ON S.RequisitionId = R.Id
    --    INNER JOIN dbo.Users U ON R.CreatedBy = U.UserName AND U.Status = 1
    --    GROUP BY S.RequisitionId, S.CandidateId, S.Status
    --)
    --SELECT ISNULL((
    --    SELECT 
    --        '[' + R.Code + '] - ' + CAST(R.Positions as varchar(5)) + ' Positions - ' + R.PosTitle as Company,
    --        LTRIM(RTRIM(ISNULL(CAND.FirstName, '') + ' ' + ISNULL(CAND.LastName, ''))) as [CandidateName],
    --        CS.CurrentStatus as [CurrentStatus],
    --        CS.DateFirstSubmitted as [DateFirstSubmitted],
    --        CS.LastActivityDate as [LastActivityDate],
    --        ISNULL(CS.ActivityNotes, '') as [ActivityNotes],
    --        R.CreatedBy [User]
    --    FROM CurrentStatus CS
    --    INNER JOIN dbo.Requisitions R ON CS.RequisitionId = R.Id
    --    INNER JOIN dbo.Companies C ON R.CompanyId = C.ID
    --    INNER JOIN dbo.Candidate CAND ON CS.CandidateId = CAND.ID
    --    INNER JOIN CompanySubmissionCounts CSC ON C.CompanyName = CSC.CompanyName
    --    ORDER BY 
    --        CSC.SubmissionCount DESC,
    --        C.CompanyName ASC,
    --        R.Id ASC,
    --        CS.DateFirstSubmitted ASC
    --    FOR JSON PATH
    --), '[]');
    DECLARE @return varchar(max) = '[]';

    SELECT @return =
        (SELECT 
            RAV.CompanyName + ' - [' + RAV.RequisitionCode + '] - ' + CAST(RAV.Positions as varchar(5)) + ' Positions - ' + RAV.RequisitionTitle as Company,
            RAV.CandidateName, RAV.CurrentStatus, RAV.DateFirstSubmitted, RAV.LastActivityDate, RAV.ActivityNotes, RAV.CreatedBy [User]
        FROM 
            [dbo].[RecentActivityView] RAV
        ORDER BY 
            RAV.SubmissionCount DESC, RAV.CompanyName ASC, RAV.RequisitionCode ASC, RAV.DateFirstSubmitted ASC
        FOR JSON PATH);

    SELECT ISNULL(@return, '[]');

    /***************************************************************************
    * RESULT SET 3: HIRED CANDIDATES REPORT (Keep Original Logic)
    * Query 1: Recent hires across all users
    ***************************************************************************/
    
    -- Hired Candidates Report for last 3 months across all users
    --DECLARE @StartDateSet3 DATE = DATEADD(MONTH, -3, @Today);
    
    --WITH HiredCandidates AS (
    --    SELECT 
    --        S.RequisitionId,
    --        S.CandidateId,
    --        MAX(S.CreatedDate) as DateHired
    --    FROM dbo.Submissions S
    --    INNER JOIN dbo.Requisitions R ON S.RequisitionId = R.Id
    --    INNER JOIN dbo.Users U ON R.CreatedBy = U.UserName AND U.Status = 1
    --    WHERE S.Status = 'HIR'
    --    GROUP BY S.RequisitionId, S.CandidateId, R.CreatedBy
    --    HAVING MAX(S.CreatedDate) >= @StartDateSet3
    --)
    --SELECT ISNULL((
    --    SELECT 
    --        C.CompanyName as Company,
    --        R.Code as [RequisitionNumber],
    --        R.Positions as [NumPosition],
    --        R.PosTitle as Title,
    --        LTRIM(RTRIM(ISNULL(CAND.FirstName, '') + ' ' + ISNULL(CAND.LastName, ''))) as [CandidateName],
    --        HC.DateHired as [DateHired],
    --        CAST(0.00 AS DECIMAL(9,2)) as [SalaryOffered],
    --        @Today as [StartDate],
    --        @Today as [DateInvoiced],
    --        @Today as [DatePaid],
    --        CAST(0.00 AS DECIMAL(9,2)) as [Placementfee],
    --        CAST(0.00 AS DECIMAL(5,2)) as [CommissionPercent],
    --        CAST(0.00 AS DECIMAL(9,2)) as [CommissionEarned],
    --        R.CreatedBy [User]
    --    FROM HiredCandidates HC
    --    INNER JOIN dbo.Requisitions R ON HC.RequisitionId = R.Id
    --    INNER JOIN dbo.Companies C ON R.CompanyId = C.ID
    --    INNER JOIN dbo.Candidate CAND ON HC.CandidateId = CAND.ID
    --    ORDER BY 
    --        R.CreatedBy,
    --        C.CompanyName,
    --        R.Code,
    --        HC.DateHired
    --    FOR JSON PATH
    --), '[]');

    SELECT @return =
        (SELECT 
            PRV.CompanyName as Company, PRV.RequisitionNumber, PRV.NumPosition, PRV.RequisitionTitle as Title, PRV.CandidateName, PRV.DateHired,
            PRV.SalaryOffered, PRV.StartDate, PRV.DateInvoiced, PRV.DatePaid, PRV.PlacementFee as [Placementfee], PRV.CommissionPercent,
            PRV.CommissionEarned, PRV.CreatedBy [User]
        FROM 
            [dbo].[PlacementReportView] PRV
        ORDER BY 
            PRV.CreatedBy ASC, PRV.CompanyName ASC, PRV.RequisitionNumber ASC, PRV.DateHired DESC
        FOR JSON PATH);

    SELECT ISNULL(@return, '[]');

    /***************************************************************************
    * RESULT SET 4: ADMIN TIMING ANALYTICS (Keep Original Logic)
    * Query 1: Time to Fill, Time to Hire, and Time in Stage metrics (By Requisition) 
    * Query 2: Time to Fill, Time to Hire, and Time in Stage metrics (By Company)
    * Shows data for ALL USERS (Admin perspective)
    ***************************************************************************/
    
    -- Get requisitions from last year for all users
    --DECLARE @StartDate365 DATE = DATEADD(YEAR, -1, @Today);
    
    ---- Create temp tables for admin timing analytics
    --DROP TABLE IF EXISTS #AdminRequisitionBase;
    --DROP TABLE IF EXISTS #AdminFirstSubmissions;
    --DROP TABLE IF EXISTS #AdminTimeToHireCalc;
    --DROP TABLE IF EXISTS #AdminStageTimings;
    --DROP TABLE IF EXISTS #AdminStageTimeCalculations;
    
    ---- Temp table 1: Get requisitions created by ALL USERS in last year
    --SELECT 
    --    R.Id as RequisitionId,
    --    R.Code as RequisitionCode,
    --    ISNULL(C.CompanyName, 'Unknown Company') as CompanyName,
    --    R.PosTitle as Title,
    --    R.CreatedDate as RequisitionCreatedDate,
    --    R.CreatedBy,
    --    -- Time to Fill calculation (only for FUL status)
    --    CASE 
    --        WHEN R.Status = 'FUL' THEN DATEDIFF(DAY, R.CreatedDate, R.UpdatedDate)
    --        ELSE NULL 
    --    END as TimeToFill
    --INTO #AdminRequisitionBase
    --FROM Requisitions R
    --LEFT JOIN Companies C ON R.CompanyId = C.ID
    --INNER JOIN dbo.Users U ON R.CreatedBy = U.UserName AND U.Status = 1
    --WHERE CAST(R.CreatedDate AS DATE) >= @StartDate365
    --    AND R.CreatedBy IS NOT NULL;
    
    ---- Temp table 2: Get first submission date for each candidate+requisition combo (all users)
    --SELECT 
    --    S.RequisitionId,
    --    S.CandidateId,
    --    MIN(S.CreatedDate) as FirstSubmissionDate
    --INTO #AdminFirstSubmissions
    --FROM Submissions S
    --INNER JOIN #AdminRequisitionBase ARB ON S.RequisitionId = ARB.RequisitionId
    --GROUP BY S.RequisitionId, S.CandidateId;
    
    ---- Temp table 3: Calculate Time to Hire for hired candidates (all users)
    --SELECT 
    --    AFS.RequisitionId,
    --    AFS.CandidateId,
    --    AFS.FirstSubmissionDate,
    --    MAX(CASE WHEN S.Status = 'HIR' THEN S.CreatedDate END) as HireDate,
    --    MAX(CASE WHEN S.Status = 'HIR' THEN DATEDIFF(DAY, AFS.FirstSubmissionDate, S.CreatedDate) END) as TimeToHire
    --INTO #AdminTimeToHireCalc
    --FROM #AdminFirstSubmissions AFS
    --INNER JOIN Submissions S ON AFS.RequisitionId = S.RequisitionId AND AFS.CandidateId = S.CandidateId
    --GROUP BY AFS.RequisitionId, AFS.CandidateId, AFS.FirstSubmissionDate;
    
    ---- Temp table 4: Get stage timing for each submission (all users)
    --SELECT 
    --    AFS.RequisitionId,
    --    AFS.CandidateId,
    --    MAX(CASE WHEN S.Status = 'PEN' THEN S.CreatedDate END) as PEN_Date,
    --    MAX(CASE WHEN S.Status = 'REJ' THEN S.CreatedDate END) as REJ_Date,
    --    MAX(CASE WHEN S.Status = 'HLD' THEN S.CreatedDate END) as HLD_Date,
    --    MAX(CASE WHEN S.Status = 'PHN' THEN S.CreatedDate END) as PHN_Date,
    --    MAX(CASE WHEN S.Status = 'URW' THEN S.CreatedDate END) as URW_Date,
    --    MAX(CASE WHEN S.Status = 'INT' THEN S.CreatedDate END) as INT_Date,
    --    MAX(CASE WHEN S.Status = 'RHM' THEN S.CreatedDate END) as RHM_Date,
    --    MAX(CASE WHEN S.Status = 'DEC' THEN S.CreatedDate END) as DEC_Date,
    --    MAX(CASE WHEN S.Status = 'NOA' THEN S.CreatedDate END) as NOA_Date,
    --    MAX(CASE WHEN S.Status = 'OEX' THEN S.CreatedDate END) as OEX_Date,
    --    MAX(CASE WHEN S.Status = 'ODC' THEN S.CreatedDate END) as ODC_Date,
    --    MAX(CASE WHEN S.Status = 'HIR' THEN S.CreatedDate END) as HIR_Date,
    --    MAX(CASE WHEN S.Status = 'WDR' THEN S.CreatedDate END) as WDR_Date
    --INTO #AdminStageTimings
    --FROM #AdminFirstSubmissions AFS
    --INNER JOIN Submissions S ON AFS.RequisitionId = S.RequisitionId AND AFS.CandidateId = S.CandidateId
    --GROUP BY AFS.RequisitionId, AFS.CandidateId;
    
    ---- Temp table 5: Calculate days spent in each stage (all users)
    --SELECT 
    --    AST.RequisitionId,
    --    AST.CandidateId,
    --    CASE 
    --        WHEN AST.PEN_Date IS NULL THEN 0
    --        WHEN AST.REJ_Date IS NOT NULL AND AST.REJ_Date > AST.PEN_Date THEN DATEDIFF(DAY, AST.PEN_Date, AST.REJ_Date)
    --        WHEN AST.HLD_Date IS NOT NULL AND AST.HLD_Date > AST.PEN_Date THEN DATEDIFF(DAY, AST.PEN_Date, AST.HLD_Date)
    --        WHEN AST.PHN_Date IS NOT NULL AND AST.PHN_Date > AST.PEN_Date THEN DATEDIFF(DAY, AST.PEN_Date, AST.PHN_Date)
    --        ELSE DATEDIFF(DAY, AST.PEN_Date, @Today)
    --    END as PEN_Days,
        
    --    CASE 
    --        WHEN AST.REJ_Date IS NULL THEN 0
    --        ELSE 0 -- REJ is final stage
    --    END as REJ_Days,
        
    --    CASE 
    --        WHEN AST.HLD_Date IS NULL THEN 0
    --        WHEN AST.PHN_Date IS NOT NULL AND AST.PHN_Date > AST.HLD_Date THEN DATEDIFF(DAY, AST.HLD_Date, AST.PHN_Date)
    --        WHEN AST.URW_Date IS NOT NULL AND AST.URW_Date > AST.HLD_Date THEN DATEDIFF(DAY, AST.HLD_Date, AST.URW_Date)
    --        ELSE DATEDIFF(DAY, AST.HLD_Date, @Today)
    --    END as HLD_Days,
        
    --    CASE 
    --        WHEN AST.PHN_Date IS NULL THEN 0
    --        WHEN AST.URW_Date IS NOT NULL AND AST.URW_Date > AST.PHN_Date THEN DATEDIFF(DAY, AST.PHN_Date, AST.URW_Date)
    --        WHEN AST.INT_Date IS NOT NULL AND AST.INT_Date > AST.PHN_Date THEN DATEDIFF(DAY, AST.PHN_Date, AST.INT_Date)
    --        ELSE DATEDIFF(DAY, AST.PHN_Date, @Today)
    --    END as PHN_Days,
        
    --    CASE 
    --        WHEN AST.URW_Date IS NULL THEN 0
    --        WHEN AST.INT_Date IS NOT NULL AND AST.INT_Date > AST.URW_Date THEN DATEDIFF(DAY, AST.URW_Date, AST.INT_Date)
    --        WHEN AST.RHM_Date IS NOT NULL AND AST.RHM_Date > AST.URW_Date THEN DATEDIFF(DAY, AST.URW_Date, AST.RHM_Date)
    --        ELSE DATEDIFF(DAY, AST.URW_Date, @Today)
    --    END as URW_Days,
        
    --    CASE 
    --        WHEN AST.INT_Date IS NULL THEN 0
    --        WHEN AST.RHM_Date IS NOT NULL AND AST.RHM_Date > AST.INT_Date THEN DATEDIFF(DAY, AST.INT_Date, AST.RHM_Date)
    --        WHEN AST.DEC_Date IS NOT NULL AND AST.DEC_Date > AST.INT_Date THEN DATEDIFF(DAY, AST.INT_Date, AST.DEC_Date)
    --        ELSE DATEDIFF(DAY, AST.INT_Date, @Today)
    --    END as INT_Days,
        
    --    CASE 
    --        WHEN AST.RHM_Date IS NULL THEN 0
    --        WHEN AST.DEC_Date IS NOT NULL AND AST.DEC_Date > AST.RHM_Date THEN DATEDIFF(DAY, AST.RHM_Date, AST.DEC_Date)
    --        WHEN AST.NOA_Date IS NOT NULL AND AST.NOA_Date > AST.RHM_Date THEN DATEDIFF(DAY, AST.RHM_Date, AST.NOA_Date)
    --        ELSE DATEDIFF(DAY, AST.RHM_Date, @Today)
    --    END as RHM_Days,
        
    --    CASE 
    --        WHEN AST.DEC_Date IS NULL THEN 0
    --        WHEN AST.NOA_Date IS NOT NULL AND AST.NOA_Date > AST.DEC_Date THEN DATEDIFF(DAY, AST.DEC_Date, AST.NOA_Date)
    --        WHEN AST.OEX_Date IS NOT NULL AND AST.OEX_Date > AST.DEC_Date THEN DATEDIFF(DAY, AST.DEC_Date, AST.OEX_Date)
    --        ELSE DATEDIFF(DAY, AST.DEC_Date, @Today)
    --    END as DEC_Days,
        
    --    CASE 
    --        WHEN AST.NOA_Date IS NULL THEN 0
    --        WHEN AST.OEX_Date IS NOT NULL AND AST.OEX_Date > AST.NOA_Date THEN DATEDIFF(DAY, AST.NOA_Date, AST.OEX_Date)
    --        WHEN AST.ODC_Date IS NOT NULL AND AST.ODC_Date > AST.NOA_Date THEN DATEDIFF(DAY, AST.NOA_Date, AST.ODC_Date)
    --        ELSE DATEDIFF(DAY, AST.NOA_Date, @Today)
    --    END as NOA_Days,
        
    --    CASE 
    --        WHEN AST.OEX_Date IS NULL THEN 0
    --        WHEN AST.HIR_Date IS NOT NULL AND AST.HIR_Date > AST.OEX_Date THEN DATEDIFF(DAY, AST.OEX_Date, AST.HIR_Date)
    --        WHEN AST.ODC_Date IS NOT NULL AND AST.ODC_Date > AST.OEX_Date THEN DATEDIFF(DAY, AST.OEX_Date, AST.ODC_Date)
    --        WHEN AST.WDR_Date IS NOT NULL AND AST.WDR_Date > AST.OEX_Date THEN DATEDIFF(DAY, AST.OEX_Date, AST.WDR_Date)
    --        ELSE DATEDIFF(DAY, AST.OEX_Date, @Today)
    --    END as OEX_Days,
        
    --    CASE 
    --        WHEN AST.ODC_Date IS NULL THEN 0
    --        ELSE 0 -- ODC is final stage
    --    END as ODC_Days,
        
    --    CASE 
    --        WHEN AST.HIR_Date IS NULL THEN 0
    --        ELSE 0 -- HIR is final stage
    --    END as HIR_Days,
        
    --    CASE 
    --        WHEN AST.WDR_Date IS NULL THEN 0
    --        ELSE 0 -- WDR is final stage
    --    END as WDR_Days
        
    --INTO #AdminStageTimeCalculations
    --FROM #AdminStageTimings AST;
    
    -- Query 1: Timing Analytics by Requisition (Admin view - all users)
    --SELECT ISNULL((
    --    SELECT 
    --        'ALL_USERS' as [User],
    --        ARB.RequisitionCode,
    --        ARB.Title,
    --        ARB.CreatedBy as RequisitionOwner,
    --        -- Average Time to Fill for this requisition (if filled)
    --        ISNULL(CEILING(AVG(CAST(ARB.TimeToFill as FLOAT))), 0) as TimeToFill_Days,
    --        -- Average Time to Hire for this requisition  
    --        ISNULL(CEILING(AVG(CAST(ATTH.TimeToHire as FLOAT))), 0) as TimeToHire_Days,
    --        -- Average time in each stage for this requisition (rounded up to next integer)
    --        ISNULL(CEILING(AVG(CAST(ASTC.PEN_Days as FLOAT))), 0) as PEN_Days,
    --        ISNULL(CEILING(AVG(CAST(ASTC.REJ_Days as FLOAT))), 0) as REJ_Days,
    --        ISNULL(CEILING(AVG(CAST(ASTC.HLD_Days as FLOAT))), 0) as HLD_Days,
    --        ISNULL(CEILING(AVG(CAST(ASTC.PHN_Days as FLOAT))), 0) as PHN_Days,
    --        ISNULL(CEILING(AVG(CAST(ASTC.URW_Days as FLOAT))), 0) as URW_Days,
    --        ISNULL(CEILING(AVG(CAST(ASTC.INT_Days as FLOAT))), 0) as INT_Days,
    --        ISNULL(CEILING(AVG(CAST(ASTC.RHM_Days as FLOAT))), 0) as RHM_Days,
    --        ISNULL(CEILING(AVG(CAST(ASTC.DEC_Days as FLOAT))), 0) as DEC_Days,
    --        ISNULL(CEILING(AVG(CAST(ASTC.NOA_Days as FLOAT))), 0) as NOA_Days,
    --        ISNULL(CEILING(AVG(CAST(ASTC.OEX_Days as FLOAT))), 0) as OEX_Days,
    --        ISNULL(CEILING(AVG(CAST(ASTC.ODC_Days as FLOAT))), 0) as ODC_Days,
    --        ISNULL(CEILING(AVG(CAST(ASTC.HIR_Days as FLOAT))), 0) as HIR_Days,
    --        ISNULL(CEILING(AVG(CAST(ASTC.WDR_Days as FLOAT))), 0) as WDR_Days,
    --        COUNT(DISTINCT CONCAT(ASTC.RequisitionId, '-', ASTC.CandidateId)) as TotalCandidates
    --    FROM #AdminRequisitionBase ARB
    --    LEFT JOIN #AdminTimeToHireCalc ATTH ON ARB.RequisitionId = ATTH.RequisitionId
    --    LEFT JOIN #AdminStageTimeCalculations ASTC ON ARB.RequisitionId = ASTC.RequisitionId
    --    GROUP BY ARB.RequisitionCode, ARB.Title, ARB.CreatedBy
    --    ORDER BY ARB.RequisitionCode
    --    FOR JSON PATH
    --), '[]');
    SELECT @return = 
        (SELECT 
            TAV.CreatedBy, TAV.RequisitionCode, TAV.CompanyName, TAV.Title, ISNULL(CEILING(AVG(CAST(TAV.TimeToFill_Days as FLOAT))), 0) as TimeToFill_Days,
            ISNULL(CEILING(AVG(CAST(TAV.TimeToHire_Days as FLOAT))), 0) as TimeToHire_Days, ISNULL(CEILING(AVG(CAST(TAV.PEN_Days as FLOAT))), 0) as PEN_Days,
            ISNULL(CEILING(AVG(CAST(TAV.REJ_Days as FLOAT))), 0) as REJ_Days, ISNULL(CEILING(AVG(CAST(TAV.HLD_Days as FLOAT))), 0) as HLD_Days,
            ISNULL(CEILING(AVG(CAST(TAV.PHN_Days as FLOAT))), 0) as PHN_Days, ISNULL(CEILING(AVG(CAST(TAV.URW_Days as FLOAT))), 0) as URW_Days,
            ISNULL(CEILING(AVG(CAST(TAV.INT_Days as FLOAT))), 0) as INT_Days, ISNULL(CEILING(AVG(CAST(TAV.RHM_Days as FLOAT))), 0) as RHM_Days,
            ISNULL(CEILING(AVG(CAST(TAV.DEC_Days as FLOAT))), 0) as DEC_Days, ISNULL(CEILING(AVG(CAST(TAV.NOA_Days as FLOAT))), 0) as NOA_Days,
            ISNULL(CEILING(AVG(CAST(TAV.OEX_Days as FLOAT))), 0) as OEX_Days, ISNULL(CEILING(AVG(CAST(TAV.ODC_Days as FLOAT))), 0) as ODC_Days,
            ISNULL(CEILING(AVG(CAST(TAV.HIR_Days as FLOAT))), 0) as HIR_Days, ISNULL(CEILING(AVG(CAST(TAV.WDR_Days as FLOAT))), 0) as WDR_Days,
            COUNT(DISTINCT CONCAT(TAV.RequisitionId, '-', TAV.CandidateId)) as TotalCandidates
        FROM 
            [dbo].[TimingAnalyticsView] TAV
        GROUP BY  
            TAV.CreatedBy, TAV.RequisitionCode, TAV.CompanyName, TAV.Title
        ORDER BY 
            TAV.RequisitionCode
        FOR JSON PATH);
    
    SELECT ISNULL(@return, '[]');

    -- Query 2: Timing Analytics by Company (Admin view - all users)
    --SELECT ISNULL((
    --    SELECT 
    --        'ALL_USERS' as [User],
    --        ARB.CompanyName,
    --        -- Average Time to Fill for this company (if filled)
    --        ISNULL(CEILING(AVG(CAST(ARB.TimeToFill as FLOAT))), 0) as TimeToFill_Days,
    --        -- Average Time to Hire for this company  
    --        ISNULL(CEILING(AVG(CAST(ATTH.TimeToHire as FLOAT))), 0) as TimeToHire_Days,
    --        -- Average time in each stage for this company (rounded up to next integer)
    --        ISNULL(CEILING(AVG(CAST(ASTC.PEN_Days as FLOAT))), 0) as PEN_Days,
    --        ISNULL(CEILING(AVG(CAST(ASTC.REJ_Days as FLOAT))), 0) as REJ_Days,
    --        ISNULL(CEILING(AVG(CAST(ASTC.HLD_Days as FLOAT))), 0) as HLD_Days,
    --        ISNULL(CEILING(AVG(CAST(ASTC.PHN_Days as FLOAT))), 0) as PHN_Days,
    --        ISNULL(CEILING(AVG(CAST(ASTC.URW_Days as FLOAT))), 0) as URW_Days,
    --        ISNULL(CEILING(AVG(CAST(ASTC.INT_Days as FLOAT))), 0) as INT_Days,
    --        ISNULL(CEILING(AVG(CAST(ASTC.RHM_Days as FLOAT))), 0) as RHM_Days,
    --        ISNULL(CEILING(AVG(CAST(ASTC.DEC_Days as FLOAT))), 0) as DEC_Days,
    --        ISNULL(CEILING(AVG(CAST(ASTC.NOA_Days as FLOAT))), 0) as NOA_Days,
    --        ISNULL(CEILING(AVG(CAST(ASTC.OEX_Days as FLOAT))), 0) as OEX_Days,
    --        ISNULL(CEILING(AVG(CAST(ASTC.ODC_Days as FLOAT))), 0) as ODC_Days,
    --        ISNULL(CEILING(AVG(CAST(ASTC.HIR_Days as FLOAT))), 0) as HIR_Days,
    --        ISNULL(CEILING(AVG(CAST(ASTC.WDR_Days as FLOAT))), 0) as WDR_Days,
    --        COUNT(DISTINCT CONCAT(ASTC.RequisitionId, '-', ASTC.CandidateId)) as TotalCandidates,
    --        COUNT(DISTINCT ARB.RequisitionId) as TotalRequisitions,
    --        COUNT(DISTINCT ARB.CreatedBy) as TotalRecruiters
    --    FROM #AdminRequisitionBase ARB
    --    LEFT JOIN #AdminTimeToHireCalc ATTH ON ARB.RequisitionId = ATTH.RequisitionId
    --    LEFT JOIN #AdminStageTimeCalculations ASTC ON ARB.RequisitionId = ASTC.RequisitionId
    --    GROUP BY ARB.CompanyName
    --    ORDER BY ARB.CompanyName
    --    FOR JSON PATH
    --), '[]');
    SELECT @return = 
        (SELECT 
            TAV.CreatedBy, TAV.CompanyName, ISNULL(CEILING(AVG(CAST(TAV.TimeToFill_Days as FLOAT))), 0) as TimeToFill_Days,
            ISNULL(CEILING(AVG(CAST(TAV.TimeToHire_Days as FLOAT))), 0) as TimeToHire_Days, ISNULL(CEILING(AVG(CAST(TAV.PEN_Days as FLOAT))), 0) as PEN_Days,
            ISNULL(CEILING(AVG(CAST(TAV.REJ_Days as FLOAT))), 0) as REJ_Days, ISNULL(CEILING(AVG(CAST(TAV.HLD_Days as FLOAT))), 0) as HLD_Days,
            ISNULL(CEILING(AVG(CAST(TAV.PHN_Days as FLOAT))), 0) as PHN_Days, ISNULL(CEILING(AVG(CAST(TAV.URW_Days as FLOAT))), 0) as URW_Days,
            ISNULL(CEILING(AVG(CAST(TAV.INT_Days as FLOAT))), 0) as INT_Days, ISNULL(CEILING(AVG(CAST(TAV.RHM_Days as FLOAT))), 0) as RHM_Days,
            ISNULL(CEILING(AVG(CAST(TAV.DEC_Days as FLOAT))), 0) as DEC_Days, ISNULL(CEILING(AVG(CAST(TAV.NOA_Days as FLOAT))), 0) as NOA_Days,
            ISNULL(CEILING(AVG(CAST(TAV.OEX_Days as FLOAT))), 0) as OEX_Days, ISNULL(CEILING(AVG(CAST(TAV.ODC_Days as FLOAT))), 0) as ODC_Days,
            ISNULL(CEILING(AVG(CAST(TAV.HIR_Days as FLOAT))), 0) as HIR_Days, ISNULL(CEILING(AVG(CAST(TAV.WDR_Days as FLOAT))), 0) as WDR_Days,
            COUNT(DISTINCT CONCAT(TAV.RequisitionId, '-', TAV.CandidateId)) as TotalCandidates, COUNT(DISTINCT TAV.RequisitionId) as TotalRequisitions,
            COUNT(DISTINCT TAV.CreatedBy) as TotalRecruiters
        FROM 
            [dbo].[TimingAnalyticsView] TAV
        GROUP BY 
            TAV.CreatedBy, TAV.CompanyName
        ORDER BY 
            TAV.CompanyName
        FOR JSON PATH);

    SELECT ISNULL(@return, '[]');

    -- Clean up admin timing temp tables
    --DROP TABLE IF EXISTS #AdminRequisitionBase;
    --DROP TABLE IF EXISTS #AdminFirstSubmissions;
    --DROP TABLE IF EXISTS #AdminTimeToHireCalc;
    --DROP TABLE IF EXISTS #AdminStageTimings;
    --DROP TABLE IF EXISTS #AdminStageTimeCalculations;

    /***************************************************************************
    * RESULT SET 6: CANDIDATE QUALITY & CONVERSION METRICS - ALL USERS
    * PEN-to-Interview, Interview-to-Offer, Offer-to-Acceptance Ratios
    ***************************************************************************/
    
    SELECT @return =
        (SELECT 
            [CreatedBy] as [User], [Period], 
            [PEN_to_INT_Ratio] as [PEN_to_Interview_Ratio],
            [INT_to_OEX_Ratio] as [Interview_to_Offer_Ratio], 
            [OEX_to_HIR_Ratio] as [Offer_to_Acceptance_Ratio],
            [Total_Submissions], [Reached_INT], [Reached_OEX], [Reached_HIR]
        FROM 
            [dbo].[CandidateQualityMetricsView]
        ORDER BY 
            [CreatedBy],
            CASE [Period] 
                WHEN '7D' THEN 1 
                WHEN 'MTD' THEN 2 
                WHEN 'QTD' THEN 3 
                WHEN 'HYTD' THEN 4 
                WHEN 'YTD' THEN 5 
            END
        FOR JSON PATH);    

    SELECT ISNULL(@return, '[]');
END;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE   PROCEDURE [DashboardRecruiter]
    @User VARCHAR(10) = 'KEVIN'
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Today DATE = CAST(GETDATE() AS DATE);
    DECLARE @StartOfYear DATE = DATEFROMPARTS(YEAR(@Today), 1, 1);
    DECLARE @WindowStart DATE = 
        CASE 
            WHEN DATEADD(DAY, -30, @Today) > @StartOfYear THEN DATEADD(DAY, -30, @Today)
            ELSE @StartOfYear
        END
        
    ---------------------------------------------------------
    -- SET 1: Summary Counts for Graphical Dashboard
    ---------------------------------------------------------

    -- Query 1: Total Candidate Submissions by Recruiter
    SELECT
        COUNT(CASE WHEN S.CreatedDate >= DATEADD(DAY, -7, @Today) THEN 1 END) AS [Last7Days],
        COUNT(CASE WHEN S.CreatedDate >= DATEADD(DAY, -30, @Today) THEN 1 END) AS [Last30Days],
        COUNT(CASE WHEN S.CreatedDate >= DATEADD(DAY, -90, @Today) THEN 1 END) AS [Last90Days],
        COUNT(CASE WHEN S.CreatedDate >= DATEADD(DAY, -180, @Today) THEN 1 END) AS [Last180Days],
        COUNT(CASE WHEN S.CreatedDate >= @StartOfYear THEN 1 END) AS [YTD]
    FROM (
        SELECT RequisitionId, CandidateId, MIN(CreatedDate) AS CreatedDate
        FROM Submissions
        WHERE CreatedBy = @User
        GROUP BY RequisitionId, CandidateId
    ) AS S;

    -- Query 2: Submissions to Open Requisitions
    SELECT
        COUNT(CASE WHEN S.CreatedDate >= DATEADD(DAY, -7, @Today) THEN 1 END) AS [Last7Days],
        COUNT(CASE WHEN S.CreatedDate >= DATEADD(DAY, -30, @Today) THEN 1 END) AS [Last30Days],
        COUNT(CASE WHEN S.CreatedDate >= DATEADD(DAY, -90, @Today) THEN 1 END) AS [Last90Days],
        COUNT(CASE WHEN S.CreatedDate >= DATEADD(DAY, -180, @Today) THEN 1 END) AS [Last180Days],
        COUNT(CASE WHEN S.CreatedDate >= @StartOfYear THEN 1 END) AS [YTD]
    FROM (
        SELECT Sub.RequisitionId, Sub.CandidateId, MIN(Sub.CreatedDate) AS CreatedDate
        FROM Submissions Sub
        INNER JOIN Requisitions Req ON Req.Id = Sub.RequisitionId
        WHERE Sub.CreatedBy = @User
          AND Req.Status IN ('OPN', 'NEW', 'PAR')
        GROUP BY Sub.RequisitionId, Sub.CandidateId
    ) AS S;

    -- Query 3: Candidates Interviewed
    SELECT
        COUNT(CASE WHEN InterviewedOn >= DATEADD(DAY, -7, @Today) THEN 1 END) AS [Last7Days],
        COUNT(CASE WHEN InterviewedOn >= DATEADD(DAY, -30, @Today) THEN 1 END) AS [Last30Days],
        COUNT(CASE WHEN InterviewedOn >= DATEADD(DAY, -90, @Today) THEN 1 END) AS [Last90Days],
        COUNT(CASE WHEN InterviewedOn >= DATEADD(DAY, -180, @Today) THEN 1 END) AS [Last180Days],
        COUNT(CASE WHEN InterviewedOn >= @StartOfYear THEN 1 END) AS [YTD]
    FROM (
        SELECT
            S.RequisitionId,
            S.CandidateId,
            MAX(S.CreatedDate) AS InterviewedOn
        FROM Submissions S
        INNER JOIN (
            SELECT RequisitionId, CandidateId, MIN(CreatedDate) AS FirstDate
            FROM Submissions
            GROUP BY RequisitionId, CandidateId
        ) AS FirstSub ON S.RequisitionId = FirstSub.RequisitionId AND S.CandidateId = FirstSub.CandidateId
        INNER JOIN Submissions OwnerCheck
            ON OwnerCheck.RequisitionId = FirstSub.RequisitionId
            AND OwnerCheck.CandidateId = FirstSub.CandidateId
            AND OwnerCheck.CreatedDate = FirstSub.FirstDate
        WHERE S.Status = 'INT'
          AND @User IN (OwnerCheck.CreatedBy, OwnerCheck.UpdatedBy)
        GROUP BY S.RequisitionId, S.CandidateId
    ) AS Interviewed;

    -- Query 4: Offers Extended
    SELECT
        COUNT(CASE WHEN OfferedOn >= DATEADD(DAY, -7, @Today) THEN 1 END) AS [Last7Days],
        COUNT(CASE WHEN OfferedOn >= DATEADD(DAY, -30, @Today) THEN 1 END) AS [Last30Days],
        COUNT(CASE WHEN OfferedOn >= DATEADD(DAY, -90, @Today) THEN 1 END) AS [Last90Days],
        COUNT(CASE WHEN OfferedOn >= DATEADD(DAY, -180, @Today) THEN 1 END) AS [Last180Days],
        COUNT(CASE WHEN OfferedOn >= @StartOfYear THEN 1 END) AS [YTD]
    FROM (
        SELECT
            S.RequisitionId,
            S.CandidateId,
            MAX(S.CreatedDate) AS OfferedOn
        FROM Submissions S
        INNER JOIN (
            SELECT RequisitionId, CandidateId, MIN(CreatedDate) AS FirstDate
            FROM Submissions
            GROUP BY RequisitionId, CandidateId
        ) AS FirstSub ON S.RequisitionId = FirstSub.RequisitionId AND S.CandidateId = FirstSub.CandidateId
        INNER JOIN Submissions OwnerCheck
            ON OwnerCheck.RequisitionId = FirstSub.RequisitionId
            AND OwnerCheck.CandidateId = FirstSub.CandidateId
            AND OwnerCheck.CreatedDate = FirstSub.FirstDate
        WHERE S.Status = 'OEX'
          AND @User IN (OwnerCheck.CreatedBy, OwnerCheck.UpdatedBy)
        GROUP BY S.RequisitionId, S.CandidateId
    ) AS Offered;

    -- Query 5: Candidates Hired
    SELECT
        COUNT(CASE WHEN HiredOn >= DATEADD(DAY, -7, @Today) THEN 1 END) AS [Last7Days],
        COUNT(CASE WHEN HiredOn >= DATEADD(DAY, -30, @Today) THEN 1 END) AS [Last30Days],
        COUNT(CASE WHEN HiredOn >= DATEADD(DAY, -90, @Today) THEN 1 END) AS [Last90Days],
        COUNT(CASE WHEN HiredOn >= DATEADD(DAY, -180, @Today) THEN 1 END) AS [Last180Days],
        COUNT(CASE WHEN HiredOn >= @StartOfYear THEN 1 END) AS [YTD]
    FROM (
        SELECT
            S.RequisitionId,
            S.CandidateId,
            MAX(S.CreatedDate) AS HiredOn
        FROM Submissions S
        INNER JOIN (
            SELECT RequisitionId, CandidateId, MIN(CreatedDate) AS FirstDate
            FROM Submissions
            GROUP BY RequisitionId, CandidateId
        ) AS FirstSub ON S.RequisitionId = FirstSub.RequisitionId AND S.CandidateId = FirstSub.CandidateId
        INNER JOIN Submissions OwnerCheck
            ON OwnerCheck.RequisitionId = FirstSub.RequisitionId
            AND OwnerCheck.CandidateId = FirstSub.CandidateId
            AND OwnerCheck.CreatedDate = FirstSub.FirstDate
        WHERE S.Status = 'HIR'
          AND @User IN (OwnerCheck.CreatedBy, OwnerCheck.UpdatedBy)
        GROUP BY S.RequisitionId, S.CandidateId
    ) AS Hired;

    -- Query 6: Offer-to-Hire Ratio (row format)
    WITH Base AS (
        SELECT
            S.RequisitionId,
            S.CandidateId,
            S.Status,
            MAX(S.CreatedDate) AS StatusDate
        FROM Submissions S
        INNER JOIN (
            SELECT RequisitionId, CandidateId, MIN(CreatedDate) AS FirstDate
            FROM Submissions
            GROUP BY RequisitionId, CandidateId
        ) AS FirstSub ON S.RequisitionId = FirstSub.RequisitionId AND S.CandidateId = FirstSub.CandidateId
        INNER JOIN Submissions OwnerCheck
            ON OwnerCheck.RequisitionId = FirstSub.RequisitionId
            AND OwnerCheck.CandidateId = FirstSub.CandidateId
            AND OwnerCheck.CreatedDate = FirstSub.FirstDate
        WHERE S.Status IN ('OEX', 'HIR')
          AND @User IN (OwnerCheck.CreatedBy, OwnerCheck.UpdatedBy)
        GROUP BY S.RequisitionId, S.CandidateId, S.Status
    )
    SELECT 'Last7Days' AS Period,
           COUNT(CASE WHEN Status = 'OEX' AND StatusDate >= DATEADD(DAY, -7, @Today) THEN 1 END) AS OEX_Count,
           COUNT(CASE WHEN Status = 'HIR' AND StatusDate >= DATEADD(DAY, -7, @Today) THEN 1 END) AS HIR_Count,
           CAST(ISNULL(
               CAST(COUNT(CASE WHEN Status = 'HIR' AND StatusDate >= DATEADD(DAY, -7, @Today) THEN 1 END) AS DECIMAL(5,2)) /
               NULLIF(COUNT(CASE WHEN Status = 'OEX' AND StatusDate >= DATEADD(DAY, -7, @Today) THEN 1 END), 0),
           0.00) AS DECIMAL(5,2)) AS Ratio
    FROM Base;


    ---------------------------------------------------------
    -- SET 2: Requisitions grouped by Company
    ---------------------------------------------------------

    WITH FirstOwnedSubmissions AS (
        SELECT RequisitionId, CandidateId
        FROM Submissions S
        WHERE CreatedBy = @User
          AND CreatedDate >= @WindowStart
          AND CreatedDate = (
              SELECT MIN(CreatedDate)
              FROM Submissions
              WHERE RequisitionId = S.RequisitionId AND CandidateId = S.CandidateId
          )
        GROUP BY RequisitionId, CandidateId
    ),
    SubmissionDetails AS (
        SELECT
            C.CompanyName,
            C.ID AS CompanyId,
            R.Code,
            R.ID AS RequisitionId,
            R.Positions,
            R.PosTitle AS Title,
            MAX(S.CreatedDate) AS LatestSubmissionDate,
            COUNT(*) AS SubmissionCount
        FROM FirstOwnedSubmissions SC
        INNER JOIN Submissions S ON S.RequisitionId = SC.RequisitionId AND S.CandidateId = SC.CandidateId
        INNER JOIN Requisitions R ON R.ID = S.RequisitionId
        INNER JOIN Companies C ON C.ID = R.CompanyId
        WHERE S.CreatedDate >= @WindowStart
        GROUP BY C.CompanyName, C.ID, R.Code, R.ID, R.Positions, R.PosTitle
    ),
    CompanyTotals AS (
        SELECT CompanyId, SUM(SubmissionCount) AS TotalCompanySubmissions
        FROM SubmissionDetails
        GROUP BY CompanyId
    )
    SELECT 
        D.CompanyName,
        D.Code,
        D.Positions,
        D.Title,
        D.LatestSubmissionDate,
        @User AS Recruiter,
        D.SubmissionCount
    FROM SubmissionDetails D
    JOIN CompanyTotals T ON D.CompanyId = T.CompanyId
    ORDER BY T.TotalCompanySubmissions DESC, D.CompanyName ASC, D.LatestSubmissionDate DESC;

    ---------------------------------------------------------
    -- SET 3: Hired Candidates list for Recruiter
    ---------------------------------------------------------

    WITH FirstOwnedPairs AS (
        SELECT RequisitionId, CandidateId
        FROM Submissions S
        WHERE CreatedBy = @User
          AND CreatedDate = (
              SELECT MIN(CreatedDate)
              FROM Submissions
              WHERE RequisitionId = S.RequisitionId AND CandidateId = S.CandidateId
          )
        GROUP BY RequisitionId, CandidateId
    ),
    Hires AS (
        SELECT
            S.RequisitionId,
            S.CandidateId,
            MAX(S.CreatedDate) AS DateHired
        FROM Submissions S
        INNER JOIN FirstOwnedPairs F
            ON F.RequisitionId = S.RequisitionId AND F.CandidateId = S.CandidateId
        WHERE S.Status = 'HIR'
          AND S.CreatedDate >= @StartOfYear
        GROUP BY S.RequisitionId, S.CandidateId
    )
    SELECT
        C.CompanyName,
        R.Code,
        R.Positions,
        R.PosTitle AS Title,
        ISNULL(Cand.FirstName + ' ' + Cand.LastName, '') AS CandidateName,
        H.DateHired,
        CAST(0.00 AS DECIMAL(10,2)) AS SalaryOffered,
        GETDATE() AS StartDate,
        GETDATE() AS InvoiceDate,
        GETDATE() AS PaymentDate
    FROM Hires H
    INNER JOIN Requisitions R ON R.ID = H.RequisitionId
    INNER JOIN Companies C ON C.ID = R.CompanyId
    INNER JOIN Candidate Cand ON Cand.ID = H.CandidateId;

END;

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [DashboardRecruiter_Refactor]
    @User varchar(10) = 'KEVIN'
AS
BEGIN
    SET NOCOUNT ON;
    SET ARITHABORT ON;
    
    DECLARE @Today DATE = CAST(GETDATE() as date);

    SELECT 
        CreatedBy as KeyValue, FullName as Text
    FROM 
        dbo.ActiveUsersView 
    WHERE 
        CreatedBy = @User
    FOR JSON AUTO;
    
    
    /***************************************************************************
    * RESULT SET 1: SUBMISSION METRICS BY PERIOD (Refactored)
    * Uses pre-calculated SubmissionMetricsView instead of CTEs/temp tables
    ***************************************************************************/
    
    -- Single query replaces entire #BaseMetrics CTE and complex date logic
    SELECT ISNULL((
        SELECT 
            [CreatedBy] as [User], [Period], [Total_Submissions] as TotalSubmissions, [Active_Requisitions_Updated] as ActiveRequisitions, [INT_Count] as INTSubmissions,
            [OEX_Count] as OEXSubmissions, [HIR_Count] as HIRSubmissions, [OEX_HIR_Ratio] as OEXHIRRatio, [Requisitions_Created] as RequisitionsCreated
        FROM 
            [dbo].[SubmissionMetricsView]
        WHERE 
            [CreatedBy] = @User
        ORDER BY 
            CASE [Period] 
                WHEN '7D' THEN 1 
                WHEN 'MTD' THEN 2 
                WHEN 'QTD' THEN 3 
                WHEN 'HYTD' THEN 4 
                WHEN 'YTD' THEN 5 
            END
        FOR JSON PATH
    ), '[]');

    /***************************************************************************
    * RESULT SET 2: CURRENT STATUS REPORT (Keep Original Logic)
    * Query 1: Current submission status for recruiter's candidates
    ***************************************************************************/
    
    -- Current Status Report for this recruiter's submissions
    DECLARE @return varchar(max) = '[]';

    --WITH CompanySubmissionCounts AS (
    --    SELECT 
    --        C.CompanyName,
    --        COUNT(DISTINCT S.RequisitionId) as SubmissionCount
    --    FROM dbo.Submissions S
    --    INNER JOIN dbo.Requisitions R ON S.RequisitionId = R.Id
    --    INNER JOIN dbo.Companies C ON R.CompanyId = C.ID
    --    WHERE S.CreatedBy = @User
    --    GROUP BY C.CompanyName
    --),
    --CurrentStatus AS (
    --    SELECT 
    --        S.RequisitionId,
    --        S.CandidateId,
    --        S.Status as CurrentStatus,
    --        MIN(S.CreatedDate) as DateFirstSubmitted,
    --        MAX(S.CreatedDate) as LastActivityDate,
    --        STRING_AGG(ISNULL(S.Notes, ''), '; ') as ActivityNotes
    --    FROM dbo.Submissions S
    --    WHERE S.CreatedBy = @User
    --    GROUP BY S.RequisitionId, S.CandidateId, S.Status
    --)
    --SELECT ISNULL((
    --    SELECT 
    --        '[' + R.Code + '] - ' + CAST(R.Positions as varchar(5)) + ' Positions - ' + R.PosTitle as Company,
    --        LTRIM(RTRIM(ISNULL(CAND.FirstName, '') + ' ' + ISNULL(CAND.LastName, ''))) as [CandidateName],
    --        CS.CurrentStatus as [CurrentStatus],
    --        CS.DateFirstSubmitted as [DateFirstSubmitted],
    --        CS.LastActivityDate as [LastActivityDate],
    --        ISNULL(CS.ActivityNotes, '') as [ActivityNotes],
    --        R.CreatedBy [User]
    --    FROM CurrentStatus CS
    --    INNER JOIN dbo.Requisitions R ON CS.RequisitionId = R.Id
    --    INNER JOIN dbo.Companies C ON R.CompanyId = C.ID
    --    INNER JOIN dbo.Candidate CAND ON CS.CandidateId = CAND.ID
    --    INNER JOIN CompanySubmissionCounts CSC ON C.CompanyName = CSC.CompanyName
    --    ORDER BY 
    --        CSC.SubmissionCount DESC,
    --        C.CompanyName ASC,
    --        R.Id ASC,
    --        CS.DateFirstSubmitted ASC
    --    FOR JSON PATH
    --), '[]');
    SELECT @return =
        (SELECT 
            RAV.CompanyName + ' - [' + RAV.RequisitionCode + '] - ' + CAST(RAV.Positions as varchar(5)) + ' Positions - ' + RAV.RequisitionTitle as Company,
            RAV.CandidateName, RAV.CurrentStatus, RAV.DateFirstSubmitted, RAV.LastActivityDate, RAV.ActivityNotes, @User [User]
        FROM 
            [dbo].[RecentActivityView] RAV
        WHERE 
            RAV.CreatedBy = @User
        ORDER BY 
            RAV.SubmissionCount DESC, RAV.CompanyName ASC, RAV.RequisitionCode ASC, RAV.DateFirstSubmitted ASC
        FOR JSON PATH);

    SELECT ISNULL(@return, '[]');

    /***************************************************************************
    * RESULT SET 3: HIRED CANDIDATES REPORT (Keep Original Logic)
    * Query 1: Recent hires for this recruiter
    ***************************************************************************/
    
    -- Hired Candidates Report for last 3 months
    --DECLARE @StartDateSet3 DATE = DATEADD(MONTH, -3, @Today);
    
    --WITH HiredCandidates AS (
    --    SELECT 
    --        S.RequisitionId,
    --        S.CandidateId,
    --        MAX(S.CreatedDate) as DateHired
    --    FROM dbo.Submissions S
    --    INNER JOIN dbo.Requisitions R ON S.RequisitionId = R.Id
    --    WHERE S.Status = 'HIR' AND S.CreatedBy = @User
    --    GROUP BY S.RequisitionId, S.CandidateId, R.CreatedBy
    --    HAVING MAX(S.CreatedDate) >= @StartDateSet3
    --)
    --SELECT ISNULL((
    --    SELECT 
    --        C.CompanyName as Company,
    --        R.Code as [RequisitionNumber],
    --        R.Positions as [NumPosition],
    --        R.PosTitle as Title,
    --        LTRIM(RTRIM(ISNULL(CAND.FirstName, '') + ' ' + ISNULL(CAND.LastName, ''))) as [CandidateName],
    --        HC.DateHired as [DateHired],
    --        CAST(0.00 AS DECIMAL(9,2)) as [SalaryOffered],
    --        @Today as [StartDate],
    --        @Today as [DateInvoiced],
    --        @Today as [DatePaid],
    --        CAST(0.00 AS DECIMAL(9,2)) as [Placementfee],
    --        CAST(0.00 AS DECIMAL(5,2)) as [CommissionPercent],
    --        CAST(0.00 AS DECIMAL(9,2)) as [CommissionEarned],
    --        R.CreatedBy [User]
    --    FROM HiredCandidates HC
    --    INNER JOIN dbo.Requisitions R ON HC.RequisitionId = R.Id
    --    INNER JOIN dbo.Companies C ON R.CompanyId = C.ID
    --    INNER JOIN dbo.Candidate CAND ON HC.CandidateId = CAND.ID
    --    ORDER BY 
    --        R.CreatedBy,
    --        C.CompanyName,
    --        R.Code,
    --        HC.DateHired
    --    FOR JSON PATH
    --), '[]');

        SELECT @return =
            (SELECT 
                PRV.CompanyName as Company, PRV.RequisitionNumber, PRV.NumPosition, PRV.RequisitionTitle as Title, PRV.CandidateName, PRV.DateHired,
                PRV.SalaryOffered, PRV.StartDate, PRV.DateInvoiced, PRV.DatePaid, PRV.PlacementFee as [Placementfee], PRV.CommissionPercent,
                PRV.CommissionEarned, @User [User]
            FROM 
                [dbo].[PlacementReportView] PRV
            WHERE 
                PRV.CreatedBy = @User
            ORDER BY 
                PRV.CompanyName ASC, PRV.RequisitionNumber ASC, PRV.DateHired DESC
            FOR JSON PATH);

    SELECT ISNULL(@return, '[]');

    /***************************************************************************
    * RESULT SET 4: RECRUITER TIMING ANALYTICS (Keep Original Logic)
    * Query 1: Time to Fill, Time to Hire, and Time in Stage metrics (By Requisition) 
    * Query 2: Time to Fill, Time to Hire, and Time in Stage metrics (By Company)
    * Shows data for SPECIFIC USER submissions only (Recruiter perspective)
    ***************************************************************************/
    
    -- Get requisitions from last year where this user made submissions
    --DECLARE @StartDate365 DATE = DATEADD(YEAR, -1, @Today);
    
    ---- Create temp tables for recruiter timing analytics
    --DROP TABLE IF EXISTS #RecruiterRequisitionBase;
    --DROP TABLE IF EXISTS #RecruiterFirstSubmissions;
    --DROP TABLE IF EXISTS #RecruiterTimeToHireCalc;
    --DROP TABLE IF EXISTS #RecruiterStageTimings;
    --DROP TABLE IF EXISTS #RecruiterStageTimeCalculations;
    
    ---- Temp table 1: Get requisitions where this user made submissions in last year
    --SELECT DISTINCT
    --    R.Id as RequisitionId,
    --    R.Code as RequisitionCode,
    --    ISNULL(C.CompanyName, 'Unknown Company') as CompanyName,
    --    R.PosTitle as Title,
    --    R.CreatedDate as RequisitionCreatedDate,
    --    S.CreatedBy as SubmissionCreatedBy,
    --    -- Time to Fill calculation (only for FUL status)
    --    CASE 
    --        WHEN R.Status = 'FUL' THEN DATEDIFF(DAY, R.CreatedDate, R.UpdatedDate)
    --        ELSE NULL 
    --    END as TimeToFill
    --INTO #RecruiterRequisitionBase
    --FROM Submissions S
    --INNER JOIN Requisitions R ON S.RequisitionId = R.Id
    --LEFT JOIN Companies C ON R.CompanyId = C.ID
    --WHERE S.CreatedBy = @User 
    --    AND CAST(S.CreatedDate AS DATE) >= @StartDate365;
    
    ---- Temp table 2: Get first submission date for each candidate+requisition combo by this user
    --SELECT 
    --    S.RequisitionId,
    --    S.CandidateId,
    --    MIN(S.CreatedDate) as FirstSubmissionDate
    --INTO #RecruiterFirstSubmissions
    --FROM Submissions S
    --INNER JOIN #RecruiterRequisitionBase RRB ON S.RequisitionId = RRB.RequisitionId
    --WHERE S.CreatedBy = @User
    --GROUP BY S.RequisitionId, S.CandidateId;
    
    ---- Temp table 3: Calculate Time to Hire for candidates submitted by this user
    --SELECT 
    --    RFS.RequisitionId,
    --    RFS.CandidateId,
    --    RFS.FirstSubmissionDate,
    --    MAX(CASE WHEN S.Status = 'HIR' THEN S.CreatedDate END) as HireDate,
    --    MAX(CASE WHEN S.Status = 'HIR' THEN DATEDIFF(DAY, RFS.FirstSubmissionDate, S.CreatedDate) END) as TimeToHire
    --INTO #RecruiterTimeToHireCalc
    --FROM #RecruiterFirstSubmissions RFS
    --INNER JOIN Submissions S ON RFS.RequisitionId = S.RequisitionId AND RFS.CandidateId = S.CandidateId
    --GROUP BY RFS.RequisitionId, RFS.CandidateId, RFS.FirstSubmissionDate;
    
    ---- Temp table 4: Get stage timing for each submission by this user
    --SELECT 
    --    RFS.RequisitionId,
    --    RFS.CandidateId,
    --    MAX(CASE WHEN S.Status = 'PEN' THEN S.CreatedDate END) as PEN_Date,
    --    MAX(CASE WHEN S.Status = 'REJ' THEN S.CreatedDate END) as REJ_Date,
    --    MAX(CASE WHEN S.Status = 'HLD' THEN S.CreatedDate END) as HLD_Date,
    --    MAX(CASE WHEN S.Status = 'PHN' THEN S.CreatedDate END) as PHN_Date,
    --    MAX(CASE WHEN S.Status = 'URW' THEN S.CreatedDate END) as URW_Date,
    --    MAX(CASE WHEN S.Status = 'INT' THEN S.CreatedDate END) as INT_Date,
    --    MAX(CASE WHEN S.Status = 'RHM' THEN S.CreatedDate END) as RHM_Date,
    --    MAX(CASE WHEN S.Status = 'DEC' THEN S.CreatedDate END) as DEC_Date,
    --    MAX(CASE WHEN S.Status = 'NOA' THEN S.CreatedDate END) as NOA_Date,
    --    MAX(CASE WHEN S.Status = 'OEX' THEN S.CreatedDate END) as OEX_Date,
    --    MAX(CASE WHEN S.Status = 'ODC' THEN S.CreatedDate END) as ODC_Date,
    --    MAX(CASE WHEN S.Status = 'HIR' THEN S.CreatedDate END) as HIR_Date,
    --    MAX(CASE WHEN S.Status = 'WDR' THEN S.CreatedDate END) as WDR_Date
    --INTO #RecruiterStageTimings
    --FROM #RecruiterFirstSubmissions RFS
    --INNER JOIN Submissions S ON RFS.RequisitionId = S.RequisitionId AND RFS.CandidateId = S.CandidateId
    --GROUP BY RFS.RequisitionId, RFS.CandidateId;
    
    ---- Temp table 5: Calculate days spent in each stage
    --SELECT 
    --    RST.RequisitionId,
    --    RST.CandidateId,
    --    CASE 
    --        WHEN RST.PEN_Date IS NULL THEN 0
    --        WHEN RST.REJ_Date IS NOT NULL AND RST.REJ_Date > RST.PEN_Date THEN DATEDIFF(DAY, RST.PEN_Date, RST.REJ_Date)
    --        WHEN RST.HLD_Date IS NOT NULL AND RST.HLD_Date > RST.PEN_Date THEN DATEDIFF(DAY, RST.PEN_Date, RST.HLD_Date)
    --        WHEN RST.PHN_Date IS NOT NULL AND RST.PHN_Date > RST.PEN_Date THEN DATEDIFF(DAY, RST.PEN_Date, RST.PHN_Date)
    --        ELSE DATEDIFF(DAY, RST.PEN_Date, @Today)
    --    END as PEN_Days,
        
    --    CASE 
    --        WHEN RST.REJ_Date IS NULL THEN 0
    --        ELSE 0 -- REJ is final stage
    --    END as REJ_Days,
        
    --    CASE 
    --        WHEN RST.HLD_Date IS NULL THEN 0
    --        WHEN RST.PHN_Date IS NOT NULL AND RST.PHN_Date > RST.HLD_Date THEN DATEDIFF(DAY, RST.HLD_Date, RST.PHN_Date)
    --        WHEN RST.URW_Date IS NOT NULL AND RST.URW_Date > RST.HLD_Date THEN DATEDIFF(DAY, RST.HLD_Date, RST.URW_Date)
    --        ELSE DATEDIFF(DAY, RST.HLD_Date, @Today)
    --    END as HLD_Days,
        
    --    CASE 
    --        WHEN RST.PHN_Date IS NULL THEN 0
    --        WHEN RST.URW_Date IS NOT NULL AND RST.URW_Date > RST.PHN_Date THEN DATEDIFF(DAY, RST.PHN_Date, RST.URW_Date)
    --        WHEN RST.INT_Date IS NOT NULL AND RST.INT_Date > RST.PHN_Date THEN DATEDIFF(DAY, RST.PHN_Date, RST.INT_Date)
    --        ELSE DATEDIFF(DAY, RST.PHN_Date, @Today)
    --    END as PHN_Days,
        
    --    CASE 
    --        WHEN RST.URW_Date IS NULL THEN 0
    --        WHEN RST.INT_Date IS NOT NULL AND RST.INT_Date > RST.URW_Date THEN DATEDIFF(DAY, RST.URW_Date, RST.INT_Date)
    --        WHEN RST.RHM_Date IS NOT NULL AND RST.RHM_Date > RST.URW_Date THEN DATEDIFF(DAY, RST.URW_Date, RST.RHM_Date)
    --        ELSE DATEDIFF(DAY, RST.URW_Date, @Today)
    --    END as URW_Days,
        
    --    CASE 
    --        WHEN RST.INT_Date IS NULL THEN 0
    --        WHEN RST.RHM_Date IS NOT NULL AND RST.RHM_Date > RST.INT_Date THEN DATEDIFF(DAY, RST.INT_Date, RST.RHM_Date)
    --        WHEN RST.DEC_Date IS NOT NULL AND RST.DEC_Date > RST.INT_Date THEN DATEDIFF(DAY, RST.INT_Date, RST.DEC_Date)
    --        ELSE DATEDIFF(DAY, RST.INT_Date, @Today)
    --    END as INT_Days,
        
    --    CASE 
    --        WHEN RST.RHM_Date IS NULL THEN 0
    --        WHEN RST.DEC_Date IS NOT NULL AND RST.DEC_Date > RST.RHM_Date THEN DATEDIFF(DAY, RST.RHM_Date, RST.DEC_Date)
    --        WHEN RST.NOA_Date IS NOT NULL AND RST.NOA_Date > RST.RHM_Date THEN DATEDIFF(DAY, RST.RHM_Date, RST.NOA_Date)
    --        ELSE DATEDIFF(DAY, RST.RHM_Date, @Today)
    --    END as RHM_Days,
        
    --    CASE 
    --        WHEN RST.DEC_Date IS NULL THEN 0
    --        WHEN RST.NOA_Date IS NOT NULL AND RST.NOA_Date > RST.DEC_Date THEN DATEDIFF(DAY, RST.DEC_Date, RST.NOA_Date)
    --        WHEN RST.OEX_Date IS NOT NULL AND RST.OEX_Date > RST.DEC_Date THEN DATEDIFF(DAY, RST.DEC_Date, RST.OEX_Date)
    --        ELSE DATEDIFF(DAY, RST.DEC_Date, @Today)
    --    END as DEC_Days,
        
    --    CASE 
    --        WHEN RST.NOA_Date IS NULL THEN 0
    --        WHEN RST.OEX_Date IS NOT NULL AND RST.OEX_Date > RST.NOA_Date THEN DATEDIFF(DAY, RST.NOA_Date, RST.OEX_Date)
    --        WHEN RST.ODC_Date IS NOT NULL AND RST.ODC_Date > RST.NOA_Date THEN DATEDIFF(DAY, RST.NOA_Date, RST.ODC_Date)
    --        ELSE DATEDIFF(DAY, RST.NOA_Date, @Today)
    --    END as NOA_Days,
        
    --    CASE 
    --        WHEN RST.OEX_Date IS NULL THEN 0
    --        WHEN RST.HIR_Date IS NOT NULL AND RST.HIR_Date > RST.OEX_Date THEN DATEDIFF(DAY, RST.OEX_Date, RST.HIR_Date)
    --        WHEN RST.ODC_Date IS NOT NULL AND RST.ODC_Date > RST.OEX_Date THEN DATEDIFF(DAY, RST.OEX_Date, RST.ODC_Date)
    --        WHEN RST.WDR_Date IS NOT NULL AND RST.WDR_Date > RST.OEX_Date THEN DATEDIFF(DAY, RST.OEX_Date, RST.WDR_Date)
    --        ELSE DATEDIFF(DAY, RST.OEX_Date, @Today)
    --    END as OEX_Days,
        
    --    CASE 
    --        WHEN RST.ODC_Date IS NULL THEN 0
    --        ELSE 0 -- ODC is final stage
    --    END as ODC_Days,
        
    --    CASE 
    --        WHEN RST.HIR_Date IS NULL THEN 0
    --        ELSE 0 -- HIR is final stage
    --    END as HIR_Days,
        
    --    CASE 
    --        WHEN RST.WDR_Date IS NULL THEN 0
    --        ELSE 0 -- WDR is final stage
    --    END as WDR_Days
        
    --INTO #RecruiterStageTimeCalculations
    --FROM #RecruiterStageTimings RST;
    
    -- Query 1: Timing Analytics by Requisition (Recruiter view - user filtered)
    SELECT @return = 
        (SELECT 
            @User as [User], TAV.RequisitionCode, TAV.CompanyName, TAV.Title, ISNULL(CEILING(AVG(CAST(TAV.TimeToFill_Days as FLOAT))), 0) as TimeToFill_Days,
            ISNULL(CEILING(AVG(CAST(TAV.TimeToHire_Days as FLOAT))), 0) as TimeToHire_Days, ISNULL(CEILING(AVG(CAST(TAV.PEN_Days as FLOAT))), 0) as PEN_Days,
            ISNULL(CEILING(AVG(CAST(TAV.REJ_Days as FLOAT))), 0) as REJ_Days, ISNULL(CEILING(AVG(CAST(TAV.HLD_Days as FLOAT))), 0) as HLD_Days,
            ISNULL(CEILING(AVG(CAST(TAV.PHN_Days as FLOAT))), 0) as PHN_Days, ISNULL(CEILING(AVG(CAST(TAV.URW_Days as FLOAT))), 0) as URW_Days,
            ISNULL(CEILING(AVG(CAST(TAV.INT_Days as FLOAT))), 0) as INT_Days, ISNULL(CEILING(AVG(CAST(TAV.RHM_Days as FLOAT))), 0) as RHM_Days,
            ISNULL(CEILING(AVG(CAST(TAV.DEC_Days as FLOAT))), 0) as DEC_Days, ISNULL(CEILING(AVG(CAST(TAV.NOA_Days as FLOAT))), 0) as NOA_Days,
            ISNULL(CEILING(AVG(CAST(TAV.OEX_Days as FLOAT))), 0) as OEX_Days, ISNULL(CEILING(AVG(CAST(TAV.ODC_Days as FLOAT))), 0) as ODC_Days,
            ISNULL(CEILING(AVG(CAST(TAV.HIR_Days as FLOAT))), 0) as HIR_Days, ISNULL(CEILING(AVG(CAST(TAV.WDR_Days as FLOAT))), 0) as WDR_Days,
            COUNT(DISTINCT CONCAT(TAV.RequisitionId, '-', TAV.CandidateId)) as TotalCandidates
        FROM 
            [dbo].[TimingAnalyticsView] TAV
        WHERE 
            TAV.CreatedBy = @User AND TAV.Context = 'RECRUITER'
        GROUP BY 
            TAV.RequisitionCode, TAV.CompanyName, TAV.Title
        ORDER BY 
            TAV.RequisitionCode
        FOR JSON PATH);

    SELECT ISNULL(@return, '[]');
    
    -- Query 2: Timing Analytics by Company (Recruiter view - user filtered)
    --SELECT ISNULL((
    --    SELECT 
    --        @User as [User],
    --        RRB.CompanyName,
    --        -- Average Time to Fill for this company (if filled)
    --        ISNULL(CEILING(AVG(CAST(RRB.TimeToFill as FLOAT))), 0) as TimeToFill_Days,
    --        -- Average Time to Hire for this company  
    --        ISNULL(CEILING(AVG(CAST(RTTH.TimeToHire as FLOAT))), 0) as TimeToHire_Days,
    --        -- Average time in each stage for this company (rounded up to next integer)
    --        ISNULL(CEILING(AVG(CAST(RSTC.PEN_Days as FLOAT))), 0) as PEN_Days,
    --        ISNULL(CEILING(AVG(CAST(RSTC.REJ_Days as FLOAT))), 0) as REJ_Days,
    --        ISNULL(CEILING(AVG(CAST(RSTC.HLD_Days as FLOAT))), 0) as HLD_Days,
    --        ISNULL(CEILING(AVG(CAST(RSTC.PHN_Days as FLOAT))), 0) as PHN_Days,
    --        ISNULL(CEILING(AVG(CAST(RSTC.URW_Days as FLOAT))), 0) as URW_Days,
    --        ISNULL(CEILING(AVG(CAST(RSTC.INT_Days as FLOAT))), 0) as INT_Days,
    --        ISNULL(CEILING(AVG(CAST(RSTC.RHM_Days as FLOAT))), 0) as RHM_Days,
    --        ISNULL(CEILING(AVG(CAST(RSTC.DEC_Days as FLOAT))), 0) as DEC_Days,
    --        ISNULL(CEILING(AVG(CAST(RSTC.NOA_Days as FLOAT))), 0) as NOA_Days,
    --        ISNULL(CEILING(AVG(CAST(RSTC.OEX_Days as FLOAT))), 0) as OEX_Days,
    --        ISNULL(CEILING(AVG(CAST(RSTC.ODC_Days as FLOAT))), 0) as ODC_Days,
    --        ISNULL(CEILING(AVG(CAST(RSTC.HIR_Days as FLOAT))), 0) as HIR_Days,
    --        ISNULL(CEILING(AVG(CAST(RSTC.WDR_Days as FLOAT))), 0) as WDR_Days,
    --        COUNT(DISTINCT CONCAT(RSTC.RequisitionId, '-', RSTC.CandidateId)) as TotalCandidates,
    --        COUNT(DISTINCT RRB.RequisitionId) as TotalRequisitions
    --    FROM #RecruiterRequisitionBase RRB
    --    LEFT JOIN #RecruiterTimeToHireCalc RTTH ON RRB.RequisitionId = RTTH.RequisitionId
    --    LEFT JOIN #RecruiterStageTimeCalculations RSTC ON RRB.RequisitionId = RSTC.RequisitionId
    --    GROUP BY RRB.CompanyName
    --    ORDER BY RRB.CompanyName
    --    FOR JSON PATH
    --), '[]');
    SELECT @return = 
        (SELECT 
            @User as [User], TAV.CompanyName, ISNULL(CEILING(AVG(CAST(TAV.TimeToFill_Days as FLOAT))), 0) as TimeToFill_Days,
            ISNULL(CEILING(AVG(CAST(TAV.TimeToHire_Days as FLOAT))), 0) as TimeToHire_Days, ISNULL(CEILING(AVG(CAST(TAV.PEN_Days as FLOAT))), 0) as PEN_Days,
            ISNULL(CEILING(AVG(CAST(TAV.REJ_Days as FLOAT))), 0) as REJ_Days, ISNULL(CEILING(AVG(CAST(TAV.HLD_Days as FLOAT))), 0) as HLD_Days,
            ISNULL(CEILING(AVG(CAST(TAV.PHN_Days as FLOAT))), 0) as PHN_Days, ISNULL(CEILING(AVG(CAST(TAV.URW_Days as FLOAT))), 0) as URW_Days,
            ISNULL(CEILING(AVG(CAST(TAV.INT_Days as FLOAT))), 0) as INT_Days, ISNULL(CEILING(AVG(CAST(TAV.RHM_Days as FLOAT))), 0) as RHM_Days,
            ISNULL(CEILING(AVG(CAST(TAV.DEC_Days as FLOAT))), 0) as DEC_Days, ISNULL(CEILING(AVG(CAST(TAV.NOA_Days as FLOAT))), 0) as NOA_Days,
            ISNULL(CEILING(AVG(CAST(TAV.OEX_Days as FLOAT))), 0) as OEX_Days, ISNULL(CEILING(AVG(CAST(TAV.ODC_Days as FLOAT))), 0) as ODC_Days,
            ISNULL(CEILING(AVG(CAST(TAV.HIR_Days as FLOAT))), 0) as HIR_Days, ISNULL(CEILING(AVG(CAST(TAV.WDR_Days as FLOAT))), 0) as WDR_Days,
            COUNT(DISTINCT CONCAT(TAV.RequisitionId, '-', TAV.CandidateId)) as TotalCandidates, COUNT(DISTINCT TAV.RequisitionId) as TotalRequisitions
        FROM 
            [dbo].[TimingAnalyticsView] TAV
        WHERE 
            TAV.CreatedBy = @User AND TAV.Context = 'RECRUITER'  -- Recruiter context only
        GROUP BY 
            TAV.CompanyName
        ORDER BY 
            TAV.CompanyName
        FOR JSON PATH);

    SELECT ISNULL(@return, '[]');

    -- Clean up recruiter timing temp tables
    --DROP TABLE IF EXISTS #RecruiterRequisitionBase;
    --DROP TABLE IF EXISTS #RecruiterFirstSubmissions;
    --DROP TABLE IF EXISTS #RecruiterTimeToHireCalc;
    --DROP TABLE IF EXISTS #RecruiterStageTimings;
    --DROP TABLE IF EXISTS #RecruiterStageTimeCalculations;
    /***************************************************************************
    * RESULT SET 6: CANDIDATE QUALITY & CONVERSION METRICS
    * PEN-to-Interview, Interview-to-Offer, Offer-to-Acceptance Ratios
    ***************************************************************************/
    
    SELECT ISNULL((
        SELECT 
            [CreatedBy] as [User], [Period], 
            [PEN_to_INT_Ratio] as [PEN_to_Interview_Ratio],
            [INT_to_OEX_Ratio] as [Interview_to_Offer_Ratio], 
            [OEX_to_HIR_Ratio] as [Offer_to_Acceptance_Ratio],
            [Total_Submissions], [Reached_INT], [Reached_OEX], [Reached_HIR]
        FROM 
            [dbo].[CandidateQualityMetricsView]
        WHERE 
            [CreatedBy] = @User
        ORDER BY 
            CASE [Period] 
                WHEN '7D' THEN 1 
                WHEN 'MTD' THEN 2 
                WHEN 'QTD' THEN 3 
                WHEN 'HYTD' THEN 4 
                WHEN 'YTD' THEN 5 
            END
        FOR JSON PATH
    ), '[]');
    
END;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [DashboardRecruiters]
    @User VARCHAR(10) = NULL
AS
BEGIN
	SET NOCOUNT ON;
	SET ARITHABORT ON;
	SET ANSI_NULLS ON;
	SET QUOTED_IDENTIFIER ON;
    
    --DECLARE @User varchar(10);
    -- Set default user if not provided
    IF @User IS NULL
        SET @User = 'KEVIN';
    
    -- Generic Query for Dropdown (Memory-Optimized)
    SELECT 
        CreatedBy as KeyValue, FullName as Text
    FROM 
        dbo.ActiveUsersView 
    WHERE 
        CreatedBy = @User
    FOR JSON AUTO;
    
    -- Common date variables
    DECLARE @Today DATE = '2025-06-30';--CAST(GETDATE() as date);
    DECLARE @YearStart DATE = DATEFROMPARTS(YEAR(@Today), 1, 1);
    
    -- Get the earliest submission date for boundary checking
    DECLARE @MinDate DATE = (SELECT MIN(CAST(CreatedDate AS DATE)) FROM Submissions WHERE CreatedBy = @User);
    
    -- Adjust YearStart if needed based on MinDate
    SET @YearStart = CASE 
        WHEN @MinDate IS NULL OR @MinDate > @YearStart THEN @MinDate
        ELSE @YearStart
    END;
    
    -- Common date calculations
    DECLARE @StartDate7D DATE = DATEADD(DAY, -6, @Today);
    DECLARE @StartDateMTD DATE = DATEADD(DAY, 1, EOMONTH(@Today, -1));
    DECLARE @StartDateQTD DATE = DATEADD(QUARTER, DATEDIFF(QUARTER, 0, @Today), 0);
    DECLARE @StartDateHYTD DATE = CASE 
        WHEN MONTH(@Today) <= 6 THEN DATEFROMPARTS(YEAR(@Today), 1, 1)
        ELSE DATEFROMPARTS(YEAR(@Today), 7, 1)
    END;
    
    -- =====================================================
    -- SET 1: Submission Pipeline Metrics (Cached with Temp Tables)
    -- =====================================================
    
    -- Create temp table for base submission/requisition data
    DROP TABLE IF EXISTS #BaseMetrics;
    
    SELECT 
        @User as [User],
        -- Submission counts
        ISNULL(SUM(CASE WHEN CAST(S.CreatedDate AS DATE) BETWEEN @StartDate7D AND @Today THEN 1 ELSE 0 END), 0) as SUB_LAST7D_COUNT,
        ISNULL(SUM(CASE WHEN CAST(S.CreatedDate AS DATE) BETWEEN @StartDateMTD AND @Today THEN 1 ELSE 0 END), 0) as SUB_MTD_COUNT,
        ISNULL(SUM(CASE WHEN CAST(S.CreatedDate AS DATE) BETWEEN @StartDateQTD AND @Today THEN 1 ELSE 0 END), 0) as SUB_QTD_COUNT,
        ISNULL(SUM(CASE WHEN CAST(S.CreatedDate AS DATE) BETWEEN @StartDateHYTD AND @Today THEN 1 ELSE 0 END), 0) as SUB_HYTD_COUNT,
        ISNULL(SUM(CASE WHEN CAST(S.CreatedDate AS DATE) BETWEEN @YearStart AND @Today THEN 1 ELSE 0 END), 0) as SUB_YTD_COUNT,
        -- Active requisition counts
        ISNULL(COUNT(DISTINCT CASE WHEN CAST(S.CreatedDate AS DATE) BETWEEN @StartDate7D AND @Today AND R.Status IN ('NEW', 'OPN', 'PAR') THEN S.RequisitionId END), 0) as REQ_LAST7D_COUNT,
        ISNULL(COUNT(DISTINCT CASE WHEN CAST(S.CreatedDate AS DATE) BETWEEN @StartDateMTD AND @Today AND R.Status IN ('NEW', 'OPN', 'PAR') THEN S.RequisitionId END), 0) as REQ_MTD_COUNT,
        ISNULL(COUNT(DISTINCT CASE WHEN CAST(S.CreatedDate AS DATE) BETWEEN @StartDateQTD AND @Today AND R.Status IN ('NEW', 'OPN', 'PAR') THEN S.RequisitionId END), 0) as REQ_QTD_COUNT,
        ISNULL(COUNT(DISTINCT CASE WHEN CAST(S.CreatedDate AS DATE) BETWEEN @StartDateHYTD AND @Today AND R.Status IN ('NEW', 'OPN', 'PAR') THEN S.RequisitionId END), 0) as REQ_HYTD_COUNT,
        ISNULL(COUNT(DISTINCT CASE WHEN CAST(S.CreatedDate AS DATE) BETWEEN @YearStart AND @Today AND R.Status IN ('NEW', 'OPN', 'PAR') THEN S.RequisitionId END), 0) as REQ_YTD_COUNT
    INTO #BaseMetrics
    FROM (SELECT 1 as dummy) d -- Ensures we always have at least one row
    LEFT JOIN Submissions S ON S.CreatedBy = @User
    LEFT JOIN Requisitions R ON S.RequisitionId = R.Id;
    
    -- Query 1: Total Submissions by Recruiter
    SELECT [User], SUB_LAST7D_COUNT as LAST7D_COUNT, SUB_MTD_COUNT as MTD_COUNT, SUB_QTD_COUNT as QTD_COUNT, SUB_HYTD_COUNT as HYTD_COUNT, SUB_YTD_COUNT as YTD_COUNT
    FROM #BaseMetrics FOR JSON PATH;--, WITHOUT_ARRAY_WRAPPER;
    
    -- Query 2: Active Requisitions Worked On
    SELECT [User], REQ_LAST7D_COUNT as LAST7D_COUNT, REQ_MTD_COUNT as MTD_COUNT, REQ_QTD_COUNT as QTD_COUNT, REQ_HYTD_COUNT as HYTD_COUNT, REQ_YTD_COUNT as YTD_COUNT
    FROM #BaseMetrics FOR JSON PATH;--, WITHOUT_ARRAY_WRAPPER;
    
    -- Create temp table for status-based queries (3-6)
    DROP TABLE IF EXISTS #RecruiterSubmissions;
    
    WITH FirstSubmissions AS (
        SELECT 
            RequisitionId,
            CandidateId,
            MIN(CreatedDate) as FirstSubmissionDate
        FROM Submissions
        GROUP BY RequisitionId, CandidateId
        HAVING MIN(CASE WHEN CreatedBy = @User THEN CreatedDate END) = MIN(CreatedDate)
    )
    SELECT 
        FS.RequisitionId,
        FS.CandidateId,
        S.Status,
        MAX(S.CreatedDate) as LatestStatusDate
    INTO #RecruiterSubmissions
    FROM FirstSubmissions FS
    INNER JOIN Submissions S 
        ON FS.RequisitionId = S.RequisitionId 
        AND FS.CandidateId = S.CandidateId
        AND S.Status IN ('INT', 'OEX', 'HIR')
    GROUP BY FS.RequisitionId, FS.CandidateId, S.Status;
    
    -- Create temp table for status-based data to avoid repetition
    DROP TABLE IF EXISTS #StatusMetrics;
    
    SELECT 
        @User as [User],
        -- Interview counts
        ISNULL(SUM(CASE WHEN CAST(RS.LatestStatusDate AS DATE) BETWEEN @StartDate7D AND @Today AND RS.Status = 'INT' THEN 1 ELSE 0 END), 0) as INT_LAST7D_COUNT,
        ISNULL(SUM(CASE WHEN CAST(RS.LatestStatusDate AS DATE) BETWEEN @StartDateMTD AND @Today AND RS.Status = 'INT' THEN 1 ELSE 0 END), 0) as INT_MTD_COUNT,
        ISNULL(SUM(CASE WHEN CAST(RS.LatestStatusDate AS DATE) BETWEEN @StartDateQTD AND @Today AND RS.Status = 'INT' THEN 1 ELSE 0 END), 0) as INT_QTD_COUNT,
        ISNULL(SUM(CASE WHEN CAST(RS.LatestStatusDate AS DATE) BETWEEN @StartDateHYTD AND @Today AND RS.Status = 'INT' THEN 1 ELSE 0 END), 0) as INT_HYTD_COUNT,
        ISNULL(SUM(CASE WHEN CAST(RS.LatestStatusDate AS DATE) BETWEEN @YearStart AND @Today AND RS.Status = 'INT' THEN 1 ELSE 0 END), 0) as INT_YTD_COUNT,
        -- Offer counts
        ISNULL(SUM(CASE WHEN CAST(RS.LatestStatusDate AS DATE) BETWEEN @StartDate7D AND @Today AND RS.Status = 'OEX' THEN 1 ELSE 0 END), 0) as OEX_LAST7D_COUNT,
        ISNULL(SUM(CASE WHEN CAST(RS.LatestStatusDate AS DATE) BETWEEN @StartDateMTD AND @Today AND RS.Status = 'OEX' THEN 1 ELSE 0 END), 0) as OEX_MTD_COUNT,
        ISNULL(SUM(CASE WHEN CAST(RS.LatestStatusDate AS DATE) BETWEEN @StartDateQTD AND @Today AND RS.Status = 'OEX' THEN 1 ELSE 0 END), 0) as OEX_QTD_COUNT,
        ISNULL(SUM(CASE WHEN CAST(RS.LatestStatusDate AS DATE) BETWEEN @StartDateHYTD AND @Today AND RS.Status = 'OEX' THEN 1 ELSE 0 END), 0) as OEX_HYTD_COUNT,
        ISNULL(SUM(CASE WHEN CAST(RS.LatestStatusDate AS DATE) BETWEEN @YearStart AND @Today AND RS.Status = 'OEX' THEN 1 ELSE 0 END), 0) as OEX_YTD_COUNT,
        -- Hire counts
        ISNULL(SUM(CASE WHEN CAST(RS.LatestStatusDate AS DATE) BETWEEN @StartDate7D AND @Today AND RS.Status = 'HIR' THEN 1 ELSE 0 END), 0) as HIR_LAST7D_COUNT,
        ISNULL(SUM(CASE WHEN CAST(RS.LatestStatusDate AS DATE) BETWEEN @StartDateMTD AND @Today AND RS.Status = 'HIR' THEN 1 ELSE 0 END), 0) as HIR_MTD_COUNT,
        ISNULL(SUM(CASE WHEN CAST(RS.LatestStatusDate AS DATE) BETWEEN @StartDateQTD AND @Today AND RS.Status = 'HIR' THEN 1 ELSE 0 END), 0) as HIR_QTD_COUNT,
        ISNULL(SUM(CASE WHEN CAST(RS.LatestStatusDate AS DATE) BETWEEN @StartDateHYTD AND @Today AND RS.Status = 'HIR' THEN 1 ELSE 0 END), 0) as HIR_HYTD_COUNT,
        ISNULL(SUM(CASE WHEN CAST(RS.LatestStatusDate AS DATE) BETWEEN @YearStart AND @Today AND RS.Status = 'HIR' THEN 1 ELSE 0 END), 0) as HIR_YTD_COUNT
    INTO #StatusMetrics
    FROM (SELECT 1 as dummy) d -- Ensures we always have at least one row
    LEFT JOIN #RecruiterSubmissions RS ON 1=1;
    
    -- Query 3: Interview Status
    SELECT [User], INT_LAST7D_COUNT as LAST7D_COUNT, INT_MTD_COUNT as MTD_COUNT, INT_QTD_COUNT as QTD_COUNT, INT_HYTD_COUNT as HYTD_COUNT, INT_YTD_COUNT as YTD_COUNT
    FROM #StatusMetrics FOR JSON PATH;--, WITHOUT_ARRAY_WRAPPER;
    
    -- Query 4: Offer Extended Status
    SELECT [User], OEX_LAST7D_COUNT as LAST7D_COUNT, OEX_MTD_COUNT as MTD_COUNT, OEX_QTD_COUNT as QTD_COUNT, OEX_HYTD_COUNT as HYTD_COUNT, OEX_YTD_COUNT as YTD_COUNT
    FROM #StatusMetrics FOR JSON PATH;--, WITHOUT_ARRAY_WRAPPER;
    
    -- Query 5: Hired Status
    SELECT [User], HIR_LAST7D_COUNT as LAST7D_COUNT, HIR_MTD_COUNT as MTD_COUNT, HIR_QTD_COUNT as QTD_COUNT, HIR_HYTD_COUNT as HYTD_COUNT, HIR_YTD_COUNT as YTD_COUNT
    FROM #StatusMetrics FOR JSON PATH;--, WITHOUT_ARRAY_WRAPPER;
    
    -- Query 6: OEX to HIR Conversion Ratio
    SELECT 
        [User],
        CASE WHEN OEX_LAST7D_COUNT = 0 THEN 0.00 ELSE CAST(HIR_LAST7D_COUNT AS DECIMAL(10,2)) / CAST(OEX_LAST7D_COUNT AS DECIMAL(10,2)) END as LAST7D_RATIO,
        CASE WHEN OEX_MTD_COUNT = 0 THEN 0.00 ELSE CAST(HIR_MTD_COUNT AS DECIMAL(10,2)) / CAST(OEX_MTD_COUNT AS DECIMAL(10,2)) END as MTD_RATIO,
        CASE WHEN OEX_QTD_COUNT = 0 THEN 0.00 ELSE CAST(HIR_QTD_COUNT AS DECIMAL(10,2)) / CAST(OEX_QTD_COUNT AS DECIMAL(10,2)) END as QTD_RATIO,
        CASE WHEN OEX_HYTD_COUNT = 0 THEN 0.00 ELSE CAST(HIR_HYTD_COUNT AS DECIMAL(10,2)) / CAST(OEX_HYTD_COUNT AS DECIMAL(10,2)) END as HYTD_RATIO,
        CASE WHEN OEX_YTD_COUNT = 0 THEN 0.00 ELSE CAST(HIR_YTD_COUNT AS DECIMAL(10,2)) / CAST(OEX_YTD_COUNT AS DECIMAL(10,2)) END as YTD_RATIO
    FROM #StatusMetrics FOR JSON PATH;--, WITHOUT_ARRAY_WRAPPER;
    
    DROP TABLE #RecruiterSubmissions;
    DROP TABLE #BaseMetrics;
    DROP TABLE #StatusMetrics;
    
    -- Query 1: Requisitions Created
    SELECT 
        CreatedBy as [User],
        MAX(CASE WHEN Period = '7D' THEN Total_Submissions ELSE 0 END) as LAST7D_COUNT,
        MAX(CASE WHEN Period = 'MTD' THEN Total_Submissions ELSE 0 END) as MTD_COUNT,
        MAX(CASE WHEN Period = 'QTD' THEN Total_Submissions ELSE 0 END) as QTD_COUNT,
        MAX(CASE WHEN Period = 'HYTD' THEN Total_Submissions ELSE 0 END) as HYTD_COUNT,
        MAX(CASE WHEN Period = 'YTD' THEN Total_Submissions ELSE 0 END) as YTD_COUNT
    FROM dbo.SubmissionMetricsView WHERE MetricDate = @Today AND CreatedBy = @User
    GROUP BY CreatedBy ORDER BY CreatedBy FOR JSON PATH;

    -- Query 2: Active Requisitions
    SELECT 
        CreatedBy as [User],
        MAX(CASE WHEN Period = '7D' THEN Active_Requisitions_Updated ELSE 0 END) as LAST7D_COUNT,
        MAX(CASE WHEN Period = 'MTD' THEN Active_Requisitions_Updated ELSE 0 END) as MTD_COUNT,
        MAX(CASE WHEN Period = 'QTD' THEN Active_Requisitions_Updated ELSE 0 END) as QTD_COUNT,
        MAX(CASE WHEN Period = 'HYTD' THEN Active_Requisitions_Updated ELSE 0 END) as HYTD_COUNT,
        MAX(CASE WHEN Period = 'YTD' THEN Active_Requisitions_Updated ELSE 0 END) as YTD_COUNT
    FROM dbo.SubmissionMetricsView WHERE MetricDate = @Today AND CreatedBy = @User
    GROUP BY CreatedBy ORDER BY CreatedBy FOR JSON PATH;

    -- Query 3: Interviews (INT)
    SELECT 
        CreatedBy as [User],
        MAX(CASE WHEN Period = '7D' THEN INT_Count ELSE 0 END) as LAST7D_COUNT,
        MAX(CASE WHEN Period = 'MTD' THEN INT_Count ELSE 0 END) as MTD_COUNT,
        MAX(CASE WHEN Period = 'QTD' THEN INT_Count ELSE 0 END) as QTD_COUNT,
        MAX(CASE WHEN Period = 'HYTD' THEN INT_Count ELSE 0 END) as HYTD_COUNT,
        MAX(CASE WHEN Period = 'YTD' THEN INT_Count ELSE 0 END) as YTD_COUNT
    FROM dbo.SubmissionMetricsView WHERE MetricDate = @Today AND CreatedBy = @User
    GROUP BY CreatedBy ORDER BY CreatedBy FOR JSON PATH;

    -- Query 4: Offers Extended (OEX)
    SELECT 
        CreatedBy as [User],
        MAX(CASE WHEN Period = '7D' THEN OEX_Count ELSE 0 END) as LAST7D_COUNT,
        MAX(CASE WHEN Period = 'MTD' THEN OEX_Count ELSE 0 END) as MTD_COUNT,
        MAX(CASE WHEN Period = 'QTD' THEN OEX_Count ELSE 0 END) as QTD_COUNT,
        MAX(CASE WHEN Period = 'HYTD' THEN OEX_Count ELSE 0 END) as HYTD_COUNT,
        MAX(CASE WHEN Period = 'YTD' THEN OEX_Count ELSE 0 END) as YTD_COUNT
    FROM dbo.SubmissionMetricsView WHERE MetricDate = @Today AND CreatedBy = @User
    GROUP BY CreatedBy ORDER BY CreatedBy FOR JSON PATH;

    -- Query 5: Hired (HIR)
    SELECT 
        CreatedBy as [User],
        MAX(CASE WHEN Period = '7D' THEN HIR_Count ELSE 0 END) as LAST7D_COUNT,
        MAX(CASE WHEN Period = 'MTD' THEN HIR_Count ELSE 0 END) as MTD_COUNT,
        MAX(CASE WHEN Period = 'QTD' THEN HIR_Count ELSE 0 END) as QTD_COUNT,
        MAX(CASE WHEN Period = 'HYTD' THEN HIR_Count ELSE 0 END) as HYTD_COUNT,
        MAX(CASE WHEN Period = 'YTD' THEN HIR_Count ELSE 0 END) as YTD_COUNT
    FROM dbo.SubmissionMetricsView WHERE MetricDate = @Today AND CreatedBy = @User
    GROUP BY CreatedBy ORDER BY CreatedBy FOR JSON PATH;

    -- Query 6: OEX:HIR Ratios
    SELECT 
        CreatedBy as [User],
        MAX(CASE WHEN Period = '7D' THEN OEX_HIR_Ratio ELSE 0.00 END) as LAST7D_RATIO,
        MAX(CASE WHEN Period = 'MTD' THEN OEX_HIR_Ratio ELSE 0.00 END) as MTD_RATIO,
        MAX(CASE WHEN Period = 'QTD' THEN OEX_HIR_Ratio ELSE 0.00 END) as QTD_RATIO,
        MAX(CASE WHEN Period = 'HYTD' THEN OEX_HIR_Ratio ELSE 0.00 END) as HYTD_RATIO,
        MAX(CASE WHEN Period = 'YTD' THEN OEX_HIR_Ratio ELSE 0.00 END) as YTD_RATIO
    FROM dbo.SubmissionMetricsView WHERE MetricDate = @Today AND CreatedBy = @User
    GROUP BY CreatedBy ORDER BY CreatedBy FOR JSON PATH;

    -- =====================================================
    -- SET 2: Recent Activity Detail
    -- =====================================================
    
    -- Get date range (last 30 days but not before year start)
    DECLARE @StartDate DATE = CASE 
        WHEN DATEADD(DAY, -30, @Today) < @YearStart THEN @YearStart
        ELSE DATEADD(DAY, -30, @Today)
    END;
    DECLARE @return varchar(max);
   
    WITH FirstSubmissions AS (
        -- Get submissions where user made the first submission
        SELECT 
            RequisitionId,
            CandidateId,
            MIN(CreatedDate) as FirstSubmissionDate
        FROM Submissions
        GROUP BY RequisitionId, CandidateId
        HAVING MIN(CASE WHEN CreatedBy = @User THEN CreatedDate END) = MIN(CreatedDate)
            AND MIN(CreatedDate) >= @StartDate
    ),
    LatestActivity AS (
        -- Get the latest activity for each req+candidate combo
        SELECT 
            FS.RequisitionId,
            FS.CandidateId,
            FS.FirstSubmissionDate,
            S.Status as CurrentStatus,
            S.CreatedDate as LastActivityDate,
            S.Notes as ActivityNotes
        FROM FirstSubmissions FS
        INNER JOIN Submissions S
            ON FS.RequisitionId = S.RequisitionId 
            AND FS.CandidateId = S.CandidateId
        WHERE S.CreatedDate = (
            SELECT MAX(S2.CreatedDate)
            FROM Submissions S2
            WHERE S2.RequisitionId = S.RequisitionId
                AND S2.CandidateId = S.CandidateId
        )
    ),
    CompanyCounts AS (
        -- Count submissions per company for sorting
        SELECT 
            R.CompanyId,
            COUNT(*) as SubmissionCount
        FROM LatestActivity LA
        INNER JOIN Requisitions R ON LA.RequisitionId = R.Id
        GROUP BY R.CompanyId
    )

    SELECT @return =
    (SELECT 
        TRIM(C.CompanyName + ' - [' + R.Code + '] - ' + CAST(R.Positions as varchar(5)) + ' Positions - ' + R.PosTitle) as [Company],
        LTRIM(RTRIM(ISNULL(CAND.FirstName, '') + ' ' + ISNULL(CAND.LastName, ''))) as [CandidateName],
        LA.CurrentStatus as [CurrentStatus],
        CAST(LA.FirstSubmissionDate AS DATE) as [DateFirstSubmitted],
        CAST(LA.LastActivityDate AS DATE) as [LastActivityDate],
        ISNULL(LA.ActivityNotes, '') as [ActivityNotes],
        @User as [User]
    FROM LatestActivity LA
    INNER JOIN Requisitions R ON LA.RequisitionId = R.Id
    INNER JOIN Companies C ON R.CompanyId = C.ID
    INNER JOIN Candidate CAND ON LA.CandidateId = CAND.ID
    INNER JOIN CompanyCounts CC ON R.CompanyId = CC.CompanyId
    ORDER BY 
        CC.SubmissionCount DESC,  -- Higher submission count first
        C.CompanyName ASC,        -- Alphabetically for ties
        R.Id ASC,                 -- RequisitionId within company
        LA.FirstSubmissionDate ASC
    FOR JSON PATH); -- First submission date
    
    SELECT @return;

    -- =====================================================
    -- SET 3: Placement Reporting
    -- =====================================================
    
    -- Get date range (last 3 months but not before year start)
    DECLARE @HireStartDate DATE = CASE 
        WHEN DATEADD(MONTH, -3, @Today) < @YearStart THEN @YearStart
        ELSE DATEADD(MONTH, -3, @Today)
    END;
    
    WITH FirstSubmissions AS (
        -- Get submissions where user made the first submission
        SELECT 
            RequisitionId,
            CandidateId,
            MIN(CreatedDate) as FirstSubmissionDate
        FROM Submissions
        GROUP BY RequisitionId, CandidateId
        HAVING MIN(CASE WHEN CreatedBy = @User THEN CreatedDate END) = MIN(CreatedDate)
    ),
    HiredCandidates AS (
        -- Get candidates with HIR status in last 3 months
        SELECT 
            FS.RequisitionId,
            FS.CandidateId,
            MAX(S.CreatedDate) as HireDate
        FROM FirstSubmissions FS
        INNER JOIN Submissions S
            ON FS.RequisitionId = S.RequisitionId 
            AND FS.CandidateId = S.CandidateId
            AND S.Status = 'HIR'
        WHERE CAST(S.CreatedDate AS DATE) >= @HireStartDate
        GROUP BY FS.RequisitionId, FS.CandidateId
    )

    SELECT @return = 
    (SELECT 
        C.CompanyName as Company,
        R.Code as [RequisitionNumber],
        R.Positions as [NumPosition],
        R.PosTitle as Title,
        CONCAT(CAND.FirstName, ' ', CAND.LastName) as [CandidateName],
        CAST(HC.HireDate AS DATE) as [DateHired],
        CAST(0.00 AS DECIMAL(9,2)) as [SalaryOffered],
        CAST(GETDATE() AS DATE) as [StartDate],
        CAST(GETDATE() AS DATE) as [DateInvoiced],
        CAST(GETDATE() AS DATE) as [DatePaid],
        CAST(0.00 AS DECIMAL(9,2)) as [Placementfee],
        CAST(0.00 AS DECIMAL(5,2)) as [CommissionPercent],
        CAST(0.00 AS DECIMAL(9,2)) as [CommissionEarned],
        @User [User]
    FROM HiredCandidates HC
    INNER JOIN Requisitions R ON HC.RequisitionId = R.Id
    INNER JOIN Companies C ON R.CompanyId = C.ID
    INNER JOIN Candidate CAND ON HC.CandidateId = CAND.ID
    ORDER BY 
        C.CompanyName ASC,
        R.Id ASC,
        HC.HireDate DESC
    FOR JSON PATH);
    
    SELECT @return;

    /***************************************************************************
    * RESULT SET 4: RECRUITER TIMING ANALYTICS (2 Queries)
    * Query 1: Time to Fill, Time to Hire, and Time in Stage metrics (By Requisition) 
    * Query 2: Time to Fill, Time to Hire, and Time in Stage metrics (By Company)
    * Shows data for SPECIFIC USER submissions only (Recruiter perspective)
    ***************************************************************************/
    
    -- Get requisitions from last year where this user made submissions
    DECLARE @StartDate365 DATE = DATEADD(YEAR, -1, @Today);
    
    -- Create temp tables for recruiter timing analytics
    DROP TABLE IF EXISTS #RecruiterRequisitionBase;
    DROP TABLE IF EXISTS #RecruiterFirstSubmissions;
    DROP TABLE IF EXISTS #RecruiterTimeToHireCalc;
    DROP TABLE IF EXISTS #RecruiterStageTimings;
    DROP TABLE IF EXISTS #RecruiterStageTimeCalculations;
    
    -- Temp table 1: Get requisitions where this user made submissions in last year
    SELECT DISTINCT
        R.Id as RequisitionId,
        R.Code as RequisitionCode,
        ISNULL(C.CompanyName, 'Unknown Company') as CompanyName,
        R.PosTitle as Title,
        R.CreatedDate as RequisitionCreatedDate,
        S.CreatedBy as SubmissionCreatedBy,
        -- Time to Fill calculation (only for FUL status)
        CASE 
            WHEN R.Status = 'FUL' THEN DATEDIFF(DAY, R.CreatedDate, R.UpdatedDate)
            ELSE NULL 
        END as TimeToFill
    INTO #RecruiterRequisitionBase
    FROM Submissions S
    INNER JOIN Requisitions R ON S.RequisitionId = R.Id
    LEFT JOIN Companies C ON R.CompanyId = C.ID
    WHERE S.CreatedBy = @User 
        AND CAST(S.CreatedDate AS DATE) >= @StartDate365;
    
    -- Temp table 2: Get first submission date for each candidate+requisition combo by this user
    SELECT 
        S.RequisitionId,
        S.CandidateId,
        MIN(S.CreatedDate) as FirstSubmissionDate
    INTO #RecruiterFirstSubmissions
    FROM Submissions S
    INNER JOIN #RecruiterRequisitionBase RRB ON S.RequisitionId = RRB.RequisitionId
    WHERE S.CreatedBy = @User
    GROUP BY S.RequisitionId, S.CandidateId;
    
    -- Temp table 3: Calculate Time to Hire for candidates submitted by this user
    SELECT 
        RFS.RequisitionId,
        RFS.CandidateId,
        RFS.FirstSubmissionDate,
        MAX(CASE WHEN S.Status = 'HIR' THEN S.CreatedDate END) as HireDate,
        MAX(CASE WHEN S.Status = 'HIR' THEN DATEDIFF(DAY, RFS.FirstSubmissionDate, S.CreatedDate) END) as TimeToHire
    INTO #RecruiterTimeToHireCalc
    FROM #RecruiterFirstSubmissions RFS
    INNER JOIN Submissions S ON RFS.RequisitionId = S.RequisitionId AND RFS.CandidateId = S.CandidateId
    WHERE S.CreatedBy = @User
    GROUP BY RFS.RequisitionId, RFS.CandidateId, RFS.FirstSubmissionDate;
    
    -- Temp table 4: Calculate time in each stage for candidates submitted by this user
    SELECT 
        RFS.RequisitionId,
        RFS.CandidateId,
        -- Get the latest date for each stage (in case of multiple records for same stage)
        MAX(CASE WHEN S.Status = 'PEN' THEN S.CreatedDate END) as PEN_Date,
        MAX(CASE WHEN S.Status = 'REJ' THEN S.CreatedDate END) as REJ_Date,
        MAX(CASE WHEN S.Status = 'HLD' THEN S.CreatedDate END) as HLD_Date,
        MAX(CASE WHEN S.Status = 'PHN' THEN S.CreatedDate END) as PHN_Date,
        MAX(CASE WHEN S.Status = 'URW' THEN S.CreatedDate END) as URW_Date,
        MAX(CASE WHEN S.Status = 'INT' THEN S.CreatedDate END) as INT_Date,
        MAX(CASE WHEN S.Status = 'RHM' THEN S.CreatedDate END) as RHM_Date,
        MAX(CASE WHEN S.Status = 'DEC' THEN S.CreatedDate END) as DEC_Date,
        MAX(CASE WHEN S.Status = 'NOA' THEN S.CreatedDate END) as NOA_Date,
        MAX(CASE WHEN S.Status = 'OEX' THEN S.CreatedDate END) as OEX_Date,
        MAX(CASE WHEN S.Status = 'ODC' THEN S.CreatedDate END) as ODC_Date,
        MAX(CASE WHEN S.Status = 'HIR' THEN S.CreatedDate END) as HIR_Date,
        MAX(CASE WHEN S.Status = 'WDR' THEN S.CreatedDate END) as WDR_Date
    INTO #RecruiterStageTimings
    FROM #RecruiterFirstSubmissions RFS
    INNER JOIN Submissions S ON RFS.RequisitionId = S.RequisitionId AND RFS.CandidateId = S.CandidateId
    WHERE S.CreatedBy = @User
    GROUP BY RFS.RequisitionId, RFS.CandidateId;
    
    -- Temp table 5: Calculate time spent in each stage for this user's candidates
    SELECT 
        RST.RequisitionId,
        RST.CandidateId,
        
        CASE 
            WHEN RST.PEN_Date IS NULL THEN 0
            WHEN RST.REJ_Date IS NOT NULL THEN DATEDIFF(DAY, RST.PEN_Date, RST.REJ_Date)
            WHEN RST.HLD_Date IS NOT NULL THEN DATEDIFF(DAY, RST.PEN_Date, RST.HLD_Date)
            WHEN RST.PHN_Date IS NOT NULL THEN DATEDIFF(DAY, RST.PEN_Date, RST.PHN_Date)
            ELSE DATEDIFF(DAY, RST.PEN_Date, @Today)
        END as PEN_Days,
        
        CASE 
            WHEN RST.REJ_Date IS NULL THEN 0
            ELSE 0 -- REJ is final stage
        END as REJ_Days,
        
        CASE 
            WHEN RST.HLD_Date IS NULL THEN 0
            WHEN RST.PHN_Date IS NOT NULL THEN DATEDIFF(DAY, RST.HLD_Date, RST.PHN_Date)
            WHEN RST.URW_Date IS NOT NULL THEN DATEDIFF(DAY, RST.HLD_Date, RST.URW_Date)
            ELSE DATEDIFF(DAY, RST.HLD_Date, @Today)
        END as HLD_Days,
        
        CASE 
            WHEN RST.PHN_Date IS NULL THEN 0
            WHEN RST.URW_Date IS NOT NULL THEN DATEDIFF(DAY, RST.PHN_Date, RST.URW_Date)
            WHEN RST.INT_Date IS NOT NULL THEN DATEDIFF(DAY, RST.PHN_Date, RST.INT_Date)
            ELSE DATEDIFF(DAY, RST.PHN_Date, @Today)
        END as PHN_Days,
        
        CASE 
            WHEN RST.URW_Date IS NULL THEN 0
            WHEN RST.INT_Date IS NOT NULL THEN DATEDIFF(DAY, RST.URW_Date, RST.INT_Date)
            ELSE DATEDIFF(DAY, RST.URW_Date, @Today)
        END as URW_Days,
        
        CASE 
            WHEN RST.INT_Date IS NULL THEN 0
            WHEN RST.RHM_Date IS NOT NULL THEN DATEDIFF(DAY, RST.INT_Date, RST.RHM_Date)
            WHEN RST.DEC_Date IS NOT NULL THEN DATEDIFF(DAY, RST.INT_Date, RST.DEC_Date)
            ELSE DATEDIFF(DAY, RST.INT_Date, @Today)
        END as INT_Days,
        
        CASE 
            WHEN RST.RHM_Date IS NULL THEN 0
            WHEN RST.DEC_Date IS NOT NULL THEN DATEDIFF(DAY, RST.RHM_Date, RST.DEC_Date)
            ELSE DATEDIFF(DAY, RST.RHM_Date, @Today)
        END as RHM_Days,
        
        CASE 
            WHEN RST.DEC_Date IS NULL THEN 0
            WHEN RST.NOA_Date IS NOT NULL THEN DATEDIFF(DAY, RST.DEC_Date, RST.NOA_Date)
            WHEN RST.OEX_Date IS NOT NULL THEN DATEDIFF(DAY, RST.DEC_Date, RST.OEX_Date)
            ELSE DATEDIFF(DAY, RST.DEC_Date, @Today)
        END as DEC_Days,
        
        CASE 
            WHEN RST.NOA_Date IS NULL THEN 0
            WHEN RST.OEX_Date IS NOT NULL THEN DATEDIFF(DAY, RST.NOA_Date, RST.OEX_Date)
            ELSE DATEDIFF(DAY, RST.NOA_Date, @Today)
        END as NOA_Days,
        
        CASE 
            WHEN RST.OEX_Date IS NULL THEN 0
            WHEN RST.ODC_Date IS NOT NULL THEN DATEDIFF(DAY, RST.OEX_Date, RST.ODC_Date)
            WHEN RST.HIR_Date IS NOT NULL THEN DATEDIFF(DAY, RST.OEX_Date, RST.HIR_Date)
            WHEN RST.WDR_Date IS NOT NULL THEN DATEDIFF(DAY, RST.OEX_Date, RST.WDR_Date)
            ELSE DATEDIFF(DAY, RST.OEX_Date, @Today)
        END as OEX_Days,
        
        CASE 
            WHEN RST.ODC_Date IS NULL THEN 0
            ELSE 0 -- ODC is final stage
        END as ODC_Days,
        
        CASE 
            WHEN RST.HIR_Date IS NULL THEN 0
            ELSE 0 -- HIR is final stage
        END as HIR_Days,
        
        CASE 
            WHEN RST.WDR_Date IS NULL THEN 0
            ELSE 0 -- WDR is final stage
        END as WDR_Days
        
    INTO #RecruiterStageTimeCalculations
    FROM #RecruiterStageTimings RST;
    
    -- Query 1: Timing Analytics by Requisition (Recruiter view - user filtered)
    SELECT 
        @User as [User],
        RRB.RequisitionCode,
        RRB.Title,
        -- Average Time to Fill for this requisition (if filled)
        ISNULL(CEILING(AVG(CAST(RRB.TimeToFill as FLOAT))), 0) as TimeToFill_Days,
        -- Average Time to Hire for this requisition  
        ISNULL(CEILING(AVG(CAST(RTTH.TimeToHire as FLOAT))), 0) as TimeToHire_Days,
        -- Average time in each stage for this requisition (rounded up to next integer)
        ISNULL(CEILING(AVG(CAST(RSTC.PEN_Days as FLOAT))), 0) as PEN_Days,
        ISNULL(CEILING(AVG(CAST(RSTC.REJ_Days as FLOAT))), 0) as REJ_Days,
        ISNULL(CEILING(AVG(CAST(RSTC.HLD_Days as FLOAT))), 0) as HLD_Days,
        ISNULL(CEILING(AVG(CAST(RSTC.PHN_Days as FLOAT))), 0) as PHN_Days,
        ISNULL(CEILING(AVG(CAST(RSTC.URW_Days as FLOAT))), 0) as URW_Days,
        ISNULL(CEILING(AVG(CAST(RSTC.INT_Days as FLOAT))), 0) as INT_Days,
        ISNULL(CEILING(AVG(CAST(RSTC.RHM_Days as FLOAT))), 0) as RHM_Days,
        ISNULL(CEILING(AVG(CAST(RSTC.DEC_Days as FLOAT))), 0) as DEC_Days,
        ISNULL(CEILING(AVG(CAST(RSTC.NOA_Days as FLOAT))), 0) as NOA_Days,
        ISNULL(CEILING(AVG(CAST(RSTC.OEX_Days as FLOAT))), 0) as OEX_Days,
        ISNULL(CEILING(AVG(CAST(RSTC.ODC_Days as FLOAT))), 0) as ODC_Days,
        ISNULL(CEILING(AVG(CAST(RSTC.HIR_Days as FLOAT))), 0) as HIR_Days,
        ISNULL(CEILING(AVG(CAST(RSTC.WDR_Days as FLOAT))), 0) as WDR_Days,
        COUNT(DISTINCT CONCAT(RSTC.RequisitionId, '-', RSTC.CandidateId)) as TotalCandidates
    FROM #RecruiterRequisitionBase RRB
    LEFT JOIN #RecruiterTimeToHireCalc RTTH ON RRB.RequisitionId = RTTH.RequisitionId
    LEFT JOIN #RecruiterStageTimeCalculations RSTC ON RRB.RequisitionId = RSTC.RequisitionId
    GROUP BY RRB.RequisitionCode, RRB.Title
    ORDER BY RRB.RequisitionCode
    FOR JSON PATH;
    
    -- Query 2: Timing Analytics by Company (Recruiter view - user filtered)
    SELECT 
        @User as [User],
        RRB.CompanyName,
        -- Average Time to Fill for this company (if filled)
        ISNULL(CEILING(AVG(CAST(RRB.TimeToFill as FLOAT))), 0) as TimeToFill_Days,
        -- Average Time to Hire for this company  
        ISNULL(CEILING(AVG(CAST(RTTH.TimeToHire as FLOAT))), 0) as TimeToHire_Days,
        -- Average time in each stage for this company (rounded up to next integer)
        ISNULL(CEILING(AVG(CAST(RSTC.PEN_Days as FLOAT))), 0) as PEN_Days,
        ISNULL(CEILING(AVG(CAST(RSTC.REJ_Days as FLOAT))), 0) as REJ_Days,
        ISNULL(CEILING(AVG(CAST(RSTC.HLD_Days as FLOAT))), 0) as HLD_Days,
        ISNULL(CEILING(AVG(CAST(RSTC.PHN_Days as FLOAT))), 0) as PHN_Days,
        ISNULL(CEILING(AVG(CAST(RSTC.URW_Days as FLOAT))), 0) as URW_Days,
        ISNULL(CEILING(AVG(CAST(RSTC.INT_Days as FLOAT))), 0) as INT_Days,
        ISNULL(CEILING(AVG(CAST(RSTC.RHM_Days as FLOAT))), 0) as RHM_Days,
        ISNULL(CEILING(AVG(CAST(RSTC.DEC_Days as FLOAT))), 0) as DEC_Days,
        ISNULL(CEILING(AVG(CAST(RSTC.NOA_Days as FLOAT))), 0) as NOA_Days,
        ISNULL(CEILING(AVG(CAST(RSTC.OEX_Days as FLOAT))), 0) as OEX_Days,
        ISNULL(CEILING(AVG(CAST(RSTC.ODC_Days as FLOAT))), 0) as ODC_Days,
        ISNULL(CEILING(AVG(CAST(RSTC.HIR_Days as FLOAT))), 0) as HIR_Days,
        ISNULL(CEILING(AVG(CAST(RSTC.WDR_Days as FLOAT))), 0) as WDR_Days,
        COUNT(DISTINCT CONCAT(RSTC.RequisitionId, '-', RSTC.CandidateId)) as TotalCandidates,
        COUNT(DISTINCT RRB.RequisitionId) as TotalRequisitions
    FROM #RecruiterRequisitionBase RRB
    LEFT JOIN #RecruiterTimeToHireCalc RTTH ON RRB.RequisitionId = RTTH.RequisitionId
    LEFT JOIN #RecruiterStageTimeCalculations RSTC ON RRB.RequisitionId = RSTC.RequisitionId
    GROUP BY RRB.CompanyName
    ORDER BY RRB.CompanyName
    FOR JSON PATH;

    -- Clean up recruiter timing temp tables
    DROP TABLE IF EXISTS #RecruiterRequisitionBase;
    DROP TABLE IF EXISTS #RecruiterFirstSubmissions;
    DROP TABLE IF EXISTS #RecruiterTimeToHireCalc;
    DROP TABLE IF EXISTS #RecruiterStageTimings;
    DROP TABLE IF EXISTS #RecruiterStageTimeCalculations;
    
    -- Clean up existing temp tables (from current DashboardRecruiters)
    DROP TABLE IF EXISTS #BaseMetrics;
    DROP TABLE IF EXISTS #RecruiterSubmissionMetrics;

END;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =====================================================
-- OPTIMIZED DASHBOARD PROCEDURES USING MATERIALIZED VIEWS
-- =====================================================

-- 1. Optimized DashboardRecruiters
CREATE   PROCEDURE [DashboardRecruiters_Optimized]
    @User VARCHAR(10) = 'KEVIN'
AS
BEGIN
    SET NOCOUNT ON;
    SET ARITHABORT ON;
    SET ANSI_NULLS ON;
    SET QUOTED_IDENTIFIER ON;
    
    IF @User IS NULL SET @User = 'KEVIN';
    
    -- Generic Query for Dropdown
    SELECT UserName as KeyValue, FirstName + ' ' + LastName as Text
    FROM dbo.Users 
    WHERE UserName = @User
    FOR JSON AUTO;
    
    -- SET 1: Submission Pipeline Metrics (Using Materialized View)
    -- Query 1: Total Submissions by Recruiter
    SELECT 
        @User as [User], 
        SMV.INT_Count + SMV.OEX_Count + SMV.HIR_Count as LAST7D_COUNT,
        0 as MTD_COUNT, 0 as QTD_COUNT, 0 as HYTD_COUNT, 0 as YTD_COUNT
    FROM [dbo].[SubmissionMetricsView] SMV
    WHERE SMV.CreatedBy = @User AND SMV.Period = '7D' AND SMV.MetricDate = CAST(GETDATE() AS DATE)
    FOR JSON PATH;
    
    -- Query 2: Active Requisitions Worked On
    SELECT 
        @User as [User],
        SMV.Active_Requisitions_Updated as LAST7D_COUNT,
        0 as MTD_COUNT, 0 as QTD_COUNT, 0 as HYTD_COUNT, 0 as YTD_COUNT
    FROM [dbo].[SubmissionMetricsView] SMV
    WHERE SMV.CreatedBy = @User AND SMV.Period = '7D' AND SMV.MetricDate = CAST(GETDATE() AS DATE)
    FOR JSON PATH;
    
    -- Queries 3-6: Status-based submissions (using materialized view)
    SELECT 
        @User as [User],
        SMV.INT_Count as LAST7D_COUNT,
        (SELECT INT_Count FROM [dbo].[SubmissionMetricsView] WHERE CreatedBy = @User AND Period = 'MTD' AND MetricDate = CAST(GETDATE() AS DATE)) as MTD_COUNT,
        (SELECT INT_Count FROM [dbo].[SubmissionMetricsView] WHERE CreatedBy = @User AND Period = 'QTD' AND MetricDate = CAST(GETDATE() AS DATE)) as QTD_COUNT,
        (SELECT INT_Count FROM [dbo].[SubmissionMetricsView] WHERE CreatedBy = @User AND Period = 'HYTD' AND MetricDate = CAST(GETDATE() AS DATE)) as HYTD_COUNT,
        (SELECT INT_Count FROM [dbo].[SubmissionMetricsView] WHERE CreatedBy = @User AND Period = 'YTD' AND MetricDate = CAST(GETDATE() AS DATE)) as YTD_COUNT
    FROM [dbo].[SubmissionMetricsView] SMV
    WHERE SMV.CreatedBy = @User AND SMV.Period = '7D' AND SMV.MetricDate = CAST(GETDATE() AS DATE)
    FOR JSON PATH;
    
    -- Continue with other queries...
    
    -- SET 4: Timing Analytics (Using Materialized View)
    SELECT 
        @User as [User],
        TAV.CompanyName,
        AVG(TAV.TimeToFill_Days) as TimeToFill_Days,
        AVG(TAV.TimeToHire_Days) as TimeToHire_Days,
        AVG(TAV.PEN_Days) as PEN_Days,
        AVG(TAV.REJ_Days) as REJ_Days,
        AVG(TAV.HLD_Days) as HLD_Days,
        AVG(TAV.PHN_Days) as PHN_Days,
        AVG(TAV.URW_Days) as URW_Days,
        AVG(TAV.INT_Days) as INT_Days,
        AVG(TAV.RHM_Days) as RHM_Days,
        AVG(TAV.DEC_Days) as DEC_Days,
        AVG(TAV.NOA_Days) as NOA_Days,
        AVG(TAV.OEX_Days) as OEX_Days,
        AVG(TAV.ODC_Days) as ODC_Days,
        AVG(TAV.HIR_Days) as HIR_Days,
        AVG(TAV.WDR_Days) as WDR_Days,
        COUNT(DISTINCT CONCAT(TAV.RequisitionId, '-', TAV.CandidateId)) as TotalCandidates,
        COUNT(DISTINCT TAV.RequisitionId) as TotalRequisitions
    FROM [dbo].[TimingAnalyticsView] TAV
    WHERE TAV.CreatedBy = @User
    GROUP BY TAV.CompanyName
    ORDER BY TAV.CompanyName
    FOR JSON PATH;
END;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Narendra Kumaran Kadhirvelu
-- Create date: 03-02-2022
-- Description:	Delete an existing Document
-- =============================================
CREATE   PROCEDURE [DeleteCandidateDocument] 
	@CandidateDocumentID int = 0,
	@User varchar(10) = 'JOLLY'
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	DECLARE @Name varchar(255), @CandidateID int, @Description varchar(7000), @Action varchar(30);

	if (@CandidateDocumentID IS NOT NULL AND @CandidateDocumentID <> 0 AND EXISTS(SELECT * FROM dbo.[CandidateDocument] A WHERE [CandidateDocID] = @CandidateDocumentID))
		BEGIN
			SELECT
				@Name = A.[DocumentLocation], @CandidateID = A.[CandidateId]
			FROM
				dbo.[CandidateDocument] A
			WHERE
				[CandidateDocID] = @CandidateDocumentID;

			-- Insert statements for procedure here
			DELETE FROM
				dbo.[CandidateDocument]
			WHERE
				[CandidateDocID] = @CandidateDocumentID;

			SET @Description = 'Deleted Document for [ID: ' + CAST(@CandidateId as varchar(10)) + '], Document Name: ' + @Name;
								
			SET @Action = 'Delete Candidate Document';
	
			exec dbo.AddAuditTrail @Action, 'Candidate Document', @Description, @User; 

			execute dbo.[GetCandidateDocuments] @CandidateId;
		END
	else
		BEGIN
			SELECT '[]';
		END
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   procedure [DeleteCandidateEducation]
	@Id int=30000,
	@CandidateId int=14331,
	@User varchar(10)='JOLLY'
AS
BEGIN
	SET NOCOUNT ON;
	
	DECLARE @EmployerId int;

	if (@ID IS NOT NULL AND @ID <> 0 AND EXISTS(SELECT * FROM dbo.[CandidateEducation] WHERE [Id] = @Id))
		BEGIN
			DECLARE @Name varchar(100), @Degree varchar(100);
			SELECT
				@Name = A.[FirstName] + ' ' + A.[LastName], @Degree = B.[Degree]
			FROM
				[dbo].[Candidate] A INNER JOIN dbo.[CandidateEducation] B ON A.[Id] = B.[CandidateId]
			WHERE
				B.[Id] = @Id;
		
			DECLARE @Description varchar(7000);
			SET @Description = 'Deleted Education for ' + @Name + ', [ID: ' + CAST(@CandidateId as varchar(10)) 
								+ '], Deleted Degree: ' + @Degree;

			DELETE FROM
				dbo.[CandidateEducation]
			WHERE
				[Id] = @Id;
	
			exec dbo.AddAuditTrail 'Delete Candidate Education', 'Candidate Details', @Description, @User; 
	END

	DECLARE @return varchar(max);	

	SELECT @return = 
	(SELECT
		A.[Id], A.[Degree], A.[College], ISNULL(A.[State], '') [State], A.[Country], A.[Year], A.[UpdatedBy]
	FROM
		dbo.[CandidateEducation] A
	WHERE
		A.[CandidateId] = @CandidateId
	ORDER BY
		A.[UpdatedDate] DESC
	FOR JSON AUTO);
		
	SELECT ISNULL(@return, '[]');

END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   procedure [DeleteCandidateExperience]
	@Id int=0,
	@CandidateId int=14331,
	@User varchar(10)='JOLLY'
AS
BEGIN
	SET NOCOUNT ON;
	
	DECLARE @EmployerId int;

	DECLARE @Name varchar(100),@Employer varchar(100);
	
	if (@ID IS NOT NULL AND @ID <> 0 AND EXISTS(SELECT * FROM dbo.[CandidateEmployer] WHERE [Id] = @Id))
		BEGIN
			SELECT
				@Name = A.[FirstName] + ' ' + A.[LastName], @Employer = B.[Employer]
			FROM
				[dbo].[Candidate] A INNER JOIN dbo.[CandidateEmployer] B ON A.[Id] = B.[CandidateId]
			WHERE
				B.[Id] = @Id;
		
			DECLARE @Description varchar(7000);
			SET @Description = 'Deleted Experience for ' + @Name + ', [ID: ' + CAST(@CandidateId as varchar(10)) + '], Deleted Employer: ' + @Employer;

			DELETE FROM
				dbo.[CandidateEmployer]
			WHERE
				[Id] = @Id;
	
			exec dbo.AddAuditTrail 'Delete Candidate Experience', 'Candidate Details', @Description, @User; 
		END
		
	DECLARE @return varchar(max);

	SELECT @return = 
	(SELECT
		A.[Id], A.[Employer], A.[Start], A.[End], A.[Location], ISNULL(REPLACE(REPLACE(REPLACE(A.[Description], CHAR(13) + CHAR(10), '<br/>'), CHAR(13), '<br/>'), CHAR(10), '<br/>'), '') [Description], 
		A.[UpdatedBy], A.[Title]
	FROM
		dbo.[CandidateEmployer] A
	WHERE
		A.CandidateId = @CandidateId
	ORDER BY
		A.[UpdatedDate] DESC
	FOR JSON AUTO);

	SELECT ISNULL(@return, '[]');
		
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   procedure [DeleteCandidateNotes]
	@Id int=0,
	@CandidateId int=14331,
	@User varchar(10)='JOLLY'
AS
BEGIN
	SET NOCOUNT ON;
	
	DECLARE @SkillId int;

	if @ID IS NOT NULL AND @ID <> 0 AND (EXISTS(SELECT * FROM dbo.[Notes]	WHERE [Id] = @Id))
		BEGIN
			DECLARE @Name varchar(100), @Note varchar(max);
			SELECT
				@Name = A.[FirstName] + ' ' + A.[LastName], @Note = LEFT(B.[Notes], 3000) + '...'
			FROM
				[dbo].[Candidate] A INNER JOIN dbo.[Notes] B ON A.[Id] = B.[EntityId]
			WHERE
				B.[Id] = @Id;
		
			DECLARE @Description varchar(7000);
			SET @Description = 'Deleted Notes for ' + @Name + ', [ID: ' + CAST(@CandidateId as varchar(10)) 
								+ '], Deleted Note: ' + @Note;
	
			DELETE FROM
				dbo.[Notes]
			WHERE
				[Id] = @Id;
	
			exec dbo.AddAuditTrail 'Delete Candidate Note', 'Candidate Details', @Description, @User; 
		END

	DECLARE @return varchar(max);

	SELECT @return = 
	(SELECT
		A.[Id], A.[UpdatedDate], A.[UpdatedBy], REPLACE(REPLACE(REPLACE(A.[NOTES], CHAR(13) + CHAR(10), '<br/>'), CHAR(13), '<br/>'), CHAR(10), '<br/>') [Notes]
	FROM
		[dbo].[Notes] A
	WHERE
		A.[EntityId] = @CandidateId
		AND A.[EntityType] = 'CND'
	ORDER BY
		A.[IsPrimary] DESC, A.[UpdatedDate] DESC
	FOR JSON AUTO);

	SELECT ISNULL(@return, '[]');
		
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   procedure [DeleteCandidateSkill]
	@Id int=0,
	@CandidateId int=14331,
	@User varchar(10)='JOLLY'
AS
BEGIN
	SET NOCOUNT ON;
	
	DECLARE @SkillId int;

	DECLARE @Name varchar(100), @Skill varchar(100);
	
	if (@ID IS NOT NULL AND @ID <> 0 AND EXISTS(SELECT * FROM dbo.[EntitySkills] WHERE [Id] = @Id))
		BEGIN
			SELECT
				@Name = A.[FirstName] + ' ' + A.[LastName], @Skill = C.[Skill]
			FROM
				[dbo].[Candidate] A INNER JOIN dbo.[EntitySkills] B ON A.[Id] = B.[EntityId]
				INNER JOIN dbo.[Skills] C ON B.[SkillId] = C.[Id]
			WHERE
				B.[Id] = @Id;
		
			DECLARE @Description varchar(7000);
			SET @Description = 'Deleted Skill for ' + @Name + ', [ID: ' + CAST(@CandidateId as varchar(10)) 
								+ '], Deleted Skill: ' + @Skill;
	
			DELETE FROM
				dbo.[EntitySkills]
			WHERE
				[Id] = @Id;

			exec dbo.AddAuditTrail 'Delete Candidate Skill', 'Candidate Details', @Description, @User; 
		END
		
	DECLARE @return varchar(max);
	SELECT @return = 
	(SELECT
		A.[Id], B.[Skill], A.[LastUsed], A.[ExpMonth], A.[UpdatedBy]
	FROM
		dbo.[EntitySkills] A INNER JOIN dbo.[Skills] B ON A.[SkillId] = B.[Id]
	WHERE
		A.[EntityId] = @CandidateId
		AND A.[EntityType] = 'CND'
	ORDER BY
		A.[UpdatedDate] DESC
	FOR JSON PATH);
		
	SELECT ISNULL(@return, '[]');
		
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Narendra Kumaran Kadhirvelu
-- Create date: 03-02-2022
-- Description:	Delete an existing Document
-- =============================================
CREATE   PROCEDURE [DeleteCompanyDocument] 
	@ID int = 0,
	@User varchar(10) = 'JOLLY'
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	DECLARE @Name varchar(255), @CompanyID int, @Description varchar(7000), @Action varchar(30);

	if (@ID IS NOT NULL AND @ID <> 0 AND EXISTS(SELECT * FROM dbo.[CompanyDocuments] A WHERE [ID] = @ID))
		BEGIN
			SELECT
				@Name = A.[OriginalFileName], @CompanyID = A.[CompanyId]
			FROM
				dbo.[CompanyDocuments] A
			WHERE
				[ID] = @ID;

			-- Insert statements for procedure here
			DELETE FROM
				dbo.[CompanyDocuments]
			WHERE
				[ID] = @ID;

			SET @Description = 'Deleted Document for [ID: ' + CAST(@CompanyId as varchar(10)) + '], Document Name: ' + @Name;
								
			SET @Action = 'Delete Company Document';
	
			exec dbo.AddAuditTrail @Action, 'Company Document', @Description, @User; 

			execute dbo.[GetCompanyDocuments] @CompanyId;
		END
	else
		BEGIN
			SELECT '[]';
		END
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE   procedure [DeleteRequisitionDocuments]
	@RequisitionDocId int = 12,
	@User varchar(10) = ''
as
BEGIN;
	SET NOCOUNT ON;

	DECLARE @Name varchar(255), @CandidateID int, @Description varchar(7000), @Action varchar(30);
	DECLARE @RequisitionId int, @DocumentLocation varchar(255);

	SELECT
		@RequisitionId = A.[RequisitionId], @DocumentLocation = A.[DocumentLocation]
	FROM
		dbo.[RequisitionDocument] A
	WHERE
		A.[RequisitionDocId] = @RequisitionDocId;

	DELETE FROM
		dbo.[RequisitionDocument]
	WHERE
		[RequisitionDocId] = @RequisitionDocId;

	SELECT @RequisitionId, @DocumentLocation;

	SET @Description = 'Deleted Document for [ID: ' + CAST(@RequisitionId as varchar(10)) + '], Document Location: ' + @DocumentLocation;
								
	SET @Action = 'Delete Requisition Document';
	
	exec dbo.AddAuditTrail @Action, 'Requisition Document', @Description, @User; 

	execute dbo.[GetRequisitionDocuments] @RequisitionId;
END;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Narendara Kadhirvelu, Jolly Joseph, DonBosco Paily
-- Create date: 04-14-2023
-- Description:	Get Resumes for a candidate either Original or Formatted
-- =============================================
CREATE PROCEDURE [DownloadCandidateResume] 
	@CandidateID int = 100090, 
	@ResumeType varchar(20) = 'Original'
AS
BEGIN
	SET NOCOUNT ON;
	DECLARE @return varchar(max);

	if (@ResumeType = 'Original')
		BEGIN
			SELECT @return = 
				(SELECT
					A.[OriginalResume] [DocumentLocation], A.[OriginalFileId] [InternalFileName]
				FROM
					dbo.[Candidate] A
				WHERE
					A.[Id] = @CandidateID
				FOR JSON AUTO, WITHOUT_ARRAY_WRAPPER);
		END
	else
		BEGIN
			SELECT @return = 
				(SELECT
				A.[FormattedResume] [DocumentLocation], A.[FormattedFileId] [InternalFileName]
			FROM
				dbo.[Candidate] A
			WHERE
				A.[Id] = @CandidateID
				FOR JSON AUTO, WITHOUT_ARRAY_WRAPPER);
		END

	SELECT ISNULL(@return, '[]');
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE   PROCEDURE [DuplicateCandidate]
    @CandidateID INT,
    @User varchar(10)
AS
BEGIN
	SET NOCOUNT ON;
	SET ARITHABORT ON;
	SET ANSI_NULLS ON;
	SET QUOTED_IDENTIFIER ON;

    BEGIN TRY
        BEGIN TRANSACTION;
        DECLARE @NewCandidateID int = 0;

        -- Step 1: Insert new Candidate
        INSERT INTO 
            dbo.Candidate 
            (FirstName, LastName, MiddleName, Title, Address1, Address2, City, StateId, ZipCode, Email, Phone1, Phone2, Phone3, Phone3Ext, EligibilityId, ExperienceId, Experience, 
            JobOptions, Communication, TaxTerm, SalaryLow, SalaryHigh, HourlyRate, HourlyRateHigh, VendorId, Relocate, RelocNotes, Background, SecurityClearanceNotes, Keywords, 
            Summary, ExperienceSummary, Objective, RateCandidate, RateNotes, MPC, MPCNotes, Status, TextResume, OriginalResume, FormattedResume, OriginalFileId, FormattedFileId, 
            LinkedIn, Facebook, Twitter, Google, ReferAccountMgr, Refer, EEO,ParsedXML, JsonFileName, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate)
        SELECT
            FirstName, LastName, MiddleName, Title, Address1, Address2, City, StateId, ZipCode, Email, Phone1, Phone2, Phone3, Phone3Ext, EligibilityId, ExperienceId, Experience, 
            JobOptions, Communication, TaxTerm, SalaryLow, SalaryHigh, HourlyRate, HourlyRateHigh, VendorId, Relocate, RelocNotes, Background, SecurityClearanceNotes, Keywords, 
            Summary, ExperienceSummary, Objective, RateCandidate, RateNotes, MPC, MPCNotes, Status, TextResume, OriginalResume, FormattedResume, OriginalFileId, FormattedFileId,
            LinkedIn, Facebook, Twitter, Google, ReferAccountMgr, Refer, EEO, ParsedXML, JsonFileName, @User, GETDATE(), @User, GETDATE()
        FROM 
            dbo.Candidate
        WHERE 
            ID = @CandidateID;

        SET @NewCandidateID = SCOPE_IDENTITY();

        -- Step 2: Education
        INSERT INTO 
            dbo.CandidateEducation 
            (CandidateId, Degree, College, State, Country, Year, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate)
        SELECT 
            @NewCandidateID, Degree, College, State, Country, Year, @User, GETDATE(), @User, GETDATE()
        FROM 
            dbo.CandidateEducation
        WHERE 
            CandidateId = @CandidateID;

        -- Step 3: Employment
        INSERT INTO 
            dbo.CandidateEmployer 
            (CandidateId, Employer, Start, [End], Location, Title, [Description], CreatedBy, CreatedDate, UpdatedBy, UpdatedDate)
        SELECT 
            @NewCandidateID, Employer, Start, [End], Location, Title, [Description], @User, GETDATE(), @User, GETDATE()
        FROM 
            dbo.CandidateEmployer
        WHERE 
            CandidateId = @CandidateID;

        -- Step 4: Skills
        INSERT INTO
            dbo.EntitySkills
            (EntityId, EntityType, SkillId, LastUsed, ExpMonth, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate)
        SELECT
            @NewCandidateID, 'CND', SkillId, LastUsed, ExpMonth, @User, GETDATE(), @User, GETDATE()
        FROM
            dbo.EntitySkills
        WHERE
            EntityId = @CandidateID AND EntityType = 'CND';

        -- Step 5: Notes
        INSERT INTO
            dbo.Notes
            (EntityId, EntityType, Notes, IsPrimary, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate)
        SELECT
            @NewCandidateID, 'CND', Notes, IsPrimary, @User, GETDATE(), @User, GETDATE()
        FROM
            dbo.Notes
        WHERE
            EntityId = @CandidateID AND EntityType = 'CND';

        -- Step 6: Document map for copying
        --DECLARE @DocMap TABLE (InternalFileName VARCHAR(100));

        INSERT INTO 
            dbo.CandidateDocument 
            (CandidateId, DocumentName, DocumentLocation, DocumentType, Notes, InternalFileName, LastUpdatedBy, LastUpdatedDate)
        --OUTPUT 
        --    inserted.InternalFileName 
        --INTO 
        --    @DocMap(InternalFileName)
        SELECT
            @NewCandidateID, DocumentName, DocumentLocation, DocumentType, Notes, InternalFileName, @User, GETDATE()
        FROM 
            dbo.CandidateDocument src
        WHERE 
            src.CandidateId = @CandidateID
            AND TRIM(ISNULL(InternalFileName, '')) <> '';  -- skip blank filenames

        COMMIT TRANSACTION;

        -- Result 1: New Candidate ID
        SELECT @NewCandidateID;

    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Narendra Kumaran Kadhirvelu
-- Create date: February 23 20223
-- Description:	Get the details of Documentd for a Candidate
-- =============================================
CREATE     PROCEDURE [GetCandidateDocumentDetails]
	@DocumentID int = 10210
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @return varchar(max);

	SELECT @return = 
		(SELECT
			A.[CandidateId] [EntityID], A.[DocumentName], A.[DocumentLocation], A.[InternalFileName]
		FROM
			dbo.[CandidateDocument] A
		WHERE
			A.[CandidateDocId] = CASE WHEN @DocumentID IS NULL OR @DocumentID = 0 THEN -1 ELSE @DocumentID END
		FOR JSON AUTO, WITHOUT_ARRAY_WRAPPER);

	SELECT ISNULL(@return, '{}');
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   procedure [GetCandidateDocuments]
	@CandidateId int=19
as
BEGIN;
	SET NOCOUNT ON;
	DECLARE @return varchar(max);

	SELECT @return = 
	(SELECT 
		A.[CandidateDocId] [ID], A.[DocumentName] [Name], A.[DocumentLocation] [Location], A.[Notes], CONVERT(varchar(20), A.[LastUpdatedDate], 101)  + ' [' + A.[LastUpdatedBy] + ']' [UpdatedBy], 
		B.[DocumentType] [DocumentType], A.[InternalFileName], A.[DocumentType] [DocumentTypeID]
	FROM 
		[dbo].[CandidateDocument] A INNER JOIN dbo.[DocumentType] B ON A.[DocumentType] = B.[Id]
	WHERE
		A.[CandidateId] = @CandidateId
	FOR JSON PATH);

	SELECT ISNULL(@return, '[]');

END;


GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create   procedure [GetCandidateRequisitionDescription]
	@CandidateID int= 11600,
	@RequisitionID int = 1674
AS
BEGIN
	SET NOCOUNT ON;

	SELECT
		A.TextResume
	FROM
		dbo.[Candidate] A
	WHERE
		A.[ID] = @CandidateID;

	SELECT
		A.Description
	FROM
		dbo.[Requisitions] A
	WHERE
		A.[Id] = @RequisitionID;
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [GetCandidates]
	@RecordsPerPage int = 25,
	@PageNumber int = 1,
	@SortColumn int = 1, --1-Updated Date, 2-Name, 3-Phone,4-Email, 5-City+State, 7-Updated By, 8-Status, 9-Rate
	@SortDirection tinyint = 0, -- 1-ASC, 0-DESC, NULL - Remove Sort, 11592, 1174, 3790 (for 18940 and 250 proximity)
	@Name varchar(255) = '',
	@Phone varchar(20) = '',
	@Email varchar(255) = '',
	@MyCandidates bit = 1,
	@IncludeAdmin bit = 1,
	@Keywords varchar(2000) = '',
	@Skill varchar(2000) = '',
	@SearchState bit = 0, 
	--@City varchar(30)='',
	@City varchar(10) = '',--,19030,19054,08618
	@State varchar(1000) = null,
	@Proximity int = 100,
	@ProximityUnit tinyint = 1, --1=miles, 0=kilometers
	@Eligibility varchar(10) = '0',
	@Reloc varchar(10) = null,
	@JobOptions varchar(10) = '',
	@Communications varchar(10) = '',
	@Security varchar(10) = '',
	@UpdatedBy varchar(10) = '',
	@UpdatedOn datetime = null,
	@Status varchar(3) = '',
	@Rating int = 0,
	@ActiveRequisitionsOnly bit = 0,
	@User varchar(10) = 'JOLLY',
	@OptionalCandidateID int = 0,
	@ThenProceed bit = 1,
	@ShowArchive bit = 0
AS
BEGIN
	--SET STATISTICS IO ON;
	--SET STATISTICS TIME ON;
	SET NOCOUNT ON;
	SET ARITHABORT ON;
	SET ANSI_NULLS ON;
	SET QUOTED_IDENTIFIER ON;

	BEGIN--DECLARE

		DECLARE @Requisitions AS dbo.PagingTable;
		DECLARE @table1 AS dbo.TempTable;

		DECLARE @ZipCode table ([Zips] varchar(10));
		DECLARE @StateTable table ([State] tinyint);
		DECLARE @FirstRecord int = ((@PageNumber * @RecordsPerPage) - @RecordsPerPage) + 1;
		DECLARE @LastRecord int = (@PageNumber * @RecordsPerPage);
		DECLARE @SingleChar bit = 0;
		DECLARE @SkillTable dbo.WordTable;
		DECLARE @g geography, @bbox geography;
		DECLARE @distance decimal(10,3);
		DECLARE @StartDate datetime, @EndDate datetime;
		DECLARE @StartRate int, @EndRate int
	 
	END

	BEGIN --SORT

	if (@SortDirection IS NULL)
		BEGIN
			SET @SortDirection = 0;
			SET @SortColumn = 1;
		END;

	END
		
	BEGIN --SET VALUES

		if (LEN(@Name) = 1 AND @Name <> '%')
			BEGIN
				SET @Name = @Name + '%';
			END
		else if (LEN(@Name) > 1)
			BEGIN
				SET @Name = '%' + @Name + '%';
			END

		if (@Skill IS NULL OR @Skill = '')
			BEGIN
				INSERT INTO
					@SkillTable
					([Term])
				VALUES
					('%');
			END
		else
			BEGIN
				INSERT INTO
					@SkillTable
					([Term])
				SELECT
					LTRIM(A.[value]) + '%'
				FROM
					string_split(@Skill, ',') A;
			END

		if ((@City IS NULL OR @City = '') AND (@State IS NULL OR @State = ''))
			BEGIN
				SET @SearchState = 1;
			END

		if (@SearchState = 0)
			BEGIN
				if (@City IS NULL OR @City = '')
					BEGIN
						SET @City = '';
						SET @State = '';
						SET @SearchState = 1;
					END
				else
					BEGIN
						--DECLARE @Start datetime, @End datetime
						--SET @Start = GETDATE();
						if (@Proximity IS NULL OR @Proximity < 1)
							BEGIN
								SET @Proximity = 1;
							END	
			
						if (@ProximityUnit IS NULL OR @ProximityUnit < 1)
							BEGIN
								SET @ProximityUnit = 1;
							END

						SELECT 
							@g = geography::Point(A.Latitude, A.Longitude, 4326)
						FROM 
							[dbo].[ZipCodes] A
						WHERE 
							A.[ZipCode] = @City
							OR A.[City] = @City;
				
						if (@ProximityUnit = 1)
							BEGIN
								SET @distance = @Proximity * 1609.344;
							END
						else
							BEGIN
								SET @distance = @Proximity * 1000;
							END

						--SET @bbox = @g.STBuffer(@distance);

						INSERT INTO 
							@ZipCode
						SELECT 
							Z.[ZipCode] 
						FROM 
							dbo.[ZipCodes] Z WITH (INDEX = SIndx_SpatialTable_geography_col1)
						WHERE 
							Z.[GeogCol1].Filter(@g.STBuffer(@distance)) = 1;
						--SET @End = GETDATE();
						--SELECT DATEDIFF(MILLISECOND, @Start, @End) 
				END
		END
		else
			BEGIN
				SET @City = ''
			END

		if (@UpdatedOn IS NOT NULL)
			BEGIN
				SET @StartDate = @UpdatedOn;
				SET @EndDate = DATEADD(d, 1, @UpdatedOn);
			END
		
	END
	SET @State = REPLACE(@State,', ', ',');
	INSERT INTO
		@StateTable
	SELECT
		CAST(value as tinyint)
	FROM
		string_split(@State, ',');

	DECLARE @SkillViewTable table(Skill varchar(100), ID int, EntityID int);
	if (@Skill IS NOT NULL AND TRIM(@Skill) <> '')
		BEGIN
			INSERT INTO	
				@SkillViewTable
			SELECT
				Skill, N.ID, EntityID
			FROM
				dbo.SkillsView N RIGHT JOIN @SkillTable F ON N.[Skill] LIKE F.[Term]
		END

	DECLARE @KeywordsTable table(Keyword varchar(100));
	if (@Keywords IS NOT NULL AND TRIM(@Keywords) <> '')
		BEGIN
			INSERT INTO	
				@KeywordsTable
			SELECT
				TRIM(S.value)
			FROM
				STRING_SPLIT(@Keywords, ',') S
		END

	;WITH CTE_Candidate AS (
	SELECT DISTINCT
		A.[Id], A.[MPC], A.[UpdatedDate], A.[FullName], A.[Phone1], A.[Email], A.[City], A.[Code], A.[UpdatedBy], A.[Status], A.[RateCandidate], A.StateID, A.ZipCode
	FROM dbo.[CandidateView] A
	WHERE 
		(ISNULL(@Name, '') = '' OR A.[FullName] LIKE @Name)
		AND (@ActiveRequisitionsOnly = 0 OR (@ActiveRequisitionsOnly = 1 AND A.RequisitionCount > 0))
		AND (
			@Keywords IS NULL OR @Keywords = '' OR 
			EXISTS (SELECT 1 FROM STRING_SPLIT(A.Keywords, ',') AS SplitC JOIN @KeywordsTable AS SplitK 
					ON TRIM(SplitC.value) LIKE '%' + TRIM(SplitK.Keyword) + '%')
		)
		AND (@Skill IS NULL OR TRIM(@Skill) = '' OR EXISTS (SELECT N.Id FROM @SkillViewTable N WHERE N.[EntityId] = A.[Id]))
		--AND (ISNULL(@Phone, '') = '' OR A.[Phone1] LIKE '%' + @Phone + '%')
		--AND (ISNULL(@Email, '') = '' OR A.[Email] LIKE '%' + @Email + '%')
		AND (ISNULL(@UpdatedBy, '') = '' OR A.[UpdatedBy] = @UpdatedBy)
		AND (@UpdatedOn IS NULL OR A.[UpdatedDate] BETWEEN @StartDate AND @EndDate)
		AND (ISNULL(@Status, '') = '' OR A.[Status] = @Status)
		AND (ISNULL(@Rating, 0) = 0 OR A.[RateCandidate] = @Rating)
		AND (ISNULL(@Eligibility, '') = '' OR @Eligibility = '0' OR A.[EligibilityId] = @Eligibility)
		AND (ISNULL(@Reloc, '') = '' OR CAST(A.[Relocate] AS VARCHAR(1)) = @Reloc)
		AND (ISNULL(@JobOptions, '') = '' OR ISNULL(A.[JobOptions], '') = @JobOptions)
		AND (ISNULL(@Communications, '') = '' OR A.[Communication] = @Communications)
		AND (ISNULL(@Security, '') = '' OR CAST(A.[Background] AS VARCHAR(1)) = @Security)
		AND (@MyCandidates = 0 OR (@MyCandidates = 1 AND ((@IncludeAdmin = 1 AND (A.UpdatedBy = @User OR A.UpdatedBy = 'ADMIN')) OR (@IncludeAdmin = 0 AND A.UpdatedBy = @User))))
		AND ((@ShowArchive = 1) OR (@ShowArchive = 0 AND (A.[UpdatedDate] >= DATEFROMPARTS(YEAR(GETDATE()) - 5, 1, 1))))
	)	
	
	INSERT INTO 
		@table1
		([ID], [Count])
	SELECT DISTINCT
		[ID], A.[Count] 
	FROM (
		SELECT 
			ID, COUNT(*) OVER() [Count], A.[MPC], A.[UpdatedDate], A.[FullName], A.[Phone1], A.[Email], A.[City], A.[Code], A.[UpdatedBy], A.[Status], A.[RateCandidate], A.StateID, A.ZipCode 
		FROM CTE_Candidate A
			LEFT JOIN @StateTable S ON @SearchState = 1 AND ISNULL(@State, '') <> '' AND A.StateID = S.State
			LEFT JOIN @ZipCode Z ON @SearchState = 0 AND ISNULL(@City, '') <> '' AND A.ZipCode = Z.Zips
		WHERE 
			(@SearchState = 1 AND EXISTS (SELECT 1 FROM @StateTable) AND S.State IS NOT NULL) 
			OR (@SearchState = 0 AND EXISTS (SELECT 1 FROM @ZipCode) AND Z.Zips IS NOT NULL)
			OR (ISNULL(@City, '') = '' AND ISNULL(@State, '') = '')
		ORDER BY 
			A.[MPC] DESC, A.[UpdatedDate] DESC, A.[Status] ASC
		--ORDER BY
		--	A.[MPC] DESC, 
		--	CASE @SortDirection
		--		WHEN 0 THEN
		--			CASE @SortColumn
		--				WHEN 1 THEN A.[UpdatedDate]
		--			END
		--	END
		--	DESC,
		--	CASE @SortDirection
		--		WHEN 1 THEN
		--			CASE @SortColumn
		--				WHEN 1 THEN A.[UpdatedDate]
		--			END
		--	END
		--	ASC,
		--	CASE @SortDirection
		--		WHEN 0 THEN
		--			CASE @SortColumn
		--				WHEN 2 THEN TRIM(A.[FullName])
		--				WHEN 3 THEN TRIM(A.[Phone1])
		--				WHEN 4 THEN TRIM(A.[Email])
		--				WHEN 5 THEN TRIM(A.[Code]) + ',' + TRIM(A.[City])
		--				WHEN 7 THEN TRIM(A.[UpdatedBy])
		--				WHEN 8 THEN TRIM(A.[Status])
		--			END
		--	END
		--	DESC,
		--	CASE @SortDirection
		--		WHEN 1 THEN
		--			CASE @SortColumn
		--				WHEN 2 THEN TRIM(A.[FullName])
		--				WHEN 3 THEN TRIM(A.[Phone1])
		--				WHEN 4 THEN TRIM(A.[Email])
		--				WHEN 5 THEN TRIM(A.[Code]) + ',' + TRIM(A.[City])
		--				WHEN 7 THEN TRIM(A.[UpdatedBy])
		--				WHEN 8 THEN TRIM(A.[Status])
		--			END
		--	END
		--	ASC,
		--	CASE @SortDirection
		--		WHEN 0 THEN
		--			CASE @SortColumn
		--				WHEN 9 THEN A.[RateCandidate]
		--			END
		--	END
		--	DESC,
		--	CASE @SortDirection
		--		WHEN 1 THEN
		--			CASE @SortColumn
		--				WHEN 9 THEN A.[RateCandidate]
		--			END
		--	END
		--	ASC
		OFFSET (@PageNumber - 1) * @RecordsPerPage ROWS FETCH NEXT @RecordsPerPage ROWS ONLY) A;
	DECLARE @TotalRecs int;

	SELECT TOP 1
		@TotalRecs = A.Count
	FROM
		@table1 A;

		--select * from @table1

	if (@OptionalCandidateID = 0 OR @ThenProceed = 1)	
		BEGIN
			SELECT
				@TotalRecs;

			if ((@PageNumber - 1) * @RecordsPerPage > @TotalRecs)
				BEGIN
					SET @FirstRecord = 0;
					SET @LastRecord = @RecordsPerPage;
				END
			else
				BEGIN
					SET @FirstRecord = ((@PageNumber * @RecordsPerPage) - @RecordsPerPage) + 1;
					SET @LastRecord = (@PageNumber * @RecordsPerPage);
				END

			DECLARE @return varchar(max);
			SET @return = (
				SELECT
					Z.[Id], A.[FullName] + ' (' + CAST(A.[RequisitionCountAll] as varchar(10)) + ')' [Name], A.[Phone1] [Phone], A.[Email] [Email], 
					ISNULL(LTRIM(SUBSTRING(REPLACE(REPLACE(A.[City] + ', ' + A.[Code]  + (CASE A.[ZipCode] WHEN '' THEN '' ELSE ', ' + A.ZipCode END), ',,', ','), ',,', ','), 
						PATINDEX('%[^, ]%', REPLACE(REPLACE(A.[City] + ', ' + A.[Code]  + (CASE A.[ZipCode] WHEN '' THEN '' ELSE ', ' + A.ZipCode END), ',,', ','), ',,', ',') + 'x'), 
						LEN(REPLACE(REPLACE(A.[City] + ', ' + A.[Code]  + (CASE A.[ZipCode] WHEN '' THEN '' ELSE ', ' + A.ZipCode END), ',,', ','), ',,', ',')))), '') AS [Location],   
					FORMAT(A.[UpdatedDate], 'd', 'en-us') + ' [' + TRIM(A.[UpdatedBy]) + ']' [Updated], C.[Status], A.[MPC], A.[RateCandidate] [Rating], A.OriginalFile [OriginalResume], 
					A.FormattedFile [FormattedResume], TRIM(A.UpdatedBy) [Owner]
					--, A.[Keywords]--, A.[ORIGINAL], A.[FORMATTED]
				FROM
					@table1 Z INNER JOIN [dbo].CandidateView A ON Z.[Id] = A.[Id]
					INNER JOIN dbo.[StatusCode] C ON A.[Status] = C.[StatusCode] AND C.[AppliesTo] = 'CND'
				ORDER BY Z.Row DESC
				FOR JSON PATH);

			SELECT @return;
		END
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [GetCandidateSubmission]
	@CandidateId int = 14078,
	@RoleId varchar(2) = 'FD'
as
BEGIN;
	SET NOCOUNT ON;
	DECLARE @return varchar(max);

	SELECT @return = 
		(SELECT DISTINCT
			'[' + TRIM(C.[Code]) + '] - ' + C.[PosTitle] [Requisition], A.[UpdatedDate], UPPER(A.[UpdatedBy]) [UpdatedBy], C.[Positions],
			(SELECT COUNT(*) FROM [Submissions] D WHERE D.[RequisitionId] = A.[RequisitionId] AND (D.[Status] = 'CFM' OR D.[Status] = 'HIR')) [PositionFilled], B.[Status], 
			A.[Notes], A.[ID], 
			CAST((SELECT ISNULL((select F.[Schedule] FROM dbo.WorkflowActivity F WHERE F.[Id] = E.[Id] AND @RoleId IN (SELECT [s] FROM dbo.BigSplit(',', F.[Role]))), 0)) as bit) [Schedule], 
			B.[AppliesTo], B.[Color], B.[Icon],
			CAST((SELECT COUNT(s) FROM dbo.BigSplit(',', E.[Role]) WHERE s = @RoleId) as bit) [DoRoleHaveRight],
			UPPER(ISNULL((SELECT TOP 1 F.UpdatedBy FROM dbo.[Submissions] F WHERE F.[CandidateId] = @CandidateId AND F.[RequisitionId] = A.[RequisitionId] ORDER BY F.[UpdatedDate] DESC), '')) [LastActionBy],
			C.[Id] [RequisitionID], UPPER(ISNULL(G.[UpdatedBy], '')) [CandidateUpdatedBy],
			(SELECT COUNT(*) FROM dbo.[Submissions] Z WHERE Z.[CandidateId] = @CandidateId AND Z.[RequisitionId] = A.[RequisitionId] AND Z.[Undone] = 0) [CountSubmitted], B.[StatusCode],
			A.[ShowCalendar], A.[DateTime] [DateTimeInterview], A.[Type] [TypeOfInterview], A.[PhoneNumber], A.[InterviewDetails], A.[Undone]
		FROM
			dbo.[Submissions] A INNER JOIN dbo.[StatusCode] B ON A.[Status] = B.[StatusCode]
			INNER JOIN dbo.[Requisitions] C ON A.[RequisitionId] = C.[Id]
			INNER JOIN dbo.[WorkflowActivity] E ON A.[Status] = E.[Step]
			INNER JOIN dbo.[Candidate] G ON A.[CandidateId] = G.[Id]
		WHERE 
			A.[CandidateId] = @CandidateId
			AND B.[AppliesTo] = 'SCN'
			--AND (A.[UpdatedDate] = (SELECT MAX(F.[UpdatedDate]) FROM dbo.[Submissions] F WHERE F.[CandidateId] = @CandidateId AND F.[RequisitionId] = A.[RequisitionId])
			AND A.[Id] = (SELECT MAX(F.[Id]) FROM dbo.[Submissions] F WHERE F.[CandidateId] = @CandidateId AND F.[RequisitionId] = A.[RequisitionId])--)
		ORDER BY
			A.[UpdatedDate] DESC
		FOR JSON PATH);

	SELECT @return;
END;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [GetCompanies]
    @RecordsPerPage tinyint = 25,
    @PageNumber smallint=1,
    @SortColumn tinyint=1,
    @SortDirection tinyint=1,
	@Name varchar(30) = '',
    @UserName varchar(50) = '',
	@GetMasterTables bit = 1
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @SortOrder VARCHAR(5);
	DECLARE @table AS dbo.PagingTable;
	DECLARE @return varchar(max);

	if (LEN(@Name) = 1)
		BEGIN
			SET @Name = UPPER(@Name) + '%';
		END
	else if (LEN(@Name) > 1)
		BEGIN
			SET @Name = '%' + UPPER(@Name) + '%';
		END
		print @Name
	IF (@UserName = '')
		BEGIN
			SET @UserName = NULL;
		END;

	;WITH CTE_Company AS (
		SELECT
			A.ID, A.UpdatedDate
		FROM
			dbo.[Companies] A INNER JOIN dbo.[CompanyLocations] B ON A.ID = B.CompanyID
			LEFT JOIN dbo.[State] C ON B.[StateID] = C.[ID]
		WHERE
			((@UserName IS NULL OR @UserName = '') OR (A.[UpdatedBy] = @UserName OR A.[UpdatedBy] = 'ADMIN')) 
			AND (TRIM(ISNULL(@Name, '')) = '' OR UPPER(A.[CompanyName_BIN2]) LIKE @Name)
			AND B.[IsPrimaryLocation] = 1
	)

	INSERT INTO 
		@table
		(ID, Count)
    SELECT
        A.ID, COUNT(*) OVER() 
	FROM 
		CTE_Company A
    ORDER BY
        A.UpdatedDate DESC
	OFFSET (@PageNumber - 1) * @RecordsPerPage ROWS FETCH NEXT @RecordsPerPage ROWS ONLY;

	SELECT TOP 1
		ISNULL(A.Count, 0)
	FROM
		@table A;

    -- Final query to return paginated results
	SELECT @return =
	(SELECT 		
		B.[ID], TRIM(B.[CompanyName]) [CompanyName], D.[CompanyEmail] [Email],D.[Phone] [Phone], 
		TRIM(', ' FROM ISNULL(D.[City], '') + ', ' + ISNULL(E.[Code], '') + ', ' + ISNULL(D.[ZipCode], '')) [Address],
		ISNULL(B.[WebsiteURL], '') [Website], B.[Status], ISNULL(C.[ContactCount], 0) [ContactsCount], 
		ISNULL([LocationCount], 0) [LocationsCount], B.[UpdatedBy], B.[UpdatedDate]--, A.[TotalCount], A.AutoIncrementID
	FROM
		@table A INNER JOIN dbo.[Companies] B ON A.[ID] = B.[ID]
		LEFT JOIN dbo.[CompanyLocations] D ON B.[ID] = D.[CompanyID] AND D.[IsPrimaryLocation] = 1
		LEFT JOIN dbo.[State] E ON D.[StateID] = E.[ID] 
		LEFT JOIN 
		(
			SELECT CompanyID, COUNT(*) AS LocationCount
			FROM CompanyLocations
			GROUP BY CompanyID
		) L ON B.ID = L.CompanyID LEFT JOIN 
		(
			SELECT CompanyID, COUNT(*) AS ContactCount
			FROM CompanyContacts
			GROUP BY CompanyID
		) C ON B.ID = C.CompanyID
	ORDER BY  
		A.[Row]
	FOR JSON PATH);

	SELECT @return;
END;

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [GetCompanies_Backup]
    @RecordsPerPage tinyint = 25,
    @PageNumber smallint=1,
    @SortColumn tinyint=1,
    @SortDirection tinyint=1,
	@Name varchar(30) = '',
    @UserName varchar(50) = null,
	@GetMasterTables bit = 1
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Offset INT = (@PageNumber - 1) * @RecordsPerPage;
    DECLARE @SortOrder VARCHAR(5);
	DECLARE @table AS dbo.PagingTable;
	DECLARE @FirstRecord int = ((@PageNumber * @RecordsPerPage) - @RecordsPerPage) + 1;
	DECLARE @LastRecord int = (@PageNumber * @RecordsPerPage);
	DECLARE @return varchar(max);

	if (@Name IS NULL OR @Name = '') 
		BEGIN
			SET @Name = '%';
		END
	if (LEN(@Name) = 1 AND @Name <> '%')
		BEGIN
			SET @Name = @Name + '%';
		END
	else if (LEN(@Name) > 1)
		BEGIN
			SET @Name = '%' + @Name + '%';
		END

	IF (@UserName = '')
		BEGIN
			SET @UserName = NULL;
		END;

	INSERT INTO 
		@table
		(ID)
    SELECT
        A.ID
    FROM
        dbo.[Companies] A INNER JOIN dbo.[CompanyLocations] B ON A.ID = B.CompanyID
		LEFT JOIN dbo.[State] C ON B.[StateID] = C.[ID]
    WHERE
        ((@UserName IS NULL OR @UserName = '') OR A.[UpdatedBy] = @UserName) 
		AND A.[CompanyName] LIKE @Name
		AND B.[IsPrimaryLocation] = 1
    ORDER BY
        CASE @SortColumn
            WHEN 1 THEN TRIM(A.CompanyName)
            WHEN 2 THEN ISNULL(B.City, '') + ', ' + ISNULL(C.State, '')
            WHEN 3 THEN CONVERT(varchar(23),A.UpdatedDate,121) + A.UpdatedBy
        END
	+ CASE WHEN @SortDirection = 2 THEN ' DESC' ELSE 'ASC' END

	SELECT 
		COUNT(*)
	FROM
		@table;

    -- Final query to return paginated results
	SELECT @return =
	(SELECT 		
		A.[ID], TRIM(B.[CompanyName]) [CompanyName], CASE WHEN D.[IsPrimaryLocation] = 1 THEN D.[CompanyEmail] ELSE '' END AS Email, CASE WHEN D.[IsPrimaryLocation] = 1 THEN D.[Phone] ELSE '' END [Phone], 
		MAX(CASE WHEN D.[IsPrimaryLocation] = 1 THEN TRIM(', ' FROM ISNULL(D.[City], '') + ', ' + ISNULL(E.[Code], '') + ', ' + ISNULL(D.[ZipCode], '')) ELSE '' END) [Address],
		ISNULL(B.[WebsiteURL], '') [Website], B.[Status], (SELECT COUNT(*) FROM dbo.[CompanyContacts] Z WHERE Z.[CompanyID] = A.[ID]) [ContactsCount], 
		(SELECT COUNT(*) FROM dbo.[CompanyLocations] Z WHERE Z.[CompanyID] = A.[ID]) [LocationsCount], B.[UpdatedBy], B.[UpdatedDate]--, A.[TotalCount], A.AutoIncrementID
	FROM
		@table A INNER JOIN dbo.[Companies] B ON A.[ID] = B.[ID]
		LEFT JOIN dbo.[CompanyLocations] D ON B.[ID] = D.[CompanyID] AND D.[IsPrimaryLocation] = 1
		LEFT JOIN dbo.[State] E ON D.[StateID] = E.[ID]
	WHERE
		A.[Row] BETWEEN @FirstRecord AND @LastRecord
	GROUP BY
		A.[ID], B.[CompanyName], D.[IsPrimaryLocation], D.[CompanyEmail], D.[Phone], B.[WebsiteURL], B.[Status], B.[UpdatedBy], B.[UpdatedDate], A.[Row]
	ORDER BY
		A.[Row]
	FOR JSON PATH);

	SELECT @return;
END;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [GetCompaniesBackup]
    @RecordsPerPage tinyint = 25,
    @PageNumber smallint=1,
    @SortColumn tinyint=1,
    @SortDirection tinyint=1,
	@Name varchar(30) = '',
    @UserName varchar(50) = null,
	@GetMasterTables bit = 1
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Offset INT = (@PageNumber - 1) * @RecordsPerPage;
    DECLARE @SortOrder VARCHAR(5);
	DECLARE @table AS dbo.PagingTable;
	DECLARE @FirstRecord int = ((@PageNumber * @RecordsPerPage) - @RecordsPerPage) + 1;
	DECLARE @LastRecord int = (@PageNumber * @RecordsPerPage);
	DECLARE @return varchar(max);

	if (@Name IS NULL OR @Name = '') 
		BEGIN
			SET @Name = '%';
		END
	if (LEN(@Name) = 1 AND @Name <> '%')
		BEGIN
			SET @Name = @Name + '%';
		END
	else if (LEN(@Name) > 1)
		BEGIN
			SET @Name = '%' + @Name + '%';
		END
print @Name
	IF (@UserName = '')
		BEGIN
			SET @UserName = NULL;
		END;

	WITH CTE AS(
    SELECT
        A.ID
    FROM
        dbo.[Companies] A INNER JOIN dbo.[CompanyLocations] B ON A.ID = B.CompanyID
		LEFT JOIN [ProfessionalMaster].dbo.[State] C ON B.[StateID] = C.[ID]
    WHERE
        ((@UserName IS NULL OR @UserName = '') OR A.[UpdatedBy] = @UserName) 
		AND A.[CompanyName] LIKE @Name
		AND B.[IsPrimaryLocation] = 1)

	INSERT INTO	
		@table 
		(ID, Count)
	SELECT
		G.ID, COUNT(*) OVER()
	FROM
		CTE G INNER JOIN dbo.Companies A ON G.ID=A.ID INNER JOIN dbo.[CompanyLocations] B ON G.ID = B.CompanyID
		LEFT JOIN [ProfessionalMaster].dbo.[State] C ON B.[StateID] = C.[ID]
    ORDER BY
        CASE @SortColumn
            WHEN 1 THEN TRIM(A.CompanyName)
            WHEN 2 THEN ISNULL(B.City, '') + ', ' + ISNULL(C.State, '')
            WHEN 3 THEN CONVERT(varchar(23),A.UpdatedDate,121) + A.UpdatedBy
        END
	+ CASE WHEN @SortDirection = 2 THEN ' DESC' ELSE 'ASC' END
	OFFSET (@PageNumber - 1) * @RecordsPerPage ROWS FETCH NEXT @RecordsPerPage ROWS ONLY;

	SELECT TOP 1
		Count
	FROM
		@table;

    -- Final query to return paginated results
	SELECT @return =
	(SELECT 		
		A.[ID], TRIM(B.[CompanyName]) [CompanyName], CASE WHEN D.[IsPrimaryLocation] = 1 THEN D.[CompanyEmail] ELSE '' END AS Email, CASE WHEN D.[IsPrimaryLocation] = 1 THEN D.[Phone] ELSE '' END [Phone], 
		MAX(CASE WHEN D.[IsPrimaryLocation] = 1 THEN TRIM(', ' FROM ISNULL(D.[City], '') + ', ' + ISNULL(E.[Code], '') + ', ' + ISNULL(D.[ZipCode], '')) ELSE '' END) [Address],
		ISNULL(B.[WebsiteURL], '') [Website], B.[Status], (SELECT COUNT(*) FROM dbo.[CompanyContacts] Z WHERE Z.[CompanyID] = A.[ID]) [ContactsCount], 
		(SELECT COUNT(*) FROM dbo.[CompanyLocations] Z WHERE Z.[CompanyID] = A.[ID]) [LocationsCount], B.[UpdatedBy], B.[UpdatedDate]--, A.[TotalCount], A.AutoIncrementID
	FROM
		@table A INNER JOIN dbo.[Companies] B ON A.[ID] = B.[ID]
		LEFT JOIN dbo.[CompanyLocations] D ON B.[ID] = D.[CompanyID] AND D.[IsPrimaryLocation] = 1
		LEFT JOIN [ProfessionalMaster].dbo.[State] E ON D.[StateID] = E.[ID]
	GROUP BY
		A.[ID], B.[CompanyName], D.[IsPrimaryLocation], D.[CompanyEmail], D.[Phone], B.[WebsiteURL], B.[Status], B.[UpdatedBy], B.[UpdatedDate], A.[Row]
	ORDER BY
		A.[Row]
	FOR JSON PATH);

	SELECT @return;
END;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE   PROCEDURE [GetCompaniesList]
	@ActiveOnly bit = 1
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @return varchar(max);

	SELECT @return = 
    (SELECT
		A.[ID], A.[CompanyName], A.[UpdatedBy], A.[CreatedBy], B.[City], C.[State] + ' - [' + C.[Code] + ']' [State], B.[ZipCode]
	FROM
		dbo.[Companies] A INNER JOIN dbo.[CompanyLocations] B ON A.[ID] = B.[CompanyID] AND B.[IsPrimaryLocation] = 1
		INNER JOIN dbo.[State] C ON B.[StateID] = C.[Id]
	WHERE
		(@ActiveOnly = 0 OR 
		A.[Status] = 1)
	ORDER BY
		A.[CompanyName]
	FOR JSON PATH);

	SELECT @return;
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE   PROCEDURE [GetCompanyContactsList]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @return varchar(max);

	SELECT @return = 
    (SELECT
		A.[ID], A.[CompanyID], A.[ContactFirstName] + ' ' + A.[ContactLastName] [ContactName]
	FROM
		dbo.[CompanyContacts] A
	ORDER BY
		A.[CompanyID]
	FOR JSON PATH);

	SELECT @return;
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/*
The [dbo].[GetCompanyDetails] stored procedure retrieves comprehensive details about a company, including its information, locations, contacts, and documents. 
The data is returned in JSON format. Parameters are @CompanyID (default 284) and @User (optional). 
The procedure uses various joins to collect the necessary information from related tables and ensures the data is formatted for easy consumption.
*/
CREATE   procedure [GetCompanyDetails]
	@CompanyID int = 184,
	@User varchar(10) = NULL
AS
BEGIN
	SET STATISTICS TIME ON;
	SET NOCOUNT ON;
	DECLARE @return varchar(max);
	DECLARE @CompanyName varchar(100);

	SELECT 
		@CompanyName = TRIM(A.[CompanyName])
	FROM
		dbo.Companies A
	WHERE
		A.[ID] = @CompanyID;


	/* Company Information */
	SELECT @return = 
	(	SELECT
		A.[ID], @CompanyName [Name], B.[CompanyEmail] [EmailAddress], A.[EIN], B.[Phone] [Phone], ISNULL(B.[Extension], '') [Extension], ISNULL(B.[Fax], '') [Fax], B.[StreetName], 
		B.[City], B.[StateID], ISNULL(C.[State], '') [State], B.[ZipCode], ISNULL(A.[WebsiteURL], '') [Website], ISNULL(A.[DUN], '') [DUNS], ISNULL(A.[NAICSCode], '0') [NAICSCode], A.[Status], ISNULL(A.[Notes], '') [Notes], 
		ISNULL(B.[Notes], '') [LocationNotes],  A.[CreatedBy], A.[CreatedDate], A.[UpdatedBy], A.[UpdatedDate], ISNULL(D.NAICSTitle, '') [NAICS]
	FROM
		dbo.[Companies] A INNER JOIN dbo.[CompanyLocations] B ON A.[ID] = B.[CompanyID]
		LEFT JOIN dbo.[State] C ON B.[StateID] = C.[ID]
		LEFT JOIN dbo.[NAICS] D ON A.[NAICSCode] = D.[ID]
	WHERE 
		B.[IsPrimaryLocation] = 1 AND A.[ID] = @CompanyID
	FOR JSON PATH, WITHOUT_ARRAY_WRAPPER);
	
	SELECT @return;

	/* Company Locations */
	SELECT @return = 
	(SELECT
		A.[ID], A.[CompanyID], A.[StreetName], A.[City], A.[StateID], ISNULL(B.[State], '') [State], A.[ZipCode], A.[CompanyEmail] [EmailAddress], A.[Phone], ISNULL(A.[Extension], '') [Extension], ISNULL(A.[Fax], '') [Fax],
		A.[IsPrimaryLocation] [PrimaryLocation], ISNULL(A.[Notes], '') [Notes], ISNULL(A.[CreatedBy], 'ADMIN') [CreatedBy], ISNULL(A.[CreatedDate], GETDATE()) [CreatedDate], ISNULL(A.[UpdatedBy], 'ADMIN') [UpdatedBy], 
		ISNULL(A.[UpdatedDate], GETDATE()) [UpdatedDate], @CompanyName [CompanyName]
	FROM
		dbo.[CompanyLocations] A LEFT JOIN dbo.[State] B ON A.[StateID] = B.[ID]
	WHERE A.[CompanyID] = @CompanyID
	ORDER BY
		A.[IsPrimaryLocation] DESC
	FOR JSON PATH);
	
	SELECT @return;

	/* Company Contacts */
	SELECT @return = 
	(SELECT
		A.[ID], A.[CompanyID], ISNULL(A.[ContactPrefix], '') [Prefix], A.[ContactFirstName] [FirstName], ISNULL(A.[ContactMiddleInitial], '') [MiddleInitial],  A.[ContactLastName] [LastName], 
		ISNULL(A.[ContactSuffix], '') AS Suffix, A.[CompanyLocationID] [LocationID], B.[StreetName], B.[City], B.[StateID], C.[State], B.[ZipCode], A.[ContactEmailAddress] [EmailAddress], A.[ContactPhone] [Phone], 
		ISNULL(A.[ContactPhoneExtension], '') [Extension], ISNULL(A.[ContactFax], '') [Fax], ISNULL(A.[Designation], '') [Title], ISNULL(A.[Department], '') AS Department, A.[Role] [RoleID], D.[RoleName] [Role], 
		D.[RoleDescription] [RoleName], ISNULL(UPPER(A.[CreatedBy]), 'ADMIN') [CreatedBy], ISNULL(A.[CreatedDate], GETDATE()) [CreatedDate], ISNULL(UPPER(A.[UpdatedBy]), 'ADMIN') [UpdatedBy], 
		ISNULL(A.[UpdatedDate], GETDATE()) [UpdatedDate], ISNULL(A.[Notes], '') [Notes], @CompanyName [CompanyName]
	FROM
		dbo.[CompanyContacts] A INNER JOIN dbo.[CompanyLocations] B ON A.[CompanyLocationID] = B.[ID]
		LEFT JOIN dbo.[State] C ON B.[StateID] = C.[ID]
		INNER JOIN dbo.[Roles] D ON A.[Role] = D.[ID]
		AND A.[CompanyID] = @CompanyID
	FOR JSON PATH);
	
	SELECT @return;

	/* Company Documents */
	exec dbo.GetCompanyDocuments @CompanyID;

	/* Company Requisitions */
	SELECT @return = 
	(SELECT
		A.[Id], A.[Code], A.[Title] + ' (' + FORMAT(
		(SELECT COUNT(*) FROM dbo.Submissions AS N WHERE (RequisitionId = A.Id) AND (UpdatedDate = (SELECT MAX(UpdatedDate) FROM dbo.Submissions AS E 
		WHERE (CandidateId = N.CandidateId) AND (RequisitionId = N.RequisitionId)))), 'g0') + ')' [Title],  
		A.[Company], A.[JobOptions], A.[Status], FORMAT(A.[Updated], 'MM/dd/yyyy', 'en-us') [Updated], A.[UpdatedBy], FORMAT(A.[DueDate], 'MM/dd/yyyy', 'en-us') [Due], A.[Icon], A.[IsHot], 
		A.[SubmitCandidate], A.[CanUpdate], A.[ChangeStatus], A.[PriorityColor], A.AssignedRecruiter
	FROM
		dbo.[RequisitionView] A WITH (NOLOCK)
	WHERE 
		A.Company = @CompanyName
	ORDER BY 
		A.[Updated] DESC
	FOR JSON PATH);

	SELECT @return;
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE     procedure [GetCompanyDocuments]
	@CompanyID int=284
as
BEGIN;
	SET NOCOUNT ON;
	DECLARE @return varchar(max);

	SELECT @return = 
	(SELECT 
		A.[ID], @CompanyID [CompanyID], B.[CompanyName], A.[DocumentName], A.[OriginalFileName] [FileName], A.[InternalFileName], A.[Notes], A.[UpdatedDate], A.[UpdatedBy]
	FROM 
		[dbo].[CompanyDocuments] A INNER JOIN dbo.[Companies] B ON A.[CompanyID] = B.[ID] AND B.ID = @CompanyID
	FOR JSON PATH);

	SELECT @return;

END;


GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [GetDesignations]
AS
BEGIN;
	SET NOCOUNT ON;

	DECLARE @return varchar(max);

	SELECT @return = 
    (SELECT
		A.[Id] [KeyValue], A.[Designation] [Text]
	FROM
		dbo.[Designation] A
	WHERE
		A.[Enabled] = 1
	ORDER BY
		A.[Designation] ASC
	FOR JSON PATH);

	SELECT @return;
END;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   procedure [GetDetailCandidate]
	@CandidateId int=14078,
	@RoleId char(2)='RC'
AS
BEGIN
	SET NOCOUNT ON;
	
	DECLARE @TaxTerm varchar(100), @JobOptions varchar(100);
	DECLARE @return varchar(max) = '';

	SELECT @return =
	(SELECT
		A.[FirstName], A.[MiddleName], A.[LastName], A.[Address1], A.[Address2], A.[City], A.[StateId] [StateID], A.[ZipCode], --0-7
		A.[Email], dbo.StripPhoneNumber(A.[Phone1]) [Phone1], dbo.StripPhoneNumber(A.[Phone2]) [Phone2], dbo.StripPhoneNumber(A.[Phone3]) [Phone3], 
		CAST(A.[Phone3Ext] as varchar(10)) [PhoneExt], A.[LinkedIn], A.[Facebook], A.[Twitter], A.[Title], --8-16
		A.[EligibilityId], A.[Relocate], A.[Background], A.[JobOptions], A.[TaxTerm], A.[OriginalResume], A.[FormattedResume], --17-23
		A.[TextResume], A.[Keywords], A.[Communication], A.[RateCandidate], A.[RateNotes], A.[MPC], ISNULL(A.[MPCNotes], '') [MPCNotes], A.[ExperienceId], --24-31
		A.[HourlyRate], A.[HourlyRateHigh], A.[SalaryHigh], A.[SalaryLow], A.[RelocNotes] [RelocationNotes], A.[SecurityClearanceNotes] [SecurityNotes], A.[Refer], --32-38
		A.[ReferAccountMgr] [ReferAccountManager], A.[EEO], '' [EEOFile], A.[Summary], A.[Google] [GooglePlus], CONVERT(varchar(15), A.[CreatedDate], 101) + ' [' + A.[CreatedBy] + ']' [Created], --39-44
		CONVERT(varchar(15), A.[UpdatedDate], 101) + ' [' + A.[UpdatedBy] + ']' [Updated], @CandidateId [CandidateID], A.[Status] /*, B.[Eligibility], C.[State], D.[Experience], @TaxTerm, @JobOptions*/ --45-50	
	FROM
		dbo.[Candidate] A
	WHERE
		A.[Id] = @CandidateId 
	FOR JSON PATH, WITHOUT_ARRAY_WRAPPER);
	
	SELECT ISNULL(@return, '[]');

	SELECT @return = 
	(SELECT
		A.[Id], A.[UpdatedDate], A.[UpdatedBy], 
		REPLACE(REPLACE(REPLACE(A.[NOTES], CHAR(13) + CHAR(10), '<br/>'), CHAR(13), '<br/>'), CHAR(10), '<br/>') [Notes]
	FROM
		[dbo].[Notes] A
	WHERE
		A.[EntityId] = @CandidateId
		AND A.[EntityType] = 'CND'
	ORDER BY
		A.[IsPrimary] DESC, A.[UpdatedDate] DESC
	FOR JSON AUTO);

	SELECT ISNULL(@return, '[]');
		
	SELECT @return = 
	(SELECT
		A.[Id], B.[Skill], A.[LastUsed], A.[ExpMonth], A.[UpdatedBy]
	FROM
		dbo.[EntitySkills] A INNER JOIN dbo.[Skills] B ON A.[SkillId] = B.[Id]
	WHERE
		A.[EntityId] = @CandidateId
		AND A.[EntityType] = 'CND'
	ORDER BY
		A.[UpdatedDate] DESC
	FOR JSON PATH);
		
	SELECT ISNULL(@return, '[]');
		
	SELECT @return = 
	(SELECT
		A.[Id], A.[Degree], A.[College], ISNULL(A.[State], '') [State], A.[Country], A.[Year], A.[UpdatedBy]
	FROM
		dbo.[CandidateEducation] A
	WHERE
		A.[CandidateId] = @CandidateId
	ORDER BY
		A.[UpdatedDate] DESC
	FOR JSON AUTO);
		
	SELECT ISNULL(@return, '[]');
		
	SELECT @return = 
	(SELECT
		A.[Id], A.[Employer], A.[Start], A.[End], A.[Location], ISNULL(REPLACE(REPLACE(REPLACE(A.[Description], CHAR(13) + CHAR(10), '<br/>'), CHAR(13), '<br/>'), CHAR(10), '<br/>'), '') [Description], 
		A.[UpdatedBy], A.[Title]
	FROM
		dbo.[CandidateEmployer] A
	WHERE
		A.CandidateId = @CandidateId
	ORDER BY
		A.[UpdatedDate] DESC
	FOR JSON AUTO);

	SELECT ISNULL(@return, '[]');
		
	execute dbo.[GetCandidateSubmission] @CandidateId, @RoleId;

	SELECT
		A.[UserName], A.[FirstName] + ' ' + A.[LastName]
	FROM
		dbo.[Users] A
	--WHERE
	--	(A.[Role] = 'SM' OR A.[RoleId] = 'RS') AND A.[Status] = 'ACT'
	ORDER BY
		A.[FirstName]

	execute dbo.[GetCandidateDocuments] @CandidateId;

END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   procedure [GetDocumentType]
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @return varchar(max);

	SELECT @return = 
    (SELECT 
		A.[Id] [KeyValue], A.[DocumentType] [Text], A.[LastUpdatedDate]
	FROM 
		[dbo].[DocumentType] A
	ORDER BY
		A.[DocumentType]	
	FOR JSON PATH);

	SELECT @return;


END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [GetEducation]
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @return varchar(max);

	SELECT @return = 
    (SELECT
		A.[Id] [KeyValue], A.[Education] [Text]
	FROM
		dbo.[Education] A
	FOR JSON PATH);

	SELECT @return;
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE procedure [GetEligibility]
AS
BEGIN
	SET NOCOUNT ON;
	
	DECLARE @return varchar(max);

	SELECT @return = 
    (SELECT
		A.[Id] [KeyValue], A.[Eligibility] [Text]
	FROM
		dbo.[Eligibility] A
	ORDER BY
		A.[Id] DESC
	FOR JSON PATH);

	SELECT @return;
END


GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE procedure [GetExperience]
AS
BEGIN
	SET NOCOUNT ON;
	
	DECLARE @return varchar(max);

	SELECT @return = 
    (SELECT
		A.[Id] [KeyValue], A.[Experience] [Text]
	FROM
		dbo.[Experience] A
	FOR JSON PATH);

	SELECT @return;
END


GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE procedure [GetJobOptions]
AS 
BEGIN
	SET NOCOUNT ON;
	
	DECLARE @return varchar(max);

	SELECT @return = 
    (SELECT
		A.[JobCode] [KeyValue], A.[JobOptions] [Text], A.[Description], CONVERT(varchar(20), A.[UpdateDate], 101) [UpdatedDate], A.[DurationReq] [Duration], A.[RateReq] [Rate], A.[SalReq] [Sal], A.[TaxTerms] [Tax], A.[ExpReq] [Exp], A.[PlaceFeeReq] [PlaceFee], A.[BenefitsReq] [Benefits], A.[RateText], A.[PercentText], A.[ShowHours], ISNULL([CostPercent], 0) / 100 [CostPercent], [ShowPercent]
	FROM
		dbo.[JobOptions] A
	FOR JSON PATH);

	SELECT @return;
END


GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [GetLeadIndustry]
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @return varchar(max);

	SELECT @return = 
    (SELECT 
		A.[Id] [KeyValue], A.[Industry] [Text]
	FROM 
		[dbo].[LeadIndustry] A
	ORDER BY
		A.[Id]
	FOR JSON PATH);

	SELECT @return;

END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [GetLeadSource]
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @return varchar(max);

	SELECT @return = 
    (	SELECT 
		A.[Id] [KeyValue], A.[LeadSource] [Text]
	FROM 
		[dbo].[LeadSource] A
	ORDER BY
		A.[Id]
	FOR JSON PATH);

	SELECT @return;

END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [GetLeadStatus]
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @return varchar(max);

	SELECT @return = 
    (SELECT 
		A.[Id] [KeyValue], A.[LeadStatus] [Text]
	FROM 
		[dbo].[LeadStatus] A
	WHERE
		A.[Id] < (SELECT MAX(B.[Id]) FROM dbo.[LeadStatus] B)
	ORDER BY
		A.[Id]
	FOR JSON PATH);

	SELECT @return;

END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   procedure [GetLocationList]
@CompanyID int = 284
AS
BEGIN
SET NOCOUNT ON;

	SELECT 
		* 
	FROM (
		SELECT 
			0 AS [ID], '--SELECT--' AS [Location], '' AS [StreetAddress], '' AS [City], '' AS [State], 0 AS [StateID], '' AS [Zip], 0 AS [IsPrimaryLocation]
		UNION ALL
		SELECT TOP 100 PERCENT
			A.[ID], 
			TRIM(', ' FROM ISNULL(A.[StreetName], '') + ', ' + ISNULL(A.[City], '') + ', ' + ISNULL(B.[Code], '') + ', ' + ISNULL(A.[ZipCode], '')) AS [Location], 
			A.[StreetName] [StreetAddress], A.[City], B.[State], A.[StateID], A.[ZipCode] AS [Zip], A.IsPrimaryLocation
		FROM 
			dbo.[CompanyLocations] A 
		INNER JOIN 
			dbo.[State] B 
		ON 
			A.[StateID] = B.[ID]
		WHERE 
			A.[CompanyID] = @CompanyID
		ORDER BY A.IsPrimaryLocation
	) AS CombinedResults
	ORDER BY 
		CombinedResults.[ID]
	FOR JSON PATH;
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE   PROCEDURE [GetNAICS]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @return varchar(max);

	SELECT @return = 
    (	SELECT
		A.[ID] [KeyValue], A.[NAICSTitle] [Text]
	FROM
		dbo.NAICS A
	ORDER BY
		A.[ID]
	FOR JSON PATH);

	SELECT @return;
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [GetNotificationEmails]
    @RequisitionID INT=1654,
    @CandidateID INT=11600,
    @SendToTemplate VARCHAR(MAX)='Administrator,Candidate Owner,Full Desk'
AS
BEGIN
    SET NOCOUNT ON;

    -- Step 1: Parse the comma-separated template into individual roles/identifiers
    WITH ParsedSendTo AS (
        SELECT TRIM(value) AS Role
        FROM STRING_SPLIT(@SendToTemplate, ',')
    ),

    -- Step 2: Match users based on their Roles
    RoleBasedUsers AS (
        SELECT DISTINCT U.EmailAddress, U.FirstName + ' ' + U.LastName AS [Name]
        FROM Users U INNER JOIN Roles R ON U.Role = R.ID
        JOIN ParsedSendTo P ON P.Role = R.RoleDescription AND U.Status = 1
    ),

    -- Step 3: Match users who are the owner (CreatedBy or UpdatedBy) of the candidate
    CandidateOwners AS (
        SELECT DISTINCT U.EmailAddress, U.FirstName + ' ' + U.LastName AS [Name]
        FROM Candidate C
        JOIN Users U ON U.UserName IN (C.CreatedBy, C.UpdatedBy)
        WHERE 'Candidate Owner' IN (SELECT Role FROM ParsedSendTo)
          AND C.ID = @CandidateID AND U.Status = 1
    ),

    -- Step 4: Match users who are the owner (CreatedBy or UpdatedBy) of the requisition
    RequisitionOwners AS (
        SELECT DISTINCT U.EmailAddress, U.FirstName + ' ' + U.LastName AS [Name]
        FROM Requisitions R
        JOIN Users U ON U.UserName IN (R.CreatedBy, R.UpdatedBy)
        WHERE 'Requisition Owner' IN (SELECT Role FROM ParsedSendTo)
          AND R.ID = @RequisitionID AND U.Status = 1
    ),

    -- Step 5: Match users assigned to the requisition (AssignedRecruiter comma list)
    RequisitionAssigned AS (
        SELECT DISTINCT U.EmailAddress, U.FirstName + ' ' + U.LastName AS [Name]
        FROM Requisitions R
        CROSS APPLY STRING_SPLIT(R.AssignedRecruiter, ',') AR
        JOIN Users U ON U.UserName = TRIM(AR.value)
        WHERE 'Requisition Assigned' IN (SELECT Role FROM ParsedSendTo)
          AND R.ID = @RequisitionID AND U.Status = 1
    ),

   -- Step 6: Combine all results and return distinct email addresses by ranking
   RankedEmails AS (
     SELECT
        EmailAddress, Name, ROW_NUMBER() OVER (PARTITION BY EmailAddress ORDER BY (SELECT NULL)) AS rn
    FROM (
        SELECT EmailAddress, Name FROM RoleBasedUsers
        UNION
        SELECT EmailAddress, Name FROM CandidateOwners
        UNION
        SELECT EmailAddress, Name FROM RequisitionOwners
        UNION
        SELECT EmailAddress, Name FROM RequisitionAssigned
    ) AS CombinedEmails)

    -- Step 7: return distinct email addresses
    SELECT EmailAddress, Name FROM RankedEmails where rn=1;
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE   procedure [GetPreferences]
AS
BEGIN;
	SET NOCOUNT ON;

	DECLARE @return varchar(max);

	SELECT @return = 
	(SELECT 
		[Id], [ReqPriorityHigh] [HighPriorityColor], [ReqPriorityNormal] [NormalPriorityColor], [ReqPriorityLow] [LowPriorityColor], [AllRecruitersSubmitCandidate] [RecruitersSubmitCandidate], [AdminCandidates], 
		[AdminRequisitions], [ReqStatusChange] [ChangeRequisitionStatus], [CandStatusChange] [ChangeCandidateStatus], [ChangeCandidateSubmissionStatus], [PageSize], [SortReqonPriority] [SortOnPriority]
	FROM 
		[dbo].[Preferences]
	FOR JSON PATH, WITHOUT_ARRAY_WRAPPER);

	SELECT @return;

END;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [GetRequisitionDetails]
	@RequisitionID int = 1265,
	@RoleID varchar(2) = 'RC'
AS
BEGIN
	SET NOCOUNT ON;
	
	DECLARE @return varchar(max);

	SELECT @return = 
	(SELECT DISTINCT
		B.[CompanyName], ISNULL(E.[ContactFirstName] + ' ' + E.[ContactLastName], '') [ContactName], A.[PosTitle] [PositionTitle], A.[Description], A.[Positions], A.[Duration], A.[DurationCode], A.[StateID], 
		ISNULL(F.[Experience], '') [Experience], A.[ExpRateLow], A.[ExpRateHigh], A.[ExpLoadLow], A.[ExpLoadHigh], (A.[PlacementFee]/ 100) [PlacementFee], A.[PlacementType], C.[JobOptions], 
		ISNULL(A.[ReportTo], '') [ReportTo], A.[SalaryLow], A.[SalaryHigh], A.[ExpPaid] [ExpensesPaid], A.[ExpStart] [ExpectedStart], D.[Status], CASE A.[IsHot] WHEN 0 THEN 'Low' WHEN 1 THEN 'Medium' WHEN '2' THEN 'High' END AS [Priority], 
		A.[CreatedBy], A.[CreatedDate], A.[UpdatedBy], A.[UpdatedDate], ISNULL(A.[SkillsReq], '') [SkillsRequired], ISNULL(H.[Education], '') [Education], ISNULL(I.[Eligibility], '') [Eligibility], A.[SecurityClearance], 
		A.[Benefits], ISNULL(A.[BenefitsNotes], '') [BenefitNotes], A.[OFCCP], A.[Due] [DueDate], D.[SubmitCandidate], A.[CompanyID], A.[HiringMgrID], ISNULL(G.[State], '') [StateName], A.[City] [City], A.[Zip] [ZipCode], 
		A.[MandatoryRequirement] [Mandatory], A.[OptionalRequirement] [Optional], A.[AssignedRecruiter] [AssignedTo], A.[IsHot] AS [PriorityID], ISNULL(A.[Eligibility], 0) [EligibilityID], 
		ISNULL(A.[ExperienceId], 0) [ExperienceID], ISNULL(A.[Education], 0) [EducationID], A.[JobOption] [JobOptionID], A.[Status] [StatusCode], ISNULL(Z.[StreetName], '') + ISNULL(', ' + Z.[City], '') [CompanyCity], 
		ISNULL(J.[State] + '- [' + J.[Code] + ']', '') [CompanyState], ISNULL(Z.[ZipCode], '') [CompanyZip], @RequisitionID [RequisitionID]
	FROM
		dbo.[Requisitions] A LEFT JOIN dbo.[Companies] B ON A.[CompanyId] = B.[Id]
		LEFT JOIN CompanyLocations Z ON B.ID = Z.CompanyID AND Z.IsPrimaryLocation=1
		LEFT JOIN dbo.[State] J ON Z.[StateId] = J.[Id]
		LEFT JOIN dbo.[JobOptions] C ON A.[JobOption] = C.[JobCode]
		LEFT JOIN dbo.[StatusCode] D ON A.[Status] = D.[StatusCode]
		LEFT JOIN dbo.[State] G ON A.[StateId] = G.[Id]
		LEFT JOIN dbo.[Experience] F ON A.[ExperienceId] = F.[Id]
		LEFT JOIN dbo.[Education] H ON A.[Education] = H.[Id]
		LEFT JOIN dbo.[Eligibility] I ON A.[Eligibility] = I.[Id]
		LEFT JOIN dbo.[CompanyContacts] E ON A.[HiringMgrId] = E.[Id]
	WHERE
		A.[Id] = @RequisitionID
	FOR JSON PATH, WITHOUT_ARRAY_WRAPPER);

	SELECT @return;

	exec dbo.[GetRequisitionSubmission] @RequisitionID, @RoleID;

	exec dbo.[GetRequisitionDocuments] @RequisitionID;

	SELECT @return = 
	(SELECT 
		A.Id, A.UpdatedDate, A.UpdatedBy, A.Notes
	FROM
		dbo.Notes A
	WHERE
		A.EntityId = @RequisitionID AND A.EntityType = 'REQ'
	FOR JSON PATH);

	SELECT @return;
		
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE   procedure [GetRequisitionDocuments]
	@RequisitionId int=1265
as
BEGIN;
	SET NOCOUNT ON;

	DECLARE @return varchar(max);

	SELECT @return = 
	(SELECT
		A.[RequisitionDocId] [ID], A.[RequisitionID], A.[DocumentName] [Name], A.[DocumentLocation] [Location], A.[LastUpdatedBy] [UpdateBy], A.[LastUpdatedDate], A.[Notes] [Notes], 
		A.[InternalFileName], B.[UpdatedBy] [RequisitionOwner]
	FROM
		dbo.[RequisitionDocument] A INNER JOIN dbo.[Requisitions] B ON A.[RequisitionId] = B.[Id]
	WHERE
		A.[RequisitionId] = @RequisitionID
	FOR JSON PATH);

	SELECT @return;

END;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [GetRequisitions]
	@Count int = 25,
	@Page int = 1,
	@SortRow int = 1, --1-Created Date, 2-Code, 3-Title,4-Company, 5-Option, 6-Status, 7-Created By, 8-Due
	@SortOrder bit = 0, -- 1-ASC, 0-DESC, NULL - Remove Sort,
	@Code varchar(15) = '',
	@Title varchar(200) = '',
	@Company varchar(200) = '',
	@Option varchar(10) = '',
	@Status varchar(100) = '',--,Open,Partially Filled',
	@CreatedBy varchar(10) = '',
	@CreatedOn datetime = '1900-01-01',
	@CreatedOnEnd datetime = '3001-01-01',
	@Due datetime = '1900-01-01',
	@DueEnd datetime = '3001-01-01',
	@Recruiter bit = 1,
	@GetCompanyInformation bit = 1,
	@User varchar(10) = 'ADMIN',
	@OptionalRequisitionID int = 0,
	@ThenProceed bit = 1,
	@LoggedUser varchar(10) = '%'
AS
BEGIN
	SET NOCOUNT ON;
	DECLARE @table dbo.PagingTable;
	DECLARE @table1 dbo.TempTable;
	DECLARE @PageCount tinyint = 50, @ReqChangeStatus tinyint, @AdminRequisitions bit, @RecSubmitCand bit;
	DECLARE @ReqLow varchar(7), @ReqHigh varchar(7), @ReqMed varchar(7);--@PageCount = A.[PageSize],
	SELECT
		@ReqChangeStatus = A.[ReqStatusChange], @AdminRequisitions = A.[AdminRequisitions],
		@ReqLow = A.[ReqPriorityLow], @ReqHigh = A.[ReqPriorityHigh], @ReqMed = A.[ReqPriorityNormal],
		@RecSubmitCand = A.[AllRecruitersSubmitCandidate]
	FROM
		dbo.[Preferences] A;

	DECLARE @FirstRecord int = ((@Page * @Count) - @Count) + 1;

	DECLARE @LastRecord int = (@Page * @Count);

	DECLARE @statusTable table ([Status] varchar(30));
	DECLARE @AssignedUser varchar(10);
	 
	if (@SortOrder IS NULL)
		BEGIN
			SET @SortOrder = 0;
			SET @SortRow = 1;
		END;

	if (@Code IS NOT NULL AND TRIM(@Code) <> '')
		BEGIN
			SET @Code = '%' + UPPER(TRIM(@Code)) + '%';
		END
		
	if (@Title IS NOT NULL AND TRIM(@Title) <> '')
		if (LEN(@Title) = 1)
			BEGIN
				SET @Title = UPPER(TRIM(@Title)) + '%';
			END
		else
			BEGIN
				SET @Title = '%' + UPPER(TRIM(@Title)) + '%';
			END

	if (@Status IS NULL OR @Status = '')
		BEGIN
			INSERT INTO @statusTable VALUES('%');
			--SET @Status = '%';
		END
	else
		BEGIN
			INSERT INTO @statusTable
			SELECT
				A.[s]
			FROM
				dbo.BigSplit(',', @Status) A;
		END;

	if (@CreatedBy IS NULL OR @CreatedBy = '')
		BEGIN
			SET @CreatedBy = '%';
		END
	else
		BEGIN
			SET @CreatedBy = @CreatedBy;
		END

	DECLARE @StartDate datetime, @EndDate datetime;
	if (@CreatedOn IS NULL OR @CreatedOn = '')
		BEGIN
			SELECT
				@StartDate = DATEADD(d, -1, MIN(A.[UpdatedDate])), @EndDate = DATEADD(d, 1, MAX(A.[UpdatedDate]))
			FROM
				dbo.[Requisitions] A;
		END
	else
		BEGIN
			if ((@CreatedOn IS NOT NULL OR @CreatedOn <> '') AND (@CreatedOnEnd IS NOT NULL OR @CreatedOnEnd <> ''))
				BEGIN
					SET @StartDate = @CreatedOn;
					SET @EndDate = @CreatedOnEnd;
				END
			else
				BEGIN
					SET @StartDate = @CreatedOn;
					SELECT
						@EndDate = DATEADD(d, 1, MAX(A.[UpdatedDate]))
					FROM
						dbo.[Requisitions] A;
				END
		END

	DECLARE @StartDue datetime, @EndDue datetime;

	if (@Due IS NULL OR @Due = '')
		BEGIN
			SELECT
				@StartDue = DATEADD(d, -1, MIN(A.[Due])), @EndDue = DATEADD(d, 1, MAX(A.[Due]))
			FROM
				dbo.[Requisitions] A;
		END
	else
		BEGIN
			if (@Due IS NOT NULL AND @DueEnd IS NOT NULL)
				BEGIN
					SET @StartDue = @Due;
					SET @EndDue = @DueEnd;
				END
			else
				BEGIN
					SET @StartDue = @Due;
					SELECT
						@EndDue = DATEADD(d, 1, MAX(A.[Due]))
					FROM
						dbo.[Requisitions] A;
				END
		END

	;WITH CTE_Reqs AS (
	SELECT
		A.[Id], A.UpdatedBy, A.AssignedRecruiter, A.IsHot, A.Status, A.Updated--, A.DueDate, A.Code, A.Title, A.Company, A.JobOptions
	FROM
		dbo.[RequisitionView] A WITH (NOLOCK)
	WHERE 
		(A.[UpdatedBy] LIKE (CASE @CreatedBy WHEN 'M' THEN @User WHEN 'A' THEN @User ELSE '%' END)
		OR A.[CreatedBy] LIKE (CASE @CreatedBy WHEN 'M' THEN @User WHEN 'A' THEN @User ELSE '%' END)
		OR (CASE @CreatedBy WHEN 'M' THEN 'atozdummyuser' ELSE @User END IN (SELECT [s] FROM dbo.[BigSplit](',', A.[AssignedRecruiter])))
		OR A.[UpdatedBy] = 'ADMIN' OR A.[CreatedBy] = 'ADMIN')
		AND (TRIM(ISNULL(@Code, '')) = '' OR UPPER(A.[Code] COLLATE Latin1_General_BIN2) LIKE @Code)
		AND (TRIM(ISNULL(@Title, '')) = '' OR UPPER(A.[Title] COLLATE Latin1_General_BIN2) LIKE @Title)
		AND (TRIM(ISNULL(@Company, '')) = '' OR A.[Company] = @Company)
		AND (TRIM(ISNULL(@Option, '')) = '' OR A.[JobOption] = @Option)
		AND (TRIM(ISNULL(@Status, '')) = '' OR A.[Status] IN (SELECT [Status] FROM @statusTable))
		AND ((A.[Updated] BETWEEN @StartDate AND @EndDate)
		AND (A.[DueDate] BETWEEN @StartDue AND @EndDue)))

	INSERT INTO 
		@table (ID, Count)
	SELECT A.ID, COUNT(*) OVER() FROM CTE_Reqs A
	ORDER BY  --1-Created Date, 2-Code, 3-Title,4-Company, 5-Option, 6-Status, 7-Created By, 8-Due
		A.[IsHot] DESC,
		CASE WHEN @SortRow <> 6 THEN 			
			CASE
				A.[Status] WHEN 'NEW'  THEN 0 
				WHEN 'OPN' THEN 2
				WHEN 'INA' THEN 3
				WHEN 'FUL' THEN 4
				WHEN 'PAR' THEN 5
				WHEN 'CLO' THEN 6
				ELSE 7
			END
		ELSE 7
		END,
		A.[Updated] DESC
	OFFSET (@Page - 1) * @Count ROWS FETCH NEXT @Count ROWS ONLY;

	DECLARE @TotalRecs int;
	
	SELECT TOP 1
		@TotalRecs = ISNULL(A.[Count], 0)
	FROM
		@table A;
		

	DECLARE @Position int = 0;
		
	if (@OptionalRequisitionID > 0)
		BEGIN
			SELECT
				@Position = A.[Row]
			FROM
				@table A
			WHERE
				A.[Id] = @OptionalRequisitionID;

			SET @Page = CAST(CEILING(cast(@Position as decimal(10,2)) / cast(@Count as decimal(10,2))) as int);
		END

	if (@OptionalRequisitionID > 0 AND @ThenProceed = 0)
		BEGIN
			SELECT
				@Page;
		END

	if (@OptionalRequisitionID = 0 OR @ThenProceed = 1)	
		BEGIN

			SELECT
				@TotalRecs;

			if ((@Page - 1) * @Count > @TotalRecs)
				BEGIN
					SET @FirstRecord = 0;
					SET @LastRecord = @Count;
				END
			else
				BEGIN
					SET @FirstRecord = ((@Page * @Count) - @Count) + 1;
					SET @LastRecord = (@Page * @Count);
				END

			DECLARE @return varchar(max);

			SELECT @return = 
			(SELECT
				A.[Id], A.[Code], A.[Title] + ' (' + FORMAT(
				(SELECT COUNT(*) FROM dbo.Submissions AS N WHERE (RequisitionId = A.Id) AND (UpdatedDate = (SELECT MAX(UpdatedDate) FROM dbo.Submissions AS E 
				WHERE (CandidateId = N.CandidateId) AND (RequisitionId = N.RequisitionId)))), 'g0') + ')' [Title],  
				A.[Company], A.[JobOptions], A.[Status], FORMAT(A.[Updated], 'MM/dd/yyyy', 'en-us') [Updated], A.[UpdatedBy], FORMAT(A.[DueDate], 'MM/dd/yyyy', 'en-us') [Due], A.[Icon], A.[IsHot], 
				A.[SubmitCandidate], A.[CanUpdate], A.[ChangeStatus], A.[PriorityColor], A.AssignedRecruiter
			FROM
				@table Z INNER JOIN dbo.[RequisitionView] A WITH (NOLOCK) ON Z.[Id] = A.[Id]
			ORDER BY
				Z.[Row]
			FOR JSON PATH);
			
			SELECT @return;

			if (@GetCompanyInformation = 1)
				BEGIN
					SELECT
						MIN(A.[Status]) [KeyValue], B.Status + ' (' +  FORMAT(COUNT(*),'g0') + ')' [Text]
					FROM 
						dbo.[Requisitions] A INNER JOIN dbo.[StatusCode] B ON A.[Status] = B.[StatusCode]
					WHERE
						B.AppliesTo='REQ'
					GROUP BY
						B.[Status]
					FOR JSON PATH;
				END
			else
				BEGIN
					SELECT 
						1
					WHERE
						1=2;
				END

			SELECT
				@Page;
		END
END

		/*
		===================================================
		ARCHIVED: Previous dynamic ORDER BY logic
		Date: 2025-04-09
		Reason: Requirement changed to fixed Updated DESC order
		Retained for possible reuse in the future
		===================================================
		*/
		/*CASE
			WHEN 1 = (SELECT TOP 1 N.SortReqonPriority FROM dbo.[Preferences] N) THEN A.[IsHot]
			ELSE 0
		END DESC,
		CASE @SortOrder 
			WHEN 0 THEN
				CASE @SortRow
					WHEN 1 THEN A.[Updated]
					WHEN 8 THEN A.[DueDate]
				END
		END
		DESC,
		CASE @SortOrder
			WHEN 1 THEN
				CASE @SortRow
					WHEN 1 THEN A.[Updated]
					WHEN 8 THEN A.[DueDate]
				END
		END
		ASC,
		CASE @SortOrder
			WHEN 0 THEN
				CASE @SortRow
					WHEN 2 THEN RTRIM(LTRIM(A.[Code]))
					WHEN 3 THEN RTRIM(LTRIM(A.[Title]))
					WHEN 4 THEN RTRIM(LTRIM(A.[Company]))
					WHEN 5 THEN RTRIM(LTRIM(A.[JobOptions]))
					WHEN 6 THEN RTRIM(LTRIM(A.[Status]))
					WHEN 7 THEN RTRIM(LTRIM(A.[UpdatedBy]))
				END
		END
		DESC,
		CASE @SortOrder
			WHEN 1 THEN
				CASE @SortRow
					WHEN 2 THEN RTRIM(LTRIM(A.[Code]))
					WHEN 3 THEN RTRIM(LTRIM(A.[Title]))
					WHEN 4 THEN RTRIM(LTRIM(A.[Company]))
					WHEN 5 THEN RTRIM(LTRIM(A.[JobOptions]))
					WHEN 6 THEN RTRIM(LTRIM(A.[Status]))
					WHEN 7 THEN RTRIM(LTRIM(A.[UpdatedBy]))
				END
		END
		ASC*/
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [GetRequisitions-Backup]
	@Count int = 25,
	@Page int = 1,
	@SortRow int = 1, --1-Created Date, 2-Code, 3-Title,4-Company, 5-Option, 6-Status, 7-Created By, 8-Due
	@SortOrder bit = 0, -- 1-ASC, 0-DESC, NULL - Remove Sort,
	@Code varchar(15) = '',
	@Title varchar(200) = '',
	@Company varchar(200) = '',
	@Option varchar(10) = '',
	@Status varchar(50) = 'New,Open,Partially Filled',
	@CreatedBy varchar(10) = '',
	@CreatedOn datetime = '1900-01-01',
	@CreatedOnEnd datetime = '3001-01-01',
	@Due datetime = '1900-01-01',
	@DueEnd datetime = '3001-01-01',
	@Recruiter bit = 1,
	@GetCompanyInformation bit = 1,
	@User varchar(10) = '%',
	@OptionalRequisitionID int = 0,
	@ThenProceed bit = 1,
	@LoggedUser varchar(10) = '%'
AS
BEGIN
	SET NOCOUNT ON;
	DECLARE @table dbo.PagingTable;
	DECLARE @table1 dbo.TempTable;
	DECLARE @PageCount tinyint = 50, @ReqChangeStatus tinyint, @AdminRequisitions bit, @RecSubmitCand bit;
	DECLARE @ReqLow varchar(7), @ReqHigh varchar(7), @ReqMed varchar(7);--@PageCount = A.[PageSize],
	SELECT
		@ReqChangeStatus = A.[ReqStatusChange], @AdminRequisitions = A.[AdminRequisitions],
		@ReqLow = A.[ReqPriorityLow], @ReqHigh = A.[ReqPriorityHigh], @ReqMed = A.[ReqPriorityNormal],
		@RecSubmitCand = A.[AllRecruitersSubmitCandidate]
	FROM
		dbo.[Preferences] A;

	DECLARE @FirstRecord int = ((@Page * @Count) - @Count) + 1;

	DECLARE @LastRecord int = (@Page * @Count);

	DECLARE @statusTable table ([Status] varchar(30));
	DECLARE @AssignedUser varchar(10);
	 
	if (@SortOrder IS NULL)
		BEGIN
			SET @SortOrder = 0;
			SET @SortRow = 1;
		END;
		
	if (@Title IS NOT NULL AND @Title <> '')
		if (LEN(@Title) = 1)
			BEGIN
				SET @Title = TRIM(@Title) + '%';
			END
		else
			BEGIN
				SET @Title = '%' + TRIM(@Title) + '%';
			END
	
	if (@Company IS NOT NULL AND @Company <> '')
		BEGIN
			if (LEN(@Company) = 1)
				BEGIN
					SET @Company = @Company + '%';
				END
			else
				BEGIN
					SET @Company = '%' + @Company + '%';
				END
		END

	if (@Status IS NULL OR @Status = '')
		BEGIN
			INSERT INTO @statusTable VALUES('%');
			--SET @Status = '%';
		END
	else
		BEGIN
			INSERT INTO @statusTable
			SELECT
				A.[s]
			FROM
				dbo.BigSplit(',', @Status) A;
		END;

	if (@CreatedBy IS NULL OR @CreatedBy = '')
		BEGIN
			SET @CreatedBy = '%';
		END
	else
		BEGIN
			SET @CreatedBy = @CreatedBy;
		END

	DECLARE @StartDate datetime, @EndDate datetime;
	if (@CreatedOn IS NULL OR @CreatedOn = '')
		BEGIN
			SELECT
				@StartDate = DATEADD(d, -1, MIN(A.[UpdatedDate])), @EndDate = DATEADD(d, 1, MAX(A.[UpdatedDate]))
			FROM
				dbo.[Requisitions] A;
		END
	else
		BEGIN
			if ((@CreatedOn IS NOT NULL OR @CreatedOn <> '') AND (@CreatedOnEnd IS NOT NULL OR @CreatedOnEnd <> ''))
				BEGIN
					SET @StartDate = @CreatedOn;
					SET @EndDate = @CreatedOnEnd;
				END
			else
				BEGIN
					SET @StartDate = @CreatedOn;
					SELECT
						@EndDate = DATEADD(d, 1, MAX(A.[UpdatedDate]))
					FROM
						dbo.[Requisitions] A;
				END
		END

	DECLARE @StartDue datetime, @EndDue datetime;

	if (@Due IS NULL OR @Due = '')
		BEGIN
			SELECT
				@StartDue = DATEADD(d, -1, MIN(A.[Due])), @EndDue = DATEADD(d, 1, MAX(A.[Due]))
			FROM
				dbo.[Requisitions] A;
		END
	else
		BEGIN
			if (@Due IS NOT NULL AND @DueEnd IS NOT NULL)
				BEGIN
					SET @StartDue = @Due;
					SET @EndDue = @DueEnd;
				END
			else
				BEGIN
					SET @StartDue = @Due;
					SELECT
						@EndDue = DATEADD(d, 1, MAX(A.[Due]))
					FROM
						dbo.[Requisitions] A;
				END
		END

	;WITH CTE_Reqs AS (
	SELECT
		A.[Id], A.UpdatedBy, A.AssignedRecruiter, A.IsHot, A.Status, A.Updated, A.DueDate, A.Code, A.Title, A.Company, A.JobOptions
	FROM
		dbo.[RequisitionView] A WITH (NOLOCK)
	WHERE 
		(A.[UpdatedBy] LIKE (CASE @CreatedBy WHEN 'M' THEN @User WHEN 'A' THEN @User ELSE '%' END)
		OR A.[CreatedBy] LIKE (CASE @CreatedBy WHEN 'M' THEN @User WHEN 'A' THEN @User ELSE '%' END)
		OR (CASE @CreatedBy WHEN 'M' THEN 'atozdummyuser' ELSE @User END IN (SELECT [s] FROM dbo.[BigSplit](',', A.[AssignedRecruiter])))
		OR A.[UpdatedBy] = 'ADMIN' OR A.[CreatedBy] = 'ADMIN')
		AND (TRIM(ISNULL(@Code, '')) = '' OR A.[Code] LIKE '%' + TRIM(@Code) + '%')
		AND (TRIM(ISNULL(@Title, '')) = '' OR A.[Title] LIKE TRIM(@Title))
		AND (TRIM(ISNULL(@Company, '')) = '' OR A.[Company] LIKE @Company)
		AND (TRIM(ISNULL(@Option, '')) = '' OR A.[JobOption] LIKE '%' + @Option + '%')
		AND (TRIM(ISNULL(@Status, '')) = '' OR A.[Status] IN (SELECT [Status] FROM @statusTable))
		AND ((A.[Updated] BETWEEN @StartDate AND @EndDate)
		AND (A.[DueDate] BETWEEN @StartDue AND @EndDue)))

	INSERT INTO 
		@table (ID, Count)
	SELECT A.ID, COUNT(*) OVER() FROM CTE_Reqs A
	ORDER BY  --1-Created Date, 2-Code, 3-Title,4-Company, 5-Option, 6-Status, 7-Created By, 8-Due
		A.[IsHot] DESC,
		CASE 
			WHEN @User = A.[UpdatedBy] THEN 'A'
			--WHEN E.[RoleId] = 'AD' THEN 'B'
			WHEN @User IN (SELECT [s] FROM dbo.[BigSplit](',', A.[AssignedRecruiter])) THEN 'C' 
			ELSE 'D' 
		END ASC,
		CASE WHEN @SortRow <> 6 THEN 			
			CASE
				A.[Status] WHEN 'NEW'  THEN 0 
				WHEN 'OPN' THEN 2
				WHEN 'INA' THEN 3
				WHEN 'FUL' THEN 4
				WHEN 'PAR' THEN 5
				WHEN 'CLO' THEN 6
				ELSE 7
			END
		ELSE 7
		END,
		A.[Updated] DESC
	OFFSET (@Page - 1) * @Count ROWS FETCH NEXT @Count ROWS ONLY;

	DECLARE @TotalRecs int;
	
	SELECT TOP 1
		@TotalRecs = ISNULL(A.[Count], 0)
	FROM
		@table A;
		

	DECLARE @Position int = 0;
		
	if (@OptionalRequisitionID > 0)
		BEGIN
			SELECT
				@Position = A.[Row]
			FROM
				@table A
			WHERE
				A.[Id] = @OptionalRequisitionID;

			SET @Page = CAST(CEILING(cast(@Position as decimal(10,2)) / cast(@Count as decimal(10,2))) as int);
		END

	if (@OptionalRequisitionID > 0 AND @ThenProceed = 0)
		BEGIN
			SELECT
				@Page;
		END

	if (@OptionalRequisitionID = 0 OR @ThenProceed = 1)	
		BEGIN

			SELECT
				@TotalRecs;

			if ((@Page - 1) * @Count > @TotalRecs)
				BEGIN
					SET @FirstRecord = 0;
					SET @LastRecord = @Count;
				END
			else
				BEGIN
					SET @FirstRecord = ((@Page * @Count) - @Count) + 1;
					SET @LastRecord = (@Page * @Count);
				END

			DECLARE @return varchar(max);

			SELECT @return = 
			(SELECT
				A.[Id], A.[Code], A.[Title] + ' (' + FORMAT(
				(SELECT COUNT(*) FROM dbo.Submissions AS N WHERE (RequisitionId = A.Id) AND (UpdatedDate = (SELECT MAX(UpdatedDate) FROM dbo.Submissions AS E 
				WHERE (CandidateId = N.CandidateId) AND (RequisitionId = N.RequisitionId)))), 'g0') + ')' [Title],  
				A.[Company], A.[JobOptions], A.[Status], FORMAT(A.[Updated], 'd', 'en-us') [Updated], A.[UpdatedBy], A.[DueDate] [Due], A.[Icon], A.[IsHot], 
				A.[SubmitCandidate], A.[CanUpdate], A.[ChangeStatus], A.[PriorityColor], A.AssignedRecruiter
			FROM
				@table Z INNER JOIN dbo.[RequisitionView] A WITH (NOLOCK) ON Z.[Id] = A.[Id]
			ORDER BY
				Z.[Row]
			FOR JSON PATH);
			
			SELECT @return;

			if (@GetCompanyInformation = 1)
				BEGIN
					SELECT
						MIN(A.[Status]) [KeyValue], B.Status + ' (' +  FORMAT(COUNT(*),'g0') + ')' [Text]
					FROM 
						dbo.[Requisitions] A INNER JOIN dbo.[StatusCode] B ON A.[Status] = B.[StatusCode]
					WHERE
						B.AppliesTo='REQ'
					GROUP BY
						B.[Status]
					FOR JSON PATH;
				END
			else
				BEGIN
					SELECT 
						1
					WHERE
						1=2;
				END

			SELECT
				@Page;
		END
END

		/*
		===================================================
		ARCHIVED: Previous dynamic ORDER BY logic
		Date: 2025-04-09
		Reason: Requirement changed to fixed Updated DESC order
		Retained for possible reuse in the future
		===================================================
		*/
		/*CASE
			WHEN 1 = (SELECT TOP 1 N.SortReqonPriority FROM dbo.[Preferences] N) THEN A.[IsHot]
			ELSE 0
		END DESC,
		CASE @SortOrder 
			WHEN 0 THEN
				CASE @SortRow
					WHEN 1 THEN A.[Updated]
					WHEN 8 THEN A.[DueDate]
				END
		END
		DESC,
		CASE @SortOrder
			WHEN 1 THEN
				CASE @SortRow
					WHEN 1 THEN A.[Updated]
					WHEN 8 THEN A.[DueDate]
				END
		END
		ASC,
		CASE @SortOrder
			WHEN 0 THEN
				CASE @SortRow
					WHEN 2 THEN RTRIM(LTRIM(A.[Code]))
					WHEN 3 THEN RTRIM(LTRIM(A.[Title]))
					WHEN 4 THEN RTRIM(LTRIM(A.[Company]))
					WHEN 5 THEN RTRIM(LTRIM(A.[JobOptions]))
					WHEN 6 THEN RTRIM(LTRIM(A.[Status]))
					WHEN 7 THEN RTRIM(LTRIM(A.[UpdatedBy]))
				END
		END
		DESC,
		CASE @SortOrder
			WHEN 1 THEN
				CASE @SortRow
					WHEN 2 THEN RTRIM(LTRIM(A.[Code]))
					WHEN 3 THEN RTRIM(LTRIM(A.[Title]))
					WHEN 4 THEN RTRIM(LTRIM(A.[Company]))
					WHEN 5 THEN RTRIM(LTRIM(A.[JobOptions]))
					WHEN 6 THEN RTRIM(LTRIM(A.[Status]))
					WHEN 7 THEN RTRIM(LTRIM(A.[UpdatedBy]))
				END
		END
		ASC*/
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   procedure [GetRequisitionSubmission]
	@RequisitionId int=1665,
	@RoleId varchar(2)='RS'
as
BEGIN;
	SET NOCOUNT ON;

	DECLARE @return varchar(max);

	SELECT @return = 
	(
	SELECT DISTINCT
		C.[FirstName] + ' ' + C.[LastName] [Requisition], A.[UpdatedDate], A.[UpdatedBy], D.[Positions],
		(SELECT COUNT(*) FROM [Submissions] E WHERE E.[RequisitionId] = A.[RequisitionId] AND (E.[Status] = 'CFM' OR E.[Status] = 'HIR')) [PositionFilled],
		B.[Status], A.[Notes], A.[ID], 
		CAST((SELECT ISNULL((select WF.[Schedule] FROM [ProfessionalMaster].dbo.WorkflowActivity WF WHERE WF.[Id] = E.[Id] AND @RoleId IN (SELECT [s] FROM dbo.BigSplit(',', WF.[Role]))), 0)) as bit) [Schedule], 
		B.[AppliesTo], B.[Color], B.[Icon],
		CAST((SELECT COUNT(s) FROM dbo.BigSplit(',', E.[Role]) WHERE s = @RoleId) as bit) [DoRoleHaveRight],
		ISNULL((SELECT TOP 1 F.UpdatedBy FROM dbo.[Submissions] F WHERE F.[CandidateId] = A.[CandidateId] AND F.[RequisitionId] = @RequisitionId ORDER BY F.[UpdatedDate] DESC), '') [LastActionBy],
		C.[Id] [RequisitionID], G.[UpdatedBy] [CandidateUpdatedBy],
		(SELECT COUNT(*) FROM dbo.[Submissions] Z WHERE Z.[CandidateId] = A.[CandidateId] AND Z.[RequisitionId] = @RequisitionId AND Z.[Undone] = 0) [CountSubmitted], B.[StatusCode],
		A.[ShowCalendar], A.[DateTime] [DateTimeInterview], A.[Type] [TypeOfInterview], A.[PhoneNumber], A.[InterviewDetails], A.[Undone]
	FROM
		dbo.[Submissions] A INNER JOIN dbo.[StatusCode] B ON A.[Status] = B.[StatusCode]
		INNER JOIN dbo.[Candidate] C ON A.[CandidateId] = C.[Id]
		INNER JOIN dbo.[Requisitions] D ON A.[RequisitionId] = D.[Id]
		INNER JOIN dbo.[StatusCode] F ON D.[Status] = F.[StatusCode]
		LEFT JOIN dbo.[WorkflowActivity] E ON A.[Status] = E.[Step]
		INNER JOIN dbo.[Candidate] G ON A.[CandidateId] = G.[Id]
	WHERE 
		A.[RequisitionId] = @RequisitionId
		AND B.[AppliesTo] = 'SCN'
		AND A.[Id] IN (SELECT [INNERTABLE].[Id] FROM (
				SELECT Id, ROW_NUMBER() OVER (PARTITION BY CandidateId, RequisitionId ORDER BY UpdatedDate DESC) AS INNERROW
				FROM Submissions Z WHERE RequisitionId = @RequisitionId AND CandidateId = A.[CandidateId]
			) AS INNERTABLE
			WHERE [INNERTABLE].[INNERROW] = 1)
	ORDER BY
		A.[UpdatedDate] DESC
	FOR JSON PATH
	);

	SELECT @return;

END;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [GetRoles]
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @return varchar(max);

	SELECT @return = 
    (SELECT 
		A.[Id] [ID], A.[RoleName], A.[RoleDescription] [Description], A.[CreateOrEditCompany], A.[CreateOrEditCandidate], A.[ViewAllCompanies], A.[ViewMyCompanyProfile], A.[EditMyCompanyProfile],
		A.[CreateOrEditRequisitions], A.[ViewOnlyMyCandidates], A.[ViewAllCandidates], A.[ManageSubmittedCandidates], A.[DownloadOriginal], A.[DownloadFormatted], A.[ViewRequisitions],
		A.[EditRequisitions], A.[AdminScreens], A.[CreatedBy], A.[CreatedDate], A.[UpdatedBy], A.[UpdatedDate]
	FROM 
		[dbo].[Roles] A
	FOR JSON PATH);

	SELECT @return;
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE procedure [GetSkills]
AS
BEGIN
	SET NOCOUNT ON;
	
	DECLARE @return varchar(max);

	SELECT @return = 
    (SELECT
		A.[Skill] [Text], A.[Id] [KeyValue]
	FROM
		dbo.[Skills] A
	ORDER BY
		A.[Skill]
	FOR JSON PATH);

	SELECT @return;
END


GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [GetStates]
AS 
BEGIN
	SET NOCOUNT ON;
	
	DECLARE @return varchar(max);

	SELECT @return = 
    (	SELECT
		A.[Id] [KeyValue], A.[State] [Text], A.[Code]
	FROM
		dbo.[State] A
	FOR JSON PATH);

	SELECT @return;

END


GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [GetStatus]
AS
BEGIN
	SET NOCOUNT ON;
	
	DECLARE @return varchar(max);

	SELECT @return = 
    (SELECT
		A.[StatusCode] [Code], A.[Status], A.[Icon], A.[AppliesTo] [AppliesToCode], A.[SubmitCandidate], A.[ShowCommission], A.[Id] [ID]
	FROM
		dbo.[StatusCode] A
	FOR JSON PATH);

	SELECT @return;
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE procedure [GetTaxTerms]
AS
BEGIN
	SET NOCOUNT ON;
	
	DECLARE @return varchar(max);

	SELECT @return = 
    (SELECT
		A.[TaxTermCode] [KeyValue], A.[TaxTerm] [Text]
	FROM
		dbo.[TaxTerm] A
	FOR JSON PATH);

	SELECT @return;
END


GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE   PROCEDURE [GetUsers]
	@ActiveOnly bit = 1
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @return varchar(max);

	SELECT @return = 
    (SELECT
		A.[UserName], A.[Role]
	FROM
		dbo.[Users] A
	WHERE
		(@ActiveOnly = 0 OR A.[Status] = 1)
	ORDER BY
		A.[UserName]
	FOR JSON PATH);

	SELECT @return;
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Narendra Kumaran Kadhirvelu
-- Create date: 01-18-2022
-- Description:	Gets the Workflow Stages
-- =============================================
CREATE    PROCEDURE [GetWorkflow] 
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @return varchar(max);

	SELECT @return = 
    (SELECT
		A.[Id] [ID], A.[Step], A.[Next], A.[IsLast], A.[Role] [RoleIDs], A.[Schedule], A.[AnyStage], '' [NextFull], '' [RoleFull]
	FROM
		dbo.[WorkflowActivity] A
	FOR JSON PATH);

	SELECT @return;

END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE   procedure [GetZipCityState]
	@Zip varchar(10) = '00210'
as
BEGIN
	SET NOCOUNT ON;

	SELECT 
		A.[City], B.[ID] [StateID]
	FROM
		dbo.[ZipCodes] A INNER JOIN dbo.[State] B ON A.[StateAbbreviation] = B.[Code]
	WHERE
		A.[ZipCode] = @Zip
	FOR JSON PATH, WITHOUT_ARRAY_WRAPPER;
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [GetZipCodes]
AS
BEGIN
	SET NOCOUNT ON;
	DECLARE @compressedResult VARBINARY(MAX);
	DECLARE @return varchar(max);

	SELECT @return = 
    (SELECT
		A.ZipCode, A.City, ISNULL(B.[State], '') [State], ISNULL(B.[Id], 0) [StateID]
	FROM
		dbo.[ZipCodes] A INNER JOIN dbo.[State] B ON A.[StateAbbreviation] = B.[Code]
	FOR JSON PATH);

	SELECT COMPRESS(@return);
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE   PROCEDURE [InsertRequisitionView]
AS
BEGIN

	DECLARE @RecSubmitCand bit=0, @ReqChangeStatus int=0, @AdminRequisitions bit
	DECLARE @ReqLow varchar(7), @ReqHigh varchar(7), @ReqMed varchar(7);--@PageCount = A.[PageSize],

	SELECT
		@ReqChangeStatus = A.[ReqStatusChange], @AdminRequisitions = A.[AdminRequisitions],
		@ReqLow = A.[ReqPriorityLow], @ReqHigh = A.[ReqPriorityHigh], @ReqMed = A.[ReqPriorityNormal],
		@RecSubmitCand = A.[AllRecruitersSubmitCandidate]
	FROM
		dbo.[Preferences] A;

	INSERT INTO dbo.RequisitionView
	SELECT A.[Id], A.[Code], A.[PosTitle], B.[CompanyName], A.[JobOption], C.JobOptions, A.[Status], D.Status, A.[UpdatedDate], A.[UpdatedBy], A.[CreatedBy], A.[Due], D.[Icon], A.[IsHot]
		  ,CAST(CASE D.[SubmitCandidate] 
				WHEN 1 THEN CASE 
								WHEN @RecSubmitCand = 0 AND E.UserName IN (SELECT [s] FROM dbo.[BigSplit](',', A.[AssignedRecruiter])) THEN 1 
								WHEN @RecSubmitCand = 1 THEN 1
								ELSE 0
							END
				ELSE 0 
				END as bit)[SubmitCandidate]
      ,CASE @ReqChangeStatus
							WHEN 0 THEN CAST(CASE WHEN A.[UpdatedBy] = E.UserName OR (@AdminRequisitions = 1 AND E.[Role] = 2) THEN 1 ELSE 0 END as bit)
							WHEN 1 THEN CAST(1 as bit)
						END [CanUpdate]
      ,CAST(CASE F.[EditRequisitions]
					WHEN 1 THEN CASE WHEN @ReqChangeStatus = 0 AND (A.[UpdatedBy] = E.UserName OR (@AdminRequisitions = 1 AND E.[Role] = 2)) THEN 1 
					WHEN @ReqChangeStatus = 1 THEN 1 ELSE 0 END
					WHEN 0 THEN 0
				END as bit) [ChangeStatus]
      ,CASE A.[IsHot]
					WHEN 0 THEN @ReqLow
					WHEN 2 THEN @ReqHigh
					ELSE @ReqMed
				END [PriorityColor], A.[AssignedRecruiter], E.[Role]
  FROM [dbo].[Requisitions] A INNER JOIN dbo.Companies B ON A.CompanyId=B.ID
  INNER JOIN dbo.JobOptions C ON A.JobOption=C.JobCode
  INNER JOIN dbo.StatusCode D ON A.Status=D.StatusCode AND D.AppliesTo='REQ'
  INNER JOIN dbo.Users E On A.UpdatedBy=E.UserName
  INNER JOIN dbo.Roles F ON E.Role=F.ID

END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [RefreshDashboardMaterializedViews]
AS
BEGIN
    SET NOCOUNT ON;
    SET ARITHABORT ON;
    SET ANSI_NULLS ON;
    SET QUOTED_IDENTIFIER ON;
    
    DECLARE @RefreshStart DATETIME = GETDATE();
    DECLARE @Today DATE = CAST(GETDATE() AS DATE);
    DECLARE @YearStart DATE = DATEFROMPARTS(YEAR(@Today), 1, 1);
    
    -- Common date calculations
    DECLARE @StartDate7D DATE = DATEADD(DAY, -6, @Today);
    DECLARE @StartDateMTD DATE = DATEADD(DAY, 1, EOMONTH(@Today, -1));
    DECLARE @StartDateQTD DATE = DATEADD(QUARTER, DATEDIFF(QUARTER, 0, @Today), 0);
    DECLARE @StartDateHYTD DATE = CASE 
        WHEN MONTH(@Today) <= 6 THEN DATEFROMPARTS(YEAR(@Today), 1, 1)
        ELSE DATEFROMPARTS(YEAR(@Today), 7, 1)
    END;

    BEGIN TRY
        BEGIN TRANSACTION;
        
        -- 1. REFRESH ActiveUsersView
        DELETE FROM [dbo].[ActiveUsersView];
        
        INSERT INTO [dbo].[ActiveUsersView] 
        ([CreatedBy], [FullName], [Role], [Status], [LastRequisitionDate], [RequisitionCount], [RefreshDate])
        SELECT 
            U.UserName,
            U.FirstName + ' ' + U.LastName,
            U.Role,
            U.Status,
            (SELECT MAX(CAST(R.CreatedDate AS DATE)) 
             FROM Requisitions R 
             WHERE R.CreatedBy = U.UserName),
            (SELECT COUNT(*) 
             FROM Requisitions R 
             WHERE R.CreatedBy = U.UserName 
             AND CAST(R.CreatedDate AS DATE) >= @YearStart),
            @RefreshStart
        FROM Users U
        WHERE U.Status = 1
        AND EXISTS (SELECT 1 FROM Requisitions R WHERE R.CreatedBy = U.UserName);
        
        -- 2. REFRESH SubmissionMetricsView  
        DELETE FROM [dbo].[SubmissionMetricsView];
        
        -- Insert metrics for each period and active user
        WITH ActiveUsers AS (
            SELECT DISTINCT U.UserName as CreatedBy
            FROM Users U
            WHERE U.Status = 1
            AND EXISTS (SELECT 1 FROM Submissions S WHERE S.CreatedBy = U.UserName)
        ),
        PeriodDates AS (
            SELECT '7D' as Period, @StartDate7D as StartDate, @Today as EndDate
            UNION ALL
            SELECT 'MTD', @StartDateMTD, @Today
            UNION ALL  
            SELECT 'QTD', @StartDateQTD, @Today
            UNION ALL
            SELECT 'HYTD', @StartDateHYTD, @Today
            UNION ALL
            SELECT 'YTD', @YearStart, @Today
        )
        INSERT INTO [dbo].[SubmissionMetricsView]
        ([CreatedBy], [MetricDate], [Period], [INT_Count], [OEX_Count], [HIR_Count], 
         [Total_Submissions], [OEX_HIR_Ratio], [Requisitions_Created], [Active_Requisitions_Updated], [RefreshDate])
        SELECT 
            AU.CreatedBy,
            @Today,
            PD.Period,
            -- Submission counts by status
            ISNULL(SUM(CASE WHEN S.Status = 'INT' AND CAST(S.CreatedDate AS DATE) BETWEEN PD.StartDate AND PD.EndDate THEN 1 ELSE 0 END), 0),
            ISNULL(SUM(CASE WHEN S.Status = 'OEX' AND CAST(S.CreatedDate AS DATE) BETWEEN PD.StartDate AND PD.EndDate THEN 1 ELSE 0 END), 0),
            ISNULL(SUM(CASE WHEN S.Status = 'HIR' AND CAST(S.CreatedDate AS DATE) BETWEEN PD.StartDate AND PD.EndDate THEN 1 ELSE 0 END), 0),
            -- Total submissions
            ISNULL(SUM(CASE WHEN CAST(S.CreatedDate AS DATE) BETWEEN PD.StartDate AND PD.EndDate THEN 1 ELSE 0 END), 0),
            -- OEX to HIR ratio
            CASE 
                WHEN SUM(CASE WHEN S.Status = 'HIR' AND CAST(S.CreatedDate AS DATE) BETWEEN PD.StartDate AND PD.EndDate THEN 1 ELSE 0 END) > 0
                THEN CAST(SUM(CASE WHEN S.Status = 'OEX' AND CAST(S.CreatedDate AS DATE) BETWEEN PD.StartDate AND PD.EndDate THEN 1 ELSE 0 END) AS DECIMAL(5,2)) / 
                     SUM(CASE WHEN S.Status = 'HIR' AND CAST(S.CreatedDate AS DATE) BETWEEN PD.StartDate AND PD.EndDate THEN 1 ELSE 0 END)
                ELSE 0 
            END,
            -- Requisitions created
            (SELECT COUNT(*) FROM Requisitions R 
             WHERE R.CreatedBy = AU.CreatedBy 
             AND CAST(R.CreatedDate AS DATE) BETWEEN PD.StartDate AND PD.EndDate),
            -- Active requisitions updated  
            (SELECT COUNT(DISTINCT R.Id) FROM Requisitions R 
             WHERE R.CreatedBy = AU.CreatedBy 
             AND CAST(R.UpdatedDate AS DATE) BETWEEN PD.StartDate AND PD.EndDate
             AND R.Status IN ('NEW', 'OPN', 'PAR')),
            @RefreshStart
        FROM ActiveUsers AU
        CROSS JOIN PeriodDates PD
        LEFT JOIN Submissions S ON S.CreatedBy = AU.CreatedBy
        GROUP BY AU.CreatedBy, PD.Period, PD.StartDate, PD.EndDate;
        
        -- 3. REFRESH TimingAnalyticsView
        DELETE FROM [dbo].[TimingAnalyticsView];
        
        --WITH SubmissionTimings AS (
        --    SELECT 
        --        S.CreatedBy,
        --        S.RequisitionId,
        --        R.Code as RequisitionCode,
        --        C.CompanyName,
        --        R.PosTitle as Title,
        --        S.CandidateId,
        --        CAST(R.CreatedDate AS DATE) as RequisitionCreatedDate,
        --        CAST(R.UpdatedDate AS DATE) as RequisitionUpdatedDate,
        --        R.Status as RequisitionStatus,
        --        MIN(CASE WHEN S.Status IN ('INT', 'OEX', 'HIR') THEN CAST(S.CreatedDate AS DATE) END) as FirstSubmissionDate,
        --        MAX(CASE WHEN S.Status = 'HIR' THEN CAST(S.CreatedDate AS DATE) END) as HireDate,
        --        -- Calculate days in each stage (simplified - you may need to adjust based on your actual status workflow)
        --        SUM(CASE WHEN S.Status = 'PEN' THEN DATEDIFF(DAY, S.CreatedDate, ISNULL(LEAD(S.CreatedDate) OVER (PARTITION BY S.CandidateId, S.RequisitionId ORDER BY S.CreatedDate), GETDATE())) ELSE 0 END) as PEN_Days,
        --        SUM(CASE WHEN S.Status = 'REJ' THEN DATEDIFF(DAY, S.CreatedDate, ISNULL(LEAD(S.CreatedDate) OVER (PARTITION BY S.CandidateId, S.RequisitionId ORDER BY S.CreatedDate), GETDATE())) ELSE 0 END) as REJ_Days,
        --        SUM(CASE WHEN S.Status = 'HLD' THEN DATEDIFF(DAY, S.CreatedDate, ISNULL(LEAD(S.CreatedDate) OVER (PARTITION BY S.CandidateId, S.RequisitionId ORDER BY S.CreatedDate), GETDATE())) ELSE 0 END) as HLD_Days,
        --        SUM(CASE WHEN S.Status = 'PHN' THEN DATEDIFF(DAY, S.CreatedDate, ISNULL(LEAD(S.CreatedDate) OVER (PARTITION BY S.CandidateId, S.RequisitionId ORDER BY S.CreatedDate), GETDATE())) ELSE 0 END) as PHN_Days,
        --        SUM(CASE WHEN S.Status = 'URW' THEN DATEDIFF(DAY, S.CreatedDate, ISNULL(LEAD(S.CreatedDate) OVER (PARTITION BY S.CandidateId, S.RequisitionId ORDER BY S.CreatedDate), GETDATE())) ELSE 0 END) as URW_Days,
        --        SUM(CASE WHEN S.Status = 'INT' THEN DATEDIFF(DAY, S.CreatedDate, ISNULL(LEAD(S.CreatedDate) OVER (PARTITION BY S.CandidateId, S.RequisitionId ORDER BY S.CreatedDate), GETDATE())) ELSE 0 END) as INT_Days,
        --        SUM(CASE WHEN S.Status = 'RHM' THEN DATEDIFF(DAY, S.CreatedDate, ISNULL(LEAD(S.CreatedDate) OVER (PARTITION BY S.CandidateId, S.RequisitionId ORDER BY S.CreatedDate), GETDATE())) ELSE 0 END) as RHM_Days,
        --        SUM(CASE WHEN S.Status = 'DEC' THEN DATEDIFF(DAY, S.CreatedDate, ISNULL(LEAD(S.CreatedDate) OVER (PARTITION BY S.CandidateId, S.RequisitionId ORDER BY S.CreatedDate), GETDATE())) ELSE 0 END) as DEC_Days,
        --        SUM(CASE WHEN S.Status = 'NOA' THEN DATEDIFF(DAY, S.CreatedDate, ISNULL(LEAD(S.CreatedDate) OVER (PARTITION BY S.CandidateId, S.RequisitionId ORDER BY S.CreatedDate), GETDATE())) ELSE 0 END) as NOA_Days,
        --        SUM(CASE WHEN S.Status = 'OEX' THEN DATEDIFF(DAY, S.CreatedDate, ISNULL(LEAD(S.CreatedDate) OVER (PARTITION BY S.CandidateId, S.RequisitionId ORDER BY S.CreatedDate), GETDATE())) ELSE 0 END) as OEX_Days,
        --        SUM(CASE WHEN S.Status = 'ODC' THEN DATEDIFF(DAY, S.CreatedDate, ISNULL(LEAD(S.CreatedDate) OVER (PARTITION BY S.CandidateId, S.RequisitionId ORDER BY S.CreatedDate), GETDATE())) ELSE 0 END) as ODC_Days,
        --        SUM(CASE WHEN S.Status = 'HIR' THEN DATEDIFF(DAY, S.CreatedDate, ISNULL(LEAD(S.CreatedDate) OVER (PARTITION BY S.CandidateId, S.RequisitionId ORDER BY S.CreatedDate), GETDATE())) ELSE 0 END) as HIR_Days,
        --        SUM(CASE WHEN S.Status = 'WDR' THEN DATEDIFF(DAY, S.CreatedDate, ISNULL(LEAD(S.CreatedDate) OVER (PARTITION BY S.CandidateId, S.RequisitionId ORDER BY S.CreatedDate), GETDATE())) ELSE 0 END) as WDR_Days
        --    FROM Submissions S
        --    INNER JOIN Requisitions R ON S.RequisitionId = R.Id
        --    INNER JOIN Companies C ON R.CompanyId = C.Id
        --    WHERE S.CreatedDate >= DATEADD(YEAR, -1, @Today) -- Only last year's data for performance
        --    GROUP BY S.CreatedBy, S.RequisitionId, R.Code, C.CompanyName, R.PosTitle, S.CandidateId, 
        --             R.CreatedDate, R.UpdatedDate, S.CreatedDate, R.Status
        --)
        -- Replace the problematic timing calculations section with this approach:

WITH SubmissionSequence AS (
    SELECT 
        S.CreatedBy,
        S.RequisitionId,
        R.Code as RequisitionCode,
        C.CompanyName,
        R.PosTitle as Title,
        S.CandidateId,
        S.Status,
        S.CreatedDate,
        CAST(R.CreatedDate AS DATE) as RequisitionCreatedDate,
        CAST(R.UpdatedDate AS DATE) as RequisitionUpdatedDate,
        R.Status as RequisitionStatus,
        -- Calculate next status date for each submission
        LEAD(S.CreatedDate) OVER (PARTITION BY S.CandidateId, S.RequisitionId ORDER BY S.CreatedDate) as NextStatusDate
    FROM Submissions S
    INNER JOIN Requisitions R ON S.RequisitionId = R.Id
    INNER JOIN Companies C ON R.CompanyId = C.Id
    WHERE S.CreatedDate >= DATEADD(YEAR, -1, @Today)
),
StageTimings AS (
    SELECT 
        CreatedBy, RequisitionId, RequisitionCode, CompanyName, Title, CandidateId,
        RequisitionCreatedDate, RequisitionUpdatedDate, RequisitionStatus,
        MIN(CASE WHEN Status IN ('INT', 'OEX', 'HIR') THEN CAST(CreatedDate AS DATE) END) as FirstSubmissionDate,
        MAX(CASE WHEN Status = 'HIR' THEN CAST(CreatedDate AS DATE) END) as HireDate,
        -- Calculate days spent in each stage
        SUM(CASE WHEN Status = 'PEN' THEN DATEDIFF(DAY, CreatedDate, ISNULL(NextStatusDate, GETDATE())) ELSE 0 END) as PEN_Days,
        SUM(CASE WHEN Status = 'REJ' THEN DATEDIFF(DAY, CreatedDate, ISNULL(NextStatusDate, GETDATE())) ELSE 0 END) as REJ_Days,
        SUM(CASE WHEN Status = 'HLD' THEN DATEDIFF(DAY, CreatedDate, ISNULL(NextStatusDate, GETDATE())) ELSE 0 END) as HLD_Days,
        SUM(CASE WHEN Status = 'PHN' THEN DATEDIFF(DAY, CreatedDate, ISNULL(NextStatusDate, GETDATE())) ELSE 0 END) as PHN_Days,
        SUM(CASE WHEN Status = 'URW' THEN DATEDIFF(DAY, CreatedDate, ISNULL(NextStatusDate, GETDATE())) ELSE 0 END) as URW_Days,
        SUM(CASE WHEN Status = 'INT' THEN DATEDIFF(DAY, CreatedDate, ISNULL(NextStatusDate, GETDATE())) ELSE 0 END) as INT_Days,
        SUM(CASE WHEN Status = 'RHM' THEN DATEDIFF(DAY, CreatedDate, ISNULL(NextStatusDate, GETDATE())) ELSE 0 END) as RHM_Days,
        SUM(CASE WHEN Status = 'DEC' THEN DATEDIFF(DAY, CreatedDate, ISNULL(NextStatusDate, GETDATE())) ELSE 0 END) as DEC_Days,
        SUM(CASE WHEN Status = 'NOA' THEN DATEDIFF(DAY, CreatedDate, ISNULL(NextStatusDate, GETDATE())) ELSE 0 END) as NOA_Days,
        SUM(CASE WHEN Status = 'OEX' THEN DATEDIFF(DAY, CreatedDate, ISNULL(NextStatusDate, GETDATE())) ELSE 0 END) as OEX_Days,
        SUM(CASE WHEN Status = 'ODC' THEN DATEDIFF(DAY, CreatedDate, ISNULL(NextStatusDate, GETDATE())) ELSE 0 END) as ODC_Days,
        SUM(CASE WHEN Status = 'HIR' THEN DATEDIFF(DAY, CreatedDate, ISNULL(NextStatusDate, GETDATE())) ELSE 0 END) as HIR_Days,
        SUM(CASE WHEN Status = 'WDR' THEN DATEDIFF(DAY, CreatedDate, ISNULL(NextStatusDate, GETDATE())) ELSE 0 END) as WDR_Days
    FROM SubmissionSequence
    GROUP BY CreatedBy, RequisitionId, RequisitionCode, CompanyName, Title, CandidateId,
             RequisitionCreatedDate, RequisitionUpdatedDate, RequisitionStatus
)
INSERT INTO [dbo].[TimingAnalyticsView]
([CreatedBy], [RequisitionId], [RequisitionCode], [CompanyName], [Title], [CandidateId],
 [RequisitionCreatedDate], [RequisitionUpdatedDate], [RequisitionStatus], 
 [TimeToFill_Days], [TimeToHire_Days], [FirstSubmissionDate], [HireDate],
 [PEN_Days], [REJ_Days], [HLD_Days], [PHN_Days], [URW_Days], [INT_Days], [RHM_Days], 
 [DEC_Days], [NOA_Days], [OEX_Days], [ODC_Days], [HIR_Days], [WDR_Days], [RefreshDate])
SELECT 
    CreatedBy, RequisitionId, RequisitionCode, CompanyName, Title, CandidateId,
    RequisitionCreatedDate, RequisitionUpdatedDate, RequisitionStatus,
    -- Time to Fill (from req creation to hire)
    CASE WHEN HireDate IS NOT NULL THEN DATEDIFF(DAY, RequisitionCreatedDate, HireDate) END,
    -- Time to Hire (from first submission to hire)  
    CASE WHEN HireDate IS NOT NULL AND FirstSubmissionDate IS NOT NULL THEN DATEDIFF(DAY, FirstSubmissionDate, HireDate) END,
    FirstSubmissionDate, HireDate,
    PEN_Days, REJ_Days, HLD_Days, PHN_Days, URW_Days, INT_Days, RHM_Days,
    DEC_Days, NOA_Days, OEX_Days, ODC_Days, HIR_Days, WDR_Days,
    @RefreshStart
FROM StageTimings;
        --INSERT INTO [dbo].[TimingAnalyticsView]
        --([CreatedBy], [RequisitionId], [RequisitionCode], [CompanyName], [Title], [CandidateId],
        -- [RequisitionCreatedDate], [RequisitionUpdatedDate], [RequisitionStatus], 
        -- [TimeToFill_Days], [TimeToHire_Days], [FirstSubmissionDate], [HireDate],
        -- [PEN_Days], [REJ_Days], [HLD_Days], [PHN_Days], [URW_Days], [INT_Days], [RHM_Days], 
        -- [DEC_Days], [NOA_Days], [OEX_Days], [ODC_Days], [HIR_Days], [WDR_Days], [RefreshDate])
        --SELECT 
        --    CreatedBy, RequisitionId, RequisitionCode, CompanyName, Title, CandidateId,
        --    RequisitionCreatedDate, RequisitionUpdatedDate, RequisitionStatus,
        --    -- Time to Fill (from req creation to hire)
        --    CASE WHEN HireDate IS NOT NULL THEN DATEDIFF(DAY, RequisitionCreatedDate, HireDate) END,
        --    -- Time to Hire (from first submission to hire)  
        --    CASE WHEN HireDate IS NOT NULL AND FirstSubmissionDate IS NOT NULL THEN DATEDIFF(DAY, FirstSubmissionDate, HireDate) END,
        --    FirstSubmissionDate, HireDate,
        --    PEN_Days, REJ_Days, HLD_Days, PHN_Days, URW_Days, INT_Days, RHM_Days,
        --    DEC_Days, NOA_Days, OEX_Days, ODC_Days, HIR_Days, WDR_Days,
        --    @RefreshStart
        --FROM SubmissionTimings;
        
        COMMIT TRANSACTION;
        
        -- Log successful refresh
        PRINT 'Dashboard materialized views refreshed successfully at ' + CONVERT(VARCHAR(23), @RefreshStart, 121);
        
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
        DECLARE @ErrorState INT = ERROR_STATE();
        
        PRINT 'Error refreshing dashboard views: ' + @ErrorMessage;
        RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
    END CATCH
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [RefreshPlacementReportView]
AS
BEGIN
    SET NOCOUNT ON;
    SET ARITHABORT ON;
    
    DECLARE @RefreshStart DATETIME2 = SYSDATETIME();
    DECLARE @Today DATE = GETDATE();
    
    -- Simple 3-month back date calculation (no complex logic)
    DECLARE @StartDate3Month DATE = DATEADD(MONTH, -3, @Today);
    
    BEGIN TRANSACTION;

    BEGIN TRY
        -- Clear existing data (nuclear refresh approach)
        DELETE FROM [dbo].[PlacementReportView];
        
        -- =====================================================================================
        -- PLACEMENT REPORT CALCULATION - REPLICATE CURRENT SET 3 LOGIC
        -- =====================================================================================
        
        WITH HiredCandidates AS (
            -- Get the latest HIR record for each Req+Cand combination (across all active users)
            SELECT 
                R.CreatedBy, S.RequisitionId, S.CandidateId, MAX(S.CreatedDate) as DateHired,
                ROW_NUMBER() OVER (PARTITION BY S.RequisitionId, S.CandidateId ORDER BY MAX(S.CreatedDate) DESC) as RN
            FROM 
                Submissions S INNER JOIN Requisitions R ON R.Id = S.RequisitionId
                INNER JOIN dbo.Users U ON R.CreatedBy = U.UserName AND U.Status = 1  -- Only active users
            WHERE 
                S.Status = 'HIR' AND R.CreatedBy IS NOT NULL
            GROUP BY 
                R.CreatedBy, S.RequisitionId, S.CandidateId
            HAVING 
                MAX(S.CreatedDate) >= @StartDate3Month AND MAX(S.CreatedDate) <= DATEADD(DAY, 1, @Today)
        ),
        PlacementData AS (
            -- Combine all placement data for final insert
            SELECT 
                HC.CreatedBy, HC.RequisitionId, HC.CandidateId, C.CompanyName, R.Code as RequisitionNumber, R.Positions as NumPosition,
                R.PosTitle as RequisitionTitle, LTRIM(RTRIM(ISNULL(CAND.FirstName, '') + ' ' + ISNULL(CAND.LastName, ''))) as CandidateName,
                CAST(HC.DateHired AS DATE) as DateHired, CAST(0.00 AS DECIMAL(9,2)) as SalaryOffered, CAST(@Today AS DATE) as StartDate, CAST(@Today AS DATE) as DateInvoiced,
                CAST(@Today AS DATE) as DatePaid, CAST(0.00 AS DECIMAL(9,2)) as PlacementFee, CAST(0.00 AS DECIMAL(5,2)) as CommissionPercent,
                CAST(0.00 AS DECIMAL(9,2)) as CommissionEarned, @RefreshStart as RefreshDate
            FROM 
                HiredCandidates HC INNER JOIN Requisitions R ON R.Id = HC.RequisitionId
                INNER JOIN Companies C ON C.ID = R.CompanyId
                INNER JOIN Candidate CAND ON CAND.ID = HC.CandidateId
            WHERE 
                HC.RN = 1  -- Only the latest hire record
        )
        
        -- Insert all placement data
        INSERT INTO [dbo].[PlacementReportView] 
            ([CreatedBy], [RequisitionId], [CandidateId], [CompanyName], [RequisitionNumber], [NumPosition], [RequisitionTitle], [CandidateName], [DateHired], 
            [SalaryOffered], [StartDate], [DateInvoiced], [DatePaid], [PlacementFee], [CommissionPercent], [CommissionEarned], [RefreshDate])
        SELECT 
            CreatedBy, RequisitionId, CandidateId, CompanyName, RequisitionNumber, NumPosition, RequisitionTitle, CandidateName, DateHired, SalaryOffered, StartDate,
            DateInvoiced, DatePaid, PlacementFee, CommissionPercent, CommissionEarned, RefreshDate
        FROM 
            PlacementData;
        
        COMMIT TRANSACTION;

        -- Log successful refresh
        DECLARE @RowCount INT = @@ROWCOUNT;
        DECLARE @RefreshEnd DATETIME2 = SYSDATETIME();
        DECLARE @Duration INT = DATEDIFF(MILLISECOND, @RefreshStart, @RefreshEnd);
        
        PRINT 'PlacementReportView refresh completed successfully.';
        PRINT 'Date range: ' + CAST(@StartDate3Month AS VARCHAR(20)) + ' to ' + CAST(@Today AS VARCHAR(20));
        PRINT 'Hired candidates processed: ' + CAST(@RowCount AS VARCHAR(10));
        PRINT 'Duration: ' + CAST(@Duration AS VARCHAR(10)) + ' ms';
        
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        -- Error handling
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @ErrorLine INT = ERROR_LINE();
        
        PRINT 'Error in PlacementReportView refresh:';
        PRINT 'Line: ' + CAST(@ErrorLine AS VARCHAR(10));
        PRINT 'Message: ' + @ErrorMessage;
        
        -- Re-throw the error
        THROW;
    END CATCH
END;

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [RefreshRecentActivityView]
AS
BEGIN
    SET NOCOUNT ON;
    SET ARITHABORT ON;
    
    DECLARE @RefreshStart DATETIME2 = SYSDATETIME();
    DECLARE @Today DATE = GETDATE();
    
    -- Simple month-1 date calculation (no complex logic)
    DECLARE @StartDate1Month DATE = DATEADD(MONTH, -1, @Today);
    
    BEGIN TRANSACTION;

    BEGIN TRY
        -- Clear existing data (nuclear refresh approach)
        DELETE FROM [dbo].[RecentActivityView];
        
        -- =====================================================================================
        -- RECENT ACTIVITY CALCULATION - REPLICATE CURRENT SET 2 LOGIC
        -- =====================================================================================
        
        WITH CompanySubmissionCounts AS (
            -- Get submission counts by company for sorting (across all active users)
            SELECT 
                C.CompanyName, COUNT(DISTINCT CAST(S.RequisitionId AS VARCHAR(10)) + '-' + CAST(S.CandidateId AS VARCHAR(10))) as SubmissionCount
            FROM 
                Submissions S INNER JOIN Requisitions R ON R.Id = S.RequisitionId
                INNER JOIN Companies C ON C.ID = R.CompanyId
                INNER JOIN dbo.Users U ON R.CreatedBy = U.UserName AND U.Status = 1  -- Only active users
            WHERE 
                S.CreatedDate >= @StartDate1Month  AND S.CreatedDate <= DATEADD(DAY, 1, @Today) AND R.CreatedBy IS NOT NULL
            GROUP BY 
                C.CompanyName
        ),
        SubmissionSummary AS (
            -- Get first and last activity for each Req+Cand combination (for all active users)
            SELECT 
                R.CreatedBy, S.RequisitionId, S.CandidateId, MIN(S.CreatedDate) as DateFirstSubmitted, MAX(S.CreatedDate) as LastActivityDate
            FROM 
                Submissions S INNER JOIN Requisitions R ON R.Id = S.RequisitionId
                INNER JOIN dbo.Users U ON R.CreatedBy = U.UserName AND U.Status = 1  -- Only active users
            WHERE 
                R.CreatedBy IS NOT NULL
            GROUP BY 
                R.CreatedBy, S.RequisitionId, S.CandidateId
            HAVING 
                MIN(S.CreatedDate) >= @StartDate1Month AND MIN(S.CreatedDate) <= DATEADD(DAY, 1, @Today)
        ),
        LastActivity AS (
            -- Get the status and notes from the most recent submission
            SELECT 
                SS.CreatedBy, SS.RequisitionId, SS.CandidateId, S.Status, S.Notes, S.CreatedDate,
                ROW_NUMBER() OVER (PARTITION BY SS.RequisitionId, SS.CandidateId ORDER BY S.CreatedDate DESC) as RN
            FROM 
                SubmissionSummary SS INNER JOIN Submissions S ON SS.RequisitionId = S.RequisitionId  AND SS.CandidateId = S.CandidateId
        ),
        RecentActivityData AS (
            -- Combine all data for final insert
            SELECT 
                SS.CreatedBy, SS.RequisitionId, SS.CandidateId, C.CompanyName, R.Code as RequisitionCode, R.PosTitle as RequisitionTitle, R.Positions,
                LTRIM(RTRIM(ISNULL(CAND.FirstName, '') + ' ' + ISNULL(CAND.LastName, ''))) as CandidateName, LA.Status as CurrentStatus, SS.DateFirstSubmitted,
                SS.LastActivityDate, ISNULL(LA.Notes, '') as ActivityNotes, CSC.SubmissionCount, @RefreshStart as RefreshDate
            FROM 
                SubmissionSummary SS INNER JOIN LastActivity LA ON LA.CreatedBy = SS.CreatedBy  AND LA.RequisitionId = SS.RequisitionId 
                AND LA.CandidateId = SS.CandidateId AND LA.RN = 1
                INNER JOIN Requisitions R ON R.Id = SS.RequisitionId
                INNER JOIN Companies C ON C.ID = R.CompanyId
                INNER JOIN Candidate CAND ON SS.CandidateId = CAND.ID
                INNER JOIN CompanySubmissionCounts CSC ON C.CompanyName = CSC.CompanyName
        )
        
        -- Insert all recent activity data
        INSERT INTO [dbo].[RecentActivityView] 
            ([CreatedBy], [RequisitionId], [CandidateId], [CompanyName], [RequisitionCode], [RequisitionTitle], [Positions], [CandidateName], [CurrentStatus], 
            [DateFirstSubmitted], [LastActivityDate], [ActivityNotes], [SubmissionCount], [RefreshDate])
        SELECT 
            CreatedBy, RequisitionId, CandidateId, CompanyName, RequisitionCode, RequisitionTitle, Positions, CandidateName, CurrentStatus, DateFirstSubmitted,
            LastActivityDate, ActivityNotes, SubmissionCount, RefreshDate
        FROM 
            RecentActivityData;
        
        COMMIT TRANSACTION;

        -- Log successful refresh
        DECLARE @RowCount INT = @@ROWCOUNT;
        DECLARE @RefreshEnd DATETIME2 = SYSDATETIME();
        DECLARE @Duration INT = DATEDIFF(MILLISECOND, @RefreshStart, @RefreshEnd);
        
        PRINT 'RecentActivityView refresh completed successfully.';
        PRINT 'Date range: ' + CAST(@StartDate1Month AS VARCHAR(20)) + ' to ' + CAST(@Today AS VARCHAR(20));
        PRINT 'Rows processed: ' + CAST(@RowCount AS VARCHAR(10));
        PRINT 'Duration: ' + CAST(@Duration AS VARCHAR(10)) + ' ms';
        
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        -- Error handling
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @ErrorLine INT = ERROR_LINE();
        
        PRINT 'Error in RecentActivityView refresh:';
        PRINT 'Line: ' + CAST(@ErrorLine AS VARCHAR(10));
        PRINT 'Message: ' + @ErrorMessage;
        
        -- Re-throw the error
        THROW;
    END CATCH
END;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [RefreshTimingAnalyticsView]
AS
BEGIN
    SET NOCOUNT ON;
    SET ARITHABORT ON;
    
    DECLARE @RefreshStart DATETIME2 = SYSDATETIME();
    DECLARE @Today DATE = '2025-06-30'-- GETDATE();
    
    -- Simple year-1 date calculation
    DECLARE @StartDate365 DATE = DATEADD(YEAR, -1, @Today);
    
    BEGIN TRANSACTIOn;

    BEGIN TRY
        -- Clear existing data (nuclear refresh approach)
        DELETE FROM [dbo].[TimingAnalyticsView];
        
        -- =====================================================================================
        -- CONTEXT 1: ACCOUNTS MANAGER PERSPECTIVE (R.CreatedBy - Requisition Owner)
        -- =====================================================================================
        
        WITH AM_RequisitionBase AS (
            -- Get requisitions created by AMs/Full Desk users in last year
            SELECT 
                R.Id as RequisitionId, R.Code as RequisitionCode, ISNULL(C.CompanyName, 'Unknown Company') as CompanyName, R.PosTitle as Title,
                R.CreatedDate as RequisitionCreatedDate, R.UpdatedDate as RequisitionUpdatedDate, R.Status as RequisitionStatus, R.CreatedBy,
                CASE 
                    WHEN R.Status = 'FUL' THEN DATEDIFF(DAY, R.CreatedDate, R.UpdatedDate)
                    ELSE NULL 
                END as TimeToFill
            FROM 
                Requisitions R LEFT JOIN Companies C ON R.CompanyId = C.ID
                INNER JOIN dbo.Users U ON R.CreatedBy = U.UserName 
            WHERE 
                U.Status = 1  AND U.Role IN (5, 6) AND CAST(R.CreatedDate AS DATE) >= @StartDate365
                AND R.CreatedBy IS NOT NULL
        ),
        AM_FirstSubmissions AS (
            -- Get first submission date for each candidate+requisition combo
            SELECT 
                S.RequisitionId, S.CandidateId, MIN(S.CreatedDate) as FirstSubmissionDate
            FROM 
                Submissions S INNER JOIN AM_RequisitionBase ARB ON S.RequisitionId = ARB.RequisitionId
            GROUP BY 
                S.RequisitionId, S.CandidateId
        ),
        AM_TimeToHireCalc AS (
            -- Calculate Time to Hire for hired candidates
            SELECT 
                AFS.RequisitionId, AFS.CandidateId, AFS.FirstSubmissionDate, MAX(CASE WHEN S.Status = 'HIR' THEN S.CreatedDate END) as HireDate,
                MAX(CASE WHEN S.Status = 'HIR' THEN DATEDIFF(DAY, AFS.FirstSubmissionDate, S.CreatedDate) END) as TimeToHire
            FROM 
                AM_FirstSubmissions AFS INNER JOIN Submissions S ON AFS.RequisitionId = S.RequisitionId AND AFS.CandidateId = S.CandidateId
            GROUP BY 
                AFS.RequisitionId, AFS.CandidateId, AFS.FirstSubmissionDate
        ),
        AM_SubmissionHistory AS (
            -- Get all submission history with next submission date for gap calculation
            SELECT 
                S.RequisitionId, S.CandidateId, S.Status, S.CreatedDate,
                LEAD(S.CreatedDate) OVER (PARTITION BY S.RequisitionId, S.CandidateId ORDER BY S.CreatedDate) as NextSubmissionDate,
                CASE WHEN ROW_NUMBER() OVER (PARTITION BY S.RequisitionId, S.CandidateId ORDER BY S.CreatedDate DESC) = 1 THEN 1 ELSE 0 END as IsCurrentStatus
            FROM 
                Submissions S INNER JOIN AM_FirstSubmissions AFS ON S.RequisitionId = AFS.RequisitionId AND S.CandidateId = AFS.CandidateId
        ),
        AM_StageTimeCalculations AS (
            -- Calculate CUMULATIVE days spent in each stage (sum all periods per status)
            SELECT 
                AFS.RequisitionId, AFS.CandidateId,
                -- PEN_Days: Sum all time spent in PEN status
                ISNULL(SUM(CASE 
                    WHEN ASH.Status = 'PEN' THEN 
                        CASE 
                            WHEN ASH.NextSubmissionDate IS NOT NULL THEN DATEDIFF(DAY, ASH.CreatedDate, ASH.NextSubmissionDate)
                            WHEN ASH.IsCurrentStatus = 1 THEN DATEDIFF(DAY, ASH.CreatedDate, @RefreshStart)
                            ELSE 0
                        END
                    ELSE 0
                END), 0) as PEN_Days,
                -- REJ_Days: Sum all time spent in REJ status
                ISNULL(SUM(CASE 
                    WHEN ASH.Status = 'REJ' THEN 
                        CASE 
                            WHEN ASH.NextSubmissionDate IS NOT NULL THEN DATEDIFF(DAY, ASH.CreatedDate, ASH.NextSubmissionDate)
                            WHEN ASH.IsCurrentStatus = 1 THEN DATEDIFF(DAY, ASH.CreatedDate, @RefreshStart)
                            ELSE 0
                        END
                    ELSE 0
                END), 0) as REJ_Days,
                -- HLD_Days: Sum all time spent in HLD status
                ISNULL(SUM(CASE 
                    WHEN ASH.Status = 'HLD' THEN 
                        CASE 
                            WHEN ASH.NextSubmissionDate IS NOT NULL THEN DATEDIFF(DAY, ASH.CreatedDate, ASH.NextSubmissionDate)
                            WHEN ASH.IsCurrentStatus = 1 THEN DATEDIFF(DAY, ASH.CreatedDate, @RefreshStart)
                            ELSE 0
                        END
                    ELSE 0
                END), 0) as HLD_Days,
                -- PHN_Days: Sum all time spent in PHN status (THIS WILL HANDLE MULTIPLE PHN PERIODS!)
                ISNULL(SUM(CASE 
                    WHEN ASH.Status = 'PHN' THEN 
                        CASE 
                            WHEN ASH.NextSubmissionDate IS NOT NULL THEN DATEDIFF(DAY, ASH.CreatedDate, ASH.NextSubmissionDate)
                            WHEN ASH.IsCurrentStatus = 1 THEN DATEDIFF(DAY, ASH.CreatedDate, @RefreshStart)
                            ELSE 0
                        END
                    ELSE 0
                END), 0) as PHN_Days,
                -- URW_Days: Sum all time spent in URW status
                ISNULL(SUM(CASE 
                    WHEN ASH.Status = 'URW' THEN 
                        CASE 
                            WHEN ASH.NextSubmissionDate IS NOT NULL THEN DATEDIFF(DAY, ASH.CreatedDate, ASH.NextSubmissionDate)
                            WHEN ASH.IsCurrentStatus = 1 THEN DATEDIFF(DAY, ASH.CreatedDate, @RefreshStart)
                            ELSE 0
                        END
                    ELSE 0
                END), 0) as URW_Days,
                -- INT_Days: Sum all time spent in INT status
                ISNULL(SUM(CASE 
                    WHEN ASH.Status = 'INT' THEN 
                        CASE 
                            WHEN ASH.NextSubmissionDate IS NOT NULL THEN DATEDIFF(DAY, ASH.CreatedDate, ASH.NextSubmissionDate)
                            WHEN ASH.IsCurrentStatus = 1 THEN DATEDIFF(DAY, ASH.CreatedDate, @RefreshStart)
                            ELSE 0
                        END
                    ELSE 0
                END), 0) as INT_Days,
                -- RHM_Days: Sum all time spent in RHM status
                ISNULL(SUM(CASE 
                    WHEN ASH.Status = 'RHM' THEN 
                        CASE 
                            WHEN ASH.NextSubmissionDate IS NOT NULL THEN DATEDIFF(DAY, ASH.CreatedDate, ASH.NextSubmissionDate)
                            WHEN ASH.IsCurrentStatus = 1 THEN DATEDIFF(DAY, ASH.CreatedDate, @RefreshStart)
                            ELSE 0
                        END
                    ELSE 0
                END), 0) as RHM_Days,
                -- DEC_Days: Sum all time spent in DEC status
                ISNULL(SUM(CASE 
                    WHEN ASH.Status = 'DEC' THEN 
                        CASE 
                            WHEN ASH.NextSubmissionDate IS NOT NULL THEN DATEDIFF(DAY, ASH.CreatedDate, ASH.NextSubmissionDate)
                            WHEN ASH.IsCurrentStatus = 1 THEN DATEDIFF(DAY, ASH.CreatedDate, @RefreshStart)
                            ELSE 0
                        END
                    ELSE 0
                END), 0) as DEC_Days,
                -- NOA_Days: Sum all time spent in NOA status
                ISNULL(SUM(CASE 
                    WHEN ASH.Status = 'NOA' THEN 
                        CASE 
                            WHEN ASH.NextSubmissionDate IS NOT NULL THEN DATEDIFF(DAY, ASH.CreatedDate, ASH.NextSubmissionDate)
                            WHEN ASH.IsCurrentStatus = 1 THEN DATEDIFF(DAY, ASH.CreatedDate, @RefreshStart)
                            ELSE 0
                        END
                    ELSE 0
                END), 0) as NOA_Days,
                
                -- OEX_Days: Sum all time spent in OEX status
                ISNULL(SUM(CASE 
                    WHEN ASH.Status = 'OEX' THEN 
                        CASE 
                            WHEN ASH.NextSubmissionDate IS NOT NULL THEN DATEDIFF(DAY, ASH.CreatedDate, ASH.NextSubmissionDate)
                            WHEN ASH.IsCurrentStatus = 1 THEN DATEDIFF(DAY, ASH.CreatedDate, @RefreshStart)
                            ELSE 0
                        END
                    ELSE 0
                END), 0) as OEX_Days,
                -- ODC_Days: Sum all time spent in ODC status
                ISNULL(SUM(CASE 
                    WHEN ASH.Status = 'ODC' THEN 
                        CASE 
                            WHEN ASH.NextSubmissionDate IS NOT NULL THEN DATEDIFF(DAY, ASH.CreatedDate, ASH.NextSubmissionDate)
                            WHEN ASH.IsCurrentStatus = 1 THEN DATEDIFF(DAY, ASH.CreatedDate, @RefreshStart)
                            ELSE 0
                        END
                    ELSE 0
                END), 0) as ODC_Days,
                
                -- HIR_Days: Sum all time spent in HIR status
                ISNULL(SUM(CASE 
                    WHEN ASH.Status = 'HIR' THEN 
                        CASE 
                            WHEN ASH.NextSubmissionDate IS NOT NULL THEN DATEDIFF(DAY, ASH.CreatedDate, ASH.NextSubmissionDate)
                            WHEN ASH.IsCurrentStatus = 1 THEN DATEDIFF(DAY, ASH.CreatedDate, @RefreshStart)
                            ELSE 0
                        END
                    ELSE 0
                END), 0) as HIR_Days,
                -- WDR_Days: Sum all time spent in WDR status
                ISNULL(SUM(CASE 
                    WHEN ASH.Status = 'WDR' THEN 
                        CASE 
                            WHEN ASH.NextSubmissionDate IS NOT NULL THEN DATEDIFF(DAY, ASH.CreatedDate, ASH.NextSubmissionDate)
                            WHEN ASH.IsCurrentStatus = 1 THEN DATEDIFF(DAY, ASH.CreatedDate, @RefreshStart)
                            ELSE 0
                        END
                    ELSE 0
                END), 0) as WDR_Days
            FROM 
                AM_FirstSubmissions AFS INNER JOIN AM_SubmissionHistory ASH ON AFS.RequisitionId = ASH.RequisitionId AND AFS.CandidateId = ASH.CandidateId
            GROUP BY 
                AFS.RequisitionId, AFS.CandidateId
        )
        
        -- Insert AM context data
        INSERT INTO [dbo].[TimingAnalyticsView] 
            ([Context], [CreatedBy], [RequisitionId], [RequisitionCode], [CompanyName], [Title], [CandidateId], [RequisitionCreatedDate], [RequisitionUpdatedDate],
            [RequisitionStatus], [TimeToFill_Days], [TimeToHire_Days], [FirstSubmissionDate], [HireDate], [PEN_Days], [REJ_Days], [HLD_Days], [PHN_Days], [URW_Days], 
            [INT_Days], [RHM_Days], [DEC_Days], [NOA_Days], [OEX_Days], [ODC_Days], [HIR_Days], [WDR_Days], [RefreshDate])
        SELECT 
            'AM' as Context, ARB.CreatedBy, ARB.RequisitionId, ARB.RequisitionCode, ARB.CompanyName, ARB.Title, AFS.CandidateId, ARB.RequisitionCreatedDate,
            ARB.RequisitionUpdatedDate, ARB.RequisitionStatus, ARB.TimeToFill, ATTH.TimeToHire, AFS.FirstSubmissionDate, ATTH.HireDate, ASTC.PEN_Days, ASTC.REJ_Days,
            ASTC.HLD_Days, ASTC.PHN_Days, ASTC.URW_Days, ASTC.INT_Days, ASTC.RHM_Days, ASTC.DEC_Days, ASTC.NOA_Days, ASTC.OEX_Days, ASTC.ODC_Days, ASTC.HIR_Days,
            ASTC.WDR_Days, @RefreshStart
        FROM 
            AM_RequisitionBase ARB INNER JOIN AM_FirstSubmissions AFS ON ARB.RequisitionId = AFS.RequisitionId
            LEFT JOIN AM_TimeToHireCalc ATTH ON AFS.RequisitionId = ATTH.RequisitionId AND AFS.CandidateId = ATTH.CandidateId
            LEFT JOIN AM_StageTimeCalculations ASTC ON AFS.RequisitionId = ASTC.RequisitionId AND AFS.CandidateId = ASTC.CandidateId;
        
        -- =====================================================================================
        -- CONTEXT 2: RECRUITER PERSPECTIVE (S.CreatedBy - Submission Owner)
        -- =====================================================================================
        
        WITH REC_SubmissionBase AS (
            -- Get submissions created by Recruiters/Full Desk users in last year
            SELECT DISTINCT
                S.RequisitionId, S.CandidateId, S.CreatedBy as SubmissionOwner, MIN(S.CreatedDate) as FirstSubmissionDate
            FROM 
                Submissions S INNER JOIN dbo.Users U ON S.CreatedBy = U.UserName 
            WHERE 
                U.Status = 1  AND U.Role IN (4, 6) AND CAST(S.CreatedDate AS DATE) >= @StartDate365 AND S.CreatedBy IS NOT NULL
            GROUP BY 
                S.RequisitionId, S.CandidateId, S.CreatedBy
        ),
        REC_RequisitionDetails AS (
            -- Get requisition details for recruiter submissions
            SELECT 
                RSB.RequisitionId, RSB.CandidateId, RSB.SubmissionOwner, RSB.FirstSubmissionDate, R.Code as RequisitionCode,
                ISNULL(C.CompanyName, 'Unknown Company') as CompanyName, R.PosTitle as Title, R.CreatedDate as RequisitionCreatedDate, R.UpdatedDate as RequisitionUpdatedDate,
                R.Status as RequisitionStatus,
                -- Time to Fill calculation (only for FUL status)
                CASE 
                    WHEN R.Status = 'FUL' THEN DATEDIFF(DAY, R.CreatedDate, R.UpdatedDate)
                    ELSE NULL 
                END as TimeToFill
            FROM 
                REC_SubmissionBase RSB INNER JOIN Requisitions R ON RSB.RequisitionId = R.Id
                LEFT JOIN Companies C ON R.CompanyId = C.ID
        ),
        REC_TimeToHireCalc AS (
            -- Calculate Time to Hire for hired candidates (recruiter context)
            SELECT 
                RSB.RequisitionId,
                RSB.CandidateId,
                RSB.SubmissionOwner,
                RSB.FirstSubmissionDate,
                MAX(CASE WHEN S.Status = 'HIR' THEN S.CreatedDate END) as HireDate,
                MAX(CASE WHEN S.Status = 'HIR' THEN DATEDIFF(DAY, RSB.FirstSubmissionDate, S.CreatedDate) END) as TimeToHire
            FROM REC_SubmissionBase RSB
            INNER JOIN Submissions S ON RSB.RequisitionId = S.RequisitionId AND RSB.CandidateId = S.CandidateId
            GROUP BY RSB.RequisitionId, RSB.CandidateId, RSB.SubmissionOwner, RSB.FirstSubmissionDate
        ),
        REC_SubmissionHistory AS (
            -- Get all submission history for recruiter context
            SELECT 
                S.RequisitionId, S.CandidateId, RSB.SubmissionOwner, S.Status, S.CreatedDate,
                LEAD(S.CreatedDate) OVER (PARTITION BY S.RequisitionId, S.CandidateId ORDER BY S.CreatedDate) as NextSubmissionDate,
                CASE WHEN ROW_NUMBER() OVER (PARTITION BY S.RequisitionId, S.CandidateId ORDER BY S.CreatedDate DESC) = 1 THEN 1 ELSE 0 END as IsCurrentStatus
            FROM 
                Submissions S INNER JOIN REC_SubmissionBase RSB ON S.RequisitionId = RSB.RequisitionId AND S.CandidateId = RSB.CandidateId
        ),
        REC_StageTimeCalculations AS (
            -- Calculate CUMULATIVE days spent in each stage (recruiter context)
            SELECT 
                RSB.RequisitionId, RSB.CandidateId, RSB.SubmissionOwner,
                -- Sum all time periods for each status (same logic as AM)
                ISNULL(SUM(CASE 
                    WHEN RSH.Status = 'PEN' THEN 
                        CASE 
                            WHEN RSH.NextSubmissionDate IS NOT NULL THEN DATEDIFF(DAY, RSH.CreatedDate, RSH.NextSubmissionDate)
                            WHEN RSH.IsCurrentStatus = 1 THEN DATEDIFF(DAY, RSH.CreatedDate, @RefreshStart)
                            ELSE 0
                        END
                    ELSE 0
                END), 0) as PEN_Days,
                ISNULL(SUM(CASE 
                    WHEN RSH.Status = 'REJ' THEN 
                        CASE 
                            WHEN RSH.NextSubmissionDate IS NOT NULL THEN DATEDIFF(DAY, RSH.CreatedDate, RSH.NextSubmissionDate)
                            WHEN RSH.IsCurrentStatus = 1 THEN DATEDIFF(DAY, RSH.CreatedDate, @RefreshStart)
                            ELSE 0
                        END
                    ELSE 0
                END), 0) as REJ_Days,
                ISNULL(SUM(CASE 
                    WHEN RSH.Status = 'HLD' THEN 
                        CASE 
                            WHEN RSH.NextSubmissionDate IS NOT NULL THEN DATEDIFF(DAY, RSH.CreatedDate, RSH.NextSubmissionDate)
                            WHEN RSH.IsCurrentStatus = 1 THEN DATEDIFF(DAY, RSH.CreatedDate, @RefreshStart)
                            ELSE 0
                        END
                    ELSE 0
                END), 0) as HLD_Days,
                ISNULL(SUM(CASE 
                    WHEN RSH.Status = 'PHN' THEN 
                        CASE 
                            WHEN RSH.NextSubmissionDate IS NOT NULL THEN DATEDIFF(DAY, RSH.CreatedDate, RSH.NextSubmissionDate)
                            WHEN RSH.IsCurrentStatus = 1 THEN DATEDIFF(DAY, RSH.CreatedDate, @RefreshStart)
                            ELSE 0
                        END
                    ELSE 0
                END), 0) as PHN_Days,
                ISNULL(SUM(CASE 
                    WHEN RSH.Status = 'URW' THEN 
                        CASE 
                            WHEN RSH.NextSubmissionDate IS NOT NULL THEN DATEDIFF(DAY, RSH.CreatedDate, RSH.NextSubmissionDate)
                            WHEN RSH.IsCurrentStatus = 1 THEN DATEDIFF(DAY, RSH.CreatedDate, @RefreshStart)
                            ELSE 0
                        END
                    ELSE 0
                END), 0) as URW_Days,
                ISNULL(SUM(CASE 
                    WHEN RSH.Status = 'INT' THEN 
                        CASE 
                            WHEN RSH.NextSubmissionDate IS NOT NULL THEN DATEDIFF(DAY, RSH.CreatedDate, RSH.NextSubmissionDate)
                            WHEN RSH.IsCurrentStatus = 1 THEN DATEDIFF(DAY, RSH.CreatedDate, @RefreshStart)
                            ELSE 0
                        END
                    ELSE 0
                END), 0) as INT_Days,
                ISNULL(SUM(CASE 
                    WHEN RSH.Status = 'RHM' THEN 
                        CASE 
                            WHEN RSH.NextSubmissionDate IS NOT NULL THEN DATEDIFF(DAY, RSH.CreatedDate, RSH.NextSubmissionDate)
                            WHEN RSH.IsCurrentStatus = 1 THEN DATEDIFF(DAY, RSH.CreatedDate, @RefreshStart)
                            ELSE 0
                        END
                    ELSE 0
                END), 0) as RHM_Days,
                ISNULL(SUM(CASE 
                    WHEN RSH.Status = 'DEC' THEN 
                        CASE 
                            WHEN RSH.NextSubmissionDate IS NOT NULL THEN DATEDIFF(DAY, RSH.CreatedDate, RSH.NextSubmissionDate)
                            WHEN RSH.IsCurrentStatus = 1 THEN DATEDIFF(DAY, RSH.CreatedDate, @RefreshStart)
                            ELSE 0
                        END
                    ELSE 0
                END), 0) as DEC_Days,
                ISNULL(SUM(CASE 
                    WHEN RSH.Status = 'NOA' THEN 
                        CASE 
                            WHEN RSH.NextSubmissionDate IS NOT NULL THEN DATEDIFF(DAY, RSH.CreatedDate, RSH.NextSubmissionDate)
                            WHEN RSH.IsCurrentStatus = 1 THEN DATEDIFF(DAY, RSH.CreatedDate, @RefreshStart)
                            ELSE 0
                        END
                    ELSE 0
                END), 0) as NOA_Days,
                ISNULL(SUM(CASE 
                    WHEN RSH.Status = 'OEX' THEN 
                        CASE 
                            WHEN RSH.NextSubmissionDate IS NOT NULL THEN DATEDIFF(DAY, RSH.CreatedDate, RSH.NextSubmissionDate)
                            WHEN RSH.IsCurrentStatus = 1 THEN DATEDIFF(DAY, RSH.CreatedDate, @RefreshStart)
                            ELSE 0
                        END
                    ELSE 0
                END), 0) as OEX_Days,
                ISNULL(SUM(CASE 
                    WHEN RSH.Status = 'ODC' THEN 
                        CASE 
                            WHEN RSH.NextSubmissionDate IS NOT NULL THEN DATEDIFF(DAY, RSH.CreatedDate, RSH.NextSubmissionDate)
                            WHEN RSH.IsCurrentStatus = 1 THEN DATEDIFF(DAY, RSH.CreatedDate, @RefreshStart)
                            ELSE 0
                        END
                    ELSE 0
                END), 0) as ODC_Days,
                ISNULL(SUM(CASE 
                    WHEN RSH.Status = 'HIR' THEN 
                        CASE 
                            WHEN RSH.NextSubmissionDate IS NOT NULL THEN DATEDIFF(DAY, RSH.CreatedDate, RSH.NextSubmissionDate)
                            WHEN RSH.IsCurrentStatus = 1 THEN DATEDIFF(DAY, RSH.CreatedDate, @RefreshStart)
                            ELSE 0
                        END
                    ELSE 0
                END), 0) as HIR_Days,
                ISNULL(SUM(CASE 
                    WHEN RSH.Status = 'WDR' THEN 
                        CASE 
                            WHEN RSH.NextSubmissionDate IS NOT NULL THEN DATEDIFF(DAY, RSH.CreatedDate, RSH.NextSubmissionDate)
                            WHEN RSH.IsCurrentStatus = 1 THEN DATEDIFF(DAY, RSH.CreatedDate, @RefreshStart)
                            ELSE 0
                        END
                    ELSE 0
                END), 0) as WDR_Days
            FROM 
                REC_SubmissionBase RSB INNER JOIN REC_SubmissionHistory RSH ON RSB.RequisitionId = RSH.RequisitionId AND RSB.CandidateId = RSH.CandidateId 
                AND RSB.SubmissionOwner = RSH.SubmissionOwner
            GROUP BY 
                RSB.RequisitionId, RSB.CandidateId, RSB.SubmissionOwner
        )
        
        -- Insert Recruiter context data
        INSERT INTO [dbo].[TimingAnalyticsView] 
            ([Context], [CreatedBy], [RequisitionId], [RequisitionCode], [CompanyName], [Title], [CandidateId], [RequisitionCreatedDate], [RequisitionUpdatedDate], 
            [RequisitionStatus], [TimeToFill_Days], [TimeToHire_Days], [FirstSubmissionDate], [HireDate], [PEN_Days], [REJ_Days], [HLD_Days], [PHN_Days], [URW_Days], 
            [INT_Days], [RHM_Days], [DEC_Days], [NOA_Days], [OEX_Days], [ODC_Days], [HIR_Days], [WDR_Days], [RefreshDate])
        SELECT 
            'RECRUITER' as Context, RRD.SubmissionOwner as CreatedBy, RRD.RequisitionId, RRD.RequisitionCode, RRD.CompanyName, RRD.Title, RRD.CandidateId,
            RRD.RequisitionCreatedDate, RRD.RequisitionUpdatedDate, RRD.RequisitionStatus, RRD.TimeToFill, RTTH.TimeToHire, RRD.FirstSubmissionDate, RTTH.HireDate, 
            RSTC.PEN_Days, RSTC.REJ_Days, RSTC.HLD_Days, RSTC.PHN_Days, RSTC.URW_Days, RSTC.INT_Days, RSTC.RHM_Days, RSTC.DEC_Days, RSTC.NOA_Days, RSTC.OEX_Days, 
            RSTC.ODC_Days, RSTC.HIR_Days, RSTC.WDR_Days, @RefreshStart
        FROM 
            REC_RequisitionDetails RRD LEFT JOIN REC_TimeToHireCalc RTTH ON RRD.RequisitionId = RTTH.RequisitionId  
            AND RRD.CandidateId = RTTH.CandidateId AND RRD.SubmissionOwner = RTTH.SubmissionOwner
            LEFT JOIN REC_StageTimeCalculations RSTC ON RRD.RequisitionId = RSTC.RequisitionId 
            AND RRD.CandidateId = RSTC.CandidateId AND RRD.SubmissionOwner = RSTC.SubmissionOwner;
        
        COMMIT TRANSACTION;

        -- Log successful refresh
        DECLARE @RowCount INT = @@ROWCOUNT;
        DECLARE @RefreshEnd DATETIME2 = SYSDATETIME();
        DECLARE @Duration INT = DATEDIFF(MILLISECOND, @RefreshStart, @RefreshEnd);
        
        PRINT 'TimingAnalyticsView refresh completed successfully.';
        PRINT 'Rows processed: ' + CAST(@RowCount AS VARCHAR(10));
        PRINT 'Duration: ' + CAST(@Duration AS VARCHAR(10)) + ' ms';
        
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        -- Error handling
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @ErrorLine INT = ERROR_LINE();
        
        PRINT 'Error in TimingAnalyticsView refresh:';
        PRINT 'Line: ' + CAST(@ErrorLine AS VARCHAR(10));
        PRINT 'Message: ' + @ErrorMessage;
        
        -- Re-throw the error
        THROW;
    END CATCH
END;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE   PROCEDURE [SaveCandidate]
	@Id int = null OUTPUT,
	@FirstName varchar(50),
	@MiddleName varchar(50),
	@LastName varchar(50),
	@Title varchar(200),
	@Eligibility int,
	@HourlyRate numeric(6, 2),
	@HourlyRateHigh numeric(6, 2),
	@SalaryLow numeric(9, 2),
	@SalaryHigh numeric(9, 2),
	@Experience int,
	@Relocate bit,
	@JobOptions varchar(50) = '',
	@Communication char(1),
	@Keywords varchar(500),
	@Status char(3),
	@TextResume varchar(max),
	@OriginalResume varchar(255),
	@FormattedResume varchar(255),
	@OriginalFileId varchar(50),
	@FormattedFileId varchar(50),
	@Address1 varchar(255),	
	@Address2 varchar(255),
	@City varchar(50),
	@StateId int,
	@ZipCode varchar(20),
	@Email varchar(255),
	@Phone1 varchar(15),
	@Phone2 varchar(15),
	@Phone3 varchar(15),
	@Phone3Ext smallint,
	@LinkedIn varchar(255),
	@Facebook varchar(255),
	@Twitter varchar(255),
	@Google varchar(255),
	@Refer bit,
	@ReferAccountMgr varchar(10),
	@TaxTerm varchar(10),
	@Background bit,
	@Summary varchar(max),
	@Objective varchar(max),
	@EEO bit,
	@RelocNotes varchar(200),
	@SecurityClearanceNotes varchar(200),
	@User varchar(10),
	@ExperienceSummary varchar(MAX) = '',
	@ExperienceMonths int = 0,
	@RandomNumber int = 0
AS
BEGIN
	SET NOCOUNT ON;
	SET ARITHABORT ON;
	SET ANSI_NULLS ON;
	SET QUOTED_IDENTIFIER ON;

    BEGIN TRY
        BEGIN TRANSACTION;
		
		DECLARE @Description varchar(7000), @Action varchar(40);
		DECLARE @SkillId int, @SkillCount int;
		DECLARE @IsAdd bit = 0;

		if (@Id IS NULL OR @Id = 0) --insert
			BEGIN
				SET @IsAdd = 1;
				DECLARE @RateNotes varchar(200), @MPCNotes varchar(200);
				SET @RateNotes = '[{"DateTime":"' + CONVERT(varchar(19), GETDATE(), 126) + '","Name":"' + @User + '","Rating":3,"Comment":"Candidate Created"}]';-- FORMAT(GETDATE(), 'MM/dd/yyyy hh:mm:ss tt') + '^' + '['+ @User +']^3^New Candidate Added';
				SET @MPCNotes = '[{"DateTime":"' + CONVERT(varchar(19), GETDATE(), 126) + '","Name":"' + @User + '","MPC":false,"Comment":"Candidate Created"}]';-- FORMAT(GETDATE(), 'MM/dd/yyyy hh:mm:ss tt') + '^' + '['+ @User +']^False^New Candidate Added';

				if ((@StateID IS NULL OR @StateID = 0) AND (@ZipCode IS NOT NULL AND @ZipCode <> ''))
					BEGIN
						SELECT TOP 1
							@StateID = ISNULL(B.Id, 1)
						FROM
							dbo.[ZipCodes] A INNER JOIN dbo.[State] B ON A.[StateAbbreviation] = B.[Code]
							AND A.ZipCode = @ZipCode;
					END

				INSERT INTO
					dbo.[Candidate]
					([FirstName], [MiddleName], [LastName], [Title], [EligibilityId], [HourlyRate], [HourlyRateHigh], [SalaryLow], [SalaryHigh], [ExperienceId], [Relocate], 
					[JobOptions], [Communication], [TextResume], [OriginalResume], [FormattedResume], [Keywords], [Status], [OriginalFileId], [FormattedFileId], [Address1], 
					[Address2], [City], [StateId], [ZipCode], [Email], [Phone1], [Phone2], [Phone3], [Phone3Ext], [LinkedIn], [Facebook], [Twitter], [Refer], [ReferAccountMgr], 
					[TaxTerm], [Background], [Summary], [Objective], [EEO], [RelocNotes], [SecurityClearanceNotes], [CreatedBy],  [CreatedDate], [UpdatedBy], [UpdatedDate], 
					[ExperienceSummary], [Experience], [Google], [RateCandidate], [RateNotes], [MPC], [MPCNotes])
				VALUES
					(@FirstName, @MiddleName, @LastName, @Title, @Eligibility, @HourlyRate, @HourlyRateHigh, @SalaryLow, @SalaryHigh, @Experience, @Relocate, 
					@JobOptions, @Communication, @TextResume, @OriginalResume, @FormattedResume, @Keywords, @Status, @OriginalFileId, @FormattedFileId, @Address1, 
					@Address2, @City, @StateId, @ZipCode, @Email, @Phone1, @Phone2, @Phone3, @Phone3Ext, @LinkedIn, @Facebook, @Twitter, @Refer, 
					@ReferAccountMgr, @TaxTerm, @Background, @Summary, @Objective, @EEO, @RelocNotes, @SecurityClearanceNotes, @User, GETDATE(), @User, GETDATE(), 
					@ExperienceSummary, @ExperienceMonths, @Google, 3, @RateNotes, 0, @MPCNotes);
				
				SET @Id = IDENT_CURRENT('Candidate');

				SELECT
					@SkillId = A.[Id]
				FROM
					dbo.[Skills] A
				WHERE
					A.[Skill] = 'OTHER';

				INSERT INTO
					dbo.[EntitySkills]
					([EntityId], [EntityType], [SkillId], [LastUsed], [ExpMonth], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate])
				VALUES
					(@Id, 'CND', @SkillId, 0, 0, @User, GETDATE(), @User, GETDATE());

				UPDATE
					dbo.[Notes]
				SET
					[EntityId] = @Id
				WHERE
					[EntityId] = @RandomNumber
					AND [EntityType] = 'CND';
	
				SET @Action = 'Insert Candidate';
				SET @Description = 'Inserted New Candidate: ' + @FirstName + ' ' + ISNULL(@MiddleName, '') + ' ' + @LastName + ', [ID: ' + CAST(@Id as varchar(10)) + ']';
			END
		else --update
			BEGIN
				SET @IsAdd = 0;

				UPDATE
					dbo.[Candidate]
				SET
					[FirstName] = @FirstName, 
					[MiddleName] = @MiddleName, 
					[LastName] = @LastName, 
					[Title] = @Title, 
					[EligibilityId] = @Eligibility, 
					[HourlyRate] = @HourlyRate, 
					[HourlyRateHigh] = @HourlyRateHigh, 
					[SalaryLow] = @SalaryLow,
					[SalaryHigh] = @SalaryHigh, 
					[ExperienceId] = @Experience, 
					[Relocate] = @Relocate, 
					[JobOptions] = @JobOptions, 
					[Communication] = @Communication, 
					[TextResume] = @TextResume, 
					[Keywords] = @Keywords, 
					[Status] = @Status, 
					[Address1] = @Address1, 
					[Address2] = @Address2, 
					[City] = @City,
					[StateId] = @StateId, 
					[ZipCode] = @ZipCode, 
					[Email] = @Email, 
					[Phone1] = REPLACE(REPLACE(REPLACE(@Phone1, '(', ''), ')', ''), ' ', ''), 
					[Phone2] = REPLACE(REPLACE(REPLACE(@Phone2, '(', ''), ')', ''), ' ', ''), 
					[Phone3] = REPLACE(REPLACE(REPLACE(@Phone3, '(', ''), ')', ''), ' ', ''), 
					[Phone3Ext] = @Phone3Ext, 
					[LinkedIn] = @LinkedIn, 
					[Facebook] = @Facebook, 
					[Twitter] = @Twitter, 
					[Google] = @Google, 
					[Refer] = @Refer, 
					[ReferAccountMgr] = @ReferAccountMgr,
					[TaxTerm] = @TaxTerm, 
					[Background] = @Background, 
					[Summary] = @Summary, 
					[Objective] = @Objective, 
					[RelocNotes] = @RelocNotes, 
					[SecurityClearanceNotes] = @SecurityClearanceNotes,
					[UpdatedBy] = @User,
					[UpdatedDate] = GETDATE(),
					[ExperienceSummary] = @ExperienceSummary,
					[Experience] = @ExperienceMonths
				WHERE
					dbo.[Candidate].[Id] = @Id;

				SET @Action = 'Update Candidate';
				SET @Description = 'Updated Candidate: ' + @FirstName + ' ' + ISNULL(@MiddleName, '') + ' ' + @LastName + ', [ID: ' + CAST(@Id as varchar(10)) + ']';
			END
	
		exec dbo.AddAuditTrail @Action, 'Candidate Details', @Description, @User; 

		DECLARE @SendTo varchar(200);
		SELECT
			@SendTo = A.[SendTo]
		FROM
			dbo.[Templates] A
		WHERE
			A.Action = CASE @IsAdd WHEN 1 THEN 1 ELSE 2 END; --Candidate Created or Updated

		SELECT
			A.[Cc], A.[Subject], A.[Template]
		FROM
			dbo.[Templates] A
		WHERE
			A.Action = CASE @IsAdd WHEN 1 THEN 1 ELSE 2 END; --Candidate Created or Updated

		SELECT
			A.[FirstName] + ' ' + A.[LastName], A.[EmailAddress]--, B.RoleName 
		FROM
			dbo.[Users] A INNER JOIN dbo.[Roles] B ON A.[Role] = B.[Id] 
			AND B.[RoleName] IN (SELECT LTRIM(RTRIM(s)) from dbo.BigSplit(',', @SendTo))
			AND A.[Status] = 1
			UNION
		SELECT 
			A.[FirstName] + ' ' + A.[LastName], A.[EmailAddress] 
		FROM
			dbo.[Users] A INNER JOIN dbo.[Candidate] B ON (A.[UserName] = B.[CreatedBy] OR A.[UserName] = B.[UpdatedBy])
			AND A.[Status] = 1
		WHERE
			B.[Id] = @Id
			AND CASE WHEN (CHARINDEX('Candidate Owner', @SendTo) > 0) THEN 1 ELSE 0 END = 1;

		if (@StateId IS NOT NULL AND @StateId > 0)
			BEGIN
				SELECT
					A.[Code]
				FROM
					dbo.[State] A
				WHERE
					A.[Id] = @StateId;
			END
		else
			BEGIN
				SELECT ''
			END

		SELECT @Id;

		COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [SaveCandidateActivity]
	@SubmissionId int = 5505,
	@CandidateId int = 19,
	@RequisitionId int = 1,
	@Notes varchar(1000) = 'P\nDAVID MATTSON\n95 Somerset Street',
	@Status char(3) = 'NOA',
	@User varchar(10) = 'JOLLY',
	@ShowCalendar bit = 0,
	@DateTime datetime = '2025-03-09 10:00:00AM',
	@Type char(1)= 'I',
	@PhoneNumber varchar(20) = '1233555220',
	@InterviewDetails varchar(2000) = '',
	@UpdateSchedule bit = 0,
	@CandScreen bit = 1,
	@RoleId char(2) = '0',
	@ClientRate numeric(10,2) = 0,
	@Hours smallint = 1,
	@CostPercent numeric(7,2) = 100,
	@CandidatePayRate numeric(7,2) = 0,
	@Spread numeric(5,2) = 0,
	@CommissionSpread numeric(5,2) = 0,
	@CommissionPercent numeric(7,2) = 0,
	@Commission numeric(6,2) = 0
AS
BEGIN;
	SET NOCOUNT ON;
	SET ARITHABORT ON;
	SET ANSI_NULLS ON;
	SET QUOTED_IDENTIFIER ON;

    BEGIN TRY
        BEGIN TRANSACTION;
		
		DECLARE @Description varchar(7000), @Action varchar(40), @Name varchar(300) = '', @Requisition varchar(200);
		DECLARE @Count int, @CountFill int, @Positions int, @CurrentId int;

		SELECT
			@Name = A.[FirstName] + ' ' + ISNULL(A.[MiddleName], '') + ' ' + A.[LastName]
		FROM
			dbo.[Candidate] A
		WHERE
			A.[Id] = @CandidateId;

		if (@SubmissionId IS NOT NULL)
			BEGIN;
				SELECT
					@CandidateId = A.[CandidateId], @RequisitionId = A.[RequisitionId]
				FROM
					dbo.[Submissions] A
				WHERE
					A.[Id] = @SubmissionId;
			END;

		SELECT
			@Requisition = A.[PosTitle]
		FROM
			dbo.[Requisitions] A
		WHERE
			A.[Id] = @RequisitionId;

		if (@UpdateSchedule = 0)
			BEGIN;
					INSERT INTO
						dbo.[Submissions]
						([RequisitionId], [CandidateId], [Status], [Notes], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate],
						[ShowCalendar], [DateTime], [Type], [PhoneNumber], [InterviewDetails])
					VALUES
						(@RequisitionId, @CandidateId, @Status, @Notes, @User, GETDATE(), @User, GETDATE(),
						@ShowCalendar, @DateTime, @Type, @PhoneNumber, @InterviewDetails);

					SELECT
						@CurrentId = IDENT_CURRENT('dbo.Submissions');

					SELECT
						@Count = (SELECT COUNT(*) FROM (SELECT COUNT(*) [Cnt] FROM dbo.[Submissions] A WHERE A.[RequisitionId] = @RequisitionId GROUP BY [RequisitionId], [CandidateId]) B),
						@CountFill = (SELECT COUNT(*) FROM dbo.[Submissions] A WHERE A.[RequisitionId] = @RequisitionId AND A.[Status] = 'HIR'),
						@Positions = (SELECT A.[Positions] FROM dbo.[Requisitions] A WHERE A.[Id] = @RequisitionId);

					DECLARE @JobOptions char(1), @Points tinyint;

					--SELECT
					--	@Points = A.[Points]
					--FROM
					--	dbo.CommissionConfig A
					--WHERE
					--	@CommissionPercent BETWEEN A.[MinSpread] AND A.[MaxSpread];

					SELECT
						@JobOptions = A.[JobOption]
					FROM
						dbo.[Requisitions] A
					WHERE
						A.[Id] = @RequisitionId;

					if (@ClientRate > 0 AND @Commission > 0)
						BEGIN
							INSERT INTO	
								dbo.[SubmissionCommission]
								([SubmissionId], [RequirementId], [JobOptions], [ClientRate], [CommissionPercent], [Hours], [CostPercent], [CandidatePayRate],
								[Spread], [CommissionSpread], [Commission], [Points])
							VALUES
								(@CurrentId, @RequisitionId, @JobOptions, @ClientRate, @CommissionPercent, @Hours, @CostPercent, @CandidatePayRate, 
								@Spread, @CommissionSpread, @Commission, @Points);
						END

					if (@CountFill >= @Positions)
						BEGIN;
							UPDATE
								dbo.[Requisitions]
							SET
								[Status] = 'FUL'
							WHERE
								[Id] = @RequisitionId;
						END;
					else if (@CountFill < @Positions AND @CountFill > 0)
						BEGIN
							UPDATE
								dbo.[Requisitions]
							SET
								[Status] = 'PAR'
							WHERE
								[Id] = @RequisitionId;
						END
					else if (@Count > 0)
						BEGIN;
							UPDATE
								dbo.[Requisitions]
							SET
								[Status] = 'OPN'
							WHERE
								[Id] = @RequisitionId;
						END;

					SET @Action = 'Submit Candidate';
					SET @Description = 'Submitted Candidate: ' + @Name + ', [ID: ' + CAST(@CandidateId as varchar(10)) 
										+ '] for Requisition ' + @Requisition + ' - [ID: ' + CAST(@RequisitionId as varchar(10)) + ']'
										+ ', Status = ' +  @Status;
			END;
		else
			BEGIN;
				UPDATE
					dbo.[Submissions]
				SET
					[ShowCalendar] = @ShowCalendar,
					[DateTime] = @DateTime,
					[Type] = @Type,
					[PhoneNumber] = @PhoneNumber,
					[InterviewDetails] = @InterviewDetails,
					[Notes] = @Notes
				WHERE
					[Id] = @SubmissionId;

				SET @Action = 'Update Schedule';
				SET @Description = 'Updated Schedule for Candidate: ' + @Name + ', [ID: ' + CAST(@CandidateId as varchar(10)) 
									+ '] for Requisition ' + @Requisition + ' - [ID: ' + CAST(@RequisitionId as varchar(10)) + ']'
									+ ', SubmissionID = ' +  CAST(@SubmissionId as varchar(10));
			END;
	
		exec dbo.AddAuditTrail @Action, 'Submit Candidate', @Description, @User; 

		if (@CandScreen = 1)
			BEGIN;
				exec dbo.[GetCandidateSubmission]  @CandidateId, @RoleId;
			END;
		else
			BEGIN;
				exec dbo.[GetRequisitionSubmission] @RequisitionId, @RoleId;
			END;

		DECLARE @FirstTime bit = 0;
		if (EXISTS(SELECT * FROM dbo.[Submissions] A WHERE A.[CandidateId] = @CandidateId AND A.[RequisitionId] = @RequisitionId))
			SET @FirstTime = 0;
		else
			SET @FirstTime = 1;

		SELECT
			A.[FirstName], A.[LastName], B.[Code], B.[PosTitle], ISNULL(A.[OriginalResume], ''), ISNULL(A.[OriginalFileId], ''), ISNULL(A.[FormattedFileId], ''), ISNULL(A.[FormattedFileId], ''), 
			ISNULL(C.CompanyName, '')
		FROM
			dbo.[Candidate] A, dbo.[Requisitions] B LEFT JOIN dbo.Companies C ON B.CompanyId = C.Id
		WHERE
			A.[Id] = @CandidateId 
			AND B.[Id] = @RequisitionId

		DECLARE @SendTo varchar(200);

		SELECT
			@SendTo = A.[SendTo]
		FROM
			dbo.[Templates] A
		WHERE
			A.Action = CASE @FirstTime WHEN 0 THEN 9 ELSE 3 END; --Candidate Submission Updated

		SELECT
			A.[Cc], A.[Subject], A.[Template]
		FROM
			dbo.[Templates] A
		WHERE
			A.Action = CASE @FirstTime WHEN 0 THEN 9 ELSE 3 END; --Candidate Submission Updated

		SELECT
			A.[FirstName] + ' ' + A.[LastName], A.[EmailAddress] 
		FROM
			dbo.[Users] A INNER JOIN [Roles] B ON A.[Role] = B.[Id] 
			AND B.[RoleName] IN (SELECT LTRIM(RTRIM(s)) from dbo.BigSplit(',', @SendTo))
			AND A.[Status] = 1

		UNION

		SELECT 
			A.[FirstName] + ' ' + A.[LastName], A.[EmailAddress] 
		FROM
			dbo.[Users] A INNER JOIN dbo.[Candidate] B ON (A.[UserName] = B.[CreatedBy] OR A.[UserName] = B.[UpdatedBy])
			AND A.[Status] = 1
		WHERE
			B.[Id] = @CandidateId
			AND CASE WHEN (CHARINDEX('Candidate Owner', @SendTo) > 0) THEN 1 ELSE 0 END = 1

		UNION

		SELECT 
			A.[FirstName] + ' ' + A.[LastName], A.[EmailAddress] 
		FROM
			dbo.[Users] A INNER JOIN dbo.[Requisitions] B ON (A.[UserName] = B.[CreatedBy] OR A.[UserName] = B.[UpdatedBy])
			AND A.[Status] = 1
		WHERE
			B.[Id] = @RequisitionId
			AND CASE WHEN (CHARINDEX('Requisition Owner', @SendTo) > 0) THEN 1 ELSE 0 END = 1

		UNION

		SELECT
			A.[FirstName] + ' ' + A.[LastName], A.[EmailAddress] 
		FROM
			dbo.[Users] A
		WHERE 
			A.[UserName] IN (SELECT LTRIM(RTRIM(s)) from dbo.BigSplit(',', (SELECT R.[AssignedRecruiter] FROM dbo.[Requisitions] R WHERE R.[Id] = @RequisitionId)))
			AND A.[Status] = 1
			AND CASE WHEN (CHARINDEX('Requisition Assigned', @SendTo) > 0) THEN 1 ELSE 0 END = 1;

        COMMIT TRANSACTION;

    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   procedure [SaveCandidateDocuments]
	@CandidateDocId int = NULL,
	@CandidateId int = 11600,
	@DocumentName varchar(255),
	@DocumentLocation varchar(255),
	@DocumentNotes varchar(2000),
	@InternalFileName varchar(50),
	@DocumentType int,
	@DocumentUpdatedDate datetime = NULL,
	@DocsUser varchar(10)
as
BEGIN;
	SET NOCOUNT ON;
	SET ARITHABORT ON;
	SET ANSI_NULLS ON;
	SET QUOTED_IDENTIFIER ON;

    BEGIN TRY
        BEGIN TRANSACTION;
		DECLARE @Description varchar(7000), @Action varchar(30);

		if (@CandidateDocId IS NULL OR @CandidateDocId = 0) --insert
			BEGIN;
				INSERT INTO 
					[dbo].[CandidateDocument]
					([CandidateId], [DocumentName], [DocumentLocation], [DocumentType], [Notes], [InternalFileName], [LastUpdatedDate], [LastUpdatedBy])
				VALUES
					(@CandidateId, @DocumentName, @DocumentLocation, @DocumentType, @DocumentNotes, @InternalFileName,
					CASE WHEN @DocumentUpdatedDate IS NULL THEN GETDATE() ELSE @DocumentUpdatedDate END, @DocsUser);

				SET @Description = 'Added Document for [ID: ' + CAST(@CandidateId as varchar(10)) + '], Document Name: ' + @DocumentName;
								
				SET @Action = 'Add Candidate Document';
			END;
		else -- update
			BEGIN;
				UPDATE 
					[dbo].[CandidateDocument]
				SET 
					[CandidateId] = @CandidateId,
					[DocumentName] = @DocumentName,
					[DocumentLocation] = CASE WHEN @DocumentLocation IS NULL THEN [DocumentLocation] ELSE @DocumentLocation END,
					[DocumentType] = @DocumentType,
					[Notes] = @DocumentNotes,
					[LastUpdatedDate] = CASE WHEN @DocumentUpdatedDate IS NULL THEN GETDATE() ELSE @DocumentUpdatedDate END,
					[LastUpdatedBy] = @DocsUser
				WHERE 
					[CandidateDocId] = @CandidateDocId;

				SET @Description = 'Updated Document for [ID: ' + CAST(@CandidateId as varchar(10)) + '], Document Name: ' + @DocumentName;
								
				SET @Action = 'Update Candidate Document';
			END;
	
		exec dbo.AddAuditTrail @Action, 'Candidate Document', @Description, @DocsUser; 

		exec dbo.[GetCandidateDocuments] @CandidateId;

        COMMIT TRANSACTION;

    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [SaveCandidateWithSubmissions]
	@Id int = null OUTPUT,
	@FirstName varchar(50),
	@MiddleName varchar(50),
	@LastName varchar(50),
	@Title varchar(200),
	@Eligibility int,
	@HourlyRate numeric(6, 2),
	@HourlyRateHigh numeric(6, 2),
	@SalaryLow numeric(9, 2),
	@SalaryHigh numeric(9, 2),
	@Experience int,
	@Relocate bit,
	@JobOptions varchar(50) = '',
	@Communication char(1),
	@Keywords varchar(500),
	@Status char(3),
	@TextResume varchar(max),
	@OriginalResume varchar(255),
	@FormattedResume varchar(255),
	@OriginalFileId varchar(50),
	@FormattedFileId varchar(50),
	@Address1 varchar(255),	
	@Address2 varchar(255),
	@City varchar(50),
	@StateId int,
	@ZipCode varchar(20),
	@Email varchar(255),
	@Phone1 varchar(15),
	@Phone2 varchar(15),
	@Phone3 varchar(15),
	@Phone3Ext smallint,
	@LinkedIn varchar(255),
	@Facebook varchar(255),
	@Twitter varchar(255),
	@Google varchar(255),
	@Refer bit,
	@ReferAccountMgr varchar(10),
	@TaxTerm varchar(10),
	@Background bit,
	@Summary varchar(max),
	@Objective varchar(max),
	@EEO bit,
	@RelocNotes varchar(200),
	@SecurityClearanceNotes varchar(200),
	@User varchar(10),
	@ExperienceSummary varchar(MAX) = '',
	@ExperienceMonths int = 0,
	@RandomNumber int = 0,
	@RequisitionID int = 0,
	@SubmissionNotes varchar(1000) = ''
AS
BEGIN
	SET NOCOUNT ON;
	SET ARITHABORT ON;
	SET ANSI_NULLS ON;
	SET QUOTED_IDENTIFIER ON;

    BEGIN TRY
        BEGIN TRANSACTION;
		
		DECLARE @Description varchar(7000), @Action varchar(40);
		DECLARE @SkillId int, @SkillCount int;
		DECLARE @IsAdd bit = 0;

		SET @IsAdd = 1;
		DECLARE @RateNotes varchar(200), @MPCNotes varchar(200);
		SET @RateNotes = '[{"DateTime":"' + CONVERT(varchar(19), GETDATE(), 126) + '","Name":"' + @User + '","Rating":3,"Comment":"Candidate Created"}]';-- FORMAT(GETDATE(), 'MM/dd/yyyy hh:mm:ss tt') + '^' + '['+ @User +']^3^New Candidate Added';
		SET @MPCNotes = '[{"DateTime":"' + CONVERT(varchar(19), GETDATE(), 126) + '","Name":"' + @User + '","MPC":false,"Comment":"Candidate Created"}]';-- FORMAT(GETDATE(), 'MM/dd/yyyy hh:mm:ss tt') + '^' + '['+ @User +']^False^New Candidate Added';

		if ((@StateID IS NULL OR @StateID = 0) AND (@ZipCode IS NOT NULL AND @ZipCode <> ''))
			BEGIN
				SELECT TOP 1
					@StateID = ISNULL(B.Id, 1)
				FROM
					dbo.[ZipCodes] A INNER JOIN dbo.[State] B ON A.[StateAbbreviation] = B.[Code]
					AND A.ZipCode = @ZipCode;
			END

		INSERT INTO
			dbo.[Candidate]
			([FirstName], [MiddleName], [LastName], [Title], [EligibilityId], [HourlyRate], [HourlyRateHigh], [SalaryLow], [SalaryHigh], [ExperienceId], [Relocate], 
			[JobOptions], [Communication], [TextResume], [OriginalResume], [FormattedResume], [Keywords], [Status], [OriginalFileId], [FormattedFileId], [Address1], 
			[Address2], [City], [StateId], [ZipCode], [Email], [Phone1], [Phone2], [Phone3], [Phone3Ext], [LinkedIn], [Facebook], [Twitter], [Refer], [ReferAccountMgr], 
			[TaxTerm], [Background], [Summary], [Objective], [EEO], [RelocNotes], [SecurityClearanceNotes], [CreatedBy],  [CreatedDate], [UpdatedBy], [UpdatedDate], 
			[ExperienceSummary], [Experience], [Google], [RateCandidate], [RateNotes], [MPC], [MPCNotes])
		VALUES
			(@FirstName, @MiddleName, @LastName, @Title, @Eligibility, @HourlyRate, @HourlyRateHigh, @SalaryLow, @SalaryHigh, @Experience, @Relocate, 
			@JobOptions, @Communication, @TextResume, @OriginalResume, @FormattedResume, @Keywords, @Status, @OriginalFileId, @FormattedFileId, @Address1, 
			@Address2, @City, @StateId, @ZipCode, @Email, @Phone1, @Phone2, @Phone3, @Phone3Ext, @LinkedIn, @Facebook, @Twitter, @Refer, 
			@ReferAccountMgr, @TaxTerm, @Background, @Summary, @Objective, @EEO, @RelocNotes, @SecurityClearanceNotes, @User, GETDATE(), @User, GETDATE(), 
			@ExperienceSummary, @ExperienceMonths, @Google, 3, @RateNotes, 0, @MPCNotes);
				
		SET @Id = IDENT_CURRENT('Candidate');

		SELECT
			@SkillId = A.[Id]
		FROM
			dbo.[Skills] A
		WHERE
			A.[Skill] = 'OTHER';

		INSERT INTO
			dbo.[EntitySkills]
			([EntityId], [EntityType], [SkillId], [LastUsed], [ExpMonth], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate])
		VALUES
			(@Id, 'CND', @SkillId, 0, 0, @User, GETDATE(), @User, GETDATE());

		INSERT INTO
			dbo.[Notes]
			(EntityId, EntityType, Notes, CreatedBy, UpdatedBy)
		VALUES
			(@Id, 'CND', 'Candidate Submitted for Requisition ID:' + CAST(@RequisitionID as varchar(10)), @User, @User);

		if (@RequisitionID <> 0 AND TRIM(@SubmissionNotes) <> '')
			BEGIN
				INSERT INTO
					dbo.[Submissions]
					([CandidateId], [RequisitionId], [Status], [Notes], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [StatusId], [ShowCalendar], [DateTime], [Type], [PhoneNumber], 
					[InterviewDetails], [Undone])
				VALUES
					(@ID, @RequisitionID, 'PEN', @SubmissionNotes, @User, GETDATE(), @User, GETDATE(), 1, 0, NULL, 'P', '', '', 0);
			END
	
		SET @Action = 'Insert Candidate';
		SET @Description = 'Inserted New Candidate: ' + @FirstName + ' ' + ISNULL(@MiddleName, '') + ' ' + @LastName + ', [ID: ' + CAST(@Id as varchar(10)) + ']';
	
		exec dbo.AddAuditTrail @Action, 'Candidate Details', @Description, @User; 

		DECLARE @SendTo varchar(200);
		WITH DistinctRoles AS (
			SELECT 
				DISTINCT LTRIM(RTRIM(value)) AS Role
			FROM 
				dbo.Templates A CROSS APPLY STRING_SPLIT(A.SendTo, ',')
			WHERE 
				A.Action IN (1, 3)
		)
		SELECT 
			@SendTo = STRING_AGG(Role, ', ')
		FROM 
			DistinctRoles;
	
		SELECT
			A.[Cc], A.[Subject], A.[Template]
		FROM
			dbo.[Templates] A
		WHERE
			A.Action IN (1, 3); --Candidate Created or Updated

		execute dbo.[GetNotificationEmails] @RequisitionID, @Id, @SendTo;

		if (@StateId IS NOT NULL AND @StateId > 0)
			BEGIN
				SELECT
					A.[Code]
				FROM
					dbo.[State] A
				WHERE
					A.[Id] = @StateId;
			END
		else
			BEGIN
				SELECT ''
			END

		if (@RequisitionID IS NOT NULL AND @RequisitionID <> 0)
			BEGIN
				SELECT 
					@Id, A.[PosTitle], B.[CompanyName]
				FROM
					dbo.[Requisitions] A INNER JOIN dbo.[Companies] B ON A.[CompanyId] = B.[ID]
					AND A.[ID] = @RequisitionID;
			END
		else
			BEGIN
				SELECT
					@Id, '', '';
			END

        COMMIT TRANSACTION;

    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [SaveCompany] 
	@ID int = 0,
	@CompanyName varchar(100) = '',
	@EIN varchar(9) = '',
	@WebsiteURL varchar(255) = '',
	@DUN varchar(20) = '',
	@NAICSCode varchar(6) = '',
	@Status bit = 1,
	@Notes varchar(2000) = '',
	@StreetName varchar(500) = '',
	@City varchar(100) = '',
	@StateID tinyint = 1,
	@ZipCode varchar(10) = '',
	@CompanyEmail varchar(255) = '',
	@Phone varchar(20) = '',
	@Extension varchar(10) = '',
	@Fax varchar(20) = '',
	@LocationNotes varchar(2000) = '',
	@User varchar(10) = 'ADMIN'
AS
BEGIN
	SET NOCOUNT ON;
	SET ARITHABORT ON;
	SET ANSI_NULLS ON;
	SET QUOTED_IDENTIFIER ON;

    BEGIN TRY
        BEGIN TRANSACTION;

		DECLARE @return varchar(max);

		if (@ID IS NULL OR @ID = 0)
			BEGIN
				INSERT INTO 
					dbo.[Companies]
					([CompanyName], [EIN], [WebsiteURL], [DUN], [NAICSCode], [Status], [Notes], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate])
				VALUES
					(@CompanyName, @EIN, @WebsiteURL, @DUN, @NAICSCode, @Status, @Notes, @User, GETDATE(), @User, GETDATE());

				SELECT
					@ID = IDENT_CURRENT('dbo.Companies');

				INSERT INTO
					dbo.CompanyLocations
					([CompanyID], [StreetName], [City], [StateID], [ZipCode], [CompanyEmail], [Phone], [Extension], [Fax], [Notes], [IsPrimaryLocation], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate])
				VALUES
					(@ID, @StreetName, @City, @StateID, @ZipCode, @CompanyEmail, @Phone, @Extension, @Fax, @LocationNotes, 1, @User, GETDATE(), @User, GETDATE());
			END
		else
			BEGIN
				UPDATE
					dbo.[Companies]
				SET
					[CompanyName] = @CompanyName,
					[EIN] = @EIN,
					[WebsiteURL] = @WebsiteURL,
					[DUN] = @DUN,
					[NAICSCode] = @NAICSCode,
					[Status] = @Status,
					[Notes] = @Notes,
					[UpdatedBy] = @User,
					[UpdatedDate] = GETDATE()
				WHERE
					[ID] = @ID;

				UPDATE
					dbo.[CompanyLocations]
				SET
					[StreetName] = @StreetName,
					[City] = @City,
					[StateID] = @StateID,
					[ZipCode] = @ZipCode,
					[CompanyEmail] = @CompanyEmail,
					[Phone] = @Phone,
					[Extension] = @Extension,
					[Fax] = @Fax,
					[Notes] = @LocationNotes,
					[UpdatedBy] = @User,
					[UpdatedDate] = GETDATE()
				WHERE
					[CompanyID] = @ID
					AND [IsPrimaryLocation] = 1;
			END

		--SELECT
		--	@ID;

		SELECT @return = 
		(SELECT
			A.[ID], @CompanyName [Name], B.[CompanyEmail] [EmailAddress], A.[EIN], B.[Phone] [Phone], ISNULL(B.[Extension], '') [Extension], ISNULL(B.[Fax], '') [Fax], B.[StreetName], 
			B.[City], B.[StateID], ISNULL(C.[State], '') [State], B.[ZipCode], ISNULL(A.[WebsiteURL], '') [Website], ISNULL(A.[DUN], '') [DUNS], ISNULL(A.[NAICSCode], '0') [NAICSCode], A.[Status], ISNULL(A.[Notes], '') [Notes], 
			ISNULL(B.[Notes], '') [LocationNotes],  A.[CreatedBy], A.[CreatedDate], A.[UpdatedBy], A.[UpdatedDate], ISNULL(D.NAICSTitle, '') [NAICS]
		FROM
			dbo.[Companies] A INNER JOIN dbo.[CompanyLocations] B ON A.[ID] = B.[CompanyID]
			LEFT JOIN dbo.[State] C ON B.[StateID] = C.[ID]
			LEFT JOIN dbo.[NAICS] D ON A.[NAICSCode] = D.[ID]
		WHERE 
			B.[IsPrimaryLocation] = 1 AND A.[ID] = @ID
		FOR JSON PATH, WITHOUT_ARRAY_WRAPPER);
	
		SELECT @return;

		SELECT @return = 
		(SELECT
			A.[ID], A.[CompanyID], A.[StreetName], A.[City], A.[StateID], ISNULL(B.[State], '') [State], A.[ZipCode], A.[CompanyEmail] [EmailAddress], A.[Phone], ISNULL(A.[Extension], '') [Extension], ISNULL(A.[Fax], '') [Fax],
			A.[IsPrimaryLocation] [PrimaryLocation], ISNULL(A.[Notes], '') [Notes], ISNULL(A.[CreatedBy], 'ADMIN') [CreatedBy], ISNULL(A.[CreatedDate], GETDATE()) [CreatedDate], ISNULL(A.[UpdatedBy], 'ADMIN') [UpdatedBy], 
			ISNULL(A.[UpdatedDate], GETDATE()) [UpdatedDate], @CompanyName [CompanyName]
		FROM
			dbo.[CompanyLocations] A LEFT JOIN dbo.[State] B ON A.[StateID] = B.[ID]
		WHERE 
			A.[CompanyID] = @ID
		ORDER BY
			A.[IsPrimaryLocation] DESC
		FOR JSON PATH);
	
		SELECT @return;

        COMMIT TRANSACTION;

    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH

END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [SaveCompanyContact]
	@ID int = 0,
	@CompanyID int = 284,
	@Prefix varchar(10) = '',
	@FirstName varchar(50) = '',
	@MiddleInitial varchar(10) = '',
	@LastName varchar(50) = '',
	@Suffix varchar(10) = '',
	@CompanyLocationID int = 0,
	@Email varchar(255) = 'arlington@delwestusa.com',
	@Phone varchar(20) = '6812975700',
	@Extension varchar(10) = '',
	@Fax varchar(20) = '',
	@Designation varchar(255) = '',
	@Department varchar(255) = '',
	@Role tinyint = 0,
	@ContactNotes varchar(2000) = 'Some Notes',
	@IsPrimaryContact bit = 0,
	@User varchar(10) = 'ADMIN'
AS
BEGIN
	SET NOCOUNT ON;
	SET ARITHABORT ON;
	SET ANSI_NULLS ON;
	SET QUOTED_IDENTIFIER ON;

    BEGIN TRY
        BEGIN TRANSACTION;

		DECLARE @Count int = 0;

		SELECT
			@Count = COUNT(*)
		FROM
			dbo.[CompanyContacts] A
		WHERE
			A.[CompanyID] = @CompanyID;

		if (@Count = 0 AND @IsPrimaryContact = 0)
			BEGIN
				SET @IsPrimaryContact = 1;
			END

		if (@IsPrimaryContact = 1)
			BEGIN
				UPDATE
					dbo.[CompanyContacts]
				SET
					[PrimaryContact] = 0
				WHERE
					[CompanyID] = @CompanyID;
			END

		if (@ID = NULL OR @ID = 0)
			BEGIN
				INSERT INTO
					dbo.[CompanyContacts]
					([CompanyID], [ContactPrefix], [ContactFirstName], [ContactMiddleInitial], [ContactLastName], [ContactSuffix], [CompanyLocationID], [ContactEmailAddress], [ContactPhone], 
					[ContactPhoneExtension], [ContactFax], [Designation], [Department], [Role], [Notes], [PrimaryContact], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate])
				VALUES
					(@CompanyID, @Prefix, @FirstName, @MiddleInitial, @LastName, @Suffix, @CompanyLocationID, @Email, @Phone, @Extension, @Fax, @Designation, @Department, @Role, @ContactNotes, 
					@IsPrimaryContact, @User, GETDATE(), @User, GETDATE());
			END
		else
			BEGIN
				UPDATE
					dbo.[CompanyContacts]
				SET
					[ContactPrefix] = @Prefix,
					[ContactFirstName] = @FirstName,
					[ContactMiddleInitial] = @MiddleInitial,
					[ContactLastName] = @LastName,
					[ContactSuffix] = @Suffix,
					[CompanyLocationID] = @CompanyLocationID,
					[ContactEmailAddress] = @Email,
					[ContactPhone] = @Phone,
					[ContactPhoneExtension] = @Extension,
					[ContactFax] = @Fax,
					[Designation] = @Designation,
					[Department] = @Department,
					[Role] = @Role,
					[Notes] = @ContactNotes,
					[PrimaryContact] = @IsPrimaryContact,
					[UpdatedBy] = @User,
					[UpdatedDate] = GETDATE()
				WHERE
					[ID] = @ID;
			END

		/* Company Locations */
		DECLARE @return varchar(max);

		SELECT @return = 
		(SELECT
			A.[ID], A.[CompanyID], ISNULL(A.[ContactPrefix], '') [Prefix], A.[ContactFirstName] [FirstName], ISNULL(A.[ContactMiddleInitial], '') [MiddleInitial],  A.[ContactLastName] [LastName], 
			ISNULL(A.[ContactSuffix], '') AS Suffix, A.[CompanyLocationID] [LocationID], B.[StreetName], B.[City], B.[StateID], C.[State], B.[ZipCode], A.[ContactEmailAddress] [EmailAddress], A.[ContactPhone] [Phone], 
			ISNULL(A.[ContactPhoneExtension], '') [Extension], ISNULL(A.[ContactFax], '') [Fax], ISNULL(A.[Designation], '') [Title], ISNULL(A.[Department], '') AS Department, A.[Role] [RoleID], D.[RoleName] [Role], 
			D.[RoleDescription] [RoleName], ISNULL(UPPER(A.[CreatedBy]), 'ADMIN') [CreatedBy], ISNULL(A.[CreatedDate], GETDATE()) [CreatedDate], ISNULL(UPPER(A.[UpdatedBy]), 'ADMIN') [UpdatedBy], 
			ISNULL(A.[UpdatedDate], GETDATE()) [UpdatedDate], ISNULL(A.[Notes], '') [Notes], [CompanyName]
		FROM
			dbo.[CompanyContacts] A INNER JOIN dbo.[CompanyLocations] B ON A.[CompanyLocationID] = B.[ID]
			INNER JOIN dbo.[State] C ON B.[StateID] = C.[ID]
			INNER JOIN dbo.[Roles] D ON A.[Role] = D.[ID]
			INNER JOIN dbo.[Companies] E ON A.[CompanyID] = E.[ID]
			AND A.[CompanyID] = @CompanyID
		ORDER BY
			A.[PrimaryContact]
		FOR JSON PATH);
	
		SELECT @return;

        COMMIT TRANSACTION;

    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH

END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [SaveCompanyDocuments]
	@ID int = NULL,
	@CompanyID int = 11600,
	@DocumentName varchar(255),
	@OriginalFileName varchar(255),
	@InternalFileName varchar(50),
	@Notes varchar(2000),
	@UpdatedDate datetime = NULL,
	@User varchar(10)
as
BEGIN;
	SET NOCOUNT ON;
	SET ARITHABORT ON;
	SET ANSI_NULLS ON;
	SET QUOTED_IDENTIFIER ON;

    BEGIN TRY
        BEGIN TRANSACTION;
		DECLARE @Description varchar(7000), @Action varchar(30);

		if (@ID IS NULL OR @ID = 0) --insert
			BEGIN;
				INSERT INTO 
					[dbo].[CompanyDocuments]
					([CompanyId], [DocumentName], [OriginalFileName], [Notes], [InternalFileName], [UpdatedDate], [UpdatedBy])
				VALUES
					(@CompanyID, @DocumentName, @OriginalFileName, @Notes, @InternalFileName, CASE WHEN @UpdatedDate IS NULL THEN GETDATE() ELSE @UpdatedDate END, @User);

				SET @Description = 'Added Document for [ID: ' + CAST(@CompanyID as varchar(10)) + '], Document Name: ' + @DocumentName;
								
				SET @Action = 'Add Candidate Document';
			END;
		else -- update
			BEGIN;
				UPDATE 
					[dbo].[CompanyDocuments]
				SET 
					[CompanyID] = @CompanyID,
					[DocumentName] = @DocumentName,
					[OriginalFileName] = CASE WHEN @OriginalFileName IS NULL THEN [OriginalFileName] ELSE @OriginalFileName END,
					[Notes] = @Notes,
					[UpdatedDate] = CASE WHEN @UpdatedDate IS NULL THEN GETDATE() ELSE @UpdatedDate END,
					[UpdatedBy] = @User
				WHERE 
					[ID] = @ID;

				SET @Description = 'Updated Document for [ID: ' + CAST(@CompanyID as varchar(10)) + '], Document Name: ' + @DocumentName;
								
				SET @Action = 'Update Candidate Document';
			END;
	
		exec dbo.AddAuditTrail @Action, 'Candidate Document', @Description, @User; 

		exec dbo.[GetCompanyDocuments] @CompanyID;

        COMMIT TRANSACTION;

    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [SaveCompanyLocation]
	@ID int = 0,
	@CompanyID int = 284,
	@StreetName varchar(500) = '128, Arlington Dr',
	@City varchar(100) = 'Livingstone',
	@StateID tinyint = 8,
	@ZipCode varchar(10) = '91356',
	@CompanyEmail varchar(255) = 'arlington@delwestusa.com',
	@Phone varchar(20) = '6812975700',
	@Extension varchar(10) = '',
	@Fax varchar(20) = '',
	@LocationNotes varchar(2000) = 'Some Notes',
	@IsPrimaryLocation bit = 0,
	@User varchar(10) = 'ADMIN'
AS
BEGIN
	SET NOCOUNT ON;
	SET ARITHABORT ON;
	SET ANSI_NULLS ON;
	SET QUOTED_IDENTIFIER ON;

    BEGIN TRY
        BEGIN TRANSACTION;

		DECLARE @Count int = 0;

		SELECT
			@Count = COUNT(*)
		FROM
			dbo.[CompanyLocations] A
		WHERE
			A.[CompanyID] = @CompanyID;

		if (@Count = 0 AND @IsPrimaryLocation = 0)
			BEGIN
				SET @IsPrimaryLocation = 1;
			END

		if (@IsPrimaryLocation = 1)
			BEGIN
				UPDATE
					dbo.[CompanyLocations]
				SET
					[IsPrimaryLocation] = 0
				WHERE
					[CompanyID] = @CompanyID;
			END

		if (@ID = NULL OR @ID = 0)
			BEGIN
			
				INSERT INTO
					dbo.[CompanyLocations]
					([CompanyID], [StreetName], [City], [StateID], [ZipCode], [CompanyEmail], [Phone], [Extension], [Fax], [Notes], [IsPrimaryLocation], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate])
				VALUES
					(@CompanyID, @StreetName, @City, @StateID, @ZipCode, @CompanyEmail, @Phone, @Extension, @Fax, @LocationNotes, @IsPrimaryLocation, @User, GETDATE(), @User, GETDATE());
			END
		else
			BEGIN
				UPDATE
					dbo.[CompanyLocations]
				SET
					[StreetName] = @StreetName,
					[City] = @City,
					[StateID] = @StateID,
					[ZipCode] = @ZipCode,
					[CompanyEmail] = @CompanyEmail,
					[Phone] = @Phone,
					[Extension] = @Extension,
					[Fax] = @Fax,
					[Notes] = @LocationNotes,
					[IsPrimaryLocation] = @IsPrimaryLocation,
					[UpdatedBy] = @User,
					[UpdatedDate] = GETDATE()
				WHERE
					[ID] = @ID;
			END

		/* Company Locations */
		DECLARE @return varchar(max);

		SELECT @return = (
			SELECT
				A.[ID], A.[CompanyID], A.[StreetName], A.[City], A.[StateID], ISNULL(B.[State], '') [State], A.[ZipCode], A.[CompanyEmail] [EmailAddress], A.[Phone], ISNULL(A.[Extension], '') [Extension], ISNULL(A.[Fax], '') [Fax],
				A.[IsPrimaryLocation] [PrimaryLocation], ISNULL(A.[Notes], '') [Notes], ISNULL(A.[CreatedBy], 'ADMIN') [CreatedBy], ISNULL(A.[CreatedDate], GETDATE()) [CreatedDate], ISNULL(A.[UpdatedBy], 'ADMIN') [UpdatedBy], 
				ISNULL(A.[UpdatedDate], GETDATE()) [UpdatedDate], C.[CompanyName]
			FROM
				dbo.[CompanyLocations] A LEFT JOIN [ProfessionalMaster].dbo.[State] B ON A.[StateID] = B.[ID]		
				INNER JOIN dbo.[Companies] C ON A.[CompanyID] = C.[ID]
			WHERE A.[CompanyID] = @CompanyID
			ORDER BY
				A.[IsPrimaryLocation] DESC
			FOR JSON PATH);

		SELECT @return;

        COMMIT TRANSACTION;

    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH

END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   procedure [SaveEducation]
	@Id int = null,
	@CandidateId int,
	@Degree varchar(100),
	@College varchar(255),
	@State varchar(100),
	@Country varchar(100),
	@Year varchar(50),
	@User varchar(10)
AS
BEGIN
	SET NOCOUNT ON;
	SET ARITHABORT ON;
	SET ANSI_NULLS ON;
	SET QUOTED_IDENTIFIER ON;

    BEGIN TRY
        BEGIN TRANSACTION;

		DECLARE @Name varchar(100), @Description varchar(7000), @Action varchar(35);
		SELECT
			@Name = A.[FirstName] + ' ' + A.[LastName]
		FROM
			[dbo].[Candidate] A
		WHERE
			A.[Id] = @CandidateId;

		if (@Id IS NULL OR @Id = 0)
			BEGIN
				INSERT INTO
					dbo.[CandidateEducation]
					([CandidateId], [Degree], [College], [State], [Country], [Year], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy])
				VALUES
					(@CandidateId, @Degree, @College, @State, @Country, @Year, GETDATE(), @User, GETDATE(), @User);
			
				SET @Description = 'Inserted Education for ' + @Name + ', [ID: ' + CAST(@CandidateId as varchar(10)) + '], Degree: ' 
									+ @Degree + ', College: ' + @College + ', State: ' + @State + ', Country: ' + @Country 
									+ ', Year:' + ISNULL(@Year, '');
								
				SET @Action = 'Insert Candidate Education';
			END
		else
			BEGIN
				UPDATE
					dbo.[CandidateEducation]
				SET
					[Degree] = @Degree,
					[College] = @College,
					[State] = @State,
					[Country] = @Country,
					[Year] = @Year,
					[UpdatedBy] = @User,
					[UpdatedDate] = GETDATE()
				WHERE
					[Id] = @Id;
			
				SET @Description = 'Updated Education for ' + @Name + ', [ID: ' + CAST(@CandidateId as varchar(10)) + '], Experience ID:' 
									+ CAST(@Id as varchar(10)) + ', Degree: ' + @Degree + ', College: ' + @College 
									+ ', State: ' + @State + ', Country: ' + @Country + ', Year:' + ISNULL(@Year, '');
								
				SET @Action = 'Update Candidate Education';
			END
	
		exec dbo.AddAuditTrail @Action, 'Candidate Details', @Description, @User; 

		DECLARE @return varchar(max) = '';
		
		SELECT @return = 
		(SELECT
			A.[Id], A.[Degree], A.[College], ISNULL(A.[State], '') [State], A.[Country], A.[Year], A.[UpdatedBy]
		FROM
			dbo.[CandidateEducation] A
		WHERE
			A.[CandidateId] = @CandidateId
		ORDER BY
			A.[UpdatedDate] DESC
		FOR JSON AUTO);
		
		SELECT ISNULL(@return, '[]');

        COMMIT TRANSACTION;

    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   procedure [SaveExperience]
	@Id int = null,
	@CandidateId int,
	@Employer varchar(100),
	@Start varchar(10),
	@End varchar(10),
	@Location varchar(100),
	@Description varchar(max),
	@User varchar(10),
	@Title varchar(255) = ''
AS
BEGIN
	SET NOCOUNT ON;
	SET ARITHABORT ON;
	SET ANSI_NULLS ON;
	SET QUOTED_IDENTIFIER ON;

    BEGIN TRY
        BEGIN TRANSACTION;

		DECLARE @Name varchar(100), @Desc varchar(7000), @Action varchar(35);
		SELECT
			@Name = A.[FirstName] + ' ' + A.[LastName]
		FROM
			[dbo].[Candidate] A
		WHERE
			A.[Id] = @CandidateId;

		if (@Id IS NULL OR @Id = 0)
			BEGIN
				INSERT INTO
					dbo.[CandidateEmployer]
					([CandidateId], [Employer], [Start], [End], [Location], [Description], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [Title])
				VALUES
					(@CandidateId, @Employer, @Start, @End, @Location, @Description, GETDATE(), @User, GETDATE(), @User, @Title);
			
				SET @Desc = 'Inserted Experience for ' + @Name + ', [ID: ' + CAST(@CandidateId as varchar(10)) + '], Employer: ' 
									+ @Employer + ', Start: ' + @Start + ', End: ' + @End + ', Location: ' + @Location;
								
				SET @Action = 'Insert Candidate Experience';
			END
		else
			BEGIN
				UPDATE
					dbo.[CandidateEmployer]
				SET
					[Employer] = @Employer,
					[Start] = @Start,
					[End] = @End,
					[Location] = @Location,
					[Description] = @Description,
					[UpdatedBy] = @User,
					[UpdatedDate] = GETDATE(),
					[Title] = @Title
				WHERE
					[Id] = @Id;
			
				SET @Desc = 'Updated Experience for ' + @Name + ', [ID: ' + CAST(@CandidateId as varchar(10)) + '], Experience ID:' 
									+ CAST(@Id as varchar(10)) + ', Employer: ' + @Employer + ', Start: ' + @Start + ', End: ' + @End 
									+ ', Location: ' + @Location;
								
				SET @Action = 'Update Candidate Experience';
			END
	
		exec dbo.AddAuditTrail @Action, 'Candidate Details', @Description, @User; 
		
		DECLARE @return varchar(max);
		SELECT @return = 
		(SELECT
			A.[Id], A.[Employer], A.[Start], A.[End], A.[Location], ISNULL(REPLACE(REPLACE(REPLACE(A.[Description], CHAR(13) + CHAR(10), '<br/>'), CHAR(13), '<br/>'), CHAR(10), '<br/>'), '') [Description], 
			A.[UpdatedBy], A.[Title]
		FROM
			dbo.[CandidateEmployer] A
		WHERE
			A.CandidateId = @CandidateId
		ORDER BY
			A.[UpdatedDate] DESC
		FOR JSON AUTO);

		SELECT ISNULL(@return, '');

        COMMIT TRANSACTION;

    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
		
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   procedure [SaveNote]
	@Id int = null,
	@CandidateId int,
	@Note varchar(max),
	@IsPrimary bit = 0,
	@EntityType varchar(5) = 'CND',
	@User varchar(10)
AS
BEGIN
	SET NOCOUNT ON;
	SET ARITHABORT ON;
	SET ANSI_NULLS ON;
	SET QUOTED_IDENTIFIER ON;

    BEGIN TRY
        BEGIN TRANSACTION;

		DECLARE @Name varchar(100) = '', @Description varchar(7000) = '', @Action varchar(30) = '';
		if (@EntityType = 'CND')
			BEGIN
				SELECT
					@Name = A.[FirstName] + ' ' + A.[LastName]
				FROM
					[dbo].[Candidate] A
				WHERE
					A.[Id] = @CandidateId;
			END
		else if (@EntityType = 'REQ')
			BEGIN
				SELECT
					@Name = A.[PosTitle]
				FROM
					[dbo].[Requisitions] A
				WHERE
					A.[Id] = @CandidateId;
			END
	
		if (@Id IS NULL OR @Id = 0)
			BEGIN
				INSERT INTO
					dbo.[Notes]
					([EntityId], [EntityType], [Notes], [IsPrimary], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate])
				VALUES
					(@CandidateId, @EntityType, @Note, 0, @User, GETDATE(), @User, GETDATE());

				SET @Id = SCOPE_IDENTITY();
			
				SET @Description = 'Inserted Note for ' + @Name + ', [ID: ' + CAST(@CandidateId as varchar(10)) + '], Note: ' 
									+ LEFT(@Note, 3000) + '...';

				SET @Action = 'Insert ' + @EntityType + ' Note';
			END
		else
			BEGIN
				UPDATE
					dbo.[Notes]
				SET
					[Notes] = @Note,
					[UpdatedBy] = @User,
					[UpdatedDate] = GETDATE()
				WHERE
					[Id] = @Id;
			
				SET @Description = 'Updated Note for ' + @Name + ', [ID: ' + CAST(@CandidateId as varchar(10)) + '],  Note: ' 
									+ LEFT(@Note, 3000) + '...';
										
				SET @Action = 'Update ' + @EntityType + ' Note';
			END

		if (@IsPrimary = 1)
			BEGIN
				UPDATE
					dbo.[Notes]
				SET
					[IsPrimary] = 0
				WHERE
					[EntityId] = @CandidateId
					AND [EntityType] = 'CND';

				UPDATE
					dbo.[Notes]
				SET
					[IsPrimary] = 1
				WHERE
					[Id] = @Id;
			END
	
		exec dbo.AddAuditTrail @Action, 'Candidate Notes', @Description, @User; 
	
		DECLARE @return varchar(max);

		SELECT @return = 
			(SELECT
				A.[Id], A.[UpdatedDate], A.[UpdatedBy], 
				REPLACE(REPLACE(REPLACE(A.[NOTES], CHAR(13) + CHAR(10), '<br/>'), CHAR(13), '<br/>'), CHAR(10), '<br/>') [NOTES]
			FROM
				[dbo].[Notes] A
			WHERE
				A.[EntityId] = @CandidateId
				AND A.[EntityType] = @EntityType
			ORDER BY
				A.[IsPrimary] DESC, A.[UpdatedDate] DESC
			FOR JSON AUTO);

		SELECT ISNULL(@return, '[]');

        COMMIT TRANSACTION;

    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [SaveRequisition]
	@RequisitionId int = 1321 OUTPUT,
	@Company int=150,
	@HiringMgr int=77,
	@City varchar(50)='Pittsburgh',
	@StateId int=1,
	@Zip varchar(10)=15212,
	@IsHot tinyint=2,
	@Title varchar(200)='PROCUREMENT OPERATIONS BUSINESS UNIT SUPPORT (ARCONIC ENGINES) ',
	@Description varchar(max)='<p>Industry: Manufacturing &amp; Production
Job Category: Materials Management - Purchasing / Inventory / Buyers
Arconic is looking for Business Unit (BU) Procurement Operations Support to report to the Plant Procurement Lead and supports the Rochester Plant Procurement Team, Category Management Team and BU Procurement Engineers on relevant Procurement activities, measures and operational activities in the execution of procurement strategies. The overall goal is the best possible support of the BU Plant Procurement team to achieve the best cost position of the respective Business Unit plant.  He/she supports the optimal collaboration of BU Plant Procurement with BU Procurement Engineering as well as with Global Category Management and Regional Procurement to achieve best procurement results. 
  
Performance areas: Cost Savings (SRT), Supplier performance, challenging of material &amp; service specifications (demand management) 
  
Key Activities: 
Assists on benchmarking efforts, should-be-cost analyses and business cases
</p>',
	@Positions int=500,
	@ExpStart datetime='2018-09-24',
	@Due datetime='2019-09-30',
	@AttachName varchar(255)='',
	@AttachFileType varchar(10)='',
	@AttachContentType varchar(100)='',
	@AttachName2 varchar(255)='',
	@AttachFileType2 varchar(10)='',
	@AttachContentType2 varchar(100)='',
	@AttachName3 varchar(255)='',
	@AttachFileType3 varchar(10)='',
	@AttachContentType3 varchar(100)='',
	@Education int=0,
	@Skills varchar(2000)='92',
	@JobOption char(1)='F',
	@ExperienceId int=5,
	@Eligibility int=0,
	@Duration varchar(50)='',
	@DurationCode char(1)='M',
	@ExpRateLow numeric(9,2)=0.0,
	@ExpRateHigh numeric(9,2)=0.0,
	@ExpLoadLow numeric(9,2)=0.0,
	@ExpLoadHigh numeric(9,2)=0.0,
	@SalLow numeric(9,2)=50000.0,
	@SalHigh numeric(9,2)=60000.0,
	@ExpPaid bit=0,
	@Status char(3)='NEW',
	@Security bit=0,
	@PlacementFee numeric(8,2)=0,
	@Benefits bit = 1,
	@BenefitsNotes varchar(max) = '',
	@BenefitsName varchar(255) = '',
	@BenefitsFileType varchar(10) = '',
	@BenefitsContentType varchar(100) = '',
	@OFCCP bit=0,
	@SetAlert bit = 0,
	@AlertFreq int = 0,
	@AlertEnd smalldatetime = NULL,
	@AlertMessage varchar(1000) = '',
	@AlertMail bit = 0,
	@User varchar(10) = 'JOLLY',
	@Assign varchar(550) = 'AGNES,DONB',
	@MandatoryRequirement varchar(8000) = '',
	@PreferredRequirement varchar(8000) = '',
	@OptionalRequirement varchar(8000) = ''
AS
BEGIN
	SET NOCOUNT ON;
	SET ARITHABORT ON;
	SET ANSI_NULLS ON;
	SET QUOTED_IDENTIFIER ON;

    BEGIN TRY
        BEGIN TRANSACTION;
		
		DECLARE @Desc varchar(7000), @Action varchar(40);
		DECLARE @IsAdd bit = 0;

		if (@AlertEnd IS NULL)
			BEGIN
				SET @AlertEnd = CAST(GETDATE() as smalldatetime);
			END

		if (@RequisitionId IS NULL OR @RequisitionId = 0) --INSERT
			BEGIN
				DECLARE @Code varchar(15) = '', @Count int;

				SELECT
					@Code = UPPER(LEFT(A.[FirstName], 1)) + UPPER(LEFT(A.[LastName], 1)) + CONVERT(varchar(10), GETDATE(), 112) + '-'
				FROM
					dbo.[Users] A
				WHERE
					A.[UserName] = @User;

				SELECT 
					@Count = COUNT(*) 
				FROM
					dbo.Requisitions A
				WHERE
					A.[Code] LIKE @Code + '%';

				SET @Code = @Code + RIGHT('000' + CAST(@Count + 1 as varchar(2)), 2);
				SET @IsAdd = 1;

				INSERT INTO
					dbo.[Requisitions]
					([Code], [CompanyId], [HiringMgrId], [PosTitle], [Description], [Positions], [Duration], [DurationCode], [ExperienceId], [ExpRateLow], 
					[ExpRateHigh], [ExpLoadLow], [ExpLoadHigh], [PlacementFee], [JobOption], [SalaryLow], [SalaryHigh], [ExpPaid], [ExpStart], [Status], 
					[AlertOn], [AlertTimeout], [AlertRepFreq], [AlertEnd], [AlertMsg], [AlertMail], [IsHot], [CreatedBy], [CreatedDate], [UpdatedBy],
					[UpdatedDate], [Attachments], [Attachments2], [Attachments3], [SkillsReq], [Education], [Eligibility], [SecurityClearance], [SecurityType],
					[Benefits], [BenefitsNotes], [BenefitsAttach], [OFCCP], [AttachName], [AttachFileType], [AttachContentType], [AttachName2], 
					[AttachFileType2], [AttachContentType2], [AttachName3], [AttachFileType3], [AttachContentType3], [BenefitName], [BenefitFileType], 
					[BenefitContentType], [Due], [City], [StateId], [Zip], [AssignedRecruiter], [MandatoryRequirement], [PreferredRequirement], [OptionalRequirement])
				VALUES
					(@Code, @Company, @HiringMgr, @Title, @Description, @PlacementFee, @Duration, @DurationCode, @ExperienceId, @ExpRateLow, 
					@ExpRateHigh, @ExpLoadLow, @ExpLoadHigh, @PlacementFee, @JobOption, @SalLow, @SalHigh, @ExpPaid, @ExpStart, 'NEW', 
					@SetAlert, 24, @AlertFreq, @AlertEnd, @AlertMessage, @AlertMail, @IsHot, @User, GETDATE(), @User,
					GETDATE(), CAST('' as varbinary(max)), CAST('' as varbinary(max)), CAST('' as varbinary(max)), @Skills, @Education, @Eligibility, @Security, 
					0, @Benefits, @BenefitsNotes, CAST('' as varbinary(max)), @OFCCP, @AttachName, @AttachFileType, @AttachContentType, @AttachName2, 
					@AttachFileType2, @AttachContentType2, @AttachName3, @AttachFileType3, @AttachContentType3, @BenefitsName, @BenefitsFileType, 
					@BenefitsContentType, @Due, @City, @StateId, @Zip, @Assign, @MandatoryRequirement, @PreferredRequirement, @OptionalRequirement);
				
				SET @RequisitionId = IDENT_CURRENT('Requisitions');
	
				SET @Action = 'Insert Requisition';
				SET @Desc = 'Inserted New Requisition: ' + @Title + ', [ID: ' + CAST(@RequisitionId as varchar(10));
			END
		else --UPDATE
			BEGIN
				UPDATE
					dbo.[Requisitions]
				SET
					[CompanyId] = @Company,
					[HiringMgrId] = @HiringMgr,
					[PosTitle] = @Title,
					[Description] = @Description, 
					[Positions] = @Positions, 
					[Duration] = @Duration, 
					[DurationCode] = @DurationCode, 
					[ExperienceId] = @ExperienceId, 
					[ExpRateLow] = @ExpRateLow, 
					[ExpRateHigh] = @ExpRateHigh, 
					[ExpLoadLow] = @ExpLoadLow, 
					[ExpLoadHigh] = @ExpLoadHigh, 
					[PlacementFee] = @PlacementFee, 
					[JobOption] = @JobOption, 
					[SalaryLow] = @SalLow, 
					[SalaryHigh] = @SalHigh, 
					[ExpPaid] = @ExpPaid, 
					[ExpStart] = @ExpStart, 
					[Status] = @Status, 
					[AlertOn] = @SetAlert, 
					[AlertTimeout] = 24, 
					[AlertRepFreq] = @AlertFreq, 
					[AlertEnd] = @AlertEnd, 
					[AlertMsg] = @AlertMessage, 
					[AlertMail] = @AlertMail, 
					[IsHot] = @IsHot, 
					[UpdatedBy] = @User,
					[UpdatedDate] = GETDATE(), 
					[SkillsReq] = @Skills, 
					[Education] = @Education, 
					[Eligibility] = @Eligibility, 
					[SecurityClearance] = @Security, 
					[SecurityType] = 0,
					[Benefits] = @Benefits, 
					[BenefitsNotes] = @BenefitsNotes, 
					[OFCCP] = @OFCCP, 
					[AttachName] = CASE WHEN @AttachName IS NULL THEN AttachName ELSE @AttachName END, 
					[AttachFileType] = CASE WHEN @AttachFileType IS NULL THEN AttachFileType ELSE @AttachFileType END, 
					[AttachContentType] = CASE WHEN @AttachContentType IS NULL THEN AttachFileType ELSE @AttachContentType END, 
					[AttachName2] = CASE WHEN @AttachName2 IS NULL THEN AttachName2 ELSE @AttachName2 END, 
					[AttachFileType2] = CASE WHEN @AttachFileType2 IS NULL THEN AttachFileType2 ELSE @AttachFileType2 END, 
					[AttachContentType2] = CASE WHEN @AttachContentType2 IS NULL THEN AttachFileType2 ELSE @AttachContentType2 END, 
					[AttachName3] = CASE WHEN @AttachName3 IS NULL THEN AttachName3 ELSE @AttachName3 END, 
					[AttachFileType3] = CASE WHEN @AttachFileType3 IS NULL THEN AttachFileType3 ELSE @AttachFileType3 END, 
					[AttachContentType3] = CASE WHEN @AttachContentType3 IS NULL THEN AttachFileType3 ELSE @AttachContentType3 END, 
					[BenefitName] = CASE WHEN @BenefitsName IS NULL THEN BenefitName ELSE @BenefitsName END, 
					[BenefitFileType] = CASE WHEN @BenefitsFileType IS NULL THEN BenefitFileType ELSE @BenefitsFileType END, 
					[BenefitContentType] = CASE WHEN @BenefitsContentType IS NULL THEN BenefitFileType ELSE @BenefitsContentType END, 
					[Due] = @Due, 
					[City] = @City, 
					[StateId] = @StateId, 
					[Zip] = @Zip,
					[AssignedRecruiter] = @Assign,
					[MandatoryRequirement] = @MandatoryRequirement,
					[PreferredRequirement] = @PreferredRequirement,
					[OptionalRequirement] = @OptionalRequirement
				WHERE
					[Id] = @RequisitionId;
	
				SET @Action = 'Update Requisition';
				SET @Desc = 'Updated Requisition: ' + @Title + ', [ID: ' + CAST(@RequisitionId as varchar(10));
			END
	
		exec dbo.AddAuditTrail @Action, 'Candidate Details', @Description, @User; 

		SELECT
			A.[Code]
		FROM
			dbo.[Requisitions] A
		WHERE
			A.[Id] = @RequisitionId;
	
		DECLARE @SendTo varchar(200);

		SELECT 
			@SendTo = A.[SendTo]
		FROM 
			dbo.[Templates] A
		WHERE
			A.[Action] = CASE @IsAdd WHEN 1 THEN 6 ELSE 7 END;

		SELECT
			A.[Cc], A.[Subject], A.[Template]
		FROM
			dbo.[Templates] A
		WHERE
			A.Action = CASE @IsAdd WHEN 1 THEN 6 ELSE 7 END; --Requisition Created or Updated
		
		execute dbo.[GetNotificationEmails] @RequisitionID, 0, @SendTo;

		if (@StateId IS NOT NULL AND @StateId > 0)
			BEGIN
				SELECT
					ISNULL(@City + ', ', '') + A.[Code]
				FROM
					dbo.[State] A
				WHERE
					A.[Id] = @StateId;
			END
		else
			BEGIN
				SELECT ''
			END

        COMMIT TRANSACTION;

    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [SaveRequisitionDocuments]
	@RequisitionDocId int = NULL,
	@RequisitionId int,
	@DocumentName varchar(255),
	@DocumentLocation varchar(255),
	@DocumentNotes varchar(2000),
	@InternalFileName varchar(50),
	@DocumentUpdatedDate datetime = NULL,
	@DocsUser varchar(10)
as
BEGIN;
	SET NOCOUNT ON;
	SET ARITHABORT ON;
	SET ANSI_NULLS ON;
	SET QUOTED_IDENTIFIER ON;

    BEGIN TRY
        BEGIN TRANSACTION;
		DECLARE @Description varchar(7000), @Action varchar(30);

		if (@RequisitionDocId IS NULL OR @RequisitionDocId = 0) --insert
			BEGIN;
				INSERT INTO 
					[dbo].[RequisitionDocument]
					([RequisitionId], [DocumentName], [DocumentLocation], [Notes], [InternalFileName], [LastUpdatedBy], [LastUpdatedDate])
				VALUES
					(@RequisitionId, @DocumentName, @DocumentLocation, @DocumentNotes, @InternalFileName, @DocsUser, CASE WHEN @DocumentUpdatedDate IS NULL THEN GETDATE() ELSE @DocumentUpdatedDate END);

				SET @Description = 'Added Document for [ID: ' + CAST(@RequisitionId as varchar(10)) + '], Document Name: ' + @DocumentName;
								
				SET @Action = 'Add Requisition Document';
			END;
		else -- update
			BEGIN;
				UPDATE 
					[dbo].[RequisitionDocument]
				SET 
					[RequisitionId] = @RequisitionId,
					[DocumentName] = @DocumentName,
					[DocumentLocation] = CASE WHEN @DocumentLocation IS NULL THEN [DocumentLocation] ELSE @DocumentLocation END,
					[Notes] = @DocumentNotes,
					[LastUpdatedDate] = CASE WHEN @DocumentUpdatedDate IS NULL THEN GETDATE() ELSE @DocumentUpdatedDate END,
					[LastUpdatedBy] = @DocsUser,
					[InternalFileName] = @InternalFileName
				WHERE 
					[RequisitionDocId] = @RequisitionDocId;

				SET @Description = 'Updated Document for [ID: ' + CAST(@RequisitionId as varchar(10)) + '], Document Name: ' + @DocumentName;
								
				SET @Action = 'Update Requisition Document';
			END;
	
		exec dbo.AddAuditTrail @Action, 'Requisition Document', @Description, @DocsUser; 

		exec dbo.[GetRequisitionDocuments] @RequisitionId;

        COMMIT TRANSACTION;

    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [SaveSkill]
	@EntitySkillId int = 19557,
	@Skill varchar(100)='C++',
	@CandidateId int=19,
	@LastUsed smallint=2023,
	@ExpMonth smallint=29,
	@User varchar(10)='JOLLY'
AS
BEGIN
	SET NOCOUNT ON;
	SET ARITHABORT ON;
	SET ANSI_NULLS ON;
	SET QUOTED_IDENTIFIER ON;

    BEGIN TRY
        BEGIN TRANSACTION;
	
		DECLARE @SkillId int;
	
		SELECT
			@SkillId = A.[Id]
		FROM
			dbo.[Skills] A
		WHERE
			A.[Skill] = @Skill;
		
		if (@SkillId IS NULL OR @SkillId = 0)
			BEGIN
				INSERT INTO
					dbo.[Skills]
					([Skill], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate])
				VALUES
					(@Skill, @User, GETDATE(), @User, GETDATE());
				
				SELECT
					@SkillId = IDENT_CURRENT('dbo.Skills');
			END

		DECLARE @Name varchar(100), @Description varchar(7000), @Action varchar(30);
		SELECT
			@Name = A.[FirstName] + ' ' + A.[LastName]
		FROM
			[dbo].[Candidate] A
		WHERE
			A.[Id] = @CandidateId;
	
		if (@EntitySkillId IS NULL OR @EntitySkillId = 0)
			BEGIN
				INSERT INTO
					dbo.[EntitySkills]
					([EntityId], [EntityType], [SkillId], [LastUsed], [ExpMonth], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate])
				VALUES
					(@CandidateId, 'CND', @SkillId, @LastUsed, @ExpMonth, @User, GETDATE(), @User, GETDATE());
			
				SET @Description = 'Inserted Skill for ' + @Name + ', [ID: ' + CAST(@CandidateId as varchar(10)) + '], Skill: ' 
									+ @Skill + ', Last Used: ' + CAST(@LastUsed as varchar(10)) + ', Experience: ' 
									+ CAST(@ExpMonth as varchar(10));
								
				SET @Action = 'Insert Candidate Skill';
			END
		else
			BEGIN
				UPDATE
					dbo.[EntitySkills]
				SET
					[SkillId] = @SkillId,
					[LastUsed] = @LastUsed,
					[ExpMonth] = @ExpMonth,
					[UpdatedBy] = @User,
					[UpdatedDate] = GETDATE()
				WHERE
					[Id] = @EntitySkillId;
			
				SET @Description = 'Updated Skill for ' + @Name + ', [ID: ' + CAST(@CandidateId as varchar(10)) + '], Skill ID:' 
									+ CAST(@EntitySkillId as varchar(10)) + ', Skill: ' + @Skill + ', Last Used: ' 
									+ CAST(@LastUsed as varchar(10)) + ', Experience: ' + CAST(@ExpMonth as varchar(10));
								
				SET @Action = 'Update Candidate Skill';
			END
	
		exec dbo.AddAuditTrail @Action, 'Candidate Details', @Description, @User; 
		

		DECLARE @return varchar(max);

		SELECT @return = 
			(SELECT
				A.[Id], B.[Skill], A.[LastUsed], A.[ExpMonth], A.[UpdatedBy]
			FROM
				dbo.[EntitySkills] A INNER JOIN dbo.[Skills] B ON A.[SkillId] = B.[Id]
			WHERE
				A.[EntityId] = @CandidateId
				AND A.[EntityType] = 'CND'
			ORDER BY
				A.[UpdatedDate] DESC
			FOR JSON PATH);

		SELECT ISNULL(@return, '[]');

        COMMIT TRANSACTION;

    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
		
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [SearchCandidates]
	@Name varchar(30) = 'ramachandran'
AS
BEGIN
	SET NOCOUNT ON;

    SET @Name = CASE WHEN LEN(@Name) = 0 THEN '%' ELSE '%' + @Name + '%' END;

	DECLARE @return varchar(max);

	SELECT @return = 
    (SELECT DISTINCT TOP 20 
        A.[FullName] [KeyValue], A.[FullName] [Text]
    FROM 
        dbo.[CandidateView] A
    WHERE 
        A.[FullName] LIKE @Name
    ORDER BY
        A.[FullName]
	FOR JSON AUTO);

	SELECT @return;
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [SearchCompanies]
	@Company varchar(30) = 'pro'
AS
BEGIN
	SET NOCOUNT ON;
	
    SET @Company = CASE WHEN LEN(@Company) = 0 THEN '%' ELSE '%' + @Company + '%' END;

	DECLARE @return varchar(max);

	SELECT @return = 
	(SELECT TOP 20 
		A.[CompanyName] [KeyValue], A.[CompanyName] [Text]
	FROM 
		dbo.[Companies] A
	WHERE 
		A.[CompanyName] LIKE @Company
	ORDER BY
		A.[CompanyName]
	FOR JSON AUTO);

	SELECT @return;
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [SearchRequisitions]
	@Requisition varchar(30) = 'data'
AS
BEGIN
	SET NOCOUNT ON;

    SET @Requisition = CASE WHEN LEN(@Requisition) = 0 THEN '%' ELSE '%' + @Requisition + '%' END;

	DECLARE @return varchar(max);

	SELECT @return = 
	(SELECT DISTINCT TOP 20
		A.[PosTitle] [KeyValue], A.[PosTitle] [Text]
	FROM
		dbo.[Requisitions] A
	WHERE
		A.[PosTitle] LIKE @Requisition
	ORDER BY
		A.[PosTitle]
	FOR JSON AUTO);

	SELECT @return;
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- Company: <Company,int,>
-- =============================================
CREATE PROCEDURE [SetCache]
	@Companies bit = 1,
	@CompanyContact bit = 1,
	@Designations bit = 1,
	@DocumentType bit = 1,
	@Education bit = 1,
	@Eligibility bit = 1,
	@Experience bit = 1,
	@JobOptions bit = 1,
	@LeadIndustry bit = 1,
	@LeadSource bit = 1,
	@LeadStatus bit = 1,
	@NAICS bit = 1,
	@Roles bit = 1,
	@Skills bit = 1,
	@States bit = 1,
	@Status bit = 1,
	@TaxTerms bit = 1,
	@Users bit = 1,
	@Workflow bit = 1,
	@ZipCodes bit = 1,
	@Preferences bit = 1
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	if (@Companies = 1)
		BEGIN
			exec dbo.[GetCompaniesList];
		END
	else
		BEGIN
			SELECT '' WHERE 1 = 2;
		END
	
	if (@CompanyContact = 1)
		BEGIN
			exec dbo.[GetCompanyContactsList];
		END
	else
		BEGIN
			SELECT '' WHERE 1 = 2;
		END

	if (@Designations = 1)
		BEGIN
			exec dbo.[GetDesignations];
		END
	else
		BEGIN
			SELECT '' WHERE 1 = 2;
		END

	if (@DocumentType = 1)
		BEGIN
			exec dbo.[GetDocumentType]
		END
	else
		BEGIN
			SELECT '' WHERE 1 = 2;
		END

	if (@Education = 1)
		BEGIN
			exec dbo.[GetEducation];
		END
	else
		BEGIN
			SELECT '' WHERE 1 = 2;
		END
	
	if (@Eligibility = 1)
		BEGIN
			exec dbo.[GetEligibility];
		END
	else
		BEGIN
			SELECT '' WHERE 1 = 2;
		END
	
	if (@Experience = 1)
		BEGIN
			exec dbo.[GetExperience];
		END
	else
		BEGIN
			SELECT '' WHERE 1 = 2;
		END
	
	if (@JobOptions = 1)
		BEGIN
			exec dbo.[GetJobOptions];
		END
	else
		BEGIN
			SELECT '' WHERE 1 = 2;
		END

	if (@LeadIndustry = 1)
		BEGIN
			exec dbo.[GetLeadIndustry];
		END
	else
		BEGIN
			SELECT '' WHERE 1 = 2;
		END

	if (@LeadSource = 1)
		BEGIN
			exec dbo.[GetLeadSource];
		END
	else
		BEGIN
			SELECT '' WHERE 1 = 2;
		END

	if (@LeadStatus = 1)
		BEGIN
			exec dbo.[GetLeadStatus];
		END
	else
		BEGIN
			SELECT '' WHERE 1 = 2;
		END

	if (@NAICS = 1)
		BEGIN
			exec dbo.[GetNAICS];
		END
	else
		BEGIN
			SELECT '' WHERE 1 = 2;
		END

	if (@Roles = 1)
		BEGIN
			exec dbo.[GetRoles];
		END
	else
		BEGIN
			SELECT '' WHERE 1 = 2;
		END
	
	if (@Skills = 1)
		BEGIN
			exec dbo.[GetSkills];
		END
	else
		BEGIN
			SELECT '' WHERE 1 = 2;
		END

	if (@States = 1)
		BEGIN
			exec dbo.[GetStates];
		END
	else
		BEGIN
			SELECT '' WHERE 1 = 2;
		END
	
	if (@Status = 1)
		BEGIN
			exec dbo.[GetStatus];
		END
	else
		BEGIN
			SELECT '' WHERE 1 = 2;
		END
	
	if (@TaxTerms = 1)
		BEGIN
			exec dbo.[GetTaxTerms];
		END
	else
		BEGIN
			SELECT '' WHERE 1 = 2;
		END
		
	if (@Users = 1)
		BEGIN
			exec dbo.[GetUsers];
		END
	else
		BEGIN
			SELECT '' WHERE 1 = 2;
		END

	if (@Workflow = 1)
		BEGIN
			exec dbo.[GetWorkflow];
		END
	else
		BEGIN
			SELECT '' WHERE 1 = 2;
		END

	if (@ZipCodes = 1)
		BEGIN
			exec dbo.[GetZipCodes];
		END
	else
		BEGIN
			SELECT '' WHERE 1 = 2;
		END

	if (@Preferences = 1)
		BEGIN
			exec dbo.[GetPreferences];
		END
	else
		BEGIN
			SELECT '' WHERE 1 = 2;
		END
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Narendra Kumaran Kadhirvelu
-- Create date: 08/11/2022 20:32
-- Description:	Submit existing Candidate to a Requisition
-- =============================================
CREATE PROCEDURE [SubmitCandidateRequisition]
	-- Add the parameters for the stored procedure here
	@RequisitionID int = 1665, 
	@CandidateID int = 11599,
	@Notes varchar(1000) = 'I am submitting this candidate for testing purposes only.',
	@RoleId varchar(2) = 'FD',
	@User varchar(10)='JOLLY'
AS
BEGIN
	SET NOCOUNT ON;
	SET ARITHABORT ON;
	SET ANSI_NULLS ON;
	SET QUOTED_IDENTIFIER ON;

    BEGIN TRY
        BEGIN TRANSACTION;

		DECLARE @Description varchar(7000), @Action varchar(40), @Name varchar(300) = '', @Requisition varchar(300);

		SELECT
			@Name = A.[FirstName] + ' ' + A.[LastName]
		FROM
			dbo.[Candidate] A
		WHERE
			A.[Id] = @CandidateID;

		SELECT
			@Requisition = A.[PosTitle]
		FROM
			dbo.[Requisitions] A
		WHERE
			A.[Id] = @RequisitionID;

		INSERT INTO
			dbo.[Submissions]
			([CandidateId], [RequisitionId], [Status], [Notes], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [StatusId], [ShowCalendar], [DateTime], [Type], [PhoneNumber], 
			[InterviewDetails], [Undone])
		VALUES
			(@CandidateID, @RequisitionID, 'PEN', @Notes, @User, GETDATE(), @User, GETDATE(), 1, 0, NULL, 'P', '', '', 0);

		SET @Action = 'Submit Candidate to Requisition';
		SET @Description = 'Submit Candidate: ' + @Name + ', [ID: ' + CAST(@CandidateId as varchar(10)) + '] for Requisition ' + @Requisition 
							+ ' - [ID: ' + CAST(@RequisitionId as varchar(10)) + ']';
	
		exec dbo.AddAuditTrail @Action, 'Candidate', @Description, @User; 

		--exec dbo.[GetCandidateSubmission] @CandidateId, @RoleId;

		SELECT
	 		A.[FirstName], A.[LastName], B.[Code], B.[PosTitle], ISNULL(A.[OriginalResume], '') [OriginalResume], ISNULL(A.[OriginalFileId], '') [OriginalFileID], ISNULL(A.[FormattedResume], '') [FormattedResume], 
			ISNULL(A.[FormattedFileId], '') [FormattedFileID], ISNULL(C.CompanyName, '')
		FROM
			dbo.[Candidate] A, dbo.[Requisitions] B LEFT JOIN dbo.Companies C ON B.CompanyId = C.Id
		WHERE 
			A.[ID] = @CandidateId AND B.[ID] = @RequisitionId

		DECLARE @SendTo varchar(200);

		SELECT
			@SendTo = A.[SendTo]
		FROM
			dbo.[Templates] A
		WHERE
			A.Action = 3; --Candidate Submission Updated

		SELECT
			A.[Cc], A.[Subject], A.[Template]
		FROM
			dbo.[Templates] A
		WHERE
			A.Action = 3; --Candidate Submission Updated

		UPDATE 
			Candidate
		SET
			[UpdatedDate] = GETDATE(),
			[UpdatedBy] = @User
		WHERE
			[ID] = @CandidateID;

		SELECT
			A.[FirstName] + ' ' + A.[LastName], A.[EmailAddress] 
		FROM
			dbo.[Users] A INNER JOIN dbo.[Roles] B ON A.[Role] = B.[Id] 
			AND B.[RoleName] IN (SELECT LTRIM(RTRIM(s)) from dbo.BigSplit(',', @SendTo))
			AND A.[Status] = 1

		UNION

		SELECT 
			A.[FirstName] + ' ' + A.[LastName], A.[EmailAddress] 
		FROM
			dbo.[Users] A INNER JOIN dbo.[Candidate] B ON (A.[UserName] = B.[CreatedBy] OR A.[UserName] = B.[UpdatedBy])
			AND A.[Status] = 1
		WHERE
			B.[Id] = @CandidateId
			AND CASE WHEN (CHARINDEX('Candidate Owner', @SendTo) > 0) THEN 1 ELSE 0 END = 1

		UNION

		SELECT 
			A.[FirstName] + ' ' + A.[LastName], A.[EmailAddress] 
		FROM
			dbo.[Users] A INNER JOIN dbo.[Requisitions] B ON (A.[UserName] = B.[CreatedBy] OR A.[UserName] = B.[UpdatedBy])
			AND A.[Status] = 1
		WHERE
			B.[Id] = @RequisitionId
			AND CASE WHEN (CHARINDEX('Requisition Owner', @SendTo) > 0) THEN 1 ELSE 0 END = 1

		UNION

		SELECT
			A.[FirstName] + ' ' + A.[LastName], A.[EmailAddress] 
		FROM
			dbo.[Users] A
		WHERE 
			A.[UserName] IN (SELECT LTRIM(RTRIM(s)) from dbo.BigSplit(',', (SELECT R.[AssignedRecruiter] FROM dbo.[Requisitions] R WHERE R.[Id] = @RequisitionId)))
			AND A.[Status] = 1
			AND CASE WHEN (CHARINDEX('Requisition Assigned', @SendTo) > 0) THEN 1 ELSE 0 END = 1;

        COMMIT TRANSACTION;

    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [UndoCandidateActivity]
	@Id int = 5513,
	@User varchar(10) = 'JOLLY',
	@CandScreen bit = 1,
	@RoleId char(2) = 'FD'
AS
BEGIN;
	SET NOCOUNT ON;
	SET ARITHABORT ON;
	SET ANSI_NULLS ON;
	SET QUOTED_IDENTIFIER ON;

    BEGIN TRY
        BEGIN TRANSACTION;

		DECLARE @Description varchar(7000), @Action varchar(40), @Name varchar(300) = '', @Requisition varchar(200), @OldStatus varchar(3);
		DECLARE @RequisitionId int, @CandidateId int, @OldId int, @UpdatedDate datetime, @Status varchar(3), @Notes varchar(1000);

		SELECT
			@RequisitionId = A.[RequisitionId],
			@CandidateId = A.[CandidateId],
			@UpdatedDate = A.[UpdatedDate],
			@Name = B.[FirstName] + ' ' + B.[LastName],
			@Requisition = C.[PosTitle],
			@Notes = A.[Notes],
			@Status = A.[Status]
		FROM
			dbo.[Submissions] A INNER JOIN dbo.[Candidate] B ON A.[CandidateId] = B.[Id]
			INNER JOIN dbo.[Requisitions] C ON A.[RequisitionId] = C.[Id]
		WHERE
			A.[Id] = @Id;

			print 'ID: ' + CAST(@Id as varchar(30));

		SELECT
			@OldId = A.[Id], @OldStatus = A.[Status]
		FROM 
			dbo.[Submissions] A 
		WHERE 
			A.[UpdatedDate] = (SELECT MAX(E.[UpdatedDate]) FROM dbo.[Submissions] E 
								WHERE E.[UpdatedDate] < @UpdatedDate 
								AND E.[CandidateId] = @CandidateId 
								AND E.[RequisitionId] = @RequisitionId
								AND E.[Undone] = 0);

				print 'Old ID: ' + CAST(ISNULL(@OldId, 0) as varchar(30));

		if (@OldId IS NOT NULL)
			BEGIN;
				DELETE FROM
					dbo.[Submissions]
				WHERE
					[Id] = @Id;

				UPDATE
					dbo.[Submissions]
				SET
					UpdatedDate = GETDATE()
				WHERE
					[Id] = @OldId;

				SET @Action = 'Undo Candidate Status';
				SET @Description = 'Undone Status for Candidate: ' + @Name + ', [ID: ' + CAST(@CandidateId as varchar(10)) 
									+ '] for Requisition ' + @Requisition + ' - [ID: ' + CAST(@RequisitionId as varchar(10)) + ']'
									+ ', Status = ' +  @OldStatus + ' - [Old Status: ' + @Status + ']';
	
				exec dbo.AddAuditTrail @Action, 'Candidate Screen', @Description, @User; 
			END;

		if (@CandScreen = 1)
			BEGIN;
				exec dbo.[GetCandidateSubmission] @CandidateId, @RoleId;
			END;
		else
			BEGIN;
				exec dbo.[GetRequisitionSubmission] @RequisitionId, @RoleId;
			END;

        COMMIT TRANSACTION;

    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [UpdateCandidateView] AS
BEGIN
    -- Clear the CandidateView table
    --TRUNCATE TABLE CandidateView;

    -- Insert the joined data into the CandidateView table
    INSERT INTO CandidateView
    SELECT 
        A.[Id], A.[MPC], A.[UpdatedDate], A.[FirstName], A.[LastName], A.[Phone1], A.[Email], A.[City], B.[Code], A.[UpdatedBy], A.[Status], A.[RateCandidate], A.[Keywords]
    FROM
        Professional.dbo.Candidate A 
        INNER JOIN ProfessionalMaster.dbo.State B ON A.[StateId] = B.[Id];
END;

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [UpdateResume]
	@CandidateID int,
	@InternalName varchar(50),
	@FileName varchar(255),
	@Type bit = 0, --0-Original, 1-Formatted
	@User varchar(10),
	@TextResume varchar(max)
AS
BEGIN
	SET NOCOUNT ON;
	SET ARITHABORT ON;
	SET ANSI_NULLS ON;
	SET QUOTED_IDENTIFIER ON;

	BEGIN TRY
		BEGIN TRANSACTION;

		if (EXISTS(SELECT * FROM dbo.Candidate WHERE ID = @CandidateID))
			BEGIN
				UPDATE
					dbo.Candidate
				SET
					OriginalFileId = CASE @Type WHEN 0 THEN @InternalName ELSE OriginalFileId END,
					OriginalResume = CASE @Type WHEN 0 THEN @FileName ELSE OriginalResume END,
					FormattedFileId = CASE @Type WHEN 1 THEN @InternalName ELSE FormattedFileId END,
					FormattedResume = CASE @Type WHEN 1 THEN @FileName ELSE FormattedResume END,
					TextResume = CASE WHEN @TextResume IS NULL OR TRIM(@TextResume) = '' THEN TextResume ELSE @TextResume END,
					UpdatedBy = @User,
					UpdatedDate = GETDATE()
				WHERE
					ID = @CandidateID;
			END

		COMMIT TRANSACTION;
	END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION;
		THROW;
	END CATCH
END;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [ValidateLogin]
	@User varchar(10)='JOLLY'
AS
BEGIN
	SET NOCOUNT ON;

	SELECT
		[UserName], [Password], [Salt], [Role]
	FROM
		dbo.Users A
	WHERE
		A.[UserName] = @User
		AND A.[Status] = 1;
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE TRIGGER [InsertCandidateTrigger_p]
   ON  [Candidate]
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
GO
ALTER TABLE [dbo].[Candidate] ENABLE TRIGGER [InsertCandidateTrigger_p]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE   TRIGGER [InsertRequisitionTrigger]
   ON  [Requisitions]
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
				@UpdatedBy varchar(10), @CreatedBy varchar(10), @DueDate smalldatetime, @Icon varchar(20), @IsHot tinyint, @SubmitCandidate bit, @CanUpdate bit, @ChangeStatus bit, @PriorityColor varchar(20),
				@AssignedRecruiter varchar(255), @RoleID int, @SubmissionCount int, @SubmissionCountAll int, @Created smalldatetime;

		DECLARE cur CURSOR LOCAL FAST_FORWARD FOR
		SELECT 
			I.ID, I.Code, I.PosTitle, B.CompanyName, I.JobOption, C.JobOptions, I.Status, D.Status, I.UpdatedDate, I.UpdatedBy, I.CreatedBy, I.Due, D.Icon, I.IsHot, 1, 1, 1, 
			CASE IsHot WHEN 2 THEN @High WHEN 1 THEN @Normal WHEN 0 THEN @Low END, I.AssignedRecruiter, 5, (SELECT COUNT(DISTINCT CandidateID) FROM Submissions S 
			WHERE S.RequisitionId = I.Id), I.CreatedDate
		FROM 
			INSERTED I INNER JOIN Companies B ON I.CompanyId=B.ID INNER JOIN JobOptions C ON I.JobOption=C.JobCode INNER JOIN dbo.StatusCode D ON I.Status=D.StatusCode AND D.AppliesTo='REQ' --State B ON I.StateID=B.ID

		-- Step 2: Open Cursor and fetch First Record
		OPEN cur;
		FETCH NEXT FROM cur INTO @ID, @Code, @Title, @Company, @JobOption, @JobOptions, @StatusCode, @Status, @Updated, @UpdatedBy, @CreatedBy, @DueDate, @Icon, @IsHot, @SubmitCandidate, @CanUpdate, @ChangeStatus, 
								@PriorityColor, @AssignedRecruiter, @RoleID, @SubmissionCount

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
							Created = @Created,
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
							SubmissionCount = @SubmissionCount
						WHERE
							ID = @ID;
					END
				else --Insert the Candidate
					BEGIN
						INSERT INTO
							dbo.RequisitionView
							(ID, Code, Title, Company, JobOption, JobOptions, StatusCode, [Status], Updated, UpdatedBy, CreatedBy, DueDate, Icon, IsHot, SubmitCandidate, CanUpdate, ChangeStatus, PriorityColor,
							AssignedRecruiter, RoleID, SubmissionCount, Created)
						VALUES
							(@ID, @Code, @Title, @Company, @JobOption, @JobOptions, @StatusCode, @Status, @Updated, @UpdatedBy, @CreatedBy, @DueDate, @Icon, @IsHot, @SubmitCandidate, @CanUpdate, @ChangeStatus, 
							@PriorityColor, @AssignedRecruiter, @RoleID, @SubmissionCount, @Created);
					END

				--Fetch Next record into the variables from Cursor
				FETCH NEXT FROM cur INTO @ID, @Code, @Title, @Company, @JobOption, @JobOptions, @StatusCode, @Status, @Updated, @UpdatedBy, @CreatedBy, @DueDate, @Icon, @IsHot, @SubmitCandidate, @CanUpdate, @ChangeStatus, 
										@PriorityColor, @AssignedRecruiter, @RoleID, @SubmissionCount, @Created;
			END

		--Step 3: Close the Cursor
		CLOSE cur;
		DEALLOCATE cur;
END
GO
ALTER TABLE [dbo].[Requisitions] ENABLE TRIGGER [InsertRequisitionTrigger]
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'0-Only assigned Recruiters, 1-All/Any Recruiters' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Preferences', @level2type=N'COLUMN',@level2name=N'AllRecruitersSubmitCandidate'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Can any Recruiter take ownership of Admin Candidates (1) or no (0)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Preferences', @level2type=N'COLUMN',@level2name=N'AdminCandidates'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Can any Recruiter take ownership of Admin Requisitions (1) or no (0)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Preferences', @level2type=N'COLUMN',@level2name=N'AdminRequisitions'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'0-Only Owner, 1-Any Sales Manager' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Preferences', @level2type=N'COLUMN',@level2name=N'ReqStatusChange'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'0-Owner only, 1-Any Recruiter' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Preferences', @level2type=N'COLUMN',@level2name=N'CandStatusChange'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'1-Only Owner SM and Assigned Recruiters, 2-Only SM and any Recruiters, 3-Any SM and any Recruiters' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Preferences', @level2type=N'COLUMN',@level2name=N'ChangeCandidateSubmissionStatus'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Number of records per page in the Grid' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Preferences', @level2type=N'COLUMN',@level2name=N'PageSize'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Should requisitions be sorted first on Priority' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Preferences', @level2type=N'COLUMN',@level2name=N'SortReqonPriority'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'This is actually priority, 0-Low, 1-Medium, 2-High' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Requisitions', @level2type=N'COLUMN',@level2name=N'IsHot'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Fetch from User' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RequisitionView_old', @level2type=N'COLUMN',@level2name=N'RoleID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "A"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 136
               Right = 208
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "B"
            Begin Extent = 
               Top = 6
               Left = 246
               Bottom = 136
               Right = 416
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
      Begin ColumnWidths = 9
         Width = 284
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'SkillsView'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'SkillsView'
GO
SET ARITHABORT ON
SET CONCAT_NULL_YIELDS_NULL ON
SET QUOTED_IDENTIFIER ON
SET ANSI_NULLS ON
SET ANSI_PADDING ON
SET ANSI_WARNINGS ON
SET NUMERIC_ROUNDABORT OFF
GO
CREATE SPATIAL INDEX [SIndx_SpatialTable_geography_col1] ON [ZipCodes]
(
	[GeogCol1]
)USING  GEOGRAPHY_AUTO_GRID 
WITH (
CELLS_PER_OBJECT = 16, PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
