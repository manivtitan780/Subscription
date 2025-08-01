CREATE   PROCEDURE [dbo].[AddAuditTrail]
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

CREATE   PROCEDURE [dbo].[Admin_CheckDocumentType]
	@ID int = 0,
	@Text varchar(100) = ''
AS
BEGIN
	SET NOCOUNT ON;

	if (@ID = NULL OR @ID = 0)
		BEGIN
			if (EXISTS(SELECT * FROM [ProfessionalMaster].dbo.[DocumentType] A WHERE A.[DocumentType] = @Text))
				SELECT CAST(1 as bit);
			else
				SELECT CAST(0 as bit);
		END
	else
		BEGIN
			if (EXISTS(SELECT * FROM [ProfessionalMaster].dbo.[DocumentType] A WHERE A.[DocumentType] = @Text AND A.[Id] <> @ID))
				SELECT CAST(1 as bit);
			else
				SELECT CAST(0 as bit);
		END
END

CREATE     PROCEDURE [dbo].[Admin_CheckEducation]
	@ID int = 0,
	@Text varchar(50) = ''
AS
BEGIN
	SET NOCOUNT ON;

	if (@ID = NULL OR @ID = 0)
		BEGIN
			if (EXISTS(SELECT * FROM [ProfessionalMaster].dbo.[Education] A WHERE A.[Education] = @Text))
				SELECT CAST(1 as bit);
			else
				SELECT CAST(0 as bit);
		END
	else
		BEGIN
			if (EXISTS(SELECT * FROM [ProfessionalMaster].dbo.[Education] A WHERE A.[Education] = @Text AND A.[Id] <> @ID))
				SELECT CAST(1 as bit);
			else
				SELECT CAST(0 as bit);
		END
END

CREATE     PROCEDURE [dbo].[Admin_CheckEligibility]
	@ID int = 0,
	@Text varchar(50) = ''
AS
BEGIN
	SET NOCOUNT ON;

	if (@ID = NULL OR @ID = 0)
		BEGIN
			if (EXISTS(SELECT * FROM [ProfessionalMaster].dbo.[Eligibility] A WHERE A.[Eligibility] = @Text))
				SELECT CAST(1 as bit);
			else
				SELECT CAST(0 as bit);
		END
	else
		BEGIN
			if (EXISTS(SELECT * FROM [ProfessionalMaster].dbo.[Eligibility] A WHERE A.[Eligibility] = @Text AND A.[Id] <> @ID))
				SELECT CAST(1 as bit);
			else
				SELECT CAST(0 as bit);
		END
END

CREATE     PROCEDURE [dbo].[Admin_CheckExperience]
	@ID int = 0,
	@Text varchar(50) = ''
AS
BEGIN
	SET NOCOUNT ON;

	if (@ID = NULL OR @ID = 0)
		BEGIN
			if (EXISTS(SELECT * FROM [ProfessionalMaster].dbo.[Experience] A WHERE A.[Experience] = @Text))
				SELECT CAST(1 as bit);
			else
				SELECT CAST(0 as bit);
		END
	else
		BEGIN
			if (EXISTS(SELECT * FROM [ProfessionalMaster].dbo.[Experience] A WHERE A.[Experience] = @Text AND A.[Id] <> @ID))
				SELECT CAST(1 as bit);
			else
				SELECT CAST(0 as bit);
		END
END

CREATE     procedure [dbo].[Admin_CheckJobCode]
	@Code char(1) = '9'
AS
BEGIN
	SET NOCOUNT ON;

	IF (EXISTS (SELECT * FROM [ProfessionalMaster].dbo.[JobOptions] A WHERE A.[JobCode] = @Code))
		BEGIN
			SELECT CAST(1 as bit);
		END
	ELSE
		BEGIN
			SELECT CAST(0 as bit);
		END
END

CREATE       PROCEDURE [dbo].[Admin_CheckJobOption]
	@Code char(1) = '',
	@Text varchar(50) = ''
AS
BEGIN
	SET NOCOUNT ON;

	if (@Code = NULL OR @Code = '')
		BEGIN
			if (EXISTS(SELECT * FROM [ProfessionalMaster].dbo.[JobOptions] A WHERE A.[JobOptions] = @Text))
				SELECT CAST(1 as bit);
			else
				SELECT CAST(0 as bit);
		END
	else
		BEGIN
			if (EXISTS(SELECT * FROM [ProfessionalMaster].dbo.[JobOptions] A WHERE A.[JobOptions] = @Text AND A.[JobCode] <> @Code))
				SELECT CAST(1 as bit);
			else
				SELECT CAST(0 as bit);
		END
END

CREATE     PROCEDURE [dbo].[Admin_CheckLeadIndustry]
	@ID int = 0,
	@Text varchar(50) = ''
AS
BEGIN
	SET NOCOUNT ON;

	if (@ID = NULL OR @ID = 0)
		BEGIN
			if (EXISTS(SELECT * FROM [ProfessionalMaster].dbo.[LeadIndustry] A WHERE A.[Industry] = @Text))
				SELECT CAST(1 as bit);
			else
				SELECT CAST(0 as bit);
		END
	else
		BEGIN
			if (EXISTS(SELECT * FROM [ProfessionalMaster].dbo.[LeadIndustry] A WHERE A.[Industry] = @Text AND A.[Id] <> @ID))
				SELECT CAST(1 as bit);
			else
				SELECT CAST(0 as bit);
		END
END

CREATE     PROCEDURE [dbo].[Admin_CheckLeadSource]
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

CREATE     PROCEDURE [dbo].[Admin_CheckLeadStatus]
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

CREATE       PROCEDURE [dbo].[Admin_CheckRole]
	@ID char(2) = 'FS',
	@Text varchar(50) = 'Administrator'
