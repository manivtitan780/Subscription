USE Subscription
GO

TRUNCATE TABLE Subscription.dbo.Submissions;

SET IDENTITY_INSERT Submissions ON;

USE [Subscription]
GO

INSERT INTO 
	Subscription.[dbo].[Submissions]
    ([Id],[RequisitionId],[CandidateId],[Status],[Notes],[CreatedBy],[CreatedDate],[UpdatedBy],[UpdatedDate],[StatusId],[ShowCalendar],[DateTime],[Type],
	[PhoneNumber],[InterviewDetails],[Undone])
SELECT 
	[SUBMISSION_ID],[REQUIREMENT_ID],[CANDIDATE_ID],[CURRENT_STATUS],[NOTES],[CREATED_BY],[DATE_CREATED],[UPDATED_BY],[DATE_UPDATED],1,0,NULL,'P',NULL,NULL,0
FROM 
	[TitanPSS].[dbo].[SUBMISSION];
  
GO

SET IDENTITY_INSERT Submissions OFF;

DECLARE @Max int = 1;

SELECT
	@Max = MAX(ID)
FROM
	Subscription.[dbo].[Submissions];

DBCC CHECKIDENT('Subscription.[dbo].[Submissions]', RESEED, @Max);

GO

