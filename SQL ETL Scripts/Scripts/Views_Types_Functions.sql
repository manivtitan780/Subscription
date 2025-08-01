CREATE TYPE [CandEducation] AS TABLE(
	[Degree] [varchar](100) NULL,
	[College] [varchar](255) NULL,
	[State] [varchar](100) NULL,
	[Country] [varchar](100) NULL,
	[Year] [varchar](50) NULL
)

CREATE TYPE [CandEmployer] AS TABLE(
	[Employer] [varchar](100) NULL,
	[Start] [varchar](10) NULL,
	[End] [varchar](10) NULL,
	[Location] [varchar](100) NULL,
	[Title] [varchar](255) NULL,
	[Description] [varchar](max) NULL
)

CREATE TYPE [CandidateType] AS TABLE(
	[ID] [int] NOT NULL,
	[MPC] [bit] NULL,
	[UpdateddDate] [smalldatetime] NULL,
	[FirstName] [varchar](50) NULL,
	[LastName] [varchar](50) NULL,
	[Phone1] [varchar](15) NULL,
	[Email] [varchar](255) NULL,
	[City] [varchar](50) NULL,
	[StateId] [int] NULL,
	[ZipCode] [varchar](20) NULL,
	[UpdatedBy] [varchar](10) NULL,
	[Status] [char](3) NULL,
	[RateCandidate] [tinyint] NULL,
	[Keywords] [varchar](500) NULL,
	PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (IGNORE_DUP_KEY = OFF)
)

CREATE TYPE [CandSkills] AS TABLE(
	[Skill] [varchar](255) NULL,
	[LastUsed] [int] NULL,
	[Month] [int] NULL
)

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
RETURNS varchar(8000)
AS
BEGIN
	IF @input IS NULL 
	BEGIN
		--Just return NULL if input string is NULL
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
		--This loop will take care of reccuring white spaces
		WHILE CHARINDEX(SUBSTRING(@input,@ctr,1), @Delimiter) > 0
		BEGIN
			SET @output = @output + SUBSTRING(@input,@ctr,1)
			SET @ctr = @ctr + 1
		END

		IF ASCII(SUBSTRING(@input,@ctr,1)) BETWEEN @LOWER_CASE_a AND @LOWER_CASE_z
		BEGIN
			--Converting the first character to upper case
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

CREATE VIEW [SkillsView]
AS
SELECT        B.Skill, A.Id, A.EntityId
FROM            dbo.EntitySkills AS A INNER JOIN
                         dbo.Skills AS B ON A.SkillId = B.Id
WHERE        (A.EntityType = 'CND')