AS
BEGIN
	SET NOCOUNT ON;

	if (@ID = NULL OR @ID = '')
		BEGIN
			if (EXISTS(SELECT * FROM [ProfessionalMaster].dbo.[Roles] A WHERE A.[Role] = @Text))
				SELECT CAST(1 as bit);
			else
				SELECT CAST(0 as bit);
		END
	else
		BEGIN
			if (EXISTS(SELECT * FROM [ProfessionalMaster].dbo.[Roles] A WHERE A.[Role] = @Text AND A.[ID] <> @ID))
				SELECT CAST(1 as bit);
			else
				SELECT CAST(0 as bit);
		END
END

CREATE       procedure [dbo].[Admin_CheckRoleId]
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

CREATE       PROCEDURE [dbo].[Admin_CheckSkill]
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

CREATE         PROCEDURE [dbo].[Admin_CheckState]
	@Code char(2) = '',
	@Text varchar(50) = ''
AS
BEGIN
	SET NOCOUNT ON;

	if (@Code = NULL OR @Code = '')
		BEGIN
			if (EXISTS(SELECT * FROM [ProfessionalMaster].dbo.[State] A WHERE A.[State] = @Text))
				SELECT CAST(1 as bit);
			else
				SELECT CAST(0 as bit);
		END
	else
		BEGIN
			if (EXISTS(SELECT * FROM [ProfessionalMaster].dbo.[State] A WHERE A.[State] = @Text AND A.[Code] <> @Code))
				SELECT CAST(1 as bit);
			else
				SELECT CAST(0 as bit);
		END
END

CREATE     procedure [dbo].[Admin_CheckStateCode]
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

CREATE         PROCEDURE [dbo].[Admin_CheckStatus]
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

CREATE     procedure [dbo].[Admin_CheckStatusCode]
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

CREATE PROCEDURE [dbo].[Admin_CheckTaxTerm]
	@Code char(10) = '1',
	@Text varchar(50) = '1099'
AS
BEGIN
	SET NOCOUNT ON;

	if (@Code = NULL OR @Code = '')
		BEGIN
			if (EXISTS(SELECT * FROM [ProfessionalMaster].dbo.[TaxTerm] A WHERE A.[TaxTerm] = @Text))
				SELECT CAST(1 as bit);
			else
				SELECT CAST(0 as bit);
		END
	else
		BEGIN
			if (EXISTS(SELECT * FROM [ProfessionalMaster].dbo.[TaxTerm] A WHERE A.[TaxTerm] = @Text AND A.[TaxTermCode] <> @Code))
				SELECT CAST(1 as bit);
			else
				SELECT CAST(0 as bit);
		END
END

CREATE     procedure [dbo].[Admin_CheckTaxTermCode]
	@Code char(1) = '2'
AS
BEGIN
	SET NOCOUNT ON;

	IF (EXISTS (SELECT * FROM [ProfessionalMaster].dbo.[TaxTerm] A WHERE A.[TaxTermCode] = @Code))
		BEGIN
			SELECT CAST(1 as bit);
		END
	ELSE
		BEGIN
			SELECT CAST(0 as bit);
		END
END

CREATE PROCEDURE [dbo].[Admin_CheckTemplateName]
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

CREATE   PROCEDURE [dbo].[Admin_CheckTitle]
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

CREATE   PROCEDURE [dbo].[Admin_CheckUserName]
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

CREATE procedure [dbo].[Admin_DeleteCandidate]
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
						
	--execute [dbo].[AddAuditTrail] @Action, 'Admin Candidate', @Description, @User;

END

CREATE Procedure [dbo].[Admin_EnableDisableUser]
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

CREATE procedure [dbo].[Admin_GetDesignationDetails]
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

CREATE procedure [dbo].[Admin_GetDesignations]
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

CREATE   procedure [dbo].[Admin_GetDocumentTypes]
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

CREATE procedure [dbo].[Admin_GetEducation]
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

CREATE procedure [dbo].[Admin_GetEducationDetails]
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

CREATE procedure [dbo].[Admin_GetEligibility]
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

CREATE procedure [dbo].[Admin_GetEligibilityDetails]
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

CREATE procedure [dbo].[Admin_GetExperience]
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

CREATE procedure [dbo].[Admin_GetExperienceDetails]
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

CREATE PROCEDURE [dbo].[Admin_GetIndustries]
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

CREATE procedure [dbo].[Admin_GetJobOptionDetails]
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

CREATE   PROCEDURE [dbo].[Admin_GetJobOptions]
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

CREATE     procedure [dbo].[Admin_GetLeadSources]
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

CREATE     procedure [dbo].[Admin_GetLeadStatuses]
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

CREATE   procedure [dbo].[Admin_GetNAICS]
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

CREATE PROCEDURE [dbo].[Admin_GetRoleDetails]
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

CREATE   procedure [dbo].[Admin_GetRoles]
	@Filter varchar(100) = ''
as
BEGIN
	SET NOCOUNT ON;

	if (LTRIM(RTRIM(@Filter)) = '')
		SET @Filter = '%';
	else
		SET @Filter = '%' + @Filter + '%';

	DECLARE @return varchar(max);
--
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

CREATE procedure [dbo].[Admin_GetSkillDetails]
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

CREATE PROCEDURE [dbo].[Admin_GetSkills]
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

CREATE procedure [dbo].[Admin_GetStage]
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
CREATE procedure [dbo].[Admin_GetStageDetails]
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
CREATE procedure [dbo].[Admin_GetStateDetails]
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


CREATE   procedure [dbo].[Admin_GetStates]
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


CREATE Procedure [dbo].[Admin_GetStatus]
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


CREATE procedure [dbo].[Admin_GetStatusCodes]
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
CREATE Procedure [dbo].[Admin_GetStatusDetails]
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

CREATE procedure [dbo].[Admin_GetTaxTermDetails]
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

CREATE   procedure [dbo].[Admin_GetTaxTerms]
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

CREATE procedure [dbo].[Admin_GetTemplateDetails]
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

CREATE procedure [dbo].[Admin_GetTemplates]
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

create Procedure [dbo].[Admin_GetUserDetails]
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

CREATE   PROCEDURE [dbo].[Admin_GetUsers]
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
		--INNER JOIN dbo.[StatusCode] C ON A.[Status] = C.[StatusCode]
		--AND C.[AppliesTo] = 'USR'
	WHERE
		A.[FirstName] + ' ' + A.[LastName] LIKE @Filter OR A.[UserName] LIKE @Filter
	ORDER BY
		A.[FirstName] + ' ' + A.[LastName], A.[UpdatedDate] DESC, B.[RoleName] DESC, A.[Status]
	FOR JSON PATH);

	SELECT @return;
	
END

create   procedure [dbo].[Admin_GetVariableCommission]
as
BEGIN
	SET NOCOUNT ON;

	SELECT
		A.[Id], A.[NoofHours], A.[OverHeadCost], A.[W2TaxLoadingRate], A.[1099CostRate], A.[FTERateOffered]
	FROM
		dbo.[VariableCommission] A;
END

CREATE     PROCEDURE [dbo].[Admin_GetWorkflow] 
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
	);

	SELECT @return;

CREATE Procedure [dbo].[Admin_SaveDesignation]
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

CREATE   Procedure [dbo].[Admin_SaveDocumentType]
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

CREATE Procedure [dbo].[Admin_SaveEducation]
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

			--SET @Id = IDENT_CURRENT('Education');

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

CREATE Procedure [dbo].[Admin_SaveEligibility]
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

CREATE Procedure [dbo].[Admin_SaveExperience]
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

CREATE   Procedure [dbo].[Admin_SaveIndustry]
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

CREATE Procedure [dbo].[Admin_SaveJobOptions]
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

CREATE     Procedure [dbo].[Admin_SaveLeadSource]
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

CREATE     Procedure [dbo].[Admin_SaveLeadStatus]
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

CREATE   Procedure [dbo].[Admin_SaveNAICS]
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

create     Procedure [dbo].[Admin_SavePreferences]
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

CREATE Procedure [dbo].[Admin_SaveRole]
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

CREATE Procedure [dbo].[Admin_SaveSkill]
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

CREATE procedure [dbo].[Admin_SaveStage]
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

CREATE Procedure [dbo].[Admin_SaveState]
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

CREATE Procedure [dbo].[Admin_SaveStatusCode]
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

CREATE Procedure [dbo].[Admin_SaveTaxTerm]
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

CREATE Procedure [dbo].[Admin_SaveTemplate]
	@Id int = 2009,
	@TemplateName varchar(50) = 'Create Candidate',
	@Cc varchar(2000) = '',
	@Subject varchar(255)= 'Candidate {FullName} Created.',
	@Template varchar(max) = '',
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

CREATE   Procedure [dbo].[Admin_SaveUser]
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

create   Procedure [dbo].[Admin_SaveVariableCommission]
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

CREATE   Procedure [dbo].[Admin_SaveWorkflow]
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

CREATE Procedure [dbo].[Admin_SearchDesignation]
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

CREATE     Procedure [dbo].[Admin_SearchDocumentTypes]
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

CREATE   Procedure [dbo].[Admin_SearchEducation]
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

CREATE   Procedure [dbo].[Admin_SearchEligibility]
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

CREATE   Procedure [dbo].[Admin_SearchExperience]
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

CREATE   Procedure [dbo].[Admin_SearchIndustry]
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

CREATE   Procedure [dbo].[Admin_SearchJobOption]
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

CREATE   Procedure [dbo].[Admin_SearchLeadSource]
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

CREATE   PROCEDURE [dbo].[Admin_SearchLeadStatus]
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

CREATE   Procedure [dbo].[Admin_SearchNAICS]
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

CREATE   Procedure [dbo].[Admin_SearchRole]
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

CREATE   Procedure [dbo].[Admin_SearchSkill]
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

CREATE   Procedure [dbo].[Admin_SearchState]
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

CREATE     Procedure [dbo].[Admin_SearchStatusCode]
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

CREATE   Procedure [dbo].[Admin_SearchTaxTerm]
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

CREATE     Procedure [dbo].[Admin_SearchUser]
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

CREATE   Procedure [dbo].[Admin_SearchWorkflow]
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

CREATE procedure [dbo].[Admin_ToggleCandidateStatus]
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

CREATE procedure [dbo].[Admin_ToggleDesignationStatus]
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

CREATE procedure [dbo].[Admin_ToggleEducationStatus]
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

CREATE procedure [dbo].[Admin_ToggleEligibilityStatus]
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

CREATE procedure [dbo].[Admin_ToggleExperienceStatus]
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

CREATE   procedure [dbo].[Admin_ToggleIndustryStatus]
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

CREATE     procedure [dbo].[Admin_ToggleLeadSourceStatus]
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

CREATE     procedure [dbo].[Admin_ToggleLeadStatusStatus]
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

CREATE procedure [dbo].[Admin_ToggleSkillStatus]
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

CREATE procedure [dbo].[Admin_ToggleTaxTermStatus]
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

CREATE procedure [dbo].[Admin_ToggleTemplateStatus]
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

CREATE   Procedure [dbo].[Admin_ToggleUserStatus]
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

CREATE   PROCEDURE [dbo].[ChangeCandidateStatus]
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

CREATE   PROCEDURE [dbo].[ChangeCollationForAllColumns]
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

CREATE     PROCEDURE [dbo].[ChangeMPC]
    @CandidateId int = 19,
    @MPC bit = 1,
    @Notes varchar(255) = 'Some Valooable candidate',
    @From varchar(10) = 'MANI'
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @DateString varchar(19), @NewJsonEntry varchar(MAX), @ExistingJson varchar(MAX);

    SELECT @DateString = CONVERT(VARCHAR(19), GETDATE(), 126);

    SET @NewJsonEntry = '{"DateTime":"' + @DateString + '","Name":"' + @From + '","MPC":' + IIF(@MPC=1, 'true', 'false') + ',"Comment":"' + @Notes + '"}';

    SELECT 
		@ExistingJson = MPCNotes
    FROM 
		[dbo].[Candidate]
    WHERE 
		Id = @CandidateId;

    if (@ExistingJson IS NULL OR @ExistingJson = '' OR @ExistingJson = '[]')
		BEGIN
			SET @ExistingJson = '[' + @NewJsonEntry + ']';
		END
    else
		BEGIN
			SET @ExistingJson = JSON_MODIFY(@ExistingJson, 'append $', JSON_QUERY(@NewJsonEntry));
		END

    UPDATE 
		[dbo].[Candidate]
    SET
        [MPC] = @MPC,
        [MPCNotes] = @ExistingJson
    WHERE
        [Id] = @CandidateId;

	DECLARE @Name varchar(100);
	SELECT
		@Name = A.[FirstName] + ' ' + A.[LastName]
	FROM
		[dbo].[Candidate] A
	WHERE
		A.[Id] = @CandidateId;
		
	DECLARE @Description varchar(7000);
	SET @Description = 'Changed MPC for ' + @Name + ', [ID: ' + CAST(@CandidateId as varchar(10)) + '] to: ' + 
						IIF(@MPC = 1, 'Yes', 'No');

	execute [dbo].[AddAuditTrail] 'Update Candidate MPC', 'Candidate Details', @Description, @From;
	
	SELECT
		A.[MPCNotes]
	FROM
		dbo.[Candidate] A
	WHERE
		A.[Id] = @CandidateId;

END

CREATE   PROCEDURE [dbo].[ChangeRating]
    @CandidateId int = 19,
    @Rating tinyint = 1,
    @Notes varchar(255) = 'Some Goooooood candidate',
    @From varchar(10) = 'MANI'
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @DateString varchar(19), @NewJsonEntry varchar(MAX), @ExistingJson varchar(MAX);

    SELECT @DateString = CONVERT(VARCHAR(19), GETDATE(), 126);

    SET @NewJsonEntry = '{"DateTime":"' + @DateString + '","Name":"' + @From + '","Rating":' + CAST(@Rating AS NVARCHAR) + ',"Comment":"' + @Notes + '"}';

    SELECT 
		@ExistingJson = RateNotes
    FROM 
		[dbo].[Candidate]
    WHERE 
		Id = @CandidateId;

    if (@ExistingJson IS NULL OR @ExistingJson = '' OR @ExistingJson = '[]')
		BEGIN
			SET @ExistingJson = '[' + @NewJsonEntry + ']';
		END
    else
		BEGIN
			SET @ExistingJson = JSON_MODIFY(@ExistingJson, 'append $', JSON_QUERY(@NewJsonEntry));
		END

    UPDATE 
		[dbo].[Candidate]
    SET
        [RateCandidate] = @Rating,
        [RateNotes] = @ExistingJson
    WHERE
        [Id] = @CandidateId;

	DECLARE @Name varchar(100);
	SELECT
		@Name = A.[FirstName] + ' ' + A.[LastName]
	FROM
		[dbo].[Candidate] A
	WHERE
		A.[Id] = @CandidateId;
		
	DECLARE @Description varchar(7000);
	SET @Description = 'Changed Rating for ' + @Name + ', [ID: ' + CAST(@CandidateId as varchar(10)) + '] to: ' + 
						CAST(@Rating as varchar(3));

	execute [dbo].[AddAuditTrail] 'Update Candidate Rating', 'Candidate Details', @Description, @From;
	
	SELECT
		A.[RateNotes]
	FROM
		dbo.[Candidate] A
	WHERE
		A.[Id] = @CandidateId;
END

CREATE   PROCEDURE [dbo].[ChangeRequisitionStatus]
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

CREATE       PROCEDURE [dbo].[CheckEIN]
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

CREATE PROCEDURE [dbo].[DashboardAccountsManager]
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
    
    -- Generic Query for Dropdown
    SELECT UserName as KeyValue, FirstName + ' ' + LastName as Text
    FROM dbo.Users 
    WHERE UserName = @User
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
        WHERE S.Status IN ('INT', 'OEX', 'HIR')
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
    FOR JSON PATH;
    
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
    FOR JSON PATH;
    
    -- Clean up temp tables
    DROP TABLE #RequisitionMetrics;
    DROP TABLE #SubmissionMetrics;
END;

CREATE PROCEDURE [dbo].[DashboardAdmin]
AS
BEGIN
	SET NOCOUNT ON;
	SET ARITHABORT ON;
	SET ANSI_NULLS ON;
	SET QUOTED_IDENTIFIER ON;
    
    DECLARE @Today DATE = GETDATE();
    DECLARE @YearStart DATE = DATEFROMPARTS(YEAR(@Today), 1, 1);
    
    -- Generic Query for Dropdown
    SELECT UserName as KeyValue, FirstName + ' ' + LastName as Text
    FROM dbo.Users 
    WHERE Role = 5 AND Status = 1
    FOR JSON AUTO;
    
    -- Create temp table for submission data
    CREATE TABLE #SubmissionData (
        CandidateId INT,
        RequisitionId INT,
        Status CHAR(3) COLLATE Latin1_General_CI_AI,
        MaxCreatedDate DATETIME,
        CreatedBy VARCHAR(10) COLLATE Latin1_General_CI_AI
    );
    
    -- Populate temp table with submission data
    INSERT INTO #SubmissionData
    SELECT 
        S.CandidateId,
        S.RequisitionId,
        S.Status,
        MAX(S.CreatedDate) as MaxCreatedDate,
        R.CreatedBy
    FROM dbo.Submissions S INNER JOIN dbo.Requisitions R ON S.RequisitionId = R.ID
    WHERE S.Status IN ('INT', 'OEX', 'HIR')
    GROUP BY S.CandidateId, S.RequisitionId, S.Status, R.CreatedBy
    HAVING R.CreatedBy IS NOT NULL;
    
    -- Common date calculations
    DECLARE @StartDate7D DATE = DATEADD(DAY, -6, @Today);
    DECLARE @StartDateMTD DATE = DATEADD(DAY, 1, EOMONTH(@Today, -1));
    DECLARE @StartDateQTD DATE = DATEADD(QUARTER, DATEDIFF(QUARTER, 0, @Today), 0);
    DECLARE @StartDateHYTD DATE = CASE 
        WHEN MONTH(@Today) <= 6 THEN DATEFROMPARTS(YEAR(@Today), 1, 1)
        ELSE DATEFROMPARTS(YEAR(@Today), 7, 1)
    END;
    
    -- Common Users CTE
    WITH AllUsers AS (
        SELECT DISTINCT R.CreatedBy
        FROM dbo.Requisitions R INNER JOIN dbo.Users U ON R.CreatedBy = U.UserName AND U.Status = 1
        WHERE R.CreatedBy IS NOT NULL AND R.CreatedDate >= DATEADD(YEAR, -1, GETDATE())
    )
    
    -- SET 1 - Query 1: Requisitions Created (Conditional Aggregation)
    SELECT 
        U.CreatedBy as [User],
        SUM(CASE WHEN CAST(R.CreatedDate AS DATE) BETWEEN @StartDate7D AND @Today THEN 1 ELSE 0 END) as LAST7D_COUNT,
        SUM(CASE WHEN CAST(R.CreatedDate AS DATE) BETWEEN @StartDateMTD AND @Today THEN 1 ELSE 0 END) as MTD_COUNT,
        SUM(CASE WHEN CAST(R.CreatedDate AS DATE) BETWEEN @StartDateQTD AND @Today THEN 1 ELSE 0 END) as QTD_COUNT,
        SUM(CASE WHEN CAST(R.CreatedDate AS DATE) BETWEEN @StartDateHYTD AND @Today THEN 1 ELSE 0 END) as HYTD_COUNT,
        SUM(CASE WHEN CAST(R.CreatedDate AS DATE) BETWEEN @YearStart AND @Today THEN 1 ELSE 0 END) as YTD_COUNT
    FROM AllUsers U
    LEFT JOIN dbo.Requisitions R ON R.CreatedBy = U.CreatedBy
    GROUP BY U.CreatedBy
    ORDER BY U.CreatedBy
        FOR JSON PATH;

    
    -- SET 1 - Query 2: Active Requisitions Updated (Conditional Aggregation)
    WITH AllUsers AS (
        SELECT DISTINCT R.CreatedBy
        FROM dbo.Requisitions R INNER JOIN dbo.Users U ON R.CreatedBy = U.UserName AND U.Status = 1
        WHERE R.CreatedBy IS NOT NULL AND R.CreatedDate >= DATEADD(YEAR, -1, GETDATE())
    )
    SELECT 
        U.CreatedBy as [User],
        SUM(CASE WHEN CAST(R.UpdatedDate AS DATE) BETWEEN @StartDate7D AND @Today AND R.Status IN ('NEW', 'OPN', 'PAR') THEN 1 ELSE 0 END) as LAST7D_COUNT,
        SUM(CASE WHEN CAST(R.UpdatedDate AS DATE) BETWEEN @StartDateMTD AND @Today AND R.Status IN ('NEW', 'OPN', 'PAR') THEN 1 ELSE 0 END) as MTD_COUNT,
        SUM(CASE WHEN CAST(R.UpdatedDate AS DATE) BETWEEN @StartDateQTD AND @Today AND R.Status IN ('NEW', 'OPN', 'PAR') THEN 1 ELSE 0 END) as QTD_COUNT,
        SUM(CASE WHEN CAST(R.UpdatedDate AS DATE) BETWEEN @StartDateHYTD AND @Today AND R.Status IN ('NEW', 'OPN', 'PAR') THEN 1 ELSE 0 END) as HYTD_COUNT,
        SUM(CASE WHEN CAST(R.UpdatedDate AS DATE) BETWEEN @YearStart AND @Today AND R.Status IN ('NEW', 'OPN', 'PAR') THEN 1 ELSE 0 END) as YTD_COUNT
    FROM AllUsers U
    LEFT JOIN dbo.Requisitions R ON R.CreatedBy = U.CreatedBy
    GROUP BY U.CreatedBy
    ORDER BY U.CreatedBy
    FOR JSON PATH;

    
    -- SET 1 - Query 3: Face-to-Face Interviews (INT) (Conditional Aggregation)
    WITH AllUsers AS (
        SELECT DISTINCT R.CreatedBy
        FROM dbo.Requisitions R INNER JOIN dbo.Users U ON R.CreatedBy = U.UserName AND U.Status = 1
        WHERE R.CreatedBy IS NOT NULL AND R.CreatedDate >= DATEADD(YEAR, -1, GETDATE())
    )
    SELECT 
        U.CreatedBy as [User],
        SUM(CASE WHEN CAST(SD.MaxCreatedDate AS DATE) BETWEEN @StartDate7D AND @Today AND SD.Status = 'INT' THEN 1 ELSE 0 END) as LAST7D_COUNT,
        SUM(CASE WHEN CAST(SD.MaxCreatedDate AS DATE) BETWEEN @StartDateMTD AND @Today AND SD.Status = 'INT' THEN 1 ELSE 0 END) as MTD_COUNT,
        SUM(CASE WHEN CAST(SD.MaxCreatedDate AS DATE) BETWEEN @StartDateQTD AND @Today AND SD.Status = 'INT' THEN 1 ELSE 0 END) as QTD_COUNT,
        SUM(CASE WHEN CAST(SD.MaxCreatedDate AS DATE) BETWEEN @StartDateHYTD AND @Today AND SD.Status = 'INT' THEN 1 ELSE 0 END) as HYTD_COUNT,
        SUM(CASE WHEN CAST(SD.MaxCreatedDate AS DATE) BETWEEN @YearStart AND @Today AND SD.Status = 'INT' THEN 1 ELSE 0 END) as YTD_COUNT
    FROM AllUsers U
    LEFT JOIN #SubmissionData SD ON SD.CreatedBy = U.CreatedBy
    GROUP BY U.CreatedBy
    ORDER BY U.CreatedBy    FOR JSON PATH;

    
    -- SET 1 - Query 4: Offers Extended (OEX) (Conditional Aggregation)
    WITH AllUsers AS (
        SELECT DISTINCT R.CreatedBy
        FROM dbo.Requisitions R INNER JOIN dbo.Users U ON R.CreatedBy = U.UserName AND U.Status = 1
        WHERE R.CreatedBy IS NOT NULL AND R.CreatedDate >= DATEADD(YEAR, -1, GETDATE())
    )
    SELECT 
        U.CreatedBy as [User],
        SUM(CASE WHEN CAST(SD.MaxCreatedDate AS DATE) BETWEEN @StartDate7D AND @Today AND SD.Status = 'OEX' THEN 1 ELSE 0 END) as LAST7D_COUNT,
        SUM(CASE WHEN CAST(SD.MaxCreatedDate AS DATE) BETWEEN @StartDateMTD AND @Today AND SD.Status = 'OEX' THEN 1 ELSE 0 END) as MTD_COUNT,
        SUM(CASE WHEN CAST(SD.MaxCreatedDate AS DATE) BETWEEN @StartDateQTD AND @Today AND SD.Status = 'OEX' THEN 1 ELSE 0 END) as QTD_COUNT,
        SUM(CASE WHEN CAST(SD.MaxCreatedDate AS DATE) BETWEEN @StartDateHYTD AND @Today AND SD.Status = 'OEX' THEN 1 ELSE 0 END) as HYTD_COUNT,
        SUM(CASE WHEN CAST(SD.MaxCreatedDate AS DATE) BETWEEN @YearStart AND @Today AND SD.Status = 'OEX' THEN 1 ELSE 0 END) as YTD_COUNT
    FROM AllUsers U
    LEFT JOIN #SubmissionData SD ON SD.CreatedBy = U.CreatedBy
    GROUP BY U.CreatedBy
    ORDER BY U.CreatedBy    FOR JSON PATH;

    
    -- SET 1 - Query 5: Hires (HIR) (Conditional Aggregation)
    WITH AllUsers AS (
        SELECT DISTINCT R.CreatedBy
        FROM dbo.Requisitions R INNER JOIN dbo.Users U ON R.CreatedBy = U.UserName AND U.Status = 1
        WHERE R.CreatedBy IS NOT NULL AND R.CreatedDate >= DATEADD(YEAR, -1, GETDATE())
    )
    SELECT 
        U.CreatedBy as [User],
        SUM(CASE WHEN CAST(SD.MaxCreatedDate AS DATE) BETWEEN @StartDate7D AND @Today AND SD.Status = 'HIR' THEN 1 ELSE 0 END) as LAST7D_COUNT,
        SUM(CASE WHEN CAST(SD.MaxCreatedDate AS DATE) BETWEEN @StartDateMTD AND @Today AND SD.Status = 'HIR' THEN 1 ELSE 0 END) as MTD_COUNT,
        SUM(CASE WHEN CAST(SD.MaxCreatedDate AS DATE) BETWEEN @StartDateQTD AND @Today AND SD.Status = 'HIR' THEN 1 ELSE 0 END) as QTD_COUNT,
        SUM(CASE WHEN CAST(SD.MaxCreatedDate AS DATE) BETWEEN @StartDateHYTD AND @Today AND SD.Status = 'HIR' THEN 1 ELSE 0 END) as HYTD_COUNT,
        SUM(CASE WHEN CAST(SD.MaxCreatedDate AS DATE) BETWEEN @YearStart AND @Today AND SD.Status = 'HIR' THEN 1 ELSE 0 END) as YTD_COUNT
    FROM AllUsers U
    LEFT JOIN #SubmissionData SD ON SD.CreatedBy = U.CreatedBy
    GROUP BY U.CreatedBy
    ORDER BY U.CreatedBy    FOR JSON PATH;

    
    -- SET 1 - Query 6: OEX to HIR Ratio (Conditional Aggregation)
    WITH AllUsers AS (
        SELECT DISTINCT R.CreatedBy
        FROM dbo.Requisitions R INNER JOIN dbo.Users U ON R.CreatedBy = U.UserName AND U.Status = 1
        WHERE R.CreatedBy IS NOT NULL AND R.CreatedDate >= DATEADD(YEAR, -1, GETDATE())
    ),
    RatioCalculations AS (
        SELECT 
            U.CreatedBy as [User],
            -- 7D calculations
            SUM(CASE WHEN CAST(SD.MaxCreatedDate AS DATE) BETWEEN @StartDate7D AND @Today AND SD.Status = 'OEX' THEN 1 ELSE 0 END) as OEX_7D,
            SUM(CASE WHEN CAST(SD.MaxCreatedDate AS DATE) BETWEEN @StartDate7D AND @Today AND SD.Status = 'HIR' THEN 1 ELSE 0 END) as HIR_7D,
            -- MTD calculations
            SUM(CASE WHEN CAST(SD.MaxCreatedDate AS DATE) BETWEEN @StartDateMTD AND @Today AND SD.Status = 'OEX' THEN 1 ELSE 0 END) as OEX_MTD,
            SUM(CASE WHEN CAST(SD.MaxCreatedDate AS DATE) BETWEEN @StartDateMTD AND @Today AND SD.Status = 'HIR' THEN 1 ELSE 0 END) as HIR_MTD,
            -- QTD calculations
            SUM(CASE WHEN CAST(SD.MaxCreatedDate AS DATE) BETWEEN @StartDateQTD AND @Today AND SD.Status = 'OEX' THEN 1 ELSE 0 END) as OEX_QTD,
            SUM(CASE WHEN CAST(SD.MaxCreatedDate AS DATE) BETWEEN @StartDateQTD AND @Today AND SD.Status = 'HIR' THEN 1 ELSE 0 END) as HIR_QTD,
            -- HYTD calculations
            SUM(CASE WHEN CAST(SD.MaxCreatedDate AS DATE) BETWEEN @StartDateHYTD AND @Today AND SD.Status = 'OEX' THEN 1 ELSE 0 END) as OEX_HYTD,
            SUM(CASE WHEN CAST(SD.MaxCreatedDate AS DATE) BETWEEN @StartDateHYTD AND @Today AND SD.Status = 'HIR' THEN 1 ELSE 0 END) as HIR_HYTD,
            -- YTD calculations
            SUM(CASE WHEN CAST(SD.MaxCreatedDate AS DATE) BETWEEN @YearStart AND @Today AND SD.Status = 'OEX' THEN 1 ELSE 0 END) as OEX_YTD,
            SUM(CASE WHEN CAST(SD.MaxCreatedDate AS DATE) BETWEEN @YearStart AND @Today AND SD.Status = 'HIR' THEN 1 ELSE 0 END) as HIR_YTD
        FROM AllUsers U
        LEFT JOIN #SubmissionData SD ON SD.CreatedBy = U.CreatedBy
        GROUP BY U.CreatedBy
    )
    SELECT 
        [User],
        CASE WHEN OEX_7D = 0 THEN 0.00 ELSE CAST(HIR_7D AS DECIMAL(10,2)) / CAST(OEX_7D AS DECIMAL(10,2)) END as LAST7D_RATIO,
        CASE WHEN OEX_MTD = 0 THEN 0.00 ELSE CAST(HIR_MTD AS DECIMAL(10,2)) / CAST(OEX_MTD AS DECIMAL(10,2)) END as MTD_RATIO,
        CASE WHEN OEX_QTD = 0 THEN 0.00 ELSE CAST(HIR_QTD AS DECIMAL(10,2)) / CAST(OEX_QTD AS DECIMAL(10,2)) END as QTD_RATIO,
        CASE WHEN OEX_HYTD = 0 THEN 0.00 ELSE CAST(HIR_HYTD AS DECIMAL(10,2)) / CAST(OEX_HYTD AS DECIMAL(10,2)) END as HYTD_RATIO,
        CASE WHEN OEX_YTD = 0 THEN 0.00 ELSE CAST(HIR_YTD AS DECIMAL(10,2)) / CAST(OEX_YTD AS DECIMAL(10,2)) END as YTD_RATIO
    FROM RatioCalculations
    ORDER BY [User]    FOR JSON PATH;

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
    DROP TABLE #SubmissionData;
    
END

CREATE PROCEDURE [dbo].[DashboardRecruiters]
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
    
    -- Generic Query for Dropdown
    SELECT UserName as KeyValue, FirstName + ' ' + LastName as Text
    FROM dbo.Users 
    WHERE UserName = @User
    FOR JSON AUTO;
    
    -- Common date variables
    DECLARE @Today DATE = CAST(GETDATE() as date);
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
END;

CREATE   PROCEDURE [dbo].[DeleteCandidateDocument] 
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

CREATE   procedure [dbo].[DeleteCandidateEducation]
	@Id int=300,
	@CandidateId int=11600,
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
		
	SELECT @return;

END

CREATE   procedure [dbo].[DeleteCandidateExperience]
	@Id int=0,
	@CandidateId int=19,
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

	SELECT @return;
		
END

CREATE   procedure [dbo].[DeleteCandidateNotes]
	@Id int=0,
	@CandidateId int=11600,
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

	SELECT @return;
		
END

CREATE   procedure [dbo].[DeleteCandidateSkill]
	@Id int=19555,
	@CandidateId int=19,
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
		
	SELECT @return;
		
END

CREATE   PROCEDURE [dbo].[DeleteCompanyDocument] 
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

CREATE   procedure [dbo].[DeleteRequisitionDocuments]
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

CREATE     PROCEDURE [dbo].[DownloadCandidateResume] 
	@CandidateID int = 19, 
	@ResumeType varchar(20) = 'Original'
AS
BEGIN
	SET NOCOUNT ON;

	if (@ResumeType = 'Original')
		BEGIN
			SELECT
				A.[OriginalResume] [DocumentLocation], A.[OriginalFileId] [InternalFileName]
			FROM
				dbo.[Candidate] A
			WHERE
				A.[Id] = @CandidateID
			FOR JSON AUTO, WITHOUT_ARRAY_WRAPPER;
		END
	else
		BEGIN
			SELECT
				A.[FormattedResume] [DocumentLocation], A.[FormattedFileId] [InternalFileName]
			FROM
				dbo.[Candidate] A
			WHERE
				A.[Id] = @CandidateID
			FOR JSON AUTO, WITHOUT_ARRAY_WRAPPER;
		END
END

CREATE   PROCEDURE [dbo].[DuplicateCandidate]
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

CREATE     PROCEDURE [dbo].[GetCandidateDocumentDetails]
	@DocumentID int = 1021
AS
BEGIN
	SET NOCOUNT ON;

	SELECT
		A.[CandidateId] [EntityID], A.[DocumentName], A.[DocumentLocation], A.[InternalFileName]
	FROM
		dbo.[CandidateDocument] A
	WHERE
		A.[CandidateDocId] = CASE WHEN @DocumentID IS NULL OR @DocumentID = 0 THEN -1 ELSE @DocumentID END
	FOR JSON AUTO, WITHOUT_ARRAY_WRAPPER;
END

CREATE   procedure [dbo].[GetCandidateDocuments]
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

create   procedure [dbo].[GetCandidateRequisitionDescription]
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

CREATE PROCEDURE [dbo].[GetCandidates]
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

CREATE procedure [dbo].[GetCandidateSubmission]
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

CREATE PROCEDURE [dbo].[GetCompanies]
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

CREATE   PROCEDURE [dbo].[GetCompaniesList]
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

CREATE   PROCEDURE [dbo].[GetCompanyContactsList]
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

CREATE   procedure [dbo].[GetCompanyDetails]
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

CREATE     procedure [dbo].[GetCompanyDocuments]
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

CREATE procedure [dbo].[GetDesignations]
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

CREATE   procedure [dbo].[GetDetailCandidate]
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
		A.[Phone3Ext] [PhoneExt], A.[LinkedIn], A.[Facebook], A.[Twitter], A.[Title], --8-16
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

CREATE   procedure [dbo].[GetDocumentType]
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

CREATE procedure [dbo].[GetEducation]
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

CREATE procedure [dbo].[GetEligibility]
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

CREATE procedure [dbo].[GetExperience]
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

CREATE procedure [dbo].[GetJobOptions]
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

CREATE procedure [dbo].[GetLeadIndustry]
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

CREATE procedure [dbo].[GetLeadSource]
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

CREATE procedure [dbo].[GetLeadStatus]
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

CREATE   procedure [dbo].[GetLocationList]
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

CREATE   PROCEDURE [dbo].[GetNAICS]
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

CREATE   PROCEDURE [dbo].[GetNotificationEmails]
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

CREATE   procedure [dbo].[GetPreferences]
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

CREATE   PROCEDURE [dbo].[GetRequisitionDetails]
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

CREATE   procedure [dbo].[GetRequisitionDocuments]
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

CREATE   PROCEDURE [dbo].[GetRequisitions]
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

CREATE   PROCEDURE [dbo].[GetRequisitions-Backup]
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

CREATE   procedure [dbo].[GetRequisitionSubmission]
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

CREATE PROCEDURE [dbo].[GetRoles]
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

CREATE procedure [dbo].[GetSkills]
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

CREATE procedure [dbo].[GetStates]
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

CREATE procedure [dbo].[GetStatus]
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

CREATE procedure [dbo].[GetTaxTerms]
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

CREATE   PROCEDURE [dbo].[GetUsers]
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

CREATE    PROCEDURE [dbo].[GetWorkflow] 
AS
BEGIN
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

CREATE   procedure [dbo].[GetZipCityState]
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

CREATE procedure [dbo].[GetZipCodes]
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

CREATE   PROCEDURE [dbo].[InsertRequisitionView]
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

CREATE   PROCEDURE [dbo].[SaveCandidate]
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

CREATE PROCEDURE [dbo].[SaveCandidateActivity]
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

CREATE   procedure [dbo].[SaveCandidateDocuments]
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

CREATE PROCEDURE [dbo].[SaveCandidateWithSubmissions]
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

CREATE PROCEDURE [dbo].[SaveCompany] 
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

CREATE PROCEDURE [dbo].[SaveCompanyContact]
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

CREATE PROCEDURE [dbo].[SaveCompanyDocuments]
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

CREATE PROCEDURE [dbo].[SaveCompanyLocation]
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

CREATE   procedure [dbo].[SaveEducation]
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
		
		SELECT @return;

        COMMIT TRANSACTION;

    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END

CREATE   procedure [dbo].[SaveExperience]
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

		SELECT @return;

        COMMIT TRANSACTION;

    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
		
END

CREATE   procedure [dbo].[SaveNote]
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
	
		SELECT
			A.[Id], A.[UpdatedDate], A.[UpdatedBy], 
			REPLACE(REPLACE(REPLACE(A.[NOTES], CHAR(13) + CHAR(10), '<br/>'), CHAR(13), '<br/>'), CHAR(10), '<br/>') [NOTES]
		FROM
			[dbo].[Notes] A
		WHERE
			A.[EntityId] = @CandidateId
			AND A.[EntityType] = @EntityType
		ORDER BY
			A.[IsPrimary] DESC, A.[UpdatedDate] DESC
		FOR JSON AUTO;

        COMMIT TRANSACTION;

    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END

CREATE PROCEDURE [dbo].[SaveRequisition]
	@RequisitionId int = 1321 OUTPUT,
	@Company int=150,
	@HiringMgr int=77,
	@City varchar(50)='Pittsburgh',
	@StateId int=1,
	@Zip varchar(10)=15212,
	@IsHot tinyint=2,
	@Title varchar(200)='PROCUREMENT OPERATIONS BUSINESS UNIT SUPPORT (ARCONIC ENGINES) ',
	@Description varchar(max)='',
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

CREATE PROCEDURE [dbo].[SaveRequisitionDocuments]
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

CREATE PROCEDURE [dbo].[SaveSkill]
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
		
		SELECT
			A.[Id], B.[Skill], A.[LastUsed], A.[ExpMonth], A.[UpdatedBy]
		FROM
			dbo.[EntitySkills] A INNER JOIN dbo.[Skills] B ON A.[SkillId] = B.[Id]
		WHERE
			A.[EntityId] = @CandidateId
			AND A.[EntityType] = 'CND'
		ORDER BY
			A.[UpdatedDate] DESC
		FOR JSON PATH;

        COMMIT TRANSACTION;

    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
		
END

CREATE PROCEDURE [dbo].[SearchCandidates]
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

CREATE PROCEDURE [dbo].[SearchCompanies]
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

CREATE PROCEDURE [dbo].[SearchRequisitions]
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

CREATE PROCEDURE [dbo].[SetCache]
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

CREATE PROCEDURE [dbo].[SubmitCandidateRequisition]
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

CREATE PROCEDURE [dbo].[UndoCandidateActivity]
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

CREATE   PROCEDURE [dbo].[UpdateCandidateView] AS
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

CREATE PROCEDURE [dbo].[UpdateResume]
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

CREATE PROCEDURE [dbo].[ValidateLogin]
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
